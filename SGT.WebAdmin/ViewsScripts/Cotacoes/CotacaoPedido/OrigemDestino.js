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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ClienteOutroEndereco.js" />
/// <reference path="CotacaoPedido.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _localidadeOrigem;
var _localidadeDestino;

var LocalidadeCliente = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), text: "Origem:", idBtnSearch: guid() });
    this.ClienteOutroEndereco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Outra Localidade:", enable: ko.observable(true), fadeVisible: ko.observable(true), eventChange: clienteOutroEnderecoBlur, idBtnSearch: guid() });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Localidade:", enable: ko.observable(true), required: true, idBtnSearch: guid(), issue: 16 });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pais:", enable: true });
    this.CodigoIBGE = PropertyEntity({ text: "Codigo IBGE: ", enable: ko.observable(true) });
    this.UF = PropertyEntity({ text: "UF: ", enable: ko.observable(true) });
    this.LocalidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade Polo:", enable: ko.observable(true), issue: 16 });
    this.Bairro = PropertyEntity({ text: "*Bairro: ", enable: ko.observable(true), required: true, maxlength: 40 });
    this.CEP = PropertyEntity({ text: "CEP: ", enable: ko.observable(true), maxlength: 9 });
    this.Numero = PropertyEntity({ text: "*Numero: ", required: ko.observable(true), enable: ko.observable(true), maxlength: 60 });
    this.Complemento = PropertyEntity({ text: "Complemento: ", enable: ko.observable(true), maxlength: 60 });
    this.IERG = PropertyEntity({ text: "IE/RG: ", required: false, enable: ko.observable(true), maxlength: 20 });
    this.Endereco = PropertyEntity({ text: "*Endereço: ", required: true, enable: ko.observable(true), maxlength: 80 });
    this.Telefone1 = PropertyEntity({ text: "Telefone: ", enable: ko.observable(true), maxlength: 14 });
    this.RGIE = PropertyEntity({ text: "I.E.: ", enable: ko.observable(true), maxlength: 20 });

    this.MudarEndereco = PropertyEntity({ text: "Mudar o Endereço? ", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.DescricaoLocalidade = PropertyEntity({ type: types.local });
    this.DescricaoEndereco = PropertyEntity({ type: types.local });
    this.DescricaoCidadePolo = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.DescricaoTelefone = PropertyEntity({ type: types.local, visible: ko.observable(false) });

}

//*******EVENTOS*******

function loadOrigemDestino() {
    _localidadeOrigem = new LocalidadeCliente();
    KoBindings(_localidadeOrigem, "knockoutOrigem");
    new BuscarLocalidades(_localidadeOrigem.Localidade, null, null, function (retorno) {
        _localidadeOrigem.Localidade.val(retorno.Descricao);
        _localidadeOrigem.Localidade.codEntity(retorno.Codigo);
        _localidadeOrigem.LocalidadePolo.val(retorno.DescricaoCidadePolo);
    });

    _localidadeDestino = new LocalidadeCliente();
    KoBindings(_localidadeDestino, "knockoutDestino");
    new BuscarLocalidades(_localidadeDestino.Localidade, null, null, function (retorno) {
        _localidadeDestino.Localidade.val(retorno.Descricao);
        _localidadeDestino.Localidade.codEntity(retorno.Codigo);
        _localidadeDestino.LocalidadePolo.val(retorno.DescricaoCidadePolo);
    });

    new BuscarClienteOutroEndereco(_localidadeOrigem.ClienteOutroEndereco, retornoOutroEnderecoClienteOrigem);
    new BuscarClienteOutroEndereco(_localidadeDestino.ClienteOutroEndereco, retornoOutroEnderecoClienteDestino, _cotacaoPedido.Destinatario);

    $("#" + _localidadeOrigem.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _localidadeDestino.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });

}

function clienteOutroEnderecoBlur(e) {
    if (e.ClienteOutroEndereco.val() === "") {
        e.ClienteOutroEndereco.fadeVisible(true);
        e.ClienteOutroEndereco.codEntity(0);
    } else {
        e.ClienteOutroEndereco.fadeVisible(false);
    }
}

//*******MÉTODOS*******

