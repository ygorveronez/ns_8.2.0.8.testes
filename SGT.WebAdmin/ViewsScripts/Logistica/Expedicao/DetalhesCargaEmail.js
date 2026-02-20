/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Empresa.js" />
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
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDetalhesCargaEmail;
var _detalhesCargaEmail;
var _emailsEnvioDetalhesCarga;

var DetalhesCargaEmail = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Email = PropertyEntity({ text: "*E-mail:", getType: typesKnockout.email, val: ko.observable(""), def: "", required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", maxlength: 2000 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarEmailEnvioDetalhesCargaClick, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
    this.Enviar = PropertyEntity({ eventClick: enviarEmailEnvioDetalhesCargaClick, type: types.event, text: "Enviar", idGrid: guid(), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadEnvioDetalhesCargaEmail() {

    _detalhesCargaEmail = new DetalhesCargaEmail();
    KoBindings(_detalhesCargaEmail, "divModalEnvioDetalhesCargaEmail");

    _emailsEnvioDetalhesCarga = new Array();

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirEmailEnvioDetalhesCargaClick }] };

    var header = [{ data: "Codigo", visible: false },
                  { data: "Email", title: "E-mail", width: "90%" }];

    _gridDetalhesCargaEmail = new BasicDataTable(_detalhesCargaEmail.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    recarregarGridDetalhesCargaEmail();

    $("#" + _detalhesCargaEmail.Email.id).keyup(function (event) {
        if (event.keyCode == 13) {
            adicionarEmailEnvioDetalhesCargaClick();
        }
        event.preventDefault();
    });

}

//*******MÉTODOS*******

function recarregarGridDetalhesCargaEmail() {
    _gridDetalhesCargaEmail.CarregarGrid(_emailsEnvioDetalhesCarga);
}

function excluirEmailEnvioDetalhesCargaClick(data) {
    for (var i = 0; i < _emailsEnvioDetalhesCarga.length; i++) {
        if (data.Codigo == _emailsEnvioDetalhesCarga[i].Codigo) {
            _emailsEnvioDetalhesCarga.splice(i, 1);
            break;
        }
    }

    recarregarGridDetalhesCargaEmail();
}

function adicionarEmailEnvioDetalhesCargaClick(e, sender) {
    if (!ValidarCamposObrigatorios(_detalhesCargaEmail)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    if (!ValidarEmail(_detalhesCargaEmail.Email.val())) {
        exibirMensagem(tipoMensagem.atencao, "E-mail Inválido", "Informe um e-mail válido!");
        return;
    }

    for (var i = 0; i < _emailsEnvioDetalhesCarga.length; i++) {
        if (_detalhesCargaEmail.Email.val() == _emailsEnvioDetalhesCarga[i].Email) {
            exibirMensagem(tipoMensagem.atencao, "E-mail já existe", "O e-mail " + _emailsEnvioDetalhesCarga[i].Email + " já foi adicionado.");
            return;
        }
    }

    _emailsEnvioDetalhesCarga.push({ Codigo: guid(), Email: _detalhesCargaEmail.Email.val() });

    recarregarGridDetalhesCargaEmail();

    limparCamposDetalhesCargaEmail();
}

function enviarEmailEnvioDetalhesCargaClick(e, sender) {
    if (_emailsEnvioDetalhesCarga.length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Adicione um e-mail para realizar o envio!");
        return;
    }

    executarReST("Expedicao/EnviarDetalhesCargaPorEmail", { Codigo: _detalhesCargaEmail.Codigo.val(), Emails: JSON.stringify(_emailsEnvioDetalhesCarga), Observacao: _detalhesCargaEmail.Observacao.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal('divModalEnvioDetalhesCargaEmail');
                exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail(s) enviado(s) com sucesso.");
                limparCamposDetalhesCargaEmail();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function abrirTelaEnvioDetalhesCargaEmailClick(e) {
    limparCamposDetalhesCargaEmail();
    _detalhesCargaEmail.Codigo.val(e.Codigo);
    _detalhesCargaEmail.Observacao.val("");
    Global.abrirModal("divModalEnvioDetalhesCargaEmail");
}

function limparCamposDetalhesCargaEmail() {
    _detalhesCargaEmail.Email.val("");
}