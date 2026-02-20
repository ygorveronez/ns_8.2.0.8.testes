var BuscarPedidos = function (knout, callbackRetorno, basicGrid, isKnoutNumeroPedidoEmbarcador) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });

        this.NumeroPedido = PropertyEntity({ text: "Nº Pedido: ", col: 4 });
        this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador: ", col: 4 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, null, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            knout.codEntity(e.Codigo);
            knout.val(e.Numero);
            divBusca.CloseModal();
            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (isKnoutNumeroPedidoEmbarcador === true) {
                knoutOpcoes.NumeroPedidoEmbarcador.val(knout.val());
            } else {
                knoutOpcoes.NumeroPedido.val(knout.val());
            }
        }

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}

var BuscarPedidosEmbarcador = function (knout, callbackRetorno, basicGrid, knoutCarga) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });

        this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador: ", col: 4 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.CodigoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
    }

    var funcaoParamentroDinamico = function () {
        if (knoutCarga != null) {
            knoutOpcoes.CodigoCarga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.CodigoCarga.val(knoutCarga.val());
        }
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, null, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            knout.codEntity(e.Codigo);
            knout.val(e.NumeroPedidoEmbarcador);
            divBusca.CloseModal();
            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroPedidoEmbarcador.val(knout.val());
        }

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}

var BuscarPedidosDisponiveis = function (knout, callbackRetorno, basicGrid, knoutFilial, programaComSessaoRoteirizador, permitirAdicionarPedidosNaEtapaUm) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    programaComSessaoRoteirizador = (programaComSessaoRoteirizador ? programaComSessaoRoteirizador : false);

    var opcoes = [
        { text: "Sim", value: true },
        { text: "Não", value: false }
    ];


    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });
        this.NumeroPedido = PropertyEntity({ col: 3, text: "Nº Pedido: " });
        this.NumeroPedidoEmbarcador = PropertyEntity({ col: 3, text: "Nº Pedido Embarcador: " });
        this.PedidosParaReentrega = PropertyEntity({ col: 3, text: "Pedidos para Reentrega:", val: ko.observable(false), options: opcoes, def: false });
        this.ProgramaComSessaoRoteirizador = PropertyEntity({ val: ko.observable(programaComSessaoRoteirizador), visible: false });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
        this.NumeroTransporte = PropertyEntity({ text: "Nº de Transporte: ", col: 4, visible: ko.observable(false) });
        this.NumeroCarregamento = PropertyEntity({ text: "Nº Carregamento: ", col: 4, visible: ko.observable(true) });
        this.PedidoComNotas = PropertyEntity({ col: 4, val: ko.observable(false), def: false, options: opcoes, text:"Pedido com Notas: " })
        this.NumeroCarregamentoPedido = PropertyEntity({ text: "Nº Carregamento do pedido: ", col: 4, visible: ko.observable(true) });


        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
        knoutOpcoes.PedidosParaReentrega.visible = false;
        knoutOpcoes.NumeroPedidoEmbarcador.col = 4;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
    {
        knoutOpcoes.NumeroPedido.visible = false;
    }

    var funcaoParametrosDinamicos = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }

        if (permitirAdicionarPedidosNaEtapaUm != null)
            knoutOpcoes.NumeroTransporte.visible(permitirAdicionarPedidosNaEtapaUm.val())
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Numero);

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            knout.codEntity(e.Codigo);
            knout.val(e.Numero);

            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (knoutOpcoes.NumeroPedido.visible) {
                knoutOpcoes.NumeroPedido.val(knout.val());
            } else {
                knoutOpcoes.NumeroPedidoEmbarcador.val(knout.val());
            }
        }
            
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}

