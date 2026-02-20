//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoControleEntrega, _crudConfiguracaoControleEntrega;

$(document).on("input", "#txtMensagemUsuario", function () {
    var limite = 750;
    var caracteresDigitados = $(this).val().length;
    var caracteresRestantes = limite - caracteresDigitados;

    $(".caracteres").text(caracteresRestantes);
});


var ConfiguracaoControleEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MensagemChatAssumirMonitoramentoCarga = PropertyEntity({ text: Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.MensagemAoMotoristaQuandoUmAnalistaAssumeSuaCarga.getFieldDescription() , maxlength: 750});
    this.TagMotorista = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga.id, Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.Motoristaa); }, type: types.event, text: Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.Motorista, visible: ko.observable(true) });
    this.TagAnalista = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracaoControleEntrega.MensagemChatAssumirMonitoramentoCarga.id, Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.Analistaa); }, type: types.event, text: Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.Analista, visible: ko.observable(true) });
    this.TempoInicioViagemAposEmissaoDoc = PropertyEntity({ text: Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.TempoParaInicioViagemAposEmissaoDocsMinutos.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });
    this.TempoInicioViagemAposFinalizacaoFluxoPatio = PropertyEntity({ text: Localization.Resources.Configuracoes.ConfiguracaoControleEntrega.TempoParaInicioViagemAposEncerramentoFluxoDePatioMinutos.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0) });

}

var CRUDConfiguracaoControleEntrega = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar,  visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadConfiguracaoControleEntrega() {
    _configuracaoControleEntrega = new ConfiguracaoControleEntrega();
    KoBindings(_configuracaoControleEntrega, "knockoutCadastro");

    _crudConfiguracaoControleEntrega = new CRUDConfiguracaoControleEntrega();
    KoBindings(_crudConfiguracaoControleEntrega, "knockoutCRUD");

    editarConfiguracaoControleEntrega();
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoControleEntrega, "ConfiguracaoControleEntrega/Atualizar", function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

//*******MÉTODOS*******

function editarConfiguracaoControleEntrega() {
    BuscarPorCodigo(_configuracaoControleEntrega, "ConfiguracaoControleEntrega/BuscarPorPadrao", function (arg) {
        contarCaracteres();
    }, null);
}

function contarCaracteres() {
    var limite = 750;
    var caracteresDigitados = $("#txtMensagemUsuario").val().length;
    var caracteresRestantes = limite - caracteresDigitados;

    $(".caracteres").text(caracteresRestantes);
}
