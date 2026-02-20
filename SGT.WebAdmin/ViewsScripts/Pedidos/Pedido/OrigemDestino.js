/// <reference path="PercursoMDFe.js" />
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
/// <reference path="Pedido.js" />
/// <reference path="SubContratacao.js" />



//*******MAPEAMENTO KNOUCKOUT*******


var _localidadeOrigem;
var _localidadeDestino;


var LocalidadeCliente = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), idBtnSearch: guid() });
    this.ClienteOutroEndereco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.OutraLocalidadeCliente.getFieldDescription(), enable: true, fadeVisible: ko.observable(true), eventChange: clienteOutroEnderecoBlur, idBtnSearch: guid() });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Localidade.getRequiredFieldDescription(), enable: true, required: true, idBtnSearch: guid(), issue: 16 });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Pais.getFieldDescription(), enable: true });
    this.CodigoIBGE = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CodigoIBGE.getFieldDescription(), enable: ko.observable(true) });
    this.UF = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.UF.getFieldDescription(), enable: ko.observable(true) });
    this.LocalidadePolo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.LocalidadePolo.getFieldDescription(), enable: ko.observable(true), issue: 16 });
    this.Bairro = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Bairro.getRequiredFieldDescription(), enable: ko.observable(true), required: true, maxlength: 40 });
    this.CEP = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.CEP.getFieldDescription(), enable: ko.observable(true), maxlength: 9 });
    this.Numero = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getRequiredFieldDescription(), required: ko.observable(true), enable: true, maxlength: 60 });
    this.Complemento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Complemento.getFieldDescription(), enable: ko.observable(true), maxlength: 60 });
    this.IERG = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IERG.getFieldDescription(), required: false, enable: ko.observable(true), maxlength: 20 });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Endereco.getRequiredFieldDescription(), required: true, enable: ko.observable(true), maxlength: 80 });
    this.Telefone1 = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.Telefone.getFieldDescription(), enable: ko.observable(true), maxlength: 14 });
    this.RGIE = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.IE.getFieldDescription(), enable: ko.observable(true), maxlength: 20 });

    this.MudarEndereco = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.MudarEndereco, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(false) });

    this.DescricaoLocalidade = PropertyEntity({ type: types.local });
    this.DescricaoEndereco = PropertyEntity({ type: types.local });
    this.DescricaoCidadePolo = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.DescricaoTelefone = PropertyEntity({ type: types.local, visible: ko.observable(false) });

    this.DetalheEnderecoOutroEndereco = PropertyEntity({ type: types.maps });
    this.DetalheLocalidadeOutroEndereco = PropertyEntity({ type: types.maps });
    this.DetalheCidadePoloOutroEndereco = PropertyEntity({ type: types.maps, visible: ko.observable(false) });
    this.DetalheTelefoneOutroEndereco = PropertyEntity({ type: types.maps, visible: ko.observable(false) });
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

    new BuscarClienteOutroEndereco(_localidadeOrigem.ClienteOutroEndereco, retornoOutroEnderecoClienteOrigem, _pedido.Remetente);
    new BuscarClienteOutroEndereco(_localidadeDestino.ClienteOutroEndereco, retornoOutroEnderecoClienteDestino, _pedido.Destinatario);

    $("#" + _localidadeOrigem.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _localidadeDestino.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });

}

function clienteOutroEnderecoBlur(e) {
    if (e.ClienteOutroEndereco.val() == "") {
        e.ClienteOutroEndereco.fadeVisible(true);
        e.ClienteOutroEndereco.codEntity(0);
    } else {
        e.ClienteOutroEndereco.fadeVisible(false);
    }
}

//*******MÉTODOS*******

function retornoOutroEnderecoClienteOrigem(data) {
    _localidadeOrigem.ClienteOutroEndereco.val(data.Descricao);
    _localidadeOrigem.ClienteOutroEndereco.codEntity(data.Codigo);
    _localidadeOrigem.ClienteOutroEndereco.fadeVisible(false);

    preencherDetalhesOutroEndereco(_localidadeOrigem, data);
}

function retornoOutroEnderecoClienteDestino(data) {
    _localidadeDestino.ClienteOutroEndereco.val(data.Descricao);
    _localidadeDestino.ClienteOutroEndereco.codEntity(data.Codigo);
    _localidadeDestino.ClienteOutroEndereco.fadeVisible(false);

    preencherDetalhesOutroEndereco(_localidadeDestino, data);
}