var BuscarPedidosDisponiveisPortalRetira = function (knout, callbackRetorno, basicGrid, knoutFilial, programaComSessaoRoteirizador, knoutCapacidadeVeiculo, knoutPesoTotal, naoExibirPedidosDoDia,transportador) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    var _naoExibirPedidosDoDia = false;
    if (naoExibirPedidosDoDia != null)
        _naoExibirPedidosDoDia = naoExibirPedidosDoDia;

    programaComSessaoRoteirizador = (programaComSessaoRoteirizador ? programaComSessaoRoteirizador : false);

    var opcoesReentrega = [
        { text: "Sim", value: true },
        { text: "Não", value: false }
    ];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });
        this.NaoExibirPedidosDoDia = PropertyEntity({ visible: false, val: ko.observable(_naoExibirPedidosDoDia), def: false });
        this.NumeroPedidoEmbarcador = PropertyEntity({ col: 3, text: "Nº Pedido Embarcador: " });
        this.PedidosParaReentrega = PropertyEntity({ col: 3, text: "Pedidos para Reentrega:", val: ko.observable(false), options: opcoesReentrega, def: false });
        this.ProgramaComSessaoRoteirizador = PropertyEntity({ val: ko.observable(programaComSessaoRoteirizador), visible: false });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
        this.Transportador = PropertyEntity({ visible: false, val: ko.observable(transportador), def: false });
        this.CapacidadeVeiculo = PropertyEntity({ col: 3, text: 'Capacidade do Veículo', getType: typesKnockout.int, val: ko.observable(0), enable: false });
        this.PesoTotal = PropertyEntity({ col: 3, text: 'Peso Total', getType: typesKnockout.string, val: ko.observable(0), enable: false });
        this.Ocupacao = PropertyEntity({ col: 3, text: 'Ocupação', getType: typesKnockout.float, val: ko.observable(0) });
        this.RetornarSaldoPesoProdutosPedido = PropertyEntity({ col: 0, getType: typesKnockout.bool, val: ko.observable(true), def: true, visible: false, enable: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                this.PesoTotal.val(0);
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (knoutFilial != null) {
        knoutOpcoes.Filial.visible = false;
        knoutOpcoes.PedidosParaReentrega.visible = false;
        knoutOpcoes.NumeroPedidoEmbarcador.col = 3;
        knoutOpcoes.CapacidadeVeiculo.col = 3;
        knoutOpcoes.PesoTotal.col = 3;
        knoutOpcoes.Ocupacao.col = 3;
    }

    var callbackSelectRows = function () {
        var pesoTotal = 0;
        if (knoutPesoTotal != null) {
            pesoTotal = knoutPesoTotal.val();
        }
        
        GridConsulta.ObterMultiplosSelecionados().forEach(function (pedido) {
            pesoTotal += parseFloat(pedido.PesoSaldoProdutos.replace(".", "").replace(",", "."));
        });
        pesoTotal = pesoTotal.toFixed(2);
        knoutOpcoes.PesoTotal.val(pesoTotal.toLocaleString());

        var capacidade = parseFloat(knoutOpcoes.CapacidadeVeiculo.val().replace(".", "").replace(",", "."));
        var ocupacao = ((pesoTotal / capacidade) * 100).toFixed(2);
        knoutOpcoes.Ocupacao.val(ocupacao + " %");

        $('#' + knoutOpcoes.Ocupacao.id).css('display', 'block');

        if (ocupacao > 100) {
            $('#' + knoutOpcoes.Ocupacao.id).removeClass('prg-ok');
            $('#' + knoutOpcoes.Ocupacao.id).addClass('prg-excedido');
            $('#' + knoutOpcoes.Ocupacao.id).css('width', '100%');
        } else {
            $('#' + knoutOpcoes.Ocupacao.id).removeClass('prg-excedido');
            $('#' + knoutOpcoes.Ocupacao.id).addClass('prg-ok');
            $('#' + knoutOpcoes.Ocupacao.id).css('width', parseInt(ocupacao) + '%');
            if (ocupacao == 0) {
                $('#' + knoutOpcoes.Ocupacao.id).css('display', 'none');
            }
        }

    }

    var funcaoParametrosDinamicos = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
        if (transportador) {
            knoutOpcoes.Transportador.val(transportador.val());
        }

        if (knoutCapacidadeVeiculo) {
            knoutOpcoes.CapacidadeVeiculo.val(knoutCapacidadeVeiculo.formatado());
        }

        GridConsulta.AtualizarRegistrosSelecionados([]);
        callbackSelectRows();
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
        $('#' + knoutOpcoes.Ocupacao.id).css('display', 'none');
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Numero);

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            knout.codEntity(e.Codigo);
            knout.val(e.Numero);

            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, callbackSelecionado: callbackSelectRows, callbackNaoSelecionado: callbackSelectRows, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroPedidoEmbarcador.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else {
                $('#' + knoutOpcoes.Ocupacao.id).css('display', 'none');
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarPedidoEntregaControleEntrega = function (knout, callbackRetorno, knoutCargaDestino) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Pedido.BuscarPedidos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Pedido.Pedidos, type: types.local });
        this.NumeroPedidoEmbarcador = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Pedido.NumeroPedidoEmbarcador.getFieldDescription() });
        this.NotaFiscal = PropertyEntity({ col: 3, getType: typesKnockout.int, text: Localization.Resources.Consultas.Pedido.NumeroNotasFiscais.getFieldDescription() });

        this.CargaDestino = PropertyEntity({ col: 0, visible: false, type: types.map, val: ko.observable(0), text: "" });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametrosDinamicos = function () {
        if (knoutCargaDestino) {
            knoutOpcoes.CargaDestino.val(knoutCargaDestino.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos);

    var callback = function (e) {
        callbackRetorno(e);
    };

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };

    this.FecharBusca = function () {
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    }
}

