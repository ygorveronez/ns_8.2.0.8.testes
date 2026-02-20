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
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="GrupoPessoas.js" />

var _grupoPessoasFornecedor;
var _gridTabelaMultiplosVencimentos;

var _parcelas = [
    { value: 1, text: "1X" },
    { value: 2, text: "2X" },
    { value: 3, text: "3X" },
    { value: 4, text: "4X" },
    { value: 5, text: "5X" },
    { value: 6, text: "6X" },
    { value: 7, text: "7X" },
    { value: 8, text: "8X" },
    { value: 9, text: "9X" },
    { value: 10, text: "10X" },
    { value: 11, text: "11X" },
    { value: 12, text: "12X" },
    { value: 13, text: "13X" },
    { value: 14, text: "14X" },
    { value: 15, text: "15X" },
    { value: 16, text: "16X" },
    { value: 17, text: "17X" },
    { value: 18, text: "18X" },
    { value: 19, text: "19X" },
    { value: 20, text: "20X" },
    { value: 21, text: "21X" },
    { value: 22, text: "22X" },
    { value: 23, text: "23X" },
    { value: 24, text: "24X" },
    { value: 25, text: "25X" },
    { value: 26, text: "26X" },
    { value: 27, text: "27X" },
    { value: 28, text: "28X" },
    { value: 29, text: "29X" },
    { value: 30, text: "30X" },
    { value: 31, text: "31X" },
    { value: 32, text: "32X" },
    { value: 33, text: "33X" },
    { value: 34, text: "34X" },
    { value: 35, text: "35X" },
    { value: 36, text: "36X" },
    { value: 37, text: "37X" },
    { value: 38, text: "38X" },
    { value: 39, text: "39X" },
    { value: 40, text: "40X" },
    { value: 41, text: "41X" },
    { value: 42, text: "42X" },
    { value: 43, text: "43X" },
    { value: 44, text: "44X" },
    { value: 45, text: "45X" },
    { value: 46, text: "46X" },
    { value: 47, text: "47X" },
    { value: 48, text: "48X" },
    { value: 49, text: "49X" },
    { value: 50, text: "50X" }
];

var TabelaMultiplosVencimentosMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.Vencimento = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DataEmissao = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DiaEmissaoInicial = PropertyEntity({ type: types.entity, val: ko.observable("") });
    this.DiaEmissaoFinal = PropertyEntity({ type: types.entity, val: ko.observable("") });
};

//*******MAPEAMENTO KNOUCKOUT*******

var GrupoPessoasFornecedor = function () {
    this.FormaTituloFornecedor = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.FormaTitulo.getFieldDescription(), val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), def: EnumFormaTitulo.Outros, visible: ko.observable(true) });

    //Duplicata automatica
    this.GerarDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.GrupoPessoas.GerarDuplicataAutomaticamente, def: false, visible: ko.observable(true) });
    this.ParcelasDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(1), options: _parcelas, def: 1, text: Localization.Resources.Pessoas.GrupoPessoas.QuantidadeParcelas.getRequiredFieldDescription(), required: false, visible: ko.observable(true) });
    this.IntervaloDiasDuplicataNotaEntrada = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IntervaloDias.getRequiredFieldDescription(), required: false, maxlength: 500, getType: typesKnockout.string, visible: ko.observable(true) });
    this.DiaPadraoDuplicataNotaEntrada = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Pessoas.GrupoPessoas.DiaVencimento.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 2 });
    this.IgnorarDuplicataRecebidaXMLNotaEntrada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.GrupoPessoas.IgnorarDuplicatasRecebidasXML, def: false, visible: ko.observable(true) });

    //Multiplos Vencimentos
    this.PermitirMultiplosVencimentos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.GrupoPessoas.MultiplosDocumentosDataFixa, def: false, visible: ko.observable(true) });
    this.TabelaMultiplosVencimentos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), text: "", idGrid: guid(), visible: ko.observable(true) });
    this.UtilizarParametrizacaoDeHorariosNoAgendamento = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false), text: Localization.Resources.Pessoas.GrupoPessoas.UtilizarParametrizacaoHorarios });

    this.DiaEmissaoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DiaInicial.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: true, maxlength: 2 });
    this.DiaEmissaoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DiaFinal.getRequiredFieldDescription(), val: ko.observable(""), getType: typesKnockout.int, visible: ko.observable(true), required: true, maxlength: 2 });
    this.Vencimento = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.int, def: "", text: Localization.Resources.Pessoas.GrupoPessoas.Vencimento.getRequiredFieldDescription(), required: true, visible: ko.observable(true), maxlength: 2 });

    this.AdicionarVencimento = PropertyEntity({ eventClick: adicionarTabelaMultiplosVencimentosClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTabelaMultiplosVencimentosClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false) });
    this.MultiploVencimentoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadGrupoPessoasFornecedor() {
    _grupoPessoasFornecedor = new GrupoPessoasFornecedor();
    KoBindings(_grupoPessoasFornecedor, "knockoutFornecedor");

    loadTabelaMultiplosVencimentos();
}

function PreencherDadosGrupoPessoasFornecedor(dados) {
    PreencherObjetoKnout(_grupoPessoasFornecedor, { Data: dados });
}

