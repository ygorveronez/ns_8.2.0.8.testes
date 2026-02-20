/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarTitulo = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, statusEmAberto, tipoAPagar) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa de Título", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Títulos", type: types.local });
        this.Descricao = PropertyEntity({ col: 4, text: "Código: " });
        this.DataInicialVencimento = PropertyEntity({ text: "Data Vencimento ", col: 4, getType: typesKnockout.date, visible: true });
        this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão ", col: 4, getType: typesKnockout.date, visible: true });
        this.Pessoa = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: true });
        this.ValorDe = PropertyEntity({ col: 2, getType: typesKnockout.decimal, required: false, text: "Valor De:", maxlength: 10, enable: ko.observable(true), def: ko.observable("0,00") });
        this.ValorAte = PropertyEntity({ col: 2, getType: typesKnockout.decimal, required: false, text: "Valor Até:", maxlength: 10, enable: ko.observable(true), def: ko.observable("0,00") });
        this.CategoriaPessoa = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), text: "Categoria Pessoa:", idBtnSearch: guid(), visible: true });
        this.Adiantado = PropertyEntity({ val: ko.observable(-1), def: -1, visible: false });
        this.FormaTitulo = PropertyEntity({ val: ko.observable(-1), def: -1, visible: false });
        this.StatusEmAberto = PropertyEntity({ val: ko.observable(-1), def: -1, visible: false });
        this.TipoAPagar = PropertyEntity({ val: ko.observable(-1), def: -1, visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarClientes(knoutOpcoes.Pessoa);
        new BuscarCategoriaPessoa(knoutOpcoes.CategoriaPessoa);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            Global.setarFocoProximoCampo(knout.id);
            divBusca.CloseModal();
        };
    }

    if (statusEmAberto != null) {
        knoutOpcoes.StatusEmAberto.val(statusEmAberto);
    }

    if (tipoAPagar != null) {
        knoutOpcoes.TipoAPagar.val(tipoAPagar);
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TituloFinanceiro/PesquisaConsulta", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 2, dir: orderDir.asc });
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
    });
};

//var BuscarDocumentosFaturamentoParaFatura = function (knout, callbackRetorno, basicGrid, knoutFatura) {

//    var idDiv = guid();
//    var GridConsulta;

//    var multiplaEscolha = false;
//    if (basicGrid != null) {
//        multiplaEscolha = true;
//    }

//    var OpcoesKnout = function () {
//        this.Titulo = PropertyEntity({ text: "Consulta de Documentos para Fatura", type: types.local });
//        this.TituloGrid = PropertyEntity({ text: "Documentos", type: types.local });
//        this.Fatura = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
//        this.NumeroDocumento = PropertyEntity({ col: 4, text: "Documento: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });

//        this.Pesquisar = PropertyEntity({
//            eventClick: function (e) {
//                GridConsulta.CarregarGrid();
//            }, type: types.event, text: "Pesquisar", visible: true
//        });
//    };

//    var knoutOpcoes = new OpcoesKnout();

//    var funcaoParamentroDinamico = null;

//    var url = "FaturaDocumento/PesquisaDocumentosParaFatura";

//    funcaoParamentroDinamico = function () {
//        if (knoutFatura != null) {
//            knoutOpcoes.Fatura.codEntity(knoutFatura.val());
//            knoutOpcoes.Fatura.val(knoutFatura.val());
//        }
//    };

//    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

//    var callback = function (e) {
//        divBusca.DefCallback(e);
//    };

//    if (callbackRetorno != null) {
//        callback = function (e) {
//            callbackRetorno(e);
//            $("#" + idDiv).modal('hide');
//        };
//    }

//    if (multiplaEscolha) {
//        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
//        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
//    } else {
//        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
//    }

//    divBusca.AddEvents(GridConsulta);

//    divBusca.AddTabPressEvent(function (outraPressionada) {
//        if (outraPressionada)
//            knoutOpcoes.NumeroDocumento.val(knout.val());

//        GridConsulta.CarregarGrid(function (lista) {
//            if (lista.data.length == 1)
//                callback(lista.data[0]);
//            else
//                divBusca.OpenModal();
//        });
//    });
//};

var BuscarTitulosParaBordero = function (knout, callbackRetorno, basicGrid, knoutBordero) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Títulos para Borderô", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Títulos", type: types.local });
        this.NumeroTitulo = PropertyEntity({ col: 4, text: "Nº Título: ", getType: typesKnockout.int });
        this.CTe = PropertyEntity({ col: 4, text: "Nº CT-e: ", getType: typesKnockout.int });
        this.Carga = PropertyEntity({ col: 4, text: "Nº Carga: ", getType: typesKnockout.int });
        this.Bordero = PropertyEntity({ getType: typesKnockout.int, visible: false, val: ko.observable(0), def: 0 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        knoutOpcoes.Bordero.val(knoutBordero.Codigo.val());
    };

    var url = "BorderoTitulo/PesquisaTitulosParaBordero";

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, null);

    var callback = function (e) {
        knout.val(e.Codigo);
        knout.codEntity(e.Codigo);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Titulo.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};

