/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ContratoPrestacaoServico.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _contratoPrestacaoServicoSaldo;
var _contratoPrestacaoServicoSaldoCadastro;
var _gridContratoPrestacaoServicoSaldo;
var _pesquisaContratoPrestacaoServicoSaldo;
var _pesquisaContratoPrestacaoServicoSaldoAuxiliar;

/*
 * Declaração das Classes
 */

var PesquisaContratoPrestacaoServicoSaldo = function () {
    this.ContratoPrestacaoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Contrato de Prestação de Serviço:", idBtnSearch: guid(), required: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataLimite = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.TipoLancamento = PropertyEntity({ val: ko.observable(EnumTipoLancamento.Todos), def: EnumTipoLancamento.Todos, options: EnumTipoLancamento.obterOpcoesPesquisa(), text: "Tipo do Lançamento:" });

    this.DataInicial.dateRangeLimit = this.DataLimite;
    this.DataLimite.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            if (ValidarCamposObrigatorios(_pesquisaContratoPrestacaoServicoSaldo)) {
                $("#contrato-prestacao-servico-saldo-container").removeClass("d-none");

                _pesquisaContratoPrestacaoServicoSaldo.ExibirFiltros.visibleFade(false);

                atualizarFiltrosUltimaPesquisa();
                recarregarGridContratoPrestacaoServicoSaldo();
            }
            else
                exibirMensagemCamposObrigatorio();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaContratoPrestacaoServicoSaldoAuxiliar = function () {
    this.ContratoPrestacaoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date });
    this.DataLimite = PropertyEntity({ getType: typesKnockout.date });
    this.TipoLancamento = PropertyEntity({ val: ko.observable(EnumTipoLancamento.Todos), def: EnumTipoLancamento.Todos, options: EnumTipoLancamento.obterOpcoesPesquisa() });
}

var ContratoPrestacaoServicoSaldo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarContratoPrestacaoServicoSaldoModalClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

var ContratoPrestacaoServicoSaldoCadastro = function () {
    this.ContratoPrestacaoServico = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400, required: true });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 18, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarContratoPrestacaoServicoSaldoClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadDroppableContratoPrestacaoServicoSaldo() {
    $("#remocao-item-contrato-prestacao-servico-saldo").droppable({
        drop: function (event, ui) {
            var idOrigem = $(ui.draggable[0]).parent().parent()[0].id;

            if (idOrigem == "grid-contrato-prestacao-servico-saldo")
                removerContratoPrestacaoServicoSaldo(ui.draggable[0].id);
        },
        hoverClass: "remocao-item-container-hover",
    });
}

function loadGridContratoPrestacaoServicoSaldo() {
    var limiteRegistros = 20;
    var ordenacaoPadrao = { column: "2", dir: "desc" };
    var totalRegistrosPorPagina = 20;

    _gridContratoPrestacaoServicoSaldo = new GridView("grid-contrato-prestacao-servico-saldo", "ContratoPrestacaoServicoSaldo/Pesquisa", _pesquisaContratoPrestacaoServicoSaldoAuxiliar, null, ordenacaoPadrao, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowContratoPrestacaoServicoSaldo);
}

