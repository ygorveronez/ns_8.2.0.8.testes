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

//********MAPEAMENTO*******

var _fimViagem;

var FimViagem = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataFimViagem = PropertyEntity({ text: "Data Fim da Viagem:", val: ko.observable(""), def: "" });
    this.InformarFimViagem = PropertyEntity({ eventClick: informarFimViagemClick, type: types.event, text: "Finalizar Viagem", idGrid: guid(), visible: ko.observable(false) });
}




//********EVENTO*******
function LoadFimViagem() {
    _fimViagem = new FimViagem();
    KoBindings(_fimViagem, "knockoutFimViagem");
}

function ExibirFimViagem(knoutFluxo) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_fimViagem);
    executarReST("FimViagem/BuscarPorCarga", { Carga: knoutFluxo.Carga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _fimViagem.InformarFimViagem.visible(false);

                PreencherObjetoKnout(_fimViagem, arg);

                if (arg.Data.ViagemAberta)
                    _fimViagem.InformarFimViagem.visible(true);

                ExibeModalEtapa('#divModalFimViagem');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}



//********MÉTODOS*******
function informarFimViagemClick() {
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja finalizar a viagem?", function () {
        executarReST("FimViagem/InformarFimViagem", { Codigo: _fimViagem.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Viagem finalizada com sucesso!");
                    AtualizarFluxoEntrega();
                    Global.fecharModal('divModalFimViagem');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}