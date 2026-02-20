/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="Canhoto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _reverterJustificativa;

var ReverterJustificativa = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Canhotos.Canhoto.Justificativa.getRequiredFieldDescription(), maxlength: 300, required: true });
    this.Reverter = PropertyEntity({ eventClick: ReverterJustificativaClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Reverter, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadReverterJustificativa() {
    _reverterJustificativa = new ReverterJustificativa();
    KoBindings(_reverterJustificativa, "KnoutReverterJustificativa");
}

function abrirModalReverterJustificativaClick(e) {
    LimparCampos(_reverterJustificativa);
    _reverterJustificativa.Codigo.val(e.Codigo);    
    Global.abrirModal('divModalReverterJustificativa');
}

function ReverterJustificativaClick() {
    var dados = RetornarObjetoPesquisa(_reverterJustificativa);

    executarReST("Canhoto/ReverterJustificativa", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Canhotos.Canhoto.CanhotoRevertidoComSucesso);
                Global.fecharModal('divModalReverterJustificativa');
                buscarCanhotos();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function VisibilidadeOpcaoReverterJustificativa(e) {
    return e.SituacaoCanhoto == EnumSituacaoCanhoto.Justificado
        && !e.CargaEncerrada
        && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador
        && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhotos_ReverterJustificativa_Canhotos, _PermissoesPersonalizadasCanhotos)
        ;
}