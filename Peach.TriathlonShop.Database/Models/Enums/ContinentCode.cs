using System.ComponentModel.DataAnnotations;

namespace Peach.TriathlonShop.Database.Models.Enums;

public enum ContinentCode
{
    [Display(Name = "Africa")]
    AF,

    [Display(Name = "Antarctica")]
    AN,
    
    [Display(Name = "Asia")]
    AS,
    
    [Display(Name = "Europe")]
    EU,
    
    [Display(Name = "North America")]
    NA,
    
    [Display(Name = "Oceania")]
    OC,
    
    [Display(Name = "South America")]
    SA,
}
