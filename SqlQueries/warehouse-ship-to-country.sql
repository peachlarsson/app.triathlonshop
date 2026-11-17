CREATE TABLE WarehouseShipToCountry (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),

    WarehouseId         BIGINT              NOT NULL,
    CountryId           BIGINT              NOT NULL,
    
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_WarehouseShipToCountry_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id) ON DELETE CASCADE,
    CONSTRAINT FK_WarehouseShipToCountry_Country FOREIGN KEY (CountryId) REFERENCES Country(Id) ON DELETE CASCADE
);
