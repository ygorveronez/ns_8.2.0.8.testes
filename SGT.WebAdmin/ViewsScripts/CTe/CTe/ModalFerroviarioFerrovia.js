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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="CTe.js" />

var ModalFerroviarioFerrovia = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Ferrovia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Ferrovia.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarFerrovia(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Ferrovias = new Array();

        KoBindings(instancia, cte.IdKnockoutModalFerroviarioFerrovia);

        new BuscarClientes(instancia.Ferrovia);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Ferrovia", visible: false },
            { data: "DescricaoFerrovia", title: Localization.Resources.CTes.CTe.Ferrovia, width: "80%" }
        ];

        cte.GridFerrovia = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarFerrovia = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridFerrovia.CarregarGrid(cte.Ferrovias, false);
    };

    this.AdicionarFerrovia = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            for (var i = 0; i < cte.Ferrovias.length; i++) {
                if (instancia.Ferrovia.codEntity() === cte.Ferrovias[i].Ferrovia) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.ItemExistente, Localization.Resources.CTes.CTe.FerroviaSelecionadaJaFoiInformada);
                    return;
                }
            }

            cte.Ferrovias.push({
                Codigo: guid(),
                Ferrovia: instancia.Ferrovia.codEntity(),
                DescricaoFerrovia: instancia.Ferrovia.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (ferrovia) {
        for (var i = 0; i < cte.Ferrovias.length; i++) {
            if (ferrovia.Codigo === cte.Ferrovias[i].Codigo) {
                cte.Ferrovias.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridFerrovia.CarregarGrid(cte.Ferrovias);
    };
};