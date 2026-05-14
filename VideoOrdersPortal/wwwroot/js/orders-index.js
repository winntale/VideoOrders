(() => {
    const tbody = document.getElementById('orders-body');
    const dialog = document.getElementById('player-dialog');
    const player = document.getElementById('player');

    // All listing times are shown in Moscow time (UTC+3).
    const MSK_FORMATTER = new Intl.DateTimeFormat('ru-RU', {
        timeZone: 'Europe/Moscow',
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit'
    });
    const fmtDate = iso => MSK_FORMATTER.format(new Date(iso));
    const fmtSize = bytes => bytes ? (bytes / (1024*1024)).toFixed(1) + ' MB' : '—';
    const statusClass = s => 'status-pill ' + (s || '').toLowerCase();
    const STATUS_LABELS = {
        Created: 'Создан',
        Pending: 'В ожидании',
        InProgress: 'В обработке',
        Processing: 'В обработке',
        ResourceReserved: 'Ресурсы зарезервированы',
        ResourceReservationFailed: 'Нет ресурсов',
        ProcessingStarted: 'В обработке',
        Completed: 'Готов',
        Failed: 'Ошибка'
    };
    const statusLabel = s => STATUS_LABELS[s] ?? s ?? '';
    const escape = s => (s ?? '').toString().replace(/[&<>"']/g, c => ({
        '&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'
    })[c]);

    let polling = null;
    const PENDING = new Set(['Created','Pending','InProgress','Processing','ResourceReserved','ProcessingStarted']);

    async function load() {
        try {
            const res = await fetch('/api/orders');
            if (!res.ok) throw new Error('Ошибка сети: ' + res.status);
            const orders = await res.json();
            render(orders);
            const hasPending = orders.some(o => PENDING.has(o.status));
            if (hasPending && !polling) polling = setInterval(load, 4000);
            if (!hasPending && polling) { clearInterval(polling); polling = null; }
        } catch (e) {
            tbody.innerHTML = `<tr><td colspan="5" class="muted">Ошибка: ${escape(e.message)}</td></tr>`;
        }
    }

    function render(orders) {
        if (orders.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" class="muted">Заказов пока нет.</td></tr>';
            return;
        }
        tbody.innerHTML = orders.map(o => {
            const ready = o.archiveFile?.isReady;
            const actions = ready ? `
                <a href="/api/orders/${o.id}/download">Скачать</a>
                &nbsp;|&nbsp;
                <a href="#" data-stream="${o.id}">Посмотреть</a>
            ` : (o.status === 'Failed' ? `<span class="muted" title="${escape(o.failureReason || '')}">${escape(o.failureReason || 'Ошибка')}</span>` : '<span class="muted">…</span>');

            return `<tr>
                <td>${escape(o.cameraName || o.cameraId)}</td>
                <td>${fmtDate(o.fromUtc)} → ${fmtDate(o.toUtc)}</td>
                <td><span class="${statusClass(o.status)}">${escape(statusLabel(o.status))}</span></td>
                <td>${fmtSize(o.archiveFile?.fileSize)}</td>
                <td>${actions}</td>
            </tr>`;
        }).join('');

        tbody.querySelectorAll('a[data-stream]').forEach(a => {
            a.addEventListener('click', e => {
                e.preventDefault();
                player.src = `/api/orders/${a.dataset.stream}/stream`;
                dialog.showModal();
            });
        });
    }

    dialog.addEventListener('close', () => { player.pause(); player.src = ''; });

    load();
})();
