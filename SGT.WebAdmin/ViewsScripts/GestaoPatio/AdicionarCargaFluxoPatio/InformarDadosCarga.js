/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="..\..\Consultas\Cliente.js" />
/// <reference path="..\..\Consultas\Filial.js" />
/// <reference path="..\..\Consultas\ModeloVeicularCarga.js" />
/// <reference path="..\..\Consultas\TipoCarga.js" />
/// <reference path="..\..\Consultas\TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _corFilaCarregamentoSelecionada = "#e6ffcc";
var _gridFilaCarregamento;
var _informarDadosCarga;
var _isPermitirRecarregarGridFilaCarregamento;

// #endregion

// #region Classes

var InformarDadosCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "*Número da Carga: ", enable: false });
    this.Doca = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Doca/Box: ", maxlength: 20, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.FilaCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Veiculo:", idBtnSearch: guid(), required: true, enable: false });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.Lacre = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", text: "Lacre: ", maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Modelo Veicular:", idBtnSearch: guid(), required: true });
    this.MotivoSelecaoMotoristaForaOrdem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "*Motivo da Seleção do Motorista Fora da Ordem:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.PrimeiroNaFila = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.ModeloVeicular.codEntity.subscribe(modeloVeicularInformarDadosCargaChange);
    this.PrimeiroNaFila.val.subscribe(primeiroNaFilaInformarDadosCargaChange);

    this.Atualizar = PropertyEntity({ eventClick: informarDadosCargaClick, type: types.event, text: ko.observable("Atualizar"), visible: true });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridFilaCarregamento() {
    var draggableRows = false;
    var limiteRegistros = 14;
    var totalRegistrosPorPagina = 14;

    _gridFilaCarregamento = new GridView("grid-fila-carregamento", "FilaCarregamento/PesquisaFluxoPatio", _informarDadosCarga, null, null, totalRegistrosPorPagina, null, false, draggableRows, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowGridInformarDadosCarga);
    _gridFilaCarregamento.CarregarGrid();
}

function loadInformarDadosCarga() {
    _isPermitirRecarregarGridFilaCarregamento = true;

    _informarDadosCarga = new InformarDadosCarga();
    KoBindings(_informarDadosCarga, "knockoutInformarDadosCarga");
    
    new BuscarModelosVeicularesCarga(_informarDadosCarga.ModeloVeicular, retornoConsultaModeloVeicularInformarDadosCarga);
    new BuscarMotivoSelecaoMotoristaForaOrdem(_informarDadosCarga.MotivoSelecaoMotoristaForaOrdem);

    loadGridFilaCarregamento();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function informarDadosCargaClick() {
    if (!ValidarCamposObrigatorios(_informarDadosCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    var informarDadosCarga = RetornarObjetoPesquisa(_informarDadosCarga);

    executarReST("AdicionarCargaFluxoPatio/AdicionarCarga", informarDadosCarga, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalInformarDadosCarga();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Carga adicionada ao fluxo de pátio com sucesso.");
                recarregarGridCargaAdicionarFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function modeloVeicularInformarDadosCargaChange() {
    if (_informarDadosCarga.ModeloVeicular.codEntity() == 0)
        recarregarGridFilaCarregamento();
}

function primeiroNaFilaInformarDadosCargaChange(isPrimeiroNaFila) {
    if (isPrimeiroNaFila) {
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.codEntity(0);
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.val("");
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.required(false);
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.visible(false);
    }
    else {
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.required(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe);
        _informarDadosCarga.MotivoSelecaoMotoristaForaOrdem.visible(true);
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function informarDadosCarga(codigoCarga) {
    executarReST("AdicionarCargaFluxoPatio/BuscarPorCodigo", { Codigo: codigoCarga }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_informarDadosCarga, retorno);
                recarregarGridFilaCarregamento();
                exibirModalInformarDadosCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function callbackRowGridInformarDadosCarga(row, data) {
    if (data.Codigo == _informarDadosCarga.FilaCarregamento.codEntity())
        $(row).css("background-color", _corFilaCarregamentoSelecionada);

    $(row).click(function () {
        selecionarFilaCarregamento(data);
    });
}

function exibirModalInformarDadosCarga() {
    Global.abrirModal('divModalInformarDadosCarga');
    $("#divModalInformarDadosCarga").one('hidden.bs.modal', function () {
        limparCamposInformarDadosCarga();
    });
}

function fecharModalInformarDadosCarga() {
    Global.fecharModal('divModalInformarDadosCarga');
}

function limparCamposInformarDadosCarga() {
    _isPermitirRecarregarGridFilaCarregamento = false;

    LimparCampos(_informarDadosCarga);
    recarregarGridFilaCarregamento();

    _isPermitirRecarregarGridFilaCarregamento = true;
}

function limparFilaCarregamentoSelecionada() {
    _informarDadosCarga.FilaCarregamento.codEntity(0);
    _informarDadosCarga.FilaCarregamento.val("");
    _informarDadosCarga.PrimeiroNaFila.val(true);
}

function recarregarGridFilaCarregamento() {
    _gridFilaCarregamento.CarregarGrid();
}

function retornoConsultaModeloVeicularInformarDadosCarga(modeloVeicularSelecionada) {
    _informarDadosCarga.ModeloVeicular.val(modeloVeicularSelecionada.Descricao);
    _informarDadosCarga.ModeloVeicular.entityDescription(modeloVeicularSelecionada.Descricao);
    _informarDadosCarga.ModeloVeicular.codEntity(modeloVeicularSelecionada.Codigo);

    recarregarGridFilaCarregamento();
}

function selecionarFilaCarregamento(filaCarregamentoSelecionada) {
    $("#grid-fila-carregamento tbody #" + _informarDadosCarga.FilaCarregamento.codEntity()).css("background-color", "");

    if (_informarDadosCarga.FilaCarregamento.codEntity() == filaCarregamentoSelecionada.Codigo)
        limparFilaCarregamentoSelecionada();
    else {
        _informarDadosCarga.FilaCarregamento.codEntity(filaCarregamentoSelecionada.Codigo);
        _informarDadosCarga.FilaCarregamento.val(filaCarregamentoSelecionada.Tracao || filaCarregamentoSelecionada.Reboques);
        _informarDadosCarga.PrimeiroNaFila.val(filaCarregamentoSelecionada.PrimeiroNaFila);

        $("#grid-fila-carregamento tbody #" + filaCarregamentoSelecionada.Codigo).css("background-color", _corFilaCarregamentoSelecionada);
    }
}

// #endregion Funções Privadas
