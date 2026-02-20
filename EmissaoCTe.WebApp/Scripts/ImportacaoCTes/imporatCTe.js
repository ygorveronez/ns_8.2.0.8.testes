var uploader;
$(document).ready(function () {
    $("#btnImportarCTe").click(function () {
        uploader = AbrirUploadPadrao({
            title: "Importação de CT-e",
            url: "/ConhecimentoDeTransporteEletronico/GerarCTeAnterior?callback=?",
            filter: [{ title: 'Arquivos XML', extensions: 'xml' }],
            max_file_size: '2000kb',
            onFinish: function (arquivos, erros) {
                console.log(arquivos);
                console.log(erros);

                if (arquivos.length > 0) {
                    ExibirMensagemSucesso("<br>" + arquivos.length + " arquivo(s) importados com sucesso!", "Importação concluída.");
                }

                if (erros.length > 0) {
                    // Cria lista de erros
                    var listaErros = "<ul>";
                    for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
                    listaErros += "</ul>"

                    // Exibe erros
                    ExibirMensagemAlerta(listaErros, "Ocorreram os seguinte erros:");
                }

                AtualizarGridCTes();
            }
        });
    });
});