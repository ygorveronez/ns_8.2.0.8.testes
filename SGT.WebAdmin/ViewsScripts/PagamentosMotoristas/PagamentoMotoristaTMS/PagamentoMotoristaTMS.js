/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumEtapaPagamentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Chamado.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="EtapasPagamentoMotorista.js" />
/// <reference path="AutorizacaoPagamentoMotorista.js" />
/// <reference path="ResumoPagamentoMotorista.js" />
/// <reference path="IntegracaoPagamentoMotorista.js" />
/// <reference path="PagamentoMotoristaTMSAnexo.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoMotorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPagamentoMotorista;
var _pagamentoMotorista;
var _CRUDPagamentoMotorista;
var _pesquisaPagamentoMotorista;

var _etapaPagamentoMotorista = [
    { text: "Todos", value: EnumEtapaPagamentoMotorista.Todos },
    { text: "Iniciada", value: EnumEtapaPagamentoMotorista.Iniciada },
    { text: "Ag. Autorizacao", value: EnumEtapaPagamentoMotorista.AgAutorizacao },
    { text: "Integração", value: EnumEtapaPagamentoMotorista.Integracao }
];

var PesquisaPagamentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número do Pagamento:", val: ko.observable(""), def: "" });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(_ConfiguracaoPaginacaoDataLimite ?? "") });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.TipoPagamentoMotorista = PropertyEntity({ val: ko.observable(EnumTipoPagamentoMotorista.Todos), options: EnumTipoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumTipoPagamentoMotorista.Todos, text: "Tipo de Pagamento: " });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamentoMotorista.Todas), options: EnumSituacaoPagamentoMotorista.obterOpcoesPesquisa(), def: EnumSituacaoPagamentoMotorista.Todas, text: "Situação: " });
    this.Etapa = PropertyEntity({ val: ko.observable(EnumEtapaPagamentoMotorista.Todos), options: _etapaPagamentoMotorista, def: EnumEtapaPagamentoMotorista.Todos, text: "Etapa: " });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.TipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Pagamento:", type: types.multiplesEntities, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Empresa/Filial: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPagamentoMotorista.CarregarGrid();
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

