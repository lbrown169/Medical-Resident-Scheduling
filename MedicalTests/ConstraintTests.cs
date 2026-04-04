using MedicalDemo.Algorithms.OnCallScheduleGenerator.Constraints;
using MedicalDemo.Models;
using MedicalDemo.Models.DTO.Scheduling;

namespace MedicalTests;

public static class ConstraintTests
{
    #region Helpers

    private static ResidentDto CreateResident(
        int pgy,
        HospitalRole?[] rolePerMonth,
        HashSet<DateOnly>? workDays = null,
        HashSet<DateOnly>? committedWorkDays = null,
        HashSet<DateOnly>? morningVacation = null,
        HashSet<DateOnly>? afternoonVacation = null
    )
    {
        return pgy switch
        {
            1 => new Pgy1Dto
            {
                ResidentId = "TEST001",
                Name = "Test Resident",
                RolePerMonth = rolePerMonth,
                WorkDays = workDays ?? [],
                CommitedWorkDays = committedWorkDays ?? [],
                MorningVacationRequests = morningVacation ?? [],
                AfternoonVacationRequests = afternoonVacation ?? []
            },
            2 => new Pgy2Dto
            {
                ResidentId = "TEST001",
                Name = "Test Resident",
                RolePerMonth = rolePerMonth,
                WorkDays = workDays ?? [],
                CommitedWorkDays = committedWorkDays ?? [],
                MorningVacationRequests = morningVacation ?? [],
                AfternoonVacationRequests = afternoonVacation ?? []
            },
            3 => new Pgy3Dto
            {
                ResidentId = "TEST001",
                Name = "Test Resident",
                RolePerMonth = rolePerMonth,
                WorkDays = workDays ?? [],
                CommitedWorkDays = committedWorkDays ?? [],
                MorningVacationRequests = morningVacation ?? [],
                AfternoonVacationRequests = afternoonVacation ?? []
            },
            _ => throw new ArgumentOutOfRangeException(nameof(pgy))
        };
    }

    // Role that allows all call types
    private static HospitalRole AllowAllRole =>
        new("Allow All", true, true, true, true);

    // Role that blocks all call types
    private static HospitalRole BlockAllRole =>
        new("Block All", false, false, false, false);

    // Role that allows only short call
    private static HospitalRole ShortOnlyRole =>
        new("Short only", true, false, false, false);

    private static HospitalRole ShortTrainingOnly =>
        new("Short training only", false, false, true, false);

    // Role that allows only long call
    private static HospitalRole LongOnlyRole => new("Long only", false, true, false, false);

    private static HospitalRole LongTrainingOnly => new("Long training only", false, false, false, true);

    // July = calendar month 7 = academic index 0
    // A weekday in July (training month)
    private static DateOnly JulyWeekday => new(2025, 7, 7);   // Monday

    // A weekday outside of July/August
    private static DateOnly OctoberWeekday => new(2025, 10, 6); // Monday

    // A weekend in July (training month)
    private static DateOnly JulySaturday => new(2025, 7, 19);

    // A weekend outside training months
    private static DateOnly OctoberSaturday => new(2025, 10, 4);

    // A Sunday in October (no PGY1s)
    private static DateOnly OctoberSunday => new(2025, 10, 5);

    // Chrismas (no PGY2s)
    private static DateOnly ChristmasDay => new(2025, 12, 25);

    /// <summary>
    /// Builds a role array where all 12 months have the given role.
    /// RolePerMonth is calendar-indexed (0 = January).
    /// </summary>
    private static HospitalRole?[] UniformRoles(HospitalRole role) =>
        Enumerable.Repeat<HospitalRole?>(role, 12).ToArray();

    #endregion

    #region OneShiftADayConstraint

    public class OneShiftADayConstraintTests
    {
        private readonly OneShiftADayConstraint _constraint = new();

        [Fact]
        public void NoViolation_WhenResidentNotWorking()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenResidentAlreadyWorkingThatDay()
        {
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [OctoberWeekday]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenResidentWorkingDifferentDay()
        {
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [OctoberWeekday.AddDays(1)]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }
    }

    #endregion

    #region NotOnVacationConstraint

    public class NotOnVacationConstraintTests
    {
        private readonly NotOnVacationConstraint _constraint = new();

