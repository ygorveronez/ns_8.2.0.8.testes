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
/// <reference path="../../../Enumeradores/EnumStatusAverbacaoCTe.js" />
/// <reference path="CTeAverbacao.js" />

var _pesquisaHistoricoIntegracaoCancelamentoAverbacao, _gridHistoricoIntegracaoCancelamentoAverbacao;

//*******MAPEAMENTO*******

var PesquisaHistoricoIntegracaoCancelamentoAverbacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function buscarCargasCTeAverbacao() {
    var averbar = { descricao: Localization.Resources.Cargas.CancelamentoCarga.CancelarAverbacao, id: guid(), metodo: reemitirAverbacaoCTeClick, icone: "", visibilidade: VisibilidadeReemitirAverbacao };
    var historicoIntegracao = { descricao: Localization.Resources.Cargas.CancelamentoCarga.HistoricoIntegraccao, id: guid(), metodo: ExibirHistoricoIntegracaoCancelamentoAverbacaoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [averbar, historicoIntegracao] };

    _gridCargaCTeAverbacao = new GridView(_cte.BuscarAverbacoes.idGrid, "CargaCTe/ConsultarCargaCTeAverbacao", _cte, menuOpcoes);
    _gridCargaCTeAverbacao.CarregarGrid(
        function () {
            if (_gridCargaCTeAverbacao.NumeroRegistros() > 0) {
                $("#liAverbacaoCTes").show();
            } else {
                $("#liAverbacaoCTes").hide();
            }
        }
    );
}

function BuscarAverbacoesCTesCargaClick() {
    _gridCargaCTeAverbacao.CarregarGrid();
}

function LiberarCancelamentoComAverbacaoCTeRejeitadaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.CancelamentoCarga.LiberarCancelamentoCTe, function () {
        executarReST("CancelamentoCargaCTe/LiberarAverbacaoRejeitada", { Codigo: _cancelamento.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CancelamentoLiberado);
                    BuscarCancelamentoPorCodigo(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

//*******METODOS*******

function reemitirAverbacaoCTeClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cancelamento.Carga.codEntity()
    };

    executarReST("CargaCTe/CancelarAvervacaoCTe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTeAverbacao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function VisibilidadeReemitirAverbacao(datagrid) {
    return datagrid.Status == EnumStatusAverbacaoCTe.Sucesso;
}

function ExibirHistoricoIntegracaoCancelamentoAverbacaoClick(integracao) {
    BuscarHistoricoIntegracaoCancelamentoAverbacao(integracao);
    Global.abrirModal("divModalHistoricoCancelamentoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoCancelamentoAverbacao(integracao) {
    _pesquisaHistoricoIntegracaoCancelamentoAverbacao = new PesquisaHistoricoIntegracaoCancelamentoAverbacao();
    _pesquisaHistoricoIntegracaoCancelamentoAverbacao.Codigo.val(integracao.Codigo);

    var download = {
        descricao: Localization.Resources.Gerais.Geral.VerDetalhes.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCancelamentoAverbacao, tamanho: "20", icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCancelamentoAverbacao = new GridView("tblHistoricoIntegracaoCancelamentoCTe", "CargaCTe/ConsultarHistoricoIntegracaoCancelamentoAverbacao", _pesquisaHistoricoIntegracaoCancelamentoAverbacao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoCancelamentoAverbacao.CarregarGrid();
}

function DownloadArquivosHistoricoIntegracaoCancelamentoAverbacao(historicoConsulta) {
    executarDownload("CargaCTe/DownloadArquivosHistoricoIntegracaoCancelamentoAverbacao", { Codigo: historicoConsulta.Codigo });
}