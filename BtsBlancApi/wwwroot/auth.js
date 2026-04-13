const API = 'https://localhost:7056/api';

const loginForm = document.getElementById('login-form');
const registerForm = document.getElementById('register-form');
const errorMsg = document.getElementById('error-msg');
const successMsg = document.getElementById('success-msg');

function showError(msg) {
    errorMsg.textContent = msg;
    errorMsg.classList.remove('hidden');
}

if (loginForm) {
    if (localStorage.getItem('token')) window.location.href = 'evenements.html';

    loginForm.addEventListener('submit', async e => {
        e.preventDefault();
        errorMsg.classList.add('hidden');
        const res = await fetch(`${API}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                email: document.getElementById('email').value,
                password: document.getElementById('password').value
            })
        });
        if (!res.ok) return showError('Email ou mot de passe incorrect.');
        const data = await res.json();
        localStorage.setItem('token', data.token);
        const payload = JSON.parse(atob(data.token.split('.')[1]));
        localStorage.setItem('username', payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']);
        localStorage.setItem('role', payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
        window.location.href = 'evenements.html';
    });
}

if (registerForm) {
    registerForm.addEventListener('submit', async e => {
        e.preventDefault();
        errorMsg.classList.add('hidden');
        successMsg.classList.add('hidden');
        const res = await fetch(`${API}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                username: document.getElementById('username').value,
                email: document.getElementById('email').value,
                password: document.getElementById('password').value
            })
        });
        if (!res.ok) {
            const msg = await res.text();
            return showError(msg);
        }
        successMsg.textContent = 'Compte créé ! Vous pouvez vous connecter.';
        successMsg.classList.remove('hidden');
        registerForm.reset();
    });
}
