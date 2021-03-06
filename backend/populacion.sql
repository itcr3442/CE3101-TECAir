

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


INSERT INTO public.aircraft (id, code, seats) VALUES ('2b2baa93-c407-4af8-aa64-b64715bbae53', 'AV1', 32);
INSERT INTO public.aircraft (id, code, seats) VALUES ('fb2f5c66-f41c-4c7d-9de3-05a3d33058a4', 'AV2', 23);
INSERT INTO public.aircraft (id, code, seats) VALUES ('6ab07338-57c5-4861-8523-52c7cc6d796d', 'AV3', 15);
INSERT INTO public.aircraft (id, code, seats) VALUES ('a22e7b2f-c1b1-4a2b-89e6-2458717d43b7', 'AV4', 60);
INSERT INTO public.aircraft (id, code, seats) VALUES ('8de9045b-db71-4882-af44-85a34ed4d852', 'AV5', 45);



INSERT INTO public.airports (id, code, comment) VALUES ('5b555250-ef37-48c2-b8bc-79dbba40c3eb', 'SJ-CR', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('64d7409f-4110-426e-99e8-3e0e823a71c1', 'TOK-JP', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('e3e06da0-314b-496b-afca-ae859f34ffb7', 'VAR-POL', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('7943edbe-fc92-45d4-b801-a9f41bab3f19', 'DF-MEX', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('5ee76f5e-ee94-4b88-93da-317226475db7', 'MI-US', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('a1581cea-3a53-4677-9897-3c02fa4e677f', 'BER-GER', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('181afd1f-5d22-413a-ba06-2ab4cb72b964', 'SVO-RUS', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('a74cc81a-ec0c-49c4-8a46-73fa28c664e1', 'LIM-PER', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('31c58a3a-24d2-401e-b5a0-1ceda92e58b6', 'DEL-IND', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('67fcda5a-16f4-4e86-9aff-7a23a027dc52', 'ICN-KOR', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('1521cc5a-b202-4ce3-a130-0804541fee44', 'PEK-CCP', 'nocomment');
INSERT INTO public.airports (id, code, comment) VALUES ('0ddfed25-e574-455a-9c78-4bf7c173d250', 'TPE-TWN', 'nocomment');



INSERT INTO public.flights (id, no, state, comment, price) VALUES ('242d3b82-e46d-4db9-b7fa-fbf8afa3c938', 1, 'booking', 'nocomment', 223);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('ce584c98-c842-456a-8856-145ca72b37f9', 2, 'booking', 'nocomment', 112);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('f67cd16d-edc6-4eeb-afd7-fa221f1a6636', 3, 'booking', 'nocomment', 500);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('2cd87923-942e-4218-b311-b95c803b3b19', 4, 'booking', 'nocomment', 100);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('f7e60da3-ebce-44b0-906b-73f8ae11dd0e', 5, 'booking', 'nocomment', 153);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('391f7ffc-c83e-4876-b582-c401809a9b9f', 6, 'booking', 'nocomment', 275);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('b9b51f2f-6457-4b51-9366-b1e99f2e704b', 7, 'booking', 'nocomment', 75);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('884875f3-3054-4a25-8531-10e4fe79d300', 8, 'checkin', 'nocomment', 155);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('d65270ef-8acc-4ebf-a577-63ac1dbd3652', 9, 'checkin', 'nocomment', 215);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('466e8b55-8445-4205-afd7-71ea24e2c3be', 10, 'checkin', 'nocomment', 345);
INSERT INTO public.flights (id, no, state, comment, price) VALUES ('071142de-64f0-4a37-89e3-8c6f2b3abb53', 11, 'checkin', 'nocomment', 425);



INSERT INTO public.users (id, username, hash, salt, first_name, last_name, phonenumber, email, university, student_id, type, miles) VALUES ('19d851aa-a27c-4eec-a831-ae647311fe41', 'john', '5e6eac40021f178d2ad50e8018a821d2', 'bbd9871fb29487fc2c2619068a1ccc2f', 'John', 'Doe', NULL, NULL, NULL, NULL, 'manager', 0);






INSERT INTO public.promos (id, code, flight, price, start_time, end_time, img) VALUES ('e02233b1-7654-4dbb-b77a-b731e7f65cb6', 'PROMO-PROMO', '242d3b82-e46d-4db9-b7fa-fbf8afa3c938', 11, '2020-01-21 09:00:00-06', '2021-05-22 09:00:00-06', '\xaeb98ae41489460c5292aafade4498ee');
INSERT INTO public.promos (id, code, flight, price, start_time, end_time, img) VALUES ('042cc082-f2c9-49bb-abf6-fb0a0170908d', 'PROMO-1', 'ce584c98-c842-456a-8856-145ca72b37f9', 11, '2020-03-20 09:00:00-06', '2021-04-30 09:00:00-06', '\xaeb98ae41489460c5292aafade4498ee');
INSERT INTO public.promos (id, code, flight, price, start_time, end_time, img) VALUES ('c269be9f-914a-41c7-afb2-921bccff42b2', 'PROMO-2', '2cd87923-942e-4218-b311-b95c803b3b19', 11, '2020-04-11 09:00:00-06', '2021-04-30 09:00:00-06', '\xaeb98ae41489460c5292aafade4498ee');
INSERT INTO public.promos (id, code, flight, price, start_time, end_time, img) VALUES ('47cefc29-4b3f-41e5-a496-cc903d5ea9e0', 'PROMO-3', '391f7ffc-c83e-4876-b582-c401809a9b9f', 11, '2020-04-30 09:00:00-06', '2021-04-30 09:00:00-06', '\xaeb98ae41489460c5292aafade4498ee');
INSERT INTO public.promos (id, code, flight, price, start_time, end_time, img) VALUES ('61163b4b-974d-455d-b4b2-7b1000162829', 'PROMO-4', 'b9b51f2f-6457-4b51-9366-b1e99f2e704b', 11, '2020-07-10 09:00:00-06', '2021-04-30 09:00:00-06', '\xaeb98ae41489460c5292aafade4498ee');






INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('071142de-64f0-4a37-89e3-8c6f2b3abb53', '242d3b82-e46d-4db9-b7fa-fbf8afa3c938', 1, '5b555250-ef37-48c2-b8bc-79dbba40c3eb', '05:00:00-06', 'a74cc81a-ec0c-49c4-8a46-73fa28c664e1', '17:00:00-06', '2b2baa93-c407-4af8-aa64-b64715bbae53');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('4d64c378-3bb2-4b9a-9751-c1fd7fd80fbe', '242d3b82-e46d-4db9-b7fa-fbf8afa3c938', 2, 'a74cc81a-ec0c-49c4-8a46-73fa28c664e1', '17:00:00-06', '31c58a3a-24d2-401e-b5a0-1ceda92e58b6', '22:00:00-06', '2b2baa93-c407-4af8-aa64-b64715bbae53');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('9de195cb-8452-4e86-bb73-baededf6b438', 'ce584c98-c842-456a-8856-145ca72b37f9', 1, '5b555250-ef37-48c2-b8bc-79dbba40c3eb', '12:30:00-06', 'a1581cea-3a53-4677-9897-3c02fa4e677f', '01:30:00-06', 'a22e7b2f-c1b1-4a2b-89e6-2458717d43b7');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('f06bab62-fdc4-41fb-9b6e-900217709df7', 'ce584c98-c842-456a-8856-145ca72b37f9', 2, 'a1581cea-3a53-4677-9897-3c02fa4e677f', '01:30:00-06', '181afd1f-5d22-413a-ba06-2ab4cb72b964', '04:30:00-06', 'fb2f5c66-f41c-4c7d-9de3-05a3d33058a4');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('236cec4f-63da-413c-9cd6-3a82cd036610', 'f67cd16d-edc6-4eeb-afd7-fa221f1a6636', 1, '64d7409f-4110-426e-99e8-3e0e823a71c1', '04:30:00+09', '67fcda5a-16f4-4e86-9aff-7a23a027dc52', '05:30:00+09', '6ab07338-57c5-4861-8523-52c7cc6d796d');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('ce827abf-0f10-4d95-8040-c4a4a220f53a', 'f67cd16d-edc6-4eeb-afd7-fa221f1a6636', 2, '67fcda5a-16f4-4e86-9aff-7a23a027dc52', '05:30:00+09', '1521cc5a-b202-4ce3-a130-0804541fee44', '06:30:00+09', '6ab07338-57c5-4861-8523-52c7cc6d796d');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('c5b3d6fe-dbcb-46c4-bfc6-f3fea42cac39', '2cd87923-942e-4218-b311-b95c803b3b19', 1, '5ee76f5e-ee94-4b88-93da-317226475db7', '11:00:00-04', 'e3e06da0-314b-496b-afca-ae859f34ffb7', '23:00:00-04', '8de9045b-db71-4882-af44-85a34ed4d852');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('78e11fd1-b8bb-49b4-8fdb-e23d59c8e178', '2cd87923-942e-4218-b311-b95c803b3b19', 2, 'e3e06da0-314b-496b-afca-ae859f34ffb7', '00:30:00-04', '31c58a3a-24d2-401e-b5a0-1ceda92e58b6', '09:30:00-04', '8de9045b-db71-4882-af44-85a34ed4d852');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('d6b65a58-f386-4614-854c-67d56212bfb1', 'f7e60da3-ebce-44b0-906b-73f8ae11dd0e', 1, 'e3e06da0-314b-496b-afca-ae859f34ffb7', '09:30:00-08', 'a74cc81a-ec0c-49c4-8a46-73fa28c664e1', '23:30:00-08', '2b2baa93-c407-4af8-aa64-b64715bbae53');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('68f878e4-ed9b-4b6f-81c0-1ea0fe6e6903', '391f7ffc-c83e-4876-b582-c401809a9b9f', 1, 'a1581cea-3a53-4677-9897-3c02fa4e677f', '23:30:00-02', '67fcda5a-16f4-4e86-9aff-7a23a027dc52', '04:30:00-02', '8de9045b-db71-4882-af44-85a34ed4d852');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('7f7e9409-c788-40b6-a53a-a4fa83975e11', 'b9b51f2f-6457-4b51-9366-b1e99f2e704b', 1, '64d7409f-4110-426e-99e8-3e0e823a71c1', '05:30:00-02', '7943edbe-fc92-45d4-b801-a9f41bab3f19', '09:00:00-02', '6ab07338-57c5-4861-8523-52c7cc6d796d');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('d37a174e-effd-446b-a21d-86f4d074207f', '884875f3-3054-4a25-8531-10e4fe79d300', 1, 'a74cc81a-ec0c-49c4-8a46-73fa28c664e1', '13:00:00-06', '5b555250-ef37-48c2-b8bc-79dbba40c3eb', '18:00:00-06', 'a22e7b2f-c1b1-4a2b-89e6-2458717d43b7');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('ff285d14-8803-4a9c-8835-e7941dad8395', '884875f3-3054-4a25-8531-10e4fe79d300', 2, '5b555250-ef37-48c2-b8bc-79dbba40c3eb', '18:00:00-06', '7943edbe-fc92-45d4-b801-a9f41bab3f19', '22:30:00-06', 'a22e7b2f-c1b1-4a2b-89e6-2458717d43b7');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('d44efeb6-4f6d-4a22-9407-f2aba816985d', '884875f3-3054-4a25-8531-10e4fe79d300', 3, '7943edbe-fc92-45d4-b801-a9f41bab3f19', '22:30:00-06', '5ee76f5e-ee94-4b88-93da-317226475db7', '03:00:00-06', 'a22e7b2f-c1b1-4a2b-89e6-2458717d43b7');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('78eb5e16-544e-4b31-ab2e-80d53513ddb8', 'd65270ef-8acc-4ebf-a577-63ac1dbd3652', 1, '31c58a3a-24d2-401e-b5a0-1ceda92e58b6', '08:00:00-07', '1521cc5a-b202-4ce3-a130-0804541fee44', '15:00:00-07', '6ab07338-57c5-4861-8523-52c7cc6d796d');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('9cfeb7f7-4312-443e-8bd7-d43fde61f99d', '466e8b55-8445-4205-afd7-71ea24e2c3be', 1, '181afd1f-5d22-413a-ba06-2ab4cb72b964', '13:00:00+04', '67fcda5a-16f4-4e86-9aff-7a23a027dc52', '17:00:00+04', '2b2baa93-c407-4af8-aa64-b64715bbae53');
INSERT INTO public.segments (id, flight, seq_no, from_loc, from_time, to_loc, to_time, aircraft) VALUES ('4bea514c-a110-44fa-b545-4f8dd577dadd', '071142de-64f0-4a37-89e3-8c6f2b3abb53', 1, '5b555250-ef37-48c2-b8bc-79dbba40c3eb', '04:00:00+07', '5ee76f5e-ee94-4b88-93da-317226475db7', '12:00:00+07', '2b2baa93-c407-4af8-aa64-b64715bbae53');






SELECT pg_catalog.setval('public.bags_no_seq', 1, false);



