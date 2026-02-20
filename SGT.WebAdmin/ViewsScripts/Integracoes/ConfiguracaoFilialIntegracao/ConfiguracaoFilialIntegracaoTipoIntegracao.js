//#region Variaveis
var _filialIntegracao;
var _gridFilialIntegracao;
//#endregion


//#region Constructores
function FilialIntegracao() {
    this.SistemaIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Integracao, val: ko.observable(""), def: "", options: ko.observable([]), required: true });
    this.GridIntegracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, id: guid(), idTab: guid() });
    this.ListaTipoIntegracao = PropertyEntity({ type: types.dynamic, val: ko.observable(new Array()) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarIntegracaoConfigFilialIntegrafcaoEventClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}
//#endregion

//#region Funções de carregamento

function loadGridFilialIntegracao() {
    _filialIntegracao = new FilialIntegracao();
    KoBindings(_filialIntegracao, "knockoutFilialIntegracao");
    GridFilialIntegracao();
    buscaIntegracoes();
}

function GridFilialIntegracao() {

    var excluir = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: removerIntegracao, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridFilialIntegracao = new BasicDataTable(_filialIntegracao.GridIntegracoes.id, header, menuOpcoes);
    _gridFilialIntegracao.CarregarGrid([]);
}

//#endregion

function buscaIntegracoes() {
    return new Promise(function (resolve) {        
        executarReST("ConfiguracaoFilialIntegracao/BuscarIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { Codigo: d.Codigo, value: d.Tipo, text: d.Descricao } });
                _filialIntegracao.SistemaIntegracao.options(integracoes);
                _filialIntegracao.ListaTipoIntegracao.val(integracoes);
            }
            resolve();
        });
    });
}

function adicionarIntegracaoConfigFilialIntegrafcaoEventClick() {
    var listaGrid = ObterRegistrosFilialIntegracoesGrid();
    listaGrid.push(obterIntegracaoSalvar());
    _gridFilialIntegracao.CarregarGrid(listaGrid);
}

function removerIntegracao(registroSelecionado) {
    var listaIntegracao = ObterRegistrosFilialIntegracoesGrid();
    listaIntegracao.forEach(function (sistemaIntegracao, i) {
        if (registroSelecionado.Codigo == sistemaIntegracao.Codigo) {
            listaIntegracao.splice(i, 1);
        }
    });
    _filialIntegracao.GridIntegracoes.val(listaIntegracao);
    _gridFilialIntegracao.CarregarGrid(listaIntegracao);

}

function ObterRegistrosFilialIntegracoesGrid() {
    return _gridFilialIntegracao.BuscarRegistros();
}

function obterIntegracaoSalvar() {
    return {
        Codigo: obterCodigoIntegracao(),
        Tipo: _filialIntegracao.SistemaIntegracao.val(),
        Descricao: obterDescricaoSistemaIntegracao()
    };
}

function obterDescricaoSistemaIntegracao() {
    var tipo = _filialIntegracao.SistemaIntegracao.val();
    var integracoes = EnumTipoIntegracao.obterOpcoes();
    for (var i = 0; i < integracoes.length; i++) {
        if (integracoes[i].value == tipo) {
            return integracoes[i].text;
        }
    }

    return "";
}

function obterCodigoIntegracao() {
    var tipo = _filialIntegracao.SistemaIntegracao.val();
    var integracoes = _filialIntegracao.ListaTipoIntegracao.val();
    for (var i = 0; i < integracoes.length; i++) {
        if (integracoes[i].value == tipo) {
            return integracoes[i].Codigo;
        }
    }
    return 0;
}

function SetaGridIntegracoes(data) {
    _gridFilialIntegracao.CarregarGrid(data);
}

function limparCamposIntegracoes() {
    _gridFilialIntegracao.CarregarGrid([]);
    _filialIntegracao.SistemaIntegracao.val("");
}

function ObterTiposIntegracaoSalvar() {
    _configuracaoFilialIntegracao.TiposIntegracao.val(JSON.stringify(ObterRegistrosFilialIntegracoesGrid()));
}