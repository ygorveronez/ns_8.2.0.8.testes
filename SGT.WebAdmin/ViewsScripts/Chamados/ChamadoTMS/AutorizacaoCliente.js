/// <reference path="ChamadoTMS.js" />
/// <reference path="AdiantamentoMotorista.js" />
/// <reference path="OrientacaoMotorista.js" />
/// <reference path="AnexosAutorizacaoCliente.js" />
/// <reference path="../../Consultas/ContatoGrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacaoClienteChamado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _autorizacaoCliente;
var _CRUDEtapa2;

var AutorizacaoCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", getType: typesKnockout.int, enable: ko.observable(true) });
    this.DataRetornoAutorizacaoCliente = PropertyEntity({ text: "Data Retorno:", getType: typesKnockout.date, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorAprovacaoParcial = PropertyEntity({ text: "Valor Aprovação Parcial:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.SituacaoAutorizacaoCliente = PropertyEntity({ val: ko.observable(EnumSituacaoAutorizacaoClienteChamado.Aberto), options: EnumSituacaoAutorizacaoClienteChamado.obterOpcoes(), enable: ko.observable(true), text: "Situação: ", def: EnumSituacaoAutorizacaoClienteChamado.Aberto, visible: ko.observable(true) });
    this.ObservacaoAutorizacaoCliente = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação:", enable: ko.observable(true) });

    //Grupo Email
    this.ContatoGrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: "*Contato Grupo Pessoa:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.AssuntoEmail = PropertyEntity({ val: ko.observable(""), def: "", text: "*Assunto:", maxlength: 1000, required: ko.observable(false), enable: ko.observable(true) });
    this.CorpoEmail = PropertyEntity({ val: ko.observable(""), def: "", text: "*Corpo do E-mail:", required: ko.observable(false), enable: ko.observable(true) });
    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosAutorizacaoClienteClick, type: types.event, text: "Anexos do E-mail", visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: enviarEmailAutorizacaoClienteClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(true), enable: ko.observable(true) });

    this.AdiantamentosMotorista = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.OrientacaoMotorista = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
};

var CRUDEtapa2 = function () {
    this.Salvar = PropertyEntity({ eventClick: salvarEtapa2Click, type: types.event, text: "Salvar Dados Etapa 2", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadAutorizacaoCliente() {
    _autorizacaoCliente = new AutorizacaoCliente();
    KoBindings(_autorizacaoCliente, "tabAutorizacaoCliente");

    _CRUDEtapa2 = new CRUDEtapa2();
    KoBindings(_CRUDEtapa2, "footerEtapa2");

    new BuscarContatoGrupoPessoa(_autorizacaoCliente.ContatoGrupoPessoa, function (data) {
        _autorizacaoCliente.ContatoGrupoPessoa.codEntity(data.Codigo);
        _autorizacaoCliente.ContatoGrupoPessoa.val(data.Descricao);

        _autorizacaoCliente.AssuntoEmail.val(data.AssuntoEmailChamado);
        _autorizacaoCliente.CorpoEmail.val(data.CorpoEmailChamado);
        _orientacaoMotorista.MensagemOrientacaoMotorista.val(data.MensagemPadraoOrientacaoMotorista);

        $("#" + _autorizacaoCliente.AssuntoEmail.id).focus();
    });

    loadAnexosAutorizacaoCliente();
    loadAdiantamentoMotorista();
    loadOrientacaoMotorista();
}

function salvarEtapa2Click(e, sender, enviaEmail) {
    _autorizacaoCliente.ContatoGrupoPessoa.required(false);
    _autorizacaoCliente.AssuntoEmail.required(false);
    _autorizacaoCliente.CorpoEmail.required(false);
    _autorizacaoCliente.OrientacaoMotorista.val(JSON.stringify(RetornarObjetoPesquisa(_orientacaoMotorista)));

    Salvar(_autorizacaoCliente, "ChamadoTMS/SalvarEtapa2", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados da Etapa 2 salvos com sucesso");
                if (enviaEmail)
                    enviaEmailEtapa2();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function enviarEmailAutorizacaoClienteClick(e, sender) {
    _autorizacaoCliente.ContatoGrupoPessoa.required(true);
    _autorizacaoCliente.AssuntoEmail.required(true);
    _autorizacaoCliente.CorpoEmail.required(true);

    var valido = ValidarCamposObrigatorios(_autorizacaoCliente);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    }

    salvarEtapa2Click(e, sender, true);
}

function enviaEmailEtapa2() {
    var data = { Codigo: _autorizacaoCliente.Codigo.val() };
    executarReST("ChamadoTMS/EnviarPorEmail", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Seu e-mail já está sendo enviado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function EditarEtapa2(data) {
    PreencherObjetoKnout(_autorizacaoCliente, { Data: data.AutorizacaoCliente });
    PreencherObjetoKnout(_orientacaoMotorista, { Data: data.OrientacaoMotorista });

    if (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.Aberto)
        ControleCamposEtapa2(true);
    else
        ControleCamposEtapa2(false);

    CarregarAnexosAutorizacaoCliente(data);
    CarregarAnexosAdiantamentoMotorista(data);
    recarregarGridAdiantamentosMotorista();
}

function ControleCamposEtapa2(status) {
    SetarEnableCamposKnockout(_autorizacaoCliente, status);

    _autorizacaoCliente.Anexo.enable(true);
    _CRUDEtapa2.Salvar.visible(status);

    ControleCamposAdiantamentoMotorista(status);
    ControleCamposOrientacaoMotorista(status);
}

function limparCamposAutorizacaoCliente() {
    LimparCampos(_autorizacaoCliente);
    limparOcorrenciaAnexosAutorizacaoCliente();
    limparCamposAdiantamentoMotorista();
    limparCamposOrientacaoMotorista();
}