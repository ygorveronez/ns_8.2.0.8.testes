//#region Variaveis
var _gatilhosIntegracao;
var _gridGatilhosIntegracao;
//#endregion


//#region Constructores
function GatilhosIntegracao() {
    this.SistemaIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Integracao, val: ko.observable(""), def: "", options: ko.observable([]), required: true });
    this.GridIntegracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, id: guid(), idTab: guid() });
    this.ListaTipoIntegracao = PropertyEntity({ type: types.dynamic, val: ko.observable (new Array()) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarIntegracaoMotivoChamadoEventClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

}
//#endregion

//#region Funções de carregamento

function loadGridGatilhosIntegracao() {
    _gatilhosIntegracao = new GatilhosIntegracao();
    KoBindings(_gatilhosIntegracao, "knockoutGatilhosIntegracao");
    gridGatilhosIntegracao();
    buscaIntegracoes();
}

function gridGatilhosIntegracao() {

    var excluir = { descricao: Localization.Resources.Chamado.MotivoChamado.Remover, id: guid(), evento: "onclick", metodo: removerIntegracao, tamanho: "15", icone: "" };
    
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridGatilhosIntegracao = new BasicDataTable(_gatilhosIntegracao.GridIntegracoes.id, header, menuOpcoes);
    _gridGatilhosIntegracao.CarregarGrid([]);
}

//#endregion

function buscaIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("MotivoChamado/BuscarIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { Codigo: d.Codigo, value: d.Tipo, text: d.Descricao } });
                _gatilhosIntegracao.SistemaIntegracao.options(integracoes);
                _gatilhosIntegracao.ListaTipoIntegracao.val(integracoes);
            }
            resolve();
        });
    });
}

function adicionarIntegracaoMotivoChamadoEventClick() {
    var listaGrid = ObterRegistrosIntegracoesGrid();
    listaGrid.push(obterIntegracaoSalvar());
    _gridGatilhosIntegracao.CarregarGrid(listaGrid);
}

function removerIntegracao(registroSelecionado) {
    var listaIntegracao = ObterRegistrosIntegracoesGrid();
    listaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (registroSelecionado.Codigo == sistemaIntegracao.Codigo) {
            listaIntegracao.splice(i, 1);
        }
    });
    _gatilhosIntegracao.GridIntegracoes.val(listaIntegracao);
    _gridGatilhosIntegracao.CarregarGrid(listaIntegracao);

}

function ObterRegistrosIntegracoesGrid() {
    return _gridGatilhosIntegracao.BuscarRegistros();
}

function obterIntegracaoSalvar() {
    return {
        Codigo: obterCodigoIntegracao(),
        Tipo: _gatilhosIntegracao.SistemaIntegracao.val(),
        Descricao: obterDescricaoSistemaIntegracao()
    };
}

function obterDescricaoSistemaIntegracao() {
    var tipo = _gatilhosIntegracao.SistemaIntegracao.val();
    var integracoes = EnumTipoIntegracao.obterOpcoes();
    for (var i = 0; i < integracoes.length; i++) {
        if (integracoes[i].value == tipo) {
            return integracoes[i].text;
        }
    }

    return "";
}

function obterCodigoIntegracao() {
    var tipo = _gatilhosIntegracao.SistemaIntegracao.val();
    var integracoes = _gatilhosIntegracao.ListaTipoIntegracao.val();
    for (var i = 0; i < integracoes.length; i++) {
        if (integracoes[i].value == tipo) {
            return integracoes[i].Codigo;
        }
    }

    return 0;
}

function SetaGridIntegracoes(data) {
    _gridGatilhosIntegracao.CarregarGrid(data);
}

function limparCamposIntegracoes() {
    _gridGatilhosIntegracao.CarregarGrid([]);
    _gatilhosIntegracao.SistemaIntegracao.val("");
}

function ObterTiposIntegracaoSalvar() {
    _motivoChamado.TiposIntegracao.val(JSON.stringify(ObterRegistrosIntegracoesGrid()));
}