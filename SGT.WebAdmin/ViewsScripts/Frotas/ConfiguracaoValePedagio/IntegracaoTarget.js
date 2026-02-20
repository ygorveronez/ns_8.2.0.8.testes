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
/// <reference path="../../Enumeradores/EnumTipoRotaTarget.js" />
/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoTarget;

var IntegracaoTarget = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiasPrazo = PropertyEntity({ text: "Dias Prazo:", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 2 });
    this.CodigoCentroCusto = PropertyEntity({ text: "Centro Custo:", getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 12 });

    this.Usuario = PropertyEntity({ text: "*Usuário: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.Senha = PropertyEntity({ text: "*Senha: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: true });
    this.Token = PropertyEntity({ text: "Token: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: false });
    this.CadastrarRotaPorIBGE = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Cadastrar rotas na target pelo IBGE?", def: false });
    this.CadastrarRotaPorCoordenadas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Cadastrar rotas na target por Latitude e Longitude?", def: false });
    this.NaoBuscarCartaoMotoristaTarget = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Não buscar cartão motorista da Target (utilizar apenas cadastrados no MultiEmbarcador)?", def: false });
    this.PreencherLatLongDaRotaIntegracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Preencher Latitude e Longitude da Rota Integração por completo.", def: false });
    this.PreencherPontosPassagemModificadoCliente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Preencher com pontos passagem modificados pelo cliente.", def: false });
    this.NotificarTransportadorPorEmail = PropertyEntity({ text: "Notificar transportador por e-mail", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });

    this.CadastrarRotaPorIBGE.val.subscribe(function (novoValor) {
        if (novoValor) {
            _integracaoTarget.CadastrarRotaPorCoordenadas.enable(false);
            _integracaoTarget.CadastrarRotaPorCoordenadas.val(false);
        }
        else {
            _integracaoTarget.CadastrarRotaPorCoordenadas.enable(true);
        }
    });
    this.CadastrarRotaPorCoordenadas.val.subscribe(function (novoValor) {
        if (novoValor) {
            _integracaoTarget.CadastrarRotaPorIBGE.enable(false);
            _integracaoTarget.CadastrarRotaPorIBGE.val(false);
        }
        else {
            _integracaoTarget.CadastrarRotaPorIBGE.enable(true);
        }
    });

    this.PreencherLatLongDaRotaIntegracao.val.subscribe(function (novoValor) {
        if (novoValor) {
            _integracaoTarget.PreencherPontosPassagemModificadoCliente.enable(false);
            _integracaoTarget.PreencherPontosPassagemModificadoCliente.val(false);
        }
        else {
            _integracaoTarget.PreencherPontosPassagemModificadoCliente.enable(true);
        }
    });

    this.PreencherPontosPassagemModificadoCliente.val.subscribe(function (novoValor) {
        if (novoValor) {
            _integracaoTarget.PreencherLatLongDaRotaIntegracao.enable(false);
            _integracaoTarget.PreencherLatLongDaRotaIntegracao.val(false);
        }
        else {
            _integracaoTarget.PreencherLatLongDaRotaIntegracao.enable(true);
        }
    });
}

//*******EVENTOS*******

function loadConfiguracaoTarget() {
    _integracaoTarget = new IntegracaoTarget();
    KoBindings(_integracaoTarget, "knockoutIntegracaoTarget");

    new BuscarClientes(_integracaoTarget.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposIntegracaoTarget() {
    LimparCampos(_integracaoTarget);
}