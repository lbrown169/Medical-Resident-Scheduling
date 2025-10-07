using MedicalDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalDemo.Services;

public class MiscService
{
    private readonly MedicalContext _context;

    public MiscService(MedicalContext context)
    {
        _context = context;
    }

    public async Task<List<Residents>> FindTotalHours()
    {
        List<Residents> residents = await _context.residents.ToListAsync();
        List<Dates> dates = await _context.dates.ToListAsync();

        List<Dates> datesForCurrentYear = dates;

        List<ResidentWithDates> residentsWithDates = new();


        foreach (Residents resident in residents)
        {
            List<Dates> datesForResident = datesForCurrentYear
                .Where(d => d.ResidentId == resident.resident_id)
                .ToList();

            int totalHours = 0;
            foreach (Dates date in datesForResident)
            {
                totalHours += HoursByCallType(date.CallType);
            }

            resident.total_hours = totalHours;
        }


        // saves to db
        await _context.SaveChangesAsync();

        return residents;
    }

    public async Task<List<Residents>> FindBiYearlyHours(int year)
    {
        List<Residents> residents = await _context.residents.ToListAsync();
        List<Dates> dates = await _context.dates.ToListAsync();

        List<Dates> filteredDates = dates
            .Where(d =>
                d.Date.Year == year && d.Date.Month >= 7 && d.Date.Month <= 12)
            .ToList();


        List<ResidentWithDates> residentsWithDates = new();


        foreach (Residents resident in residents)
        {
            List<Dates> datesForResident = filteredDates
                .Where(d => d.ResidentId == resident.resident_id)
                .ToList();

            int totalHours = 0;
            foreach (Dates date in datesForResident)
            {
                totalHours += HoursByCallType(date.CallType);
            }

            resident.bi_yearly_hours = totalHours;
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
        public Residents Resident { get; set; }
        public List<Dates> Dates { get; set; }
    }
}