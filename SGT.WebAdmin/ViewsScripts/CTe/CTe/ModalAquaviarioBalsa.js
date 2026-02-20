/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
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

var ModalAquaviarioBalsa = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });
    this.Balsa = PropertyEntity({ text: Localization.Resources.CTes.CTe.IdentificadorDaBalsa.getRequiredFieldDescription(), maxlength: 20, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarBalsa(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Balsas = new Array();

        KoBindings(instancia, cte.IdKnockoutModalAquaviarioBalsa);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Balsa", title: Localization.Resources.CTes.CTe.Balsa, width: "80%" }
        ];

        cte.GridBalsa = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarBalsa = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridBalsa.CarregarGrid(cte.Balsas, false);
    };

    this.AdicionarBalsa = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            if (cte.Balsas.length === 3) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.InformacoesDeBalsas, Localization.Resources.CTes.CTe.PermitidoInformarNoMaximoTresBalsas);
                return;
            }

            cte.Balsas.push({
                Codigo: guid(),
                Balsa: instancia.Balsa.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (balsa) {
        for (var i = 0; i < cte.Balsas.length; i++) {
            if (balsa.Codigo === cte.Balsas[i].Codigo) {
                cte.Balsas.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridBalsa.CarregarGrid(cte.Balsas);
    };
};