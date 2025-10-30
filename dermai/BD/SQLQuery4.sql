CREATE PROCEDURE SP_RegistrarUsuario
	@Nombre VARCHAR(50),
	@Email VARCHAR(50),
	@Contraseña VARCHAR(50),
	@FechaDeNacimiento DATE,
	@IdPerfil INT
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Usuario (Nombre, Email, Contraseña, FechaDeNacimiento, IdPerfil)
    VALUES (@Nombre, @Email, @Contraseña, @FechaDeNacimiento, @IdPerfil);

    SELECT SCOPE_IDENTITY() AS IdUsuario;
END
GO
