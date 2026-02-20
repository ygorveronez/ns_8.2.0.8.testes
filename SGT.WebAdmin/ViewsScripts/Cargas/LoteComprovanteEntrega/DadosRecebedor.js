/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />

/**
 *  Esse arquivo cuida de tudo que é necessário para gerenciar o modal dos dados de recebedor das cargaEntregas
 * */

var _dadosRecebedor;
var _canhotoAtual;

// region: ENTIDADES KNOCKOUT

var DadosRecebedor = function () {
    this.Nome = PropertyEntity({ text: "*Nome: ", required: true });
    this.CPF = PropertyEntity({ text: "*CPF: ", getType: typesKnockout.cpf, required: true });
    this.DataEntrega = PropertyEntity({ text: "*Data Entrega: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: ko.observable("Salvar"), visible: ko.observable(true), enable: ko.observable(true) });
    this.AplicarDemais = PropertyEntity({ eventClick: AplicarDadosRecebedorDemaisClick, type: types.event, text: ko.observable("Aplicar dados as demais paradas sem dados de recebedor"), visible: ko.observable(true), enable: ko.observable(true) });
}

// region: ACTIONS

function SalvarClick() {

    if (!ValidarCamposObrigatorios(_dadosRecebedor)) {
        return;
    }

    _canhotoAtual.DadosRecebedor = {
        Nome: _dadosRecebedor.Nome.val(),
        CPF: _dadosRecebedor.CPF.val(),
        DataEntrega: _dadosRecebedor.DataEntrega.val(),
    }
    
    Global.fecharModal("divModalDadosRecebedor");
}

/*
 *  Aplica os dados recebedor atual a todos as outras cargaEntregas da lista que não têm uma imagem
 */
async function AplicarDadosRecebedorDemaisClick() {
    if (!ValidarCamposObrigatorios(_dadosRecebedor)) {
        return;
    }

    exibirConfirmacao("Aplicar dados de recebedor", "Tem certeza que deseja aplicar esses dados de recebedor às paradas sem dados de recebedor?", async function () {
        var dadosRecebedor = {
            Nome: _dadosRecebedor.Nome.val(),
            CPF: _dadosRecebedor.CPF.val(),
            DataEntrega: _dadosRecebedor.DataEntrega.val(),
        }

        for (let cargaEntrega of _dadosComprovanteEtapa.ListaCargaEntrega.val()) {
            if (!cargaEntrega.DadosRecebedor) {
                cargaEntrega.DadosRecebedor = dadosRecebedor;
            }
        }
    })
}

function loadModalDadosRecebedor() {
    _dadosRecebedor = new DadosRecebedor();
    KoBindings(_dadosRecebedor, "knockoutDadosRecebedor");
}

function exibirModalDadosRecebedor(cargaEntrega) {
    // Desabilita botões se for modo de visualização
    _dadosRecebedor.Salvar.enable(podeEditarLote());
    _dadosRecebedor.AplicarDemais.enable(podeEditarLote());

    _canhotoAtual = cargaEntrega;
    Global.abrirModal('divModalDadosRecebedor');
    $("#divModalDadosRecebedor").on("hidden.bs.modal", () => {
        LimparCampos(_dadosRecebedor)
        recarregarGridCargaEntrega();
    });

    if (cargaEntrega.DadosRecebedor) {
        _dadosRecebedor.Nome.val(cargaEntrega.DadosRecebedor.Nome);
        _dadosRecebedor.CPF.val(cargaEntrega.DadosRecebedor.CPF);
        _dadosRecebedor.DataEntrega.val(cargaEntrega.DadosRecebedor.DataEntrega);
    }

}
