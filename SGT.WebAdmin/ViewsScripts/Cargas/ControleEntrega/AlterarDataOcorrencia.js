
// #region Objetos Globais do Arquivo

var _alterarDataOcorrencia;

// #endregion Objetos Globais do Arquivo

// #region Classes

var AlterarDataOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NovaDataDeOcorrencia.getFieldDescription(), required: true, getType: typesKnockout.dateTime });
    this.Alterar = PropertyEntity({ type: types.event, eventClick: alterarDataOcorrenciaClick, text: Localization.Resources.Cargas.ControleEntrega.Alterar, visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadAlterarDataOcorrencia() {
    _alterarDataOcorrencia = new AlterarDataOcorrencia();
    KoBindings(_alterarDataOcorrencia, "knoutAlterarDataOcorrencia");
}

function abrirModalAlterarDataOcorrencia(ocorrencia) {
    _alterarDataOcorrencia.Codigo.val(ocorrencia.Codigo);
    const data = moment(ocorrencia.DataOcorrencia, "DD/MM/YYYY hh:mm");
    console.log({ ocorrencia, data });

    _alterarDataOcorrencia.DataOcorrencia.val(ocorrencia.DataOcorrencia);
    _alterarDataOcorrencia.DataOcorrencia.def = ocorrencia.DataOcorrencia;

    Global.abrirModal("divModalAlterarDataOcorrencia");
    
    $("#divModalAlterarDataOcorrencia").one('hidden.bs.modal', function () {
        LimparCampos(_alterarDataOcorrencia);
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function alterarDataOcorrenciaClick() {
    Salvar(_alterarDataOcorrencia, "ControleEntregaEntrega/AlterarDataOcorrencia", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataAlteradaComSucesso);
                fecharModalAlterarDataOcorrencia();
                exibirDetalhesEntrega(_etapaAtualFluxo, { Codigo: _entrega.Codigo.val() })
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #endregion Funções Públicas

// #region Funções Privadas

function fecharModalAlterarDataOcorrencia() {
    Global.fecharModal("divModalAlterarDataOcorrencia");
}

// #endregion Funções Públicas
