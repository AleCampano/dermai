CREATE PROCEDURE sp_RegistrarUsuario
@Nombre NVARCHAR(100),
@Email NVARCHAR(100),
@Contraseña NVARCHAR(100),
@FechaDeNacimiento DATE,
@IdPerfil INT 
AS
BEGIN
	INSERT INTO Usuario (Nombre, Email, Contraseña, FechaDeNacimiento, IdPerfil)
	Values (@Nombre, @Email, @Contraseña, @FechaDeNacimiento, @IdPerfil)
END