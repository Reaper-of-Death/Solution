-- Связующая таблица run_items связывает runs и items (многие ко многим)
ALTER TABLE run_items
    ADD CONSTRAINT fk_run_items_run
    FOREIGN KEY (run_id)
    REFERENCES runs(id)
    ON DELETE CASCADE;

ALTER TABLE run_items
    ADD CONSTRAINT fk_run_items_item
    FOREIGN KEY (item_id)
    REFERENCES items(id)
    ON DELETE CASCADE;

-- Уникальная пара (run_id, item_id), чтобы не дублировать предметы в забеге
ALTER TABLE run_items
    ADD CONSTRAINT unique_run_item UNIQUE (run_id, item_id);