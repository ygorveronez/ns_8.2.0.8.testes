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

var Transporte = function (nfe) {

    var instancia = this;

    this.CNPJTransportadora = PropertyEntity({ text: ko.observable("CNPJ/CPF da Transportadora: "), required: false, getType: typesKnockout.cpfCnpj, visible: ko.observable(true) });
    this.NomeTransportadora = PropertyEntity({ text: "Nome Transportadora: ", required: false, maxlength: 59 });
    this.IETransportadora = PropertyEntity({ text: "IE Transportadora: ", required: false, maxlength: 14 });
    this.EnderecoTransportadora = PropertyEntity({ text: "Endereço Transportadora: ", required: false, maxlength: 59 });
    this.EstadoTransportadora = PropertyEntity({ val: ko.observable(""), options: EnumEstado.obterOpcoesCadastro(), def: "", text: "Estado Transportadora: " });
    this.EmailTransportadora = PropertyEntity({ text: "E-mails Transportadora: ", required: false, maxlength: 500, getType: typesKnockout.email });

    this.PlacaVeiculo = PropertyEntity({ text: "Placa Veículo: ", required: false, maxlength: 7 });

    this.EstadoVeiculo = PropertyEntity({ val: ko.observable(""), options: EnumEstado.obterOpcoesCadastro(), def: "", text: "Estado Veículo: " });
    this.ANTTVeiculo = PropertyEntity({ text: "ANTT Veículo: ", required: false, maxlength: 20 });

    this.Quantidade = PropertyEntity({ text: "Quantidade: ", required: false, maxlength: 15, getType: typesKnockout.int, configDecimal: { precision: 0, allowZero: true } });
    this.Especie = PropertyEntity({ text: "Espécie: ", required: false, maxlength: 60 });
    this.Marca = PropertyEntity({ text: "Marca: ", required: false, maxlength: 60 });
    this.Volume = PropertyEntity({ text: "Número de Volumes: ", required: false, maxlength: 4, getType: typesKnockout.int, configDecimal: { precision: 0, allowZero: true } });
    this.PesoBruto = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Peso Bruto (KG):", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });
    this.PesoLiquido = PropertyEntity({ def: "0,00", val: ko.observable("0,00"), text: "Peso Líquido (KG):", getType: typesKnockout.decimal, maxlength: 20, required: false, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: true } });

    this.TipoFrete = PropertyEntity({ val: ko.observable(EnumModalidadeFrete.SemFrete), def: EnumModalidadeFrete.SemFrete, options: EnumModalidadeFrete.obterOpcoes(), text: "*Tipo Frete:", required: true, enable: ko.observable(true) });

    this.Transportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportadora:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.CidadeTransportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Transportadora:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo Transportadora:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Load = function () {
        KoBindings(instancia, nfe.IdKnockoutTransporte);
        $("#" + instancia.PlacaVeiculo.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

        new BuscarClientes(instancia.Transportadora, function (data) {
            instancia.Transportadora.codEntity(data.Codigo);
            instancia.Transportadora.val(data.Nome);
            data.CPF_CNPJ = data.CPF_CNPJ.replace(/[^0-9]+/g, '');
            instancia.CNPJTransportadora.val(data.CPF_CNPJ);
            instancia.NomeTransportadora.val(data.Nome);
            instancia.IETransportadora.val(data.IE_RG);
            instancia.EnderecoTransportadora.val(data.Endereco);
            instancia.EstadoTransportadora.val(data.Estado);
            instancia.CidadeTransportadora.codEntity(data.CodigoLocalidade);
            instancia.CidadeTransportadora.val(data.Localidade);
            instancia.EmailTransportadora.val(data.Email);
        }, true);
        new BuscarLocalidadesBrasil(instancia.CidadeTransportadora);
        new BuscarVeiculos(instancia.Veiculo, function (data) {
            instancia.Veiculo.codEntity(data.Codigo);
            instancia.Veiculo.val(data.Placa);
            instancia.PlacaVeiculo.val(data.Placa);
            instancia.EstadoVeiculo.val(data.Estado);
            if (data.RNTRC > 0)
                instancia.ANTTVeiculo.val(data.RNTRC);
            else
                instancia.ANTTVeiculo.val("");
        });
    };

    this.DestivarTransporte = function () {
        DesabilitarCamposInstanciasNFe(instancia);
    };
};

var ValidarCPFCNPJ = function (instancia) {
    var cpfCnpj = instancia.CNPJTransportadora.val();
    if (cpfCnpj.length == 11) {
        if (!ValidarCPF(cpfCnpj)) {
            exibirAlertaNotificacao("CPF informado é inválido");
            $("#" + instancia.CNPJTransportadora.id).focus();
        }
    } else if (cpfCnpj.length == 14) {
        if (!ValidarCNPJ(cpfCnpj)) {
            exibirAlertaNotificacao("CNPJ informado é inválido");
            $("#" + instancia.CNPJTransportadora.id).focus();
        }
    } else if (cpfCnpj.length > 0) {
        exibirAlertaNotificacao("CNPJ/CPF informado é inválido");
        $("#" + instancia.CNPJTransportadora.id).focus();
    }
};