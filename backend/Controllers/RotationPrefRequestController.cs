using MedicalDemo.Converters;
using MedicalDemo.Enums;
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
        List<RotationPrefRequest> allRequests = await IncludeAllRotationPrefRequestProperties(
                context.RotationPrefRequests
            )
            .ToListAsync();

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

    // Get: api/rotation-pref-requests/resident/{residentId}
    [HttpGet("resident/{residentId}")]
    public async Task<ActionResult<RotationPrefResponse>> GetByResidentId(
        [FromRoute] string residentId
    )
    {
        RotationPrefRequest? request = await IncludeAllRotationPrefRequestProperties(
                context.RotationPrefRequests
            )
            .FirstOrDefaultAsync(r => r.ResidentId == residentId);

        if (request == null)
        {
            return NotFound();
        }

        RotationPrefResponse response =
            RotationPrefRequestConverter.CreateRotationPrefResponseFromModel(request);

        return Ok(response);
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

        List<Guid> prefRotationTypeIds = [];
        prefRotationTypeIds.AddRange(addRequestDto.Priorities);
        prefRotationTypeIds.AddRange(addRequestDto.Alternatives);
        prefRotationTypeIds.AddRange(addRequestDto.Avoids);

        bool isValid = await ValidatePrefRotationTypes(prefRotationTypeIds);
        if (!isValid)
        {
            return BadRequest(ModelState);
        }

        // Check if resident already has already posted a preference request
        bool residentPrefRequestAlreadyExists =
            (
                await context.RotationPrefRequests.FirstOrDefaultAsync(
                    (request) => request.ResidentId == addRequestDto.ResidentId
                )
            ) != null;
        if (residentPrefRequestAlreadyExists)
        {
            ModelState.AddModelError(
                "Resident Request Already Exist",
                $"Resident ID {addRequestDto.ResidentId} has already made a rotation preference request."
            );
            return BadRequest(ModelState);
        }

        Resident? requester = await context.Residents.FirstOrDefaultAsync(
            (resident) => resident.ResidentId == addRequestDto.ResidentId
        );
        // Check if resident exist
        if (requester == null)
        {
            ModelState.AddModelError(
                "Resident Not Found",
                $"Resident ID {addRequestDto.ResidentId} does not exist."
            );
            return BadRequest(ModelState);
        }

        // Check if resident is a PGY3
        if (requester.GraduateYr != 3)
        {
            ModelState.AddModelError(
                "Incorrect Resident PGY",
                $"Resident ID {addRequestDto.ResidentId} is not a PGY 3."
            );
            return BadRequest(ModelState);
        }

        RotationPrefRequest requestModel = RotationPrefRequestConverter.CreateModelFromRequestDto(
            addRequestDto
        );

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

        List<Guid> prefRotationTypeIds = [];
        prefRotationTypeIds.AddRange(updateRequest.Priorities);
        prefRotationTypeIds.AddRange(updateRequest.Alternatives);
        prefRotationTypeIds.AddRange(updateRequest.Avoids);

        bool isValid = await ValidatePrefRotationTypes(prefRotationTypeIds);
        if (!isValid)
        {
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

    private async Task<List<Guid>> ValidateRotationTypeExistence(List<Guid> rotationTypeIds)
    {
        List<Guid> existingGuids = await context
            .RotationTypes.Where((rt) => rotationTypeIds.Contains(rt.RotationTypeId))
            .Select((rt) => rt.RotationTypeId)
            .ToListAsync();
        List<Guid> invalidGuids = [.. rotationTypeIds.Except(existingGuids)];

        return invalidGuids;
    }

    // Check if rotation types are repeated in the preference request
    private static List<Guid> ValidateRotationTypeUniqueness(List<Guid> rotationTypeIds)
    {
        List<Guid> repeatedGuids = [];
        HashSet<Guid> allGuidsSet = [];

        foreach (Guid id in rotationTypeIds)
        {
            if (allGuidsSet.Contains(id))
            {
                repeatedGuids.Add(id);
            }
            allGuidsSet.Add(id);
        }

        return repeatedGuids;
    }

    // Only PGY year 4 rotations can be in the preference
    private static List<Guid> ValidateToOnlyPGY4RotationTypes(List<RotationType> rotationTypes)
    {
        List<Guid> invalidGuids =
        [
            .. rotationTypes
                .Where((rt) => (rt.PgyYearFlags & PgyYearFlags.Pgy4) == 0)
                .Select((rt) => rt.RotationTypeId),
        ];

        return invalidGuids;
    }

    // Only non chief rotation types can be be in the preference
    private static List<Guid> ValidateToNonChiefRotationTypes(List<RotationType> rotationTypes)
    {
        List<Guid> invalidGuids =
        [
            .. rotationTypes
                .Where((rt) => rt.IsChiefRotation == true)
                .Select((rt) => rt.RotationTypeId),
        ];

        return invalidGuids;
    }

    private static IQueryable<RotationPrefRequest> IncludeAllRotationPrefRequestProperties(
        DbSet<RotationPrefRequest> rotationPrefRequestDbSet
    )
    {
        return rotationPrefRequestDbSet
            .Include(r => r.Resident)
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

    // Return false if encountered validation error, true other wise
    private async Task<bool> ValidatePrefRotationTypes(List<Guid> prefRotationTypeIds)
    {
        // Validate rotation type ID uniqueness
        List<Guid> repeatedGuids = ValidateRotationTypeUniqueness(prefRotationTypeIds);
        if (repeatedGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Repeated IDs",
                $"ID: [{string.Join(", ", repeatedGuids)}] appear in multiple places."
            );
            return false;
        }

        // Validate rotation type ID existenace
        List<Guid> nonExistentGuids = await ValidateRotationTypeExistence(prefRotationTypeIds);
        if (nonExistentGuids.Count != 0)
        {
            ModelState.AddModelError(
                "Invalid IDs",
                $"ID: [{string.Join(", ", nonExistentGuids)}] does not exist."
            );
            return false;
        }

        List<RotationType> prefRotationTypes = await context
            .RotationTypes.Where((rt) => prefRotationTypeIds.Contains(rt.RotationTypeId))
            .ToListAsync();

        // Only allow PGY4 rotation types
        List<Guid> nonPgy4RotationTypes = ValidateToOnlyPGY4RotationTypes(prefRotationTypes);
        if (nonPgy4RotationTypes.Count != 0)
        {
            ModelState.AddModelError(
                "Non PGY4 Rotation Types",
                $"ID: [{string.Join(", ", nonPgy4RotationTypes)}] is not a PGY4 Rotation Type."
            );
            return false;
        }

        // Only allow non chief rotation types
        List<Guid> chiefRotationTypes = ValidateToNonChiefRotationTypes(prefRotationTypes);
        if (chiefRotationTypes.Count != 0)
        {
            ModelState.AddModelError(
                "Chief Rotation Types",
                $"ID: [{string.Join(", ", chiefRotationTypes)}] is a chief rotation type and should not be passed."
            );
            return false;
        }

        return true;
    }
}