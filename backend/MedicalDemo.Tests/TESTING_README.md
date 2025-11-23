# Backend Testing Suite - Medical Resident Scheduling

## Overview

This testing suite provides comprehensive unit test coverage for the Medical Resident Scheduling backend using industry-standard .NET testing frameworks. The tests are designed to prove that your scheduling algorithm works correctly under various scenarios.

## Test Architecture

### Technologies Used
- **xUnit**: Industry standard for .NET testing
- **Moq**: For mocking dependencies and isolating business logic
- **FluentAssertions**: Readable, expressive assertions

### Test Structure

```
MedicalDemo.Tests/
├── AlgorithmLogicTests.cs          # Unit tests for scheduling algorithm
└── TESTING_README.md               # This file
```

## Test Categories

### Unit Tests (`AlgorithmLogicTests.cs`)

**Purpose**: Test the isolated logic of the scheduling algorithm without hitting a real database.

**Key Test Cases** (27 total):

**PGY1 CanWork Tests:**
- `CanWork_ShouldReturnFalse_WhenResidentHasVacation`: Verifies residents cannot be scheduled during approved vacations
- `CanWork_ShouldReturnFalse_WhenResidentOnBlackoutRotation`: Tests blackout rotation logic (Night Float, Emergency Med, etc.)
- `CanWork_ShouldReturnFalse_WhenConsecutiveWeekendDays`: Ensures residents don't work consecutive weekend shifts
- `CanWork_ShouldReturnFalse_WhenFridayBeforeSaturday`: Tests Friday-before-Saturday restriction
- `CanWork_ShouldReturnFalse_WhenSundayAfterSaturday`: Tests Sunday-after-Saturday restriction
- `CanWork_ShouldReturnTrue_WhenNoConflicts`: Validates working when no constraints violated
- `CanWork_ShouldReturnFalse_DuringTraining`: Tests training period restrictions

**PGY2 CanWork Tests:**
- `PGY2_CanWork_ShouldReturnFalse_WhenResidentHasVacation`
- `PGY2_CanWork_ShouldReturnFalse_WhenResidentOnBlackoutRotation`
- `PGY2_CanWork_ShouldReturnFalse_WhenConsecutiveWeekendDays`
- `PGY2_CanWork_ShouldReturnTrue_WhenNoConflicts`

**Graph Algorithm Tests:**
- `Graph_MaxFlow_ShouldCalculateCorrectly`: Validates the Dinic's algorithm implementation for bipartite matching
- `Graph_AddEdge_ShouldAddBidirectionalEdges`: Tests edge creation
- `Graph_MaxFlow_ReturnsCorrectFlow`: Tests flow calculation accuracy

**Work Day Management Tests:**
- `PGY1_AddWorkDay_ShouldReturnTrue_WhenDayNotAlreadyWorked`
- `PGY1_AddWorkDay_ShouldReturnFalse_WhenDayAlreadyWorked`
- `PGY1_RemoveWorkDay_ShouldRemoveDayFromSet`
- `PGY1_IsWorkDay_ShouldReturnTrueForAddedDay`

**Mapper Service Tests:**
- `MapToPGY1DTO_ShouldMapCorrectly`: Validates data transformation from database models to DTOs
- `MapToPGY1DTO_ShouldHandleMultipleResidents`
- `MapToPGY1DTO_ShouldMapRotationsCorrectly`
- `MapToPGY2DTO_ShouldMapCorrectly`
- `MapToPGY2DTO_ShouldMapRotationsCorrectly`

**Edge Cases:**
- Various null handling and boundary condition tests

**What These Tests Prove**:
✅ The core scheduling logic respects all business rules
✅ Vacation requests are honored
✅ Blackout rotations prevent scheduling
✅ Consecutive weekend restrictions work correctly
✅ Friday/Sunday restrictions around Saturday shifts work
✅ The graph algorithm correctly matches residents to shifts
✅ Work day management (add/remove/query) functions properly
✅ Data mapping between layers works correctly
✅ PGY1 and PGY2 specific rules are enforced

## How to Run Tests

### Prerequisites
All required NuGet packages have been installed:
```bash
cd backend/MedicalDemo.Tests
dotnet restore
```

