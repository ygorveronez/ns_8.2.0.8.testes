/// <reference path="FechamentoAgregado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoIntegracaoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridIntegracao;
var _etapa3Integracao;

var Etapa3Integracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: "", issue: 272 });

    this.PesquisarIntegracao = PropertyEntity({
        eventClick: function (e) {
            GridIntegracao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadEtapaIntegracao() {
    _etapa3Integracao = new Etapa3Integracao();
    KoBindings(_etapa3Integracao, "knockoutEtapa3Integracao");

    // Inicia grid de dados
    GridIntegracao();
}

//*******MÉTODOS*******

function GridIntegracao() {
    return;

    let linhasPorPaginas = 5;

    let opcaoIntegrar = {
        descricao: Localization.Resources.Gerais.Geral.Integrar,
        id: guid(),
        metodo: integrarClick,
        icone: "",
        visibilidade: VisibilidadeIntegrar
    };

    let menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        tamanho: 10,
        opcoes: [opcaoIntegrar]
    };

    let configExportacao = {
        url: "FechamentoAgregado/ExportarPesquisaIntegracao",
        titulo: "Integrações",
        id: "btnExportarIntegracao"
    };

    _etapa3Integracao.Codigo.val(_fechamentoAgregado.Codigo.val());
    _gridIntegracao = new GridView(_etapa3Integracao.PesquisarIntegracao.idGrid, "FechamentoAgregado/PesquisaIntegracao", _etapa3Integracao, menuOpcoes, null, linhasPorPaginas, null, null, null, null, null, null, configExportacao);
    _gridIntegracao.SetPermitirRedimencionarColunas(true);
    _gridIntegracao.CarregarGrid(function () {
        setTimeout(function () {
            if (_fechamentoAgregado.Codigo.val() > 0)
                $("#btnExportarIntegracao").show();
            else
                $("#btnExportarIntegracao").hide();
        }, 200);
    });
}

function integrarClick(registroSelecionado) {
    executarReST("FechamentoAgregado/Integrar", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                GridIntegracao();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function VisibilidadeIntegrar(registro) {
    let opcoesIntegrar = [EnumSituacaoIntegracaoCarga.AgIntegracao, EnumSituacaoIntegracaoCarga.ProblemaIntegracao];

    return opcoesIntegrar.indexOf(parseInt(registro.Situacao)) != -1;
}