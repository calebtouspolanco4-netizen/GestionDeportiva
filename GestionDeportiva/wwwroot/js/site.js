// ============================================================
// La Jugada — site.js
// ============================================================

// Mark nav-items active based on current URL
(function () {
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-item').forEach(el => {
        const href = (el.getAttribute('href') || '').toLowerCase();
        if (href && href !== '/' && path.startsWith(href)) {
            el.classList.add('active');
        }
    });
})();

// Currency formatter (Colombian pesos)
function formatCOP(value) {
    return '$' + Number(value).toLocaleString('es-CO');
}

// Generic toast notification
function showToast(msg, type = 'success') {
    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: type,
        title: msg,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        background: '#252830',
        color: '#fff'
    });
}
