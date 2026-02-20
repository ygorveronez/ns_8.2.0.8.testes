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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Enumeradores/EnumModalidadeFrete.js" />
/// <reference path="../../Enumeradores/EnumEstado.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="NFe.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />

var RetiradaEntrega = function (nfe) {

    var instancia = this;

    this.UtilizarEnderecoRetirada = PropertyEntity({ text: "Informar outro endereço de retirada?", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.ClienteRetirada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente Retirada:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeRetirada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade Retirada:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.RetiradaLogradouro = PropertyEntity({ text: "Endereço Retirada: ", required: false, maxlength: 59 });
    this.RetiradaNumeroLogradouro = PropertyEntity({ text: "Número Retirada: ", required: false, maxlength: 10 });
    this.RetiradaComplementoLogradouro = PropertyEntity({ text: "Complemento Retirada: ", required: false, maxlength: 20 });
    this.RetiradaBairro = PropertyEntity({ text: "Bairro Retirada: ", required: false, maxlength: 100 });
    this.RetiradaCEP = PropertyEntity({ text: "CEP Retirada: ", required: false, maxlength: 16, getType: typesKnockout.cep });
    this.RetiradaTelefone = PropertyEntity({ text: "Telefone Retirada: ", required: false, maxlength: 59, getType: typesKnockout.phone });
    this.RetiradaEmail = PropertyEntity({ text: "E-mail Retirada: ", required: false, maxlength: 200, getType: typesKnockout.multiplesEmails });
    this.RetiradaIE = PropertyEntity({ text: "I.E. Retirada: ", required: false, maxlength: 59 });

    this.UtilizarEnderecoEntrega = PropertyEntity({ text: "Informar outro endereço de entrega?", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.ClienteEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente Entrega:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade Entrega:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EntregaLogradouro = PropertyEntity({ text: "Endereço Entrega: ", required: false, maxlength: 59 });
    this.EntregaNumeroLogradouro = PropertyEntity({ text: "Número Entrega: ", required: false, maxlength: 10 });
    this.EntregaComplementoLogradouro = PropertyEntity({ text: "Complemento Entrega: ", required: false, maxlength: 20 });
    this.EntregaBairro = PropertyEntity({ text: "Bairro Entrega: ", required: false, maxlength: 100 });
    this.EntregaCEP = PropertyEntity({ text: "CEP Entrega: ", required: false, maxlength: 16, getType: typesKnockout.cep });
    this.EntregaTelefone = PropertyEntity({ text: "Telefone Entrega: ", required: false, maxlength: 59, getType: typesKnockout.phone });
    this.EntregaEmail = PropertyEntity({ text: "E-mail Entrega: ", required: false, maxlength: 200, getType: typesKnockout.multiplesEmails });
    this.EntregaIE = PropertyEntity({ text: "I.E. Entrega: ", required: false, maxlength: 59 });


    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutRetiradaEntrega);

        new BuscarClientes(instancia.ClienteRetirada, function (data) {
            instancia.ClienteRetirada.codEntity(data.Codigo);
            instancia.ClienteRetirada.val(data.Nome);
            instancia.LocalidadeRetirada.codEntity(data.CodigoLocalidade);
            instancia.LocalidadeRetirada.val(data.Localidade);
            instancia.RetiradaLogradouro.val(data.Endereco);
            instancia.RetiradaNumeroLogradouro.val(data.Numero);
            instancia.RetiradaComplementoLogradouro.val(data.Complemento);
            instancia.RetiradaBairro.val(data.Bairro);
            instancia.RetiradaCEP.val(data.CEP);
            instancia.RetiradaTelefone.val(data.Telefone1);
            instancia.RetiradaEmail.val(data.Email);
            instancia.RetiradaIE.val(data.IE_RG);
        }, true);
        new BuscarLocalidades(instancia.LocalidadeRetirada);

        new BuscarClientes(instancia.ClienteEntrega, function (data) {
            instancia.ClienteEntrega.codEntity(data.Codigo);
            instancia.ClienteEntrega.val(data.Nome);
            instancia.LocalidadeEntrega.codEntity(data.CodigoLocalidade);
            instancia.LocalidadeEntrega.val(data.Localidade);
            instancia.EntregaLogradouro.val(data.Endereco);
            instancia.EntregaNumeroLogradouro.val(data.Numero);
            instancia.EntregaComplementoLogradouro.val(data.Complemento);
            instancia.EntregaBairro.val(data.Bairro);
            instancia.EntregaCEP.val(data.CEP);
            instancia.EntregaTelefone.val(data.Telefone1);
            instancia.EntregaEmail.val(data.Email);
            instancia.EntregaIE.val(data.IE_RG);
        }, true);
        new BuscarLocalidades(instancia.LocalidadeEntrega);
    };

    this.DestivarRetiradaEntrega = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };
};