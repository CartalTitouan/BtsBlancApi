const API = 'https://localhost:7056/api';
const token = localStorage.getItem('token');
const role = localStorage.getItem('role');
const username = localStorage.getItem('username');

if (!token) window.location.href = 'index.html';

document.getElementById('nav-username').textContent = `${username} (${role})`;

const TYPES = ['Salon professionnel', 'Conférence', 'Journées thématiques'];

function authHeaders() {
    return { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` };
}

function logout() {
    localStorage.clear();
    window.location.href = 'index.html';
}

function showCreateError(msg) {
    const el = document.getElementById('create-error');
    el.textContent = msg;
    el.classList.remove('hidden');
    setTimeout(() => el.classList.add('hidden'), 4000);
}

function formatDate(dateStr) {
    return new Date(dateStr).toLocaleString('fr-FR', {
        day: '2-digit', month: 'long', year: 'numeric',
        hour: '2-digit', minute: '2-digit'
    });
}

let inscriptions = new Set();

async function loadInscriptions(evenements) {
    inscriptions.clear();
    await Promise.all(evenements.map(async e => {
        const res = await fetch(`${API}/inscription/evenement/${e.id}`, { headers: authHeaders() });
        if (!res.ok) return;
        const inscrits = await res.json();
        const userId = getUserId();
        if (inscrits.some(i => i.id === userId)) inscriptions.add(e.id);
    }));
}

function getUserId() {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
}

async function loadEvenements() {
    const res = await fetch(`${API}/evenement`, { headers: authHeaders() });
    const evenements = await res.json();
    await loadInscriptions(evenements);
    renderEvenements(evenements);
}

function renderEvenements(evenements) {
    const container = document.getElementById('events-list');
    if (evenements.length === 0) {
        container.innerHTML = '<p style="color:var(--text-muted)">Aucun événement pour le moment.</p>';
        return;
    }
    container.innerHTML = evenements.map(e => `
        <div class="event-card">
            <div class="event-badge">${TYPES[e.type]}</div>
            <h3 class="${inscriptions.has(e.id) ? 'inscrit' : ''}">${e.titre}</h3>
            <p class="event-contenu">${e.contenu}</p>
            <div class="event-meta">
                <span>📅 ${formatDate(e.date)}</span>
                <span>📍 ${e.lieu}</span>
            </div>
            <div class="event-actions">
                ${inscriptions.has(e.id)
                    ? `<button class="btn btn-secondary" onclick="desinscrire(${e.id})">Se désinscrire</button>`
                    : `<button class="btn btn-success" onclick="inscrire(${e.id})">S'inscrire</button>`
                }
                <button class="btn-trash" onclick="supprimerEvenement(${e.id})" title="Supprimer">🗑️</button>
            </div>
        </div>
    `).join('');
}

async function inscrire(evenementId) {
    const res = await fetch(`${API}/inscription/${evenementId}`, { method: 'POST', headers: authHeaders() });
    if (res.ok) { inscriptions.add(evenementId); loadEvenements(); }
}

async function desinscrire(evenementId) {
    const res = await fetch(`${API}/inscription/${evenementId}`, { method: 'DELETE', headers: authHeaders() });
    if (res.ok) { inscriptions.delete(evenementId); loadEvenements(); }
}

async function supprimerEvenement(id) {
    if (role !== 'Admin') {
        const err = document.getElementById('events-error');
        err.textContent = 'Accès refusé : seuls les admins peuvent supprimer un événement.';
        err.classList.remove('hidden');
        setTimeout(() => err.classList.add('hidden'), 4000);
        return;
    }
    if (!confirm('Supprimer cet événement ?')) return;
    const res = await fetch(`${API}/evenement/${id}`, { method: 'DELETE', headers: authHeaders() });
    if (res.ok) loadEvenements();
}

document.getElementById('create-form').addEventListener('submit', async e => {
    e.preventDefault();
    document.getElementById('create-error').classList.add('hidden');
    const body = {
        titre: document.getElementById('titre').value,
        contenu: document.getElementById('contenu').value,
        date: new Date(document.getElementById('date').value).toISOString(),
        lieu: document.getElementById('lieu').value,
        type: parseInt(document.getElementById('type').value)
    };
    const res = await fetch(`${API}/evenement`, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify(body)
    });
    if (!res.ok) {
        showCreateError(res.status === 403 ? 'Accès refusé : seuls les admins peuvent créer un événement.' : 'Erreur lors de la création.');
        return;
    }
    document.getElementById('create-form').reset();
    loadEvenements();
});

loadEvenements();
