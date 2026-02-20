/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Consultas/Licenca.js" />

var _registro = null;

var Registro = function () {
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Juridica), eventChange: tipoPessoaChange, options: EnumTipoPessoa.obterOpcoes(), text: "*Tipo de pessoa: ", def: EnumTipoPessoa.Juridica, enable: ko.observable(true) });
    this.CNPJ = PropertyEntity({ text: ko.observable("*CNPJ: "), required: true, getType: typesKnockout.cnpj, enable: ko.observable(true), visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable("*CPF: "), required: false, getType: typesKnockout.cpf, maxlength: 14, enable: ko.observable(true), visible: ko.observable(false) });
    this.IE = PropertyEntity({ text: ko.observable("*IE: "), required: true, visible: ko.observable(true), maxlength: 14, enable: ko.observable(true) });
    this.RG = PropertyEntity({ text: ko.observable("RG: "), required: false, visible: ko.observable(true), maxlength: 14, enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: ko.observable("*Razão Social: "), required: true, maxlength: 80, enable: ko.observable(true) });
    this.Fantasia = PropertyEntity({ text: "*Fantasia: ", required: false, visible: ko.observable(true), maxlength: 150, enable: ko.observable(true) });
    this.TelefonePrincipal = PropertyEntity({ text: ko.observable("*Telefone Principal: "), required: true, getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: ko.observable("*E-mail:"), getType: typesKnockout.multiplesEmails, maxlength: 1000, enable: ko.observable(true) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Atividade:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    // Dados do endereço
    this.CEP = PropertyEntity({ text: ko.observable("*CEP: "), required: true, enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: ko.observable("*Endereço Principal: "), required: true, maxlength: 80, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable("*Bairro: "), required: true, maxlength: 40, enable: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Cidade:"), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: ko.observable("Complemento: "), required: false, maxlength: 60, enable: ko.observable(true) });
    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: "*Tipo Log.: ", issue: 18, required: true, enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: ko.observable("*Número: "), required: true, maxlength: 60, enable: ko.observable(true) });
    this.TipoEndereco = PropertyEntity({ val: ko.observable(EnumTipoEndereco.Comercial), options: EnumTipoEndereco.obterOpcoes(), def: EnumTipoEndereco.Comercial, text: "*Tipo End.: ", issue: 17, required: true, enable: ko.observable(true) });

    //Senha
    this.Senha = PropertyEntity({ text: ko.observable("*Senha: "), required: true, maxlength: 80, enable: ko.observable(true), visible: ko.observable(true) });
    this.ConfirmarSenha = PropertyEntity({ text: ko.observable("*Confirmar Senha: "), required: true, maxlength: 80, enable: ko.observable(true), visible: ko.observable(true) });

    // Ação
    this.Continuar = PropertyEntity({ eventClick: continuar, visible: ko.observable(true) });
    this.Registrar = PropertyEntity({ eventClick: registrar, visible: ko.observable(false) });
    this.TentarNovamente = PropertyEntity({ eventClick: tentarNovamente, visible: ko.observable(false) });
};

function LoadRegistro() {
    _registro = new Registro();
    KoBindings(_registro, "knockoutRegistro");

    new BuscarLocalidades(_registro.Localidade);
    new BuscarAtividades(_registro.Atividade);

    prepararFormPessoaFisica();
}

function tipoPessoaChange(e, sender) {
    if (_registro.TipoPessoa.val() == EnumTipoPessoa.Juridica) {
        prepararFormPessoaJuridica();
    } else if (_registro.TipoPessoa.val() == EnumTipoPessoa.Fisica) {
        prepararFormPessoaFisica();
    }
}

function prepararFormPessoaFisica() {
    _registro.TipoPessoa.val(EnumTipoPessoa.Fisica);
    _registro.CPF.getType = typesKnockout.cpf;
    _registro.CNPJ.getType = typesKnockout.string;

    _registro.Nome.text("*Nome: ");
    _registro.CNPJ.visible(false);
    _registro.CNPJ.required = false;

    _registro.CPF.visible(true);
    _registro.CPF.required = true;

    _registro.IE.visible(false);
    _registro.IE.required = false;

    _registro.RG.visible(true);

    _registro.Fantasia.visible(false);
    _registro.Fantasia.required = false;
}

function prepararFormPessoaJuridica() {
    _registro.TipoPessoa.val(EnumTipoPessoa.Juridica);
    _registro.CPF.getType = typesKnockout.string;
    _registro.CNPJ.getType = typesKnockout.cnpj;

    _registro.Nome.text("*Razão Social: ");
    _registro.CNPJ.visible(true);
    _registro.CNPJ.required = true;

    _registro.CPF.visible(false);
    _registro.CPF.required = false;

    _registro.IE.visible(true);
    _registro.IE.required = true;

    _registro.RG.visible(false);

    _registro.Fantasia.visible(true);
    _registro.Fantasia.required = true;
}

function prepararFormApenasSenha() {
    _registro.TipoPessoa.required = false;
    _registro.CNPJ.required = false;
    _registro.CPF.required = false;
    _registro.IE.required = false;
    _registro.RG.required = false;
    _registro.Nome.required = false;
    _registro.Fantasia.required = false;
    _registro.TelefonePrincipal.required = false;
    _registro.Email.required = false;
    _registro.Atividade.required = false;
    _registro.CEP.required = false;
    _registro.Endereco.required = false;
    _registro.Bairro.required = false;
    _registro.Localidade.required = false;
    _registro.Complemento.required = false;
    _registro.TipoLogradouro.required = false;
    _registro.Numero.required = false;
    _registro.TipoEndereco.required = false;
}

function continuar() {
    let cpfCnpj = _registro.TipoPessoa.val() == EnumTipoPessoa.Fisica ? _registro.CPF.val() : _registro.CNPJ.val();

    executarReST("Registro/BuscarUsuarioJaExiste", { CPF_CNPJ: cpfCnpj }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.JaTemAcesso) {
                    // Já tem acesso, então ele deve logar ou usar o Esqueci minha senha
                    mostrarMensagemJaTemAcesso();
                } else if (arg.Data.JaExiste && !arg.Data.JaTemAcesso) {
                    // Já tem conta, mas sem acesso. Apenas por a senha
                    mostrarMensagemJaTemConta();
                    prepararFormApenasSenha();
                } else {
                    // Não tem conta, continuar o registro
                    liberarRegistroCompleto(true);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    });
}

