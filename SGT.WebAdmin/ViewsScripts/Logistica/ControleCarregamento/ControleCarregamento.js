/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoControleCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridControleCarregamento;
var _pesquisaControleCarregamento;
var _pesquisaControleCarregamentoAuxiliar;

/*
 * Declaração das Classes
 */

var PesquisaControleCarregamento = function () {
    var dataPadrao = Global.DataAtual();

    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*CD:", idBtnSearch: guid(), required: true });
    this.DataLimite = PropertyEntity({ text: "Data Limite:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.DataInicio = PropertyEntity({ text: "Data Inicial:", val: ko.observable(dataPadrao), def: dataPadrao, getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoControleCarregamento.obterListaOpcoesPendentes()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoControleCarregamento.obterOpcoes(), text: "Situação:" });
    this.DataInicio.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicio;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaControleCarregamento)) {
                _pesquisaControleCarregamento.ExibirFiltros.visibleFade(false);

                $("#controle-carregamento-container").removeClass("d-none");

                atualizarFiltrosUltimaPesquisa();
                _gridControleCarregamento.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção!", "Verifique os campos obrigatórios!");
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), visibleFade: ko.observable(false)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaControleCarregamentoAuxiliar = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataLimite = PropertyEntity({ getType: typesKnockout.date });
    this.DataInicio = PropertyEntity({ getType: typesKnockout.date });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoControleCarregamento.obterOpcoes() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadControleCarregamento() {
    _pesquisaControleCarregamento = new PesquisaControleCarregamento();
    KoBindings(_pesquisaControleCarregamento, "knockoutPesquisaControleCarregamento", _pesquisaControleCarregamento.Pesquisar.id);

    _pesquisaControleCarregamentoAuxiliar = new PesquisaControleCarregamentoAuxiliar();

    new BuscarCentrosCarregamento(_pesquisaControleCarregamento.CentroCarregamento);
    
    loadGridControleCarregamento();
}

function loadGridControleCarregamento() {
    var opcaoChegadaDoca = { descricao: "Chegada na Doca", id: guid(), evento: "onclick", metodo: chegadaDocaClick, icone: "", visibilidade: isPermiteInformarChegadaDoca };
    var opcaoFinalizarCarregamento = { descricao: "Finalizar Carregamento", id: guid(), evento: "onclick", metodo: finalizarCarregamentoClick, icone: "", visibilidade: isPermiteFinalizarCarregamento };
    var opcaoIniciarCarregamento = { descricao: "Iniciar Carregamento", id: guid(), evento: "onclick", metodo: iniciarCarregamentoClick, icone: "", visibilidade: isPermiteIniciarCarregamento };
    var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 10, opcoes: [opcaoChegadaDoca, opcaoFinalizarCarregamento, opcaoIniciarCarregamento] };

    _gridControleCarregamento = new GridView("grid-controle-carregamento", "ControleCarregamento/Pesquisa", _pesquisaControleCarregamentoAuxiliar, menuOpcoes);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaControleCarregamentoAuxiliar.CentroCarregamento.codEntity(_pesquisaControleCarregamento.CentroCarregamento.codEntity());
    _pesquisaControleCarregamentoAuxiliar.CentroCarregamento.val(_pesquisaControleCarregamento.CentroCarregamento.val());
    _pesquisaControleCarregamentoAuxiliar.DataLimite.val(_pesquisaControleCarregamento.DataLimite.val());
    _pesquisaControleCarregamentoAuxiliar.DataInicio.val(_pesquisaControleCarregamento.DataInicio.val());
    _pesquisaControleCarregamentoAuxiliar.Situacao.val(_pesquisaControleCarregamento.Situacao.val());
}

function chegadaDocaClick(registroSelecionado) {
    executarReST("ControleCarregamento/ChegadaDoca", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Chegada na doca informada com sucesso");
                _gridControleCarregamento.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function finalizarCarregamentoClick(registroSelecionado) {
    executarReST("ControleCarregamento/FinalizarCarregamento", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Carregamento finalizado com sucesso");
                _gridControleCarregamento.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function iniciarCarregamentoClick(registroSelecionado) {
    executarReST("ControleCarregamento/IniciarCarregamento", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Carregamento iniciado com sucesso");
                _gridControleCarregamento.CarregarGrid()
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

/*
 * Declaração das Funções
 */

function isPermiteInformarChegadaDoca(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoControleCarregamento.Aguardando;
}

function isPermiteFinalizarCarregamento(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoControleCarregamento.EmCarregamento;
}

function isPermiteIniciarCarregamento(registroSelecionado) {
    return registroSelecionado.Situacao == EnumSituacaoControleCarregamento.EmDoca;
}