/// <reference path="../../../Consultas/Produto.js" />
/// <reference path="NotaFiscal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaNotaFiscalProduto;
var _cargaNotaFiscalProduto;
var _CRUDCargaNotaFiscalProduto;

var CargaNotaFiscalProduto = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Quantidade = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Quantidade.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true, enable: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValorUnitario.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true, enable: ko.observable(true) });
    this.UnidadeMedida = PropertyEntity({ text: Localization.Resources.Cargas.Carga.UnidadeDeMedida.getRequiredFieldDescription(), maxlength: 6, required: true, enable: ko.observable(true) });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Produto.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
};

var CRUDCargaNotaFiscalProduto = function () {
    this.Atualizar = PropertyEntity({ eventClick: AtualizarCargaNotaFiscalProdutoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirCargaNotaFiscalProdutoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarCargaNotaFiscalProdutoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(PodeEditarValoresDaNota()) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCargaNotaFiscalProdutoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(PodeEditarValoresDaNota()) });
};

//*******EVENTOS*******

function LoadCargaNotaFiscalProduto() {

    _cargaNotaFiscalProduto = new CargaNotaFiscalProduto();
    KoBindings(_cargaNotaFiscalProduto, "knoutProdutosAdicionarNotasCarga");

    _CRUDCargaNotaFiscalProduto = new CRUDCargaNotaFiscalProduto();
    KoBindings(_CRUDCargaNotaFiscalProduto, "knoutCRUDProdutosAdicionarNotasCarga");

    new BuscarProdutos(_cargaNotaFiscalProduto.Produto);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarCargaNotaFiscalProdutoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Produto", title: Localization.Resources.Cargas.Carga.Produto, width: "45%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.Carga.Quantidade, width: "15%" },
        { data: "ValorUnitario", title: Localization.Resources.Cargas.Carga.ValorUnitario, width: "15%" },
        { data: "UnidadeMedida", title: Localization.Resources.Cargas.Carga.UN, width: "15%" }
    ];

    _gridCargaNotaFiscalProduto = new BasicDataTable(_cargaNotaFiscalProduto.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.desc });

    RecarregarGridCargaNotaFiscalProduto();
}

function RecarregarGridCargaNotaFiscalProduto() {
    var data = new Array();

    if (_notaFiscal.Produtos != undefined)
        $.each(_notaFiscal.Produtos.list, function (i, produto) {
            var produtoGrid = new Object();

            produtoGrid.Codigo = produto.Codigo.val;
            produtoGrid.Produto = produto.Produto.val;
            produtoGrid.Quantidade = produto.Quantidade.val;
            produtoGrid.ValorUnitario = produto.ValorUnitario.val;
            produtoGrid.UnidadeMedida = produto.UnidadeMedida.val;

            data.push(produtoGrid);
        });

    _gridCargaNotaFiscalProduto.CarregarGrid(data);
}

function AtualizarCargaNotaFiscalProdutoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cargaNotaFiscalProduto);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    for (var i = 0; i < _notaFiscal.Produtos.list.length; i++) {
        if (_cargaNotaFiscalProduto.Codigo.val() == _notaFiscal.Produtos.list[i].Codigo.val) {
            _notaFiscal.Produtos.list[i].Produto.codEntity = _cargaNotaFiscalProduto.Produto.codEntity();
            _notaFiscal.Produtos.list[i].Produto.val = _cargaNotaFiscalProduto.Produto.val();
            _notaFiscal.Produtos.list[i].Quantidade.val = _cargaNotaFiscalProduto.Quantidade.val();
            _notaFiscal.Produtos.list[i].ValorUnitario.val = _cargaNotaFiscalProduto.ValorUnitario.val();
            _notaFiscal.Produtos.list[i].UnidadeMedida.val = _cargaNotaFiscalProduto.UnidadeMedida.val();
            break;
        }
    }

    RecarregarGridCargaNotaFiscalProduto();
    LimparCamposCargaNotaFiscalProduto();
}

function ExcluirCargaNotaFiscalProdutoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteExcluirEsseProduto, function () {
        for (var i = 0; i < _notaFiscal.Produtos.list.length; i++) {
            if (_cargaNotaFiscalProduto.Codigo.val() == _notaFiscal.Produtos.list[i].Codigo.val) {
                _notaFiscal.Produtos.list.splice(i, 1);
                break;
            }
        }

        RecarregarGridCargaNotaFiscalProduto();
        LimparCamposCargaNotaFiscalProduto();
    });
}

function CancelarCargaNotaFiscalProdutoClick(e, sender) {
    LimparCamposCargaNotaFiscalProduto();
}

function EditarCargaNotaFiscalProdutoClick(data) {
    for (var i = 0; i < _notaFiscal.Produtos.list.length; i++) {
        if (data.Codigo == _notaFiscal.Produtos.list[i].Codigo.val) {
            var produtoNotaFiscal = _notaFiscal.Produtos.list[i];

            _cargaNotaFiscalProduto.Codigo.val(produtoNotaFiscal.Codigo.val);
            _cargaNotaFiscalProduto.Produto.codEntity(produtoNotaFiscal.Produto.codEntity);
            _cargaNotaFiscalProduto.Produto.val(produtoNotaFiscal.Produto.val);
            _cargaNotaFiscalProduto.Quantidade.val(produtoNotaFiscal.Quantidade.val);
            _cargaNotaFiscalProduto.ValorUnitario.val(produtoNotaFiscal.ValorUnitario.val);
            _cargaNotaFiscalProduto.UnidadeMedida.val(produtoNotaFiscal.UnidadeMedida.val);

            break;
        }
    }

    _CRUDCargaNotaFiscalProduto.Atualizar.visible(true);
    _CRUDCargaNotaFiscalProduto.Excluir.visible(true);
    _CRUDCargaNotaFiscalProduto.Cancelar.visible(true);
    _CRUDCargaNotaFiscalProduto.Adicionar.visible(false);
}

function AdicionarCargaNotaFiscalProdutoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_cargaNotaFiscalProduto);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _cargaNotaFiscalProduto.Codigo.val(guid());
    _notaFiscal.Produtos.list.push(SalvarListEntity(_cargaNotaFiscalProduto));

    RecarregarGridCargaNotaFiscalProduto();
    LimparCamposCargaNotaFiscalProduto();
}

function LimparCamposCargaNotaFiscalProduto() {
    LimparCampos(_cargaNotaFiscalProduto);
    _CRUDCargaNotaFiscalProduto.Atualizar.visible(false);
    _CRUDCargaNotaFiscalProduto.Excluir.visible(false);
    _CRUDCargaNotaFiscalProduto.Cancelar.visible(false);
    _CRUDCargaNotaFiscalProduto.Adicionar.visible(true);
}

function PodeEditarValoresDaNota() {
    return !(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
}