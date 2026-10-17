CREATE TABLE Article (
    Id                  BIGINT              PRIMARY KEY             IDENTITY(1,1),
    ArticleNumber       VARCHAR(50)         NOT NULL,
    ArticleName         VARCHAR(255)        NOT NULL,
    UnitPrice           DECIMAL(18, 2)      NOT NULL,
    CategoryId          BIGINT              NOT NULL                DEFAULT 1,
    BrandId             BIGINT              NOT NULL                DEFAULT 1,
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL,
);
