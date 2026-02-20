/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filtroPeriodo;
var _gridFiltroPeriodo;

/*
 * Declaração das Classes
 */

var FiltroPeriodo = function () {
    this.ListaFiltroPeriodo = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.ListaFiltroPeriodo.val.subscribe(function () {
        recarregarGridFiltroPeriodo();
    });

    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Remetente.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Destinatario.getFieldDescription() , idBtnSearch: guid(), required: false });

    this.AdicionarFiltro = PropertyEntity({ eventClick: adicionarFiltroPeriodoClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Adicionar.getFieldDescription(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFiltroPeriodo() {
    _filtroPeriodo = new FiltroPeriodo();
    KoBindings(_filtroPeriodo, "knockoutFiltroPeriodo");

    new BuscarClientes(_filtroPeriodo.Remetente);
    new BuscarClientes(_filtroPeriodo.Destinatario);

    loadGridFiltroPeriodo();
}

function loadGridFiltroPeriodo() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [{ descricao: Localization.Resources.Ocorrencias.TipoOcorrencia.Excluir, id: guid(), evento: "onclick", tamanho: "15", icone: "", metodo: removerFiltroPeriodoClick }] };
    var header = [
        { data: "Remetente", visible: false },
        { data: "Destinatario", visible: false },
        { data: "DescricaoRemetente", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Remetente, width: "45%", className: "text-align-left" },
        { data: "DescricaoDestinatario", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Destinatario, width: "45%", className: "text-align-left" },
    ];

    _gridFiltroPeriodo = new BasicDataTable(_filtroPeriodo.ListaFiltroPeriodo.idGrid, header, menuOpcoes, null, null, 10);
    _gridFiltroPeriodo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarFiltroPeriodoClick() {
    var filtroPeriodoSalvar = obterFiltroPeriodoSalvar();
    
    if (filtroPeriodoSalvar.Remetente == filtroPeriodoSalvar.Destinatario) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.TipoOcorrencia.Filtro, Localization.Resources.Ocorrencias.TipoOcorrencia.RemetenteDestinatarioDevemSerDiferentes);
        return;
    }

    var listaFiltroPeriodo = obterListaFiltroPeriodo();

    if (isFiltroPeriodoDuplicado(listaFiltroPeriodo, filtroPeriodoSalvar)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.TipoOcorrencia.Filtro, Localization.Resources.Ocorrencias.TipoOcorrencia.JaExisteUmFiltroComMesmoDestinatario);
        return;
    }

    listaFiltroPeriodo.push(filtroPeriodoSalvar);
    _filtroPeriodo.ListaFiltroPeriodo.val(listaFiltroPeriodo);

    limparCamposCadastroFiltroPeriodo();
}

function removerFiltroPeriodoClick(registroSelecionado) {
    var listaFiltroPeriodo = obterListaFiltroPeriodo();

    for (var i = 0; i < listaFiltroPeriodo.length; i++) {
        var filtroPeriodo = listaFiltroPeriodo[i];

        if (filtroPeriodo.Remetente == registroSelecionado.Remetente && filtroPeriodo.Destinatario == registroSelecionado.Destinatario) {
            listaFiltroPeriodo.splice(i, 1);
            break;
        }
    }

    _filtroPeriodo.ListaFiltroPeriodo.val(listaFiltroPeriodo);
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposFiltroPeriodo() {
    limparCamposCadastroFiltroPeriodo();

    _filtroPeriodo.ListaFiltroPeriodo.val([]);
}

function preencherFiltroPeriodo(dadosFiltroPeriodo) {
    _filtroPeriodo.ListaFiltroPeriodo.val(dadosFiltroPeriodo);
}

function preencherFiltroPeriodoSalvar(tipoOcorrencia) {
    var listaFiltroPeriodo = obterListaFiltroPeriodo();

    tipoOcorrencia["FiltrosPeriodo"] = JSON.stringify(listaFiltroPeriodo);
}

/*
 * Declaração das Funções Privadas
 */

function isFiltroPeriodoDuplicado(listaFiltroPeriodo, filtroPeriodoSalvar) {
    for (var i = 0; i < listaFiltroPeriodo.length; i++) {
        var filtroPeriodo = listaFiltroPeriodo[i];

        if (filtroPeriodo.Remetente == filtroPeriodoSalvar.Remetente && filtroPeriodo.Destinatario == filtroPeriodoSalvar.Destinatario)
            return true;
    }

    return false;
}

function limparCamposCadastroFiltroPeriodo() {
    _filtroPeriodo.Remetente.codEntity(_filtroPeriodo.Remetente.defCodEntity);
    _filtroPeriodo.Destinatario.codEntity(_filtroPeriodo.Destinatario.defCodEntity);
    _filtroPeriodo.Remetente.val(_filtroPeriodo.Remetente.def);
    _filtroPeriodo.Destinatario.val(_filtroPeriodo.Destinatario.def);
}

function obterFiltroPeriodoSalvar() {
    return {
        Remetente: _filtroPeriodo.Remetente.codEntity(),
        Destinatario: _filtroPeriodo.Destinatario.codEntity(),
        DescricaoRemetente: _filtroPeriodo.Remetente.val(),
        DescricaoDestinatario: _filtroPeriodo.Destinatario.val()
    };
}

function obterListaFiltroPeriodo() {
    return _filtroPeriodo.ListaFiltroPeriodo.val().slice();
}

function recarregarGridFiltroPeriodo() {
    var listaFiltroPeriodo = obterListaFiltroPeriodo();

    _gridFiltroPeriodo.CarregarGrid(listaFiltroPeriodo);
}
