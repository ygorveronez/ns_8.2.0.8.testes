/// <reference path="../../Consultas/Cliente.js" />

//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaAgendamentoParaDescarga;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaAgendamentoParaDescarga = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });

    this.DataCarregamento = PropertyEntity({ text: "Data de Carregamento", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false) });
    this.DataDescarregamento = PropertyEntity({ text: "Data de Descarga Prevista", getType: typesKnockout.dateTime, val: ko.observable(null), required: ko.observable(true), enable: ko.observable(true) });

    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: salvarAgendamentoParaDescarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaAgendamentoParaDescarga(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/AgendamentoParaDescarga.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaAgendamentoParaDescarga = new GestaoDevolucaoEtapaAgendamentoParaDescarga();
                KoBindings(_gestaoDevolucaoEtapaAgendamentoParaDescarga, "knockoutAgendamentoParaDescarga");

                PreencherObjetoKnout(_gestaoDevolucaoEtapaAgendamentoParaDescarga, r);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaAgendamentoParaDescarga);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function salvarAgendamentoParaDescarga() {
    if (!ValidarCamposObrigatorios(_gestaoDevolucaoEtapaAgendamentoParaDescarga)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios!");
        return;
    }

    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaAgendamentoParaDescarga.CodigoGestaoDevolucao.val(),
        DataCarregamento: _gestaoDevolucaoEtapaAgendamentoParaDescarga.DataCarregamento.val(),
        DataDescarregamento: _gestaoDevolucaoEtapaAgendamentoParaDescarga.DataDescarregamento.val(),
    }
    executarReST("GestaoDevolucao/SalvarAgendamentoParaDescarga", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Local da coleta definido com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas