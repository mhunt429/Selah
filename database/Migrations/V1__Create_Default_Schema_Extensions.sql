CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

SET CONSTRAINTS ALL DEFERRED; -- Allows us to support db transactions with foreign key constraints


CREATE OR REPLACE FUNCTION set_audit_timestamps()
RETURNS TRIGGER AS $$
BEGIN

    IF TG_OP = 'INSERT' THEN
        NEW.original_insert := now() AT TIME ZONE 'utc';
    END IF;

    NEW.last_update := now() AT TIME ZONE 'utc';

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;