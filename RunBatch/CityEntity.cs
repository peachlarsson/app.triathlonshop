namespace RunBatch;

public class CityEntity
{
    public string Name { get; set; } = string.Empty;
    public string AsciiName { get; set; } = string.Empty;
    public string? AlternateNames { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string FeatureClass { get; set; } = string.Empty;
    public string FeatureCode { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;
    public string? Cc2 { get; set; }

    public string? Admin1Code { get; set; }
    public string? Admin2Code { get; set; }
    public string? Admin3Code { get; set; }
    public string? Admin4Code { get; set; }

    public long Population { get; set; }

    public int? Elevation { get; set; }
    public int Dem { get; set; }

    public string Timezone { get; set; } = string.Empty;
    public DateOnly ModificationDate { get; set; }
}