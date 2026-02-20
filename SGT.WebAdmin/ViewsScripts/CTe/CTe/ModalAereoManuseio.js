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
/// <reference path="../../Enumeradores/EnumInformacaoManuseio.js" />
/// <reference path="CTe.js" />

var ModalAereoManuseio = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });
    this.InformacaoManuseio = PropertyEntity({ val: ko.observable(""), options: EnumInformacaoManuseio.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.InformacaoDeManuseio.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarManuseio(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Manuseios = new Array();

        KoBindings(instancia, cte.IdKnockoutModalAereoManuseio);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoInformacaoManuseio", visible: false },
            { data: "DescricaoInformacaoManuseio", title: Localization.Resources.CTes.CTe.InformacaoManuseio, width: "80%" }
        ];

        cte.GridManuseio = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarManuseio = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridManuseio.CarregarGrid(cte.Manuseios, false);
    };

    this.AdicionarManuseio = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            for (var i = 0; i < cte.Manuseios.length; i++) {
                if (instancia.InformacaoManuseio.val() === cte.Manuseios[i].CodigoInformacaoManuseio) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.ItemExistente, Localization.Resources.CTes.CTe.InformacaoDeManuseioJaInformada);
                    return;
                }
            }

            cte.Manuseios.push({
                Codigo: guid(),
                CodigoInformacaoManuseio: instancia.InformacaoManuseio.val(),
                DescricaoInformacaoManuseio: $("#" + instancia.InformacaoManuseio.id + "  option:selected").text()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (manuseio) {
        for (var i = 0; i < cte.Manuseios.length; i++) {
            if (manuseio.Codigo === cte.Manuseios[i].Codigo) {
                cte.Manuseios.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridManuseio.CarregarGrid(cte.Manuseios);
    };
};