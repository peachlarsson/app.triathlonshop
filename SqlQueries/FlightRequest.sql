CREATE TABLE FlightRequest (
    Id                      BIGINT              PRIMARY KEY             IDENTITY(1,1),

    RequestParams           NVARCHAR(MAX)       NOT NULL,
    ResponseBody            NVARCHAR(MAX)       NULL,
    Source                  NVARCHAR(255)       NULL,
    StatusCode              INT                 NOT NULL,
    
    CreatedAt               DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt               DATETIMEOFFSET      NULL
);
