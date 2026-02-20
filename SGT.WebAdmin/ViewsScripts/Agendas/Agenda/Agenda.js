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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumStatusAgendaTarefa.js" />
/// <reference path="CalendarioAgenda.js" />
/// <reference path="../AgendaTarefa/AgendaTarefa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _agenda;
var _pesquisaAgenda;
var _dadosPesquisaAgenda;
var _calendarioAgenda;

var PesquisaAgenda = function () {
    this.Observacao = PropertyEntity({ text: "Observação: " });
    this.DataAgenda = PropertyEntity({ text: "*Data da Agenda: ", getType: typesKnockout.date, required: true, val: ko.observable(Global.DataAtual()), def: ko.observable(Global.DataAtual()) });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAgendaTarefa.Todos), options: EnumStatusAgendaTarefa.obterOpcoesPesquisa(), def: EnumStatusAgendaTarefa.Todos, text: "Status: " });

    this.Colaborador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            CarregarDadosPesquisa();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Agenda = function () {
    this.AdicionarNovaTarefa = PropertyEntity({ eventClick: adicionarNovaTarefaClick, type: types.event, text: "Adicionar Nova Tarefa", enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadAgenda() {
    _pesquisaAgenda = new PesquisaAgenda();
    KoBindings(_pesquisaAgenda, "knockoutPesquisaAgenda", false, _pesquisaAgenda.Pesquisar.id);

    _agenda = new Agenda();
    KoBindings(_agenda, "knockoutAgenda");

    new BuscarFuncionario(_pesquisaAgenda.Colaborador);
    new BuscarClientes(_pesquisaAgenda.Cliente);

    _calendarioAgenda = new CalendarioAgenda();

    // Previne a página de dar scroll quando o usuário está usando o scroll de uma das divs
    scrollfix();

    carregarLancamentoAgendaTarefa("conteudoAgendaTarefa");
    $("#divModalAgendaTarefa").on('hidden.bs.modal', function () {
        RenderizarDadosTarefa();
    });

    CarregarDadosPesquisa();
}

function scrollfix() {
    $('.scrollable, .fc-scroller').bind('mousewheel DOMMouseScroll', function (e) {
        if ($(this)[0].scrollHeight !== $(this).outerHeight()) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;

            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });
}

function adicionarNovaTarefaClick(e, sender) {
    limparCamposAgendaTarefa();
    Global.abrirModal('divModalAgendaTarefa');
}

//*******MÉTODOS*******
function CarregarDadosPesquisa() {
    var valido = ValidarCamposObrigatorios(_pesquisaAgenda);

    if (!valido)
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");

    // Guarda os valores da pesquisa
    _dadosPesquisaAgenda = RetornarObjetoPesquisa(_pesquisaAgenda);

    //Ao modificar tarefas com a agenda aberta, apenas fará a renderização
    RenderizarDadosTarefa();
}

function RenderizarDadosTarefa() {
    _pesquisaAgenda.ExibirFiltros.visibleFade(false);

    // Renderiza o conteudo
    $("#divGeralAgenda").removeClass("hidden");
    _calendarioAgenda.carregar();
}