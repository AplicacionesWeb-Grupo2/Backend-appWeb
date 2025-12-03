using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Services;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_tb4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly JwtService _jwtService;

    public UsersController(IRepository<User> userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpGet]
    [Authorize] // Requiere autenticación
    public async Task<ActionResult<BaseResponse<IEnumerable<User>>>> GetAll()
    {
        var users = await _userRepository.ListAsync();
        // Ocultar contraseñas en la respuesta
        foreach (var user in users)
        {
            user.Password = null;
        }
        return Ok(BaseResponse<IEnumerable<User>>.Ok(users));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<BaseResponse<User>>> GetById(int id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        user.Password = null;
        return Ok(BaseResponse<User>.Ok(user));
    }

    [HttpPost]
    [AllowAnonymous] // Permitir crear usuarios sin autenticación
    public async Task<ActionResult<BaseResponse<User>>> Create(User user)
    {
        // Asignar valores por defecto
        user.Tipo = user.Tipo ?? "paciente";
        user.CreatedAt = DateTime.UtcNow;

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // No devolver la contraseña
        user.Password = null;
            
        return CreatedAtAction(nameof(GetById),
            new { id = user.Id },
            BaseResponse<User>.Ok(user, "Usuario creado"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        // Buscar usuario por email y password
        var users = await _userRepository.ListAsync();
        var user = users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            
        if (user == null)
            return Unauthorized(BaseResponse<LoginResponse>.Fail("Credenciales inválidas"));

        // Generar token JWT
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Tipo);

        // Crear respuesta con token
        var response = new LoginResponse
        {
            Id = user.Id,
            Email = user.Email,
            Nombre = user.Nombre,
            Tipo = user.Tipo,
            Token = token,
            TokenExpiration = DateTime.UtcNow.AddMinutes(120) // 2 horas
        };

        return Ok(BaseResponse<LoginResponse>.Ok(response, "Login exitoso"));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<BaseResponse<User>>> Update(int id, User user)
    {
        var existing = await _userRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        // Actualizar solo campos permitidos
        existing.Nombre = user.Nombre;
        existing.Email = user.Email;
        if (!string.IsNullOrEmpty(user.Password))
            existing.Password = user.Password;
        existing.Tipo = user.Tipo;

        _userRepository.Update(existing);
        await _userRepository.SaveChangesAsync();

        existing.Password = null;
        return Ok(BaseResponse<User>.Ok(existing, "Usuario actualizado"));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult<BaseResponse<string>>> Delete(int id)
    {
        var existing = await _userRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<string>.Fail("Usuario no encontrado"));

        _userRepository.Remove(existing);
        await _userRepository.SaveChangesAsync();

        return Ok(BaseResponse<string>.Ok("Usuario eliminado correctamente"));
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }
}