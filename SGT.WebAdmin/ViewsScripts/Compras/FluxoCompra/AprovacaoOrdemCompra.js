/// <reference path="../AutorizacaoOrdemCompra/AutorizacaoOrdemCompra.js" />
/// <reference path="FluxoCompra.js" />
/// <reference path="FluxoCompraTratativa.js" />
/// <reference path="../../Enumeradores/EnumTratativaFluxoCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aprovacaoOrdemCompra;
var _CRUDAprovacaoOrdemCompra;
var _gridAprovacaoOrdemCompra;

var AprovacaoOrdemCompra = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.OrdensCompra = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDAprovacaoOrdemCompra = function () {
    this.AprovarFluxoCompra = PropertyEntity({ eventClick: AprovarFluxoCompraClick, type: types.event, text: "Aprovar Fluxo de Compra", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadAprovacaoOrdemCompra() {
    _aprovacaoOrdemCompra = new AprovacaoOrdemCompra();
    KoBindings(_aprovacaoOrdemCompra, "knockoutAprovacaoOrdemCompra");

    _CRUDAprovacaoOrdemCompra = new CRUDAprovacaoOrdemCompra();
    KoBindings(_CRUDAprovacaoOrdemCompra, "knockoutCRUDAprovacaoOrdemCompra");

    LoadGridAprovacaoOrdemCompra();

    carregarModalDetalhesOrdemCompra("ModalDetalhesOrdemCompra");
}

function AprovarFluxoCompraClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja aprovar o Fluxo de Compra?", function () {
        executarReST("FluxoCompraOrdemCompra/AprovarFluxoCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {

                    ConcluirTratativa();
                    _fluxoCompra.EtapaAtual.val(EnumEtapaFluxoCompra.RecebimentoProduto);
                    controleCamposAprovacaoOrdemCompra();
                    controleCamposRecebimentoProduto();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo de compra aprovado com sucesso!");

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

////*******MÉTODOS*******

function CarregarAprovacaoOrdemCompra() {
    executarReST("FluxoCompraOrdemCompra/BuscarOrdensCompra", { Codigo: _fluxoCompra.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_aprovacaoOrdemCompra, r);
                RecarregarGridAprovacaoOrdemCompra();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LoadGridAprovacaoOrdemCompra() {

    let selecionar = { descricao: "Selecionar", id: guid(), metodo: detalharAutorizacaoOrdemCompra, icone: "" };
    let tratativa = { descricao: "Tratativas", id: guid(), metodo: SelecionarFluxoCompraTratativaClick, icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [selecionar, tratativa] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "7%" },
        { data: "Fornecedor", title: "Fornecedor", width: "20%" },
        { data: "Data", title: "Data", width: "10%" },
        { data: "DataPrevisaoRetorno", title: "Data Prev", width: "10%" },
        { data: "Situacao", title: "Situação", width: "10%" },
        { data: "SituacaoTratativa", title: "Tratativa", width: "10%" },
        { data: "ValorTotal", title: "Valor Total", width: "10%" }
    ];

    _gridAprovacaoOrdemCompra = new BasicDataTable(_aprovacaoOrdemCompra.Grid.id, header, menuOpcoes);

    RecarregarGridAprovacaoOrdemCompra();
}

function RecarregarGridAprovacaoOrdemCompra() {
    let data = new Array();

    $.each(_aprovacaoOrdemCompra.OrdensCompra.list, function (i, item) {
        let itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.Numero = item.Numero.val;
        itemGrid.Fornecedor = item.Fornecedor.val;
        itemGrid.Data = item.Data.val;
        itemGrid.DataPrevisaoRetorno = item.DataPrevisaoRetorno.val;
        itemGrid.Situacao = item.Situacao.val;
        itemGrid.SituacaoTratativa = EnumTratativaFluxoCompra.obterDescricao(item.SituacaoTratativa.val);
        itemGrid.ValorTotal = item.ValorTotal.val;

        data.push(itemGrid);
    });

    _gridAprovacaoOrdemCompra.CarregarGrid(data);
}

function controleCamposAprovacaoOrdemCompra() {
    _CRUDAprovacaoOrdemCompra.AprovarFluxoCompra.visible(false);

    if (_fluxoCompra.EtapaAtual.val() === EnumEtapaFluxoCompra.AprovacaoOrdemCompra && _fluxoCompra.Situacao.val() === EnumSituacaoFluxoCompra.Aberto) {
        _CRUDAprovacaoOrdemCompra.AprovarFluxoCompra.visible(true);
    }
}

function LimparCamposAprovacaoOrdemCompra() {
    _CRUDAprovacaoOrdemCompra.AprovarFluxoCompra.visible(true);

    LimparCampos(_aprovacaoOrdemCompra);
}

function ConcluirTratativa() {

    $.each(_aprovacaoOrdemCompra.OrdensCompra.list, function (i, item) {

        if (EnumTratativaFluxoCompra.Concluido != item.SituacaoTratativa.val) {
            ConcluirTratativaAutomaticamente(item.Codigo.val, true);
        }

    });
}