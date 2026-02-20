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
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Enumeradores/EnumTipoPropriedade.js" />
/// <reference path="../../Enumeradores/EnumTipoCarroceria.js" />
/// <reference path="../../Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../Enumeradores/EnumCategoriaHabilitacao.js" />
/// <reference path="../../Enumeradores/EnumTipoRodadoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumCondicaoLicenca.js" />
/// <reference path="RegraPlanejamentoFrota.js" />
/// <reference path="../../Enumeradores/EnumDiaSemanaMesAno.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _regraRegraPlanejamentoFrota;

var RegraRegraPlanejamentoFrota = function () {
    this.TipoDePropriedade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Tipo da Proriedade: ", options: ko.observable(EnumTipoPropriedade.obterOpcoes()) });
    this.TipoDeCarroceria = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Tipo da Carroceria: ", options: ko.observable(EnumTipoCarroceria.obterOpcoes()) });
    this.TipoDeProprietario = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Tipo do Proprietário: ", options: ko.observable(EnumTipoProprietarioVeiculo.obterOpcoes()) });
    this.CategoriaDaCNH = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Tipo da CNH: ", options: ko.observable(EnumCategoriaHabilitacao.obterOpcoes()) });
    this.TipoDaTracao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Tipo da Tração: ", options: ko.observable(EnumTipoRodadoVeiculo.obterOpcoes()) });
    this.CondicaoParaLicencas = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Condição para Licenças: ", options: ko.observable(EnumCondicaoLicenca.obterOpcoes()) });
    this.CondicaoParaLiberacoesGR = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.selectMultiple, text: "Condição para Liberações de GR: ", options: ko.observable(EnumCondicaoLiberacaoGR.obterOpcoes()) });

    this.QuantidadeCarga = PropertyEntity({ text: "Quantidade Mínima de Carga: ", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(true), visible: ko.observable(true), val: ko.observable("") });
    this.ValidarQuantidadeVeiculoEReboque = PropertyEntity({ text: "Validar quantidade mínima para Veiculo e Reboque", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PeriodoQuantidadeCarga = PropertyEntity({ text: "Periodo (Tempo): ", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(true), visible: ko.observable(true), val: ko.observable("") });
    this.TipoPeriodoQuantidadeCarga = PropertyEntity({ val: ko.observable(EnumDiaSemanaMesAno.Dia), options: EnumDiaSemanaMesAno.obterOpcoes(), def: EnumDiaSemanaMesAno.Dia, text: ko.observable("Periodo (Tipo): ") });

    this.ApenasVeiculosComRastreadorAtivo = PropertyEntity({ text: "Apenas Veículos com Rastreador ativo", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ApenasVeiculoQuePossuiTravaQuintaRoda = PropertyEntity({ text: "Apenas com Veículo que possui Trava de Quinta Roda", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ApenasVeiculoQuePossuiImobilizador = PropertyEntity({ text: "Apenas com Veículo que possui o Imobilizador", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.ValidarPorQuantidadeMotorista = PropertyEntity({ text: "Validar quantidade de viagens apenas para motoristas", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.ApenasTracaoComIdadeMaxima = PropertyEntity({ text: "Apenas com Tração com uma Idade Máxima", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ApenasReboqueComIdadeMaxima = PropertyEntity({ text: "Apenas com Reboque com uma Idade Máxima", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.LimitarPelaAlturaCarreta = PropertyEntity({ text: "Limitar pela Altura da Carreta", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ApenasComInformacoesDeIscaInformadaNoPedido = PropertyEntity({ text: "Apenas com as informações de Isca informada no Pedido", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ApenasComInformacoesDeEscoltaInformadaNoPedido = PropertyEntity({ text: "Apenas com as informações de Escolta informada no Pedido", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.LimitarPelaAlturaCavalo = PropertyEntity({ text: "Limitar pela Altura do Cavalo", val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.IdadeMaximaTracao = PropertyEntity({ text: "Idade:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.IdadeMaximaReboque = PropertyEntity({ text: "Idade:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.QuantidadeIsca = PropertyEntity({ text: "Quantidade de Isca:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.QuantidadeEscolta = PropertyEntity({ text: "Quantidade de Escolta:", getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.MetrosAlturaCarreta = PropertyEntity({ text: "Metros:", getType: typesKnockout.decimal, maxlength: 9, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });
    this.MetrosAlturaCavalo = PropertyEntity({ text: "Metros:", getType: typesKnockout.decimal, maxlength: 9, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("") });

    this.ApenasTracaoComIdadeMaxima.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.IdadeMaximaTracao.enable(novoValor);
        _regraRegraPlanejamentoFrota.IdadeMaximaTracao.visible(novoValor);
        _regraRegraPlanejamentoFrota.IdadeMaximaTracao.val("");
    });

    this.ApenasReboqueComIdadeMaxima.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.IdadeMaximaReboque.enable(novoValor);
        _regraRegraPlanejamentoFrota.IdadeMaximaReboque.visible(novoValor);
        _regraRegraPlanejamentoFrota.IdadeMaximaReboque.val("");
    });

    this.ApenasComInformacoesDeIscaInformadaNoPedido.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.QuantidadeIsca.enable(novoValor);
        _regraRegraPlanejamentoFrota.QuantidadeIsca.visible(novoValor);
        _regraRegraPlanejamentoFrota.QuantidadeIsca.val("");
    });

    this.ApenasComInformacoesDeEscoltaInformadaNoPedido.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.QuantidadeEscolta.enable(novoValor);
        _regraRegraPlanejamentoFrota.QuantidadeEscolta.visible(novoValor);
        _regraRegraPlanejamentoFrota.QuantidadeEscolta.val("");
    });

    this.LimitarPelaAlturaCarreta.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.MetrosAlturaCarreta.enable(novoValor);
        _regraRegraPlanejamentoFrota.MetrosAlturaCarreta.visible(novoValor);
        _regraRegraPlanejamentoFrota.MetrosAlturaCarreta.val("");
    });

    this.LimitarPelaAlturaCavalo.val.subscribe(function (novoValor) {
        _regraRegraPlanejamentoFrota.MetrosAlturaCavalo.enable(novoValor);
        _regraRegraPlanejamentoFrota.MetrosAlturaCavalo.visible(novoValor);
        _regraRegraPlanejamentoFrota.MetrosAlturaCavalo.val("");
    });
};

//*******EVENTOS*******

function loadRegraRegraPlanejamentoFrota() {
    _regraRegraPlanejamentoFrota = new RegraRegraPlanejamentoFrota();
    KoBindings(_regraRegraPlanejamentoFrota, "knockoutCadastroRegra");

    //HeaderAuditoria("RegraPlanejamentoFrota", _regraPlanejamentoFrota);
}