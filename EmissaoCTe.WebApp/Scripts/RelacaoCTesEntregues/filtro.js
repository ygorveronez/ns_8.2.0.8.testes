$(document).ready(function () {
    $("#txtFiltroNumeroInicial").mask("?999999999");
    $("#txtFiltroNumeroFinal").mask("?999999999");
    $("#txtFiltroCTe").mask("?999999999");
    FormatarCampoDate("txtFiltroDataInicial");
    FormatarCampoDate("txtFiltroDataFinal");

    CarregarConsultadeClientes("btnBuscarFiltroCliente", "btnBuscarFiltroCliente", RetornoConsultaClienteFiltro, true, false);
    RemoveConsulta($("#txtFiltroCliente"), function ($this) {
        $this.val("");
        $this.data('codigo', 0);
    });

    $("#btnConsultarGrid").click(function () {
        AtualizarGrid();
    });
});

function DadosFiltros() {
    return {
        NumeroInicial: $("#txtFiltroNumeroInicial").val(),
        NumeroFinal: $("#txtFiltroNumeroFinal").val(),
        DataInicial: $("#txtFiltroDataInicial").val(),
        DataFinal: $("#txtFiltroDataFinal").val(),
        Cliente: $("#txtFiltroCliente").data('codigo'),
        Status: $("#selFiltroStatus").val(),
        NumeroCTe: $("#txtFiltroCTe").val(),
        Descricao: $("#txtFiltroDescricao").val(),
        NumeroControle: $("#txtFiltroNumeroControle").val()
    };
}


function AtualizarGrid() {
    var dados = DadosFiltros();

    CriarGridView("/RelacaoCTesEntregues/Consultar?callback=?", dados, "tbl_relacoes_consulta_table", "tbl_relacoes_consulta", "tbl_relacoes_consulta_paginacao", [
        { Descricao: "Editar", Evento: ExpandirRelacao },
        { Descricao: "Relatório", Evento: DownloadRelatorioRelacao },
    ], [0], null);
}

function RetornoConsultaClienteFiltro(cliente) {
    $("#txtFiltroCliente").val(cliente.Nome);
    $("#txtFiltroCliente").data('codigo', cliente.CPFCNPJ.replace(/[^0-9]/g, ''));
}


// Opcoes
function ExpandirRelacao(item) {
    // Busca informacoes
    executarRest("/RelacaoCTesEntregues/ObterDetalhes?callback=?", { Codigo: item.data.Codigo }, function (r) {
        if (r.Sucesso) {
            // Seta valores sobre edicao
            $("body").data('codigo', r.Objeto.Codigo);
            $("#txtNumero").val(r.Objeto.Numero);

            STATUS_RELACAO_ABERTA = r.Objeto.Status;

            AbrirModal(r.Objeto);
            RecalcularTodosItens();
        } else {
            ExibirMensagemErro(r.Erro, "Erro ao buscar dados.");
        }
    });
}


function DownloadRelatorioRelacao(item) {
    executarDownload("/RelacaoCTesEntregues/RelatorioRelacao?callback=?", { Codigo: item.data.Codigo });
}