var cameraQRCodeAtiva = false;

function LerQRCodeCamera() {

	// Start the live stream scanner when the modal opens
    $('#livestream_scanner').on('shown.bs.modal', function (e) {
        cameraQRCodeAtiva = true;
		// Quando abre o modal
        var video = document.createElement("video");
        var loadingMessage = document.getElementById("erro-camera");
        var canvasElement = document.getElementById("canvas-camera");
        var canvas = canvasElement.getContext("2d");
        

        // Use facingMode: environment to attemt to get the front camera on phones
        navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } }).then(function (stream) {
            video.srcObject = stream;
            video.setAttribute("playsinline", true); // required to tell iOS safari we don't want fullscreen
            video.play();
            requestAnimationFrame(tick);
        }).catch((error) => {
            loadingMessage.hidden = false;
            loadingMessage.innerText = "Erro: por favor, conceda a permissão de utilização da câmera para o sistema"
        });

        function tick() {
            if (!cameraQRCodeAtiva) return;
            loadingMessage.innerText = "Carregando vídeo";
            if (video.readyState === video.HAVE_ENOUGH_DATA) {
                loadingMessage.hidden = true;
                canvasElement.hidden = false;

                canvasElement.height = video.videoHeight;
                canvasElement.width = video.videoWidth;
                canvas.drawImage(video, 0, 0, canvasElement.width, canvasElement.height);

                var imageData = canvas.getImageData(0, 0, canvasElement.width, canvasElement.height);
                var code = jsQR(imageData.data, imageData.width, imageData.height, {
                    inversionAttempts: "dontInvert",
                });

                if (code) { // Ao detectar
                    Global.fecharModal('livestream_scanner');
                    _volume.CodigoBarras.val(code.data);
                    return;
                } else {
                    //outputMessage.hidden = false;
                    //outputData.parentElement.hidden = true;
                }
            }
            requestAnimationFrame(tick);
        }

	});

	// Stop quagga in any case, when the modal is closed
	$('#livestream_scanner').on('hide.bs.modal', function () {
        cameraQRCodeAtiva = false;
	});

    Global.abrirModal('livestream_scanner');
}