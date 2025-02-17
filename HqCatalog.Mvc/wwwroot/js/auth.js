document.addEventListener("DOMContentLoaded", function () {
    const passwordInput = document.getElementById("password");
    const confirmPasswordInput = document.getElementById("confirmPassword");
    const passwordHelp = document.getElementById("passwordHelp");
    const confirmPasswordHelp = document.getElementById("confirmPasswordHelp");
    const registerBtn = document.getElementById("registerBtn");
    const togglePasswordIcons = document.querySelectorAll(".toggle-password");

    function validatePassword() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        const regex = /^(?=.*[A-Z])(?=.*\d).{6,}$/;

        if (!regex.test(password)) {
            passwordInput.classList.add("is-invalid");
            passwordInput.classList.remove("is-valid");
            passwordHelp.style.color = "red";
            registerBtn.disabled = true;
        } else {
            passwordInput.classList.remove("is-invalid");
            passwordInput.classList.add("is-valid");
            passwordHelp.style.color = "green";
        }

        if (confirmPassword !== password || confirmPassword === "") {
            confirmPasswordInput.classList.add("is-invalid");
            confirmPasswordInput.classList.remove("is-valid");
            confirmPasswordHelp.textContent = "As senhas não coincidem.";
            confirmPasswordHelp.style.color = "red";
            registerBtn.disabled = true;
        } else {
            confirmPasswordInput.classList.remove("is-invalid");
            confirmPasswordInput.classList.add("is-valid");
            confirmPasswordHelp.textContent = "Senhas coincidem!";
            confirmPasswordHelp.style.color = "green";
            registerBtn.disabled = false;
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        const logoutForm = document.getElementById("logoutForm");

        if (logoutForm) {
            logoutForm.addEventListener("submit", function () {
                console.log("Logout acionado! Enviando requisição ao servidor...");
            });
        }
    });


    document.addEventListener("DOMContentLoaded", function () {
        const logoutButton = document.querySelector("#logoutForm button");
        if (logoutButton) {
            logoutButton.addEventListener("click", function (event) {
                event.preventDefault();
                document.getElementById("logoutForm").submit();
            });
        }
    });


    function togglePasswordVisibility(event) {
        const input = event.target.parentNode.previousElementSibling;
        if (input.type === "password") {
            input.type = "text";
            event.target.classList.remove("fa-eye");
            event.target.classList.add("fa-eye-slash");
        } else {
            input.type = "password";
            event.target.classList.remove("fa-eye-slash");
            event.target.classList.add("fa-eye");
        }
    }

    passwordInput.addEventListener("input", validatePassword);
    confirmPasswordInput.addEventListener("input", validatePassword);

    togglePasswordIcons.forEach(icon => {
        icon.addEventListener("click", togglePasswordVisibility);
    });
});
