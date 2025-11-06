document.addEventListener("DOMContentLoaded", function() {
    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", function (event) {
            const email = document.getElementById("Email").value.trim();
            const password = document.getElementById("Contraseña").value.trim();

            if (email === "" || password === "") {
                alert("Por favor, complete todos los campos.");
                event.preventDefault();
                return;
            }

            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                alert("Por favor, ingrese un correo electrónico válido.");
                event.preventDefault();
            }
        });
    }

    const registroForm = document.getElementById("registroForm");
    if (registroForm) {
        registroForm.addEventListener("submit", function (event) {
            const nombre = document.getElementById("Nombre").value.trim();
            const email = document.getElementById("Email").value.trim();
            const contraseña = document.getElementById("Contraseña").value.trim();
            const fecha = document.getElementById("FechaDeNacimiento").value;

            if (!nombre || !email || !contraseña || !fecha) {
                alert("Por favor, complete todos los campos.");
                event.preventDefault();
                return;
            }

            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                alert("El correo electrónico no tiene un formato válido.");
                event.preventDefault();
                return;
            }

            if (contraseña.length < 6) {
                alert("La contraseña debe tener al menos 6 caracteres.");
                event.preventDefault();
            }
        });
    }
});