$(document).ready(function () {
    $("#txtValorMercadoriaParaEfeitoDeAverbacao").priceFormat({ prefix: '' });
    $("#btnSalvarInformacaoSeguro").click(function () {
        SalvarInformacaoSeguro();
    });
    $("#btnCancelarInformacaoSeguro").click(function () {
        LimparCamposInformacaoSeguro();
    });
    $("#btnExcluirInformacaoSeguro").click(function () {
        ExcluirInformacaoSeguro();
    });
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", "", RetornoConsultaApoliceSeguro, true, false);
    $("#selTomadorServico").change(function () {
        BuscarApoliceDeSeguroDoTomador();
    });

    $("#txtCNPJSeguradora").mask("99.999.999/9999-99");

    BuscarInformacoesSeguroAverbacao();
});

function BuscarInformacoesSeguroAverbacao() {
    executarRest("/ApoliceDeSeguro/ObterInformacoesApoliceAverbacao?callback=?", {}, function (r) {

        $('#divInformacaoServicosSeguro').find('span').remove();

        if (r.Sucesso) {

            if (r.Objeto.Mensagem != null && r.Objeto.Mensagem != "") {
                $("#divInformacaoServicosSeguro").prepend('<span class="label label-success" style="padding-top: 4px; padding-bottom: 4px;">' + r.Objeto.Mensagem + '</span>');
                $("#divInformacaoServicosSeguro").removeClass("hidden");
            } else {
                $("#divInformacaoServicosSeguro").addClass("hidden");
            }

        }
        //else {
        //    $("#divInformacaoServicosSeguro").prepend('<span class="label label-info" style="padding-top: 4px; padding-bottom: 4px;">Dados do frete não encontrados! O valor do frete deve ser informado manualmente.</span>');
        //    $("#divInformacaoServicosSeguro").removeClass("hidden");
        //}

    });
}

