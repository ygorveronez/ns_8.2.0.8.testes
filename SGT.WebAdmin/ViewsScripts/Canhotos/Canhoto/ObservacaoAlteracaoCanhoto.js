/// <reference path="Canhoto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _observacaoAlteracaoCanhoto;

var ObservacaoAlteracaoCanhoto = function () {
    this.ObservacaoOperador = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Canhotos.Canhoto.Observacao.getFieldDescription(), maxlength: 300, required: false });
    this.DataAlteracaoCanhoto = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataAlteracaoCanhoto.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoRastreio = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Canhotos.Canhoto.CodigoRastreio.getFieldDescription(), maxlength: 100, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ConfirmarEntregaMotorista = PropertyEntity({ eventClick: EnviarAlteracaoStatus, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharModalObservacaoAlteracaoCanhoto, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Fechar, icon: "fa fa-window-close", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadObservacaoAlteracaoCanhoto() {
    _observacaoAlteracaoCanhoto = new ObservacaoAlteracaoCanhoto();
    KoBindings(_observacaoAlteracaoCanhoto, "divModalObservacaoAlteracaoCanhoto");
}

function AbrirModalObservacaoAlteracaoCanhotoClick(e) {
    if (_knoutPesquisar.SituacaoCanhoto.val().length !== 1)
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.NaoPermitidoConfirmarQuandoSelecionarMaisQueUmaSituacaoNoFiltro);

    LimparCampos(_observacaoAlteracaoCanhoto);
        
    Global.abrirModal('divModalObservacaoAlteracaoCanhoto');
}

function EnviarAlteracaoStatus() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.VoceRealmenteDesejaConfirmarOsCanhotosSelecionados, function () {
        PreencherListaConhecimentos();
        var dados = {
            SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
            CanhotosSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
            CanhotosNaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados()),
            Status: _knoutPesquisar.SituacaoCanhoto.val()[0],
            ListaCTes: _knoutPesquisar.ListaCTes.val(),
            ObservacaoOperador: _observacaoAlteracaoCanhoto.ObservacaoOperador.val(),
            DataAlteracaoCanhoto: _observacaoAlteracaoCanhoto.DataAlteracaoCanhoto.val(),
            CodigoRastreio: _observacaoAlteracaoCanhoto.CodigoRastreio.val()
        };

        var dadosPesquisa = $.extend({}, RetornarObjetoPesquisa(_knoutPesquisar), dados);

        executarReST("Canhoto/AlterarTodosStatus", dadosPesquisa, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.EnviadoComSucesso);
                    LimparTodosCampos();
                    buscarCanhotos();
                    FecharModalObservacaoAlteracaoCanhoto();
                    if (arg.Data != null && arg.Data.Mensagem != undefined && arg.Data.Mensagem != "")
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Data.Mensagem, 10000);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        })
    });
}

function FecharModalObservacaoAlteracaoCanhoto() {
    Global.fecharModal('divModalObservacaoAlteracaoCanhoto');
}