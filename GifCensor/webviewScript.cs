using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GifCensor
{
    public static class webviewScript
    {
        public static readonly string Script = @"
            (function () {
                const img = document.querySelector('img');
                //if (!img) return;

                // --- Setup container and wrapper ---
                document.body.style.margin = '0';
                document.body.style.overflow = 'hidden';

                // Create fixed control panel at top
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

                const paintBtn = document.createElement('button');
                paintBtn.textContent = 'Paint Mode';
                const rectBtn = document.createElement('button');
                rectBtn.textContent = 'Rectangle Mode';
                const floodBtn = document.createElement('button');
                floodBtn.textContent = 'Flood Fill Mode';

                const brushLabel = document.createElement('label');
                brushLabel.textContent = 'Brush Size:';
                brushLabel.style.color = 'white';
                const brushSlider = document.createElement('input');
                brushSlider.type = 'range';
                brushSlider.min = '1';
                brushSlider.max = '100';
                brushSlider.value = '20';

                const floodLabel = document.createElement('label');
                floodLabel.textContent = 'Sensitivity:';
                floodLabel.style.color = 'white';
                floodLabel.style.display = 'none';
                const floodSlider = document.createElement('input');
                floodSlider.type = 'range';
                floodSlider.min = '0';
                floodSlider.max = '100';
                floodSlider.value = '15';
                floodSlider.style.display = 'none';

                controls.appendChild(paintBtn);
                controls.appendChild(rectBtn);
                controls.appendChild(floodBtn);
                controls.appendChild(brushLabel);
                controls.appendChild(brushSlider);
                controls.appendChild(floodLabel);
                controls.appendChild(floodSlider);
                document.body.appendChild(controls);

                // Create main container below controls
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

                wrapper.appendChild(img);
                img.style.display = 'block';
                img.style.userSelect = 'none';
                img.style.pointerEvents = 'none';

                const naturalWidth = img.naturalWidth;
                const naturalHeight = img.naturalHeight;

                const canvas = document.createElement('canvas');
                canvas.width = naturalWidth;
                canvas.height = naturalHeight;
                canvas.style.position = 'absolute';
                canvas.style.left = '0';
                canvas.style.top = '0';
                canvas.style.zIndex = '9999';
                wrapper.appendChild(canvas);
                const ctx = canvas.getContext('2d');

                const maskCanvas = document.createElement('canvas');
                maskCanvas.width = naturalWidth;
                maskCanvas.height = naturalHeight;
                const maskCtx = maskCanvas.getContext('2d');
                maskCtx.fillStyle = 'rgb(255,0,0)';
                maskCtx.strokeStyle = 'rgb(255,0,0)';
                maskCtx.lineWidth = parseInt(brushSlider.value);
                maskCtx.lineCap = 'round';

                const tempCanvas = document.createElement('canvas');
                tempCanvas.width = naturalWidth;
                tempCanvas.height = naturalHeight;
                tempCanvas.style.position = 'absolute';
                tempCanvas.style.left = '0';
                tempCanvas.style.top = '0';
                tempCanvas.style.zIndex = '9998';
                wrapper.appendChild(tempCanvas);
                const tempCtx = tempCanvas.getContext('2d');

                wrapper.style.width = naturalWidth + 'px';
                wrapper.style.height = naturalHeight + 'px';

                let scale = 1;
                let translateX = 0;
                let translateY = 0;

                function applyTransform() {
                    const scaledWidth = naturalWidth * scale;
                    const scaledHeight = naturalHeight * scale;
                    wrapper.style.width = `${scaledWidth}px`;
                    wrapper.style.height = `${scaledHeight}px`;
                    wrapper.style.left = `${translateX}px`;
                    wrapper.style.top = `${translateY}px`;
                    img.style.width = `${scaledWidth}px`;
                    img.style.height = `${scaledHeight}px`;
                    canvas.style.width = `${scaledWidth}px`;
                    canvas.style.height = `${scaledHeight}px`;
                    tempCanvas.style.width = `${scaledWidth}px`;
                    tempCanvas.style.height = `${scaledHeight}px`;
                }
                requestAnimationFrame(() => {
                    const rect = container.getBoundingClientRect();
                    const margin = 10;
                    const availableWidth = rect.width - margin * 2;
                    const availableHeight = rect.height - margin * 2;
                    const scaleX = availableWidth / naturalWidth;
                    const scaleY = availableHeight / naturalHeight;
                    scale = Math.min(scaleX, scaleY);
                    const scaledWidth = naturalWidth * scale;
                    const scaledHeight = naturalHeight * scale;
                    translateX = (rect.width - scaledWidth) / 2;
                    translateY = (rect.height - scaledHeight) / 2;
                    applyTransform();
                });

                let isPanning = false;
                let panStartX = 0;
                let panStartY = 0;

                container.addEventListener('wheel', e => {
                    e.preventDefault();
                    const MIN_SCALE = 0.1;
                    const MAX_SCALE = 10;
                    const delta = e.deltaY < 0 ? 1.1 : 0.9;
                    let newScale = scale * delta;
                    newScale = Math.min(MAX_SCALE, Math.max(MIN_SCALE, newScale));
                    const rect = container.getBoundingClientRect();
                    const offsetX = e.clientX - rect.left;
                    const offsetY = e.clientY - rect.top;
                    const x = (offsetX - translateX) / scale;
                    const y = (offsetY - translateY) / scale;
                    scale = newScale;
                    translateX = offsetX - x * scale;
                    translateY = offsetY - y * scale;
                    applyTransform();
                }, { passive: false });

                container.addEventListener('mousedown', e => {
                    if (e.button === 1) {
                        isPanning = true;
                        panStartX = e.clientX - translateX;
                        panStartY = e.clientY - translateY;
                        e.preventDefault();
                    }
                });
                container.addEventListener('mouseup', e => { if (e.button === 1) isPanning = false; });
                container.addEventListener('mouseleave', () => { if (isPanning) isPanning = false; });
                container.addEventListener('mousemove', e => {
                    if (isPanning) {
                        translateX = e.clientX - panStartX;
                        translateY = e.clientY - panStartY;
                        applyTransform();
                    }
                });

                let mode = 'paint';
                let drawing = false;
                let isErasing = false;
                let startX = 0;
                let startY = 0;

                function renderPreview() {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    ctx.globalAlpha = 0.4;
                    ctx.drawImage(maskCanvas, 0, 0);
                    ctx.globalAlpha = 1.0;
                }

                brushSlider.oninput = () => {
                    maskCtx.lineWidth = parseInt(brushSlider.value);
                };

                function floodFillFromImage(x, y, tolerance = 15) {
                    const imgCanvas = document.createElement('canvas');
                    imgCanvas.width = naturalWidth;
                    imgCanvas.height = naturalHeight;
                    const imgCtx = imgCanvas.getContext('2d');
                    imgCtx.drawImage(img, 0, 0);

                    const imageData = imgCtx.getImageData(0, 0, imgCanvas.width, imgCanvas.height);
                    const data = imageData.data;
                    const targetIndex = (Math.floor(y) * imgCanvas.width + Math.floor(x)) * 4;
                    const targetColor = data.slice(targetIndex, targetIndex + 3);

                    const visited = new Uint8Array(imgCanvas.width * imgCanvas.height);
                    const queue = [[x | 0, y | 0]];

                    while (queue.length > 0) {
                        const [cx, cy] = queue.pop();
                        if (cx < 0 || cy < 0 || cx >= imgCanvas.width || cy >= imgCanvas.height) continue;
                        const idx = (cy * imgCanvas.width + cx);
                        if (visited[idx]) continue;

                        const i4 = idx * 4;
                        const color = data.slice(i4, i4 + 3);
                        const diff = Math.sqrt(
                            (color[0] - targetColor[0]) ** 2 +
                            (color[1] - targetColor[1]) ** 2 +
                            (color[2] - targetColor[2]) ** 2
                        );

                        if (diff <= tolerance) {
                            visited[idx] = 1;
                            maskCtx.fillRect(cx, cy, 1, 1);
                            queue.push([cx + 1, cy]);
                            queue.push([cx - 1, cy]);
                            queue.push([cx, cy + 1]);
                            queue.push([cx, cy - 1]);
                        }
                    }

                    renderPreview();
                }

                canvas.addEventListener('mousedown', e => {
                    if (e.button === 1) return;
                    e.preventDefault();
                    const rect = canvas.getBoundingClientRect();
                    startX = (e.clientX - rect.left) / scale;
                    startY = (e.clientY - rect.top) / scale;

                    if (mode === 'flood') {
                        const tolerance = parseInt(floodSlider.value);
                        floodFillFromImage(startX, startY, tolerance);
                        return;
                    }

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
                    const rect = canvas.getBoundingClientRect();
                    const endX = (e.clientX - rect.left) / scale;
                    const endY = (e.clientY - rect.top) / scale;
                    if (mode === 'rect') {
                        const x = Math.min(startX, endX);
                        const y = Math.min(startY, endY);
                        const w = Math.abs(endX - startX);
                        const h = Math.abs(endY - startY);
                        maskCtx.globalCompositeOperation = 'source-over';
                        maskCtx.fillRect(x, y, w, h);
                        tempCtx.clearRect(0, 0, tempCanvas.width, tempCanvas.height);
                    } else {
                        maskCtx.closePath();
                    }
                    maskCtx.globalCompositeOperation = 'source-over';
                    renderPreview();
                });

                document.addEventListener('mousemove', e => {
                    if (!drawing) return;
                    const rect = canvas.getBoundingClientRect();
                    const x = (e.clientX - rect.left) / scale;
                    const y = (e.clientY - rect.top) / scale;

                    if (mode === 'paint') {
                        maskCtx.lineTo(x, y);
                        maskCtx.stroke();
                        maskCtx.beginPath();
                        maskCtx.moveTo(x, y);
                        renderPreview();
                    } else if (mode === 'rect') {
                        tempCtx.clearRect(0, 0, tempCanvas.width, tempCanvas.height);
                        tempCtx.strokeStyle = 'rgba(255,0,0,0.8)';
                        tempCtx.lineWidth = 2;
                        tempCtx.setLineDash([6]);
                        const rectX = Math.min(startX, x);
                        const rectY = Math.min(startY, y);
                        const rectW = Math.abs(x - startX);
                        const rectH = Math.abs(y - startY);
                        tempCtx.strokeRect(rectX, rectY, rectW, rectH);
                    }
                });

                canvas.addEventListener('contextmenu', e => e.preventDefault());

                paintBtn.onclick = () => {
                    mode = 'paint';
                    paintBtn.disabled = true;
                    rectBtn.disabled = false;
                    floodBtn.disabled = false;
                    floodSlider.style.display = 'none';
                    floodLabel.style.display = 'none';
                };
                rectBtn.onclick = () => {
                    mode = 'rect';
                    rectBtn.disabled = true;
                    paintBtn.disabled = false;
                    floodBtn.disabled = false;
                    floodSlider.style.display = 'none';
                    floodLabel.style.display = 'none';
                };
                floodBtn.onclick = () => {
                    mode = 'flood';
                    floodBtn.disabled = true;
                    paintBtn.disabled = false;
                    rectBtn.disabled = false;
                    floodSlider.style.display = 'inline';
                    floodLabel.style.display = 'inline';
                };

                paintBtn.disabled = true;
                renderPreview();





                window.sendMaskToHost = () => {
                    const outputCanvas = document.createElement('canvas');
                    outputCanvas.width = naturalWidth;
                    outputCanvas.height = naturalHeight;
                    const outputCtx = outputCanvas.getContext('2d');
                    outputCtx.drawImage(maskCanvas, 0, 0);
                    const data = outputCanvas.toDataURL('image/png');
                    window.chrome.webview.postMessage({ maskData: data });
                };

window.receiveBitmapFromHost = (base64Png) => {
    const img = new Image();
    img.onload = () => {
        maskCtx.clearRect(0, 0, maskCanvas.width, maskCanvas.height);
        maskCtx.drawImage(img, 0, 0);
        renderPreview();
    };
    img.src = 'data:image/png;base64,' + base64Png;
};

    window.pickPixelColor = () => {
                    const imgCanvas = document.createElement('canvas');
                    imgCanvas.width = naturalWidth;
                    imgCanvas.height = naturalHeight;
                    const imgCtx = imgCanvas.getContext('2d');
                    imgCtx.drawImage(img, 0, 0);

                    const onClick = (e) => {
                        const rect = container.getBoundingClientRect();
                        const x = (e.clientX - rect.left - translateX) / scale;
                        const y = (e.clientY - rect.top - translateY) / scale;
                        const pixel = imgCtx.getImageData(Math.floor(x), Math.floor(y), 1, 1).data;
                        const rgb = { r: pixel[0], g: pixel[1], b: pixel[2] };
                        window.chrome.webview.postMessage({ pickedColor: rgb });

                        container.removeEventListener('click', onClick);
                        console.log('Picked color:', rgb);
                    };

                    container.addEventListener('click', onClick);
                };


           

            })();
        ";
    }
}
