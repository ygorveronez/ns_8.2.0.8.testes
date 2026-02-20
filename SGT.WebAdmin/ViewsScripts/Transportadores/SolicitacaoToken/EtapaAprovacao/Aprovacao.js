
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../Enumeradores/EnumTipoAutenticacao.js" />

var _detalhesAutorizacaoToken;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({val: ko.observable(0)})
    this.DataSolicitacao = PropertyEntity({text:"Data do Solicitação: ", val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.string});
    this.AprovacoesNecessarias = PropertyEntity({text:"Aprovações Necessárias: ", val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.int});
    this.Aprovacoes = PropertyEntity({text:"Aprovações: ", val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.int});
    this.Reprovacoes = PropertyEntity({ text: "Reprovações: ", val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.int });
    this.Grid = PropertyEntity({ id: guid() });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasSolicitacao, type: types.event, text: 'Reprocessar Regras', visible: ko.observable(false) });
}

function DetalhesAutorizacaoToken() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

var loadEtapaAprovacaoToken = function () {
    _AprovacaoToken = new Aprovacao();
    KoBindings(_AprovacaoToken, "knockouEtapaAprovacaoToken");

    _detalhesAutorizacaoToken = new DetalhesAutorizacaoToken();
    KoBindings(_detalhesAutorizacaoToken, "knockoutDetalheAutorizacaoToken");
}

function reprocessarRegrasSolicitacao() {
    executarReST("AutorizacaoToken/ReprocessarRegras", { Codigo: _AprovacaoToken.Codigo.val() }, (arg) => {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras Reprocessadas com sucesso")
        limparTudo();
    })
}



function LimparDadosAutorizacaoTokenDetalhes() {
    LimparCampos(_detalhesAutorizacaoToken);
}