function buscarKmTotalPorOrigemEDestino() {
    if ((_cotacaoPedido.Origem.codEntity() > 0) && (_cotacaoPedido.Destino.codEntity() > 0)) {
        executarReST("CotacaoPedido/ObterDadosRotaFrete", { Origem: _cotacaoPedido.Origem.codEntity(), Destino: _cotacaoPedido.Destino.codEntity() }, function (retorno) {
            if (retorno.Success) {
                
                if (retorno.Data) {
                    if (!!retorno.Data.Quilometros === true)
                        _cotacaoPedidoAdicional.KMTotal.val(retorno.Data.Quilometros);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function retornoOutroEnderecoClienteOrigem(data) {
    _localidadeOrigem.ClienteOutroEndereco.val(data.Descricao);
    _localidadeOrigem.ClienteOutroEndereco.codEntity(data.Codigo);
    _localidadeOrigem.ClienteOutroEndereco.fadeVisible(false);
}

function retornoOutroEnderecoClienteDestino(data) {
    _localidadeDestino.ClienteOutroEndereco.val(data.Descricao);
    _localidadeDestino.ClienteOutroEndereco.codEntity(data.Codigo);
    _localidadeDestino.ClienteOutroEndereco.fadeVisible(false);
}

function preecherLocalidadeCliente(knoutCliente, knouLocalidade, tipoCliente) {
    var data = { Codigo: knoutCliente.codEntity() };
    executarReST("Cliente/BuscarLocalidadeCliente", data, function (arg) {
        if (arg.Success) {
            var localidade = arg.Data;
            knouLocalidade.Cliente.visible(true);
            knouLocalidade.Cliente.codEntity(knoutCliente.codEntity());
            knouLocalidade.Cliente.val(knoutCliente.val());

            if (localidade.Localidade.LocalidadePolo !== null) {
                knouLocalidade.DescricaoCidadePolo.val("Cidade Polo:" + localidade.Localidade.LocalidadePolo.DescricaoCidadeEstado);
                knouLocalidade.DescricaoCidadePolo.visible(true);
            } else {
                knouLocalidade.DescricaoCidadePolo.visible(false);
            }
            knouLocalidade.CodigoIBGE.val(localidade.Localidade.CodigoIBGE);
            knouLocalidade.UF.val(localidade.Localidade.Estado.Sigla);

            var endereco = localidade.Endereco;
            if (localidade.Numero !== "")
                endereco += ", " + localidade.Numero;
            if (localidade.Bairro !== "")
                endereco += " - " + localidade.Bairro;
            if (localidade.Complemento !== "" && localidade.Complemento !== null)
                endereco += " (" + localidade.Complemento + ")";
            knouLocalidade.DescricaoEndereco.val(endereco);

            var local = localidade.Localidade.DescricaoCidadeEstado;
            if (localidade.Localidade.Pais !== null)
                local += ", " + localidade.Localidade.Pais.Nome;

            if (localidade.CEP !== "")
                local += " CEP: " + localidade.CEP;

            knouLocalidade.DescricaoLocalidade.val(local);

            if (localidade.Telefone1 !== "") {
                knouLocalidade.DescricaoTelefone.visible(true);
                knouLocalidade.DescricaoTelefone.val(localidade.Telefone1);
            } else {
                knouLocalidade.DescricaoTelefone.visible(false);
            }

            if (tipoCliente !== null) {
                if (tipoCliente === "R") {
                    _cotacaoPedido.Origem.val(localidade.Localidade.Descricao);
                    _cotacaoPedido.Origem.entityDescription(localidade.Localidade.Descricao);
                    _cotacaoPedido.Origem.codEntity(localidade.Localidade.Codigo);

                    buscarKmTotalPorOrigemEDestino();
                }
                else if (tipoCliente === "D") {
                    _cotacaoPedido.Destino.val(localidade.Localidade.Descricao);
                    _cotacaoPedido.Destino.entityDescription(localidade.Localidade.Descricao);
                    _cotacaoPedido.Destino.codEntity(localidade.Localidade.Codigo);

                    buscarKmTotalPorOrigemEDestino();
                }
            }

        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function preecherDescricaoEnderecoOrigemUtilizado(data) {

    if (data.CidadePoloOrigem !== "") {
        _localidadeOrigem.DescricaoCidadePolo.val("Cidade Polo:" + data.CidadePoloOrigem);
        _localidadeOrigem.DescricaoCidadePolo.visible(true);
    } else {
        _localidadeOrigem.DescricaoCidadePolo.visible(false);
    }

    var endereco = data.Endereco;
    if (data.Numero !== "")
        endereco += ", " + data.Numero;
    if (data.Bairro !== "")
        endereco += " - " + data.Bairro;
    if (data.Complemento !== "")
        endereco += " (" + data.Complemento + ")";
    _localidadeOrigem.DescricaoEndereco.val(endereco);

    var local = data.Origem;
    if (data.PaisOrigem !== "")
        local += ", " + data.PaisOrigem;

    if (data.CEP !== "")
        local += " CEP: " + data.CEP;

    _localidadeOrigem.DescricaoLocalidade.val(local);

    if (data.Telefone1 !== "") {
        _localidadeOrigem.DescricaoTelefone.visible(true);
        _localidadeOrigem.DescricaoTelefone.val(data.Telefone);
    } else {
        _localidadeOrigem.DescricaoTelefone.visible(false);
    }
}

function preecherDescricaoEnderecoDestinoUtilizado(data) {

    if (data.CidadePoloDestino !== "") {
        _localidadeDestino.DescricaoCidadePolo.val("Cidade Polo:" + data.CidadePoloDestino);
        _localidadeDestino.DescricaoCidadePolo.visible(true);
    } else {
        _localidadeDestino.DescricaoCidadePolo.visible(false);
    }

    var endereco = data.Endereco;

    if (data.Numero !== "")
        endereco += ", " + data.Numero;
    if (data.Bairro !== "")
        endereco += " - " + data.Bairro;
    if (data.Complemento !== "")
        endereco += " (" + data.Complemento + ")";
    _localidadeDestino.DescricaoEndereco.val(endereco);

    var local = data.Destino;
    if (data.PaisDestino !== "")
        local += ", " + data.PaisDestino;

    if (data.CEP !== "")
        local += " CEP: " + data.CEP;

    _localidadeDestino.DescricaoLocalidade.val(local);

    if (data.Telefone1 !== "") {
        _localidadeDestino.DescricaoTelefone.visible(true);
        _localidadeDestino.DescricaoTelefone.val(data.Telefone1);
    } else {
        _localidadeDestino.DescricaoTelefone.visible(false);
    }
}

function limparCamposOrigemDestino() {
    LimparCampos(_localidadeDestino);
    _localidadeDestino.Cliente.visible(false);
    LimparCampos(_localidadeOrigem);
    _localidadeOrigem.Cliente.visible(false);
}
