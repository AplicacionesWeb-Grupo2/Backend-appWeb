using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

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
        return Ok(BaseResponse<IEnumerable<User>>.Ok(users));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<User>>> GetById(int id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        return Ok(BaseResponse<User>.Ok(user));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<User>>> Create(User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = user.Id },
            BaseResponse<User>.Ok(user, "Usuario creado"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<User>>> Update(int id, User user)
    {
        var existing = await _userRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<User>.Fail("Usuario no encontrado"));

        existing.FullName     = user.FullName;
        existing.Email        = user.Email;
        existing.PasswordHash = user.PasswordHash;
        existing.Role         = user.Role;

        _userRepository.Update(existing);
        await _userRepository.SaveChangesAsync();

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
