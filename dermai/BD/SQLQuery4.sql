CREATE PROCEDURE SP_RegistrarUsuario
	@Nombre VARCHAR(50),
	@Email VARCHAR(50),
	@Contrase�a VARCHAR(50),
	@FechaDeNacimiento DATE,
	@IdPerfil INT
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Usuario (Nombre, Email, Contrase�a, FechaDeNacimiento, IdPerfil)
    VALUES (@Nombre, @Email, @Contrase�a, @FechaDeNacimiento, @IdPerfil);

    SELECT SCOPE_IDENTITY() AS IdUsuario;
END
GO
