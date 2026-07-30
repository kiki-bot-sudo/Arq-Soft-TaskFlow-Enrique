const mode = document.body.dataset.mode;
const form = document.getElementById("auth-form");
const errorBox = document.getElementById("auth-error");
const password = document.getElementById("password");
document.getElementById("toggle-password").onclick = event => {
  const show = password.type === "password";
  password.type = show ? "text" : "password";
  event.currentTarget.textContent = show ? "Ocultar" : "Mostrar";
};
form.onsubmit = async event => {
  event.preventDefault(); errorBox.textContent = "";
  const button = form.querySelector("button[type=submit]");
  button.disabled = true; button.textContent = "Procesando...";
  const payload = {email:document.getElementById("email").value.trim(),password:password.value};
  if (mode === "register") {
    payload.displayName = document.getElementById("display-name").value.trim();
    payload.confirmPassword = document.getElementById("confirm-password").value;
    if (payload.password !== payload.confirmPassword) {
      errorBox.textContent = "Las contraseñas no coinciden.";
      button.disabled = false; button.textContent = "Crear cuenta"; return;
    }
  }
  try {
    const response = await fetch(`/api/auth/${mode}`, {method:"POST",headers:{"Content-Type":"application/json"},body:JSON.stringify(payload)});
    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.message || Object.values(data.errors || {}).flat()[0] || "No fue posible continuar.");
    }
    location.href = "/";
  } catch (error) {
    errorBox.textContent = error.message; button.disabled = false;
    button.textContent = mode === "register" ? "Crear cuenta" : "Iniciar sesión";
  }
};
