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
        // Asignar valores por defecto
        appointment.Status = appointment.Status ?? "confirmed";
        appointment.CreatedAt = DateTime.UtcNow;

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

        // Actualizar campos
        existing.PsychologistId = appointment.PsychologistId;
        existing.PsychologistName = appointment.PsychologistName;
        existing.PatientId = appointment.PatientId;
        existing.PatientName = appointment.PatientName;
        existing.Date = appointment.Date;
        existing.Time = appointment.Time;
        existing.Notes = appointment.Notes;
        existing.Status = appointment.Status;

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

    // Nuevo endpoint: Obtener citas por paciente
    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<Appointment>>>> GetByPatient(int patientId)
    {
        var appointments = await _appointmentRepository.ListAsync();
        var patientAppointments = appointments.Where(a => a.PatientId == patientId).ToList();
            
        return Ok(BaseResponse<IEnumerable<Appointment>>.Ok(patientAppointments));
    }

    // Nuevo endpoint: Obtener citas por psicólogo
    [HttpGet("psychologist/{psychologistId:int}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<Appointment>>>> GetByPsychologist(int psychologistId)
    {
        var appointments = await _appointmentRepository.ListAsync();
        var psychologistAppointments = appointments.Where(a => a.PsychologistId == psychologistId).ToList();
            
        return Ok(BaseResponse<IEnumerable<Appointment>>.Ok(psychologistAppointments));
    }
}