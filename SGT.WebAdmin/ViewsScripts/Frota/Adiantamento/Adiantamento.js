/// <reference path="../../Consultas/TipoMovimentoMotorista.js" />
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
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAdiantamento.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoEntidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridAdiantamento;
var _adiantamento;
var _pesquisaAdiantamento;

var _sitaucaoAdiantamento = [{ text: "Todos", value: EnumSituacaoAdiantamento.Todos }, { text: "Ativo", value: EnumSituacaoAdiantamento.Ativo }, { text: "Estornado", value: EnumSituacaoAdiantamento.Estornado }];
var _tipoPagamentoAdiantamentoMotorista = [{ text: "Adiantamento", value: EnumTipoPagamentoAdiantamentoMotorista.Adiantamento }, { text: "Pagamento de Salário", value: EnumTipoPagamentoAdiantamentoMotorista.Comissao }];
var _situacaoPagamentoAdiantamentoMotorista = [{ text: "Entrada", value: EnumTipoMovimentoEntidade.Entrada }, { text: "Saída", value: EnumTipoMovimentoEntidade.Saida }];

var PesquisaAdiantamento = function () {
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), val: ko.observable("") });
    this.ValorInicial = PropertyEntity({ text: "Valor Inicial:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.ValorFinal = PropertyEntity({ text: "Valor Final:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAdiantamento.Ativo), options: _sitaucaoAdiantamento, def: EnumSituacaoAdiantamento.Ativo });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAdiantamento.CarregarGrid();
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

var Adiantamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true });
    this.TipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de Pagamento:"), idBtnSearch: guid(), required: true });
    this.TipoMovimentoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Tipo de Movimento do Motorista:"), idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ text: "*Valor:", required: true, getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(0.00), maxlength: 10 });
    this.Data = PropertyEntity({ text: "*Data do Movimento:", required: true, getType: typesKnockout.date });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Estornar = PropertyEntity({ eventClick: estornarClick, type: types.event, text: "Estornar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadAdiantamento() {

    _adiantamento = new Adiantamento();
    KoBindings(_adiantamento, "knockoutLancamentoAdiantamento");

    _pesquisaAdiantamento = new PesquisaAdiantamento();
    KoBindings(_pesquisaAdiantamento, "knockoutPesquisaAdiantamento", false, _pesquisaAdiantamento.Pesquisar.id);

    HeaderAuditoria("Adiantamento", _adiantamento);

    new BuscarMotoristas(_pesquisaAdiantamento.Motorista);

    new BuscarMotoristas(_adiantamento.Motorista);
    new BuscarTipoPagamentoRecebimento(_adiantamento.TipoPagamento);
    new BuscarTipoMovimentoMotorista(_adiantamento.TipoMovimentoMotorista, null, null, RetornoTipoMovimentoMotorista);

    buscarAdiantamento();
}

function RetornoTipoMovimentoMotorista(data) {
    _adiantamento.TipoMovimentoMotorista.val(data.Descricao);
    _adiantamento.TipoMovimentoMotorista.codEntity(data.Codigo);
    if (data.TipoMovimentoEntidade == EnumTipoMovimentoEntidade.Entrada) {
        _adiantamento.TipoMovimentoMotorista.text("*Tipo de Movimento do Motorista (Entrada):");
        _adiantamento.TipoPagamento.text("*Tipo de Pagamento (Saída):");
    } else if (data.TipoMovimentoEntidade == EnumTipoMovimentoEntidade.Saida) {
        _adiantamento.TipoMovimentoMotorista.text("*Tipo de Movimento do Motorista (Saída):");
        _adiantamento.TipoPagamento.text("*Tipo de Pagamento (Entrada):");
    }
}

function adicionarClick(e, sender) {
    Salvar(e, "Adiantamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridAdiantamento.CarregarGrid();
                limparAdiantamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function estornarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja estornar esse adiantamento?", function () {
        ExcluirPorCodigo(_adiantamento, "Adiantamento/EstornarPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Estornado com sucesso");
                    _gridAdiantamento.CarregarGrid();
                    limparAdiantamento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparAdiantamento();
}

//*******MÉTODOS*******


function buscarAdiantamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAdiantamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAdiantamento = new GridView(_pesquisaAdiantamento.Pesquisar.idGrid, "Adiantamento/Pesquisa", _pesquisaAdiantamento, menuOpcoes, null);
    _gridAdiantamento.CarregarGrid();
}

function editarAdiantamento(adiantamentoGrid) {
    limparAdiantamento();
    _adiantamento.Codigo.val(adiantamentoGrid.Codigo);
    BuscarPorCodigo(_adiantamento, "Adiantamento/BuscarPorCodigo", function (arg) {
        var adianamentoAtivo = arg.Data.Situacao == EnumSituacaoAdiantamento.Ativo;

        _pesquisaAdiantamento.ExibirFiltros.visibleFade(false);
        _adiantamento.Cancelar.visible(true);
        _adiantamento.Estornar.visible(adianamentoAtivo);
        _adiantamento.Adicionar.visible(false);
    }, null);
}

function limparAdiantamento() {
    _adiantamento.Cancelar.visible(false);
    _adiantamento.Estornar.visible(false);
    _adiantamento.Adicionar.visible(true);
    _adiantamento.TipoPagamento.text("*Tipo de Pagamento:");
    _adiantamento.TipoMovimentoMotorista.text("*Tipo de Movimento do Motorista:");
    LimparCampos(_adiantamento);
}
