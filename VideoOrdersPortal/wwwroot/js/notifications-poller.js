(() => {
    const STORAGE_KEY = 'notif:lastSeenAtUtc';
    const POLL_MS = 5000;
    const TOAST_TTL_MS = 6000;

    const container = document.getElementById('toast-container');
    const badge = document.getElementById('notif-badge');

    const escape = s => (s ?? '').toString().replace(/[&<>"']/g, c => ({
        '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
    })[c]);

    const TYPE_LABELS = {
        OrderCompleted: 'Архив готов',
        OrderFailed: 'Ошибка обработки',
        ResourceReservationFailed: 'Ресурсы недоступны'
    };

    function getLastSeenMs() {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (!raw) return 0;
        const t = Date.parse(raw);
        return Number.isFinite(t) ? t : 0;
    }

    function showToast(notification) {
        const div = document.createElement('div');
        div.className = 'toast toast-' + (notification.type || '').toLowerCase();
        const title = TYPE_LABELS[notification.type] || notification.type || 'Уведомление';
        const camera = notification.cameraName ? escape(notification.cameraName) : '';
        div.innerHTML = `
            <div class="toast-title">${escape(title)}</div>
            <div class="toast-body">${escape(notification.message)}</div>
            ${camera ? `<div class="toast-meta">Камера: ${camera}</div>` : ''}
            <button class="toast-close" aria-label="Закрыть">&times;</button>
        `;
        div.querySelector('.toast-close').addEventListener('click', () => div.remove());
        container.appendChild(div);
        setTimeout(() => {
            div.classList.add('toast-leaving');
            setTimeout(() => div.remove(), 250);
        }, TOAST_TTL_MS);
    }

    function setBadge(count) {
        if (!badge) return;
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : String(count);
            badge.style.display = '';
        } else {
            badge.textContent = '';
            badge.style.display = 'none';
        }
    }

    setBadge(0);

    // Toasts only fire once per id per tab session; unread count is recomputed
    // each tick from localStorage so "mark all read" in another tab is reflected.
    const toastedIds = new Set();

    async function tick() {
        try {
            const res = await fetch('/api/notifications', { headers: { 'Accept': 'application/json' } });
            if (!res.ok) return;
            const list = await res.json();

            const lastSeenMs = getLastSeenMs();
            const unread = list.filter(n => Date.parse(n.createdAtUtc) > lastSeenMs);

            const toToast = unread
                .filter(n => !toastedIds.has(n.id))
                .slice()
                .reverse();
            for (const n of toToast) {
                showToast(n);
                toastedIds.add(n.id);
            }

            setBadge(unread.length);
        } catch {
            /* network blip — ignore */
        }
    }

    window.addEventListener('focus', () => tick());

    document.addEventListener('notifications:mark-read', () => {
        const now = new Date().toISOString();
        localStorage.setItem(STORAGE_KEY, now);
        setBadge(0);
    });

    window.addEventListener('storage', e => {
        if (e.key === STORAGE_KEY) tick();
    });

    tick();
    setInterval(tick, POLL_MS);
})();
