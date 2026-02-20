/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ParametroOcorrencia.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridParametros;
var _parametro;

/*
 * Declaração das Classes
 */

var Parametro = function () {
    this.ListaParametro = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.ListaParametro.val.subscribe(function () {
        recarregarGridParametro();
    });

    this.Parametro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.TipoOcorrencia.Parametro.getFieldDescription(), TipoParametro: 0, idBtnSearch: guid(), required: true });

    this.AdicionarParametro = PropertyEntity({ eventClick: adicionarParametroClick, type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.Adicionar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridParametro() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Ocorrencias.TipoOcorrencia.Excluir, id: guid(), metodo: excluirParametroClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Descricao, width: "80%" }
    ];

    _gridParametros = new BasicDataTable(_parametro.ListaParametro.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridParametros.CarregarGrid([]);
}

function loadParametro() {
    _parametro = new Parametro();
    KoBindings(_parametro, "knockoutParametros");

    new BuscarParametroOcorrencia(_parametro.Parametro, function (data) {
        _parametro.Parametro.codEntity(data.Codigo);
        _parametro.Parametro.val(data.Descricao);
        _parametro.Parametro.TipoParametro = data.TipoParametro;
    });

    loadGridParametro();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarParametroClick() {
    var valido = ValidarCampoObrigatorioEntity(_parametro.Parametro);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _parametro.Parametro.requiredClass("form-control is-invalid");
        return;
    }

    var listaParametro = obterListaParametro();

    if (isParametroDuplicado(listaParametro)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Ocorrencias.TipoOcorrencia.ParametroExistente, Localization.Resources.Ocorrencias.TipoOcorrencia.OJaEstaCarregado.format(_parametro.Parametro.val()));
        return;
    }

    var parametro = obterParametroSalvar();

    listaParametro.push(parametro);
    _parametro.ListaParametro.val(listaParametro);
    limparCamposCadastroParametro();

    $("#" + _parametro.Parametro.id).focus();
}

function excluirParametroClick(registroSelecionado) {
    var listaParametro = obterListaParametro();

    for (var i = 0; i < listaParametro.length; i++) {
        if (listaParametro[i].Codigo == registroSelecionado.Codigo) {
            listaParametro.splice(i, 1);
            break;
        }
    }

    _parametro.ListaParametro.val(listaParametro);
}

/*
 * Declaração das Funções Públicos
 */

function limparCamposParametro() {
    limparCamposCadastroParametro();

    _parametro.ListaParametro.val([]);
}

function preencherParametro(dadosParametro) {
    _parametro.ListaParametro.val(dadosParametro);
}

function preencherParametroSalvar(tipoOcorrencia) {
    var listaParametro = obterListaParametro();

    tipoOcorrencia["Parametros"] = JSON.stringify(listaParametro);
}

/*
 * Declaração das Funções Privadas
 */

function isParametroDuplicado(listaParametro) {
    for (var i = 0; i < listaParametro.length; i++) {
        if (listaParametro[i].Codigo == _parametro.Parametro.codEntity())
            return true;
    }

    return false;
}

function limparCamposCadastroParametro() {
    LimparCampos(_parametro);
}

function obterListaParametro() {
    return _parametro.ListaParametro.val().slice();
}

function obterParametroSalvar() {
    return {
        Codigo: _parametro.Parametro.codEntity(),
        Descricao: _parametro.Parametro.val(),
        TipoParametro: _parametro.Parametro.TipoParametro
    };
}

function recarregarGridParametro() {
    var listaParametro = obterListaParametro();

    var isPossuiParametroPeriodo = listaParametro.some(function (p) {
        return p.TipoParametro == EnumTipoParametroOcorrencia.Periodo
    });

    if (isPossuiParametroPeriodo)
        _tipoOcorrencia.UtilizarEntradaSaidaDoRaioCargaEntrega.visible(true);
    else
        _tipoOcorrencia.UtilizarEntradaSaidaDoRaioCargaEntrega.visible(false);

    _gridParametros.CarregarGrid(listaParametro);
}
