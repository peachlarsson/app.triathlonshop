CREATE TABLE Country (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    Code                NVARCHAR(2)         NOT NULL,
    CountryName         NVARCHAR(100)       NOT NULL,
    ContinentCode       NVARCHAR(2)         NOT NULL,
    ContinentName       NVARCHAR(100)       NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);
