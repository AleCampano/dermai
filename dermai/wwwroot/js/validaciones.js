document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");

    if (loginForm) {
        loginForm.addEventListener("submit", function (event) {
            event.preventDefault(); // Evita recargar y mostrar los datos en la URL

            const email = document.getElementById("Email").value.trim();
            const contraseña = document.getElementById("Contraseña").value.trim();

            if (!email || !contraseña) {
                alert("Por favor, complete todos los campos.");
                return;
            }

            fetch("/api/Account/Login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: email, Contraseña: contraseña })
            })
            .then(async response => {
                const result = await response.json();

                if (response.ok) {
                    alert(result.mensaje);
                    window.location.href = result.redireccion;
                } else {
                    alert(result.error || "Error al iniciar sesión.");
                }
            })
            .catch(error => {
                alert("Error al conectar con el servidor: " + error.message);
            });
        });
    }
});