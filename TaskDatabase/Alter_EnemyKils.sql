ALTER TABLE run_enemy_kills
    ADD CONSTRAINT fk_kills_run
    FOREIGN KEY (run_id)
    REFERENCES runs(id)
    ON DELETE CASCADE;

ALTER TABLE run_enemy_kills
    ADD CONSTRAINT fk_kills_enemy
    FOREIGN KEY (enemy_id)
    REFERENCES enemies(id)
    ON DELETE CASCADE;