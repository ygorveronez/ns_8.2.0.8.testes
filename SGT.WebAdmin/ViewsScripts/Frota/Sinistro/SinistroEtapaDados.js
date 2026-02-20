/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TipoSinistro.js" />
/// <reference path="../../Enumeradores/EnumCausadorSinistro.js" />
/// <reference path="../../Enumeradores/EnumEtapaSinistro.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEtapaFluxoSinistro.js" />
/// <reference path="Sinistro.js" />

var _etapaDadosSinistro;

var DadosSinistro = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaSinistro.Dados), options: EnumEtapaSinistro.obterOpcoes(), def: EnumEtapaSinistro.Dados });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoEtapaFluxoSinistro.Aberto), def: EnumSituacaoEtapaFluxoSinistro.Aberto });

    this.Numero = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: "Número:" });
    this.CausadorSinistro = PropertyEntity({ options: EnumCausadorSinistro.obterOpcoes(), def: EnumCausadorSinistro.VeiculoProprio, val: ko.observable(EnumCausadorSinistro.VeiculoProprio), text: "*Causador do Sinistro:", required: true, enable: ko.observable(true) });
    this.NumeroBoletimOcorrencia = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 100, text: "Número do B.O.:", enable: ko.observable(true) });
    this.DataSinistro = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: "*Data e Hora do Sinistro:", required: true, enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "", text: "Data Emissão:", enable: ko.observable(true) });
    this.Local = PropertyEntity({ getType: typesKnockout.text, val: ko.observable(""), def: "", text: "*Local:", required: true, enable: ko.observable(true) });
    this.Cidade = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Cidade:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Endereço:", enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Veículo do Sinistro:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.VeiculoReboque = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Reboque:", required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "*Motorista:", required: true, idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoSinistro = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Tipo Sinistro:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.GravidadeSinistro = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: "Gravidade Sinistro:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Observação:", enable: ko.observable(true), maxlength: 3000 });

    this.IniciarFluxo = PropertyEntity({ eventClick: iniciarFluxoSinistroClick, type: types.event, text: "Iniciar Fluxo de Sinistro", visible: ko.observable(true) });
    this.SalvarNumeroBoletimOcorrencia = PropertyEntity({ eventClick: salvarNumeroBoletimOcorrenciaClick, type: types.event, text: "Salvar N° B.O.", visible: this.Codigo.val });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.AtualizarFluxo = PropertyEntity({ eventClick: atualizarFluxoSinistroClick, type: types.event, text: "Atualizar Fluxo de Sinistro", visible: ko.observable(false) });
};

function loadEtapaDadosSinistro() {
    _etapaDadosSinistro = new DadosSinistro();
    KoBindings(_etapaDadosSinistro, "knockoutFluxoSinistroDados");

    HeaderAuditoria("SinistroDados", _etapaDadosSinistro);

    new BuscarMotoristas(_etapaDadosSinistro.Motorista);
    new BuscarVeiculos(_etapaDadosSinistro.Veiculo, RetornoVeiculo);
    new BuscarReboques(_etapaDadosSinistro.VeiculoReboque);
    new BuscarLocalidades(_etapaDadosSinistro.Cidade);
    new BuscarTipoSinistro(_etapaDadosSinistro.TipoSinistro);
    new BuscarGravidadeSinistro(_etapaDadosSinistro.GravidadeSinistro);
}
var dados;
function RetornoVeiculo(data) {
    dados = data;
    _etapaDadosSinistro.Veiculo.codEntity(data.Codigo);
    _etapaDadosSinistro.Veiculo.val(data.Descricao);
    if (data.Tipo == "1")
        _etapaDadosSinistro.VeiculoReboque.visible(false);
    else {
        _etapaDadosSinistro.VeiculoReboque.visible(true);
        _etapaDadosSinistro.VeiculoReboque.codEntity(data.CodigosVeiculosVinculados.split(",", 1)[0]);
        _etapaDadosSinistro.VeiculoReboque.val(data.VeiculosVinculados.split(",", 1)[0]);
    }

}

function iniciarFluxoSinistroClick() {
    if (!ValidarCamposObrigatorios(_etapaDadosSinistro)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("Sinistro/IniciarFluxoSinistro", RetornarObjetoPesquisa(_etapaDadosSinistro), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                limparFluxoSinistro();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo iniciado com sucesso.");
                CarregarDadosSinistro(retorno.Data.Codigo);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarFluxoSinistroClick() {
    Salvar(_etapaDadosSinistro, "Sinistro/AtualizarFluxoSinistro", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                limparFluxoSinistro();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo atualizado com sucesso.");
                CarregarDadosSinistro(retorno.Data.Codigo);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function salvarNumeroBoletimOcorrenciaClick() {
    executarReST("Sinistro/SalvarNumeroBoletimOcorrencia", { Codigo: _etapaDadosSinistro.Codigo.val(), NumeroBoletimOcorrencia: _etapaDadosSinistro.NumeroBoletimOcorrencia.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampos(_etapaDadosSinistro);
                recarregarGridSinistro();
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function limparClick() {
    limparFluxoSinistro();
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o Sinistro?", function () {
        ExcluirPorCodigo(_etapaDadosSinistro, "Sinistro/CancelarSinistro", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso");
                    limparClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function preencherEtapaDadosSinistro(dadosSinistro) {
    PreencherObjetoKnout(_etapaDadosSinistro, { Data: dadosSinistro });

    _etapaDadosSinistro.IniciarFluxo.visible(false);

    if (!dadosSinistro.PossuiReboque)
        _etapaDadosSinistro.VeiculoReboque.visible(false);
    else
        _etapaDadosSinistro.VeiculoReboque.visible(true);

    if (_etapaDadosSinistro.Etapa.val() !== EnumEtapaSinistro.Dados)
        SetarEnableCamposKnockout(_etapaDadosSinistro, false);

    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.Dados && _etapaDadosSinistro.Codigo.val() > 0)
        _etapaDadosSinistro.AtualizarFluxo.visible(true);

    if (_etapaDadosSinistro.Situacao.val() == EnumSituacaoEtapaFluxoSinistro.Cancelado)
        _etapaDadosSinistro.SalvarNumeroBoletimOcorrencia.visible(false);
    else
        _etapaDadosSinistro.NumeroBoletimOcorrencia.enable(true);

    if (_etapaDadosSinistro.Situacao.val() != EnumSituacaoEtapaFluxoSinistro.Cancelado)
        _etapaDadosSinistro.Cancelar.visible(true);
}

function limparCamposSinistroEtapaDados() {
    LimparCampos(_etapaDadosSinistro);
    _etapaDadosSinistro.IniciarFluxo.visible(true);
    _etapaDadosSinistro.AtualizarFluxo.visible(false);
    SetarEnableCamposKnockout(_etapaDadosSinistro, true);
    _etapaDadosSinistro.Cancelar.visible(false);
}