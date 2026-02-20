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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoTransporteAnteriorPapelCTe.js" />
/// <reference path="CTe.js" />

var DocumentoTransporteAnteriorPapel = function (cte) {
    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Emitente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoDocumentoTransporteAnteriorPapelCTe.ATRE), def: EnumTipoDocumentoTransporteAnteriorPapelCTe.ATRE, options: EnumTipoDocumentoTransporteAnteriorPapelCTe.obterOpcoes(), text: Localization.Resources.CTes.CTe.TipoDeDocumento.getRequiredFieldDescription(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: Localization.Resources.CTes.CTe.DataDeEmissao.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.CTes.CTe.Numero.getRequiredFieldDescription(), maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Serie = PropertyEntity({ text: Localization.Resources.CTes.CTe.Serie.getRequiredFieldDescription(), maxlength: 3, required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarDocumento() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutDocumentoTransporteAnteriorPapel);

        new BuscarClientes(instancia.Emitente);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Numero", title: Localization.Resources.CTes.CTe.Numero, width: "20%" },
            { data: "Serie", title: Localization.Resources.CTes.CTe.Serie, width: "15%" },
            { data: "DataEmissao", title: Localization.Resources.CTes.CTe.DataDeEmissao, width: "20%" },
            { data: "Emitente", title: Localization.Resources.CTes.CTe.Emitente, width: "25%" }
        ];

        cte.GridDocumentoTransporteAnteriorPapel = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        cte.DocumentosTransporteAnteriorPapel = new Array();

        instancia.RecarregarGrid();
    };

    this.DestivarDocumentoTransporteAnteriorPapel = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridDocumentoTransporteAnteriorPapel.CarregarGrid(cte.DocumentosTransporteAnteriorPapel, false);
    };

    this.AdicionarDocumento = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.DocumentosTransporteAnteriorPapel.push({
                Codigo: guid(),
                CodigoEmitente: instancia.Emitente.codEntity(),
                Numero: instancia.Numero.val(),
                Serie: instancia.Serie.val(),
                DataEmissao: instancia.DataEmissao.val(),
                Emitente: instancia.Emitente.val(),
                Tipo: instancia.Tipo.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (documento) {
        for (var i = 0; i < cte.DocumentosTransporteAnteriorPapel.length; i++) {
            if (documento.Codigo == cte.DocumentosTransporteAnteriorPapel[i].Codigo) {
                cte.DocumentosTransporteAnteriorPapel.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridDocumentoTransporteAnteriorPapel.CarregarGrid(cte.DocumentosTransporteAnteriorPapel);
    };
};