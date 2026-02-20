//*******MAPEAMENTO KNOUCKOUT*******

var _etapa2Consolidacao;
var _gridCIOTConsolidacao;

var Etapa2Consolidacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Consolidado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.bool });

    this.ValorAdiantamento = PropertyEntity({ text: "(+) Valor Adiantamento:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorSaldo = PropertyEntity({ text: "(+) Valor Saldo:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorAcrescimo = PropertyEntity({ text: ko.observable("(+) Valor Acréscimo:"), getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorAbastecimento = PropertyEntity({ text: "(+) Valor Abastecimento:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorFreteBruto = PropertyEntity({ text: "Valor Frete Bruto:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorIRRF = PropertyEntity({ text: "(-) Valor IRRF:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorINSS = PropertyEntity({ text: "(-) Valor INSS:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorSESTSENAT = PropertyEntity({ text: "(-) Valor SESTSENAT:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorDesconto = PropertyEntity({ text: ko.observable("(-) Valor Desconto:"), getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });
    this.ValorFreteLiquido = PropertyEntity({ text: "Valor Frete Líquido:", getType: typesKnockout.decimal, required: false, visible: ko.observable(true) });

    this.PesquisarCIOTConsolidacao = PropertyEntity({
        eventClick: function (e) {
            GridCIOTConsolidacao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.EncerrarCIOTAgregado = PropertyEntity({ eventClick: EncerrarCIOTAgregadoClick, type: types.event, text: ko.observable("Encerrar CIOT Agregado"), visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadEtapaConsolidacao() {
    _etapa2Consolidacao = new Etapa2Consolidacao();
    KoBindings(_etapa2Consolidacao, "knockoutEtapa2Consolidacao");

    // Inicia grid de dados
}

function EncerrarCIOTAgregadoClick(e, sender) {
    exibirConfirmacao("Encerrar CIOT Agregado", "Deseja realmente encerrar o CIOT selecionado?", function () {
        let dados = RetornarObjetoPesquisa(_etapa2Consolidacao);

        executarReST("FechamentoAgregado/EncerrarCIOTAgregado", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento de agregado gerado com sucesso!");
                    CarregarDadosFechamentoAgregado(arg.Data);
                    SetarEtapasFechamentoAgregado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function GridCIOTConsolidacao() {
    let linhasPorPaginas = 10;

    //-- Cabecalho
    let detalhes = {
        descricao: "Detalhes",
        id: guid(),
        evento: "onclick",
        metodo: AbrirModalDetalhes,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    let acrescimodesconto = {
        descricao: "Acréscimo e Desconto",
        id: guid(),
        evento: "onclick",
        metodo: AbrirModalAcrescimoDesconto,
        tamanho: "10",
        icone: "",
        visibilidade: true
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [detalhes, acrescimodesconto ]
    };

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaCIOTConsolidacao",
        titulo: "CIOTs",
        id: "btnExportarCIOTConsolidacao"
    };

    _etapa2Consolidacao.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridCIOTConsolidacao = new GridView(_etapa2Consolidacao.PesquisarCIOTConsolidacao.idGrid, "FechamentoAgregado/PesquisaCIOTConsolidacao", _etapa2Consolidacao, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridCIOTConsolidacao.SetPermitirRedimencionarColunas(true);
    _gridCIOTConsolidacao.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarCIOTConsolidacao").show();
            else
                $("#btnExportarCIOTConsolidacao").hide();
        }, 200);
    });
}

