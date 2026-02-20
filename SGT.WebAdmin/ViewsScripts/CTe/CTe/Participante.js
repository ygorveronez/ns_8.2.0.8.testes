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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Atividade.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="CTe.js" />

var Participante = function (cte, idKnockout) {

    var instancia = this;

    this.PessoaExterior = PropertyEntity({ text: Localization.Resources.CTes.CTe.CPFCNPJ.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true) });
    this.CPFCNPJ = PropertyEntity({ text: Localization.Resources.CTes.CTe.CPFCNPJ.getRequiredFieldDescription(), getType: typesKnockout.cpfCnpj, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.IE = PropertyEntity({ text: Localization.Resources.CTes.CTe.IE.getRequiredFieldDescription(), maxlength: 15, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.RazaoSocial = PropertyEntity({ text: Localization.Resources.CTes.CTe.RazaoSocial.getRequiredFieldDescription(), maxlength: 80, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.NomeFantasia = PropertyEntity({ text: Localization.Resources.CTes.CTe.NomeFantasia.getFieldDescription(), maxlength: 150, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TelefonePrincipal = PropertyEntity({ getType: typesKnockout.phone, text: Localization.Resources.CTes.CTe.TelefonePrincipal.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.TelefoneSecundario = PropertyEntity({ getType: typesKnockout.phone, text: Localization.Resources.CTes.CTe.TelefoneSecundario.getFieldDescription(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.CTes.CTe.Atividade.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(true), issue: 47, visible: ko.observable(true), enable: ko.observable(true) });
    this.CEP = PropertyEntity({ getType: typesKnockout.cep, text: Localization.Resources.CTes.CTe.CEP.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: Localization.Resources.CTes.CTe.EnderecoPrincipal.getRequiredFieldDescription(), required: ko.observable(true), maxlength: 80, visible: ko.observable(true), enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.CTes.CTe.Numero.getRequiredFieldDescription(), required: ko.observable(true), maxlength: 60, visible: ko.observable(true), enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: Localization.Resources.CTes.CTe.Bairro.getRequiredFieldDescription(), required: ko.observable(true), maxlength: 40, visible: ko.observable(true), enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: Localization.Resources.CTes.CTe.Complemento.getFieldDescription(), required: ko.observable(false), maxlength: 60, visible: ko.observable(true), enable: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: Localization.Resources.CTes.CTe.Cidade.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LocalidadeExterior = PropertyEntity({ text: Localization.Resources.CTes.CTe.Cidade.getRequiredFieldDescription(), required: ko.observable(false), maxlength: 60, visible: ko.observable(false), enable: ko.observable(true) });
    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.CTes.CTe.Pais.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.EmailGeral = PropertyEntity({ text: Localization.Resources.CTes.CTe.EmailGeral.getFieldDescription(), required: ko.observable(false), maxlength: 1000, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarXMLEmailGeral = PropertyEntity({ text: Localization.Resources.CTes.CTe.XML, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailContato = PropertyEntity({ text: Localization.Resources.CTes.CTe.EmailContato.getFieldDescription(), required: false, maxlength: 1000, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarXMLEmailContato = PropertyEntity({ text: Localization.Resources.CTes.CTe.XML, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.EmailContador = PropertyEntity({ text: Localization.Resources.CTes.CTe.EmailContador.getFieldDescription(), required: false, maxlength: 1000, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarXMLEmailContador = PropertyEntity({ text: Localization.Resources.CTes.CTe.XML, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ParticipanteExterior = PropertyEntity({ text: Localization.Resources.CTes.CTe.ParticipanteDoExterior, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.SalvarEndereco = PropertyEntity({ text: Localization.Resources.CTes.CTe.SalvarEnderecoNoCadastroDoParticipante, val: ko.observable(true), def: true, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, idBtnSearch: guid(), visible: false, enable: ko.observable(true), callback: null });

    this.ParticipanteExterior.val.subscribe(function (exterior) {
        instancia.AlterarEstadoParticipanteExterior(exterior);
    });

    this.AtivarPermissoesEspecificasParticipante = function (emissaoCTe) {
        if (emissaoCTe.VerificarSePossuiPermissao(EnumPermissoesEdicaoCTe.AlterarIEParticipanete))
            instancia.IE.enable(true);
    };

    this.DestivarParticipante = function () {
        DesabilitarCamposInstanciasCTe(instancia);
    };

    this.AlterarEstadoParticipanteExterior = function (exterior) {

        if (exterior) {
            SetarEnableCamposKnockout(instancia, false);
            instancia.ParticipanteExterior.enable(true);
            instancia.PessoaExterior.enable(true);

            instancia.IE.required(false);
            instancia.NomeFantasia.required(false);
            instancia.TelefonePrincipal.required(false);
            instancia.Atividade.required(false);
            instancia.CEP.required(false);

            instancia.PessoaExterior.visible(true);
            instancia.PessoaExterior.required(true);
            instancia.CPFCNPJ.visible(false);
            instancia.CPFCNPJ.required(false);
            instancia.Localidade.visible(false);
            instancia.Localidade.required(false);
            instancia.LocalidadeExterior.visible(true);
            instancia.LocalidadeExterior.required(true);
            instancia.Pais.visible(true);
            instancia.Pais.required(true);
        } else {
            SetarEnableCamposKnockout(instancia, true);

            instancia.IE.required(true);
            instancia.NomeFantasia.required(true);
            instancia.TelefonePrincipal.required(true);
            instancia.Atividade.required(true);
            instancia.CEP.required(true);

            instancia.PessoaExterior.visible(false);
            instancia.PessoaExterior.required(false);
            instancia.CPFCNPJ.visible(true);
            instancia.CPFCNPJ.required(true);
            instancia.Localidade.visible(true);
            instancia.Localidade.required(true);
            instancia.LocalidadeExterior.visible(false);
            instancia.LocalidadeExterior.required(false);
            instancia.Pais.visible(false);
            instancia.Pais.required(false);
        }

        if (instancia.NaoLimparDados == null || instancia.NaoLimparDados == false) {
            LimparCampo(instancia.PessoaExterior);
            instancia.CPFCNPJ.val("");
            instancia.IE.val("");
            instancia.NomeFantasia.val("");
            instancia.TelefonePrincipal.val("");
            instancia.TelefoneSecundario.val("");
            LimparCampo(instancia.Atividade);
            instancia.CEP.val("");
            instancia.EnviarXMLEmailGeral.val(false);
            instancia.EmailContato.val("");
            instancia.EnviarXMLEmailContato.val(false);
            instancia.EmailContador.val("");
            instancia.EnviarXMLEmailContador.val(false);
            instancia.SalvarEndereco.val(true);
            LimparCampo(instancia.Localidade);
            LimparCampo(instancia.Pais);
            instancia.LocalidadeExterior.val("");
            instancia.RazaoSocial.val("");
            instancia.Endereco.val("");
            instancia.Numero.val("");
            instancia.Bairro.val("");
            instancia.Complemento.val("");
            instancia.EmailGeral.val("");
        }
    };

    this.Limpar = function () {
        LimparCampos(instancia);
    };

    this.Validar = function (validarSemValor) {
        var validar = true;

        if (!validarSemValor) {
            if (instancia.ParticipanteExterior.val() == false) {
                if (instancia.CPFCNPJ.val() == "")
                    validar = false;
            } else {
                if (instancia.RazaoSocial.val() == "")
                    validar = false;
            }
        }
        var valido = true;

        if (validar) {
            valido = ValidarCamposObrigatorios(instancia);

            if (!valido) {
                $('a[href="#' + idKnockout.replace("knockout", "div") + '"]').tab("show");
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.CTes.CTe.VerifiqueOsCamposObrigatorios);
            }
        }

        return valido;
    };

    this.Load = function () {

        KoBindings(instancia, idKnockout);

        new BuscarLocalidadesBrasil(instancia.Localidade);
        new BuscarAtividades(instancia.Atividade);
        new BuscarPaises(instancia.Pais);

        new BuscarClientes(instancia.PessoaExterior, function (retorno) {
            instancia.PessoaExterior.codEntity(retorno.Codigo);
            instancia.PessoaExterior.val(retorno.CPF_CNPJ);
            instancia.ObterDadosPorCPFCNPJ(0, retorno.Codigo);
        }, null, null, null, null, null, null, null, "E");

        new BuscarClientes(instancia.Cliente, function (retorno) {
            instancia.CPFCNPJ.val(retorno.CPF_CNPJ);
            instancia.BuscarDadosPorCPFCNPJ();
        });

        $("#" + instancia.CPFCNPJ.id).focusout(function () {
            instancia.BuscarDadosPorCPFCNPJ();
        });
    };

    this.BuscarDadosPorCPFCNPJ = function () {
        var cpfCnpj = (instancia.CPFCNPJ.val() || '').replace(/[^0-9]+/g, '');

        if (cpfCnpj.trim() == "") {
            instancia.Limpar();
            return;
        }

        if (cpfCnpj.length != 11 && cpfCnpj.length != 14) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CPFCNPJInvalido, Localization.Resources.CTes.CTe.CPFCNPJInformadoInvalido);
            instancia.Limpar();
            return;
        }

        if (cpfCnpj.length == 11 && !ValidarCPF(cpfCnpj, true)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CPFInvalido, Localization.Resources.CTes.CTe.CPFInformadoInvalido);
            instancia.Limpar();
            return;
        } else if (cpfCnpj.length == 14 && !ValidarCNPJ(cpfCnpj, true)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.CNPJInvalido, Localization.Resources.CTes.CTe.CNPJInformadoInvalido);
            instancia.Limpar();
            return;
        }

        instancia.ObterDadosPorCPFCNPJ(cpfCnpj, 0);
    };

    this.ObterDadosPorCPFCNPJ = function (cpfCnpj, codigo) {
        executarReST("Cliente/ObterDetalhesPorCPFCNPJ", { CPF_CNPJ: cpfCnpj, Codigo: codigo }, function (r) {
            if (r.Success) {
                if (r.Data != null) {
                    instancia.Participante = r.Data;

                    PreencherObjetoKnout(instancia, r);

                    if (codigo > 0 && instancia.Pais.codEntity() == 0)
                        instancia.Pais.enable(true);
                    else
                        instancia.Pais.enable(false);

                    if (instancia.Cliente.callback != null)
                        instancia.Cliente.callback(r.Data);

                    cte.Seguro.ObterSeguroTomador();

                    Global.setarFocoProximoCampo(instancia.CPFCNPJ.id);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.CTes.CTe.PessoaNaoEncontrada, Localization.Resources.CTes.CTe.CPFCNPJInformadoNaoEstaCadastradoInformeOsDadosMesmoSeraCadastradoQuandoCTeForSalvoOuEmitido);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };
};