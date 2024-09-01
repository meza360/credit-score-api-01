-- 02 data-sat-tributario

-- DROP TABLE IF EXISTS tbl_regime;
INSERT INTO sat.tbl_regime (name, description)
VALUES 
    ('General', '12% sobre compras y paga formulario SAT-2237'),
    ('Pequeño contribuyente', 'Rinde el 5% mensual de ventas y presenta formulario SAT-2046'),
    ('Asalariado', 'Con obligacion del patrono');

-- DROP TABLE IF EXISTS tbl_contributor;
INSERT INTO sat.tbl_contributor (nit, first_name, last_name, cui, date_of_birth, nationality, email, regime_id)
VALUES 
    ('12345678', 'John', 'Doe', '1234567890123', '1990-01-13', 'American', 'john.doe@example.com', 1),
    ('87654321', 'Jane', 'Smith', '9876543210987', '1985-05-20', 'British', 'jane.smith@example.com', 2),
    ('11111111', 'Michael', 'Johnson', '4567890123456', '1995-09-02', 'Canadian', 'alice.johnson@example.com', 3),
    ('33333333', 'Emily', 'Brown', '7890123456789', '1988-12-10', 'German', 'emily.brown@example.com', 2),
    ('22222222', 'William', 'Martinez', '6543210987654', '1992-03-15', 'Australian', 'bob.williams@example.com', 1),
    ('55555555', 'Sophia', 'Garcia', '3210987654321', '1998-07-25', 'Italian', 'sophia.davis@example.com', 1),
    ('88888888', 'James', 'Rodriguez', '2109876543210', '1991-11-30', 'Mexican', 'matthew.taylor@example.com', 1),
    ('66666666', 'Olivia', 'Lopez', '5432109876543', '1996-04-05', 'Spanish', 'daniel.miller@example.com', 2),
    ('77777777', 'Benjamin', 'Perez', '8765432109876', '1987-08-18', 'Brazilian', 'olivia.wilson@example.com', 3),
    ('44444444', 'Isabella', 'Gonzalez', '2345678901234', '1994-06-22', 'French', 'michael.jones@example.com', 3),
    ('77777733', 'Jane', 'Smith', '9876543210912', '1985-05-20', 'Brazilian', 'olivia.wilson@example.com', 3);
    
-- insert data for tbl_statement according to contributor id 2, regime 2 and statement_type SAT-2046
WITH RECURSIVE months AS (
    SELECT 
        DATE_TRUNC('month', CURRENT_DATE) AS month
    UNION ALL
    SELECT 
        month - INTERVAL '1 month'
    FROM 
        months
    WHERE 
        month > CURRENT_DATE - INTERVAL '15 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR contributor with id 87654321
INSERT INTO sat.tbl_statement (statement_type, issue_date, statement_overdue, statement_month, statement_year, statement_amount, regime_id, contributor_id)
SELECT 
    'SAT-2046', 
    month,
    (SELECT CASE
        WHEN (FLOOR(random() * 1)) = 1 THEN TRUE
        WHEN (FLOOR(random() * 0)) = 0 THEN FALSE
    END), 
    -- CAST(random() * 1 AS INTEGER), --false, 
    EXTRACT(MONTH FROM month)::integer, 
    EXTRACT(YEAR FROM month)::integer, 
    random() * 10000, -- Generate a random number from 0 to 10000
    2, 
    2
FROM
    months
ORDER BY
    month;

-- generate a random number from 0 to 10000 in sql
-- SELECT random() * 10000;


-- insert data for tbl_statement according to contributor id 2, regime 2 and statement_type SAT-2046
DO $$DECLARE
	ccontributor RECORD;
BEGIN

FOR ccontributor IN (SELECT statement_id, statement_amount, statement_month, statement_year,issue_date FROM sat.tbl_statement WHERE contributor_id = 2) LOOP
    INSERT INTO sat.tbl_payment (payment_date, payment_amount, statement_id)
	VALUES (
	(MAKE_DATE(ccontributor.statement_year, ccontributor.statement_month, 10)
	),ccontributor.statement_amount,ccontributor.statement_id);
END LOOP;

END$$;

-- insert data for tbl_statement according to contributor id 4, regime 2 and statement_type SAT-2046
WITH RECURSIVE months AS (
    SELECT 
        DATE_TRUNC('month', CURRENT_DATE) AS month
    UNION ALL
    SELECT 
        month - INTERVAL '1 month'
    FROM 
        months
    WHERE 
        month > CURRENT_DATE - INTERVAL '10 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR contributor with id 87654321
INSERT INTO sat.tbl_statement (statement_type, issue_date, statement_overdue, statement_month, statement_year, statement_amount, regime_id, contributor_id)
SELECT 
    'SAT-2046', 
    month,
    (SELECT CASE
        WHEN (FLOOR(random() * 0)) = 0 THEN TRUE
        WHEN (FLOOR(random() * 0)) = 0 THEN FALSE
    END), 
    -- CAST(random() * 1 AS INTEGER), --false, 
    EXTRACT(MONTH FROM month)::integer, 
    EXTRACT(YEAR FROM month)::integer, 
    random() * 1000, -- Generate a random number from 0 to 10000
    2, 
    4
FROM
    months
ORDER BY
    month;

-- insert data for tbl_statement according to contributor id 4, regime 2 and statement_type SAT-2046
DO $$DECLARE
	ccontributor RECORD;
BEGIN

FOR ccontributor IN (SELECT statement_id, statement_amount, statement_month, statement_year,issue_date FROM sat.tbl_statement WHERE contributor_id = 4) LOOP
    INSERT INTO sat.tbl_payment (payment_date, payment_amount, statement_id)
	VALUES (
        (CASE
            WHEN ccontributor.statement_month = 12 THEN (MAKE_DATE(ccontributor.statement_year, ccontributor.statement_month, CAST(random()*(28-1) + 1 AS INTEGER)))
            ELSE (MAKE_DATE(ccontributor.statement_year,CAST(random()*(1-0)+0 AS INTEGER) + ccontributor.statement_month, CAST(random()*(28-1) + 1 AS INTEGER)))
        END),
    ccontributor.statement_amount,ccontributor.statement_id);
END LOOP;

END$$;