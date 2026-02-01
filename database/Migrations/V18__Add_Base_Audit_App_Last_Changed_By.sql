ALTER TABLE base_audit_table
ADD CONSTRAINT fk_base_audit_app_user
FOREIGN KEY (app_last_changed_by)
REFERENCES app_user(id);