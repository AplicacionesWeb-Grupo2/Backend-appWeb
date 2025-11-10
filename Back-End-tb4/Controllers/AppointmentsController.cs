using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Back_End_tb4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IRepository<Appointment> _appointmentRepository;

    public AppointmentsController(IRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<Appointment>>>> GetAll()
    {
        var items = await _appointmentRepository.ListAsync();
        return Ok(BaseResponse<IEnumerable<Appointment>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<Appointment>>> GetById(int id)
    {
        var appointment = await _appointmentRepository.FindByIdAsync(id);
        if (appointment == null)
            return NotFound(BaseResponse<Appointment>.Fail("Cita no encontrada"));

        return Ok(BaseResponse<Appointment>.Ok(appointment));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<Appointment>>> Create(Appointment appointment)
    {
        await _appointmentRepository.AddAsync(appointment);
        await _appointmentRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = appointment.Id },
            BaseResponse<Appointment>.Ok(appointment, "Cita creada"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<Appointment>>> Update(int id, Appointment appointment)
    {
        var existing = await _appointmentRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<Appointment>.Fail("Cita no encontrada"));

        existing.UserId        = appointment.UserId;
        existing.PsychologistId = appointment.PsychologistId;
        existing.DateTime      = appointment.DateTime;
        existing.Status        = appointment.Status;

        _appointmentRepository.Update(existing);
        await _appointmentRepository.SaveChangesAsync();

        return Ok(BaseResponse<Appointment>.Ok(existing, "Cita actualizada"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BaseResponse<string>>> Delete(int id)
    {
        var existing = await _appointmentRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<string>.Fail("Cita no encontrada"));

        _appointmentRepository.Remove(existing);
        await _appointmentRepository.SaveChangesAsync();

        return Ok(BaseResponse<string>.Ok("Cita eliminada correctamente"));
    }
}
