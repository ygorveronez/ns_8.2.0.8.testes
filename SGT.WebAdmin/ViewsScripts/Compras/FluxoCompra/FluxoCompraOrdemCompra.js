/// <reference path="../OrdemCompra/OrdemCompra.js" />
/// <reference path="FluxoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fluxoCompraOrdemCompra;
var _CRUDFluxoCompraOrdemCompra;
var _CRUDOrdemCompraDados;
var _gridFluxoCompraOrdemCompra;

var FluxoCompraOrdemCompra = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.OrdensCompra = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDFluxoCompraOrdemCompra = function () {
    this.FinalizarOrdemCompra = PropertyEntity({ eventClick: FinalizarOrdemCompraClick, type: types.event, text: "Finalizar Ordens de Compras", visible: ko.observable(true) });
};

var CRUDOrdemCompraDados = function () {
    this.Salvar = PropertyEntity({ type: types.event, eventClick: SalvarOrdemCompraDadosClick, text: "Salvar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: CancelarOrdemCompraDadosClick, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadFluxoCompraOrdemCompra() {
    _fluxoCompraOrdemCompra = new FluxoCompraOrdemCompra();
    KoBindings(_fluxoCompraOrdemCompra, "knockoutFluxoCompraOrdemCompra");

    _CRUDFluxoCompraOrdemCompra = new CRUDFluxoCompraOrdemCompra();
    KoBindings(_CRUDFluxoCompraOrdemCompra, "knockoutCRUDFluxoCompraOrdemCompra");

    _CRUDOrdemCompraDados = new CRUDOrdemCompraDados();
    KoBindings(_CRUDOrdemCompraDados, "knockoutCRUDOrdemCompraDados");

    LoadGridFluxoCompraOrdemCompra();
    carregarLancamentoOrdemCompra("conteudoOrdemCompra");
}

function FinalizarOrdemCompraClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar as ordens de compras e avançar para as aprovações?", function () {
        executarReST("FluxoCompraOrdemCompra/FinalizarOrdemCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.AprovacaoOrdemCompra);
                    controleCamposFluxoCompraOrdemCompra();
                    controleCamposAprovacaoOrdemCompra();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem(ns) de Compra(s) finalizadas com sucesso!");

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

function SalvarOrdemCompraDadosClick() {
    Salvar(_ordemCompra, "OrdemCompra/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                LimparCamposOrdemCompra();
                CarregarFluxoCompraOrdemCompra();
                Global.fecharModal('divModalOrdemCompraDados');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function CancelarOrdemCompraDadosClick() {    
    Global.fecharModal('divModalOrdemCompraDados');
}

////*******MÉTODOS*******

function CarregarFluxoCompraOrdemCompra() {
    executarReST("FluxoCompraOrdemCompra/BuscarOrdensCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_fluxoCompraOrdemCompra, r);
                RecarregarGridFluxoCompraOrdemCompra();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LoadGridFluxoCompraOrdemCompra() {

    var selecionar = { descricao: "Selecionar", id: guid(), metodo: SelecionarFluxoCompraOrdemCompraClick, icone: "" };
    var imprimirOrdemCompra = { descricao: "Imprimir", id: guid(), metodo: ImprimirCompraOrdemCompraClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [selecionar, imprimirOrdemCompra] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "7%" },
        { data: "Fornecedor", title: "Fornecedor", width: "20%" },
        { data: "Data", title: "Data", width: "10%" },
        { data: "DataPrevisaoRetorno", title: "Data Prev", width: "10%" },
        { data: "Situacao", title: "Situação", width: "10%" },
        { data: "ValorTotal", title: "Valor Total", width: "10%" }
    ];

    _gridFluxoCompraOrdemCompra = new BasicDataTable(_fluxoCompraOrdemCompra.Grid.id, header, menuOpcoes);

    RecarregarGridFluxoCompraOrdemCompra();
}

function ImprimirCompraOrdemCompraClick(itemGrid) {
    executarDownload("OrdemCompra/Imprimir", { Codigo: itemGrid.Codigo });
}

function SelecionarFluxoCompraOrdemCompraClick(data) {
    LimparCamposOrdemCompra();

    _ordemCompra.Codigo.val(data.Codigo);

    BuscarPorCodigo(_ordemCompra, "OrdemCompra/BuscarPorCodigo", function (arg) {
        if (arg.Data != null) {
            CarregarProdutosDaOrdem(arg.Data.Produtos);

            if (_ordemCompra.Situacao.val() === EnumSituacaoOrdemCompra.Aberta) {
                ControleCamposOrdemCompra(true);
                _CRUDOrdemCompraDados.Salvar.visible(true);
            } else {
                ControleCamposOrdemCompra(false);
                _CRUDOrdemCompraDados.Salvar.visible(false);
            }

            Global.abrirModal("divModalOrdemCompraDados");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RecarregarGridFluxoCompraOrdemCompra() {
    var data = new Array();

    $.each(_fluxoCompraOrdemCompra.OrdensCompra.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Numero = item.Numero.val;
        itemGrid.Fornecedor = item.Fornecedor.val;
        itemGrid.Data = item.Data.val;
        itemGrid.DataPrevisaoRetorno = item.DataPrevisaoRetorno.val;
        itemGrid.Situacao = item.Situacao.val;
        itemGrid.ValorTotal = item.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridFluxoCompraOrdemCompra.CarregarGrid(data);
}

function controleCamposFluxoCompraOrdemCompra() {
    _CRUDFluxoCompraOrdemCompra.FinalizarOrdemCompra.visible(false);

    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.OrdemCompra && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDFluxoCompraOrdemCompra.FinalizarOrdemCompra.visible(true);
    }
}

function LimparCamposFluxoCompraOrdemCompra() {
    _CRUDFluxoCompraOrdemCompra.FinalizarOrdemCompra.visible(true);

    LimparCampos(_fluxoCompraOrdemCompra);

    LimparCamposOrdemCompra();
}