using FluentAssertions;
using MedicalDemo.Algorithm;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;
using MedicalDemo.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MedicalDemo.Tests;

/// <summary>
/// Unit tests for the scheduling algorithm logic.
/// These tests focus on isolated business logic without database dependencies.
/// Uses Moq to mock the database context for complete isolation.
/// </summary>
public class AlgorithmLogicTests
{
    private MedicalContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MedicalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MedicalContext(options);
    }

    #region PGY1 CanWork Tests

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenResidentHasVacation()
    {
        // Arrange
        var pgy1 = new PGY1("John Doe")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        // Set up a valid role that allows work
        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var vacationDate = new DateTime(2026, 7, 15); // Tuesday
        pgy1.requestVacation(vacationDate);

        // Act
        bool canWork = pgy1.canWork(vacationDate);

        // Assert
        canWork.Should().BeFalse("resident has vacation on this date");
        pgy1.isVacation(vacationDate).Should().BeTrue();
    }

    [Fact]
    public void CanWork_ShouldReturnTrue_WhenResidentHasNoVacation()
    {
        // Arrange
        var pgy1 = new PGY1("Jane Smith")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var workDate = new DateTime(2026, 7, 15); // Tuesday

        // Act
        bool canWork = pgy1.canWork(workDate);

        // Assert
        canWork.Should().BeTrue("resident has no vacation and valid role");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenResidentOnBlackoutRotation_NightFloat()
    {
        // Arrange
        var pgy1 = new PGY1("Bob Johnson")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        // Set NightFloat rotation for July (index 0 in academic year)
        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = i == 0 ? HospitalRole.NightFloat : HospitalRole.Inpatient;
        }

        var julyDate = new DateTime(2026, 7, 15); // Tuesday in July

        // Act
        bool canWork = pgy1.canWork(julyDate);

        // Assert
        canWork.Should().BeFalse("NightFloat rotation does not allow short calls");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenResidentOnEmergencyMedRotation()
    {
        // Arrange
        var pgy1 = new PGY1("Alice Brown")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        // Set EmergencyMed rotation for August (index 1)
        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = i == 1 ? HospitalRole.EmergencyMed : HospitalRole.Inpatient;
        }

        var augustDate = new DateTime(2026, 8, 12); // Wednesday in August

        // Act
        bool canWork = pgy1.canWork(augustDate);

        // Assert
        canWork.Should().BeFalse("EmergencyMed rotation does not allow short calls");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenConsecutiveWeekendDays()
    {
        // Arrange
        var pgy1 = new PGY1("Charlie Davis")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var saturday = new DateTime(2026, 7, 18); // Saturday
        var sunday = new DateTime(2026, 7, 19);   // Sunday

        pgy1.addWorkDay(saturday);

        // Act
        bool canWorkSunday = pgy1.canWork(sunday);

        // Assert
        canWorkSunday.Should().BeFalse("cannot work consecutive weekend days");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenNoRoleAssigned()
    {
        // Arrange
        var pgy1 = new PGY1("Eve Wilson")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        // Leave all roles as null
        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = null;
        }

        var testDate = new DateTime(2026, 7, 15); // Tuesday

        // Act
        bool canWork = pgy1.canWork(testDate);

        // Assert
        canWork.Should().BeFalse("resident has no role assigned for this month");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenWorkingBeforeLastTrainingDate()
    {
        // Arrange
        var pgy1 = new PGY1("Frank Miller")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = true,
            lastTrainingDate = new DateTime(2026, 8, 31)
        };

        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var dateBeforeTrainingEnds = new DateTime(2026, 8, 15);

        // Act
        bool canWork = pgy1.canWork(dateBeforeTrainingEnds);

        // Assert
        canWork.Should().BeFalse("cannot work before last training date");
    }

    [Fact]
    public void CanWork_ShouldReturnFalse_WhenWeekdayBeforeWeekendShift()
    {
        // Arrange
        var pgy1 = new PGY1("Grace Lee")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        for (int i = 0; i < 12; i++)
        {
            pgy1.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var friday = new DateTime(2026, 7, 17);   // Friday
        var saturday = new DateTime(2026, 7, 18); // Saturday

        pgy1.addWorkDay(saturday);

        // Act
        bool canWorkFriday = pgy1.canWork(friday);

        // Assert
        canWorkFriday.Should().BeFalse("cannot work Friday before Saturday shift");
    }

    #endregion

    #region PGY2 CanWork Tests

    [Fact]
    public void PGY2_CanWork_ShouldReturnFalse_WhenResidentHasVacation()
    {
        // Arrange
        var pgy2 = new PGY2("Mark Taylor")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        for (int i = 0; i < 12; i++)
        {
            pgy2.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var vacationDate = new DateTime(2026, 9, 10);
        pgy2.requestVacation(vacationDate);

        // Act
        bool canWork = pgy2.canWork(vacationDate);

        // Assert
        canWork.Should().BeFalse("PGY2 has vacation on this date");
    }

    [Fact]
    public void PGY2_CanWork_ShouldReturnFalse_WhenConsecutiveWeekendDays()
    {
        // Arrange
        var pgy2 = new PGY2("Nancy Adams")
        {
            id = Guid.NewGuid().ToString(),
            inTraining = false
        };

        for (int i = 0; i < 12; i++)
        {
            pgy2.rolePerMonth[i] = HospitalRole.Inpatient;
        }

        var saturday = new DateTime(2026, 9, 12); // Saturday
        var sunday = new DateTime(2026, 9, 13);   // Sunday

        pgy2.addWorkDay(saturday);

        // Act
        bool canWorkSunday = pgy2.canWork(sunday);

        // Assert
        canWorkSunday.Should().BeFalse("PGY2 cannot work consecutive weekend days");
    }

    #endregion

    #region Graph MaxFlow Tests

    [Fact]
    public void Graph_MaxFlow_ShouldCalculateCorrectly_SimpleCase()
    {
        // Arrange
        // Simple bipartite graph: 2 residents, 2 dates
        // Source -> Resident1 (capacity 1)
        // Source -> Resident2 (capacity 1)
        // Resident1 -> Date1 (capacity 1)
        // Resident1 -> Date2 (capacity 1)
        // Resident2 -> Date1 (capacity 1)
        // Resident2 -> Date2 (capacity 1)
        // Date1 -> Sink (capacity 1)
        // Date2 -> Sink (capacity 1)

        int nodeCount = 6; // Source, Resident1, Resident2, Date1, Date2, Sink
        int source = 0;
        int resident1 = 1;
        int resident2 = 2;
        int date1 = 3;
        int date2 = 4;
        int sink = 5;

        var graph = new Graph(nodeCount);

        // Source to residents
        graph.addEdge(source, resident1, 1);
        graph.addEdge(source, resident2, 1);

        // Residents to dates
        graph.addEdge(resident1, date1, 1);
        graph.addEdge(resident1, date2, 1);
        graph.addEdge(resident2, date1, 1);
        graph.addEdge(resident2, date2, 1);

        // Dates to sink
        graph.addEdge(date1, sink, 1);
        graph.addEdge(date2, sink, 1);

        // Act
        int maxFlow = graph.getFlow(source, sink);

        // Assert
        maxFlow.Should().Be(2, "max flow should match the number of dates that can be filled");
    }

    [Fact]
    public void Graph_MaxFlow_ShouldCalculateCorrectly_BottleneckCase()
    {
        // Arrange
        // Test case where source has limited capacity
        int nodeCount = 4; // Source, Middle, Middle2, Sink
        int source = 0;
        int middle1 = 1;
        int middle2 = 2;
        int sink = 3;

        var graph = new Graph(nodeCount);

        // Bottleneck at source
        graph.addEdge(source, middle1, 2);
        graph.addEdge(middle1, middle2, 5);
        graph.addEdge(middle2, sink, 5);

        // Act
        int maxFlow = graph.getFlow(source, sink);

        // Assert
        maxFlow.Should().Be(2, "max flow should be limited by the bottleneck edge");
    }

    [Fact]
    public void Graph_MaxFlow_ShouldReturnZero_WhenNoPath()
    {
        // Arrange
        int nodeCount = 4;
        int source = 0;
        int isolated1 = 1;
        int isolated2 = 2;
        int sink = 3;

        var graph = new Graph(nodeCount);

        // No edge from source to sink
        graph.addEdge(source, isolated1, 1);
        graph.addEdge(isolated2, sink, 1);
        // No connection between isolated1 and isolated2

        // Act
        int maxFlow = graph.getFlow(source, sink);

        // Assert
        maxFlow.Should().Be(0, "max flow should be zero when no path exists");
    }

    [Fact]
    public void Graph_MaxFlow_ShouldCalculateCorrectly_ComplexBipartiteGraph()
    {
        // Arrange
        // 3 residents, 4 dates, each resident can work max 2 days
        int nodeCount = 9; // Source, R1, R2, R3, D1, D2, D3, D4, Sink
        int source = 0;
        int r1 = 1, r2 = 2, r3 = 3;
        int d1 = 4, d2 = 5, d3 = 6, d4 = 7;
        int sink = 8;

        var graph = new Graph(nodeCount);

        // Source to residents (each can work max 2 days)
        graph.addEdge(source, r1, 2);
        graph.addEdge(source, r2, 2);
        graph.addEdge(source, r3, 2);

        // Residents to dates (full availability)
        graph.addEdge(r1, d1, 1);
        graph.addEdge(r1, d2, 1);
        graph.addEdge(r1, d3, 1);
        graph.addEdge(r1, d4, 1);

        graph.addEdge(r2, d1, 1);
        graph.addEdge(r2, d2, 1);
        graph.addEdge(r2, d3, 1);
        graph.addEdge(r2, d4, 1);

        graph.addEdge(r3, d1, 1);
        graph.addEdge(r3, d2, 1);
        graph.addEdge(r3, d3, 1);
        graph.addEdge(r3, d4, 1);

        // Dates to sink (each date needs 1 resident)
        graph.addEdge(d1, sink, 1);
        graph.addEdge(d2, sink, 1);
        graph.addEdge(d3, sink, 1);
        graph.addEdge(d4, sink, 1);

        // Act
        int maxFlow = graph.getFlow(source, sink);

        // Assert
        maxFlow.Should().Be(4, "all 4 dates should be covered");
    }

    #endregion

    #region Edge Tests

    [Fact]
    public void Edge_Flow_ShouldCalculateCorrectly()
    {
        // Arrange
        var edge = new Edge(destination: 5, reverse: 0, cap: 10);

        // Act
        edge.currentCap = 3; // 7 units have flowed
        int flow = edge.flow();

        // Assert
        flow.Should().Be(7, "flow = original capacity - current capacity");
        edge.originalCap.Should().Be(10);
        edge.currentCap.Should().Be(3);
    }

    [Fact]
    public void Edge_Flow_ShouldHandleFullCapacity()
    {
        // Arrange
        var edge = new Edge(destination: 2, reverse: 1, cap: 5);

        // Act
        edge.currentCap = 0; // fully utilized
        int flow = edge.flow();

        // Assert
        flow.Should().Be(5, "flow should equal original capacity when fully utilized");
    }

    [Fact]
    public void Edge_Flow_ShouldHandleNoFlow()
    {
        // Arrange
        var edge = new Edge(destination: 3, reverse: 2, cap: 8);

        // Act (no change to currentCap, so no flow)
        int flow = edge.flow();

        // Assert
        flow.Should().Be(0, "flow should be zero when no capacity has been used");
        edge.currentCap.Should().Be(8);
    }

    #endregion

    #region PGY Model Work Day Management Tests

    [Fact]
    public void PGY1_AddWorkDay_ShouldReturnTrue_WhenDayNotAlreadyWorked()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var workDate = new DateTime(2026, 7, 15);

        // Act
        bool result = pgy1.addWorkDay(workDate);

        // Assert
        result.Should().BeTrue();
        pgy1.isWorking(workDate).Should().BeTrue();
    }

    [Fact]
    public void PGY1_AddWorkDay_ShouldReturnFalse_WhenDayAlreadyWorked()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var workDate = new DateTime(2026, 7, 15);
        pgy1.addWorkDay(workDate);

        // Act
        bool result = pgy1.addWorkDay(workDate);

        // Assert
        result.Should().BeFalse("cannot add the same work day twice");
    }

    [Fact]
    public void PGY1_RemoveWorkDay_ShouldRemoveDay()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var workDate = new DateTime(2026, 7, 15);
        pgy1.addWorkDay(workDate);

        // Act
        pgy1.removeWorkDay(workDate);

        // Assert
        pgy1.isWorking(workDate).Should().BeFalse();
    }

    [Fact]
    public void PGY1_LastWorkDay_ShouldReturnLatestDate()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var date1 = new DateTime(2026, 7, 15);
        var date2 = new DateTime(2026, 7, 20);
        var date3 = new DateTime(2026, 7, 10);

        pgy1.addWorkDay(date1);
        pgy1.addWorkDay(date2);
        pgy1.addWorkDay(date3);

        // Act
        DateTime lastDay = pgy1.lastWorkDay();

        // Assert
        lastDay.Should().Be(date2, "should return the latest work date");
    }

    [Fact]
    public void PGY1_FirstWorkDay_ShouldReturnEarliestDate()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var date1 = new DateTime(2026, 7, 15);
        var date2 = new DateTime(2026, 7, 20);
        var date3 = new DateTime(2026, 7, 10);

        pgy1.addWorkDay(date1);
        pgy1.addWorkDay(date2);
        pgy1.addWorkDay(date3);

        // Act
        DateTime firstDay = pgy1.firstWorkDay();

        // Assert
        firstDay.Should().Be(date3, "should return the earliest work date");
    }

    [Fact]
    public void PGY1_RequestVacation_ShouldAddVacation()
    {
        // Arrange
        var pgy1 = new PGY1("Test Resident");
        var vacationDate = new DateTime(2026, 8, 1);

        // Act
        pgy1.requestVacation(vacationDate);

        // Assert
        pgy1.isVacation(vacationDate).Should().BeTrue();
    }

    [Fact]
    public void PGY2_AddWorkDay_ShouldWorkCorrectly()
    {
        // Arrange
        var pgy2 = new PGY2("Test Resident");
        var workDate = new DateTime(2026, 9, 15);

        // Act
        bool result = pgy2.addWorkDay(workDate);

        // Assert
        result.Should().BeTrue();
        pgy2.isWorking(workDate).Should().BeTrue();
    }

    #endregion

    #region SchedulingMapperService Tests

    [Fact]
    public void MapToPGY1DTO_ShouldMapCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = new SchedulingMapperService(context);

        var resident = new Residents
        {
            resident_id = Guid.NewGuid().ToString(),
            first_name = "John",
            last_name = "Doe",
            graduate_yr = 1,
            email = "john@test.com"
        };

        var rotations = new List<Rotations>
        {
            new Rotations
            {
                RotationId = Guid.NewGuid(),
                ResidentId = resident.resident_id,
                Month = "July",
                Rotation = "Inpt Psy"
            }
        };

        var vacations = new List<Vacations>
        {
            new Vacations
            {
                VacationId = Guid.NewGuid(),
                ResidentId = resident.resident_id,
                Date = new DateTime(2026, 7, 15),
                Status = "Approved"
            }
        };

        // Act
        var dto = mapper.MapToPGY1DTO(resident, rotations, vacations, new List<DatesDTO>());

        // Assert
        dto.Should().NotBeNull();
        dto.ResidentId.Should().Be(resident.resident_id);
        dto.Name.Should().Be("John Doe");
        dto.VacationRequests.Should().HaveCount(1);
        dto.VacationRequests.Should().Contain(new DateTime(2026, 7, 15));
        dto.InTraining.Should().BeTrue("graduate_yr == 1 means in training");
    }

    [Fact]
    public void MapToPGY2DTO_ShouldMapCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mapper = new SchedulingMapperService(context);

        var resident = new Residents
        {
            resident_id = Guid.NewGuid().ToString(),
            first_name = "Jane",
            last_name = "Smith",
            graduate_yr = 2,
            email = "jane@test.com"
        };

        var rotations = new List<Rotations>
        {
            new Rotations
            {
                RotationId = Guid.NewGuid(),
                ResidentId = resident.resident_id,
                Month = "September",
                Rotation = "Geri"
            }
        };

        var vacations = new List<Vacations>();

        // Act
        var dto = mapper.MapToPGY2DTO(resident, rotations, vacations, new List<DatesDTO>());

        // Assert
        dto.Should().NotBeNull();
        dto.ResidentId.Should().Be(resident.resident_id);
        dto.Name.Should().Be("Jane Smith");
        dto.VacationRequests.Should().BeEmpty();
        dto.InTraining.Should().BeTrue("graduate_yr == 2 means in training");
    }

    #endregion
}
