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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaDBTrans.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaFreteDBTrans.js" />
/// <reference path="../../Enumeradores/EnumMeioPagamentoDBTrans.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoDBTrans;

var IntegracaoDBTrans = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(true) });
    this.CodigoCliente = PropertyEntity({ text: "*Código Cliente: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: "*Usuário: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Senha = PropertyEntity({ text: "*Senha: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.IdLocalImpressao = PropertyEntity({ text: "*ID Local Impressão: ", getType: typesKnockout.int, enable: ko.observable(true), required: ko.observable(true) });

    this.TipoRota = PropertyEntity({ text: "*Tipo Rota:", val: ko.observable(EnumTipoRotaDBTrans.RotaFixa), options: EnumTipoRotaDBTrans.obterOpcoes(), def: EnumTipoRotaDBTrans.RotaFixa, enable: ko.observable(true), required: ko.observable(true) });
    this.TipoRotaFrete = PropertyEntity({ text: "Tipo Rota Frete:", val: ko.observable(EnumTipoRotaFreteDBTrans.NaoEspecificado), options: EnumTipoRotaFreteDBTrans.obterOpcoes(), def: EnumTipoRotaFreteDBTrans.NaoEspecificado, enable: ko.observable(true), visible: ko.observable(false) });
    this.MeioPagamento = PropertyEntity({ text: "*Meio de Pagamento:", val: ko.observable(EnumMeioPagamentoDBTrans.Cupom), options: EnumMeioPagamentoDBTrans.obterOpcoes(), def: EnumMeioPagamentoDBTrans.Cupom, enable: ko.observable(true), required: ko.observable(true) });
    this.TipoTomador = PropertyEntity({ text: "Tipo de Tomador:", val: ko.observable(EnumTipoTomador.Todos), options: EnumTipoTomador.obterOpcoes(), def: EnumTipoTomador.Todos, enable: ko.observable(true) });

    this.NaoEnviarTransportadorNaIntegracao = PropertyEntity({ text: "Não enviar o transportador na integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.NaoEnviarMotoristaNaIntegracao = PropertyEntity({ text: "Não enviar o motorista na integração", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VerificarVeiculoCompraPorTag = PropertyEntity({ text: "Verificar se veículo compra por Tag", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConsultarValorPedagioParaRota = PropertyEntity({ text: "Consultar valor do pedágio para a rota", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.CadastrarTransportadorAntesDaCompra = PropertyEntity({ text: "Cadastrar Transportador antes da compra", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastrarDocumentoTransportadorAntesDaCompra = PropertyEntity({ text: "Cadastrar Documento Transportador antes da compra", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastrarMotoristaAntesDaCompra = PropertyEntity({ text: "Cadastrar Motorista antes da compra", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CadastrarVeiculoAntesDaCompra = PropertyEntity({ text: "Cadastrar Veículo antes da compra", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });

    this.VerificarVeiculoCompraPorTag.val.subscribe(function (novoValor) {
        _integracaoDBTrans.MeioPagamento.enable(true);

        if (novoValor) {
            _integracaoDBTrans.MeioPagamento.enable(false);
            LimparCampo(_integracaoDBTrans.MeioPagamento);
        }
    });
    this.TipoRota.val.subscribe(function (novoValor) {
        _integracaoDBTrans.TipoRotaFrete.visible(true);

        if (novoValor == EnumTipoRotaDBTrans.RotaFixa) {
            _integracaoDBTrans.TipoRotaFrete.visible(false);
            LimparCampo(_integracaoDBTrans.TipoRotaFrete);
        }
    });
};

//*******EVENTOS*******

function loadConfiguracaoDBTrans() {
    _integracaoDBTrans = new IntegracaoDBTrans();
    KoBindings(_integracaoDBTrans, "knockoutIntegracaoDBTrans");

    new BuscarClientes(_integracaoDBTrans.FornecedorValePedagio);

    if (_CONFIGURACAO_TMS.PermitirConsultaDeValoresPedagio) {
        _integracaoDBTrans.ConsultarValorPedagioParaRota.visible(true);
    }
}

//*******MÉTODOS*******

function limparCamposDBTrans() {
    LimparCampos(_integracaoDBTrans);
}