function preencherDetalhesOutroEndereco(knoutLocalidade, data) {
    knoutLocalidade.DetalheTelefoneOutroEndereco.visible(false);
    knoutLocalidade.DetalheCidadePoloOutroEndereco.visible(false);

    if (data.LocalidadePolo) {
        knoutLocalidade.DetalheCidadePoloOutroEndereco.val(data.LocalidadePolo);
        knoutLocalidade.DetalheCidadePoloOutroEndereco.visible(true);
    }

    knoutLocalidade.CodigoIBGE.val(data.CodigoIBGE);
    knoutLocalidade.UF.val(data.Estado);

    var endereco = data.Endereco;
    if (data.Numero != "") endereco += ", " + data.Numero;
    if (data.Bairro != "") endereco += " - " + data.Bairro;
    if (data.Complemento != "" && data.Complemento != null) endereco += " (" + data.Complemento + ")";
    knoutLocalidade.DetalheEnderecoOutroEndereco.val(endereco);

    var local = data.Descricao;
    if (data.Pais != null) local += ", " + data.Pais;
    if (data.CEP != "") local += " CEP: " + data.CEP;

    knoutLocalidade.DetalheLocalidadeOutroEndereco.val(local);

    if (data.Telefone != "") {
        knoutLocalidade.DetalheTelefoneOutroEndereco.visible(true);
        knoutLocalidade.DetalheTelefoneOutroEndereco.val(data.Telefone);
    }
}

