# Medical Resident Scheduling Algorithm Documentation

## Overview

The PSYCALL medical resident scheduling system uses a sophisticated multi-phase algorithm that combines **maximum flow network theory** with **constraint satisfaction** to generate optimal call schedules for medical residents across different post-graduate years (PGY1, PGY2, PGY3).

## Call Types and Duration

The system defines three distinct call types based on day of the week:

### Call Type Definitions
- **Short Call**: Monday-Friday, 3 hours (4:30pm-8:00pm)
- **12-Hour Call**: Sunday, 12 hours
- **24-Hour Call**: Saturday, 24 hours

### Call Type Determination Logic
```csharp
public static int shiftType(DateTime curDate)
{
    if (curDate.DayOfWeek == DayOfWeek.Saturday) return 24;
    if (curDate.DayOfWeek == DayOfWeek.Sunday) return 12;
    return 3; // Monday-Friday
}
```

## Resident Classifications and Roles

### PGY1 (First-Year Residents)
- **Training Period**: July-August with supervision
- **Independent Period**: September-June
- **Hospital Roles**: Must match rotation requirements for call eligibility
- **Supervision Requirements**: Paired with PGY2/PGY3 during training period

### PGY2 (Second-Year Residents)
- **Primary Role**: Supervise PGY1 residents during training
- **Coverage Responsibilities**: Holiday and weekend coverage
- **Independent Operations**: Throughout academic year

### PGY3 (Third-Year Residents)
- **Supervisory Role**: Train PGY1 residents on short calls
- **Limited Participation**: Primarily involved in training period only

## Hospital Role System

Each resident is assigned monthly hospital roles that determine call eligibility:

### Role Properties
- **DoesShort**: Can work short calls (M-F)
- **DoesLong**: Can work weekend calls (Sat/Sun)
- **FlexShort**: Flexible short call eligibility during training
- **FlexLong**: Flexible long call eligibility during training

### Example Roles
```csharp
Inpatient     => (true,  true,  false, false)  // All calls
NightFloat    => (false, false, false, false)  // No calls
EmergencyMed  => (false, false, false, true)   // Training long calls only
```

## Three-Phase Algorithm Structure

### Phase 1: Training Period (July-August)
**Purpose**: Schedule supervised training for PGY1 residents

#### Short Call Training (PGY1 + PGY3)
- **Requirement**: Each PGY1 must complete 3 supervised short calls
- **Constraint**: PGY1 and PGY3 must be paired for each training day
- **Network Structure**:
  - Source → PGY1 nodes (capacity: 3 each)
  - PGY1 nodes → Training day nodes (capacity: 1 each)
  - Training day nodes → PGY3 nodes (capacity: 1 each)
  - PGY3 nodes → Sink (capacity: ceiling(3×PGY1_count/PGY3_count))

#### Weekend Training (PGY1 + PGY2)
- **Saturday 24h**: Each PGY1 paired with one PGY2
- **Sunday 12h**: Each PGY1 paired with one PGY2
- **Network Structure**: Similar bipartite matching with split nodes

### Phase 2: Independent Schedule Part 1 (September-December)
**Purpose**: Generate regular call schedule for post-training period

#### Key Features:
- **Random Assignment**: Uses probabilistic distribution for initial allocation
- **Constraint Satisfaction**: Ensures no consecutive calls or blackout violations
- **Load Balancing**: Iterative adjustment to maintain 24-hour workload equity
- **Maximum Flow Validation**: Verifies schedule feasibility

### Phase 3: Independent Schedule Part 2 (January-June)
**Purpose**: Complete annual schedule for second half of academic year

#### Builds Upon Previous Phases:
- Inherits committed work days from previous phases
- Maintains continuity in resident workload distribution
- Applies same constraint satisfaction and flow validation

## Maximum Flow Network Algorithm

### Core Components

#### Node Structure
```
Source → [Resident×ShiftType nodes] → [Day nodes] → Sink
```

#### Edge Capacities
- **Source to Residents**: Number of assigned shifts per resident
- **Residents to Days**: 1 (if resident can work that day)
- **Days to Sink**: 1 (each day needs exactly one resident)

#### Flow Validation
```csharp
int flow = graph.getFlow(sourceIndex, sinkIndex);
if (flow != totalDaysToSchedule) {
    // Schedule infeasible - adjust constraints or retry
    return false;
}
```

### Constraint Enforcement

