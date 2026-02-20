/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaDescarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaDescarregamentoCadastrada.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Logistica/Tracking/Tracking.lib.js" />
/// <reference path="CapacidadeDescarregamentoDados.js" />
/// <reference path="ComposicaoHorarioDescarregamento.js" />
/// <reference path="DetalhesCarga.js" />
/// <reference path="HorarioDescarregamento.js" />
/// <reference path="HorarioDescarregamentoPeriodo.js" />
/// <reference path="JanelaDescarga.js" />
/// <reference path="InformarSenhaAgendamento.js" />
/// <reference path="HistoricoIntegracaoAgendamento.js" />
/// <reference path="Observacoes.js" />
/// <reference path="DisponibilidadeVeiculo.js" />

var TabelaDescarregamento = function () {
    this._idContainerTabela = "divListaDescarregamento";
    this._dataAtual;
    this._gridTabelaDescarregamento;
    this._pesquisaTabelaJanelaDescarregamento;
    this._classeCargaSelecionada = "item-selecionado";

    this._init();
};

TabelaDescarregamento.prototype = {
    Load: function () {
        this._dataAtual = _dadosPesquisaDescarregamento.DataDescarregamento ? moment(_dadosPesquisaDescarregamento.DataDescarregamento, "DD/MM/YYYY") : moment();
        this._dataInicial = _dadosPesquisaDescarregamento.DataDescarregamentoInicial ? moment(_dadosPesquisaDescarregamento.DataDescarregamentoInicial, "DD/MM/YYYY") : moment();
        this._dataFinal = _dadosPesquisaDescarregamento.DataDescarregamentoFinal ? moment(_dadosPesquisaDescarregamento.DataDescarregamentoFinal, "DD/MM/YYYY") : moment();

        LoadHistoricoIntegracaoAgendamento();
        this._loadGridTabelaDescarregamento();
        this._carregarCargas();
    },
    ObterData: function () {
        return this._dataAtual.format("DD/MM/YYYY");
    },
    _adicionarEventos: function () {
        var self = this;

        $("#" + this._idContainerTabela + "-prev").on("click", function () { self._carregarCargasDataDescarregamentoAnterior(); });
        $("#" + this._idContainerTabela + "-next").on("click", function () { self._carregarCargasProximaDataDescarregamento(); });

        $("#botao-opcoes-multipla-selecao").on("click", function () { self._controlarVisibilidadeOpcoes(); });
        $("#cancelar-agendamento").on("click", function () { self._cancelarAgendamento(); });
        $("#confirmar-agendamento").on("click", function () { self._confirmarAgendamento(); });
        $("#buscar-senha-agendamento").on("click", function () { self._buscarSenhaAgendamento(); });
        $("#finalizar-descarregamento").on("click", function () { self._finalizarDescarregamento(); });
        $("#nao-comparecimento").on("click", function () { self._informarNaoComparecimento(); });
        $("#carga-devolvida").on("click", function () { self._informarCargaDevolvida(); });
        $("#alterar-horario-descarregamento").on("click", function () { self._alterarHorarioDescarregamento(); });
        $("#alterar-situacao-para-validacao-fiscal").on("click", function () { self._alterarSituacao(undefined, EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal); });
        $("#alterar-situacao-para-nucleo").on("click", function () { self._alterarSituacao(undefined, EnumSituacaoCargaJanelaDescarregamento.Nucleo); });
    },
    _aguardandoCarregamento: function (cargaSelecionada) {
        var self = this;

        exibirConfirmacao("Confirmação", "Deseja realmente alterar para aguardando carregamento?", function () {
            executarReST("JanelaDescarga/AlterarSituacaoCadastrada", { Codigo: cargaSelecionada.Codigo, Situacao: EnumSituacaoCargaJanelaDescarregamentoCadastrada.AguardandoCarregamento }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Descarregamento alterado para aguardando carregamento com sucesso!");
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _controlarVisibilidadeOpcoes: function (cargaSelecionada) {
        if (!_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo && this._obterCodigos(cargaSelecionada).length > 1) {
            $("#alterar-horario-descarregamento").hide();
        }
        else {
            $("#alterar-horario-descarregamento").show();
        }
    },
    _alterarAgendamento: function (cargaSelecionada) {
        abrirModalAlterarAgendamento(cargaSelecionada);
    },
    _alterarHorarioDescarregamento: function (cargaSelecionada, solicitarMotivo) {

        if (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo) {
            if (cargaSelecionada)
                exibirModalAlteracaoHorarioDescarregamentoPeriodo(cargaSelecionada.Codigo, this.ObterData(), solicitarMotivo);
            else
                exibirModalAlteracaoMultiplosHorariosDescarregamentosPeriodos(this._obterCodigos(cargaSelecionada), solicitarMotivo);
        }
        else {
            var codigos = this._obterCodigos(cargaSelecionada);
            exibirModalAlteracaoHorarioDescarregamento(codigos[0], solicitarMotivo);
        }

    },
    _bloquearDescarga: function (cargaSelecionada) {
        var self = this;

        exibirConfirmacao("Confirmação", "Deseja realmente bloquear o descarregamento?", function () {
            executarReST("JanelaDescarga/AlterarSituacaoCadastrada", { Codigo: cargaSelecionada.Codigo, Situacao: EnumSituacaoCargaJanelaDescarregamentoCadastrada.BloqueadaParaDescarga }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Descarregamento bloqueado com sucesso!");
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _buscarSenhaAgendamento: function (cargaSelecionadas) {
        var self = this;
        var codigos = self._obterCodigosAgendamentoColetaPedido(cargaSelecionadas);

        executarReST("JanelaDescarga/BuscarSenhaAgendamento", { Codigos: JSON.stringify(codigos) }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    carregarGridRetornoFinalizarAgendamento(retorno.Data.Retorno);
                    Global.abrirModal('divModalRetornoFinalizarAgendamento');

                    self._carregarCargas();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _modalMotivoCancelamentoAgendamento: function (cargaSelecionada) {
        var self = this;

        LimparCampos(_motivoCancelamentoAgendamento);

        _motivoCancelamentoAgendamento.Cancelar.eventClick = function () {
            self._cancelarAgendamento(cargaSelecionada);
        };
        Global.abrirModal("divModalMotivoCancelamentoAgendamento");
        $("#divModalMotivoCancelamentoAgendamento")
            .on("hidden.bs.modal", function () {
                LimparCampos(_motivoCancelamentoAgendamento);
            });
    },
    _cancelarAgendamento: function (cargaSelecionada) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);
        var mensagemConfirmacao = codigos.length > 1 ? "Deseja realmente cancelar os agendamentos selecionados?" : "Deseja realmente cancelar o agendamento desta carga?";
        var mensagemSucesso = codigos.length > 1 ? "Agendamentos cancelados com sucesso!" : "Agendamento cancelado com sucesso!";

        var motivoCancelamento = _motivoCancelamentoAgendamento.Motivo.val();

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/CancelarAgendamento", { Codigos: JSON.stringify(codigos), Motivo: motivoCancelamento }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        if (retorno.Data.Msg)
                            exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Data.Msg);
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);

                        Global.fecharModal("divModalMotivoCancelamentoAgendamento");
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _callbackColumnDefault: function (cabecalho, valorColuna, dadosLinha) {
        if (cabecalho.name == "RastreadorOnline")
            return '<div class="tracking-indicador">' + TrackingIconRastreador(dadosLinha.RastreadorOnline) + '</div>';

        if ((cabecalho.name == "SenhaAgendamento") && Boolean(dadosLinha.StatusBuscaSenhaAutomatica))
            return '<span><i class="fa fa-exclamation-circle blink" style="color: red"></i></span>';
    },
    _callbackNaoSelecionado: function (cargaSelecionada) {
        $("tr[id=\"" + cargaSelecionada.Codigo + "\"]").removeClass(this._classeCargaSelecionada);

        this._controlarExibicaoMultiplasOpcoes();
    },
    _callbackSelecionado: function (cargaSelecionada) {
        var registrosSelecionados = this._gridTabelaDescarregamento.ObterMultiplosSelecionados();
        var mensagem = "";

        if (cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento
            && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Nucleo)
            mensagem = "Não é possível selecionar o registro pois a situação do mesmo não permite múltipla seleção."
        else if (registrosSelecionados.filter(function (reg) { return reg.Situacao != cargaSelecionada.Situacao }).length > 0)
            mensagem = "Só é possível selecionar registros com a mesma situação.";

        if (mensagem.length > 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção", mensagem);

            registrosSelecionados = registrosSelecionados.filter(function (carga) { return carga.Codigo != cargaSelecionada.Codigo; });

            this._gridTabelaDescarregamento.AtualizarRegistrosSelecionados(registrosSelecionados);
            this._callbackNaoSelecionado(cargaSelecionada);

            return;
        }

        this._configurarMenuDropDownMultiplaSelecao(cargaSelecionada);

        $("#botao-opcoes-multipla-selecao").removeClass("d-none");

        $("tr[id=\"" + cargaSelecionada.Codigo + "\"]").addClass(this._classeCargaSelecionada);
    },
    _carregarCargas: function () {
        _dadosPesquisaDescarregamento.DataDescarregamento = this._dataAtual.format('DD/MM/YYYY');

        this._pesquisaTabelaJanelaDescarregamento.DataDescarregamento.val(this._dataAtual.format('DD/MM/YYYY'));
        this._pesquisaTabelaJanelaDescarregamento.DataDescarregamentoInicial.val(this._dataInicial.format('DD/MM/YYYY'));
        this._pesquisaTabelaJanelaDescarregamento.DataDescarregamentoFinal.val(this._dataFinal.format('DD/MM/YYYY'));
        this._pesquisaTabelaJanelaDescarregamento.CentroDescarregamento.val(_dadosPesquisaDescarregamento.CentroDescarregamento);
        this._pesquisaTabelaJanelaDescarregamento.Situacao.val(_dadosPesquisaDescarregamento.Situacao);
        this._pesquisaTabelaJanelaDescarregamento.SituacaoAgendamentoPallet.val(_dadosPesquisaDescarregamento.SituacaoAgendamentoPallet);
        this._pesquisaTabelaJanelaDescarregamento.SituacaoCadastrada.val(_dadosPesquisaDescarregamento.SituacaoCadastrada);
        this._pesquisaTabelaJanelaDescarregamento.SenhaAgendamento.val(_dadosPesquisaDescarregamento.SenhaAgendamento);
        this._pesquisaTabelaJanelaDescarregamento.NumeroCarga.val(_dadosPesquisaDescarregamento.NumeroCarga);
        this._pesquisaTabelaJanelaDescarregamento.NumeroPedido.val(_dadosPesquisaDescarregamento.NumeroPedido);
        this._pesquisaTabelaJanelaDescarregamento.Fornecedor.val(_dadosPesquisaDescarregamento.Fornecedor);
        this._pesquisaTabelaJanelaDescarregamento.DataLancamento.val(_dadosPesquisaDescarregamento.DataLancamento);
        this._pesquisaTabelaJanelaDescarregamento.NumeroNF.val(_dadosPesquisaDescarregamento.NumeroNF);
        this._pesquisaTabelaJanelaDescarregamento.NumeroCTe.val(_dadosPesquisaDescarregamento.NumeroCTe);
        this._pesquisaTabelaJanelaDescarregamento.NumeroLacre.val(_dadosPesquisaDescarregamento.NumeroLacre);
        this._pesquisaTabelaJanelaDescarregamento.Veiculo.val(_dadosPesquisaDescarregamento.Veiculo);
        this._pesquisaTabelaJanelaDescarregamento.TipoCarga.val(_dadosPesquisaDescarregamento.TipoCarga);
        this._pesquisaTabelaJanelaDescarregamento.Transportador.val(_dadosPesquisaDescarregamento.Transportador);
        this._pesquisaTabelaJanelaDescarregamento.ExcedenteDescarregamento.val(_dadosPesquisaDescarregamento.ExcedenteDescarregamento);

        $("#" + this._idContainerTabela + "-data-description").text(this._dataAtual.locale('pt-br').format('LL'));
        $("#" + this._idContainerTabela + "-day-of-week").text(this._dataAtual.locale('pt-br').format('dddd'));

        this._recarregarGridTabelaDescarregamento();
    },
    _carregarCargasDataDescarregamentoAnterior: function () {
        this._dataAtual.add(-1, 'days');
        this._carregarCargas();

        buscarCapacidadeDescarregamentoDados();
    },
    _carregarCargasProximaDataDescarregamento: function () {
        this._dataAtual.add(1, 'days');
        this._carregarCargas();

        buscarCapacidadeDescarregamentoDados();
    },
    _composicaoHorarioCarregamento: function (cargaSelecionada) {
        exibirComposicaoHorarioDescarregamento(cargaSelecionada);
    },
    _configurarMenuDropDownMultiplaSelecao: function (cargaSelecionada) {
        if (cargaSelecionada.Situacao == EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento) {
            $("#confirmar-agendamento").hide();
            $("#finalizar-descarregamento").show();
            $("#nao-comparecimento").show();
            $("#carga-devolvida").show();
            $("#alterar-situacao-para-validacao-fiscal").hide();
            $("#alterar-situacao-para-nucleo").hide();
        }
        else if (cargaSelecionada.Situacao == EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento) {
            $("#confirmar-agendamento").show();
            $("#finalizar-descarregamento").hide();
            $("#nao-comparecimento").hide();
            $("#carga-devolvida").hide();
            $("#alterar-situacao-para-validacao-fiscal").show();
            $("#alterar-situacao-para-nucleo").show();


            if (this._isPermitirBuscarSenhaAgendamento(cargaSelecionada))
                $("#buscar-senha-agendamento").show();
        }
        else if (cargaSelecionada.Situacao == EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal) {
            $("#alterar-situacao-para-nucleo").show();
            $("#alterar-situacao-para-validacao-fiscal").hide();
        }
        else if (cargaSelecionada.Situacao == EnumSituacaoCargaJanelaDescarregamento.Nucleo) {
            $("#alterar-situacao-para-nucleo").hide();
            $("#alterar-situacao-para-validacao-fiscal").show();
        }

        $("#alterar-horario-descarregamento").show();
    },
    _confirmarAgendamento: function (cargaSelecionada) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);

        var mensagemConfirmacao = codigos.length > 1 ? "Deseja realmente confirmar os agendamentos selecionados?" : "Deseja realmente confirmar o agendamento desta carga?";
        var mensagemSucesso = codigos.length > 1 ? "Agendas confirmadas com sucesso!" : "Confirmação de agendamento realizada com sucesso!";

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/ConfirmarAgendamento", { Codigos: JSON.stringify(codigos) }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _controlarExibicaoMultiplasOpcoes: function () {
        var existemRegistrosSelecionados = _CONFIGURACAO_TMS.TransformarJanelaDeDescarregamentoEmMultiplaSelecao && (this._gridTabelaDescarregamento.ObterMultiplosSelecionados().length > 0);

        if (existemRegistrosSelecionados)
            $("#botao-opcoes-multipla-selecao").removeClass("d-none");
        else
            $("#botao-opcoes-multipla-selecao").addClass("d-none");
    },
    _desagendarCarga: function (cargaSelecionada) {
        var self = this;

        exibirConfirmacao("Confirmação", "Deseja realmente desagendar o descarregamento desta carga?", function () {
            executarReST("JanelaDescarga/DesagendarCarga", { Codigo: cargaSelecionada.Codigo }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Desagendamento realizado com sucesso!");

                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _exibirDetalhes: function (cargaSelecionada) {
        ExibirDetalhesCarga(cargaSelecionada.CodigoCarga);
    },
    _exibirDetalhesMonitoramento: function (cargaSelecionada) {
        exibirDetalhesMonitoramentoPorCodigo(cargaSelecionada.CodigoMonitoramento);
    },
    _exibirHistoricoMonitoramento: function (cargaSelecionada) {
        exibirHistoricoMonitoramentoPorCodigo(cargaSelecionada.CodigoMonitoramento);
    },
    _finalizarDescarregamento: function (cargaSelecionada) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);

        var mensagemConfirmacao = codigos.length > 1 ? "Deseja realmente finalizar os descarregamentos selecionados?" : "Deseja realmente finalizar o descarregamento desta carga?";
        var mensagemSucesso = codigos.length > 1 ? "Descarregamentos finalizados com sucesso!" : "Descarregamento finalizado com sucesso!";

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/FinalizarDescarregamento", { Codigos: JSON.stringify(codigos) }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _imprimirAgendamento: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0) {
            executarDownload("AgendamentoPallet/Imprimir", { Codigo: cargaSelecionada.CodigoAgendamentoPallet });
            return;
        }

        executarDownload("AgendamentoColeta/Imprimir", { Codigo: cargaSelecionada.CodigoAgendamentoColeta });
    },
    _informarCargaDevolvida: function (cargaSelecionada) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);

        var mensagemConfirmacao = codigos.length > 1 ? "Deseja informar a devolução das cargas?" : "Deseja informar a devolução da carga?";
        var mensagemSucesso = codigos.length > 1 ? "Agendas marcadas com status de carga devolvida." : "Agenda marcada com status de carga devolvida.";

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/InformarCargaDevolvida", { Codigos: JSON.stringify(codigos) }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        if (retorno.Data.Msg != undefined && retorno.Data.Msg.length > 0)
                            exibirMensagem(tipoMensagem.atencao, "Atenção", "Um ou mais agendas não puderam ser informadas como devolvidas: " + retorno.Data.Msg);
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _informarCargaDevolvidaParcialmente: function (cargaSelecionada) {
        buscarPedidoProdutosDescargaParcial(cargaSelecionada, false);
    },
    _informarCargaEntregueParcialmente: function (cargaSelecionada) {
        buscarPedidoProdutosDescargaParcial(cargaSelecionada, true);
    },
    _visualizarCargaDevolvidaouEntregueParcialmente: function (cargaSelecionada) {
        visualizarPedidoProdutosDescargaParcial(cargaSelecionada);
    },
    _informarNaoComparecimento: function (cargaSelecionada) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);

        var mensagemConfirmacao = codigos.length > 1 ? "Deseja marcar as janelas como não comparecidas?" : "Deseja marcar a janela como não comparecida?";
        var mensagemSucesso = codigos.length > 1 ? "Agendas marcadas com status de não comparecimento." : "Agenda marcada com status de não comparecimento.";

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/InformarNaoComparecimento", { Codigos: JSON.stringify(codigos) }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        if (retorno.Data.Msg != undefined && retorno.Data.Msg.length > 0)
                            exibirMensagem(tipoMensagem.atencao, "Atenção", "Um ou mais agendas não puderam ter o não comparecimento informado: " + retorno.Data.Msg);
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _informarSenhaAgendamento: function (cargaSelecionada) {
        exibirModalInformarSenhaAgendamento(cargaSelecionada.CodigoCarga);
    },
    _historicoIntegracao: function (cargaSelecionada) {
        var codigos = this._obterCodigosAgendamentoColetaPedido(cargaSelecionada);

        ExibirHistoricoIntegracaoAgendamento(codigos);
    },
    _alterarSituacao: function (cargaSelecionada, novaSituacao) {
        var self = this;
        var codigos = self._obterCodigos(cargaSelecionada);

        var mensagemConfirmacao = codigos.length > 1 ? "Deseja alterar a situação dos descarregamentos?" : "Deseja alterar a situação do descarregamento?";
        var mensagemSucesso = codigos.length > 1 ? "Situações alteradas com sucesso." : "Situação alterada com sucesso.";

        exibirConfirmacao("Confirmação", mensagemConfirmacao, function () {
            executarReST("JanelaDescarga/AlterarSituacaoCargaJanelaDescarregamento", { Codigos: JSON.stringify(codigos), Situacao: novaSituacao }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        if (retorno.Data.Msg != undefined && retorno.Data.Msg.length > 0)
                            exibirMensagem(tipoMensagem.atencao, "Atenção", "Uma ou mais cargas não puderam ter a situação alterada: " + retorno.Data.Msg);
                        else
                            exibirMensagem(tipoMensagem.ok, "Sucesso", mensagemSucesso);
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    },
    _init: function () {
        this._pesquisaTabelaJanelaDescarregamento = new PesquisaTabelaJanelaDescarregamento();

        $("#" + this._idContainerTabela).css("min-height", "0");
        $("#" + this._idContainerTabela).addClass("fc fc-ltr fc-unthemed");
        $("#" + this._idContainerTabela).html(this._obterHtml());

        this._adicionarEventos();
    },
    _isPermitirAlterarAgendamento: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento) && _CONFIGURACAO_TMS.ControlarAgendamentoSKU && (parseInt(cargaSelecionada.CodigoAgendamentoColeta) > 0);
    },
    _isPermitirAlterarHorarioDescarregamento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && !EnumSituacaoCargaJanelaDescarregamento.isSituacaoDescarregamentoFinalizado(cargaSelecionada.Situacao) && (!cargaSelecionada.ExibirJanelaDescargaPorPedido || cargaSelecionada.PermiteInformarAcaoParcial) && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirAlterarParaAguardandoCarregamento: function (cargaSelecionada) {
        if (_situacoesCadastradas.length == 0)
            return false;

        if (_situacoesCadastradas.filter(situacaoCadastrada => situacaoCadastrada.Situacao == EnumSituacaoCargaJanelaDescarregamentoCadastrada.AguardandoCarregamento).length == 0)
            return false;

        return _situacoesCadastradas.filter(situacaoCadastrada => situacaoCadastrada.Codigo == cargaSelecionada.CodigoSituacaoCadastrada && situacaoCadastrada.Situacao == EnumSituacaoCargaJanelaDescarregamentoCadastrada.AguardandoCarregamento).length == 0;
    },
    _isPermitirBloquearDescarga: function (cargaSelecionada) {
        if (_situacoesCadastradas.length == 0)
            return false;

        if (_situacoesCadastradas.filter(situacaoCadastrada => situacaoCadastrada.Situacao == EnumSituacaoCargaJanelaDescarregamentoCadastrada.BloqueadaParaDescarga).length == 0)
            return false;

        return _situacoesCadastradas.filter(situacaoCadastrada => situacaoCadastrada.Codigo == cargaSelecionada.CodigoSituacaoCadastrada && situacaoCadastrada.Situacao == EnumSituacaoCargaJanelaDescarregamentoCadastrada.BloqueadaParaDescarga).length == 0;
    },
    _isPermitirBuscarSenhaAgendamento: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        if (_FormularioSomenteLeitura)
            return false;

        var valido = true;

        if (cargaSelecionada != undefined)
            valido = (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha);

        return valido && (_tiposIntegracao.filter(function (tipo) {
            return tipo.value == EnumTipoIntegracao.SAD;
        }).length > 0);
    },
    _isPermitirCancelarAgendamento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && !EnumSituacaoCargaJanelaDescarregamento.isSituacaoDescarregamentoFinalizado(cargaSelecionada.Situacao) && (!cargaSelecionada.ExibirJanelaDescargaPorPedido || cargaSelecionada.PermiteInformarAcaoParcial) && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirConfirmarAgendamento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && _CONFIGURACAO_TMS.UtilizarSituacaoNaJanelaDescarregamento && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.Nucleo);
    },
    _isPermitirDesagendarCarga: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && !EnumSituacaoCargaJanelaDescarregamento.isSituacaoDescarregamentoFinalizado(cargaSelecionada.Situacao) && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirFinalizarDescarregamento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && _CONFIGURACAO_TMS.UtilizarSituacaoNaJanelaDescarregamento && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.ChegadaConfirmada);
    },
    _isPermitirVisualizarCargaDevolvidaouEntregueParcialmente: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && _CONFIGURACAO_TMS.UtilizarSituacaoNaJanelaDescarregamento && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado);
    },
    _isPermitirInformarAcaoParcial: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.ChegadaConfirmada) && cargaSelecionada.PermiteInformarAcaoParcial;
    },
    _isPermitirImprimirAgendamento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && (cargaSelecionada.CodigoAgendamentoColeta > 0 || cargaSelecionada.CodigoAgendamentoPallet > 0);
    },
    _isPermitirInformarCargaDevolvida: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.ChegadaConfirmada);
    },
    _isPermitirInformarObservacaoFluxoPatio: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && cargaSelecionada.PermitirInformarObservacaoFluxoPatio;
    },
    _isPermitirInformarSenhaAgendamento: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return !_FormularioSomenteLeitura && _CONFIGURACAO_TMS.NaoGerarSenhaAgendamento && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
    },
    _isPermitirNaoComparecimento: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
    },
    _isPermitirSetarDataPrevisaoChegada: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirConfirmarChegada: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirConfirmarSaidaVeiculo: function (cargaSelecionada) {
        if (cargaSelecionada.CodigoAgendamentoPallet > 0)
            return false;

        return cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirVisualizarDadosMonitoramento: function (cargaSelecionada) {
        return cargaSelecionada.CodigoMonitoramento > 0;
    },
    _isVisivelHistoricoIntegracao: function (cargaSelecionada) {
        if (cargaSelecionada != undefined)
            return cargaSelecionada.QuantidadeArquivoIntegracao > 0

        return false;
    },
    _isPermitirAdicionarDescargaArmazemExterno: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && _centroDescarregamentoAtual.PermitirGerarDescargaArmazemExterno && cargaSelecionada.Situacao != EnumSituacaoCargaJanelaDescarregamento.Cancelado;
    },
    _isPermitirVisualizarAVIPED: function () {
        return _CONFIGURACAO_TMS.PossuiIntegracaoBoticario;
    },
    _isPermirirReagendar: function (cargaSelecionada) {
        return !!((_CONFIGURACAO_TMS.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento && cargaSelecionada.Situacao == EnumSituacaoCargaJanelaDescarregamento.NaoComparecimento) && (cargaSelecionada.SituacaoCarga != EnumSituacoesCarga.Cancelada && cargaSelecionada.SituacaoCarga != EnumSituacoesCarga.Anulada));
    },
    _isPermitirVisualizarAgendamentoPallet: function (cargaSelecionada) {
        return cargaSelecionada.CodigoAgendamentoPallet > 0;
    },
    _isPermitirVisualzarValidacaoFiscal: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.Nucleo || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento);
    },
    _isPermitirVisualzarNucleo: function (cargaSelecionada) {
        return !_FormularioSomenteLeitura && (cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal || cargaSelecionada.Situacao === EnumSituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento);
    },
    _loadGridTabelaDescarregamento: function () {
        var self = this;
        var exibirPaginacao = _CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo;
        var totalRegistrosPorPagina = _CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo ? 20 : 99999;
        var configExportacao = {
            url: "JanelaDescarga/ExportarTabelaDescarregamento",
            titulo: "Tabela Descarregamento"
        };

        var opcaoAguardandoCarregamento = { descricao: "Aguardando Carregamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._aguardandoCarregamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirAlterarParaAguardandoCarregamento };
        var opcaoAlterarAgendamento = { descricao: "Alterar Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._alterarAgendamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirAlterarAgendamento };
        var opcaoAlterarHorario = { descricao: (_CONFIGURACAO_TMS.AlterarDataCarregamentoEDescarregamentoPorPeriodo ? "Alterar Data do Agendamento" : "Alterar Horário"), id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._alterarHorarioDescarregamento(cargaSelecionada, false); }, tamanho: "10", icone: "", visibilidade: self._isPermitirAlterarHorarioDescarregamento };
        var opcaoBloquearDescarga = { descricao: "Bloquear Descarga", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._bloquearDescarga(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirBloquearDescarga };
        var opcaoBuscarSenhaAgendamento = { descricao: "Buscar Senha do Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._buscarSenhaAgendamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirBuscarSenhaAgendamento };
        var opcaoCancelarAgendamento = { descricao: "Cancelar Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._modalMotivoCancelamentoAgendamento(cargaSelecionada);/*self._cancelarAgendamento(cargaSelecionada);*/ }, tamanho: "10", icone: "", visibilidade: self._isPermitirCancelarAgendamento };
        var opcaoCargaDevolvida = { descricao: "Carga Devolvida", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._informarCargaDevolvida(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirInformarCargaDevolvida };
        var opcaoCargaDevolvidaParcialmente = {
            descricao: "Carga Devolvida Parcialmente", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._informarCargaDevolvidaParcialmente(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: function (cargaSelecionada) { return (self._isPermitirInformarAcaoParcial(cargaSelecionada) && !self._isPermitirVisualizarAgendamentoPallet(cargaSelecionada)) }
        };
        var opcaoCargaEntregueParcialmente = { descricao: "Carga Entregue Parcialmente", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._informarCargaEntregueParcialmente(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirInformarAcaoParcial };
        var opcaoCargaDevolvidaouEntregueParcialmente = { descricao: "Visualizar Devolução/Entrega Parcial", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._visualizarCargaDevolvidaouEntregueParcialmente(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualizarCargaDevolvidaouEntregueParcialmente };
        var opcaoComposicaoHorarioDescarregamento = { descricao: "Composição do Horário", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._composicaoHorarioCarregamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: !self._isPermitirVisualizarAgendamentoPallet };
        var opcaoConfirmarAgendamento = { descricao: "Confirmar Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._confirmarAgendamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirConfirmarAgendamento };
        var opcaoDesagendarCarga = { descricao: "Desagendar Carga", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._desagendarCarga(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirDesagendarCarga };
        var opcaoDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: self._exibirDetalhes, tamanho: "10", icone: "" };
        var opcaoDetalhesMonitoramento = { descricao: "Detalhes do Monitoramento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._exibirDetalhesMonitoramento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualizarDadosMonitoramento };
        var opcaoDisponibilidadeVeiculo = { descricao: "Disponibilidade do Veículo", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._salvarDataPrevisaoChegada(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: function (cargaSelecionada) { return self._isPermitirSetarDataPrevisaoChegada(cargaSelecionada); } };
        var opcaoConfirmarChegada = { descricao: "Confirmar Chegada", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._confirmarChegada(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirConfirmarChegada };
        var opcaoConfirmarSaidaVeiculo = { descricao: "Confirmar Saída de Veículo", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._confirmarSaidaVeiculo(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirConfirmarSaidaVeiculo };
        var opcaoFinalizarDescarregamento = { descricao: "Finalizar Descarregamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._finalizarDescarregamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirFinalizarDescarregamento };
        var opcaoHistoricoIntegracao = { descricao: "Histórico de Integração", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._historicoIntegracao(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isVisivelHistoricoIntegracao };
        var opcaoHistoricoMonitoramento = { descricao: "Histórico do Monitoramento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._exibirHistoricoMonitoramento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualizarDadosMonitoramento };
        var opcaoImprimirAgendamento = { descricao: "Imprimir Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._imprimirAgendamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirImprimirAgendamento };
        var opcaoInformarSenhaAgendamento = { descricao: "Informar Senha do Agendamento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._informarSenhaAgendamento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirInformarSenhaAgendamento };
        var opcaoNaoComparecimento = { descricao: "Não Comparecimento", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._informarNaoComparecimento(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirNaoComparecimento };
        var opcaoObservacaoFluxoPatio = { descricao: "Observação do Fluxo de Pátio", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { exibirObservacaoFluxoPatio(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: function (cargaSelecionada) { return self._isPermitirInformarObservacaoFluxoPatio(cargaSelecionada); } };
        var adicionarDescargaArmazemExterno = { descricao: "Descarga Armazém Externo", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { exibirModalDescargaArmazemExterno(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirAdicionarDescargaArmazemExterno };
        var opcaoVisualizarAVIPED = { descricao: Localization.Resources.Cargas.Carga.VisualizarAVIPED, id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { exibirModalIntegracaoAVIPED(cargaSelecionada); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualizarAVIPED };
        var reagendar = { descricao: "Reagendar", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._alterarHorarioDescarregamento(cargaSelecionada, true); }, tamanho: "10", icone: "", visibilidade: self._isPermirirReagendar };
        var validacaoFiscal = { descricao: "Validação Fiscal", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._alterarSituacao(cargaSelecionada, EnumSituacaoCargaJanelaDescarregamento.ValidacaoFiscal); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualzarValidacaoFiscal };
        var nucleo = { descricao: "Validação Núcleo", id: guid(), evento: "onclick", metodo: function (cargaSelecionada) { self._alterarSituacao(cargaSelecionada, EnumSituacaoCargaJanelaDescarregamento.Nucleo); }, tamanho: "10", icone: "", visibilidade: self._isPermitirVisualzarNucleo };

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [opcaoAguardandoCarregamento, reagendar, opcaoAlterarAgendamento, opcaoAlterarHorario, opcaoBloquearDescarga, opcaoBuscarSenhaAgendamento, opcaoCancelarAgendamento, opcaoCargaDevolvida, opcaoCargaDevolvidaParcialmente, opcaoCargaEntregueParcialmente, opcaoCargaDevolvidaouEntregueParcialmente, opcaoComposicaoHorarioDescarregamento, opcaoConfirmarAgendamento, opcaoDesagendarCarga, opcaoDetalhes, opcaoDetalhesMonitoramento, opcaoDisponibilidadeVeiculo, opcaoConfirmarChegada, opcaoFinalizarDescarregamento, opcaoHistoricoIntegracao, opcaoHistoricoMonitoramento, opcaoImprimirAgendamento, opcaoInformarSenhaAgendamento, opcaoNaoComparecimento, opcaoObservacaoFluxoPatio, opcaoConfirmarSaidaVeiculo, adicionarDescargaArmazemExterno, opcaoVisualizarAVIPED, validacaoFiscal, nucleo], tamanho: 10 };

        var multiplaEscolha = _CONFIGURACAO_TMS.TransformarJanelaDeDescarregamentoEmMultiplaSelecao ? {
            basicGrid: null,
            eventos: {},
            selecionados: new Array(),
            naoSelecionados: new Array(),
            callbackNaoSelecionado: function (argumentoNulo, cargaSelecionada) { self._callbackNaoSelecionado(cargaSelecionada); },
            callbackSelecionado: function (argumentoNulo, cargaSelecionada) { self._callbackSelecionado(cargaSelecionada); },
            somenteLeitura: false,
            classeSelecao: self._classeCargaSelecionada
        } : null;

        this._gridTabelaDescarregamento = new GridView("grid-tabela-descarregamento", "JanelaDescarga/Pesquisa", self._pesquisaTabelaJanelaDescarregamento, menuOpcoes, null, totalRegistrosPorPagina, null, false, undefined, multiplaEscolha, totalRegistrosPorPagina, null, configExportacao, undefined, exibirPaginacao, null, self._callbackColumnDefault);
        this._gridTabelaDescarregamento.SetPermitirEdicaoColunas(true);
        this._gridTabelaDescarregamento.SetSalvarPreferenciasGrid(true);
    },
    _obterCodigos: function (cargaSelecionada) {
        var codigos = new Array();

        if (cargaSelecionada != undefined)
            codigos.push(cargaSelecionada.Codigo);
        else
            this._gridTabelaDescarregamento.ObterMultiplosSelecionados().forEach(function (registro) {
                codigos.push(registro.Codigo);
            });

        return codigos;
    },
    _obterCodigosAgendamentoColetaPedido: function (cargaSelecionada) {
        var codigos = new Array();

        if (cargaSelecionada != undefined)
            codigos.push(cargaSelecionada.CodigoAgendamentoColetaPedido);
        else
            this._gridTabelaDescarregamento.ObterMultiplosSelecionados().forEach(function (registro) {
                codigos.push(registro.CodigoAgendamentoColetaPedido);
            });

        return codigos;
    },
    _obterHtml: function () {
        var html = '';

        html += '<div>';

        if (!_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) {
            html += '    <div class="fc-toolbar">';
            html += '        <div class="fc-left">';
            html += '            <h2 id="' + this._idContainerTabela + '-data-description"></h2>';
            html += '        </div>';
            html += '        <div class="fc-right">';
            html += '            <div class="fc-button-group">';
            html += '                <button type="button" class="fc-prev-button fc-button fc-state-default fc-corner-left" id="' + this._idContainerTabela + '-prev">';
            html += '                    <span class="fc-icon fc-icon-left-single-arrow"></span>';
            html += '                </button>';
            html += '                <button type="button" class="fc-next-button fc-button fc-state-default fc-corner-right" id="' + this._idContainerTabela + '-next">';
            html += '                    <span class="fc-icon fc-icon-right-single-arrow"></span>';
            html += '                </button>';
            html += '            </div>';
            html += '        </div>';
            html += '        <div class="fc-center">';
            html += '            </div><div class="fc-clear">';
            html += '        </div>';
            html += '    </div>';
        }

        html += '    <div class="fc-view-container">';
        html += '        <div class="fc-view fc-agendaDay-view fc-agenda-view">';
        html += '            <table>';

        if (!_CONFIGURACAO_TMS.ExibirJanelaDescargaPorPeriodo) {
            html += '                <thead class="fc-head">';
            html += '                    <tr>';
            html += '                        <td class="fc-head-container fc-widget-header">';
            html += '                            <div class="fc-row fc-widget-header">';
            html += '                                <table>';
            html += '                                    <thead>';
            html += '                                        <tr><th class="fc-day-header fc-widget-header fc-tue" id="' + this._idContainerTabela + '-day-of-week"></th></tr>';
            html += '                                    </thead>';
            html += '                                </table>';
            html += '                            </div>';
            html += '                        </td>';
            html += '                    </tr>';
            html += '                </thead>';
        }

        html += '                <tbody class="fc-body">';
        html += '                    <tr>';
        html += '                        <td class="fc-widget-content fc-widget-content-table">';
        html += '                            <div class="widget-body no-padding-bottom" id="container-grid-tabela-descarregamento">';
        html += '                                <table width="100%" class="table table-bordered table-hover table-condensed table-striped tableCursorMove" id="grid-tabela-descarregamento" cellspacing="0"></table>';
        html += '                            </div>';
        html += '                            <div class="d-flex justify-content-end">';
        html += '                                <div class="btn-group dropup d-none mt-2" id="botao-opcoes-multipla-selecao">';
        html += '                                    <button type="button" class="btn btn-warning waves-effect waves-themed rounded" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
        html += '                                        <i class="fal fa-list"></i> <span>Opções</span>';
        html += '                                    </button>';
        html += '                                    <div class="dropdown-menu">';
        html += '                                        <a class="dropdown-item" id="alterar-horario-descarregamento">Alterar Horário Descarregamento</a>';
        html += '                                        <a class="dropdown-item" id="buscar-senha-agendamento">Buscar Senha Agendamento</a>';
        html += '                                        <a class="dropdown-item" id="cancelar-agendamento">Cancelar Agendamento</a>';
        html += '                                        <a class="dropdown-item" id="carga-devolvida">Carga devolvida</a>';
        html += '                                        <a class="dropdown-item" id="confirmar-agendamento">Confirmar Agendamento</a>';
        html += '                                        <a class="dropdown-item" id="finalizar-descarregamento">Finalizar descarregamento</a>';
        html += '                                        <a class="dropdown-item" id="nao-comparecimento">Não comparecimento</a>';
        html += '                                        <a class="dropdown-item" id="alterar-situacao-para-validacao-fiscal">Validação Fiscal</a>';
        html += '                                        <a class="dropdown-item" id="alterar-situacao-para-nucleo">Validação Núcleo</a>';
        html += '                                    </div>';
        html += '                                </div>';
        html += '                            </div>';
        html += '                        </td>';
        html += '                    </tr>';
        html += '                </tbody>';
        html += '            </table>';
        html += '        </div>';
        html += '    </div>';
        html += '</div>';

        return html;
    },
    _recarregarGridTabelaDescarregamento: function () {
        var self = this;

        self._gridTabelaDescarregamento.AtualizarRegistrosSelecionados([]);
        self._gridTabelaDescarregamento.CarregarGrid(function () {
            _RequisicaoIniciada = false;
            self._controlarExibicaoMultiplasOpcoes();
        });
    },
    _salvarDataPrevisaoChegada: function (cargaSelecionada) {
        exibirModalDisponibilidadeVeiculo(cargaSelecionada.Codigo);
    },
    _confirmarChegada: function (registro) {
        exibirModalConfirmarChegada(registro);
    },
    _confirmarSaidaVeiculo: function (cargaSelecionada) {
        var self = this;

        exibirConfirmacao("Confirmação", "Deseja realmente confirmar a saída do veículo desta carga?", function () {
            executarReST("JanelaDescarga/ConfirmarSaidaVeiculo", { Codigo: (cargaSelecionada.Codigo) }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Saída do veículo confirmada com sucesso!");
                        self._carregarCargas();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
}

var PesquisaTabelaJanelaDescarregamento = function () {
    this.CentroDescarregamento = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.DataDescarregamento = PropertyEntity({ getType: typesKnockout.date, def: Global.DataAtual() });
    this.DataDescarregamentoInicial = PropertyEntity({ getType: typesKnockout.date, def: Global.DataAtual() });
    this.DataDescarregamentoFinal = PropertyEntity({ getType: typesKnockout.date, def: Global.DataAtual() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCargaJanelaCarregamento.Todas), def: EnumSituacaoCargaJanelaCarregamento.Todas });
    this.SituacaoAgendamentoPallet = PropertyEntity({ val: ko.observable(EnumSituacaoAgendamentoPallet.Todas), def: EnumSituacaoAgendamentoPallet.Todas });
    this.ExcedenteDescarregamento = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos2), def: EnumSimNaoPesquisa.Todos2 });
    this.SituacaoCadastrada = PropertyEntity({});
    this.SenhaAgendamento = PropertyEntity({});
    this.NumeroPedido = PropertyEntity({});
    this.NumeroCarga = PropertyEntity({});
    this.DataLancamento = PropertyEntity({ getType: typesKnockout.date });
    this.Fornecedor = PropertyEntity({ getType: typesKnockout.decimal });
    this.Veiculo = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.TipoCarga = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.Transportador = PropertyEntity({ type: types.int, val: ko.observable(0), def: 0 });
    this.NumeroNF = PropertyEntity({});
    this.NumeroCTe = PropertyEntity({});
    this.NumeroLacre = PropertyEntity({});
}
