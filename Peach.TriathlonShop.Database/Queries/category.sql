CREATE TABLE Category (
    Id                  BIGINT              PRIMARY KEY,
    Name                VARCHAR(255)        NOT NULL                DEFAULT 'Unknown',
    CreatedAt           DATETIMEOFFSET      NOT NULL                DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt           DATETIMEOFFSET      NULL
);

INSERT INTO Category (Id, Name) VALUES (1, 'Default');

SELECT * FROM Category;