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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="CentroDescarregamento.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridEmail;
var _email;

var Email = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Email = PropertyEntity({ text: Localization.Resources.Logistica.CentroDescarregamento.Email.getRequiredFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true), required: true, maxlength: 150 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarEmailClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadEmail() {

    _email = new Email();
    KoBindings(_email, "knockoutEmail");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: ExcluirEmailClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "Email", title: Localization.Resources.Logistica.CentroDescarregamento.Email, width: "90%" }];

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

    $.each(_centroDescarregamento.Emails.list, function (i, email) {
        var emailGrid = new Object();

        emailGrid.Codigo = email.Codigo.val;
        emailGrid.Email = email.Email.val;

        data.push(emailGrid);
    });

    _gridEmail.CarregarGrid(data);
}


function ExcluirEmailClick(data) {
    for (var i = 0; i < _centroDescarregamento.Emails.list.length; i++) {
        if (data.Codigo == _centroDescarregamento.Emails.list[i].Codigo.val) {
            _centroDescarregamento.Emails.list.splice(i, 1);
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
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.CentroDescarregamento.EmailInvalido, Localization.Resources.Logistica.CentroDescarregamento.informeEmailValido);
        return;
    }

    for (var i = 0; i < _centroDescarregamento.Emails.list.length; i++) {
        if (_centroDescarregamento.Emails.list[i].Email.val == _email.Email.val()) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.CentroDescarregamento.EmailJaExiste, Localization.Resources.Logistica.CentroDescarregamento.EsteEmailJaFoiCadastrado);
            return;
        }
    }

    _email.Codigo.val(guid());
    _centroDescarregamento.Emails.list.push(SalvarListEntity(_email));

    RecarregarGridEmail();
    LimparCamposEmail();
}

function LimparCamposEmail() {
    LimparCampos(_email);
}