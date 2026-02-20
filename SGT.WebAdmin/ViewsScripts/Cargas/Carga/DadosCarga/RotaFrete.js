//*******MAPEAMENTO KNOUCKOUT*******

var CargaRotaFrete = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RotaFrete = PropertyEntity({ type: types.entity, required: false, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Rota.getFieldDescription(), idBtnSearch: guid(), idGrid: guid() });

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: ConfirmarAlteracaoRotaFreteClick, text: Localization.Resources.Cargas.Carga.ConfirmarRota, visible: ko.observable(true) });

    this.OperadorCarga = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function alterarRotaFrete(carga, rota) {

    executarReST("CargaRotaFrete/AlterarRota", { Carga: carga.Codigo.val(), RotaFrete: rota.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                carga.Rota.val(rota.Descricao);
                carga.Rota.codEntity(rota.Codigo);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resouces.Gerais.Geral.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);


}

function abrirBuscaRotaManual(e) {
    e.Rota.entityDescription(e.Rota.val());
    var buscaRotaFrete = new BuscarRotasFrete(e.Rota, function (retorno) {

        if (retorno.Codigo !== 0)
            alterarRotaFrete(e, retorno);
    });

    buscaRotaFrete.AbrirBusca();
}

function buscarRotaAutomatica(e) {

    exibirConfirmacao(Localization.Resources.Cargas.Carga.BuscarRota, Localization.Resources.Cargas.Carga.VoceTemCertezaQueDesejaBuscarRotaParaCarga, function () {
        executarReST("CargaRotaFrete/BuscarRotas", { Carga: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {

                    e.AgSelecaoRotaOperador.val(arg.Data.AgSelecaoRotaOperador);
                    e.Rota.val(arg.Data.Descricao);
                    e.Rota.codEntity(arg.Data.Codigo);

                    // Automaticamente adiciona as fronteiras da rota no campo de fronteiras da carga
                    if (arg.Data.Fronteiras != null && arg.Data.Fronteiras.lenght > 0) {
                        const fronteiras = JSON.parse(arg.Data.Fronteiras);
                        if (fronteiras && fronteiras.length > 0) {
                            if (fronteiras.length == 1) {
                                _cargaDadosEmissaoGeral.Fronteiras.codEntity(fronteiras[0].Codigo);
                                _cargaDadosEmissaoGeral.Fronteiras.val(fronteiras[0].Descricao);
                                _cargaDadosEmissaoGeral.Fronteiras.entityDescription(fronteiras[0].Descricao);
                            } else {
                                _cargaDadosEmissaoGeral.Fronteiras.multiplesEntities(fronteiras);
                            }
                        }
                    }

                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });

}

function AlterarRotaFreteClick(e, sender) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
        var cargaRotaFrete = new CargaRotaFrete();
        KoBindings(cargaRotaFrete, "divModalAlterarRotaFrete");

        cargaRotaFrete.Carga.val(e.Codigo.val());

        cargaRotaFrete.RotaFrete.val(e.Rota.val());
        cargaRotaFrete.RotaFrete.codEntity(e.Rota.codEntity());
        cargaRotaFrete.RotaFrete.Rota = e.Rota;

        new BuscarRotasFrete(cargaRotaFrete.RotaFrete, null, null, null, null, e.Codigo.val(), null, false);

        Global.abrirModal("divModalAlterarRotaFrete");

        return;
    }

    if (e.AgSelecaoRotaOperador.val()) {
        var cargaRotaFrete = new CargaRotaFrete();
        KoBindings(cargaRotaFrete, "divModalSelecionarRotaFrete");

        cargaRotaFrete.Carga.val(e.Codigo.val());

        var handleSelecaoRotaFrete = function (rota) {
            executarReST("CargaRotaFrete/AlterarRota", { Carga: e.Codigo.val(), RotaFrete: rota.Codigo }, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        e.AgSelecaoRotaOperador.val(false);
                        e.Rota.val(arg.Data.Descricao);
                        e.Rota.codEntity(arg.Data.Codigo);

                        Global.fecharModal("divModalSelecionarRotaFrete");
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resouces.Gerais.Geral.Aviso, arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }, null);
        };

        var opcoes = {
            tipo: TypeOptionMenu.link,
            descricao: Localization.Resources.Gerais.Geral.Selecionar,
            tamanho: 5,
            opcoes: [
                { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: guid(), evento: "onclick", metodo: handleSelecaoRotaFrete, tamanho: "10", icone: "" }
            ]
        };

        var grid = new GridView(cargaRotaFrete.RotaFrete.idGrid, 'CargaRotaFrete/BuscarRotasCompativeis', cargaRotaFrete, opcoes);
        grid.CarregarGrid();

        Global.abrirModal("divModalSelecionarRotaFrete");
        $('#divModalSelecionarRotaFrete').one('hidden.bs.modal', function () {
            grid.Destroy();
        });

        return;
    }

    if (_CONFIGURACAO_TMS.UtilizarBuscaRotaFreteManualCarga)
        abrirBuscaRotaManual(e)
    else
        buscarRotaAutomatica(e);
}

function ConfirmarAlteracaoRotaFreteClick(e) {
    if (ValidarCamposObrigatorios(e)) {
        Salvar(e, "CargaRotaFrete/AlterarRota", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    e.RotaFrete.Rota.val(arg.Data.Descricao);
                    e.RotaFrete.Rota.codEntity(arg.Data.Codigo);

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.RotaInformadaComSucesso);

                    Global.fecharModal("divModalAlterarRotaFrete");
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.InformeCamposObrigatorios);

    }
}

