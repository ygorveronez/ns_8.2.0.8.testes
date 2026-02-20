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
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSubcontratacao;
var _subcontratacao;

var Subcontratacao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.TransportadorTerceiro.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 56 });
    this.PercentualDesconto = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemDesconto.getFieldDescription(), issue: 702, val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), getType: typesKnockout.decimal, maxlength: 5, required: false, configDecimal: { precision: 2, allowZero: true } });
    this.PercentualCobranca = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemCobranca.getFieldDescription(), issue: 703, val: ko.observable(Globalize.format(0, "n2")), def: Globalize.format(0, "n2"), getType: typesKnockout.decimal, maxlength: 5, required: false, configDecimal: { precision: 2, allowZero: true } });

    this.PercentualCobrancaPadrao = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemCobrancaPadrao.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: false, configDecimal: { precision: 6, allowZero: false }, requiredClass: ko.observable("") });
    this.PercentualCobrancaVeiculoFrota = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.PorcentagemCobrancaVeiculoFrota.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, required: false, configDecimal: { precision: 6, allowZero: false }, requiredClass: ko.observable("") });

    this.Adicionar = PropertyEntity({ eventClick: adicionarSubcontratacaoClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadSubcontratacao() {

    _subcontratacao = new Subcontratacao();
    KoBindings(_subcontratacao, "knockoutSubcontratacao");

    new BuscarClientes(_subcontratacao.Pessoa, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirSubcontratacaoClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Transportador, width: "45%" },
    { data: "PercentualDesconto", title: Localization.Resources.Fretes.TabelaFrete.PorcentagemDesconto, width: "20%" },
    { data: "PercentualCobranca", title: Localization.Resources.Fretes.TabelaFrete.PorcentagemCobranca, width: "20%" }];

    _gridSubcontratacao = new BasicDataTable(_subcontratacao.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridSubcontratacao();
}

function recarregarGridSubcontratacao() {
    _subcontratacao.PercentualCobrancaPadrao.val(_tabelaFrete.PercentualCobrancaPadrao.val());
    _subcontratacao.PercentualCobrancaVeiculoFrota.val(_tabelaFrete.PercentualCobrancaVeiculoFrota.val());
    var data = new Array();

    $.each(_tabelaFrete.Subcontratacoes.list, function (i, subcontratacao) {
        var subcontratacaoGrid = new Object();

        subcontratacaoGrid.Codigo = subcontratacao.Codigo.val;
        subcontratacaoGrid.Descricao = subcontratacao.Pessoa.val;
        subcontratacaoGrid.PercentualDesconto = subcontratacao.PercentualDesconto.val;
        subcontratacaoGrid.PercentualCobranca = subcontratacao.PercentualCobranca.val;
        data.push(subcontratacaoGrid);
    });

    _gridSubcontratacao.CarregarGrid(data);
}


function excluirSubcontratacaoClick(data) {
    for (var i = 0; i < _tabelaFrete.Subcontratacoes.list.length; i++) {
        if (data.Codigo == _tabelaFrete.Subcontratacoes.list[i].Codigo.val) {
            _tabelaFrete.Subcontratacoes.list.splice(i, 1);
            break;
        }
    }

    recarregarGridSubcontratacao();
}

function adicionarSubcontratacaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_subcontratacao);

    if (valido) {

        for (var i = 0; i < _tabelaFrete.Subcontratacoes.list.length; i++) {
            if (_tabelaFrete.Subcontratacoes.list[i].Pessoa.codEntity == _subcontratacao.Pessoa.codEntity()) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.TransportadorJaExistente, OTransportadorJaEstaCadastrado.format(_subcontratacao.Pessoa.val()));
                return;
            }
        }

        _tabelaFrete.Subcontratacoes.list.push(SalvarListEntity(_subcontratacao));

        recarregarGridSubcontratacao();

        $("#" + _subcontratacao.Pessoa.id).focus();

        limparCamposSubcontratacao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposSubcontratacao() {
    LimparCampos(_subcontratacao);
}