function preecherLocalidadeCliente(knoutCliente, knouLocalidade, tipoCliente) {
    var data = { Codigo: knoutCliente.codEntity() };
    executarReST("Cliente/BuscarLocalidadeCliente", data, function (arg) {
        if (arg.Success) {
            var localidade = arg.Data;
            knouLocalidade.Cliente.visible(true);
            knouLocalidade.Cliente.codEntity(knoutCliente.codEntity());
            knouLocalidade.Cliente.val(knoutCliente.val());

            if (localidade.Localidade.LocalidadePolo != null) {
                knouLocalidade.DescricaoCidadePolo.val(localidade.Localidade.LocalidadePolo.DescricaoCidadeEstado);
                knouLocalidade.DescricaoCidadePolo.visible(true);
            } else {
                knouLocalidade.DescricaoCidadePolo.visible(false);
            }
            knouLocalidade.CodigoIBGE.val(localidade.Localidade.CodigoIBGE);
            knouLocalidade.UF.val(localidade.Localidade.Estado.Sigla);

            var endereco = localidade.Endereco;
            if (localidade.Numero != "")
                endereco += ", " + localidade.Numero;
            if (localidade.Bairro != "")
                endereco += " - " + localidade.Bairro;
            if (localidade.Complemento != "" && localidade.Complemento != null)
                endereco += " (" + localidade.Complemento + ")";
            knouLocalidade.DescricaoEndereco.val(endereco);

            var local = localidade.Localidade.DescricaoCidadeEstado;
            if (localidade.Localidade.Pais != null)
                local += ", " + localidade.Localidade.Pais.Nome;

            if (localidade.CEP != "")
                local += " CEP: " + localidade.CEP;

            knouLocalidade.DescricaoLocalidade.val(local);

            if (localidade.Telefone1 != "") {
                knouLocalidade.DescricaoTelefone.visible(true);
                knouLocalidade.DescricaoTelefone.val(localidade.Telefone1);
            } else {
                knouLocalidade.DescricaoTelefone.visible(false);
            }

            verificarFronteira();
            if (tipoCliente != null) {
                if (tipoCliente == "R") {
                    _pedido.Origem.val(localidade.Localidade.Descricao);
                    _pedido.Origem.codEntity(localidade.Localidade.Codigo);

                    _percursosEntreEstados.EstadoOrigem.val(localidade.Localidade.Estado.Sigla);
                    _pedido.UFOrigem.val(localidade.Localidade.Estado.Sigla);
                    mapaOrigemDestinoChange();

                } else if (tipoCliente == "D") {

                    _percursosEntreEstados.EstadoDestino.val(localidade.Localidade.Estado.Sigla);
                    _pedido.UFDestino.val(localidade.Localidade.Estado.Sigla);
                    mapaOrigemDestinoChange();
                }
            }

        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function verificarOrigemDestino() {
    if (_localidadeOrigem.MudarEndereco.val()) {
        _pedido.UsarOutroEnderecoOrigem.val(true);
        if (_localidadeOrigem.ClienteOutroEndereco.codEntity() == 0) {
            if (ValidarCamposObrigatorios(_localidadeOrigem)) {
                _pedido.LocalidadeClienteOrigem.codEntity(0);
                _pedido.Origem.codEntity(_localidadeOrigem.Localidade.codEntity());
                _pedido.BairroOrigem.val(_localidadeOrigem.Bairro.val());
                _pedido.CEPOrigem.val(_localidadeOrigem.CEP.val());
                _pedido.NumeroOrigem.val(_localidadeOrigem.Numero.val());
                _pedido.ComplementoOrigem.val(_localidadeOrigem.Complemento.val());
                _pedido.EnderecoOrigem.val(_localidadeOrigem.Endereco.val());
                _pedido.Telefone1Origem.val(_localidadeOrigem.Telefone1.val());
                _pedido.RGIE1Origem.val(_localidadeOrigem.RGIE.val());

            } else {
                $("#myTab a:eq(1)").tab("show");
                exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
                return false;
            }
        } else {
            _pedido.LocalidadeClienteOrigem.codEntity(_localidadeOrigem.ClienteOutroEndereco.codEntity());
            _pedido.LocalidadeClienteOrigem.val(_localidadeOrigem.ClienteOutroEndereco.val());
        }
    } else {
        _pedido.UsarOutroEnderecoOrigem.val(false);
    }

    if (_localidadeDestino.MudarEndereco.val()) {
        _pedido.UsarOutroEnderecoDestino.val(true);
        if (_localidadeDestino.ClienteOutroEndereco.codEntity() == 0) {
            if (ValidarCamposObrigatorios(_localidadeDestino)) {
                _pedido.LocalidadeClienteDestino.codEntity(0);
                _pedido.Destino.codEntity(_localidadeDestino.Localidade.codEntity());
                _pedido.Destino.val(_localidadeDestino.Localidade.codEntity());
                _pedido.BairroDestino.val(_localidadeDestino.Bairro.val());
                _pedido.CEPDestino.val(_localidadeDestino.CEP.val());
                _pedido.NumeroDestino.val(_localidadeDestino.Numero.val());
                _pedido.ComplementoDestino.val(_localidadeDestino.Complemento.val());
                _pedido.EnderecoDestino.val(_localidadeDestino.Endereco.val());
                _pedido.Telefone1Destino.val(_localidadeDestino.Telefone1.val());
                _pedido.RGIE1Destino.val(_localidadeDestino.RGIE.val());
            } else {
                $("#myTab a:eq(2)").tab("show");
                exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
                return false;
            }
        } else {
            _pedido.LocalidadeClienteDestino.codEntity(_localidadeDestino.ClienteOutroEndereco.codEntity());
            _pedido.LocalidadeClienteDestino.val(_localidadeDestino.ClienteOutroEndereco.val());
        }
    } else {
        _pedido.UsarOutroEnderecoDestino.val(false);
    }
    return true;
}


function preecherDescricaoEnderecoOrigemUtilizado() {

    if (_pedido.CidadePoloOrigem.val() != "") {
        _localidadeOrigem.DescricaoCidadePolo.val(Localization.Resources.Pedidos.Pedido.CidadePolo +  ":" + _pedido.CidadePoloOrigem.val());
        _localidadeOrigem.DescricaoCidadePolo.visible(true);
    } else {
        _localidadeOrigem.DescricaoCidadePolo.visible(false);
    }

    var endereco = _pedido.EnderecoOrigem.val();
    if (_pedido.NumeroOrigem.val() != "")
        endereco += ", " + _pedido.NumeroOrigem.val();
    if (_pedido.BairroOrigem.val() != "")
        endereco += " - " + _pedido.BairroOrigem.val();
    if (_pedido.ComplementoOrigem.val() != "")
        endereco += " (" + _pedido.ComplementoOrigem.val() + ")";
    _localidadeOrigem.DescricaoEndereco.val(endereco);

    var local = _pedido.Origem.val();
    if (_pedido.PaisOrigem.val() != "")
        local += ", " + _pedido.PaisOrigem.val();

    if (_pedido.CEPOrigem.val() != "")
        local += " CEP: " + _pedido.CEPOrigem.val();

    _localidadeOrigem.DescricaoLocalidade.val(local);

    if (_pedido.Telefone1Origem.val() != "") {
        _localidadeOrigem.DescricaoTelefone.visible(true);
        _localidadeOrigem.DescricaoTelefone.val(_pedido.Telefone1Origem.val());
    } else {
        _localidadeOrigem.DescricaoTelefone.visible(false);
    }
}

function preecherDescricaoEnderecoDestinoUtilizado() {

    if (_pedido.CidadePoloDestino.val() != "") {
        _localidadeDestino.DescricaoCidadePolo.val(Localization.Resources.Pedidos.Pedido.CidadePolo + ":" + _pedido.CidadePoloDestino.val());
        _localidadeDestino.DescricaoCidadePolo.visible(true);
    } else {
        _localidadeDestino.DescricaoCidadePolo.visible(false);
    }

    var endereco = _pedido.EnderecoDestino.val();

    if (_pedido.NumeroDestino.val() != "")
        endereco += ", " + _pedido.NumeroDestino.val();
    if (_pedido.BairroDestino.val() != "")
        endereco += " - " + _pedido.BairroDestino.val();
    if (_pedido.ComplementoDestino.val() != "")
        endereco += " (" + _pedido.ComplementoDestino.val() + ")";
    _localidadeDestino.DescricaoEndereco.val(endereco);

    var local = _pedido.Destino.val();
    if (_pedido.PaisDestino.val() != "")
        local += ", " + _pedido.PaisDestino.val();

    if (_pedido.CEPDestino.val() != "")
        local += " CEP: " + _pedido.CEPDestino.val();

    _localidadeDestino.DescricaoLocalidade.val(local);

    if (_pedido.Telefone1Destino.val() != "") {
        _localidadeDestino.DescricaoTelefone.visible(true);
        _localidadeDestino.DescricaoTelefone.val(_pedido.Telefone1Destino.val());
    } else {
        _localidadeDestino.DescricaoTelefone.visible(false);
    }
}
