/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumSituacaoNotasPendetesIntegracaoMercadoLivre.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _CargaNotaPendenteIntegracaoMercadoLivre;
var _gridCargaNotaPendenteIntegracaoMercadoLivre;

function knocloutAdicionarCargaPendenteIntegracaoMercadoLivre() {

    this.Carga = PropertyEntity({ type: types.Entities, codEntity: ko.observable(0), val: ko.observable(""), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarCargaClick, text: ko.observable("Adicionar Carga"), visible: ko.observable(true) });
    this.SituacaoDownload = PropertyEntity({ text: "Situação:", options: EnumSituacaoNotasPendetesIntegracaoMercadoLivre.obterOpcoes(), val: ko.observable(EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente), def: EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(""), def: "", maxlength: 10, getType: typesKnockout.string, text: "*Carga:", visible: ko.observable(true), enable: ko.observable(true), required: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaNotaPendenteIntegracaoMercadoLivre.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
   
}

function loadAdicionarCargaNotaPendenteIntegracaoMercadoLivre() {

    _CargaNotaPendenteIntegracaoMercadoLivre = new knocloutAdicionarCargaPendenteIntegracaoMercadoLivre();
    KoBindings(_CargaNotaPendenteIntegracaoMercadoLivre, "knoutAdicionarCargaNotasPendentesIntegracaoMercadoLivre");

    new BuscarCargas(_CargaNotaPendenteIntegracaoMercadoLivre.Carga);

    buscarCargasComNotasPendentesIntegracaoMercadoLivre()
}

function buscarCargasComNotasPendentesIntegracaoMercadoLivre() {

    var reprocessar = { descricao: "Reprocessar", id: guid(), evento: "onclick", metodo: reprocessarClick, tamanho: "20", icone: "", visibilidade: habilitarReprocessar };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [reprocessar],
        tamanho: 7
    };

    _gridCargaNotaPendenteIntegracaoMercadoLivre = new GridView(_CargaNotaPendenteIntegracaoMercadoLivre.Pesquisar.idGrid, "PainelNFeTransporte/PesquisaCargaNotasPendentesIntegracao", _CargaNotaPendenteIntegracaoMercadoLivre, menuOpcoes, null);
    _gridCargaNotaPendenteIntegracaoMercadoLivre.CarregarGrid();
}

function abrirModalSelecaoCargaNotasPendetesIntegracaoMercadoLivre() {

    Global.abrirModal('divModalAdicionarCargaNotasPendentesIntegracaoMercadoLivre');
}

function reprocessarClick(registroSelecionado) {
    executarReST("PainelNFeTransporte/ReprocessarCargaNotasPendentes", { Codigo: registroSelecionado.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data === true) {
                buscarCargasComNotasPendentesIntegracaoMercadoLivre();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Sucesso, "Carga reprocessada com sucesso.");
            } else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function adicionarCargaClick() {

   if (ValidarCamposObrigatorios(_CargaNotaPendenteIntegracaoMercadoLivre)) {
        executarReST("PainelNFeTransporte/AdicionarCargaSemNota", { CodigoCarga: _CargaNotaPendenteIntegracaoMercadoLivre.CodigoCarga.val() }, function (r) {
            if (r.Success) {
                if (r.Data === true) {
                    buscarCargasComNotasPendentesIntegracaoMercadoLivre();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Sucesso, "Carga adicionada com sucesso.");
                } else 
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
   } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
   }
}

function habilitarReprocessar(registroSelecionado)
{
   
    if (registroSelecionado.SituacaoDownloadNotasCarga === "Concluído") {
        return false;
    }

    return true;
}

