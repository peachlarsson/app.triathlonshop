CREATE TABLE Purchase (
    Id                  BIGINT              PRIMARY KEY,
    Price               DECIMAL(18, 2)      NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);

