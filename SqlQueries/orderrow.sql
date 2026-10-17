CREATE TABLE SalesOrderRow (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    OrderId             BIGINT              NOT NULL,
    LineNumber          INT                 NOT NULL,
    ArticleId           BIGINT              NOT NULL,
    Quantity            INT                 NOT NULL,
    RowPrice            DECIMAL(18, 2)      NOT NULL,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);