function loadContratoPrestacaoServicoSaldo() {
    _pesquisaContratoPrestacaoServicoSaldo = new PesquisaContratoPrestacaoServicoSaldo();
    KoBindings(_pesquisaContratoPrestacaoServicoSaldo, "knockoutPesquisaContratoPrestacaoServicoSaldo", false, _pesquisaContratoPrestacaoServicoSaldo.Pesquisar.id);

    _pesquisaContratoPrestacaoServicoSaldoAuxiliar = new PesquisaContratoPrestacaoServicoSaldoAuxiliar();

    _contratoPrestacaoServicoSaldo = new ContratoPrestacaoServicoSaldo();
    KoBindings(_contratoPrestacaoServicoSaldo, "knockoutContratoPrestacaoServicoSaldo");

    _contratoPrestacaoServicoSaldoCadastro = new ContratoPrestacaoServicoSaldoCadastro();
    KoBindings(_contratoPrestacaoServicoSaldoCadastro, "knockoutCadastroContratoPrestacaoServicoSaldo");

    new BuscarContratoPrestacaoServicoAtivo(_pesquisaContratoPrestacaoServicoSaldo.ContratoPrestacaoServico);

    loadGridContratoPrestacaoServicoSaldo();
    loadDroppableContratoPrestacaoServicoSaldo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarContratoPrestacaoServicoSaldoClick() {
    if (ValidarCamposObrigatorios(_contratoPrestacaoServicoSaldoCadastro)) {
        executarReST("ContratoPrestacaoServicoSaldo/Adicionar", RetornarObjetoPesquisa(_contratoPrestacaoServicoSaldoCadastro), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Adicionado saldo com sucesso");

                    fecharContratoPrestacaoServicoSaldoModal();
                    recarregarGridContratoPrestacaoServicoSaldo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarContratoPrestacaoServicoSaldoModalClick() {
    if (_pesquisaContratoPrestacaoServicoSaldoAuxiliar.ContratoPrestacaoServico.codEntity() > 0) {
        _contratoPrestacaoServicoSaldoCadastro.ContratoPrestacaoServico.val(_pesquisaContratoPrestacaoServicoSaldoAuxiliar.ContratoPrestacaoServico.codEntity());

        Global.abrirModal('divModalCadastroContratoPrestacaoServicoSaldo');
        $("#divModalCadastroContratoPrestacaoServicoSaldo").one('hidden.bs.modal', function () {
            LimparCampos(_contratoPrestacaoServicoSaldoCadastro);
        });
    }
}

/*
 * Declaração das Funções
 */

function atualizarFiltrosUltimaPesquisa() {
    _pesquisaContratoPrestacaoServicoSaldoAuxiliar.ContratoPrestacaoServico.codEntity(_pesquisaContratoPrestacaoServicoSaldo.ContratoPrestacaoServico.codEntity());
    _pesquisaContratoPrestacaoServicoSaldoAuxiliar.ContratoPrestacaoServico.val(_pesquisaContratoPrestacaoServicoSaldo.ContratoPrestacaoServico.val());
    _pesquisaContratoPrestacaoServicoSaldoAuxiliar.DataInicial.val(_pesquisaContratoPrestacaoServicoSaldo.DataInicial.val());
    _pesquisaContratoPrestacaoServicoSaldoAuxiliar.DataLimite.val(_pesquisaContratoPrestacaoServicoSaldo.DataLimite.val());
    _pesquisaContratoPrestacaoServicoSaldoAuxiliar.TipoLancamento.val(_pesquisaContratoPrestacaoServicoSaldo.TipoLancamento.val());
}

function callbackRowContratoPrestacaoServicoSaldo(nRow, aData) {
    if (aData.TipoLancamento == EnumTipoLancamento.Manual) {
        $(nRow).draggable({
            cursor: "move",
            helper: function (event) {
                var html = '';

                $(event.currentTarget).children().each(function (i, coluna) {
                    var $coluna = $(coluna);

                    html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
                });

                var corLinha = $(event.currentTarget).css("background-color");
                var corLinhaSelecionada = "#ecf3f8";
                var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
                var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

                return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
            },
            revert: 'invalid',
            start: function () {
                $("#remocao-item-contrato-prestacao-servico-saldo").show();
            },
            stop: function () {
                $("#remocao-item-contrato-prestacao-servico-saldo").hide();
            }
        });
    }
    else
        $(nRow).css("cursor", "default");
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function fecharContratoPrestacaoServicoSaldoModal() {
    Global.fecharModal('divModalCadastroContratoPrestacaoServicoSaldo');
}

function recarregarGridContratoPrestacaoServicoSaldo() {
    _gridContratoPrestacaoServicoSaldo.CarregarGrid();
}

function removerContratoPrestacaoServicoSaldo(codigoContratoPrestacaoServicoSaldo) {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir o saldo?", function () {
        executarReST("ContratoPrestacaoServicoSaldo/Remover", { Codigo: codigoContratoPrestacaoServicoSaldo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Removido saldo com sucesso");
                    recarregarGridContratoPrestacaoServicoSaldo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}