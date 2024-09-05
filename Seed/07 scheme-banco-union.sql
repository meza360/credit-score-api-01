-- Database: banco-union

-- DROP DATABASE IF EXISTS sat;

CREATE DATABASE IF NOT EXISTS banco_union
    WITH
    OWNER = operativosa
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

-- create scheme if not exists banco-nuion
CREATE SCHEMA IF NOT EXISTS banco_union;


-- Table: banco-union.tbl_customer

CREATE TABLE IF NOT EXISTS banco_union.tbl_customer(
    id serial NOT NULL,
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    cui varchar(13) NOT NULL,
    date_of_birth date NOT NULL,
    CONSTRAINT pk_customer PRIMARY KEY (id)
);

-- Table: banco-union.tbl_loan

CREATE TABLE IF NOT EXISTS banco_union.tbl_loan(
    id serial NOT NULL,
    total_value numeric(10,2) NOT NULL,
    installments integer NOT NULL,
    issue_date date NOT NULL,
    customer_id integer NOT NULL,
    CONSTRAINT pk_loan PRIMARY KEY (id),
    CONSTRAINT fk_customer FOREIGN KEY (customer_id)
        REFERENCES banco_union.tbl_customer (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);

--Table: tbl_installment

CREATE TABLE IF NOT EXISTS banco_union.tbl_installment(
    id serial NOT NULL,
    due_date date NOT NULL,
    installment_overdue boolean NOT NULL default false,
    days_overdue integer default NULL,
    amount numeric(10,2) NOT NULL,
    loan_id integer NOT NULL,
    CONSTRAINT pk_installments PRIMARY KEY (id),
    CONSTRAINT fk_loan FOREIGN KEY (loan_id)
        REFERENCES banco_union.tbl_loan (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);

-- Table: banco-union.tbl_credit
CREATE TABLE IF NOT EXISTS banco_union.tbl_credit(
    id serial NOT NULL,
    max_allowed numeric(10,2) NOT NULL,
    customer_id integer NOT NULL,
    CONSTRAINT pk_credit PRIMARY KEY (id),
    CONSTRAINT fk_customer FOREIGN KEY (customer_id)
        REFERENCES banco_union.tbl_customer (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);

-- Table: banco-union.tbl_statement
CREATE TABLE IF NOT EXISTS banco_union.tbl_statement(
    id serial NOT NULL,
    due_date date NOT NULL,
    statement_overdue boolean NOT NULL default false,
    days_overdue integer default NULL,
    statement_amount numeric(10,2) NOT NULL,
    statement_month integer NOT NULL,
    statement_year integer NOT NULL,
    credit_id integer NOT NULL,
    CONSTRAINT pk_statement PRIMARY KEY (id),
    CONSTRAINT fk_credit FOREIGN KEY (credit_id)
        REFERENCES banco_union.tbl_credit (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);


-- Table: banco-union.tbl_payment

CREATE TABLE IF NOT EXISTS banco_union.tbl_payment(
    id serial NOT NULL,
    payment_date date NOT NULL,
    payment_amount numeric(10,2) NOT NULL,
    installment_id integer,
    statement_id integer,
    CONSTRAINT pk_payment PRIMARY KEY (id),
    CONSTRAINT fk_installment FOREIGN KEY (installment_id)
        REFERENCES banco_union.tbl_installment (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL,
    CONSTRAINT fk_statement FOREIGN KEY (statement_id)
        REFERENCES banco_union.tbl_statement (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);