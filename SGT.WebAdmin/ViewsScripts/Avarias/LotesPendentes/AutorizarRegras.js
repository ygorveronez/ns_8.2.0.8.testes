/// <reference path="Autorizacao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="Autorizacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _regraLote;

var RegraLote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******

function loadRegras() {
    _regraLote = new RegraLote();
}

function rejeitarLoteClick() {
    var dados = {
        Codigo: _regraLote.Codigo.val()
    }
    executarReST("Lotes/VoltarEtapa", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote pendente de aprovação.");
                limparRejeicao();
                _gridLote.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function aprovarLoteClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja integrar o lote?", function () {
        var dados = {
            Codigo: _regraLote.Codigo.val(),
        };
        executarReST("Lotes/IntegrarLote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Lote integrado com sucesso.");
                    limparRejeicao();
                    _gridLote.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}




//*******MÉTODOS*******
function limparRejeicao() {
    LimparCampos(_autorizacao);
    Global.fecharModal('divModalLote');
}