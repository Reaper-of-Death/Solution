-- Получить всех незаблокированных персонажей с здоровьем > 80, отсортированных по урону
SELECT name, health, damage, speed
FROM characters
WHERE is_unlocked = TRUE AND health > 80.00
ORDER BY damage DESC;

-- Получить завершённые забеги с высоким счётом
SELECT * FROM runs
WHERE is_completed = TRUE AND score > 1000.00
ORDER BY score DESC, total_time_seconds ASC;