/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="CentroCarregamento.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEmail;
var _email;

var Email = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.EnviarNotificacoesPorEmail = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarNotificacoesPorEmail, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarNotificacoesCargasRejeitadasPorEmail = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarNotificacoesCargasRejeitadasPorEmail, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailAlertaLeilaoParaTransportadorOfertado = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailAlertaLeilaoParaTransportadorOfertado, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado, val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Email = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.Email.getRequiredFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), required: true, maxlength: 150 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarEmailClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadEmail() {

    _email = new Email();
    KoBindings(_email, "knockoutEmail");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: ExcluirEmailClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Email", title: Localization.Resources.Logistica.CentroCarregamento.Email, width: "90%" }];

    _gridEmail = new BasicDataTable(_email.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridEmail();

    $("#" + _email.Email.id).keyup(function (event) {
        if (event.keyCode == 13)
            AdicionarEmailClick();
        event.preventDefault();
    });
}

function RecarregarGridEmail() {

    var data = new Array();

    $.each(_centroCarregamento.Emails.list, function (i, email) {
        var emailGrid = new Object();

        emailGrid.Codigo = email.Codigo.val;
        emailGrid.Email = email.Email.val;

        data.push(emailGrid);
    });

    _gridEmail.CarregarGrid(data);
}


function ExcluirEmailClick(data) {
    for (var i = 0; i < _centroCarregamento.Emails.list.length; i++) {
        if (data.Codigo == _centroCarregamento.Emails.list[i].Codigo.val) {
            _centroCarregamento.Emails.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridEmail();
}

function AdicionarEmailClick(e, sender) {
    if (!ValidarCamposObrigatorios(_email)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!ValidarEmail(_email.Email.val())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.CentroCarregamento.EmailInvalido, Localization.Resources.Logistica.CentroCarregamento.InformeUmEmailValido);
        return;
    }

    for (var i = 0; i < _centroCarregamento.Emails.list.length; i++) {
        if (_centroCarregamento.Emails.list[i].Email.val == _email.Email.val()) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroCarregamento.EmailJaExiste, Localization.Resources.Logistica.CentroCarregamento.EsteEmailJafoiCadastrado);
            return;
        }
    }

    _email.Codigo.val(guid());
    _centroCarregamento.Emails.list.push(SalvarListEntity(_email));

    RecarregarGridEmail();
    LimparCamposEmail();
}

function LimparCamposEmail() {
    LimparCampo(_email.Codigo);
    LimparCampo(_email.Email);
}

function PreencherEmail(data) {
    RecarregarGridEmail();
    _email.EnviarNotificacoesPorEmail.val(data.EnviarNotificacoesPorEmail);
    _email.EnviarNotificacoesCargasRejeitadasPorEmail.val(data.EnviarNotificacoesCargasRejeitadasPorEmail);
    _email.EnviarEmailAlertaLeilaoParaTransportadorOfertado.val(data.EnviarEmailAlertaLeilaoParaTransportadorOfertado);
    _email.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente.val(data.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente);
    _email.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado.val(data.EnviarEmailConfirmacaoAgendamentoSomenteQuandoSituacaoAgendamentoForFinalizado);
}

function VerificarEmail() {
    if (_email.EnviarEmailQuandoVencedorNaoForDefinidoAutomaticamente.val() && !_centroCarregamento.Emails.list.length > 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.CentroCarregamento.EmailPrecisaSerCadastrado);
        return false
    }
    return true
}