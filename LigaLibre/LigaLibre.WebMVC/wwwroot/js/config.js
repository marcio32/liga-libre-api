// Configuración de la API
const API_CONFIG = {
    baseUrl: 'https://localhost:44348',
    endpoints: {
        auth: {
            login: '/api/auth/login',
            register: '/api/auth/register'
        },
        club: {
            getAll: '/api/club/GetAll',
            getById: '/api/club/GetById',
            create: '/api/club/CreateClub',
            update: '/api/club/UpdateClub',
            delete: '/api/club/Delete'
        }
    }
};

// Función helper para construir URLs completas
function getApiUrl(endpoint) {
    return API_CONFIG.baseUrl + endpoint;
}
