CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

SET CONSTRAINTS ALL DEFERRED; -- Allows us to support db transactions with foreign key constraints