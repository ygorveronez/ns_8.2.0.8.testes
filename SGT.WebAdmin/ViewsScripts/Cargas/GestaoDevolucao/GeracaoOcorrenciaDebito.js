// #region Objetos Globais do Arquivo
var _gestaoDevolucaoGeracaoOcorrenciaDebito
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaGeracaoOcorrenciaDebito = function () {
    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: null, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: null, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaGeracaoOcorrenciaDebito(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", { CodigoGestaoDevolucao: _informacoesDevolucao.CodigoDevolucao.val(), EtapaGestaoDevolucao: 'GeracaoOcorrenciaDebito' }, function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/GeracaoOcorrenciaDebito.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);
                _gestaoDevolucaoGeracaoOcorrenciaDebito = new GestaoDevolucaoEtapaGeracaoOcorrenciaDebito();
                KoBindings(_gestaoDevolucaoGeracaoOcorrenciaDebito, "knockoutGeracaoOcorrenciaDebito");

                loadOcorrenciaGestao(function () {
                    buscarOcorrenciaGestao(r.Data.CodigoOcorrencia);
                });

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoGeracaoOcorrenciaDebito);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Privadas
function loadOcorrenciaGestao(callback) {
    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia", function () {
        _ocorrencia.CodigoGestaoDevolucao.val(_informacoesDevolucao.CodigoDevolucao.val());
        _ocorrencia.NaoLimparCarga.val(true);
        const situacao = _gestaoDevolucaoGeracaoOcorrenciaDebito.SituacaoDevolucao.val();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe || (situacao !== null && situacao != 0)) {
            _ocorrencia.Carga.enable(false);
            _ocorrencia.TipoOcorrencia.enable(false);
            _ocorrencia.Quantidade.enable(false);
            _ocorrencia.DataOcorrencia.enable(false);
            _ocorrencia.ComponenteFrete.enable(false);
            _ocorrencia.Observacao.enable(false);
            _ocorrencia.Anexo.enable(false);
            _CRUDOcorrencia.Adicionar.visible(false);
        }

        if (typeof callback === "function") {
            callback();
        }
    });
}

function buscarOcorrenciaGestao(codigoOcorrencia) {
    if (codigoOcorrencia > 0) {
        _ocorrencia.Codigo.val(codigoOcorrencia);
        buscarOcorrenciaPorCodigo(function () {
        });
    }
}
// #endregion Funções Privadas