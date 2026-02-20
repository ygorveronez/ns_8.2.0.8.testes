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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _psquisaDestinoPrioritarioCalculoFrete;
var _destinoPrioritarioCalculoFrete;
var _gridDstinoPrioritarioCalculoFrete;

var PesquisaDestinoPrioritarioCalculoFrete = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDstinoPrioritarioCalculoFrete.CarregarGrid();
        }, type: types.event, text: "Pesquisar", id: guid(), idGrid: guid(), visible: ko.observable(true)
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

var DestinoPrioritarioCalculoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.AdicionarTabela = PropertyEntity({ type: types.event, text: "Adicionar", idBtnSearch: guid() });
    this.TabelasFrete = PropertyEntity({ type: types.val, idGrid: guid(), val: GetSetTabelasFrete });

    this.AdicionarLocalidade = PropertyEntity({ type: types.event, text: "Adicionar", idBtnSearch: guid() });
    this.Localidades = PropertyEntity({ type: types.map, list: [], idGrid: guid(), val: GetLocalidades});

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadDestinoPrioritarioCalculoFrete() {
    _destinoPrioritarioCalculoFrete = new DestinoPrioritarioCalculoFrete();
    KoBindings(_destinoPrioritarioCalculoFrete, "knockoutDestinoPrioritarioCalculoFrete");

    _pesquisaDestinoPrioritarioCalculoFrete = new PesquisaDestinoPrioritarioCalculoFrete();
    KoBindings(_pesquisaDestinoPrioritarioCalculoFrete, "knockoutPesquisaDestinoPrioritarioCalculoFrete", false, _pesquisaDestinoPrioritarioCalculoFrete.Pesquisar.id);

    HeaderAuditoria("DestinoPrioritarioCalculoFrete");

    CarregarGrid();

    new BuscarLocalidades(_destinoPrioritarioCalculoFrete.AdicionarLocalidade, null, null, RetornoLocalidades, _auxGridLocalidades);
    var _buscaTabelasFrete = new BuscarTabelasDeFrete(_destinoPrioritarioCalculoFrete.AdicionarTabela, null, null, _gridTabelasFrete);
    _buscaTabelasFrete.Opcoes.CalcularFreteDestinoPrioritario.val(true);

    BuscarDestinoPrioritarioCalculoFrete();
}

function adicionarClick(e, sender) {
    Salvar(e, "DestinoPrioritarioCalculoFrete/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridDstinoPrioritarioCalculoFrete.CarregarGrid();
                LimparCamposDestinoPrioritarioCalculoFrete();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_destinoPrioritarioCalculoFrete, "DestinoPrioritarioCalculoFrete/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridDstinoPrioritarioCalculoFrete.CarregarGrid();
                LimparCamposDestinoPrioritarioCalculoFrete();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}
function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração?", function () {
        ExcluirPorCodigo(_destinoPrioritarioCalculoFrete, "DestinoPrioritarioCalculoFrete/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridDstinoPrioritarioCalculoFrete.CarregarGrid();
                    LimparCamposDestinoPrioritarioCalculoFrete();
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
    LimparCamposDestinoPrioritarioCalculoFrete();
}

function BuscarDestinoPrioritarioCalculoFrete() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoPrioridade, tamanho: 15, icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridDstinoPrioritarioCalculoFrete= new GridView(_pesquisaDestinoPrioritarioCalculoFrete.Pesquisar.idGrid, "DestinoPrioritarioCalculoFrete/Pesquisa", _pesquisaDestinoPrioritarioCalculoFrete, menuOpcoes, null);
    _gridDstinoPrioritarioCalculoFrete.CarregarGrid();
}

function editarConfiguracaoPrioridade(data) {
    LimparCamposDestinoPrioritarioCalculoFrete();
    BuscarPorCodigo(data.Codigo);
}

//*******MÉTODOS*******
function CarregarGrid() {
    CarregarGridTabelaFrete();
    CarregarGridLocalidades();
}

function BuscarPorCodigo(codigo) {
    executarReST("DestinoPrioritarioCalculoFrete/BuscarPorCodigo", {Codigo: codigo}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherObjetoKnout(_destinoPrioritarioCalculoFrete, { Data: arg.Data.Dados });

                _pesquisaDestinoPrioritarioCalculoFrete.ExibirFiltros.visibleFade(false);
                _destinoPrioritarioCalculoFrete.Atualizar.visible(true);
                _destinoPrioritarioCalculoFrete.Cancelar.visible(true);
                _destinoPrioritarioCalculoFrete.Excluir.visible(true);
                _destinoPrioritarioCalculoFrete.Adicionar.visible(false);

                if (arg.Data.TabelasFrete) {
                    _gridTabelasFrete.CarregarGrid(arg.Data.TabelasFrete);
                    _destinoPrioritarioCalculoFrete.TabelasFrete.list = arg.Data.TabelasFrete;
                }
                    

                if (arg.Data.Localidades) {
                    var listaLocalidades = arg.Data.Localidades.map(function (loc) {
                        _codigosLocalidade.push(loc.Localidade.Codigo + "");

                        var _localidade = new LocalidadeMap();
                        _localidade.Codigo.val(loc.Codigo);
                        _localidade.Localidade.val(loc.Localidade.Descricao);
                        _localidade.Localidade.codEntity(loc.Localidade.Codigo);
                        _localidade.Ordem.val(loc.Ordem);
                        _localidade.Ativo.val(loc.Ativo);

                        return _localidade;
                    });
                    _destinoPrioritarioCalculoFrete.Localidades.list = listaLocalidades;
                    RecarregarDadosGridLocalidade();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LimparCamposDestinoPrioritarioCalculoFrete() {
    _gridTabelasFrete.CarregarGrid([]);
    SetLocalidadesList([]);
    _codigosLocalidade = [];
    _auxGridLocalidades.CarregarGrid([]);
    RecarregarDadosGridLocalidade();

    _destinoPrioritarioCalculoFrete.Codigo.val(_destinoPrioritarioCalculoFrete.Codigo.def);
    _destinoPrioritarioCalculoFrete.Ativo.val(_destinoPrioritarioCalculoFrete.Ativo.def);
    _destinoPrioritarioCalculoFrete.Descricao.val(_destinoPrioritarioCalculoFrete.Descricao.def);

    _destinoPrioritarioCalculoFrete.Atualizar.visible(false);
    _destinoPrioritarioCalculoFrete.Cancelar.visible(false);
    _destinoPrioritarioCalculoFrete.Excluir.visible(false);
    _destinoPrioritarioCalculoFrete.Adicionar.visible(true);

    $("#tabDados").click();
}