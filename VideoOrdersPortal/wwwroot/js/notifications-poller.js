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

    function getLastSeen() {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (!raw) return 0;
        const t = Date.parse(raw);
        return Number.isFinite(t) ? t : 0;
    }

    function setLastSeen(iso) {
        localStorage.setItem(STORAGE_KEY, iso);
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
            badge.hidden = false;
        } else {
            badge.hidden = true;
        }
    }

    let lastSeenMs = getLastSeen();

    async function tick() {
        try {
            const res = await fetch('/api/notifications', { headers: { 'Accept': 'application/json' } });
            if (!res.ok) return;
            const list = await res.json();

            const fresh = list.filter(n => Date.parse(n.createdAtUtc) > lastSeenMs);

            for (const n of fresh.slice().reverse()) {
                showToast(n);
            }

            setBadge(fresh.length);

            if (fresh.length > 0) {
                lastSeenMs = Math.max(...fresh.map(n => Date.parse(n.createdAtUtc)));
                setLastSeen(new Date(lastSeenMs).toISOString());
            }
        } catch {
            /* network blip — ignore */
        }
    }

    window.addEventListener('focus', () => {
        lastSeenMs = getLastSeen();
        tick();
    });

    document.addEventListener('notifications:mark-read', () => {
        const now = new Date().toISOString();
        setLastSeen(now);
        lastSeenMs = Date.parse(now);
        setBadge(0);
    });

    tick();
    setInterval(tick, POLL_MS);
})();
