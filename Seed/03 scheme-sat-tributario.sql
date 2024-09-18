-- Database: sat

-- DROP DATABASE IF EXISTS sat;

CREATE DATABASE IF NOT EXISTS sat
    WITH
    OWNER = operativosa
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

-- Table: public.regime
CREATE TABLE IF NOT EXISTS sat.tbl_regime(
    id serial NOT NULL,
    name varchar(100) NOT NULL,
    description varchar(255) NOT NULL,
    CONSTRAINT regime_pkey PRIMARY KEY (id)
);

-- Creacion de tabla unica de contribuyentes

CREATE TABLE IF NOT EXISTS sat.tbl_contributor(
    id serial NOT NULL,
    nit varchar(8) NOT NULL,
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    cui varchar(13) NOT NULL,
    date_of_birth date NOT NULL,
    nationality varchar(50) NOT NULL,
    email varchar(50) NOT NULL,
    regime_id integer NOT NULL,
    CONSTRAINT contributor_pkey PRIMARY KEY (id),
    CONSTRAINT contributor_regime_id_fkey FOREIGN KEY (regime_id)
        REFERENCES sat.tbl_regime (id)
        ON UPDATE SET NULL
        ON DELETE SET NULL,
	CONSTRAINT cui_unique UNIQUE(cui),
	CONSTRAINT nit_unique UNIQUE(nit)
);

-- creacion de tabal de declaraciones

CREATE TABLE IF NOT EXISTS sat.tbl_statement(
    statement_id serial NOT NULL,
    statement_type varchar(10) NOT NULL,
    issue_date date NOT NULL,
    statement_overdue boolean NOT NULL default false,
    statement_month integer NOT NULL,
    statement_year integer NOT NULL,
    statement_amount numeric(10,2) NOT NULL,
    regime_id integer NOT NULL,
    contributor_id integer NOT NULL,
    CONSTRAINT statement_pkey PRIMARY KEY (statement_id),
    CONSTRAINT statement_regime_id_fkey FOREIGN KEY (regime_id)
        REFERENCES sat.tbl_regime (id)
        ON UPDATE SET NULL
        ON DELETE SET NULL,
    CONSTRAINT statement_contributor_id_fkey FOREIGN KEY (contributor_id)
        REFERENCES sat.tbl_contributor (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS sat.tbl_payment(
    payment_id serial NOT NULL,
    payment_date date NOT NULL,
    payment_amount numeric(10,2) NOT NULL,
    statement_id integer NOT NULL,
    CONSTRAINT payment_pkey PRIMARY KEY (payment_id),
    CONSTRAINT payment_statement_id_fkey FOREIGN KEY (statement_id)
        REFERENCES sat.tbl_statement (statement_id) 
        ON UPDATE CASCADE
        ON DELETE SET NULL
);


-- tabla de facturas emitidas a nombre del contribuyente
CREATE TABLE IF NOT EXISTS sat.tbl_imposition(
    id serial NOT NULL,
    payment_date date NOT NULL,
    payment_amount numeric(10,2) NOT NULL,
    contributor_id integer NOT NULL,
    CONSTRAINT imposition_pkey PRIMARY KEY (id),
    CONSTRAINT imposition_contributor_id_fkey FOREIGN KEY (contributor_id)
        REFERENCES sat.tbl_contributor (id)
        ON UPDATE CASCADE
        ON DELETE SET NULL
);