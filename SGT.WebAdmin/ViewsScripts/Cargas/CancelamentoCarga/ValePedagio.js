/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Enumeradores/EnumStatusValePedagio.js" />
/// <reference path="ValePedagio.js" />

//*******MAPEAMENTO*******

var _pesquisaHistoricoIntegracaoValePedagio;
var _gridHistoricoIntegracaoValePedagio;

var PesquisaHistoricoIntegracaoValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******


var _gridCargaValePedagio;

function buscarCargasValePedagio() {
    var cancelarValePedagio = { descricao: Localization.Resources.Cargas.CancelamentoCarga.CancelarValePedagio, id: guid(), metodo: reemitirValePedagioClick, icone: "", visibilidade: VisibilidadeReemitirCancelamentoValepedagio };
    var historico = { descricao: Localization.Resources.Cargas.CancelamentoCarga.HistoricoIntegraccao, id: guid(), metodo: ExibirHistoricoIntegracaoValePedagio, icone: "" };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaIntegracaoValePedagio"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [historico, cancelarValePedagio, auditar] };

    _gridCargaValePedagio = new GridView(_cte.BuscarValePedagio.idGrid, "CargaIntegracaoValePedagio/ConsultarCargaValePedagio", _cte, menuOpcoes);
    _gridCargaValePedagio.CarregarGrid(
        function () {
            if (_gridCargaValePedagio.NumeroRegistros() > 0) {
                $("#liValePedagio").show();
            } else {
                $("#liValePedagio").hide();
            }
        }
    );
}

function BuscarValePedagioCargaClick() {
    _gridCargaValePedagio.CarregarGrid();
}

//*******METODOS*******
function reemitirValePedagioClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cancelamento.Carga.codEntity()
    }

    executarReST("CargaIntegracaoValePedagio/CancelarValePedagio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaValePedagio.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function VisibilidadeReemitirCancelamentoValepedagio(datagrid) {
    return (datagrid.SituacaoValePedagio == EnumSituacaoValePedagio.Confirmada);
}


function ExibirHistoricoIntegracaoValePedagio(integracao) {
    BuscarHistoricoIntegracaoValePedagio(integracao);
    Global.abrirModal("divModalHistoricoCancelamentoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoValePedagio(integracao) {
    _pesquisaHistoricoIntegracaoValePedagio = new PesquisaHistoricoIntegracaoValePedagio();
    _pesquisaHistoricoIntegracaoValePedagio.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoValePedagio, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoValePedagio = new GridView("tblHistoricoIntegracaoCancelamentoCTe", "CargaIntegracaoValePedagio/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoValePedagio, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoValePedagio.CarregarGrid();
}


function DownloadArquivosHistoricoIntegracaoValePedagio(historicoConsulta) {
    executarDownload("CargaIntegracaoValePedagio/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function LiberarCancelamentoComValePedagioRejeitadoClick() {
    exibirConfirmacao("Atenção!", Localization.Resources.Cargas.CancelamentoCarga.LiberarCancelamentoValePedagio, function () {
        executarReST("CancelamentoCargaCTe/LiberarValePedagioRejeitado", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", Localization.Resources.Gerais.Geral.CancelamentoLiberado);
                    BuscarCancelamentoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}