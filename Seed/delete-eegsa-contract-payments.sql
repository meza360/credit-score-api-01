select * from eegsa.tbl_customer;

select * from eegsa.tbl_contract;

select * from eegsa.tbl_bill;

select * from eegsa.tbl_customer where cui = '1234567890123';
SELECT * FROM eegsa.tbl_contract WHERE customer_id = 1;


select distinct contract_id from eegsa.tbl_bill;

SELECT tc.id AS contract_id,tc.customer_id, SUM(tb.bill_amount)
FROM eegsa.tbl_contract tc
LEFT JOIN eegsa.tbl_bill tb
ON tc.id = tb.contract_id
WHERE tc.id IN (1,2,3) and tb.bill_overdue=true
GROUP BY tc.id, tc.customer_id;

SELECT SUM(tb.bill_amount)
FROM eegsa.tbl_bill tb
WHERE tb.contract_id IN (1,2,3);