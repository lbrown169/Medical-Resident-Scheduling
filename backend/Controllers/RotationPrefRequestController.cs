
using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/rotation-pref-request")]
public class RotationPrefRequestController(
    MedicalContext context
    ) : ControllerBase
{
    private readonly MedicalContext context = context;

    // GET: api/rotation-pref-requests/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RotationPrefResponse>> GetById([FromRoute] Guid id)
    {
        RotationPrefRequest? request = await context.RotationPrefRequests
            .Include(r => r.FirstPriority)
            .Include(r => r.SecondPriority)
            .Include(r => r.ThirdPriority)
            .Include(r => r.FourthPriority)
            .Include(r => r.SixthPriority)
            .Include(r => r.SeventhPriority)
            .Include(r => r.EighthPriority)
            .Include(r => r.FirstAlternative)
            .Include(r => r.SecondAlternative)
            .Include(r => r.ThirdAlternative)
            .Include(r => r.FirstAvoid)
            .Include(r => r.SecondAvoid)
            .Include(r => r.ThirdAvoid)
            .FirstOrDefaultAsync(r => r.RotationPrefRequestId == id);

        if (request == null)
        {
            return NotFound();
        }

        RotationPrefResponse response = RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(request);

        return Ok(response);
    }

    // GET: api/rotation-pref-requests
    [HttpGet()]
    public async Task<ActionResult<RotationPrefRequestsListResponse>> GetAll()
    {
        List<RotationPrefRequest> allRequests = [.. context.RotationPrefRequests.Include(r => r.FirstPriority)
            .Include(r => r.SecondPriority)
            .Include(r => r.ThirdPriority)
            .Include(r => r.FourthPriority)
            .Include(r => r.SixthPriority)
            .Include(r => r.SeventhPriority)
            .Include(r => r.EighthPriority)
            .Include(r => r.FirstAlternative)
            .Include(r => r.SecondAlternative)
            .Include(r => r.ThirdAlternative)
            .Include(r => r.FirstAvoid)
            .Include(r => r.SecondAvoid)
            .Include(r => r.ThirdAvoid)];

        List<RotationPrefResponse> prefsResponse = [.. allRequests.Select(RotationPrefRequestConverter.CreateRotationPrefResponseFromModel)];
        RotationPrefRequestsListResponse finalResponse = new()
        {
            Count = prefsResponse.Count,
            RotationPrefRequests = prefsResponse
        };

        return Ok(finalResponse);
    }

    // POST: api/rotation-pref-requests
    [HttpPost]
    public async Task<ActionResult<RotationPrefResponse>> Create([FromBody] RotationPrefRequestDto addRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate rotation type ID uniqueness
        List<Guid> repeatedGuids = ValidateRotationTypeUniqueness(addRequestDto.Priorities, addRequestDto.Alternatives, addRequestDto.Avoids);
        if (repeatedGuids.Count != 0)
        {
            ModelState.AddModelError("Repeated IDs", $"ID: [{string.Join(", ", repeatedGuids)}] appear in multiple places.");
            return BadRequest(ModelState);
        }

        // Validate rotation type ID existenace
        List<Guid> invalidGuids = await ValidateRotationTypeExistence(addRequestDto.Priorities, addRequestDto.Alternatives, addRequestDto.Avoids);
        if (invalidGuids.Count != 0)
        {
            ModelState.AddModelError("Invalid IDs", $"ID: [{string.Join(", ", invalidGuids)}] does not exist.");
            return BadRequest(ModelState);
        }

        int prioritiesCount = addRequestDto.Priorities.Count;
        int alternativesCount = addRequestDto.Alternatives.Count;
        int avoidsCount = addRequestDto.Avoids.Count;

        RotationPrefRequest requestModel = new()
        {
            RotationPrefRequestId = Guid.NewGuid(),
            ResidentId = addRequestDto.ResidentId,

            FirstPriorityId = addRequestDto.Priorities[0],
            SecondPriorityId = addRequestDto.Priorities[1],
            ThirdPriorityId = addRequestDto.Priorities[2],
            FourthPriorityId = addRequestDto.Priorities[3],
            FifthPriorityId = prioritiesCount > 4 ? addRequestDto.Priorities[4] : null,
            SixthPriorityId = prioritiesCount > 5 ? addRequestDto.Priorities[5] : null,
            SeventhPriorityId = prioritiesCount > 6 ? addRequestDto.Priorities[6] : null,
            EighthPriorityId = prioritiesCount > 7 ? addRequestDto.Priorities[7] : null,

            FirstAlternativeId = alternativesCount > 0 ? addRequestDto.Alternatives[0] : null,
            SecondAlternativeId = alternativesCount > 1 ? addRequestDto.Alternatives[1] : null,
            ThirdAlternativeId = alternativesCount > 2 ? addRequestDto.Alternatives[2] : null,

            FirstAvoidId = avoidsCount > 0 ? addRequestDto.Avoids[0] : null,
            SecondAvoidId = avoidsCount > 1 ? addRequestDto.Avoids[1] : null,
            ThirdAvoidId = avoidsCount > 2 ? addRequestDto.Avoids[2] : null,

            AdditionalNotes = addRequestDto.AdditionalNotes
        };

        await context.RotationPrefRequests.AddAsync(requestModel);
        await context.SaveChangesAsync();

        RotationPrefRequest resultModel = await context.RotationPrefRequests
            .Include(r => r.FirstPriority)
            .Include(r => r.SecondPriority)
            .Include(r => r.ThirdPriority)
            .Include(r => r.FourthPriority)
            .Include(r => r.SixthPriority)
            .Include(r => r.SeventhPriority)
            .Include(r => r.EighthPriority)
            .Include(r => r.FirstAlternative)
            .Include(r => r.SecondAlternative)
            .Include(r => r.ThirdAlternative)
            .Include(r => r.FirstAvoid)
            .Include(r => r.SecondAvoid)
            .Include(r => r.ThirdAvoid)
            .FirstAsync(r => r.RotationPrefRequestId == requestModel.RotationPrefRequestId);

        RotationPrefResponse response = RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(resultModel);

        return CreatedAtAction(nameof(GetById), new { id = response.RotationPrefRequestId }, response);
    }

    // PUT: api/rotation-pref-requests/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RotationPrefResponse>> UpdateById([FromRoute] Guid id, [FromBody] UpdateRotationPrefRequest updateRequest)
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate rotation type ID uniqueness
        List<Guid> repeatedGuids = ValidateRotationTypeUniqueness(updateRequest.Priorities, updateRequest.Alternatives, updateRequest.Avoids);
        if (repeatedGuids.Count != 0)
        {
            ModelState.AddModelError("Repeated IDs", $"ID: [{string.Join(", ", repeatedGuids)}] appear in multiple places.");
            return BadRequest(ModelState);
        }

        // Validate rotation type ID existence
        List<Guid> invalidGuids = await ValidateRotationTypeExistence(updateRequest.Priorities, updateRequest.Alternatives, updateRequest.Avoids);
        if (invalidGuids.Count != 0)
        {
            ModelState.AddModelError("Invalid IDs", $"ID: [{string.Join(", ", invalidGuids)}] does not exist.");
            return BadRequest(ModelState);
        }

        RotationPrefRequest? foundPrefRequest = await context.RotationPrefRequests.FirstOrDefaultAsync((rt) => rt.RotationPrefRequestId == id);

        if (foundPrefRequest == null)
        {
            return NotFound();
        }

        // Update request
        int prioritiesCount = updateRequest.Priorities.Count;
        int alternativesCount = updateRequest.Alternatives.Count;
        int avoidsCount = updateRequest.Avoids.Count;

        Console.WriteLine(avoidsCount);

        foundPrefRequest.FirstPriorityId = updateRequest.Priorities[0];
        foundPrefRequest.SecondPriorityId = updateRequest.Priorities[1];
        foundPrefRequest.ThirdPriorityId = updateRequest.Priorities[2];
        foundPrefRequest.FourthPriorityId = updateRequest.Priorities[3];
        foundPrefRequest.FifthPriorityId = prioritiesCount > 4 ? updateRequest.Priorities[4] : null;
        foundPrefRequest.SixthPriorityId = prioritiesCount > 5 ? updateRequest.Priorities[5] : null;
        foundPrefRequest.SeventhPriorityId = prioritiesCount > 6 ? updateRequest.Priorities[6] : null;
        foundPrefRequest.EighthPriorityId = prioritiesCount > 7 ? updateRequest.Priorities[7] : null;

        foundPrefRequest.FirstAlternativeId = alternativesCount > 0 ? updateRequest.Alternatives[0] : null;
        foundPrefRequest.SecondAlternativeId = alternativesCount > 1 ? updateRequest.Alternatives[1] : null;
        foundPrefRequest.ThirdAlternativeId = alternativesCount > 2 ? updateRequest.Alternatives[2] : null;

        foundPrefRequest.FirstAvoidId = avoidsCount > 0 ? updateRequest.Avoids[0] : null;
        foundPrefRequest.SecondAvoidId = avoidsCount > 1 ? updateRequest.Avoids[1] : null;
        foundPrefRequest.ThirdAvoidId = avoidsCount > 2 ? updateRequest.Avoids[2] : null;

        foundPrefRequest.AdditionalNotes = updateRequest.AdditionalNotes;

        // Save changes
        await context.SaveChangesAsync();

        RotationPrefRequest foundUpdatedPrefRequest = await context.RotationPrefRequests
           .Include(r => r.FirstPriority)
           .Include(r => r.SecondPriority)
           .Include(r => r.ThirdPriority)
           .Include(r => r.FourthPriority)
           .Include(r => r.SixthPriority)
           .Include(r => r.SeventhPriority)
           .Include(r => r.EighthPriority)
           .Include(r => r.FirstAlternative)
           .Include(r => r.SecondAlternative)
           .Include(r => r.ThirdAlternative)
           .Include(r => r.FirstAvoid)
           .Include(r => r.SecondAvoid)
           .Include(r => r.ThirdAvoid)
           .FirstAsync(r => r.RotationPrefRequestId == id);

        RotationPrefResponse response = RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(foundUpdatedPrefRequest);
        return Ok(response);
    }

    // DELETE: api/rotation-pref-requests/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        RotationPrefRequest? foundPrefRequest = await context.RotationPrefRequests.FirstOrDefaultAsync((rt) => rt.RotationPrefRequestId == id);
        if (foundPrefRequest == null)
        {
            return NotFound();
        }

        context.RotationPrefRequests.Remove(foundPrefRequest);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<List<Guid>> ValidateRotationTypeExistence(List<Guid> priorities, List<Guid> alternatives, List<Guid> avoids)
    {
        List<Guid> allGuids = [];
        allGuids.AddRange(priorities);
        allGuids.AddRange(alternatives);
        allGuids.AddRange(avoids);

        List<Guid> existingGuids = await context.RotationTypes.Where((rt) => allGuids.Contains(rt.RotationTypeId)).Select((rt) => rt.RotationTypeId).ToListAsync();
        List<Guid> invalidGuids = [.. allGuids.Except(existingGuids)];

        return invalidGuids;
    }

    private static List<Guid> ValidateRotationTypeUniqueness(List<Guid> priorities, List<Guid> alternatives, List<Guid> avoids)
    {
        List<Guid> repeatedGuids = [];
        HashSet<Guid> allGuidsSet = [];

        List<Guid> allGuids = [];
        allGuids.AddRange(priorities);
        allGuids.AddRange(alternatives);
        allGuids.AddRange(avoids);

        foreach (Guid id in allGuids)
        {
            if (allGuidsSet.Contains(id))
            {
                repeatedGuids.Add(id);
            }
            allGuidsSet.Add(id);
        }

        return repeatedGuids;
    }
}