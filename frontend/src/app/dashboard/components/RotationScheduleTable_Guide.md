# RotationScheduleTable Usage

schedule: your schedule data, residents with their rotations
colorMap: maps rotation name to hex color like { "EmergencyMed": "#ef4444" }
displayNames: optional short names for the table like { "EmergencyMed": "EM" }
rotationTypes: { id, name }[] of allowed rotation types. get from GET /api/rotation-types?pgyYear=<year>
readOnly: set to true to just show colored labels instead of dropdowns (no clicking)
allowResidentReassignment: set to true to turn on resident reassignment dropdown
residentList (we dont use this, change it however you need): { id, name }[] of all residents
onRotationChange(residentId, monthIndex, newRotationTypeId): fires with the rotation type ID when a rotation is changed
onResidentChange(rotationId, newResidentId) we dont use this. change it however you need. it fires when a resident is reassigned

months are hardcoded July-June. can make this a prop if you need
colorMap keys need to exactly match the rotation names from your API or they fall back to gray.
onRotationChange gives you the rotation type ID not the name.

you can also look at PGY4RotationPage.tsx in on the G23-113 branch as a reference for how we use it.
