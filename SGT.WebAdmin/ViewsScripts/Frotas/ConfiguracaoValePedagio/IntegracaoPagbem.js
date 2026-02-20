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

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoPagbem;

var IntegracaoPagbem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URLPagbem = PropertyEntity({ text: "URL:", maxlength: 500 });
    this.UsuarioPagbem = PropertyEntity({ text: "Usuário:", maxlength: 100 });
    this.SenhaPagbem = PropertyEntity({ text: "Senha:", maxlength: 100 });
    this.CNPJEmpresaContratante = PropertyEntity({ text: "CNPJ Contratante:", maxlength: 14, getType: typesKnockout.string });    
    this.IntegrarNumeroRPSNFSE = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Integrar número do RPS da NFS-e??", def: false });
    this.LiberarViagemManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Liberar viagem manualmente?", def: false });
    this.ConsultarVeiculoSemParar = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: "Consular se o veículo está ativo na Sem Parar?", def: false });    
    this.QuantidadeEixosPadraoValePedagio = PropertyEntity({ text: "Qtd Eixo padrão para Vale Pedágio:", maxlength: 4, getType: typesKnockout.int });    

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadConfiguracaoPagbem() {
    _integracaoPagbem = new IntegracaoPagbem();
    KoBindings(_integracaoPagbem, "knockoutIntegracaoPagbem");

    new BuscarClientes(_integracaoPagbem.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposIntegracaoPagbemt() {
    LimparCampos(_integracaoPagbem);
}