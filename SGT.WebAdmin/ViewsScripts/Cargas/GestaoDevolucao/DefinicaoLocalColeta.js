/// <reference path="../../Consultas/Cliente.js" />

//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaDefinicaoLocalColeta;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaDefinicaoLocalColeta = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Pesquisar Cliente", visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: salvarDefinicaoLocalColeta, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaDefinicaoLocalColeta(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/DefinicaoLocalColeta.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaDefinicaoLocalColeta = new GestaoDevolucaoEtapaDefinicaoLocalColeta();
                KoBindings(_gestaoDevolucaoEtapaDefinicaoLocalColeta, "knockoutDefinicaoLocalColeta");

                BuscarClientes(_gestaoDevolucaoEtapaDefinicaoLocalColeta.Cliente);

                PreencherObjetoKnout(_gestaoDevolucaoEtapaDefinicaoLocalColeta, r);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaDefinicaoLocalColeta);

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
function salvarDefinicaoLocalColeta() {
    let dados = {
        CodigoGestaoDevolucao: _gestaoDevolucaoEtapaDefinicaoLocalColeta.CodigoGestaoDevolucao.val(),
        Cliente: _gestaoDevolucaoEtapaDefinicaoLocalColeta.Cliente.codEntity()
    }
    executarReST("GestaoDevolucao/SalvarLocalColeta", dados, function (r) {
        if (r.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Local da coleta definido com sucesso.");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas