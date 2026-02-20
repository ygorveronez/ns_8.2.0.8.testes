/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumVerificarStatusViagem.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoStatusViagem;
var _CRUDcadastroMonitoramentoEventoStatusViagem;
var _gridMonitoramentoEventoStatusViagem;
var _monitoramentoEventoStatusViagem;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoStatusViagem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoStatusViagemClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoStatusViagemClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoStatusViagemClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoStatusViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MonitoramentoStatusViagem = PropertyEntity({ text: "*Status de viagem:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
}

var MonitoramentoEventoStatusViagem = function () {
    this.VerificarStatusViagem = PropertyEntity({ text: "Verificar status de viagem?", getType: typesKnockout.bool, val: ko.observable(EnumVerificarStatusViagem.NaoVerificar), options: EnumVerificarStatusViagem.obterOpcoes(), visible: ko.observable(true) });
    this.VerificarStatusViagem.val.subscribe(function (val) {
        if (val == EnumVerificarStatusViagem.NaoVerificar) {
            _monitoramentoEventoStatusViagem.ListaStatusViagem.visible(false);
        } else {
            _monitoramentoEventoStatusViagem.ListaStatusViagem.visible(true);
        }
    });
    _monitoramentoEvento.VerificarStatusViagem = this.VerificarStatusViagem;

    this.ListaStatusViagem = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });
    this.ListaStatusViagem.val.subscribe(function () {
        recarregarGridMonitoramentoEventoStatusViagem();
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoStatusViagemModalClick, type: types.event, text: "Adicionar status de viagem" });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoStatusViagem() {
    var linhasPorPaginas = 10;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoStatusViagemClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoMonitoramentoStatusViagem", visible: false },
        { data: "DescricaoMonitoramentoStatusViagem", title: "Descrição", width: "100%", className: "text-align-left", orderable: false }
    ];
    _gridMonitoramentoEventoStatusViagem = new BasicDataTable(_monitoramentoEventoStatusViagem.ListaStatusViagem.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas, null, null, null);
    _gridMonitoramentoEventoStatusViagem.CarregarGrid([]);
}

function loadMonitoramentoEventoStatusViagem() {
    _monitoramentoEventoStatusViagem = new MonitoramentoEventoStatusViagem();
    KoBindings(_monitoramentoEventoStatusViagem, "knockoutMonitoramentoEventoStatusViagem");

    _cadastroMonitoramentoEventoStatusViagem = new CadastroMonitoramentoEventoStatusViagem();
    KoBindings(_cadastroMonitoramentoEventoStatusViagem, "knockoutCadastroMonitoramentoEventoStatusViagem");

    _CRUDcadastroMonitoramentoEventoStatusViagem = new CRUDCadastroMonitoramentoEventoStatusViagem();
    KoBindings(_CRUDcadastroMonitoramentoEventoStatusViagem, "knockoutCRUDCadastroMonitoramentoEventoStatusViagem");

    new BuscarMonitoramentoEventoStatusViagem(_cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem);

    loadGridMonitoramentoEventoStatusViagem();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoStatusViagemClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoStatusViagem)) {
        var valido = true;
        var listaStatusViagem = obterListaStatusViagem();
        listaStatusViagem.forEach(function (status, i) {
            if (_cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.codEntity() == status.CodigoMonitoramentoStatusViagem) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Status de viagem já adicionado", "Não é possível adicionar status de viagem repetidos.");
            }
        });
        if (valido) {
            _monitoramentoEventoStatusViagem.ListaStatusViagem.val().push(obterCadastroMonitoramentoEventoStatusViagemSalvar());
            recarregarGridMonitoramentoEventoStatusViagem();
            fecharModalCadastroMonitoramentoEventoStatusViagem();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function adicionarMonitoramentoEventoStatusViagemModalClick() {
    _cadastroMonitoramentoEventoStatusViagem.Codigo.val(guid());
    controlarBotoesCadastroMonitoramentoEventoStatusViagemHabilitados(false);
    exibirModalCadastroMonitoramentoEventoStatusViagem();
}

function atualizarMonitoramentoEventoStatusViagemClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoStatusViagem)) {
        var listaStatusViagem = obterListaStatusViagem();
        listaStatusViagem.forEach(function (status, i) {
            if (_cadastroMonitoramentoEventoStatusViagem.Codigo.val() == status.Codigo) {
                listaStatusViagem.splice(i, 1, obterCadastroMonitoramentoEventoStatusViagemSalvar());
            }
        });
        _monitoramentoEventoStatusViagem.ListaStatusViagem.val(listaStatusViagem);
        recarregarGridMonitoramentoEventoStatusViagem();
        fecharModalCadastroMonitoramentoEventoStatusViagem();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function editarMonitoramentoEventoStatusViagemClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoStatusViagem, { Data: registroSelecionado });
    _cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.codEntity(registroSelecionado.CodigoMonitoramentoStatusViagem);
    _cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.val(registroSelecionado.DescricaoMonitoramentoStatusViagem);
    _cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.entityDescription(registroSelecionado.DescricaoMonitoramentoStatusViagem);
    controlarBotoesCadastroMonitoramentoEventoStatusViagemHabilitados(true);
    exibirModalCadastroMonitoramentoEventoStatusViagem();
}

function excluirMonitoramentoEventoStatusViagemClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o status de viagem?", function () {
        removerMonitoramentoEventoStatusViagem(_cadastroMonitoramentoEventoStatusViagem.Codigo.val());
        fecharModalCadastroMonitoramentoEventoStatusViagem();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoStatusViagemSalvar() {
    var listaStatusViagem = obterListaStatusViagem();
    return JSON.stringify(listaStatusViagem);
}

function preencherMonitoramentoEventoStatusViagem(dados) {
    _monitoramentoEventoStatusViagem.ListaStatusViagem.val(dados);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoStatusViagemHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoStatusViagem.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoStatusViagem.Atualizar.visible(isEdicao);
    _CRUDcadastroMonitoramentoEventoStatusViagem.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoStatusViagem() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoStatusViagem');
    $("#divModalCadastroMonitoramentoEventoStatusViagem").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoStatusViagem);
    });
}

function fecharModalCadastroMonitoramentoEventoStatusViagem() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoStatusViagem');
}

function limparCamposMonitoramentoEventoStatusViagem() {
    preencherMonitoramentoEventoStatusViagem([]);
}

function obterCadastroMonitoramentoEventoStatusViagemSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoStatusViagem.Codigo.val(),
        CodigoMonitoramentoStatusViagem: _cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.codEntity(),
        DescricaoMonitoramentoStatusViagem: _cadastroMonitoramentoEventoStatusViagem.MonitoramentoStatusViagem.val()
    };
}

function obterListaStatusViagem() {
    return _monitoramentoEventoStatusViagem.ListaStatusViagem.val().slice();
}

function recarregarGridMonitoramentoEventoStatusViagem() {
    var listaStatusViagem = obterListaStatusViagem();
    _gridMonitoramentoEventoStatusViagem.CarregarGrid(listaStatusViagem);
}

function removerMonitoramentoEventoStatusViagem(codigo) {
    var listaStatusViagem = obterListaStatusViagem();
    listaStatusViagem.forEach(function (status, i) {
        if (codigo == status.Codigo) {
            listaStatusViagem.splice(i, 1);
        }
    });
    _monitoramentoEventoStatusViagem.ListaStatusViagem.val(listaStatusViagem);
}

function validarCamposObrigatoriosMonitoramentoEventoStatusViagem() {
    if (_monitoramentoEventoStatusViagem.VerificarStatusViagem.val() != EnumVerificarStatusViagem.NaoVerificar) {
        var listaStatusViagem = obterListaStatusViagem();
        if (listaStatusViagem.length == 0) {
            return false;
        }
    }
    return true;
}
