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
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCIOT.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _resumoCIOT;

var Resumo = function () {
    this.Numero = PropertyEntity({ text: "Número: ", visible: ko.observable(false) });
    this.CodigoVerificador = PropertyEntity({ text: "Código Verificador: " });
    this.Transportador = PropertyEntity({ text: "Transportador: " });
    this.DataTermino = PropertyEntity({ text: "Data de Término: " });
    this.DataAbertura = PropertyEntity({ text: "Data de Abertura: " });
    this.DataFechamento = PropertyEntity({ text: "Data do Encerramento: ", visible: ko.observable(false) });
    this.DataCancelamento = PropertyEntity({ text: "Data de Cancelamento: ", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: "Situação: " });
}

//*******EVENTOS*******

function LoadResumo() {
    _resumoCIOT = new Resumo();
    KoBindings(_resumoCIOT, "knockoutResumoCIOT");
}

//*******MÉTODOS*******

function PreecherResumoCIOT(dados) {
    _resumoCIOT.Numero.visible(true);

    if (dados.Situacao == EnumSituacaoCIOT.Cancelado)
        _resumoCIOT.DataCancelamento.visible(true);
    else if (dados.Situacao == EnumSituacaoCIOT.Encerrado)
        _resumoCIOT.DataFechamento.visible(true);

    _resumoCIOT.Numero.val(dados.Numero);
    _resumoCIOT.CodigoVerificador.val(dados.CodigoVerificador);
    _resumoCIOT.Transportador.val(dados.Transportador.Descricao);
    _resumoCIOT.DataTermino.val(dados.DataFinalViagem);
    _resumoCIOT.DataAbertura.val(dados.DataAbertura);
    _resumoCIOT.DataFechamento.val(dados.DataFechamento);
    _resumoCIOT.DataCancelamento.val(dados.DataCancelamento);
    _resumoCIOT.Situacao.val(dados.DescricaoSituacao);
}

function LimparResumoCIOT() {
    _resumoCIOT.Numero.visible(false);
    _resumoCIOT.DataCancelamento.visible(false);
    _resumoCIOT.DataFechamento.visible(false);
    LimparCampos(_resumoCIOT);
}