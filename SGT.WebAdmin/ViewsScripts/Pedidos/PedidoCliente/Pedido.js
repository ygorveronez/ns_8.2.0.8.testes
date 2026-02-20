/// <reference path="../atendimentocliente/chatatendimento.js" />


var pedidoSelecionadoChat;
var _grid;

var TelaPedidos = function () {
    var self = this;
    var _pesquisaPedido;
    var _detalhesPedido;
    var _modalCanhotosPedidos;
    var _gridNotasFiscais = null;
    var HTMLDetalhesPedido = '';
    var timeoutFiltro = null;

    /**
     * Definições Knockout
     */
    var PesquisaPedido = function () {
        var self = this;
        this.AgColeta = PropertyEntity({ val: ko.observable(0), text: "Ag. Expedição", enum: EnumSituacaoAcompanhamentoPedido.AgColeta, active: ko.observable(false) });
        this.EmTransporte = PropertyEntity({ val: ko.observable(0), text: "Em transferência", enum: EnumSituacaoAcompanhamentoPedido.EmTransporte, active: ko.observable(false) });
        this.SaiuParaEntrega = PropertyEntity({ val: ko.observable(0), text: "Em rota", enum: EnumSituacaoAcompanhamentoPedido.SaiuParaEntrega, active: ko.observable(false) });
        this.Entregue = PropertyEntity({ val: ko.observable(0), text: "Entregue", enum: EnumSituacaoAcompanhamentoPedido.Entregue, active: ko.observable(false) });
        this.EntregaParcial = PropertyEntity({ val: ko.observable(0), text: "Entrega parcial", enum: EnumSituacaoAcompanhamentoPedido.EntregaParcial, active: ko.observable(false) });
        this.EntregaRejeitada = PropertyEntity({ val: ko.observable(0), text: "Devolução", enum: EnumSituacaoAcompanhamentoPedido.EntregaRejeitada, active: ko.observable(false) });
        this.ProblemaNoTransporte = PropertyEntity({ val: ko.observable(0), text: "Ocorrências", enum: EnumSituacaoAcompanhamentoPedido.ProblemaNoTransporte, active: ko.observable(false) });

        this.Filtro = PropertyEntity({ val: ko.observable(""), filtros: [this.AgColeta, this.EmTransporte, this.SaiuParaEntrega, this.Entregue, this.EntregaParcial, this.EntregaRejeitada, this.ProblemaNoTransporte] });
        this.Filtro.width = ko.computed(function () {
            return (100 / self.Filtro.filtros.length).toFixed(4) + "%"
        });

        this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoAcompanhamentoPedido.obterOpcoesPortalCliente(), eventBoxClick: onFiltroBoxesChange });
        this.DataInicial = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date });
        this.DataFinal = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.date });
        this.DataInicial.dateRangeLimit = this.DataFinal;
        this.DataFinal.dateRangeInit = this.DataInicial;

        this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de operação", idBtnSearch: guid() });
        this.Grid = PropertyEntity({});

        this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
        this.SelecionarPedidosParaCotacaoEspecial = PropertyEntity({ text: "Selecionar pedidos para cotação especial", getType: typesKnockout.bool, def: false, val: ko.observable(false), visible: ko.observable(true) });
        this.SolicitarCotacaoFrete = PropertyEntity({ eventClick: solicitarCotacaoFrete, type: types.event, text: "Solicitar cotação de frete", idGrid: guid(), visible: ko.observable(false) });
        this.SelecionarPedidosParaCotacaoEspecial.val.subscribe(function (novoValor) {
            if (novoValor) {
                _pesquisaPedido.SolicitarCotacaoFrete.visible(true);
                _pesquisaPedido.SelecionarTodos.visible(true);
                _pesquisaPedido.SelecionarPedidosParaCotacaoEspecial.val(true);

                carregarGridPedidos();

            } else {
                _pesquisaPedido.SolicitarCotacaoFrete.visible(false);
                _pesquisaPedido.SelecionarTodos.visible(false);

                carregarGridPedidos();
            }
        });
    };

    var DetalhePedido = function () {
        this.Codigo = PropertyEntity({ val: ko.observable(0) });

        this.Acompanhar = PropertyEntity({ val: ko.observable(0), text: "Acompanhar entrega", eventClick: navegarParaTelaPedidosClick });
        this.Atendimento = PropertyEntity({ val: ko.observable(0), text: "Solicitar atendimento", eventClick: solicitarNovoAtendimento });
        this.AbrirOcorrencia = PropertyEntity({ val: ko.observable(0), text: "Solicitar devolução/Reentrega", eventClick: abrirModalOcorrencia });

        this.Grid = PropertyEntity({});
    };

    /**
     * Eventos Knockout
     */
    var navegarParaTelaPedidosClick = function (e) {
        navegarParaTelaPedidos(e.Codigo.val());
    }

    var navegarParaTelaPedidos = function (data) {
        _notificacaoPortal.SetCodigoGlobal(data);
        window.location.hash = "Pedidos/DetalheEntrega";
    }

    var solicitarNovoAtendimento = function (e) {
        pedidoSelecionadoChat = e.Codigo.val();
        $("#OpcaoMotivoNovoAtendimento").val(0);

        Global.abrirModal("divSolicitarNovoAtendimento");
    }

    function abrirModalOcorrencia(e) {
        loadHtmlGestaoOcorrencia("gestaoOcorrencia", true, function () {
            buscarEntregaDoPedido(e.Codigo.val())
        });
    };


    var onFiltroBoxesChange = function (e, val) {
        _pesquisaPedido.Situacao.val(val.toString());

        for (var prop in _pesquisaPedido) {
            if (_pesquisaPedido[prop].enum != undefined && _pesquisaPedido[prop].enum == val) {
                _pesquisaPedido[prop].active(true);
                break;
            }
        }
    }

    var onFiltroChange = function () {
        if (timeoutFiltro != null)
            clearTimeout(timeoutFiltro);

        timeoutFiltro = setTimeout(self.ExecutarPesquisa, 700);
    }

    var onDataChange = function (val) {
        if ("__/__/____" == val) return;

        self.ExecutarPesquisa();
    }

    var onSituacaoChange = function () {
        limparFiltros();
        self.ExecutarPesquisa();
    }

    var onTipoOperacaoChange = function () {
        self.ExecutarPesquisa();
    };

    /**
     * Métodos Público
     */
    this.Load = function (id, idModalCanhotos) {
        _pesquisaPedido = new PesquisaPedido();
        _modalCanhotosPedidos = new ModalCanhotosPedidos();
        KoBindings(_pesquisaPedido, id);

        HTMLDetalhesPedido = $("#content-detalhes-pedido").html();
        _pesquisaPedido.Filtro.val(_FiltroPedidoGlobal.texto);
        _FiltroPedidoGlobal.texto = '';

        _pesquisaPedido.Situacao.val(_FiltroPedidoGlobal.situacao);
        _FiltroPedidoGlobal.situacao = null;

        new BuscarTiposOperacao(_pesquisaPedido.TipoOperacao);

        if (_CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga > 0)
            _pesquisaPedido.DataInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, _CONFIGURACAO_TMS.DiasAnterioresPesquisaCarga, EnumTipoOperacaoObjetoDate.Days));

        carregarQuantidadesFiltro();
        carregarGridPedidos();
        adicionarObservadores();

        _modalCanhotosPedidos.Load(idModalCanhotos);
    }

    var isPesquisaSendoExecutada = false;
    this.ExecutarPesquisa = function () {
        if (isPesquisaSendoExecutada) return;
        isPesquisaSendoExecutada = true;
        _grid.CarregarGrid(function (data) {
            carregarQuantidadesFiltro();
            isPesquisaSendoExecutada = false;
        });
    }

    /**
     * Métodos Privados
     */
    var limparFiltros = function () {
        for (var prop in _pesquisaPedido) {
            if (_pesquisaPedido[prop].enum != undefined) {
                _pesquisaPedido[prop].active(false);
            }
        }
    }

    var adicionarObservadores = function () {
        _pesquisaPedido.Filtro.val.subscribe(onFiltroChange);
        _pesquisaPedido.DataInicial.val.subscribe(onDataChange);
        _pesquisaPedido.DataFinal.val.subscribe(onDataChange);
        _pesquisaPedido.Situacao.val.subscribe(onSituacaoChange);
        _pesquisaPedido.TipoOperacao.val.subscribe(function (novoValor) {
            if (novoValor == '' && _pesquisaPedido.TipoOperacao.multiplesEntities().length > 0) {
                _pesquisaPedido.TipoOperacao.multiplesEntities([]);
            }
        });
        _pesquisaPedido.TipoOperacao.multiplesEntities.subscribe(function (novoValor) {
            onTipoOperacaoChange();
        });
    }

    var carregarQuantidadesFiltro = function () {
        var dados = RetornarObjetoPesquisa(_pesquisaPedido);

        executarReST("PedidoCliente/AtualizarDadosFiltroPedidos", dados, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

            var dados = arg.Data.Filtros;

            for (var i in dados) {
                var preenchimentoFiltro = dados[i];

                for (var prop in _pesquisaPedido) {
                    if ('enum' in _pesquisaPedido[prop] && _pesquisaPedido[prop].enum == preenchimentoFiltro.Situacao) {
                        _pesquisaPedido[prop].val(preenchimentoFiltro.Quantidade);
                        break;
                    }
                }
            }
        });
    }

    var loadDetalhesPedidos = function (id, data) {
        _detalhesPedido = new DetalhePedido();
        KoBindings(_detalhesPedido, id);

        _detalhesPedido.Codigo.val(data.Codigo);

        carregarGridNotas();
    }

    var carregarGridNotas = function () {
        _gridNotasFiscais = new GridView(_detalhesPedido.Grid.id, "PedidoCliente/PesquisaNotas", { Codigo: _detalhesPedido.Codigo });

        _gridNotasFiscais.SetHabilitarLinhaClicavel(true);
        _gridNotasFiscais.OnClickLinha(function (data) {
            _modalCanhotosPedidos.CarregarCanhotoNota(data);
        });

        _gridNotasFiscais.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            DescricaoStatusCanhoto: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
        });

        _gridNotasFiscais.CarregarGrid();
    }

    var carregarGridPedidos = function () {
        var koFiltros = {
            Filtro: _pesquisaPedido.Filtro,
            DataInicial: _pesquisaPedido.DataInicial,
            DataFinal: _pesquisaPedido.DataFinal,
            Situacao: _pesquisaPedido.Situacao,
            TipoOperacao: _pesquisaPedido.TipoOperacao
        };

        if (_pesquisaPedido.SelecionarPedidosParaCotacaoEspecial.val()) {
            var multiplaescolha = {
                basicGrid: null,
                callbackSelecionado: null,
                callbackNaoSelecionado: null,
                eventos: function () { },
                selecionados: new Array(),
                naoSelecionados: new Array(),
                SelecionarTodosKnout: _pesquisaPedido.SelecionarTodos,
                somenteLeitura: false
            };
            _grid = new GridView(_pesquisaPedido.Grid.id, "PedidoCliente/Pesquisa", koFiltros, null, null, 10, null, null, null, multiplaescolha)
        } else {
            _grid = new GridView(_pesquisaPedido.Grid.id, "PedidoCliente/Pesquisa", koFiltros, null, null, 10);
        }

        _grid.SetDefinicaoColunas({
            DescricaoStatus: function (data, row) {
                return '<span class="label label-warning">' + data + '</span >';
            },
            Notas: function (data, row) {
                var porcentagem = row.QuantidadeNotasEntregues / row.QuantidadeNotas * 100;

                return '<div class="progress quantidade-notas">'
                    + '<div class="progress-bar bg-warning" role="progressbar" style="width: ' + porcentagem + '%" aria-valuenow="' + Globalize.format(porcentagem, "n2") + '" aria-valuemin="0" aria-valuemax="100"></div>'
                    + '</div>'
                    + '<span class="label-quantidade-notas">' + data + '</span>'
                    ;
            },
        });

        _grid.SetHabilitarExpansaoLinha(true);
        _grid.SetHabilitarLinhaClicavel(true);

        if (!(_pesquisaPedido.SelecionarPedidosParaCotacaoEspecial.val())) {
            _grid.OnClickLinha(function (data) {
                navegarParaTelaPedidos(data.Codigo);
            });
        } 

        _grid.OnLinhaExpandida(function (data) {
            var id = guid();
            var _html = HTMLDetalhesPedido.replace('{{id}}', id);

            return {
                html: _html,
                callback: function () {
                    loadDetalhesPedidos(id, data);
                }
            };
        });

        _grid.OnLinhaRecolhida(function (data) {
            _detalhesPedido = null;
        });

        self.ExecutarPesquisa();
    }
}

