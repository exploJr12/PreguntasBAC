# PreguntasBAC

Para poder ejecutar el programa cambien el nombre del servidor de la base de datos.
Luego ejecute las migraciones en la consola de los paquete nuGets.
Luego en Sql Server ejecute los siguientes procesos almacenados (el motivo del porque no estan en las migraciones los procesos es porque de cierta manera da mejor libertad para ejecutarlos).
-- procesos --

CREATE PROCEDURE RegistrarUsuario
    @Nombre NVARCHAR(50),
    @Contraseña NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Usuarios WHERE Nombre = @Nombre)
    BEGIN
        THROW 50001, 'El usuario ya existe.', 1;
    END
    ELSE
    BEGIN
        INSERT INTO Usuarios (Nombre, Contraseña)
        VALUES (@Nombre, @Contraseña);
    END
END

go

CREATE PROCEDURE IniciarSesion
    @Nombre NVARCHAR(50),
    @Contraseña NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Usuarios WHERE Nombre = @Nombre AND Contraseña = @Contraseña)
    BEGIN
        SELECT IdUsuario, Nombre FROM Usuarios WHERE Nombre = @Nombre;
    END
    ELSE
    BEGIN
        THROW 50002, 'Usuario o contraseña incorrectos.', 1;
    END
END

go

CREATE PROCEDURE RegistrarPregunta
    @UsuarioId INT,
    @Contenido NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Preguntas (UsuarioId, Contenido)
    VALUES (@UsuarioId, @Contenido);
END;

go

CREATE PROCEDURE RegistrarRespuesta
    @PreguntaId INT,
    @UsuarioId INT,
    @Contenido NVARCHAR(MAX)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Preguntas WHERE IdPregunta = @PreguntaId AND Cerrada = 0)
    BEGIN
        INSERT INTO Respuestas (PreguntaId, UsuarioId, Contenido)
        VALUES (@PreguntaId, @UsuarioId, @Contenido);
    END
    ELSE
    BEGIN
        THROW 50003, 'La pregunta está cerrada o no existe.', 1;
    END
END

go

CREATE PROCEDURE CerrarPregunta
    @PreguntaId INT,
    @UsuarioId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Preguntas WHERE IdPregunta = @PreguntaId AND UsuarioId = @UsuarioId AND Cerrada = 0)
    BEGIN
        UPDATE Preguntas
        SET Cerrada = 1
        WHERE IdPregunta = @PreguntaId;
    END
    ELSE
    BEGIN
        THROW 50004, 'No tienes permisos para cerrar esta pregunta o ya está cerrada.', 1;
    END
END



go

CREATE PROCEDURE PreguntasOrdenadas
    @Orden NVARCHAR(10) -- Parámetro que define el orden: 'ASC' o 'DESC'
AS
BEGIN
    IF @Orden = 'ASC'
    BEGIN
        SELECT 
            p.IdPregunta AS PreguntaId,
            p.Contenido,
            p.FechaCreacion,
            p.Cerrada,
            u.Nombre AS Autor
        FROM 
            Preguntas p
        INNER JOIN 
            Usuarios u ON p.UsuarioId = u.IdUsuario
        ORDER BY 
            p.FechaCreacion ASC; -- Más antiguas a más recientes
    END
    ELSE
    BEGIN
        SELECT 
            p.IdPregunta AS PreguntaId,
            p.Contenido,
            p.FechaCreacion,
            p.Cerrada,
            u.Nombre AS Autor
        FROM 
            Preguntas p
        INNER JOIN 
            Usuarios u ON p.UsuarioId = u.IdUsuario
        ORDER BY 
            p.FechaCreacion DESC; -- Más recientes a más antiguas
    END
END;
Si por algun motivo llega aparecer el boton de logout en ve de una pregunta (no tienes cuenta? ) presion el boton de logout y le saldra la pregunta y podra crear el usuario
