$(document).ready(function () {
    $("#txtValorServicoItem").priceFormat();
    $("#txtQuantidadeItem").priceFormat();
    $("#txtValorTotalItem").priceFormat();
    $("#txtValorDescontoIncondicionadoItem").priceFormat();
    $("#txtValorDescontoCondicionadoItem").priceFormat();
    $("#txtValorDeducoesItem").priceFormat();
    $("#txtBaseCalculoISSItem").priceFormat();
    $("#txtAliquotaISSItem").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtValorISSItem").priceFormat();
    // Base de cálculo IBS / CBS
    $("#txtBaseCalculoIBSCBSItem").priceFormat();

    // Valores IBS
    $("#txtValorIBSEstadualItem").priceFormat();
    $("#txtValorIBSMunicipalItem").priceFormat();
    $("#txtValorCBSItem").priceFormat();


    $("#txtAliquotaIBSEstadualItem").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtPercentualReducaoIBSEstadualItem").priceFormat({ prefix: '', centsLimit: 4 });

    $("#txtAliquotaIBSMunicipalItem").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtPercentualReducaoIBSMunicipalItem").priceFormat({ prefix: '', centsLimit: 4 });

    $("#txtAliquotaCBSItem").priceFormat({ prefix: '', centsLimit: 4 });
    $("#txtPercentualReducaoCBSItem").priceFormat({ prefix: '', centsLimit: 4 });

    CarregarConsultaDeServicosNFSe("btnBuscarServico", "btnBuscarServico", "A", RetornoConsultaServico, true, false);

    $("#selEstadoItem").change(function () {
        BuscarLocalidades($(this).val(), 'selLocalidadeItem');
    });

    $("#selEstadoIncidenciaItem").change(function () {
        BuscarLocalidades($(this).val(), 'selLocalidadeIncidenciaItem');
    });

    $("#btnSalvarItem").click(function () {
        SalvarItem();
    });

    $("#btnExcluirItem").click(function () {
        ExcluirItem();
    });

    $("#btnCancelarItem").click(function () {
        LimparCamposItem();
    });
});

function RetornoConsultaServico(servico) {
    $("body").data("servico", servico);
    $("#txtServico").val(servico.Descricao);
    $("#txtAliquotaISSItem").val(servico.Aliquota);
    $("#txtQuantidadeItem").val("1,00")
}

