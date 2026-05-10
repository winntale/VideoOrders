(() => {
    const cameraList = document.getElementById('camera-list');
    const fromInput = document.getElementById('from-utc');
    const toInput = document.getElementById('to-utc');
    const accessStatus = document.getElementById('access-status');
    const archiveStatus = document.getElementById('archive-status');
    const submitBtn = document.getElementById('submit');
    const submitStatus = document.getElementById('submit-status');

    let selectedCamera = null;
    let accessOk = false;
    let archiveOk = false;

    function setStatus(el, text, ok) {
        el.textContent = text;
        el.className = 'status ' + (text ? (ok ? 'ok' : 'bad') : '');
    }

    function updateSubmit() {
        submitBtn.disabled = !(selectedCamera && accessOk && archiveOk && fromInput.value && toInput.value);
    }

    // Inputs are interpreted as UTC+3 wall-clock; we convert to absolute UTC ISO before sending.
    function toIsoUtc(local) {
        if (!local) return null;
        return new Date(local + '+03:00').toISOString();
    }

    async function loadCameras() {
        try {
            const res = await fetch('/api/cameras');
            if (!res.ok) throw new Error('HTTP ' + res.status);
            const cameras = await res.json();
            cameraList.innerHTML = '';
            if (cameras.length === 0) {
                cameraList.innerHTML = '<li class="muted">Камеры не найдены. Положите файлы в ./test-videos.</li>';
                return;
            }
            for (const cam of cameras) {
                const li = document.createElement('li');
                li.dataset.id = cam.id;
                li.innerHTML = `<span>${cam.name}</span><span class="size">${(cam.fileSize / (1024*1024)).toFixed(1)} MB</span>`;
                li.addEventListener('click', () => selectCamera(cam, li));
                cameraList.appendChild(li);
            }
        } catch (e) {
            cameraList.innerHTML = `<li class="muted">Ошибка загрузки: ${e.message}</li>`;
        }
    }

    async function selectCamera(cam, li) {
        selectedCamera = cam;
        for (const item of cameraList.children) item.classList.remove('selected');
        li.classList.add('selected');
        setStatus(accessStatus, 'Проверка доступа…', true);
        accessOk = false;
        updateSubmit();
        try {
            const res = await fetch('/api/orders/validate-access', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ cameraId: cam.id })
            });
            const data = await res.json();
            accessOk = !!data.isAllowed;
            setStatus(accessStatus, accessOk ? 'Доступ к камере есть' : ('Нет доступа: ' + (data.denyReason || 'unknown')), accessOk);
        } catch (e) {
            setStatus(accessStatus, 'Ошибка: ' + e.message, false);
        }
        await checkArchive();
        updateSubmit();
    }

    async function checkArchive() {
        if (!selectedCamera || !fromInput.value || !toInput.value) {
            setStatus(archiveStatus, '', true);
            archiveOk = false;
            updateSubmit();
            return;
        }
        const fromUtc = toIsoUtc(fromInput.value);
        const toUtc = toIsoUtc(toInput.value);
        if (new Date(toUtc) <= new Date(fromUtc)) {
            archiveOk = false;
            setStatus(archiveStatus, 'Конец интервала должен быть позже начала', false);
            updateSubmit();
            return;
        }
        setStatus(archiveStatus, 'Проверка наличия архива…', true);
        archiveOk = false;
        updateSubmit();
        try {
            const res = await fetch('/api/orders/validate-archive', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ cameraId: selectedCamera.id, fromUtc, toUtc })
            });
            const data = await res.json();
            archiveOk = !!data.isAllowed;
            setStatus(archiveStatus, archiveOk ? 'Архив доступен' : ('Архив недоступен: ' + (data.denyReason || 'unknown')), archiveOk);
        } catch (e) {
            setStatus(archiveStatus, 'Ошибка: ' + e.message, false);
        }
        updateSubmit();
    }

    fromInput.addEventListener('change', checkArchive);
    toInput.addEventListener('change', checkArchive);

    submitBtn.addEventListener('click', async () => {
        submitBtn.disabled = true;
        setStatus(submitStatus, 'Создание заказа…', true);
        try {
            const res = await fetch('/api/orders', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    cameraId: selectedCamera.id,
                    fromUtc: toIsoUtc(fromInput.value),
                    toUtc: toIsoUtc(toInput.value)
                })
            });
            if (!res.ok) {
                const err = await res.text();
                setStatus(submitStatus, 'Не удалось создать: ' + err, false);
                submitBtn.disabled = false;
                return;
            }
            window.location.href = '/Orders';
        } catch (e) {
            setStatus(submitStatus, 'Ошибка: ' + e.message, false);
            submitBtn.disabled = false;
        }
    });

    loadCameras();
})();
