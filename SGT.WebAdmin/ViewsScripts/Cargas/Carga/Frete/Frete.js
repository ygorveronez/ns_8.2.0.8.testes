/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoAlteracaoFreteCarga.js" />
/// <reference path="../Roteirizador/Roteirizador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CodigoCargaDetalhamentoFrete = 0;
var _solicitacaoFrete;
var _retorno;
var _simulacaoFreteDetalheCarga;
var _gridAlteraValorFretePorPedido;
var _AlteraValorFretePorPedido;
var _Carga;
var _utilizandoAbaFilialEmissora;
/*
 * Declaração das Classes
 */

function SolicitacaoFrete() {
    this.KnoutCarga = PropertyEntity({ ko: null, type: types.local });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable(Localization.Resources.Cargas.Carga.MotivoSolicitacaoDeFrete.getRequiredFieldDescription()), idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 2000 });
    this.ListaAnexo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.ListaAnexo.val.subscribe(function () {
        recarregarGridSolicitacaoFreteAnexo();
    });

    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarSolicitacaoFreteAnexoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarAnexos, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: atualizarValorFreteModalClick, type: types.event, text: Localization.Resources.Gerais.Geral.Alterar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCargaFrete() {
    loadCargaFreteRota();
    loadCargaComissao();
    loadCargaFreteCliente();
    loadCargaFreteNaoEncontrado();
    loadCargaFreteSubcontratacao();
    loadCargaAprovacaoFrete();
    loadCargaIntegracaoValePedagio();
    loadCargaAprovacaoFreteSolicitacao();
    if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorCarga)
        loadCargaFreteSubcontratacaoTerceiro();
}

function loadSolicitacaoFrete() {
    _solicitacaoFrete = new SolicitacaoFrete();
    KoBindings(_solicitacaoFrete, "knoutSolicitacaoFrete");

    new BuscarMotivoSolicitacaoFrete(_solicitacaoFrete.Motivo);

    loadSolicitacaoFreteAnexo();
}

function loadSimulacaoFreteDetalheCarga() {
    _simulacaoFreteDetalheCarga = new SimulacaoFrete();
    KoBindings(_simulacaoFreteDetalheCarga, "knoutSimulacaoFreteDetalheCarga");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarValorFreteClick(e) {
    if (e.ValorFreteOperador.val() != "" || e.ValorTotalMoeda.val() != "") {
        e.ValorFreteOperador.requiredClass("form-control");

        if ((e.ValorFreteTabelaFrete.val() > Globalize.parseFloat(e.ValorFreteOperador.val())) || _CONFIGURACAO_TMS.PermitirOperadorInformarValorFreteMaiorQueTabela) {
            if (_CONFIGURACAO_TMS.ObrigarMotivoSolicitacaoFrete)
                exibirModalSolicitacaoFrete(e);
            else
                EnviarFreteManualParaProcessamento(e);
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.ValorInvalido, Localization.Resources.Cargas.Carga.ValorDoFreteDeveSerInferiorAoEstipuladoPelaTabela.format(e.ValorFreteTabelaFrete.val()));
    }
    else {
        e.ValorFreteOperador.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.Carga.ParaAlterarValorDoFreteInformeNovoValor);
    }
}

function atualizarValorFreteModalClick() {
    if (!validarCamposObrigatoriosSolicitacaoFrete())
        return;

    EnviarFreteManualParaProcessamento(_solicitacaoFrete.KnoutCarga.ko, _solicitacaoFrete.Motivo.codEntity(), _solicitacaoFrete.Observacao.val());
}

function detalhesFreteClick(e, sender) {
    var data = _cargaAtual;
    resetarTabsDetalheFrete();
    verDetalhesFreteNaCargaClick(data);
}

function LiberarEmissaoDiferencaValorFreteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteAutorizarEmissaoDaCargaComDiferencaNoValorDoFrete, function () {

        var dados = { Carga: e.Codigo.val() };

        executarReST("CargaFrete/AutorizarDiferencaValorFrete", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AutorizacaoRealizadaComSucesso);
                    e.AutorizarEmissaoDocumentos.visible(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    });
}

function rateiarValorNotaClick(e) {
    var data = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/RatearFreteEntreAsNotas", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.Sucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function retornoAlteracaoFrete(e, arg) {
    $("#" + e.DivCarga.id + "_ribbonCargaNova").hide();
    if (arg.Data.situacao == EnumSituacaoRetornoDadosFrete.FreteValido || arg.Data.situacao == EnumSituacaoRetornoDadosFrete.CalculandoFrete) {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.Pendencias, Localization.Resources.Cargas.Carga.ExistemPendenciasParaCalcularFrete);
    }
    preecherRetornoFrete(e, arg.Data);
}

function retornoRemoverPreCalculoFrete(e, arg) {
    e.ExcluirPreCalculo.visible(false);
    e.RecalcularFrete.visible(true);
}

function recalcularFreteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRecalcularFreteLembrandoQueTodasAsAlteracoesNoFreteSeraoDesfeitasFreteFicaraComValorOriginalDaTabela, function (confi) {
        recalcularFrete(e);
    });
}

function recalcularFreteBIDClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRecalcularFreteBID, function (confi) {
        recalcularFreteBID(e);
    });
}

function excluirPreCalculoClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverPreCalculoDeFrete, function (confi) {
        removerPreCalculoFrete(e);
    });
}

function RoteirizarCargaNovamenteClick(e, sender) {
    var dados = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/SolicitarRoteirizacaoCarga", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                carregarDadosPedido(0);
                retornoAlteracaoFrete(e, arg);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function UtilizarContratoFreteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.RealmenteDesejaUtilizarContratoDeFreteParaEstaCarga, function () {
        var dados = { Codigo: e.Codigo.val() };
        executarReST("CargaFrete/LiberarUsoContratoFreteCarga", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    carregarDadosPedido(0);
                    retornoAlteracaoFrete(e, arg);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    });
}

function verDetalhesFreteNaCargaClick(e) {
    _CodigoCargaDetalhamentoFrete = e.Codigo.val();
    verificarFrete(e, function (retorno) {
        resetarTabsDetalheFrete();
        preencherDetalhesFrete(retorno, true);
        preencherComposicaoImpostos(retorno, e);
        preencherRacionalRateioFrete(e);
        preencherComposicaoRateioFrete(e);
        Global.abrirModal("divModalDetalheValorFrete");
    });
}

function conferenciaDeFreteClick(e) {
    preencherConferenciaDeFrete(e);
    Global.abrirModal("divModalConferenciaDeFrete");
}

function verificarFreteClick(e, sender) {
    _cargaAtual = e;
    ocultarTodasAbas(e);
    loadCargaDadosEmissao(function () {
        //carregarDadosPedido(0, function () {
        verificarFrete(e);
        //});
    });
}

function verSimulacaoFreteClick(e, sender) {
    LimparCampos(_simulacaoFreteDetalheCarga);
    executarReST("Roteirizador/BuscarSimulacaoFrete", { Carga: e.Codigo.val() }, function (arg) {
        if (arg.Data) {
            PreencherObjetoKnout(_simulacaoFreteDetalheCarga, arg);
            Global.abrirModal("divModalSimulacaoFreteDetalheCarga");
        }
    });
}

/*
 * Declaração das Funções Públicas
 */

