/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumVerificarTipoDeOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroMonitoramentoEventoTipoDeOperacao;
var _CRUDcadastroMonitoramentoEventoTipoDeOperacao;
var _gridMonitoramentoEventoTipoDeOperacao;
var _monitoramentoEventoTipoDeOperacao;

/*
 * Declaração das Classes
 */

var CRUDCadastroMonitoramentoEventoTipoDeOperacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTipoDeOperacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarMonitoramentoEventoTipoDeOperacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirMonitoramentoEventoTipoDeOperacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CadastroMonitoramentoEventoTipoDeOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoDeOperacao = PropertyEntity({ text: "*Tipo de Operacao:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });
}

var MonitoramentoEventoTipoDeOperacao = function () {
    this.VerificarTipoDeOperacao = PropertyEntity({ text: "Verificar tipo de viagem?", getType: typesKnockout.bool, val: ko.observable(EnumVerificarTipoDeOperacao.NaoVerificar), options: EnumVerificarTipoDeOperacao.obterOpcoes(), visible: ko.observable(true) });
    this.VerificarTipoDeOperacao.val.subscribe(function (val) {
        if (val == EnumVerificarTipoDeOperacao.NaoVerificar) {
            _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.visible(false);
        } else {
            _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.visible(true);
        }
    });
    _monitoramentoEvento.VerificarTipoDeOperacao = this.VerificarTipoDeOperacao;

    this.ListaTipoDeOperacao = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(false) });
    this.ListaTipoDeOperacao.val.subscribe(function () {
        recarregarGridMonitoramentoEventoTipoDeOperacao();
    });
    this.Adicionar = PropertyEntity({ eventClick: adicionarMonitoramentoEventoTipoDeOperacaoModalClick, type: types.event, text: "Adicionar tipo de Operacao" });
}



/*
 * Declaração das Funções de Inicialização
 */

function loadGridMonitoramentoEventoTipoDeOperacao() {
    var linhasPorPaginas = 10;
    var ordenacao = { column: 3, dir: orderDir.asc };
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarMonitoramentoEventoTipoDeOperacaoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoDeOperacao", visible: false },
        { data: "DescricaoTipoDeOperacao", title: "Descrição", width: "100%", className: "text-align-left", orderable: false }
    ];
    _gridMonitoramentoEventoTipoDeOperacao = new BasicDataTable(_monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas, null, null, null);
    _gridMonitoramentoEventoTipoDeOperacao.CarregarGrid([]);
}

function loadMonitoramentoEventoTipoDeOperacao() {
    
    _monitoramentoEventoTipoDeOperacao = new MonitoramentoEventoTipoDeOperacao();
    KoBindings(_monitoramentoEventoTipoDeOperacao, "knockoutMonitoramentoEventoTipoDeOperacao");

    _cadastroMonitoramentoEventoTipoDeOperacao = new CadastroMonitoramentoEventoTipoDeOperacao();
    KoBindings(_cadastroMonitoramentoEventoTipoDeOperacao, "knockoutCadastroMonitoramentoEventoTipoDeOperacao");

    _CRUDcadastroMonitoramentoEventoTipoDeOperacao = new CRUDCadastroMonitoramentoEventoTipoDeOperacao();
    KoBindings(_CRUDcadastroMonitoramentoEventoTipoDeOperacao, "knockoutCRUDCadastroMonitoramentoEventoTipoDeOperacao");

    new BuscarTiposOperacao(_cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao);

    loadGridMonitoramentoEventoTipoDeOperacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarMonitoramentoEventoTipoDeOperacaoClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTipoDeOperacao)) {
        var valido = true;
        var listaTipoDeOperacao = obterListaTipoDeOperacao();
        listaTipoDeOperacao.forEach(function (row, i) {
            if (_cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.codEntity() == row.CodigoTipoDeOperacao) {
                valido = false;
                exibirMensagem(tipoMensagem.atencao, "Tipo de Operacao já adicionado", "Não é possível adicionar tipos de Operacao repetidos.");
            }
        });
        if (valido) {
            _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.val().push(obterCadastroMonitoramentoEventoTipoDeOperacaoSalvar());
            recarregarGridMonitoramentoEventoTipoDeOperacao();
            fecharModalCadastroMonitoramentoEventoTipoDeOperacao();
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function adicionarMonitoramentoEventoTipoDeOperacaoModalClick() {
    _cadastroMonitoramentoEventoTipoDeOperacao.Codigo.val(guid());
    controlarBotoesCadastroMonitoramentoEventoTipoDeOperacaoHabilitados(false);
    exibirModalCadastroMonitoramentoEventoTipoDeOperacao();
}

function atualizarMonitoramentoEventoTipoDeOperacaoClick() {
    if (ValidarCamposObrigatorios(_cadastroMonitoramentoEventoTipoDeOperacao)) {
        var listaTipoDeOperacao = obterListaTipoDeOperacao();
        listaTipoDeOperacao.forEach(function (row, i) {
            if (_cadastroMonitoramentoEventoTipoDeOperacao.Codigo.val() == row.Codigo) {
                listaTipoDeOperacao.splice(i, 1, obterCadastroMonitoramentoEventoTipoDeOperacaoSalvar());
            }
        });
        _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.val(listaTipoDeOperacao);
        recarregarGridMonitoramentoEventoTipoDeOperacao();
        fecharModalCadastroMonitoramentoEventoTipoDeOperacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function editarMonitoramentoEventoTipoDeOperacaoClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroMonitoramentoEventoTipoDeOperacao, { Data: registroSelecionado });
    _cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.codEntity(registroSelecionado.CodigoTipoDeOperacao);
    _cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.val(registroSelecionado.DescricaoTipoDeOperacao);
    _cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.entityDescription(registroSelecionado.DescricaoTipoDeOperacao);
    controlarBotoesCadastroMonitoramentoEventoTipoDeOperacaoHabilitados(true);
    exibirModalCadastroMonitoramentoEventoTipoDeOperacao();
}

function excluirMonitoramentoEventoTipoDeOperacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de Operacao?", function () {
        removerMonitoramentoEventoTipoDeOperacao(_cadastroMonitoramentoEventoTipoDeOperacao.Codigo.val());
        fecharModalCadastroMonitoramentoEventoTipoDeOperacao();
    });
}

