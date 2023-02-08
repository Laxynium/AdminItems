CREATE TABLE IF NOT EXISTS users(
  "id" bigint GENERATED ALWAYS AS IDENTITY,
  "user" varchar(200) UNIQUE NOT NULL,
  "hash" text NOT NULL,
  "roles" text NOT NULL
)