function BuscarValorTotalComponentes(retornoFrete) {
    var valorComponentes = 0;
    if (retornoFrete.componentesFrete != null) {
        $.each(retornoFrete.componentesFrete, function (i, componente) {
            if (componente.TipoComponenteFrete != EnumTipoComponenteFrete.ICMS && componente.TipoComponenteFrete != EnumTipoComponenteFrete.ISS) {
                if (typeof componente.Valor == "number")
                    valorComponentes += componente.Valor;
                else
                    valorComponentes += Globalize.parseFloat(componente.Valor);
            }
        });
    }
    return valorComponentes;
}

function exibirFreteFilialEmissora() {
    preecherRetornoFrete(_cargaAtual, _retorno.DadosFreteFilialEmissora, false, true);
}

function exibirValorFreteFrete() {
    preecherRetornoFrete(_cargaAtual, _retorno, false, false);
}

function permitirInformarValorFreteManualmente(e) {
    if (e.EtapaFreteEmbarcador.enable()) {
        e.ValorFreteOperador.visible(true);
        e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.InformarValorDoFreteManualmente.getFieldDescription());

        if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && e.Moeda.val() !== null && e.Moeda.val() !== EnumMoedaCotacaoBancoCentral.Real) {
            e.Moeda.visible(true);
            e.ValorTotalMoeda.visible(true);
            e.ValorCotacaoMoeda.visible(true);
            e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.ValorDoFrete.getFieldDescription());
            e.ValorFreteOperador.enable(false);
        }

        e.AtualizarValorFrete.visible(true);
    }
}

