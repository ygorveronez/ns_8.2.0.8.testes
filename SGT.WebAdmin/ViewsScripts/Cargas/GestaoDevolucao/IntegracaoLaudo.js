//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaIntegracaoLaudo;
var _GRIDGestaODevolucaoEtapaIntegracaoLaudo;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaIntegracaoLaudo = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.Voltar = PropertyEntity({ text: "Voltar", eventClick: voltarIntegracaoLaudo, visible: ko.observable(true), enable: ko.observable(true) });

    this.auditarLaudoClick = auditarLaudoClick;

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaIntegracaoLaudo(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/IntegracaoLaudo.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaIntegracaoLaudo = new GestaoDevolucaoEtapaIntegracaoLaudo();
                KoBindings(_gestaoDevolucaoEtapaIntegracaoLaudo, "knockoutGestaoDevolucaoEtapaIntegracaoLaudo");
                _gestaoDevolucaoEtapaIntegracaoLaudo.CodigoGestaoDevolucao.val(_informacoesDevolucao.CodigoDevolucao.val());

                HeaderAuditoria("GestaoDevolucaoLaudo", _gestaoDevolucaoEtapaIntegracaoLaudo);
                _gestaoDevolucaoEtapaIntegracaoLaudo.CodigoGestaoDevolucao.val(_informacoesDevolucao.CodigoDevolucao.val());

                loadGridGestaoDevolucaoIntegracao(_gestaoDevolucaoEtapaIntegracaoLaudo.Grid.id, _gestaoDevolucaoEtapaIntegracaoLaudo.CodigoGestaoDevolucao.val(), etapa.etapa, r.Data);

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
function voltarIntegracaoLaudo() {
    LimparCampos(_gestaoDevolucaoEtapaIntegracaoLaudo);
    limparEtapasDevolucao();
}

function auditarLaudoClick() {
    __AbrirModalAuditoria();
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas