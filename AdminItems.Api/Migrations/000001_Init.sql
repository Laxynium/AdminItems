CREATE TABLE IF NOT EXISTS "admin_items"(
    "id" bigint PRIMARY KEY,
    "code" varchar(12) UNIQUE NOT NULL,
    "name" varchar(200) UNIQUE NOT NULL,
    "comments" varchar(1000) NOT NULL,
    "color" varchar(50) NOT NULL    
);

CREATE TABLE IF NOT EXISTS "colors"(
    "id" bigint PRIMARY KEY,
    "name" varchar(50) NOT NULL
);