var PagamentoMotorista = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoSituacao = PropertyEntity({ text: "Situação: ", visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamentoMotorista.Todas), def: EnumSituacaoPagamentoMotorista.Todas, getType: typesKnockout.int });
    this.NumeroPagamentoMotorista = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: "Número: ", def: "", enable: false });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Carga:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 195 });
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Chamado:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Motorista:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataPagamentoMotorista = PropertyEntity({ text: "*Data do Pagamento: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), required: true, enable: ko.observable(true), defEnable: true });
    this.TipoPagamentoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo do Pagamento:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataVencimentoTituloPagar = PropertyEntity({ text: "*Vencimento Tit. Pagar: ", getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), defEnable: true, visible: ko.observable(false) });

    this.PessoaTituloPagar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Fornecedor:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Terceiro:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.Valor = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, required: true, enable: ko.observable(true), visible: ko.observable(true), val: ko.observable("0,00"), def: "0,00" });
    this.SaldoDescontado = PropertyEntity({ text: "*Saldo a Descontar: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("0,00"), def: "0,00" });
    this.TotalPagamento = PropertyEntity({ text: "*Total: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("0,00"), def: "0,00" });
    this.DataEfetivacao = PropertyEntity({ text: "Data de Efetivação: ", getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: false, enable: ko.observable(true), defEnable: true });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(_ConfiguracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista), text: "Empresa/Filial: ", idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(true) });

    this.SaldoDiariaMotorista = PropertyEntity({ text: "Saldo Motorista: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("0,00"), def: "0,00" });    

    this.ValorINSS = PropertyEntity({ text: "Valor INSS: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("0,00"), def: "0,00" });
    this.ValorSEST = PropertyEntity({ text: "Valor SEST: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("0,00"), def: "0,00" });
    this.ValorSENAT = PropertyEntity({ text: "Valor SENAT: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("0,00"), def: "0,00" });
    this.ValorIRRF = PropertyEntity({ text: "Valor IRRF: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("0,00"), def: "0,00" });
    this.ValorLiquido = PropertyEntity({ text: "Valor Líquido: ", getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(false), val: ko.observable("0,00"), def: "0,00" });
    this.ReterImpostoPagamentoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.bool, val: ko.observable(false) });

    this.PlanoDeContaDebito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Plano de Saída:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PlanoDeContaCredito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Plano de Entrada:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 5000, required: false, enable: ko.observable(true) });

    this.SolicitacaoCredito = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.dynamic });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.ReterImpostoPagamentoMotorista.val.subscribe(function (novoValor) {
        if (novoValor) {
            _pagamentoMotorista.ValorINSS.visible(true);
            _pagamentoMotorista.ValorSEST.visible(true);
            _pagamentoMotorista.ValorSENAT.visible(true);
            _pagamentoMotorista.ValorIRRF.visible(true);
            _pagamentoMotorista.ValorLiquido.visible(true);
        } else {
            _pagamentoMotorista.ValorINSS.visible(false);
            _pagamentoMotorista.ValorSEST.visible(false);
            _pagamentoMotorista.ValorSENAT.visible(false);
            _pagamentoMotorista.ValorIRRF.visible(false);
            _pagamentoMotorista.ValorLiquido.visible(false);
        }
    });
};

var CRUDPagamentoMotorista = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparPagamentoMotoristaClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.Reverter = PropertyEntity({ eventClick: ReverterPagamentoMotoristaClick, type: types.event, text: "Reverter Pagamento", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarPagamentoMotoristaClick, type: types.event, text: "Cancelar Pagamento", visible: ko.observable(false) });
    this.ImprimirRecibo = PropertyEntity({ eventClick: ImprimirReciboClick, type: types.event, text: "Imprimir Recibo", visible: ko.observable(false) });
    this.ConfirmarPagamentoMotorista = PropertyEntity({ eventClick: confirmarPagamentoMotoristaClick, type: types.event, text: "Confirmar Pagamento", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.GerarNovaPagamentoMotorista = PropertyEntity({ eventClick: gerarNovaPagamentoMotoristaClick, type: types.event, text: "Gerar Novo Pagamento", visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: buscarRegrasPagamentoMotoristaClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "PagamentoMotoristaTMS/Importar",
        UrlConfiguracao: "PagamentoMotoristaTMS/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O048_ImportacaoPagamentoMotorista,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: false
            };
        },
        CallbackImportacao: function () {
            _gridPagamentoMotorista.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadPagamentoMotoristaTMS() {
    _pesquisaPagamentoMotorista = new PesquisaPagamentoMotorista();
    KoBindings(_pesquisaPagamentoMotorista, "knockoutPesquisaPagamentoMotorista", false, _pesquisaPagamentoMotorista.Pesquisar.id);

    new BuscarTransportadores(_pesquisaPagamentoMotorista.Empresa, null, null, true);
    new BuscarFuncionario(_pesquisaPagamentoMotorista.Operador);
    new BuscarMotorista(_pesquisaPagamentoMotorista.Motorista, RetornoMotoristaPesquisa);
    new BuscarPagamentoMotoristaTipo(_pesquisaPagamentoMotorista.TipoPagamento);

    _pagamentoMotorista = new PagamentoMotorista();
    KoBindings(_pagamentoMotorista, "knockoutCadastroPagamentoMotorista");

    new BuscarCargas(_pagamentoMotorista.Carga, RetornoCarga);
    new BuscarTodosChamadosParaOcorrencia(_pagamentoMotorista.Chamado, RetornoChamado);
    new BuscarMotorista(_pagamentoMotorista.Motorista, RetornoMotorista, null, null, null, _CONFIGURACAO_TMS.NaoGerarPagamentoParaMotoristaTerceiro);
    new BuscarPagamentoMotoristaTipo(_pagamentoMotorista.TipoPagamentoMotorista, RetornoPagamentoMotoristaTip);
    new BuscarPlanoConta(_pagamentoMotorista.PlanoDeContaDebito, "Selecione a Conta Analítica de Saída", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pagamentoMotorista.PlanoDeContaCredito, "Selecione a Conta Analítica de Entrada", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarClientes(_pagamentoMotorista.PessoaTituloPagar);
    new BuscarClientes(_pagamentoMotorista.Terceiro);
    new BuscarTransportadores(_pagamentoMotorista.Empresa, null, null, true);

    _CRUDPagamentoMotorista = new CRUDPagamentoMotorista();
    KoBindings(_CRUDPagamentoMotorista, "knockoutCRUDCadastroPagamentoMotorista");

    HeaderAuditoria("PagamentoMotoristaTMS", _pagamentoMotorista);

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _pagamentoMotorista.MoedaCotacaoBancoCentral.visible(true);
        _pagamentoMotorista.DataBaseCRT.visible(true);
        _pagamentoMotorista.ValorMoedaCotacao.visible(true);
        _pagamentoMotorista.ValorOriginalMoedaEstrangeira.visible(true);
    }

    loadEtapaPagamentoMotorista();
    loadResumoPagamentoMotorista();
    loadPagamentoMotoristaAutorizacao();
    loadPagamentoMotoristaAnexo();

    limparCamposPagamentoMotorista();
    buscarPagamentoMotoristas();
}

function RetornoCarga(data) {
    _pagamentoMotorista.Carga.val(data.CodigoCargaEmbarcador);
    _pagamentoMotorista.Carga.codEntity(data.Codigo);
    retornarMotorista(data.Codigo, 0);
    retornarEmpresa(data.Codigo);
}

function retornarEmpresa(codigoCarga) {
    executarReST("PagamentoMotoristaTMS/ObterEmpresa", { Carga: codigoCarga }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                if (data.Codigo > 0) {
                    _pagamentoMotorista.Empresa.codEntity(data.Codigo);
                    _pagamentoMotorista.Empresa.val(data.Descricao);
                    _pagamentoMotorista.Empresa.enable(false);
                } else {
                    _pagamentoMotorista.Empresa.codEntity(0);
                    _pagamentoMotorista.Empresa.val("");
                    _pagamentoMotorista.Empresa.enable(_ConfiguracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RetornoChamado(data) {
    _pagamentoMotorista.Chamado.val(data.Numero);
    _pagamentoMotorista.Chamado.codEntity(data.Codigo);
    _pagamentoMotorista.Carga.val(data.Carga);
    _pagamentoMotorista.Carga.codEntity(data.CodigoCarga);
    retornarMotorista(data.CodigoCarga, 0);
}

function retornarMotorista(codigoCarga, codigoChamado) {
    executarReST("PagamentoMotoristaTMS/ObterMotorista", { Carga: codigoCarga, Chamado: codigoChamado }, function (r) {
        if (r.Success) {
            if (r.Data) {
                var data = r.Data;
                if (data.Codigo > 0) {
                    _pagamentoMotorista.Motorista.codEntity(data.Codigo);
                    _pagamentoMotorista.Motorista.val(data.Nome);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function RetornoPagamentoMotoristaTip(data) {
    _pagamentoMotorista.TipoPagamentoMotorista.codEntity(data.Codigo);
    _pagamentoMotorista.TipoPagamentoMotorista.val(data.Descricao);

    if (data.GerarMovimentoAutomatico) {
        if (!_ConfiguracaoGeral.NaoCarregarPlanoEntradaSaidaTipoPagamento && (_pagamentoMotorista.PlanoDeContaDebito.codEntity() === 0 || _pagamentoMotorista.PlanoDeContaDebito.codEntity() === "" || _pagamentoMotorista.PlanoDeContaDebito.val() === "" || _pagamentoMotorista.PlanoDeContaDebito.codEntity() != data.CodigoPlanoDeContaCredito)) {
            if (data.CodigoPlanoDeContaCredito > 0) {
                _pagamentoMotorista.PlanoDeContaDebito.codEntity(data.CodigoPlanoDeContaCredito);
                _pagamentoMotorista.PlanoDeContaDebito.val(data.DescricaoPlanoDeContaCredito);
            } else {
                LimparCampoEntity(_pagamentoMotorista.PlanoDeContaDebito);
            }
        }

        if (!_ConfiguracaoGeral.NaoCarregarPlanoEntradaSaidaTipoPagamento && (_pagamentoMotorista.PlanoDeContaCredito.codEntity() === 0 || _pagamentoMotorista.PlanoDeContaCredito.codEntity() === "" || _pagamentoMotorista.PlanoDeContaCredito.val() === "" || _pagamentoMotorista.PlanoDeContaCredito.codEntity() != data.CodigoPlanoDeContaDebito)) {
            if (data.CodigoPlanoDeContaDebito > 0) {
                _pagamentoMotorista.PlanoDeContaCredito.codEntity(data.CodigoPlanoDeContaDebito);
                _pagamentoMotorista.PlanoDeContaCredito.val(data.DescricaoPlanoDeContaDebito);
            } else {
                LimparCampoEntity(_pagamentoMotorista.PlanoDeContaCredito);
            }
        }
    }

    if (data.TipoPagamentoMotorista == EnumTipoPagamentoMotorista.Terceiro)
        _pagamentoMotorista.Terceiro.visible(true);
    else
        _pagamentoMotorista.Terceiro.visible(false);

    if (data.PessoaSeraInformadaGeracaoPagamento) {
        _pagamentoMotorista.PessoaTituloPagar.visible(true);
        _pagamentoMotorista.PessoaTituloPagar.required(true);
        _pagamentoMotorista.PessoaTituloPagar.codEntity(data.CodigoFornecedor);
        _pagamentoMotorista.PessoaTituloPagar.val(data.NomeFornecedor);
    }
    else {
        _pagamentoMotorista.PessoaTituloPagar.visible(false);
        _pagamentoMotorista.PessoaTituloPagar.required(false);
    }

    if (data.GerarTituloPagar || data.GerarTituloAPagarAoMotorista) {
        _pagamentoMotorista.DataVencimentoTituloPagar.visible(true);
        _pagamentoMotorista.DataVencimentoTituloPagar.required(true);
    } else {
        _pagamentoMotorista.DataVencimentoTituloPagar.visible(false);
        _pagamentoMotorista.DataVencimentoTituloPagar.required(false);
    }
    BuscarSaldoDescontadoMotorista();

    if (data.DesabilitarAlteracaoDosPlanosDeContas) {
        _pagamentoMotorista.PlanoDeContaDebito.enable(false);
        _pagamentoMotorista.PlanoDeContaCredito.enable(false);
    }
    else {
        _pagamentoMotorista.PlanoDeContaDebito.enable(true);
        _pagamentoMotorista.PlanoDeContaCredito.enable(true);
    }
}

function RetornoMotorista(data) {
    _pagamentoMotorista.Motorista.codEntity(data.Codigo);
    _pagamentoMotorista.Motorista.val(data.Nome);
    _pagamentoMotorista.SaldoDiariaMotorista.val(data.SaldoDiaria);

    if (data.CodigoContaMotorista !== null && data.CodigoContaMotorista !== undefined && data.CodigoContaMotorista > 0 &&
        data.CodigoContaUsuario !== null && data.CodigoContaUsuario !== undefined && data.CodigoContaUsuario > 0) {

        _pagamentoMotorista.PlanoDeContaDebito.codEntity(data.CodigoContaUsuario);
        _pagamentoMotorista.PlanoDeContaDebito.val(data.DescricaoContaUsuario);

        _pagamentoMotorista.PlanoDeContaCredito.codEntity(data.CodigoContaMotorista);
        _pagamentoMotorista.PlanoDeContaCredito.val(data.DescricaoContaMotorista);
    }
    BuscarSaldoDescontadoMotorista();
}

function RetornoMotoristaPesquisa(data) {
    _pesquisaPagamentoMotorista.Motorista.codEntity(data.Codigo);
    _pesquisaPagamentoMotorista.Motorista.val(data.Nome);
}

function ImprimirReciboAdiantamentoMotorista() {
    var data = { Codigo: _pagamentoMotorista.Codigo.val(), PagamentoMotorista: true };
    executarReST("MovimentoFinanceiro/GerarRecibo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function BuscarSaldoDescontadoMotorista() {
    if (_pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.Finalizada
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.FinalizadoPagamento
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.AgAprovacao
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.Rejeitada
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.AgIntegracao
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.FalhaIntegracao
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.AutorizacaoPendente
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.SemRegraAprovacao
        && _pagamentoMotorista.Situacao.val() !== EnumSituacaoPagamentoMotorista.Cancelada
        && _pagamentoMotorista.Motorista.codEntity() > 0
        && _pagamentoMotorista.TipoPagamentoMotorista.codEntity() > 0) {
        var data = { Motorista: _pagamentoMotorista.Motorista.codEntity(), TipoPagamentoMotorista: _pagamentoMotorista.TipoPagamentoMotorista.codEntity(), Valor: _pagamentoMotorista.Valor.val() };
        executarReST("PagamentoMotoristaTMS/BuscarSaldoDescontadoMotorista", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data !== null) {
                    _pagamentoMotorista.SaldoDescontado.val(arg.Data.SaldoDescontado);
                    _pagamentoMotorista.TotalPagamento.val(arg.Data.TotalPagamento);
                } else {
                    _pagamentoMotorista.SaldoDescontado.val("0,00");
                    _pagamentoMotorista.TotalPagamento.val(_pagamentoMotorista.Valor.val());
                }
            } else {
                _pagamentoMotorista.SaldoDescontado.val("0,00");
                _pagamentoMotorista.TotalPagamento.val(_pagamentoMotorista.Valor.val());
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}

function FuncoesValorBlur() {
    BuscarSaldoDescontadoMotorista();
    ConverterValorOriginal();
}

function ImprimirReciboClick(e, sender) {
    ImprimirReciboAdiantamentoMotorista();
}

function LimparPagamentoMotoristaClick(e, sender) {
    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.ImprimirRecibo.visible(false);
    _CRUDPagamentoMotorista.Reverter.visible(false);
    _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.Cancelar.visible(false);
    _CRUDPagamentoMotorista.Adicionar.visible(true);

    limparCamposPagamentoMotorista();
    setarEtapaInicioPagamentoMotorista();
}

function CancelarPagamentoMotoristaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja cancelar o pagamento ao motorista?", function () {
        Salvar(_pagamentoMotorista, "PagamentoMotoristaTMS/CancelarPagamento", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridPagamentoMotorista.CarregarGrid();
                    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.ImprimirRecibo.visible(false);
                    _CRUDPagamentoMotorista.Reverter.visible(false);
                    _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.Cancelar.visible(false);
                    _CRUDPagamentoMotorista.Adicionar.visible(true);

                    limparCamposPagamentoMotorista();
                    setarEtapaInicioPagamentoMotorista();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento cancelado com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function ReverterPagamentoMotoristaClick(e, sender) {

    exibirConfirmacao("Confirmação", "Realmente deseja reverter e cancelar o pagamento ao motorista?", function () {

        executarReST("PagamentoMotoristaTMS/ReverterPagamento", { Codigo: _pagamentoMotorista.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridPagamentoMotorista.CarregarGrid();
                    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.ImprimirRecibo.visible(false);
                    _CRUDPagamentoMotorista.Reverter.visible(false);
                    _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(false);
                    _CRUDPagamentoMotorista.Cancelar.visible(false);
                    _CRUDPagamentoMotorista.Adicionar.visible(true);

                    limparCamposPagamentoMotorista();
                    setarEtapaInicioPagamentoMotorista();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento revertido e cancelado com sucesso.");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function adicionarClick(e, sender) {
    iniciarRequisicao();
    Salvar(_pagamentoMotorista, "PagamentoMotoristaTMS/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                preencherRetornoPagamentoMotorista(arg);

                enviarArquivosAnexados(arg.Data.Codigo);

                buscarPagamentoMotoristaPorCodigo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                finalizarRequisicao();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            finalizarRequisicao();
        }
    }, sender, exibirCamposObrigatorio);
}

function preencherRetornoPagamentoMotorista(arg) {
    _gridPagamentoMotorista.CarregarGrid();

    var data = arg.Data;
    _pagamentoMotorista.NumeroPagamentoMotorista.val(data.NumeroPagamentoMotorista);
    _pagamentoMotorista.Codigo.val(data.Codigo);
    _pagamentoMotorista.Situacao.val(data.Situacao);
    if (data.Situacao == EnumSituacaoPagamentoMotorista.Finalizada)
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrada com sucesso!");

    var dataSituacao = data.Situacao;
    if (dataSituacao == EnumSituacaoPagamentoMotorista.AgAprovacao || dataSituacao == EnumSituacaoPagamentoMotorista.SemRegraAprovacao)
        exibirMensagem(tipoMensagem.aviso, "Sucesso", "Pagamento Registrado com sucesso, porém, está aguardando uma autorização para ser finalizada.");

    if (!string.IsNullOrWhiteSpace(data.MensagemRetorno))
        exibirMensagem(tipoMensagem.aviso, "Aviso", data.MensagemRetorno);
}

function gerarNovaPagamentoMotoristaClick(e) {
    limparCamposPagamentoMotorista();
    setarEtapaInicioPagamentoMotorista();

    _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.Cancelar.visible(false);
    _CRUDPagamentoMotorista.Adicionar.visible(true);
}

//*******MÉTODOS*******


function buscarPagamentoMotoristas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPagamentoMotorista, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "PagamentoMotoristaTMS/ExportarPesquisa",
        titulo: "Pagamento ao Motorista"
    };

    _gridPagamentoMotorista = new GridView(_pesquisaPagamentoMotorista.Pesquisar.idGrid, "PagamentoMotoristaTMS/Pesquisa", _pesquisaPagamentoMotorista, menuOpcoes, null, null, function () {
        _pesquisaPagamentoMotorista.Codigo.val(_pesquisaPagamentoMotorista.Codigo.def);
    }, null, null, null, null, null, configExportacao);

    _gridPagamentoMotorista.CarregarGrid();
}

function editarPagamentoMotorista(pagamentoMotoristaGrid) {
    limparCamposPagamentoMotorista();
    _pagamentoMotorista.Codigo.val(pagamentoMotoristaGrid.Codigo);
    buscarPagamentoMotoristaPorCodigo();
}

function buscarPagamentoMotoristaPorCodigo(callback) {
    BuscarPorCodigo(_pagamentoMotorista, "PagamentoMotoristaTMS/BuscarPorCodigo", function (arg) {
        PreencherObjetoKnout(_pagamentoMotorista, arg);

        // _pagamentoMotorista.Empresa.val("teste");

        if (arg.Data.Empresa.Codigo > 0) {

        } else {

        }



        var data = arg.Data;

        if (data.GerarTituloPagar || data.GerarTituloAPagarAoMotorista) {
            _pagamentoMotorista.DataVencimentoTituloPagar.visible(true);
            _pagamentoMotorista.DataVencimentoTituloPagar.required(true);
        } else {
            _pagamentoMotorista.DataVencimentoTituloPagar.visible(false);
            _pagamentoMotorista.DataVencimentoTituloPagar.required(false);
        }

        if (data.PessoaSeraInformadaGeracaoPagamento) {
            _pagamentoMotorista.PessoaTituloPagar.visible(true);
            _pagamentoMotorista.PessoaTituloPagar.required(true);
        }
        else {
            _pagamentoMotorista.PessoaTituloPagar.visible(false);
            _pagamentoMotorista.PessoaTituloPagar.required(false);
        }

        if (data.TipoPagamentoMotoristaEnum == EnumTipoPagamentoMotorista.Terceiro)
            _pagamentoMotorista.Terceiro.visible(true);
        else
            _pagamentoMotorista.Terceiro.visible(false);

        preecherCamposEdicaoPagamentoMotorista();
        DesabilitarCamposInstancias(_pagamentoMotorista);

        _anexo.Anexos.val(data.Anexos);

        if (callback != null)
            callback();
        else
            finalizarRequisicao();

        _pagamentoMotorista.ValorMoedaCotacao.val(data.ValorMoedaCotacao);
    }, function (arg) {
        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        limparCamposPagamentoMotorista();
    });
}

function preecherCamposEdicaoPagamentoMotorista() {

    _pesquisaPagamentoMotorista.ExibirFiltros.visibleFade(false);
    _CRUDPagamentoMotorista.GerarNovaPagamentoMotorista.visible(true);

    if (_pagamentoMotorista.Situacao.val() != EnumSituacaoPagamentoMotorista.FinalizadoPagamento && _pagamentoMotorista.Situacao.val() != EnumSituacaoPagamentoMotorista.Cancelada)
        _CRUDPagamentoMotorista.Cancelar.visible(true);

    if (_pagamentoMotorista.Situacao.val() === EnumSituacaoPagamentoMotorista.FinalizadoPagamento) {
        _CRUDPagamentoMotorista.ImprimirRecibo.visible(true);
        _CRUDPagamentoMotorista.Reverter.visible(true);
    }

    _CRUDPagamentoMotorista.Adicionar.visible(false);

    preecherPagamentoMotoristaAutorizacao(_pagamentoMotorista.SolicitacaoCredito.val());
    preecherResumoPagamentoMotorista();

    setarEtapasPagamentoMotorista();
}

function limparCamposPagamentoMotorista() {

    LimparCampos(_pagamentoMotorista);
    limparPagamentoMotoristaAutorizacao();
    limparResumoPagamentoMotorista();
    limparCamposAnexo();
    _resumoPagamentoMotorista.NumeroPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.Cancelar.visible(false);
    _CRUDPagamentoMotorista.ImprimirRecibo.visible(false);
    _CRUDPagamentoMotorista.ConfirmarPagamentoMotorista.visible(false);
    _CRUDPagamentoMotorista.Reverter.visible(false);
    _pagamentoMotorista.DataVencimentoTituloPagar.visible(false);
    _pagamentoMotorista.DataVencimentoTituloPagar.required(false);
    HabilitarCamposInstancias(_pagamentoMotorista);
    setarEtapaInicioPagamentoMotorista();
    _pagamentoMotorista.DataPagamentoMotorista.val(Global.DataHoraAtual());
    _pagamentoMotorista.PlanoDeContaDebito.enable(true);
    _pagamentoMotorista.PlanoDeContaCredito.enable(true);
    _pagamentoMotorista.Terceiro.visible(false);
    _pagamentoMotorista.PessoaTituloPagar.visible(false);
    _pagamentoMotorista.PessoaTituloPagar.required(false);
    _pagamentoMotorista.Empresa.enable(_ConfiguracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista);


    Global.ResetarAbas();
}

function CalcularMoedaEstrangeira() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _pagamentoMotorista.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _pagamentoMotorista.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _pagamentoMotorista.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValor();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValor() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_pagamentoMotorista.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_pagamentoMotorista.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _pagamentoMotorista.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
            BuscarSaldoDescontadoMotorista();
        }
    }
}

function ConverterValorOriginal() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_pagamentoMotorista.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_pagamentoMotorista.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _pagamentoMotorista.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
    finalizarRequisicao();
}

function DesabilitarCamposInstancias(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === true || knout.enable === false)
                knout.enable = false;
            else
                knout.enable(false);
        }
    });
}

function HabilitarCamposInstancias(instancia) {
    $.each(instancia, function (i, knout) {
        if (knout.enable != null) {
            if (knout.enable === false || knout.enable === true)
                knout.enable = true;
            else
                knout.enable(true);
        }
    });

    _pagamentoMotorista.SaldoDiariaMotorista.enable(false);
    _pagamentoMotorista.SaldoDescontado.enable(false);
    _pagamentoMotorista.TotalPagamento.enable(false);
    _pagamentoMotorista.ValorINSS.enable(false);
    _pagamentoMotorista.ValorSEST.enable(false);
    _pagamentoMotorista.ValorSENAT.enable(false);
    _pagamentoMotorista.ValorIRRF.enable(false);
    _pagamentoMotorista.ValorLiquido.enable(false);
}