CREATE TABLE Route (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId             BIGINT              NOT NULL,
    RouteStatus         NVARCHAR(50)        NOT NULL,
    StartLocationId     BIGINT              NOT NULL,
    EndLocationId       BIGINT              NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
