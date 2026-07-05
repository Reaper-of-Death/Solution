-- Количество забегов по персонажам
SELECT 
    c.name AS character_name,
    COUNT(r.id) AS runs_count,
    AVG(r.score) AS avg_score,
    MAX(r.score) AS max_score
FROM characters c
LEFT JOIN runs r ON c.id = r.character_id
WHERE r.is_completed = TRUE
GROUP BY c.name
ORDER BY runs_count DESC;

-- Статистика по предметам: сколько раз использовались в забегах
SELECT 
    i.name,
    i.item_type,
    COUNT(ri.id) AS usage_count,
    AVG(i.damage_bonus) AS avg_damage_bonus
FROM items i
LEFT JOIN run_items ri ON i.id = ri.item_id
GROUP BY i.id, i.name, i.item_type
HAVING COUNT(ri.id) > 0
ORDER BY usage_count DESC;