function IniciarNovoChatTelaPedidos() {
    var valorOpcaoMotivo = $("#OpcaoMotivoNovoAtendimento").val();

    if (valorOpcaoMotivo == 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Necessário informar um motivo.");
        return;
    }

    enviarMensagem(pedidoSelecionadoChat, "Solicitando atendimento", valorOpcaoMotivo)
    Global.fecharModal('divSolicitarNovoAtendimento');
}

function closeModalSolicitarNovoAtendimento() {
    pedidoSelecionadoChat = 0;
    Global.fecharModal('divSolicitarNovoAtendimento');
}


function enviarMensagem(pedido, mensagem, motivo) {
    if (mensagem != "") {
        var mensagem = { Pedido: pedido, Mensagem: mensagem, Motivo: motivo }

        executarReST("AtendimentoPedidoCliente/EnviarMensagemChat", mensagem, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    abrirChamadoPaginaPedidos(pedido);
                    _cardativo = 0;
                    window.location.hash = "Pedidos/AtendimentoCliente";
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        });
    }
}

function buscarEntregaDoPedido(codigo) {
    _gestaoOcorrencia.CargaEntrega.enable(false);
    _gestaoOcorrencia.Carga.enable(false);
    executarReST("PedidoCliente/BuscarCargaEntregaDoPedido", { Codigo: codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                _gestaoOcorrencia.Carga.codEntity(arg.Data.Carga.Codigo);
                _gestaoOcorrencia.Carga.val(arg.Data.Carga.Descricao);

                _gestaoOcorrencia.CargaEntrega.codEntity(arg.Data.CargaEntrega.Codigo);
                _gestaoOcorrencia.CargaEntrega.val(arg.Data.CargaEntrega.Descricao);

                Global.abrirModal('divModalGestaoOcorrencia');

            }
            else if (gerarAviso)
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function solicitarCotacaoFrete(e, data) {

    var pedidosSelecionados = _grid.ObterMultiplosSelecionados();

    if (pedidosSelecionados.length === 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum pedido selecionado.");
        return;
    }

    var codigosPedidos = pedidosSelecionados.map(function (pedido) {
        return pedido.Codigo;
    });

    if (codigosPedidos && codigosPedidos.length > 0) {
        Global.abrirModal('#divModalEscolhaTipo');

        $('#btnSalvarTipoModal').off('click').on('click', function () {
            const tipoModal = $('#tipoModal').val();

            if (!tipoModal) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor, selecione o tipo de modal.");
                return;
            }

            Global.fecharModal('#divModalEscolhaTipo');

            exibirConfirmacao("Confirmação", "Realmente deseja solicitar cotação de frete?", function () {
                var data = {
                    Codigos: JSON.stringify(codigosPedidos),
                    TipoModal: JSON.stringify(tipoModal)
                };

                executarReST("PedidoCliente/SolicitarCotacaoFrete", data, function (arg) {
                    if (arg.Success) {
                        _grid.AtualizarRegistrosSelecionados([]);
                        _grid.CarregarGrid();

                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de cotação de frete realizada com sucesso.");
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            });
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum pedido selecionado.");
    }
}