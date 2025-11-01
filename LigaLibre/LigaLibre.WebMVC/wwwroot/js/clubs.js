const API_BASE = API_CONFIG.baseUrl + '/api/club';
let editingClubId = null;
let deletingClubId = null;

$(document).ready(function () {
    loadClubs();
});

function getToken() {
    return localStorage.getItem('token');
}

function getHeaders() {
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getToken()}`
    };
}

function loadClubs() {
    fetch(getApiUrl(API_CONFIG.endpoints.club.getAll), {
        headers: getHeaders()
    })
    .then(response => response.json())
    .then(data => {
        displayClubs(data);
    })
    .catch(error => {
        showToast('Error al cargar clubes', 'danger');
        console.error(error);
    });
}

function displayClubs(clubs) {
    const container = $('#clubsList');
    container.empty();

    if (clubs.length === 0) {
        container.html('<div class="col-12"><p class="text-center text-muted">No hay clubes registrados</p></div>');
        return;
    }

    clubs.forEach(club => {
        const card = `
            <div class="col-md-6 col-lg-4">
                <div class="club-card">
                    <h5><i class="fas fa-shield-alt text-primary"></i> ${club.name}</h5>
                    <p class="mb-2"><i class="fas fa-city"></i> ${club.city}</p>
                    <p class="mb-2"><i class="fas fa-stadium"></i> ${club.stadiumName}</p>
                    <p class="mb-2"><i class="fas fa-users"></i> ${club.numberOfPartners} socios</p>
                    <p class="mb-3"><i class="fas fa-envelope"></i> ${club.email}</p>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-outline-primary" onclick="editClub(${club.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteClub(${club.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </div>
                </div>
            </div>`;
        container.append(card);
    });
}

function openCreateModal() {
    editingClubId = null;
    $('#modalTitle').text('Nuevo Club');
    $('#clubForm')[0].reset();
    $('#clubId').val('');
}

function editClub(id) {
    fetch(`${getApiUrl(API_CONFIG.endpoints.club.getById)}?id=${id}`, {
        headers: getHeaders()
    })
    .then(response => response.json())
    .then(club => {
        editingClubId = id;
        $('#modalTitle').text('Editar Club');
        $('#clubId').val(club.id);
        $('#name').val(club.name);
        $('#city').val(club.city);
        $('#email').val(club.email);
        $('#phone').val(club.phone);
        $('#address').val(club.address);
        $('#stadiumName').val(club.stadiumName);
        $('#numberOfPartners').val(club.numberOfPartners);
        
        const modal = new bootstrap.Modal(document.getElementById('clubModal'));
        modal.show();
    })
    .catch(error => {
        showToast('Error al cargar el club', 'danger');
        console.error(error);
    });
}

function saveClub() {
    const clubData = {
        name: $('#name').val(),
        city: $('#city').val(),
        email: $('#email').val(),
        phone: $('#phone').val(),
        address: $('#address').val(),
        stadiumName: $('#stadiumName').val(),
        numberOfPartners: parseInt($('#numberOfPartners').val())
    };

    if (editingClubId) {
        clubData.id = editingClubId;
        updateClub(clubData);
    } else {
        createClub(clubData);
    }
}

function createClub(clubData) {
    fetch(getApiUrl(API_CONFIG.endpoints.club.create), {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(clubData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Club creado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('clubModal')).hide();
        loadClubs();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function updateClub(clubData) {
    fetch(getApiUrl(API_CONFIG.endpoints.club.update), {
        method: 'PUT',
        headers: getHeaders(),
        body: JSON.stringify(clubData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Club actualizado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('clubModal')).hide();
        loadClubs();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function deleteClub(id) {
    deletingClubId = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDelete() {
    fetch(`${getApiUrl(API_CONFIG.endpoints.club.delete)}?id=${deletingClubId}`, {
        method: 'DELETE',
        headers: getHeaders()
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        showToast('Club eliminado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
        loadClubs();
    })
    .catch(error => {
        handleValidationErrors(error, 'Error al eliminar el club');
    });
}

function handleValidationErrors(error, defaultMessage = 'Error de validación') {
    if (Array.isArray(error)) {
        error.forEach(err => showToast(err.errorMessage, 'danger'));
        return;
    }
    if (error && typeof error === 'object') {
        const messages = [];
        for (const field in error) {
            if (Array.isArray(error[field])) {
                messages.push(...error[field]);
            }
        }
        if (messages.length > 0) {
            messages.forEach(msg => showToast(msg, 'danger'));
            return;
        }
    }
    showToast(defaultMessage, 'danger');
}
