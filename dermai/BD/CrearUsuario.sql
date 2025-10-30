CREATE PROCEDURE sp_RegistrarUsuario
@Nombre NVARCHAR(100),
@Email NVARCHAR(100),
@Contrase�a NVARCHAR(100),
@FechaDeNacimiento DATE,
@IdPerfil INT 
AS
BEGIN
	INSERT INTO Usuario (Nombre, Email, Contrase�a, FechaDeNacimiento, IdPerfil)
	Values (@Nombre, @Email, @Contrase�a, @FechaDeNacimiento, @IdPerfil)
END