function preecherRetornoFrete(e, retorno, mostrarAbaValor, filialEmissora) {
    if (retorno == null)
        return;

    _utilizandoAbaFilialEmissora = filialEmissora;

    if (mostrarAbaValor == null)
        mostrarAbaValor = true;
    if (filialEmissora == null)
        filialEmissora = false;
    if (mostrarAbaValor) {
        if (_cargaAtual.ExibirCalculoFreteCargaAgrupada.val()) {
            $("#tabValoresFreteAgrupada_" + e.DadosEmissaoFrete.id + "_li").show();
            $("#tabValoresFreteAgrupada_" + e.DadosEmissaoFrete.id + "_li").addClass("active");
            $("#tabValoresFreteAgrupada_" + e.DadosEmissaoFrete.id).addClass("active in");

            $("#tabValoresFrete_" + e.DadosEmissaoFrete.id + "_li").hide();
            $("#tabValoresFrete_" + e.DadosEmissaoFrete.id).removeClass("active in");
            $("#tabValoresFrete_" + e.DadosEmissaoFrete.id + "_li").removeClass("active");

            preencherTabsCargas(retorno.cargas, e);

        } else {
            $("#tabValoresFrete_" + e.DadosEmissaoFrete.id + "_li").addClass("active");
            $("#tabValoresFrete_" + e.DadosEmissaoFrete.id).addClass("active in");
        }
    }

    $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).hide();

    e.AtualizarValorFrete.visible(e.EtapaFreteEmbarcador.enable());
    e.AtualizarValorFrete.enable(e.EtapaFreteEmbarcador.enable());
    e.RecalcularFrete.enable(e.EtapaFreteEmbarcador.enable());
    e.UtilizarContratoFrete.enable(e.EtapaFreteEmbarcador.enable());
    e.RoteirizarCargaNovamente.enable(e.EtapaFreteEmbarcador.enable());
    e.ComponenteFrete.enable(e.EtapaFreteEmbarcador.enable());
    e.AdicionarComplementoFrete.enable(e.EtapaFreteEmbarcador.enable());
    e.RetornarParaEtapaNFeTMS.enable(e.EtapaFreteEmbarcador.enable());
    e.AutorizarEmissaoDocumentos.enable(e.EtapaFreteEmbarcador.enable());
    e.ConferenciaDeFrete.enable(e.EtapaFreteEmbarcador.enable());

    e.ValorPorPedido.enable(e.EtapaFreteEmbarcador.enable());
    e.ValorPorPedidoFilialEmissora.enable(e.EtapaFreteEmbarcador.enable());
    e.ValorPorPedido.visible(false);
    e.ValorPorPedidoFilialEmissora.visible(false);

    if (retorno.PermiteAlterarValorFretePedidoPosCalculoFrete) {
        e.ValorPorPedido.visible(true);
        e.ValorPorPedidoFilialEmissora.visible(true);
    }

    if (retorno.ExigirConferenciaManual)
        e.ConferenciaDeFrete.visible(true);
    else
        e.ConferenciaDeFrete.visible(false);

    if (retorno.situacao == EnumSituacaoRetornoDadosFrete.FreteValido) {
        if (e.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Embarcador) {
            preencherRetornoFreteEmbarcador(e, retorno);
            bloquearAlteracoesFrete(e);
            e.RecalcularFrete.visible(false);
            e.ComponenteFrete.visible(false);
        } else if (e.TipoFreteEscolhido.val() == EnumTipoFreteEscolhido.Cliente) {
            preencherRetornoFretePorContaCliente(e, retorno);
            bloquearAlteracoesFrete(e);
            e.RecalcularFrete.visible(false);
        }
        else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaRota)
            preencherRetornoFreteRota(e, retorno);
        else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto)
            preencherRetornoComissao(e, retorno);
        else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaCliente)
            preencherRetornoFreteCliente(e, retorno, filialEmissora);
        else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.freteSemTabela)
            preencherRetornoFreteSemTabela(e, retorno);
        else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaSubContratacao)
            preencherRetornoFreteSubcontratacao(e, retorno);

        if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorCarga)
            preencherDadosContratoTerceiro(e, retorno);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            preencherComplementosFrete(e, retorno.complementosDoFrete);
        else
            preecherComponentesFrete(e, retorno.componentesFrete);

        if (!filialEmissora) {
            preencherDadosFreteFilialEmissora(e, retorno);
            _retorno = retorno;
        } else {
            setarVisibilidadeEdicaoFilialEmissora(e);
        }

    }
    else if (retorno.situacao == EnumSituacaoRetornoDadosFrete.ProblemaCalcularFrete) {
        if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaRota) {
            exibirProblemaFreteRota(e, retorno);
        } else if (retorno.tipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto) {
            exibirProblemaFreteComissao(e, retorno);
        } else {
            setarMensagemPendenciaFrete(e, retorno.mensagem != null ? retorno.mensagem.replace(/\n/g, "<br />") : "");
        }

        if (retorno.VeiculoPossuiContratoFrete)
            e.UtilizarContratoFrete.visible(true);
        else
            e.UtilizarContratoFrete.visible(false);
    }
    else if (retorno.situacao == EnumSituacaoRetornoDadosFrete.RotaNaoEncontrada) {
        var titulo = Localization.Resources.Cargas.Carga.NaoFoiEncontradoUmaRotaEntreOsSeguintesLugares;
        var html = "";
        $.each(retorno.rotasNaoEncontradas, function (i, rota) {
            html += _HTMLPendenciasCarga.replace(/#idPendencia/g, guid())
                .replace(/#DescricaoPendecia/g, rota.Origem.Descricao + " " + Localization.Resources.Cargas.Carga.Ate + " " + rota.Destino.Descricao);
        });
        setarMensagemPendenciaFrete(e, html, titulo, false);
    }
    else if (retorno.situacao == EnumSituacaoRetornoDadosFrete.CalculandoFrete) {
        $("#tabValoresFrete_" + e.DadosEmissaoFrete.id + " .DivDetalheFrete").hide();

        if (_cargaAtual.PendenteGerarCargaDistribuidor.val()) {
            $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).hide();
            $("#divProgressGerandoCargaSegundoTrecho_" + e.DadosEmissaoFrete.id).show();
        } else {
            $("#divProgressGerandoCargaSegundoTrecho_" + e.DadosEmissaoFrete.id).hide();

            if (!retorno.PermiteRoterizarNovamente) {

                if (_cargaAtual.IntegrandoValePedagio.val()) {
                    //ainda esta integrando vale pedagio, buscando valores...
                    $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).hide();
                    $("#divProgressConsultandoValoresFrete_" + e.DadosEmissaoFrete.id).show();

                    if (_cargaAtual.ProblemaIntegracaoValePedagio.val() && !_cargaAtual.LiberadoComProblemaValePedagio.val()) {
                        setarMensagemPendenciaFrete(e, retorno.mensagem != null ? retorno.mensagem.replace(/\n/g, "<br />") : "");
                        $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCarga").show();
                        $("#divProgressConsultandoValoresFrete_" + e.DadosEmissaoFrete.id).hide();
                    } else {
                        if (_cargaAtual.ProblemaIntegracaoValePedagio.val() && _cargaAtual.LiberadoComProblemaValePedagio.val()) {
                            $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).show();
                            $("#divProgressConsultandoValoresFrete_" + e.DadosEmissaoFrete.id).hide();
                        }
                    }

                } else {
                    if (!_cargaAtual.IntegrandoValePedagio.val() && _cargaAtual.ProblemaIntegracaoValePedagio.val()) {
                        setarMensagemPendenciaFrete(e, retorno.mensagem != null ? retorno.mensagem.replace(/\n/g, "<br />") : "");
                        $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCarga").show();
                        $("#divProgressConsultandoValoresFrete_" + e.DadosEmissaoFrete.id).hide();

                    } else {
                        $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).show();
                    }
                }

                $("#divProblemaRoteirizacao_" + e.DadosEmissaoFrete.id).hide();
                _cargaAtual.RoteirizarCargaNovamente.visible(false);
            }
            else {
                $("#divProgressCalculandoFrete_" + e.DadosEmissaoFrete.id).hide();
                $("#divProblemaRoteirizacao_" + e.DadosEmissaoFrete.id).show();
                _cargaAtual.RoteirizarCargaNovamente.visible(true);
            }
        }

        e.AtualizarValorFrete.visible(false);
        e.UtilizarContratoFrete.visible(false);
        e.AtualizarValorFrete.enable(false);

        e.RecalcularFrete.enable(false);
        e.ComponenteFrete.enable(false);
        e.AdicionarComplementoFrete.enable(false);
        e.RetornarParaEtapaNFeTMS.enable(false);
        e.AutorizarEmissaoDocumentos.enable(false);
        e.ValorFreteOperador.visible(false);
        e.Moeda.visible(false);
        e.ValorTotalMoeda.visible(false);
        e.ValorCotacaoMoeda.visible(false);
        e.ConferenciaDeFrete.enable(false);
    }
    preencherCargaAprovacaoFrete(e, retorno);
    preencherCargaAprovacaoFreteSolicitacao(e);
    preencherCargaValoresValePedagio(e);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && (retorno.situacao === EnumSituacaoRetornoDadosFrete.ProblemaCalcularFrete || retorno.situacao === EnumSituacaoRetornoDadosFrete.FreteValido) && e.TipoFreteEscolhido.val() !== EnumTipoFreteEscolhido.Embarcador) {

        e.Moeda.val(Globalize.format(retorno.Moeda || 0, "n2"));
        e.ValorCotacaoMoeda.val(Globalize.format(retorno.ValorCotacaoMoeda || 0, "n10"));

        if (retorno.Moeda !== null && retorno.Moeda !== EnumMoedaCotacaoBancoCentral.Real) {
            e.Moeda.visible(true);
            e.ValorTotalMoeda.visible(true);
            e.ValorCotacaoMoeda.visible(true);
            e.ValorFreteOperador.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
            e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.ValorDoFrete.getFieldDescription());
            e.ValorFreteOperador.enable(false);
        } else {
            e.Moeda.visible(false);
            e.ValorTotalMoeda.visible(false);
            e.ValorCotacaoMoeda.visible(false);
            e.ValorFreteOperador.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
            e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.AlterarValorDoFrete.getFieldDescription());
            e.ValorFreteOperador.enable(true);
        }

    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarValorFrete, _PermissoesPersonalizadasCarga)) {
        e.AtualizarValorFrete.enable(false);
        e.ValorFreteOperador.enable(false);

        if (retorno.tipoTabelaFrete !== EnumTipoTabelaFrete.freteSemTabela) {
            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarValorFreteApenasComTabelaFrete, _PermissoesPersonalizadasCarga)) {
                e.AtualizarValorFrete.enable(true);
                e.ValorFreteOperador.enable(true);
            }
        }
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_CalcularFreteNovamente, _PermissoesPersonalizadasCarga)) {
        e.RecalcularFrete.enable(false);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarMoeda, _PermissoesPersonalizadasCarga)) {
            e.AlterarMoedaCarga.enable(false);
            e.AlterarMoedaCarga.visible(false);
        } else {
            if (e.SituacaoCarga.val() === EnumSituacoesCarga.EmTransporte || e.SituacaoCarga.val() === EnumSituacoesCarga.Encerrada) {
                e.AlterarMoedaCarga.enable(true);
                e.AlterarMoedaCarga.visible(true);
            }
        }
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AdicionarComponentes, _PermissoesPersonalizadasCarga)) {
        e.ComponenteFrete.enable(false);
        e.AdicionarComplementoFrete.enable(false);
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_RetornarEtapaNotasFiscais, _PermissoesPersonalizadasCarga) && !_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente && !e.PermiteImportarDocumentosManualmente.val())
        e.RetornarParaEtapaNFeTMS.enable(false);

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos, _PermissoesPersonalizadasCarga))
        e.AutorizarEmissaoDocumentos.enable(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        e.RecalcularFrete.visible(false);
        e.AdicionarComplementoFrete.visible(false);
        e.ComponenteFrete.enable(false);
        e.AdicionarComplementoFrete.visible(false);
        e.RetornarParaEtapaNFeTMS.visible(_CONFIGURACAO_TMS.PermitirTransportadorRetornarEtapaNFe);
        e.AutorizarEmissaoDocumentos.visible(false);
        e.AtualizarValorFrete.visible(false);
        e.ConferenciaDeFrete.visible(false);
    }

    if (_cargaAtual.CargaTransbordo.val() === true || _cargaAtual.CargaSVM.val() === true) {
        if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && !_CONFIGURACAO_TMS.CargaTransbordoNaEtapaInicial) || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            e.RecalcularFrete.visible(false);

        if (_CONFIGURACAO_TMS.TipoContratoFreteTerceiro == EnumTipoContratoFreteTerceiro.PorPagamentoAgregado) {
            e.ComponenteFrete.visibleFade(false);
            e.AtualizarValorFrete.visible(false);
            e.ValorFreteOperador.visible(false);
            e.Moeda.visible(false);
            e.ValorTotalMoeda.visible(false);
            e.ValorCotacaoMoeda.visible(false);
        }

        if (_cargaAtual.CargaSVM.val() === true) {
            e.ValorFreteOperador.visible(false);
            e.Moeda.visible(false);
            e.ValorTotalMoeda.visible(false);
            e.ValorCotacaoMoeda.visible(false);
            e.ComponenteFrete.visible(false);
            e.AtualizarValorFrete.visible(false);
            $("#tabComponentes_" + e.DadosEmissaoFrete.id + "_li").hide();
        }
    }



    if (_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoAlteracaoValorFrete && (e.SituacaoAlteracaoFreteCarga.val() == EnumSituacaoAlteracaoFreteCarga.Aprovada)) {
        e.RecalcularFrete.visible(false);
        e.AdicionarComplementoFrete.visibleFade(false);
        e.AtualizarValorFrete.visible(false);
        e.ValorFreteOperador.visible(false);
        e.Moeda.visible(false);
        e.ValorTotalMoeda.visible(false);
        e.ValorCotacaoMoeda.visible(false);
    }
    if (retorno.cargas != undefined && retorno.cargas.length > 0 && _cargaAtual.ExibirCalculoFreteCargaAgrupada.val())
        buscarValorFrete(retorno.cargas[0].Codigo);

    loadDadosIntegracaoCargaFreteIntegracao(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        e.AutorizarEmissaoDocumentos.enable(_cargaAtual.PermiteTransportadorAvancarEtapaEmissao.val());
        e.ValorFreteOperador.enable(_cargaAtual.PermiteTransportadorAvancarEtapaEmissao.val());
        _cargaDadosEmissaoPassagem.AlterarPercursoMDFe.enable(_cargaAtual.PermiteTransportadorAvancarEtapaEmissao.val())
    }

}

