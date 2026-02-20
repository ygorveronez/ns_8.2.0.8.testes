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
/// <reference path="../../Enumeradores/EnumUnidadeDeMedidaCTeAereo.js" />
/// <reference path="CTe.js" />

var ProdutoPerigoso = function (cte) {

    var instancia = this;

    this.Grid = PropertyEntity({ type: types.local });

    this.NumeroONU = PropertyEntity({ text: Localization.Resources.CTes.CTe.NumeroONUUN.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 4, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: Localization.Resources.CTes.CTe.Nome.getRequiredFieldDescription(), maxlength: 150, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.ClasseRisco = PropertyEntity({ text: Localization.Resources.CTes.CTe.ClasseDeRisco.getRequiredFieldDescription(), maxlength: 40, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.GrupoEmbalagem = PropertyEntity({ text: Localization.Resources.CTes.CTe.GrupoDeEmbalagem.getFieldDescription(), maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: Localization.Resources.CTes.CTe.Quantidade.getRequiredFieldDescription(), maxlength: 20, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.PontoFulgor = PropertyEntity({ text: Localization.Resources.CTes.CTe.PontoDeFulgor.getFieldDescription(), maxlength: 6, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeTipoVolume = PropertyEntity({ text: Localization.Resources.CTes.CTe.QuantidadeTipoDeVolumes.getFieldDescription(), maxlength: 60, required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeTotal = PropertyEntity({ text: Localization.Resources.CTes.CTe.QuantidadeTotal.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 16, val: ko.observable("0,0000"), def: "0,0000", configDecimal: { precision: 4, allowZero: true, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });

    this.UnidadeDeMedida = PropertyEntity({ val: ko.observable(""), options: EnumUnidadeDeMedidaCTeAereo.obterOpcoes(), def: "", text: Localization.Resources.CTes.CTe.UnidadeDeMedida.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: function () { instancia.AdicionarProduto(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {

        KoBindings(instancia, cte.IdKnockoutProdutoPerigoso);

        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: instancia.Excluir }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "NumeroONU", title: Localization.Resources.CTes.CTe.NumeroONU, width: "15%" },
            { data: "Nome", title: Localization.Resources.CTes.CTe.Nome, width: "25%" },
            { data: "ClasseRisco", title: Localization.Resources.CTes.CTe.ClasseDeRisco, width: "20%" },
            { data: "Quantidade", title: Localization.Resources.CTes.CTe.Quantidade, width: "20%" }
        ];

        cte.GridProdutoPerigoso = new BasicDataTable(instancia.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
        cte.ProdutosPerigosos = new Array();

        instancia.RecarregarGrid();
    };

    this.DestivarProdutoPerigoso = function () {
        DesabilitarCamposInstanciasCTe(instancia);
        cte.GridProdutoPerigoso.CarregarGrid(cte.ProdutosPerigosos, false);
    };

    this.AdicionarProduto = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {

            cte.ProdutosPerigosos.push({
                Codigo: guid(),
                NumeroONU: instancia.NumeroONU.val(),
                Nome: instancia.Nome.val(),
                ClasseRisco: instancia.ClasseRisco.val(),
                GrupoEmbalagem: instancia.GrupoEmbalagem.val(),
                Quantidade: instancia.Quantidade.val(),
                PontoFulgor: instancia.PontoFulgor.val(),
                QuantidadeTipoVolume: instancia.QuantidadeTipoVolume.val(),
                QuantidadeTotal: instancia.QuantidadeTotal.val(),
                UnidadeDeMedida: instancia.UnidadeDeMedida.val()
            });

            instancia.RecarregarGrid();

            LimparCampos(instancia);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    };

    this.Excluir = function (produto) {
        for (var i = 0; i < cte.ProdutosPerigosos.length; i++) {
            if (produto.Codigo === cte.ProdutosPerigosos[i].Codigo) {
                cte.ProdutosPerigosos.splice(i, 1);
                break;
            }
        }

        instancia.RecarregarGrid();
    };

    this.RecarregarGrid = function () {
        cte.GridProdutoPerigoso.CarregarGrid(cte.ProdutosPerigosos);
    };
};