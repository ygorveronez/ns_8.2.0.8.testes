/// <reference path="CargaPendente.js" />
/// <reference path="ControleCorCarga.js" />
/// <reference path="..\..\Enumeradores\EnumCamposVisiveisNaJanela.js" />
/// <reference path="..\..\Enumeradores\EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="..\..\Enumeradores\EnumTipoNaoComparecimento.js" />

var DadosEventoCarga = function () {
}

DadosEventoCarga.prototype = {
    ObterDadosEventoCarga: function (janelaCarregamento, evento) {
        if (evento == null) evento = {};

        evento.allDay = false;
        evento.carga = janelaCarregamento;
        evento.className = this._obterClasseCor(janelaCarregamento);
        evento.constraint = "horarioDisponivel";
        evento.duration = moment.utc().hours((janelaCarregamento.TempoCarregamento / 60 | 0)).minutes((janelaCarregamento.TempoCarregamento % 60 | 0)).format("HH:mm");
        evento.durationEditable = false;
        evento.end = moment(janelaCarregamento.TerminoCarregamento, "DD/MM/YYYY HH:mm");
        evento.id = janelaCarregamento.Codigo;
        evento.start = moment(janelaCarregamento.InicioCarregamento, "DD/MM/YYYY HH:mm");
        evento.startEditable = janelaCarregamento.Editavel;
        evento.title = janelaCarregamento.Carga.Numero;
        evento._html = this._obterHtmlDadosCarga(janelaCarregamento);

        return evento;
    },
    _obterClasseCor: function (janelaCarregamento) {
        var controleCorCarga = new ControleCorCarga();
        var classeCor = Boolean(janelaCarregamento.Cores) ? Global.ObterClasseDinamica(janelaCarregamento.Cores) : controleCorCarga.ObterClasse(janelaCarregamento);

        if ((classeCor === "well-blue") && janelaCarregamento.PermitirLiberarFilaCarregamentoManualmente && _CONFIGURACAO_TMS.UtilizarFilaCarregamento)
            classeCor = "well-blue-font-color-red";

        return classeCor;
    },
    _obterHtmlDadosCarga: function (janelaCarregamento) {
        var camposVisiveis = janelaCarregamento.CamposVisiveis.split(";");
        var html = '';

        html += this._obterHtmlInteressadosCarga(janelaCarregamento);
        html += this._obterHtmlVisualizacoesCarga(janelaCarregamento);
        html += this._obterHtmlCargaLiberadaCotacao(janelaCarregamento);
        html += this._obterHtmlFaixaInformativa(janelaCarregamento);
        html += this._obterHtmlClassificacao(janelaCarregamento);
        html += '<div class="row position-relative carga-codigo-' + janelaCarregamento.Carga.Codigo + '" style="padding: 2px;">';

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.Carga.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.DescricaoCarga + ': ', janelaCarregamento.Carga.Numero + ' - ' + janelaCarregamento.Destinatario + ' - ' + janelaCarregamento.Carga.DataCarga, 12, null, 'dados-janela-carregamento-texto-sem-radius');

        if (janelaCarregamento.DatasAgendadasDivergentes)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.DataAgendadasDivergentes, '', 12, null, 'aviso');

        if (janelaCarregamento.ChegadaDenegada)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ChegadaDenegada, '', 12, null, 'aviso');

        if (janelaCarregamento.SituacaoCotacao != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.SituacaoCotacao + ': ', janelaCarregamento.SituacaoCotacao);

        if ((janelaCarregamento.Carga.PrevisaoEntrega != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.PrevisaoEntrega.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.PrevisaoEntrega + ': ', janelaCarregamento.Carga.PrevisaoEntrega);

        if (janelaCarregamento.DataColeta)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.DataColeta + ': ', janelaCarregamento.DataColeta);

        if (janelaCarregamento.Carga.CargaPerigosa)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.CargaPerigosa + ': ', janelaCarregamento.Carga.CargaPerigosa);

        if ((janelaCarregamento.DataDisponibilizacaoTransportadores != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.Disponibilizada.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Disponibilizada + ': ', janelaCarregamento.DataDisponibilizacaoTransportadores);

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.TipoCarga.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.TipoDaCarga + ': ', janelaCarregamento.TipoCarga.Descricao);

        if (janelaCarregamento.TipoCondicaoPagamento != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.TipoDeFrete + ': ', janelaCarregamento.TipoCondicaoPagamento);

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.Veiculo.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Veiculo + ': ', janelaCarregamento.ModeloVeiculo.Descricao, 6, null, janelaCarregamento.TipoCarga.ExigeVeiculoRastreado ? 'color-danger-900' : '');

        if (janelaCarregamento.Ordem != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Ordem + ': ', janelaCarregamento.Ordem);

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.Origem.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Origem + ': ', janelaCarregamento.Origem);

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.Destino.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Destino + ': ', janelaCarregamento.Destino);

        if ((janelaCarregamento.TipoOperacao != null) && (janelaCarregamento.TipoOperacao.Descricao != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.Operacao.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Operacao + ': ', janelaCarregamento.TipoOperacao.Descricao, 6, janelaCarregamento.TipoOperacao.Cores);

        if (_centroCarregamentoAtual && (_centroCarregamentoAtual.LimiteCarregamentos == EnumLimiteCarregamentosCentroCarregamento.QuantidadeDocas) && (janelaCarregamento.Carga.PesoTotal != ''))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.PesoTotal + ': ', janelaCarregamento.Carga.PesoTotal + ' KG');

        if ((janelaCarregamento.QuantidadeEntregas != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.QuantidadeEntregas.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.QuantidadeEntregas + ': ', janelaCarregamento.QuantidadeEntregas);

        if ((janelaCarregamento.ValorTarget != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.ValorTarget.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ValorTarget + ': ', janelaCarregamento.ValorTarget);

        if ((janelaCarregamento.ValorFrete != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.ValorFrete.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ValorDoFrete + ': ', janelaCarregamento.ValorFrete, 6);

        if ((janelaCarregamento.PossuiJanelaDestino != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.PossuiJanelaDestino.toString()) && janelaCarregamento.PossuiJanelaDestino)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.PossuiJanelaDestino + ': ', Localization.Resources.Gerais.Geral.Sim, 6);

        if (janelaCarregamento.NumeroAgendamento != "" || true)
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.NumeroAgendamento + ': ', janelaCarregamento.NumeroAgendamento, 6);

        if ((janelaCarregamento.RecomendacaoGR != ""))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.RecomendacaoGR + ': ', janelaCarregamento.RecomendacaoGR);

        if (camposVisiveis.includes(EnumCamposVisiveisNaJanela.Transportador.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Transportador + ': ', janelaCarregamento.Transportador.Descricao, 12);

        if (janelaCarregamento.ObservacaoGuarita != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ObservacaoGuarita + ': ', janelaCarregamento.ObservacaoGuarita, 12);

        if (janelaCarregamento.ObservacaoLocalEntrega != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ObservacaoLocalEntrega + ':', janelaCarregamento.ObservacaoLocalEntrega, 12);

        if ((janelaCarregamento.ObservacaoTransportador != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.ObservacaoTransportador.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.ObservacaoTransportador + ': ', janelaCarregamento.ObservacaoTransportador, 12);

        if ((janelaCarregamento.EnderecoCliente != "") && camposVisiveis.includes(EnumCamposVisiveisNaJanela.EnderecoCliente.toString()))
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.EnderecoCliente + ': ', janelaCarregamento.EnderecoCliente, 12);
        
        if (janelaCarregamento.DataTerminoCotacao != "") {
            var idJanelaCarregamento = 'janelaCarregamentoId' + janelaCarregamento.Codigo;
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.TempoRestante + ': ', janelaCarregamento.DataTerminoCotacao, 12, null, null, idJanelaCarregamento);

            setTimeout(function () {
                $("#" + idJanelaCarregamento)
                    .countdown(moment(janelaCarregamento.DataTerminoCotacao, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        if (event.elapsed) {
                            $(this).text("["+Localization.Resources.Cargas.Carga.Esgotado+"]");
                            self.NovoLance.visible(false);
                        }
                        else {

                            if (event.offset.totalDays > 0)
                                $(this).text(event.strftime('%-Dd %H:%M:%S'));
                            else
                                $(this).text(event.strftime('%H:%M:%S'));
                        }
                    })
            }, 300);
        }

        if (janelaCarregamento.SituacaoCotacaoCarga != "")
            html += this._obterHtmlDadosCargaCampo(Localization.Resources.Cargas.Carga.Situacao + ': ', janelaCarregamento.SituacaoCotacaoCarga, 12);

        html += '</div>';

        return html;
    },
    _obterHtmlDadosCargaCampo: function (descricao, valor, tamanhoColuna, cores, classeContainer, idValor) {
        var classeCor = '';
        var html = '';

        if (cores)
            classeCor = cores ? Global.ObterClasseDinamica(cores) : "";

        html += '<div class="col col-' + (tamanhoColuna || 6) + ' ' + (classeContainer || '') + '">';
        html += '    <div class="dados-janela-carregamento-container-texto">';
        html += '        <div class="dados-janela-carregamento-texto ' + classeCor + '"><span><b>' + descricao + '</b></span><span id="' + (idValor || '') + '">' + valor + '</span></div>';
        html += '    </div>';
        html += '</div>';

        return html;
    },
    _obterHtmlCargaLiberadaCotacao: function (janelaCarregamento) {
        if (janelaCarregamento.CargaLiberadaCotacao && (janelaCarregamento.Situacao === EnumSituacaoCargaJanelaCarregamento.SemTransportador))
            return '<div class="carga-liberada-cotacao"><i class="fal fa-usd-circle"></i></div>';

        return '';
    },
    _obterHtmlClassificacao: function (janelaCarregamento) {
        var htmlClassificacao = '<div class="container-icone-classificacao">';

        if (janelaCarregamento.ClassificacaoPessoaCor) {
            htmlClassificacao += '<div class="icone-classificacao">';
            htmlClassificacao += '    <i class="fa fa-star fa-2x" style="color: ' + janelaCarregamento.ClassificacaoPessoaCor + '"></i>';
            htmlClassificacao += '    <i class="fal fa-star fa-2x" style="color: #000000;"></i>';
            htmlClassificacao += '</div>';
        }

        if (janelaCarregamento.CargaDeComplemento) {
            htmlClassificacao += '<div class="icone-classificacao">';
            htmlClassificacao += '    <i class="fa fa-link fa-2x icone-classificacao-carga-com-complemento"></i>';
            htmlClassificacao += '</div>';
        }

        htmlClassificacao += '</div>';

        return htmlClassificacao;
    },
    _obterHtmlFaixa: function (corFaixa, informacao, styleSpanInformacao) {
        if (styleSpanInformacao == null)
            styleSpanInformacao = "";

        return '<div class="ribbon-tms ribbon-tms-' + corFaixa + '"><span' + styleSpanInformacao + '>' + informacao + '</span></div>';
    },
    _obterHtmlFaixaInformativa: function (janelaCarregamento) {
        /* Faixa de Horário encaixado*/
        if (janelaCarregamento.HorarioEncaixado)
            return this._obterHtmlFaixa("green", Localization.Resources.Cargas.Carga.Encaixado);

        /* Retorna faixa de horário cancelado */
        if (janelaCarregamento.HorarioDesencaixado)
            return this._obterHtmlFaixa("red", "H. Cancelado", ' style="font-size: 9px;"');

        /* Retorna sem faixa caso config ativa */
        if (janelaCarregamento.NaoPermitirExibirTagsPadroes)
            return "";

        /* Faixa de no show */
        if (janelaCarregamento.NaoComparecido != EnumTipoNaoComparecimento.Compareceu)
            return this._obterHtmlFaixa("dark-purple", EnumTipoNaoComparecimento.obterDescricao(janelaCarregamento.NaoComparecido));

        /* Faixa de carrgamento atrasado */
        if (janelaCarregamento.DiasAtrazo > 0) {
            var informacao = (janelaCarregamento.CarregamentoReservado ? "R " : "") + janelaCarregamento.DiasAtrazo + " " + Localization.Resources.Cargas.Carga.SomenteDia + (janelaCarregamento.DiasAtrazo > 1 ? 's' : '');

            return this._obterHtmlFaixa("red", informacao);
        }

        /* Faixa de carrgamento em dia */
        if (!janelaCarregamento.CarregamentoReservado)
            return this._obterHtmlFaixa("green", Localization.Resources.Cargas.Carga.EmDia);

        /* Faixa de carga adiantada */
        if (janelaCarregamento.DiasAtrazo < 0) {
            var diasAdiantamento = Math.abs(janelaCarregamento.DiasAtrazo);
            var informacao = (janelaCarregamento.CarregamentoReservado ? "R " : "") + diasAdiantamento + " " + Localization.Resources.Cargas.Carga.SomenteDia + (diasAdiantamento > 1 ? 's' : '');

            return this._obterHtmlFaixa("blue", informacao);
        }

        /* Carregamento reservado */
        if (janelaCarregamento.CarregamentoReservado)
            return this._obterHtmlFaixa("gray", Localization.Resources.Cargas.Carga.Reservada);

        return "";
    },
    _obterHtmlInteressadosCarga: function (janelaCarregamento) {
        var htmlInteressados = '';

        if ((janelaCarregamento.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador) && (janelaCarregamento.Interessados > 0)) {
            htmlInteressados += '<div oncontextmenu="myFunction()">';
            htmlInteressados += '    <a href="javascript:void(0);" class="btn-interesado-carga" onclick="AbrirTelaInteressadosCarga(' + janelaCarregamento.Carga.Codigo + ', event);">' + janelaCarregamento.Interessados + ' interessado' + (janelaCarregamento.Interessados > 0 ? "s" : "") + '</a>';
            htmlInteressados += '</div>';
        }

        return htmlInteressados;
    },
    _obterHtmlVisualizacoesCarga: function (janelaCarregamento) {
        var htmlVisualizacoes = '';
        if ((janelaCarregamento.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador) && (janelaCarregamento.Visualizacoes > 0)) {
            htmlVisualizacoes += '<div oncontextmenu="myFunction()">';
            htmlVisualizacoes += '    <a href="javascript:void(0);" class="btn-interesado-carga" onclick="AbrirTelaVisualizacoesCarga(' + janelaCarregamento.Carga.Codigo + ', event);">' + janelaCarregamento.Visualizacoes + ' visualizações</a>';
            htmlVisualizacoes += '</div>';
        }

        return htmlVisualizacoes;
    }
}