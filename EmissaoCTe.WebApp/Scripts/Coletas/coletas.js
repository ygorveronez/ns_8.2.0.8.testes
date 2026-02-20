$(document).ready(function () {

    FormatarCampoDateTime("txtDataInicial");
    FormatarCampoDateTime("txtDataFinal");

    FormatarCampoDate("txtDataEntrega");

    $("#txtPeso").priceFormat();
    $("#txtValorNFs").priceFormat();
    $("#txtValorFrete").priceFormat();
    $("#txtNumero").mask("9?99999");

    $("#txtQtVolumes, #txtNumeroNotaCliente").on('change', function () {
        var clearValue = this.value.replace(/[^0-9]/g, '');
        this.value = clearValue;
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    LimparCampos();           
});

function LimparCampos() {
    $("body").data('codigo', null);

    $("body").data('motoristas', null);
    LimparCamposMotorista();
    RenderizarMotoristas();

    $("body").data("veiculos", null);
    LimparCamposVeiculo();
    RenderizarVeiculos();

    LimparCamposDestinatario();
    LimparCamposRemetente();
    LimparCamposTomador();

    $("body").data("codigoTipoColeta", null);
    $("#txtTipoColeta").val("");

    $("body").data("codigoTipoCarga", null);
    $("#txtTipoCarga").val("");

    $("#txtNumero").val("0");
    $("#txtDataInicial").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
    $("#txtDataFinal").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
    $("#txtDataEntrega").val("");
    $("#txtPeso").val("0,00");
    $("#txtValorNFs").val("0,00");
    $("#txtValorFrete").val("0,00");
    $("#selRequisitante").val($("#selRequisitante option:first").val());
    $("#selTipoPagamento").val($("#selTipoPagamento option:first").val());
    $("#txtCodigoPedidoCliente").val("");
    $("#selSituacao").val($("#selSituacao option:first").val());
    $("#txtObservacao").val("");
    $("#txtQtVolumes").val("");
    $("#txtNumeroNotaCliente").val("");
     
    $("#txtObservacaoCTe").val("");
    $("#btnDownloadEspelho").hide();

    BuscarProximoNumero();
}

function BuscarProximoNumero() {
    executarRest("/Coleta/ObterProximoNumero?callback=?", '', function (r) {
        if (r.Sucesso) {
            $("#txtNumero").val(r.Objeto.numero);
        } else {
            $("#txtNumero").val("1");
        }
    });
}

function ValidarCampos() {
    var codigoRemetente = $("body").data("codigoRemetente");
    var codigoDestinatario = $("body").data("codigoDestinatario");
    var codigoOrigem = $("body").data("codigoOrigem");
    var codigoDestino = $("body").data("codigoDestino");
    var dataInicio = $("#txtDataInicial").val();
    var dataFim = $("#txtDataFinal").val();
    var dataEntrega = $("#txtDataEntrega").val();
    var descricaoDestino = $("#txtDestino").val();

    var valido = true;

    if (dataInicio == "" || dataInicio == null) {
        CampoComErro("#txtDataInicial");
        valido = false;
    } else {
        CampoSemErro("#txtDataInicial");
    }

    if (dataFim == "" || dataFim == null) {
        CampoComErro("#txtDataFinal");
        valido = false;
    } else {
        CampoSemErro("#txtDataFinal");
    }

    if (dataEntrega == "" || dataEntrega == null) {
        CampoComErro("#txtDataEntrega");
        valido = false;
    } else {
        CampoSemErro("#txtDataEntrega");
    }

    if (isNaN(codigoOrigem) || codigoOrigem <= 0) {
        CampoComErro("#txtOrigem");
        valido = false;
    } else {
        CampoSemErro("#txtOrigem");
    }

    if (isNaN(codigoDestino) || (codigoDestino <= 0 && descricaoDestino == "")) {
        CampoComErro("#txtDestino");
        valido = false;
    } else {
        CampoSemErro("#txtDestino");
    }

    return valido;
}

function Salvar() {
    if (ValidarCampos()) {
        var dados = {
            Codigo: $("body").data("codigo"),
            Numero: $("#txtNumero").val(),
            DataInicial: $("#txtDataInicial").val(),
            DataFinal: $("#txtDataFinal").val(),
            DataEntrega: $("#txtDataEntrega").val(),
            CodigoOrigem: $("body").data("codigoOrigem"),
            CodigoDestino: $("body").data("codigoDestino"),
            CodigoRemetente: $("body").data("codigoRemetente"),
            CodigoDestinatario: $("body").data("codigoDestinatario"),
            CodigoTomador: $("body").data("codigoTomador"),
            CodigoTipoCarga: $("body").data("codigoTipoCarga"),
            CodigoTipoColeta: $("body").data("codigoTipoColeta"),
            Peso: $("#txtPeso").val(),
            ValorNFs: $("#txtValorNFs").val(),
            ValorFrete: $("#txtValorFrete").val(),
            Requisitante: $("#selRequisitante").val(),
            TipoPagamento: $("#selTipoPagamento").val(),
            Situacao: $("#selSituacao").val(),
            CodigoPedidoCliente: $("#txtCodigoPedidoCliente").val(),
            Observacao: $("#txtObservacao").val(),
            ObservacaoCTe: $("#txtObservacaoCTe").val(),
            QtVolumes: $("#txtQtVolumes").val(),
            NumeroNotaCliente: $("#txtNumeroNotaCliente").val(),
            Veiculos: JSON.stringify(ObterCodigosVeiculos()),
            Motoristas: JSON.stringify(ObterCodigosMotoristas())
        };

        if (dados.CodigoRemetente == null || dados.CodigoRemetente == "" || dados.CodigoDestinatario == null || dados.CodigoDestinatario == "") {
            if ($("body").data("veiculos") == null || $("body").data("veiculos").length <= 0) {
                ExibirMensagemAlerta("Selecione ao menos um veículo para a coleta.", "Atenção!");
                return;
            }
        }

        executarRest("/Coleta/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
    }
}

function ObterCodigosMotoristas() {
    var motoristas = $("body").data("motoristas") == null ? new Array() : $("body").data("motoristas");
    var codigos = new Array();

    for (var i = 0; i < motoristas.length; i++)
        codigos.push(motoristas[i].Codigo);

    return codigos;
}

function ObterCodigosVeiculos() {
    var veiculos = $("body").data("veiculos") == null ? new Array() : $("body").data("veiculos");
    var codigos = new Array();

    for (var i = 0; i < veiculos.length; i++)
        codigos.push(veiculos[i].Codigo);

    return codigos;
}