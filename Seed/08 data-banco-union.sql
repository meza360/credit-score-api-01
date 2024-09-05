
-- Insercion de clientes

INSERT INTO banco_union.tbl_customer (cui, first_name, last_name, date_of_birth)
VALUES 
('1234567890123', 'John', 'Doe', '1990-01-13'),
('9876543210987', 'Jane', 'Smith', '1985-05-20'),
('4567890123456', 'Michael', 'Johnson', '1995-09-02'),
('7890123456789', 'Emily', 'Brown', '1988-12-10'),
('6543210987654', 'William', 'Martinez', '1992-03-15'),
('3210987654321', 'Sophia', 'Garcia', '1998-07-25'),
('2109876543210', 'James', 'Rodriguez', '1991-11-30'),
('5432109876543', 'Olivia', 'Lopez', '1996-04-05'),
('8765432109876', 'Benjamin', 'Perez', '1987-08-18'),
('2345678901234', 'Isabella', 'Gonzalez', '1994-06-22'),
('9876543210912', 'Jane', 'Smith', '1985-05-20');


-- insercion  de prestamos

INSERT INTO banco_union.tbl_loan (total_value, installments, issue_date, customer_id)
VALUES
-- cliente 1
(random() * 10000, 12, '2015-01-01', 1),
(random() * 10000, 10, '2018-01-01', 1),
(random() * 10000, 12, '2022-01-01', 1),
--cliente 2
(random() * 10000, 12, '2016-01-01', 2),
(random() * 10000, 24, '2020-01-01', 2),
(random() * 10000, 12, '2022-01-01', 2),
-- cliente 3
(random() * 10000, 12, '2017-01-01', 3),
-- cliente 4 no tiene prestamos
-- cliente 5
(random() * 10000, 10, '2018-01-01', 5),
(random() * 10000, 48, '2029-01-01', 5),
-- cliente 6
(random() * 10000, 6, '2019-01-01', 6);

-- insercion de cuotas
-- SELECT total_value, installments 
-- FROM banco_union.tbl_loan
-- WHERE banco_union.tbl_loan.customer_id = 1;

-- Divide todas las cuotas en los meses indicados
-- cliente 1 todos los prestamos

DO $$DECLARE
    cloan RECORD;
BEGIN
    FOR cloan IN (
                    SELECT id, total_value, installments, issue_date 
                    FROM banco_union.tbl_loan
                    WHERE customer_id = 1
                )
    LOOP
        INSERT INTO banco_union.tbl_installment (due_date, amount, loan_id)
        VALUES
       (
        generate_series(cloan.issue_date, cloan.issue_date + interval '1 year' - interval '1 day', interval '1 month'), -- genera la cantidad de valores necesarios
        (cloan.total_value / cloan.installments),
        cloan.id);
    END LOOP;
END$$;

-- Divide todas las cuotas en los meses indicados
-- cliente 2 todos los prestamos

DO $$DECLARE
    cloan RECORD;
BEGIN
    FOR cloan IN (
                    SELECT id, total_value, installments, issue_date 
                    FROM banco_union.tbl_loan
                    WHERE customer_id = 2
                )
    LOOP
        INSERT INTO banco_union.tbl_installment (due_date, amount, loan_id)
        VALUES
       (
        generate_series(cloan.issue_date, cloan.issue_date + interval '1 year' - interval '1 day', interval '1 month'), -- genera la cantidad de valores necesarios
        (cloan.total_value / cloan.installments),
        cloan.id);
    END LOOP;
END$$;

-- Divide todas las cuotas en los meses indicados
-- cliente 3 todos los prestamos

DO $$DECLARE
    cloan RECORD;
BEGIN
    FOR cloan IN (
                    SELECT id, total_value, installments, issue_date 
                    FROM banco_union.tbl_loan
                    WHERE customer_id = 3
                )
    LOOP
        INSERT INTO banco_union.tbl_installment (due_date, amount, loan_id)
        VALUES
       (
        generate_series(cloan.issue_date, cloan.issue_date + interval '1 year' - interval '1 day', interval '1 month'), -- genera la cantidad de valores necesarios
        (cloan.total_value / cloan.installments),
        cloan.id);
    END LOOP;
END$$;

-- Divide todas las cuotas en los meses indicados
-- cliente 5 todos los prestamos

DO $$DECLARE
    cloan RECORD;
