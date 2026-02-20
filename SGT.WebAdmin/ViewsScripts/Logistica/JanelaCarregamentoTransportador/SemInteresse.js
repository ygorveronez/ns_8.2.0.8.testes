/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumTipoFreteEscolhido.js" />

// #region Objetos Globais do Arquivo

var _informarValorFrete;
var _SemInteresse;
var _carga;
// #endregion Objetos Globais do Arquivo

// #region Classes


var SemInteresseCarga = function () {

    this.Observacao = PropertyEntity({ text: "Justificativa da Recusa:", val: ko.observable(""), def: "" });

    this.MotivoRecusaCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.MotivoRecusa), idBtnSearch: guid(), visible: ko.observable(true), required: false });
    this.MensagemRetorno = PropertyEntity({});

    this.SalvarSemInteresse = PropertyEntity({ eventClick: function (e, sender) { confirmarRecusaInteresseClick(e, sender); }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Confirmar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadSemInteresse(e) {
    _carga = e;
    _SemInteresse = new SemInteresseCarga();
    KoBindings(_SemInteresse, "divModalSolicitarRecusaInteresseCarga");
    Global.abrirModal('divModalSolicitarRecusaInteresseCarga');
    BuscarMotivoRetiradaFilaCarregamento(_SemInteresse.MotivoRecusaCarga);


}
function naoTenhoInteresseClick(e, sender) {
    loadSemInteresse(e);
}

function confirmarRecusaInteresseClick(e, sender) {
    if (_SemInteresse.MotivoRecusaCarga.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
    if (_SemInteresse.Observacao.val() == "") {
        return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
    }
    var dados = { Carga: _carga.Carga.val(), MotivoRecusaCarga: _SemInteresse.MotivoRecusaCarga.codEntity(), Observacao: _SemInteresse.Observacao.val() };
    exibirConfirmacao("Confirmação", "Realmente deseja marcar como sem interesse na carga?", function () {
        executarReST("JanelaCarregamentoTransportador/MarcarSemInteresseCarga", dados, function (r) {
            if (r.Success) {
                if (r.Data !== false) {

                    recarregarPesquisaCargas();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });

    Global.fecharModal('divModalSolicitarRecusaInteresseCarga');

}

