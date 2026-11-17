CREATE TABLE AirportSchedule (
    Id                      BIGINT              PRIMARY KEY             IDENTITY(1,1),

    IdentIcao               NVARCHAR(10)        NOT NULL,
    IdentIata               NVARCHAR(10)        NOT NULL,
    Operator                NVARCHAR(255)       NOT NULL,
    FlightStatus            NVARCHAR(50)        NOT NULL,

    OriginAirportId         BIGINT              NOT NULL,
    DestinationLocationId   BIGINT              NULL,
    DestinationAirportId    BIGINT              NULL,
    DestinationAirportIaco  BIGINT              NOT NULL,
    DestinationAirportIata  BIGINT              NOT NULL,

    ScheduledDeparture      DATETIMEOFFSET      NOT NULL,
    ScheduledArrival        DATETIMEOFFSET      NOT NULL,
    ActualDeparture         DATETIMEOFFSET      NULL,
    ActualArrival           DATETIMEOFFSET      NULL,
    
    CreatedAt               DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt               DATETIMEOFFSET      NULL,
    CONSTRAINT FK_AirportSchedule_OriginAirport FOREIGN KEY (OriginAirportId) REFERENCES Airport(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AirportSchedule_DestinationAirport FOREIGN KEY (DestinationAirportId) REFERENCES Airport(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_AirportSchedule_DestinationLocation FOREIGN KEY (DestinationLocationId) REFERENCES Location(Id) ON DELETE NO ACTION
);

