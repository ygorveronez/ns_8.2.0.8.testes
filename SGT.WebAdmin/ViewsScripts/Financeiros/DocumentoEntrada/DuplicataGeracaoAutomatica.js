/// <reference path="Duplicata.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/CFOP.js" />
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
/// <reference path="DocumentoEntrada.js" />
/// <reference path="../../Enumeradores/EnumUnidadeMedida.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _duplicataGeracaoAutomatica;

var DuplicataGeracaoAutomatica = function () {
    this.Parcelas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: "*Quantidade de Parcelas:", maxlength: 2, required: true, enable: ko.observable(true) });
    this.QuantidadeDias = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, text: "*Intervalo de Dias (Ex.: 20.30.40 para intervalos diferentes):", maxlength: 500, required: true, enable: ko.observable(true) });
    this.DataPrimeiroVencimento = PropertyEntity({ text: "*Data de Vencimento:", getType: typesKnockout.date, required: true, enable: ko.observable(true) });
    this.Arredondamento = PropertyEntity({ val: ko.observable(0), def: 0, options: EnumTipoArredondamento.ObterOpcoes(), text: "*Arredondar na:", required: true, enable: ko.observable(true) });

    this.GerarDuplicatas = PropertyEntity({ eventClick: GerarDuplicatasClick, type: types.event, text: "Gerar Duplicatas", enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadDuplicataGeracaoAutomatica() {

    _duplicataGeracaoAutomatica = new DuplicataGeracaoAutomatica();
    KoBindings(_duplicataGeracaoAutomatica, "knockoutDuplicataGeracaoAutomatica");

}

function GerarDuplicatasClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_duplicataGeracaoAutomatica);

    if (valido) {

        if (_documentoEntrada.Duplicatas.list.length > 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "Já existem duplicatas geradas para este documento de entrada.");
            return;
        }

        var valorTotal = Globalize.parseFloat(_documentoEntrada.ValorTotal.val());

        if (isNaN(valorTotal) || valorTotal <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "O valor total do documento de entrada deve ser maior que zero para a geração das duplicatas.");
            return;
        }

        if (_documentoEntrada.Numero.val() == "") {
            exibirMensagem(tipoMensagem.atencao, "Atenção!", "Informe o número do documento de entrada para a geração das duplicatas.");
            return;
        }

        var quantidadeDuplicatas = Globalize.parseInt(_duplicataGeracaoAutomatica.Parcelas.val());
        //var frequenciaVencimentos = Globalize.parseInt(_duplicataGeracaoAutomatica.QuantidadeDias.val());
        var x = _duplicataGeracaoAutomatica.QuantidadeDias.val();
        if (x.indexOf(".") >= 0) {
            var arrayDias = x.split(".");
            if (arrayDias.length != quantidadeDuplicatas) {
                exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "As quantidades das parcelas não estão de acordo com o intervalo de dias informado!");
                return;
            }
            for (var i = 0; i < arrayDias.length; i++) {
                //if (!parseInt(arrayDias[i]) > 0 || parseInt(arrayDias[i]) == NaN) {
                if (parseInt(arrayDias[i]) == NaN) {
                    exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                    return;
                }
            }
        } else {
            var arrayDias = new Array;
            arrayDias[0] = x;
            if (!parseInt(arrayDias[0]) > 0 || parseInt(arrayDias[0]) == NaN) {
                exibirMensagem(tipoMensagem.atencao, "Intervalo de Dias", "O intervalo de dias está fora do formato desejado!");
                return;
            }
        }

        //var dataPrimeiroVencimento = Globalize.parseDate(_duplicataGeracaoAutomatica.DataPrimeiroVencimento.val(), "dd/MM/yyyy");
        var dataUltimaParcela = _duplicataGeracaoAutomatica.DataPrimeiroVencimento.val();
        var arredondarPrimeiraParcela = _duplicataGeracaoAutomatica.Arredondamento.val() == "0" ? true : false;
        var valorParcela = Globalize.parseFloat(Globalize.format(valorTotal / quantidadeDuplicatas, "n2"));
        var valorDiferenca = Globalize.parseFloat(Globalize.format(valorTotal - (valorParcela * quantidadeDuplicatas), "n2"));

        if (isNaN(quantidadeDuplicatas) || quantidadeDuplicatas <= 0)
            quantidadeDuplicatas = 1;

        //if (isNaN(frequenciaVencimentos) || frequenciaVencimentos <= 0)
        //    frequenciaVencimentos = 30;

        //if (dataPrimeiroVencimento == null)
        //    dataPrimeiroVencimento = new Date();

        for (var i = 0; i < quantidadeDuplicatas; i++) {
            //if (i != 1)
            //    dataPrimeiroVencimento.setDate(dataPrimeiroVencimento.getDate() + frequenciaVencimentos);

            dataUltimaParcela = dataUltimaParcela.substr(6, 4) + "/" + dataUltimaParcela.substr(3, 2) + "/" + dataUltimaParcela.substr(0, 2);
            var dataVencimento = new Date(dataUltimaParcela);
            if (i > 0) {
                if (arrayDias.length > 1)
                    dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[i]));
                else
                    dataVencimento.setDate(dataVencimento.getDate() + parseInt(arrayDias[0]));
            }

            _duplicata.Codigo.val(guid());

            _duplicata.DataVencimento.val(moment(dataVencimento).format("DD/MM/YYYY"));
            _duplicata.Sequencia.val(i + 1);
            _duplicata.Numero.val(_documentoEntrada.Numero.val() + "/" + (i + 1).toString());

            if ((arredondarPrimeiraParcela && i == 1) || (!arredondarPrimeiraParcela && i == quantidadeDuplicatas))
                _duplicata.Valor.val(Globalize.format((valorParcela + valorDiferenca), "n2"));
            else
                _duplicata.Valor.val(Globalize.format(valorParcela, "n2"));
            _duplicata.FormaTitulo.val(EnumFormaTitulo.Outros);

            var yyyy = dataVencimento.getFullYear().toString();
            var mm = (dataVencimento.getMonth() + 1).toString();
            var dd = dataVencimento.getDate().toString();

            dataUltimaParcela = (dd[1] ? dd : "0" + dd[0]) + "/" + (mm[1] ? mm : "0" + mm[0]) + "/" + yyyy;

            _documentoEntrada.Duplicatas.list.push(SalvarListEntity(_duplicata));
        }

        RecarregarGridDuplicata();
        LimparCamposDuplicata();
        LimparCamposDuplicataGeracaoAutomatica();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposDuplicataGeracaoAutomatica() {
    LimparCampos(_duplicataGeracaoAutomatica);
}