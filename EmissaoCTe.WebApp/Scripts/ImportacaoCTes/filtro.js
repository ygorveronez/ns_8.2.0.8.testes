var RetornosConsultas = {
    RemetenteFiltro: "",
    DestinatarioFiltro: "",
};
$(document).ready(function () {
    var today = new Date();
    var yesterday = new Date(today);
    var tomorrow = new Date(today);
    yesterday.setDate(today.getDate() - 1);
    tomorrow.setDate(today.getDate() + 1);

    $("#txtInicialCTeFiltro").val(Globalize.format(yesterday, "dd/MM/yyyy"));
    $("#txtFinalCTeFiltro").val(Globalize.format(tomorrow, "dd/MM/yyyy"));

    $("#txtNumeroInicialCTeFiltro").mask("9?999999999");
    $("#txtNumeroFinalCTeFiltro").mask("9?999999999");
    $("#txtNumeroNF").mask("9?999999999");
    $("#txtPlacaCTeFiltro").mask("*******");

    FormatarCampoDate("txtDataEmissaoInicialCTeFiltro");
    FormatarCampoDate("txtDataEmissaoFinalCTeFiltro");

    CarregarConsultadeClientes("btnBuscarRemetenteCTeFiltro", "btnBuscarRemetenteCTeFiltro", RetornoConsultaRemetenteFiltro, true, false);
    CarregarConsultadeClientes("btnBuscarDestinatarioCTeFiltro", "btnBuscarDestinatarioCTeFiltro", RetornoConsultaDestinatarioFiltro, true, false);

    RemoveConsulta($("#txtRemetenteCTeFiltro"), function () {
        LimparCamposRemetenteDestinatarioFiltro("Remetente");
    });
    RemoveConsulta($("#txtDestinatarioCTeFiltro"), function () {
        LimparCamposRemetenteDestinatarioFiltro("Destinatario");
    });

    $("#txtCPFCNPJRemetenteFiltro").focusout(function () {
        BuscarRemetenteDestinatarioFiltro($(this), "Remetente");
    });
    $("#txtCPFCNPJDestinatarioFiltro").focusout(function () {
        BuscarRemetenteDestinatarioFiltro($(this), "Remetente");
    });

    $("#btnConsultarCTe").click(function () {
        AtualizarGridCTes();
    });

    AtualizarGridCTes();
});

function DadosFiltro() {
    return {
        inicioRegistros: 0,
        NumeroInicial: $("#txtNumeroInicialCTeFiltro").val(),
        NumeroFinal: $("#txtNumeroFinalCTeFiltro").val(),
        Remetente: RetornosConsultas.RemetenteFiltro.replace(/[^0-9]/g, ''),
        Destinatario: RetornosConsultas.DestinatarioFiltro.replace(/[^0-9]/g, ''),
        DataEmissaoInicial: $("#txtDataEmissaoInicialCTeFiltro").val(),
        DataEmissaoFinal: $("#txtDataEmissaoFinalCTeFiltro").val(),
        Placa: $("#txtPlacaCTeFiltro").val(),
        Motorista: $("#txtMotoristaCTeFiltro").val(),
        Status: $("#selStatusCTeFiltro").val(),
        Finalidade: $("#selFinalidadeCTeFiltro").val(),
        TipoOcorrencia: $("#selTipoOcorrencia").val(),
        Serie: $("#ddlSerieFiltro").val(),
        NumeroNF: $("#txtNumeroNF").val(),
        Contem: $("#chkContem")[0].checked
    };
}
function AtualizarGridCTes() {
    var dados = DadosFiltro();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar DACTE", Evento: DownloadDacte });
    opcoes.push({ Descricao: "Baixar XML", Evento: DownloadXML });

    CriarGridView("/ConhecimentoDeTransporteEletronico/Consultar?callback=?", dados, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", opcoes, [0, 1, 2], null, [2, 3, 4, 5, 6, 7, 8, 10, 12]);
}