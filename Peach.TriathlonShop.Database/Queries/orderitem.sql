CREATE TABLE OrderItem (
    Id                  BIGINT              PRIMARY KEY,
    CustomerId          BIGINT              NOT NULL,
    ProductId           BIGINT              NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_OrderItem_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_OrderItem_Product FOREIGN KEY (ProductId) REFERENCES Product(Id)
);
