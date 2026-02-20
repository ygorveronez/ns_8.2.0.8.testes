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
/// <reference path="../../Enumeradores/EnumTipoDocumentoCTe.js" />
/// <reference path="CTe.js" />

var ModalAquaviarioContainerDocumento = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Serie = PropertyEntity({ text: "*Série:", maxlength: 3, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "*Número:", maxlength: 20, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.Chave = PropertyEntity({ text: "*Chave:", maxlength: 44, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.UnidadeMedidaRateada = PropertyEntity({ text: "Unidade Medida Rateada:", getType: typesKnockout.decimal, maxlength: 6, val: ko.observable("0,00"), def: "0,00", configDecimal: { precision: 2, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCTe.NFeNotaFiscalEletronica), options: EnumTipoDocumentoCTe.obterOpcoes(), def: EnumTipoDocumentoCTe.NFeNotaFiscalEletronica, text: "*Tipo do Documento: ", required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarContainerDocumento(); }, type: types.event, text: "Adicionar Documento", visible: ko.observable(true), enable: ko.observable(true) });

    instancia.TipoDocumento.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoDocumentoCTe.NFeNotaFiscalEletronica) {
            instancia.Serie.visible(false);
            instancia.Serie.required(false);
            instancia.Numero.visible(false);
            instancia.Numero.required(false);

            instancia.Chave.visible(true);
            instancia.Chave.required(true);
        } else if (novoValor === EnumTipoDocumentoCTe.NotaFiscal) {
            instancia.Chave.visible(false);
            instancia.Chave.required(false);

            instancia.Serie.visible(true);
            instancia.Serie.required(true);
            instancia.Numero.visible(true);
            instancia.Numero.required(true);
        }
    });

    this.Load = function () {
        cte.ContainerDocumentos = new Array();

        KoBindings(instancia, cte.IdKnockoutModalAquaviarioContainerDocumento);
        $("#" + instancia.Chave.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoContainer", visible: false },
            { data: "CodigoTipoDocumento", visible: false },
            { data: "TipoDocumento", title: "Tipo Documento", width: "20%" },

            { data: "Serie", title: "Série", width: "10%" },
            { data: "Numero", title: "Número", width: "10%" },

            { data: "Chave", title: "Chave", width: "30%" },
            { data: "UnidadeMedidaRateada", title: "UN Rateada", width: "10%" }
        ];

        cte.GridContainerDocumento = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarContainerDocumento = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridContainerDocumento.CarregarGrid(cte.ContainerDocumentos, false);
    };

    this.AdicionarContainerDocumento = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.ContainerDocumentos.push({
                Codigo: guid(),
                CodigoContainer: "",
                CodigoTipoDocumento: instancia.TipoDocumento.val(),
                TipoDocumento: $("#" + instancia.TipoDocumento.id + " option:selected").text(),
                Serie: instancia.Serie.val(),
                Numero: instancia.Numero.val(),
                Chave: instancia.Chave.val(),
                UnidadeMedidaRateada: instancia.UnidadeMedidaRateada.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        }
    };

    this.Excluir = function (container) {
        for (var i = 0; i < cte.ContainerDocumentos.length; i++) {
            if (container.Codigo === cte.ContainerDocumentos[i].Codigo) {
                cte.ContainerDocumentos.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridContainerDocumento.CarregarGrid(cte.ContainerDocumentos.filter(function (obj) { return obj.CodigoContainer === ""; }));
    };
};