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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDiarioBordoSemanal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDiarioBordoSemanal;
var _diarioBordoSemanal;
var _pesquisaDiarioBordoSemanal;

var PesquisaDiarioBordoSemanal = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.SituacaoDiarioBordoSemanal = PropertyEntity({ val: ko.observable(EnumSituacaoDiarioBordoSemanal.Todas), options: EnumSituacaoDiarioBordoSemanal.ObterOpcoesPesquisa(), def: EnumSituacaoDiarioBordoSemanal.Todas, text: "Situação do Diário: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDiarioBordoSemanal.CarregarGrid();
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

var DiarioBordoSemanal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "*Número: ", required: ko.observable(false), maxlength: 500, getType: typesKnockout.int, enable: ko.observable(false), visible: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });

    this.DataInicio = PropertyEntity({ text: "*Data Início: ", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "*Data Final: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(false) });
    this.SituacaoDiarioBordoSemanal = PropertyEntity({ val: ko.observable(EnumSituacaoDiarioBordoSemanal.Aberto), options: EnumSituacaoDiarioBordoSemanal.ObterOpcoes(), def: EnumSituacaoDiarioBordoSemanal.Aberto, text: "*Situação do Diário: ", required: true });

    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, enable: ko.observable(true), maxlength: 2000 });
}

var CRUDDiarioBordoSemanal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadDiarioBordoSemanal() {
    _diarioBordoSemanal = new DiarioBordoSemanal();
    KoBindings(_diarioBordoSemanal, "knockoutCadastroDiarioBordoSemanal");

    HeaderAuditoria("DiarioBordoSemanal", _diarioBordoSemanal);

    _crudDiarioBordoSemanal = new CRUDDiarioBordoSemanal();
    KoBindings(_crudDiarioBordoSemanal, "knockoutCRUDDiarioBordoSemanal");

    _pesquisaDiarioBordoSemanal = new PesquisaDiarioBordoSemanal();
    KoBindings(_pesquisaDiarioBordoSemanal, "knockoutPesquisaDiarioBordoSemanal", false, _pesquisaDiarioBordoSemanal.Pesquisar.id);

    new BuscarCargas(_pesquisaDiarioBordoSemanal.Carga);
    new BuscarVeiculos(_pesquisaDiarioBordoSemanal.Veiculo);
    new BuscarMotoristas(_pesquisaDiarioBordoSemanal.Motorista);

    new BuscarCargas(_diarioBordoSemanal.Carga, RetornoCarga);
    new BuscarVeiculos(_diarioBordoSemanal.Veiculo);
    new BuscarMotoristas(_diarioBordoSemanal.Motorista);

    buscarDiarioBordoSemanal();
}

function RetornoCarga(data) {
    _diarioBordoSemanal.Carga.codEntity(data.Codigo);
    _diarioBordoSemanal.Carga.val(data.CodigoCargaEmbarcador);
    if (data.CodigoMotorista > 0)
    {
        _diarioBordoSemanal.Motorista.codEntity(data.CodigoMotorista);
        _diarioBordoSemanal.Motorista.val(data.Motorista);
    }
    if (data.CodigoVeiculo > 0) {
        _diarioBordoSemanal.Veiculo.codEntity(data.CodigoVeiculo);
        _diarioBordoSemanal.Veiculo.val(data.Veiculo);
    }
}

function adicionarClick(e, sender) {
    Salvar(_diarioBordoSemanal, "DiarioBordoSemanal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridDiarioBordoSemanal.CarregarGrid();                
                limparCamposDiarioBordoSemanal();
                BuscarDiarioBordo(arg.Data.Codigo);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_diarioBordoSemanal, "DiarioBordoSemanal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridDiarioBordoSemanal.CarregarGrid();
                limparCamposDiarioBordoSemanal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a o diário de bordo selecionado?", function () {
        ExcluirPorCodigo(_diarioBordoSemanal, "DiarioBordoSemanal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridDiarioBordoSemanal.CarregarGrid();
                limparCamposDiarioBordoSemanal();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposDiarioBordoSemanal();
}

function ImprimirClick(e) {
    executarDownload("DiarioBordoSemanal/Imprimir", { Codigo: _diarioBordoSemanal.Codigo.val() });
}

//*******MÉTODOS*******


function buscarDiarioBordoSemanal() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDiarioBordoSemanal, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "DiarioBordoSemanal/ExportarPesquisa",
        titulo: "Diário de Bordo Semanal"
    };

    _gridDiarioBordoSemanal = new GridViewExportacao(_pesquisaDiarioBordoSemanal.Pesquisar.idGrid, "DiarioBordoSemanal/Pesquisa", _pesquisaDiarioBordoSemanal, menuOpcoes, configExportacao);
    _gridDiarioBordoSemanal.CarregarGrid();
}

function editarDiarioBordoSemanal(diarioBordoSemanalGrid) {
    limparCamposDiarioBordoSemanal();
    BuscarDiarioBordo(diarioBordoSemanalGrid.Codigo);
}

function BuscarDiarioBordo(codigo) {
    _diarioBordoSemanal.Codigo.val(codigo);
    BuscarPorCodigo(_diarioBordoSemanal, "DiarioBordoSemanal/BuscarPorCodigo", function (arg) {
        _pesquisaDiarioBordoSemanal.ExibirFiltros.visibleFade(false);
        _crudDiarioBordoSemanal.Atualizar.visible(true);
        _crudDiarioBordoSemanal.Cancelar.visible(true);
        _crudDiarioBordoSemanal.Imprimir.visible(true);
        _crudDiarioBordoSemanal.Excluir.visible(true);
        _crudDiarioBordoSemanal.Adicionar.visible(false);

        _diarioBordoSemanal.Numero.visible(true);
        _diarioBordoSemanal.DataFim.visible(true);
    }, null);
}

function limparCamposDiarioBordoSemanal() {
    _crudDiarioBordoSemanal.Atualizar.visible(false);
    _crudDiarioBordoSemanal.Cancelar.visible(false);
    _crudDiarioBordoSemanal.Imprimir.visible(false);
    _crudDiarioBordoSemanal.Excluir.visible(false);
    _crudDiarioBordoSemanal.Adicionar.visible(true);
    LimparCampos(_diarioBordoSemanal);
}