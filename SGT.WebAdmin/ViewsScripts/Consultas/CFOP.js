/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="NaturezaOperacao.js" />

var BuscarCFOPs = function (knout, tipo, callbackRetorno, ativo, basicGrid) {
    var idDiv = guid();
    var gridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.CFOP.ConsultaDeCFOPs, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.CFOP.CFOPs, type: types.local });

        this.NaturezaOperacao = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CFOP.NaturezaDaOperacao.getFieldDescription(), idBtnSearch: guid() });
        this.CFOP = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.CFOP.DescricaoCFOP.getFieldDescription() });
        this.Tipo = PropertyEntity({ visible: false, val: ko.observable(tipo), def: tipo });
        this.Ativo = PropertyEntity({ visible: false, val: ko.observable(ativo), def: ativo });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.CFOP.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {

        new BuscarNaturezasOperacoes(knoutOpcoes.NaturezaOperacao);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CFOP/Consultar", knoutOpcoes, null, null, null, null, null, null,objetoBasicGrid);
    }
    else
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CFOP/Consultar", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.CFOP.val(knout.val());
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

var BuscarCFOPNotaFiscal = function (knout, callbackRetorno, tipo, dentroEstado, knoutPessoa, knoutTipoEmissao, knoutEmpresa, knoutIndicadorPresenca, basicGrid, knoutNaturezaOperacao, knoutLocalidadeInicioPrestacao, knoutLocalidadeTerminoPrestacao) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.CFOP.ConsultaDeCFOPs, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.CFOP.CFOPs, type: types.local });

        this.NumeroCFOP = PropertyEntity({ col: 4, text: Localization.Resources.Consultas.CFOP.DescricaoCFOP.getFieldDescription() });
        this.Extensao = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.CFOP.Extensao.getFieldDescription() });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.CFOP.Descricao.getFieldDescription() });

        this.Tipo = PropertyEntity({ visible: false, val: ko.observable(tipo), def: tipo });
        this.DentroEstado = PropertyEntity({ visible: false, val: ko.observable(dentroEstado), def: dentroEstado });
        this.Status = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Status.getFieldDescription(), visible: false, val: ko.observable("A"), def: ko.observable("A") });
        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.NaturezaOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.IndicadorPresenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(9), idBtnSearch: guid(), visible: false });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CFOP.Pessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.TipoEmissao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.CFOP.TipoEmissao.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.LocalidadeInicioPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.LocalidadeTerminoPrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.CFOP.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutPessoa != null || knoutTipoEmissao != null || knoutEmpresa != null || knoutIndicadorPresenca != null || knoutNaturezaOperacao != null || knoutLocalidadeInicioPrestacao != null || knoutLocalidadeTerminoPrestacao != null) {
        knoutOpcoes.Pessoa.visible = knoutPessoa == null;
        funcaoParamentroDinamico = function () {
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }
            if (knoutTipoEmissao != null) {
                knoutOpcoes.TipoEmissao.codEntity(knoutTipoEmissao.val());
                knoutOpcoes.TipoEmissao.val(knoutTipoEmissao.val());
            } else {
                knoutOpcoes.TipoEmissao.codEntity(-1);
                knoutOpcoes.TipoEmissao.val(-1);
            }
            if (knoutEmpresa != null) {
                knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
                knoutOpcoes.Empresa.val(knoutEmpresa.val());
            }
            if (knoutIndicadorPresenca != null) {
                knoutOpcoes.IndicadorPresenca.codEntity(knoutIndicadorPresenca.val());
                knoutOpcoes.IndicadorPresenca.val(knoutIndicadorPresenca.val());
            }
            if (knoutNaturezaOperacao != null) {
                knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
                knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
            }
            if (knoutLocalidadeInicioPrestacao != null) {
                knoutOpcoes.LocalidadeInicioPrestacao.codEntity(knoutLocalidadeInicioPrestacao.codEntity());
                knoutOpcoes.LocalidadeInicioPrestacao.val(knoutLocalidadeInicioPrestacao.val());
            }
            if (knoutLocalidadeTerminoPrestacao != null) {
                knoutOpcoes.LocalidadeTerminoPrestacao.codEntity(knoutLocalidadeTerminoPrestacao.codEntity());
                knoutOpcoes.LocalidadeTerminoPrestacao.val(knoutLocalidadeTerminoPrestacao.val());
            }
        }
    } else {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Pessoa.codEntity(0);
            knoutOpcoes.Pessoa.val(0);
            knoutOpcoes.TipoEmissao.codEntity(-1);
            knoutOpcoes.TipoEmissao.val(-1);
            knoutOpcoes.IndicadorPresenca.codEntity(9);
            knoutOpcoes.IndicadorPresenca.val(9);
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

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
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CFOPNotaFiscal/Pesquisa", knoutOpcoes, null, { column: 1, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "CFOPNotaFiscal/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            if (knout.val().indexOf(".") > -1) {
                var posicaoPonto = knout.val().indexOf(".");
                var stringPesquisa = knout.val();
                var extensao = stringPesquisa.substring(posicaoPonto + 1);
                var numeroCFOP = stringPesquisa.substring(0, posicaoPonto);
                knoutOpcoes.Extensao.val(extensao);
                knoutOpcoes.NumeroCFOP.val(numeroCFOP);
            } else {
                knoutOpcoes.NumeroCFOP.val(knout.val());
                knoutOpcoes.Extensao.val("");
            }
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.CarregarCFOPNaturezaOperacaoSelecionada = function () {
        if (knoutNaturezaOperacao != null) {
            LimparCampos(knoutOpcoes);
            funcaoParamentroDinamico();

            gridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };

    this.Destroy = function () {
        divBusca.Destroy();
    };
};