/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="DetalhesDocumento.js" />
/// <reference path="ControleDocumento.js" />

// #region Objetos Globais do Arquivo

var _ctesAprovacao;
var _detalhePreCTeAprovacao;
var _detalheParqueamento;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalheControleDocumento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
}

var DetalheParqueamento = function () {
    this.Codigo = PropertyEntity({ def: 0, val: ko.observable(0), getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), enable: false });
    this.MotivoTransportador = PropertyEntity({ text: ko.observable(""), val: ko.observable(""), enable: false, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalheControleDocumento() {
    _detalhePreCTeAprovacao = new DetalheDocumento();
    KoBindings(_detalhePreCTeAprovacao, "knoutDetalhePreCTeAprovacao");

    _detalheParqueamento = new DetalheParqueamento();
    KoBindings(_detalheParqueamento, "knoutDetalheParqueamento");

    _ctesAprovacao = new DetalhesDocumentosCTes();
    KoBindings(_ctesAprovacao, "knockoutDetalhesCTes");
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirDetalheControleDocumento(registroSelecionado) {
    LimparCampos(_detalhePreCTeAprovacao);
    _ctesAprovacao.CTes.val.removeAll();

    executarReST("ControleDocumento/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherDetalhesCTes(retorno.Data, _ctesAprovacao);
                Global.ExibirAba(_ctesAprovacao.CTes.val().find(x => x.IndexTab.val() === 0).IdTab.val())
                Global.abrirModal('divModalDetalheGestaoDocumentoAprovacao');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirDetalheParqueamento(registroSelecionado) {
    LimparCampos(_detalheParqueamento);

    executarReST("ControleDocumento/BuscarDadosParqueamento", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                detalheParqueamento = retorno.Data;
                PreencherObjetoKnout(_detalheParqueamento, { Data: detalheParqueamento });
                if (detalheParqueamento.Aprovado) {
                    _detalheParqueamento.MotivoTransportador.text("Motivo da Aprovação:");
                    _detalheParqueamento.MotivoTransportador.val(detalheParqueamento.MotivoTransportador);
                    _detalheParqueamento.MotivoTransportador.visible(true);
                }
                else if (detalheParqueamento.Rejeitado) {
                    _detalheParqueamento.MotivoTransportador.text("Motivo da Rejeição:");
                    _detalheParqueamento.MotivoTransportador.val(detalheParqueamento.MotivoTransportador);
                    _detalheParqueamento.MotivoTransportador.visible(true);
                }
                else {
                    _detalheParqueamento.MotivoTransportador.visible(false);
                }

                Global.abrirModal('divModalDetalheParqueamento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function fecharModalDetalheControleDocumento() {
    Global.fecharModal('divModalDetalheControleDocumento');
}


// #endregion Funções Privadas
