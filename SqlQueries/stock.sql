CREATE TABLE Stock (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    ArticleId           BIGINT              NOT NULL,
    Quantity            INT                 NOT NULL                DEFAULT 0,
    WarehouseId         BIGINT              NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_Stock_Article FOREIGN KEY (ArticleId) REFERENCES Article(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Stock_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouse(Id) ON DELETE CASCADE
);