function liberarRegistroCompleto(liberar) {
    if (liberar) {
        $("#registroCompleto").show();
        _registro.Continuar.visible(false);
        _registro.Registrar.visible(true);
        $("#containerSenha").show();
    } else {
        $("#registroCompleto").hide();
        _registro.Continuar.visible(true);
        _registro.Registrar.visible(false);
        $("#containerSenha").hide();
    }
}

function mostrarMensagemJaTemAcesso() {
    $("#mensagemJaTemAcesso").show();
    _registro.Continuar.visible(false);
    _registro.Registrar.visible(false);
}

function mostrarMensagemJaTemConta() {
    $("#mensagemJaTemConta").show();
    $("#containerSenha").show();
    _registro.Senha.visible(true);
    _registro.ConfirmarSenha.visible(true);
    _registro.Registrar.visible(true);
    _registro.Continuar.visible(false);
}

function mostrarMensagemConferirEmail() {
    $("#mensagemConferirEmail").show();
    $('#login-form').hide();
}

function registrar() {
    if (_registro.Senha.val() != _registro.ConfirmarSenha.val()) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A confirmação de senha não coincide com a senha");
        return;
    }

    Salvar(_registro, "Registro/Registrar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro efetuado com sucesso");
                mostrarMensagemConferirEmail();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null, function () {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    });
}

function tentarNovamente() {
    LimparCampos(_registro);
    _registro.TipoPessoa.val(EnumTipoPessoa.Fisica);
    prepararFormPessoaFisica();
    $("#mensagemJaTemAcesso").hide();
    _registro.Continuar.visible(true);
    _registro.Registrar.visible(false);
    liberarRegistroCompleto(false);
}

function preencherTestePessoaFisica() {
    _registro.TipoPessoa.val(EnumTipoPessoa.Fisica);
    _registro.CNPJ.val(null);
    _registro.CPF.val("000.000.000-00");
    _registro.IE.val(null);
    _registro.Nome.val("Renato MK");
    _registro.Fantasia.val(null);
    _registro.TelefonePrincipal.val("(49) 99171-9309");
    _registro.Email.val("renato.konflanz@unochapeco.edu.br");
    _registro.Atividade.val(null);
    _registro.CEP.val("00000000");
    _registro.Endereco.val("Rua dos Anjos, 987");
    _registro.Bairro.val("Paraíso");
    _registro.Localidade.codEntity(2);
    _registro.Complemento.val();
    _registro.TipoLogradouro.val(1);
    _registro.Numero.val(1);
    _registro.TipoEndereco.val(1);
}