BEGIN
    FOR cloan IN (
                    SELECT id, total_value, installments, issue_date 
                    FROM banco_union.tbl_loan
                    WHERE customer_id = 5
                )
    LOOP
        INSERT INTO banco_union.tbl_installment (due_date, amount, loan_id)
        VALUES
       (
        generate_series(cloan.issue_date, cloan.issue_date + interval '1 year' - interval '1 day', interval '1 month'), -- genera la cantidad de valores necesarios
        (cloan.total_value / cloan.installments),
        cloan.id);
    END LOOP;
END$$;
-- Divide todas las cuotas en los meses indicados
-- cliente 6 todos los prestamos

DO $$DECLARE
    cloan RECORD;
BEGIN
    FOR cloan IN (
                    SELECT id, total_value, installments, issue_date 
                    FROM banco_union.tbl_loan
                    WHERE customer_id = 6
                )
    LOOP
        INSERT INTO banco_union.tbl_installment (due_date, amount, loan_id)
        VALUES
       (
        generate_series(cloan.issue_date, cloan.issue_date + interval '1 year' - interval '1 day', interval '1 month'), -- genera la cantidad de valores necesarios
        (cloan.total_value / cloan.installments),
        cloan.id);
    END LOOP;
END$$;


-- insercion de creditos
-- cliente 1, 1 credito

INSERT INTO banco_union.tbl_credit (max_allowed, customer_id)
VALUES
(random() * 10000, 1);

-- cliente 2, 1 credito

INSERT INTO banco_union.tbl_credit (max_allowed, customer_id)
VALUES
(random() * 10000, 2);

-- cliente 3, 3 creditos
INSERT INTO banco_union.tbl_credit (max_allowed, customer_id)
VALUES
(random() * 10000, 3),
(random() * 10000, 3),
(random() * 10000, 3);


-- cliente 4, 5 creditos
INSERT INTO banco_union.tbl_credit (max_allowed, customer_id)
VALUES
(random() * 10000, 4),
(random() * 10000, 4),
(random() * 10000, 4),
(random() * 10000, 4),
(random() * 10000, 4);

-- cliente 5, no tiene creditos

--cliente 6, no creditos

-- insercion de estados de cuenta
-- cliente 1, credito 1, 65 estados de cuenta

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
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER, 1+EXTRACT(MONTH FROM month)::INTEGER, 1),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=1)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    1
FROM
    months
ORDER BY
    month;

-- cliente 2, credito 1, 48 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '4 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER, 1+EXTRACT(MONTH FROM month)::INTEGER, 1),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=2)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    2
FROM
    months
ORDER BY
    month;

-- cliente 3, credito 1, 48 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '4 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=3 AND id=3)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    3
FROM
    months
ORDER BY
    month;

-- cliente 3, credito 2, 12 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '1 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=3 AND id=4)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    4
FROM
    months
ORDER BY
    month;
-- cliente 3, credito 3, 6 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '0.5 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=3 AND id=5)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    5
FROM
    months
ORDER BY
    month;


-- cliente 4, credito 1, 6 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '0.5 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=4 AND id=6)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    6
FROM
    months
ORDER BY
    month;


-- cliente 4, credito 2, 34 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '2.8 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=4 AND id=7)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    7
FROM
    months
ORDER BY
    month;

-- cliente 4, credito 3, 72 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '6 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=4 AND id=8)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    8
FROM
    months
ORDER BY
    month;

-- cliente 4, credito 4, 24 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '2 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=4 AND id=9)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    9
FROM
    months
ORDER BY
    month;

-- cliente 4, credito 5, 60 estados de cuenta
WITH RECURSIVE months AS (
SELECT 
    DATE_TRUNC('month', CURRENT_DATE) AS month
UNION ALL
SELECT 
    month - INTERVAL '1 month'
FROM 
    months
WHERE 
    month > CURRENT_DATE - INTERVAL '5 years'
)
-- WITH EACH MONTH GENERATE A ROW FOR bills for customer 1 contract 1
INSERT INTO banco_union.tbl_statement (due_date, statement_amount, statement_month, statement_year, credit_id)
SELECT 
    (SELECT
        CASE
            WHEN EXTRACT(MONTH FROM month)::INTEGER = 12 THEN MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER+1, 1, 1)
            ELSE MAKE_DATE(EXTRACT(YEAR FROM month)::INTEGER,1+EXTRACT(MONTH FROM month)::INTEGER, 1)
        END
    ),
    (random() * (SELECT max_allowed from banco_union.tbl_credit where customer_id=4 AND id=10)),
    EXTRACT(MONTH FROM month),
    EXTRACT(YEAR FROM month),
    10
FROM
    months
ORDER BY
    month;

