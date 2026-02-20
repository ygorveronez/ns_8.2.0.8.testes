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


//*******MAPEAMENTO KNOUCKOUT*******


var _gridRota;
var _rota;
var _pesquisaRota;

var PesquisaRota = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", required: false, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", required: false, idBtnSearch: guid() });

    this.SomenteNaoIntegradaSemParar = PropertyEntity({ getType: typesKnockout.bool, text: "Retornar somente rotas que não estão integradas com a Sem Parar?", val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRota.CarregarGrid();
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

var Rota = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Origem:", issue: 16, required: true, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Destino:", issue: 16, required: true, idBtnSearch: guid() });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", issue: 52, required: false, idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", issue: 52, required: false, idBtnSearch: guid() });

    this.PossuiPedagio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, eventClick: verificarPossuiPedagio, text: "Rota possui pedágios?", def: false });
    this.NumeroPedagios = PropertyEntity({ getType: typesKnockout.int, text: "*Número de Pedágios:", maxlength: 2, visible: ko.observable(false) });
    this.DistanciaKM = PropertyEntity({ getType: typesKnockout.int, text: "*Distância da Rota (KM):", issue: 839, maxlength: 6, required: true });
    this.TempoViagem = PropertyEntity({ text: "Tempo em Viagem: ", issue: 840, getType: typesKnockout.string, required: false });
    this.Ativo = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true });
    this.DescricaoRotaSemParar = PropertyEntity({ val: ko.observable(""), text: "Rota (Código para Integração):", issue: 1059, maxlength: 64, visible: ko.observable(true) });
    this.CodigosPracaSemParar = PropertyEntity({ val: ko.observable(""), text: "Códigos praças de pedágio Sem Parar", issue: 977, maxlength: 1000, visible: ko.observable(false) });

    this.CEPs = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.CodigoCEP = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CEPInicial = PropertyEntity({ text: "*CEP Inicial: ", required: false, maxlength: 15, val: ko.observable("") });
    this.CEPFinal = PropertyEntity({ text: "*CEP Final: ", required: false, maxlength: 15, val: ko.observable("") });

    this.AdicionarCEP = PropertyEntity({ eventClick: AdicionarCEPClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true) });

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

        UrlImportacao: "Rota/Importar",
        UrlConfiguracao: "Rota/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O008_Rota,
        CallbackImportacao: function () {
            _gridRota.CarregarGrid();
        }
    });
}

//*******EVENTOS*******

function loadRota() {

    _rota = new Rota();
    KoBindings(_rota, "knockoutCadastroRota");

    HeaderAuditoria("Rota", _rota);

    _pesquisaRota = new PesquisaRota();
    KoBindings(_pesquisaRota, "knockoutPesquisaRota", false, _pesquisaRota.Pesquisar.id);

    $("#" + _rota.PossuiPedagio.id).click(verificarPossuiPedagio);

    $("#" + _rota.TempoViagem.id).mask("00:00", { selectOnFocus: true, clearIfNotMatch: true });

    new BuscarLocalidades(_rota.Origem, "Buscar Cidades de Origem", "Cidades de Origem", retornoOrigem);
    new BuscarLocalidades(_rota.Destino, "Buscar Cidades de Destino", "Cidades de Destino", retornoDestino);
    new BuscarLocalidades(_pesquisaRota.Origem, "Buscar Cidades de Origem", "Cidades de Origem");
    new BuscarLocalidades(_pesquisaRota.Destino, "Buscar Cidades de Destino", "Cidades de Destino");

    new BuscarClientes(_pesquisaRota.Remetente);
    new BuscarClientes(_pesquisaRota.Destinatario);
    new BuscarClientes(_rota.Remetente, retornoRemetente);
    new BuscarClientes(_rota.Destinatario, retornoDestinatario);

    loadRotaCEP();
    buscarRotas();
}

function retornoOrigem(data) {
    _rota.Origem.codEntity(data.Codigo);
    _rota.Origem.val(data.Descricao);
    _rota.Remetente.codEntity(0);
    _rota.Remetente.val("");
}

function retornoDestino(data) {
    _rota.Destino.codEntity(data.Codigo);
    _rota.Destino.val(data.Descricao);
    _rota.Destinatario.codEntity(0);
    _rota.Destinatario.val("");
}

function retornoRemetente(data) {
    _rota.Remetente.codEntity(data.Codigo);
    _rota.Remetente.val(data.Descricao);
    _rota.Origem.codEntity(data.CodigoLocalidade);
    _rota.Origem.val(data.Localidade);
}

function retornoDestinatario(data) {
    _rota.Destinatario.codEntity(data.Codigo);
    _rota.Destinatario.val(data.Descricao);
    _rota.Destino.codEntity(data.CodigoLocalidade);
    _rota.Destino.val(data.Localidade);
}

function adicionarClick(e, sender) {
    Global.ResetarAbas();
    Salvar(e, "Rota/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Salvo com sucesso.");
                _gridRota.CarregarGrid();
                limparCamposRota();
            } else {
                exibirMensagem("aviso", "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Global.ResetarAbas();
    Salvar(e, "Rota/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso!", "Atualizado com sucesso.");
            _gridRota.CarregarGrid();
            limparCamposRota();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    Global.ResetarAbas();
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a rota de " + _rota.Origem.val() + " a " + _rota.Destino.val() + "?", function () {
        ExcluirPorCodigo(_rota, "Rota/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridRota.CarregarGrid();
                    limparCamposRota();
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
    limparCamposRota();
}

//*******MÉTODOS*******

function verificarPossuiPedagio(e) {
    if (_rota.PossuiPedagio.val() == true) {
        if (_PossuiSemParar) {
            _rota.CodigosPracaSemParar.visible(true);
        }
        else {
            _pesquisaRota.SomenteNaoIntegradaSemParar.visible(false);
            _rota.CodigosPracaSemParar.visible(false);
        }

    } else {
        _rota.NumeroPedagios.visible(false);
        _rota.NumeroPedagios.required = false;
        _rota.CodigosPracaSemParar.visible(false);
        _pesquisaRota.SomenteNaoIntegradaSemParar.visible(false);
    }
}

function buscarRotas() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarRota, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridRota = new GridView(_pesquisaRota.Pesquisar.idGrid, "Rota/Pesquisa", _pesquisaRota, menuOpcoes, null);
    _gridRota.CarregarGrid();
}

function editarRota(rotaGrid) {
    limparCamposRota();
    _rota.Codigo.val(rotaGrid.Codigo);
    BuscarPorCodigo(_rota, "Rota/BuscarPorCodigo", function (arg) {
        Global.ResetarAbas();
        recarregarGridRotaCEP();
        _pesquisaRota.ExibirFiltros.visibleFade(false);
        _rota.Atualizar.visible(true);
        _rota.Cancelar.visible(true);
        _rota.Excluir.visible(true);
        _rota.Adicionar.visible(false);
        verificarPossuiPedagio(_rota);
    }, null);
}

function limparCamposRota() {
    _rota.Atualizar.visible(false);
    _rota.Cancelar.visible(false);
    _rota.Excluir.visible(false);
    _rota.Adicionar.visible(true);
    _rota.NumeroPedagios.visible(false);
    _rota.NumeroPedagios.required = false;

    Global.ResetarAbas();

    limparCEP();
    LimparCampos(_rota);
    recarregarGridRotaCEP();

    _rota.AdicionarCEP.text("Adicionar");
}
