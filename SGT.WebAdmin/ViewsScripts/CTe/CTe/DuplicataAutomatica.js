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
/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumTipoArredondamento.js" />

var DuplicataAutomatica = function (cte) {

    var instancia = this;

    this.QuantidadeParcelas = PropertyEntity({ text: Localization.Resources.CTes.CTe.QuantidadeDeParcelas.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 2, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.FrequenciaVencimento = PropertyEntity({ text: Localization.Resources.CTes.CTe.FrequênciaDeVencimento, getType: typesKnockout.int, maxlength: 2, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.PrimeiroVencimento = PropertyEntity({ text: Localization.Resources.CTes.CTe.PrimeiroVencimento.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoArredondamento = PropertyEntity({ val: ko.observable(EnumTipoArredondamento.UltimoItem), def: EnumTipoArredondamento.UltimoItem, options: EnumTipoArredondamento.ObterOpcoes(), text: Localization.Resources.CTes.CTe.Arredondamento.getRequiredFieldDescription(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Gerar = PropertyEntity({ eventClick: function () { instancia.GerarDuplicata() }, type: types.event, text: Localization.Resources.CTes.CTe.GerarDuplicatas, visible: ko.observable(true), enable: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, cte.IdKnockoutDuplicataAutomatica);
    }

    this.DestivarDuplicataAutomatica = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    }

    this.GerarDuplicata = function () {
        var valido = ValidarCamposObrigatorios(instancia);

        if (valido) {
        
            var quantidadeDuplicatas = Globalize.parseInt(instancia.QuantidadeParcelas.val());
            var frequenciaVencimentos = Globalize.parseInt(instancia.FrequenciaVencimento.val());
            var dataPrimeiroVencimento = Globalize.parseDate(instancia.PrimeiroVencimento.val(), "dd/MM/yyyy");
            var arredondarPrimeiraParcela = instancia.TipoArredondamento.val() == EnumTipoArredondamento.PrimeiroItem ? true : false;

            var valorTotal = 1000; //Globalize.parseFloat($("#txtValorFreteContratado").val());
            var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidadeDuplicatas, "n2"));
            var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidadeDuplicatas), "n2"));

            instancia.Duplicatas = new Array();

            for (var i = 1; i <= quantidadeDuplicatas; i++) {

                if (i != 1)
                    dataPrimeiroVencimento.setDate(dataPrimeiroVencimento.getDate() + frequenciaVencimentos);

                var duplicata = {
                    Codigo: guid(),
                    DataVencimento: moment(dataPrimeiroVencimento).format("DD/MM/YYYY"),
                    Valor: Globalize.format(valorParcela, "n2"),
                    Numero: "Automático",
                    Parcela: i
                };

                cte.Duplicatas.push(duplicata);
            }

            if (valorDiferenca > 0) {
                if (arredondarPrimeiraParcela) {
                    var valor = Globalize.parseFloat(cte.Duplicatas[0].Valor);

                    cte.Duplicatas[0].Valor = Globalize.format(valor + valorDiferenca, "n2");
                } else {
                    var indice = cte.Duplicatas.length - 1;

                    var valor = Globalize.parseFloat(cte.Duplicatas[indice].Valor);

                    cte.Duplicatas[indice].Valor = Globalize.format(valor + valorDiferenca, "n2");
                }
            }

            cte.Duplicata.RecarregarGrid();

            cte.Duplicata.Parcela.def = cte.Duplicatas.length + 1;

            LimparCampos(cte.Duplicata);
            LimparCampos(cte.DuplicataAutomatica);

        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        }
    }
}