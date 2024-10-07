SELECT * FROM banco_union.tbl_installment;

SELECT * FROM banco_union.tbl_customer
RIGHT JOIN banco_union.tbl_loan
ON tbl_customer.id = tbl_loan.customer_id;


SELECT * FROM banco_union.tbl_installment
WHERE installment_overdue=true;


SELECT * FROM banco_union.tbl_installment
WHERE loan_id=3;
