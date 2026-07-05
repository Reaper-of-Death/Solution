-- 1. Таблица персонажей (игровых классов)
CREATE TABLE characters (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),          -- GUID
    name VARCHAR(100) NOT NULL,                            -- строка
    health DECIMAL(8,2) DEFAULT 100.00,                   -- десятичное число
    damage DECIMAL(6,2) DEFAULT 10.00,                    -- десятичное число
    speed DECIMAL(4,2) DEFAULT 1.00,                      -- десятичное число
    is_unlocked BOOLEAN DEFAULT TRUE,                     -- булевый тип
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,       -- дата/время
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Таблица предметов (активные и пассивные)
CREATE TABLE items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(150) NOT NULL,
    description TEXT,
    item_type VARCHAR(50) CHECK (item_type IN ('active', 'passive', 'trinket')),
    damage_bonus DECIMAL(6,2) DEFAULT 0.00,
    health_bonus DECIMAL(6,2) DEFAULT 0.00,
    speed_bonus DECIMAL(4,2) DEFAULT 0.00,
    is_cursed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Таблица врагов (монстров)
CREATE TABLE enemies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    health DECIMAL(8,2) NOT NULL,
    damage DECIMAL(6,2) NOT NULL,
    speed DECIMAL(4,2) DEFAULT 1.00,
    experience_reward INTEGER DEFAULT 10,
    is_boss BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 4. Таблица забегов (прохождений игроков)
CREATE TABLE runs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    character_id UUID NOT NULL,                            -- связь 1:N с персонажами
    run_seed VARCHAR(50),                                  -- строка (сид уровня)
    floor_number INTEGER DEFAULT 1,
    score DECIMAL(10,2) DEFAULT 0.00,                     -- десятичное число
    is_completed BOOLEAN DEFAULT FALSE,                   -- булевый тип
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,       -- дата/время
    completed_at TIMESTAMP,
    total_time_seconds INTEGER DEFAULT 0
);

-- 5. Связующая таблица для связи N:N (предметы в забеге)
CREATE TABLE run_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    run_id UUID NOT NULL,
    item_id UUID NOT NULL,
    pickup_order INTEGER,                                 -- порядок подбора
    picked_up_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,    -- дата/время
    is_used BOOLEAN DEFAULT FALSE                        -- булевый тип
);

-- 6. Дополнительная таблица для статистики по врагам (для полноты)
CREATE TABLE run_enemy_kills (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    run_id UUID NOT NULL,
    enemy_id UUID NOT NULL,
    kill_count INTEGER DEFAULT 0,
    first_kill_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_kill_at TIMESTAMP
);