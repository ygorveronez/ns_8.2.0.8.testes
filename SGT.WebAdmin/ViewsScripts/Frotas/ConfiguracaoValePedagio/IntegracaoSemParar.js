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
/// <reference path="../../Enumeradores/EnumTipoRotaSemParar.js" />
/// <reference path="../../Enumeradores/EnumTipoConsultaRota.js" />
/// <reference path="../../Enumeradores/EnumTipoBuscarPracasNaGeracaoDaCarga.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoSemParar;

var IntegracaoSemParar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiasPrazo = PropertyEntity({ text: "Dias Prazo:", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 2 });
    this.TipoRota = PropertyEntity({ text: "*Tipo Rota:", val: ko.observable(EnumTipoRotaSemParar.RotaFixa), options: EnumTipoRotaSemParar.obterOpcoes(), def: EnumTipoRotaSemParar.RotaFixa, enable: ko.observable(true), required: ko.observable(true) });

    this.Usuario = PropertyEntity({ text: "*Usuário: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.Senha = PropertyEntity({ text: "*Senha: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.CNPJ = PropertyEntity({ text: "*CNPJ/Código: ", enable: ko.observable(true), required: true });
    this.UrlIntegracaoRest = PropertyEntity({ text: "*URL Rest: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.NomeRpt = PropertyEntity({ text: "Nome Arquivo Relatório: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao1 = PropertyEntity({ text: "Observação 1: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao2 = PropertyEntity({ text: "Observação 2: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao3 = PropertyEntity({ text: "Observação 3: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao4 = PropertyEntity({ text: "Observação 4: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao5 = PropertyEntity({ text: "Observação 5: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao6 = PropertyEntity({ text: "Observação 6: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });

    this.UtilizarModeoVeicularCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Utilizar modelo veicular da carga?", def: false });
    this.BuscarPracasNaGeracaoDaCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Buscar praças na geração da carga?", def: false, visible: ko.observable(false) });
    this.ComprarRetornoVazioSeparado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Comprar vale pedágio retorno separado?", def: false, visible: ko.observable(false) });
    this.NaoComprarValePedagioVeiculoSemTag = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Não comprar vale pedágio para veículos sem tag", def: false, visible: ko.observable(true) });
    this.ConsultarValorPedagioParaRota = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Consultar valor do pedágio para a rota", def: false, visible: ko.observable(false) });
    this.ConsultarEComprarPedagioFreteEmbarcador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Consultar e Comprar VP quando o frete é do tipo embarcador", def: false, visible: ko.observable(false) });
    this.ConsultarSeVeiculoPossuiCadastro = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Consultar se veículo possuí cadastro na Sem Parar", def: false, visible: ko.observable(true) });
    this.NotificarTransportadorPorEmail = PropertyEntity({ text: "Notificar transportador por e-mail", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio = PropertyEntity({ text: "Gerar registro mesmo se rota não possuir praça de pedágio", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.ComprarSomenteNoMesVigente = PropertyEntity({ text: "Comprar VP somente no mês vigente (Data Criação da Carga)", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.ConsultarExtrato = PropertyEntity({ text: "Consultar Extrato", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.QuantidadeDiasConsultarExtrato = PropertyEntity({ text: "Quantidade de dias para consultar os extratos:", getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 2 });

    this.TipoConsultaRota = PropertyEntity({ text: "Consulta da Rota/Pedágio:", val: ko.observable(EnumTipoConsultaRota.MaisRapida), options: EnumTipoConsultaRota.obterOpcoes(), def: EnumTipoConsultaRota.MaisRapida, enable: ko.observable(true), required: ko.observable(false) });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.TipoRota.val.subscribe(function () {
        changeTipoRotaSemParar();
    });

    this.TagNumeroCargaObservacao1 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao1.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao1 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao1.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao1 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao1.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TagNumeroCargaObservacao2 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao2.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao2 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao2.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao2 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao2.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TagNumeroCargaObservacao3 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao3.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao3 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao3.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao3 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao3.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TagNumeroCargaObservacao4 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao4.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao4 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao4.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao4 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao4.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TagNumeroCargaObservacao5 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao5.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao5 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao5.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao5 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao5.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TagNumeroCargaObservacao6 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao6.id, "#NumeroCarga"); }, type: types.event, text: "Número da Carga" });
    this.TagNomeTransportadoraObservacao6 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao6.id, "#NomeTransportadora"); }, type: types.event, text: "Nome da Transportadora" });
    this.TagTipoOperacaoObservacao6 = PropertyEntity({ eventClick: function (e) { InserirTag(_integracaoSemParar.Observacao6.id, "#TipoOperacao"); }, type: types.event, text: "Tipo de Operação" });

    this.TipoBuscarPracasNaGeracaoDaCarga = PropertyEntity({ text: "Tipo da Busca de Praças na Geração da Carga:", val: ko.observable(EnumTipoBuscarPracasNaGeracaoDaCarga.OrigemDestino), options: EnumTipoBuscarPracasNaGeracaoDaCarga.obterOpcoes(), def: EnumTipoBuscarPracasNaGeracaoDaCarga.OrigemDestino, enable: ko.observable(true), required: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoSemParar() {
    _integracaoSemParar = new IntegracaoSemParar();
    KoBindings(_integracaoSemParar, "knockoutIntegracaoSemParar");

    if (_CONFIGURACAO_TMS.PermitirConsultaDeValoresPedagio) {
        _integracaoSemParar.ConsultarValorPedagioParaRota.visible(true);
        _integracaoSemParar.ConsultarEComprarPedagioFreteEmbarcador.visible(true);
    }

    new BuscarClientes(_integracaoSemParar.FornecedorValePedagio);

    changeTipoRotaSemParar();
}

//*******MÉTODOS*******

function changeTipoRotaSemParar() {
    _integracaoSemParar.BuscarPracasNaGeracaoDaCarga.visible(_integracaoSemParar.TipoRota.val() == EnumTipoRotaSemParar.RotaTemporaria);
    _integracaoSemParar.ComprarRetornoVazioSeparado.visible(_integracaoSemParar.TipoRota.val() == EnumTipoRotaSemParar.RotaFixa);
    _integracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio.visible(_integracaoSemParar.TipoRota.val() == EnumTipoRotaSemParar.RotaTemporaria);

    if (_integracaoSemParar.TipoRota.val() == EnumTipoRotaSemParar.RotaFixa) {
        LimparCampo(_integracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio);
    }
}

function limparCamposIntegracaoSemParar() {
    LimparCampos(_integracaoSemParar);
}