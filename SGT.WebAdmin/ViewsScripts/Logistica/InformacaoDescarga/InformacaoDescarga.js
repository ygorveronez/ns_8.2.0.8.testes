/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _informacaoDescarga;
var _pesquisaInformacaoDescarga;
var _gridInformacaoDescarga;

var InformacaoDescarga = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Data = PropertyEntity({ text: "*Data: ", required: true, getType: typesKnockout.date, type: types.date, enable: ko.observable(true) });
    this.Hora = PropertyEntity({ text: "*Hora: ", required: true, getType: typesKnockout.time, type: types.time, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "*Empresa: ", required: true, getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.NotaFiscal = PropertyEntity({ text: "*Nota Fiscal: ", required: true, getType: typesKnockout.int, type: types.int, configInt: { precision: 0, allowZero: true }, enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: "*Série: ", required: true, getType: typesKnockout.int, type: types.int, configInt: { precision: 0, allowZero: true }, enable: ko.observable(true) });
    this.Placa = PropertyEntity({ text: "*Placa: ", required: true, getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.PesoDescarga = PropertyEntity({ text: "*Peso Descarga:", maxlength: 25, required: true, configDecimal: { precision: 4, allowZero: false, allowNegative: false }, getType: typesKnockout.decimal, val: ko.observable("") });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    // CRUD
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

        UrlImportacao: "InformacaoDescarga/Importar",
        UrlConfiguracao: "InformacaoDescarga/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O035_InformacaoDescaga,
        CallbackImportacao: function () {
            _gridInformacaoDescarga.CarregarGrid();
        }
    });
}

var PesquisaInformacaoDescarga = function () {
    this.Data = PropertyEntity({ text: "Data:", getType: typesKnockout.date, val: ko.observable("") });
    this.NotaFiscal = PropertyEntity({ text: "Nota Fiscal:", getType: typesKnockout.int });
    this.Placa = PropertyEntity({ text: "Placa:", getType: typesKnockout.string, val: ko.observable("") });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridInformacaoDescarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadInformacaoDescarga() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaInformacaoDescarga = new PesquisaInformacaoDescarga();
    KoBindings(_pesquisaInformacaoDescarga, "knockoutPesquisaInformacaoDescarga", false, _pesquisaInformacaoDescarga.Pesquisar.id);

    // Instancia objeto principal
    _informacaoDescarga = new InformacaoDescarga();
    KoBindings(_informacaoDescarga, "knockoutInformacaoDescarga");

    HeaderAuditoria("InformacaoDescarga", _informacaoDescarga);

    new BuscarCargas(_pesquisaInformacaoDescarga.Carga);
    new BuscarCargas(_informacaoDescarga.Carga);

    $("#" + _informacaoDescarga.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _pesquisaInformacaoDescarga.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    // Inicia busca
    buscarInformacaoDescarga();
}

function adicionarClick(e, sender) {
    Salvar(_informacaoDescarga, "InformacaoDescarga/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridInformacaoDescarga.CarregarGrid();
                limparCamposInformacaoDescarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_informacaoDescarga, "InformacaoDescarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridInformacaoDescarga.CarregarGrid();
                limparCamposInformacaoDescarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_informacaoDescarga, "InformacaoDescarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridInformacaoDescarga.CarregarGrid();
                    limparCamposInformacaoDescarga();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposInformacaoDescarga();
}

function editarInformacaoDescargaClick(itemGrid) {
    // Limpa os campos
    limparCamposInformacaoDescarga();

    // Seta o codigo do objeto
    _informacaoDescarga.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_informacaoDescarga, "InformacaoDescarga/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaInformacaoDescarga.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _informacaoDescarga.Atualizar.visible(true);
                _informacaoDescarga.Excluir.visible(true);
                _informacaoDescarga.Cancelar.visible(true);
                _informacaoDescarga.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarInformacaoDescarga() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarInformacaoDescargaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridInformacaoDescarga = new GridView(_pesquisaInformacaoDescarga.Pesquisar.idGrid, "InformacaoDescarga/Pesquisa", _pesquisaInformacaoDescarga, menuOpcoes, null);
    _gridInformacaoDescarga.CarregarGrid();
}

function limparCamposInformacaoDescarga() {
    _informacaoDescarga.Atualizar.visible(false);
    _informacaoDescarga.Cancelar.visible(false);
    _informacaoDescarga.Excluir.visible(false);
    _informacaoDescarga.Adicionar.visible(true);
    LimparCampos(_informacaoDescarga);
}