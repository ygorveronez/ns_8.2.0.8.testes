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
/// <reference path="CTe.js" />
/// <reference path="PreCTe.js" />

var _pesquisaHistoricoIntegracaoValePedagio;
var _gridHistoricoIntegracaoValePedagio;

var PesquisaHistoricoIntegracaoValePedagio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function buscarCargasValePedagio() {
    var reenviarIntegracao = { descricao: Localization.Resources.Cargas.Carga.Integrar, id: guid(), metodo: integrarValePedagioClick, icone: "", visibilidade: VisibilidadeReemitirIntegracaoValePedagio };
    var imprimirPortal = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: ImpressaoValePedagioViaPortalClick, icone: "", visibilidade: VisibilidadeReemitirIntegracaoValePedagioViaPortal };
    var imprimir = { descricao: Localization.Resources.Cargas.Carga.Imprimir, id: guid(), metodo: ImpressaoValePedagioClick, icone: "", visibilidade: VisibilidadeImprimirIntegracaoValePedagio };
    var cancelarValePedagioNaCarga = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: CancelarValePedagioNaCargaClick, icone: "", visibilidade: VisibilidadeCancelarValePedagioNaCarga };
    var historico = { descricao: Localization.Resources.Cargas.Carga.HistoricoDeIntegracao, id: guid(), metodo: ExibirHistoricoIntegracaoValePedagio, icone: "", visibilidade: VisibilidadeHistoricoIntegracao };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaIntegracaoValePedagio"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    _cargaCTe.SituacaoIntegracaoValePedagio.visible(true);

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [imprimir, imprimirPortal, reenviarIntegracao, cancelarValePedagioNaCarga, historico, auditar] };

    var isProblemaValePedagio = _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe && (_cargaAtual.ProblemaIntegracaoValePedagio.val() || _cargaAtual.LiberadoComProblemaValePedagio.val() || _cargaAtual.NaoPermitirLiberarSemValePedagio.val());

    if (_isPreCte) {
        _gridCargaIntegracaoValePedagioPreCTe = new GridView(_cargaCTe.PesquisarIntegracaoValePedagioPreCte.idGrid, "CargaIntegracaoValePedagio/ConsultarCargaValePedagio", _cargaCTe, menuOpcoes);
        _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid(function () { callbackGridValePedagioPreCte(isProblemaValePedagio) });

        habilitarTabIntegracaoValePedagio(false);

        if (isProblemaValePedagio) {
            _cargaCTe.LiberarComProblemaValePedagioPreCte.visible(true);

            if (_cargaCTe.LiberarComProblemaValePedagioPreCte.val())
                _cargaCTe.LiberarComProblemaValePedagioPreCte.enable(false);
            else
                _cargaCTe.LiberarComProblemaValePedagioPreCte.enable(true);
        } else {
            _cargaCTe.LiberarComProblemaValePedagioPreCte.visible(false);
        }
    } else {
        _gridCargaIntegracaoValePedagio = new GridView(_cargaCTe.PesquisarIntegracaoValePedagio.idGrid, "CargaIntegracaoValePedagio/ConsultarCargaValePedagio", _cargaCTe, menuOpcoes);
        _gridCargaIntegracaoValePedagio.CarregarGrid(function () { callbackGridValePedagio(isProblemaValePedagio) });

        habilitarTabIntegracaoValePedagioPreCte(false);

        if (isProblemaValePedagio) {
            _cargaCTe.LiberarComProblemaValePedagio.visible(true);

            _cargaCTe.LiberarSemValePedagio.visible(true);
            _cargaCTe.LiberarSemValePedagio.enable(!_cargaAtual.LiberadoComProblemaValePedagio.val() || _cargaAtual.NaoPermitirLiberarSemValePedagio.val());

            if (_cargaAtual.LiberadoComProblemaValePedagio.val())
                _cargaCTe.LiberarComProblemaValePedagio.enable(false);
            else
                _cargaCTe.LiberarComProblemaValePedagio.enable(true);
        } else {
            _cargaCTe.LiberarComProblemaValePedagio.visible(false);
            _cargaCTe.LiberarSemValePedagio.visible(false);
        }
    }
}

function callbackGridValePedagio(isProblemaValePedagio) {
    habilitarTabIntegracaoValePedagio(isProblemaValePedagio || _gridCargaIntegracaoValePedagio.NumeroRegistros() > 0);
}

function callbackGridValePedagioPreCte(isProblemaValePedagio) {
    habilitarTabIntegracaoValePedagioPreCte(isProblemaValePedagio || _gridCargaIntegracaoValePedagioPreCTe.NumeroRegistros() > 0);
}

function habilitarTabIntegracaoValePedagio(visivel) {
    const tabIntegracaoValePedagio = $("#tabIntegracaoValePedagio_" + _cargaAtual.DadosCTes.id + "_li")

    if (visivel) {
        tabIntegracaoValePedagio.show();
        return;
    }

    tabIntegracaoValePedagio.hide();
}

function habilitarTabIntegracaoValePedagioPreCte(visivel) {
    const tabIntegracaoValePedagioPreCte = $("#tabIntegracaoValePedagioPreCtes_" + _cargaAtual.DadosCTes.id + "_li")

    if (visivel) {
        tabIntegracaoValePedagioPreCte.show();
        return;
    }

    tabIntegracaoValePedagioPreCte.hide();
}

