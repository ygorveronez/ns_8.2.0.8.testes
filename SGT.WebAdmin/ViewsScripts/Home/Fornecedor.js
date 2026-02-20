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

var _fornecedor;

var Fornecedor = function () {
    this.TextoAviso = ko.observable();
    this.Anexos = ko.observableArray([]);
    this.DownloadAnexo = downloadAnexoClick;
};

//*******EVENTOS*******

function LoadHomeFornecedor() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor || !_CONFIGURACAO_TMS.LayoutAmarelo)
        return;

    _fornecedor = new Fornecedor();
    KoBindings(_fornecedor, "knockoutAvisoFornecedor");
    buscarAvisoFornecedor();
    buscarTotalAgendamentosPendentes();
}

function downloadAnexoClick(event, codigo) {
    if (event && event.preventDefault) event.preventDefault();

    executarDownload("Home/DownloadAnexoFornecedor", { Codigo: codigo });

    return false;
}

//*******MÉTODOS*******

function buscarAvisoFornecedor() {
    executarReST("Home/ObterAvisoFornecedor", {}, function (response) {
        if (!response.Success) {
            exibirMensagem(tipoMensagem.falha, "Falha", response.Msg);
            return;
        }

        if (typeof response.Data != "object")
            return;

        _fornecedor.TextoAviso(response.Data.TextoAviso);
        if (response.Data.Anexos != undefined) {
            _fornecedor.Anexos(response.Data.Anexos);

            if (response.Data.TextoAviso || response.Data.Anexos.length > 0)
                $("#knockoutAvisoFornecedor").removeClass("d-none");
        }
    }, null, false);
}

function buscarTotalAgendamentosPendentes() {
    executarReST("RetiradaProduto/ObterTotalAgendamentosPendentes", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data > 0) {
                $("#mensagem-aviso-agendamentos-pendentes-container").show();
                $("#opcao-excluir-agendamentos-pendentes-tela-inicial").show();
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirAgendamentosPendentes() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir todos os agendamentos pendentes?", function () {
        executarReST("RetiradaProduto/ExcluirAgendamentosPendentes", {}, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.TotalAgendamentosPendentesRestantes == 0) {
                        $("#mensagem-aviso-agendamentos-pendentes-container").hide();
                        $("#opcao-excluir-agendamentos-pendentes-tela-inicial").hide();
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Todos os agendamentos pendentes foram excluídos com sucesso");
                    }
                    else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Data.TotalAgendamentosPendentesExcluidos + " de " + retorno.Data.TotalAgendamentosPendentes + " agendamentos pendentes foram excluídos com sucesso");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}