function preecherDetalhesTiposFrete(e, htmlDetalhe, knoutDetalhes, retornoFrete) {
    var idGrid = guid();
    var html = '<div class="row" id="' + idGrid + '">';
    html += htmlDetalhe;
    html += '</div>';
    if (_cargaAtual.ExibirCalculoFreteCargaAgrupada.val()) {
        $("#tabValoresFreteAgrupada_" + e.DadosEmissaoFrete.id + " .DivDetalheFrete").html(html);
        $("#tabValoresFreteAgrupada_" + e.DadosEmissaoFrete.id + " .DivDetalheFrete").show();
    } else {
        $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivDetalheFrete").html(html);
        $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivDetalheFrete").show();
    }
    KoBindings(knoutDetalhes, idGrid);
    $("#" + knoutDetalhes.DetalhesFrete.id).prop("disabled", false);
    $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCarga").hide();

    PercentualEmRelacaoValorFreteVisible(e, knoutDetalhes, retornoFrete);

    if (e.EtapaFreteEmbarcador.enable() === true && e.TipoCalculoTabelaFrete.val() !== EnumTipoCalculoTabelaFrete.PorDocumentoEmitido) {
        e.ValorFreteOperador.visible(true);
        e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.AlterarValorDoFrete.getFieldDescription());
        e.AtualizarValorFrete.visible(true);

        if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira && e.Moeda.val() !== null && e.Moeda.val() !== EnumMoedaCotacaoBancoCentral.Real) {
            e.Moeda.visible(true);
            e.ValorTotalMoeda.visible(true);
            e.ValorCotacaoMoeda.visible(true);
            e.ValorFreteOperador.enable(false);
            e.ValorFreteOperador.text(Localization.Resources.Cargas.Carga.ValorDoFrete.getFieldDescription());
        }


    } else {
        e.ValorFreteOperador.visible(false);
        e.Moeda.visible(false);
        e.ValorTotalMoeda.visible(false);
        e.ValorCotacaoMoeda.visible(false);
        e.AtualizarValorFrete.visible(false);
    }

    if (e.ExigeNotaFiscalParaCalcularFrete.val()) {
        if (e.SituacaoCarga.val() == EnumSituacoesCarga.CalculoFrete) {
            if (e.EmissaoDocumentosAutorizada.val())
                EtapaFreteTMSAprovada(e);
            else
                EtapaFreteTMSAguardando(e);
        }
    }
    else {
        if (e.SituacaoCarga.val() == EnumSituacoesCarga.AgTransportador) {
            if (e.Veiculo.codEntity() > 0) {
                EtapaDadosTransportadorAprovada(e);
            }
        } else if (e.SituacaoCarga.val() == EnumSituacoesCarga.Nova || e.SituacaoCarga.val() == EnumSituacoesCarga.CalculoFrete) {
            if (!e.TipoOperacao.ExigeConformacaoFreteAntesEmissao)
                EtapaDadosTransportadorLiberada(e);
            else
                EtapaFreteEmbarcadorAguardando(e);
        }
    }

    HabilitarAutorizarEmissaoDocumentos(e);
    e.PossuiPendencia.val(false);

    if (e.OrigemFretePelaJanelaTransportador.val() && !_CONFIGURACAO_TMS.PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador) {
        e.ValorFreteOperador.visible(false);
        e.AtualizarValorFrete.visible(false);
        e.RecalcularFrete.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        knoutDetalhes.DetalhesFrete.visible(!e.TipoOperacao.naoExibirDetalhesDoFretePortalTransportador);
    }
}

function preencherConferenciaDeFrete(e) {

    CarregarGridConferenciaDeFrete(e)
}

function preencherDetalhesFrete(retornoFrete, preencherFilialEmissora) {

    var header = [
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%" },
        { data: "Valor", title: Localization.Resources.Cargas.Carga.Valor, width: "25%", className: "text-align-right" }
    ];
    var valorTotalPrestacao = retornoFrete.valorFreteAPagar;
    var dataFrete = new Array();
    var frete = { "Descricao": Localization.Resources.Cargas.Carga.ValorDoFrete, "Valor": Globalize.format(retornoFrete.valorFrete, "n2") };

    dataFrete.push(frete);

    if (retornoFrete.ValorRetencaoISS > 0 && _CONFIGURACAO_TMS.VisualizarValorNFSeDescontandoISSRetido) {
        //valorTotalPrestacao = valorTotalPrestacao - retornoFrete.ValorRetencaoISS;
        var valorISSRetido = { "Descricao": Localization.Resources.Cargas.Carga.ISSRetido, "Valor": Globalize.format(retornoFrete.ValorRetencaoISS * -1, "n2") };
        dataFrete.push(valorISSRetido);
    }

    if (retornoFrete.componentesFrete != null) {
        $.each(retornoFrete.componentesFrete, function (i, componente) {
            dataFrete.push({ "Descricao": componente.Descricao, "Valor": Globalize.format(componente.Valor, "n2") });
        });
    }

    $("#spanDetalheFreteTotalPrestacao").text(Globalize.format(valorTotalPrestacao, "n2"));
    $("#contentDetalheValorFrete").html("<table width='100%' class='table table-bordered table-hover' cellspacing='0'></table>");
    var gridDetalhesValor = new BasicDataTable("contentDetalheValorFrete table", header, null);
    gridDetalhesValor.CarregarGrid(dataFrete);

    if (
        (retornoFrete.ComposicaoFreteCarga != null && retornoFrete.ComposicaoFreteCarga.length > 0) ||
        (retornoFrete.ComposicaoFretePedido != null && retornoFrete.ComposicaoFretePedido.length > 0) ||
        (retornoFrete.ComposicaoFreteStage != null && retornoFrete.ComposicaoFreteStage.length > 0) ||
        (retornoFrete.ComposicaoFreteCargaSubTrecho != null && retornoFrete.ComposicaoFreteCargaSubTrecho.CargasSubTrecho.length > 0) ||
        (retornoFrete.ComposicaoFreteDocumento != null && (
            (retornoFrete.ComposicaoFreteDocumento.NotasFiscais != null && retornoFrete.ComposicaoFreteDocumento.NotasFiscais.length > 0) ||
            (retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao != null && retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao.length > 0)
        ))
    )
        PreencherComposicaoFrete(retornoFrete, false);
    else
        $("#liComposicaoFrete").hide();

    if (retornoFrete.DadosFreteFilialEmissora != null && preencherFilialEmissora)
        preencherDetalhesFreteFilialEmissora(retornoFrete.DadosFreteFilialEmissora);
    else {
        $("#liDetalhesFreteFilialEmissora").hide();
        $("#liComposicaoFreteFilialEmissora").hide();
    }
}

