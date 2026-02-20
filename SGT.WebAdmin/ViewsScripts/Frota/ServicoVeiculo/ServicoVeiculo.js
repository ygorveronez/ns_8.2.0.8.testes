/// <reference path="../../Enumeradores/EnumMotivoServicoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoManutencaoServicoVeiculo.js" />
/// <reference path="../../Enumeradores/EnumCores.js" />
/// <reference path="ServicoVeiculoGrupoServico.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridServicoVeiculo;
var _servicoVeiculo;
var _pesquisaServicoVeiculo;
var _CRUDServicoVeiculo;

/*
 * Declaração das Classes
 */

var PesquisaServicoVeiculo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 150 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridServicoVeiculo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ServicoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 150, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", maxlength: 50 });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoServicoVeiculo.Ambos), def: EnumTipoServicoVeiculo.Ambos, options: EnumTipoServicoVeiculo.obterOpcoes(), text: "*Tipo:", enable: ko.observable(true) });
    this.ValidadeDias = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Validade em Dias:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaDias = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 3, text: "Tolerância em Dias:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ValidadeKM = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 7, text: "Validade em KM:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaKM = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Tolerância em KM:", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.ValidadeHorimetro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 15, text: "Validade em Horímetro:", required: false, visible: ko.observable(false), configInt: { precision: 0, allowZero: true } });
    this.ToleranciaHorimetro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 15, text: "Tolerância em Horímetro:", required: false, visible: ko.observable(false), configInt: { precision: 0, allowZero: true } });
    this.ExecucaoUnica = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Este serviço deve ser realizado uma única vez?", cssClass: ko.observable("col-12 col-md-3 input-margin-top-22-sm") });
    this.PermiteLancamentoSemValor = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Permitir lançamento em OS sem valor?", cssClass: ko.observable("col-12 col-md-3 input-margin-top-22-sm") });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Plano de Contas:", idBtnSearch: guid(), visible: false });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 400 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Motivo = PropertyEntity({ val: ko.observable(EnumMotivoServicoVeiculo.Outros), def: EnumMotivoServicoVeiculo.Outros, options: EnumMotivoServicoVeiculo.obterOpcoes(), text: "*Motivo:", enable: ko.observable(true) });
    this.ObrigatorioParaRealizarCarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "É obrigatório que este serviço esteja em dia para realizar uma carga?", cssClass: ko.observable("col-12 col-md-12") });
    this.ServicoParaEquipamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Serviço para equipamentos?", cssClass: ko.observable("col-12 col-md-12") });
    this.TempoEstimado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 6, text: "Tempo estimado (min):", required: false, visible: ko.observable(true), configInt: { precision: 0, allowZero: true } });
    this.TipoManutencao = PropertyEntity({ val: ko.observable(EnumTipoManutencaoServicoVeiculo.Outros), def: EnumTipoManutencaoServicoVeiculo.Outros, options: EnumTipoManutencaoServicoVeiculo.obterOpcoes(), text: "Tipo Manutenção: " });
    this.Cores = PropertyEntity({ val: ko.observable(EnumCores.Branco), def: EnumCores.Branco, options: EnumCores.obterOpcoes(), text: "Cor: ", visible: ko.observable(true) });
    this.Prioridade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, maxlength: 2, text: "Prioridade:", visible: ko.observable(true), configInt: { precision: 0, allowZero: false } });

    this.GruposServico = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Tipo.val.subscribe(function (novoValor) {
        TipoServicoVeiculoChange(novoValor);
    });

    this.ExecucaoUnica.val.subscribe(function (novoValor) {
        if (novoValor) {
            _servicoVeiculo.Tipo.enable(false);
            _servicoVeiculo.Tipo.val(EnumTipoServicoVeiculo.PorKM);
            _servicoVeiculo.ValidadeDias.val("0");
            _servicoVeiculo.ToleranciaDias.val("0");
            _servicoVeiculo.ToleranciaDias.required = false;
            _servicoVeiculo.ValidadeHorimetro.required = false;
            _servicoVeiculo.ToleranciaHorimetro.required = false;
            _servicoVeiculo.ValidadeDias.required = false;
            _servicoVeiculo.ValidadeKM.required = false;
            _servicoVeiculo.ToleranciaKM.required = false;
        } else {
            _servicoVeiculo.Tipo.enable(true);
        }
    });
};

var CRUDServicoVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadServicoVeiculo() {
    _servicoVeiculo = new ServicoVeiculo();
    KoBindings(_servicoVeiculo, "knockoutCadastroServicoVeiculo");

    _pesquisaServicoVeiculo = new PesquisaServicoVeiculo();
    KoBindings(_pesquisaServicoVeiculo, "knockoutPesquisaServicoVeiculo", _pesquisaServicoVeiculo.Pesquisar.id);

    HeaderAuditoria("ServicoVeiculoFrota", _servicoVeiculo);

    _CRUDServicoVeiculo = new CRUDServicoVeiculo();
    KoBindings(_CRUDServicoVeiculo, "knockoutCRUDServicoVeiculo");

    new BuscarPlanoConta(_servicoVeiculo.PlanoConta);

    BuscarServicosVeiculo();

    LoadServicoVeiculoGrupoServico();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_servicoVeiculo, "ServicoVeiculo/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridServicoVeiculo.CarregarGrid();
                LimparCamposServicoVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_servicoVeiculo, "ServicoVeiculo/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridServicoVeiculo.CarregarGrid();
                LimparCamposServicoVeiculo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function CancelarClick(e) {
    LimparCamposServicoVeiculo();
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir este serviço de veículo?", function () {
        ExcluirPorCodigo(_servicoVeiculo, "ServicoVeiculo/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridServicoVeiculo.CarregarGrid();
                    LimparCamposServicoVeiculo();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function TipoServicoVeiculoChange(novoValor) {
    var possuiKM = false;
    var possuiDias = false;
    var possuiHorimetro = false;

    _servicoVeiculo.ValidadeHorimetro.required = false;
    _servicoVeiculo.ToleranciaHorimetro.required = false;

    _servicoVeiculo.ToleranciaDias.required = false;
    _servicoVeiculo.ValidadeDias.required = false;

    _servicoVeiculo.ValidadeKM.required = false;
    _servicoVeiculo.ToleranciaKM.required = false;

    switch (novoValor) {
        case EnumTipoServicoVeiculo.Ambos:
            possuiKM = true;
            possuiDias = true;
            possuiHorimetro = false;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorDia:
            possuiKM = false;
            possuiDias = true;
            possuiHorimetro = false;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            break;
        case EnumTipoServicoVeiculo.PorKM:
            possuiDias = false;
            possuiKM = true;
            possuiHorimetro = false;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorHorimetro:
            possuiDias = false;
            possuiKM = false;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            break;
        case EnumTipoServicoVeiculo.Todos:
            possuiDias = true;
            possuiKM = true;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            _servicoVeiculo.ValidadeKM.required = true;
            _servicoVeiculo.ToleranciaKM.required = true;
            break;
        case EnumTipoServicoVeiculo.PorHorimetroDia:
            possuiDias = true;
            possuiKM = false;
            possuiHorimetro = true;
            _servicoVeiculo.ValidadeHorimetro.required = true;
            _servicoVeiculo.ToleranciaHorimetro.required = true;
            _servicoVeiculo.ToleranciaDias.required = true;
            _servicoVeiculo.ValidadeDias.required = true;
            break;
        case EnumTipoServicoVeiculo.Nenhum:
            possuiDias = false;
            possuiKM = false;
            possuiHorimetro = false;
            _servicoVeiculo.ValidadeHorimetro.required = false;
            _servicoVeiculo.ToleranciaHorimetro.required = false;
            _servicoVeiculo.ToleranciaDias.required = false;
            _servicoVeiculo.ValidadeDias.required = false;
            _servicoVeiculo.ValidadeKM.required = false;
            _servicoVeiculo.ToleranciaKM.required = false;
            break;
        default:
            break;
    }

    if (possuiDias) {
        _servicoVeiculo.ValidadeDias.visible(true);
        _servicoVeiculo.ToleranciaDias.visible(true);
    } else {
        _servicoVeiculo.ValidadeDias.val(0);
        _servicoVeiculo.ToleranciaDias.val(0);
        _servicoVeiculo.ValidadeDias.visible(false);
        _servicoVeiculo.ToleranciaDias.visible(false);
    }

    if (possuiKM) {
        _servicoVeiculo.ValidadeKM.visible(true);
        _servicoVeiculo.ToleranciaKM.visible(true);
    } else {
        _servicoVeiculo.ValidadeKM.val(0);
        _servicoVeiculo.ToleranciaKM.val(0);
        _servicoVeiculo.ValidadeKM.visible(false);
        _servicoVeiculo.ToleranciaKM.visible(false);
    }

    if (possuiHorimetro) {
        _servicoVeiculo.ValidadeHorimetro.visible(true);
        _servicoVeiculo.ToleranciaHorimetro.visible(true);
    } else {
        _servicoVeiculo.ValidadeHorimetro.val(0);
        _servicoVeiculo.ToleranciaHorimetro.val(0);
        _servicoVeiculo.ValidadeHorimetro.visible(false);
        _servicoVeiculo.ToleranciaHorimetro.visible(false);
    }

    if (possuiDias === possuiKM && possuiKM === possuiHorimetro) {
        _servicoVeiculo.ExecucaoUnica.cssClass("col-12 col-md-12");
        _servicoVeiculo.PermiteLancamentoSemValor.cssClass("col-12 col-md-12");
        _servicoVeiculo.ObrigatorioParaRealizarCarga.cssClass("col-12 col-md-12");
        _servicoVeiculo.ServicoParaEquipamento.cssClass("col-12 col-md-12");
    }
    else {
        _servicoVeiculo.ExecucaoUnica.cssClass("col-12 col-md-3");
        _servicoVeiculo.PermiteLancamentoSemValor.cssClass("col-12 col-md-3");
        _servicoVeiculo.ObrigatorioParaRealizarCarga.cssClass("col col-xs-12 col-sm-6");
        _servicoVeiculo.ServicoParaEquipamento.cssClass("col col-xs-12 col-sm-6");
    }
}

/*
 * Declaração das Funções
 */

function BuscarServicosVeiculo() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarServicoVeiculo, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridServicoVeiculo = new GridView(_pesquisaServicoVeiculo.Pesquisar.idGrid, "ServicoVeiculo/Pesquisa", _pesquisaServicoVeiculo, menuOpcoes, null);
    _gridServicoVeiculo.CarregarGrid();
}

function EditarServicoVeiculo(servicoVeiculoGrid) {
    LimparCamposServicoVeiculo();
    _servicoVeiculo.Codigo.val(servicoVeiculoGrid.Codigo);
    BuscarPorCodigo(_servicoVeiculo, "ServicoVeiculo/BuscarPorCodigo", function (arg) {
        _pesquisaServicoVeiculo.ExibirFiltros.visibleFade(false);
        _CRUDServicoVeiculo.Atualizar.visible(true);
        _CRUDServicoVeiculo.Cancelar.visible(true);
        _CRUDServicoVeiculo.Excluir.visible(true);
        _CRUDServicoVeiculo.Adicionar.visible(false);

        RecarregarGridServicoVeiculoGrupoServico();
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe)
            $("#liTabGrupoServico").show();
    }, null);
}

function LimparCamposServicoVeiculo() {
    _CRUDServicoVeiculo.Atualizar.visible(false);
    _CRUDServicoVeiculo.Cancelar.visible(false);
    _CRUDServicoVeiculo.Excluir.visible(false);
    _CRUDServicoVeiculo.Adicionar.visible(true);
    LimparCampos(_servicoVeiculo);
    _servicoVeiculo.Tipo.enable(true);

    LimparCamposServicoVeiculoGrupoServico();
    Global.ResetarAbas();
}

