CREATE TABLE Location (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    CountryId           BIGINT              NOT NULL,
    CityId              BIGINT              NULL,
    AirportId           BIGINT              NULL,

    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_Location_Country FOREIGN KEY (CountryId) REFERENCES Country(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Location_City FOREIGN KEY (CityId) REFERENCES City(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Location_Airport FOREIGN KEY (AirportId) REFERENCES Airport(Id) ON DELETE CASCADE
);