function SalvarItem() {
    if (ValidarCamposItem()) {

        var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");
        if (itens.length > 0 && $("#btnExcluirItem").is(':visible') == false) {
            ExibirMensagemAlerta("Somente é possível adicionar um Serviço!", "Atenção!", "placeholder-msgEmissaoNFSe");
        }
        else {
            var item = {
                Codigo: $("body").data("item") != null ? $("body").data("item").Codigo : 0,
                CodigoServico: $("body").data("servico").Codigo,
                DescricaoServico: $("body").data("servico").Descricao,
                Estado: $("#selEstadoItem").val(),
                Localidade: $("#selLocalidadeItem").val(),
                EstadoIncidencia: $("#selEstadoIncidenciaItem").val(),
                LocalidadeIncidencia: $("#selLocalidadeIncidenciaItem").val(),
                ServicoPrestadoPais: $("#selServicoPrestadoPais").val(),
                Pais: $("#selPaisItem").val(),
                ValorServico: Globalize.parseFloat($("#txtValorServicoItem").val()),
                Quantidade: Globalize.parseFloat($("#txtQuantidadeItem").val()),
                ValorTotal: Globalize.parseFloat($("#txtValorTotalItem").val()),
                ValorDescontoIncondicionado: Globalize.parseFloat($("#txtValorDescontoIncondicionadoItem").val()),
                ValorDescontoCondicionado: Globalize.parseFloat($("#txtValorDescontoCondicionadoItem").val()),
                ValorDeducoes: Globalize.parseFloat($("#txtValorDeducoesItem").val()),
                BaseCalculoISS: Globalize.parseFloat($("#txtBaseCalculoISSItem").val()),
                AliquotaISS: Globalize.parseFloat($("#txtAliquotaISSItem").val()),
                ValorISS: Globalize.parseFloat($("#txtValorISSItem").val()),
                ExigibilidadeISS: $("#selExigibilidadeISSItem").val(),
                Discriminacao: $("#txtDiscriminacaoItem").val(),
                IncluirISSNoFrete: $("#chkIncluirISSNoFrete")[0].checked,
                Excluir: false,
                NBS: $("#txtNBSItem").val(),
                CodigoIndicadorOperacao: $("#txtCodigoIndicadorOperacaoItem").val(),
                CSTIBSCBS: $("#txtCstibscbsItem").val(),
                ClassificacaoTributariaIBSCBS: $("#txtClassificacaoTributariaIBSCBSItem").val(),
                BaseCalculoIBSCBS: Globalize.parseFloat(
                    $("#txtBaseCalculoIBSCBSItem").val()
                ),
                AliquotaIBSEstadual: Globalize.parseFloat(
                    $("#txtAliquotaIBSEstadualItem").val()
                ),
                PercentualReducaoIBSEstadual: Globalize.parseFloat(
                    $("#txtPercentualReducaoIBSEstadualItem").val()
                ),
                ValorIBSEstadual: Globalize.parseFloat(
                    $("#txtValorIBSEstadualItem").val()
                ),
                AliquotaIBSMunicipal: Globalize.parseFloat(
                    $("#txtAliquotaIBSMunicipalItem").val()
                ),
                PercentualReducaoIBSMunicipal: Globalize.parseFloat(
                    $("#txtPercentualReducaoIBSMunicipalItem").val()
                ),
                ValorIBSMunicipal: Globalize.parseFloat(
                    $("#txtValorIBSMunicipalItem").val()
                ),
                AliquotaCBS: Globalize.parseFloat(
                    $("#txtAliquotaCBSItem").val()
                ),
                PercentualReducaoCBS: Globalize.parseFloat(
                    $("#txtPercentualReducaoCBSItem").val()
                ),
                ValorCBS: Globalize.parseFloat(
                    $("#txtValorCBSItem").val()
                )
            };

            var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

            itens.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

            if (item.Codigo == 0)
                item.Codigo = (itens.length > 0 ? (itens[0].Codigo > 0 ? -1 : (itens[0].Codigo - 1)) : -1);

            for (var i = 0; i < itens.length; i++) {
                if (itens[i].Codigo == item.Codigo) {
                    itens.splice(i, 1);
                    break;
                }
            }

            itens.push(item);

            $("body").data("itens", itens);

            RenderizarItens();
            LimparCamposItem();
        }

    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!", "placeholder-msgEmissaoNFSe");
    }
}

