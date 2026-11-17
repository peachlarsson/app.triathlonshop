CREATE TABLE Airport (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),

    IataCode            NVARCHAR(10)        NULL,
    IcaoCode            NVARCHAR(10)        NULL,
    Name                NVARCHAR(255)       NULL,
    IsoCountry          NVARCHAR(50)        NULL,
    Municipality        NVARCHAR(255)       NULL,
    Timezone            NVARCHAR(100)       NULL,
    Latitude            DECIMAL(9,6)        NULL,
    Longitude           DECIMAL(9,6)        NULL,
    
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
