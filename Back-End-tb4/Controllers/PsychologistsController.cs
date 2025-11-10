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
        return Ok(BaseResponse<IEnumerable<Psychologist>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<Psychologist>>> GetById(int id)
    {
        var item = await _psychologistRepository.FindByIdAsync(id);
        if (item == null)
            return NotFound(BaseResponse<Psychologist>.Fail("Psicólogo no encontrado"));

        return Ok(BaseResponse<Psychologist>.Ok(item));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<Psychologist>>> Create(Psychologist psychologist)
    {
        await _psychologistRepository.AddAsync(psychologist);
        await _psychologistRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = psychologist.Id },
            BaseResponse<Psychologist>.Ok(psychologist, "Psicólogo creado"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<Psychologist>>> Update(int id, Psychologist psychologist)
    {
        var existing = await _psychologistRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<Psychologist>.Fail("Psicólogo no encontrado"));

        existing.FullName    = psychologist.FullName;
        existing.Specialty   = psychologist.Specialty;
        existing.Description = psychologist.Description;
        existing.Rating      = psychologist.Rating;

        _psychologistRepository.Update(existing);
        await _psychologistRepository.SaveChangesAsync();

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
