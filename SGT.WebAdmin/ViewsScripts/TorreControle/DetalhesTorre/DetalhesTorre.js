var _detalhesTorre;

var DetalhesTorre = function () {
    //TAB RESUMO
    this.Codigo = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Codigo }),
    this.MotoristaNome = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Motorista }),
    this.MotoristaTelefone = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Telefone }),
    this.MotoristaCPF = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Cpf }),
    this.Veiculo = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Veiculo }),
    this.Tecnologia = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Tecnologia }),
    this.RastreadorStatus = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Rastreador }),
    this.RastreadorDescricao = PropertyEntity({ val: ko.observable() }),
    this.InicioMonitoramento = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.InicioMonitoramento }),
    this.FimMonitoramento = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.FimMonitoramento }),
    this.Velocidade = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Velocidade }),
    this.Localizacao = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Localizacao }),
    this.Cidade = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Cidade }),
    this.PrimeiraPosicao = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.PrimeiraPosicao }),
    this.DataPosicao = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataPosicao }),
    this.Temperatura = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Temperatura }),
    this.Ignicao = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Ignicao }),
    this.Status = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Status, getType: typesKnockout.select, options: ko.observable([]) }),
    this.DataInicialStatus = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataInicio, getType: typesKnockout.dateTime })
    this.DistanciaRealizada = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DistanciaRealizada }),
    this.DistanciaPrevista = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DistanciaPrevista }),
    this.PrevisaoChegada = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.PrevisaoChegada }),
    this.DistanciaDestino = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DistanciaDestino }),
    this.MonitoramentoCritico = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.MonitoramentoCritico }),
    this.Observacao = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Observacao })
    this.Auditar = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.Auditar, eventClick: AbrirAuditoria })
    this.FinalizarViagem = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.FinalizarViagem, eventClick: FinalizarMonitoramentoManualmenteClick })
    this.SalvarAlteracoes = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.SalvarAlteracoes, eventClick: AtualizarCamposDetalhesTorre })
    this.DetalhesTorre = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DetalhesTorre })
    this.Resumo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Resumo })
    this.Paradas = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Paradas })
    this.HistoricoPosicoes = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.HistoricoPosicoes })
    this.Graficos = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Graficos })
    this.LinhaTempo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.LinhaTempo })
    this.Monitoramento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Monitoramento })
    this.PosicaoAtual = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PosicaoAtual })
    this.SituacaoViagem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.SituacaoViagem })
    this.HistoricoPosicoes = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.HistoricoPosicoes })
    this.TemperaturaComUnidade = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TemperaturaComUnidade })
    this.VelocidadeComUnidade = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.VelocidadeComUnidade })
    this.Tempo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Tempo })
    this.Pesquisar = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Pesquisar })
    this.DataInicial = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataInicial })
    this.DataFinal = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataFinal })

    //TAB PARADAS
    this.GridParadas = PropertyEntity({ idGrid: guid(), val: ko.observable(new Array()), type: types.local, getType: typesKnockout.dynamic, def: new Array() })

    //TAB GRAFICOS
    this.DataInicialPosicoes = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataInicial, getType: typesKnockout.dateTime })
    this.DataFinalPosicoes = PropertyEntity({ val: ko.observable(), text: Localization.Resources.Logistica.Monitoramento.DataFinal, getType: typesKnockout.dateTime })
    this.TipoGrafico = PropertyEntity({ val: ko.observable("temperatura") });
    this.Pesquisar = PropertyEntity({ eventClick: pesquisarGraficos, text: Localization.Resources.Logistica.Monitoramento.Pesquisar })
    this.Temperaturas = PropertyEntity({ val: ko.observable([]) })
    this.Velocidades = PropertyEntity({ val: ko.observable([]) })

    //TAB LINHA DO TEMPO
    this.Historico = PropertyEntity({ val: ko.observable([]) })
    this.toggleItem = function (item) {
        item.isOpen(!item.isOpen());
    };
};

function loadModalDetalhesTorre(codigoMonitoramento) {
    $.get("Content/TorreControle/DetalhesTorre/ModalDetalhesTorre.html?dyn=" + guid(), function (html) {
        $("#ModalDetalhesTorre").html(html);

        _detalhesTorre = new DetalhesTorre();

        KoBindings(_detalhesTorre, "knockoutDetalhesTorre");

        _detalhesTorre.GridParadas.val([{}]);

        loadDetalhesTorre(codigoMonitoramento);
    });
}

