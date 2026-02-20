/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />
/// <reference path="SolicitacaoAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _lote;
var _gridLote;

var Lote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Lote = PropertyEntity({ text: "Dados Lote", idGrid: guid(), visible: ko.observable(true) });
    this.Mensagem = PropertyEntity({ text: ko.observable(""), visible: ko.observable(true), cssClass: ko.observable("") });
    this.Situacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
};

//*******EVENTOS*******

function loadLote() {
    _lote = new Lote();
    KoBindings(_lote, "knockoutLotes");

    _gridLote = new GridView(_lote.Lote.idGrid, "FluxoAvaria/PesquisaLote", _fluxoAvaria);
}

//*******METODOS*******

function VerificaLote() {
    executarReST("FluxoAvaria/LoteSolicitacao", { Codigo: _fluxoAvaria.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.PossuiLote) {
                    _gridLote.CarregarGrid();
                    _lote.Lote.visible(true);
                    _lote.Situacao = arg.Data.SituacaoLote;
                    if (arg.Data.SituacaoLote == EnumSituacaoLote.EmCriacao || arg.Data.SituacaoLote == EnumSituacaoLote.EmCorrecao) {
                        _lote.Mensagem.text("Aguardando Finalização do Lote");
                        _lote.Mensagem.cssClass("warning");
                        _lote.Mensagem.visible(true);
                    } else if (arg.Data.SituacaoLote == EnumSituacaoLote.AgIntegracao || arg.Data.SituacaoLote == EnumSituacaoLote.EmIntegracao) {
                        _lote.Mensagem.text("Aguardando a Integração do Lote");
                        _lote.Mensagem.cssClass("warning");
                        _lote.Mensagem.visible(true);
                    } else if (arg.Data.SituacaoLote == EnumSituacaoLote.Finalizada || arg.Data.SituacaoLote == EnumSituacaoLote.FinalizadaComDestino) {
                        _lote.Mensagem.text("Lote Integrado com Sucesso");
                        _lote.Mensagem.cssClass("success");
                        _lote.Mensagem.visible(true);
                    }
                } else {
                    _lote.Lote.visible(false);

                    _lote.Mensagem.text("Aguardando criação do Lote");
                    _lote.Mensagem.cssClass("info");
                    _lote.Mensagem.visible(true);
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, "Falha", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparLote() {

}