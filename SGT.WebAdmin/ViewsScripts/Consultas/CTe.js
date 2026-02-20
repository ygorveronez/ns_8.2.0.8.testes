/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Carga.js" />
/// <reference path="Cliente.js" />

var BuscarCTesSemCarga = function (knout, callbackRetorno, basicGrid) {

    let idDiv = guid();
    let gridConsulta;

    let multiplaEscolha = (basicGrid != null);
    let knoutOpcoes = null;

    let OpcoesKnout = function () {
        this.NumeroInicial = PropertyEntity({ text: "Nº Inicial:", col: 3 });
        this.NumeroFinal = PropertyEntity({ text: "Nº Final:", col: 3 });
        this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial:", val: ko.observable(Global.PrimeiraDataDoMesAnterior()), getType: typesKnockout.date, col: 3 });
        this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final:", getType: typesKnockout.date, col: 3 });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
        this.GrupoPessoas = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: true });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });
        this.Titulo = PropertyEntity({ text: "Buscar CTe-s sem Cargas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "CT-es sem Cargas", type: types.local });
        this.Remetente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: true });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade()) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: Localization.Resources.Consultas.Veiculo.Avancada, idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.NumeroNF = PropertyEntity({ col: 2, text: "Nº NF", val: ko.observable(0), def: 0, maxlength: 9, visible: true });
        this.ChaveNF = PropertyEntity({ col: 6, text: "Chave NF:", val: ko.observable(""), def: "", maxlength: 44, visible: true });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.NumeroInicial.val.subscribe(function (novoValor) {
            knoutOpcoes.NumeroFinal.val(novoValor);
        });
    }

    knoutOpcoes = new OpcoesKnout();

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas);
        new BuscarClientes(knoutOpcoes.Remetente);
    });

    let callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    const url = "CargaPedidoDocumentoCTe/ConsultarCTesSemCarga";

    if (multiplaEscolha) {
        let objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, { column: 0, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroInicial.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}

var BuscarCTes = function (knout, callbackRetorno, situacoes, TituloOpcional, TituloGridOpcional) {

    var idDiv = guid();
    var GridConsulta;

    var isMultiTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS;

    var OpcoesKnout = function () {
        this.Numero = PropertyEntity({ text: "Numero: ", col: 2 });
        this.Serie = PropertyEntity({ text: "Série: ", col: 2 });
        this.Transportador = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: isMultiTMS });
        this.Chave = PropertyEntity({ text: "Chave: ", col: 4, visible: true });

        this.Status = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(situacoes)), idGrid: guid(), visible: false });

        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });

        this.Titulo = PropertyEntity({ text: !string.IsNullOrWhiteSpace(TituloOpcional) ? TituloOpcional : "Buscar CTe-s", type: types.local });
        this.TituloGrid = PropertyEntity({ text: !string.IsNullOrWhiteSpace(TituloGridOpcional) ? TituloGridOpcional : "CT-es", type: types.local });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade() == true) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Remetente = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: true });
        this.Tomador = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: true });
        this.Veiculo = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarClientes(knoutOpcoes.Tomador);
        new BuscarTransportadores(knoutOpcoes.Transportador);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConsultaCTe/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Chave.val(knout.val());
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

var BuscarDocumentosDeAgregado = function (knout, callbackRetorno, basicGrid, knoutCliente, knoutDataInicial, knoutDataFinal, knoutPagamentoAgregado, knoutTipoOcorrencia) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Documentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Documentos", type: types.local });
        this.Numero = PropertyEntity({ text: "Número: ", col: 6 });
        this.DataEmissao = PropertyEntity({ text: "Data Emissão: ", col: 6, getType: typesKnockout.date, visible: true });
        this.Motorista = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: true });

        this.PagamentoAgregado = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Pagamento Agregado:", idBtnSearch: guid(), visible: false });
        this.Cliente = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: false });
        this.TipoOcorrencia = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: false });
        this.DataInicial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Inicial:", idBtnSearch: guid(), visible: false });
        this.DataFinal = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Final:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCliente != null && knoutDataInicial != null && knoutDataFinal != null && knoutPagamentoAgregado != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.PagamentoAgregado.codEntity(knoutPagamentoAgregado.val());
            knoutOpcoes.PagamentoAgregado.val(knoutPagamentoAgregado.val());

            knoutOpcoes.Cliente.codEntity(knoutCliente.codEntity());
            knoutOpcoes.Cliente.val(knoutCliente.val());

            knoutOpcoes.DataInicial.codEntity(knoutDataInicial.val());
            knoutOpcoes.DataInicial.val(knoutDataInicial.val());

            knoutOpcoes.DataFinal.codEntity(knoutDataFinal.val());
            knoutOpcoes.DataFinal.val(knoutDataFinal.val());

            if (knoutTipoOcorrencia != null) {
                knoutOpcoes.TipoOcorrencia.codEntity(knoutTipoOcorrencia.codEntity());
                knoutOpcoes.TipoOcorrencia.val(knoutTipoOcorrencia.val());
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarMotoristas(knoutOpcoes.Motorista);
        new BuscarClientes(knoutOpcoes.Destinatario);
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConsultaCTe/PesquisaDocumentoAgregado", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConsultaCTe/PesquisaDocumentoAgregado", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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

var BuscarCTesComCarga = function (knout, callbackRetorno, knoutCarga, TituloOpcional, TituloGridOpcional) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Numero = PropertyEntity({ text: "Numero: ", col: 2 });
        this.Serie = PropertyEntity({ text: "Série: ", col: 2 });
        this.Chave = PropertyEntity({ text: "Chave: ", col: 4, visible: true });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga", issue: 629, idBtnSearch: guid(), visible: false });

        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: "Situação: " });

        this.Titulo = PropertyEntity({ text: !string.IsNullOrWhiteSpace(TituloOpcional) ? TituloOpcional : "Buscar CTe-s", type: types.local });
        this.TituloGrid = PropertyEntity({ text: !string.IsNullOrWhiteSpace(TituloGridOpcional) ? TituloGridOpcional : "CT-es", type: types.local });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade() == true) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Remetente = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: true });
        this.Destinatario = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), visible: true });
        this.Tomador = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: true });
        this.Veiculo = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        if (knoutCarga != null) {
            knoutOpcoes.Carga.codEntity(knoutCarga.codEntity());
            knoutOpcoes.Carga.val(knoutCarga.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarClientes(knoutOpcoes.Tomador);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConsultaCTe/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Chave.val(knout.val());
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