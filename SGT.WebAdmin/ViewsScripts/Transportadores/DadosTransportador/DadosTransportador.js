/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Enumeradores/EnumTipoInclusaoPedagioBaseCalculoICMS.js" />
/// <reference path="../../Enumeradores/EnumVersaoNFe.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumRegimeEspecial.js" />
/// <reference path="../../Enumeradores/EnumRegimeTributarioCTe.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _crudDadosCadastraisTransportador;
var _dadosCadastraisTransportador;

var _fusoHorarioDadosCadastrais = [
    { text: "(UTC-02:00) Coordinated Universal Time-02", value: "UTC-02" },
    { text: "(UTC-03:00) Brasilia (Horario de Verão)", value: "E. South America Standard Time" },
    { text: "(UTC-03:00) Araguaina (Horario de Verão)", value: "Tocantins Standard Time" },
    { text: "(UTC-03:00) Cayenne, Fortaleza", value: "SA Eastern Standard Time" },
    { text: "(UTC-04:00) Cuiaba (Horario de Verão)", value: "Central Brazilian Standard Time" },
    { text: "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", value: "SA Western Standard Time" },
    { text: "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", value: "SA Pacific Standard Time" }
];

var _statusCharDadosCadastrais = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

/*
 * Declaração das Classes
 */

var DadosCadastraisTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RazaoSocial = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription()), required: true, maxlength: 80 });
    this.NomeFantasia = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NomeFantasia.getRequiredFieldDescription(), required: true });
    this.TipoEmpresa = PropertyEntity({ val: ko.observable("J"), options: ObterTiposEmpresaDadosCadastrais(), text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), def: "J", enable: ko.observable(false), eventChange: tipoEmpresaChangeDadosCadastrais, visible: ko.observable(true) });
    this.InscricaoEstadual = PropertyEntity({ text: ko.observable((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : "*") + Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription()), issue: 744, maxlength: 20, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro) });
    this.InscricaoMunicipal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.InscricaoMunicipal.getFieldDescription(), issue: 750, maxlength: 20, required: false, visible: true });
    this.RegistroANTT = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RNTRC.getRequiredFieldDescription()), issue: 660, maxlength: 8, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro), cssClass: ko.observable("col col-2") });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Cidade.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Endereco.getRequiredFieldDescription(), required: true });
    this.Numero = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Numero.getRequiredFieldDescription(), required: true });
    this.Bairro = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Bairro.getRequiredFieldDescription(), required: true });
    this.Complemento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Complemento.getFieldDescription(), maxlength: 100 });
    this.Telefone = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Telefone.getRequiredFieldDescription(), issue: 749, required: true, getType: typesKnockout.phone });
    this.CEP = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CEP.getRequiredFieldDescription(), maxlength: 10, required: true });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusCharDadosCadastrais, def: "A", text: Localization.Resources.Transportadores.Transportador.Situacao.getRequiredFieldDescription(), issue: 557, required: true });
    this.Contato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.NomeContato.getFieldDescription()), issue: 747 });
    this.TelefoneContato = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TelefoneContato.getFieldDescription(), issue: 748, getType: typesKnockout.phone });
    this.NomeContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NomeContador.getFieldDescription() });
    this.TelefoneContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TelefoneContador.getFieldDescription(), getType: typesKnockout.phone });
    this.CRCContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CRCContador.getFieldDescription(), maxlength: 20 });
    this.CNAE = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CNAE.getFieldDescription(), issue: 746, maxlength: 20, required: false });
    this.Suframa = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Suframa.getFieldDescription(), issue: 742, maxlength: 20 });
    this.FusoHorario = PropertyEntity({ val: ko.observable("E. South America Standard Time"), options: _fusoHorarioDadosCadastrais, def: "E. South America Standard Time", text: Localization.Resources.Transportadores.Transportador.FusoHorario.getRequiredFieldDescription(), issue: 65, required: true });
    this.Setor = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Setor.getFieldDescription(), maxlength: 20 });
    this.OptanteSimplesNacional = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.SimplesNacional, issue: 752 });
    this.OptanteSimplesNacionalComExcessoReceitaBruta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.ExcessoSublimiteReceitaBruta, visible: ko.observable(false) });
    this.RegistroANTT = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RNTRC.getRequiredFieldDescription()), issue: 660, maxlength: 8, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro), cssClass: ko.observable("col col-2") });
    this.Email = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroTransportador || _CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas ? "*" : "") + Localization.Resources.Transportadores.Transportador.Email.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, required: _CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroTransportador, maxlength: 1000 });
    this.EmailAdministrativo = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailAdministrativo.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.EmailContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailContador.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.EnviarEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailAdministrativo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailContador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" })
    this.RegimenTributario = PropertyEntity({ val: ko.observable(EnumRegimenTributacao.NaoSelecionado), options: EnumRegimenTributacao.ObterOpcoes(), text: Localization.Resources.Transportadores.Transportador.RegimenTributario, visible: ko.observable(false) });
    this.RegimeEspecial = PropertyEntity({ val: ko.observable(EnumRegimeEspecial.Nenhum), options: EnumRegimeEspecial.obterOpcoes(), text: Localization.Resources.Transportadores.Transportador.RegimeTributarioEspecial.getFieldDescription(), visible: ko.observable(true) });
};

var DadosCadastraisCRUDTransportador = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClickDadosCadastrais, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClickDadosCadastrais, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function ObterTiposEmpresaDadosCadastrais() {
    _tipoEmpresa = new Array();

    _tipoEmpresa.push({ value: "J", text: Localization.Resources.Transportadores.Transportador.Juridica });
    _tipoEmpresa.push({ value: "E", text: Localization.Resources.Transportadores.Transportador.Exterior });

    return _tipoEmpresa;
}

