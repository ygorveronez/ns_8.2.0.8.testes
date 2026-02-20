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
/// <reference path="../../Consultas/UnidadeMedida.js" />
/// <reference path="CTe.js" />

var QuantidadeCarga = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.Quantidade = PropertyEntity({ text: Localization.Resources.CTes.CTe.Quantidade.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 11, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false } });
    this.UnidadeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.UnidadeDeMedida, idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ExibirTipoMedida = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoMedida = PropertyEntity({ text: Localization.Resources.CTes.CTe.ExibirOutraDescricaoParaUnidadeDeMedida, required: ko.observable(false), enable: ko.observable(false), maxlength: 20, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarQuantidadeCarga() }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutQuantidadeCarga);

        new BuscarUnidadesMedida(instancia.UnidadeMedida);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoUnidadeMedida", visible: false },
            { data: "Quantidade", title: Localization.Resources.CTes.CTe.Quantidade, width: "25%" },
            { data: "UnidadeMedida", title: Localization.Resources.CTes.CTe.UnidadeDeMedida, width: "25%" },
            { data: "TipoMedida", title: Localization.Resources.CTes.CTe.TipoDaMedida, width: "25%" }
        ];

        cte.GridQuantidadeCarga = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc }, null, 5);
        cte.QuantidadesCarga = new Array();

        instancia.RecarregarGrid();
    };

    this.DestivarQuantidadeCarga = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridQuantidadeCarga.CarregarGrid(cte.QuantidadesCarga, false);
    };

    this.AdicionarQuantidadeCarga = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.QuantidadesCarga.push({
                Codigo: guid(),
                CodigoUnidadeMedida: instancia.UnidadeMedida.codEntity(),
                UnidadeMedida: instancia.UnidadeMedida.val(),
                TipoMedida: !string.IsNullOrWhiteSpace(instancia.TipoMedida.val()) ? instancia.TipoMedida.val() : instancia.UnidadeMedida.val(),
                Quantidade: instancia.Quantidade.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (quantidadeCarga) {
        for (var i = 0; i < cte.QuantidadesCarga.length; i++) {
            if (quantidadeCarga.Codigo == cte.QuantidadesCarga[i].Codigo) {
                cte.QuantidadesCarga.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.Validar = function () {
        if (cte.QuantidadesCarga == null || cte.QuantidadesCarga.length <= 0) {
            $('a[href="#divInformacoes_' + cte.IdModal + '"]').tab("show");
            $('a[href="#divInformacoesCarga_' + cte.IdModal + '"]').tab("show");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.QuantidadeDaCargaObrigatorio, Localization.Resources.CTes.CTe.NecessarioAdicionarAoMenosUmaQuantidadeDaCargaParaEmitirCTe);
            return false;
        }

        return true;
    };

    this.RecarregarGrid = function () {
        cte.GridQuantidadeCarga.CarregarGrid(cte.QuantidadesCarga);
    };

    this.ExibirTipoMedida.val.subscribe(function (novoValor) {
        if (novoValor) {
            instancia.TipoMedida.required(true);
            instancia.TipoMedida.enable(true);
        } else {
            instancia.TipoMedida.enable(false);
            instancia.TipoMedida.required(false);
            instancia.TipoMedida.val("");
        }
    });
};