/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../ViewsScripts/Transportadores/SolicitacaoToken/EtapaAprovacao/Aprovacao.js" />
/// <reference path="./EtapaSolicitacao/PermissoesWS.js" />


var _pesquisaSolicitacaoToken;
var _gridSolicitacaoToken;
var _CRUDSolicitacaoToken;
var _tokensTransportadoresGrid;
var _gridTokensTransportadores;
var _gridAutorizadoresSolicitacaoToken;

var PesquisaSolicitacaoToken = function () {
    this.Descricao = PropertyEntity({ text: 'Descrição:', val: ko.observable(), getType: typesKnockout.string });
    this.NumeroProtocolo = PropertyEntity({ text: 'Número do Protocolo:', val: ko.observable(), getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSolicitacaoToken.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: 'grid-pesquisa-SolicitacaoToken', visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus"); //grid - pesquisa - SolicitacaoToken
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(false)
    });
};

var CRUDSolicitacaoToken = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarSolicitacaoTokenClick, type: types.event, text: 'Adicionar', visible: ko.observable(true) });
    this.LimparTudo = PropertyEntity({ eventClick: limparTudo, type: types.event, text: 'Limpar Tudo', visible: ko.observable(true) });
};

function TokensTransportadores() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Grid = PropertyEntity({ idGrid: guid() })
}

function loadSolicitacaoToken() {
    _pesquisaSolicitacaoToken = new PesquisaSolicitacaoToken();
    KoBindings(_pesquisaSolicitacaoToken, "knockoutPesquisaSolicitacaoToken");

    _CRUDSolicitacaoToken = new CRUDSolicitacaoToken();
    KoBindings(_CRUDSolicitacaoToken, "knockoutCRUDSolicitacaoToken");

    _tokensTransportadoresGrid = new TokensTransportadores();
    KoBindings(_tokensTransportadoresGrid, "knockoutTokens");


    loadGridSolicitacaoToken();
    loadEtapaSolicitacaoToken();
    loadEtapaAprovacaoToken();

    loadItensEtapaSolicitacao();
    loadGridAutorizadoresSolicitacao();
    loadGridTokensTransportadorSolicitacao();
}

function loadGridSolicitacaoToken() {
    const editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarSolicitacaoTokenClick };
    const menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [editar], icone: "" };

    _gridSolicitacaoToken = new GridView('grid-pesquisa-SolicitacaoToken', 'SolicitacaoToken/Pesquisa', _pesquisaSolicitacaoToken, menuOpcoes)
    _gridSolicitacaoToken.CarregarGrid();
}

function loadItensEtapaSolicitacao() {
    loadCadastroSolicitacaoToken();
    loadTransportadoresSolicitacaoToken();
    loadPermissoesWSSolicitacaoToken();
}

function limparTudo() {
    LimparCampos(_solicitacaoToken);
    LimparCamposPermissoesWSSolicitacaoToken();
    recarregarGridTransportadoresSolicitacaoToken();
    recarregarGridPermissoesWSSolicitacaoToken();

    _CRUDSolicitacaoToken.Adicionar.visible(true);
    _solicitacaoTokenTransportadores.Transportadores.visible(true);
    _CRUDSolicitacaoTokenPermissoesWS.Adicionar.visible(true);
    _CRUDSolicitacaoTokenPermissoesWS.Limpar.visible(true);
    SetarEnableCamposKnockout(_solicitacaoToken, true);
    SetarEnableCamposKnockout(_solicitacaoTokenPermissoesWS, true);
    _solicitacaoToken.NumeroProtocolo.enable(false);
    _AprovacaoToken.ReprocessarRegras.visible(false);
    setarEtapaInicioSolicitacaoToken();
}

