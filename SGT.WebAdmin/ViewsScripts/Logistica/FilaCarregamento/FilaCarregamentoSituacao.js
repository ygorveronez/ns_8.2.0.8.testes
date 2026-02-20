/// <reference path="FilaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFilaCarregamentoVeiculo.js" />

/*
 * Declaração das Classes
 */

var FilaCarregamentoSituacao = function () {
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.Remover = PropertyEntity({ eventClick: this._alterarSituacaoUtilizandoModal, type: types.event, text: ko.observable("Remover"), visible: true });
    
    this._centroCarregamento = 0;
    this._codigo = 0;
    this._situacao = 0;
}

FilaCarregamentoSituacao.prototype = {
    alterarSituacaoFila: function (codigo, centroCarregamento, idContainerDestino) {
        const novaSituacao = this._obterNovaSituacao(idContainerDestino);

        this._recarregarDadosFila(codigo, centroCarregamento, novaSituacao);
        this._alterarSituacao();
    },
    remover: function (codigo) {
        this._recarregarDadosFila(codigo, 0, EnumSituacaoFilaCarregamentoVeiculo.Removida);

        this._exibirModalRemocao();
    },
    _alterarSituacao: function () {
        const self = this;

        executarReST("FilaCarregamento/AlterarSituacaoFila", self._obterDadosFila(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    self._fecharModalRemocao();
                    self._limparCampos();
                    recarregarFilaCarregamento();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro removido da fila com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _alterarSituacaoUtilizandoModal: function () {
        if (ValidarCampoObrigatorioEntity(this.Motivo))
            this._alterarSituacao();
        else
            exibirMensagemCamposObrigatorio();
    },
    _exibirModalRemocao: function () {
        const self = this;

        Global.abrirModal('divModalRemocaoFilaCarregamento');
        $("#divModalRemocaoFilaCarregamento").one('hidden.bs.modal', function () {
            self._limparCampos();
        });
    },
    _fecharModalRemocao: function () {
        Global.fecharModal('divModalRemocaoFilaCarregamento');
    },
    _limparCampos: function () {
        LimparCampoEntity(this.Motivo);
        this._recarregarDadosFila(0, 0, EnumSituacaoFilaCarregamentoVeiculo.Todas);
    },
    _obterDadosFila: function () {
        return {
            CentroCarregamento: this._centroCarregamento,
            Codigo: this._codigo,
            Motivo: this.Motivo.codEntity(),
            Situacao: this._situacao,
            Observacao: this.Observacao.val()
        }
    },
    _obterNovaSituacao: function (idContainerDestino) {
        switch (idContainerDestino) {
            case "container-grid-fila-carregamento":
                return EnumSituacaoFilaCarregamentoVeiculo.Disponivel;

            case "container-grid-fila-em-transicao":
                return EnumSituacaoFilaCarregamentoVeiculo.EmTransicao;
        }
    },
    _recarregarDadosFila: function (codigo, centroCarregamento, situacao) {
        this._centroCarregamento = centroCarregamento;
        this._codigo = codigo;
        this._situacao = situacao;
    }
}