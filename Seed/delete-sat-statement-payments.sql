-- nit 87654321
SELECT * FROM sat.tbl_statement
WHERE contributor_id = 2;

SELECT * FROM sat.tbl_statement stat
LEFT JOIN sat.tbl_payment pay
ON stat.statement_id = pay.statement_id;

SELECT * FROM sat.tbl_statement stat
LEFT JOIN sat.tbl_payment pay
ON stat.statement_id = pay.statement_id
WHERE stat.contributor_id = 2;


-- suma 7523.73
DELETE 
FROM sat.tbl_payment
WHERE payment_id = 180;

DELETE 
FROM sat.tbl_payment
WHERE payment_id = 181;

UPDATE sat.tbl_statement
SET statement_overdue = true
WHERE statement_id = 180;

UPDATE sat.tbl_statement
SET statement_overdue = true
WHERE statement_id = 181;