/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CTe.js" />

var Observacao = function (grid, idKnockout, tamanhoObservacao) {

    var instancia = this;

    instancia.Lista = new Array();
    instancia.Grid = PropertyEntity({ type: types.local });

    instancia.Identificador = PropertyEntity({ text: Localization.Resources.CTes.CTe.Identificador.getRequiredFieldDescription(), maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    instancia.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: tamanhoObservacao, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    instancia.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarObservacao() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, idKnockout);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
            { data: "Identificador", title: Localization.Resources.CTes.CTe.Identificador, width: "20%" },
            { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%" }];

        grid = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        
        instancia.RecarregarGrid();
    }

    this.DestivarObservacao = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        grid.CarregarGrid(instancia.Lista, false);
    }

    this.AdicionarObservacao = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            instancia.Lista.push({
                Codigo: guid(),
                Identificador: instancia.Identificador.val(),
                Descricao: instancia.Descricao.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }

    this.Excluir = function (observacao) {
        for (var i = 0; i < instancia.Lista.length; i++) {
            if (observacao.Codigo == instancia.Lista[i].Codigo) {
                instancia.Lista.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        grid.CarregarGrid(instancia.Lista);
    }
}