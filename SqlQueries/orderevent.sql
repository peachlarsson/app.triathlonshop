CREATE TABLE OrderEvent (
    Id                      BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId                 BIGINT              NOT NULL,
    Status                  NVARCHAR(50)        NOT NULL,
    Description             NVARCHAR(MAX)       NOT NULL,
    CreatedAt               DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt               DATETIMEOFFSET      NULL,
);
