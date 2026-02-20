/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Globais.js" />

var _gridTokensTransportador, _gridRegras, _autorizacaoToken, _tokensDetalhes, _tokensTransportador, _pesquisaAutorizacaoToken, _motivoRejeicaoAutorizaocao, _gridMetodosSolicitados, _transportadoresAutorizacao, _gridTransportadoresAutorizacao;


var _situacaoToken = [
    { text: "Todas", value: "" },
    { text: "Finalizada", value: 1 },
    { text: "Rejeidada", value: 2 },
    { text: "Aprovada", value: 3 },
];

var _etapaToken = [
    { text: "Todas", value: "" },
    { text: "Sem regra de aprovação", value: 1 },
    { text: "Cancelado", value: 2 },
    { text: "Aguardando aprovação", value: 3 },
    { text: "Solicitação aprovada", value: 4 },
    { text: "Solicitação reprovada", value: 5 },
    { text: "Finalizada", value: 6 },
];

var _prioridadToken = [
    { text: "Todos", value: 99 },
    { text: 0, value: 0 },
    { text: 1, value: 1 },
    { text: 2, value: 2 },
    { text: 3, value: 3 },
    { text: 4, value: 4 },
    { text: 5, value: 5 },
    { text: 6, value: 6 },
];


var AutorizacaoToken = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Motivo = PropertyEntity({ text: "*Motivo: ", maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ eventClick: AbrirModalRejeicaoAutorizacaoToken, type: types.event, text: "Rejeitar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparRegrasAutorizacaoToken, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: aprovarMultiplasRegrasClick, text: "Aprovar Regras" });
}

var PesquisaAutorizacionTokenTransportador = function () {
    this.NumeroProtocolo = PropertyEntity({ text: "Número do Protocolo:", getType: typesKnockout.int });
    this.DataInicioVigencia = PropertyEntity({ text: "Data Início da Vigência de:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.date });
    this.DataFimVigencia = PropertyEntity({ text: "Data Final da Vigência até:", val: ko.observable(""), visible: ko.observable(true), enable: ko.observable(false), getType: typesKnockout.date });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(""), options: _etapaToken, def: "Todas", text: "Etapa de Autorização:", visible: ko.observable(true) });
    this.Prioridade = PropertyEntity({ val: ko.observable(""), options: _prioridadToken, def: 6, text: "*Prioridade de Aprovação:", visible: ko.observable(true), required: false });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTokensTransportador.CarregarGrid();
        }, type: types.event, text: "Pesquisar", visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}


var TokensTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todos", visible: ko.observable(true) });
    this.TokensGrid = PropertyEntity({ idGrid: guid() });
    this.Autorizar = PropertyEntity({ text: "Autorizar", type: types.event, eventClick: autorizaToken, visible: ko.observable(false) });
    this.Rejeitar = PropertyEntity({ text: "Rejeitar", type: types.event, eventClick: rejeitaToken, visible: ko.observable(false) });

}

var DetalhesToken = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.NumeroProtocolo = PropertyEntity({ text: "Número do Protocolo:", getType: typesKnockout.int });
    this.DataInicioVigencia = PropertyEntity({ text: "Data Inicial da Vigência:", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFimVigencia = PropertyEntity({ text: "Data Final da Vigência:", val: ko.observable(""), getType: typesKnockout.date });
    this.EtapaAutorizacao = PropertyEntity({ text: "Etapa de Autorização:", val: ko.observable(""), getType: typesKnockout.string });
    this.MetodosSolicitados = PropertyEntity({ type: types.BasicDataTable, val: ko.observable([]), codEntity: ko.observable(0), idGrid: guid() });
    this.GridMetodosSolicitados = PropertyEntity({ type: types.local });
}

var Transportadores = function () {
    this.Transportadores = PropertyEntity({ type: types.BasicDataTable, val: ko.observable([]), codEntity: ko.observable(0), idGrid: guid() });
    this.Grid = PropertyEntity({ type: types.local });
}

var MotivoRejeicaoAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.map, val: ko.observable(""), text: ko.observable("Motivo: "), maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ eventClick: RejeitarAlteracaoSolicitaoaoToken, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });
};

