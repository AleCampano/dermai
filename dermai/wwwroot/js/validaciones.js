document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");

    if (!loginForm) return;

    loginForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const email = document.getElementById("Email").value.trim();
        const contraseña = document.getElementById("Contraseña").value.trim();

        if (!email || !contraseña) {
            alert("Por favor, complete todos los campos.");
            return;
        }

        try {
            const response = await fetch("/api/Account/Login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: email, Contraseña: contraseña })
            });
            
            if (!response.ok) {
                const errResult = await response.json().catch(() => ({}));
                alert(errResult.error || "Error al iniciar sesión.");
                return;
            }
            
            const result = await response.json();
            alert(result.mensaje);
            window.location.href = result.redireccion || "/User/CompletarFormularioPiel";
        } catch (error) {
            alert("Error al conectar con el servidor: " + error.message);
        }
    });
});