function loadDetalhesTorre(numero) {
    executarReST("DetalhesTorre/Pesquisa", { codigo: numero }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var data = arg.Data;

                if (data.Historico) {
                    data.Historico.forEach(item => {
                        item.isOpen = ko.observable(false);
                    });
                }
                PreencherObjetoKnout(_detalhesTorre, arg);

                var selector = ".icone-ignicao";

                if (data.Ignicao) {
                    _detalhesTorre.Ignicao.val(Localization.Resources.Logistica.Monitoramento.Ligada);
                    $(selector).css("color", TRACKING_IGNICAO_COR_LIGADO);
                }
                else {
                    _detalhesTorre.Ignicao.val(Localization.Resources.Logistica.Monitoramento.Desligada);
                    $(selector).css("color", TRACKING_IGNICAO_COR_DELIGADO);
                }
                    
                icone = ObterIconeStatusTracking(data.RastreadorStatus, 30);
                document.getElementById('iconeRastreador').innerHTML = icone;

                if (data.RastreadorStatus) {
                    if (data.RastreadorStatus === 1) {
                        _detalhesTorre.RastreadorStatus.val(Localization.Resources.Logistica.Monitoramento.SemPosicao);
                    } else if (data.RastreadorStatus === 3) {
                        _detalhesTorre.RastreadorStatus.val(Localization.Resources.Logistica.Monitoramento.Online);
                    } else if (data.RastreadorStatus === 4) {
                        _detalhesTorre.RastreadorStatus.val(Localization.Resources.Logistica.Monitoramento.Offline);
                    }
                }

                _detalhesTorre.DataInicialPosicoes.val(data.PrimeiraPosicao);
                _detalhesTorre.DataFinalPosicoes.val(data.DataPosicao);
                _detalhesTorre.Status.options(data.Status);
                _detalhesTorre.Status.val(data.StatusAtual);

                carregarGridEntregas(data.Paradas);
                pesquisarGraficos();

                Global.abrirModal("divModalDetalhesTorre");
            }
        }
    })
}

function carregarGridEntregas(paradas) {
    var header = [
        { data: "Cliente", title: Localization.Resources.Logistica.Monitoramento.Cliente, width: "10%" },
        { data: "Cidade", title: Localization.Resources.Logistica.Monitoramento.Cidade, width: "10%" },
        { data: "Tipo", title: Localization.Resources.Logistica.Monitoramento.Tipo, width: "8%" },
        { data: "Situacao", title: Localization.Resources.Logistica.Monitoramento.Situacao, width: "8%" },
        { data: "PesoKg", title: Localization.Resources.Logistica.Monitoramento.PesoKg, width: "8%", className: "text-align-right" },
        { data: "ValorTotalProdutos", title: Localization.Resources.Logistica.Monitoramento.ValorTotalProdutos, width: "10%", className: "text-align-right" },
        { data: "SequenciaPlanejada", title: Localization.Resources.Logistica.Monitoramento.SequenciaPlanejada, width: "8%" },
        { data: "SequenciaExecutada", title: Localization.Resources.Logistica.Monitoramento.SequenciaExecutada, width: "8%" },
        { data: "PrevisaoChegada", title: Localization.Resources.Logistica.Monitoramento.PrevisaoChegada, width: "10%" },
        { data: "PrevisaoChegadaReprogramada", title: Localization.Resources.Logistica.Monitoramento.PrevisaoChegadaReprogramada, width: "10%" },
        { data: "EntradaRaio", title: Localization.Resources.Logistica.Monitoramento.EntradaRaio, width: "8%" },
        { data: "InicioEntrega", title: Localization.Resources.Logistica.Monitoramento.InicioEntrega, width: "8%" },
        { data: "FimEntrega", title: Localization.Resources.Logistica.Monitoramento.FimEntrega, width: "8%" },
        { data: "SaidaRaio", title: Localization.Resources.Logistica.Monitoramento.SaidaRaio, width: "8%" },
        { data: "Entrega", title: Localization.Resources.Logistica.Monitoramento.Entrega, width: "8%" }
    ];

    var gridId = _detalhesTorre.GridParadas.idGrid;
    var grid = new BasicDataTable(gridId, header, null, null, null, 12);
    grid.SetPermitirEdicaoColunas(true);
    grid.SetSalvarPreferenciasGrid(true);
    grid.SetHabilitarScrollHorizontal(true, 200);
    grid.CarregarGrid(paradas);
}

