/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="RotaFrete.js" />

// #region Objetos Globais do Arquivo

var _gridLocalidade;
var _localidade;
var _buscaLocalidade;

// #endregion Objetos Globais do Arquivo

// #region Classes

var Localidade = function () {
    this.Ordenar = PropertyEntity({ val: _rotaFrete.OrdenarLocalidades.val, def: _rotaFrete.OrdenarLocalidades.def, getType: _rotaFrete.OrdenarLocalidades.getType });

    this.Ordenar.val.subscribe(ordenarLocalidadeChange);

    this.Localidade = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarLocalidade, idBtnSearch: guid(), idGrid: guid(), issue: 12 });
}

// #endregion Classes

// #region Funções de Inicialização

function loadBuscaLocalidade() {
    _buscaLocalidade = new BuscarLocalidades(_localidade.Localidade, null, null, callbackBuscaLocalidade, _gridLocalidade);
}

function loadGridLocalidade() {
    if (_localidade.Ordenar.val())
        loadGridLocalidadeComOrdem();
    else
        loadGridLocalidadeSemOrdem();
}

function loadGridLocalidadeComOrdem() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirLocalidadeClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoExcluir] };
    var isExibirPaginacao = false;
    var mostrarInfo = false;
    var ordenacao = { column: 4, dir: orderDir.asc };
    var quantidadePorPagina = 9999999;
    var header = [];

    header.push({ data: "DT_RowId", visible: false });
    header.push({ data: "Codigo", visible: false });
    header.push({ data: "Latitude", visible: false });
    header.push({ data: "Longitude", visible: false });
    header.push({ data: "Ordem", title: Localization.Resources.Logistica.RotaFrete.Ordem, width: "20%", className: "text-align-center", orderable: false });
    header.push({ data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%", orderable: false });

    _gridLocalidade = new BasicDataTable(_localidade.Localidade.idGrid, header, menuOpcoes, ordenacao, null, quantidadePorPagina, mostrarInfo, isExibirPaginacao, null, callbackOrdenacaoLocalidade);
}

function loadGridLocalidadeSemOrdem() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirLocalidadeClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoExcluir] };
    var ordenacao = { column: 4, dir: orderDir.asc };
    var header = [];

    header.push({ data: "DT_RowId", visible: false });
    header.push({ data: "Codigo", visible: false });
    header.push({ data: "Latitude", visible: false });
    header.push({ data: "Longitude", visible: false });
    header.push({ data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" });

    _gridLocalidade = new BasicDataTable(_localidade.Localidade.idGrid, header, menuOpcoes, ordenacao);
}

function loadLocalidade() {
    _localidade = new Localidade();
    KoBindings(_localidade, "knockoutLocalidade");

    loadGridLocalidade();
    loadBuscaLocalidade();

    _gridLocalidade.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirLocalidadeClick(registroSelecionado) {
    var localidades = _gridLocalidade.BuscarRegistros();
    var localidadesSalvar = [];

    for (var i = 0; i < localidades.length; i++) {
        var localidade = localidades[i];

        if (localidade.Ordem > registroSelecionado.Ordem)
            localidade.Ordem--;

        if (localidade.DT_RowId != registroSelecionado.DT_RowId)
            localidadesSalvar.push(localidade);
    }

    _gridLocalidade.CarregarGrid(localidadesSalvar);
}

function ordenarLocalidadeChange() {
    var listaRegistros = _gridLocalidade.BuscarRegistros();
    var listaRegistrosAdicionar = [];
    var ordenar = _localidade.Ordenar.val();

    _buscaLocalidade.Destroy();
    _gridLocalidade.Destroy();

    loadGridLocalidade();
    loadBuscaLocalidade();

    for (var i = 0; i < listaRegistros.length; i++) {
        var registroAdicionar = listaRegistros[i];

        listaRegistrosAdicionar.push({
            DT_RowId: registroAdicionar.Codigo,
            Codigo: registroAdicionar.Codigo,
            Latitude: registroAdicionar.Latitude,
            Longitude: registroAdicionar.Longitude,
            Ordem: ordenar ? (i + 1) : 0,
            Descricao: registroAdicionar.Descricao
        });
    }

    _gridLocalidade.CarregarGrid(listaRegistrosAdicionar);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function preencherLocalidade(dadosLocalidades) {
    _localidade.Ordenar.val(dadosLocalidades.Ordenar);
    _gridLocalidade.CarregarGrid(dadosLocalidades.Localidades);
}

function preencherLocalidadeSalvar(rotaFrete) {
    rotaFrete["OrdenarLocalidades"] = _localidade.Ordenar.val();
    rotaFrete["Localidades"] = JSON.stringify(_gridLocalidade.BuscarRegistros());
}

function limparCamposLocalidade() {
    _gridLocalidade.CarregarGrid([]);

    LimparCampos(_localidade);
}

// #endregion Funções Públicas

// #region Funções Privadas

function callbackBuscaLocalidade(registrosSelecionados) {
    var listaRegistros = _gridLocalidade.BuscarRegistros();
    var ultimaOrdem = listaRegistros.length;
    var ordenar = _localidade.Ordenar.val();

    for (var i = 0; i < registrosSelecionados.length; i++) {
        var registroAdicionar = registrosSelecionados[i];

        listaRegistros.push({
            DT_RowId: registroAdicionar.Codigo,
            Codigo: registroAdicionar.Codigo,
            Latitude: registroAdicionar.Latitude,
            Longitude: registroAdicionar.Longitude,
            Ordem: ordenar ? ++ultimaOrdem : 0,
            Descricao: registroAdicionar.Descricao
        });
    }

    _gridLocalidade.CarregarGrid(listaRegistros);
}

function callbackOrdenacaoLocalidade(retornoOrdenacao) {
    var listaRegistros = _gridLocalidade.BuscarRegistros();
    var listaRegistrosReordenada = [];

    for (var i = 0; i < retornoOrdenacao.listaRegistrosReordenada.length; i++) {
        var registroReordenado = retornoOrdenacao.listaRegistrosReordenada[i];

        for (var j = 0; j < listaRegistros.length; j++) {
            var registro = listaRegistros[j];

            if (registro.DT_RowId == registroReordenado.idLinha) {
                registro.Ordem = registroReordenado.posicao;
                listaRegistrosReordenada.push(registro);
                break;
            }
        }
    }

    _gridLocalidade.CarregarGrid(listaRegistrosReordenada);
}

// #endregion Funções Privadas
