/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoServico.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />
/// <reference path="../OrdemServico/ResumoOrdemServico.js" />
/// <reference path="../OrdemServico/EtapaOrdemServico.js" />
/// <reference path="../OrdemServico/Servico.js" />
/// <reference path="../OrdemServico/Orcamento.js" />
/// <reference path="../OrdemServico/Fechamento.js" />
/// <reference path="../OrdemServico/Aprovacao.js" />
/// <reference path="../OrdemServico/OrdemServicoLote.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeOrdemServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ordemServico, _CRUDOrdemServico, _rejeicaoOrcamentoOrdemServico, _cancelarOrdemServico, _CRUDAberturaOrdemServico, _dadosUsuarioLogado = null, _buscaGrupoServico;

var OrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable("Automático"), def: "Automático", enable: ko.observable(false) });
    this.DataProgramada = PropertyEntity({ text: "*Data Programada:", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), required: true, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOrdemServicoFrota.EmDigitacao), def: EnumSituacaoOrdemServicoFrota.EmDigitacao });
    this.SituacaoAnteriorCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoOrdemServicoFrota.EmDigitacao), def: EnumSituacaoOrdemServicoFrota.EmDigitacao });
    this.TipoManutencao = PropertyEntity({ val: ko.observable(EnumTipoManutencaoOrdemServicoFrota.Corretiva), def: EnumTipoManutencaoOrdemServicoFrota.Corretiva });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Horimetro = PropertyEntity({ text: "Horímetro:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.QuilometragemVeiculo = PropertyEntity({ text: "KM Atual:", required: false, getType: typesKnockout.int, maxlength: 15, enable: ko.observable(true), configInt: { precision: 0, allowZero: true }, def: "0", val: ko.observable("0"), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.LocalManutencao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Local de Manutenção:"), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false) });
    this.Observacao = PropertyEntity({ maxlength: 1000, text: "*Observação:", required: true, enable: ko.observable(true) });
    this.CondicaoPagamento = PropertyEntity({ maxlength: 1000, text: "Condição de Pagamento:", enable: ko.observable(true), required: ko.observable(_obrigaInformarCondicaoPagamento) });
    this.Tipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(_CONFIGURACAO_TMS.TipoOrdemServicoObrigatorio ? "*Tipo:" : "Tipo:"), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.TipoOrdemServicoObrigatorio) });
    this.LancarServicosManualmente = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.LancarOsServicosDaOrdemDeServicoAutomaticamente || _CONFIGURACAO_TMS.LancamentoServicoManualNaOSMarcadadoPorDefault), getType: typesKnockout.bool, def: (_CONFIGURACAO_TMS.LancarOsServicosDaOrdemDeServicoAutomaticamente || _CONFIGURACAO_TMS.LancamentoServicoManualNaOSMarcadadoPorDefault), text: "Deseja lançar os serviços de forma manual?", enable: ko.observable(true), visible: ko.observable(true) });
    this.GrupoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Grupo de Serviço:"), idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataLimiteExecucao = PropertyEntity({ text: "Data limite para a execução:", getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), enable: ko.observable(true) });
    this.Prioridade = PropertyEntity({ text: "Prioridade: ", val: ko.observable(EnumPrioridadeOrdemServico.Baixo), options: EnumPrioridadeOrdemServico.obterOpcoes(), def: EnumPrioridadeOrdemServico.Baixo, enable: ko.observable(true) });

    this.GrupoServico.val.subscribe(function (novoValor) {
        if (novoValor == "")
            _ordemServico.LancarServicosManualmente.enable(true);
    });
};

