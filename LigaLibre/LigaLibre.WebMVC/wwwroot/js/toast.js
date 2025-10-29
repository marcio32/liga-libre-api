// Sistema de notificaciones Toast genérico
window.showToast = function(message, type = 'info') {
    // Crear contenedor si no existe
    if ($('.toast-container').length === 0) {
        $('body').append('<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 9999;"></div>');
    }

    const colors = {
        success: { bg: '#b8e6b8', text: '#2d5f2d', progress: '#7bc97b' },
        danger: { bg: '#f4a6b8', text: '#8b2e3f', progress: '#e88fa3' },
        warning: { bg: '#ffe4a3', text: '#8b6914', progress: '#ffd670' },
        info: { bg: '#a7c7e7', text: '#2d4a6b', progress: '#7ba7d9' }
    };

    const color = colors[type] || colors.info;
    const toastId = 'toast-' + Date.now();
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center border-0" role="alert" style="background: ${color.bg}; color: ${color.text}; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
            <div class="d-flex">
                <div class="toast-body" style="font-weight: 500;">${message}</div>
                <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" style="filter: brightness(0.6);"></button>
            </div>
            <div class="toast-progress-bar" style="height: 3px; background: ${color.progress};"></div>
        </div>`;
    
    $('.toast-container').append(toastHtml);
    const toastElement = $('#' + toastId)[0];
    const progressBar = $('#' + toastId + ' .toast-progress-bar');
    
    const toast = new bootstrap.Toast(toastElement, { delay: 10000, autohide: true });
    toast.show();
    
    // Animación de barra de progreso
    progressBar.css({ width: '100%', transition: 'width 10s linear' });
    setTimeout(() => progressBar.css('width', '0%'), 10);
    
    toastElement.addEventListener('hidden.bs.toast', function () {
        $(this).remove();
    });
};
