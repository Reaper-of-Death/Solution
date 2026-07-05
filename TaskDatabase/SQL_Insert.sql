-- Добавляем персонажей
INSERT INTO characters (name, health, damage, speed, is_unlocked) VALUES
('Isaac', 100.00, 10.50, 1.20, TRUE),
('Magdalene', 120.00, 8.00, 0.90, TRUE),
('Cain', 80.00, 12.00, 1.40, TRUE),
('Judas', 70.00, 15.00, 1.10, TRUE),
('???', 90.00, 11.00, 1.00, FALSE);

-- Добавляем предметы
INSERT INTO items (name, description, item_type, damage_bonus, health_bonus, speed_bonus, is_cursed) VALUES
('Brimstone', 'Blood laser', 'active', 20.00, 0.00, 0.00, FALSE),
('Sacred Heart', 'Massive damage up', 'passive', 25.00, 10.00, -0.20, FALSE),
('Soy Milk', 'Tears up, damage down', 'passive', -5.00, 0.00, 0.00, TRUE),
('Mom''s Knife', 'Replaces tears', 'active', 15.00, 0.00, 0.00, FALSE),
('Magic Mushroom', 'All stats up', 'passive', 5.00, 5.00, 0.50, FALSE);

-- Добавляем врагов
INSERT INTO enemies (name, health, damage, speed, experience_reward, is_boss) VALUES
('Gaper', 15.00, 5.00, 0.80, 5, FALSE),
('Mulliboom', 10.00, 8.00, 1.50, 8, FALSE),
('Mom', 200.00, 20.00, 0.50, 50, TRUE),
('Satan', 300.00, 25.00, 0.60, 100, TRUE),
('Fly', 5.00, 3.00, 1.80, 2, FALSE);

-- Добавляем забеги
INSERT INTO runs (character_id, run_seed, floor_number, score, is_completed, started_at, completed_at, total_time_seconds) VALUES
((SELECT id FROM characters WHERE name = 'Isaac'), 'SEED123', 5, 1500.00, TRUE, '2026-07-01 10:00:00', '2026-07-01 10:45:00', 2700),
((SELECT id FROM characters WHERE name = 'Magdalene'), 'SEED456', 3, 850.00, TRUE, '2026-07-02 14:30:00', '2026-07-02 15:10:00', 2400),
((SELECT id FROM characters WHERE name = 'Cain'), 'SEED789', 1, 120.00, FALSE, '2026-07-03 09:00:00', NULL, 600);

-- Добавляем предметы в забеги
INSERT INTO run_items (run_id, item_id, pickup_order, is_used) VALUES
((SELECT id FROM runs WHERE run_seed = 'SEED123'), (SELECT id FROM items WHERE name = 'Brimstone'), 1, TRUE),
((SELECT id FROM runs WHERE run_seed = 'SEED123'), (SELECT id FROM items WHERE name = 'Sacred Heart'), 2, TRUE),
((SELECT id FROM runs WHERE run_seed = 'SEED456'), (SELECT id FROM items WHERE name = 'Soy Milk'), 1, FALSE);