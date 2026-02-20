/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumSituacaoAbastecimento.js" />

var BuscarConfiguracaoAbastecimento = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Configurações de Abastecimento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Configurações", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);


    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfiguracaoAbastecimento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarAbastecimentosSemAcertoDeViagem = function (knout, callbackRetorno, basicGrid, knoutCodigoAcertoViagem, knoutCodigoVeiculo, knoutTipoAbastecimento) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Abastecimentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Abastecimento", type: types.local });

        this.Kilometragem = PropertyEntity({ text: "Kilometragem ", col: 3, getType: typesKnockout.int });
        this.Horimetro = PropertyEntity({ text: "Horímetro ", col: 3, getType: typesKnockout.int });
        this.Data = PropertyEntity({ text: "Data: ", col: 6, getType: typesKnockout.date });

        this.AcertoViagem = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Acerto de Viagem:", idBtnSearch: guid(), visible: false });
        this.Veiculo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: false });
        this.TipoAbastecimento = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCodigoAcertoViagem != null && knoutCodigoVeiculo != null && knoutTipoAbastecimento != null) {
        knoutOpcoes.AcertoViagem.visible = false;
        knoutOpcoes.Veiculo.visible = false;
        knoutOpcoes.TipoAbastecimento.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.AcertoViagem.codEntity(knoutCodigoAcertoViagem.val());
            knoutOpcoes.AcertoViagem.val(knoutCodigoAcertoViagem.val());

            knoutOpcoes.Veiculo.codEntity(knoutCodigoVeiculo.val());
            knoutOpcoes.Veiculo.val(knoutCodigoVeiculo.val());

            knoutOpcoes.TipoAbastecimento.codEntity(knoutTipoAbastecimento);
            knoutOpcoes.TipoAbastecimento.val(knoutTipoAbastecimento);
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/PesquisaAbastecimentoSemAcertoDeViagem", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/PesquisaAbastecimentoSemAcertoDeViagem", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Nome.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarAbastecimentos = function (knout, callbackRetorno, basicGrid, knoutVeiculo, knoutProduto, knoutSituacao, knoutCodigoAbastecimento, knoutPosto, knoutData) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Abastecimentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Abastecimento", type: types.local });

        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.Posto = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Posto:", idBtnSearch: guid(), visible: true });
        this.Kilometragem = PropertyEntity({ text: "Kilometragem ", col: 3, getType: typesKnockout.int });
        this.Horimetro = PropertyEntity({ text: "Horímetro ", col: 3, getType: typesKnockout.int });
        this.DataInicial = PropertyEntity({ text: "Data: ", col: 6, getType: typesKnockout.date, visible: true });
        this.Placa = PropertyEntity({ text: "Placa ", col: 6, getType: typesKnockout.string, visible: false });

        this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoAbastecimento.Todos), def: EnumSituacaoAbastecimento.Todos, visible: false });
        this.Produto = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.Abastecimento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.NumeroDocumentoInicial = PropertyEntity({ col: 3, getType: typesKnockout.int, val: ko.observable(0), visible: true, text: "N° Documento Inicial:" });
        this.NumeroDocumentoFinal = PropertyEntity({ col: 3, getType: typesKnockout.int, val: ko.observable(0), visible: true, text: "N° Documento Final:" });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutVeiculo != null) {
        knoutOpcoes.Veiculo.visible = false;
        knoutOpcoes.Placa.visible = true;
    } else {
        knoutOpcoes.Placa.visible = false;
    }

    if (knoutProduto != null || knoutVeiculo != null || knoutSituacao != null || knoutCodigoAbastecimento != null || knoutPosto != null || knoutData != null) {
        funcaoParamentroDinamico = function () {
            if (knoutVeiculo != null) {
                knoutOpcoes.Veiculo.codEntity(knoutVeiculo.codEntity());
                knoutOpcoes.Veiculo.val(knoutVeiculo.val());
            }

            if (knoutProduto != null) {
                knoutOpcoes.Produto.codEntity(knoutProduto.codEntity());
                knoutOpcoes.Produto.val(knoutProduto.val());
            }

            if (knoutSituacao != null)
                knoutOpcoes.Situacao.val(knoutSituacao);

            if (knoutCodigoAbastecimento != null) {
                knoutOpcoes.Abastecimento.codEntity(knoutCodigoAbastecimento.val());
                knoutOpcoes.Abastecimento.val(knoutCodigoAbastecimento.val());
            }

            if (knoutPosto != null) {
                knoutOpcoes.Posto.codEntity(knoutPosto.codEntity());
                knoutOpcoes.Posto.val(knoutPosto.val());
            }

            if (knoutData != null)
                knoutOpcoes.DataInicial.val(knoutData.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.Posto);
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/Pesquisa", knoutOpcoes, null, { column: 3, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 3, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Data.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};

var BuscarAbastecimentosAgregado = function (knout, callbackRetorno, basicGrid, knoutCliente, knoutDataInicial, knoutDataFinal, knoutPagamentoAgregado, knoutVeiculo) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Abastecimentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Abastecimentos", type: types.local });        
        this.DataInicial = PropertyEntity({ text: "Data Inicial: ", col: 2, getType: typesKnockout.date, visible: true });
        this.DataFinal = PropertyEntity({ text: "Data Final: ", col: 2, getType: typesKnockout.date, visible: true });
        this.Motorista = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: true });

        this.PagamentoAgregado = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Pagento Agregado:", idBtnSearch: guid(), visible: false });;
        this.Veiculo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: false });;
        this.Cliente = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: false });;
        this.DataInicial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Inicial:", idBtnSearch: guid(), visible: false });;
        this.DataFinal = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Final:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCliente != null && knoutDataInicial != null && knoutDataFinal != null && knoutPagamentoAgregado != null && knoutVeiculo != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.PagamentoAgregado.codEntity(knoutPagamentoAgregado.val());
            knoutOpcoes.PagamentoAgregado.val(knoutPagamentoAgregado.val());

            knoutOpcoes.Veiculo.codEntity(knoutVeiculo.val());
            knoutOpcoes.Veiculo.val(knoutVeiculo.val());

            knoutOpcoes.Cliente.codEntity(knoutCliente.codEntity());
            knoutOpcoes.Cliente.val(knoutCliente.val());

            knoutOpcoes.DataInicial.codEntity(knoutDataInicial.val());
            knoutOpcoes.DataInicial.val(knoutDataInicial.val());

            knoutOpcoes.DataFinal.codEntity(knoutDataFinal.val());
            knoutOpcoes.DataFinal.val(knoutDataFinal.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarMotoristas(knoutOpcoes.Motorista);
    });

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/PesquisaAbastecimentoAgregado", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Abastecimento/PesquisaAbastecimentoAgregado", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}