function BuscarApoliceDeSeguroDoTomador() {
    var cpfCnpjTomador = "";
    var responsavelSeguro = "0";

    switch ($("#selTomadorServico").val()) {
        case "0":
            responsavelSeguro = '0';
            cpfCnpjTomador = $("#hddRemetente").val();
            break;
        case "1":
            responsavelSeguro = '1';
            cpfCnpjTomador = $("#hddExpedidor").val();
            break;
        case "2":
            responsavelSeguro = '2';
            cpfCnpjTomador = $("#hddRecebedor").val();
            break;
        case "3":
            responsavelSeguro = '3';
            cpfCnpjTomador = $("#hddDestinatario").val();
            break;
        case "4":
            responsavelSeguro = '5';
            cpfCnpjTomador = $("#hddTomador").val();
            break;
        default:
            cpfCnpjTomador = "";
            break;
    }

    var infomacoesSeguro = $("#hddInformacoesSeguro").val() == "" ? new Array() : JSON.parse($("#hddInformacoesSeguro").val());
    var infomacoesSeguroAnterior = $("#hddInformacoesSeguro").val() == "" ? new Array() : JSON.parse($("#hddInformacoesSeguro").val());

    for (var i = infomacoesSeguro.length - 1; i >= 0; i--) {
        if (infomacoesSeguro[i].Id <= 0)
            infomacoesSeguro.splice(i, 1);
        else
            infomacoesSeguro[i].Excluir = true;
    }

    if (cpfCnpjTomador != "") {
        executarRest("/ApoliceDeSeguro/BuscarPorCliente?callback=?", { CPFCNPJCliente: cpfCnpjTomador }, function (r) {
            if (r.Objeto.length >= 0) {
                var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
                responsavelSeguro = (configuracaoEmpresa != null && configuracaoEmpresa.ResponsavelSeguro != null && configuracaoEmpresa.ResponsavelSeguro != "" ? configuracaoEmpresa.ResponsavelSeguro : responsavelSeguro);
                if (r.Objeto.length >= 1) {
                    var resSeguro = r.Objeto[0].Responsavel >= 0 ? r.Objeto[0].Responsavel : responsavelSeguro;
                    var informacaoSeguro = {
                        Id: -(infomacoesSeguro.length + 1),
                        Responsavel: resSeguro,
                        DescricaoResponsavel: $("#selResponsavelSeguro option[value=" + resSeguro + "]").text(),
                        Seguradora: r.Objeto[0].NomeSeguradora,
                        NumeroApolice: r.Objeto[0].NumeroApolice,
                        CNPJSeguradora: r.Objeto[0].CNPJSeguradora,
                        NumeroAverberacao: "",
                        ValorMercadoria: 0,
                        Excluir: false
                    };
                    infomacoesSeguro.push(informacaoSeguro);
                } else {
                    if (configuracaoEmpresa != null) {
                        var informacaoSeguro = {
                            Id: -(infomacoesSeguro.length + 1),
                            Responsavel: configuracaoEmpresa.ResponsavelSeguro,
                            DescricaoResponsavel: $("#selResponsavelSeguro option[value=" + configuracaoEmpresa.ResponsavelSeguro + "]").text(),
                            Seguradora: configuracaoEmpresa.NomeSeguradora,
                            CNPJSeguradora: configuracaoEmpresa.CNPJSeguradora,
                            NumeroApolice: configuracaoEmpresa.NumeroApoliceSeguro,
                            NumeroAverberacao: "",
                            ValorMercadoria: 0,
                            Excluir: false
                        };
                        infomacoesSeguro.push(informacaoSeguro);
                    }
                }
            }
            if (infomacoesSeguro[0].Seguradora != "" || infomacoesSeguroAnterior.length == 0) {
                $("#hddInformacoesSeguro").val(JSON.stringify(infomacoesSeguro));
                RenderizarInformacaoSeguro();
                LimparCamposInformacaoSeguro();
            }
        });
    } else {
        $("#hddInformacoesSeguro").val(JSON.stringify(infomacoesSeguro));
        RenderizarInformacaoSeguro();
        LimparCamposInformacaoSeguro();
    }

    $("#btnBuscarApoliceSeguro").off();
    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", cpfCnpjTomador, RetornoConsultaApoliceSeguro, true, false);
}
function RetornoConsultaApoliceSeguro(apolice) {
    if (apolice.Codigo == undefined)
        apolice = apolice.data;
    $("#txtNumeroApolice").val(apolice.NumeroApolice);
    $("#txtNomeSeguradora").val(apolice.NomeSeguradora);
    $("#txtCNPJSeguradora").val(apolice.CNPJSeguradora).trigger("blur");
}
function ValidarCamposInformacaoSeguro() {
    var valido = true;
    /*if ($("#txtCNPJSeguradora").is(':visible')) {
        if (!ValidarCNPJ($("#txtCNPJSeguradora").val())) {
            valido = false;
            CampoComErro($("#txtCNPJSeguradora"));
        } else {
            CampoSemErro($("#txtCNPJSeguradora"));
        }
    }*/
    return valido;
}
function SalvarInformacaoSeguro() {
    if (ValidarCamposInformacaoSeguro()) {
        var informacaoSeguro = {
            Id: Globalize.parseInt($("#hddIdInformacaoSeguroEmEdicao").val()),
            Responsavel: $("#selResponsavelSeguro").val(),
            DescricaoResponsavel: $("#selResponsavelSeguro :selected").text(),
            Seguradora: $("#txtNomeSeguradora").val(),
            NumeroApolice: $("#txtNumeroApolice").val(),
            NumeroAverberacao: $("#txtNumeroAverberacao").val(),
            CNPJSeguradora: $("#txtCNPJSeguradora").val().replace(/[^0-9]/g, ''),
            ValorMercadoria: Globalize.parseFloat($("#txtValorMercadoriaParaEfeitoDeAverbacao").val()),
            Excluir: false
        };
        
        InsereSeguro(informacaoSeguro);
        LimparCamposInformacaoSeguro();
    }
}

function InsereSeguro(obj) {
    var objSeguro = $.extend({
        Id: 0,
        Responsavel: 0,
        DescricaoResponsavel: "",
        Seguradora: "",
        NumeroApolice: 0,
        NumeroAverberacao: 0,
        CNPJSeguradora: 0,
        ValorMercadoria: 0,
        Excluir: false
    }, obj);

    var infomacoesSeguro = $("#hddInformacoesSeguro").val() == "" ? new Array() : JSON.parse($("#hddInformacoesSeguro").val());
    if (objSeguro.Id == 0) {
        objSeguro.Id = BuscaMenorProximoCodigo(infomacoesSeguro);
    }

    for (var i = 0; i < infomacoesSeguro.length; i++) {
        if (infomacoesSeguro[i].Id == objSeguro.Id) {
            infomacoesSeguro.splice(i, 1);
            break;
        }
    }
    infomacoesSeguro.push(objSeguro);
    $("#hddInformacoesSeguro").val(JSON.stringify(infomacoesSeguro));
    RenderizarInformacaoSeguro();
}

