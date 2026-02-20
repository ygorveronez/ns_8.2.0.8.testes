/// <reference path="PrevisaoConsulta.js" />
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
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _reservaCargaGrupoPessoa;
var _pesquisaReservaCargaGrupoPessoa;
var _gridReservaCargaGrupoPessoa;
var _gridPrevisoes;

var ReservaCargaGrupoPessoa = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Pessoa:",issue: 58, idBtnSearch: guid() });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", issue: 320, idBtnSearch: guid() });
    this.DataReserva = PropertyEntity({ type: types.map, getType: typesKnockout.date, val: ko.observable(""), text: "*Data da Reserva:" });
    this.DataReserva.val.subscribe(verificaPeriodos);

    this.QuantidadeReservada = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), text: "*Quantidade Reservada", count: ko.observable(0), visible: ko.observable(false) });
    this.PrevisaoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Previsão de Carregamento:", issue: 339, idBtnSearch: guid(), visible: ko.observable(false) });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaReservaCargaGrupoPessoa = function () {
    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", issue: 320, idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridReservaCargaGrupoPessoa.CarregarGrid();
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
function loadReservaCargaGrupoPessoa() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaReservaCargaGrupoPessoa = new PesquisaReservaCargaGrupoPessoa();
    KoBindings(_pesquisaReservaCargaGrupoPessoa, "knockoutPesquisaReservaCargaGrupoPessoa", false, _pesquisaReservaCargaGrupoPessoa.Pesquisar.id);

    // Instancia objeto principal
    _reservaCargaGrupoPessoa = new ReservaCargaGrupoPessoa();
    KoBindings(_reservaCargaGrupoPessoa, "knockoutReservaCargaGrupoPessoa");

    HeaderAuditoria("ReservaCargaGrupoPessoa", _reservaCargaGrupoPessoa);

    // Instancia buscas
    new BuscarCentrosCarregamento(_pesquisaReservaCargaGrupoPessoa.CentroCarregamento);

    new BuscarGruposPessoas(_reservaCargaGrupoPessoa.GrupoPessoa);
    new BuscarCentrosCarregamento(_reservaCargaGrupoPessoa.CentroCarregamento, function (cc) {
        _reservaCargaGrupoPessoa.CentroCarregamento.codEntity(cc.Codigo);
        _reservaCargaGrupoPessoa.CentroCarregamento.val(cc.Descricao);

        verificaPeriodos();
    });
    new BuscarPrevisaoCarregamento(_reservaCargaGrupoPessoa.PrevisaoCarregamento, _reservaCargaGrupoPessoa.CentroCarregamento, _reservaCargaGrupoPessoa.DataReserva, function (previsao) {
        _reservaCargaGrupoPessoa.QuantidadeReservada.count(previsao.QuantidadeCargas);
        _reservaCargaGrupoPessoa.QuantidadeReservada.visible(true);
    });

    // Carrega módulos

    // Inicia busca
    buscarReservaCargaGrupoPessoa();
}

function adicionarClick(e, sender) {
    if (!ValidarReserva())
        return;

    Salvar(_reservaCargaGrupoPessoa, "ReservaCargaGrupoPessoa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridReservaCargaGrupoPessoa.CarregarGrid();
                limparCamposReservaCargaGrupoPessoa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarReserva())
        return;

    Salvar(_reservaCargaGrupoPessoa, "ReservaCargaGrupoPessoa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridReservaCargaGrupoPessoa.CarregarGrid();
                limparCamposReservaCargaGrupoPessoa();
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
        ExcluirPorCodigo(_reservaCargaGrupoPessoa, "ReservaCargaGrupoPessoa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridReservaCargaGrupoPessoa.CarregarGrid();
                    limparCamposReservaCargaGrupoPessoa();
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
    limparCamposReservaCargaGrupoPessoa();
}

function editarReservaCargaGrupoPessoaClick(itemGrid) {
    // Limpa os campos
    limparCamposReservaCargaGrupoPessoa();

    // Seta o codigo do objeto
    _reservaCargaGrupoPessoa.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_reservaCargaGrupoPessoa, "ReservaCargaGrupoPessoa/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesquisa
                _pesquisaReservaCargaGrupoPessoa.ExibirFiltros.visibleFade(false);

                _reservaCargaGrupoPessoa.QuantidadeReservada.count(arg.Data.QuantidadeReservadaCount);
                _reservaCargaGrupoPessoa.QuantidadeReservada.visible(true);

                // Alternas os campos de CRUD
                _reservaCargaGrupoPessoa.Atualizar.visible(true);
                _reservaCargaGrupoPessoa.Excluir.visible(true);
                _reservaCargaGrupoPessoa.Cancelar.visible(true);
                _reservaCargaGrupoPessoa.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarReservaCargaGrupoPessoa() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarReservaCargaGrupoPessoaClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridReservaCargaGrupoPessoa = new GridView(_pesquisaReservaCargaGrupoPessoa.Pesquisar.idGrid, "ReservaCargaGrupoPessoa/Pesquisa", _pesquisaReservaCargaGrupoPessoa, menuOpcoes, { column: 1, dir: 'asc' });
    _gridReservaCargaGrupoPessoa.CarregarGrid();
}

function ValidarReserva() {
    var qtdReservada = parseInt(_reservaCargaGrupoPessoa.QuantidadeReservada.val());
    var maxReservada = parseInt(_reservaCargaGrupoPessoa.QuantidadeReservada.count());
    if (qtdReservada > maxReservada) {
        exibirMensagem(tipoMensagem.aviso, "Limite ultrapassado", "Quantidade Reservada não pode ultrapassar " + _reservaCargaGrupoPessoa.QuantidadeReservada.count() + ".");
        return false;
    }

    return true;
}

function limparCamposReservaCargaGrupoPessoa() {
    _reservaCargaGrupoPessoa.Atualizar.visible(false);
    _reservaCargaGrupoPessoa.Cancelar.visible(false);
    _reservaCargaGrupoPessoa.Excluir.visible(false);
    _reservaCargaGrupoPessoa.Adicionar.visible(true);

    _reservaCargaGrupoPessoa.QuantidadeReservada.visible(false);

    LimparCampos(_reservaCargaGrupoPessoa);
}

function verificaPeriodos() {
    var reserva = RetornarObjetoPesquisa(_reservaCargaGrupoPessoa);
    var visivel = (reserva.CentroCarregamento > 0 && reserva.DataReserva != "");

    _reservaCargaGrupoPessoa.PrevisaoCarregamento.visible(visivel);
}