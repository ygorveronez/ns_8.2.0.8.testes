function DownloadBoletoClick(e, sender) {
    var dados = JSON.stringify({
        CodigoTitulo: e
    })
    ExecutarDownload("../DownloadBoleto", dados);
}

function DownloadDanfeCLick() {
    var dados = JSON.stringify({
        ChaveNfe: $('#chaveNfe').text()
    })

    ExecutarDownload("../DownloadDanfe", dados);
}

function DownloadXmlClick(e, sender) {
    var dados = JSON.stringify({
        CodigoNotaFiscal: e,
        ChaveNfe: $('#chaveNfe').text()
    })

    ExecutarDownload("../DownloadXML", dados);
}


async function ExecutarDownload(url, dados) {

    try {
        fetch(url, {
            method: "POST",
            headers: new Headers({ 'content-type': 'application/json' }),
            body: dados
        }).then((result) => {
            const header = result.headers.get('Content-Disposition');
            if (header == null)
                return ExibirNotificacao(false, "Um problema ocorreu ao baixar o arquivo")

            const parts = header.split(';');
            filename = parts[1].split('=')[1].replaceAll("\"", "");

            return result.blob();
        }).then((blob) => {
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            a.remove();

            ExibirNotificacao(true)
        })
    } catch (error) {
        ExibirNotificacao(false, "Um problema ocorreu ao baixar o arquivo")
        console.error(error.message);
    }
}

function ExibirNotificacao(sucesso, mensagem) {
    var notificacao = $('#liveToast');
    var notificacaoTitulo = $('#NotificacaoTitulo');
    var notificacaoDescricao = $('#NotificacaoDescricao');
    var iconeSucesso = $('#IconeSucesso');
    var iconeErro = $('#IconeErro');

    if (sucesso) {
        notificacaoTitulo.text("Download concluído");
        notificacaoDescricao.text("Seu download foi concluído com sucesso!");

        iconeErro.addClass("d-none")
        iconeSucesso.removeClass("d-none")

        notificacao.removeClass("bg-danger text-bg-danger").addClass("bg-success text-bg-success")
    }
    else {
        notificacaoTitulo.text("Erro ao baixar arquivo");
        notificacaoDescricao.text(mensagem);

        iconeSucesso.addClass("d-none")
        iconeErro.removeClass("d-none")

        notificacao.removeClass("bg-success text-bg-success").addClass("bg-danger text-bg-danger")
    }

    bootstrap.Toast.getOrCreateInstance(notificacao).show()
}
