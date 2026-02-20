/// <reference path="AutorizacaoPagamentoContratoFrete.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoIntegracao;
var _etapaPagamentoIntegracao;

var EtapaPagamentoIntegracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.PesquisarPagamentoIntegracao = PropertyEntity({
        eventClick: function (e) {
            GridPagamentoIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadEtapaPagamentoIntegracao() {
    _etapaPagamentoIntegracao = new EtapaPagamentoIntegracao();
    KoBindings(_etapaPagamentoIntegracao, "knockoutEtapaPagamentoIntegracao");

    // Inicia grid de dados
    GridPagamentoIntegracao();
}

//*******MÉTODOS*******

function GridPagamentoIntegracao() {
    let linhasPorPaginas = 5;

    let opcaoIntegrar = {
        descricao: Localization.Resources.Gerais.Geral.Integrar,
        id: guid(),
        metodo: integrarPagamentoClick,
        icone: "",
        visibilidade: VisibilidadeIntegrarContrato
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        tamanho: 10,
        opcoes: [opcaoIntegrar]
    };

    let configExportacao = {
        url: "AutorizacaoPagamentoContratoFrete/ExportarPesquisaPagamentoIntegracao",
        titulo: "Pagamento Integrações",
        id: "btnExportarIntegracao"
    };

    _etapaPagamentoIntegracao.Codigo.val(_autorizacaoPagamentoContratoFrete.Codigo.val());
    _gridPagamentoIntegracao = new GridView(_etapaPagamentoIntegracao.PesquisarPagamentoIntegracao.idGrid, "AutorizacaoPagamentoContratoFrete/PesquisaPagamentoIntegracao", _etapaPagamentoIntegracao, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridPagamentoIntegracao.SetPermitirRedimencionarColunas(true);
    _gridPagamentoIntegracao.CarregarGrid(function () {
        setTimeout(function () {
            if (_autorizacaoPagamentoContratoFrete.Codigo.val() > 0)
                $("#btnExportarPagamentoIntegracao").show();
            else
                $("#btnExportarPagamentoIntegracao").hide();
        }, 200);
    });
}

function integrarPagamentoClick(registroSelecionado) {
    executarReST("AutorizacaoPagamentoContratoFrete/IntegrarPagamentoContrato", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                GridPagamentoIntegracao();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function VisibilidadeIntegrarContrato(registro) {
    let opcoesIntegrar = [EnumSituacaoIntegracaoCarga.AgIntegracao, EnumSituacaoIntegracaoCarga.ProblemaIntegracao];

    return opcoesIntegrar.indexOf(parseInt(registro.Situacao)) != -1;
}