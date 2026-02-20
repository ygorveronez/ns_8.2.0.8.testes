/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />

var _inicioViagem;

var InicioViagem = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataInicioViagem = PropertyEntity({ text: "Data Início da Viagem:", val: ko.observable(""), def: "" });
    this.InformarInicioViagem = PropertyEntity({ eventClick: informarInicioViagemClick, type: types.event, text: "Iniciar Viagem", idGrid: guid(), visible: ko.observable(false) });
}

function LoadInicioViagem() {
    _inicioViagem = new InicioViagem();
    KoBindings(_inicioViagem, "knockoutInicioViagem");
}

function ExibirDetalhesInicioViagemFluxoEntrega(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_inicioViagem);

    executarReST("FluxoEntrega/BuscarPorCarga", { Carga: knoutFluxo.Carga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _inicioViagem.InformarInicioViagem.visible(false);

                PreencherObjetoKnout(_inicioViagem, arg);

                if (arg.Data.ViagemAberta)
                    _inicioViagem.InformarInicioViagem.visible(true);

                ExibeModalEtapa('#divModalInicioViagem');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}



//********MÉTODOS*******
function informarInicioViagemClick() {
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja iniciar a viagem?", function () {
        executarReST("FluxoEntrega/InformarInicioViagem", { Codigo: _inicioViagem.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Viagem iniciada com sucesso!");
                    AtualizarFluxoEntrega();
                    Global.fecharModal('divModalInicioViagem');
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}