function preencherDetalhesFreteFilialEmissora(retornoFrete) {
    var header = [
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%" },
        { data: "Valor", title: Localization.Resources.Cargas.Carga.Valor, width: "25%", className: "text-align-right" }
    ];
    var valorTotalPrestacao = retornoFrete.valorFreteAPagar;
    var dataFrete = new Array();
    var frete = { "Descricao": Localization.Resources.Cargas.Carga.ValorDoFrete, "Valor": Globalize.format(retornoFrete.valorFrete, "n2") };

    dataFrete.push(frete);

    if (retornoFrete.componentesFrete != null) {
        $.each(retornoFrete.componentesFrete, function (i, componente) {
            dataFrete.push({ "Descricao": componente.Descricao, "Valor": Globalize.format(componente.Valor, "n2") });
        });
    }
    $("#spanDetalheFreteTotalPrestacaoFilialEmissora").text(Globalize.format(valorTotalPrestacao, "n2"));
    $("#contentDetalheValorFreteFilialEmissora").html("<table width='100%' class='table table-bordered table-hover' cellspacing='0'></table>");
    var gridDetalhesValor = new BasicDataTable("contentDetalheValorFreteFilialEmissora table", header, null);
    gridDetalhesValor.CarregarGrid(dataFrete);

    $("#liDetalhesFreteFilialEmissora").show();

    if ((retornoFrete.ComposicaoFreteCarga != null && retornoFrete.ComposicaoFreteCarga.length > 0) ||
        (retornoFrete.ComposicaoFretePedido != null && retornoFrete.ComposicaoFretePedido.length > 0) ||
        (retornoFrete.ComposicaoFreteCargaSubTrecho != null && retornoFrete.ComposicaoFreteCargaSubTrecho.CargasSubTrecho.length > 0) ||
        (retornoFrete.ComposicaoFreteDocumento != null &&
            ((retornoFrete.ComposicaoFreteDocumento.NotasFiscais != null && retornoFrete.ComposicaoFreteDocumento.NotasFiscais.length > 0) ||
                (retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao != null && retornoFrete.ComposicaoFreteDocumento.CTesParaSubcontratacao.length > 0))))
        PreencherComposicaoFrete(retornoFrete, true);
    else
        $("#liComposicaoFreteFilialEmissora").hide();


}

function preecherRetornoMensagemAlerta(e, retorno) {
    $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCargaAlerta").hide();
    if (retorno != undefined && retorno != null)
        setarMensagemAlerta(e, retorno.mensagem != null ? retorno.mensagem.replace(/\n/g, "<br />") : "");
}

function resetarTabsDetalheFrete() {

    $("#liDetalhesFreteFilialEmissora").hide();
    $("#liComposicaoFreteFilialEmissora").hide();
    $("#liRacionalRateioFrete").hide();
    $("#liComposicaoRateioFrete").hide();

    //$("#liDetalhesFreteFilialEmissora").removeClass("active");
    //$("#liComposicaoFreteFilialEmissora").removeClass("active");
    //$("#liComposicaoFrete").removeClass("active");

    //$("#divDetalhesFreteFilialEmissora").attr("class", "tab-pane fade");
    //$("#divComposicaoFrete").attr("class", "tab-pane fade");
    //$("#divComposicaoFreteFilialEmissora").attr("class", "tab-pane fade");

    //$("#liDetalheFrete").attr("class", "active");
    //$("#divDetalheFrete").attr("class", "tab-pane active in");

    Global.ResetarAba("divModalDetalheValorFrete");
}

function setarMensagemPendenciaFrete(e, mensagem, titulo, exibirAlerta) {
    if (titulo == null)
        titulo = Localization.Resources.Cargas.Carga.Pendencia;

    if (exibirAlerta == null)
        exibirAlerta = true;

    var html = exibirAlerta ? "<div class='alert alert-info alert-block'>" : "<div>";
    html += "<h6 class='alert-heading'>" + titulo + "</h6>";
    html += mensagem;
    html += "</div>";
    setarControlesNaoCalculouFrete(e, html);
}

/*
 * Declaração das Funções Privadas
 */

function bloquearAcoesAlteracaoFrete(e) {
    e.AutorizarEmissaoDocumentos.enable(false);
    bloquearAlteracoesFrete(e);
}

function bloquearAlteracoesFrete(e) {
    e.ValorFreteOperador.visible(false);
    e.Moeda.visible(false);
    e.ValorTotalMoeda.visible(false);
    e.ValorCotacaoMoeda.visible(false);
    e.AtualizarValorFrete.visible(false);
}

function EnviarFreteManualParaProcessamento(e, codigoMotivo, observacao) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAlterarValorDoFretePara.format(e.ValorFreteOperador.val()), function (confi) {
        var dados = {
            Carga: e.Codigo.val(),
            ValorFrete: e.ValorFreteOperador.val(),
            Motivo: codigoMotivo,
            Observacao: observacao,
            Moeda: e.Moeda.val(),
            ValorTotalMoeda: e.ValorTotalMoeda.val(),
            ValorCotacaoMoeda: e.ValorCotacaoMoeda.val(),
            FreteFilialEmissora: _utilizandoAbaFilialEmissora
        };

        executarReST("CargaFrete/InformarValorFreteManual", dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.FreteAlteradoComSucesso);

                    e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Operador);
                    e.ValorFreteOperador.val(e.ValorFreteOperador.def);
                    e.ValorTotalMoeda.val(e.ValorTotalMoeda.def);

                    if (_cargaDadosEmissaoGeral != null)
                        _cargaDadosEmissaoGeral.IncluirICMSBC.val(true);

                    e.PossuiPendencia.val(false);

                    preecherRetornoFrete(e, retorno.Data);
                    carregarDadosPedido(0);
                    enviarArquivosAnexadosSolicitacaoFrete(e.Codigo.val());
                    fecharModalSolicitacaoFrete();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function exibirModalSolicitacaoFrete(e) {
    _solicitacaoFrete.KnoutCarga.ko = e;

    Global.abrirModal("divModalSolicitacaoFrete");
    $('#divModalSolicitacaoFrete').one('hidden.bs.modal', function () {
        limparCamposSolicitacaoFrete();
    });
}

function fecharModalSolicitacaoFrete() {
    Global.fecharModal("divModalSolicitacaoFrete");
}

function HabilitarAutorizarEmissaoDocumentos(e) {
    e.AutorizarEmissaoDocumentos.enable(e.EtapaFreteEmbarcador.enable()); //deixa habilitada apenas se a etapa estiver habilitada;
}

function limparCamposSolicitacaoFrete() {
    LimparCampos(_solicitacaoFrete);

    _solicitacaoFrete.KnoutCarga.ko = null;
    _solicitacaoFrete.ListaAnexo.val(new Array());
}

