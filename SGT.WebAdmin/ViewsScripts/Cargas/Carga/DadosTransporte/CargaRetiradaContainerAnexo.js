/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Container.js" />
/// <reference path="../../../Consultas/LocalRetiradaContainer.js" />

// #region Objetos Globais do Arquivo

var _cargaRetiradaContainerAnexo;
var _gridCargaRetiradaContainerAnexo;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CargaRetiradaContainerAnexo = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoContainer = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Anexo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Anexos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaRetiradaContainerAnexo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCargaRetiradaContainerAnexo() {
    _cargaRetiradaContainerAnexo = new CargaRetiradaContainerAnexo();
    KoBindings(_cargaRetiradaContainerAnexo, "knockoutInformarRetiradaContainerAnexo");

    loadGridCargaRetiradaContainerAnexo();
    //controlarExibicaoAba();
}

function loadGridCargaRetiradaContainerAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: downloadCargaRetiradaContainerAnexoClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoDownload] };

    _gridCargaRetiradaContainerAnexo = new GridView(_cargaRetiradaContainerAnexo.Anexos.idGrid, "ColetaContainerAnexo/PesquisaAnexo", { CodigoCarga: _cargaRetiradaContainerAnexo.CodigoCarga }, menuOpcoes, null, linhasPorPaginas);
    _gridCargaRetiradaContainerAnexo.CarregarGrid();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function downloadCargaRetiradaContainerAnexoClick(registroSelecionado) {
    executarDownload("ColetaContainerAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

// #endregion Funções Associadas a Eventos