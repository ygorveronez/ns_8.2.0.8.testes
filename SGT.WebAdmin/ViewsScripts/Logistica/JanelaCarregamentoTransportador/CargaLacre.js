/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroCargaLacre;
var _gridCargaLacre;

/*
 * Declaração das Classes
 */

var CadastroCargaLacre = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número: ", maxlength: 60, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCargaLacreClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCargaLacre() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 1, dir: orderDir.asc };
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirCargaLacreClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número do Lacre", width: "75%", className: "text-align-left", orderable: false }
    ];

    _gridCargaLacre = new BasicDataTable(_detalhesCarga.ListaCargaLacre.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCargaLacre.CarregarGrid([]);
}

function loadCargaLacre() {
    _cadastroCargaLacre = new CadastroCargaLacre();
    KoBindings(_cadastroCargaLacre, "knockoutCadastroCargaLacre");

    loadGridCargaLacre();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCargaLacreClick() {
    if (ValidarCamposObrigatorios(_cadastroCargaLacre)) {
        if (!isCargaLacreDuplicado()) {
            _detalhesCarga.ListaCargaLacre.val().push(obterCadastroCargaLacreSalvar());

            recarregarGridCargaLacre(true);
            fecharModalCadastroCargaLacre();
        }
        else
            exibirMensagem(tipoMensagem.atencao, "Registro Duplicado", "Número de lacre já adicionado na carga!");
    }
    else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

function adicionarCargaLacreModalClick() {
    _cadastroCargaLacre.Codigo.val(guid());

    exibirModalCadastroCargaLacre();
}

function excluirCargaLacreClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o lacre?", function () {
        removerCargaLacre(registroSelecionado.Codigo);
        recarregarGridCargaLacre(true);
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposCargaLacre() {
    preencherCargaLacre([], false);
}

function obterCargaLacresSalvar() {
    var listaCargaLacre = obterListaCargaLacre();

    return JSON.stringify(listaCargaLacre);
}

function preencherCargaLacre(dadosCargaLacre, habilitarEdicaoDados) {
    _detalhesCarga.ListaCargaLacre.val(dadosCargaLacre);

    recarregarGridCargaLacre(habilitarEdicaoDados);
}

/*
 * Declaração das Funções
 */

function exibirModalCadastroCargaLacre() {
    Global.abrirModal('divModalCadastroCargaLacre');
    $("#divModalCadastroCargaLacre").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroCargaLacre);
    });
}

function fecharModalCadastroCargaLacre() {
    Global.fecharModal('divModalCadastroCargaLacre');
}

function isCargaLacreDuplicado() {
    var listaCargaLacre = obterListaCargaLacre();

    for (var i = 0; i < listaCargaLacre.length; i++) {
        if (_cadastroCargaLacre.Numero.val().trim() == listaCargaLacre[i].Numero.trim())
            return true;
    }

    return false;
}

function obterCadastroCargaLacreSalvar() {
    return {
        Codigo: _cadastroCargaLacre.Codigo.val(),
        Numero: _cadastroCargaLacre.Numero.val()
    };
}

function obterListaCargaLacre() {
    return _detalhesCarga.ListaCargaLacre.val().slice();
}

function recarregarGridCargaLacre(habilitarOpcoes) {
    var listaCargaLacre = obterListaCargaLacre();

    _gridCargaLacre.CarregarGrid(listaCargaLacre, habilitarOpcoes);
}

function removerCargaLacre(codigo) {
    var listaCargaLacre = obterListaCargaLacre();

    for (var i = 0; i < listaCargaLacre.length; i++) {
        var cargaLacre = listaCargaLacre[i];

        if (codigo == cargaLacre.Codigo)
            listaCargaLacre.splice(i, 1);
    }

    _detalhesCarga.ListaCargaLacre.val(listaCargaLacre);
}
