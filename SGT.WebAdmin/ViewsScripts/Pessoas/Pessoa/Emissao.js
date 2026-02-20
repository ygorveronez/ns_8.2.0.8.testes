/// <reference path="../../Enumeradores/EnumTipoPessoa.js" />
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
/// <reference path="TransportadorTerceiro.js" />
/// <reference path="Fornecedor.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/RateioFormula.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />

//var _emissao;

//var EmissaoMap = function () {
//    this.RateioFormula = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Fórmula do Rateio do Frete:", idBtnSearch: guid(), enable: ko.observable(true) });
//    this.TipoEmissaoCTeDocumentos = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada), options: _TiposEmissaoCTeCliente, text: "Tipo do Rateio dos Documentos: ", def: EnumTipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada });
//}


//function loadEmissao() {
//    _emissao = new EmissaoMap();
//    KoBindings(_emissao, "knockoutEmissao");
//    new BuscarRateioFormulas(_emissao.RateioFormula);
//}

//function LimparEmissao() {
//    LimparCampos(_emissao);
//}

//function setarEmissao() {
//    _emissao.RateioFormula.val(_pessoa.RateioFormula.val());
//    _emissao.RateioFormula.codEntity(_pessoa.RateioFormula.codEntity());
//    _emissao.TipoEmissaoCTeDocumentos.val(_pessoa.TipoEmissaoCTeDocumentos.val());
//}

//function setarPessoaEmissao() {
//    _pessoa.RateioFormula.val(_emissao.RateioFormula.val());
//    _pessoa.RateioFormula.codEntity(_emissao.RateioFormula.codEntity());
//    _pessoa.TipoEmissaoCTeDocumentos.val(_emissao.TipoEmissaoCTeDocumentos.val());
//}