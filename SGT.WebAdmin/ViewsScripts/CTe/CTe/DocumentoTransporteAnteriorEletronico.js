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
/// <reference path="CTe.js" />

var DocumentoTransporteAnteriorEletronico = function (cte) {
    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Emitente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Chave = PropertyEntity({ text: Localization.Resources.CTes.CTe.Chave.getRequiredFieldDescription(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarDocumento() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        cte.DocumentosTransporteAnteriorEletronico = new Array();

        KoBindings(instancia, cte.IdKnockoutDocumentoTransporteAnteriorEletronico);

        new BuscarClientes(instancia.Emitente);

        $("#" + instancia.Chave.id).mask("0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000", { selectOnFocus: true, clearIfNotMatch: true });

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [{ data: "Codigo", visible: false },
        { data: "Chave", title: Localization.Resources.CTes.CTe.Chave, width: "50%" },
        { data: "Emitente", title: Localization.Resources.CTes.CTe.Emitente, width: "30%" }];

        cte.GridDocumentoTransporteAnteriorEletronico = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    }


    this.DestivarDocumentoTransporteAnteriorEletronico = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridDocumentoTransporteAnteriorEletronico.CarregarGrid(cte.DocumentosTransporteAnteriorEletronico, false)
    }

    this.AdicionarDocumento = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.DocumentosTransporteAnteriorEletronico.push({
                Codigo: guid(),
                CodigoEmitente: instancia.Emitente.codEntity(),
                Chave: instancia.Chave.val(),
                Emitente: instancia.Emitente.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }

    this.Excluir = function (documento) {
        for (var i = 0; i < cte.DocumentosTransporteAnteriorEletronico.length; i++) {
            if (documento.Codigo == cte.DocumentosTransporteAnteriorEletronico[i].Codigo) {
                cte.DocumentosTransporteAnteriorEletronico.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    }

    this.RecarregarGrid = function () {
        cte.GridDocumentoTransporteAnteriorEletronico.CarregarGrid(cte.DocumentosTransporteAnteriorEletronico);
    }
}