function LoadTokenTransportador() {
    _pesquisaAutorizacaoToken = new PesquisaAutorizacionTokenTransportador();
    KoBindings(_pesquisaAutorizacaoToken, "knockoutPesquisaAutorizacaoToken");

    _tokensTransportador = new TokensTransportador();
    KoBindings(_tokensTransportador, "knockoutAutorizacacoesToken");

    _tokensDetalhes = new DetalhesToken();
    KoBindings(_tokensDetalhes, "knockoutDetalhesToken");

    _autorizacaoToken = new AutorizacaoToken();
    KoBindings(_autorizacaoToken, "knockoutAutorizacaoToken");

    _motivoRejeicaoAutorizaocao = new MotivoRejeicaoAutorizacao();
    KoBindings(_motivoRejeicaoAutorizaocao, "knockoutMotivoRejeicaoAutorizacao");

    _transportadoresAutorizacao = new Transportadores();
    KoBindings(_transportadoresAutorizacao, "knockoutTransportadoresAutorizacao");

    BuscarFuncionario(_pesquisaAutorizacaoToken.Usuario);
    loadGridTokenAutorizacao();
    loadGridRegrasAutorizcaoToken();
    loadGridMetodosSolicitados();
    loadGridTransportadoresAutorizacao();
}

// Eventos

function loadGridTokenAutorizacao() {
    const opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: detalharValidacaoToken,
        tamanho: "10",
        icone: ""
    };

    const menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    const configuracaoExportacao = {
        url: "AutorizacaoToken/ExportarPesquisa",
        titulo: "Exportar"
    };

    _gridTokensTransportador = new GridView(_tokensTransportador.TokensGrid.idGrid, "AutorizacaoToken/Pesquisa", _pesquisaAutorizacaoToken, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridTokensTransportador.CarregarGrid();
}

function loadGridMetodosSolicitados() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "NomeMetodo", title: "Método" },
        { data: "RequisicoesMinuto", title: "Requisições por minuto" },
        { data: "QuantidadeRequisicoes", title: "Quantidade de Requisições" }
    ];

    _gridMetodosSolicitados = new BasicDataTable(_tokensDetalhes.GridMetodosSolicitados.id, header, null, { column: 1, dir: orderDir.desc });
    _gridMetodosSolicitados.CarregarGrid(_tokensDetalhes.MetodosSolicitados);
}

function loadGridTransportadoresAutorizacao() {
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "45%", className: "text-align-left" },
        { data: "CNPJ", title: "CNPJ", width: "45%", className: "text-align-center" },
    ];

    _gridTransportadoresAutorizacao = new BasicDataTable(_transportadoresAutorizacao.Grid.id, header, null, { column: 1, dir: orderDir.desc });

    _gridTransportadoresAutorizacao.CarregarGrid([]);
}

function autorizaToken() {
    const existemRegistrosSelecionados = _gridTokensTransportador.ObterMultiplosSelecionados();
    if (existemRegistrosSelecionados.length == 0)
        return exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor selecione registros para aprovar");

    const dados = {};
    dados.SelecionarTodos = _tokensTransportador.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridTokensTransportador.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridTokensTransportador.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoToken/AprovarMultiplosItens", dados, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registros Aprovados Com Sucesso");
    });

};

function rejeitaToken(e) {
    const dados = {};
    dados.SelecionarTodos = _pesquisaEstornoProvisao.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridEstornoProvisao.ObterMultiplosNaoSelecionados());
    executarReST("AutorizacaoToken/ReprovarMultiplosItens", dados, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Registros Reprovados Com Sucesso");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirOpcoes() {
    _tokensTransportador.Autorizar.visible(true);
    _tokensTransportador.Rejeitar.visible(true);

}


