/// <reference path="../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarTiposDeCargaPorDestinatario = function (knout, callbackRetorno, knoutDestinatario) {
    var idDiv = guid();
    var GridConsulta;
    var codigoDinamico;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.BuscarTiposDeCargas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.TiposDeCargas, type: types.local });
        this.CodigoTipoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Destinatario = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Buscar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    var funcaoParametroDinamico = function () {
        if (knoutDestinatario != null) {
            knoutOpcoes.Destinatario.val(knoutDestinatario.val());
        }

        if (codigoDinamico > 0)
            knoutOpcoes.CodigoTipoCarga.val(codigoDinamico);
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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

    this.CarregarTipoDeCargaPorCodigo = function (codigo) {
        if (codigo > 0) {
            codigoDinamico = codigo;
            LimparCampos(knoutOpcoes);
            funcaoParametroDinamico();

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };
}

var BuscarTiposdeCarga = function (knout, callbackRetorno, knoutGrupoPessoas, basicGrid, knoutPessoa, codigoCarga, isFiltrarPorConfiguracaoOperadorLogistica, isFiltrarSomenteDisponiveisMontagemCarga, isFiltrarTipoCargaPrincipal, knoutTipoOperacaoEmissao) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    if (codigoCarga == null)
        codigoCarga = 0;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.PesquisarTiposCargas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.TiposDeCargas, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.GrupoPessoas = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.GrupoPessoas.getFieldDescription(), idBtnSearch: guid() });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid() });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.TipoOperacaoEmissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });
        this.FiltrarSomenteDisponiveisMontagemCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarSomenteDisponiveisMontagemCarga === true), visible: false });
        this.FiltrarTiposPrincipais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarTipoCargaPrincipal === true), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutGrupoPessoas != null) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
    }

    if (knoutPessoa != null) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
    }

    if (codigoCarga > 0) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.Carga.codEntity(codigoCarga);
        knoutOpcoes.Carga.val(codigoCarga);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
    }

    if (knoutGrupoPessoas != null ||
        knoutPessoa != null || 
        knoutTipoOperacaoEmissao != null) {

        funcaoParamentroDinamico = function () {
            if (knoutGrupoPessoas != null) {
                knoutOpcoes.GrupoPessoas.codEntity(knoutGrupoPessoas.codEntity());
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
            }

            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }

            if (knoutTipoOperacaoEmissao != null) {
                knoutOpcoes.TipoOperacaoEmissao.codEntity(knoutTipoOperacaoEmissao.codEntity());
                knoutOpcoes.TipoOperacaoEmissao.val(knoutTipoOperacaoEmissao.val());
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarga/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarga/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

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

var BuscarTiposdeCargaPorFilial = function (knout, callbackRetorno, knoutFilial, basicGrid, isFiltrarPorConfiguracaoOperadorLogistica, isFiltrarSomenteDisponiveisMontagemCarga) {
    var idDiv = guid();
    var gridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.PesquisarTiposCargas, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoCarga.TiposCargas, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Filiais = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), visible: false });
        this.FiltrarPorConfiguracaoOperadorLogistica = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarPorConfiguracaoOperadorLogistica != false), visible: false });
        this.FiltrarSomenteDisponiveisMontagemCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(isFiltrarSomenteDisponiveisMontagemCarga === true), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFilial) {
        funcaoParamentroDinamico = function () {
            if (knoutFilial.type == types.entity) {
                knoutOpcoes.Filial.codEntity(knoutFilial.codEntity());
                knoutOpcoes.Filial.val(knoutFilial.val());
            }
            else if (knoutFilial.type == types.multiplesEntities)
                knoutOpcoes.Filiais.multiplesEntities(knoutFilial.multiplesEntities().slice());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarga/PesquisaPorFilial", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoCarga/PesquisaPorFilial", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}