//*******METODOS*******

function integrarValePedagioClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo,
        Carga: _cargaAtual.Codigo.val()
    }

    executarReST("CargaIntegracaoValePedagio/ReenviarIntegracaoRejeitadas", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_isPreCte) {
                    _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid();
                } else {
                    _gridCargaIntegracaoValePedagio.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function LiberarComProblemaValePedagioClick(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaAvancarEtapaMesmoQueTenhaValePedagiosComFalhaNaIntegracao, function () {
        executarReST("CargaIntegracaoValePedagio/LiberarComProblemaValePedagio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _cargaCTe.LiberarComProblemaValePedagio.enable(false);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function LiberarComProblemaValePedagioClickSemFalha(e) {
    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    executarReST("CargaIntegracaoValePedagio/LiberarComProblemaValePedagio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cargaCTe.LiberarSemValePedagio.enable(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ImpressaoValePedagioViaPortalClick(datagrid) {
    var data = {
        Codigo: datagrid.Codigo
    }

    if (datagrid.TipoIntegradora == EnumTipoIntegracao.DBTrans) {

        executarReST("CargaIntegracaoValePedagio/BuscarURLValePedagio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    window.open(arg.Msg, '_blank').focus();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    }
    else {
        executarDownload("CargaIntegracaoValePedagio/ImpressaoValePedagio", data);
    }
}

function CancelarValePedagioNaCargaClick(datagrid) {
    exibirConfirmacao(Localization.Resources.Cargas.Carga.CancelarValePedagio, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaCancelarValePedagioSemCancelarCarga, function () {
        var data = {
            Codigo: datagrid.Codigo
        }

        executarReST("CargaIntegracaoValePedagio/CancelarValePedagioNaCarga", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_isPreCte) {
                        _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid();
                    } else {
                        _gridCargaIntegracaoValePedagio.CarregarGrid();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}


function VisibilidadeReemitirIntegracaoValePedagio(datagrid) {
    return (datagrid.SituacaoIntegracao == EnumSituacaoIntegracaoCarga.ProblemaIntegracao);
}

function VisibilidadeImprimirIntegracaoValePedagio(datagrid) {
    var visible = false;
    if (datagrid.SituacaoIntegracao == EnumSituacaoIntegracaoCarga.Integrado) {
        //if (datagrid.TipoIntegradora == EnumTipoIntegracao.SemParar) todo: ver outros tipos que disnponibilizam impressão.
    }
    return visible;
}

function VisibilidadeReemitirIntegracaoValePedagioViaPortal(datagrid) {
    var visible = false;
    if (datagrid.SituacaoIntegracao == EnumSituacaoIntegracaoCarga.Integrado) {
        if (datagrid.TipoIntegradora == EnumTipoIntegracao.SemParar || datagrid.TipoIntegradora == EnumTipoIntegracao.Target || datagrid.TipoIntegradora == EnumTipoIntegracao.Repom ||
            datagrid.TipoIntegradora == EnumTipoIntegracao.PagBem || datagrid.TipoIntegradora == EnumTipoIntegracao.DBTrans || datagrid.TipoIntegradora == EnumTipoIntegracao.EFrete ||
            datagrid.TipoIntegradora == EnumTipoIntegracao.Extratta || datagrid.TipoIntegradora == EnumTipoIntegracao.DigitalCom || datagrid.TipoIntegradora == EnumTipoIntegracao.Pamcard)
            visible = true;
    }
    return visible;
}

function VisibilidadeCancelarValePedagioNaCarga(datagrid) {

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_NaoPermitirCancelarValePedagio, _PermissoesPersonalizadasCarga))
        return false;

    return (datagrid.SituacaoValePedagio == EnumSituacaoValePedagio.Confirmada || datagrid.SituacaoValePedagio == EnumSituacaoValePedagio.Comprada) && !datagrid.RecebidoPorIntegracao ;
}

function VisibilidadeHistoricoIntegracao(datagrid) {
    var visible = true;
    if (datagrid.RecebidoPorIntegracao) {
        visible = false;
    }
    return visible;
}


function ExibirHistoricoIntegracaoValePedagio(integracao) {
    BuscarHistoricoIntegracaoValePedagio(integracao);
    Global.abrirModal("divModalHistoricoIntegracaoCTe");
}

function BuscarHistoricoIntegracaoValePedagio(integracao) {
    _pesquisaHistoricoIntegracaoValePedagio = new PesquisaHistoricoIntegracaoValePedagio();
    _pesquisaHistoricoIntegracaoValePedagio.Codigo.val(integracao.Codigo);

    var download = { descricao: Localization.Resources.Cargas.Carga.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoValePedagio, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoValePedagio = new GridView("tblHistoricoIntegracaoCTe", "CargaIntegracaoValePedagio/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoValePedagio, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoValePedagio.CarregarGrid();
}

function ImpressaoValePedagioClick(integracao) {
    executarDownload("CargaIntegracaoValePedagio/ImpressaoRecibo", { Codigo: integracao.Codigo });
}

function DownloadArquivosHistoricoIntegracaoValePedagio(historicoConsulta) {
    executarDownload("CargaIntegracaoValePedagio/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}