/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />


var KnoutCadastrarColetaContainer = function () {
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Container:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: "Cancelar", visible: ko.observable(true) });

    var $this = this;
}

var BuscarColetaContainers = function (knout, callbackRetorno, basicGrid, permitirAddColetaContainer, callbackCadastrar) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    if (permitirAddColetaContainer == null)
        permitirAddColetaContainer = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Coletas Containers", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Coleta Container", type: types.local });
        this.NumeroContainer = PropertyEntity({ col: 3, text: "Número Container: " });
        this.DataColeta = PropertyEntity({ col: 4, text: "Data Coleta: " });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
        this.AdicionarColetaContainer = PropertyEntity({ type: types.event, text: "Adicionar nova Coleta Container", visible: permitirAddColetaContainer, cssClass: "btn btn-default", icon: " " });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    var fnPreecherRetornoSelecao = function (knout, e, idDiv, knoutOpcoes) {
        knout.codEntity(e.Codigo);
        knout.val(e.Container.Numero);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    }

    if (permitirAddColetaContainer) {

        var fnAbrirModalAddContainer = function (e, knoutCadastro, modalCadastro, coletacontainer) {
            coletacontainer.Container.codEntity(0);            
            Global.abrirModal(modalCadastro);
        }

        var knoutCadastro = idDiv + "_knockoutCadastroColetaContainer";
        var modalCadastro = idDiv + "divModalCadastrarColetaContainer";
        $.get("Content/Static/Consultas/Cadastros/ColetaContainer.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroColetaContainer/g, knoutCadastro).replace(/#divModalCadastrarColetaContainer/g, modalCadastro);
            $('#js-page-content').append(html);
            var coletacontainer = new KnoutCadastrarColetaContainer();

            coletacontainer.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            coletacontainer.Adicionar.eventClick = function (e) {
                Salvar(e, "ColetaContainer/Adicionar", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            fnPreecherRetornoSelecao(knout, arg.Data, idDiv, knoutOpcoes);
                            exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            };

            KoBindings(coletacontainer, knoutCadastro, false);

            new BuscarContainers(coletacontainer.Container);

            knoutOpcoes.AdicionarColetaContainer.eventClick = function (e) {
                fnAbrirModalAddContainer(e, knoutCadastro, modalCadastro, coletacontainer);
            }
        });
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        //divBusca.DefCallback(e);
        fnPreecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ColetaContainer/Pesquisa", knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ColetaContainer/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroContainer.val(knout.val());
        }
        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 0) {
                if (knoutOpcoes.NumeroContainer.val() != null && callbackCadastrar != null) {
                    exibirConfirmacao("Confirmação", "Coleta Container pesquisada pelo número Container: " + knoutOpcoes.NumeroContainer.val() + " não foi encontrada, deseja cadastrar nova Coleta de Container?", function () {
                        callbackCadastrar();
                    });
                } else {
                    divBusca.OpenModal();
                }
            }
            else if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};