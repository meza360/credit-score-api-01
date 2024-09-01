-- Database: renap

-- DROP DATABASE IF EXISTS renap;

CREATE DATABASE IF NOT EXISTS renap
    WITH
    OWNER = operativosa
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;

-- Creacion de tabla unica de personas

CREATE TABLE IF NOT EXISTS CITIZEN
(
    id serial NOT NULL,
    cui character varying(13) COLLATE pg_catalog."default" NOT NULL,
    first_name character varying(70) COLLATE pg_catalog."default" NOT NULL,
    last_name character varying(70) COLLATE pg_catalog."default" NOT NULL,
    date_of_birth date NOT NULL,
    date_of_decease date DEFAULT NULL,
    nationality character varying(50) COLLATE pg_catalog."default" DEFAULT "Guatemalteco/a" NOT NULL,
    CONSTRAINT citizen_pkey PRIMARY KEY (id),
    CONSTRAINT cui_unique UNIQUE (cui)
);

