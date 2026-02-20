$(document).ready(function () {
    CarregarConsultaCalculoRelacaoCTesEntregues("default-search", "default-search", RetornoCalculoRelacaoCTesEntregues, true, false);
    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);
    //CarregarConsultadeClientes("btnBuscarEmissor", "btnBuscarEmissor", RetornoConsultaEmissor, true, false);

    $("#txtValorDiaria").priceFormat();
    $("#txtValorMeiaDiaria").priceFormat();
    $("#txtPercentualPorCTe").priceFormat();
    $("#txtValorMinimoPorCTe").priceFormat();
    $("#txtValorMinimoCTeMesmoDestino").priceFormat();
    $("#txtFracaoKG").priceFormat({ centsLimit: 4 });
    $("#txtValorPorFracao").priceFormat();
    $("#txtValorPorFracaoEmEntregasIguais").priceFormat();
    $("#txtValorKMExcedente").priceFormat();
    $("#txtColetaValorPorEvento").priceFormat();
    $("#txtColetaFracao").priceFormat({ centsLimit: 4 });
    $("#txtColetaValorPorFracao").priceFormat();
    $("#txtFranquiaKM").priceFormat({ centsLimit: 0, centsSeparator: '' });

    $("#txtPercentualPorCTe, #txtValorMinimoCTeMesmoDestino").change(ControleCampoPercentual);

    RemoveConsulta("#txtCliente", LimparCliente, true);
    //RemoveConsulta("#txtEmissor", LimparEmissor, true);

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    setTimeout(LimparCampos, 100);
});

var _Codigo = 0;

function LimparCampos() {
    _Codigo = 0;

    $("#txtValorDiaria").val("0,00");
    $("#txtValorMeiaDiaria").val("0,00");
    $("#txtPercentualPorCTe").val("0,00");
    $("#txtValorMinimoPorCTe").val("0,00");
    $("#txtValorMinimoCTeMesmoDestino").val("0,00");
    $("#txtFracaoKG").val("0,0000");
    $("#txtValorPorFracao").val("0,00");
    $("#txtValorPorFracaoEmEntregasIguais").val("0,00");
    $("#txtValorKMExcedente").val("0,00");
    $("#txtColetaValorPorEvento").val("0,00");
    $("#txtColetaFracao").val("0,0000");
    $("#txtColetaValorPorFracao").val("0,00");

    $("#txtFranquiaKM").val("0");

    ControleCampoPercentual();

    LimparCliente();
    //LimparEmissor();

    StateCidades.clear();
}

function LimparCliente() {
    $("#txtCliente").val("");
    $("#txtCliente").data("Codigo", 0);
}

//function LimparEmissor() {
//    $("#txtEmissor").val("");
//    $("#txtEmissor").data("Codigo", 0);
//}

function RetornoConsultaCliente(cliente) {
    $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome);
    $("#txtCliente").data("Codigo", cliente.CPFCNPJ.replace(/[^0-9]/g, ""));
}

//function RetornoConsultaEmissor(emissor) {
//    $("#txtEmissor").val(emissor.CPFCNPJ + " - " + emissor.Nome);
//    $("#txtEmissor").data("Codigo", emissor.CPFCNPJ.replace(/[^0-9]/g, ""));
//}

function RetornoCalculoRelacaoCTesEntregues(data) {
    executarRest("/CalculoRelacaoCTesEntregues/ObterDetalhes?callback=?", data, function (r) {
        if (r.Sucesso) {
            var obj = r.Objeto;
            _Codigo = obj.Codigo;

            $("#txtCliente").val(obj.Cliente.Descricao);
            $("#txtCliente").data("Codigo", obj.Cliente.Codigo);
           
            if (obj.Emissor != null) {
                $("#txtEmissor").val(obj.Emissor.Descricao);
                $("#txtEmissor").data("Codigo", obj.Emissor.Codigo);
            }

            $("#txtValorDiaria").val(obj.ValorDiaria);
            $("#txtValorMeiaDiaria").val(obj.ValorMeiaDiaria);
            $("#txtPercentualPorCTe").val(obj.PercentualPorCTe);
            $("#txtValorMinimoPorCTe").val(obj.ValorMinimoPorCTe);
            $("#txtValorMinimoCTeMesmoDestino").val(obj.ValorMinimoCTeMesmoDestino);
            $("#txtFracaoKG").val(obj.FracaoKG);
            $("#txtValorPorFracao").val(obj.ValorPorFracao);
            $("#txtValorPorFracaoEmEntregasIguais").val(obj.ValorPorFracaoEmEntregasIguais);
            $("#txtFranquiaKM").val(obj.FranquiaKM);
            $("#txtValorKMExcedente").val(obj.ValorKMExcedente);
            $("#txtColetaValorPorEvento").val(obj.ColetaValorPorEvento);
            $("#txtColetaFracao").val(obj.ColetaFracao);
            $("#txtColetaValorPorFracao").val(obj.ColetaValorPorFracao);

            ControleCampoPercentual();

            StateCidades.set(obj.Cidades);
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function Salvar() {
    var dados = Dados();
    executarRest("/CalculoRelacaoCTesEntregues/Salvar?callback=?", dados, function (r) {
        if (r.Sucesso) {
            ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
            LimparCampos();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function Dados() {
    return {
        Codigo: _Codigo,
        Cliente: $("#txtCliente").data("Codigo"),
        //Emissor: $("#txtEmissor").data("Codigo"),
        ValorDiaria: $("#txtValorDiaria").val(),
        ValorMeiaDiaria: $("#txtValorMeiaDiaria").val(),
        PercentualPorCTe: $("#txtPercentualPorCTe").val(),
        ValorMinimoPorCTe: $("#txtValorMinimoPorCTe").val(),
        ValorMinimoCTeMesmoDestino: $("#txtValorMinimoCTeMesmoDestino").val(),
        FracaoKG: $("#txtFracaoKG").val(),
        ValorPorFracao: $("#txtValorPorFracao").val(),
        ValorPorFracaoEmEntregasIguais: $("#txtValorPorFracaoEmEntregasIguais").val(),
        FranquiaKM: $("#txtFranquiaKM").val(),
        ValorKMExcedente: $("#txtValorKMExcedente").val(),
        ColetaValorPorEvento: $("#txtColetaValorPorEvento").val(),
        ColetaFracao: $("#txtColetaFracao").val(),
        ColetaValorPorFracao: $("#txtColetaValorPorFracao").val(),
        Cidades: StateCidades.toJson()
    };
}

function ControleCampoPercentual() {
    if ($("#txtPercentualPorCTe").val() != "0,00")
        $("#txtValorMinimoCTeMesmoDestino").prop("disabled", true);
    else if ($("#txtValorMinimoCTeMesmoDestino").val() != "0,00")
        $("#txtPercentualPorCTe").prop("disabled", true);
    else {
        $("#txtValorMinimoCTeMesmoDestino").prop("disabled", false);
        $("#txtPercentualPorCTe").prop("disabled", false);
    }
}