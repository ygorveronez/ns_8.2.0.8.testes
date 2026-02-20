/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Empresa.js" />

var _dadosTransporteCarga;
var _CRUDdadosTransporteCarga;
var _modalInformarMotoristaVeiculo;

var DadosTransporteCarga = function () {
    this.Guarita = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Transportador:", idBtnSearch: guid(), required: true });
}

var CRUDDadosTransporteCarga = function () {
    this.Liberar = PropertyEntity({ eventClick: salvarDadosTransporteCargaClick, type: types.event, text: "Liberar", idFade: guid(), visible: ko.observable(true) });
}

function loadDadosCarga() {
    _dadosTransporteCarga = new DadosTransporteCarga();
    KoBindings(_dadosTransporteCarga, "knockoutInformacaoTransporteCarga");

    _CRUDdadosTransporteCarga = new CRUDDadosTransporteCarga();
    KoBindings(_CRUDdadosTransporteCarga, "knockoutInformacaoTransporteCargaCRUD");

    new BuscarMotoristas(_dadosTransporteCarga.Motorista);
    new BuscarVeiculos(_dadosTransporteCarga.Veiculo);
    new BuscarTransportadores(_dadosTransporteCarga.Transportador);

    _modalInformarMotoristaVeiculo = new bootstrap.Modal(document.getElementById("divModalInformarMotoristaVeiculo"), { backdrop: 'static', keyboard: true });
}

function salvarDadosTransporteCargaClick() {
    if (!ValidarCamposObrigatorios(_dadosTransporteCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var data = {
        Codigo: _dadosTransporteCarga.Carga.val(),
        Empresa: _dadosTransporteCarga.Transportador.codEntity(),
        Motorista: _dadosTransporteCarga.Motorista.codEntity(),
        Veiculo: _dadosTransporteCarga.Veiculo.codEntity()
    };

    executarReST("CargaTransportador/SalvarDadosTransportador", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                liberarCarga({ Codigo: _dadosTransporteCarga.Guarita.val() });
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 60000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function BuscarDadosDaCarga(codigo, codigoGuarita) {
    executarReST("Guarita/BuscarDadosCarga", { Carga: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_dadosTransporteCarga, retorno);
                _dadosTransporteCarga.Carga.val(codigo);
                _dadosTransporteCarga.Guarita.val(codigoGuarita);

                _modalInformarMotoristaVeiculo.show();

                $("#divModalInformarMotoristaVeiculo").one('hidden.bs.modal', function () {
                    LimparCampos(_dadosTransporteCarga);
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg, 60000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}