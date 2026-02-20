//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaGestaoCustoContabil;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaGestaoCustoContabil = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CentroCusto = PropertyEntity({ text: "Centro de custo", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true) });
    this.ContaContabil = PropertyEntity({ text: "Conta contábil", val: ko.observable(""), getType: typesKnockout.string, enable: ko.observable(true) });
    this.EnviadaAoEmail = PropertyEntity({ val: ko.observable(false), text: "Notificação ao transportador", val: ko.observable(""), getType: typesKnockout.bool, visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: salvarGestaoCustoContabil, visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoEtapaGestaoCustoContabil(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/GestaoCustoContabil.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaGestaoCustoContabil = new GestaoDevolucaoEtapaGestaoCustoContabil();
                KoBindings(_gestaoDevolucaoEtapaGestaoCustoContabil, "knockoutGestaoCustoContabil");

                PreencherObjetoKnout(_gestaoDevolucaoEtapaGestaoCustoContabil, { Data: r.Data });

                if (_gestaoDevolucaoEtapaGestaoCustoContabil.EnviadaAoEmail.val())
                    _gestaoDevolucaoEtapaGestaoCustoContabil.EnviadaAoEmail.visible(true);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoEtapaGestaoCustoContabil);

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
function salvarGestaoCustoContabil() {
    exibirConfirmacao("Confirmação", "Ao salvar os dados um email de confirmação será enviado ao transportador. Deseja continuar?", function () {
        let dados = {
            CodigoGestaoDevolucao: _gestaoDevolucaoEtapaGestaoCustoContabil.CodigoGestaoDevolucao.val(),
            CentroCusto: _gestaoDevolucaoEtapaGestaoCustoContabil.CentroCusto.val(),
            ContaContabil: _gestaoDevolucaoEtapaGestaoCustoContabil.ContaContabil.val()
        }
        executarReST("GestaoDevolucao/SalvarGestaoCustoContabil", dados, function (r) {
            if (r.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Gestão de custo e contábil salva com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });

    });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas