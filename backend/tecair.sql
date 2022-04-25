

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;


CREATE DATABASE tecair WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'es_CR.UTF-8';


\connect tecair

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;


CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;



CREATE TYPE public.flight_state AS ENUM (
    'booking',
    'checkin',
    'closed'
);



CREATE TYPE public.user_type AS ENUM (
    'pax',
    'manager'
);


SET default_tablespace = '';

SET default_table_access_method = heap;


CREATE TABLE public.aircraft (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(32) NOT NULL,
    seats integer NOT NULL
);



CREATE TABLE public.airports (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(8) NOT NULL,
    comment text
);



CREATE TABLE public.bags (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    owner uuid NOT NULL,
    flight uuid NOT NULL,
    no integer NOT NULL,
    weight numeric NOT NULL,
    color character(7) NOT NULL
);



CREATE SEQUENCE public.bags_no_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



ALTER SEQUENCE public.bags_no_seq OWNED BY public.bags.no;



CREATE TABLE public.bookings (
    flight uuid NOT NULL,
    pax uuid NOT NULL,
    promo uuid
);



CREATE TABLE public.checkins (
    segment uuid NOT NULL,
    pax uuid NOT NULL,
    seat integer NOT NULL
);



CREATE TABLE public.flights (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    no integer NOT NULL,
    state public.flight_state NOT NULL,
    comment text,
    price numeric NOT NULL
);



CREATE TABLE public.promos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(16) NOT NULL,
    flight uuid NOT NULL,
    price numeric NOT NULL,
    start_time timestamp with time zone NOT NULL,
    end_time timestamp with time zone NOT NULL,
    img bytea
);



CREATE TABLE public.segments (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    flight uuid NOT NULL,
    seq_no integer NOT NULL,
    from_loc uuid NOT NULL,
    from_time time with time zone NOT NULL,
    to_loc uuid NOT NULL,
    to_time time with time zone NOT NULL,
    aircraft uuid NOT NULL
);



CREATE TABLE public.users (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    username character varying(64) NOT NULL,
    hash character(32),
    salt character(32),
    first_name character varying(256) NOT NULL,
    last_name character varying(256),
    phonenumber character varying(32),
    email character varying(256),
    university character varying(256),
    student_id character varying(64),
    type public.user_type NOT NULL,
    miles numeric DEFAULT 0
);



ALTER TABLE ONLY public.bags ALTER COLUMN no SET DEFAULT nextval('public.bags_no_seq'::regclass);



ALTER TABLE ONLY public.aircraft
    ADD CONSTRAINT aircraft_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.airports
    ADD CONSTRAINT airports_code_key UNIQUE (code);



ALTER TABLE ONLY public.airports
    ADD CONSTRAINT airports_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_no_key UNIQUE (no);



ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_flight_pax_key UNIQUE (flight, pax);



ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_pax_key UNIQUE (segment, pax);



ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_seat_key UNIQUE (segment, seat);



ALTER TABLE ONLY public.flights
    ADD CONSTRAINT flights_no_key UNIQUE (no);



ALTER TABLE ONLY public.flights
    ADD CONSTRAINT flights_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_code_key UNIQUE (code);



ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_flight_seq_no_key UNIQUE (flight, seq_no);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);



ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);



CREATE INDEX ix_bags_flight ON public.bags USING btree (flight);



CREATE INDEX ix_bags_owner_flight ON public.bags USING btree (owner, flight);



ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);



ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_owner_fkey FOREIGN KEY (owner) REFERENCES public.users(id);



ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);



ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_pax_fkey FOREIGN KEY (pax) REFERENCES public.users(id);



ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_promo_fkey FOREIGN KEY (promo) REFERENCES public.promos(id);



ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_pax_fkey FOREIGN KEY (pax) REFERENCES public.users(id);



ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_fkey FOREIGN KEY (segment) REFERENCES public.segments(id);



ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_aircraft_fkey FOREIGN KEY (aircraft) REFERENCES public.aircraft(id);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_from_loc_fkey FOREIGN KEY (from_loc) REFERENCES public.airports(id);



ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_to_loc_fkey FOREIGN KEY (to_loc) REFERENCES public.airports(id);



