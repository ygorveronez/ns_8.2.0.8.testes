/// <reference path="FechamentoAgregado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCIOT.js" />
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFechamentoAgregadoCIOT;
var _etapa1SelecaoCIOT;

var Etapa1SelecaoCIOT = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(true), text: "Transportador:", idBtnSearch: guid() });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });
    this.SituacaoCIOT = PropertyEntity({ val: ko.observable(EnumSituacaoCIOT.Todos), options: EnumSituacaoCIOT.ObterOpcoesPesquisa(), def: EnumSituacaoCIOT.Todos });

    this.PesquisarCIOT = PropertyEntity({
        eventClick: function (e) {
            GridSelecaoCIOT();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarFechamentoAgregadoClick, type: types.event, text: ko.observable("Gerar Fechamento Agregado"), visible: ko.observable(true) });

    this.Transportador.val.subscribe(function (novoValor) {
        GridSelecaoCIOT();
    });
}

//*******EVENTOS*******

function LoadEtapaSelecaoCIOT() {
    _etapa1SelecaoCIOT = new Etapa1SelecaoCIOT();
    KoBindings(_etapa1SelecaoCIOT, "knockoutEtapa1SelecaoCIOT");

    // Busca componentes pesquisa
    BuscarClientes(_etapa1SelecaoCIOT.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);

    // Inicia grid de dados
    GridSelecaoCIOT();
}

function AdicionarFechamentoAgregadoClick(e, sender) {
    if (!ValidarCamposObrigatorios(_etapa1SelecaoCIOT)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Informe os campos obrigatórios!");
        return;
    }

    exibirConfirmacao("Gerar Fechamento de Agregado", "Deseja realmente gerar o fechamento de agregado para o CIOT selecionado?", function () {
        let dados = RetornarObjetoPesquisa(_etapa1SelecaoCIOT);

        dados.SelecionarTodos = _etapa1SelecaoCIOT.SelecionarTodos.val();

        if (dados.SelecionarTodos === false)
            dados.ListaCIOT = JSON.stringify(_gridFechamentoAgregadoCIOT.ObterMultiplosSelecionados().map(o => o.Codigo));
        else
            dados.ListaCIOT = JSON.stringify(_gridFechamentoAgregadoCIOT.ObterMultiplosNaoSelecionados().map(o => o.Codigo));


        executarReST("FechamentoAgregado/Adicionar", dados, function (arg) {
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

function CarregarDadosFechamentoAgregado(data) {
    _etapa1SelecaoCIOT.Adicionar.visible(false);
    _etapa1SelecaoCIOT.Transportador.visible(false);
    _etapa2Consolidacao.EncerrarCIOTAgregado.visible(true);
    _CRUDAFechamentoAgregado.Limpar.visible(true);

    _etapa1SelecaoCIOT.SituacaoCIOT.val(data.SituacaoCIOT);
    _etapa2Consolidacao.Consolidado.val(data.Consolidado);

    if (_etapa2Consolidacao.Consolidado.val()) {
        if (_etapa1SelecaoCIOT.SituacaoCIOT.val() == EnumSituacaoCIOT.Encerrado) {
            _etapa2Consolidacao.EncerrarCIOTAgregado.visible(false);
        }
    }

    CarregarDadosCampos(data);

    GridSelecaoCIOT();
    GridCIOTConsolidacao();
}

function CarregarDadosCampos(data) {
    _fechamentoAgregado.Codigo.val(data.Codigo);
    _fechamentoAgregado.Numero.val(data.Numero);
    _etapa2Consolidacao.ValorAdiantamento.val(data.ValorAdiantamento);
    _etapa2Consolidacao.ValorSaldo.val(data.ValorSaldo);
    _etapa2Consolidacao.ValorAcrescimo.val(data.ValorAcrescimo);
    _etapa2Consolidacao.ValorFreteBruto.val(data.ValorFreteBruto);
    _etapa2Consolidacao.ValorIRRF.val(data.ValorIRRF);
    _etapa2Consolidacao.ValorINSS.val(data.ValorINSS);
    _etapa2Consolidacao.ValorSESTSENAT.val(data.ValorSESTSENAT);
    _etapa2Consolidacao.ValorDesconto.val(data.ValorDesconto);
    _etapa2Consolidacao.ValorFreteLiquido.val(data.ValorFreteLiquido);
    _etapa2Consolidacao.ValorAbastecimento.val(data.ValorAbastecimento);
}

function GridSelecaoCIOT() {
    let menuOpcoes = null;

    let multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _etapa1SelecaoCIOT.SelecionarTodos,
        callbackNaoSelecionado: function () {
        },
        callbackSelecionado: function () {
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    if (_fechamentoAgregado.Codigo.val() > 0)
        multiplaescolha = null;

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaCIOT",
        titulo: "CIOTs para consolidação do fechamento de agregado",
        id: "btnExportarCIOT"
    };

    _etapa1SelecaoCIOT.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridFechamentoAgregadoCIOT = new GridView(_etapa1SelecaoCIOT.PesquisarCIOT.idGrid, "FechamentoAgregado/PesquisaCIOT", _etapa1SelecaoCIOT, menuOpcoes, null, 10, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridFechamentoAgregadoCIOT.SetPermitirRedimencionarColunas(true);
    _gridFechamentoAgregadoCIOT.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarCIOT").show();
            else
                $("#btnExportarCIOT").hide();
        }, 200);
    });
}