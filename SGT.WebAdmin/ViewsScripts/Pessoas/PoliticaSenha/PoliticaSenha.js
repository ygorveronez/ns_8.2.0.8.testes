/// <reference path="../../Enumeradores/EnumValorTipoAutomatizacaoTipoCarga.js" />
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
/// <reference path="TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoAutomatizacaoTipoCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _politicasSenhas;
var PoliticaSenha = function (data) {

    this.Codigo = PropertyEntity({ val: ko.observable(true), def: 0, getType: typesKnockout.int });
    this.HabilitarPoliticaSenha = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Pessoas.PoliticaSenha.HabilitarPoliticaSenhas, def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.ExigirTrocaSenhaPrimeiroAcesso = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Pessoas.PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso, issue: 643, def: false, visible: ko.observable(true) });
    // Senha padrão primeiro acesso - Campo ocultado conforme solicitação
    this.SenhaPadraoPrimeiroAcesso = PropertyEntity({ text: Localization.Resources.Pessoas.PoliticaSenha.SenhaPadraoPrimeiroAcesso.getFieldDescription(), issue: 645, required: false, maxlength: 30, visible: ko.observable(false) });
    // Campo ocultado por questões de segurança - Account Takeover Prevention
    this.UsarCPFUsuarioComoSenhaPadraoPrimeiroAcesso = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), text: Localization.Resources.Pessoas.PoliticaSenha.UsarCPFUsuarioComoSenhaPrimeiroAcesso, issue: 644, def: false, visible: ko.observable(false) });

    this.NumeroMinimoCaracteresSenha = PropertyEntity({ text: Localization.Resources.Pessoas.PoliticaSenha.QuantidadeMinimaCaracteresParaSenha.getFieldDescription(), issue: 646, getType: typesKnockout.int, required: false, maxlength: 2, configInt: { precision: 0, allowZero: false, allowNegative: false }, visible: ko.observable(true) });
    this.PrazoExpiraSenha = PropertyEntity({ text: Localization.Resources.Pessoas.PoliticaSenha.SenhaDeveExpirarAposQuantosDias.getFieldDescription(), issue: 647, getType: typesKnockout.int, required: false, maxlength: 3, configInt: { precision: 0, allowZero: false, allowNegative: false }, visible: ko.observable(true) });
    this.BloquearUsuarioAposQuantidadeTentativas = PropertyEntity({ text: Localization.Resources.Pessoas.PoliticaSenha.BloquearUsuarioAposQuantasTentativasInvalidas.getFieldDescription(), issue: 648, getType: typesKnockout.int, required: false, maxlength: 3, configInt: { precision: 0, allowZero: false, allowNegative: false }, visible: ko.observable(true) });

    this.ExigirSenhaForte = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Pessoas.PoliticaSenha.SenhaDeveConter, issue: 649, def: false, visible: ko.observable(true) });
    this.HabilitarCriptografia = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Pessoas.PoliticaSenha.SenhaDeveSerArmazenadaCriptografada, issue: 650, def: false, visible: ko.observable(true) });
    this.NaoPermitirAcessosSimultaneos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), text: Localization.Resources.Pessoas.PoliticaSenha.NaoPermitirAcessosSimultaneos, issue: 904, def: false, visible: ko.observable(true) });
    this.LiberarAcessoMinutos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "", def: ko.observable(false) });
    this.TempoEmMinutosBloqueioUsuario = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.PoliticaSenha.LiberarAcessoApos.getFieldDescription()), issue: 905, getType: typesKnockout.int, required: false, maxlength: 2, enable: ko.observable(false) });

    this.QuantasSenhasAnterioresNaoRepetir = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.PoliticaSenha.NaoRepetirUltimasSenhas.getFieldDescription()), issue: 905, getType: typesKnockout.int, required: false, maxlength: 2, enable: ko.observable(true) });
    this.InativarUsuarioAposDiasSemAcessarSistema = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.PoliticaSenha.InativarUsuarioAposDiasSemAcessarSistema.getFieldDescription()), getType: typesKnockout.int, required: false, maxlength: 2, enable: ko.observable(true) });
    this.TipoServico = PropertyEntity({ getType: typesKnockout.int, val: ko.observable() });

    PreencherObjetoKnout(this, { Data: data });
}

