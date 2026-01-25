using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalDemo.Models;

[Table("pgy4_rotation_requests")]
public class PGY4RotationRequests
{

    [Key][Column("request_id")] public Guid RequestId { get; set; }

    [Column("resident_id")]
    [MaxLength(15)]
    public string ResidentId { get; set; }

    [Column("first_priority")]
    [MaxLength(45)]
    public string? FirstPriority { get; set; }

    [Column("second_priority")]
    [MaxLength(45)]
    public string? SecondPriority { get; set; }

    [Column("third_priority")]
    [MaxLength(45)]
    public string? ThirdPriority { get; set; }

    [Column("fourth_priority")]
    [MaxLength(45)]
    public string? FourthPriority { get; set; }

    [Column("fifth_priority")]
    [MaxLength(45)]
    public string? FifthPriority { get; set; }

    [Column("sixth_priority")]
    [MaxLength(45)]
    public string? SixthPriority { get; set; }

    [Column("seventh_priority")]
    [MaxLength(45)]
    public string? SeventhPriority { get; set; }

    [Column("eighth_priority")]
    [MaxLength(45)]
    public string? EighthPriority { get; set; }

    [Column("alternative_1")]
    [MaxLength(45)]
    public string? Alternative1 { get; set; }

    [Column("alternative_2")]
    [MaxLength(45)]
    public string? Alternative2 { get; set; }

    [Column("alternative_3")]
    [MaxLength(45)]
    public string? Alternative3 { get; set; }

    [Column("avoid_1")]
    [MaxLength(45)]
    public string? Avoid1 { get; set; }

    [Column("avoid_2")]
    [MaxLength(45)]
    public string? Avoid2 { get; set; }

    [Column("avoid_3")]
    [MaxLength(45)]
    public string? Avoid3 { get; set; }

    [Column("additional_notes")]
    [MaxLength(1000)]
    public string? AdditionalNotes { get; set; }
}