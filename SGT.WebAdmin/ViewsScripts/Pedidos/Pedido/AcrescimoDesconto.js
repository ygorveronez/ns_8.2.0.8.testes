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
/// <reference path="../../Consultas/ClassificacaoRiscoONU.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/LinhaSeparacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="Pedido.js" />
/// <reference path="Adicional.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _acrescimoDesconto;
var _gridAcrescimoDesconto;

var AcrescimoDesconto = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, idGrid: guid() });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pedidos.Pedido.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", required: true });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Pedidos.Pedido.Observacao.getFieldDescription(), val: ko.observable(""), def: "", required: false, maxlength: 400, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Justificativa.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Aplicacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAcrescimoDescontoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAcrescimoDescontoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar });
}

//*******EVENTOS*******

function loadAcrescimoDesconto() {
    _acrescimoDesconto = new AcrescimoDesconto();
    KoBindings(_acrescimoDesconto, "knockoutAcrescimoDesconto");

    new BuscarJustificativas(_acrescimoDesconto.Justificativa, RetornoSelecaoJustificativa, null, [EnumTipoFinalidadeJustificativa.ContratoFrete, EnumTipoFinalidadeJustificativa.Todas]);

    loadGridAcrescimoDesconto();
}

function AdicionarAcrescimoDescontoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_acrescimoDesconto)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.VerifiqueCampos);
        return;
    }

    var acrescimosDescontosPedido = _pedido.AcrescimoDesconto.val();

    acrescimosDescontosPedido.push({
        Codigo: guid(),
        Justificativa: {
            Descricao: _acrescimoDesconto.Justificativa.val(),
            Codigo: _acrescimoDesconto.Justificativa.codEntity()
        },
        Valor: _acrescimoDesconto.Valor.val(),
        Observacao: _acrescimoDesconto.Observacao.val(),
        Tipo: _acrescimoDesconto.Tipo.val(),
        Aplicacao: _acrescimoDesconto.Aplicacao.val(),
    });

    _pedido.AcrescimoDesconto.val(acrescimosDescontosPedido);

    recarregarGridAcrescimoDesconto();
    limparAcrescimoDesconto();
}

function AtualizarAcrescimoDescontoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_acrescimoDesconto)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pedidos.Pedido.VerifiqueCampos);
        return;
    }

    var acrescimosDescontosPedido = _pedido.AcrescimoDesconto.val();

    for (var i = 0; i < acrescimosDescontosPedido.length; i++) {
        if (acrescimosDescontosPedido[i].Codigo == _acrescimoDesconto.Codigo.val()) {
            acrescimosDescontosPedido[i] = {
                Codigo: _acrescimoDesconto.Codigo.val(),
                Justificativa: {
                    Descricao: _acrescimoDesconto.Justificativa.val(),
                    Codigo: _acrescimoDesconto.Justificativa.codEntity()
                },
                Valor: _acrescimoDesconto.Valor.val(),
                Observacao: _acrescimoDesconto.Observacao.val(),
                Tipo: _acrescimoDesconto.Tipo.val(),
                Aplicacao: _acrescimoDesconto.Aplicacao.val(),
            };
            break;
        }
    }

    _pedido.AcrescimoDesconto.val(acrescimosDescontosPedido);

    recarregarGridAcrescimoDesconto();
    limparAcrescimoDesconto();
}

function CancelarAcrescimoDescontoClick(e, sender) {
    limparAcrescimoDesconto();
}

function EditarAcrescimoDescontoClick(data) {
    var acrescimosDescontosPedido = _pedido.AcrescimoDesconto.val();

    for (var i = 0; i < acrescimosDescontosPedido.length; i++) {
        if (acrescimosDescontosPedido[i].Codigo == data.Codigo) {
            PreencherObjetoKnout(_acrescimoDesconto, { Data: acrescimosDescontosPedido[i] });
            _acrescimoDesconto.Adicionar.visible(false);
            _acrescimoDesconto.Atualizar.visible(true);
            _acrescimoDesconto.Excluir.visible(true);
            break;
        }
    }
}

function ExcluirAcrescimoDescontoClick(data) {
    var acrescimosDescontosPedido = _pedido.AcrescimoDesconto.val();

    for (var i = 0; i < acrescimosDescontosPedido.length; i++) {
        if (acrescimosDescontosPedido[i].Codigo == _acrescimoDesconto.Codigo.val()) {
            acrescimosDescontosPedido.splice(i, 1);
            break;
        }
    }

    _pedido.AcrescimoDesconto.val(acrescimosDescontosPedido);
    recarregarGridAcrescimoDesconto();
    limparAcrescimoDesconto();
}

//*******MÉTODOS*******

function loadGridAcrescimoDesconto() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: EditarAcrescimoDescontoClick, tamanho: "10", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: "Opções", tamanho: 10, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Justificativa", title: Localization.Resources.Pedidos.Pedido.Justificativa , width: "23%" },
        { data: "Tipo", title: Localization.Resources.Gerais.Geral.Tipo, width: "23%" },
        { data: "Aplicacao", title: Localization.Resources.Pedidos.Pedido.Aplicacao, width: "23%" },
        { data: "Valor", title: Localization.Resources.Pedidos.Pedido.Valor, width: "23%" },
    ];

    _gridAcrescimoDesconto = new BasicDataTable(_acrescimoDesconto.Codigo.idGrid, header, menuOpcoes);
    recarregarGridAcrescimoDesconto();
}

function recarregarGridAcrescimoDesconto() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pedido.AcrescimoDesconto.val())) {
        $.each(_pedido.AcrescimoDesconto.val(), function (i, acrescimentoDesconto) {
            var obj = {
                Codigo: acrescimentoDesconto.Codigo,
                Justificativa: acrescimentoDesconto.Justificativa.Descricao,
                Tipo: acrescimentoDesconto.Tipo,
                Aplicacao: acrescimentoDesconto.Aplicacao,
                Valor: acrescimentoDesconto.Valor,
            };
            data.push(obj);
        });
    }
    _gridAcrescimoDesconto.CarregarGrid(data);
}

function limparCamposAcrescimoDesconto() {
    _pedido.AcrescimoDesconto.val([]);
    recarregarGridAcrescimoDesconto();
}

function limparAcrescimoDesconto() {
    LimparCampos(_acrescimoDesconto);
    recarregarGridAcrescimoDesconto();

    _acrescimoDesconto.Adicionar.visible(true);
    _acrescimoDesconto.Atualizar.visible(false);
    _acrescimoDesconto.Excluir.visible(false);
}

function RetornoSelecaoJustificativa(data) {
    _acrescimoDesconto.Justificativa.val(data.Descricao);
    _acrescimoDesconto.Justificativa.entityDescription(data.Descricao);
    _acrescimoDesconto.Justificativa.codEntity(data.Codigo);
    _acrescimoDesconto.Tipo.val(data.DescricaoTipoJustificativa);
    _acrescimoDesconto.Aplicacao.val(data.DescricaoAplicacaoValorContratoFrete);
}