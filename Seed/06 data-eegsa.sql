
-- insert data into customer table

INSERT INTO TBL_CUSTOMER (cui, first_name, last_name, date_of_birth)
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

-- insert data into contract table

INSERT INTO TBL_CONTRACT (customer_id, is_active)
VALUES 
(1,true),
(1, false),
(1, false),
(2,true),
(3,true),
(4,true),
(4,true),
(4,true),
(5,true),
(6,true),
(7,true),
(8,true),
(8, false),
(9,true),
(10,true),
(10,true),
(11,true),
(11,true),
(11,true),
(11,true);

-- insert data into bill table for customer 1 contract 1

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
INSERT INTO TBL_BILL (bill_type, issue_date, due_date, bill_overdue, days_overdue, contract_id, bill_amount)
SELECT 
    'Electricity', 
    month,
    month + INTERVAL '1 month',
    (SELECT CASE
        WHEN (FLOOR(random() * 1)) = 1 THEN TRUE
        ELSE TRUE
    END), 
    -- generate days overdue
    (SELECT CASE
        WHEN (FLOOR(random() * 1)) = 1 THEN FLOOR(random() * 30)
        WHEN (FLOOR(random() * 0)) = 0 THEN 0
    END),
    3,
    random() * 10000
     -- Generate a random number from 0 to 10000
FROM
    months
ORDER BY
    month;

-- insert data into payment for bills for customer 1 contract 1

DO $$DECLARE
    cbill RECORD;
BEGIN

FOR cbill IN (SELECT id, bill_amount, bill_overdue, issue_date, due_date FROM tbl_bill WHERE contract_id = 1) LOOP
    INSERT INTO tbl_payment (payment_date, payment_amount, bill_id)
    VALUES (
    (CASE
    -- cuando es diciembre, y la factura esta vencida
            WHEN EXTRACT(MONTH FROM cbill.issue_date)::integer = 12 AND cbill.bill_overdue THEN 
            (MAKE_DATE -- returns date with the given year, month, and day
                (
                EXTRACT(YEAR FROM cbill.due_date)::integer,
                1,
                -- EXTRACT(MONTH FROM cbill.due_date)::integer,
                EXTRACT(DAY FROM cbill.due_date)::integer
                )
            )
            -- cuando es diciembre y la factura no esta vencida y el pago es antes de un 10 Y LUEGO DE UN 1
            WHEN EXTRACT(MONTH FROM cbill.issue_date)::integer = 12 AND cbill.bill_overdue = false THEN 
            (MAKE_DATE
                (
                EXTRACT(YEAR FROM cbill.due_date)::integer,
                EXTRACT(MONTH FROM cbill.due_date)::integer,
                    (
                    CASE WHEN EXTRACT(DAY FROM cbill.due_date)::integer < 10 AND EXTRACT(DAY FROM cbill.due_date)::integer > 6 THEN
                        (EXTRACT(DAY FROM cbill.due_date)::integer) - 5
                    ELSE
                       (EXTRACT(DAY FROM cbill.due_date)::integer)
                    END
                    )
                )
            )
            -- cuando una factura es vencida, y el mes no es diciembre, agrega un mes asi nunca llega a mes 13
            WHEN EXTRACT(MONTH FROM cbill.issue_date)::integer <> 12 AND cbill.bill_overdue THEN 
            (MAKE_DATE
                (
                EXTRACT(YEAR FROM cbill.due_date)::integer,
                1 + EXTRACT(MONTH FROM cbill.due_date)::integer, 
                CAST(random()*(28-1) + 1 AS INTEGER)
                )
            )
            ELSE (
                MAKE_DATE(
                    EXTRACT(YEAR FROM cbill.due_date)::integer,
                    EXTRACT(MONTH FROM cbill.due_date)::integer, 
                    EXTRACT(DAY FROM cbill.due_date)::integer
                )
            )
        END),
        cbill.bill_amount,
        cbill.id);
END LOOP;

END$$;



