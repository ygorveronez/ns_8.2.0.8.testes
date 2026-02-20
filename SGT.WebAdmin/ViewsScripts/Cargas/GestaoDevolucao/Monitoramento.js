//#region Objetos Globais do Arquivo
var _gestaoDevolucaoEtapaMonitoramento;
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoEtapaMonitoramento = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.CodigoCargaDevolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.TipoCarga = PropertyEntity({ text: "Tipo de Carga", val: ko.observable(0) });
    this.DataColeta = PropertyEntity({ text: "Data da coleta", val: ko.observable(0) });
    this.Hora = PropertyEntity({ text: "Hora", val: ko.observable(0) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
}
//#endregion Classes

// #region Funções de Inicialização

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function loadGestaoDevolucaoEtapaMonitoramento(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/Monitoramento.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoEtapaMonitoramento = new GestaoDevolucaoEtapaMonitoramento();
                KoBindings(_gestaoDevolucaoEtapaMonitoramento, "knockoutMonitoramento");

                buscarDetalhesOperador(function () {
                    loadGestaoDevolucaoMonitoramentoControleEntrega(function () {
                        registraComponente();
                        loadEtapasControleEntrega();

                        _containerControleEntrega = new ContainerControleEntrega();
                        KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");

                        visualizarDetalhesEntregaClick();
                    });
                });

                PreencherObjetoKnout(_gestaoDevolucaoEtapaMonitoramento, r);

                $('#grid-devolucoes').hide();
                $('#container-principal').show();

                if (_inicioViagem && _gestaoDevolucaoGeracaoOcorrenciaDebito.SituacaoDevolucao.val() != 0) {
                    _inicioViagem.InformarInicioViagem?.visible(false);
                    _inicioViagem.InformarInicioViagemAlterar?.visible(false);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function visualizarDetalhesEntregaClick() {
    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: _gestaoDevolucaoEtapaMonitoramento.CodigoCargaDevolucao.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _containerControleEntrega.Entregas.val([arg.Data.Entregas]);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
function loadGestaoDevolucaoMonitoramentoControleEntrega(callback) {
    carregarHTMLComponenteControleEntrega(callback);
}
// #endregion Funções Privadas