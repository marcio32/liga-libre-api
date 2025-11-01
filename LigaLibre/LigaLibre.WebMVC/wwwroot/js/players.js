let editingPlayerId = null;
let deletingPlayerId = null;

$(document).ready(function () {
    loadPlayers();
    loadClubs();
});

function loadClubs() {
    fetch('/Player/GetClubs')
    .then(response => response.json())
    .then(clubs => {
        const select = $('#clubId');
        select.empty();
        select.append('<option value="">Seleccionar club...</option>');
        clubs.forEach(club => {
            select.append(`<option value="${club.id}">${club.name}</option>`);
        });
    })
    .catch(error => {
        console.error('Error al cargar clubes:', error);
    });
}

function getPositionClass(position) {
    const positionMap = {
        'Portero': 'position-portero',
        'Defensa': 'position-defensa',
        'Mediocampista': 'position-mediocampista',
        'Delantero': 'position-delantero'
    };
    return positionMap[position] || '';
}

function loadPlayers() {
    fetch('/Player/GetAll')
    .then(response => response.json())
    .then(data => {
        displayPlayers(data);
    })
    .catch(error => {
        showToast('Error al cargar jugadores', 'danger');
        console.error(error);
    });
}

function displayPlayers(players) {
    const container = $('#playersList');
    container.empty();

    if (players.length === 0) {
        container.html('<div class="col-12"><p class="text-center text-muted">No hay jugadores registrados</p></div>');
        return;
    }

    players.forEach(player => {
        const statusBadge = player.isActive 
            ? '<span class="badge-active"><i class="fas fa-check-circle"></i> Activo</span>'
            : '<span class="badge-inactive"><i class="fas fa-times-circle"></i> Inactivo</span>';

        const positionClass = getPositionClass(player.position);

        const card = `
            <div class="col-md-6 col-lg-4">
                <div class="player-card">
                    <h5><i class="fas fa-user-circle text-success"></i> ${player.firstName} ${player.lastName}</h5>
                    <div class="player-info">
                        <span class="position-badge ${positionClass}">${player.position}</span>
                        <span class="player-info-item"><i class="fas fa-tshirt"></i> #${player.jerseyNumber}</span>
                        <span class="player-info-item"><i class="fas fa-flag"></i> ${player.nationality}</span>
                    </div>
                    <div class="player-stats">
                        <div class="stat-item">
                            <div class="stat-value">${player.goals || 0}</div>
                            <div class="stat-label">Goles</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-value">${player.assists || 0}</div>
                            <div class="stat-label">Asistencias</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-value">${player.matchesPlayed || 0}</div>
                            <div class="stat-label">Partidos</div>
                        </div>
                    </div>
                    <p class="mb-2"><i class="fas fa-birthday-cake"></i> ${player.age} años | <i class="fas fa-ruler-vertical"></i> ${player.height}m | <i class="fas fa-weight"></i> ${player.weight}kg</p>
                    <p class="mb-3">${statusBadge}</p>
                    <div class="d-flex gap-2">
                        <button class="btn btn-sm btn-outline-primary" onclick="editPlayer(${player.id})">
                            <i class="fas fa-edit"></i> Editar
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="deletePlayer(${player.id})">
                            <i class="fas fa-trash"></i> Eliminar
                        </button>
                    </div>
                </div>
            </div>`;
        container.append(card);
    });
}

function openCreateModal() {
    editingPlayerId = null;
    $('#modalTitle').text('Nuevo Jugador');
    $('#playerForm')[0].reset();
    $('#playerId').val('');
    $('#isActive').prop('checked', true);
}

function savePlayer() {
    const playerData = {
        id: $('#playerId').val() || 0,
        firstName: $('#firstName').val(),
        lastName: $('#lastName').val(),
        position: $('#position').val(),
        nationality: $('#nationality').val(),
        age: parseInt($('#age').val()),
        jerseyNumber: parseInt($('#jerseyNumber').val()),
        height: parseFloat($('#height').val()),
        weight: parseFloat($('#weight').val()),
        clubId: parseInt($('#clubId').val()),
        dateOfBirth: $('#dateOfBirth').val(),
        isActive: $('#isActive').is(':checked')
    };

    if (playerData.id > 0) {
        updatePlayer(playerData);
    } else {
        createPlayer(playerData);
    }
}

function editPlayer(id) {
    fetch(`/Player/GetById?id=${id}`)
    .then(response => response.json())
    .then(player => {
        editingPlayerId = id;
        $('#modalTitle').text('Editar Jugador');
        $('#playerId').val(player.id);
        $('#firstName').val(player.firstName);
        $('#lastName').val(player.lastName);
        $('#position').val(player.position);
        $('#nationality').val(player.nationality);
        $('#age').val(player.age);
        $('#jerseyNumber').val(player.jerseyNumber);
        $('#height').val(player.height);
        $('#weight').val(player.weight);
        $('#clubId').val(player.clubId);
        $('#dateOfBirth').val(player.dateOfBirth.split('T')[0]);
        $('#isActive').prop('checked', player.isActive);
        
        const modal = new bootstrap.Modal(document.getElementById('playerModal'));
        modal.show();
    })
    .catch(error => {
        showToast('Error al cargar el jugador', 'danger');
        console.error(error);
    });
}

function createPlayer(playerData) {
    fetch('/Player/Create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(playerData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Jugador creado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('playerModal')).hide();
        loadPlayers();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function updatePlayer(playerData) {
    fetch('/Player/Update', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(playerData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        return response.json();
    })
    .then(() => {
        showToast('Jugador actualizado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('playerModal')).hide();
        loadPlayers();
    })
    .catch(error => {
        handleValidationErrors(error);
    });
}

function deletePlayer(id) {
    deletingPlayerId = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

function confirmDelete() {
    fetch(`/Player/Delete?id=${deletingPlayerId}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => Promise.reject(err));
        }
        showToast('Jugador eliminado exitosamente', 'success');
        bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
        loadPlayers();
    })
    .catch(error => {
        handleValidationErrors(error, 'Error al eliminar el jugador');
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
