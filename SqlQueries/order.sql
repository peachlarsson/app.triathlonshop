CREATE TABLE SalesOrder (
    Id                      BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderNumber             VARCHAR(50)         NOT NULL,
    OrderStatus             NVARCHAR(50)        NOT NULL,
    CustomerId              BIGINT              NOT NULL,
    TotalPrice              DECIMAL(18, 2)      NOT NULL,
    PlacedAt                DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    DestinationLocationId   BIGINT              NOT NULL,
    DestinationAddress      NVARCHAR(500)       NULL,
    IsConfirmed             BIT                 NOT NULL                DEFAULT 0,
    ConfirmedAt             DATETIMEOFFSET      NULL,
    CreatedAt               DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt               DATETIMEOFFSET      NULL,
    CONSTRAINT CK_SalesOrder_CustomerId FOREIGN KEY (CustomerId) REFERENCES Customer(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_SalesOrder_DestinationLocation FOREIGN KEY (DestinationLocationId) REFERENCES Location(Id) ON DELETE NO ACTION
);
