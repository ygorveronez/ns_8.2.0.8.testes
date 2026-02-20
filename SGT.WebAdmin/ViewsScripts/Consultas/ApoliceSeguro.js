/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="Seguradora.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="GrupoPessoa.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Configuracao/ConfiguracaoTMS.js" />

var BuscarApolicesSeguro = function (knout, knoutGrupoPessoas, knoutPessoa, basicGrid, callbackRetorno, somenteNaoVencidos, knoutEmpresa, exibirEmbarcador) {
    var idDiv = guid();
    var GridConsulta;
    var somenteNaoVencidos = somenteNaoVencidos == true;
    var exibirEmbarcador = exibirEmbarcador == true;

    var multiplaEscolha = false;
    if (basicGrid != null) 
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ApoliceSeguro.ConsultaDeApolicesDeSeguro, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ApoliceSeguro.ApolicesDeSeguro, type: types.local });
        this.NumeroApolice = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ApoliceSeguro.NumeroDaApolice.getFieldDescription(), maxlength: 20 });
        this.NumeroAverbacao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ApoliceSeguro.NumeroDaAverbacao.getFieldDescription(), maxlength: 20 });
        this.Seguradora = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.Seguradora.getFieldDescription(), idBtnSearch: guid() });
        this.GrupoPessoas = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid() });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.Pessoa.getFieldDescription(), idBtnSearch: guid() });
        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false)  });
        this.Validar = PropertyEntity({ getType: typesKnockout.bool, def: true, val: ko.observable(true), visible: false  });
        this.SomenteNaoVencidos = PropertyEntity({ getType: typesKnockout.bool, def: somenteNaoVencidos, val: ko.observable(somenteNaoVencidos), visible: false });
        this.Responsavel = PropertyEntity({ val: ko.observable(EnumResponsavelSeguro.Todos), def: EnumResponsavelSeguro.Todos, options: EnumResponsavelSeguro.obterOpcoesPesquisa(), text: Localization.Resources.Consultas.ApoliceSeguro.Responsavel.getFieldDescription(), visible: false });
        this.ExibirEmbarcador = PropertyEntity({ getType: typesKnockout.bool, def: exibirEmbarcador, val: ko.observable(exibirEmbarcador), visible: false });
        this.Ativa = PropertyEntity({ type: types.int, val: ko.observable(1), visible: false });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ApoliceSeguro.Descricao.getFieldDescription(), maxlength: 300, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }
    
    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;


    if (knoutGrupoPessoas != null || knoutPessoa != null || knoutEmpresa != null) {
        knoutOpcoes.Pessoa.visible = false;
        funcaoParamentroDinamico = function () {
            if (knoutGrupoPessoas != null) {
                knoutOpcoes.GrupoPessoas.codEntity(knoutGrupoPessoas.codEntity());
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
            }
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }

            if (knoutEmpresa != null) {
                knoutOpcoes.Responsavel.val(EnumResponsavelSeguro.Embarcador);
                if (knoutEmpresa.codEntity() > 0) {
                    knoutOpcoes.Responsavel.val(null);

                }

                knoutOpcoes.Empresa.codEntity(knoutEmpresa.codEntity());
                knoutOpcoes.Empresa.val(knoutEmpresa.val());
            }

            

        }
    }
     
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
        new BuscarClientes(knoutOpcoes.Pessoa);
        new BuscarSeguradoras(knoutOpcoes.Seguradora);
    });
    
    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ApoliceSeguro/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ApoliceSeguro/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) 
            knoutOpcoes.Descricao.val(knout.val());
        
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}

var BuscarApolicesSeguroMultitransportador = function (knout, knoutGrupoPessoas, knoutPessoa, basicGrid, callbackRetorno, somenteNaoVencidos, knoutEmpresa, exibirEmbarcador) {
    var idDiv = guid();
    var GridConsulta;
    var somenteNaoVencidos = somenteNaoVencidos == true;
    var exibirEmbarcador = exibirEmbarcador == true;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ApoliceSeguro.ConsultaDeApolicesDeSeguro, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ApoliceSeguro.ApolicesDeSeguro, type: types.local });
        this.NumeroApolice = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.ApoliceSeguro.NumeroDaApolice.getFieldDescription(), maxlength: 20 });
        this.Seguradora = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.Seguradora.getFieldDescription(), idBtnSearch: guid() });
        this.Empresa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ApoliceSeguro.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarSeguradoras(knoutOpcoes.Seguradora);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ApoliceSeguro/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ApoliceSeguro/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}