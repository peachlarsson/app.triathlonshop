CREATE TABLE Invoice (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId             BIGINT              NOT NULL,
    InvoiceStatus       NVARCHAR(50)        NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