function adicionarTabelaMultiplosVencimentosClick() {
    if (!ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.DiaEmissaoInicial) || !ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.DiaEmissaoFinal) || !ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.Vencimento)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    var obj = new TabelaMultiplosVencimentosMap();

    obj.Codigo.val = guid();
    obj.Vencimento.val = _grupoPessoasFornecedor.Vencimento.val();
    obj.DiaEmissaoInicial.val = _grupoPessoasFornecedor.DiaEmissaoInicial.val();
    obj.DiaEmissaoFinal.val = _grupoPessoasFornecedor.DiaEmissaoFinal.val();
    obj.DataEmissao.val = obterDataFormatada(_grupoPessoasFornecedor.DiaEmissaoInicial, _grupoPessoasFornecedor.DiaEmissaoFinal);

    _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list.push(obj);
    LimparCamposTabelaVencimentos();
}

function editarTabelaMultiplosVencimentosClick(data) {
    _grupoPessoasFornecedor.AdicionarVencimento.visible(false);
    _grupoPessoasFornecedor.Atualizar.visible(true);
    _grupoPessoasFornecedor.DiaEmissaoInicial.val(data.DiaEmissaoInicial);
    _grupoPessoasFornecedor.MultiploVencimentoCodigo.val(data.Codigo);
    _grupoPessoasFornecedor.DiaEmissaoFinal.val(data.DiaEmissaoFinal);
    _grupoPessoasFornecedor.Vencimento.val(data.Vencimento);
}

function excluirTabelaMultiplosVencimentosClick(data) {
    var novaLista = new Array();

    $.each(_grupoPessoasFornecedor.TabelaMultiplosVencimentos.list, function (i, Tabela) {
        if (Tabela.Codigo.val != data.Codigo) {
            novaLista.push(Tabela);
        }
    });

    _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list = novaLista;
    recarregarGridTabelaMultiplosVencimentos();
}

function atualizarTabelaMultiplosVencimentosClick() {
    if (!ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.Vencimento) || !ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.DiaEmissaoInicial) || !ValidarCampoObrigatorioMap(_grupoPessoasFornecedor.DiaEmissaoFinal)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    for (let i = 0; i < _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list.length; i++) {
        if (_grupoPessoasFornecedor.TabelaMultiplosVencimentos.list[i].Codigo.val == _grupoPessoasFornecedor.MultiploVencimentoCodigo.val()) {
            _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list[i].Vencimento.val = _grupoPessoasFornecedor.Vencimento.val();
            _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list[i].DiaEmissaoInicial.val = _grupoPessoasFornecedor.DiaEmissaoInicial.val();
            _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list[i].DiaEmissaoFinal.val = _grupoPessoasFornecedor.DiaEmissaoFinal.val();
            _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list[i].DataEmissao.val = obterDataFormatada(_grupoPessoasFornecedor.DiaEmissaoInicial, _grupoPessoasFornecedor.DiaEmissaoFinal);
        }
    }
    LimparCamposTabelaVencimentos();
    _grupoPessoasFornecedor.AdicionarVencimento.visible(true);
    _grupoPessoasFornecedor.Atualizar.visible(false);
    _grupoPessoasFornecedor.MultiploVencimentoCodigo.val(0);
}

//*******MÉTODOS*******

function loadTabelaMultiplosVencimentos() {
    var excluir = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), evento: "onclick", metodo: excluirTabelaMultiplosVencimentosClick, tamanho: "15", icone: "" };
    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: editarTabelaMultiplosVencimentosClick, tamanho: "15", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Pessoas.GrupoPessoas.Opcoes, tamanho: 10, opcoes: [editar, excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DiaEmissaoFinal", visible: false },
        { data: "DiaEmissaoInicial", visible: false },
        { data: "DataEmissao", title: Localization.Resources.Pessoas.GrupoPessoas.DataEmissao, width: "80%", className: "text-align-left" },
        { data: "Vencimento", title: Localization.Resources.Pessoas.GrupoPessoas.Vencimento, width: "20%", className: "text-align-center" }
    ];
    _gridTabelaMultiplosVencimentos = new BasicDataTable(_grupoPessoasFornecedor.TabelaMultiplosVencimentos.idGrid, header, menuOpcoes);
    recarregarGridTabelaMultiplosVencimentos();
}

function recarregarGridTabelaMultiplosVencimentos() {
    var dataArray = new Array();

    $.each(_grupoPessoasFornecedor.TabelaMultiplosVencimentos.list, function (i, tabela) {

        var obj = new Object();
        obj.Codigo = tabela.Codigo.val;
        obj.DataEmissao = tabela.DataEmissao.val;
        obj.DiaEmissaoInicial = tabela.DiaEmissaoInicial.val;
        obj.DiaEmissaoFinal = tabela.DiaEmissaoFinal.val;
        obj.Vencimento = tabela.Vencimento.val;

        dataArray.push(obj);
    });

    _gridTabelaMultiplosVencimentos.CarregarGrid(dataArray);
}

function obterDataFormatada(datainicial, datafinal) {
    return datainicial.val() + " até " + datafinal.val();
}

function LimparCamposTabelaVencimentos() {
    recarregarGridTabelaMultiplosVencimentos();
    _grupoPessoasFornecedor.Vencimento.val(0);
    _grupoPessoasFornecedor.DiaEmissaoFinal.val("");
    _grupoPessoasFornecedor.DiaEmissaoInicial.val("");
}

function LimparCamposGrupoPessoasFornecedor() {
    LimparCampos(_grupoPessoasFornecedor);
}