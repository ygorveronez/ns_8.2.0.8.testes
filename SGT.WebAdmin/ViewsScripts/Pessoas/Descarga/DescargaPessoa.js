/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDescarga;
var _descarga;
var _pesquisaDescarga;
var _crudDescarga;

var PesquisaDescarga = function () {
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
    this.PessoaOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa Origem:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDescarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Descarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: "*Pessoa:", idBtnSearch: guid() });
    this.PessoaOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Pessoa Origem:", idBtnSearch: guid() });
    this.HoraInicio = PropertyEntity({ text: "Hora Início: ", getType: typesKnockout.time, required: false });
    this.HoraFim = PropertyEntity({ text: "Hora Fim: ", getType: typesKnockout.time, required: false });
    this.ValorPallet = PropertyEntity({ text: "Valor por Pallet: ", val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, required: true });
    this.ValorPorVolume = PropertyEntity({ text: "Valor por Volume: ", val: ko.observable("0,000"), def: "0,000", getType: typesKnockout.decimal, required: true, configDecimal: { precision: 3, allowZero: true, allowNegative: false } });
    this.DeixarReboqueParaDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Deixa o reboque para descarga?", def: false });
};

var CRUDDescarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "DescargaPessoa/Importar",
        UrlConfiguracao: "DescargaPessoa/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O006_DescargaPessoas,
        CallbackImportacao: function () {
            _gridDescarga.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadDescarga() {

    _pesquisaDescarga = new PesquisaDescarga();
    KoBindings(_pesquisaDescarga, "knockoutPesquisaDescargaPessoa", false, _pesquisaDescarga.Pesquisar.id);

    _descarga = new Descarga();
    KoBindings(_descarga, "knockoutCadastroDescargaPessoa");

    HeaderAuditoria("ClienteDescarga", _descarga);

    _crudDescarga = new CRUDDescarga();
    KoBindings(_crudDescarga, "knockoutCRUDCadastroDescargaPessoa");

    new BuscarClientes(_pesquisaDescarga.Pessoa);
    new BuscarClientes(_pesquisaDescarga.PessoaOrigem);

    new BuscarClientes(_descarga.Pessoa);
    new BuscarClientes(_descarga.PessoaOrigem);

    buscarDescargas();
}

function adicionarClick(e, sender) {
    Salvar(_descarga, "DescargaPessoa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridDescarga.CarregarGrid();
                limparCamposDescarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_descarga, "DescargaPessoa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridDescarga.CarregarGrid();
                limparCamposDescarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a descarga?", function () {
        ExcluirPorCodigo(_descarga, "DescargaPessoa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridDescarga.CarregarGrid();
                    limparCamposDescarga();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposDescarga();
}

//*******MÉTODOS*******

function buscarDescargas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDescarga, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDescarga = new GridView(_pesquisaDescarga.Pesquisar.idGrid, "DescargaPessoa/Pesquisa", _pesquisaDescarga, menuOpcoes, null);
    _gridDescarga.CarregarGrid();
}

function editarDescarga(descargaGrid) {
    limparCamposDescarga();
    _descarga.Codigo.val(descargaGrid.Codigo);
    BuscarPorCodigo(_descarga, "DescargaPessoa/BuscarPorCodigo", function (arg) {
        _pesquisaDescarga.ExibirFiltros.visibleFade(false);
        _crudDescarga.Atualizar.visible(true);
        _crudDescarga.Cancelar.visible(true);
        _crudDescarga.Excluir.visible(true);
        _crudDescarga.Adicionar.visible(false);
        _descarga.Pessoa.enable(false);
    }, null);
}

function limparCamposDescarga() {
    _crudDescarga.Atualizar.visible(false);
    _crudDescarga.Cancelar.visible(false);
    _crudDescarga.Excluir.visible(false);
    _crudDescarga.Adicionar.visible(true);
    _descarga.Pessoa.enable(true);
    LimparCampos(_descarga);
}
