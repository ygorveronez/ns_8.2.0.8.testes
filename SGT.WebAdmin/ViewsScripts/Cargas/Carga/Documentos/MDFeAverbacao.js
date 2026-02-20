/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
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

//*******MAPEAMENTO*******

var _pesquisaHistoricoIntegracaoAverbacao, _gridHistoricoIntegracaoAverbacao;

var PesquisaHistoricoIntegracaoAverbacaoMDFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function BuscarCargaMDFeAverbacao() {
    var averbar = { descricao: Localization.Resources.Cargas.Carga.Averbar, id: guid(), metodo: ReemitirAverbacaoMDFeClick, icone: "", visibilidade: VisibilidadeReemitirAverbacaoMDFe };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("AverbacaoMDFe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoAverbacaoMDFe, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [averbar, historico, auditar] };

    _gridCargaMDFeAverbacao = new GridView(_cargaMDFe.PesquisarMDFeAverbacoes.idGrid, "CargaMDFe/ConsultarCargaMDFeAverbacao", _cargaMDFe, menuOpcoes);
    _gridCargaMDFeAverbacao.CarregarGrid();

    if (_cargaAtual.ProblemaAverbacaoMDFe.val())
        _cargaMDFe.EmitirNovamenteAverbacoesMDFe.visible(true);
    else
        _cargaMDFe.EmitirNovamenteAverbacoesMDFe.visible(false);
}

function ExibirHistoricoIntegracaoAverbacaoMDFe(integracao) {
    BuscarHistoricoIntegracaoAverbacaoMDFe(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoAverbacaoMDFe(integracao) {
    _pesquisaHistoricoIntegracaoAverbacao = new PesquisaHistoricoIntegracaoAverbacao();
    _pesquisaHistoricoIntegracaoAverbacao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAverbacaoMDFe, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoAverbacao = new GridView("tblHistoricoIntegracaoCTe", "CargaMDFe/ConsultarHistoricoIntegracaoAverbacao", _pesquisaHistoricoIntegracaoAverbacao, menuOpcoes, { column: 2, dir: orderDir.desc });
    _gridHistoricoIntegracaoAverbacao.CarregarGrid();
}

function LiberarCargaComAverbacoesMDFeRejeitadosClick(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    };

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaAverbacoesComRejeicao, function () {
        executarReST("CargaMDFe/LiberarComProblemaAverbacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaMDFe.LiberarCargaComAverbacoesMDFeRejeitados.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function DownloadArquivosHistoricoIntegracaoAverbacaoMDFe(historicoConsulta) {
    executarDownload("CargaMDFe/DownloadArquivosHistoricoIntegracaoAverbacao", { Codigo: historicoConsulta.Codigo });
}

//*******METODOS*******

function ReemitirAverbacaoMDFeClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cargaAtual.Codigo.val()
    };

    executarReST("CargaMDFe/AverbarMDFe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaMDFeAverbacao.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function VisibilidadeReemitirAverbacaoMDFe(datagrid) {
    return (datagrid.Status == EnumStatusAverbacaoMDFe.Pendente || datagrid.Status == EnumStatusAverbacaoCTe.Rejeicao);
}

function ReaverbarRejeitadosMDFeClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReaverbarTodasAsAverbacoesDeMdfeRejeitadasDestaCarga, function () {

        _cargaAtual.PossuiPendencia.val(false);

        EtapaMDFeAguardando(_cargaAtual);

        $("#" + _cargaAtual.EtapaMDFe.idGrid + " .DivMensagemAverbacaoMDFe").hide();
        _cargaMDFe.EmitirNovamenteAverbacoesMDFe.visible(false);

        var data = {
            Carga: _cargaAtual.Codigo.val()
        };

        executarReST("CargaMDFe/ReaverbarRejeitadas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaMDFeAverbacao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function PreencherMotivoPendenciaAverbacaoMDFe(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "") {
        var html = "<div class='alert alert-info alert-block'>";
        html += "<h4 class='alert-heading'>" + Localization.Resources.Cargas.Carga.Pendencia + "</h4>";
        html += knoutCarga.MotivoPendencia.val();
        html += "</div>";

        $("#" + knoutCarga.EtapaMDFe.idGrid + " .MensagemAverbacaoMDFe").html(html);
        $("#" + knoutCarga.EtapaMDFe.idGrid + " .DivMensagemAverbacaoMDFe").show();
    }

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCarga)) {
        _cargaMDFe.LiberarCargaComAverbacoesRejeitados.visible(true);
        _cargaMDFe.LiberarCargaComAverbacoesRejeitados.enable(true);
    }
}

function RecarregarGridProblemaAverbacaoMDFeSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaMDFeAverbacao != null) {
        if ($("#" + _cargaAtual.EtapaMDFe.idGrid).is(":visible")) {
            PreencherMotivoPendenciaAverbacaoMDFe(knoutCarga);
            _gridCargaMDFeAverbacao.CarregarGrid();// nesse caso atualiza a grid de mdfes pois pode ser que o usuário esteja visualizando essa tela

            if (_cargaMDFe != null)
                _cargaMDFe.EmitirNovamenteAverbacoesMDFe.visible(true);
        }
    }
    EtapaMDFeProblema(knoutCarga);
}