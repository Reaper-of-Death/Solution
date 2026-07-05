-- Один персонаж может иметь много забегов
ALTER TABLE runs
    ADD CONSTRAINT fk_runs_character
    FOREIGN KEY (character_id)
    REFERENCES characters(id)
    ON DELETE CASCADE;