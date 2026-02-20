//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaOrdemERemessa;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaOrdemERemessa = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Ordem = PropertyEntity({ text: "Ordem", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true), required: ko.observable(true) });
    this.Remessa = PropertyEntity({ text: "Remessa", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true), required: ko.observable(true) });
    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: salvarOrdemERemessa, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
    this.Grid = PropertyEntity({ type: types.local });
}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaOrdemERemessa(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/OrdemERemessa.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaOrdemERemessa = new GestaoDevolucaoEtapaOrdemERemessa();
                KoBindings(_gestaoDevolucaoEtapaOrdemERemessa, "knockoutOrdemERemessa");

                PreencherObjetoKnout(_gestaoDevolucaoEtapaOrdemERemessa, r);
                loadGridGestaoDevolucaoIntegracao(_gestaoDevolucaoEtapaOrdemERemessa.Grid.id, _gestaoDevolucaoEtapaOrdemERemessa.CodigoGestaoDevolucao.val(), etapa.etapa);
                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaOrdemERemessa);

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
function salvarOrdemERemessa() {
    if (!ValidarCamposObrigatorios(_gestaoDevolucaoEtapaOrdemERemessa)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, "Preencha os campos obrigatórios!");
        return;
    }

    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaOrdemERemessa.CodigoGestaoDevolucao.val(),
        Ordem: _gestaoDevolucaoEtapaOrdemERemessa.Ordem.val(),
        Remessa: _gestaoDevolucaoEtapaOrdemERemessa.Remessa.val(),
    }
    executarReST("GestaoDevolucao/SalvarOrdemERemessa", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Ordem e Remessa salvas com sucesso.");
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