var _reordenarEntregasCarga;

var ReordenarEntregasCarga = function () {
    this.Carga = PropertyEntity({});
    this.Km = PropertyEntity({});
    this.Tempo = PropertyEntity({});
    this.PolilinhaRota = PropertyEntity({});
    this.PontosDaRota = PropertyEntity({});
    this.SalvarRota = PropertyEntity({ eventClick: salvarReordemClick, type: types.event, text: Localization.Resources.Cargas.Carga.SalvarRota, visible: ko.observable(false) });
}

//function alterouOrdemEntrega(resposta, pontoOrigem, pontoDestino) {
//    _reordenarEntregasCarga.SalvarRota.visible(true);
//    setInfoRespostaRoteirizacao(resposta);
//}

function alterouRota(resposta) {
    _reordenarEntregasCarga.SalvarRota.visible(true);
    setInfoRespostaRoteirizacao(resposta);
}

function setInfoRespostaRoteirizacao(resposta) {
    var distanciaKM = resposta.distancia / 1000;
    _reordenarEntregasCarga.PolilinhaRota.val(resposta.polilinha);
    _reordenarEntregasCarga.Km.val(distanciaKM);
    _reordenarEntregasCarga.Tempo.val(Math.trunc(resposta.tempo / 60));
    _reordenarEntregasCarga.PontosDaRota.val(resposta.pontosroteirizacao);
}

function VisualizarRotaMapaClick(e, sender) {
    if (e.PossuiRotaDefinida.val()) {

        if (_reordenarEntregasCarga == null) {
            _reordenarEntregasCarga = new ReordenarEntregasCarga();
            KoBindings(_reordenarEntregasCarga, "divModalRotaMapa");
        }
        
        executarReST("CargaRotaFrete/PesquisarRota", { Carga: e.Codigo.val() }, function (r) {
            if (r.Success) {
                var rota = r.Data;
                if (rota != null && rota.PolilinhaRota != "" && rota.PolilinhaRota != null) {

                    _reordenarEntregasCarga.Carga.val(e.Codigo.val());
                    _reordenarEntregasCarga.PolilinhaRota.val(rota.PolilinhaRota);
                    _reordenarEntregasCarga.PontosDaRota.val(rota.PontosDaRota);
                    _reordenarEntregasCarga.SalvarRota.visible(false);

                    Global.abrirModal("divModalRotaMapa");
                    if (!this._mapa) {
                        this._mapa = new Mapa("visualizarrotacarga", false);
                        //this._mapa.setarCallbackAlteracaoEntrega(alterouOrdemEntrega);
                        this._mapa.setarCallbackAlteracaoRota(alterouRota);
                    }

                    this._mapa.limparMapa();

                    this._mapa.adicionarPracasPedagio(rota.PracasPedagio);

                    setTimeout(function () {
                        if (!rota.PermiteReordenarEntregasCarga)
                            this._mapa.desenharPolilinha(rota.PolilinhaRota, true);

                        this._mapa.adicionarMarcadorComPontosDaRota(rota.PontosDaRota, true, rota.PermiteReordenarEntregasCarga);
                                                
                        if (rota.PermiteReordenarEntregasCarga) {
                            this._mapa.limparPolilinhas();
                            this._mapa.roteirizarComPontosDaRota(rota.PontosDaRota, alterouRota);
                        }
                    }, 500);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.Carga.PorFavorVerifiqueRoteirizacaoDessaRota);
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    }
}

function salvarReordemClick(e) {
    Salvar(e, "CargaRotaFrete/SalvarOrdemEntrega", function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.RotaSalvaComSucesso);

                Global.fecharModal("divModalRotaMapa");

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
