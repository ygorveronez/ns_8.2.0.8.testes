// #region Objetos Globais do Arquivo
var _gestaoDevolucaoGeracaoCargaDevolucao
// #endregion Objetos Globais do Arquivo

//#region Classes
var GestaoDevolucaoGeracaoCargaDevolucao = function () {
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0) });
    this.CodigoCargaDevolucao = PropertyEntity({ val: ko.observable(0) });
    this.SituacaoCargaJanelaDescarregamento = PropertyEntity({ val: ko.observable("-"), visible: ko.observable(false) });
    this.CorSituacaoCargaJanelaDescarregamento = PropertyEntity({ val: ko.observable("") });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

    this.Atualizar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Atualizar, eventClick: buscarDadosCarga, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: cancelarGeracaoCargaDevolucao, visible: ko.observable(true), enable: ko.observable(true) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadGestaoDevolucaoGeracaoCargaDevolucao(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/GeracaoCargaDevolucao.html?dyn=" + guid(), function (html) {
                $("#container-principal-content").html(html);

                _gestaoDevolucaoGeracaoCargaDevolucao = new GestaoDevolucaoGeracaoCargaDevolucao();
                KoBindings(_gestaoDevolucaoGeracaoCargaDevolucao, "knockoutGeracaoCargaDevolucao");

                PreencherObjetoKnout(_gestaoDevolucaoGeracaoCargaDevolucao, r);

                controlarAcoesContainerPrincipal(etapa, _gestaoDevolucaoGeracaoCargaDevolucao);

                if (_gestaoDevolucaoGeracaoCargaDevolucao.SituacaoCargaJanelaDescarregamento.val() != '-') {
                    _gestaoDevolucaoGeracaoCargaDevolucao.SituacaoCargaJanelaDescarregamento.visible(true);
                    $("#gestaoDevolucaoSituacaoCargaJanelaDescarregamento").css("color", _gestaoDevolucaoGeracaoCargaDevolucao.CorSituacaoCargaJanelaDescarregamento.val());
                }

                if (_gestaoDevolucaoGeracaoCargaDevolucao.CodigoCargaDevolucao.val() > 0) {
                    carregarConteudosHTML(function () {
                        buscarDadosCarga();
                    });
                }

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

// #endregion Funções de Inicialização

// #region Funções Privadas
function buscarDadosCarga() {
    executarReST("Carga/BuscarCargaPorCodigo", { Carga: _gestaoDevolucaoGeracaoCargaDevolucao.CodigoCargaDevolucao.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html("");
                GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function cancelarGeracaoCargaDevolucao() {
    LimparCampos(_gestaoDevolucaoGeracaoCargaDevolucao);
    $("#container-principal-content").html("");
    mostrarGridDevolucoes();
}
// #endregion Funções Privadas