var BuscarPedidosReentregaControleEntrega = function (knout, callbackRetorno, knoutFilial) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Pedido.BuscarPedidos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Pedido.Pedidos, type: types.local });
        this.NumeroPedidoEmbarcador = PropertyEntity({ col: 3, text: Localization.Resources.Consultas.Pedido.NumeroPedidoEmbarcador.getFieldDescription() });
        this.NotaFiscal = PropertyEntity({ col: 3, getType: typesKnockout.int, text: Localization.Resources.Consultas.Pedido.NumeroNotasFiscais.getFieldDescription() });

        this.PedidosParaReentrega = PropertyEntity({ text: Localization.Resources.Consultas.Pedido.PedidosParaReentrega.getFieldDescription(), val: ko.observable(true), visible: false, def: true });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.Pedido.Filial.getFieldDescription(), idBtnSearch: guid() });

        this.CodigoCargaEmbarcador = PropertyEntity({ col: 3, getType: typesKnockout.string, text: Localization.Resources.Consultas.Pedido.NumeroDaCarga.getFieldDescription(), def: "" });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametrosDinamicos = function () {
        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, null, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        callbackRetorno(e);
    };

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveis", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };

    this.FecharBusca = function () {
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    }
}

var BuscarPedidosParaTroca = function (knout, callbackRetorno, basicGrid, knoutCarga, knoutFilial) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });
        this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
        this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº Pedido Embarcador: ", col: 4 });
        this.Filial = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (knoutFilial != null)
        knoutOpcoes.Filial.visible = false;

    var funcaoParametrosDinamicos = function () {
        if (knoutCarga)
            knoutOpcoes.Carga.val(knoutCarga.val());

        if (knoutFilial) {
            knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
            knoutOpcoes.Filial.val(knoutFilial.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.Filial);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveisParaTroca", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaDisponiveisParaTroca", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroPedidoEmbarcador.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}

var BuscarPedidosPendetesAgendamento = function (knout, callbackRetorno, basicGrid, knoutDestinatario) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Pedidos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Pedidos", type: types.local });
        this.NumeroPedidoFiltro = PropertyEntity({ text: "Nº Pedidos: ", col: 4 });
        this.DestinatarioFiltro = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (knoutDestinatario != null)
        knoutOpcoes.DestinatarioFiltro.visible = false;

    var funcaoParametrosDinamicos = function () {
        if (knoutDestinatario) {
            knoutOpcoes.DestinatarioFiltro.codEntity(knoutDestinatario.codEntity());
            knoutOpcoes.DestinatarioFiltro.val(knoutDestinatario.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.DestinatarioFiltro);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();

            if (multiplaEscolha)
                $("#" + idDiv + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");

            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaGridPendentes", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Pedido/PesquisaGridPendentes", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroPedidoFiltro.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}
