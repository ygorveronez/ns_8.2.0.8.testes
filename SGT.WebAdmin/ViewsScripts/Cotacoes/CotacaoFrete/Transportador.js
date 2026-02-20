/// <reference path="../../Consultas/Produto.js" />
/// <reference path="CotacaoFrete.js" />
/// <reference path="DetalheCotacaoFreteTransportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _transportador;
var _CRUDTransportador;
var _gridTransportador;

var Transportador = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Transportadores = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });

    this.Grid = PropertyEntity({ type: types.local });
};

var CRUDTransportador = function () {
    this.GerarCotacao = PropertyEntity({ eventClick: GerarCotacaoClick, type: types.event, text: "Gerar Cotação", visible: ko.observable(true) });
    this.Voltar = PropertyEntity({ eventClick: VoltarClick, type: types.event, text: "Voltar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadTransportador() {
    _transportador = new Transportador();
    KoBindings(_transportador, "knockoutTransportador");

    _CRUDTransportador = new CRUDTransportador();
    KoBindings(_CRUDTransportador, "knockoutCRUDTransportador");

    LoadGridTransportador();
}

function GerarCotacaoClick() {
    if (_gridTransportador.ListaSelecionados().length == 0)
        return exibirMensagem(tipoMensagem.atencao, "Seleção transportadores", "Necessário selecionar um transportador!");

    if (_gridTransportador.ListaSelecionados().length != 1)
        return exibirMensagem(tipoMensagem.atencao, "Seleção transportadores", "Permitido selecionar apenas um transportador!");

    exibirConfirmacao("Confirmação", "Realmente deseja gerar a cotação para o Transportador selecionado?", function () {
        var transportadorEscolhido = _gridTransportador.ListaSelecionados()[0];

        var cotacaoEnviar = RetornarObjetoPesquisa(_cotacaoFrete);
        cotacaoEnviar["TransportadorEscolhido"] = JSON.stringify(transportadorEscolhido);

        executarReST("CotacaoFrete/GerarCotacao", cotacaoEnviar, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cotação gerada com sucesso");

                    _gridCotacaoFrete.CarregarGrid();

                    LimparCamposCotacaoFrete();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function VoltarClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja voltar para alterar as informações?", function () {
        SetarEtapaInicioCotacaoFrete();
    });
}

////*******MÉTODOS*******

function LoadGridTransportador() {

    let detalhes = { descricao: 'Detalhes', id: guid(), evento: "onclick", metodo: DetalhesCotacaoFreteTransportadorClick, tamanho: "10", icone: "" };
    let menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [detalhes] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Transportador", title: "Transportador", width: "40%" },
        { data: "DataPrevisaoColeta", title: "Data Coleta Prevista", width: "15%" },
        { data: "DataPrazoEntrega", title: "Data Prazo Entrega", width: "15%" },
        { data: "PrazoEntrega", title: "Prazo Entrega", width: "10%" },
        { data: "ValorFrete", title: "Valor Frete", width: "10%" },
        { data: "ValorCotacao", title: "Valor Cotação", width: "10%" },
        { data: "DistanciaRaioKM", title: "Distância Raio KM", width: "10%" },
        { data: "CanalEntrega", title: "Canal Entrega", width: "10%" },
        { data: "RetornoCompleto", visible: false }
    ];

    var configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: false };
    _gridTransportador = new BasicDataTable(_transportador.Grid.id, header, menuOpcoes, null, configRowsSelect);

    RecarregarGridTransportador();
}

function RecarregarGridTransportador() {
    var data = new Array();

    $.each(_transportador.Transportadores.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Transportador.CNPJ;
        itemGrid.Transportador = item.Transportador.Descricao;
        itemGrid.PrazoEntrega = item.PrazoEntrega;
        itemGrid.ValorFrete = item.ValorFrete;
        itemGrid.DataPrazoEntrega = item.DataPrazoEntrega;
        itemGrid.DataPrevisaoColeta = item.DataPrevisaoColeta;
        itemGrid.ValorCotacao = item.ValorCotacao;
        itemGrid.DistanciaRaioKM = item.DistanciaRaioKM;
        itemGrid.RetornoCompleto = item.RetornoCompleto;
        itemGrid.CanalEntrega = item.CanalEntrega;

        data.push(itemGrid);
    });

    _gridTransportador.CarregarGrid(data);
}

function LimparCamposTransportador() {
    LimparCampos(_transportador);

    RecarregarGridTransportador();
}