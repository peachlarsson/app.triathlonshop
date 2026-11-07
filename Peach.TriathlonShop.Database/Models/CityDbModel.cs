namespace Peach.TriathlonShop.Database.Models;

public class City : BaseEntity
{
    public int GeonameId { get; set; }
    public string Name { get; set; } = "";
    public string AsciiName { get; set; } = "";
    public string AlternateNames { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string FeatureClass { get; set; } = "";
    public string FeatureCode { get; set; } = "";
    public long CountryId { get; set; }
    public string Cc2 { get; set; } = "";
    public string Admin1Code { get; set; } = "";
    public string Admin2Code { get; set; } = "";
    public string Admin3Code { get; set; } = "";
    public string Admin4Code { get; set; } = "";
    public long Population { get; set; }
    public int? Elevation { get; set; }
    public int Dem { get; set; }
    public string Timezone { get; set; } = "";
    public DateOnly ModificationDate { get; set; }

    public CountryEntity Country { get; set; } = default!;
}