var CRUDOrdemServico = function () {
    this.GerarOrdemServico = PropertyEntity({ eventClick: GerarOrdemServicoClick, type: types.event, text: "Gerar OS", icon: "fal fa-chevron-right", visible: ko.observable(true) });
    this.ConfirmarExecucaoServicos = PropertyEntity({ eventClick: ConfirmarExecucaoServicosClick, type: types.event, text: "Confirmar Execução dos Serviços", icon: "fal fa-chevron-right", visible: ko.observable(false) });
    this.AutorizarOrdemServico = PropertyEntity({ eventClick: AutorizarOrdemServicoClick, type: types.event, text: "Autorizar OS", icon: "fal fa-thumbs-up", visible: ko.observable(false) });
    this.RejeitarOrdemServico = PropertyEntity({ eventClick: AbrirTelaMotivoRejeicaoOrcamentoOrdemServico, type: types.event, text: "Rejeitar OS", icon: "fal fa-thumbs-down", visible: ko.observable(false) });
    this.NovaOrdemServico = PropertyEntity({ eventClick: LimparCamposOrdemServico, type: types.event, text: "Limpar Campos / Nova OS", icon: "fal fa-undo", visible: ko.observable(true) });
    this.FecharOrdemServico = PropertyEntity({ eventClick: FecharOrdemServicoClick, type: types.event, text: "Fechar OS", icon: "fal fa-chevron-down", visible: ko.observable(false) });
    this.ReabrirOrdemServico = PropertyEntity({ eventClick: ReabrirOrdemServicoClick, type: types.event, text: "Reabrir OS", icon: "fal fa-chevron-left", visible: ko.observable(false) });
    this.CancelarOrdemServico = PropertyEntity({ eventClick: AbrirTelaMotivoCancelamentoOrdemServico, type: types.event, text: "Cancelar OS", icon: "fal fa-ban", visible: ko.observable(false) });
    this.DownloadOrdemServico = PropertyEntity({ eventClick: DownloadOrdemServicoClick, type: types.event, text: "Imprimir OS", icon: "fal fa-print", visible: ko.observable(false) });
    this.LiberarVeiculoDaManutencao = PropertyEntity({ eventClick: AbrirLiberacaoManutencaoClick, type: types.event, text: "Liberar da manutenção", icon: "fal fa-chevron-down", visible: ko.observable(false) });
    this.GerarLoteOrdemServico = PropertyEntity({ eventClick: GerarLoteOrdemServicoClick, type: types.event, text: "Gerar Lote de OS", icon: "fal fa-indent", visible: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar Histórico",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",

        UrlImportacao: "OrdemServico/Importar",
        UrlConfiguracao: "OrdemServico/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O015_Produto,
        CallbackImportacao: function () {
            _gridOrdemServico.CarregarGrid();
        }
    });
};

var CRUDAberturaOrdemServico = function () {
    this.AtualizarObservacao = PropertyEntity({ eventClick: AtualizarObservacaoOrdemServicoClick, type: types.event, text: "Atualizar Observação", visible: ko.observable(false) });
    this.AtualizarLocalManutencao = PropertyEntity({ eventClick: AtualizarLocalManutencaoOrdemServicoClick, type: types.event, text: "Atualizar Local Manutenção", visible: ko.observable(false) });
};

var RejecaoOrcamentoOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), def: "", required: true, maxlength: 250 });

    this.Rejeitar = PropertyEntity({ eventClick: RejeitarOrdemServicoClick, type: types.event, text: "Rejeitar", icon: "fal fa-thumbs-down", idGrid: guid() });
    this.Cancelar = PropertyEntity({ eventClick: FecharTelaMotivoRejeicaoOrcamentoOrdemServico, type: types.event, text: "Cancelar", icon: "fal fa-ban", idGrid: guid() });
};

var CancelarOrdemServico = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), def: "", required: true, maxlength: 250 });

    this.Cancelar = PropertyEntity({ eventClick: CancelarOrdemServicoClick, type: types.event, text: "Cancelar", icon: "fal fa-ban", idGrid: guid() });
    this.Limpar = PropertyEntity({ eventClick: FecharTelaMotivoCancelamentoOrdemServico, type: types.event, text: "Voltar", icon: "fal fa-circle", idGrid: guid() });
};