var BuscarTitulosPendentesParaBaixaTituloReceberNova = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = basicGrid != null;

    var _titulosDeAgrupamento = [
        { text: "Todos", value: "" },
        { text: "Gerado de negociação", value: 1 },
        { text: "Não gerado de negociação", value: 0 }
    ];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Títulos para Baixa", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Títulos", type: types.local });

        this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: '' }, col: 2, visible: true });
        this.DataInicial = PropertyEntity({ text: "Vencto. Inicial: ", getType: typesKnockout.date, col: 2, visible: true });
        this.DataFinal = PropertyEntity({ text: "Vencto. Final: ", getType: typesKnockout.date, col: 2, visible: true });
        this.DataEmissaoInicial = PropertyEntity({ text: "Emissão Inicial: ", getType: typesKnockout.date, col: 2, visible: true });
        this.DataEmissaoFinal = PropertyEntity({ text: "Emissão Final: ", getType: typesKnockout.date, col: 2, visible: true });
        this.TitulosDeAgrupamento = PropertyEntity({ val: ko.observable(""), options: _titulosDeAgrupamento, def: "", text: "Agrupado: ", col: 2, visible: true });

        this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", col: 2, visible: true });
        this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), col: 7, visible: ko.observable(false) });
        this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), col: 7, visible: ko.observable(true) });

        this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), col: 3, visible: true });
        this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), col: 3, visible: true });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), col: 3, visible: true });

        this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", col: 2, visible: true });
        this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", col: 2, visible: true });
        this.NumeroDocumentoOriginario = PropertyEntity({ text: "Nº Doc. Originário: ", getType: typesKnockout.int, val: ko.observable(""), configInt: { precision: 0, allowZero: false, thousands: '' }, col: 2, visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.TipoPessoa.val.subscribe(function (novoValor) {
            if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
                knoutOpcoes.Pessoa.visible(true);
                knoutOpcoes.GrupoPessoa.visible(false);
                LimparCampoEntity(knoutOpcoes.GrupoPessoa);
            } else if (novoValor == EnumTipoPessoaGrupo.GrupoPessoa) {
                knoutOpcoes.Pessoa.visible(false);
                knoutOpcoes.GrupoPessoa.visible(true);
                LimparCampoEntity(knoutOpcoes.Pessoa);
            }
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var url = "BaixaTituloReceberNovo/PesquisaTitulosPendentes";

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
        new BuscarClientes(knoutOpcoes.Pessoa);
        new BuscarFatura(knoutOpcoes.Fatura, function (data) {
            knoutOpcoes.Fatura.codEntity(data.Codigo);
            knoutOpcoes.Fatura.val(data.Numero);
        });
        new BuscarConhecimentoNotaReferencia(knoutOpcoes.Conhecimento, function (data) {
            knoutOpcoes.Conhecimento.codEntity(data.Codigo);
            knoutOpcoes.Conhecimento.val(data.Numero + "-" + data.Serie);
        });
        new BuscarCargas(knoutOpcoes.Carga, null, null, [EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.AgIntegracao]);
    }, null, true);

    var callback = function (e) {
        knout.val(e.Codigo);
        knout.codEntity(e.Codigo);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Titulo.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};

var BuscarTitulosPendentesParaBaixaTituloPagar = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = basicGrid != null;

    var _titulosDeAgrupamento = [
        { text: "Todos", value: 9 },
        { text: "Gerado de negociação", value: 1 },
        { text: "Não gerado de negociação", value: 0 }
    ];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Títulos para Baixa", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Títulos", type: types.local });

        this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo Pessoa: ", visible: true, col: 2 });
        this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), col: 6 });
        this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(true), col: 6 });

        this.TitulosDeAgrupamento = PropertyEntity({ val: ko.observable(9), options: _titulosDeAgrupamento, def: 9, text: "Agrupado:", visible: true, col: 2 });
        this.NumeroTitulo = PropertyEntity({ text: "Nº Título:", getType: typesKnockout.int, maxlength: 16, visible: true, col: 2 });

        this.DataInicialVencimento = PropertyEntity({ text: "Vencimento de:", getType: typesKnockout.date, visible: true, col: 2 });
        this.DataFinalVencimento = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, visible: true, col: 2 });
        this.DataInicial = PropertyEntity({ text: "Emissão de:", getType: typesKnockout.date, visible: true, col: 2 });
        this.DataFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, visible: true, col: 2 });

        this.ValorInicial = PropertyEntity({ text: "Valor de: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal, visible: true, col: 2 });
        this.ValorFinal = PropertyEntity({ text: "Até: ", val: ko.observable("0,00"), def: ko.observable("0,00"), getType: typesKnockout.decimal, visible: true, col: 2 });

        this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Doc. Entrada:", idBtnSearch: guid(), visible: true, col: 4 });
        this.NumeroDocumento = PropertyEntity({ text: "Nº Documento:", visible: true, col: 2 });
        this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Todos), options: EnumFormaTitulo.obterOpcoesPesquisa(), text: "Forma do Título:", def: EnumFormaTitulo.Todos, visible: true, col: 2 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

        this.TipoPessoa.val.subscribe(function (novoValor) {
            if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
                knoutOpcoes.Pessoa.visible(true);
                knoutOpcoes.GrupoPessoa.visible(false);
                LimparCampoEntity(knoutOpcoes.GrupoPessoa);
            } else if (novoValor == EnumTipoPessoaGrupo.GrupoPessoa) {
                knoutOpcoes.Pessoa.visible(false);
                knoutOpcoes.GrupoPessoa.visible(true);
                LimparCampoEntity(knoutOpcoes.Pessoa);
            }
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var url = "BaixaTituloPagar/PesquisaTitulosPendentes";

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Fornecedores);
        new BuscarClientes(knoutOpcoes.Pessoa);
        new BuscarDocumentoEntrada(knoutOpcoes.DocumentoEntrada, function (data) {
            knoutOpcoes.DocumentoEntrada.val(data.Numero);
            knoutOpcoes.DocumentoEntrada.codEntity(data.Codigo);
        });
    }, null, true);

    var callback = function (e) {
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Titulo.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
};