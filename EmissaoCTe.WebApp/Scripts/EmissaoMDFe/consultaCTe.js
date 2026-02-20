/// <reference path="../jquery-3.0.0.js" />

$(document).ready(function () {
    FormatarCampoDate("txtDataEmissaoInicialCTeConsulta");
    FormatarCampoDate("txtDataEmissaoFinalCTeConsulta");

    $("#txtNumeroInicialCTeConsulta").mask("9?99999999999");
    $("#txtNumeroFinalCTeConsulta").mask("9?99999999999");
    $("#txtPlacaVeiculoCTeConsulta").mask("*******");
    $("#txtCPFMotoristaCTeConsulta").mask("99999999999");
});

function ConsultarCTes(callback, ignorarOrigem) {
    var dados = {
        CodigoMunicipio: $("body").data("municipioDescarregamentoDocumento") != null ? $("body").data("municipioDescarregamentoDocumento").CodigoMunicipio : 0,
        UFDescarregamento: $("body").data("ufConsultaCTe") != null ? $("body").data("ufConsultaCTe") : "",
        UFCarregamento: $("body").data("ufCarregamentoConsultaCTe") != null ? $("body").data("ufCarregamentoConsultaCTe") : "",
        DataInicial: $("#txtDataEmissaoInicialCTeConsulta").val(),
        DataFinal: $("#txtDataEmissaoFinalCTeConsulta").val(),
        NumeroInicial: $("#txtNumeroInicialCTeConsulta").val(),
        NumeroFinal: $("#txtNumeroFinalCTeConsulta").val(),
        Placa: $("#txtPlacaVeiculoCTeConsulta").val(),
        NomeMotorista: $("#txtNomeMotoristaCTeConsulta").val(),
        CPFMotorista: $("#txtCPFMotoristaCTeConsulta").val(),
        TipoServico: $("#selTipoServico").val(),
        TipoCTe: $("#selTipoCTe").val(),
        CTesSemMDFe: false,
        IgnorarOrigem: ignorarOrigem,
        inicioRegistros: 0
    };

    CriarGridView("/ManifestoEletronicoDeDocumentosFiscais/ConsultarCTesParaEmissao?callback=?", dados, "tbl_ctes_consulta_table", "tbl_ctes_consulta", "tbl_ctes_consulta_paginacao", [{ Descricao: "Selecionar", Evento: callback }], [0, 1, 2, 3, 4, 5], null);

}

function AbrirTelaConsultaCTes(callback, ignorarOrigem) {
    var dataInicial = new Date();
    var dataFinal = new Date();

    dataInicial.setDate(dataInicial.getDate() - 7);

    $("#txtDataEmissaoInicialCTeConsulta").val(Globalize.format(dataInicial, "dd/MM/yyyy"));
    $("#txtDataEmissaoFinalCTeConsulta").val(Globalize.format(dataFinal, "dd/MM/yyyy"));
    
    $("body").data("ctesSelecionadosConsultaAvancada", null);

    $("#containerCTesSelecionados").html("");

    ConsultarCTes(callback, ignorarOrigem);

    $("#divConsultaCTes").modal({ keyboard: false, backdrop: 'static' });

    $("#btnBuscarCTesConsulta").off();

    $("#btnBuscarCTesConsulta").on("click", function () { ConsultarCTes(callback, ignorarOrigem) });
}


function FecharTelaConsultaCTes() {
    $("#divConsultaCTes").modal('hide');
    LimparCamposConsultaCTe();
}

function LimparCamposConsultaCTe() {
    $("#btnSelecionarTodosOsCTes").addClass("disabled");
    $("#divConsultaCTes .modal-footer").hide();
    $("body").data("ufConsultaCTe", null);
    $("body").data("ufCarregamentoConsultaCTe", null);
    $("#containerCTesSelecionados").html("");
    $("body").data("ctesSelecionadosConsultaAvancada", null);
    $("#txtDataEmissaoInicialCTeConsulta").val('');
    $("#txtDataEmissaoFinalCTeConsulta").val('');
    $("#txtNumeroInicialCTeConsulta").val('');
    $("#txtNumeroFinalCTeConsulta").val('');
    $("#selTipoServico").val($("#selTipoServico option:first").val());
    $("#selTipoCTe").val($("#selTipoCTe option:first").val());
}