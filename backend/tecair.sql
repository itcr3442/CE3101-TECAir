--
-- PostgreSQL database dump
--

-- Dumped from database version 14.2
-- Dumped by pg_dump version 14.2

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

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- Name: flight_state; Type: TYPE; Schema: public; Owner: ale
--

CREATE TYPE public.flight_state AS ENUM (
    'booking',
    'checkin',
    'closed'
);



--
-- Name: user_type; Type: TYPE; Schema: public; Owner: ale
--

CREATE TYPE public.user_type AS ENUM (
    'pax',
    'manager'
);

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: aircraft; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.aircraft (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(32) NOT NULL,
    seats integer NOT NULL
);



--
-- Name: airports; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.airports (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(8) NOT NULL,
    comment text
);



--
-- Name: bags; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.bags (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    owner uuid NOT NULL,
    flight uuid NOT NULL,
    no integer NOT NULL,
    weight numeric NOT NULL,
    color character(7) NOT NULL
);



--
-- Name: bags_no_seq; Type: SEQUENCE; Schema: public; Owner: ale
--

CREATE SEQUENCE public.bags_no_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: bags_no_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: ale
--

ALTER SEQUENCE public.bags_no_seq OWNED BY public.bags.no;


--
-- Name: bookings; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.bookings (
    flight uuid NOT NULL,
    pax uuid NOT NULL,
    promo uuid
);



--
-- Name: checkins; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.checkins (
    segment uuid NOT NULL,
    pax uuid NOT NULL,
    seat integer NOT NULL
);



--
-- Name: endpoints; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.endpoints (
    flight uuid NOT NULL,
    from_loc uuid NOT NULL,
    to_loc uuid NOT NULL
);



--
-- Name: flights; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.flights (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    no integer NOT NULL,
    state public.flight_state NOT NULL,
    comment text,
    price numeric NOT NULL
);



--
-- Name: promos; Type: TABLE; Schema: public; Owner: ale
--

CREATE TABLE public.promos (
    id uuid DEFAULT public.uuid_generate_v4() NOT NULL,
    code character varying(16) NOT NULL,
    flight uuid NOT NULL,
    price numeric NOT NULL,
    start_time timestamp with time zone NOT NULL,
    end_time timestamp with time zone NOT NULL,
    img bytea
);



--
-- Name: segments; Type: TABLE; Schema: public; Owner: ale
--

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



--
-- Name: users; Type: TABLE; Schema: public; Owner: ale
--

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
    type public.user_type NOT NULL
);



--
-- Name: bags no; Type: DEFAULT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bags ALTER COLUMN no SET DEFAULT nextval('public.bags_no_seq'::regclass);


--
-- Data for Name: aircraft; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.aircraft (id, code, seats) FROM stdin;
\.


--
-- Data for Name: airports; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.airports (id, code, comment) FROM stdin;
\.


--
-- Data for Name: bags; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.bags (id, owner, flight, no, weight, color) FROM stdin;
\.


--
-- Data for Name: bookings; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.bookings (flight, pax, promo) FROM stdin;
\.


--
-- Data for Name: checkins; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.checkins (segment, pax, seat) FROM stdin;
\.


--
-- Data for Name: endpoints; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.endpoints (flight, from_loc, to_loc) FROM stdin;
\.


--
-- Data for Name: flights; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.flights (id, no, state, comment, price) FROM stdin;
\.


--
-- Data for Name: promos; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.promos (id, code, flight, price, start_time, end_time, img) FROM stdin;
\.


--
-- Data for Name: segments; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) FROM stdin;
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: ale
--

COPY public.users (id, username, hash, salt, first_name, last_name, phonenumber, email, university, student_id, type) FROM stdin;
\.


--
-- Name: bags_no_seq; Type: SEQUENCE SET; Schema: public; Owner: ale
--

SELECT pg_catalog.setval('public.bags_no_seq', 1, false);


