CREATE TABLE Customer (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    IsCompany           BIT                 NOT NULL                DEFAULT 0,
    CustomerName        VARCHAR(255)        NOT NULL,
    Email               VARCHAR(255)        NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);