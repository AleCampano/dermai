ALTER PROCEDURE SP_CrearPerfil
	@IdUsuario INT,
	@CaracteristicasPiel VARCHAR(50),
	@PreferenciaProducto VARCHAR(50),
	@Presupuesto VARCHAR(50),
	@FecuenciaRutina VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Perfil(IdUsuario, CaracteristicasPiel, PreferenciaProducto, Presupuesto, FrecuenciaRutina)
	VALUES (@IdUsuario, @CaracteristicasPiel, @PreferenciaProducto, @Presupuesto, @FecuenciaRutina);

	SELECT SCOPE_IDENTITY() AS IdPerfil;

END
GO

