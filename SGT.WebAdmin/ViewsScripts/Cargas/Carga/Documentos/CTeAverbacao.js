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

//*******MAPEAMENTO*******

//*******EVENTOS*******

var _pesquisaHistoricoIntegracaoAverbacao;
var _gridHistoricoIntegracaoAverbacao;

var PesquisaHistoricoIntegracaoAverbacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

function buscarCargasCTeAverbacao() {
    var email = { descricao: Localization.Resources.Cargas.Carga.EmailProvedor, id: guid(), metodo: enviarEmailAverbacaoCTeClick, icone: "", visibilidade: VisibilidadeEnviarEmailProvedor };
    var averbar = { descricao: Localization.Resources.Cargas.Carga.Averbar, id: guid(), metodo: reemitirAverbacaoCTeClick, icone: "", visibilidade: VisibilidadeReemitirAverbacao };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("AverbacaoCTe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoAverbacao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [averbar, historico, auditar, email] };

    _gridCargaCTeAverbacao = new GridView(_cargaCTe.PesquisarCTeAverbacoes.idGrid, "CargaCTe/ConsultarCargaCTeAverbacao", _cargaCTe, menuOpcoes);
    _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);

    // Exibe botão de reenvio de averbações
    if (_cargaAtual.ProblemaAverbacaoCTe.val())
        _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
    else
        _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(false);
}

function callbackGridAverbacaoCTe(retorno) {
    if (_gridCargaCTeAverbacao.NumeroRegistros() > 0) {
        const emitirNovamenteAverbacoesPendentesCte = retorno.data.some(averbacao => averbacao.Status == EnumStatusAverbacaoCTe.Pendente);
        _cargaCTe.EmitirNovamenteAverbacoesPendentesCTe.visible(emitirNovamenteAverbacoesPendentesCte);

        $("#tabAverbacaoCTes_" + _cargaAtual.DadosCTes.id + "_li").show();
    }
    else
        $("#tabAverbacaoCTes_" + _cargaAtual.DadosCTes.id + "_li").hide();
}

function ExibirHistoricoIntegracaoAverbacao(integracao) {
    BuscarHistoricoIntegracaoAverbacao(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoAverbacao(integracao) {
    _pesquisaHistoricoIntegracaoAverbacao = new PesquisaHistoricoIntegracaoAverbacao();
    _pesquisaHistoricoIntegracaoAverbacao.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoAverbacao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoAverbacao = new GridView("tblHistoricoIntegracaoCTe", "CargaCTe/ConsultarHistoricoIntegracaoAverbacao", _pesquisaHistoricoIntegracaoAverbacao, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoAverbacao.CarregarGrid();
}

function LiberarCargaComAverbacoesRejeitadosClick(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaAverbacoesComRejeicao, function () {
        executarReST("CargaCTe/LiberarComProblemaAverbacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaCTe.LiberarCargaComAverbacoesRejeitados.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function DownloadArquivosHistoricoIntegracaoAverbacao(historicoConsulta) {
    executarDownload("CargaCTe/DownloadArquivosHistoricoIntegracaoAverbacao", { Codigo: historicoConsulta.Codigo });
}

//*******METODOS*******

function enviarEmailAverbacaoCTeClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo
    }
    executarReST("CargaCTe/EnviarEmailProvedor", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function reemitirAverbacaoCTeClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cargaAtual.Codigo.val()
    }

    executarReST("CargaCTe/AverbarCTe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function VisibilidadeEnviarEmailProvedor(datagrid) {
    return (datagrid.Status == EnumStatusAverbacaoCTe.Sucesso && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
}

function VisibilidadeReemitirAverbacao(datagrid) {
    return (datagrid.Status == EnumStatusAverbacaoCTe.Pendente || datagrid.Status == EnumStatusAverbacaoCTe.Rejeicao)
}

function reaverbarRejeitadosClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReaverbarTodasAsAverbacoesRejeitadasDestaCarga, function () {
        _cargaAtual.PossuiPendencia.val(false);
        EtapaCTeNFsAguardando(_cargaAtual);
        $("#" + _cargaAtual.EtapaCTeNFs.idGrid + " .DivMensagemAverbacaoCTe").hide();
        _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(false);

        var data = {
            Carga: _cargaAtual.Codigo.val()
        }

        executarReST("CargaCTe/ReaverbarRejeitadas", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function reaverbarPendentesClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReaverbarTodasAsAverbacoesPendentesDestaCarga, function () {
        var dados = {
            Carga: _cargaAtual.Codigo.val()
        }

        executarReST("CargaCTe/ReaverbarPendentes", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg, 20000);

                    _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}