CREATE PROCEDURE SP_CrearPerfil
    @CaracteristicasPiel NVARCHAR(100),
    @PreferenciaProducto NVARCHAR(100),
    @Presupuesto NVARCHAR(100),
    @FrecuenciaRutina NVARCHAR(100)
AS
BEGIN
    INSERT INTO Perfil(CaracteristicasPiel, PreferenciaProducto, Presupuesto, FrecuenciaRutina)
    VALUES (@CaracteristicasPiel, @PreferenciaProducto, @Presupuesto, @FrecuenciaRutina);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdPerfil;
END;

