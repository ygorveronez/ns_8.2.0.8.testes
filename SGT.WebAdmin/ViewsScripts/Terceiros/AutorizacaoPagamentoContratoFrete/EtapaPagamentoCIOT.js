//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoCIOT;
var _etapaPagamentoCIOT;

var EtapaPagamentoCIOT = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.PesquisarPagamentoCIOT = PropertyEntity({
        eventClick: function (e) {
            GridPagamentoCIOT();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadEtapaPagamentoCIOT() {
    _etapaPagamentoCIOT = new EtapaPagamentoCIOT();
    KoBindings(_etapaPagamentoCIOT, "knockoutEtapaPagamentoCIOT");

    // Inicia grid de dados
    GridPagamentoCIOT();
}

//*******MÉTODOS*******

function GridPagamentoCIOT() {
    let linhasPorPaginas = 10;

    let opcaoIntegrar = {
        descricao: Localization.Resources.Gerais.Geral.Integrar,
        id: guid(),
        metodo: integrarPagamentoCIOTClick,
        icone: "",
        visibilidade: VisibilidadeIntegrarCIOT
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        tamanho: 10,
        opcoes: [opcaoIntegrar]
    };

    let configExportacao = {
        url: "AutorizacaoPagamentoContratoFrete/ExportarPesquisaPagamentoCIOT",
        titulo: "Pagamento parcelas CIOT",
        id: "btnExportarPagamentoCIOT"
    };

    _etapaPagamentoCIOT.Codigo.val(_autorizacaoPagamentoContratoFrete.Codigo.val());
    _gridPagamentoCIOT = new GridView(_etapaPagamentoCIOT.PesquisarPagamentoCIOT.idGrid, "AutorizacaoPagamentoContratoFrete/PesquisaPagamentoCIOT", _etapaPagamentoCIOT, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridPagamentoCIOT.SetPermitirRedimencionarColunas(true);
    _gridPagamentoCIOT.CarregarGrid(function () {
        setTimeout(function () {
            if (_autorizacaoPagamentoContratoFrete.Codigo.val() > 0)
                $("#btnExportarPagamentoCIOT").show();
            else
                $("#btnExportarPagamentoCIOT").hide();
        }, 200);
    });
}

function integrarPagamentoCIOTClick(registroSelecionado) {
    executarReST("AutorizacaoPagamentoContratoFrete/IntegrarPagamentoCIOT", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                GridPagamentoCIOT();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function VisibilidadeIntegrarCIOT(registro) {
    let opcoesIntegrar = [EnumSituacaoIntegracaoCarga.AgIntegracao, EnumSituacaoIntegracaoCarga.ProblemaIntegracao];

    return opcoesIntegrar.indexOf(parseInt(registro.Situacao)) != -1;
}