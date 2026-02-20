var _entregasFimViagem;
var _listaEntregasComDatas;
var DataFimEntregasFimViagem = function () {
    this.CodigoEntrega = PropertyEntity({ val: ko.observable(0) });
    this.Entregas = PropertyEntity({ val: ko.observable([]) });
    this.DataFimEntrega = PropertyEntity({ text: ko.observable(""), getType: typesKnockout.dateTime, val: ko.observable(""), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: "Cliente", getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status", getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroListagemEntregas = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
}

var EntregasFimViagem = function () {
    this.DataFimViagem = PropertyEntity({ text: "Data Fim Viagem", getType: typesKnockout.string, val: ko.observable("") });
    this.Entregas = PropertyEntity({ val: ko.observableArray([]) });
}
function loadModalDataEntregasFimViagem(carga) {
    _entregasFimViagem = new EntregasFimViagem();
    KoBindings(_entregasFimViagem, "knockoutEntregasFimViagem");
    $('#alerta-personalizado-viagem-finalizada').html("");
    executarReST("/ControleEntrega/ObterControleEntregaPorcarga", { Carga: carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                let entregas = arg.Data.Entregas.Entregas;
                let tornarFinalizacaoDeEntregasAssincrona = arg.Data.Entregas.TornarFinalizacaoDeEntregasAssincrona;

                if (arg.Data.Entregas.DataFimViagem != "") {
                    _entregasFimViagem.DataFimViagem.val(arg.Data.Entregas.DataFimViagem);
                    $('#alerta-personalizado-viagem-finalizada').html(Localization.Resources.Cargas.ControleEntrega.AViagemFoiFinalizadaNoDia + ' ' + _entregasFimViagem.DataFimViagem.val() + '')
                }

                montarHTMLDataEntregasFimViagem(entregas, tornarFinalizacaoDeEntregasAssincrona);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

async function montarHTMLDataEntregasFimViagem(entregas, tornarFinalizacaoDeEntregasAssincrona) {
    $('#conteudo-modal-data-entregas-fim-viagem').html("");

    for (let i = 0; i < entregas.length; i++) {
        await new Promise((resolve) => {
            $.get("Content/Static/Carga/ControleEntrega/ConteudoModalDataEntregaFimViagem.html?dyn=" + guid(), function (html) {
                _dataFimEntregasFimViagem = new DataFimEntregasFimViagem();

                let entrega = entregas[i];
                let numeroEntrega = i + 1;
                let statusEntrega = entrega.Situacao != EnumSituacaoEntrega.Entregue ? Localization.Resources.Cargas.ControleEntrega.Pendente : Localization.Resources.Cargas.ControleEntrega.Entregue;
                _listaEntregasComDatas = [];

                _dataFimEntregasFimViagem.Cliente.val(entrega.Cliente);
                _dataFimEntregasFimViagem.CodigoEntrega.val(entrega.Codigo);
                _dataFimEntregasFimViagem.Status.val(statusEntrega);
                _dataFimEntregasFimViagem.NumeroListagemEntregas.val((entrega.Coleta ? "Coleta #" + numeroEntrega : "Entrega #" + numeroEntrega));
                _dataFimEntregasFimViagem.DataFimEntrega.text((entrega.Coleta ? "Data e Hora Confirmação da Coleta" : "Data e Hora Confirmação da Entrega"));

                if (entrega.DataEntrega != '') {
                    _dataFimEntregasFimViagem.DataFimEntrega.val(entrega.DataEntrega);
                    _dataFimEntregasFimViagem.DataFimEntrega.enable(false);
                }

                if (entrega.SituacaoProcessamento == EnumSituacaoProcessamentoIntegracaoSuperApp.AguardandoProcessamento && tornarFinalizacaoDeEntregasAssincrona && entrega.DataEntrega == '') {
                    _dataFimEntregasFimViagem.Status.val("Em processamento");
                    _dataFimEntregasFimViagem.DataFimEntrega.enable(false);
                }

                let knockoutEntregasFimViagem = "knockoutEntregasFimViagem";
                let knockoutEntregasFimViagemDinamico = knockoutEntregasFimViagem + guid();

                html = html.replaceAll(knockoutEntregasFimViagem, knockoutEntregasFimViagemDinamico);
                $('#conteudo-modal-data-entregas-fim-viagem').append(html);

                KoBindings(_dataFimEntregasFimViagem, knockoutEntregasFimViagemDinamico);
                _dataFimEntregasFimViagem.DataFimEntrega.val(entrega.DataConfirmacao);

                _entregasFimViagem.Entregas.val.push(_dataFimEntregasFimViagem);

                resolve();
            });
        });
    }

}

function obterEntregasComDatas() {
    let listaEntregas = [];

    for (let entrega of _entregasFimViagem.Entregas.val()) {
        listaEntregas.push({
            CodigoEntrega: entrega.CodigoEntrega.val(),
            DataFimEntrega: entrega.DataFimEntrega.val()
        })
    }

    return JSON.stringify(listaEntregas);
}

function validarCamposObrigatoriosDatasEntregas() {
    let algumCampoInvalido = false;

    for (let entrega of _entregasFimViagem.Entregas.val()) {

        if (!ValidarCamposObrigatorios(entrega)) {
            if (entrega.DataFimEntrega.val() == "") {
                entrega.DataFimEntrega.requiredClass("form-control is-invalid");
                algumCampoInvalido = true;
            }
        }
    }

    if (algumCampoInvalido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return false;
    }

    return true;
}
