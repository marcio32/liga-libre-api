const API_BASE = API_CONFIG.baseUrl + '/api/Referee';
let editingRefereeId = null;
let deletingRefereeId = null;

$(document).ready(function () {
    loadReferees();
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

function getCategoryText(categoryId) {
    const categories = ['Nacional', 'Regional', 'Internacional'];
    return categories[categoryId] || 'Desconocida';
}

function loadReferees() {
    fetch('/Referee/GetAll')
    .then(response => response.json())
    .then(data => {
        displayReferees(data);
    })
    .catch(error => {
        showToast('Error al cargar árbitros', 'danger');
        console.error(error);
    });
}

function displayReferees(referees) {
    const container = $('#refereesList');
    container.empty();

    if (referees.length === 0) {
        container.html('<div class="col-12"><p class="text-center text-muted">No hay árbitros registrados</p></div>');
        return;
    }

    referees.forEach(referee => {
        const statusBadge = referee.isActive 
            ? '<span class="badge-active"><i class="fas fa-check-circle"></i> Activo</span>'
            : '<span class="badge-inactive"><i class="fas fa-times-circle"></i> Inactivo</span>';

        const card = `
            <div class="col-md-6 col-lg-4">
                <div class="referee-card">
                    <h5><i class="fas fa-user-tie text-primary"></i> ${referee.firstName} ${referee.lastName}</h5>
                    <p class="mb-2"><i class="fas fa-star"></i> ${getCategoryText(referee.category)}</p>
                    <p class="mb-2"><i class="fas fa-id-card"></i> ${referee.licenseNumber}</p>
                    <p class="mb-3">${statusBadge}</p>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-outline-primary" onclick="editReferee(${referee.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deleteReferee(${referee.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </div>
                </div>
            </div>`;
        container.append(card);
    });
}

function openCreateModal() {
    editingRefereeId = null;
    $('#modalTitle').text('Nuevo Árbitro');
    $('#refereeForm')[0].reset();
    $('#refereeId').val('');
    $('#isActive').prop('checked', true);
}

function saveReferee() {
    const refereeData = {
        id: $('#refereeId').val() || 0,
        firstName: $('#firstName').val(),
        lastName: $('#lastName').val(),
        category: parseInt($('#category').val()),
        licenseNumber: $('#licenseNumber').val(),
        isActive: $('#isActive').is(':checked')
    };

    if (refereeData.id > 0) {
        updateReferee(refereeData);
    } else {
        createReferee(refereeData);
    }
}

function editReferee(id) {
    fetch(`/Referee/GetById?id=${id}`)
    .then(response => response.json())
    .then(referee => {
        editingRefereeId = id;
        $('#modalTitle').text('Editar Árbitro');
        $('#refereeId').val(referee.id);
        $('#firstName').val(referee.firstName);
        $('#lastName').val(referee.lastName);
        $('#category').val(referee.category);
        $('#licenseNumber').val(referee.licenseNumber);
        $('#isActive').prop('checked', referee.isActive);
        
        const modal = new bootstrap.Modal(document.getElementById('refereeModal'));
        modal.show();
    })
    .catch(error => {
        showToast('Error al cargar el árbitro', 'danger');
        console.error(error);
    });
}
function createReferee(refereeData) {
    fetch('/Referee/Create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(refereeData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Árbitro creado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('refereeModal')).hide();
        loadReferees();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function updateReferee(refereeData) {
    fetch('/Referee/Update', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(refereeData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Árbitro actualizado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('refereeModal')).hide();
        loadReferees();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function deleteReferee(id) {
    deletingRefereeId = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDelete() {
    fetch(`/Referee/Delete?id=${deletingRefereeId}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        showToast('Árbitro eliminado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
        loadReferees();
    })
    .catch(error => {
        handleValidationErrors(error, 'Error al eliminar el árbitro');
    });
}

function handleValidationErrors(error, defaultMessage = 'Error de validación') {
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
