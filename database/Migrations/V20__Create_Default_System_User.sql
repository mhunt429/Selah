DO $$
BEGIN
    -- Check if user with id = -1 exists
    IF NOT EXISTS (SELECT 1 FROM app_user WHERE id = -1) THEN
        -- Temporarily drop the foreign key constraint to allow self-reference
        ALTER TABLE base_audit_table DROP CONSTRAINT IF EXISTS fk_base_audit_app_user;
        
        -- Insert the default system user
        INSERT INTO app_user(id, app_last_changed_by) VALUES(-1, -1);
        
        -- Re-create the foreign key constraint
        ALTER TABLE base_audit_table
        ADD CONSTRAINT fk_base_audit_app_user
        FOREIGN KEY (app_last_changed_by)
        REFERENCES app_user(id);
    END IF;
END $$;