--
-- Name: aircraft aircraft_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.aircraft
    ADD CONSTRAINT aircraft_pkey PRIMARY KEY (id);


--
-- Name: airports airports_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.airports
    ADD CONSTRAINT airports_pkey PRIMARY KEY (id);


--
-- Name: bags bags_no_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_no_key UNIQUE (no);


--
-- Name: bags bags_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_pkey PRIMARY KEY (id);


--
-- Name: bookings bookings_flight_pax_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_flight_pax_key UNIQUE (flight, pax);


--
-- Name: checkins checkins_segment_pax_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_pax_key UNIQUE (segment, pax);


--
-- Name: checkins checkins_segment_seat_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_seat_key UNIQUE (segment, seat);


--
-- Name: endpoints endpoints_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.endpoints
    ADD CONSTRAINT endpoints_pkey PRIMARY KEY (flight);


--
-- Name: flights flights_no_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.flights
    ADD CONSTRAINT flights_no_key UNIQUE (no);


--
-- Name: flights flights_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.flights
    ADD CONSTRAINT flights_pkey PRIMARY KEY (id);


--
-- Name: promos promos_code_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_code_key UNIQUE (code);


--
-- Name: promos promos_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_pkey PRIMARY KEY (id);


--
-- Name: segments segments_flight_seq_no_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_flight_seq_no_key UNIQUE (flight, seq_no);


--
-- Name: segments segments_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_pkey PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- Name: ix_bags_flight; Type: INDEX; Schema: public; Owner: ale
--

CREATE INDEX ix_bags_flight ON public.bags USING btree (flight);


--
-- Name: ix_bags_owner_flight; Type: INDEX; Schema: public; Owner: ale
--

CREATE INDEX ix_bags_owner_flight ON public.bags USING btree (owner, flight);


--
-- Name: bags bags_flight_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);


--
-- Name: bags bags_owner_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bags
    ADD CONSTRAINT bags_owner_fkey FOREIGN KEY (owner) REFERENCES public.users(id);


--
-- Name: bookings bookings_flight_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);


--
-- Name: bookings bookings_pax_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_pax_fkey FOREIGN KEY (pax) REFERENCES public.users(id);


--
-- Name: bookings bookings_promo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_promo_fkey FOREIGN KEY (promo) REFERENCES public.promos(id);


--
-- Name: checkins checkins_pax_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_pax_fkey FOREIGN KEY (pax) REFERENCES public.users(id);


--
-- Name: checkins checkins_segment_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.checkins
    ADD CONSTRAINT checkins_segment_fkey FOREIGN KEY (segment) REFERENCES public.segments(id);


--
-- Name: endpoints endpoints_flight_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.endpoints
    ADD CONSTRAINT endpoints_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);


--
-- Name: endpoints endpoints_from_loc_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.endpoints
    ADD CONSTRAINT endpoints_from_loc_fkey FOREIGN KEY (from_loc) REFERENCES public.airports(id);


--
-- Name: endpoints endpoints_to_loc_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.endpoints
    ADD CONSTRAINT endpoints_to_loc_fkey FOREIGN KEY (to_loc) REFERENCES public.airports(id);


--
-- Name: promos promos_flight_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.promos
    ADD CONSTRAINT promos_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);


--
-- Name: segments segments_aircraft_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_aircraft_fkey FOREIGN KEY (aircraft) REFERENCES public.aircraft(id);


--
-- Name: segments segments_flight_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_flight_fkey FOREIGN KEY (flight) REFERENCES public.flights(id);


--
-- Name: segments segments_from_loc_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_from_loc_fkey FOREIGN KEY (from_loc) REFERENCES public.airports(id);


--
-- Name: segments segments_to_loc_fkey; Type: FK CONSTRAINT; Schema: public; Owner: ale
--

ALTER TABLE ONLY public.segments
    ADD CONSTRAINT segments_to_loc_fkey FOREIGN KEY (to_loc) REFERENCES public.airports(id);


--
-- PostgreSQL database dump complete
--

