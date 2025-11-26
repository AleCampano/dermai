document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    
    if (!form) return;

    form.addEventListener("submit", async function (event) {
        event.preventDefault();
        
        const submitButton = form.querySelector('button[type="submit"]');
        const originalText = submitButton.textContent;
        
        try {
            // Mostrar loading
            submitButton.textContent = "Generando rutina...";
            submitButton.disabled = true;

            // Obtener datos del formulario
            const formData = new FormData(form);
            
            // Primero guardar el formulario
            const saveResponse = await fetch(form.action, {
                method: "POST",
                body: formData,
                headers: {
                    'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (!saveResponse.ok) {
                throw new Error("Error al guardar el formulario");
            }

            // Luego generar la rutina con IA
            const iaResponse = await fetch("/api/Home/GenerarRutina", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (!iaResponse.ok) {
                const errorData = await iaResponse.json();
                throw new Error(errorData.error || "Error al generar la rutina");
            }

            const result = await iaResponse.json();
            
            // Mostrar resultado
            alert("¡Rutina generada exitosamente!");
            window.location.href = "/Home/VerRutinaGuardada";
            
        } catch (error) {
            console.error("Error:", error);
            alert("Error: " + error.message);
        } finally {
            // Restaurar botón
            submitButton.textContent = originalText;
            submitButton.disabled = false;
        }
    });
});