/*
 * Declaração das Funções Públicas
 */

function obterMonitoramentoEventoTipoDeOperacaoSalvar() {
    var listaTipoDeOperacao = obterListaTipoDeOperacao();
    return JSON.stringify(listaTipoDeOperacao);
}

function preencherMonitoramentoEventoTipoDeOperacao(dados) {
    _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.val(dados);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroMonitoramentoEventoTipoDeOperacaoHabilitados(isEdicao) {
    _CRUDcadastroMonitoramentoEventoTipoDeOperacao.Adicionar.visible(!isEdicao);
    _CRUDcadastroMonitoramentoEventoTipoDeOperacao.Atualizar.visible(isEdicao);
    _CRUDcadastroMonitoramentoEventoTipoDeOperacao.Excluir.visible(isEdicao);
}

function exibirModalCadastroMonitoramentoEventoTipoDeOperacao() {
    Global.abrirModal('divModalCadastroMonitoramentoEventoTipoDeOperacao');
    $("#divModalCadastroMonitoramentoEventoTipoDeOperacao").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroMonitoramentoEventoTipoDeOperacao);
    });
}

function fecharModalCadastroMonitoramentoEventoTipoDeOperacao() {
    Global.fecharModal('divModalCadastroMonitoramentoEventoTipoDeOperacao');
}

function limparCamposMonitoramentoEventoTipoDeOperacao() {
    preencherMonitoramentoEventoTipoDeOperacao([]);
}

function obterCadastroMonitoramentoEventoTipoDeOperacaoSalvar() {
    return {
        Codigo: _cadastroMonitoramentoEventoTipoDeOperacao.Codigo.val(),
        CodigoTipoDeOperacao: _cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.codEntity(),
        DescricaoTipoDeOperacao: _cadastroMonitoramentoEventoTipoDeOperacao.TipoDeOperacao.val()
    };
}

function obterListaTipoDeOperacao() {
    return _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.val().slice();
}

function recarregarGridMonitoramentoEventoTipoDeOperacao() {
    var listaTipoDeOperacao = obterListaTipoDeOperacao();
    _gridMonitoramentoEventoTipoDeOperacao.CarregarGrid(listaTipoDeOperacao);
}

function removerMonitoramentoEventoTipoDeOperacao(codigo) {
    var listaTipoDeOperacao = obterListaTipoDeOperacao();
    listaTipoDeOperacao.forEach(function (row, i) {
        if (codigo == row.Codigo) {
            listaTipoDeOperacao.splice(i, 1);
        }
    });
    _monitoramentoEventoTipoDeOperacao.ListaTipoDeOperacao.val(listaTipoDeOperacao);
}

function validarCamposObrigatoriosMonitoramentoEventoTipoDeOperacao() {
    if (_monitoramentoEventoTipoDeOperacao.VerificarTipoDeOperacao.val() != EnumVerificarTipoDeOperacao.NaoVerificar) {
        var listaTipoDeOperacao = obterListaTipoDeOperacao();
        if (listaTipoDeOperacao.length == 0) {
            return false;
        }
    }
    return true;
}
