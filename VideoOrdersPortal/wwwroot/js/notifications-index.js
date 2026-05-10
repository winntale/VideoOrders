(() => {
    const STORAGE_KEY = 'notif:lastSeenAtUtc';
    const tbody = document.getElementById('notifications-body');
    const markBtn = document.getElementById('mark-read');

    const MSK = new Intl.DateTimeFormat('ru-RU', {
        timeZone: 'Europe/Moscow',
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit', second: '2-digit'
    });
    const fmt = iso => MSK.format(new Date(iso));
    const escape = s => (s ?? '').toString().replace(/[&<>"']/g, c => ({
        '&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'
    })[c]);

    const TYPE_LABELS = {
        OrderCompleted: 'Готово',
        OrderFailed: 'Ошибка',
        ResourceReservationFailed: 'Нет ресурсов'
    };

    function getLastSeenMs() {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (!raw) return 0;
        const t = Date.parse(raw);
        return Number.isFinite(t) ? t : 0;
    }

    let lastList = [];

    function updateMarkBtn(unread) {
        markBtn.disabled = unread === 0;
        markBtn.textContent = unread > 0
            ? `Отметить все прочитанными (${unread})`
            : 'Все прочитано';
    }

    function render(list) {
        lastList = list;
        if (list.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="muted">Уведомлений пока нет.</td></tr>';
            updateMarkBtn(0);
            return;
        }

        const lastSeenMs = getLastSeenMs();
        let unread = 0;

        tbody.innerHTML = list.map(n => {
            const isUnread = Date.parse(n.createdAtUtc) > lastSeenMs;
            if (isUnread) unread++;
            return `
                <tr class="${isUnread ? 'unread' : ''}">
                    <td>${escape(fmt(n.createdAtUtc))}${isUnread ? ' <span class="unread-dot" title="Новое"></span>' : ''}</td>
                    <td><span class="status-pill ${escape((n.type || '').toLowerCase())}">${escape(TYPE_LABELS[n.type] || n.type)}</span></td>
                    <td>${escape(n.cameraName || '—')}</td>
                    <td>${escape(n.message)}</td>
                </tr>`;
        }).join('');

        updateMarkBtn(unread);
    }

    async function load() {
        try {
            const res = await fetch('/api/notifications');
            if (!res.ok) throw new Error('HTTP ' + res.status);
            const list = await res.json();
            render(list);
        } catch (e) {
            tbody.innerHTML = `<tr><td colspan="4" class="muted">Ошибка: ${escape(e.message)}</td></tr>`;
        }
    }

    markBtn.addEventListener('click', () => {
        const now = new Date().toISOString();
        localStorage.setItem(STORAGE_KEY, now);
        document.dispatchEvent(new CustomEvent('notifications:mark-read'));
        render(lastList);
    });

    load();
    setInterval(load, 5000);
})();