function PercentualEmRelacaoValorFreteVisible(knoutCarga, knoutDetalhe, retornoFrete) {
    var visible = false;
    var valorFrete = 0;
    var valorTabela = 0;
    var tipoFreteEscolhido = knoutCarga.TipoFreteEscolhido.val();

    if (tipoFreteEscolhido != EnumTipoFreteEscolhido.todos && tipoFreteEscolhido != EnumTipoFreteEscolhido.Tabela && retornoFrete) {
        valorTabela = retornoFrete.valorFreteTabelaFrete;

        if (_CONFIGURACAO_TMS.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga) {
            valorTabela -= (valorTabela * (retornoFrete.aliquotaICMS / 100)) + (valorTabela * (retornoFrete.aliquotaISS / 100));
            valorTabela -= BuscarValorTotalComponentes(retornoFrete);
        }

        if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Embarcador)
            valorFrete = retornoFrete.valorFreteEmbarcador;
        else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Operador)
            valorFrete = retornoFrete.valorFreteOperador;
        else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Leilao)
            valorFrete = retornoFrete.valorFreteLeilao;
        else if (tipoFreteEscolhido == EnumTipoFreteEscolhido.Cliente)
            valorFrete = retornoFrete.valorFreteContratoFrete;
    }

    if ("PercentualEmRelacaoTabela" in knoutDetalhe && "PercentualEmRelacaoValorFrete" in knoutDetalhe) {
        if (valorTabela > 0 && valorFrete > 0) {
            visible = true;
            var percentual = 0;
            var percentualInverso = 0;

            if (_CONFIGURACAO_TMS.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga) {
                percentual = ((valorFrete - valorTabela) * 100) / valorTabela;
                percentualInverso = ((valorFrete - valorTabela) * 100) / valorFrete;
            } else {
                var maiorValor = 0;
                var menorValor = 0;
                var fator = 1;

                if (valorFrete >= valorTabela) {
                    fator = -1;
                    maiorValor = valorFrete;
                    menorValor = valorTabela;
                }
                else {
                    maiorValor = valorTabela;
                    menorValor = valorFrete;
                }

                percentual = (((menorValor * 100) / maiorValor) - 100) * fator;
                percentualInverso = (((maiorValor * 100) / menorValor) - 100) * (-1 * fator);
            }

            knoutDetalhe.PercentualEmRelacaoTabela.val(Globalize.format(percentual, "n2") + "%");
            knoutDetalhe.PercentualEmRelacaoValorFrete.val(Globalize.format(percentualInverso, "n2") + "%");
        }

        knoutDetalhe.PercentualEmRelacaoTabela.visible(visible && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);
        knoutDetalhe.PercentualEmRelacaoValorFrete.visible(visible && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador);
    }
}

function preencherComposicaoImpostos(retornoFrete, e) {

    _CodigoCargaDetalhamentoFrete = e.Codigo.val();


    $("#spanCodigoClassificacao").text(retornoFrete.CodigoClassificacaoTributaria);
    $("#spanAliquotaICMS").text(Globalize.format(retornoFrete.aliquotaICMS, "n2"));
    $("#spanValorICMS").text(Globalize.format(retornoFrete.valorICMS, "n2"));
    $("#spanAliquotaIBSUF").text(Globalize.format(retornoFrete.AliquotaIBSUF, "n2"));
    $("#spanValorIBSUF").text(Globalize.format(retornoFrete.ValorIBSUF, "n2"));
    $("#spanReducaoIBSUF").text(Globalize.format(retornoFrete.ReducaoIBSUF, "n2"));
    $("#spanAliquotaIBSMunicipio").text(Globalize.format(retornoFrete.AliquotaIBSMunicipio, "n2"));
    $("#spanValorIBSMunicipio").text(Globalize.format(retornoFrete.ValorIBSMunicipio, "n2"));
    $("#spanReducaoIBSMunicipio").text(Globalize.format(retornoFrete.ReducaoIBSMunicipio, "n2"));
    $("#spanAliquotaCBS").text(Globalize.format(retornoFrete.AliquotaCBS, "n2"));
    $("#spanValorCBS").text(Globalize.format(retornoFrete.ValorCBS, "n2"));
    $("#spanReducaoCBS").text(Globalize.format(retornoFrete.ReducaoCBS, "n2"));
    $("#spanAliquotaISS").text(Globalize.format(retornoFrete.aliquotaISS, "n2"));
    $("#spanDetalheISS").text(Globalize.format(retornoFrete.valorISS, "n2"));

    $("#detalhesComposicaoPedido").click(function () {
        CarregarGridComposicaoImpostosPedido(e);
    });

}

function preencherDadosContratoTerceiro(e, retorno) {

    if (retorno.freteSubContratacao != null) {
        preencherRetornoFreteSubcontratacaoTerceiro(e, retorno.freteSubContratacao);
    } else {
        $("#tabTerceiros_" + e.DadosEmissaoFrete.id + "_li").hide();
        $("#" + e.EtapaFreteTMS.idTerceiros).html("");
    }
}

function preencherDadosFreteFilialEmissora(e, retorno) {
    if (retorno.DadosFreteFilialEmissora != null) {
        $("#tabValoresFreteFilialEmissora_" + e.DadosEmissaoFrete.id + "_li").show();
        setarVisibilidadeEdicaoFilialEmissora(e);
        if (_cargaAtual.CalcularFreteCliente.val())
            $("#spanFilialEmissora_" + e.DadosEmissaoFrete.id).text(Localization.Resources.Cargas.Carga.FreteDoCliente);

    } else {
        $("#tabValoresFreteFilialEmissora_" + e.DadosEmissaoFrete.id + "_li").hide();
    }
}

