/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
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
/// <reference path="../../Consultas/Sistema.js" />
/// <reference path="../../Consultas/Modulo.js" />
/// <reference path="../../Consultas/Servico.js" />
/// <reference path="../../Enumeradores/EnumStatusAgendaTarefa.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeAtendimento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAgendaTarefa;
var _agendaTarefa;
var _pesquisaAgendaTarefa;

var _prioridadeConsulta = [
    { text: "Todos", value: EnumPrioridadeAtendimento.Todos },
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var _prioridadeChamado = [
    { text: "Baixa", value: EnumPrioridadeAtendimento.Baixa },
    { text: "Normal", value: EnumPrioridadeAtendimento.Normal },
    { text: "Alta", value: EnumPrioridadeAtendimento.Alta }
];

var PesquisaAgendaTarefa = function () {
    this.Observacao = PropertyEntity({ text: "Observação: " });

    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAgendaTarefa.Todos), options: EnumStatusAgendaTarefa.obterOpcoesPesquisa(), def: EnumStatusAgendaTarefa.Todos, text: "Status: " });
    this.DataInicial = PropertyEntity({ text: "Data da tarefa de: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Até: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Colaborador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAgendaTarefa.CarregarGrid();
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

var AgendaTarefa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "Data e Hora Inicial: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.dateTime, required: true });
    this.DataFinal = PropertyEntity({ text: "Data e Hora Final: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.dateTime, required: true });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusAgendaTarefa.Aberto), options: EnumStatusAgendaTarefa.obterOpcoes(), def: EnumStatusAgendaTarefa.Aberto, text: "*Status: ", required: true });
    this.Prioridade = PropertyEntity({ val: ko.observable(EnumPrioridadeAtendimento.Normal), options: _prioridadeChamado, def: EnumPrioridadeAtendimento.Normal, text: "*Prioridade do Chamado: ", required: true });

    this.Observacao = PropertyEntity({ text: "*Observação: ", required: true, maxlength: 5000 });

    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Colaborador:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), required: false });
    this.Servico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Serviço:", idBtnSearch: guid(), visible: ko.observable(true), required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAgendaTarefa() {
    carregarLancamentoAgendaTarefa("conteudoAgendaTarefa", loadPesquisaAgendaTarefa);
}

function loadPesquisaAgendaTarefa() {
    HeaderAuditoria("AgendaTarefa", _agendaTarefa);

    _pesquisaAgendaTarefa = new PesquisaAgendaTarefa();
    KoBindings(_pesquisaAgendaTarefa, "knockoutPesquisaAgendaTarefa", false, _pesquisaAgendaTarefa.Pesquisar.id);

    new BuscarFuncionario(_pesquisaAgendaTarefa.Usuario);
    new BuscarClientes(_pesquisaAgendaTarefa.Cliente);

    buscarAgendaTarefas();
}

function carregarLancamentoAgendaTarefa(idDivConteudo, callback) {
    $.get("Content/Static/Agenda/AgendaTarefa.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);

        _agendaTarefa = new AgendaTarefa();
        KoBindings(_agendaTarefa, "knockoutCadastroAgendaTarefa");

        new BuscarFuncionario(_agendaTarefa.Usuario);
        new BuscarClientes(_agendaTarefa.Cliente);
        new BuscarServicoTMS(_agendaTarefa.Servico);

        buscarFuncionarioLogado();

        if (callback !== undefined && callback !== null)
            callback();
    });
}

function adicionarClick(e, sender) {
    Salvar(e, "AgendaTarefa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridAgendaTarefa();
                limparCamposAgendaTarefa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "AgendaTarefa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridAgendaTarefa();
                limparCamposAgendaTarefa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tarefa " + _agendaTarefa.Observacao.val() + "?", function () {
        ExcluirPorCodigo(_agendaTarefa, "AgendaTarefa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    recarregarGridAgendaTarefa();
                    limparCamposAgendaTarefa();
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
    limparCamposAgendaTarefa();
}

//*******MÉTODOS*******

function buscarAgendaTarefas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAgendaTarefa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAgendaTarefa = new GridView(_pesquisaAgendaTarefa.Pesquisar.idGrid, "AgendaTarefa/Pesquisa", _pesquisaAgendaTarefa, menuOpcoes, null);
    _gridAgendaTarefa.CarregarGrid();
}

function recarregarGridAgendaTarefa() {
    if (_gridAgendaTarefa !== undefined && _gridAgendaTarefa !== null)
        _gridAgendaTarefa.CarregarGrid();
}

function editarAgendaTarefa(agendaTarefaGrid, codigo) {
    limparCamposAgendaTarefa();

    if (agendaTarefaGrid == null)//Quando edita pela Agenda
        _agendaTarefa.Codigo.val(codigo);
    else
        _agendaTarefa.Codigo.val(agendaTarefaGrid.Codigo);
    BuscarPorCodigo(_agendaTarefa, "AgendaTarefa/BuscarPorCodigo", function (arg) {
        if (_pesquisaAgendaTarefa != undefined)
            _pesquisaAgendaTarefa.ExibirFiltros.visibleFade(false);
        _agendaTarefa.Atualizar.visible(true);
        _agendaTarefa.Cancelar.visible(true);
        _agendaTarefa.Excluir.visible(true);
        _agendaTarefa.Adicionar.visible(false);
    }, null);
}

function limparCamposAgendaTarefa() {
    _agendaTarefa.Atualizar.visible(false);
    _agendaTarefa.Cancelar.visible(false);
    _agendaTarefa.Excluir.visible(false);
    _agendaTarefa.Adicionar.visible(true);
    LimparCampos(_agendaTarefa);
    _agendaTarefa.DataInicial.val("");
    _agendaTarefa.DataFinal.val("");
    buscarFuncionarioLogado();
    if (Global.contemModalAberto("divModalAgendaTarefa"))
        Global.fecharModal("divModalAgendaTarefa");
}

function buscarFuncionarioLogado() {
    executarReST("PedidoVenda/BuscarFuncionarioLogado", null, function (r) {
        if (r.Success) {
            _agendaTarefa.Usuario.codEntity(r.Data.Codigo);
            _agendaTarefa.Usuario.val(r.Data.Nome);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}