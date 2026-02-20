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
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDadoBancario;
var _dadoBancario;

var DadoBancario = function () {
    let self = this;
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.Banco.getFieldDescription(), issue: 49, idBtnSearch: guid(), required: false });
    this.Agencia = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.Agencia.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 10 });
    this.Digito = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.Digito.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 1 });
    this.NumeroConta = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.NumeroConta.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 10 });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoesBordero(), def: EnumTipoConta.Corrente, text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), required: false });
    this.CnpjIpef = PropertyEntity({ text: "CNPJ IPEF", maxlength: 18, visible: ko.observable(true), getType: typesKnockout.cpfCnpj });
    this.TipoChavePIX = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TipoChavePIX.getFieldDescription(), options: EnumTipoChavePix.obterOpcoes(), val: ko.observable(EnumTipoChavePix.Nenhum), def: EnumTipoChavePix.Nenhum, visible: ko.observable(true) });

    this.EmpresaFavorecida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.EmpresaFavorecida.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.AntecipacaoPagamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.AntecipacaoPagamento });

    this.ChavePIXCPFCNPJ = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ChavePIXCPFCNPJ.getFieldDescription(), maxlength: 18, visible: ko.observable(false), getType: typesKnockout.cpfCnpj });
    this.ChavePIXEmail = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ChavePIXEmail.getFieldDescription(), maxlength: 200, visible: ko.observable(false), getType: typesKnockout.email });
    this.ChavePIXCelular = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ChavePIXCelular.getFieldDescription(), maxlength: 15, visible: ko.observable(false), getType: typesKnockout.phone });
    this.ChavePIXAleatoria = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ChavePIXAleatoria.getFieldDescription(), maxlength: 200, visible: ko.observable(false), getType: typesKnockout.text });

    this.TipoChavePIX.val.subscribe(function (novoValor) {

        self.ChavePIXCPFCNPJ.visible(false);
        self.ChavePIXEmail.visible(false);
        self.ChavePIXCelular.visible(false);
        self.ChavePIXAleatoria.visible(false);

        switch (novoValor) {
            case EnumTipoChavePix.CPFCNPJ: self.ChavePIXCPFCNPJ.visible(true); break;
            case EnumTipoChavePix.Email: self.ChavePIXEmail.visible(true); break;
            case EnumTipoChavePix.Celular: self.ChavePIXCelular.visible(true); break;
            case EnumTipoChavePix.Aleatoria: self.ChavePIXAleatoria.visible(true); break;
        }
    });
};

//*******EVENTOS*******

function loadDadoBancario() {
    $("#liTabDadosBancarios").show();
    _dadoBancario = new DadoBancario();
    KoBindings(_dadoBancario, "knockoutCadastroDadosBancarios");

    if (_CONFIGURACAO_TMS.PermitirInformarEmpresaFavorecidaNosDadosBancarios)
        _dadoBancario.EmpresaFavorecida.visible(true);

    new BuscarBanco(_dadoBancario.Banco);
    new BuscarTransportadores(_dadoBancario.EmpresaFavorecida);
}

function limparCamposDadoBancario() {
    LimparCampos(_dadoBancario);
}