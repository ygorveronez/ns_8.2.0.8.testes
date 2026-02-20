/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />

var _grupoPessoasConfigEmail;
var _gridEmailsDocumentos;
var arrayEmailDocumentos = new Array();

var GrupoPessoasConfigEmail = function () {
    this.CodigoEmailDocumento = PropertyEntity({ type: types.string });
    this.Emails = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EmailsSeparados.getRequiredFieldDescription(), required: true, visible: true, maxlength: 500, enable: ko.observable(true) });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.ModeloDocumento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarEmailDocumentoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirEmailDocumentoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarEmailDocumentoClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.EmailsDocumentos = PropertyEntity({ type: types.local });
}

function loadGrupoPessoasConfigEmail() {
    _grupoPessoasConfigEmail = new GrupoPessoasConfigEmail();
    KoBindings(_grupoPessoasConfigEmail, "knockoutConfigEmails");

    loadGridEmailsDocumentos();

    new BuscarModeloDocumentoFiscal(_grupoPessoasConfigEmail.ModeloDocumento);
}

function loadGridEmailsDocumentos() {
    var linhasPorPagina = 2;
    var opcaoEditar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: EditarEmailDocumentoClick, icone: "", visiblidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoModeloDocumento", visible: false },
        { data: "ModeloDocumentoFiscal", title: Localization.Resources.Pessoas.GrupoPessoas.ModeloDocumentoFiscal, width: "60%", className: "text-align-left" },
        { data: "Emails", title: Localization.Resources.Pessoas.GrupoPessoas.Emails, width: "40%", className: "text-align-left" }
    ];
    _gridEmailsDocumentos = new BasicDataTable(_grupoPessoasConfigEmail.EmailsDocumentos.id, header, menuOpcoes, null, null, linhasPorPagina);
    _gridEmailsDocumentos.CarregarGrid([]);
}

function AdicionarEmailDocumentoClick() {
    if (!ValidarCampoObrigatorioEntity(_grupoPessoasConfigEmail.ModeloDocumento) || !ValidarCamposObrigatorios(_grupoPessoasConfigEmail)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    if (!ValidarMultiplosEmails(_grupoPessoasConfigEmail.Emails.val())) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.NaoFoiPossivelValidarEmails);
        return;
    }

    if (!verificarDocumentoTabela()) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TabelaJaPossuiRegistroDocumento);
        return;
    }

    let emailDocumento = {
        Codigo: guid(),
        CodigoModeloDocumento: _grupoPessoasConfigEmail.ModeloDocumento.codEntity(),
        ModeloDocumentoFiscal: _grupoPessoasConfigEmail.ModeloDocumento.val(),
        Emails: _grupoPessoasConfigEmail.Emails.val()
    }
    arrayEmailDocumentos.push(emailDocumento);
    LimparCamposConfigEmails();
}

function AtualizarEmailDocumentoClick() {
    if (!ValidarCampoObrigatorioEntity(_grupoPessoasConfigEmail.ModeloDocumento) || !ValidarCamposObrigatorios(_grupoPessoasConfigEmail)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    if (!verificarDocumentoTabela(true)) {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.TabelaJaPossuiRegistroDocumento);
        return;
    }

    for (let i = 0; i < arrayEmailDocumentos.length; i++) {
        if (arrayEmailDocumentos[i].Codigo == _grupoPessoasConfigEmail.CodigoEmailDocumento.val()) {
            arrayEmailDocumentos[i].ModeloDocumentoFiscal = _grupoPessoasConfigEmail.ModeloDocumento.val();
            arrayEmailDocumentos[i].Emails = _grupoPessoasConfigEmail.Emails.val();
            arrayEmailDocumentos[i].CodigoModeloDocumento = _grupoPessoasConfigEmail.ModeloDocumento.codEntity();
            break;
        }
    }
    LimparCamposConfigEmails();
    esconderBotoesDocEmail();
}

function ExcluirEmailDocumentoClick() {
    for (let i = 0; i < arrayEmailDocumentos.length; i++) {
        if (arrayEmailDocumentos[i].Codigo == _grupoPessoasConfigEmail.CodigoEmailDocumento.val()) {
            arrayEmailDocumentos.splice(i, 1);
            break;
        }
    }
    LimparCamposConfigEmails();
    esconderBotoesDocEmail();
}

function EditarEmailDocumentoClick(registroSelecionado) {
    mostrarBotoesDocEmail();
    _grupoPessoasConfigEmail.CodigoEmailDocumento.val(registroSelecionado.Codigo);
    _grupoPessoasConfigEmail.ModeloDocumento.val(registroSelecionado.ModeloDocumentoFiscal);
    _grupoPessoasConfigEmail.Emails.val(registroSelecionado.Emails);
    _grupoPessoasConfigEmail.ModeloDocumento.codEntity(registroSelecionado.CodigoModeloDocumento);
}

function LimparCamposConfigEmails() {
    _gridEmailsDocumentos.CarregarGrid(arrayEmailDocumentos);
    _grupoPessoasConfigEmail.ModeloDocumento.val("");
    _grupoPessoasConfigEmail.Emails.val("");
}

function RecarregarGridEmailDocumentos() {
    _gridEmailsDocumentos.CarregarGrid(arrayEmailDocumentos);
}

function mostrarBotoesDocEmail() {
    _grupoPessoasConfigEmail.Adicionar.visible(false);
    _grupoPessoasConfigEmail.Atualizar.visible(true);
    _grupoPessoasConfigEmail.Excluir.visible(true);
}

function esconderBotoesDocEmail() {
    _grupoPessoasConfigEmail.Adicionar.visible(true);
    _grupoPessoasConfigEmail.Atualizar.visible(false);
    _grupoPessoasConfigEmail.Excluir.visible(false);
}

function verificarDocumentoTabela(atualizar = false) {
    if (atualizar) {
        for (let i = 0; i < arrayEmailDocumentos.length; i++) {
            if (arrayEmailDocumentos[i].Codigo != _grupoPessoasConfigEmail.CodigoEmailDocumento.val()) {
                if (arrayEmailDocumentos[i].ModeloDocumentoFiscal == _grupoPessoasConfigEmail.ModeloDocumento.val()
                    || arrayEmailDocumentos[i].CodigoModeloDocumento == _grupoPessoasConfigEmail.ModeloDocumento.codEntity())
                    return false;
            }
        }
        return true;
    }

    for (let i = 0; i < arrayEmailDocumentos.length; i++) {
        if (arrayEmailDocumentos[i].ModeloDocumentoFiscal == _grupoPessoasConfigEmail.ModeloDocumento.val()
            || arrayEmailDocumentos[i].CodigoModeloDocumento == _grupoPessoasConfigEmail.ModeloDocumento.codEntity())
            return false;
    }
    return true;
}