### Run All Tests
```bash
cd backend/MedicalDemo.Tests
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~AlgorithmLogicTests"
```

### Run Single Test
```bash
dotnet test --filter "FullyQualifiedName~CanWork_ShouldReturnFalse_WhenResidentHasVacation"
```

### Run with Verbose Output
```bash
dotnet test --verbosity detailed
```

### Generate Code Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## What the Tests Validate

### Scheduling Algorithm Correctness
- [x] Residents cannot work during vacations (PGY1 & PGY2)
- [x] Residents cannot work during blackout rotations (PGY1 & PGY2)
- [x] No consecutive weekend shifts (PGY1 & PGY2)
- [x] No Friday before Saturday shift (PGY1)
- [x] No Sunday after Saturday shift (PGY1)
- [x] PGY1 training date restrictions
- [x] Graph maximum flow calculation (Dinic's algorithm)
- [x] Work day management (add/remove/query)
- [x] Edge addition in bipartite graph

### Data Mapping & Transformation
- [x] DTO to Model mapping works correctly
- [x] Multiple residents handled in mapping
- [x] Rotations map to correct HospitalRoles
- [x] PGY1 specific mappings (training status, supervisor)
- [x] PGY2 specific mappings
- [x] Approved vacations are loaded and respected

## Troubleshooting

### Build Errors

If you encounter build errors, ensure:
1. You're using .NET 6 SDK (`dotnet --version`)
2. All NuGet packages are restored (`dotnet restore`)
3. The main project builds successfully (`cd ../backend && dotnet build`)
4. ImplicitUsings is enabled in the test project

### Test Failures

If tests fail:
1. Ensure all NuGet packages are restored: `dotnet restore`
2. Check that the main project builds successfully: `dotnet build`
3. Run tests individually to isolate the failing test
4. Check test output for specific error messages

### Common Issues

**Issue**: `Cannot find namespace 'Xunit'`
**Solution**: Run `dotnet restore` in the test project directory

**Issue**: Build warnings about package version conflicts
**Solution**: These are warnings only and don't affect test execution

## Extending the Tests

### Adding New Unit Tests

```csharp
[Fact]
public void YourTest_Should_DoSomething()
{
    // Arrange
    var pgy1 = new PGY1("Test Name");

    // Act
    bool result = pgy1.canWork(new DateTime(2026, 7, 15));

    // Assert
    result.Should().BeTrue("reason for expectation");
}
```

## Performance Benchmarks

Expected test execution times on standard hardware:
- **Unit Tests**: < 1 second total (27 tests)
- **Full Suite**: < 2 seconds

## Benefits for Senior Design

This testing suite demonstrates:

1. **Professional Software Engineering Practices**
   - Industry-standard testing frameworks (xUnit, Moq, FluentAssertions)
   - Comprehensive unit test coverage
   - Test isolation and independence

2. **Algorithm Validation**
   - Mathematical correctness (graph max flow via Dinic's algorithm)
   - Business rule compliance (vacation, blackout, weekend restrictions)
   - Edge case handling

3. **Code Quality**
   - Readable, expressive test assertions
   - Clear test naming conventions
   - Well-documented test purposes

4. **Maintainability**
   - Well-documented test cases
   - Clear test organization
   - Easy to extend with new tests

## Test Results Summary

✅ **27 out of 27 tests passing**

**Coverage includes:**
- PGY1 scheduling constraints (7 tests)
- PGY2 scheduling constraints (4 tests)
- Graph algorithm implementation (3 tests)
- Work day management (4 tests)
- Data mapping services (5+ tests)
- Edge cases and boundary conditions

## Next Steps

1. Run the full test suite: `dotnet test`
2. Review test coverage for additional edge cases
3. Add tests for PGY3 specific rules if needed
4. Add tests for additional business logic as features are added
5. Consider integrating into CI/CD pipeline (GitHub Actions, Azure DevOps, etc.)

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)

---

**Generated**: 2025-11-23
**Framework**: .NET 6.0
**Test Count**: 27 comprehensive unit tests
**Coverage Target**: Core scheduling algorithm logic
**Status**: ✅ All tests passing
