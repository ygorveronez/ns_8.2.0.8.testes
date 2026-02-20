/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="Cliente.js" />
/// <reference path="GrupoPessoa.js" />

var BuscarRotasFrete = function (knout, callbackRetorno, basicGrid, knoutGrupoPessoas, knoutPessoa, codigoCarga, afterDefaultCallback, pedidos, knoutTabelaFrete, knoutRemetente, knoutDestinatario, knoutRecebedor, knoutExpedidor) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    if (codigoCarga == null)
        codigoCarga = 0;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.RotaFrete.PesquisaDeRotasDeFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.RotaFrete.RotasDeFrete, type: types.local });

        this.Descricao = PropertyEntity({ col: 7, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.CodigoIntegracao = PropertyEntity({ col: 5, text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription() });
        this.Remetente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.RotaFrete.Remetente.getFieldDescription(), idBtnSearch: guid() });
        this.Destinatario = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.RotaFrete.Destinatario.getFieldDescription(), idBtnSearch: guid() });

        this.GrupoPessoas = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.RotaFrete.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false });
        this.SomenteGrupo = PropertyEntity({ val: ko.observable(true), def: true, visible: false });
        this.RotaExclusivaCompraValePedagio = PropertyEntity({ val: ko.observable(false), def: false, visible: false });
        this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, codEntity: ko.observable(0), getType: typesKnockout.int, visible: false });
        this.Pedidos = PropertyEntity({ visible: false, type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
        this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && knoutGrupoPessoas == null)
            this.GrupoPessoas.visible = true;

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
        knoutOpcoes.SomenteGrupo.val(false);

    if (codigoCarga > 0) {
        knoutOpcoes.GrupoPessoas.visible = false;
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.Carga.codEntity(codigoCarga);
        knoutOpcoes.Carga.val(codigoCarga);
    }

    if (knoutGrupoPessoas != null || knoutPessoa != null || pedidos || knoutTabelaFrete != null || knoutRemetente != null || knoutDestinatario != null) {
        funcaoParametroDinamico = function () {
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
                knoutOpcoes.GrupoPessoas.visible = false;
            }

            if (knoutGrupoPessoas != null) {
                knoutOpcoes.GrupoPessoas.codEntity(knoutGrupoPessoas.codEntity());
                knoutOpcoes.GrupoPessoas.val(knoutGrupoPessoas.val());
                knoutOpcoes.GrupoPessoas.visible = false;
            }

            if (knoutTabelaFrete != null) {
                knoutOpcoes.TabelaFrete.codEntity(knoutTabelaFrete.codEntity());
                knoutOpcoes.TabelaFrete.val(knoutTabelaFrete.val());
            }

            if (knoutRecebedor!= null && knoutRecebedor.codEntity().length > 0) {
                knoutOpcoes.Remetente.codEntity(knoutExpedidor.codEntity());
                knoutOpcoes.Remetente.val(knoutExpedidor.val());

            } else if (knoutDestinatario != null && knoutDestinatario.codEntity().length > 0) {
                knoutOpcoes.Destinatario.codEntity(knoutDestinatario.codEntity());
                knoutOpcoes.Destinatario.val(knoutDestinatario.val());
            }

            if (knoutExpedidor!= null && knoutExpedidor.codEntity().length > 0) {
                knoutOpcoes.Destinatario.codEntity(knoutRecebedor.codEntity());
                knoutOpcoes.Destinatario.val(knoutRecebedor.val());

            } else if (knoutRemetente != null && knoutRemetente.codEntity().length > 0) {
                knoutOpcoes.Remetente.codEntity(knoutRemetente.codEntity());
                knoutOpcoes.Remetente.val(knoutRemetente.val());
            }

            if (pedidos) {
                var codigoPedidos = [];

                for (var i = 0; i < pedidos().length; i++) {
                    codigoPedidos.push(pedidos()[i].Codigo);
                }

                var listaPedidos = JSON.stringify(codigoPedidos);

                knoutOpcoes.Pedidos.val(listaPedidos);
            }
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {

        new BuscarClientes(knoutOpcoes.Remetente);
        new BuscarClientes(knoutOpcoes.Destinatario);
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    var url = "RotaFrete/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, afterDefaultCallback: afterDefaultCallback, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };

    this.ExecuteSearch = function () {
        LimparCampos(knoutOpcoes);
        divBusca.UpdateGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
        });
    };
};