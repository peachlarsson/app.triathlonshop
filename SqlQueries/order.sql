CREATE TABLE SalesOrder (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderNumber         VARCHAR(50)         NOT NULL,
    OrderStatus         NVARCHAR(50)        NOT NULL,
    CustomerId          BIGINT              NOT NULL,
    TotalPrice          DECIMAL(18, 2)      NOT NULL,
    IsConfirmed         BIT                 NOT NULL                DEFAULT 0,
    ConfirmedAt         DATETIMEOFFSET      NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