function adicionarSolicitacaoTokenClick() {
    if (!validarSolicitacaoToken())
        return;

    executarReST("SolicitacaoToken/Adicionar", PreencherObjetoBack(), function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", r.Msg);
            limparTudo();
            _gridSolicitacaoToken.CarregarGrid();
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function validarSolicitacaoToken() {
    let valido = true;

    if (!ValidarCamposObrigatorios(_solicitacaoToken)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É Necessário preencher todos os campos obrigatórios do Cadastro");
        valido = false;
    }

    else if (_gridTransportadoresSolicitacaoToken.BuscarRegistros() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É Necessário adicionar pelo menos um Transportador na lista de Transportadores");
        valido = false;
    }

    else if (_gridPermissoesWSSolicitacaoToken.BuscarRegistros() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É Necessário adicionar pelo menos uma Permissão na lista de Permissões WebService");
        valido = false;
    }

    return valido;
}

function PreencherObjetoFront(dados) {
    PreencherObjetoKnout(_solicitacaoToken, dados);
    _gridTransportadoresSolicitacaoToken.CarregarGrid(dados.Transportadores);
    _gridPermissoesWSSolicitacaoToken.CarregarGrid(dados.PermissoesWS);
    PreencherObjetoKnout(_AprovacaoToken, { Data: dados.ResumoAprovacao});

}

function PreencherObjetoBack() {
    return {
        ...RetornarObjetoPesquisa(_solicitacaoToken),
        Transportadores: JSON.stringify(_gridTransportadoresSolicitacaoToken.BuscarRegistros().map(x => x = parseInt(x.Codigo))),
        PermissoesWebService: JSON.stringify(_gridPermissoesWSSolicitacaoToken.BuscarRegistros()),
    }
}

function editarSolicitacaoTokenClick(registro) {

    _solicitacaoToken.Codigo.val(registro.Codigo);
    _AprovacaoToken.Codigo.val(registro.Codigo);
    _tokensTransportadoresGrid.Codigo.val(registro.Codigo);

    BuscarPorCodigo(_solicitacaoToken, 'SolicitacaoToken/BuscarPorCodigo', (retorno) => {
        
        PreencherObjetoFront(retorno.Data);
        _AprovacaoToken.PossuiRegras.val(_solicitacaoToken.PossuiRegras.val())


        setarEtapasSolicitacaoToken();
        _gridAutorizadoresSolicitacaoToken.CarregarGrid();
        _gridTokensTransportadores.CarregarGrid()
        _CRUDSolicitacaoToken.Adicionar.visible(false);
        _solicitacaoTokenTransportadores.Transportadores.visible(false);
        _CRUDSolicitacaoTokenPermissoesWS.Adicionar.visible(false);
        _CRUDSolicitacaoTokenPermissoesWS.Limpar.visible(false);
        SetarEnableCamposKnockout(_solicitacaoToken, false);
        SetarEnableCamposKnockout(_solicitacaoTokenPermissoesWS, false);
    });

}


function loadGridAutorizadoresSolicitacao() {
    const Detalhes = { descricao: "Detalhes", id: guid(), metodo: abrirModalDetalhes };
    const menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 10, opcoes: [Detalhes], icone: "" };

    _gridAutorizadoresSolicitacaoToken = new GridView(_AprovacaoToken.Grid.id, 'SolicitacaoToken/ConsultarAutorizacoes', _AprovacaoToken, menuOpcoes);
    _gridAutorizadoresSolicitacaoToken.CarregarGrid();
}


function loadGridTokensTransportadorSolicitacao() {
    _gridTokensTransportadores = new GridView(_tokensTransportadoresGrid.Grid.idGrid, 'SolicitacaoToken/ObterTokensGeradosPelaSolicitacao', _tokensTransportadoresGrid)
    _gridTokensTransportadores.CarregarGrid()
}


function abrirModalDetalhes(e) {

    _detalhesAutorizacaoToken.Codigo.val(e.Codigo);
    BuscarPorCodigo(_detalhesAutorizacaoToken, "SolicitacaoToken/DetalhesAutorizacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data != null) {
                Global.abrirModal("divModalDetalhesAutorizacaoSolicitacaoToken");
                $("#divModalDetalhesAutorizacaoSolicitacaoToken").one('hidden.bs.modal', function () {
                    LimparDadosAutorizacaoTokenDetalhes();
                });
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
    });
}

