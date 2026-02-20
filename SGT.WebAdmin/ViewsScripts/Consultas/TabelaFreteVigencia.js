/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarVigenciasTabelaFrete = function (knout, knoutTabelaFrete, callbackRetorno, permitirAddVigencia, knoutTransportador, knoutDataInicial, knoutDataFinal, knoutDataInicialVigencia, knoutDataFinalVigencia, knoutContratoTransportador) {

    var vigencia;
    var idDiv = guid();
    var GridConsulta;

    if (permitirAddVigencia == null)
        permitirAddVigencia = false;

    var visibleEmpresa = false;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        visibleEmpresa = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TabelaFrete.ConsultaDeVigenciasDaTabelaDeFrete, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TabelaFrete.Vigencias, type: types.local });
        this.DataInicial = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.TabelaFrete.DataInicial.getFieldDescription(), getType: typesKnockout.date });
        this.DataFinal = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.TabelaFrete.DataFinal.getFieldDescription(), getType: typesKnockout.date });

        this.TabelaFrete = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Empresa = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), visible: visibleEmpresa, text: Localization.Resources.Consultas.TabelaFrete.Transportador.getFieldDescription(), idBtnSearch: guid() });
        this.ContratoDoTransportador = PropertyEntity({  visible: false, val: ko.observable(0) });

        this.DataInicialVigencia = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: false });
        this.DataFinalVigencia = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.TabelaFrete.Pesquisar, visible: true
        });
        this.AdicionarVigencia = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.TabelaFrete.AdicionarNovaVigencia, visible: permitirAddVigencia, cssClass: "btn btn-default", icon: " " });
    }

    var KnoutCadastrarVigencia = function () {
        this.TabelaFrete = PropertyEntity({ type: types.map, val: ko.observable(0) });
        this.ContratoDoTransportador = PropertyEntity({ val: ko.observable(0),  getType: typesKnockout.int });
        this.DataInicial = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.TabelaFrete.DataInicial.getRequiredFieldDescription(), required: true, getType: typesKnockout.date });
        this.DataFinal = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.TabelaFrete.DataFinal.getFieldDescription(), getType: typesKnockout.date });

        this.DataInicialContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(false) });
        this.DataFinalContrato = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(false) });

        this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.TabelaFrete.Adicionar, visible: ko.observable(true) });
        this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.TabelaFrete.Cancelar, visible: ko.observable(true) });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = function () {
        knoutOpcoes.TabelaFrete.codEntity(knoutTabelaFrete.codEntity());
        knoutOpcoes.TabelaFrete.val(knoutTabelaFrete.val());

        if (knoutTransportador != null) {
            knoutOpcoes.Empresa.visible = false;
            knoutOpcoes.Empresa.codEntity(knoutTransportador.codEntity());
            knoutOpcoes.Empresa.val(knoutTransportador.val());
        }
        if (knoutDataInicial != null) {
            vigencia.DataInicialContrato.val(knoutDataInicial.val());
        }
        if (knoutDataFinal != null) {
            vigencia.DataFinalContrato.val(knoutDataFinal.val());
        }
        if (knoutDataInicialVigencia != null) {
            knoutOpcoes.DataInicialVigencia.val(knoutDataInicialVigencia.val());
            knoutOpcoes.DataInicialVigencia.visible = false;
        }
        if (knoutDataFinalVigencia != null) {
            knoutOpcoes.DataFinalVigencia.val(knoutDataFinalVigencia.val());
            knoutOpcoes.DataFinalVigencia.visible = false;
        }
        if (knoutContratoTransportador != null) {
            knoutOpcoes.ContratoDoTransportador.val(knoutContratoTransportador.codEntity());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, false);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (permitirAddVigencia) {
        var fnPreecherRetornoSelecao = function (ko, e, idDiv, knoutOpcoes) {
            GridConsulta.CarregarGrid();
            knoutOpcoes.DataInicial.val(knoutOpcoes.DataInicial.def);
            knoutOpcoes.DataFinal.val(knoutOpcoes.DataFinal.def);
            callback(e);
        }

        var fnAbrirModalAddVigencia = function (e, knoutCadastro, modalCadastro, vigencia) {
            vigencia.DataInicial.val(e.DataInicial.val());
            vigencia.DataFinal.val(e.DataFinal.val());
            vigencia.TabelaFrete.val(e.TabelaFrete.codEntity());

            if (!string.IsNullOrWhiteSpace(vigencia.DataInicialContrato.val())) 
                vigencia.DataInicial.val(vigencia.DataInicialContrato.val())

            if (!string.IsNullOrWhiteSpace(vigencia.DataFinalContrato.val())) 
                vigencia.DataFinal.val(vigencia.DataFinalContrato.val())

            if (vigencia.TabelaFrete.val() == 0)
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Consultas.TabelaFrete.DescricaoTabelaFrete, Localization.Resources.Consultas.TabelaFrete.InformeUmaTabelaDeFreteAntesDeCriarUmaVigencia);

            Global.abrirModal(modalCadastro);
        }

        var knoutCadastro = idDiv + "_knockoutCadastroVigencia";
        var modalCadastro = idDiv + "divModalCadastrarVigencia";

        $.get("Content/Static/Consultas/Cadastros/Vigencia.html?dyn=" + guid(), function (data) {
            var html = data.replace(/#knockoutCadastroVigencia/g, knoutCadastro).replace(/#divModalCadastrarVigencia/g, modalCadastro);
            $('#js-page-content').append(html);
            vigencia = new KnoutCadastrarVigencia();

            vigencia.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            vigencia.Adicionar.eventClick = function (e) {
                Salvar(e, "TabelaFrete/AdicionarPorModal", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {
                            Global.fecharModal(modalCadastro);
                            fnPreecherRetornoSelecao(knout, arg.Data, idDiv, knoutOpcoes);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Consultas.TabelaFrete.Sucesso, Localization.Resources.Consultas.TabelaFrete.CadastradoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Consultas.TabelaFrete.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Consultas.TabelaFrete.Falha, arg.Msg);
                    }
                });
            };

            KoBindings(vigencia, knoutCadastro, false);

            knoutOpcoes.AdicionarVigencia.eventClick = function (e) {
                if (permitirAddVigencia && knoutOpcoes.TabelaFrete.codEntity()) {
                    fnAbrirModalAddVigencia(e, knoutCadastro, modalCadastro, vigencia);
                }
            }
        });
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TabelaFrete/PesquisarVigencia", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function () {
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });
}