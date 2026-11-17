CREATE TABLE Warehouse (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    WarehouseName       NVARCHAR(200)       NOT NULL,
    LocationId          BIGINT              NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_Warehouse_Location FOREIGN KEY (LocationId) REFERENCES Location(Id) ON DELETE CASCADE
);
