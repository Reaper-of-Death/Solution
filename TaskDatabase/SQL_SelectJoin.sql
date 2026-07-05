-- LEFT JOIN: все персонажи и их забеги (даже если забегов нет)
SELECT 
    c.name AS character_name,
    r.run_seed,
    r.score,
    r.is_completed
FROM characters c
LEFT JOIN runs r ON c.id = r.character_id
ORDER BY c.name;

-- RIGHT JOIN: все забеги и их предметы (аналог LEFT, но с другой стороны)
SELECT 
    r.run_seed,
    r.score,
    i.name AS item_name,
    ri.pickup_order
FROM run_items ri
RIGHT JOIN runs r ON ri.run_id = r.id
LEFT JOIN items i ON ri.item_id = i.id
ORDER BY r.started_at;

-- INNER JOIN (пересечение): только забеги с предметами
SELECT 
    r.run_seed,
    r.score,
    COUNT(ri.id) AS items_collected,
    AVG(i.damage_bonus) AS avg_item_damage
FROM runs r
INNER JOIN run_items ri ON r.id = ri.run_id
INNER JOIN items i ON ri.item_id = i.id
GROUP BY r.id, r.run_seed, r.score
HAVING COUNT(ri.id) >= 2
ORDER BY items_collected DESC;

-- Полное пересечение: забеги, где были и предметы, и убийства врагов
SELECT 
    r.run_seed,
    r.score,
    COUNT(DISTINCT ri.item_id) AS unique_items,
    SUM(ek.kill_count) AS total_kills
FROM runs r
INNER JOIN run_items ri ON r.id = ri.run_id
INNER JOIN run_enemy_kills ek ON r.id = ek.run_id
GROUP BY r.id, r.run_seed, r.score
HAVING COUNT(DISTINCT ri.item_id) > 1 AND SUM(ek.kill_count) > 10;