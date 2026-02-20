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
/// <reference path="../../Consultas/Container.js" />
/// <reference path="CTe.js" />

var ModalAquaviarioContainer = function (cte, documentosContainer) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Lacre1 = PropertyEntity({ text: Localization.Resources.CTes.CTe.LacreUm.getFieldDescription(), maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.Lacre2 = PropertyEntity({ text: Localization.Resources.CTes.CTe.LacreDois.getFieldDescription(), maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.Lacre3 = PropertyEntity({ text: Localization.Resources.CTes.CTe.LacreTres.getFieldDescription(), maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Container.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarContainer(); }, type: types.event, text: Localization.Resources.CTes.CTe.AdicionarContainer, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        cte.Containers = new Array();

        KoBindings(instancia, cte.IdKnockoutModalAquaviarioContainer);

        new BuscarContainers(instancia.Container);

        var menuOpcoes = { tipo: TypeOptionMenu.list, tamanho: 7, opcoes: [{ descricao: Localization.Resources.CTes.CTe.DetalheDocumentos, id: guid(), metodo: instancia.Detalhe }, { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "Container", visible: false },
            { data: "DescricaoContainer", title: Localization.Resources.CTes.CTe.Container, width: "44%" },
            { data: "Lacre1", title: Localization.Resources.CTes.CTe.LacreUm, width: "12%" },
            { data: "Lacre2", title: Localization.Resources.CTes.CTe.LacreDois, width: "12%" },
            { data: "Lacre3", title: Localization.Resources.CTes.CTe.LacreTres, width: "12%" }
        ];

        cte.GridContainer = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    };

    this.DestivarContainer = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridContainer.CarregarGrid(cte.Containers, false);
    };

    this.AdicionarContainer = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            for (var i = 0; i < cte.Containers.length; i++) {
                if (instancia.Container.codEntity() === cte.Containers[i].Container) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.ItemExistente, Localization.Resources.CTes.CTe.ContainerSelecionadoJaFoiInformado);
                    return;
                }
            }

            var codigoItem = guid();
            for (var j = 0; j < cte.ContainerDocumentos.length; j++) {
                if (cte.ContainerDocumentos[j].CodigoContainer === "")
                    cte.ContainerDocumentos[j].CodigoContainer = codigoItem;
            }

            cte.Containers.push({
                Codigo: codigoItem,
                Container: instancia.Container.codEntity(),
                DescricaoContainer: instancia.Container.val(),
                Lacre1: instancia.Lacre1.val(),
                Lacre2: instancia.Lacre2.val(),
                Lacre3: instancia.Lacre3.val()
            });

            instancia.RecarregarGrid();
            documentosContainer.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Detalhe = function (container) {
        new DetalheDocumentosContainer(cte, container);
        Global.abrirModal('divDetalheDocumentosContainer');
    };

    this.Excluir = function (container) {
        for (var i = 0; i < cte.Containers.length; i++) {
            if (container.Codigo === cte.Containers[i].Codigo) {

                for (var j = 0; j < cte.ContainerDocumentos.length; j++) {
                    if (cte.ContainerDocumentos[j].CodigoContainer === "" || cte.ContainerDocumentos[j].CodigoContainer === container.Codigo)
                        cte.ContainerDocumentos.splice(j, 1);
                }

                cte.Containers.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
        documentosContainer.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridContainer.CarregarGrid(cte.Containers);
    };
};

var DetalheDocumentosContainer = function (cte, container) {

    var instancia = this;
    var _gridDocumentosContainer;

    this.Grid = PropertyEntity({ type: types.local });

    this.Load = function () {
        KoBindings(instancia, "knoutDetalheDocumentosContainer");

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoContainer", visible: false },
            { data: "CodigoTipoDocumento", visible: false },
            { data: "TipoDocumento", title: Localization.Resources.CTes.CTe.TipoDocumento, width: "20%" },

            { data: "Serie", title: Localization.Resources.CTes.CTe.Serie, width: "10%" },
            { data: "Numero", title: Localization.Resources.CTes.CTe.Numero, width: "10%" },

            { data: "Chave", title: Localization.Resources.CTes.CTe.Chave, width: "30%" },
            { data: "UnidadeMedidaRateada", title: Localization.Resources.CTes.CTe.UNRateada, width: "10%" }
        ];

        _gridDocumentosContainer = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 15);

        instancia.RecarregarGrid();
    };

    this.Excluir = function (documento) {
        for (var i = 0; i < cte.ContainerDocumentos.length; i++) {
            if (documento.Codigo === cte.ContainerDocumentos[i].Codigo) {
                cte.ContainerDocumentos.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        _gridDocumentosContainer.CarregarGrid(cte.ContainerDocumentos.filter(function (obj) { return obj.CodigoContainer === container.Codigo; }));
    };

    instancia.Load();
};