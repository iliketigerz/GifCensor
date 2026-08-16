(function () {
    // --- Body setup ---
    document.body.style.margin = '0';
    document.body.style.overflow = 'hidden';

    // --- Controls ---
    const controls = document.createElement('div');
    controls.style.position = 'fixed';
    controls.style.top = '0';
    controls.style.left = '0';
    controls.style.right = '0';
    controls.style.height = '40px';
    controls.style.background = 'rgba(0,0,0,0.8)';
    controls.style.display = 'flex';
    controls.style.alignItems = 'center';
    controls.style.padding = '0 10px';
    controls.style.zIndex = '10000';
    controls.style.gap = '10px';

    const paintBtn = document.createElement('button'); paintBtn.textContent = 'Paint Mode';
    const rectBtn = document.createElement('button'); rectBtn.textContent = 'Rectangle Mode';
    const floodBtn = document.createElement('button'); floodBtn.textContent = 'Flood Fill Mode';

    const brushLabel = document.createElement('label'); brushLabel.textContent = 'Brush Size:'; brushLabel.style.color = 'white';
    const brushSlider = document.createElement('input'); brushSlider.type = 'range'; brushSlider.min = '1'; brushSlider.max = '100'; brushSlider.value = '20';
    const floodLabel = document.createElement('label'); floodLabel.textContent = 'Sensitivity:'; floodLabel.style.color = 'white'; floodLabel.style.display = 'none';
    const floodSlider = document.createElement('input'); floodSlider.type = 'range'; floodSlider.min = '0'; floodSlider.max = '100'; floodSlider.value = '15'; floodSlider.style.display = 'none';

    controls.append(paintBtn, rectBtn, floodBtn, brushLabel, brushSlider, floodLabel, floodSlider);
    document.body.appendChild(controls);

    // --- Container & wrapper ---
    const container = document.createElement('div');
    container.style.position = 'fixed';
    container.style.top = '40px';
    container.style.left = '0';
    container.style.right = '0';
    container.style.bottom = '0';
    container.style.overflow = 'hidden';
    container.style.background = '#222';
    document.body.appendChild(container);

    const wrapper = document.createElement('div');
    wrapper.style.position = 'absolute';
    wrapper.style.left = '0';
    wrapper.style.top = '0';
    wrapper.style.transformOrigin = '0 0';
    container.appendChild(wrapper);

    // --- Media ---
    let mediaElement = document.createElement('img'); // default media
    mediaElement.src = '8f7.gif';
    mediaElement.style.userSelect = 'none';
    mediaElement.style.pointerEvents = 'none';
    wrapper.appendChild(mediaElement);

    // --- Canvas setup ---
    const overlayCanvas = document.createElement('canvas');
    overlayCanvas.style.position = 'absolute';
    overlayCanvas.style.left = '0';
    overlayCanvas.style.top = '0';
    overlayCanvas.style.zIndex = '9999';
    wrapper.appendChild(overlayCanvas);
    const overlayCtx = overlayCanvas.getContext('2d');

    const maskCanvas = document.createElement('canvas');
    const maskCtx = maskCanvas.getContext('2d');
    maskCtx.fillStyle = 'rgb(255,0,0)';
    maskCtx.strokeStyle = 'rgb(255,0,0)';
    maskCtx.lineCap = 'round';
    maskCtx.lineWidth = parseInt(brushSlider.value);

    const tempCanvas = document.createElement('canvas');
    tempCanvas.style.position = 'absolute';
    tempCanvas.style.left = '0';
    tempCanvas.style.top = '0';
    tempCanvas.style.zIndex = '9998';
    wrapper.appendChild(tempCanvas);
    const tempCtx = tempCanvas.getContext('2d');

    // --- Layout & scaling ---
    let scale = 1, translateX = 0, translateY = 0;

    function resetLayout() {
        const w = mediaElement.videoWidth || mediaElement.naturalWidth;
        const h = mediaElement.videoHeight || mediaElement.naturalHeight;

        overlayCanvas.width = maskCanvas.width = tempCanvas.width = w;
        overlayCanvas.height = maskCanvas.height = tempCanvas.height = h;

        wrapper.style.width = w + 'px';
        wrapper.style.height = h + 'px';
        mediaElement.style.width = w + 'px';
        mediaElement.style.height = h + 'px';

        // --- Center & scale media in container ---
        const rect = container.getBoundingClientRect();
        const scaleX = rect.width / w;
        const scaleY = rect.height / h;
        scale = Math.min(scaleX, scaleY);
        const scaledWidth = w * scale;
        const scaledHeight = h * scale;
        translateX = (rect.width - scaledWidth) / 2;
        translateY = (rect.height - scaledHeight) / 2;
        applyTransform();
    }

    function applyTransform() {
        const w = overlayCanvas.width * scale;
        const h = overlayCanvas.height * scale;
        wrapper.style.width = w + 'px';
        wrapper.style.height = h + 'px';
        wrapper.style.left = translateX + 'px';
        wrapper.style.top = translateY + 'px';
        mediaElement.style.width = w + 'px';
        mediaElement.style.height = h + 'px';
        overlayCanvas.style.width = w + 'px';
        overlayCanvas.style.height = h + 'px';
        tempCanvas.style.width = w + 'px';
        tempCanvas.style.height = h + 'px';
    }

    // --- Pan / zoom ---
    container.addEventListener('wheel', e => {
        e.preventDefault();
        const rect = container.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;
        const delta = e.deltaY < 0 ? 1.1 : 0.9;
        const newScale = Math.min(10, Math.max(0.1, scale * delta));
        translateX -= (mouseX - translateX) * (newScale / scale - 1);
        translateY -= (mouseY - translateY) * (newScale / scale - 1);
        scale = newScale;
        applyTransform();
    }, { passive: false });

    let isPanning = false, panStartX = 0, panStartY = 0;
    container.addEventListener('mousedown', e => { if (e.button === 1) { isPanning = true; panStartX = e.clientX - translateX; panStartY = e.clientY - translateY; } });
    container.addEventListener('mouseup', e => { if (e.button === 1) isPanning = false; });
    container.addEventListener('mouseleave', () => { if (isPanning) isPanning = false; });
    container.addEventListener('mousemove', e => { if (isPanning) { translateX = e.clientX - panStartX; translateY = e.clientY - panStartY; applyTransform(); } });

    // --- Drawing tools ---
    let mode = 'paint', drawing = false, startX = 0, startY = 0, isErasing = false;
    brushSlider.oninput = () => { maskCtx.lineWidth = parseInt(brushSlider.value); };

    function renderOverlay() {
        overlayCtx.clearRect(0, 0, overlayCanvas.width, overlayCanvas.height);
        overlayCtx.globalAlpha = 0.4;
        overlayCtx.drawImage(maskCanvas, 0, 0);
        overlayCtx.globalAlpha = 1.0;
    }

    function applyMaskAlpha(alpha = 0.8) {
        const imgData = maskCtx.getImageData(0, 0, maskCanvas.width, maskCanvas.height);
        const data = imgData.data;
        for (let i = 0; i < data.length; i += 4) if (data[i + 3] !== 0) data[i + 3] = alpha * 255;
        maskCtx.putImageData(imgData, 0, 0);
    }

    overlayCanvas.addEventListener('mousedown', e => {
        if (e.button === 1) return;
        const rect = overlayCanvas.getBoundingClientRect();
        startX = (e.clientX - rect.left) / scale;
        startY = (e.clientY - rect.top) / scale;

        if (mode === 'flood') { floodFill(startX, startY, parseInt(floodSlider.value)); return; }

        drawing = true;
        isErasing = (e.button === 2);
        if (mode === 'paint') {
            maskCtx.beginPath();
            maskCtx.moveTo(startX, startY);
            maskCtx.globalCompositeOperation = isErasing ? 'destination-out' : 'source-over';
            maskCtx.strokeStyle = isErasing ? 'rgba(0,0,0,1)' : 'rgb(255,0,0)';
        }
    });

    document.addEventListener('mouseup', e => {
        if (!drawing) return;
        drawing = false;
        const rect = overlayCanvas.getBoundingClientRect();
        const endX = (e.clientX - rect.left) / scale;
        const endY = (e.clientY - rect.top) / scale;

        if (mode === 'rect') {
            maskCtx.fillStyle = 'rgb(255,0,0)';
            maskCtx.fillRect(Math.min(startX, endX), Math.min(startY, endY), Math.abs(endX - startX), Math.abs(endY - startY));
            tempCtx.clearRect(0, 0, tempCanvas.width, tempCanvas.height);
        } else {
            maskCtx.closePath();
        }
        maskCtx.globalCompositeOperation = 'source-over';
        applyMaskAlpha(0.8);
        renderOverlay();
    });

    document.addEventListener('mousemove', e => {
        if (!drawing) return;
        const rect = overlayCanvas.getBoundingClientRect();
        const x = (e.clientX - rect.left) / scale;
        const y = (e.clientY - rect.top) / scale;

        if (mode === 'paint') {
            maskCtx.beginPath();
            maskCtx.arc(x, y, parseInt(brushSlider.value) / 2, 0, Math.PI * 2);
            maskCtx.fillStyle = 'rgb(255,0,0)';
            maskCtx.globalAlpha = 1.0;
            maskCtx.fill();
            renderOverlay();
        } else if (mode === 'rect') {
            tempCtx.clearRect(0, 0, tempCanvas.width, tempCanvas.height);
            tempCtx.strokeStyle = 'rgba(255,0,0,0.8)';
            tempCtx.lineWidth = 2;
            tempCtx.setLineDash([6]);
            tempCtx.strokeRect(Math.min(startX, x), Math.min(startY, y), Math.abs(x - startX), Math.abs(y - startY));
        }
    });

    overlayCanvas.addEventListener('contextmenu', e => e.preventDefault());

    // --- Modes ---
    paintBtn.onclick = () => { mode = 'paint'; paintBtn.disabled = true; rectBtn.disabled = false; floodBtn.disabled = false; floodSlider.style.display = 'none'; floodLabel.style.display = 'none'; };
    rectBtn.onclick = () => { mode = 'rect'; rectBtn.disabled = true; paintBtn.disabled = false; floodBtn.disabled = false; floodSlider.style.display = 'none'; floodLabel.style.display = 'none'; };
    floodBtn.onclick = () => { mode = 'flood'; floodBtn.disabled = true; paintBtn.disabled = false; rectBtn.disabled = false; floodSlider.style.display = 'inline'; floodLabel.style.display = 'inline'; };
    paintBtn.disabled = true;

    function floodFill(startX, startY, tolerance = 15) {
    startX |= 0;
    startY |= 0;

    const w = maskCanvas.width;
    const h = maskCanvas.height;

    // Draw current frame to an offscreen canvas (WebView-safe)
    const srcCanvas = document.createElement('canvas');
    srcCanvas.width = w;
    srcCanvas.height = h;
    const srcCtx = srcCanvas.getContext('2d');
    srcCtx.drawImage(mediaElement, 0, 0, w, h);

    let srcData;
    try {
        srcData = srcCtx.getImageData(0, 0, w, h);
    } catch (e) {
        console.error('Flood fill blocked by tainted canvas', e);
        return;
    }

    const src = srcData.data;

    const maskData = maskCtx.getImageData(0, 0, w, h);
    const mask = maskData.data;

    const idx0 = (startY * w + startX) * 4;
    const r0 = src[idx0];
    const g0 = src[idx0 + 1];
    const b0 = src[idx0 + 2];

    const tolSq = tolerance * tolerance;

    const visited = new Uint8Array(w * h);
    const stack = [[startX, startY]];

    while (stack.length) {
        const [x, y] = stack.pop();
        if (x < 0 || y < 0 || x >= w || y >= h) continue;

        const p = y * w + x;
        if (visited[p]) continue;
        visited[p] = 1;

        const i = p * 4;
        const dr = src[i] - r0;
        const dg = src[i + 1] - g0;
        const db = src[i + 2] - b0;

        if ((dr * dr + dg * dg + db * db) > tolSq) continue;

        // Write solid red into mask buffer
        mask[i] = 255;
        mask[i + 1] = 0;
        mask[i + 2] = 0;
        mask[i + 3] = 255;

        stack.push(
            [x + 1, y],
            [x - 1, y],
            [x, y + 1],
            [x, y - 1]
        );
    }

    maskCtx.putImageData(maskData, 0, 0);
    applyMaskAlpha(0.8);
    renderOverlay();
}


    // --- WebView2 integration ---
    window.sendMaskToHost = () => {
        const outputCanvas = document.createElement('canvas');
        outputCanvas.width = maskCanvas.width; outputCanvas.height = maskCanvas.height;
        const outputCtx = outputCanvas.getContext('2d');
        outputCtx.drawImage(maskCanvas, 0, 0);
        const data = outputCanvas.toDataURL('image/png');
        window.chrome.webview.postMessage({ maskData: data });
    };

    window.receiveBitmapFromHost = base64Png => {
        const img = new Image();
        img.onload = () => { maskCtx.clearRect(0, 0, maskCanvas.width, maskCanvas.height); maskCtx.drawImage(img, 0, 0); renderOverlay(); };
        img.src = 'data:image/png;base64,' + base64Png;
    };

    window.pickPixelColor = () => {
        const imgCanvas = document.createElement('canvas'); imgCanvas.width = maskCanvas.width; imgCanvas.height = maskCanvas.height;
        const imgCtx = imgCanvas.getContext('2d'); imgCtx.drawImage(mediaElement, 0, 0);
        const onClick = e => {
            const rect = container.getBoundingClientRect();
            const x = (e.clientX - rect.left - translateX) / scale;
            const y = (e.clientY - rect.top - translateY) / scale;
            const pixel = imgCtx.getImageData(Math.floor(x), Math.floor(y), 1, 1).data;
            window.chrome.webview.postMessage({ pickedColor: { r: pixel[0], g: pixel[1], b: pixel[2] } });
            container.removeEventListener('click', onClick);
        };
        container.addEventListener('click', onClick);
    };

    // --- Media switching ---
    window.showMedia = path => {
        if (mediaElement) wrapper.removeChild(mediaElement);
        const ext = path.split('.').pop().toLowerCase();
        if (['mp4','webm','ogg'].includes(ext)) { 
            mediaElement = document.createElement('video'); 
            mediaElement.src = path; 
            mediaElement.autoplay = true; 
            mediaElement.loop = true; 
            mediaElement.muted = true; 
        }
        else { 
            mediaElement = document.createElement('img'); 
            mediaElement.src = path; 
        }
        mediaElement.style.userSelect='none'; 
        mediaElement.style.pointerEvents='none';
        wrapper.insertBefore(mediaElement, overlayCanvas);
        mediaElement.onload = mediaElement.onloadedmetadata = () => {
            resetLayout();
        };
    };

    // --- Initial layout ---
    mediaElement.onload = mediaElement.onloadedmetadata = resetLayout;
})();