function EditarInformacaoSeguro(informacao) {
    $("#hddIdInformacaoSeguroEmEdicao").val(informacao.Id);
    $("#selResponsavelSeguro").val(informacao.Responsavel);
    $("#txtNomeSeguradora").val(informacao.Seguradora);
    $("#txtNumeroApolice").val(informacao.NumeroApolice)
    $("#txtNumeroAverberacao").val(informacao.NumeroAverberacao);
    $("#txtCNPJSeguradora").val(informacao.CNPJSeguradora).trigger("blur");
    $("#txtValorMercadoriaParaEfeitoDeAverbacao").val(Globalize.format(informacao.ValorMercadoria, "n2"));
    $("#btnExcluirInformacaoSeguro").show();
}
function ExcluirInformacaoSeguro() {
    var id = Globalize.parseInt($("#hddIdInformacaoSeguroEmEdicao").val());
    var infomacoesSeguro = $("#hddInformacoesSeguro").val() == "" ? new Array() : JSON.parse($("#hddInformacoesSeguro").val());
    for (var i = 0; i < infomacoesSeguro.length; i++) {
        if (infomacoesSeguro[i].Id == id) {
            if (id <= 0)
                infomacoesSeguro.splice(i, 1);
            else
                infomacoesSeguro[i].Excluir = true;
            break;
        }
    }
    $("#hddInformacoesSeguro").val(JSON.stringify(infomacoesSeguro));
    RenderizarInformacaoSeguro();
    LimparCamposInformacaoSeguro();
}
function LimparCamposInformacaoSeguro() {
    $("#hddIdInformacaoSeguroEmEdicao").val('0');
    $("#selResponsavelSeguro").val($("#selResponsavelSeguro option:first").val());
    $("#txtNomeSeguradora").val('');
    $("#txtNumeroApolice").val('');
    $("#txtNumeroAverberacao").val('');
    $("#txtCNPJSeguradora").val('');
    $("#txtValorMercadoriaParaEfeitoDeAverbacao").val('0,00');
    $("#btnExcluirInformacaoSeguro").hide();
}
function RenderizarInformacaoSeguro() {
    var infomacoesSeguro = $("#hddInformacoesSeguro").val() == "" ? new Array() : JSON.parse($("#hddInformacoesSeguro").val());
    var campoCNPJ = "";
    $("#tblInformacaoSeguro tbody").html("");
    for (var i = 0; i < infomacoesSeguro.length; i++) {
        if (!infomacoesSeguro[i].Excluir) {
            campoCNPJ = "";
            
            if (EmissaoCTe.VersaoMDFe == "3.00") {
                if (!infomacoesSeguro[i].CNPJSeguradora || infomacoesSeguro[i].CNPJSeguradora.length != 14)
                    campoCNPJ = "<td>00.000.000/0000-00</td>";
                else
                    campoCNPJ = "<td>" + FormataMascara(infomacoesSeguro[i].CNPJSeguradora, "##.###.###/####-##") + "</td>";
            }
            $("#tblInformacaoSeguro tbody").append("<tr><td>" + infomacoesSeguro[i].DescricaoResponsavel + "</td><td>" + infomacoesSeguro[i].Seguradora + "</td>" + campoCNPJ + "<td>" + infomacoesSeguro[i].NumeroApolice + "</td><td>" + infomacoesSeguro[i].NumeroAverberacao + "</td><td>" + Globalize.format(infomacoesSeguro[i].ValorMercadoria, "n2") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarInformacaoSeguro(" + JSON.stringify(infomacoesSeguro[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblInformacaoSeguro tbody").html() == "")
        $("#tblInformacaoSeguro tbody").html("<tr><td colspan='6'>Nenhum registro encontrado.</td></tr>");
}