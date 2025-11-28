Planificación obligatoria (previa al desarrollo)
Antes de comenzar a programar, el grupo deberá realizar una planificación de arquitectura MVC. Esta debe incluir:

Controllers: lista de controllers a desarrollar con su responsabilidad principal.
-AccountController: se encarga del inicio de sesión y registro del usuario a la aplicación.
-HomeController: se encarga de las rutinas (conectar con IA, ver, modificar, crear, guardar, etc)
-UserController: se encarga de los datos del perfil del usuario (manejar el ingreso y guardado de los datos del usuario en formularios → características y preferencias).


Models: entidades con sus propiedades (no Id propio porque son autoincrementales).
-Artículo: IdPerfil (int)
-BD: conexión con la base de datos Dermai
-Objeto: convierte string en lista, lista en string, objeto en string y string en objeto
-Perfil: IdUsuario (int), CaracteristicasPiel (string), PreferenciaProducto (string), Presupuesto (string), FrecuenciaRutina (string)
-Rutina: Rutinas (bool), RutinaFinal (string), IdUsuario (int)
-Usuario: Nombre (string), Email (string), Contraseña (string), FechaDeNacimiento (DateTime), -IdPerfil (int).

Mientras avanzamos agregamos estos Models:
-PielFormModel: List<string> Caracteristicas, List<string> Preferencias (string), Presupuesto (string),  (string)Frecuencia
-A Rutina le agregamos el atributo “Content” (int) porque por alguna razon sin eso no funciona.
-UsuarioLogin: Email (string), Contraseña (string).
-GeminiConfig: ApiKey (string), BaseUrl (string) → para proteger la api

Views: vistas necesarias para las funcionalidades requeridas.
1.Home:
-HacerRutina
-Index NO SE USA
-InfoTipoDePiel
-Inicio
-Recomendación
-MostrarRutina
-MiPerfil

2.Account:
-InicioSesion
-Registro

3.User:
-IngresoPiel