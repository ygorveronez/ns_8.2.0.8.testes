/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var KnoutCadastrarSeguradora = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome.getRequiredFieldDescription(), maxlength: 150, required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

var BuscarSeguradoras = function (knout, callbackRetorno, permitirAddSeguradora) {

    var idDiv = guid();
    var gridConsulta;

    if (permitirAddSeguradora == null)
        permitirAddSeguradora = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Seguradora.ConsultaDeSeguradoras, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Seguradora.DescricaoSeguradoras, type: types.local });
        this.Nome = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
        this.Ativo = PropertyEntity({ visible: false, val: ko.observable(1), def: 1 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });

        this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Consultas.Seguradora.AdicionarSeguradora, visible: permitirAddSeguradora, cssClass: "btn btn-default", icon: " " });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);
    


    var callback = function (e) {
        Global.fecharModal(idDiv)
        PreencherRetornoSelecaoSeguradora(e, knout);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            Global.fecharModal(idDiv);
            callbackRetorno(e);
        }
    }

    if (permitirAddSeguradora) {
        var knoutCadastro = idDiv + "_knockoutCadastroSeguradora";
        var modalCadastro = idDiv + "divModalCadastrarSeguradora";

        $.get("Content/Static/Consultas/Cadastros/Seguradora.html?dyn=" + guid(), function (data) {

            var html = data.replace(/#knockoutCadastroSeguradora/g, knoutCadastro).replace(/#divModalCadastrarSeguradora/g, modalCadastro);

            $('#js-page-content').append(html);

            var seguradora = new KnoutCadastrarSeguradora();

            seguradora.Cancelar.eventClick = function (e) {
                Global.fecharModal(modalCadastro);
            };

            seguradora.Adicionar.eventClick = function (e) {
                Salvar(e, "Seguradora/Adicionar", function (arg) {
                    if (arg.Success) {
                        if (arg.Data != false) {                            
                            Global.fecharModal(modalCadastro);                            
                            divBusca.CloseModal();
                            PreencherRetornoSelecaoSeguradora(arg.Data, knout);
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
                        }
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    }
                });
            };

            KoBindings(seguradora, knoutCadastro, false);

            knoutOpcoes.Adicionar.eventClick = function (e) {
                abrirModalAddSeguradora(e, knoutCadastro, modalCadastro, seguradora);
            }
        });
    }

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "Seguradora/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {

        if (outraPressionada)
            knoutOpcoes.Nome.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });

    });
}

function PreencherRetornoSelecaoSeguradora(dados, knout) {
    knout.codEntity(dados.Codigo);
    knout.val(dados.Nome);
}

function abrirModalAddSeguradora(e, knoutCadastro, modalCadastro, seguradora) {

    seguradora.Nome.val(e.Nome.val());
        
    Global.abrirModal(modalCadastro);
}