# Backend Testing Suite - Medical Resident Scheduling

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
