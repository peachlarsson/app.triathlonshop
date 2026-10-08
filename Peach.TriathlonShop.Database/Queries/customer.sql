CREATE TABLE Customer (
    Id                  BIGINT              PRIMARY KEY,
    IsGuest             BIT                 NOT NULL                DEFAULT 0,
    Email               VARCHAR(255)        NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);
