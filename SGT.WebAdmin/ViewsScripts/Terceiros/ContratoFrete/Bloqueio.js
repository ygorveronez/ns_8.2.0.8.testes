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
/// <reference path="../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Cargas/Carga/Carga.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _bloqueioContratoFrete;

var BloqueioContratoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Justificativa = PropertyEntity({ val: ko.observable(""), def: "", text: "Justificativa:", required: true, maxlength: 500 });

    this.Bloquear = PropertyEntity({ eventClick: BloquearContratoFreteClick, type: types.event, text: "Bloquear", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaBloqueioContratoFrete, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadBloqueioContratoFrete() {
    _bloqueioContratoFrete = new BloqueioContratoFrete();
    KoBindings(_bloqueioContratoFrete, "divModalBloqueioContratoFrete");
}

function BloquearContratoFreteClick() {
    _bloqueioContratoFrete.Codigo.val(_contratoFrete.Codigo.val());

    Salvar(_bloqueioContratoFrete, "ContratoFrete/BloquearContrato", function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                _contratoFrete.DesbloquearContrato.visible(true);
                _contratoFrete.BloquearContrato.visible(false);
                _contratoFrete.RejeitarContrato.visible(false);
                _contratoFrete.AutorizarContrato.visible(false);
                _contratoFrete.ReabrirContrato.visible(false);
                _contratoFrete.FinalizarContrato.visible(false);

                _detalhesContratoFrete.PercentualAdiantamento.enable(false);
                _detalhesContratoFrete.Descontos.enable(false);
                _detalhesContratoFrete.Observacao.enable(false);
                _detalhesContratoFrete.AlterarContrato.visible(false);
                _detalhesContratoFrete.ValorOutrosAdiantamento.enable(false);
                _detalhesContratoFrete.ValorFreteSubcontratacao.enable(false);
                _detalhesContratoFrete.JustificativaBloqueio.val(arg.Data.Justificativa);
                _detalhesContratoFrete.Bloqueado.val(arg.Data.Bloqueado);

                _gridContratoFrete.CarregarGrid();

                FecharTelaBloqueioContratoFrete();

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato bloqueado com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function DesbloquearContratoFreteClick(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja desbloquear esse contrato de frete?", function () {
        Salvar(e, "ContratoFrete/DesbloquearContrato", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Contrato desbloqueado com sucesso!");
                    editarContratoFrete({ Codigo: _contratoFrete.Codigo.val(), CodigoCarga: _contratoFrete.Carga.val() });
                    _gridContratoFrete.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function AbrirTelaBloqueioContratoFrete() {
    LimparCampos(_bloqueioContratoFrete);
    Global.abrirModal("divModalBloqueioContratoFrete");
}

function FecharTelaBloqueioContratoFrete() {
    LimparCampos(_bloqueioContratoFrete);
    Global.fecharModal('divModalBloqueioContratoFrete');
}

