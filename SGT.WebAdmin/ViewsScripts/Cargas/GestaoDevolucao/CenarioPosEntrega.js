//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaPosEntrega;
var _CRUDGestaODevolucaoEtapaPosEntrega;
var _GRIDGestaODevolucaoEtapaPosEntrega;
var opcoesAnaliseGestaoDevolucaoPosEntrega = [
    { text: "Procedente", value: true },
    { text: "Improcedente", value: false }
]
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaPosEntrega = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Analise = PropertyEntity({ text: "Análise", val: ko.observable(true), options: opcoesAnaliseGestaoDevolucaoPosEntrega, def: true, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observações da Análise", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.local });

    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmar, eventClick: salvarAnaliseCenarioPosEntrega, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: cancelarAnaliseCenarioPosEntrega, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaPosEntrega(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/CenarioPosEntrega.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaPosEntrega = new GestaoDevolucaoEtapaPosEntrega();
                KoBindings(_gestaoDevolucaoEtapaPosEntrega, "knockoutGestaoDevolucaoEtapaPosEntrega");

                PreencherObjetoKnout(_gestaoDevolucaoEtapaPosEntrega, r);
                loadGridGestaoDevolucaoIntegracao(_gestaoDevolucaoEtapaPosEntrega.Grid.id, _gestaoDevolucaoEtapaPosEntrega.CodigoGestaoDevolucao.val(), etapa.etapa);
                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaPosEntrega);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function salvarAnaliseCenarioPosEntrega() {
    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaPosEntrega.CodigoGestaoDevolucao.val(),
        Analise: _gestaoDevolucaoEtapaPosEntrega.Analise.val(),
        Observacao: _gestaoDevolucaoEtapaPosEntrega.Observacao.val()
    }
    executarReST("GestaoDevolucao/SalvarAnaliseCenarioPosEntrega", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Análise do cenário Pós Entrega salva com sucesso.");
            setTimeout(function () {
                _gridGestaoDevolucaoDevolucoes.CarregarGrid();
                limparEtapasDevolucao();
            }, 1000);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function cancelarAnaliseCenarioPosEntrega() {
    LimparCampos(_gestaoDevolucaoEtapaPosEntrega);
    limparEtapasDevolucao();
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas