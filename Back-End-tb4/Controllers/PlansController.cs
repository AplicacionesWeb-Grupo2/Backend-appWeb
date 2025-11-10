using System.Collections.Generic;
using System.Threading.Tasks;
using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Back_End_tb4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly IPlanRepository _planRepository;

    public PlansController(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    // GET: api/plans
    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<Plan>>>> GetAll()
    {
        var plans = await _planRepository.ListAsync();
        return Ok(BaseResponse<IEnumerable<Plan>>.Ok(plans));
    }

    // GET: api/plans/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse<Plan>>> GetById(int id)
    {
        var plan = await _planRepository.FindByIdAsync(id);
        if (plan == null)
            return NotFound(BaseResponse<Plan>.Fail("Plan no encontrado"));

        return Ok(BaseResponse<Plan>.Ok(plan));
    }

    // POST: api/plans
    [HttpPost]
    public async Task<ActionResult<BaseResponse<Plan>>> Create(Plan plan)
    {
        await _planRepository.AddAsync(plan);
        await _planRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = plan.Id },
            BaseResponse<Plan>.Ok(plan, "Plan creado correctamente"));
    }

    // PUT: api/plans/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BaseResponse<Plan>>> Update(int id, Plan plan)
    {
        var existing = await _planRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<Plan>.Fail("Plan no encontrado"));

        existing.Name = plan.Name;
        existing.Description = plan.Description;
        existing.Price = plan.Price;

        _planRepository.Update(existing);
        await _planRepository.SaveChangesAsync();

        return Ok(BaseResponse<Plan>.Ok(existing, "Plan actualizado"));
    }

    // DELETE: api/plans/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BaseResponse<string>>> Delete(int id)
    {
        var existing = await _planRepository.FindByIdAsync(id);
        if (existing == null)
            return NotFound(BaseResponse<string>.Fail("Plan no encontrado"));

        _planRepository.Remove(existing);
        await _planRepository.SaveChangesAsync();

        return Ok(BaseResponse<string>.Ok("Plan eliminado correctamente"));
    }
}
