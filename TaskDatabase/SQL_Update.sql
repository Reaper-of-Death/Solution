-- Обновление: увеличить здоровье всех врагов-боссов на 20%
UPDATE enemies
SET health = health * 1.20
WHERE is_boss = TRUE;

-- Обновление: пометить все предметы с отрицательным бонусом как проклятые
UPDATE items
SET is_cursed = TRUE
WHERE damage_bonus < 0 OR health_bonus < 0;

-- Удаление: удалить все незавершённые забеги на 1-м этаже
DELETE FROM runs
WHERE is_completed = FALSE AND floor_number = 1;

-- Безопасное удаление с проверкой
DELETE FROM run_items
WHERE run_id IN (
    SELECT id FROM runs
    WHERE is_completed = FALSE AND started_at < CURRENT_DATE - INTERVAL '30 days'
);