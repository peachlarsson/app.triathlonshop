CREATE TABLE CustomerNotification (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId             BIGINT              NOT NULL,
    OrderStatus         NVARCHAR(50)        NOT NULL,
    Destination         NVARCHAR(MAX)       NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
