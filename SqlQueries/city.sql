CREATE TABLE City (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),

    CityName            NVARCHAR(200)       NOT NULL,
    CountryCode         NVARCHAR(10)        NOT NULL,
    Latitude            FLOAT               NULL,
    Longitude           FLOAT               NULL,
    Population          BIGINT              NULL,
    Elevation           INT                 NULL,
    Timezone            NVARCHAR(40)        NULL,

    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);
