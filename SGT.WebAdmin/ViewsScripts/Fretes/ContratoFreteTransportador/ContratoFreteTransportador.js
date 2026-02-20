/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoComplementoContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumTipoFechamentoFrete.js" />
/// <reference path="../../Enumeradores/EnumEstruturaTabela.js" />
/// <reference path="../../Enumeradores/EnumTipoDisponibilidadeContratoFrete.js" />
/// <reference path="Acordo.js" />
/// <reference path="AcordoValoresOutrosRecursos .js" />
/// <reference path="Veiculos.js" />

// #region Objetos Globais do Arquivo

var _gridContratoFreteTransportador;
var _contratoFreteTransportador;
var _CRUDContratoFreteTransportador;
var _pesquisaContratoFreteTransportador;
var _CAMPOS_BLOQUEADOS = false;
var _UsuarioLogado = null;
var _pesquisaHistoricoIntegracaoContratoFreteTransportador;
let _duplicarComNovaVigencia;
var _gridHistoricoIntegracaoContratoFreteTransportador;

//var _pesquisaSituacao = [
//    { text: 'Todas', value: '' },
//    { text: 'Ag. Aprovação', value: EnumSituacaoContratoFreteTransportador.AgAprovacao },
//    { text: 'Aprovado', value: EnumSituacaoContratoFreteTransportador.Aprovado },
//    { text: 'Rejeitado', value: EnumSituacaoContratoFreteTransportador.Rejeitado },
//    { text: 'Sem Regra', value: EnumSituacaoContratoFreteTransportador.SemRegra },
//    { text: 'Vencidos', value: EnumSituacaoContratoFreteTransportador.Vencido }
//];

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaHistoricoIntegracaoContratoFreteTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var PesquisaContratoFreteTransportador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", issue: 586 });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid() });
    this.TipoContratoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo do Contrato de Frete:", issue: 1605, idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", getType: typesKnockout.date });
    this.NumeroContrato = PropertyEntity({
        text: "Número do Contrato: "
    });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: ", issue: 557 });
    this.Situacao = PropertyEntity({ val: ko.observable(''), options: EnumSituacaoContratoFreteTransportador.ObterOpcoesPesquisa(), text: "Situação: ", issue: 1999 });
    this.Placa = PropertyEntity({ getType: typesKnockout.placa, text: "Placa:", val: ko.observable(""), def: "" })
    this.StatusAceite = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Status Aceite:"), idBtnSearch: guid() });
    this.SituacaoIntegracao = PropertyEntity({ val: ko.observable(EnumSituacaoIntegracaoCarga.Todas), def: EnumSituacaoIntegracaoCarga.DisponibilizarParaTodosVeiculos, options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), text: "Situação Integração: ", visible: ko.observable(true) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFreteTransportador.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ReenviarContratos = PropertyEntity({ eventClick: ReenviarContratosClick, type: types.event, text: "Reenviar Contratos", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CRUDContratoFreteTransportador = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Copiar = PropertyEntity({ eventClick: CopiarClick, type: types.event, text: "Gerar Cópia", visible: ko.observable(true) });
    this.Impressao = PropertyEntity({ eventClick: ImpressaoClick, type: types.event, text: "Gerar PDF", visible: ko.observable(true) });
    this.ImpressaoExcell = PropertyEntity({ eventClick: ImpressaoExcellClick, type: types.event, text: "Gerar Excel", visible: ko.observable(true) });
    this.EnviarEmail = PropertyEntity({ eventClick: EnviarEmailClick, type: types.event, text: "Enviar E-mail", visible: ko.observable(true) });
    this.VisualizarAlteracoes = PropertyEntity({ eventClick: function () { __AbrirModalAuditoria(); }, type: types.event, text: "Histórico Alterações", visible: ko.observable(false) });
};

var ContratoFreteTransportador = function () {
    var self = this;
    var fechamentoPorFaixaKm = (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorFaixaKm);

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoOriginario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.GeraNumeroSequenciar = PropertyEntity({ val: ko.observable(false), def: 0, getType: typesKnockout.bool });
    this.EncerrarVigenciaContratoOriginario = PropertyEntity({ val: ko.observable(false), def: 0, getType: typesKnockout.bool });
    this.Numero = PropertyEntity({ text: "Número do Contrato:", issue: 1929, val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), issue: 69, idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TipoContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tipo de Contrato de Frete:"), issue: 1605, idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Modelo de Documento Fiscal:"), idBtnSearch: guid(), required: fechamentoPorFaixaKm, enable: ko.observable(true), visible: ko.observable(fechamentoPorFaixaKm) });

    this.ContratoTransporteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Contrato Transportador:"), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });
    this.FiltrarPorTransportadorContrato = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", issue: 2, required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", issue: 2, required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, maxlength: 150, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, maxlength: 2000, enable: ko.observable(true) });
    this.TambemUtilizarContratoParaFiliaisDoTransportador = PropertyEntity({ val: ko.observable(false), def: false, text: "Também utilizar contrato de frete para as filiais deste transportador", enable: ko.observable(true) });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: ", issue: 557, enable: ko.observable(true) });
    this.TipoEmissaoComplemento = PropertyEntity({ val: ko.observable(EnumTipoEmissaoComplementoContratoFreteTransportador.PorTomador), options: EnumTipoEmissaoComplementoContratoFreteTransportador.obterOpcoes(), def: EnumTipoEmissaoComplementoContratoFreteTransportador.PorTomador, text: "*Tipo Emissão de Complemento: ", enable: ko.observable(true) });
    this.TipoDisponibilidadeContratoFrete = PropertyEntity({ val: ko.observable(EnumTipoDisponibilidadeContratoFrete.TodosModelosVeiculares), options: EnumTipoDisponibilidadeContratoFrete.obterOpcoes(), def: EnumTipoDisponibilidadeContratoFrete.TodosModelosVeiculares, text: "Disponibilidade do Contrato de Frete:" });
    this.OcorrenciaEmAberto = PropertyEntity({ def: 0, val: ko.observable(0) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoContratoFreteTransportador.Novo), def: EnumSituacaoContratoFreteTransportador.Novo, getType: typesKnockout.int });
    this.Copia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.CustoAcessorio = PropertyEntity({ text: "Custo Acessório:", required: false, maxlength: 150, enable: ko.observable(true) });
    this.TipoCusto = PropertyEntity({ text: "Tipo custo:", required: false, maxlength: 150, enable: ko.observable(true) });
    this.EstruturaTabela = PropertyEntity({ text: ko.observable("Estrutura de Tabela:"), val: ko.observable(EnumEstruturaTabela.CustoFixo), options: EnumEstruturaTabela.obterOpcoes(), def: EnumEstruturaTabela.CustoFixo, enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoLBC) });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.ClienteTomador = _clientes.ClienteTomador;

    this.DescontarValoresOutrasCargas = _outrosValores.DescontarValoresOutrasCargas;
    this.DiariaVeiculo = _outrosValores.DiariaVeiculo;
    this.QuinzenaVeiculo = _outrosValores.QuinzenaVeiculo;
    this.QuantidadeMotoristas = _outrosValores.QuantidadeMotoristas;
    this.DiariaMotorista = _outrosValores.DiariaMotorista;
    this.QuinzenaMotorista = _outrosValores.QuinzenaMotorista;
    this.OutrosValoresValorKmExcedente = _outrosValores.ValorKmExcedente;

    this.TipoFechamento = _acordoConfiguracao.TipoFechamento;
    this.TipoFranquia = _acordoConfiguracao.TipoFranquia;
    this.ValorPorMotorista = _acordoConfiguracao.ValorPorMotorista;
    this.ValorMensal = _acordoConfiguracao.ValorMensal;
    this.QuantidadeMensalCargas = _acordoConfiguracao.QuantidadeMensalCargas;
    this.DeduzirValorPorCarga = _acordoConfiguracao.DeduzirValorPorCarga;
    this.UtilizarValorFixoModeloVeicular = _acordoConfiguracao.UtilizarValorFixoModeloVeicular;
    this.ExigeTabelaFreteComValor = _acordoConfiguracao.ExigeTabelaFreteComValor;
    this.NaoEmitirComplementoFechamentoFrete = _acordoConfiguracao.NaoEmitirComplementoFechamentoFrete;

    this.UtilizarComponenteFreteValorContrato = _acordoConfiguracao.UtilizarComponenteFreteValorContrato;
    this.ComponenteFreteValorContrato = _acordoConfiguracao.ComponenteFreteValorContrato;

    this.TotalPorCavalo = _franquia.TotalPorCavalo;
    this.ValorKmExcedente = _franquia.ValorKmExcedente;
    this.ValorKmUtilizado = _franquia.ValorKmUtilizado;
    this.TotalKm = _franquia.TotalKm;
    this.ContratoMensal = _franquia.ContratoMensal;
    this.ValorPago = _franquia.ValorPago;

    this.PercentualRota = _adicionais.PercentualRota;
    this.QuantidadeEntregas = _adicionais.QuantidadeEntregas;
    this.CapacidadeOTM = _adicionais.CapacidadeOTM;
    this.DominioOTM = _adicionais.DominioOTM;
    this.PontoPlanejamentoTransporte = _adicionais.PontoPlanejamentoTransporte;
    this.TipoIntegracao = _adicionais.TipoIntegracao;
    this.IDExterno = _adicionais.IDExterno;
    this.StatusAceiteContrato = _adicionais.StatusAceiteContrato;
    this.GrupoCarga = _adicionais.GrupoCarga;
    this.GerenciarCapacidade = _adicionais.GerenciarCapacidade;

    this.TiposOcorrencia = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Clientes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.ValoresVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Veiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.Acordos = PropertyEntity({ type: types.listEntity, getType: typesKnockout.listEntity, def: [], val: ko.observableArray([]), codEntity: ko.observable(0) });
    this.TipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.CanalEntrega = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.TabelasFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });
    this.TipoEmissaoComplemento.val.subscribe(function (novoValor) {

        _acordoConfiguracao.ValorPorMotorista.visible(novoValor == EnumTipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista);
        _valoresOutrosRecursos.ListaValoresOutrosRecursos.visible(novoValor == EnumTipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista);
        RenderizaResumoVeiculos();
    })
};

var DuplicarComNovaVigencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Contrato = PropertyEntity({ val: ko.observable() });
    this.DataInicialContrato = PropertyEntity({ text: "*Data Inicial:", required: true, getType: typesKnockout.date, enable: ko.observable(true) });

    this.DuplicarComNovaVigencia = PropertyEntity({ eventClick: DuplicarComNovaVigenciaClick, type: types.event, text: "Duplicar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function ajustarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaContratoFreteTransportador.Transportador.text("Empresa/Filial:");
        _contratoFreteTransportador.Transportador.text("Empresa/Filial:");
        _contratoFreteTransportador.Transportador.required = false;

        _pesquisaContratoFreteTransportador.SituacaoIntegracao.visible(false);
        _pesquisaContratoFreteTransportador.ReenviarContratos.visible(false);
    }
}

function loadAuditoriaContrato() {

    HeaderAuditoria('ContratoFreteTransportador', _contratoFreteTransportador, null, {
        ContratoFreteTransportadorAcordo: 'Acordo',
        ContratoFreteTransportadorFilial: 'Filial',
        ContratoFreteTransportadorVeiculo: 'Veículo',
        DataAlteracao: 'Data Alteração',
        FranquiaContratoMensal: 'Franquia Contrato Mensal',
        FranquiaTotalKM: 'Franquia Total KM',
        FranquiaTotalPorCavalo: 'Franquia Total Por Cavalo',
        FranquiaValorKM: 'Franquia Valor KM',
        FranquiaValorKmExcedente: 'Franquia Valor Km Excedente',
        PeriodoAcordo: 'Período Acordo'
    }, null, true);
}

