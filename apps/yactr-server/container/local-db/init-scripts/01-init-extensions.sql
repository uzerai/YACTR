CREATE EXTENSION IF NOT EXISTS pg_uuidv7;

-- spatial data extension
CREATE EXTENSION IF NOT EXISTS postgis;

-- Local DB for integration tests.
-- This is a separate database from the main playground database, and can frequently be dropped and recreated.
CREATE DATABASE yactr_test WITH OWNER yactr;
GRANT ALL PRIVILEGES ON DATABASE yactr_test TO yactr;
ALTER DATABASE yactr_test SET search_path TO public;