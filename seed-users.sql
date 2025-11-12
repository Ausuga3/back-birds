-- Usuario de prueba para login
-- Email: admin@test.com
-- Password: Admin123!
-- Role: Admin (0 en la BD)

-- Hash BCrypt de "Admin123!" generado con work factor 11
-- Nota: Usa el paquete BCrypt.Net-Next en .NET

INSERT INTO Users (Id, Email, PasswordHash, Name, Role)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'admin@test.com',
    '$2a$11$5tZDgKX8Fm9vYQXhKJp7rHN8W.kpS.YZqLnK6ykC3DhCNv5gU9zQV',
    'Admin Test',
    0
);