function LoadContratoFreteTransportador() {
    loadEtapasContratoTransportador();
    LoadTiposOcorrencia();
    LoadClientes();
    LoadValoresVeiculos();

    LoadOutrosValores();
    LoadFilial();
    LoadAcordo();
    LoadTipoOperacaoContrato();
    LoadFranquia();
    loadFranquiaPorFaixaKm();
    LoadAbas();
    loadAprovacao();
    loadAnexos();
    loadValorFreteMinimo();
    loadContratoFreteTransportadorIntegracoes();
    LoadAdicionais();
    LoadTipoCargaContrato();
    LoadCanalEntregaContrato();

    _pesquisaContratoFreteTransportador = new PesquisaContratoFreteTransportador();
    KoBindings(_pesquisaContratoFreteTransportador, "knockoutPesquisaContratoFreteTransportador", null, _pesquisaContratoFreteTransportador.Pesquisar.id);

    _CRUDContratoFreteTransportador = new CRUDContratoFreteTransportador();
    KoBindings(_CRUDContratoFreteTransportador, "knockoutCRUDContratoFreteTransportador");

    _contratoFreteTransportador = new ContratoFreteTransportador();
    KoBindings(_contratoFreteTransportador, "knockoutCadastroContratoFreteTransportador");

    LoadVeiculos();
    LoadTabelaFreteContrato();


    new BuscarTransportadores(_pesquisaContratoFreteTransportador.Transportador, null, null, true);
    new BuscarTransportadores(_contratoFreteTransportador.Transportador, null, null, true);

    new BuscarTipoContratoFrete(_pesquisaContratoFreteTransportador.TipoContratoFrete);
    new BuscarTipoContratoFrete(_contratoFreteTransportador.TipoContratoFrete);



    new BuscarModeloDocumentoFiscal(_contratoFreteTransportador.ModeloDocumentoFiscal);
    new BuscarContratosTransporteFrete(_contratoFreteTransportador.ContratoTransporteFrete, PreencherDataVigenciaTransportador, null, _contratoFreteTransportador.FiltrarPorTransportadorContrato);

    new BuscarStatusAssinaturaContrato(_pesquisaContratoFreteTransportador.StatusAceite);

    buscarContratoFreteTransportador();
    LimparCamposContratoFreteTransportador();
    verificarIntegracaoLBC(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);

    $("#contrato-tabs").tab();

    loadAuditoriaContrato();
    ajustarLayoutPorTipoServico();
    ControlarCampoNumeroContrato()
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function ReenviarContratosClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar todos as integrações com falha?", function () {
        executarReST("ContratoFreteTransportador/ReenviarIntegracaoContrato", null, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde as integrações serem finalizadas.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}
function DuplicarComNovaVigenciaClick(e) {
    EditarContratoFreteTransportador(_duplicarComNovaVigencia.Contrato.val(), true, _duplicarComNovaVigencia.DataInicialContrato.val());
    Global.fecharModal("divModalDuplicarComNovaVigencia");
}

function AdicionarClick(e, sender) {
    if (!validarObrigatoriedadesContrato())
        return;

    if (!validarVeiculosAdicionados())
        return;
    ObterValorTabelaFrete();
    executarReST("ContratoFreteTransportador/Adicionar", obterContratoFreteTransportadorSalvar(), function (arg) {
        if (arg.Success) {
            if (arg.Data && typeof arg.Data == "object")
                exibirMensagem(tipoMensagem.aviso, arg.Data.Bag, arg.Data.Msg);
            else if (arg.Data) {
                _contratoFreteTransportador.Codigo.val(arg.Data);
                EnviarArquivosAnexados();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridContratoFreteTransportador.CarregarGrid();
                LimparCamposContratoFreteTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, sender, ExibirCamposObrigatorios);
}

function AtualizarClick(e, sender) {
    var _handleUpdate = function () {
        if (!validarObrigatoriedadesContrato())
            return;
        if (!validarVeiculosAdicionados())
            return;
        ObterValorTabelaFrete();
        executarReST("ContratoFreteTransportador/Atualizar", obterContratoFreteTransportadorSalvar(), function (arg) {
            if (arg.Success) {
                if (arg.Data && typeof arg.Data == "object")
                    exibirMensagem(tipoMensagem.aviso, arg.Data.Bag, arg.Data.Msg);
                else if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                    _gridContratoFreteTransportador.CarregarGrid();
                    LimparCamposContratoFreteTransportador();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }, sender, ExibirCamposObrigatorios);
    };

    // Valida se existe ocorrencia aberta
    if (_contratoFreteTransportador.OcorrenciaEmAberto.val() > 0)
        exibirConfirmacao("Ocorrência Aberta", "A ocorrência " + _contratoFreteTransportador.OcorrenciaEmAberto.val() + " não está finalizada e a alteração desse Contrato de Frete pode influenciar nos resultados.", _handleUpdate);
    else
        _handleUpdate();
}

function CancelarClick(e) {
    LimparCamposContratoFreteTransportador();
}

function CopiarClick() {
    ControleDeCampos(true);

    limparAnexosTela();

    _contratoFreteTransportador.Codigo.val(0);
    _contratoFreteTransportador.CodigoOriginario.val(0);
    _contratoFreteTransportador.EncerrarVigenciaContratoOriginario.val(false);
    _contratoFreteTransportador.Numero.val(_contratoFreteTransportador.Numero.def);
    _contratoFreteTransportador.DataInicial.val(_contratoFreteTransportador.DataInicial.def);
    _contratoFreteTransportador.DataFinal.val(_contratoFreteTransportador.DataFinal.def);
    _contratoFreteTransportador.Descricao.val(_contratoFreteTransportador.Descricao.def);
    _contratoFreteTransportador.Transportador.val(_contratoFreteTransportador.Transportador.def);
    _contratoFreteTransportador.Transportador.codEntity(_contratoFreteTransportador.Transportador.defCodEntity);
    _contratoFreteTransportador.Observacao.val(_contratoFreteTransportador.Observacao.def);
    Etapa2Desabilitada();
    Etapa3Desabilitada();

    _CRUDContratoFreteTransportador.Atualizar.visible(false);
    _CRUDContratoFreteTransportador.Excluir.visible(false);
    _CRUDContratoFreteTransportador.Copiar.visible(false);
    _CRUDContratoFreteTransportador.Impressao.visible(false);
    _CRUDContratoFreteTransportador.EnviarEmail.visible(false);
    _CRUDContratoFreteTransportador.Adicionar.visible(true);
}

function EnviarEmailClick() {
    var dados = {
        Codigo: _contratoFreteTransportador.Codigo.val()
    };
    executarReST("ContratoFreteTransportador/EnviarEmail", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "E-mail enviado com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExcluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o contrato de frete " + _contratoFreteTransportador.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_contratoFreteTransportador, "ContratoFreteTransportador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridContratoFreteTransportador.CarregarGrid();
                    LimparCamposContratoFreteTransportador();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function ImpressaoClick() {
    executarDownload("ContratoFreteTransportador/Imprimir", { Codigo: _contratoFreteTransportador.Codigo.val(), Excell: false });
}

function ImpressaoExcellClick() {
    executarDownload("ContratoFreteTransportador/Imprimir", { Codigo: _contratoFreteTransportador.Codigo.val(), Excell: true });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function controlarCamposHabilitadosPorKnockout(knockout, habilitar) {
    for (var i in knockout) {
        var propertyEntity = knockout[i];

        if (propertyEntity.enable instanceof Function)
            propertyEntity.enable(habilitar);
    }
}

function controlarBotoesVisiveisPorKnockout(knockout, habilitar) {
    for (var i in knockout) {
        var propertyEntity = knockout[i];

        if ((propertyEntity.visible instanceof Function) && (propertyEntity.type == types.event))
            propertyEntity.visible(habilitar);
    }
}

function ExibirHistoricoIntegracaoContratoFreteTransportador(integracao) {
    HistoricoIntegracaoContratoFreteTransportador(integracao);
    Global.abrirModal("divModalHistoricoContratoFreteTransportador");
}

function ExibirModalDuplicarComNovaVigencia(contrato) {
    if (!_duplicarComNovaVigencia) {
        _duplicarComNovaVigencia = new DuplicarComNovaVigencia();
        KoBindings(_duplicarComNovaVigencia, "knoutDuplicarComNovaVigencia");
    }
    LimparCampos(_duplicarComNovaVigencia);

    let hoje = new Date();
    let dataInicialContrato = Global.criarData(contrato.DataInicial).getDate();
    let menorData = hoje;

    if (hoje <= dataInicialContrato)
        menorData = dataInicialContrato;

    let dataDef = new Date();
    dataDef.setDate(hoje.getDate() + 1);

    _duplicarComNovaVigencia.DataInicialContrato.minDate(Global.criarData(formatarDataParaString(menorData)).getDate())
    _duplicarComNovaVigencia.DataInicialContrato.val(formatarDataParaString(dataDef));
    _duplicarComNovaVigencia.Contrato.val(contrato);

    Global.abrirModal("divModalDuplicarComNovaVigencia");
}

function HistoricoIntegracaoContratoFreteTransportador(data) {
    _pesquisaHistoricoIntegracaoContratoFreteTransportador = new PesquisaHistoricoIntegracaoContratoFreteTransportador();
    _pesquisaHistoricoIntegracaoContratoFreteTransportador.Codigo.val(data.Codigo);

    var download = { descricao: "Download", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoContratoFreteTransportador, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoContratoFreteTransportador = new GridView("tblHistoricoIntegracaoContratoFreteTransportador", "ContratoFreteTransportador/ConsultarHistoricoIntegracao", _pesquisaHistoricoIntegracaoContratoFreteTransportador, menuOpcoes, { column: 1, dir: orderDir.desc });
    _gridHistoricoIntegracaoContratoFreteTransportador.CarregarGrid();
}

function formatarDataParaString(data) {
    let dia = data.getDate().toString().padStart(2, '0');
    let mes = (data.getMonth() + 1).toString().padStart(2, '0');
    let ano = data.getFullYear();
    return `${dia}/${mes}/${ano}`;
}

function DownloadArquivosHistoricoIntegracaoContratoFreteTransportador(historicoConsulta) {
    executarDownload("ContratoFreteTransportador/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function ReenviarContratoFreteTransportador(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja enviar todos as integrações deste contrato?", function () {
        executarReST("ContratoFreteTransportador/ReenviarIntegracao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde as integrações serem finalizadas.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function EditarContratoFreteTransportador(ContratoFreteTransportadorGrid, duplicar, dataInicialContrato) {
    LimparCamposContratoFreteTransportador();

    executarReST("ContratoFreteTransportador/BuscarPorCodigo", { Codigo: ContratoFreteTransportadorGrid.Codigo, Duplicar: duplicar, DataInicialContrato: dataInicialContrato }, function (arg) {
        PreencherObjetoKnout(_contratoFreteTransportador, arg);

        _pesquisaContratoFreteTransportador.ExibirFiltros.visibleFade(false);

        if (arg.Data.Situacao != EnumSituacaoContratoFreteTransportador.Aprovado && arg.Data.Situacao != EnumSituacaoContratoFreteTransportador.Vencido)
            ControleDeCampos(false);

        if (arg.Data.BloqueioEdicao)
            ControleDeCampos(false);

        if (duplicar)
            ControleDeCampos(true);

        if (arg.Data.Situacao == EnumSituacaoContratoFreteTransportador.AgAprovacao)
            ControleDeCampos(true);

        EditarClientes(arg.Data);
        EditarTiposOcorrencia(arg.Data);
        EditarValoresVeiculos(arg.Data);
        EditarVeiculos(arg.Data);
        EditarFilial(arg.Data);
        CarregarAcordos(arg.Data);
        EditarFranquia(arg.Data);
        EditarTipoOperacaoContrato(arg.Data);
        EditarTipoCargaContrato(arg.Data);
        EditarCanalEntregaContrato(arg.Data);
        EditarAdicionais(arg.Data);
        ListarAprovacoes(arg.Data);
        preencherFranquiaPorFaixaKm(arg.Data.FaixasKmFranquia);
        preencherValorFreteMinimo(arg.Data.ValorFreteMinimo);
        preencherValoresOutrosRecursos(arg.Data.ValoresOutrosRecursos);
        CarregarListaGridTabela(arg.Data.TabelasFrete);
        _anexos.Anexos.val(arg.Data.Anexos);
        _contratoFreteTransportador.EncerrarVigenciaContratoOriginario.val(arg.Data.EncerrarVigenciaContratoOriginario);
        _contratoFreteTransportador.Numero.enable(duplicar);
        _CRUDContratoFreteTransportador.Atualizar.visible(!duplicar);
        _CRUDContratoFreteTransportador.Cancelar.visible(true);
        _CRUDContratoFreteTransportador.Excluir.visible(!duplicar);
        _CRUDContratoFreteTransportador.Copiar.visible(!duplicar);
        _CRUDContratoFreteTransportador.Impressao.visible(true);
        _CRUDContratoFreteTransportador.EnviarEmail.visible(false);
        _CRUDContratoFreteTransportador.Adicionar.visible(duplicar);
        _veiculos.ImportarVeiculo.visible(false);

        if (!arg.Data.TipoContratoFrete.TipoAditivo)
            _CRUDContratoFreteTransportador.ImpressaoExcell.visible(true);

        ControlarCampoNumeroContrato()

        if (_CAMPOS_BLOQUEADOS) {
            _CRUDContratoFreteTransportador.Atualizar.visible(false);
            _CRUDContratoFreteTransportador.Excluir.visible(false);
        }

        SetarEtapasContratoTransportador();

        if (_UsuarioLogado != null && _UsuarioLogado.PerfilAcessoAdmin)
            _CRUDContratoFreteTransportador.VisualizarAlteracoes.visible(true);
        else
            _CRUDContratoFreteTransportador.VisualizarAlteracoes.visible(false);

        //Verificar se tem permissão personalizada para habilitar as abas
        GerenciarVisibilidadeAbas();
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function buscarContratoFreteTransportador() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { EditarContratoFreteTransportador(data, false); }, tamanho: "10", icone: "" };
    var duplicar = { descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (data) { EditarContratoFreteTransportador(data, true); }, tamanho: "10", icone: "", visibilidade: permitirDuplicarContrato };
    var duplicarComNovaVigencia = { descricao: "Duplicar com nova vigência", id: guid(), evento: "onclick", metodo: function (data) { ExibirModalDuplicarComNovaVigencia(data); }, tamanho: "10", icone: "", visibilidade: permitirDuplicarContratoComVigencia };
    var Reenviar = { descricao: "Reenviar", id: guid(), evento: "onclick", metodo: function (data) { ReenviarContratoFreteTransportador(data); }, tamanho: "10", icone: "" };
    var historico = { descricao: "Histórico Integração", id: guid(), evento: "onclick", metodo: function (data) { ExibirHistoricoIntegracaoContratoFreteTransportador(data); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar], tamanho: 10 };
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
        menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar, duplicarComNovaVigencia, historico, Reenviar], tamanho: 10 };
    _gridContratoFreteTransportador = new GridView(_pesquisaContratoFreteTransportador.Pesquisar.idGrid, "ContratoFreteTransportador/Pesquisa", _pesquisaContratoFreteTransportador, menuOpcoes, null);
    _gridContratoFreteTransportador.CarregarGrid();
}

function ControleDeCampos(permitirEdicao) {
    _CAMPOS_BLOQUEADOS = !permitirEdicao;

    controlarCamposHabilitadosPorKnockout(_clientes, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_valoresVeiculos, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_veiculos, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_outrosValores, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_acordo, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_valoresOutrosRecursos, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_acordoConfiguracao, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_tipoOperacaoContrato, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_tipoCargaContrato, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_contratoFreteTransportador, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_franquia, permitirEdicao);
    controlarCamposHabilitadosPorKnockout(_adicionais, permitirEdicao);

    _acordoConfiguracao.ValorMensal.enableout(permitirEdicao && (_CONFIGURACAO_TMS.TipoFechamentoFrete == EnumTipoFechamentoFrete.FechamentoPorFaixaKm));
    _filial.Adicionar.visible(permitirEdicao);
    _acordo.Adicionar.visible(permitirEdicao);
    _acordo.Cancelar.visible(permitirEdicao);
    _clientes.Adicionar.visible(permitirEdicao);
    _tiposOcorrencia.Adicionar.visible(permitirEdicao);
    _tipoOperacaoContrato.AdicionarTipoOperacao.visible(permitirEdicao);
    _tipoCargaContrato.AdicionarTipoCarga.visible(permitirEdicao);
    _franquia.AdicionarTipoCarga.visible(permitirEdicao);

    controlarEdicaoFranquiaPorFaixaKm();
    controlarEdicaoValorFreteMinimo();
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios!");
}

function LimparCamposContratoFreteTransportador() {
    ControleDeCampos(true);

    _CRUDContratoFreteTransportador.Atualizar.visible(false);
    _CRUDContratoFreteTransportador.Cancelar.visible(false);
    _CRUDContratoFreteTransportador.Excluir.visible(false);
    _CRUDContratoFreteTransportador.Copiar.visible(false);
    _CRUDContratoFreteTransportador.Impressao.visible(false);
    _CRUDContratoFreteTransportador.ImpressaoExcell.visible(false);
    _CRUDContratoFreteTransportador.EnviarEmail.visible(false);
    _CRUDContratoFreteTransportador.Adicionar.visible(true);
    _CRUDContratoFreteTransportador.VisualizarAlteracoes.visible(false);
    _veiculos.ImportarVeiculo.visible(true);

    LimparCampos(_contratoFreteTransportador);
    LimparCamposTiposOcorrencia();
    LimparCamposClientes();
    LimparCamposValoresVeiculos();
    LimparCamposTipoOperacaoContrato();
    LimparCamposTipoCargaContrato();
    LimparCamposCanalEntregaContrato();
    LimparCamposVeiculos();
    LimparCamposOutrosValores();
    LimparCamposFilial();
    LimparCamposAcordo();
    limparCamposValoresOutrosRecurso();
    limparCamposFranquiaPorFaixaKm();
    limparCamposFranquia();
    LimparCamposAprovacao();
    limparCamposValorFreteMinimo();
    LimparCamposAdicionais();
    limparAnexosTela();
    LimparTabelasFreteCliente();

    SetarEtapaInicio();

    $("#knockoutCadastroContratoFreteTransportador").click();
    $(".li-controle").hide();

    //Verificar se tem permissão personalizada para habilitar as abas
    GerenciarVisibilidadeAbas();
    ControlarCampoNumeroContrato();
}

function obterContratoFreteTransportadorSalvar() {
    var contratoFreteTransportador = RetornarObjetoPesquisa(_contratoFreteTransportador);

    preencherFranquiaPorFaixaKmSalvar(contratoFreteTransportador);
    preencherValorFreteMinimoSalvar(contratoFreteTransportador);
    preencherValoresOutrosRecursoSalvar(contratoFreteTransportador);

    return contratoFreteTransportador;
}

function permitirDuplicarContrato(contrato) {
    return contrato.PermitirDuplicar;
}

function permitirDuplicarContratoComVigencia(contrato) {
    return contrato.PermitirDuplicarComNovaVigencia;
}

function validarObrigatoriedadesContrato() {
    var valido = true;

    if (!ValidarCamposObrigatorios(_contratoFreteTransportador)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "É necessário informar os campos obrigatórios.");
        return false;
    }

    if (_CONFIGURACAO_TMS.ObrigatorioInformarDadosContratoFrete) {
        var resumo = ExtraiResumoAcordos();

        if (resumo.Resumo.length <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Acordo Obrigatório", "É obrigatório preencher o acordo do contrato.");
            return false;
        }

        if (_veiculos.Veiculos.val() == null || _veiculos.Veiculos.val().length <= 0) {
            exibirMensagem(tipoMensagem.atencao, "Veículos Obrigatórios", "É obrigatório informar os veículos do contrato.");
            valido = false;
        } else if (_acordoConfiguracao.TipoFranquia.val() != EnumPeriodoAcordoContratoFreteTransportador.NaoPossui && (_franquia.TotalPorCavalo.val() == "" || _franquia.TotalPorCavalo.val() == "0" || _franquia.TotalPorCavalo.val() == null)) {
            exibirMensagem(tipoMensagem.atencao, "Franquia Obrigatória", "É obrigatório preencher os dados da franquia do contrato.");
            valido = false;
        }
    }
    return valido;

}

function ControlarCampoNumeroContrato() {
    _contratoFreteTransportador.Numero.enable(!_CONFIGURACAO_TMS.GeraNumeroSequenciar);
}

function PreencherDataVigenciaTransportador(r) {
    _contratoFreteTransportador.ContratoTransporteFrete.codEntity(r.Codigo);
    _contratoFreteTransportador.ContratoTransporteFrete.val(r.Descricao);
    _contratoFreteTransportador.Transportador.val(r.Transportador);
    _contratoFreteTransportador.Transportador.codEntity(r.CodigoTransportador);
    _contratoFreteTransportador.DataInicial.val(r.DataInicio);
    _contratoFreteTransportador.DataFinal.val(r.DataFim);
}

function verificarIntegracaoLBC(config) {
    if (config) {
        _contratoFreteTransportador.TipoContratoFrete.text("*Tipo de Contrato de Frete:");

        _contratoFreteTransportador.ContratoTransporteFrete.text("*Contrato Transportador:");

        _contratoFreteTransportador.EstruturaTabela.text("*Estrutura de Tabela:");

        _contratoFreteTransportador.TipoFranquia.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.TipoFranquia.text("*Tipo da Franquia:");
        _contratoFreteTransportador.TipoFranquia.val("");
        _contratoFreteTransportador.TipoFranquia.def("");

        _contratoFreteTransportador.QuantidadeEntregas.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.QuantidadeEntregas.text("*Quantidade Entregas:");

        _contratoFreteTransportador.CapacidadeOTM.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.CapacidadeOTM.text("*Capacidade OTM:");
        _contratoFreteTransportador.CapacidadeOTM.val(0);
        _contratoFreteTransportador.CapacidadeOTM.def(0);

        _contratoFreteTransportador.DominioOTM.text("*Dominio OTM");
        _contratoFreteTransportador.DominioOTM.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);

        _contratoFreteTransportador.TipoIntegracao.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.TipoIntegracao.text("*Tipo Integração:");
        _contratoFreteTransportador.TipoIntegracao.val(EnumTipoIntegracaoUnilever.None);
        _contratoFreteTransportador.TipoIntegracao.def(EnumTipoIntegracaoUnilever.None);

        _contratoFreteTransportador.GrupoCarga.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.GrupoCarga.text("*Grupo de Carga:");
        _contratoFreteTransportador.GrupoCarga.options(EnumTipoGrupoCarga.obterOpcoesPesquisaIntegracaoLBC());
        _contratoFreteTransportador.GrupoCarga.val(null);
        _contratoFreteTransportador.GrupoCarga.def(null);

        _contratoFreteTransportador.PontoPlanejamentoTransporte.required(_CONFIGURACAO_TMS.PossuiIntegracaoLBC);
        _contratoFreteTransportador.PontoPlanejamentoTransporte.text("*Ponto planejamento transporte:");
        _contratoFreteTransportador.PontoPlanejamentoTransporte.options(EnumPontoPlanejamentoTransporte.obterOpcoesPesquisaIntegracaoLBC());
        _contratoFreteTransportador.PontoPlanejamentoTransporte.val(EnumPontoPlanejamentoTransporte.Selecione);
        _contratoFreteTransportador.PontoPlanejamentoTransporte.def(EnumPontoPlanejamentoTransporte.Selecione);
    }
}

// #endregion Funções Privadas
