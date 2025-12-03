using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Back_End_tb4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PsychologistsController : ControllerBase
{
    private readonly IRepository<Psychologist> _psychologistRepository;

    public PsychologistsController(IRepository<Psychologist> psychologistRepository)
    {
        _psychologistRepository = psychologistRepository;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<Psychologist>>>> GetAll()
    {
        var items = await _psychologistRepository.ListAsync();
        // Ocultar contraseñas
        foreach (var psychologist in items)
        {
            psychologist.Password = null;
        }
        return Ok(BaseResponse<IEnumerable<Psychologist>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<Psychologist>>> GetById(int id)
    {
        var item = await _psychologistRepository.FindByIdAsync(id);
        if (item == null)
            return NotFound(BaseResponse<Psychologist>.Fail("Psicólogo no encontrado"));

        item.Password = null;
        return Ok(BaseResponse<Psychologist>.Ok(item));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<Psychologist>>> Create(Psychologist psychologist)
    {
        // Asignar valores por defecto
        psychologist.Tipo = "psicologo";
        psychologist.Calificacion = psychologist.Calificacion == 0 ? 5 : psychologist.Calificacion;

        await _psychologistRepository.AddAsync(psychologist);
        await _psychologistRepository.SaveChangesAsync();

        psychologist.Password = null;
        return CreatedAtAction(nameof(GetById),
            new { id = psychologist.Id },
            BaseResponse<Psychologist>.Ok(psychologist, "Psicólogo creado"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse<Psychologist>>> Login([FromBody] PsychologistLoginRequest request)
    {
        var psychologists = await _psychologistRepository.ListAsync();
        var psychologist = psychologists.FirstOrDefault(p => p.Email == request.Email && p.Password == request.Password);
            
        if (psychologist == null)
            return Unauthorized(BaseResponse<Psychologist>.Fail("Credenciales inválidas"));

        psychologist.Password = null;
        return Ok(BaseResponse<Psychologist>.Ok(psychologist, "Login exitoso"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<Psychologist>>> Update(int id, Psychologist psychologist)
    {
        var existing = await _psychologistRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<Psychologist>.Fail("Psicólogo no encontrado"));

        // Actualizar campos
        existing.Nombre = psychologist.Nombre;
        existing.Email = psychologist.Email;
        if (!string.IsNullOrEmpty(psychologist.Password))
            existing.Password = psychologist.Password;
        existing.Especialidad = psychologist.Especialidad;
        existing.Calificacion = psychologist.Calificacion;
        existing.Imagen = psychologist.Imagen;
        existing.Descripcion = psychologist.Descripcion;
        existing.Biografia = psychologist.Biografia;
        existing.Educacion = psychologist.Educacion;
        existing.Certificaciones = psychologist.Certificaciones;
        existing.Idiomas = psychologist.Idiomas;
        existing.Metodologias = psychologist.Metodologias;
        existing.AnosExperiencia = psychologist.AnosExperiencia;
        existing.AtendeEdades = psychologist.AtendeEdades;
        existing.Horarios = psychologist.Horarios;

        _psychologistRepository.Update(existing);
        await _psychologistRepository.SaveChangesAsync();

        existing.Password = null;
        return Ok(BaseResponse<Psychologist>.Ok(existing, "Psicólogo actualizado"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BaseResponse<string>>> Delete(int id)
    {
        var existing = await _psychologistRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<string>.Fail("Psicólogo no encontrado"));

        _psychologistRepository.Remove(existing);
        await _psychologistRepository.SaveChangesAsync();

        return Ok(BaseResponse<string>.Ok("Psicólogo eliminado correctamente"));
    }
}

public class PsychologistLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}