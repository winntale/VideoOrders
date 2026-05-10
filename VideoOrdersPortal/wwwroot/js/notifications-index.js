(() => {
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

    async function load() {
        try {
            const res = await fetch('/api/notifications');
            if (!res.ok) throw new Error('HTTP ' + res.status);
            const list = await res.json();
            if (list.length === 0) {
                tbody.innerHTML = '<tr><td colspan="5" class="muted">Уведомлений пока нет.</td></tr>';
                return;
            }
            tbody.innerHTML = list.map(n => `
                <tr>
                    <td>${escape(fmt(n.createdAtUtc))}</td>
                    <td><span class="status-pill ${escape((n.type || '').toLowerCase())}">${escape(TYPE_LABELS[n.type] || n.type)}</span></td>
                    <td>${escape(n.cameraName || '—')}</td>
                    <td>${escape(n.message)}</td>
                    <td><code class="order-id" title="${escape(n.orderId)}">${escape(n.orderId.slice(0, 8))}…</code></td>
                </tr>
            `).join('');
        } catch (e) {
            tbody.innerHTML = `<tr><td colspan="5" class="muted">Ошибка: ${escape(e.message)}</td></tr>`;
        }
    }

    markBtn.addEventListener('click', () => {
        document.dispatchEvent(new CustomEvent('notifications:mark-read'));
    });

    load();
    setInterval(load, 5000);
})();