function pesquisarGraficos() {
    let chart;
    let dadosGrafico;
    let tipo = _detalhesTorre.TipoGrafico.val();
    const temperaturas = filtrarDadosPorPeriodo(_detalhesTorre.Temperaturas.val(), _detalhesTorre.DataInicialPosicoes.val(), _detalhesTorre.DataFinalPosicoes.val());
    const velocidades = filtrarDadosPorPeriodo(_detalhesTorre.Velocidades.val(), _detalhesTorre.DataInicialPosicoes.val(), _detalhesTorre.DataFinalPosicoes.val());

    if (tipo === 'temperatura') {
        dadosGrafico = temperaturas.map(p => ({
            dataHora: p.Data,
            valor: p.Valor
        }));
    } else {
        dadosGrafico = velocidades.map(p => ({
            dataHora: p.Data,
            valor: p.Valor
        }));
    }

    const labels = dadosGrafico.map((d => d.dataHora));
    const values = dadosGrafico.map(d => d.valor);
    const tooltips = dadosGrafico.map(d => `${d.dataHora} - ${d.valor}°C`);

    const ctx = document.getElementById('graficoTemp').getContext('2d');
    if (chart) chart.destroy();

    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: tipo,
                data: values,
                fill: false,
                borderColor: 'purple',
                backgroundColor: 'white',
                pointBorderColor: 'purple',
                pointBackgroundColor: 'white',
                pointRadius: 5,
                tension: 0
            }]
        },
        options: {
            responsive: true,
            plugins: {
                tooltip: {
                    callbacks: {
                        label: (context) => {
                            const idx = context.dataIndex;
                            return tooltips[idx];
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};
function parseDataBR(str) {
    if (!str)
        return;
    const [dia, mes, anoHora] = str.split('/');
    const [ano, hora] = anoHora.split(' ');
    const [horas, minutos] = hora.split(':');
    return new Date(+ano, +mes - 1, +dia, +horas, +minutos);
}

function filtrarDadosPorPeriodo(dados, dataInicial, dataFinal) {
    if (!dataInicial && !dataFinal) return dados;

    let inicio = dataInicial ? parseDataBR(dataInicial) : null;
    let fim = dataFinal ? parseDataBR(dataFinal) : null;
    if (fim) fim.setSeconds(59, 999);

    return dados.filter(p => {
        const data = parseDataBR(p.Data);
        if (inicio && fim) {
            return data >= inicio && data <= fim;
        } else if (inicio) {
            return data >= inicio;
        } else if (fim) {
            return data <= fim;
        }
        return true;
    });
}

function AtualizarCamposDetalhesTorre() {
    exibirConfirmacao("Confirmação", Localization.Resources.Logistica.Monitoramento.ConfirmarAtualizacaoCampos, function () {
        executarReST("DetalhesTorre/AtualizarCamposDetalhesTorre",
            {
                Codigo: _detalhesTorre.Codigo.val(),
                Critico: _detalhesTorre.MonitoramentoCritico.val(),
                Status: _detalhesTorre.Status.val(),
                Observacao: _detalhesTorre.Observacao.val(),
                DataInicialStatus: _detalhesTorre.DataInicialStatus.val()
            },
            function (retorno) {
                if (retorno.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
                }
            }
        );
    })
}

function FinalizarMonitoramentoManualmenteClick() {
    exibirConfirmacao("Confirmação", Localization.Resources.Logistica.Monitoramento.ConfirmarFinalizarMonitoramento, function () {
        executarReST("Monitoramento/FinalizarMonitoramentoManualmente", { codigo: _detalhesTorre.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.MonitoramentoDetalhes.Sucesso, retorno.Msg);
                atualizarDetalhesMonitoramentoClick();

                var pagina = window.location.href;
                if (pagina.includes('MonitoramentoNovo')) {
                    recarregarDadosMonitoramentoNovo();
                    Global.fecharModal("divModalDetalhesTorre");
                } else
                    recarregarDados();
                    Global.fecharModal("divModalDetalhesTorre");
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.MonitoramentoDetalhes.Falha, retorno.Msg);
        });
    })
}

function AbrirAuditoria() {
    var data = { Codigo: _detalhesTorre.Codigo.val() };
    var closureAuditoria = OpcaoAuditoria("Monitoramento");
    closureAuditoria(data);
}