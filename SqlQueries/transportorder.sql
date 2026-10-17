CREATE TABLE TransportOrder (
    Id                      BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId                 BIGINT              NOT NULL,
    TransportStatus         NVARCHAR(50)        NOT NULL,
    ConsignmentNumber       NVARCHAR(50)        NOT NULL,
    TrackingUrl             NVARCHAR(MAX)       NULL,
    Carrier                 NVARCHAR(100)       NULL,
    Destination             NVARCHAR(MAX)       NULL,
    ShippedAt               DATETIMEOFFSET      NULL,
    EstimatedDeliveryDate   DATETIMEOFFSET      NULL,
    CreatedAt               DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt               DATETIMEOFFSET      NULL,
);
