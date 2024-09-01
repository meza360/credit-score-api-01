-- postgresql://operativosa:operativosa@localhost:5432/eegsa

CREATE DATABASE eegsa
    WITH
    OWNER = operativosa
    ENCODING = 'UTF8'
    LOCALE_PROVIDER = 'libc'
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

GRANT CONNECT, CREATE ON DATABASE eegsa TO "PROJG2";


-- create customer table

CREATE TABLE IF NOT EXISTS TBL_CUSTOMER
(
    id serial NOT NULL,
    cui character varying(13) COLLATE pg_catalog."default" NOT NULL,
    first_name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    last_name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    date_of_birth date NOT NULL,
    CONSTRAINT tbl_customer_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS TBL_CONTRACT
(
    id serial NOT NULL,
    customer_id integer NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT tbl_contract_pkey PRIMARY KEY (id),
    CONSTRAINT tbl_contract_customer_id_fkey FOREIGN KEY (customer_id)
        REFERENCES TBL_CUSTOMER (id)
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

CREATE TABLE IF NOT EXISTS TBL_BILL
(
    id serial NOT NULL,
    bill_type character varying(100) COLLATE pg_catalog."default" NOT NULL,
    issue_date date NOT NULL,
    due_date date NOT NULL,
    bill_overdue boolean NOT NULL DEFAULT false,
    days_overdue integer NOT NULL DEFAULT 0,
    contract_id integer NOT NULL,
    bill_amount numeric(10,2) NOT NULL,
    CONSTRAINT tbl_bill_pkey PRIMARY KEY (id),
    CONSTRAINT tbl_bill_contract_id_fkey FOREIGN KEY (contract_id)
        REFERENCES TBL_CONTRACT (id)
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);

CREATE TABLE IF NOT EXISTS TBL_PAYMENT
(
    id serial NOT NULL,
    payment_date date NOT NULL,
    payment_amount numeric(10,2) NOT NULL,
    bill_id integer NOT NULL,
    CONSTRAINT tbl_payment_pkey PRIMARY KEY (id),
    CONSTRAINT tbl_payment_bill_id_fkey FOREIGN KEY (id)
        REFERENCES TBL_BILL (id)
        ON UPDATE CASCADE
        ON DELETE NO ACTION
);
