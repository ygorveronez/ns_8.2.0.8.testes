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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="TabelaFreteRotaTipoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridTabelaFreteRota;
var _tabelaFreteRota;
var _pesquisaTabelaFreteRota;
var _tabelaFrete;
var _gridDestinos;

var PesquisaTabelaFreteRota = function () {
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.CodigoEmbarcador = PropertyEntity({ text: "Código: ", maxlength: 50 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteRota.CarregarGrid();
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

var TabelaFreteRota = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destino:", required: true, idBtnSearch: guid() });
    this.CodigoEmbarcador = PropertyEntity({ text: "*Código do Frete no Embarcador: ", required: true, maxlength: 50 });
    this.DescricaoDestinos = PropertyEntity({ text: "Descrição do destino:", type: types.map });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.TiposCarga = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "*Tipo de Carga:", required: false, idBtnSearch: guid() });
    this.AdicionarTiposCarga = PropertyEntity({ eventClick: adicionarTipoCargaClick, type: types.event, text: "Adicionar Tipo de Carga", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTabelaFreteRota() {

    _tabelaFreteRota = new TabelaFreteRota();
    KoBindings(_tabelaFreteRota, "knockoutCadastroTabelaFreteRota");

    _pesquisaTabelaFreteRota = new PesquisaTabelaFreteRota();
    KoBindings(_pesquisaTabelaFreteRota, "knockoutPesquisaTabelaFreteRota", false, _pesquisaTabelaFreteRota.Pesquisar.id);

    _tabelaFrete = new PesquisaTabelaFreteRota();
    KoBindings(_tabelaFrete, "knockoutTabelaFrete");

    new BuscarTabelasDeFrete(_tabelaFrete.TabelaFrete, retornoTabelaFrete, EnumTipoTabelaFrete.tabelaRota);
    new BuscarLocalidades(_tabelaFreteRota.Origem, "Buscar Cidades de Origem", "Cidades de Origem");
    new BuscarLocalidades(_tabelaFreteRota.Destino, "Buscar Cidades de Destino", "Cidades de Destino");
    new BuscarLocalidades(_pesquisaTabelaFreteRota.Origem, "Buscar Cidades de Origem", "Cidades de Origem");
    new BuscarLocalidades(_pesquisaTabelaFreteRota.Destino, "Buscar Cidades de Destino", "Cidades de Destino");

    var data = { TipoTabelaFrete: EnumTipoTabelaFrete.tabelaRota };
    executarReST("TabelaFrete/BuscarTabelasPorTipo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data.length == 1) {
                retornoTabelaFrete(arg.Data[0]);
            }
        }
    });

    loadTabelaFreteRotaTipoCarga();
}

function retornoTabelaFrete(e) {
    _tabelaFrete.TabelaFrete.codEntity(e.Codigo);
    _tabelaFrete.TabelaFrete.val(e.Descricao);
    _pesquisaTabelaFreteRota.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _pesquisaTabelaFreteRota.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _pesquisaTabelaFreteRota.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _pesquisaTabelaFreteRota.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    _tabelaFreteRota.TabelaFrete.codEntity(_tabelaFrete.TabelaFrete.codEntity());
    _tabelaFreteRota.TabelaFrete.defCodEntity = _tabelaFrete.TabelaFrete.codEntity();
    _tabelaFreteRota.TabelaFrete.val(_tabelaFrete.TabelaFrete.val());
    _tabelaFreteRota.TabelaFrete.def = _tabelaFrete.TabelaFrete.val();
    buscarTabelaFreteRotas();
}

function adicionarClick(e, sender) {
    Salvar(e, "TabelaFreteRota/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "cadastrado");
                _gridTabelaFreteRota.CarregarGrid();
                limparCamposTabelaFreteRota();
            } else {
                exibirMensagem("aviso", "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(e, "TabelaFreteRota/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTabelaFreteRota.CarregarGrid();
                limparCamposTabelaFreteRota();
            } else {
                exibirMensagem("aviso", "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o frete de " + _tabelaFreteRota.Origem.val() + " até " + _tabelaFreteRota.Destino.val() + "?", function () {
        ExcluirPorCodigo(_tabelaFreteRota, "TabelaFreteRota/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTabelaFreteRota.CarregarGrid();
                    limparCamposTabelaFreteRota();
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
    resetarTabs();
    limparCamposTabelaFreteRota();
}


function buscarTabelaFreteRotas() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTabelaFreteRota, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaFreteRota = new GridView(_pesquisaTabelaFreteRota.Pesquisar.idGrid, "TabelaFreteRota/Pesquisa", _pesquisaTabelaFreteRota, menuOpcoes, null, null, retornoGridRotas);
    _gridTabelaFreteRota.CarregarGrid();
}

function retornoGridRotas() {

    $("#divContentTabelaRota").show();
    $("#wid-id-6").hide();
    $("#spanNomeTabela").text("Tabela de Frete (" + _tabelaFrete.TabelaFrete.val() + ")");
}

function editarTabelaFreteRota(tabelaFreteRotaGrid) {
    limparCamposTabelaFreteRota();
    _tabelaFreteRota.Codigo.val(tabelaFreteRotaGrid.Codigo);
    BuscarPorCodigo(_tabelaFreteRota, "TabelaFreteRota/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaFreteRota.ExibirFiltros.visibleFade(false);
        _tabelaFreteRota.Atualizar.visible(true);
        _tabelaFreteRota.Cancelar.visible(true);
        _tabelaFreteRota.Excluir.visible(true);
        _tabelaFreteRota.Adicionar.visible(false);
        recarregarTiposCarga(_tabelaFreteRota.TiposCarga.list);
    }, null);
}

function limparCamposTabelaFreteRota() {
    _tabelaFreteRota.Atualizar.visible(false);
    _tabelaFreteRota.Cancelar.visible(false);
    _tabelaFreteRota.Excluir.visible(false);
    _tabelaFreteRota.Adicionar.visible(true);
    _tabelaFreteRota.TiposCarga.list = new Array();
    LimparCampos(_tabelaFreteRota);
    LimparCamposTipoCarga();
}

function resetarTabs() {
    let firstTabEl = document.querySelector('#liTabRota a');

    (new bootstrap.Tab(firstTabEl)).show();
}

