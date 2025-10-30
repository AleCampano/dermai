CREATE PROCEDURE SP_Crearperfil
	@CaracteristicasPiel NVARCHAR(200) = NULL,
	@PreferenciaProducto NVARCHAR(200) = NULL,
	@Presupuesto NVARCHAR(50) = NULL,
	@FrecuenciaRutina NVARCHAR(50) = NULL

AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Perfil (CaracteristicasPiel, PreferenciaProducto, Presupuesto, FrecuenciaRutina)
	VALUES (@CaracteristicasPiel, @PreferenciaProducto, @Presupuesto, @FrecuenciaRutina);
	SELECT SCOPE_IDENTITY();
END;