function detalharValidacaoToken(registroSelecionado) {

    _tokensTransportador.Codigo.val(registroSelecionado.Codigo);
    BuscarPorCodigo(_tokensTransportador, "AutorizacaoToken/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_tokensDetalhes, { Data: retorno.Data });
                PreencherObjetoKnout(_transportadoresAutorizacao, { Data: retorno.Data });
                _gridRegras.CarregarGrid();
                _gridMetodosSolicitados.CarregarGrid(_tokensDetalhes.MetodosSolicitados.val());
                _gridTransportadoresAutorizacao.CarregarGrid(_transportadoresAutorizacao.Transportadores.val());
                Global.abrirModal("divModalDetalheToken");
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", retorno.Msg);
            }
        }
    });
}

function exibirMultiplasToken() {
    _tokensTransportador.Autorizar.visible(false);
    _tokensTransportador.Rejeitar.visible(false);

    const existemRegistrosSelecionados = _gridTokensTransportador.ObterMultiplosSelecionados().length > 0;
    const selecionadoTodos = _tokensTransportador.SelecionarTodos.val();

    if (existemRegistrosSelecionados || selecionadoTodos) {
        _tokensTransportador.Autorizar.visible(true);
        _tokensTransportador.Rejeitar.visible(true);
    }
}

function loadGridRegrasAutorizcaoToken() {
    const opcaoAprovar = {
        descricao: "Aprovar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar;
        },
        metodo: AprovarAlteracaoSolicitaoaoToken
    };

    const opcaoRejeitar = {
        descricao: "Rejeitar",
        id: guid(),
        evento: "onclick",
        visibilidade: function (dataRow) {
            return dataRow.PodeAprovar;
        },
        metodo: AbrirModalRejeicaoAutorizacaoToken
    };

    const menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [opcaoAprovar, opcaoRejeitar]
    };

    _gridRegras = new GridView(_autorizacaoToken.Regras.idGrid, "AutorizacaoToken/RegrasAprovacao", _tokensDetalhes, menuOpcoes);
}



function AprovarAlteracaoSolicitaoaoToken(registroSelecionado) {
    executarReST("AutorizacaoToken/Aprovar", registroSelecionado, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                Global.fecharModal("divModalDetalheToken");
                limparRegrasAutorizacaoToken();
                _gridTokensTransportador.CarregarGrid();
                _gridRegras.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function AbrirModalRejeicaoAutorizacaoToken(regra) {
    _motivoRejeicaoAutorizaocao.Codigo.val(regra.Codigo);
    Global.abrirModal("divModalMotivoRejeicaoAutorizacao");
}

function RejeitarAlteracaoSolicitaoaoToken(rejeicao) {
    const data = {
        Codigo: rejeicao.Codigo.val(),
        Motivo: rejeicao.Motivo.val()
    }
executarReST("AutorizacaoToken/Reprovar", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Enviado com sucesso.");
                Global.fecharModal("divModalMotivoRejeicaoAutorizacao");
                Global.fecharModal("divModalDetalheToken");
                limparRegrasAutorizacaoToken();
                _gridTokensTransportador.CarregarGrid();
                _gridRegras.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}


function limparRegrasAutorizacaoToken() {
    _autorizacaoToken.Motivo.visible(false);
    _autorizacaoToken.Rejeitar.visible(false);

    LimparCampos(_autorizacaoToken);
}

function aprovarMultiplasRegrasClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja aprovar todas as regras?", function () {
        executarReST("AutorizacaoToken/AprovarMultiplasRegras", { Codigo: _tokensDetalhes.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.RegrasModificadas > 1) {
                        const alcadas = retorno.Data.RegrasModificadas > 1 ? " alçadas foram aprovadas." : " alçada foi aprovada.";
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.RegrasModificadas + alcadas);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Nenhuma alçada pendente para seu usuário.");

                    limparRegrasAutorizacaoToken();
                    _gridTokensTransportador.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        })
    })
}