function EditarItem(item) {
    $("body").data("item", item);

    $("body").data("servico", { Codigo: item.CodigoServico, Descricao: item.DescricaoServico });
    $("#txtServico").val(item.DescricaoServico);

    $("#selEstadoItem").val(item.Estado);
    BuscarLocalidades(item.Estado, "selLocalidadeItem", item.Localidade);

    $("#selEstadoIncidenciaItem").val(item.EstadoIncidencia);
    BuscarLocalidades(item.EstadoIncidencia, "selLocalidadeIncidenciaItem", item.LocalidadeIncidencia);

    $("#selServicoPrestadoPais").val(item.ServicoPrestadoPais.toString());
    $("#selPaisItem").val(item.Pais);
    $("#txtValorServicoItem").val(Globalize.format(item.ValorServico, "n2"));
    $("#txtQuantidadeItem").val(Globalize.format(item.Quantidade, "n2"));
    $("#txtValorTotalItem").val(Globalize.format(item.ValorTotal, "n2"));
    $("#txtValorDescontoIncondicionadoItem").val(Globalize.format(item.ValorDescontoIncondicionado, "n2"));
    $("#txtValorDescontoCondicionadoItem").val(Globalize.format(item.ValorDescontoCondicionado, "n2"));
    $("#txtValorDeducoesItem").val(Globalize.format(item.ValorDeducoes, "n2"));
    $("#txtBaseCalculoISSItem").val(Globalize.format(item.BaseCalculoISS, "n2"));
    $("#txtAliquotaISSItem").val(Globalize.format(item.AliquotaISS, "n4"));
    $("#txtValorISSItem").val(Globalize.format(item.ValorISS, "n2"));
    $("#selExigibilidadeISSItem").val(item.ExigibilidadeISS);
    $("#txtDiscriminacaoItem").val(item.Discriminacao);    
    $("#chkIncluirISSNoFrete")[0].checked = item.IncluirISSNoFrete;
    /* === NOVOS CAMPOS === */

    $("#txtNBSItem").val(item.NBS);
    $("#txtCodigoIndicadorOperacaoItem").val(item.CodigoIndicadorOperacao);

    $("#txtCstibscbsItem").val(item.CSTIBSCBS);
    $("#txtClassificacaoTributariaIBSCBSItem").val(item.ClassificacaoTributariaIBSCBS);

    $("#txtBaseCalculoIBSCBSItem").val(
        Globalize.format(item.BaseCalculoIBSCBS, "n2")
    );

    /* IBS Estadual */
    $("#txtAliquotaIBSEstadualItem").val(
        Globalize.format(item.AliquotaIBSEstadual, "n4")
    );
    $("#txtPercentualReducaoIBSEstadualItem").val(
        Globalize.format(item.PercentualReducaoIBSEstadual, "n4")
    );
    $("#txtValorIBSEstadualItem").val(
        Globalize.format(item.ValorIBSEstadual, "n2")
    );

    /* IBS Municipal */
    $("#txtAliquotaIBSMunicipalItem").val(
        Globalize.format(item.AliquotaIBSMunicipal, "n4")
    );
    $("#txtPercentualReducaoIBSMunicipalItem").val(
        Globalize.format(item.PercentualReducaoIBSMunicipal, "n4")
    );
    $("#txtValorIBSMunicipalItem").val(
        Globalize.format(item.ValorIBSMunicipal, "n2")
    );

    /* CBS */
    $("#txtAliquotaCBSItem").val(
        Globalize.format(item.AliquotaCBS, "n4")
    );
    $("#txtPercentualReducaoCBSItem").val(
        Globalize.format(item.PercentualReducaoCBS, "n4")
    );
    $("#txtValorCBSItem").val(
        Globalize.format(item.ValorCBS, "n2")
    );
    $("#btnExcluirItem").show();
}

function ExcluirItem() {
    var item = $("body").data("item");

    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

    for (var i = 0; i < itens.length; i++) {
        if (itens[i].Codigo == item.Codigo) {
            if (item.Codigo <= 0)
                itens.splice(i, 1);
            else
                itens[i].Excluir = true;
            break;
        }
    }

    $("body").data("itens", itens);

    RenderizarItens();
    LimparCamposItem();
}

function ValidarCamposItem() {    
    var servico = $("body").data("servico");
    var localidade = $("#selLocalidadeItem").val();
    var localidadeIncidencia = $("#selLocalidadeIncidenciaItem").val();
    var pais = $("#selPaisItem").val();
    var valorServico = Globalize.parseFloat($("#txtValorServicoItem").val());
    var quantidade = Globalize.parseFloat($("#txtQuantidadeItem").val());
    var valorTotalItem = Globalize.parseFloat($("#txtValorTotalItem").val());
    var exigibilidadeISS = $("#selExigibilidadeISSItem").val();
    var discriminacao = $("#txtDiscriminacaoItem").val();

    var valido = true;

    if (servico == null || servico.Codigo <= 0) {
        CampoComErro("#txtServico");
        valido = false;
    } else {
        CampoSemErro("#txtServico");
    }

    if (localidade == null || localidade == "") {
        CampoComErro("#selLocalidadeItem");
        valido = false;
    } else {
        CampoSemErro("#selLocalidadeItem");
    }

    if (localidadeIncidencia == null || localidadeIncidencia == "") {
        CampoComErro("#selLocalidadeIncidenciaItem");
        valido = false;
    } else {
        CampoSemErro("#selLocalidadeIncidenciaItem");
    }

    if (pais == null || pais == "") {
        CampoComErro("#selPaisItem");
        valido = false;
    } else {
        CampoSemErro("#selPaisItem");
    }

    if (isNaN(valorServico) || valorServico <= 0) {
        CampoComErro("#txtValorServicoItem");
        valido = false;
    } else {
        CampoSemErro("#txtValorServicoItem");
    }

    if (isNaN(quantidade) || quantidade <= 0) {
        CampoComErro("#txtQuantidadeItem");
        valido = false;
    } else {
        CampoSemErro("#txtQuantidadeItem");
    }

    if (isNaN(valorTotalItem) || valorTotalItem <= 0) {
        CampoComErro("#txtValorTotalItem");
        valido = false;
    } else {
        CampoSemErro("#txtValorTotalItem");
    }

    if (exigibilidadeISS == null || exigibilidadeISS == "") {
        CampoComErro("#selExigibilidadeISSItem");
        valido = false;
    } else {
        CampoSemErro("#selExigibilidadeISSItem");
    }

    if (discriminacao == null || discriminacao == "") {
        CampoComErro("#txtDiscriminacaoItem");
        valido = false;
    } else {
        CampoSemErro("#txtDiscriminacaoItem");
    }

    return valido;
}

