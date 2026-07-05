(function () {
    let timeoutId;
    let countdownId;
    let secondsLeft = 60;
    
    // Configuración robusta por defecto o inyectada por la vista
    const config = window.SessionConfig || { timeoutMs: 60000, redirectUrl: '/' };
    const modalElement = document.getElementById('modalExpiracion');
    const mbsModal = new bootstrap.Modal(modalElement);
    const lblContador = document.getElementById('lblContador');

    function init() {
        timeoutId = setTimeout(showWarning, config.timeoutMs);
    }

    function showWarning() {
        secondsLeft = 60;
        lblContador.innerText = secondsLeft;
        mbsModal.show();

        countdownId = setInterval(() => {
            secondsLeft--;
            lblContador.innerText = secondsLeft;

            if (secondsLeft <= 0) {
                clearInterval(countdownId);
                mbsModal.hide();
                window.location.href = config.redirectUrl;
            }
        }, 1000);
    }

    // Única función expuesta de forma segura para el click del botón del modal
    window.extenderSesion = function () {
        clearInterval(countdownId);
        mbsModal.hide();
        clearTimeout(timeoutId);
        init(); 
    };

    window.onload = init;
})();