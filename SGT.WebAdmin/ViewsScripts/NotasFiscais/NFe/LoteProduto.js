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
/// <reference path="NFe.js" />

var LoteProduto = function (nfe) {

    var instancia = this;

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.CodigoItem = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroLote = PropertyEntity({ text: "Número Lote:", getType: typesKnockout.string, maxlength: 20, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.QuantidadeLote = PropertyEntity({ def: "0,000", val: ko.observable("0,000"), text: "Quantidade Lote:", getType: typesKnockout.decimal, maxlength: 18, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 3, allowZero: true } });
    this.DataFabricacao = PropertyEntity({ getType: typesKnockout.date, text: "Data Fabricação:", required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataValidade = PropertyEntity({ getType: typesKnockout.date, text: "Data Validade:", required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.CodigoAgregacao = PropertyEntity({ text: "Código Agregação:", getType: typesKnockout.string, maxlength: 20, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.SalvarLoteNFe = PropertyEntity({ eventClick: function () { instancia.AdicionarLoteProduto() }, type: types.event, text: "Salvar Lote", visible: ko.observable(true), enable: ko.observable(true) });
    this.LotesNFe = PropertyEntity({ type: types.local, id: guid() });

    this.Load = function () {
        nfe.LotesProdutos = new Array();

        KoBindings(instancia, nfe.IdKnockoutLotes);
        $("#" + instancia.CodigoAgregacao.id).mask("09999999999999999999", { selectOnFocus: true, clearIfNotMatch: true });

        var editarItem = { descricao: "Editar", id: guid(), metodo: instancia.Editar, icone: "" };
        var excluirItem = { descricao: "Excluir", id: guid(), metodo: instancia.Excluir, icone: "" };

        var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [editarItem, excluirItem] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoItem", visible: false },

            { data: "NumeroLote", title: "Número Lote", width: "20%", orderable: true },
            { data: "QuantidadeLote", title: "Quantidade Lote", width: "20%" },
            { data: "DataFabricacao", title: "Data Fabricação", width: "15%" },
            { data: "DataValidade", title: "Data Validade", width: "15%" },
            { data: "CodigoAgregacao", title: "Código Agregação", width: "20%" }
        ];

        nfe.GridLote = new BasicDataTable(instancia.LotesNFe.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

        instancia.RecarregarGrid();
    }

    this.AdicionarLoteProduto = function () {
        var valido = ValidarCamposObrigatorios(instancia);
        instancia.QuantidadeLote.requiredClass("form-control");

        //if (valido) {
        //    if (parseFloat(instancia.QuantidadeLote.val().toString().replace(".", "").replace(",", ".")) == 0) {
        //        valido = false;
        //        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Favor informe a Quantidade do Lote!");
        //        instancia.QuantidadeLote.requiredClass("form-control is-invalid");
        //        return;
        //    }
        //}

        if (valido) {
            if (instancia.Codigo.val() > "0" && instancia.Codigo.val() != undefined) {
                for (var i = 0; i < nfe.LotesProdutos.length; i++) {
                    if (instancia.Codigo.val() == nfe.LotesProdutos[i].Codigo) {
                        nfe.LotesProdutos.splice(i, 1);
                        break;
                    }
                }
            }
            instancia.Codigo.val(guid());

            nfe.LotesProdutos.push({
                Codigo: instancia.Codigo.val(),
                CodigoItem: instancia.CodigoItem.val(),
                NumeroLote: instancia.NumeroLote.val(),
                QuantidadeLote: instancia.QuantidadeLote.val(),
                DataFabricacao: instancia.DataFabricacao.val(),
                DataValidade: instancia.DataValidade.val(),
                CodigoAgregacao: instancia.CodigoAgregacao.val()
            });

            instancia.RecarregarGrid();

            var codigoItem = instancia.CodigoItem.val();
            LimparCampos(instancia);
            instancia.CodigoItem.val(codigoItem);
            $("#" + instancia.NumeroLote.id).focus();
        } else {
            exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        }
    }

    this.RecarregarGrid = function () {
        nfe.GridLote.CarregarGrid(nfe.LotesProdutos.filter(function (obj) { return obj.CodigoItem == instancia.CodigoItem.val(); }));
    }

    this.Excluir = function (loteProduto) {

        if (instancia.Codigo.val() > 0) {
            exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique os lotes pois existe um em edição.");
            return;
        }

        if (instancia.NumeroLote.val() != "") {
            exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique pois existe um Lote sem salvar o mesmo.");
            return
        }

        exibirConfirmacao("Confirmação", "Realmente deseja excluir o item " + loteProduto.NumeroLote + "?", function () {
            for (var i = 0; i < nfe.LotesProdutos.length; i++) {
                if (loteProduto.Codigo == nfe.LotesProdutos[i].Codigo) {
                    nfe.LotesProdutos.splice(i, 1);
                    break;
                }
            }
            instancia.RecarregarGrid();
        });
    }

    this.Editar = function (loteProduto) {

        if (instancia.Codigo.val() > 0) {
            exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique os lotes pois existe um em edição.");
            return;
        }

        if (instancia.NumeroLote.val() != "") {
            exibirMensagem("atencao", "Lote em Edição", "Por favor, verifique pois existe um Lote sem salvar o mesmo.");
            return
        }

        instancia.Codigo.val(loteProduto.Codigo);
        instancia.CodigoItem.val(loteProduto.CodigoItem);
        instancia.NumeroLote.val(loteProduto.NumeroLote);
        instancia.QuantidadeLote.val(Globalize.format(loteProduto.QuantidadeLote, "n3"));
        instancia.DataFabricacao.val(loteProduto.DataFabricacao);
        instancia.DataValidade.val(loteProduto.DataValidade);
        instancia.CodigoAgregacao.val(loteProduto.CodigoAgregacao);

        $("#" + instancia.NumeroLote.id).focus();
    }

    this.DestivarLoteProduto = function () {
        DesabilitarCamposInstanciasNFe(instancia);
        nfe.GridLote.CarregarGrid(nfe.LotesProdutos, false);
    }

    this.HabilitarLoteProduto = function () {
        HabilitarCamposInstanciasNFe(instancia);
    }
}