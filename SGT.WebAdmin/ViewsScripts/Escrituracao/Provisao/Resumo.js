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


//*******MAPEAMENTO KNOUCKOUT*******

var _resumo;

var Resumo = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0)});
    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.Filial = PropertyEntity({ text: "Filial: " });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Carga = PropertyEntity({ text: "Carga: " });
    this.Ocorrencia = PropertyEntity({ text: "Ocorrência: " });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: " });
    this.DataFinal = PropertyEntity({ text: "Data Final: " });
    this.Situacao = PropertyEntity({ text: "Situação: " });
    this.Tomador = PropertyEntity({ text: "Tomador: " });
    this.QuantidadeDocumentos = PropertyEntity({ text: "Quantidade Documentos: " });
    this.ValorFrete = PropertyEntity({ text: "Valor Frete: " });
    this.ValorProvisao = PropertyEntity({ text: "Valor Provisão: " });
}


//*******EVENTOS*******

function loadResumo() {
    _resumo = new Resumo();
    KoBindings(_resumo, "knockoutResumo");
}


//*******MÉTODOS*******

function PreecherResumo(data) {
    if (data.Resumo != null)
        PreencherObjetoKnout(_resumo, { Data: data.Resumo});
}

function LimparResumo() {
    LimparCampos(_resumo);
}