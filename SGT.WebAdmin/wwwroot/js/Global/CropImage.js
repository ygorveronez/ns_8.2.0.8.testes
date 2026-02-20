function ImageCropper(canvas, croppedImageElement, imageSrc) {
    let startX, startY, endX, endY, isDragging = false, isMoving = false;
    let offsetX, offsetY;
    let resizingEdge = null;
    let rotationAngle = 0;
    const ctx = canvas.getContext('2d');
    const image = new Image();
    const minWidth = 30;
    const minHeight = 30;
    const edgeThreshold = 10;
    let croppedImage = null;

    image.crossOrigin = 'anonymous';
    image.src = imageSrc;

    image.onload = function () {
        adjustCanvasSize();
        setDefaultCropArea(); // Definir área de corte padrão ao carregar a imagem
        drawImage();
        drawCropAreaWithGrid();
    };

    function adjustCanvasSize() {
        const angleRad = (rotationAngle % 180 === 0) ? 0 : Math.PI / 2;
        const rotatedWidth = Math.abs(Math.cos(angleRad) * image.width + Math.sin(angleRad) * image.height);
        const rotatedHeight = Math.abs(Math.sin(angleRad) * image.width + Math.cos(angleRad) * image.height);

        canvas.width = rotatedWidth;
        canvas.height = rotatedHeight;
    }

    function setDefaultCropArea() {
        // Definir a área de corte padrão para 50% da largura e altura da imagem, centralizada
        const cropWidth = canvas.width * 0.5;
        const cropHeight = canvas.height * 0.5;
        startX = (canvas.width - cropWidth) / 2;
        startY = canvas.height * 0.1;
        endX = startX + cropWidth;
        endY = startY + cropHeight;
    }

    function drawImage() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        ctx.save();
        ctx.translate(canvas.width / 2, canvas.height / 2);
        ctx.rotate(rotationAngle * Math.PI / 180);

        ctx.drawImage(image, -image.width / 2, -image.height / 2, image.width, image.height);

        ctx.restore();
    }

    function drawCropAreaWithGrid() {
        ctx.strokeStyle = 'white';
        ctx.lineWidth = 4;

        const cornerLength = 20;
        const middleLength = 20;

        ctx.beginPath();
        ctx.moveTo(startX, startY);
        ctx.lineTo(startX + cornerLength, startY);
        ctx.moveTo(startX, startY);
        ctx.lineTo(startX, startY + cornerLength);

        ctx.moveTo(endX, startY);
        ctx.lineTo(endX - cornerLength, startY);
        ctx.moveTo(endX, startY);
        ctx.lineTo(endX, startY + cornerLength);

        ctx.moveTo(startX, endY);
        ctx.lineTo(startX + cornerLength, endY);
        ctx.moveTo(startX, endY);
        ctx.lineTo(startX, endY - cornerLength);

        ctx.moveTo(endX, endY);
        ctx.lineTo(endX - cornerLength, endY);
        ctx.moveTo(endX, endY);
        ctx.lineTo(endX, endY - cornerLength);

        ctx.moveTo(startX, startY + (endY - startY) / 2 - middleLength / 2);
        ctx.lineTo(startX, startY + (endY - startY) / 2 + middleLength / 2);
        ctx.moveTo(endX, startY + (endY - startY) / 2 - middleLength / 2);
        ctx.lineTo(endX, startY + (endY - startY) / 2 + middleLength / 2);
        ctx.moveTo(startX + (endX - startX) / 2 - middleLength / 2, startY);
        ctx.lineTo(startX + (endX - startX) / 2 + middleLength / 2, startY);
        ctx.moveTo(startX + (endX - startX) / 2 - middleLength / 2, endY);
        ctx.lineTo(startX + (endX - startX) / 2 + middleLength / 2, endY);
        ctx.stroke();

        const width = endX - startX;
        const height = endY - startY;

        ctx.strokeStyle = 'rgba(255, 255, 255, 0.8)';
        ctx.lineWidth = 2;

        ctx.beginPath();
        ctx.moveTo(startX + width / 3, startY);
        ctx.lineTo(startX + width / 3, endY);
        ctx.moveTo(startX + (2 * width) / 3, startY);
        ctx.lineTo(startX + (2 * width) / 3, endY);

        ctx.moveTo(startX, startY + height / 3);
        ctx.lineTo(endX, startY + height / 3);
        ctx.moveTo(startX, startY + (2 * height) / 3);
        ctx.lineTo(endX, startY + (2 * height) / 3);
        ctx.stroke();
    }

    function isInsideCropArea(x, y) {
        return x >= startX && x <= endX && y >= startY && y <= endY;
    }

    function getResizingEdge(x, y) {
        if (Math.abs(x - startX) < edgeThreshold) return 'left';
        if (Math.abs(x - endX) < edgeThreshold) return 'right';
        if (Math.abs(y - startY) < edgeThreshold) return 'top';
        if (Math.abs(y - endY) < edgeThreshold) return 'bottom';
        return null;
    }

    canvas.onmousedown = function (e) {
        const mouseX = e.offsetX;
        const mouseY = e.offsetY;

        resizingEdge = getResizingEdge(mouseX, mouseY);

        if (resizingEdge) {
            isDragging = true;
        } else if (isInsideCropArea(mouseX, mouseY)) {
            isMoving = true;
            offsetX = mouseX - startX;
            offsetY = mouseY - startY;
            canvas.style.cursor = 'move';
        }
    };

    canvas.onmousemove = function (e) {
        const mouseX = e.offsetX;
        const mouseY = e.offsetY;

        if (isMoving) {
            const newStartX = mouseX - offsetX;
            const newStartY = mouseY - offsetY;
            const width = endX - startX;
            const height = endY - startY;

            startX = newStartX;
            startY = newStartY;
            endX = startX + width;
            endY = startY + height;

            drawImage();
            drawCropAreaWithGrid();
        } else if (isDragging && resizingEdge) {
            if (resizingEdge === 'left') startX = mouseX;
            if (resizingEdge === 'right') endX = mouseX;
            if (resizingEdge === 'top') startY = mouseY;
            if (resizingEdge === 'bottom') endY = mouseY;

            const width = Math.abs(endX - startX);
            const height = Math.abs(endY - startY);

            if (width < minWidth) {
                if (resizingEdge === 'left') startX = endX - minWidth;
                if (resizingEdge === 'right') endX = startX + minWidth;
            }
            if (height < minHeight) {
                if (resizingEdge === 'top') startY = endY - minHeight;
                if (resizingEdge === 'bottom') endY = startY + minHeight;
            }

            drawImage();
            drawCropAreaWithGrid();
        } else {
            resizingEdge = getResizingEdge(mouseX, mouseY);
            if (resizingEdge === 'left' || resizingEdge === 'right') {
                canvas.style.cursor = 'ew-resize';
            } else if (resizingEdge === 'top' || resizingEdge === 'bottom') {
                canvas.style.cursor = 'ns-resize';
            } else if (isInsideCropArea(mouseX, mouseY)) {
                canvas.style.cursor = 'move';
            } else {
                canvas.style.cursor = 'default';
            }
        }
    };

    canvas.onmouseup = function () {
        isDragging = false;
        isMoving = false;
        resizingEdge = null;
        canvas.style.cursor = 'default';
    };

    function getCroppedImageData() {
        const cropWidth = Math.abs(endX - startX);
        const cropHeight = Math.abs(endY - startY);

        if (cropWidth === 0 || cropHeight === 0) {
            alert("Selecione uma área para cortar.");
            return null;
        }

        const tempCanvas = document.createElement('canvas');
        tempCanvas.width = canvas.width;
        tempCanvas.height = canvas.height;
        const tempCtx = tempCanvas.getContext('2d');

        tempCtx.save();
        tempCtx.translate(canvas.width / 2, canvas.height / 2);
        tempCtx.rotate(rotationAngle * Math.PI / 180);
        tempCtx.drawImage(image, -image.width / 2, -image.height / 2);
        tempCtx.restore();

        const croppedCanvas = document.createElement('canvas');
        croppedCanvas.width = cropWidth;
        croppedCanvas.height = cropHeight;
        const croppedCtx = croppedCanvas.getContext('2d');

        croppedCtx.drawImage(
            tempCanvas,
            startX, startY, cropWidth, cropHeight,
            0, 0, cropWidth, cropHeight
        );

        croppedImage = croppedCanvas.toDataURL('image/png');

        return croppedImage;
    }

    function rotateImage() {
        rotationAngle = (rotationAngle + 90) % 360;
        adjustCanvasSize();
        setDefaultCropArea(); // Reajustar a área de corte ao tamanho padrão
        drawImage();
        drawCropAreaWithGrid();
    }

    return {
        getCroppedImageData: getCroppedImageData,
        rotateImage: rotateImage
    };
}