#### Vacation Handling
- Residents cannot be assigned to requested vacation days
- Vacation requests are loaded from approved database entries

#### Role-Based Restrictions
```csharp
// Example: Check if resident can work based on monthly role
int month = (curDay.Month + 5) % 12; // Academic year offset
if (!rolePerMonth[month].DoesLong && curDay.DayOfWeek == DayOfWeek.Saturday) {
    return false; // Cannot work Saturday 24h call
}
```

#### Consecutive Call Prevention
- No back-to-back weekend calls
- No weekend call adjacent to weekday call
- Prevents resident fatigue and ensures safe patient care

## Load Balancing Algorithm

### Initial Random Distribution
```csharp
// Random ratio assignment for each shift type
double ratio = rand.NextDouble();
if (rand.NextDouble() < ratio) {
    assignToPGY1(shift);
} else {
    assignToPGY2(shift);
}
```

### Iterative Rebalancing
1. **Calculate Work Hours**: Sum all assigned shifts per resident
2. **Identify Extremes**: Find highest and lowest workload residents
3. **Shift Transfer**: Move shifts from overloaded to underloaded residents
4. **Constraint Validation**: Ensure transfers maintain all restrictions
5. **Convergence Check**: Repeat until workload difference ≤ 24 hours

### Workload Equity Target
```csharp
int max = Math.Max(pgy1WorkTime.Max(), pgy2WorkTime.Max());
int min = Math.Min(pgy1WorkTime.Min(), pgy2WorkTime.Min());
bool balanced = (max - min <= 24); // Target: within 24 hours
```

## Conflict Resolution

### Weekend Conflict Fixing
When residents are assigned calls they cannot work due to rotation conflicts:

1. **Identify Conflicts**: Check `canWork(date)` vs `isWorking(date)`
2. **Find Swap Partners**: Locate residents who can work the conflicted date
3. **Execute Swaps**: Exchange assignments maintaining shift type matching
4. **Cross-PGY Swaps**: Allow PGY1↔PGY2 swaps when same-level swaps fail

### Retry Mechanism
- **Multiple Attempts**: Up to 10 randomized scheduling attempts
- **Failure Handling**: Return detailed error information for manual intervention
- **Graceful Degradation**: Partial schedule preservation when possible

## Database Integration

### Schedule Generation Flow
1. **Load Resident Data**: Active residents, rotations, approved vacations
2. **Execute Algorithm**: Three-phase scheduling with validation
3. **Clear Previous Schedules**: Remove existing schedule to prevent conflicts
4. **Save New Schedule**: Create schedule record and associated date entries
5. **Calculate Hours**: Compute total and bi-yearly hour summaries

### Data Persistence
```csharp
// Convert algorithm output to database entities
var dateEntities = dateDTOs.Select(dto => new Dates {
    DateId = dto.DateId,
    ScheduleId = dto.ScheduleId,
    ResidentId = dto.ResidentId,
    Date = dto.Date,
    CallType = dto.CallType
}).ToList();
```

## Algorithm Complexity and Performance

### Time Complexity
- **Maximum Flow**: O(V²E) per phase using Ford-Fulkerson
- **Load Balancing**: O(R×S×I) where R=residents, S=shifts, I=iterations
- **Overall**: O(D×R²) where D=days in year, R=residents

### Space Complexity
- **Graph Storage**: O(V+E) for flow network
- **Resident State**: O(R×D) for work day tracking
- **Overall**: O(R×D) for annual schedule

### Optimization Strategies
- **Early Termination**: Stop when workload balanced within threshold
- **Constraint Pruning**: Remove infeasible edges before flow computation
- **Randomization**: Multiple starting points to avoid local optima

## Error Handling and Validation

### Common Failure Modes
1. **Insufficient Residents**: Not enough available residents for required calls
2. **Over-constrained Schedules**: Vacation/rotation conflicts prevent valid assignment
3. **Workload Imbalance**: Cannot achieve equitable distribution within constraints

### Validation Checks
- **Flow Feasibility**: Verify maximum flow equals required coverage
- **Constraint Compliance**: Ensure no resident works during blackout periods
- **Workload Equity**: Confirm hour distribution within acceptable range
- **Continuity**: Validate no gaps in call coverage

This algorithm represents a sophisticated approach to medical resident scheduling that balances operational requirements, educational needs, and resident welfare through advanced computational techniques.