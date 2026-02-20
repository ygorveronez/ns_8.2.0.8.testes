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

var Duplicata = function (cte) {
    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Parcela = PropertyEntity({ text: Localization.Resources.CTes.CTe.Parcela.getRequiredFieldDescription(), getType: typesKnockout.int, val: ko.observable(1), def: 1, required: true, visible: ko.observable(true), enable: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataDeVencimento.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: Localization.Resources.CTes.CTe.Valor.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarDuplicata() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutDuplicata);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
            { data: "Numero", title: Localization.Resources.CTes.CTe.Numero, width: "20%" },
            { data: "Parcela", title: Localization.Resources.CTes.CTe.Parcela, width: "20%" },
            { data: "DataVencimento", title: Localization.Resources.CTes.CTe.DataDeVencimento, width: "20%" },
            { data: "Valor", title: Localization.Resources.CTes.CTe.Valor, width: "20%" }];

        cte.GridDuplicata = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        cte.Duplicatas = new Array();

        instancia.RecarregarGrid();
    }

    this.DestivarDuplicata = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridDuplicata.CarregarGrid(cte.Duplicatas, false);
    }

    this.AdicionarDuplicata = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.Duplicatas.push({
                Codigo: guid(),
                Numero: "Automático",
                Parcela: Globalize.parseInt(instancia.Parcela.val().toString()),
                DataVencimento: instancia.DataVencimento.val(),
                Valor: instancia.Valor.val()
            });

            instancia.RecarregarGrid();

            instancia.Parcela.def = cte.Duplicatas.length + 1;

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }

    this.Excluir = function (duplicata) {
        for (var i = 0; i < cte.Duplicatas.length; i++) {
            if (duplicata.Codigo == cte.Duplicatas[i].Codigo) {

                for (var j = 0; j < cte.Duplicatas.length; j++)
                    if (cte.Duplicatas[j].Parcela > cte.Duplicatas[i].Parcela)
                        cte.Duplicatas[j].Parcela--;

                cte.Duplicatas.splice(i, 1);

                break;
            }
        }

        instancia.Parcela.val(cte.Duplicatas.length + 1);
        instancia.Parcela.def = cte.Duplicatas.length + 1;

        instancia.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        cte.GridDuplicata.CarregarGrid(cte.Duplicatas);
    }
}