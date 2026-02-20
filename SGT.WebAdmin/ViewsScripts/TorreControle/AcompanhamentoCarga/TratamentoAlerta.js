/// <reference path="../../../ViewsScripts/Consultas/Motorista.js" />
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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumConsultaPorEntregaStatus.js" />
/// <reference path="../../Enumeradores/EnumStatusViagemControleEntrega.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="acompanhamentocarga.js" />
/// <reference path="signalr.js" />

var _CRUDTratativaAlerta;

function loadCRUDTratativaAlerta() {
    _CRUDTratativaAlerta = new CRUDTratativaAlerta();
    KoBindings(_CRUDTratativaAlerta, "knockoutCRUDTratativaAlerta");
    limparCamposTratativa();
}

var CRUDTratativaAlerta = function () {

    this.DescricaoAlerta = PropertyEntity({ val: ko.observable("") });
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarTratativaClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarTratativaClick, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Tratativa = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Tratativa.getRequiredFieldDescription(), val: ko.observable(true), options: ko.observable([]), def: 1, visible: ko.observable(true) });
    this.UtilizaTratativa = PropertyEntity({ val: ko.observable(true) });
    this.TipoAlerta = PropertyEntity({ val: ko.observable(1) });// 1 - carga, 2 - monitoramento
    this.CodigoAlerta = PropertyEntity();
};


function alertaCardMapClick(idCarga) {
    for (var i = 0; i < _cardAcompanhamentoCarga.Cargas().length; i++) {
        if (_cardAcompanhamentoCarga.Cargas()[i].CodigoCarga.val() == idCarga) {
            card = _cardAcompanhamentoCarga.Cargas()[i];
            alertaCardClick(card);
            break;
        }
    }
}


//buscar os dados dos alertas (seja alerta de monitoramento ou Alerta de carga) e tratar individualmente
function alertaCardClick(dados) {
    if (_etapaAtualFluxo == undefined || _etapaAtualFluxo == null)
        _etapaAtualFluxo = new fluxoEntrega();

    _etapaAtualFluxo.Carga.val(dados.CodigoCarga.val());
    _etapaAtualFluxo.NomeMotorista.val(dados.Motoristas.val());
    _etapaAtualFluxo.NumeroMotorista.val(dados.NumeroMotorista.val());

    if (dados.Data.TipoUltimoAlertaCarga != null) {
        //alerta de carga
        var tratativa = false;

        executarReST("AlertaCarga/BuscarETratarAlertaPorCodigo", { Codigo: dados.Data.CodigoUltimoAlerta, TratativaAutomatica: tratativa }, function (arg) {
            if (arg.Success) {
                var data = arg.Data.Dados;
                if (arg.Data && data !== false) {

                    //abrir chamado
                    if (data.CodigoChamado != undefined) {
                        AbrirChamadoPorCodigo(data.CodigoChamado); //função dos scripts antigos
                    } else {

                        _CRUDTratativaAlerta.DescricaoAlerta.val(data.DescricaoAlerta);
                        _CRUDTratativaAlerta.CodigoAlerta.val(data.Codigo);
                        _CRUDTratativaAlerta.TipoAlerta.val(1);
                        _CRUDTratativaAlerta.Tratativa.visible(false);//alerta carga nao tem tratativa

                        ExibirModalTratativaAlerta();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
            }
        });
    } else if (dados.Data.TipoUltimoAlertaMonitoramento != null) {
        //alerta de Monitoramento
        loadTratativaAlerta({ CodigoAlerta: dados.Data.CodigoUltimoAlerta }, []);


    }
}

function ExibirModalTratativaAlerta() {
    Global.abrirModal('divModalTratativaAlerta');
}


function cancelarTratativaClick() {
    Global.fecharModal('divModalTratativaAlerta');
    limparCamposTratativa();
}

function confirmarTratativaClick(e, sender) {
    _CRUDTratativaAlerta.UtilizaTratativa.val(true);

    if (_CRUDTratativaAlerta.TipoAlerta.val() == 1) {
        //carga
        Salvar(_CRUDTratativaAlerta, "AlertaCarga/EfetuarTratativa", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

                    Global.fecharModal("divModalTratativaAlerta");
                    limparCamposTratativa();

                    BuscarCargasAcompanhamento(1, false).then(function () {
                        AplicarConfigWidget();
                        loadCargasNoMapa();
                    });
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);

    } else {
        //Monitoramento
        Salvar(_CRUDTratativaAlerta, "AlertaTratativa/Adicionar", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

                    Global.fecharModal("divModalTratativaAlerta");
                    limparCamposTratativa();

                    BuscarCargasAcompanhamento(1, false).then(function () {
                        AplicarConfigWidget();
                        loadCargasNoMapa();
                    });
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, sender);
    }
}

function limparCamposTratativa() {
    LimparCampos(_CRUDTratativaAlerta);
    _CRUDTratativaAlerta.Tratativa.options([]);
}
