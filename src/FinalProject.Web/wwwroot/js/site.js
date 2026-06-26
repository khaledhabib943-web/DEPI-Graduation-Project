// ===== Dark Mode Toggle =====
function toggleDarkMode() {
    const html = document.documentElement;
    if (html.classList.contains('dark')) {
        html.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    } else {
        html.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    }
}

// Apply saved theme on load
(function () {
    const saved = localStorage.getItem('theme');
    if (saved === 'dark') {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
})();

// ===== Mobile Sidebar Toggle =====
function toggleSidebar() {
    const sidebar = document.getElementById('mobileSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar && overlay) {
        sidebar.classList.toggle('active');
        overlay.classList.toggle('active');
    }
}

function closeSidebar() {
    const sidebar = document.getElementById('mobileSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar && overlay) {
        sidebar.classList.remove('active');
        overlay.classList.remove('active');
    }
}

// ===== Language Toggle =====
// Maps EN action names to AR action names and vice versa.
// Routes follow: /Controller/Action (EN) <-> /Controller/ActionAr (AR)
// Language preference is stored in a cookie (salahly_lang) for persistence across page navigations.
function setCookieLang(lang) {
    var maxAge = 365 * 24 * 60 * 60; // 1 year in seconds
    document.cookie = 'salahly_lang=' + lang + '; path=/; max-age=' + maxAge + '; SameSite=Lax';
}

function switchLanguage(targetLang) {
    // Persist the language choice
    setCookieLang(targetLang);

    let path = window.location.pathname;

    // Handle root URL
    if (path === '/' || path === '') {
        if (targetLang === 'ar') {
            window.location.href = '/Home/IndexAr';
        } else {
            window.location.href = '/';
        }
        return;
    }

    if (targetLang === 'ar') {
        // EN -> AR: append "Ar" to the action name
        if (path.endsWith('Ar')) return; // already Arabic

        var segments = path.split('/');
        if (segments.length >= 3) {
            segments[2] = segments[2] + 'Ar';
        } else if (segments.length === 2) {
            segments.push('IndexAr');
        }
        window.location.href = segments.join('/') + window.location.search;

    } else {
        // AR -> EN: remove "Ar" suffix from the action name
        var segments = path.split('/');
        if (segments.length >= 3 && segments[2].endsWith('Ar')) {
            segments[2] = segments[2].slice(0, -2);
        }

        var newPath = segments.join('/');
        if (segments.length >= 3 && segments[2] === '') {
            segments[2] = 'Index';
            newPath = segments.join('/');
        }

        window.location.href = newPath + window.location.search;
    }
}

// ===== Filter Panel Toggle =====
function toggleFilterPanel() {
    const panel = document.getElementById('filterPanel');
    if (panel) {
        panel.classList.toggle('hidden');
    }
}
