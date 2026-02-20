$(document).ready(function () {
    $("#opcoesExportacoes").on('click', '*[data-tipo]', function (e) {
        e.preventDefault();

        executarDownload("/ImportacaoArquivo/ExportarDados", { Tipo: $(this).data('tipo') }, null, function (html) {
            try {
                var retorno = JSON.parse(html.replace("(", "").replace(");", ""));
                ExibirMensagemErro(retorno.Erro, "Atenção!", placeholder);
            } catch (ex) {
                ExibirMensagemErro("Não foi possível realizar a exportação dos dados.", "Atenção!");
            }
        });
    });
});