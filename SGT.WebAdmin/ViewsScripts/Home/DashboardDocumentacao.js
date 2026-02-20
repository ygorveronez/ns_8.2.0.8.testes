/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../Configuracao/Sistema/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _dashboardDocumentacao;
var _idDashboardDocumentacao;
var _caminhoDashboardDocumentacao;

var DashboardDocumentacao = function () {
    this.QuantidadeNavioFechado = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.QuantidadeNavioAberto = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.DataInicial = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: ":", getType: typesKnockout.string, val: ko.observable("") });
    this.Acessar = PropertyEntity({ eventClick: AcessarDashboardDocumentacaoClick, type: types.event, text: "Acessar o Dashboard", visible: ko.observable(true) });
};


//*******EVENTOS*******

function LoadDashboardDocumentacao() {
    _dashboardDocumentacao = new DashboardDocumentacao();
    KoBindings(_dashboardDocumentacao, "knockoutDashboardDocumentacao");
    _idDashboardDocumentacao = "";
    _caminhoDashboardDocumentacao = "";

    BuscarDashboardDocumentacao();
}

function AcessarDashboardDocumentacaoClick(e, sender) {
    executarReST("Dashboard/CriarLinkAcessoDashboard", { idDashboardDocumentacao: _idDashboardDocumentacao, caminhoDashboardDocumentacao: _caminhoDashboardDocumentacao }, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false) {
                if (r.Data.URL != "") {
                    openInNewTab(r.Data.URL);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Dashboardo não disponível");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}

//*******MÉTODOS*******

function openInNewTab(href) {
    Object.assign(document.createElement('a'), {
        target: '_blank',
        rel: 'noopener noreferrer',
        href: href,
    }).click();
}

function BuscarDashboardDocumentacao() {
    executarReST("Dashboard/BuscarDashboardDocumentacao", {}, function (r) {
        if (r.Success) {
            if (r.Data != null && r.Data !== false) {
                if (r.Data.PossuiAcessoAoDashboardDocumentacao) {
                    _dashboardDocumentacao.QuantidadeNavioFechado.val(r.Data.QuantidadeNavioFechado);
                    _dashboardDocumentacao.QuantidadeNavioAberto.val(r.Data.QuantidadeNavioAberto);
                    _dashboardDocumentacao.DataInicial.val(r.Data.DataInicial);
                    _dashboardDocumentacao.DataFinal.val(r.Data.DataFinal);
                    _idDashboardDocumentacao = r.Data.IDDashboardDocumentacao;
                    _caminhoDashboardDocumentacao = r.Data.CaminhoDashboardDocumentacao;

                    $("#knockoutDashboardDocumentacao").show();
                }
                else
                    $("#knockoutDashboardDocumentacao").hide();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, null, false);
}