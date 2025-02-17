document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll(".filter-checkbox");
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", filterHQs);
    });

    function filterHQs() {
        let selectedEditoras = [...document.querySelectorAll("[data-filter='editora']:checked")].map(cb => cb.value);
        let selectedGeneros = [...document.querySelectorAll("[data-filter='genero']:checked")].map(cb => cb.value);

        document.querySelectorAll(".hq-item").forEach(item => {
            let editora = item.getAttribute("data-editora");
            let genero = item.getAttribute("data-genero");

            let editoraMatch = selectedEditoras.length === 0 || selectedEditoras.includes(editora);
            let generoMatch = selectedGeneros.length === 0 || selectedGeneros.includes(genero);

            item.style.display = editoraMatch && generoMatch ? "flex" : "none";
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const logoutForm = document.getElementById("logoutForm");

    if (logoutForm) {
        logoutForm.addEventListener("submit", function (event) {
            event.preventDefault(); // 🔹 Evita envio padrão

            console.log("🔹 Logout acionado! Enviando requisição...");

            fetch(logoutForm.action, {
                method: "POST",
                body: new FormData(logoutForm),
                headers: {
                    "X-Requested-With": "XMLHttpRequest"
                }
            }).then(response => {
                console.log("🔹 Resposta do servidor:", response);

                if (response.redirected) {
                    console.log("🔹 Redirecionando para:", response.url);
                    window.location.href = response.url;
                }
            }).catch(error => console.error("❌ Erro ao fazer logout:", error));
        });
    }
});