//*******EVENTOS*******

function LoadOrdemServico() {

    _ordemServico = new OrdemServico();
    KoBindings(_ordemServico, "knockoutCadastroOrdemServico");

    HeaderAuditoria("OrdemServicoFrota", _ordemServico);

    _CRUDOrdemServico = new CRUDOrdemServico();
    KoBindings(_CRUDOrdemServico, "knockoutCRUDOrdemServico");

    _CRUDAberturaOrdemServico = new CRUDAberturaOrdemServico();
    KoBindings(_CRUDAberturaOrdemServico, "knockoutCRUDAberturaOrdemServico");

    _rejeicaoOrcamentoOrdemServico = new RejecaoOrcamentoOrdemServico();
    KoBindings(_rejeicaoOrcamentoOrdemServico, "knockoutRejeicaoOrcamento");

    _cancelarOrdemServico = new CancelarOrdemServico();
    KoBindings(_cancelarOrdemServico, "knockoutCancelamento");

    new BuscarFuncionario(_ordemServico.Operador);
    new BuscarClientes(_ordemServico.LocalManutencao, RetornoConsultaLocalManutencao, true, [EnumModalidadePessoa.Fornecedor]);
    new BuscarVeiculos(_ordemServico.Veiculo, RetornoVeiculo, null, null, _ordemServico.Motorista);
    new BuscarMotoristas(_ordemServico.Motorista);
    new BuscarTipoOrdemServico(_ordemServico.Tipo);
    new BuscarCentroResultado(_ordemServico.CentroResultado);
    new BuscarEquipamentos(_ordemServico.Equipamento, RetornoEquipamento);
    _buscaGrupoServico = new BuscarGrupoServico(_ordemServico.GrupoServico, RetornoGrupoServico, _ordemServico.Tipo);
    new BuscarFuncionario(_ordemServico.Responsavel);

    LoadEtapaOrdemServico();
    LoadResumoOrdemServico();
    LoadServicoOrdemServico();
    LoadOrcamentoOrdemServico();
    LoadAprovacaoOrdemServico();
    LoadFechamentoOrdemServico();
    LoadOrdemServicoLote();

    if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosOrdemServico) {
        _ordemServico.LocalManutencao.required(true);
        _ordemServico.LocalManutencao.text("*Local de Manutenção:");
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe)
        _ordemServico.GrupoServico.visible(false);
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor)
        ConfigurarPropriedadesCamposFornecedor();

    if (_obrigaInformarCondicaoPagamento) {
        _ordemServico.CondicaoPagamento.required(true);
        _ordemServico.CondicaoPagamento.text("*Condição de Pagamento:");
    }

    if (CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO > 0)
        EditarOrdemServico(null);
}

function RetornoVeiculo(data) {
    if (data != null & data.Motorista != null & data.Motorista != "" & data.CodigoMotorista > 0) {
        _ordemServico.Motorista.val(data.Motorista);
        _ordemServico.Motorista.codEntity(data.CodigoMotorista);
    }
    if (data != null & data.CentroResultado != null & data.CentroResultado != "" & data.CodigoCentroResultado > 0) {
        _ordemServico.CentroResultado.val(data.CentroResultado);
        _ordemServico.CentroResultado.codEntity(data.CodigoCentroResultado);
    }
    if (data != null & data.Placa != null & data.Placa != "" & data.Codigo > 0) {
        _ordemServico.Veiculo.val(data.DescricaoComMarcaModelo);
        _ordemServico.Veiculo.codEntity(data.Codigo);
        if (parseFloat(data.UltimoKMAbastecimento) > 0)
            _ordemServico.QuilometragemVeiculo.val(parseFloat(data.UltimoKMAbastecimento));
        else
            _ordemServico.QuilometragemVeiculo.val(data.KMAtual);
    }
    //ControleEquipamento(data.TipoVeiculo, true);

    SugerirGrupoServico();
}

