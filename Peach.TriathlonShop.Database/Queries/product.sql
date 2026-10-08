CREATE TABLE Product (
    Id                  BIGINT              PRIMARY KEY,
    CategoryId          BIGINT              NOT NULL                DEFAULT 1,
    Name                VARCHAR(255)        NOT NULL                DEFAULT 'Unknown',
    Price               DECIMAL(18, 2)      NOT NULL                DEFAULT 0,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
    CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);

-- INSERT INTO 