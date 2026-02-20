$(document).ready(function () {
    FormatarCampoDate("txtFiltroDataInicial");
    FormatarCampoDate("txtFiltroDataFinal");

    $("#btnAtualizarGridArquivos").click(function () {
        AtualizarGridArquivos();
    });

    AtualizarGridArquivos();
});

function AtualizarGridArquivos() {
    var dados = {
        inicioRegistros: 0,
        DataInicial: $("#txtFiltroDataInicial").val(),
        DataFinal: $("#txtFiltroDataFinal").val(),
        Descricao: $("#txtFiltroDescricao").val(),
        Status: $("#selFiltroStatus").val(),
    };

    var opcoes = [];

    opcoes.push({ Descricao: "Editar", Evento: EditarArquivo });
    opcoes.push({ Descricao: "Download", Evento: DownloadArquivo });

    CriarGridView("/ArquivoTransportador/Consultar?callback=?", dados, "tbl_arquivos_table", "tbl_arquivos", "tbl_paginacao_arquivos", opcoes, [0], null);
}

function DownloadArquivo(arquivo) {
    executarDownload("/ArquivoTransportador/DownloadArquivo", { Codigo: arquivo.data.Codigo });
}

function EditarArquivo(arquivo) {
    CodigoArquivo = arquivo.data.Codigo;
    BuscarPorCodigo();
}