function loadDadosCadastraisTransportador() {
    _dadosCadastraisTransportador = new DadosCadastraisTransportador();
    KoBindings(_dadosCadastraisTransportador, "knockoutCadastroTransportador");

    _dadosCadastraisCRUDTransportador = new DadosCadastraisCRUDTransportador();
    KoBindings(_dadosCadastraisCRUDTransportador, "knockoutCRUDTransportador");

    HeaderAuditoria(Localization.Resources.Transportadores.Transportador.Empresa, _dadosCadastraisTransportador);

    $("#" + _dadosCadastraisTransportador.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _dadosCadastraisTransportador.RegistroANTT.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

    new BuscarLocalidades(_dadosCadastraisTransportador.Localidade);

    BuscarDadosCadastraisTransportador();

    _dadosCadastraisTransportador.OptanteSimplesNacional.val.subscribe(OptanteSimplesNacionalChangeDadosCadastrais);

    _dadosCadastraisTransportador.OptanteSimplesNacional.val.subscribe(OptanteSimplesNacionalChangeDadosCadastrais);

    if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
        definirCamposMinimosObrigatoriosDadosCadastrais();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function OptanteSimplesNacionalChangeDadosCadastrais() {
    if (_dadosCadastraisTransportador.OptanteSimplesNacional.val()) {
        _dadosCadastraisTransportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(true);
        _dadosCadastraisTransportador.RegimenTributario.visible(true);
        _dadosCadastraisTransportador.RegimeEspecial.visible(false);
    } else {
        _dadosCadastraisTransportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(false);
        _dadosCadastraisTransportador.RegimenTributario.visible(false);
        _dadosCadastraisTransportador.RegimeEspecial.visible(true);
    }
}

function tipoEmpresaChangeDadosCadastrais(e, sender) {
    if (_dadosCadastraisTransportador.TipoEmpresa.val() == "F") {
        _dadosCadastraisTransportador.RazaoSocial.text(Localization.Resources.Gerais.Geral.Nome.getRequiredFieldDescription());
        _dadosCadastraisTransportador.InscricaoEstadual.text((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription()));
        _dadosCadastraisTransportador.InscricaoEstadual.required(!_CONFIGURACAO_TMS.SistemaEstrangeiro);
    } else if (_dadosCadastraisTransportador.TipoEmpresa.val() == "E") {
        _dadosCadastraisTransportador.InscricaoEstadual.text(Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription());
        _dadosCadastraisTransportador.InscricaoEstadual.required(false);
        _dadosCadastraisTransportador.RazaoSocial.text(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription());
    } else {
        _dadosCadastraisTransportador.RazaoSocial.text(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription());
        _dadosCadastraisTransportador.InscricaoEstadual.text((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : "*") + Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription());
        _dadosCadastraisTransportador.InscricaoEstadual.required(!_CONFIGURACAO_TMS.SistemaEstrangeiro);
    }
}

function atualizarClickDadosCadastrais() {
    if (!validarCamposCadastroTransportadorDadosCadastrais())
        return;
    executarReST("DadosTransportador/Atualizar", obterTransportadorSalvarDadosCadastrais(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClickDadosCadastrais(e) {
    limparCamposTransportadorDadosCadastrais();
}

/*
 * Declaração das Funções
 */

function BuscarDadosCadastraisTransportador() {
    _EditouTransportador = true;
    BuscarPorCodigo(_dadosCadastraisTransportador, "DadosTransportador/BuscarPorCodigo", function (arg) {
        preecherRetornoEditarTransportadorDadosCadastrais(arg);

        if (_dadosCadastraisTransportador.OptanteSimplesNacional.val())
            _dadosCadastraisTransportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(true);
        else
            _dadosCadastraisTransportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(false);
    }, null);
}

function exibirMensagemCamposObrigatorioDadosCadastrais() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function preecherRetornoEditarTransportadorDadosCadastrais() {
    tipoEmpresaChangeDadosCadastrais();

    if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
        definirCamposMinimosObrigatoriosDadosCadastrais();
}

function limparCamposTransportadorDadosCadastrais() {
    LimparCampos(_dadosCadastraisTransportador);

    tipoEmpresaChangeDadosCadastrais();

    if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
        definirCamposMinimosObrigatoriosDadosCadastrais();
}

function obterTransportadorSalvarDadosCadastrais() {
    var transportador = RetornarObjetoPesquisa(_dadosCadastraisTransportador);
    return transportador;
}

function validarCamposCadastroTransportadorDadosCadastrais() {
    if (!ValidarCamposObrigatorios(_dadosCadastraisTransportador)) {
        exibirMensagemCamposObrigatorioDadosCadastrais();
        return false;
    }

    if (_CONFIGURACAO_TMS.ExigirAnexosNoCadastroDoTransportador && _dadosCadastraisTransportador.Status.val() === "A" && !_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.PorFavorInformeMaisAnexosTransportador);
        return false;
    }

    return true;
}

function definirCamposMinimosObrigatoriosDadosCadastrais() {
    _dadosCadastraisTransportador.NomeFantasia.required = false;
    _dadosCadastraisTransportador.InscricaoEstadual.required(false);
    _dadosCadastraisTransportador.Endereco.required = false;
    _dadosCadastraisTransportador.Numero.required = false;
    _dadosCadastraisTransportador.Bairro.required = false;
    _dadosCadastraisTransportador.Telefone.required = false;
    _dadosCadastraisTransportador.RegistroANTT.required = false;
    _dadosCadastraisTransportador.RegistroANTT.text(Localization.Resources.Transportadores.Transportador.RNTRC.getFieldDescription());
    _dadosCadastraisTransportador.Email.required = true;
}