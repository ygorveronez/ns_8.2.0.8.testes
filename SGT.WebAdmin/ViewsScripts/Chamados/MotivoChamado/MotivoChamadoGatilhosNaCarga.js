/// <reference path="../../Enumeradores/EnumMotivoChamadoTipoGatilhoNaCarga.js" />

//#region Variaveis
var _gatilhosNaCarga;
var _gridGatilhosNaCarga;
//#endregion


//#region Constructores
function GatilhosNaCarga() {
    this.GatilhoNaCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Gatilho, val: ko.observable(EnumMotivoChamadoTipoGatilhoNaCarga.Nenhum), def: EnumMotivoChamadoTipoGatilhoNaCarga.Nenhum, options: ko.observable(EnumMotivoChamadoTipoGatilhoNaCarga.ObterOpcoes()), required: false });
    this.GridGatilhos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, id: guid(), idTab: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarGatilhoNaCargaEventClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}
//#endregion

//#region Funções de carregamento
function loadGridGatilhosNaCarga() {
    _gatilhosNaCarga = new GatilhosNaCarga();
    KoBindings(_gatilhosNaCarga, "knockoutGatilhosNaCarga");
    gridGatilhosNaCarga();
}

function gridGatilhosNaCarga() {
    var excluir = { descricao: Localization.Resources.Chamado.MotivoChamado.Remover, id: guid(), evento: "onclick", metodo: removerGatilhoNaCargaEventClick, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Gatilho", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridGatilhosNaCarga = new BasicDataTable(_gatilhosNaCarga.GridGatilhos.id, header, menuOpcoes);
    _gridGatilhosNaCarga.CarregarGrid([]);
}

//#endregion

function adicionarGatilhoNaCargaEventClick() {
    var gatilho = obterGatilhoSalvar();

    if (gatilho.Gatilho == EnumMotivoChamadoTipoGatilhoNaCarga.Nenhum) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Aviso, "Selecione um gatilho.");
        return;
    }

    var listaGrid = ObterGatilhosNaCarga();
    if (listaGrid.some(gat => gat.Gatilho == gatilho.Gatilho)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Aviso, "Gatilho já cadastrado.");
        return;
    }

    listaGrid.push(gatilho);
    _gridGatilhosNaCarga.CarregarGrid(listaGrid);
}

function removerGatilhoNaCargaEventClick(registroSelecionado) {
    var listaGatilhosNaCarga = ObterGatilhosNaCarga();
    listaGatilhosNaCarga.forEach(function (gatilho, i) {
        if (registroSelecionado.Codigo == gatilho.Codigo) {
            listaGatilhosNaCarga.splice(i, 1);
        }
    });
    _gatilhosNaCarga.GridGatilhos.val(listaGatilhosNaCarga);
    _gridGatilhosNaCarga.CarregarGrid(listaGatilhosNaCarga);

}

function ObterGatilhosNaCarga() {
    return _gridGatilhosNaCarga.BuscarRegistros();
}

function obterGatilhoSalvar() {
    var gatilho = _gatilhosNaCarga.GatilhoNaCarga.val();
    return {
        Codigo: guid(),
        Gatilho: gatilho,
        Descricao: EnumMotivoChamadoTipoGatilhoNaCarga.ObterDescricao(gatilho)
    };
}

function SetaGridGatilhosNaCarga() {
    const listGatilhos = _motivoChamado.GatilhosNaCarga.val();
    _gridGatilhosNaCarga.CarregarGrid(listGatilhos);
}

function limparCamposGatilhosNaCarga() {
    _gridGatilhosNaCarga.CarregarGrid([]);
    _gatilhosNaCarga.GatilhoNaCarga.val("");
}

function ObterGatilhosNaCargaSalvar() {
    _motivoChamado.GatilhosNaCarga.val(JSON.stringify(ObterGatilhosNaCarga()));
}