function ControleEquipamento(tipoVeiculo, buscarEquipamentoPadrao) {
    if (buscarEquipamentoPadrao && _ordemServico.Veiculo.codEntity() > 0) {
        let data = { Codigo: _ordemServico.Veiculo.codEntity() };
        executarReST("Veiculo/BuscaEquipamentoPadrao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _ordemServico.Equipamento.val(arg.Data.Descricao);
                    _ordemServico.Equipamento.codEntity(arg.Data.Codigo);
                    _ordemServico.Horimetro.val(arg.Data.Horimetro);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function RetornoEquipamento(data) {

    if (data != null & data.CentroResultado != null & data.CentroResultado != "" & data.CodigoCentroResultado > 0) {
        _ordemServico.CentroResultado.val(data.CentroResultado);
        _ordemServico.CentroResultado.codEntity(data.CodigoCentroResultado);
    }
    _ordemServico.Equipamento.val(data.DescricaoComMarcaModelo + " (" + data.Numero + ")");
    _ordemServico.Equipamento.codEntity(data.Codigo);
    _ordemServico.Horimetro.val(data.Horimetro);

    SugerirGrupoServico();
}

function RetornoGrupoServico(data) {
    _ordemServico.GrupoServico.val(data.Descricao);
    _ordemServico.GrupoServico.codEntity(data.Codigo);
    _ordemServico.LancarServicosManualmente.val(false);
    _ordemServico.LancarServicosManualmente.enable(false);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _ordemServico.Tipo.codEntity(data.CodigoTipoOrdemServico);
        _ordemServico.Tipo.val(data.DescricaoTipoOrdemServico);
    }
}

function GerarOrdemServicoClick() {
    ValidarOrdemEmAndamento();
}

function ValidarOrdemEmAndamento() {
    Salvar(_ordemServico, "OrdemServico/ValidarOrdemEmAndamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (!arg.Data.ExisteOutraOS)
                    AdicionarOrdemServico();
                else
                    exibirConfirmacao("Ordem de Serviço em Andamento", "Já existe uma OS em andamento para esse Veículo/Equipamento. Tem certeza que deseja continuar?", AdicionarOrdemServico);
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}

function AdicionarOrdemServico() {
    Salvar(_ordemServico, "OrdemServico/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridOrdemServico.CarregarGrid();
                PreencherObjetoKnout(_ordemServico, r);
                _CRUDOrdemServico.GerarOrdemServico.visible(false);
                _CRUDOrdemServico.GerarLoteOrdemServico.visible(false);
                _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(true);
                _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de serviço gerada com sucesso!");
                _ordemServico.Codigo.val(r.Data.Codigo);
                PreecherResumoOrdemServico(r.Data);
                SetarEtapaOrdemServico();
                SetarEnableCamposKnockout(_ordemServico, false);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ConfirmarExecucaoServicosClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente confirmar a execução dos serviços?", function () {
        Salvar(_ordemServico, "OrdemServico/ConfirmarExecucaoServicos", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, r);
                    _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(false);
                    _CRUDOrdemServico.AutorizarOrdemServico.visible(true);
                    _CRUDOrdemServico.RejeitarOrdemServico.visible(true);
                    _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                    Etapa3Aguardando();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Execução dos serviços confirmada com sucesso!");
                    PreecherResumoOrdemServico(r.Data);
                    BloquearAlteracaoServicos();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function AutorizarOrdemServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente autorizar o orçamento desta ordem de serviço?", function () {
        Salvar(_ordemServico, "OrdemServico/AutorizarOrcamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, r);
                    _CRUDOrdemServico.AutorizarOrdemServico.visible(false);
                    _CRUDOrdemServico.RejeitarOrdemServico.visible(false);
                    _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                    _CRUDOrdemServico.CancelarOrdemServico.visible(true);

                    _CRUDOrdemServico.ReabrirOrdemServico.visible(false);
                    if (_ordemServico.Situacao.val() !== EnumSituacaoOrdemServicoFrota.AguardandoAprovacao && _ordemServico.Situacao.val() !== EnumSituacaoOrdemServicoFrota.SemRegraAprovacao) {
                        _CRUDOrdemServico.FecharOrdemServico.visible(true);
                        _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(true);
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        _fechamentoOrdemServico.OrdemServico.val(_ordemServico.Codigo.val());
                    }

                    SetarEtapaOrdemServico();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Orçamento autorizado com sucesso!");
                    PreecherResumoOrdemServico(r.Data);
                    BloquearAlteracaoOrcamento();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function AbrirTelaMotivoRejeicaoOrcamentoOrdemServico() {
    LimparCampos(_rejeicaoOrcamentoOrdemServico);
    _rejeicaoOrcamentoOrdemServico.Codigo.val(_ordemServico.Codigo.val());
    Global.abrirModal("knockoutRejeicaoOrcamento");
}

function FecharTelaMotivoRejeicaoOrcamentoOrdemServico() {
    LimparCampos(_rejeicaoOrcamentoOrdemServico);
    Global.fecharModal("knockoutRejeicaoOrcamento");
}

function RejeitarOrdemServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente rejeitar o orçamento desta ordem de serviço?", function () {
        Salvar(_rejeicaoOrcamentoOrdemServico, "OrdemServico/RejeitarOrcamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, r);
                    _CRUDOrdemServico.AutorizarOrdemServico.visible(false);
                    _CRUDOrdemServico.RejeitarOrdemServico.visible(false);
                    _CRUDOrdemServico.CancelarOrdemServico.visible(false);
                    _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                    Etapa3Reprovada();
                    PreecherResumoOrdemServico(r.Data);
                    BloquearAlteracaoOrcamento();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Orçamento rejeitado com sucesso!");
                    FecharTelaMotivoRejeicaoOrcamentoOrdemServico();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function AbrirTelaMotivoCancelamentoOrdemServico() {
    LimparCampos(_cancelarOrdemServico);
    _cancelarOrdemServico.Codigo.val(_ordemServico.Codigo.val());
    Global.abrirModal('knockoutCancelamento');
}

function FecharTelaMotivoCancelamentoOrdemServico() {
    LimparCampos(_cancelarOrdemServico);
    Global.fecharModal('knockoutCancelamento');
}

function CancelarOrdemServicoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar esta ordem de serviço?", function () {
        Salvar(_cancelarOrdemServico, "OrdemServico/Cancelar", function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_ordemServico, r);
                    PreecherResumoOrdemServico(r.Data);
                    SetarEtapaOrdemServico();

                    _CRUDOrdemServico.GerarOrdemServico.visible(false);
                    _CRUDOrdemServico.GerarLoteOrdemServico.visible(false);
                    _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(false);
                    _CRUDOrdemServico.AutorizarOrdemServico.visible(false);
                    _CRUDOrdemServico.RejeitarOrdemServico.visible(false);
                    _CRUDOrdemServico.FecharOrdemServico.visible(false);
                    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(false);
                    _CRUDOrdemServico.ReabrirOrdemServico.visible(false);
                    _CRUDOrdemServico.CancelarOrdemServico.visible(false);
                    _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                    _CRUDAberturaOrdemServico.AtualizarObservacao.visible(false);
                    _CRUDAberturaOrdemServico.AtualizarLocalManutencao.visible(false);

                    BloquearAlteracaoOrcamento();
                    BloquearAlteracaoServicos();

                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de serviço cancelada com sucesso!");

                    FecharTelaMotivoCancelamentoOrdemServico();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        }, sender);
    });
}

function DownloadOrdemServicoClick(e, sender) {
    executarDownload("OrdemServico/DownloadDetalhesOS", { Codigo: _ordemServico.Codigo.val() });
}

function GerarLoteOrdemServicoClick() {
    LimparCamposOrdemServicoLote();
    Global.abrirModal("divModalOrdemServicoLote");
}

function SugerirGrupoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor)
        return;

    let data = {
        Veiculo: _ordemServico.Veiculo.codEntity(),
        QuilometragemVeiculo: _ordemServico.QuilometragemVeiculo.val(),
        Equipamento: _ordemServico.Equipamento.codEntity(),
        Horimetro: _ordemServico.Horimetro.val()
    };

    executarReST("OrdemServico/SugerirGrupoServico", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _ordemServico.GrupoServico.codEntity(arg.Data.Codigo);
                _ordemServico.GrupoServico.val(arg.Data.Descricao);

                if (arg.Data.Codigo > 0) {
                    _ordemServico.LancarServicosManualmente.val(false);
                    _ordemServico.LancarServicosManualmente.enable(false);
                } else {
                    _ordemServico.LancarServicosManualmente.val(_CONFIGURACAO_TMS.LancarOsServicosDaOrdemDeServicoAutomaticamente);
                    _ordemServico.LancarServicosManualmente.enable(true);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ObterDadosUsuarioLogado() {
    if (_dadosUsuarioLogado != null)
        return;

    executarReST("OrdemServico/DadosUsuarioLogado", null, function (retorno) {
        if (retorno.Success) {
            _dadosUsuarioLogado = retorno.Data;
            PreencherDadosUsuarioLogado();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
};

function AtualizarObservacaoOrdemServicoClick(e, sender) {
    Salvar(_ordemServico, "OrdemServico/AtualizarObservacao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de serviço atualizada com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

function AtualizarLocalManutencaoOrdemServicoClick(e, sender) {
    Salvar(_ordemServico, "OrdemServico/AtualizarLocalManutencao", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Local de manutenção atualizado com sucesso!");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

////*******MÉTODOS*******

function RetornoConsultaLocalManutencao(dados) {
    _ordemServico.LocalManutencao.codEntity(dados.Codigo);
    _ordemServico.LocalManutencao.val(dados.Nome + " (" + dados.Localidade + ")");
}

function PreencherDadosUsuarioLogado() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor || _dadosUsuarioLogado == null)
        return;

    _ordemServico.LocalManutencao.codEntity(_dadosUsuarioLogado.CpfCnpjCliente);
    _ordemServico.LocalManutencao.val(_dadosUsuarioLogado.NomeCliente);
}

function ConfigurarPropriedadesCamposFornecedor() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor)
        return;

    _ordemServico.Tipo.enable(false);
    _ordemServico.Tipo.required(true);
    _ordemServico.Tipo.text("*Tipo:");
    _ordemServico.LocalManutencao.enable(false);
    _ordemServico.LocalManutencao.required(true);
    _ordemServico.LocalManutencao.text("*Local de Manutenção:");
    _ordemServico.GrupoServico.required(true);
    _ordemServico.GrupoServico.text("*Grupo de Serviço:");
    _CRUDOrdemServico.GerarLoteOrdemServico.visible(false);

    ObterDadosUsuarioLogado();
    _buscaGrupoServico.CarregarGrupoServicoFornecedor();
}

function EditarOrdemServico(ordemServicoGrid) {
    LimparCamposOrdemServico();

    if (CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO > 0 && ordemServicoGrid === null)
        _ordemServico.Codigo.val(CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO);
    else
        _ordemServico.Codigo.val(ordemServicoGrid.Codigo);

    BuscarPorCodigo(_ordemServico, "OrdemServico/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _CRUDOrdemServico.GerarOrdemServico.visible(false);
                _CRUDOrdemServico.GerarLoteOrdemServico.visible(false);
                _CRUDAberturaOrdemServico.AtualizarObservacao.visible(false);
                _CRUDAberturaOrdemServico.AtualizarLocalManutencao.visible(false);
                _fechamentoOrdemServico.OrdemServico.val(_ordemServico.Codigo.val());

                SetarEnableCamposKnockout(_ordemServico, false);

                PreecherResumoOrdemServico(r.Data);
                SetarEtapaOrdemServico();

                let situacao = _ordemServico.Situacao.val();
                switch (situacao) {
                    case EnumSituacaoOrdemServicoFrota.EmDigitacao:
                        _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(true);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.AgAutorizacao:
                        _CRUDOrdemServico.AutorizarOrdemServico.visible(true);
                        _CRUDOrdemServico.RejeitarOrdemServico.visible(true);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.EmManutencao:
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.FecharOrdemServico.visible(true);
                        _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(true);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.AgNotaFiscal:
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.FecharOrdemServico.visible(true);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.Finalizada:
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.AguardandoAprovacao:
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(false);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.FecharOrdemServico.visible(false);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    case EnumSituacaoOrdemServicoFrota.SemRegraAprovacao:
                        _CRUDOrdemServico.ReabrirOrdemServico.visible(true);
                        _CRUDOrdemServico.CancelarOrdemServico.visible(true);
                        _CRUDOrdemServico.FecharOrdemServico.visible(false);
                        _CRUDOrdemServico.DownloadOrdemServico.visible(true);
                        break;
                    default:
                        break;
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
                    if (situacao == EnumSituacaoOrdemServicoFrota.AgAutorizacao || situacao == EnumSituacaoOrdemServicoFrota.EmDigitacao) {
                        _CRUDAberturaOrdemServico.AtualizarLocalManutencao.visible(true);
                        _ordemServico.LocalManutencao.enable(true);
                    }
                }

                if (situacao !== EnumSituacaoOrdemServicoFrota.Finalizada && situacao !== EnumSituacaoOrdemServicoFrota.Cancelada &&
                    situacao !== EnumSituacaoOrdemServicoFrota.Rejeitada && situacao !== EnumSituacaoOrdemServicoFrota.AprovacaoRejeitada) {
                    _ordemServico.Observacao.enable(true);
                    _CRUDAberturaOrdemServico.AtualizarObservacao.visible(true);
                }

                window.history.replaceState(null, null, '/#Frota/OrdemServico');

                setTimeout(function () {
                    $("#" + _fechamentoOrdemServico.CodigoBarras.id).focus();
                }, 1000);


            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function LimparCamposOrdemServico() {
    _CRUDOrdemServico.GerarOrdemServico.visible(true);
    _CRUDOrdemServico.GerarLoteOrdemServico.visible(true);
    _CRUDOrdemServico.ConfirmarExecucaoServicos.visible(false);
    _CRUDOrdemServico.AutorizarOrdemServico.visible(false);
    _CRUDOrdemServico.RejeitarOrdemServico.visible(false);
    _CRUDOrdemServico.FecharOrdemServico.visible(false);
    _CRUDOrdemServico.LiberarVeiculoDaManutencao.visible(false);
    _CRUDOrdemServico.ReabrirOrdemServico.visible(false);
    _CRUDOrdemServico.CancelarOrdemServico.visible(false);
    _CRUDOrdemServico.DownloadOrdemServico.visible(false);
    _CRUDAberturaOrdemServico.AtualizarObservacao.visible(false);
    _CRUDAberturaOrdemServico.AtualizarLocalManutencao.visible(false);

    SetarEnableCamposKnockout(_ordemServico, true);
    _ordemServico.Numero.enable(false);

    LimparCampos(_ordemServico);
    LimparResumoOrdemServico();
    LimparCamposOrcamentoOrdemServico();
    LimparCamposServicoOrdemServico();
    LimparCamposFechamentoOrdemServico();
    LimparCamposAprovacaoOrdemServico();

    SetarEtapaInicioOrdemServico();

    ConfigurarPropriedadesCamposFornecedor();
    PreencherDadosUsuarioLogado();
}