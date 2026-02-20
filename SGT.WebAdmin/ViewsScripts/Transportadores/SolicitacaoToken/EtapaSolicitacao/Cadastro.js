/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../Enumeradores/EnumTipoAutenticacao.js" />
/// <reference path="../../../Enumeradores/EnumEtapaSolicitacaoToken.js" />

var _solicitacaoToken;

var SolicitacaoToken = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(), getType: typesKnockout.int, visible: false });
    this.Situacao = PropertyEntity({ val: ko.observable("") });
    this.PossuiRegras = PropertyEntity({ val: ko.observable(false) });
    this.NumeroProtocolo = PropertyEntity({ text: 'Número Protocolo:', val: ko.observable(), enable: ko.observable(false), getType: typesKnockout.int });
    this.DataInicioVigencia = PropertyEntity({ text: '*Data Início da Vigência:', val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.date, required: true });
    this.DataFimVigencia = PropertyEntity({ text: '*Data Fim da Vigência:', val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.date, required: true });
    this.Descricao = PropertyEntity({ text: '*Descrição:', val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.string, required: true });
    this.TipoAutenticacao = PropertyEntity({ text: 'Tipo de Autenticação:', enable: ko.observable(true), val: ko.observable(EnumTipoAutenticacao.Token), options: EnumTipoAutenticacao.obterOpcoes(), def: EnumTipoAutenticacao.Token, eventChange: exibirCampoTempoExpiracao });
    this.Observacao = PropertyEntity({ text: 'Observação:', val: ko.observable(), enable: ko.observable(true), getType: typesKnockout.string });
    this.TempoExpiracao = PropertyEntity({ text: 'Tempo de expiração (minutos):', val: ko.observable(0), visible: ko.observable(false), getType: typesKnockout.string })
}

var loadCadastroSolicitacaoToken = function () {
    _solicitacaoToken = new SolicitacaoToken();
    KoBindings(_solicitacaoToken, "knockoutSolicitacaoTokenCadastro");
}

function exibirCampoTempoExpiracao(event) {
    event.TempoExpiracao.visible(event.TipoAutenticacao.val() == EnumTipoAutenticacao.UsuarioESenha)
}