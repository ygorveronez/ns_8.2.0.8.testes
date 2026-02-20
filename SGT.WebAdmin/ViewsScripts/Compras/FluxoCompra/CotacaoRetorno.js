/// <reference path="FluxoCompra.js" />
/// <reference path="Cotacao.js" />
/// <reference path="CotacaoRetornoProdutoFornecedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cotacaoRetorno;
var _CRUDCotacaoRetorno;
var _gridCotacaoMercadoriaRetorno;

var CotacaoRetorno = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Grid = PropertyEntity({ type: types.local });

    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.QuantidadeOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorUnitarioOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.ValorTotalItemOriginal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Mercadorias = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.Fornecedores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.Retornos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDCotacaoRetorno = function () {
    this.SalvarCotacaoRetorno = PropertyEntity({ eventClick: SalvarCotacaoRetornoClick, type: types.event, text: "Salvar Retorno Preenchido", visible: ko.observable(true) });
    this.GerarOrdemCompra = PropertyEntity({ eventClick: GerarOrdemCompraClick, type: types.event, text: "Gerar Ordem de Compra para os fornecedores selecionados", visible: ko.observable(true) });
    this.VoltarParaCotacao = PropertyEntity({ eventClick: VoltarParaCotacaoClick, type: types.event, text: "Voltar para Cotação", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCotacaoRetorno() {
    _cotacaoRetorno = new CotacaoRetorno();
    KoBindings(_cotacaoRetorno, "knockoutCotacaoRetorno");

    _CRUDCotacaoRetorno = new CRUDCotacaoRetorno();
    KoBindings(_CRUDCotacaoRetorno, "knockoutCRUDCotacaoRetorno");

    LoadCotacaoRetornoProdutoFornecedor();

    LoadGridCotacaoMercadoriaRetorno();
}

function SalvarCotacaoRetornoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja salvar os dados de retorno alterados manualmente?", function () {
        Salvar(_cotacaoRetorno, "FluxoCompraCotacao/AtualizarRetornoCotacao", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Retorno da cotação atualizada com sucesso!");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function GerarOrdemCompraClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja gerar as ordens de compras?", function () {
        executarReST("FluxoCompraOrdemCompra/GerarOrdemCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.OrdemCompra);
                    controleCamposCotacaoRetornoFluxoCompra();
                    controleCamposFluxoCompraOrdemCompra();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem(ns) de Compra(s) gerada com sucesso!");

                    RecarregarGridPesquisa();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function VoltarParaCotacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja voltar para a etapa de Cotação para adicionar mais fornecedores?", function () {
        executarReST("FluxoCompraCotacao/VoltarParaCotacao", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.Cotacao);
                    _fluxoCompra.VoltouParaEtapaAtual.val(true);

                    controleCamposCotacaoRetornoFluxoCompra();
                    controleCamposCotacaoFluxoCompra();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Voltado para etapa de cotação com sucesso!");

                    RecarregarGridPesquisa();
                    Etapa4DesabilitadaFluxoCompra();
                    SetarEtapaFluxoCompra();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

////*******MÉTODOS*******

function CarregarCotacaoRetornoFluxoCompra() {
    executarReST("FluxoCompraCotacao/BuscarRetornoCotacaoPorCodigo", { Codigo: _cotacao.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _cotacaoRetorno.Codigo.val(_cotacao.Codigo.val());
                PreencherObjetoKnout(_cotacaoRetorno, r);
                RecarregarGridCotacaoMercadoriaRetorno();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LoadGridCotacaoMercadoriaRetorno() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Selecionar", id: guid(), metodo: SelecionarCotacaoMercadoriaRetornoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoProduto", visible: false },
        { data: "Produto", title: "Produto", width: "40%" },
        { data: "Quantidade", title: "Qtd.", width: "10%" },
        { data: "ValorUnitario", title: "Val. Unit.", width: "10%" },
        { data: "ValorTotal", title: "Val. Total", width: "10%" }
    ];

    _gridCotacaoMercadoriaRetorno = new BasicDataTable(_cotacaoRetorno.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCotacaoMercadoriaRetorno();
}

function SelecionarCotacaoMercadoriaRetornoClick(data) {
    LimparCamposRetornoProdutoFornecedor();
    Global.abrirModal("divRetornoMercadoria");

    _cotacaoRetorno.QuantidadeOriginal.val(data.Quantidade);
    _cotacaoRetorno.ValorUnitarioOriginal.val(data.ValorUnitario);
    _cotacaoRetorno.ValorTotalItemOriginal.val(data.ValorTotal);
    _cotacaoRetorno.Produto.codEntity(data.CodigoProduto);
    _cotacaoRetorno.Produto.val(data.Produto);

    RecarregarGridRetornoProdutoFornecedor();

    if (_fluxoCompra.EtapaAtual.val() !== EnumEtapaFluxoCompra.RetornoCotacao || _fluxoCompra.Situacao.val() !== EnumSituacaoFluxoCompra.Aberto)
        _gridRetornoProdutoFornecedor.DesabilitarOpcoes();
}

function RecarregarGridCotacaoMercadoriaRetorno() {
    var data = new Array();

    $.each(_cotacaoRetorno.Mercadorias.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.CodigoProduto = item.Produto.codEntity;
        itemGrid.Produto = item.Produto.val;
        itemGrid.Quantidade = item.Quantidade.val;
        itemGrid.ValorUnitario = item.ValorUnitario.val;
        itemGrid.ValorTotal = item.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridCotacaoMercadoriaRetorno.CarregarGrid(data);
}

function controleCamposCotacaoRetornoFluxoCompra() {
    _CRUDCotacaoRetorno.SalvarCotacaoRetorno.visible(false);
    _CRUDCotacaoRetorno.GerarOrdemCompra.visible(false);
    _CRUDCotacaoRetorno.VoltarParaCotacao.visible(false);

    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.RetornoCotacao && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDCotacaoRetorno.SalvarCotacaoRetorno.visible(true);
        _CRUDCotacaoRetorno.GerarOrdemCompra.visible(true);
        _CRUDCotacaoRetorno.VoltarParaCotacao.visible(true);

        SetarEnableCamposKnockout(_retornoProdutoFornecedor, true);
    } else
        SetarEnableCamposKnockout(_retornoProdutoFornecedor, false);
}

function LimparCamposCotacaoRetornoFluxoCompra() {
    _CRUDCotacaoRetorno.SalvarCotacaoRetorno.visible(true);
    _CRUDCotacaoRetorno.GerarOrdemCompra.visible(true);

    LimparCampos(_cotacaoRetorno);

    SetarEnableCamposKnockout(_retornoProdutoFornecedor, true);
    _gridRetornoProdutoFornecedor.HabilitarOpcoes();
}