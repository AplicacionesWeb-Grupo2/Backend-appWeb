using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_tb4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepository;

    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
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
    public async Task<ActionResult<BaseResponse<User>>> GetById(int id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        user.Password = null;
        return Ok(BaseResponse<User>.Ok(user));
    }

    [HttpPost]
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
    public async Task<ActionResult<BaseResponse<User>>> Login([FromBody] LoginRequest request)
    {
        // Buscar usuario por email y password
        var users = await _userRepository.ListAsync();
        var user = users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            
        if (user == null)
            return Unauthorized(BaseResponse<User>.Fail("Credenciales inválidas"));

        // No devolver la contraseña
        user.Password = null;
            
        return Ok(BaseResponse<User>.Ok(user, "Login exitoso"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<User>>> Update(int id, User user)
    {
        var existing = await _userRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        // Actualizar solo campos permitidos
        existing.Nombre = user.Nombre;
        existing.Email = user.Email;
        // Solo actualizar contraseña si se proporciona una nueva
        if (!string.IsNullOrEmpty(user.Password))
            existing.Password = user.Password;
        existing.Tipo = user.Tipo;

        _userRepository.Update(existing);
        await _userRepository.SaveChangesAsync();

        existing.Password = null;
        return Ok(BaseResponse<User>.Ok(existing, "Usuario actualizado"));
    }

    [HttpDelete("{id:int}")]
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