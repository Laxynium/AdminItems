CREATE EXTENSION IF NOT EXISTS citext;
ALTER TABLE "admin_items" ALTER COLUMN "code" TYPE citext;
ALTER TABLE "admin_items" ALTER COLUMN "name" TYPE citext;