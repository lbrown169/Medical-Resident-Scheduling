using MedicalDemo.Converters;
using MedicalDemo.Models.DTO.Requests;
using MedicalDemo.Models.DTO.Responses;
using MedicalDemo.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Controllers;

[ApiController]
[Route("api/rotation-pref-request")]
public class RotationPrefRequestController(MedicalContext context) : ControllerBase
{
    private readonly MedicalContext context = context;

    // GET: api/rotation-pref-requests/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RotationPrefResponse>> GetById([FromRoute] Guid id)
    {
        RotationPrefRequest? request = await IncludeAllRotationPrefRequestProperties(
                context.RotationPrefRequests
            )
            .FirstOrDefaultAsync(r => r.RotationPrefRequestId == id);

        if (request == null)
        {
            return NotFound();
        }

        RotationPrefResponse response =
            RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(request);

        return Ok(response);
    }

    // GET: api/rotation-pref-requests
    [HttpGet()]
    public async Task<ActionResult<RotationPrefRequestsListResponse>> GetAll()
    {
        List<RotationPrefRequest> allRequests = await IncludeAllRotationPrefRequestProperties(context.RotationPrefRequests).ToListAsync();

        List<RotationPrefResponse> prefsResponse =
        [
            .. allRequests.Select(RotationPrefRequestConverter.CreateRotationPrefResponseFromModel),
        ];
        RotationPrefRequestsListResponse finalResponse = new()
        {
            Count = prefsResponse.Count,
            RotationPrefRequests = prefsResponse,
        };

        return Ok(finalResponse);
    }

    // POST: api/rotation-pref-requests
    [HttpPost]
    public async Task<ActionResult<RotationPrefResponse>> Create(
        [FromBody] RotationPrefRequestDto addRequestDto
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate rotation type ID uniqueness
        List<Guid> repeatedGuids = ValidateRotationTypeUniqueness(
            addRequestDto.Priorities,
            addRequestDto.Alternatives,
            addRequestDto.Avoids
        );
        if (repeatedGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Repeated IDs",
                $"ID: [{string.Join(", ", repeatedGuids)}] appear in multiple places."
            );
            return BadRequest(ModelState);
        }

        // Validate rotation type ID existenace
        List<Guid> invalidGuids = await ValidateRotationTypeExistence(
            addRequestDto.Priorities,
            addRequestDto.Alternatives,
            addRequestDto.Avoids
        );
        if (invalidGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Invalid IDs",
                $"ID: [{string.Join(", ", invalidGuids)}] does not exist."
            );
            return BadRequest(ModelState);
        }

        RotationPrefRequest requestModel = RotationPrefRequestConverter.CreateModelFromRequestDto(addRequestDto);

        await context.RotationPrefRequests.AddAsync(requestModel);
        await context.SaveChangesAsync();

        RotationPrefRequest resultModel = await IncludeAllRotationPrefRequestProperties(
                context.RotationPrefRequests
            )
            .FirstAsync(r => r.RotationPrefRequestId == requestModel.RotationPrefRequestId);

        RotationPrefResponse response =
            RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(resultModel);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.RotationPrefRequestId },
            response
        );
    }

    // PUT: api/rotation-pref-requests/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RotationPrefResponse>> UpdateById(
        [FromRoute] Guid id,
        [FromBody] UpdateRotationPrefRequest updateRequest
    )
    {
        // Validate model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate rotation type ID uniqueness
        List<Guid> repeatedGuids = ValidateRotationTypeUniqueness(
            updateRequest.Priorities,
            updateRequest.Alternatives,
            updateRequest.Avoids
        );
        if (repeatedGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Repeated IDs",
                $"ID: [{string.Join(", ", repeatedGuids)}] appear in multiple places."
            );
            return BadRequest(ModelState);
        }

        // Validate rotation type ID existence
        List<Guid> invalidGuids = await ValidateRotationTypeExistence(
            updateRequest.Priorities,
            updateRequest.Alternatives,
            updateRequest.Avoids
        );
        if (invalidGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Invalid IDs",
                $"ID: [{string.Join(", ", invalidGuids)}] does not exist."
            );
            return BadRequest(ModelState);
        }

        RotationPrefRequest? foundPrefRequest =
            await context.RotationPrefRequests.FirstOrDefaultAsync(
                (rt) => rt.RotationPrefRequestId == id
            );

        if (foundPrefRequest == null)
        {
            return NotFound();
        }

        // Update request
        RotationPrefRequestConverter.UpdateModelFromUpdateRequest(foundPrefRequest, updateRequest);

        // Save changes
        await context.SaveChangesAsync();

        RotationPrefRequest foundUpdatedPrefRequest = await IncludeAllRotationPrefRequestProperties(
                context.RotationPrefRequests
            )
            .FirstAsync(r => r.RotationPrefRequestId == id);

        RotationPrefResponse response =
            RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(
                foundUpdatedPrefRequest
            );
        return Ok(response);
    }

    // DELETE: api/rotation-pref-requests/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        RotationPrefRequest? foundPrefRequest =
            await context.RotationPrefRequests.FirstOrDefaultAsync(
                (rt) => rt.RotationPrefRequestId == id
            );
        if (foundPrefRequest == null)
        {
            return NotFound();
        }

        context.RotationPrefRequests.Remove(foundPrefRequest);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<List<Guid>> ValidateRotationTypeExistence(
        List<Guid> priorities,
        List<Guid> alternatives,
        List<Guid> avoids
    )
    {
        List<Guid> allGuids = [];
        allGuids.AddRange(priorities);
        allGuids.AddRange(alternatives);
        allGuids.AddRange(avoids);

        List<Guid> existingGuids = await context
            .RotationTypes.Where((rt) => allGuids.Contains(rt.RotationTypeId))
            .Select((rt) => rt.RotationTypeId)
            .ToListAsync();
        List<Guid> invalidGuids = [.. allGuids.Except(existingGuids)];

        return invalidGuids;
    }

    private static List<Guid> ValidateRotationTypeUniqueness(
        List<Guid> priorities,
        List<Guid> alternatives,
        List<Guid> avoids
    )
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

    private static IQueryable<RotationPrefRequest> IncludeAllRotationPrefRequestProperties(
        DbSet<RotationPrefRequest> rotations
    )
    {
        return rotations
            .Include(r => r.FirstPriority)
            .Include(r => r.SecondPriority)
            .Include(r => r.ThirdPriority)
            .Include(r => r.FourthPriority)
            .Include(r => r.FifthPriority)
            .Include(r => r.SixthPriority)
            .Include(r => r.SeventhPriority)
            .Include(r => r.EighthPriority)
            .Include(r => r.FirstAlternative)
            .Include(r => r.SecondAlternative)
            .Include(r => r.ThirdAlternative)
            .Include(r => r.FirstAvoid)
            .Include(r => r.SecondAvoid)
            .Include(r => r.ThirdAvoid);
    }
}