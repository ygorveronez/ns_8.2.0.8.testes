/// <reference path="FluxoPatio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _crudConfiguracaoAlerta;

var CRUDConfiguracaoAlerta = function () {
    this.Salvar = PropertyEntity({ eventClick: salvarConfiguracaoAlertaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar });
    this.Cancelar = PropertyEntity({ eventClick: fecharConfiguracaoAlertaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar });
};

//*******EVENTOS*******

function loadConfiguracaoAlerta() {
    _crudConfiguracaoAlerta = new CRUDConfiguracaoAlerta();
    KoBindings(_crudConfiguracaoAlerta, "knockoutCRUDConfiguracaoAlerta");
}

function abrirConfiguracaoAlertaClick() {
    if (_pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options().length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.GestaoPatio.FluxoPatio.NenhumaEtapaDisponivelParaFilialSelecionada);
        return;
    }

    if (_pesquisaFluxoPatio.Filial.multiplesEntities().length !== 1) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.GestaoPatio.FluxoPatio.ConfiguracaoDeAlertaExclusivaPorFilialNaoPodeSerUsadoQuandoTemMaisQueUmaSelecionada);
        return;
    }

    executarReST("FluxoPatio/BuscarConfiguracaoAlertaFilialDoUsuario", { Filial: _pesquisaFluxoPatio.Filial.codEntity() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;

                preencherHTMLConfiguracaoAlerta();
                loadConfiguracaoAlerta();

                if (Boolean(data.Etapas))
                    setarConfiguracoesAlerta(data.Etapas);
                                
                Global.abrirModal('divModalConfiguracaoAlertas');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function salvarConfiguracaoAlertaClick() {
    var dados = {
        Filial: _pesquisaFluxoPatio.Filial.codEntity(),
        ConfiguracaoAlertaEtapas: obterConfiguracoesAlerta(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options())
    };

    executarReST("FluxoPatio/SalvarConfiguracaoAlerta", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.ConfiguracoesSalvasComSucesso);
                Global.fecharModal('divModalConfiguracaoAlertas');
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function fecharConfiguracaoAlertaClick() {
    Global.fecharModal('divModalConfiguracaoAlertas');
}

function ckbAlertaVisualTodosClick() {
    if ($("#ckb_AlertaVisualTodos").prop("checked"))
        $("#tableEtapasConfiguracaoAlerta input.alertaVisual").prop("checked", true);
    else
        $("#tableEtapasConfiguracaoAlerta input.alertaVisual").prop("checked", false);
}

function ckbAlertaSonoroTodosClick() {
    if ($("#ckb_AlertaSonoroTodos").prop("checked"))
        $("#tableEtapasConfiguracaoAlerta input.alertaSonoro").prop("checked", true);
    else
        $("#tableEtapasConfiguracaoAlerta input.alertaSonoro").prop("checked", false);
}

//*******MÉTODOS*******

function preencherHTMLConfiguracaoAlerta() {
    var html = "<ul>";
    html += criarHTMLEtapasConfiguracaoAlerta(_pesquisaFluxoPatio.EtapaFluxoGestaoPatio.options());
    html += "</ul>";
    $("#divEtapasAlertas").html(html);
}

function criarHTMLEtapasConfiguracaoAlerta(etapas) {
    var html = "";
    html += "<li>";
    html += "<table class='table table-bordered table-hover table-condensed table-striped' style='position: relative;z-index: 1;margin-top: -26px;margin-bottom: -6px;background: #FFF;' id='tableEtapasConfiguracaoAlerta'>";

    html += "<thead>";
    html += "<tr>";
    html += "<td><label>" + Localization.Resources.GestaoPatio.FluxoPatio.Etapa + "</label></td>";
    html += "<td style='width:25%'><input type='checkbox' style='margin-top:0px' onclick='ckbAlertaVisualTodosClick();' id='ckb_AlertaVisualTodos' />&nbsp;<label for='ckb_AlertaVisualTodos'>" + Localization.Resources.GestaoPatio.FluxoPatio.AlertaVisualPiscando + "</label></td>";
    html += "<td style='width:25%'><input type='checkbox' style='margin-top:0px'  onclick='ckbAlertaSonoroTodosClick();' id='ckb_AlertaSonoroTodos' />&nbsp;<label for='ckb_AlertaSonoroTodos'>" + Localization.Resources.GestaoPatio.FluxoPatio.AlertaSonoro + "</label></td>";
    html += "</tr>";
    html += "</thead>";
    html += "<tbody>";
    $.each(etapas, function (i, etapa) {
        html += "<tr>";
        html += "<td><label>" + etapa.text + "</label></td>";
        html += "<td><input type='checkbox' style='margin-top:0px' class='alertaVisual' id='ckbAlertaVisual_" + etapa.value + "' /></td>";
        html += "<td><input type='checkbox' style='margin-top:0px' class='alertaSonoro' id='ckbAlertaSonoro_" + etapa.value + "' /></td>";
        html += "</tr>";
    });
    html += "</tbody>";
    html += "</table>";
    html += "</li>";
    return html;
}

function obterConfiguracoesAlerta(etapas) {
    var listEtapas = new Array();

    $.each(etapas, function (j, etapa) {
        var configuracaoMap = new Object();
        configuracaoMap.CodigoEtapa = etapa.value;
        configuracaoMap.AlertaVisual = $("#ckbAlertaVisual_" + etapa.value).prop("checked");
        configuracaoMap.AlertaSonoro = $("#ckbAlertaSonoro_" + etapa.value).prop("checked");

        listEtapas.push(configuracaoMap);
    });

    return JSON.stringify(listEtapas);
}

function setarConfiguracoesAlerta(etapasSalvas) {
    $.each(etapasSalvas, function (i, etapa) {
        if ($("#ckbAlertaVisual_" + etapa.EtapaFluxoGestaoPatio).closest("table").attr("id") != null) {
            if (etapa.AlertaVisual) {
                $("#ckbAlertaVisual_" + etapa.EtapaFluxoGestaoPatio).prop("checked", true);
                if ($("#tableEtapasConfiguracaoAlerta input.alertaVisual:checkbox:not(:checked)").length == 0)
                    $("#ckb_AlertaVisualTodos").prop("checked", true);
            }

            if (etapa.AlertaSonoro) {
                $("#ckbAlertaSonoro_" + etapa.EtapaFluxoGestaoPatio).prop("checked", true);
                if ($("#tableEtapasConfiguracaoAlerta input.alertaSonoro:checkbox:not(:checked)").length == 0)
                    $("#ckb_AlertaSonoroTodos").prop("checked", true);
            }
        }
    });
}