using MedicalDemo.Models;
using MedicalDemo.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class MiscService
{
    private readonly MedicalContext _context;

    public MiscService(MedicalContext context)
    {
        _context = context;
    }

    public async Task<List<Resident>> FindTotalHours()
    {
        List<Resident> residents = await _context.Residents.ToListAsync();
        List<Date> dates = await _context.Dates.ToListAsync();

        List<Date> datesForCurrentYear = dates;

        List<ResidentWithDates> residentsWithDates = new();


        foreach (Resident resident in residents)
        {
            List<Date> datesForResident = datesForCurrentYear
                .Where(d => d.ResidentId == resident.ResidentId)
                .ToList();

            int totalHours = 0;
            foreach (Date date in datesForResident)
            {
                totalHours += HoursByCallType(date.CallType);
            }

            resident.TotalHours = totalHours;
        }


        // saves to db
        await _context.SaveChangesAsync();

        return residents;
    }

    public async Task<List<Resident>> FindBiYearlyHours(int year)
    {
        List<Resident> residents = await _context.Residents.ToListAsync();
        List<Date> dates = await _context.Dates.Where(date => date.Schedule.GeneratedYear == year).ToListAsync();

        foreach (Resident resident in residents)
        {
            List<Date> datesForResident = dates
                .Where(d => d.ResidentId == resident.ResidentId)
                .ToList();

            int totalHours = 0;
            foreach (Date date in datesForResident)
            {
                totalHours += HoursByCallType(date.CallType);
            }

            resident.BiYearlyHours = totalHours;
        }


        // saves to db
        await _context.SaveChangesAsync();

        return residents;
    }

    // call types to total hours by call types
    private static int HoursByCallType(string callType)
    {
        return callType switch
        {
            "Short" => 3,
            "12h" => 12,
            "24h" => 24,
            _ => 0
        };
    }


    public class ResidentWithDates
    {
        public Resident Resident { get; set; }
        public List<Date> Dates { get; set; }
    }
}