function preencherRacionalRateioFrete(knoutCarga) {
    executarReST("CargaFrete/ObterDadosRateioFrete", { Carga: knoutCarga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            let $conteudoRacionalRateioFrete = $("#contentRacionalRateioFrete");

            $conteudoRacionalRateioFrete.empty();

            if (retorno.Data.length == 0)
                return;

            $("#liRacionalRateioFrete").show();

            let ordenacaoPadrao = { column: 0, dir: orderDir.asc };
            let header = [
                { data: "Ordem", visible: false },
                { data: "NumeroPedido", title: "Pedido", width: "20%", className: "text-align-center", orderable: false },
                { data: "NumeroStage", title: "Stage", width: "20%", className: "text-align-center", orderable: false },
                { data: "PesoNota", title: "Peso da Nota", width: "20%", className: "text-align-right", orderable: false },
                { data: "PesoPedido", title: "Peso do Pedido", width: "20%", className: "text-align-right", orderable: false },
                { data: "Percentual", title: "Percentual", width: "20%", className: "text-align-right", orderable: false },
                { data: "Valor", title: "Valor", width: "20%", className: "text-align-right", orderable: false }
            ];

            for (let indiceDadosRateio = 0; indiceDadosRateio < retorno.Data.length; indiceDadosRateio++) {
                let dadosRateio = retorno.Data[indiceDadosRateio];
                let idContainerRacionalRateioFrete = "container-racional-rateio-frete_" + dadosRateio.NumeroCte;
                let idGridRacionalRateioFrete = "grid-racional-rateio-frete_" + dadosRateio.NumeroCte;
                let htmlRacionalRateioFrete = '';

                htmlRacionalRateioFrete += '<div class="accordion-item">';
                htmlRacionalRateioFrete += '    <h2 class="accordion-header">';
                htmlRacionalRateioFrete += '        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#' + idContainerRacionalRateioFrete + '" aria-controls"' + idContainerRacionalRateioFrete + '" aria-expanded="false">';
                htmlRacionalRateioFrete += '            <span class="me-3">CT-e: ' + dadosRateio.NumeroCte + '</span><span> Valor: R$ ' + dadosRateio.ValorCte + '</span>';
                htmlRacionalRateioFrete += '        </button>';
                htmlRacionalRateioFrete += '    </h2>';
                htmlRacionalRateioFrete += '    <div class="accordion-collapse collapse p-3" id="' + idContainerRacionalRateioFrete + '" data-bs-parent="#contentRacionalRateioFrete">';
                htmlRacionalRateioFrete += '        <table width="100%" class="table table-bordered table-hover" cellspacing="0" id="' + idGridRacionalRateioFrete + '"></table>';
                htmlRacionalRateioFrete += '    </div>';
                htmlRacionalRateioFrete += '</div>';

                $conteudoRacionalRateioFrete.append(htmlRacionalRateioFrete);

                let gridRacionalRateioFrete = new BasicDataTable(idGridRacionalRateioFrete, header, null, ordenacaoPadrao);

                gridRacionalRateioFrete.CarregarGrid(dadosRateio.Pedidos);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function preencherComposicaoRateioFrete(knoutCarga) {
    executarReST("CargaFrete/ObterDadosComposicaoRateioFrete", { Carga: knoutCarga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data[0].DisponibilizarComposicaoRateioCarga || retorno.Data.length == 0)
                return;

            $("#liComposicaoRateioFrete").show();

            var header = [
                { data: "NumeroPedido", title: "Pedido", width: "20%", className: "text-align-center", orderable: false },
                { data: "DescricaoRateio", title: "Rateio", width: "20%", className: "text-align-center", orderable: false },
                { data: "PesoPedido", title: "Peso do Pedido", width: "20%", className: "text-align-right", orderable: false },
                { data: "DistanciaPedido", title: "KM", width: "20%", className: "text-align-center", orderable: false },
                { data: "FatorPonderacao", title: "Ponderação", width: "20%", className: "text-align-right", orderable: false },
                { data: "TaxaElemento", title: "%", width: "20%", className: "text-align-right", orderable: false },
                { data: "ValorPedido", title: "Valor Pedido", width: "20%", className: "text-align-right", orderable: false },
                { data: "ValorCalculado", title: "Valor Calculado", width: "20%", className: "text-align-right", orderable: false },
                { data: "CodigoTabela", title: "Código Tabela", width: "20%", className: "text-align-center", orderable: false },
                { data: "Origem", title: "Origem", width: "30%", className: "text-align-center", orderable: false },
                { data: "Destino", title: "Destino", width: "30%", className: "text-align-center", orderable: false }
            ];

            var dataFrete = new Array();
            if (retorno.Data != null) {
                $.each(retorno.Data, function (i, dadosRateio) {
                    dataFrete.push({
                        "NumeroPedido": Globalize.format(dadosRateio.NumeroPedido, "n2"),
                        "DescricaoRateio": Globalize.format(dadosRateio.DescricaoRateio, "n2"),
                        "PesoPedido": Globalize.format(dadosRateio.PesoPedido, "n2"),
                        "DistanciaPedido": Globalize.format(dadosRateio.DistanciaPedido, "n2"),
                        "FatorPonderacao": Globalize.format(dadosRateio.FatorPonderacao, "n2"),
                        "TaxaElemento": Globalize.format(dadosRateio.TaxaElemento, "n2"),
                        "ValorPedido": Globalize.format(dadosRateio.ValorPedido, "n2"),
                        "ValorCalculado": Globalize.format(dadosRateio.ValorCalculado, "n2"),
                        "CodigoTabela": dadosRateio.CodigoTabela,
                        "Origem": dadosRateio.Origem,
                        "Destino": dadosRateio.Destino,
                    });
                });
            }

            $("#contentComposicaoRateioFrete").html("<table width='100%' class='table table-bordered table-hover' cellspacing='0'></table>");
            var gridDetalhesValor = new BasicDataTable("contentComposicaoRateioFrete table", header, null);
            gridDetalhesValor.CarregarGrid(dataFrete);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function recalcularFrete(e) {
    var tabelaSelecionada = $("#" + e.EtapaFreteEmbarcador.idGrid + " .selectTabelaFrete").val();//todo: remover aqui após remover a tabela de rotas, não deve jamais existir conflito entre duas tabelas
    var data = { Codigo: e.Codigo.val(), TabelaFreteRota: tabelaSelecionada };
    executarReST("CargaFrete/RecalcularFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Embarcador);
                carregarDadosPedido(0);
                retornoAlteracaoFrete(e, arg);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function recalcularFreteBID(e) {
    var data = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/RecalcularFreteBID", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                e.TipoFreteEscolhido.val(EnumTipoFreteEscolhido.Embarcador);
                carregarDadosPedido(0);
                retornoAlteracaoFrete(e, arg);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function removerPreCalculoFrete(e) {
    var data = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/RemoverPreCalculoFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                retornoRemoverPreCalculoFrete(e, arg);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function setarControlesNaoCalculouFrete(e, html) {

    $("#" + e.EtapaFreteEmbarcador.idGrid + " .MensagemCarga").html(html);
    $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCarga").show();
    PreecherInformacaoValorFrete(e, 0);
    $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivDetalheFrete").hide();
    bloquearAcoesAlteracaoFrete(e);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.PermitirOperadorInformarValorFreteMaiorQueTabela) {
        if (e.ExigeNotaFiscalParaCalcularFrete.val())
            EtapaFreteTMSProblema(e);
        else
            EtapaFreteEmbarcadorProblema(e);

        if (e.MotivoPendenciaFrete.val() != EnumMotivoPendenciaFrete.DivergenciaPreCalculoFrete)
            permitirInformarValorFreteManualmente(e);

        e.ComponenteFrete.visibleFade(false);
    }
    else {
        EtapaFreteEmbarcadorProblema(e);
        e.AdicionarComplementoFrete.visibleFade(false);
    }
}

function setarMensagemAlerta(e, mensagem, titulo, exibirAlerta) {
    if (titulo == null)
        titulo = Localization.Resources.Gerais.Geral.Atencao;

    if (exibirAlerta == null)
        exibirAlerta = true;

    let html = exibirAlerta ? "<div class='alert alert-info alert-dismissible'><button class='btn-close' data-bs-dismiss='alert'><i class='fal fa-times'></i></button>" : "<div>";
    html += "<h6 class='alert-heading fw-500'>" + titulo + "</h6>";
    html += mensagem;
    html += "</div>";

    $("#" + e.EtapaFreteEmbarcador.idGrid + " .MensagemCargaAlerta").html(html);
    $("#" + e.EtapaFreteEmbarcador.idGrid + " .DivMensagemCargaAlerta").show();
}

function setarVisibilidadeEdicaoFilialEmissora(e) {
    //e.AdicionarComplementoFrete.visibleFade(false);
    //e.ValorFreteOperador.visible(false);
    //e.AtualizarValorFrete.visible(false);
}

function validarCamposObrigatoriosSolicitacaoFrete() {
    if (!ValidarCamposObrigatorios(_solicitacaoFrete)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    if (_solicitacaoFrete.KnoutCarga.ko.ObrigatorioInformarAnexoSolicitacaoFrete.val() && (obterListaSolicitacaoFreteAnexo().length == 0)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorAdicioneUmOuMaisAnexos);
        return false;
    }

    return true;
}

function verificarFrete(e, callback) {
    var data = { Codigo: e.Codigo.val() };
    executarReST("CargaFrete/VerificarFrete", data, function (arg) {
        if (arg.Success) {
            if (callback != null) {
                callback(arg.Data);
            } else {
                var retorno = arg.Data;
                preecherRetornoFrete(e, retorno);
            }

        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function preencherCargaFreteintegracao(e) {
    var data = { Codigo: e.Codigo.val() };
}



//Alteração de frete por pedido 
function loadAlteracaoFretePorPedido(e, AlteraValorFrete, AlteraValorFreteFilialEmissora) {
    _Carga = e;
    _AlteraValorFretePorPedido = new AlteraValorFretePorPedido(AlteraValorFrete, AlteraValorFreteFilialEmissora);
    KoBindings(_AlteraValorFretePorPedido, "knockoutAlteraValorFretePorPedido");
    _AlteraValorFretePorPedido.codigoCarga.val(e.Codigo.val());
    _AlteraValorFretePorPedido.AlteraValorFrete.val(AlteraValorFrete);
    _AlteraValorFretePorPedido.AlteraValorFreteFilialEmissora.val(AlteraValorFreteFilialEmissora);
    loadGrisAlteraValorFretePorPedido(e, AlteraValorFrete, AlteraValorFreteFilialEmissora);
}

var AlteraValorFretePorPedido = function () {
    this.codigoCarga = PropertyEntity({ val: ko.observable(false), def: false });
    this.AlteraValorFrete = PropertyEntity({ val: ko.observable(false), def: false });
    this.AlteraValorFreteFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false });
    this.GridAlteraValorFretePorPedido = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
    this.ConfirmarAlteraValorFretePorPedido = PropertyEntity({ eventClick: () => ConfirmarAlteraValorFretePorPedido(), type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.CancelaAlteraValorFretePorPedido = PropertyEntity({ eventClick: () => CancelaAlteraValorFretePorPedido(), type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}


function ConfirmarAlteraValorFretePorPedido() {
    Codigo = _AlteraValorFretePorPedido.codigoCarga.val();
    AlteraValorFrete = _AlteraValorFretePorPedido.AlteraValorFrete.val();
    AlteraValorFreteFilialEmissora = _AlteraValorFretePorPedido.AlteraValorFreteFilialEmissora.val();


    exibirConfirmacao("Confirmação", "Você realmente deseja salvar valor do frete?", function () { //Localization.Resources.Cargas.Frete.DesejaSalvarValorDoFrete
        var lstValoresDeFretePorPedido = JSON.stringify(_gridAlteraValorFretePorPedido.BuscarRegistros());
        executarReST("CargaFrete/SalvarValorFretePorPedido", { lstValoresDeFretePorPedido, Codigo, AlteraValorFrete, AlteraValorFreteFilialEmissora }, (arg) => {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Successo", "Frete salvo com sucesso") //Localization.Resources.Cargas.Frete.FreteSalvoComSucesso
                    verificarFreteClick(_Carga);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            Global.fecharModal("divModalAlteraValorFrete");
        });
    });
}

function CancelaAlteraValorFretePorPedido() {
    Global.fecharModal("divModalAlteraValorFrete");
}

function ValorPorPedidoClick(e) {
    Global.abrirModal("divModalAlteraValorFrete");
    loadAlteracaoFretePorPedido(e, true, false);
}
function ValorPorPedidoFilialEmissoraClick(e) {
    Global.abrirModal("divModalAlteraValorFrete");
    loadAlteracaoFretePorPedido(e, false, true);
}


function AtualizarValorPorPedido(e) {
    if (_AlteraValorFretePorPedido.AlteraValorFrete.val()) {
        e.ValorFreteAntesDaAlteracaoManual = e.ValorFreteDatabase;
    }
    if (_AlteraValorFretePorPedido.AlteraValorFreteFilialEmissora.val()) {
        e.ValorFreteFilialEmissoraAntesDaAlteracaoManual = e.ValorFreteFilialEmissoraDatabase;
    }
}

function CriarGridAlteraValorFreteManualPorPedido(knoutGrid, idGrid, AlteraValorFrete, AlteraValorFreteFilialEmissora) {
    let editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: (e) => AtualizarValorPorPedido(e)
    };

    let editable = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.decimal,
        numberMask: ConfigDecimal({ allowZero: true })
    };




    let detalhes = { descricao: "Detalhes", id: guid(), metodo: (e) => AbriModalAgrupamento(e, tipo), icone: "" };
    let recalcularfrete = { descricao: "Recalcular Frete", id: guid(), metodo: (e) => RecalcularFreteAgrupamento(e), icone: "", visibilidade: validarVisibilidadeRecalcularFrete };

    if (AlteraValorFrete) {
        var header = [
            { data: "ValorFreteDatabase", visible: false },
            { data: "NumeroPedido", title: "Numero Pedido", width: "10%", className: "text-align-center" },//Localization.Resources.Cargas.Pedido.NumeroPedido
            { data: "NomeRemetente", title: Localization.Resources.Cargas.Carga.Remetente, width: "25%", className: "text-align-center" },
            { data: "NomeDestinatario", title: Localization.Resources.Cargas.Carga.Destinatario, width: "25%", className: "text-align-center" },
            { data: "ValorFrete", title: Localization.Resources.Cargas.Carga.ValorFrete, width: "10%", className: "text-align-center", editableCell: editable },
            { data: "ValorFreteAntesDaAlteracaoManual", title: "Valor antes alteração manual", width: "10%", className: "text-align-center" },//Localization.Resources.Cargas.Frete.ValorFreteAntesDaAlteracaoManual
        ];
    }
    if (AlteraValorFreteFilialEmissora) {
        var header = [
            { data: "ValorFreteFilialEmissoraDatabase", visible: false },
            { data: "NumeroPedido", title: "Numero pedido", width: "10%", className: "text-align-center" },//Localization.Resources.Cargas.Pedido.NumeroPedido
            { data: "NomeRemetente", title: Localization.Resources.Cargas.Carga.Remetente, width: "25%", className: "text-align-center" },
            { data: "NomeDestinatario", title: Localization.Resources.Cargas.Carga.Destinatario, width: "25%", className: "text-align-center" },
            { data: "ValorFreteFilialEmissora", title: Localization.Resources.Cargas.Carga.ValorPorPedidoFilialEmissora, width: "10%", className: "text-align-center", editableCell: editable },
            { data: "ValorFreteFilialEmissoraAntesDaAlteracaoManual", title: "Valor antes alteração manual", width: "10%", className: "text-align-right" },//Localization.Resources.Cargas.Frete.ValorFreteFilialEmissoraAntesDaAlteracaoManual
        ];
    }
    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [detalhes, recalcularfrete] };
    return new BasicDataTable(idGrid, header, menuOpcoes, null, null, null, null, null, editarColuna);
}

function loadGrisAlteraValorFretePorPedido(e) {
    _gridAlteraValorFretePorPedido = CriarGridAlteraValorFreteManualPorPedido(_gridAlteraValorFretePorPedido, _AlteraValorFretePorPedido.GridAlteraValorFretePorPedido.idGrid, _AlteraValorFretePorPedido.AlteraValorFrete.val(), _AlteraValorFretePorPedido.AlteraValorFreteFilialEmissora.val());
    executarReST("CargaFrete/ObterListaValorFretePorPedido", { Codigo: e.Codigo.val(), AlteraValorFrete: _AlteraValorFretePorPedido.AlteraValorFrete.val(), AlteraValorFreteFilialEmissora: _AlteraValorFretePorPedido.AlteraValorFreteFilialEmissora.val() }, (arg) => {
        if (arg.Success)
            _gridAlteraValorFretePorPedido.CarregarGrid(arg.Data.StagesColetas);
        else
            _gridAlteraValorFretePorPedido.CarregarGrid([]);
    });
}