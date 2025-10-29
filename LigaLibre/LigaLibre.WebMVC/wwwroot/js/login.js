$(document).ready(function () {
    const API_URL = getApiUrl(API_CONFIG.endpoints.auth.login);

    $('#loginForm').on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid()) {
            return;
        }

        const email = $('#Email').val();
        const password = $('#Password').val();

        $('#btnLogin').prop('disabled', true);
        $('#btnText').addClass('d-none');
        $('#btnSpinner').removeClass('d-none');

        fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => Promise.reject(err));
            }
            return response.json();
        })
        .then(data => {
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data));
            
            // Crear sesión en el servidor
            return fetch('/Auth/SetSession', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: data.token, email: data.email })
            }).then(() => {
                showToast('Login exitoso. Redirigiendo...', 'success');
                setTimeout(() => {
                    window.location.href = '/Home/Index';
                }, 1500);
            });
        })
        .catch(error => {
            const mensaje = error.message || 'Credenciales inválidas';
            showToast(mensaje, 'danger');
        })
        .finally(() => {
            $('#btnLogin').prop('disabled', false);
            $('#btnText').removeClass('d-none');
            $('#btnSpinner').addClass('d-none');
        });
    });
});