//*******EVENTOS*******

function loadPoliticaSenha() {
    _politicasSenhas = new ContainerPoliticasSenha();
    KoBindings(_politicasSenhas, "knockoutConfigPoliticaSenha");
    HeaderAuditoria("PoliticaSenha", _politicasSenhas);

    BuscarPorCodigo(_politicasSenhas, "PoliticaSenha/BuscarPoliticaSenha", function (arg) {
        $("#knockoutConfigPoliticaSenha").show();
        if (arg.Success) {
            if (arg.Data !== false) {
                $.each(arg.Data.PoliticasSenha, function (i, politicaSenha) {
                    AdicionarPolitica(politicaSenha);
                });
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.PoliticaSenha.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.PoliticaSenha.Falha, arg.Msg);
        }
        
        //let triggerEl = document.querySelector('#tabsPoliticaSenha a:first-of-type');
        //let firstTab = new bootstrap.Tab(triggerEl);
        //firstTab.show();
    });
}


function AdicionarPolitica(politica) {
    var ko_containerPolitica = new PoliticaSenha(politica);
    
    if (politica.TempoEmMinutosBloqueioUsuario != "") {
        ko_containerPolitica.LiberarAcessoMinutos.val(true);
    };

    if (politica.HabilitarPoliticaSenha) {
        ko_containerPolitica.HabilitarPoliticaSenha.enable(false)
    }

    $("#" + ko_containerPolitica.LiberarAcessoMinutos.id).click(verificarLiberarAcessoMinutos(ko_containerPolitica));
    _politicasSenhas.PoliticasSenha.push(ko_containerPolitica);
}

var ContainerPoliticasSenha = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PoliticasSenha = ko.observableArray();
    this.ListaPoliticaSalvar = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic });
    this.AdicionarPoliticaSenha = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pessoas.PoliticaSenha.Salvar});
    this.TipoServicoMultiSoftwareNome = PropertyEntity({ val: ko.observable("") });
    this.TipoServicoMultiSoftwareVisible = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
};

function verificarLiberarAcessoMinutos(politica) {
    if (politica.LiberarAcessoMinutos.val()) {
        politica.TempoEmMinutosBloqueioUsuario.enable(true);
        politica.TempoEmMinutosBloqueioUsuario.required = true;
        politica.TempoEmMinutosBloqueioUsuario.text(Localization.Resources.Pessoas.PoliticaSenha.LiberarAcessoApos.getRequiredFieldDescription());
    } else {
        politica.TempoEmMinutosBloqueioUsuario.val("");
        politica.TempoEmMinutosBloqueioUsuario.enable(false);
        politica.TempoEmMinutosBloqueioUsuario.required = false;
        politica.TempoEmMinutosBloqueioUsuario.text(Localization.Resources.Pessoas.PoliticaSenha.LiberarAcessoApos.getFieldDescription());
    }
}

function preencherObjetoSalvar() {
    var ListaPoliticaSalvar = new Array();
    $.each(_politicasSenhas.PoliticasSenha(), function (i, politicaSenha) {
        ListaPoliticaSalvar.push(RetornarObjetoPesquisa(politicaSenha));
    });

    console.log(ListaPoliticaSalvar);
    _politicasSenhas.ListaPoliticaSalvar.val(JSON.stringify(ListaPoliticaSalvar));
}

function atualizarClick(e, sender) {
    preencherObjetoSalvar();
    Salvar(_politicasSenhas, "PoliticaSenha/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.PoliticaSenha.Sucesso, Localization.Resources.Pessoas.PoliticaSenha.AtualizadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.PoliticaSenha.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.PoliticaSenha.Falha, arg.Msg);
        }
    }, sender);
}