        [Fact]
        public void NoViolation_WhenNotOnVacation()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenOnMorningVacation_OnWeekday()
        {
            // Short shifts on weekdays arent' affected by morning vacations
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                morningVacation: [OctoberWeekday]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenOnMorningVacation_OnWeekend()
        {
            // Weekdays cover morning
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                morningVacation: [OctoberSaturday]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenOnAfternoonVacation_OnWeekend()
        {
            // Weekends (long call) cover afternoon
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                afternoonVacation: [OctoberSaturday]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenMorningVacationOnDifferentDay()
        {
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                morningVacation: [OctoberWeekday.AddDays(1)]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }
    }

    #endregion

    #region ShiftMatchesRotationConstraint

    public class ShiftMatchesRotationConstraintTests
    {
        private readonly ShiftMatchesRotationConstraint _constraint = new();

        // Allow all
        [Fact]
        public void NoViolation_WhenRotationAllowsAll_OnTrainingWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulyWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsAll_OnTrainingWeekend()
        {
            ResidentDto resident = CreateResident(2, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulySaturday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsAll_OnNormalWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsAll_OnNormalWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.False(result.IsViolated);
        }

        // Short only
        [Fact]
        public void Violation_WhenRotationAllowsShort_OnTrainingWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulyWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsShort_OnTrainingWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulySaturday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsShort_OnNormalWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsShort_OnNormalWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.True(result.IsViolated);
        }

        // Short Training Only
        [Fact]
        public void NoViolation_WhenRotationAllowsTrainingShort_OnTrainingWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, JulyWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsTrainingShort_OnTrainingWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, JulySaturday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsTrainingShort_OnNormalWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsTrainingShort_OnNormalWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(ShortTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.True(result.IsViolated);
        }

        // Long only
        [Fact]
        public void Violation_WhenRotationAllowsLong_OnTrainingWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulyWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsLong_OnTrainingWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, JulySaturday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsLong_OnNormalWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsLong_OnNormalWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongOnlyRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.False(result.IsViolated);
        }

        // Long Training Only
        [Fact]
        public void Violation_WhenRotationAllowsTrainingLong_OnTrainingWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, JulyWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenRotationAllowsTrainingLong_OnTrainingWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, JulySaturday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenRotationAllowsTrainingLong_OnNormalWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WWhenRotationAllowsTrainingLong_OnNormalWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(LongTrainingOnly));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.True(result.IsViolated);
        }
    }

    #endregion

    #region ShiftMatchesPgyDateConstraint

    public class ShiftMatchesPgyDateConstraintTests
    {
        private readonly ShiftMatchesPgyDateConstraint _constraint = new();

        [Fact]
        public void NoViolation_WhenValidShiftTypeExistsForPgy1_OnWeekday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenValidShiftTypeExistsForPgy1_OnWeekend()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenNoValidShiftTypeExistsForPgy1_OnSunday()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSunday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenValidShiftTypeExistsForPgy2_OnWeekday()
        {
            ResidentDto resident = CreateResident(2, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenValidShiftTypeExistsForPgy2_OnWeekend()
        {
            ResidentDto resident = CreateResident(2, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenNoValidShiftTypeExistsForPgy2_OnWeekend()
        {
            ResidentDto resident = CreateResident(2, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, ChristmasDay);
            Assert.True(result.IsViolated);
        }
    }

    #endregion

    #region NoConsecutiveShiftConstraint

    public class NoConsecutiveShiftConstraintTests
    {
        private readonly NoConsecutiveShiftConstraint _constraint = new();

        [Fact]
        public void NoViolation_WhenNoAdjacentShifts()
        {
            ResidentDto resident = CreateResident(1, UniformRoles(AllowAllRole));
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenWorkingPreviousDay()
        {
            DateOnly prevDay = OctoberWeekday.AddDays(-1);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [prevDay]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenWorkingNextDay()
        {
            DateOnly nextDay = OctoberWeekday.AddDays(1);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [nextDay]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenCommittedPreviousDay()
        {
            DateOnly prevDay = OctoberWeekday.AddDays(-1);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                committedWorkDays: [prevDay]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenCommittedNextDay()
        {
            DateOnly nextDay = OctoberWeekday.AddDays(1);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                committedWorkDays: [nextDay]
            );
            ConstraintResult result = _constraint.Evaluate(resident, OctoberWeekday);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void Violation_WhenSameShiftTypeWorkedWithinWeek()
        {
            // Two Saturdays (same long call type) within a week
            DateOnly saturday1 = OctoberSaturday;
            DateOnly saturday2 = OctoberSaturday.AddDays(7);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [saturday1]
            );
            ConstraintResult result = _constraint.Evaluate(resident, saturday2);
            Assert.True(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenSameShiftTypeWorkedBeyondWeek()
        {
            // Two Saturdays more than 7 days apart
            DateOnly saturday1 = OctoberSaturday;
            DateOnly saturday2 = OctoberSaturday.AddDays(14);
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [saturday1]
            );
            ConstraintResult result = _constraint.Evaluate(resident, saturday2);
            Assert.False(result.IsViolated);
        }

        [Fact]
        public void NoViolation_WhenWorkingTwoDaysApart_DifferentShiftTypes()
        {
            // Weekday and weekend are different shift types, so no in-a-row violation
            ResidentDto resident = CreateResident(
                1,
                UniformRoles(AllowAllRole),
                workDays: [OctoberWeekday]
            );
            // Saturday is 2+ days away and a different shift type
            ConstraintResult result = _constraint.Evaluate(resident, OctoberSaturday);
            Assert.False(result.IsViolated);
        }
    }

    #endregion
}