function LimparCamposItem() {
    $("body").data("item", null);
    $("body").data("servico", null);
    $("#txtServico").val("");

    $("#txtValorServicoItem").val("0,00");
    $("#txtQuantidadeItem").val("0,00");
    $("#txtValorTotalItem").val("0,00");
    $("#txtValorDescontoIncondicionadoItem").val("0,00");
    $("#txtValorDescontoCondicionadoItem").val("0,00");
    $("#txtValorDeducoesItem").val("0,00");
    $("#txtBaseCalculoISSItem").val("0,00");
    $("#txtAliquotaISSItem").val("0,0000");
    $("#txtValorISSItem").val("0,00");
    $("#selExigibilidadeISSItem").val($("#selExigibilidadeISSItem option:first").val());
    $("#txtDiscriminacaoItem").val("");
    $("#chkIncluirISSNoFrete").prop('checked', false);

    // IBS / CBS
    $("#txtBaseCalculoIBSCBSItem").val("0,00");

    $("#txtAliquotaIBSEstadualItem").val("0,0000");
    $("#txtPercentualReducaoIBSEstadualItem").val("0,0000");
    $("#txtValorIBSEstadualItem").val("0,00");

    $("#txtAliquotaIBSMunicipalItem").val("0,0000");
    $("#txtPercentualReducaoIBSMunicipalItem").val("0,0000");
    $("#txtValorIBSMunicipalItem").val("0,00");

    $("#txtAliquotaCBSItem").val("0,0000");
    $("#txtPercentualReducaoCBSItem").val("0,0000");
    $("#txtValorCBSItem").val("0,00");

    $("#btnExcluirItem").hide();

    if ($("body").data("configuracao") != null) {

        $("#selServicoPrestadoPais").val("true");
        $("#selPaisItem").val($("body").data("configuracao").Pais);

        $("#selEstadoItem").val($("body").data("configuracao").Estado);
        $("#selEstadoIncidenciaItem").val($("body").data("configuracao").Estado);

        BuscarLocalidades($("body").data("configuracao").Estado, ["selLocalidadeItem", "selLocalidadeIncidenciaItem"], $("body").data("configuracao").Cidade);

    } else {
        $("#selEstadoItem").val('');
        $("#selLocalidadeItem").html("");
        $("#selEstadoIncidenciaItem").val('');
        $("#selLocalidadeIncidenciaItem").html("");
        $("#selServicoPrestadoPais").val("");
        $("#selPaisItem").val("");
    }
}

function RenderizarItens(disabled) {
    var itens = $("body").data("itens") == null ? new Array() : $("body").data("itens");

    $("#tblItens tbody").html("");

    for (var i = 0; i < itens.length; i++) {
        if (!itens[i].Excluir)
            $("#tblItens tbody").append("<tr><td>" + itens[i].DescricaoServico + "</td><td>" + Globalize.format(itens[i].Quantidade, "n2") + "</td><td>" + Globalize.format(itens[i].ValorTotal, "n2") + "</td><td>" + Globalize.format(itens[i].ValorISS, "n2") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' " + (disabled ? "disabled" : "") + " onclick='EditarItem(" + JSON.stringify(itens[i]) + ")'>Editar</button></td></tr>");
    }

    if ($("#tblItens tbody").html() == "")
        $("#tblItens tbody").html("<tr><td colspan='5'>Nenhum registro encontrado.</td></tr>");
}