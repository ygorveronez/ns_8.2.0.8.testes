/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="..\..\Enumeradores\EnumSituacoesCarga.js" />
/// <reference path="CapacidadeCarregamentoDados.js" />
/// <reference path="CargaPendente.js" />
/// <reference path="CotacaoHistorico.js" />
/// <reference path="DadosEventoCarga.js" />
/// <reference path="DadosGradeTipoOperacao.js" />
/// <reference path="DetalheCarga.js" />
/// <reference path="JanelaCarregamento.js" />
/// <reference path="Observacoes.js" />
/// <reference path="PossiveisVeiculos.js" />
/// <reference path="SolicitarReagendamento.js" />
/// <reference path="TipoTransportadorCarga.js" />
/// <reference path="TransportadoresOfertados.js" />
/// <reference path="../../Cargas/Carga/DadosTransporte/VeiculosHUB.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />

var CalendarioCarregamento = function () {
    this._idContainerCalendario = "#divListaCarregamento";
    this._dataAtual;
    this._cargaMovidaParaExcedente = 0;
    
    this._init();
}

CalendarioCarregamento.prototype = {
    AdicionarOuAtualizarCarga: function (dadosJanelaCarregamento) {
        var eventos = $(this._idContainerCalendario).fullCalendar('clientEvents', dadosJanelaCarregamento.Codigo);

        if ((eventos != null) && (eventos.length > 0))
            $(this._idContainerCalendario).fullCalendar('updateEvent', this._obterDadosEventoCarga(dadosJanelaCarregamento, eventos[0]));
        else {
            $(this._idContainerCalendario).fullCalendar('renderEvent', this._obterDadosEventoCarga(dadosJanelaCarregamento));
            this._atualizaGradePeriodo(moment(dadosJanelaCarregamento.InicioCarregamento, "DD/MM/YYYY HH:mm"));
        }

        buscarCapacidadeCarregamentoDados();
    },
    RemoverCarga: function (dadosJanelaCarregamento) {
        var eventos = $(this._idContainerCalendario).fullCalendar('clientEvents', dadosJanelaCarregamento.Codigo);

        if ((eventos != null) && (eventos.length > 0)) {
            $(this._idContainerCalendario).fullCalendar('removeEvents', eventos[0].id);
            this._atualizaGradePeriodo(eventos[0].start);
        }
    },
    Destroy: function () {
        this._removerEventos();

        $(this._idContainerCalendario).fullCalendar('destroy');
        $(this._idContainerCalendario).html("");
    },
    Load: function () {
        var self = this;
        var dataCarregamento = _dadosPesquisaCarregamento.DataCarregamento ? moment(_dadosPesquisaCarregamento.DataCarregamento, "DD/MM/YYYY") : moment();

        self.Destroy();

        $(self._idContainerCalendario).fullCalendar({
            header: {
                left: 'title',
                center: '',
                right: 'prev,next'
            },
            defaultView: 'agendaDay',
            allDaySlot: false,
            allDayDefault: false,
            forceEventDuration: true,
            lang: _CONFIGURACAO_TMS.Culture.toLowerCase(),
            height: 607,
            scrollTime: self._obterScrollTime(),
            defaultDate: dataCarregamento,
            slotDuration: '00:05:00',
            slotLabelFormat: 'HH:mm',
            slotEventOverlap: false,
            snapDuration: "00:01:00",
            timeFormat: 'HH:mm',
            displayEventTime: true,
            displayEventEnd: true,
            nowIndicator: true,
            dropAccept: ".carga-aceita-calendario",
            droppable: true,
            events: function (start, end, timezone, callback) {
                self._dataAtual = start.format("DD/MM/YYYY");
                self._carregarCargas(callback);
                buscarCapacidadeCarregamentoDados();
            },
            eventRender: function (event, element) {
                if (event.carga != null)
                    return self._renderizarEventoCarga(event, element);

                if (event.gradeTipoOperacao)
                    return self._renderizarEventoGradeTipoOperacao(event, element);
            },
            eventDrop: self._adicionarCarregamento.bind(self),
            eventReceive: function (event) { self._alterarHorarioCarregamento(event); },
            eventClick: self._exibirDetalhes,
            eventDragStop: function (event, jsEvent) {
                self._removerCarregamentoParaCargasExcedentes(event, jsEvent);
            }
        });
    },
    ObterData: function () {
        return this._dataAtual;
    },
    Render: function () {
        $(this._idContainerCalendario).fullCalendar('render');
    },
    SetAltura: function (altura) {
        var fc = $(this._idContainerCalendario).data('fullCalendar');
        fc.option('height', altura);
        $(this._idContainerCalendario).css("min-height", altura + "px");
    },
    CargaMovidaParaExcedente: function () {
        if (arguments.length == 0)
            return this._cargaMovidaParaExcedente;
        else
            this._cargaMovidaParaExcedente = arguments[0];
    },
    _renderizarEventoCarga: function (event, element) {
        var self = this;

        // Seta o conteúdo como HTML
        element.find('.fc-content').html(event._html);

        // Habilita o menu do botão direito
        element.contextmenu(function (e) {
            var menu = document.querySelectorAll("#ulMenu");
            var menuLateral = document.getElementById("left-panel");

            menu[0].style.display = 'block';
            menu[0].style.left = e.clientX - ((menuLateral.offsetWidth > 400 ? 0 : menuLateral.offsetWidth) + menuLateral.offsetLeft) - 60 + $(document).scrollLeft() + 'px';
            menu[0].style.top = e.clientY - (menuLateral.offsetWidth > 400 ? 120 : 60) + $(document).scrollTop() + 'px';

            $("#menuAreaVeiculo").unbind();
            $("#menuAreaVeiculo").click(function () {
                exibirAreaVeiculo(event.carga.Codigo);
            });

            $("#menuBloquearCotacao").unbind();
            $("#menuBloquearCotacao").click(function () {
                bloquearCargaCotacao(event.carga.Codigo);
            });

            $("#menuBloquearFilaCarregamento").unbind();
            $("#menuBloquearFilaCarregamento").click(function () {
                bloquearCargaFilaCarregamento(event.carga.Codigo);
            });

            $("#menuFluxoCarga").unbind();
            $("#menuFluxoCarga").click(function () {
                ExibirDetalhesCarga(event.carga);
            });

            $("#menuDetalhesCarga").unbind();
            $("#menuDetalhesCarga").click(function () {
                exibirDetalhesPedidos(event.carga.Carga.Codigo);
            });

            $("#menuDesagendarCarga").unbind();
            $("#menuDesagendarCarga").click(function () {
                desagendarCarga(event.carga.Carga.Codigo);
            });

            $("#menuDescartarCotacao").unbind();
            $("#menuDescartarCotacao").click(function () {
                descartarCargaCotacao(event.carga.Codigo);
            });

            $("#menuHistoricoCotacao").unbind();
            $("#menuHistoricoCotacao").click(function () {
                visualizarHistoricoCotacao(event.carga.Codigo);
            });

            $("#menuHistoricoTransportadoresInteressadosCarga").unbind();
            $("#menuHistoricoTransportadoresInteressadosCarga").click(function () {
                AbrirTelaHistoricoTransportadoresInteressadosCarga(event.carga.Carga.Codigo);
            });

            $("#menuLiberarCotacao").unbind();
            $("#menuLiberarCotacao").click(function () {
                liberarCargaCotacao(event.carga.Codigo);
            });

            $("#menuLiberarFilaCarregamento").unbind();
            $("#menuLiberarFilaCarregamento").click(function () {
                liberarCargaFilaCarregamento(event.carga.Codigo);
            });

            $("#menuObservacaoTransportador").unbind();
            $("#menuObservacaoTransportador").click(function () {
                exibirObservacaoTransportador(event.carga);
            });

            $("#menuObservacaoFluxoPatio").unbind();
            $("#menuObservacaoFluxoPatio").click(function () {
                exibirObservacaoFluxoPatio(event.carga);
            });

            $("#menuObservacaoGuarita").unbind();
            $("#menuObservacaoGuarita").click(function () {
                exibirObservacaoGuarita(event.carga);
            });

            $("#menuTransportadoresOfertados").unbind();
            $("#menuTransportadoresOfertados").click(function () {
                visualizarTransportadoresOfertados(event.carga.Codigo);
            });

            $("#menuVeiculosDisponiveis").unbind();
            $("#menuVeiculosDisponiveis").click(function () {
                visualizarVeiculosDisponviveisClick(event.carga);
            });

            $("#menuSolicitarReagendamento").unbind();
            $("#menuSolicitarReagendamento").click(function () {
                exibirSolicitarReagendamento(event.carga);
            });

            $("#menuReverterSituacaoNoShowCarga").unbind();
            $("#menuReverterSituacaoNoShowCarga").click(function () {
                ReverterSituacaoNoShowCarga(event.carga);
            });

            $("#menuConfirmarRetirada").unbind();
            $("#menuConfirmarRetirada").click(function () {
                console.log(event.carga);
                ConfirmarRetirada(event.carga);
            });

            $("#menuRejeitarCargaTransportador").unbind();
            $("#menuRejeitarCargaTransportador").click(function () {
                RejeitarCargaJanelaCarregamentoTransportador(event.carga);
            });

            $("#menuRetornarParaNovaLiberacao").unbind();
            $("#menuRetornarParaNovaLiberacao").click(function () {
                retornarParaNovaLiberacao(event.carga);
            });

            $("#menuConsultarVeiculosSugeridosHUB").unbind();
            $("#menuConsultarVeiculosSugeridosHUB").click(function () {
                exibirModalVeiculosSugeridosHUB(event.carga);
            });

            if (self._isPermitirInformarAreaVeiculo(event.carga))
                $("#limenuAreaVeiculo").show();
            else
                $("#limenuAreaVeiculo").hide();

            if (self._isPermitirBloquearCargaCotacao(event.carga))
                $("#limenuBloquearCotacao").show();
            else
                $("#limenuBloquearCotacao").hide();

            if (self._isPermitirBloquearCargaFilaCarregamento(event.carga))
                $("#limenuBloquearFilaCarregamento").show();
            else
                $("#limenuBloquearFilaCarregamento").hide();

            if (self._isPermitirExibirDetalhes(event.carga))
                $("#limenuFluxoCarga").show();
            else
                $("#limenuFluxoCarga").hide();

            if (self._isPermitirExibirDetalhes(event.carga))
                $("#limenuDetalhesCarga").show();
            else
                $("#limenuDetalhesCarga").hide();

            if (self._isPermitirDesagendarCarga(event.carga))
                $("#limenuDesagendarCarga").show();
            else
                $("#limenuDesagendarCarga").hide();

            if (self._isPermitirDescartarCargaCotacao(event.carga))
                $("#limenuDescartarCotacao").show();
            else
                $("#limenuDescartarCotacao").hide();

            if (self._isPermitirExibirHistoricoCotacao(event.carga))
                $("#limenuHistoricoCotacao").show();
            else
                $("#limenuHistoricoCotacao").hide();

            if (self._isPermitirExibirHistoricoTransportadoresInteressadosCarga(event.carga))
                $("#limenuHistoricoTransportadoresInteressadosCarga").show();
            else
                $("#limenuHistoricoTransportadoresInteressadosCarga").hide();

            if (self._isPermitirLiberarCargaCotacao(event.carga))
                $("#limenuLiberarCotacao").show();
            else
                $("#limenuLiberarCotacao").hide();

            if (self._isPermitirLiberarCargaFilaCarregamento(event.carga))
                $("#limenuLiberarFilaCarregamento").show();
            else
                $("#limenuLiberarFilaCarregamento").hide();

            if (self._isPermitirInformarObservacaoTransportador(event.carga))
                $("#menuObservacaoTransportador").show();
            else
                $("#menuObservacaoTransportador").hide();

            if (self._isPermitirInformarObservacaoFluxoPatio(event.carga))
                $("#menuObservacaoFluxoPatio").show();
            else
                $("#menuObservacaoFluxoPatio").hide();

            if (self._isPermitirInformarObservacaoGuarita(event.carga))
                $("#menuObservacaoGuarita").show();
            else
                $("#menuObservacaoGuarita").hide();

            if (self._isPermitirExibirTransportadoresOfertados(event.carga))
                $("#limenuTransportadoresOfertados").show();
            else
                $("#limenuTransportadoresOfertados").hide();

            if (self._isPermitirExibirVeiculos(event.carga))
                $("#limenuVeiculosDisponiveis").show();
            else
                $("#limenuVeiculosDisponiveis").hide();

            if (self._isPermitirSolicitarReagendamento(event.carga))
                $("#limenuSolicitarReagendamento").show();
            else
                $("#limenuSolicitarReagendamento").hide();

            $("#limenuReverterSituacaoNoShowCarga").show();

            if (self._isPermitirConfirmarRetirada(event.carga))
                $("#limenuConfirmarRetirada").show();
            else
                $("#limenuConfirmarRetirada").hide();

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
                $("#limenuRejeitarCargaTransportador").show();
            else
                $("#limenuRejeitarCargaTransportador").hide();

            if (self._isPermitirRetornarParaNovaLiberacao(event.carga))
                $("#limenuRetornarParaNovaLiberacao").show();
            else
                $("#limenuRetornarParaNovaLiberacao").hide();

            if (_CONFIGURACAO_TMS.PossuiIntegracaoHUBOfertas) 
                $("#limenuConsultarVeiculosSugeridosHUB").show();
            else
                $("#limenuConsultarVeiculosSugeridosHUB").hide();

            e.preventDefault();
        });
    },
    _renderizarEventoGradeTipoOperacao: function (event, element) {
        element.find('.fc-content').html(event._html);
    },
    _adicionarCarregamento: function (event, delta, revertFunc) {
        var _confirmarNaoComparecimento = function (setarComoNaoComparecimento) {
            executarReST("JanelaCarregamento/AlocarHorarioCarregamento", { Codigo: event.carga.Codigo, NovoHorario: event.start.format("DD/MM/YYYY HH:mm"), NaoComparecimento: setarComoNaoComparecimento }, function (r) {
                if (r.Success) {
                    if (r.Data)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.HorarioAlocadoComSucesso);
                    else {
                        revertFunc();
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    }
                }
                else {
                    revertFunc();
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });
        };

        executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { JanelaCarregamento: event.carga.Codigo, Data: event.start.format("DD/MM/YYYY HH:mm") }, function (retornoVerificacao) {
            if (!retornoVerificacao.Success) {
                revertFunc();
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);
            }

            if (!retornoVerificacao.Data.PermiteModificarHorario) {
                revertFunc();
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retornoVerificacao.Data.MensagemPermiteModificarHorario);
            }

            if (retornoVerificacao.Data.PossibilidadeNoShow)
                exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _confirmarNaoComparecimento(true) }, function () { _confirmarNaoComparecimento(false) });
            else
                _confirmarNaoComparecimento(false);
        });
    },
    _adicionarEventos: function () {
        $('#ulMenu').on("mouseleave", function () {
            $(this).hide();
        });
    },
    _alterarHorarioCarregamento: function (event) {
        var self = this;
        executarReST("JanelaCarregamento/AlocarHorarioCarregamento", { Codigo: event.carga.Codigo, NovoHorario: event.start.format("DD/MM/YYYY HH:mm") }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.HorarioAlocadoComSucesso);

                    if (r.Data.PermiteExceder)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.Carga.CapacidadeCarregamentoPeriodoAtingida);

                    if (r.Data.InicioCarregamento)
                        self._atualizaGradePeriodo(moment(r.Data.InicioCarregamento, "DD/MM/YYYY HH:mm"));
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                    $(self._idContainerCalendario).fullCalendar('removeEvents', event.id);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                $(self._idContainerCalendario).fullCalendar('removeEvents', event.id);
            }
        });
    },
    _carregarCargas: function (callback) {
        var self = this;

        _dadosPesquisaCarregamento.DataCarregamento = self._dataAtual;

        executarReST("JanelaCarregamento/ObterInformacoesCargas", _dadosPesquisaCarregamento, function (retorno) {
            if (retorno.Success) {
                var cargas = new Array();
                _cargasCarregamento = [];

                retorno.Data.PeriodosCarregamento.forEach(function (periodo) {
                    cargas.push({
                        id: "horarioDisponivel",
                        className: "backgroundHorarioDisponivelCalendario",
                        start: moment(self._dataAtual + " " + periodo.HoraInicio, "DD/MM/YYYY HH:mm:ss"),
                        end: moment(self._dataAtual + " " + periodo.HoraTermino, "DD/MM/YYYY HH:mm:ss").add(periodo.ToleranciaExcessoTempo, "minutes"),
                        rendering: 'background',
                    });

                    if (periodo.ToleranciaExcessoTempo > 0) {
                        cargas.push({
                            id: "horarioDisponivel",
                            className: "backgroundToleranciaExcessoTempoCalendario",
                            start: moment(self._dataAtual + " " + periodo.HoraTermino, "DD/MM/YYYY HH:mm:ss"),
                            end: moment(self._dataAtual + " " + periodo.HoraTermino, "DD/MM/YYYY HH:mm:ss").add(periodo.ToleranciaExcessoTempo, "minutes"),
                            rendering: 'background'
                        });
                    }
                });

                retorno.Data.PeriodosBloqueios.forEach(function (bloqueio) {
                    cargas.push(self._obterDadosPeriodoComBloqueio(bloqueio));
                });

                retorno.Data.Cargas.forEach(function (carga) {
                    cargas.push(self._obterDadosEventoCarga(carga));
                    _cargasCarregamento.push(carga.Carga.Codigo);
                });

                retorno.Data.PeriodosCarregamento.forEach(function (periodo) {
                    if (!$.isArray(periodo.TipoOperacaoSimultaneo)) return;

                    periodo.TipoOperacaoSimultaneo.map(function (gradeTipoOperacao) {
                        cargas.push(self._obterDadosDisponbilidadeTipoOperacao(gradeTipoOperacao, {
                            DataAtual: self._dataAtual,
                            HoraInicio: periodo.HoraInicio,
                            HoraTermino: periodo.HoraTermino
                        }));
                    });
                });

                callback(cargas);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    },
    _exibirDetalhes: function (event) {
        if (event.carga && event.carga.Editavel) {
            if ((event.carga.Tipo == EnumTipoCargaJanelaCarregamento.Carregamento) && (event.carga.Situacao == EnumSituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && ((event.carga.Carga.SituacaoCarga != EnumSituacoesCarga.Nova) || event.carga.Carga.ExigeNotaFiscalParaCalcularFrete))
                exibirTipoTransportadorCarga(event.carga);
            else
                ExibirDetalhesCarga(event.carga);
        }
    },
    _init: function () {
        $(this._idContainerCalendario).css("min-height", "600px");

        this._adicionarEventos();
    },
    _isItemSoltadoAbaCargasExcedentes: function (x, y) {
        var $abaCargasExcedentes = $('#tabCargasExcedentes');

        if ($abaCargasExcedentes.css("display") !== "none") {
            var offset = $abaCargasExcedentes.offset();

            offset.right = $abaCargasExcedentes.width() + offset.left;
            offset.bottom = $abaCargasExcedentes.height() + offset.top;
            y += $(document).scrollTop();

            return (x >= offset.left) && (y >= offset.top) && (x <= offset.right) && (y <= offset.bottom);
        }

        return false;
    },
    _isPermitirBloquearCargaCotacao: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.CargaLiberadaCotacao && !janelaCarregamentoSelecionada.CargaLiberadaCotacaoAutomaticamente && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirDescartarCargaCotacao: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (janelaCarregamentoSelecionada.CargaLiberadaCotacao || janelaCarregamentoSelecionada.ProcessoCotacaoFinalizada) && !janelaCarregamentoSelecionada.CargaLiberadaCotacaoAutomaticamente && ((janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador) || (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento));
    },
    _isPermitirBloquearCargaFilaCarregamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirBloquearFilaCarregamentoManualmente && (janelaCarregamentoSelecionada.Situacao != EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
    },
    _isPermitirDesagendarCarga: function (janelaCarregamentoSelecionada) {
        if (!this._isPermitirEditar(janelaCarregamentoSelecionada) || janelaCarregamentoSelecionada.PreCarga)
            return false;

        return _CONFIGURACAO_TMS.PermitirDesagendarCargaJanelaCarregamento;
    },
    _isPermitirEditar: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.Editavel && (janelaCarregamentoSelecionada.Tipo == EnumTipoCargaJanelaCarregamento.Carregamento);
    },
    _isPermitirExibirDetalhes: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.PreCarga == false;
    },
    _isPermitirExibirHistoricoCotacao: function (janelaCarregamentoSelecionada) {
        return janelaCarregamentoSelecionada.PossuiHistoricoCotacao;
    },
    _isPermitirExibirHistoricoTransportadoresInteressadosCarga: function (janelaCarregamentoSelecionada) {
        return (janelaCarregamentoSelecionada.Situacao !== EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirExibirTransportadoresOfertados: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (_centroCarregamentoAtual.ExibirTransportadoresOfertadosComMenorValorFreteTabela || _centroCarregamentoAtual.ExibirTransportadoresOfertadosPorPrioridadeDeRota);
    },
    _isPermitirExibirVeiculos: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && !_CONFIGURACAO_TMS.UtilizarFilaCarregamento && (janelaCarregamentoSelecionada.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirInformarAreaVeiculo: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && _centroCarregamentoAtual.PermitirInformarAreaVeiculo;
    },
    _isPermitirInformarObservacaoFluxoPatio: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirInformarObservacaoFluxoPatio;
    },
    _isPermitirInformarObservacaoGuarita: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirInformarObservacaoGuarita;
    },
    _isPermitirInformarObservacaoTransportador: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada);
    },
    _isPermitirLiberarCargaCotacao: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && !janelaCarregamentoSelecionada.CargaLiberadaCotacao && !_CONFIGURACAO_TMS.LiberarCargaParaCotacaoAoLiberarParaTransportadores && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador);
    },
    _isPermitirLiberarCargaFilaCarregamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.PermitirLiberarFilaCarregamentoManualmente && (janelaCarregamentoSelecionada.Situacao != EnumSituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
    },
    _isPermitirSolicitarReagendamento: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && janelaCarregamentoSelecionada.DatasAgendadasDivergentes;
    },
    _isPermitirConfirmarRetirada: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && Boolean(janelaCarregamentoSelecionada.PermiteConfirmarRetirada);
    },
    _isPermitirRetornarParaNovaLiberacao: function (janelaCarregamentoSelecionada) {
        return this._isPermitirEditar(janelaCarregamentoSelecionada) && (janelaCarregamentoSelecionada.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador) && (!janelaCarregamentoSelecionada.CargaLiberadaCotacaoAutomaticamente) && (janelaCarregamentoSelecionada.ExpirouTempoLimiteParaEscolhaAutomatica);
    },
    _obterDadosEventoCarga: function (carga, evento) {
        var dadosEventoCarga = new DadosEventoCarga();

        return dadosEventoCarga.ObterDadosEventoCarga(carga, evento);
    },
    _obterDadosDisponbilidadeTipoOperacao: function (grade, dados) {
        var instancia = new DadosGradeTipoOperacao();

        return instancia.ObterDadosDisponibilidade(grade, dados);
    },
    _obterDadosPeriodoComBloqueio: function (bloqueio) {
        var dadosPeriodoComBloqueio = new DadosPeriodoComBloqueio();

        return dadosPeriodoComBloqueio.ObterDadosPeriodoComBloqueio(bloqueio);
    },
    _atualizaGradePeriodo: function (start) {
        var self = this;
        var fullCalendar = $(self._idContainerCalendario);
        if (_centroCarregamentoAtual.Configuracao.ExibirVisualizacaoDosTiposDeOperacao !== true) return;

        var startDataFormatada = start.format("DD/MM/YYYY HH:mm");
        var eventosGradeDoPeriodo = fullCalendar.fullCalendar('clientEvents', function (event) {
            return ('gradeTipoOperacao' in event) && startDataFormatada == event.start.format("DD/MM/YYYY HH:mm");
        });

        eventosGradeDoPeriodo.map(function (event) {
            fullCalendar.fullCalendar('removeEvents', event.id);
        });

        var parametros = $.extend({}, _dadosPesquisaCarregamento, { InicioPeriodo: startDataFormatada });

        executarReST("JanelaCarregamento/ObterGradeTipoOperacaoDoPeriodo", parametros, function (retorno) {
            if (!retorno.Success || !retorno.Data || retorno.Data.PeriodoCarregamento == null)
                return;

            retorno.Data.PeriodoCarregamento.forEach(function (gradeTipoOperacao) {
                var eventoGrade = self._obterDadosDisponbilidadeTipoOperacao(gradeTipoOperacao, {
                    DataAtual: self._dataAtual,
                    HoraInicio: retorno.Data.HoraInicio,
                    HoraTermino: retorno.Data.HoraTermino
                });
                fullCalendar.fullCalendar('renderEvent', eventoGrade);
            });
        });
    },
    _obterDiaSemana: function () {
        var diaSemana = moment(self._dataAtual, "DD/MM/YYYY").isoWeekday();

        if (diaSemana == 7)
            return 1;

        return diaSemana + 1;
    },
    _obterHorarioFocarCalendario: function () {
        /* Hora da carga no filtro por carga */
        if (FluxoPorCarga() && _fluxoPorCarga.Codigo > 0)
            return _fluxoPorCarga.HoraCarregamento;

        /* Hora atual quando for dia atual */
        if (_dadosPesquisaCarregamento.DataCarregamento == Global.DataAtual())
            return moment().format("HH:mm:00");

        /* Período de carregamento mais cedo */
        if (_centroCarregamentoAtual.PeriodosCarregamento.length > 0) {
            var menorHora = null;

            _centroCarregamentoAtual.PeriodosCarregamento.forEach(function (periodoCarregamento) {
                var horaInicio = moment(periodoCarregamento.HoraInicio, "HH:mm:ss");

                if ((menorHora == null) || (horaInicio.diff(menorHora) < 0))
                    menorHora = horaInicio;
            });

            return menorHora.format("HH:mm:ss");
        }

        /* Hora padrão */
        return "12:00:00";
    },
    _obterScrollTime: function () {
        var horarioFocarCalendario = this._obterHorarioFocarCalendario();
        var horario = moment(horarioFocarCalendario, "HH:mm:ss");
        var isHorarioMenorDezMinutos = (horario.format('HH') == "00") && (parseInt(horario.format('mm')) < 10);

        if (isHorarioMenorDezMinutos)
            return horarioFocarCalendario;

        /* Diminui 10 minutos do horário para dar espaço visualmente */
        return horario.add(-10, 'minute').format('HH:mm:ss');
    },
    _removerCarregamentoParaCargasExcedentes: function (event, jsEvent) {
        var self = this;

        if (!self._isItemSoltadoAbaCargasExcedentes(jsEvent.clientX, jsEvent.clientY))
            return;

        var _confirmarNaoComparecimento = function (setarComoNaoComparecimento) {
            self.CargaMovidaParaExcedente(event.carga.Carga.Codigo);
            executarReST("JanelaCarregamento/MandarCargaExcedentesCarregamento", { JanelaCarregamento: event.carga.Codigo, NaoComparecimento: setarComoNaoComparecimento }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaRetornouAsExcedentes);

                        $(self._idContainerCalendario).fullCalendar('removeEvents', event.id);
                        self._atualizaGradePeriodo(event.start);

                        RecarregarCargasExcedentes();
                        buscarCapacidadeCarregamentoDados();
                    }
                    else
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        };

        executarReST("JanelaCarregamento/VerificarPossibilidadeModificacaoJanela", { JanelaCarregamento: event.carga.Codigo, Data: Global.DataHoraAtual() }, function (retornoVerificacao) {
            if (!retornoVerificacao.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retornoVerificacao.Msg);

            if (retornoVerificacao.Data.PossibilidadeNoShow)
                exibirConfirmacao(Localization.Resources.Cargas.Carga.NoShow, Localization.Resources.Cargas.Carga.DesejaMarcarCargaComoNoShow, function () { _confirmarNaoComparecimento(true) }, function () { _confirmarNaoComparecimento(false) });
            else
                _confirmarNaoComparecimento(false);
        });
    },
    _removerEventos: function () {
        $('#ulMenu').off("mouseleave", "**");
    }
}