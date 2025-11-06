document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");

    if (loginForm) {
        loginForm.addEventListener("submit", function (event) {
            event.preventDefault();

            const email = document.getElementById("Email").value.trim();
            const contraseña = document.getElementById("Contraseña").value.trim();

            if (!email || !contraseña) {
                alert("Por favor, complete todos los campos.");
                return;
            }

            fetch("/api/account/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    Email: email,
                    Contraseña: contraseña
                })
            })            
            .then(response => {
                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    return response.text().then(html => {
                        document.body.innerHTML = html;
                    });
                }
            })
            .catch(error => {
                alert("Error al iniciar sesión: " + error.message);
            });
        });
    }
});