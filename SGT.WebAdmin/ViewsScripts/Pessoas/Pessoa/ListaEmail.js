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
/// <reference path="Pessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridListaEmail;
var _listaEmail;

var ListaEmail = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Email = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Email.getRequiredFieldDescription(), required: false, getType: typesKnockout.email, maxlength: 1000 });
    this.EnviarEmail = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: Localization.Resources.Pessoas.Pessoa.XML });

    this.TipoEmail = PropertyEntity({ val: ko.observable(EnumTipoEmail.Principal), options: EnumTipoEmail.obterOpcoes(), def: EnumTipoEmail.Principal, text: Localization.Resources.Pessoas.Pessoa.TipoEmail.getRequiredFieldDescription(), required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarListaEmailClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};


//*******EVENTOS*******

function loadListaEmail() {
    $("#liTabListaEmail").show();
    _listaEmail = new ListaEmail();
    KoBindings(_listaEmail, "knockoutListaEmail");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirListaEmailClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "EnviarEmail", visible: false },
        { data: "Email", title: Localization.Resources.Pessoas.Pessoa.Email, width: "70%" },
        { data: "TipoEmail", visible: false },
        { data: "DescricaoTipo", title: Localization.Resources.Pessoas.Pessoa.Tipo, width: "10%" }
    ];

    _gridListaEmail = new BasicDataTable(_listaEmail.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridListaEmail();
}

function recarregarGridListaEmail() {

    var data = new Array();

    $.each(_pessoa.ListaEmail.list, function (i, listaEmail) {
        var listaEmailGrid = new Object();

        listaEmailGrid.Codigo = 0;
        listaEmailGrid.Email = listaEmail.Email.val;
        listaEmailGrid.EnviarEmail = listaEmail.EnviarEmail.val;
        listaEmailGrid.TipoEmail = listaEmail.TipoEmail.val;

        if (listaEmail.TipoEmail.val == 99)
            listaEmailGrid.DescricaoTipo = EnumTipoEmail.obterDescricao(EnumTipoEmail.SAC);
        else
            listaEmailGrid.DescricaoTipo = EnumTipoEmail.obterDescricao(listaEmail.TipoEmail.val);

        data.push(listaEmailGrid);
    });

    _gridListaEmail.CarregarGrid(data);
}


function excluirListaEmailClick(data) {
    $.each(_pessoa.ListaEmail.list, function (i, listaEmail) {
        if (data.Email == listaEmail.Email.val) {
            _pessoa.ListaEmail.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridListaEmail();
}

function adicionarListaEmailClick(e, sender) {
    var valido = true;
    var vemailValido = true;

    valido = _listaEmail.Email.val() != "";
    _listaEmail.Email.requiredClass("form-control");

    if (!ValidarEmail(_listaEmail.Email.val())) {
        valido = false;
        vemailValido = false;
    }

    if (valido) {
        var existe = false;
        $.each(_pessoa.ListaEmail.list, function (i, listaEmail) {
            if (listaEmail.Email.val == _listaEmail.Email.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Pessoa.EmailJaExistente, Localization.Resources.Pessoas.Pessoa.EmailJaEstaCadastrado.format(_listaEmail.Email.val()));
            return;
        }
        _pessoa.ListaEmail.list.push(SalvarListEntity(_listaEmail));
        recarregarGridListaEmail();
        $("#" + _listaEmail.Email.id).focus();
        limparCamposListaEmail();
    } else {
        if (!vemailValido)
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorioss, Localization.Resources.Pessoas.Pessoa.InformeUmEmailValido);
        else
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        _listaEmail.Email.requiredClass("form-control is-invalid");
    }
}

function limparCamposListaEmail() {
    LimparCampos(_listaEmail);
    _listaEmail.Email.requiredClass("form-control");
}