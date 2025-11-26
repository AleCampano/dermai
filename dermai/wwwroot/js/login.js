document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");

    if (!loginForm) return;

    loginForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        const email = document.getElementById("Email").value.trim();
        const contraseña = document.getElementById("Contraseña").value.trim();
        const submitButton = loginForm.querySelector('button[type="submit"]');
        const originalText = submitButton.textContent;

        if (!email || !contraseña) {
            alert("Por favor, complete todos los campos.");
            return;
        }

        try {
            submitButton.textContent = "Iniciando sesión...";
            submitButton.disabled = true;

            const response = await fetch("/api/Account/Login", {
                method: "POST",
                headers: { 
                    "Content-Type": "application/json",
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({ Email: email, Contraseña: contraseña })
            });
            
            if (!response.ok) {
                const errResult = await response.json().catch(() => ({}));
                throw new Error(errResult.error || "Error al iniciar sesión.");
            }
            
            const result = await response.json();
            alert(result.mensaje);
            window.location.href = result.redireccion || "/User/CompletarFormularioPiel";
        } catch (error) {
            alert("Error: " + error.message);
        } finally {
            submitButton.textContent = originalText;
            submitButton.disabled = false;
        }
    });
});