/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/TipoBonificacao.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/ControleTacografo.js" />
/// <reference path="../../../Areas/Relatorios/ViewsScripts/Relatorios/Global/Relatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Enumeradores/EnumMoedaCotacaoBancoCentral.js" />
/// <reference path="../../Enumeradores/EnumFormaRecebimentoMotoristaAcerto.js" />
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
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Pedagio.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Infracao.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/Cheque.js" />
/// <reference path="../../Consultas/Titulo.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _fechamentoAcertoViagem;
var _gridDescontoMotorista;
var _gridBonificacoesMotorista;
var _gridVeiculosFechamento;
var _gridAcertoViagemInfracao;
var _gridAcertoViagemTacografo;
var _HTMLFechamentoAcertoViagem;
var _adicionarFolga;
var _novoDesconto;
var _novaBonificacao;
var _obterAssinatura;
var canvasAssinatura;
var signaturePadAssinatura;


var _finalizandoAcertoViagem = false;
var _gridAdiantamentos;
var _gridCheques;
var _gridFolgas;
var _gridDevolucoesMoedaEstrangeira;
var _gridVariacoesCambial;
var _detalheMoedaEstrangeira;
var _gridDetalheMoedaEstrangeira;
var _consultaDetalheMoedaEstrangeira;
var _detalheSaldoMoedaEstrangeira;

var _gridVeiculoRelatorio;

var FechamentoAcertoViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Motorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.CPFMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Periodo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ComissaoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.SaldoAtualMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PrevisaoDiarias = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Descontos = PropertyEntity({ type: types.map, required: false, text: "Descontos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarDesconto = PropertyEntity({ eventClick: InformarDescontoMotoristaClick, type: types.event, text: "Adicionar Desconto", visible: ko.observable(true), enable: ko.observable(true) });

    this.ReceitaFrete = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioRecebido = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutrosRecebimentos = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacaoCliente = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.Ocorrencias = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceita = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.DespesaCombustivel = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DespesaArla = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioPago = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DespesaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.TotalSaldo = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.RetornoAdiantamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.VariacaoCambial = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.VariacaoCambialReceita = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
    this.BonificacoesMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoFichaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.ComissaoReceitaMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.PercentualComissao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.NumeroViagensCompartilhada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.VeiculosFechamento = PropertyEntity({ type: types.map, required: false, text: "Veiculos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.RemoverVeiculo = PropertyEntity({ eventClick: RemoverVeiculosClick, type: types.event, text: "Remover Veículos Selecionadas", visible: ko.observable(false), enable: ko.observable(true) });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "*Segmento do Veículo:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Cheque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Cheque:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.AcertoViagemInfracao = PropertyEntity({ type: types.map, required: false, text: "Infrações", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.AcertoViagemTacografo = PropertyEntity({ type: types.map, required: false, text: "Tacógrafos", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.ControleTacografo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Tacógrafo:", idBtnSearch: guid(), enable: ko.observable(true), enable: ko.observable(true) });
    this.HouveExcesso = PropertyEntity({ text: "Houve Excesso?", getType: typesKnockout.bool, val: ko.observable(false), enable: ko.observable(true) });
    this.AdicionarTacografo = PropertyEntity({ eventClick: AdicionarTacografoClick, type: types.event, text: "Add", visible: ko.observable(true), enable: ko.observable(true) });

    this.Bonificacoes = PropertyEntity({ type: types.map, required: false, text: "Bonificações", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarBonificacao = PropertyEntity({ eventClick: InformarBonificacaoMotoristaClick, type: types.event, text: "Adicionar Bonificação", visible: ko.observable(true), enable: ko.observable(true) });

    this.SaldoAtualAlimentacaoMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Motorista:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00"), visible: ko.observable(false) });
    this.ValorTotalAlimentacaoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Total Pagamento:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAlimentacaoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Repassado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAlimentacaoComprovado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Comprovado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAlimentacaoSaldo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo da Viagem:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.SaldoPrevistoAlimentacaoMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Previsto:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00"), visible: ko.observable(false) });
    this.ValorAlimentacaoComprovado.val.subscribe(function () {
        AjustarValorAlimentacao();
    });

    this.SaldoAtualOutrasDepesasMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Motorista:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00"), visible: ko.observable(false) });
    this.ValorTotalAdiantamentoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Total Pagamento:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAdiantamentoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Repassado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAdiantamentoComprovado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Comprovado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.ValorAdiantamentoSaldo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo da Viagem:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00") });
    this.SaldoPrevistoOutrasDepesasMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Previsto:", maxlength: 15, enable: ko.observable(false), val: ko.observable("0,00"), visible: ko.observable(false) });
    this.ValorAdiantamentoComprovado.val.subscribe(function () {
        AjustarValorAdiantamento();
    });

    this.NumeroViagens = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorViagensCompartilhada = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.FaturamentoLiquido = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.FaturamentoBruto = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.TotalImposto = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.AdiantamentoXDespesas = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ComissaoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalBonificacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorTotalDesconto = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalPagarMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.ValorLiquidoMes = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });

    this.PremioComissaoMotorista = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PercentualPremioComissao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.RetornarOutraDespesa = PropertyEntity({ eventClick: RetornarOutraDespesaClick, type: types.event, text: ko.observable("Reabrir Acerto"), visible: ko.observable(false), enable: ko.observable(true) });
    this.FinalizarAcerto = PropertyEntity({ eventClick: FinalizarAcertoClick, type: types.event, text: "Finalizar Acerto", visible: ko.observable(true), enable: ko.observable(true) });
    this.VisualizarDetalhe = PropertyEntity({ eventClick: VisualizarDetalheClick, type: types.event, text: ko.observable("Visualizar Detalhes"), visible: ko.observable(true), enable: ko.observable(true) });

    this.ObterAssinatura = PropertyEntity({ eventClick: obterAssinaturaClick, type: types.event, text: ko.observable("Assinatura Pendente"), visible: ko.observable(true), enable: ko.observable(true) });
    

    this.VisualizarRecibo = PropertyEntity({ eventClick: VisualizarReciboClick, type: types.event, text: ko.observable("Visualizar Recibo"), visible: ko.observable(false), enable: ko.observable(true) });
    this.VisualizarReciboMotorista = PropertyEntity({ eventClick: VisualizarReciboMotoristaClick, type: types.event, text: ko.observable("Recibo Motorista"), visible: ko.observable(_CONFIGURACAO_TMS.VisualizarReciboPorMotoristaNoAcertoDeViagem), enable: ko.observable(true) });

    this.VisualizarAdiantamentos = PropertyEntity({
        eventClick: function (e) {
            if (e.VisualizarAdiantamentos.visibleFade() === true) {
                e.VisualizarAdiantamentos.visibleFade(false);
                e.VisualizarAdiantamentos.icon("fa fa-eye");
            } else {
                e.VisualizarAdiantamentos.visibleFade(true);
                e.VisualizarAdiantamentos.icon("fa fa-eye-slash");
            }
        }, type: types.event, text: "Visualizar Adiantamentos", idFade: guid(), icon: ko.observable("fa fa-eye"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.GridAdiantamentos = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Adiantamentos = PropertyEntity({ type: types.event, text: "Adiantamentos", idBtnSearch: guid(), enable: ko.observable(true) });

    this.GridCheques = PropertyEntity({ type: types.local });
    this.Cheques = PropertyEntity({ type: types.event, text: "Cheques", idBtnSearch: guid(), enable: ko.observable(true) });
    this.ListaCheque = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.GridFolgas = PropertyEntity({ type: types.local });
    this.Folgas = PropertyEntity({ type: types.event, text: "Lançar folga", idBtnSearch: guid(), enable: ko.observable(true), eventClick: AdicionarFolgaClick });
    this.ListaFolga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.AprovarFolgas = PropertyEntity({ eventClick: AprovarFolgasClick, type: types.event, text: "Aprovar Folgas", visible: ko.observable(false), enable: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });

    this.DevolucoesMoedaEstrangeira = PropertyEntity({ type: types.map, required: false, text: "Devoluções", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "*Moeda: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.DataBaseCRT = PropertyEntity({ getType: typesKnockout.dateTime, required: false, text: "Data Base:", enable: ko.observable(true) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginal = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor R$:", val: ko.observable("0,00"), enable: ko.observable(true) });

    this.CodigoDevolucao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorDevolucao = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor Moeda:", val: ko.observable("0,00"), enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.AdicionarDevolucao = PropertyEntity({ eventClick: AdicionarDevolucaoClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });

    this.VariacoesCambial = PropertyEntity({ type: types.map, required: false, text: "Variações Cambial", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });
    this.MoedaCotacaoBancoCentralVariacao = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "*Moeda: ", visible: ko.observable(true), enable: ko.observable(true) });

    this.DataBaseCRTVariacao = PropertyEntity({ getType: typesKnockout.dateTime, required: false, text: "Data Base:", enable: ko.observable(true) });
    this.ValorMoedaCotacaoVariacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalVariacao = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Valor R$:", val: ko.observable("0,00"), enable: ko.observable(true) });

    this.CodigoVariacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorVariacao = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor Variação:", val: ko.observable("0,00"), enable: ko.observable(true) });
    this.JustificativaVariacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: false, enable: ko.observable(true) });
    this.AdicionarVariacao = PropertyEntity({ eventClick: AdicionarVariacaoClick, type: types.event, text: ko.observable("Adicionar"), visible: ko.observable(true), enable: ko.observable(true) });

    this.ObservacaoAcertoMotorista = PropertyEntity({ text: "Observação:", maxlength: 3000, enable: ko.observable(true), visible: ko.observable(true) });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Banco:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.FormaRecebimentoMotoristaAcerto = PropertyEntity({ val: ko.observable(EnumFormaRecebimentoMotoristaAcerto.NadaFazer), options: EnumFormaRecebimentoMotoristaAcerto.obterOpcoes(), def: EnumFormaRecebimentoMotoristaAcerto.NadaFazer, text: "Forma Recebimento: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.DataVencimentoMotoristaAcerto = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Vencimento:", enable: ko.observable(true), visible: ko.observable(false) });
    this.ObservacaoMotoristaAcerto = PropertyEntity({ text: "Observação:", maxlength: 3000, enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoMovimentoMotoristaAcerto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.FormaRecebimentoMotoristaAcerto.val.subscribe(function () {
        if (_fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.val() == EnumFormaRecebimentoMotoristaAcerto.CriarTitulo) {
            _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.visible(true);
            _fechamentoAcertoViagem.ObservacaoMotoristaAcerto.visible(true);
            _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.visible(true);
        }
        else {
            _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.visible(false);
            _fechamentoAcertoViagem.ObservacaoMotoristaAcerto.visible(false);
            _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.visible(false);
        }
    });
};

var AdicionarDesconto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.Data = PropertyEntity({ getType: typesKnockout.date, dataLimit: _acertoViagem.DataFinal, required: true, text: "*Data:", issue: 2, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, required: true, maxlength: 300, text: "*Motivo do Desconto:", enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veiculo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarDescontoMotoristaClick, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: AtualizarDescontoMotoristaClick, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
};

var ObterAssinatura = function () {
    this.Assinatura = PropertyEntity({ text: "Assinatura: ", visible: ko.observable(true) });
    this.LimparAssinatura = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaClick, type: types.event, enable: ko.observable(false) });
    this.CancelarAssinatura = PropertyEntity({ text: "Cancelar Assinatura", eventClick: cancelarAssinaturaClick, type: types.event, enable: ko.observable(false) });
    this.FinalizarAssinatura = PropertyEntity({ text: "Finalizar Assinatura", eventClick: finalizarAssinaturaClick, type: types.event, enable: ko.observable(false) });
};




var AdicionarBonificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor:", maxlength: 10, enable: ko.observable(true) });
    this.Data = PropertyEntity({ getType: typesKnockout.date, dataLimit: _acertoViagem.DataFinal, required: true, text: "*Data:", issue: 2, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, required: false, maxlength: 500, text: "Motivo da Bonificação:", enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veiculo:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    //this.TipoBonificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Bonificação:", idBtnSearch: guid(), required: true });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });

    this.MoedaCotacaoBancoCentral = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), def: EnumMoedaCotacaoBancoCentral.Real, text: "Moeda: ", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataBaseCRT = PropertyEntity({ text: "Data Base CRT: ", required: false, getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorMoedaCotacao = PropertyEntity({ text: "Valor Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false), configDecimal: { precision: 10, allowZero: false, allowNegative: false }, maxlength: 22 });
    this.ValorOriginalMoedaEstrangeira = PropertyEntity({ text: "Valor Original Moeda: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarBonificacaoMotoristaClick, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, eventClick: AtualizarBonificacaoMotoristaClick, text: "Atualizar", visible: ko.observable(false), enable: ko.observable(true) });
};

var AdicionarFolga = function () {
    this.DataInicioFolga = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data inicio:", issue: 2 });
    this.DataFinalFolga = PropertyEntity({ getType: typesKnockout.date, required: true, text: "*Data final:", issue: 2 });
    this.DataFinalFolga.dateRangeInit = this.DataInicioFolga;

    this.QuantidadeDiasFolga = PropertyEntity({ getType: typesKnockout.int, required: false, enable: false, maxlength: 100, text: "Total de dias de folga:", visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: AdicionarFolgaNoAcertoClick, text: "Adicionar", visible: ko.observable(true) });
};

var DetalheMoedaEstrangeira = function () {
    this.DetalheMoedaEstrangeira = PropertyEntity({ type: types.map, required: false, text: "Detalhes", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid() });

    this.TotalReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalMoeda = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharDetalheMoedaEstrangeiraClick, text: "Fechar", visible: ko.observable(true) });
};

var DetalheSaldoMoedaEstrangeira = function () {
    this.AbastecimentoMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoDolar = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoPesoArgentino = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoPesoUruguaio = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoPesoChileno = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoGuarani = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoNovoSol = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.AbastecimentoMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.PedagioMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.OutraDespesaMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.AdiantamentoMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DiariaMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalDespesaMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.TotalReceitaMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.SaldoMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DescontosMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.BonificacoesMotoristaReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.DevolucoesReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });
    this.RecebidoConversaoReais = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: true });

    this.MoedaCotacaoBancoCentralOrigem = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Dolar), options: EnumMoedaCotacaoBancoCentral.obterOpcoesComReais(), def: EnumMoedaCotacaoBancoCentral.Dolar, text: "*Moeda Origem: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorOrigem = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Saldo:", maxlength: 10, val: ko.observable("0,00"), enable: ko.observable(true) });
    this.MoedaCotacaoBancoCentralDestino = PropertyEntity({ val: ko.observable(EnumMoedaCotacaoBancoCentral.Dolar), options: EnumMoedaCotacaoBancoCentral.obterOpcoesComReais(), def: EnumMoedaCotacaoBancoCentral.Dolar, text: "*Moeda Destino: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorCotacao = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Cotação:", maxlength: 10, val: ko.observable("0,0000"), enable: ko.observable(true), configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.ValorFinal = PropertyEntity({ getType: typesKnockout.decimal, required: true, text: "*Valor Final:", maxlength: 10, val: ko.observable("0,00"), enable: ko.observable(true) });
    this.Converter = PropertyEntity({ eventClick: ConverterMoedaClick, type: types.event, text: "Converter", visible: ko.observable(true), enable: ko.observable(true) });

    this.LimparConversoes = PropertyEntity({ type: types.event, eventClick: LimparConversoesClick, text: "Limpar Conversões", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: FecharDetalheSaldoMoedaEstrangeiraClick, text: "Fechar", visible: ko.observable(true) });
};

var ConsultaDetalheMoedaEstrangeira = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tipo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******


function assinado(st) {
    if (st) {
        _fechamentoAcertoViagem.ObterAssinatura.text("Assinatura OK");
    } else {
        _fechamentoAcertoViagem.ObterAssinatura.text("Assinatura Pendente");
    }
}


function obterAssinaturaClick() {
    var dados = {
        Codigo: _acertoViagem.Codigo.val()
    };
    loadSignatureAssinatura();
    executarReST("AcertoFechamento/ObterAssinatura", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;
                if (data.PossuiAssinatura) {
                    _obterAssinatura.Assinatura.visible(true);

                    setTimeout(function (data) {
                        if (!string.IsNullOrWhiteSpace(data.Assinatura))
                            signaturePadAssinatura.fromDataURL(data.Assinatura);
                    }, 500, data);
                }
                _obterAssinatura.LimparAssinatura.enable(true);
                _obterAssinatura.CancelarAssinatura.enable(true);
                _obterAssinatura.FinalizarAssinatura.enable(true);
                _obterAssinatura.Assinatura.visible(true);
                Global.abrirModal('divObterAssinatura');
            } else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}
function limparAssinaturaClick() {
    signaturePadAssinatura.clear();
}

function finalizarAssinaturaClick() {
    var dados = preencherDadosAssinatura();
    executarReST("AcertoFechamento/SalvarAssinatura", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                if (dados.Assinatura == "") {
                    assinado(false);
                } else {
                    assinado(true);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                assinado(false);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            assinado(false);
        }
        Global.fecharModal('divObterAssinatura');
    });
}


function preencherDadosAssinatura() {
    var dados = {
        Codigo: _acertoViagem.Codigo.val(),
        Assinatura: !signaturePadAssinatura.isEmpty() ? canvasAssinatura.toDataURL() : ""
    };
    return dados;
}




function cancelarAssinaturaClick() {
    Global.fecharModal('divObterAssinatura');
}

function loadSignatureAssinatura() {
    canvasAssinatura = document.getElementById(_obterAssinatura.Assinatura.id);
    signaturePadAssinatura = new SignaturePad(canvasAssinatura, { backgroundColor: 'rgb(255, 255, 255)' });
    window.onresize = resizeCanvasAssinatura;
}

function resizeCanvasAssinatura() {
    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    resizeCanvasAssinaturaElement(ratio, canvasAssinatura);
}


function resizeCanvasAssinaturaElement(ratio, canvas) {
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);
}



function loadFechamentoAcertoViagem() {
    $("#contentFechamentoAcertoViagem").html("");
    var idDiv = guid();
    $("#contentFechamentoAcertoViagem").append(_HTMLFechamentoAcertoViagem.replace(/#fechamentoAcertoViagem/g, idDiv));
    _fechamentoAcertoViagem = new FechamentoAcertoViagem();
    KoBindings(_fechamentoAcertoViagem, idDiv);
    
    _novoDesconto = new AdicionarDesconto();
    KoBindings(_novoDesconto, "knoutAdicionarDescontoMotorista");

    _detalheSaldoMoedaEstrangeira = new DetalheSaldoMoedaEstrangeira();
    KoBindings(_detalheSaldoMoedaEstrangeira, "knoutDetalheSaldoMoedaEstrangeira");

    _detalheMoedaEstrangeira = new DetalheMoedaEstrangeira();
    KoBindings(_detalheMoedaEstrangeira, "knoutDetalheMoedaEstrangeira");

    
    _obterAssinatura = new ObterAssinatura();
    KoBindings(_obterAssinatura, "knoutObterAssinatura");

    


    _novaBonificacao = new AdicionarBonificacao();
    KoBindings(_novaBonificacao, "knoutAdicionarBonificacaoMotorista");

    _adicionarFolga = new AdicionarFolga();
    KoBindings(_adicionarFolga, "knoutAdicionarFolga");

    if (_CONFIGURACAO_TMS.SepararValoresAdiantamentoMotoristaPorTipo) {
        _fechamentoAcertoViagem.RetornoAdiantamento.visible(true);
    }
    else {
        _fechamentoAcertoViagem.RetornoAdiantamento.visible(false);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        _novoDesconto.MoedaCotacaoBancoCentral.visible(true);
        _novoDesconto.DataBaseCRT.visible(true);
        _novoDesconto.ValorMoedaCotacao.visible(true);
        _novoDesconto.ValorOriginalMoedaEstrangeira.visible(true);

        _novaBonificacao.MoedaCotacaoBancoCentral.visible(true);
        _novaBonificacao.DataBaseCRT.visible(true);
        _novaBonificacao.ValorMoedaCotacao.visible(true);
        _novaBonificacao.ValorOriginalMoedaEstrangeira.visible(true);

        _fechamentoAcertoViagem.DevolucoesMotorista.visible(true);
        _fechamentoAcertoViagem.VariacaoCambial.visible(true);
        _fechamentoAcertoViagem.VariacaoCambialReceita.visible(true);
    }
    if (_CONFIGURACAO_TMS.ExibirSaldoPrevistoAcertoViagem) {
        _fechamentoAcertoViagem.SaldoPrevistoAlimentacaoMotorista.visible(true);
        _fechamentoAcertoViagem.SaldoPrevistoOutrasDepesasMotorista.visible(true);
    }

    if (_CONFIGURACAO_TMS.OcultarInformacoesFaturamentoAcertoViagem) {
        _fechamentoAcertoViagem.ValorLiquidoMes.visible(false);
        _fechamentoAcertoViagem.TotalImposto.visible(false);
        _fechamentoAcertoViagem.FaturamentoBruto.visible(false);
        _fechamentoAcertoViagem.FaturamentoLiquido.visible(false);
    }
    if (!_CONFIGURACAO_TMS.HabilitarFormaRecebimentoTituloAoMotorista) {
        _fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.visible(false);
        $("#liTabFormaRecebimento").hide();
        $("#tabFormaRecebimento").hide();
    }
    if (!_CONFIGURACAO_TMS.HabilitarLancamentoTacografo) {
        $("#liTabTacografo").hide();
        $("#tabTacografo").hide();
    }
    if (!_CONFIGURACAO_TMS.HabilitarInformacaoAcertoMotorista) {
        $("#liTabAcertoMotorista").hide();
        $("#tabAcertoMotorista").hide();
    }
    if (_CONFIGURACAO_TMS.SolicitarAprovacaoFolgaAcertoViagem) {
        //$("#liTabResultadoViagem").removeClass("active");
        //$("#tabResultadoViagem").removeClass("active");

        //$("#liTabResultadoMotorista").addClass("active");
        //$("#tabResultadoMotorista").addClass("active");

        //$("#liTabVeiculos").removeClass("active");
        //$("#tabVeiculos").removeClass("active");

        //$("#liTabFolga").addClass("active");
        //$("#tabFolga").addClass("active");
        _fechamentoAcertoViagem.AprovarFolgas.visible(true);
        $("#myTabResultado a:eq(0)").tab("show");
    }
    else if (_CONFIGURACAO_TMS.OcultarInformacoesResultadoViagemAcertoViagem) {
        $("#liTabResultadoViagem").hide();
        $("#tabResultadoViagem").hide();
        //$("#tabResultadoMotorista").show();
        //$("#liTabResultadoViagem").removeClass("active");
        //$("#liTabResultadoMotorista").addClass("active");
        //$("#tabResultadoViagem").removeClass("active");

        $("#myTabResultado a:eq(1)").tab("show");

        //Global.ResetarAba("#myTabResultado");
    }
    else {
        //$("#liTabResultadoViagem").show();
        //$("#liTabResultadoViagem").addClass("active");
        //$("#liTabResultadoMotorista").removeClass("active");
        //$("#tabResultadoViagem").addClass("active");
        //$("#liTabResultadoMotorista").removeClass("active");
    }

    new BuscarVeiculos(_novoDesconto.Veiculo, null, null, null, null, null, _acertoViagem.Codigo);
    new BuscarVeiculos(_novaBonificacao.Veiculo, null, null, null, null, null, _acertoViagem.Codigo);
    //new BuscarTipoBonificacao(_novaBonificacao.TipoBonificacao);

    new BuscarControleTacografo(_fechamentoAcertoViagem.ControleTacografo, null, null, true);
    new BuscarTitulo(_fechamentoAcertoViagem.Titulo);
    new BuscarBanco(_fechamentoAcertoViagem.Banco);
    new BuscarTipoMovimento(_fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto);
    new BuscarJustificativas(_fechamentoAcertoViagem.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemMotorista]);
    new BuscarJustificativas(_fechamentoAcertoViagem.JustificativaVariacao, null, null, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemMotorista]);
    new BuscarJustificativas(_novaBonificacao.Justificativa, null, EnumTipoJustificativa.Acrescimo, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemMotorista]);
    new BuscarJustificativas(_novoDesconto.Justificativa, null, EnumTipoJustificativa.Desconto, [EnumTipoFinalidadeJustificativa.Todas, EnumTipoFinalidadeJustificativa.AcertoViagemMotorista]);

    new BuscarSegmentoVeiculo(_fechamentoAcertoViagem.SegmentoVeiculo);
    new BuscarCheque(_fechamentoAcertoViagem.Cheque, retornoCheque);

    _fechamentoAcertoViagem.AdicionarBonificacao.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao, _PermissoesPersonalizadas));
    _fechamentoAcertoViagem.AdicionarDesconto.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto, _PermissoesPersonalizadas));
    _fechamentoAcertoViagem.FinalizarAcerto.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto, _PermissoesPersonalizadas));
    _fechamentoAcertoViagem.VisualizarDetalhe.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_OcultarResultadosViagem, _PermissoesPersonalizadas));
    _fechamentoAcertoViagem.VisualizarRecibo.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || _CONFIGURACAO_TMS.AcertoDeViagemComDiaria);

    _fechamentoAcertoViagem.ValorAlimentacaoComprovado.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem, _PermissoesPersonalizadas));
    _fechamentoAcertoViagem.ValorAdiantamentoComprovado.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirAdicionarAlterarValorComprovadoSaldoViagem, _PermissoesPersonalizadas));

    _fechamentoAcertoViagem.RetornarOutraDespesa.visible(false);

    if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria)
        $("#liTabResultadoMotorista").show();
    else
        $("#liTabResultadoMotorista").hide();

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        $("#liTabDevolucaoMotorista").show();
        $("#liTabVariacaoCambial").hide();
    }
    else {
        $("#liTabDevolucaoMotorista").hide();
        $("#liTabVariacaoCambial").hide();
        $("#hrfDetalheSaldo").hide();
        $("#hrfDetalheBonificacao").hide();
        $("#hrfDetalheOutraDespesa").hide();
        $("#hrfDetalheDesconto").hide();
        $("#hrfDetalhePedagio").hide();
        $("#hrfDetalheAdiantamento").hide();
        $("#hrfDetalheAbastecimento").hide();
        $("#hrfVariacaoCambial").hide();
        $("#hrfVariacaoCambialReceital").hide();
    }

    if (!_CONFIGURACAO_TMS.DesabilitarSaldoViagemAcerto)
        $("#liTabSaldoViagem").show();
    else
        $("#liTabSaldoViagem").hide();

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_OcultarResultadosViagem, _PermissoesPersonalizadas)) {
        $("#liTabResultadoViagem").hide();
        $("#tabResultadoViagem").hide();
        $("#divResumoFinal").hide();
        $("#myTabResultado a:eq(0)").tab("show");
        $("#myTabResultado a:eq(1)").tab("show");
    } else {
        //$("#liTabResultadoViagem").show();
        $("#myTabResultado a:eq(0)").tab("show");
        $("#divResumoFinal").show();
    }

    if (_CONFIGURACAO_TMS.VisualizarReciboPorMotoristaNoAcertoDeViagem) {
        $("#divResumoFinal").hide();
        _fechamentoAcertoViagem.ComissaoReceitaMotorista.visible(true);
        _fechamentoAcertoViagem.SaldoFichaMotorista.visible(true);
    }

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverVeiculoFechamentoClick(_fechamentoAcertoViagem.VeiculosFechamento, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoAcerto", visible: false },
        { data: "Placa", title: "Placa", width: "20%", className: "text-align-left" },
        { data: "Reboque", title: "Reboque", width: "50%", className: "text-align-left" }
    ];

    _gridVeiculosFechamento = new BasicDataTable(_fechamentoAcertoViagem.VeiculosFechamento.idGrid, header, menuOpcoes);
    _fechamentoAcertoViagem.VeiculosFechamento.basicTable = _gridVeiculosFechamento;

    new BuscarVeiculos(_fechamentoAcertoViagem.VeiculosFechamento, RetornoInserirVeiculoFechamento, null, null, null, null, null, null, null, null, null, null, null, _gridVeiculosFechamento);

    carregarGridTacografo();

    carregarGridAdiantamentos();
    _fechamentoAcertoViagem.Adiantamentos.basicTable = _gridAdiantamentos;

    new BuscarPagamentoMotoristaTMS(_fechamentoAcertoViagem.Adiantamentos, AdicionarAdiancamentoAcerto, _acertoViagem.Codigo, _gridAdiantamentos, true);


    var excluirInfracao = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverAcertoViagemInfracaoClick(_fechamentoAcertoViagem.AcertoViagemInfracao, data);
        }, tamanho: "8", icone: ""
    };
    var assinarInfracao = {
        descricao: "Assinar", id: guid(), evento: "onclick", metodo: function (data) {
            AssinarAcertoViagemInfracaoClick(_fechamentoAcertoViagem.AcertoViagemInfracao, data);
        }, tamanho: "8", icone: ""
    };
    var menuOpcoesInfracao = new Object();
    menuOpcoesInfracao.tipo = TypeOptionMenu.list;
    menuOpcoesInfracao.opcoes = new Array();
    menuOpcoesInfracao.opcoes.push(excluirInfracao);
    menuOpcoesInfracao.opcoes.push(assinarInfracao);

    var headerInfracao = [
        { data: "Codigo", visible: false },
        { data: "CodigoAcerto", visible: false },
        { data: "Placa", title: "Placa", width: "15%", className: "text-align-left" },
        { data: "NumeroAtuacao", title: "Número da Autuação", width: "30%", className: "text-align-left" },
        { data: "NumeroInfracao", title: "Número da Ocorrência/Infração", width: "25%", className: "text-align-right" }
    ];

    _gridAcertoViagemInfracao = new BasicDataTable(_fechamentoAcertoViagem.AcertoViagemInfracao.idGrid, headerInfracao, menuOpcoesInfracao);
    _fechamentoAcertoViagem.AcertoViagemInfracao.basicTable = _gridAcertoViagemInfracao;

    new BuscarInfracoes(_fechamentoAcertoViagem.AcertoViagemInfracao, RetornoInserirAcertoViagemInfracao, _gridAcertoViagemInfracao, _acertoViagem.Motorista);

    var menuOpcoesCheque = {
        tipo: TypeOptionMenu.link, tamanho: 4, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirChequeAcertoClick(_fechamentoAcertoViagem.Cheques, data);
            }
        }]
    };

    var headerCheque = [
        { data: "Codigo", visible: false },
        { data: "NumeroCheque", title: "Número", width: "15%" },
        { data: "Banco", title: "Banco", width: "35%" },
        { data: "Valor", title: "Valor", width: "10%" }
    ];

    _gridCheques = new BasicDataTable(_fechamentoAcertoViagem.GridCheques.id, headerCheque, menuOpcoesCheque, { column: 1, dir: orderDir.asc });

    new BuscarCheque(_fechamentoAcertoViagem.Cheques, AdicionarChequeAcertoClick, _gridCheques);

    _fechamentoAcertoViagem.Cheques.basicTable = _gridCheques;
    RecarregarGridCheques();

    var menuOpcoesFolga = {
        tipo: TypeOptionMenu.link, tamanho: 4, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirFolgaAcertoClick(_fechamentoAcertoViagem.Folgas, data);
            }
        }]
    };

    var headerFolga = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
        { data: "Dias", title: "Dia(s)", width: "10%" }
    ];

    _gridFolgas = new BasicDataTable(_fechamentoAcertoViagem.GridFolgas.id, headerFolga, menuOpcoesFolga, { column: 1, dir: orderDir.asc });
    RecarregarGridFolgas();

    CarregarFechamento();
    //Global.ResetarAbas();

    loadSignatureAssinatura();



}

function RemoverVeiculosClick(e, sender) {

}

function retornoCheque(data) {
    _fechamentoAcertoViagem.Cheque.codEntity(data.Codigo);
    _fechamentoAcertoViagem.Cheque.val(data.NumeroCheque);
    _acertoViagem.Cheque.codEntity(data.Codigo);
    _acertoViagem.Cheque.val(data.NumeroCheque);
}

function AjustarValorAlimentacao() {
    var saldoAtualAlimentacaoMotorista = Globalize.parseFloat(_fechamentoAcertoViagem.SaldoAtualAlimentacaoMotorista.val());
    var valorAlimentacaoRepassado = Globalize.parseFloat(_fechamentoAcertoViagem.ValorTotalAlimentacaoRepassado.val());
    var valorAlimentacaoComprovado = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAlimentacaoComprovado.val());

    if (isNaN(valorAlimentacaoRepassado))
        valorAlimentacaoRepassado = 0;
    if (isNaN(valorAlimentacaoComprovado))
        valorAlimentacaoComprovado = 0;
    if (isNaN(saldoAtualAlimentacaoMotorista))
        saldoAtualAlimentacaoMotorista = 0;

    var valorAlimentacaoSaldo = (valorAlimentacaoRepassado - valorAlimentacaoComprovado);
    if (isNaN(valorAlimentacaoSaldo))
        valorAlimentacaoSaldo = 0;

    _fechamentoAcertoViagem.ValorAlimentacaoSaldo.val(Globalize.format(valorAlimentacaoSaldo, "n2"));

    var saldoPrevistoAlimentacaoMotorista = (saldoAtualAlimentacaoMotorista + valorAlimentacaoSaldo);

    if (isNaN(saldoPrevistoAlimentacaoMotorista))
        saldoPrevistoAlimentacaoMotorista = 0;
    _fechamentoAcertoViagem.SaldoPrevistoAlimentacaoMotorista.val(Globalize.format(saldoPrevistoAlimentacaoMotorista, "n2"));
}

function AjustarValorAdiantamento() {
    var saldoAtualOutrasDepesasMotorista = Globalize.parseFloat(_fechamentoAcertoViagem.SaldoAtualOutrasDepesasMotorista.val());
    var valorAdiantamentoRepassado = Globalize.parseFloat(_fechamentoAcertoViagem.ValorTotalAdiantamentoRepassado.val());
    var valorAdiantamentoComprovado = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAdiantamentoComprovado.val());

    if (isNaN(valorAdiantamentoRepassado))
        valorAdiantamentoRepassado = 0;
    if (isNaN(valorAdiantamentoComprovado))
        valorAdiantamentoComprovado = 0;
    if (isNaN(saldoAtualOutrasDepesasMotorista))
        saldoAtualOutrasDepesasMotorista = 0;

    var valorAdiantamentoSaldo = (valorAdiantamentoRepassado - valorAdiantamentoComprovado);
    if (isNaN(valorAdiantamentoSaldo))
        valorAdiantamentoSaldo = 0;

    _fechamentoAcertoViagem.ValorAdiantamentoSaldo.val(Globalize.format(valorAdiantamentoSaldo, "n2"));

    var saldoPrevistoOutrasDepesasMotorista = (saldoAtualOutrasDepesasMotorista + valorAdiantamentoSaldo);

    if (isNaN(saldoPrevistoOutrasDepesasMotorista))
        saldoPrevistoOutrasDepesasMotorista = 0;
    _fechamentoAcertoViagem.SaldoPrevistoOutrasDepesasMotorista.val(Globalize.format(saldoPrevistoOutrasDepesasMotorista, "n2"));
}

function AdicionarAdiancamentoAcerto(r) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    if (r !== null) {
        var adiantamentos = new Array();
        for (var i = 0; i < r.length; i++)
            adiantamentos.push({
                Codigo: r[i].Codigo,
                Numero: r[i].Numero,
                DataPagamento: r[i].DataPagamento,
                Motorista: r[i].Motorista,
                PagamentoMotoristaTipo: r[i].PagamentoMotoristaTipo,
                MoedaCotacaoBancoCentral: r[i].MoedaCotacaoBancoCentral,
                ValorMoedaCotacao: r[i].ValorMoedaCotacao,
                ValorOriginalMoedaEstrangeira: r[i].ValorOriginalMoedaEstrangeira,
                Valor: r[i].Valor
            });
        var data = {
            Adiantamentos: JSON.stringify(adiantamentos),
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/AdicionarAdiantamentoAcerto", data, function (arg) {
            if (arg.Success) {
                _gridAdiantamentos.CarregarGrid();
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    }
}

function AprovarFolgasClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja aprovar as folgas lançadas?", function () {
        var data = {
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/AprovarFolgasAcerto", data, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Folgas aprovadas");
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function ExcluirAdiancamentoAcertoClick(dataSelecao) {

    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    //if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirRemoverAdiantamento, _PermissoesPersonalizadas)) {
    //    exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para remover o adiantamento.");
    //    return;
    //}

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o adiantamento selecionado?", function () {
        var data = {
            Codigo: dataSelecao.Codigo,
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/RemoverAdiantamentoAcerto", data, function (arg) {
            if (arg.Success) {
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function RecarregarGridCheques() {

    var data = new Array();

    if (_fechamentoAcertoViagem.ListaCheque.val() !== null && _fechamentoAcertoViagem.ListaCheque.val() !== undefined && _fechamentoAcertoViagem.ListaCheque.val() != "" && _fechamentoAcertoViagem.ListaCheque.val().length > 0) {
        $.each(_fechamentoAcertoViagem.ListaCheque.val(), function (i, cheque) {
            var chequeGrid = new Object();

            chequeGrid.Codigo = cheque.Codigo;
            chequeGrid.NumeroCheque = cheque.NumeroCheque;
            chequeGrid.Banco = cheque.Banco;
            chequeGrid.Valor = cheque.Valor;

            data.push(chequeGrid);
        });
    }

    _gridCheques.CarregarGrid(data);
}


function AdicionarChequeAcertoClick(r) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }


    if (r !== null) {
        var cheques = _gridCheques.BuscarRegistros();
        for (var i = 0; i < r.length; i++)
            cheques.push({
                Codigo: r[i].Codigo,
                NumeroCheque: r[i].NumeroCheque,
                Banco: r[i].Banco,
                Valor: r[i].Valor
            });

        _gridCheques.CarregarGrid(cheques);

        var data = {
            Cheques: JSON.stringify(cheques),
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/AdicionarChequeAcerto", data, function (arg) {
            if (arg.Success) {
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    }
}

function RecarregarGridFolgas() {

    var data = new Array();

    if (_fechamentoAcertoViagem.ListaFolga.val() !== null && _fechamentoAcertoViagem.ListaFolga.val() !== undefined && _fechamentoAcertoViagem.ListaFolga.val() != "" && _fechamentoAcertoViagem.ListaFolga.val().length > 0) {
        $.each(_fechamentoAcertoViagem.ListaFolga.val(), function (i, folga) {
            var folgaGrid = new Object();

            folgaGrid.Codigo = folga.Codigo;
            folgaGrid.Descricao = folga.Descricao;
            folgaGrid.Dias = folga.Dias;

            data.push(folgaGrid);
        });
    }

    _gridFolgas.CarregarGrid(data);
}

function AdicionarFolgaNoAcertoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoMotorista: _acertoViagem.Motorista.codEntity(),
            DataInicioFolga: e.DataInicioFolga.val(),
            DataFinalFolga: e.DataFinalFolga.val()
        };
        executarReST("AcertoFechamento/InserirFolga", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                Global.fecharModal('divAdicionarFolga');

                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function ConverterMoedaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var tudoCerto = true;
    _detalheSaldoMoedaEstrangeira.ValorOrigem.requiredClass("form-control");
    _detalheSaldoMoedaEstrangeira.ValorCotacao.requiredClass("form-control");
    _detalheSaldoMoedaEstrangeira.ValorFinal.requiredClass("form-control");

    if (Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorFinal.val()) <= 0) {
        tudoCerto = false;
        _detalheSaldoMoedaEstrangeira.ValorFinal.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorOrigem.val()) <= 0) {
        tudoCerto = false;
        _detalheSaldoMoedaEstrangeira.ValorOrigem.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorCotacao.val()) <= 0) {
        tudoCerto = false;
        _detalheSaldoMoedaEstrangeira.ValorCotacao.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {
        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            MoedaCotacaoBancoCentralOrigem: _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralOrigem.val(),
            ValorOrigem: _detalheSaldoMoedaEstrangeira.ValorOrigem.val(),
            MoedaCotacaoBancoCentralDestino: _detalheSaldoMoedaEstrangeira.MoedaCotacaoBancoCentralDestino.val(),
            ValorCotacao: _detalheSaldoMoedaEstrangeira.ValorCotacao.val(),
            ValorFinal: _detalheSaldoMoedaEstrangeira.ValorFinal.val()
        };
        executarReST("AcertoFechamento/InserirConversaoMoedaEstrangeira", data, function (arg) {
            if (arg.Success) {
                LimparCampos(_detalheSaldoMoedaEstrangeira);
                CarregarDadosSaldoMoedaEstrangeira();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function LimparConversoesClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja limpar TODAS as conversões inseridas neste acerto?", function () {
        var data = {
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/LimparConversaoMoedaEstrangeira", data, function (arg) {
            if (arg.Success) {
                LimparCampos(_detalheSaldoMoedaEstrangeira);
                CarregarDadosSaldoMoedaEstrangeira();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });
}

function FecharDetalheSaldoMoedaEstrangeiraClick(e, sender) {
    Global.fecharModal('divDetalheSaldoMoedaEstrangeira');
}



function FecharDetalheMoedaEstrangeiraClick(e, sender) {
    Global.fecharModal('divDetalheMoedaEstrangeira');
}

function CalcularVariacaoCambial() {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja recalcular a variação cambial deste acerto?", function () {
        var data = {
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/CalcularVariacaoCambial", data, function (arg) {
            if (arg.Success) {
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function DetalheMoedaEstrangeiraClick(tipo) {

    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }

    _consultaDetalheMoedaEstrangeira = new ConsultaDetalheMoedaEstrangeira();
    _consultaDetalheMoedaEstrangeira.Codigo.val(_acertoViagem.Codigo.val());
    _consultaDetalheMoedaEstrangeira.Tipo.val(tipo);

    _gridDetalheMoedaEstrangeira = new GridView(_detalheMoedaEstrangeira.DetalheMoedaEstrangeira.idGrid, "AcertoFechamento/PesquisarDetalheMoedaEstrangeira", _consultaDetalheMoedaEstrangeira, null, null, null, null);
    _gridDetalheMoedaEstrangeira.CarregarGrid(BuscarTotalizadorMoedaEstrangeira(_consultaDetalheMoedaEstrangeira));

    Global.abrirModal('divDetalheMoedaEstrangeira');
}

function BuscarTotalizadorMoedaEstrangeira(consultaDetalheMoedaEstrangeira) {
    var data = {
        Codigo: consultaDetalheMoedaEstrangeira.Codigo.val(),
        Tipo: consultaDetalheMoedaEstrangeira.Tipo.val()
    };
    executarReST("AcertoFechamento/TotaisDetalheMoedaEstrangeira", data, function (arg) {
        if (arg.Success) {
            _detalheMoedaEstrangeira.TotalReais.val(arg.Data.TotalReais);
            _detalheMoedaEstrangeira.TotalMoeda.val(arg.Data.TotalMoeda);
        } else {
            exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
        }
    });
}

function DetalheSaldoAcertoViagemClick(tipo) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }

    CarregarDadosSaldoMoedaEstrangeira();

    Global.abrirModal('divDetalheSaldoMoedaEstrangeira');
}

function AdicionarFolgaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    LimparCampos(_novaBonificacao);

    Global.abrirModal('divAdicionarFolga');
}

function ExcluirFolgaAcertoClick(knoutCheque, dataSelecao) {

    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a folga selecionada?", function () {
        var data = {
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/RemoverFolgaAcerto", data, function (arg) {
            if (arg.Success) {
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}


function ExcluirChequeAcertoClick(knoutCheque, dataSelecao) {

    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o cheque selecionado?", function () {
        var data = {
            Codigo: dataSelecao.Codigo,
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/RemoverChequeAcerto", data, function (arg) {
            if (arg.Success) {
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}


function RetornoInserirAcertoViagemInfracao(data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    if (data !== null) {
        var dataGrid = _gridAcertoViagemInfracao.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CodigoAcerto = _acertoViagem.Codigo.val();
            obj.Placa = data[i].Veiculo;
            obj.NumeroAtuacao = data[i].NumeroAtuacao;
            obj.NumeroInfracao = data[i].Numero;

            dataGrid.push(obj);
        }
        _gridAcertoViagemInfracao.CarregarGrid(dataGrid);

        preencherListasSelecao();
        data = {
            Codigo: _acertoViagem.Codigo.val(),
            ListaInfracoes: _acertoViagem.ListaInfracoes.val()
        };
        executarReST("AcertoFechamento/AtualizarInfracaoAcerto", data, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                CarregarDadosAcertoViagem(_acertoViagem.Codigo.val(), null, EnumEtapaAcertoViagem.Fechamento);
            }
        });
    }
}

function RetornoInserirVeiculoFechamento(data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    if (data !== null) {
        var dataGrid = _gridVeiculosFechamento.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CodigoAcerto = _acertoViagem.Codigo.val();
            obj.Placa = data[i].Placa;
            obj.Reboque = data[i].Reboque;

            dataGrid.push(obj);
        }
        _gridVeiculosFechamento.CarregarGrid(dataGrid);

        preencherListasSelecao();
        data = {
            Codigo: _acertoViagem.Codigo.val(),
            ListaVeiculosFechamento: _acertoViagem.ListaVeiculosFechamento.val()
        };
        executarReST("AcertoFechamento/AtualizarVeiculoAcerto", data, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                CarregarDadosAcertoViagem(_acertoViagem.Codigo.val(), null, EnumEtapaAcertoViagem.Fechamento);
            }
        });
    }
}

function AssinarAcertoViagemInfracaoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja marcar que a infração foi assinada?", function () {
        data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoInfracao: sender.Codigo
        };
        executarReST("AcertoFechamento/AssinarInfracaoAcerto", data, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Infração assinada com sucesso.");
            }
        });
    });
}

function RemoverAcertoViagemInfracaoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir a infração selecionada?", function () {
        var infracaoGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < infracaoGrid.length; i++) {
            if (sender.Codigo === infracaoGrid[i].Codigo) {
                infracaoGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(infracaoGrid);

        preencherListasSelecao();
        data = {
            Codigo: _acertoViagem.Codigo.val(),
            ListaInfracoes: _acertoViagem.ListaInfracoes.val()
        };
        executarReST("AcertoFechamento/AtualizarInfracaoAcerto", data, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                CarregarDadosAcertoViagem(_acertoViagem.Codigo.val(), null, EnumEtapaAcertoViagem.Fechamento);
            }
        });
    });
}

function RemoverVeiculoFechamentoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o veículo " + sender.Placa + "?", function () {
        var veiculoGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < veiculoGrid.length; i++) {
            if (sender.Codigo === veiculoGrid[i].Codigo) {
                veiculoGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(veiculoGrid);

        preencherListasSelecao();
        data = {
            Codigo: _acertoViagem.Codigo.val(),
            ListaVeiculosFechamento: _acertoViagem.ListaVeiculosFechamento.val()
        };
        executarReST("AcertoFechamento/AtualizarVeiculoAcerto", data, function (arg) {
            if (!arg.Success) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                CarregarDadosAcertoViagem(_acertoViagem.Codigo.val(), null, EnumEtapaAcertoViagem.Fechamento);
            }
        });
    });
}

function RemoverTacografoClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tacógrafo selecionado?", function () {
        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoFechamento/RemoverControleTacografo", data, function (arg) {
            if (arg.Success) {
                _gridAcertoViagemTacografo.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function AdicionarTacografoClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var tudoCerto = true;
    _fechamentoAcertoViagem.ControleTacografo.requiredClass("form-control");

    if (!(_fechamentoAcertoViagem.ControleTacografo.codEntity() > 0)) {
        tudoCerto = false;
        _fechamentoAcertoViagem.ControleTacografo.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {
        var data = {
            CodigoAcerto: _acertoViagem.Codigo.val(),
            ControleTacografo: _fechamentoAcertoViagem.ControleTacografo.codEntity(),
            HouveExcesso: _fechamentoAcertoViagem.HouveExcesso.val()
        };
        executarReST("AcertoFechamento/InserirControleTacografo", data, function (arg) {
            if (arg.Success) {
                LimparCampoEntity(_fechamentoAcertoViagem.ControleTacografo);
                _fechamentoAcertoViagem.HouveExcesso.val(false);

                $("#" + _fechamentoAcertoViagem.ControleTacografo.id).focus();
                _gridAcertoViagemTacografo.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe o tacógrafo.");
    }
}

function callbackEditarColunaTacografo(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, Excesso: dataRow.Excesso };

    executarReST("AcertoFechamento/AlterarDadosTacografo", data, function (arg) {
        if (arg.Success) {
            _gridAcertoViagemTacografo.CarregarGrid();
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function InformarBonificacaoMotoristaClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para adicionar bonificação ao motorista.");
        return;
    }
    LimparCampos(_novaBonificacao);
    _novaBonificacao.Atualizar.visible(false);
    _novaBonificacao.Adicionar.visible(true);

    Global.abrirModal('divAdicionarBonificacaoMotorista');
}

function AdicionarBonificacaoMotoristaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {

        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoMotorista: _acertoViagem.Motorista.codEntity(),
            Veiculo: e.Veiculo.codEntity(),
            Data: e.Data.val(),
            Valor: e.Valor.val(),
            Observacao: e.Observacao.val(),
            Justificativa: e.Justificativa.codEntity(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val()
        };
        executarReST("AcertoFechamento/InserirBonificacaoMotorista", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                $("#" + e.Valor.id).focus();

                _gridBonificacoesMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function RemoverBonificacaoMotoristaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para remover bonificação do motorista.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a bonificação " + e.Motivo + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoFechamento/RemoverBonificacaoMotorista", data, function (arg) {
            if (arg.Success) {

                _gridBonificacoesMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function AdicionarVariacaoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var tudoCerto = true;
    _fechamentoAcertoViagem.MoedaCotacaoBancoCentralVariacao.requiredClass("form-control");
    _fechamentoAcertoViagem.ValorVariacao.requiredClass("form-control");
    _fechamentoAcertoViagem.JustificativaVariacao.requiredClass("form-control");
    _fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.requiredClass("form-control");

    if (!(_fechamentoAcertoViagem.JustificativaVariacao.codEntity() > 0)) {
        tudoCerto = false;
        _fechamentoAcertoViagem.JustificativaVariacao.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_fechamentoAcertoViagem.ValorVariacao.val()) <= 0) {
        tudoCerto = false;
        _fechamentoAcertoViagem.ValorVariacao.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {

        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoVariacao: _fechamentoAcertoViagem.CodigoVariacao.val(),
            MoedaCotacaoBancoCentral: _fechamentoAcertoViagem.MoedaCotacaoBancoCentralVariacao.val(),
            ValorVariacao: _fechamentoAcertoViagem.ValorVariacao.val(),
            Justificativa: _fechamentoAcertoViagem.JustificativaVariacao.codEntity(),
            DataBaseCRT: _fechamentoAcertoViagem.DataBaseCRTVariacao.val(),
            ValorOriginal: _fechamentoAcertoViagem.ValorOriginalVariacao.val(),
            ValorMoedaCotacao: _fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.val()
        };
        executarReST("AcertoFechamento/InserirVariacaoCambial", data, function (arg) {
            if (arg.Success) {
                _gridVariacoesCambial.CarregarGrid();
                LimparCampo(_fechamentoAcertoViagem.MoedaCotacaoBancoCentralVariacao);
                LimparCampo(_fechamentoAcertoViagem.ValorVariacao);
                LimparCampo(_fechamentoAcertoViagem.JustificativaVariacao);
                LimparCampo(_fechamentoAcertoViagem.DataBaseCRTVariacao);
                LimparCampo(_fechamentoAcertoViagem.ValorOriginalVariacao);
                LimparCampo(_fechamentoAcertoViagem.ValorMoedaCotacaoVariacao);
                LimparCampo(_fechamentoAcertoViagem.CodigoVariacao);
                _fechamentoAcertoViagem.AdicionarVariacao.text("Adicionar");

                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function AdicionarDevolucaoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var tudoCerto = true;
    _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.requiredClass("form-control");
    _fechamentoAcertoViagem.ValorDevolucao.requiredClass("form-control");
    _fechamentoAcertoViagem.Justificativa.requiredClass("form-control");
    _fechamentoAcertoViagem.ValorMoedaCotacao.requiredClass("form-control");

    if (!(_fechamentoAcertoViagem.Justificativa.codEntity() > 0)) {
        tudoCerto = false;
        _fechamentoAcertoViagem.Justificativa.requiredClass("form-control is-invalid");
    }
    if (Globalize.parseFloat(_fechamentoAcertoViagem.ValorDevolucao.val()) <= 0) {
        tudoCerto = false;
        _fechamentoAcertoViagem.ValorDevolucao.requiredClass("form-control is-invalid");
    }

    if (tudoCerto) {

        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoDevolucao: _fechamentoAcertoViagem.CodigoDevolucao.val(),
            MoedaCotacaoBancoCentral: _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.val(),
            ValorDevolucao: _fechamentoAcertoViagem.ValorDevolucao.val(),
            Justificativa: _fechamentoAcertoViagem.Justificativa.codEntity(),
            DataBaseCRT: _fechamentoAcertoViagem.DataBaseCRT.val(),
            ValorOriginal: _fechamentoAcertoViagem.ValorOriginal.val(),
            ValorMoedaCotacao: _fechamentoAcertoViagem.ValorMoedaCotacao.val()
        };
        executarReST("AcertoFechamento/InserirDevolucoesMoedaEstrangeira", data, function (arg) {
            if (arg.Success) {
                _gridDevolucoesMoedaEstrangeira.CarregarGrid();
                LimparCampo(_fechamentoAcertoViagem.MoedaCotacaoBancoCentral);
                LimparCampo(_fechamentoAcertoViagem.ValorDevolucao);
                LimparCampo(_fechamentoAcertoViagem.Justificativa);
                LimparCampo(_fechamentoAcertoViagem.DataBaseCRT);
                LimparCampo(_fechamentoAcertoViagem.ValorOriginal);
                LimparCampo(_fechamentoAcertoViagem.ValorMoedaCotacao);
                LimparCampo(_fechamentoAcertoViagem.CodigoDevolucao);
                _fechamentoAcertoViagem.AdicionarDevolucao.text("Adicionar");

                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function InformarDescontoMotoristaClick(e, data) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para adicionar desconto ao motorista.");
        return;
    }
    LimparCampos(_novoDesconto);
    _novoDesconto.Atualizar.visible(false);
    _novoDesconto.Adicionar.visible(true);

    Global.abrirModal('divAdicionarDescontoMotorista');
}

function AdicionarDescontoMotoristaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (ValidarCamposObrigatorios(e)) {

        var data = {
            Codigo: _acertoViagem.Codigo.val(),
            CodigoMotorista: _acertoViagem.Motorista.codEntity(),
            Veiculo: e.Veiculo.codEntity(),
            Data: e.Data.val(),
            Valor: e.Valor.val(),
            Observacao: e.Observacao.val(),
            Justificativa: e.Justificativa.codEntity(),
            MoedaCotacaoBancoCentral: e.MoedaCotacaoBancoCentral.val(),
            DataBaseCRT: e.DataBaseCRT.val(),
            ValorMoedaCotacao: e.ValorMoedaCotacao.val(),
            ValorOriginalMoedaEstrangeira: e.ValorOriginalMoedaEstrangeira.val()
        };
        executarReST("AcertoFechamento/InserirDescontoMotorista", data, function (arg) {
            if (arg.Success) {
                LimparCampos(e);
                $("#" + e.Valor.id).focus();

                _gridDescontoMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios.");
    }
}

function EditarVariacaoMoedaEstrangeiraClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    _fechamentoAcertoViagem.MoedaCotacaoBancoCentralVariacao.val(e.CodigoMoedaCotacaoBancoCentral);
    _fechamentoAcertoViagem.ValorVariacao.val(e.ValorVariacao);
    _fechamentoAcertoViagem.JustificativaVariacao.codEntity(e.CodigoJustificativa);
    _fechamentoAcertoViagem.JustificativaVariacao.val(e.Justificativa);
    _fechamentoAcertoViagem.DataBaseCRTVariacao.val(e.DataBaseCRT);
    _fechamentoAcertoViagem.ValorOriginalVariacao.val(e.ValorOriginal);
    _fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.val(e.ValorMoedaCotacao);
    _fechamentoAcertoViagem.CodigoVariacao.val(e.CodigoVariacao)
    _fechamentoAcertoViagem.AdicionarVariacao.text("Atualizar");
}

function RemoverVariacaoMoedaEstrangeiraClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir a variação cambial selecionada?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoFechamento/RemoverVariacaoCambial", data, function (arg) {
            if (arg.Success) {

                _gridVariacoesCambial.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function EditarDevolucaoMoedaEstrangeiraClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.val(e.CodigoMoedaCotacaoBancoCentral);
    _fechamentoAcertoViagem.ValorDevolucao.val(e.ValorDevolucao);
    _fechamentoAcertoViagem.Justificativa.codEntity(e.CodigoJustificativa);
    _fechamentoAcertoViagem.Justificativa.val(e.Justificativa);
    _fechamentoAcertoViagem.DataBaseCRT.val(e.DataBaseCRT);
    _fechamentoAcertoViagem.ValorOriginal.val(e.ValorOriginal);
    _fechamentoAcertoViagem.ValorMoedaCotacao.val(e.ValorMoedaCotacao);
    _fechamentoAcertoViagem.CodigoDevolucao.val(e.CodigoDevolucao)
    _fechamentoAcertoViagem.AdicionarDevolucao.text("Atualizar");
}

function RemoverDevolucaoMoedaEstrangeiraClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja excluir a devolução selecionada?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoFechamento/RemoverDevolucaoMoedaEstrangeira", data, function (arg) {
            if (arg.Success) {

                _gridDevolucoesMoedaEstrangeira.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function RemoverDescontoMotoristaClick(e) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteAdicionarDesconto, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para remover desconto do motorista.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o desconto " + e.Motivo + "?", function () {

        var data = {
            Codigo: e.Codigo
        };
        executarReST("AcertoFechamento/RemoverDescontoMotorista", data, function (arg) {
            if (arg.Success) {

                _gridDescontoMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "falha", arg.Msg);
            }
        });
    });
}

function VisualizarDetalheClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    _gridVeiculoRelatorio = new GridView("qualquercoisa", "Relatorios/AcertoFechamentoRelatorio/Pesquisa", _fechamentoAcertoViagem);
    _fechamentoAcertoViagem.Codigo.val(_acertoViagem.Codigo.val());
    var _relatorioAcertoViagem = new RelatorioGlobal("Relatorios/AcertoFechamentoRelatorio/BuscarDadosRelatorio", _gridVeiculoRelatorio, function () {
        _relatorioAcertoViagem.loadRelatorio(function () {
            _relatorioAcertoViagem.gerarRelatorio("Relatorios/AcertoFechamentoRelatorio/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
        });
    }, null, null, _fechamentoAcertoViagem);

}

function VisualizarReciboMotoristaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    executarDownload("AcertoFechamento/ImprimirReciboMotorista", { Codigo: _acertoViagem.Codigo.val() });
}

function VisualizarReciboClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() !== EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem não se encontra finalizado.");
        return;
    }

    executarDownload("AcertoFechamento/ImprimirRecibo", { Codigo: _acertoViagem.Codigo.val() });

}

function FinalizarAcertoClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para Finalizar o Acerto de Viagem.");
        return;
    }
    _fechamentoAcertoViagem.FinalizarAcerto.enable(false);
    ValidarSaldosViagem(e, sender);
}

function exibirCamposObrigatorioFinalizacaoAcerto() {
    _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
    _finalizandoAcertoViagem = false;
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function RetornarOutraDespesaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteReabirAcerto, _PermissoesPersonalizadas)) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Seu usuário não possui permissão para Reabrir o Acerto de Viagem.");
        return;
    }
    ValidarEstornarSaldosViagem(e, sender);
}

function VerificaVisibilidadeBotoesFechamento(vFinalizarAcerto, vRetornarOutraDespesa) {
    _fechamentoAcertoViagem.FinalizarAcerto.visible(false);
    _fechamentoAcertoViagem.RetornarOutraDespesa.visible(false);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteReabirAcerto, _PermissoesPersonalizadas) && vRetornarOutraDespesa) {
        _fechamentoAcertoViagem.RetornarOutraDespesa.visible(true);
    }
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteFecharAcerto, _PermissoesPersonalizadas) && vFinalizarAcerto) {
        _fechamentoAcertoViagem.FinalizarAcerto.visible(true);
    }
}

//*******MÉTODOS*******

function CarregarFechamento() {
    _fechamentoAcertoViagem.Codigo.val(_acertoViagem.Codigo.val());
    _fechamentoAcertoViagem.SegmentoVeiculo.val(_acertoViagem.SegmentoVeiculo.val());
    _fechamentoAcertoViagem.SegmentoVeiculo.codEntity(_acertoViagem.SegmentoVeiculo.codEntity());

    _fechamentoAcertoViagem.Cheque.val(_acertoViagem.Cheque.val());
    _fechamentoAcertoViagem.Cheque.codEntity(_acertoViagem.Cheque.codEntity());

    carregarGridDevolucoesMoedaEstrangeira();
    carregarGridVariacaoCambial();
    carregarGridDescontoMotorista();
    carregarGridBonificacaoMotorista();
    carregarGridVeiculosFechamento();
    carregarGridAcertoViagemInfracao();

    _gridAcertoViagemTacografo.CarregarGrid();
    _gridAdiantamentos.CarregarGrid();
}

function carregarGridVariacaoCambial() {
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoVariacaoCambial"), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var excuir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverVariacaoMoedaEstrangeiraClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarVariacaoMoedaEstrangeiraClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = VisibilidadeOpcaoAuditoria() ? TypeOptionMenu.list : TypeOptionMenu.link;
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(excuir);
    if (VisibilidadeOpcaoAuditoria())
        menuOpcoes.opcoes.push(auditar);

    _gridVariacoesCambial = new GridView(_fechamentoAcertoViagem.VariacoesCambial.idGrid, "AcertoFechamento/PesquisarVariacaoCambial", _fechamentoAcertoViagem, menuOpcoes, null, null, null);
    _gridVariacoesCambial.CarregarGrid();
}

function carregarGridDevolucoesMoedaEstrangeira() {
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoDevolucaoMoedaEstrangeira"), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var excuir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverDevolucaoMoedaEstrangeiraClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarDevolucaoMoedaEstrangeiraClick, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = VisibilidadeOpcaoAuditoria() ? TypeOptionMenu.list : TypeOptionMenu.link;
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(excuir);
    if (VisibilidadeOpcaoAuditoria())
        menuOpcoes.opcoes.push(auditar);

    _gridDevolucoesMoedaEstrangeira = new GridView(_fechamentoAcertoViagem.DevolucoesMoedaEstrangeira.idGrid, "AcertoFechamento/PesquisarDevolucoesMoedaEstrangeira", _fechamentoAcertoViagem, menuOpcoes, null, null, null);
    _gridDevolucoesMoedaEstrangeira.CarregarGrid();
}

function carregarGridTacografo() {
    var editar = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverTacografoClick, tamanho: "10", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var editarColuna = { permite: true, callback: callbackEditarColunaTacografo, atualizarRow: false };

    _gridAcertoViagemTacografo = new GridView(_fechamentoAcertoViagem.AcertoViagemTacografo.idGrid, "AcertoFechamento/PesquisarTacografo", _fechamentoAcertoViagem, menuOpcoes, null, null, null, null, null, null, null, editarColuna);
    _gridAcertoViagemTacografo.CarregarGrid();

    //_gridAcertoViagemTacografo.SetarEditarColunas(editarColuna);
}

function carregarGridAdiantamentos() {
    var editar = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirAdiancamentoAcertoClick, tamanho: "10", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.tamanho = "10";
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAdiantamentos = new GridView(_fechamentoAcertoViagem.GridAdiantamentos.idGrid, "AcertoFechamento/PesquisarAdiantamento", _fechamentoAcertoViagem, menuOpcoes);
    _gridAdiantamentos.CarregarGrid();
}

function carregarGridDescontoMotorista() {
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoDesconto"), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverDescontoMotoristaClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarDescontoMotorista, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar, excluir, auditar] };

    _gridDescontoMotorista = new GridView(_fechamentoAcertoViagem.Descontos.idGrid, "AcertoFechamento/PesquisarDesconto", _fechamentoAcertoViagem, menuOpcoes, null, null, null);
    _gridDescontoMotorista.CarregarGrid();
}

function carregarGridBonificacaoMotorista() {
    var auditar = { descricao: "Auditar", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("AcertoBonificacao"), tamanho: "10", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: RemoverBonificacaoMotoristaClick, tamanho: "10", icone: "" };
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBonificacaoMotorista, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 10, opcoes: [editar, excluir, auditar] };

    _gridBonificacoesMotorista = new GridView(_fechamentoAcertoViagem.Bonificacoes.idGrid, "AcertoFechamento/PesquisarBonificacao", _fechamentoAcertoViagem, menuOpcoes, null, null, null);
    _gridBonificacoesMotorista.CarregarGrid();
}

function carregarGridVeiculosFechamento() {

    //_gridVeiculosFechamento = new GridView(_fechamentoAcertoViagem.VeiculosFechamento.idGrid, "AcertoFechamento/PesquisarVeiculos", _fechamentoAcertoViagem, null, null, null, null);
    //_gridVeiculosFechamento.CarregarGrid();

    var data = new Array();

    if (_acertoViagem.ListaVeiculosFechamento.val() != "" && _acertoViagem.ListaVeiculosFechamento.val().length > 0) {
        $.each(_acertoViagem.ListaVeiculosFechamento.val(), function (i, veiculo) {
            var obj = new Object();

            obj.Codigo = veiculo.Codigo;
            obj.CodigoAcerto = veiculo.CodigoAcerto;
            obj.Placa = veiculo.Placa;
            obj.Reboque = veiculo.Reboque;

            data.push(obj);
        });
    }

    _gridVeiculosFechamento.CarregarGrid(data);

}

function carregarGridAcertoViagemInfracao() {

    var data = new Array();

    if (_acertoViagem.ListaInfracoes.val() != "" && _acertoViagem.ListaInfracoes.val().length > 0) {
        $.each(_acertoViagem.ListaInfracoes.val(), function (i, veiculo) {
            var obj = new Object();

            obj.Codigo = veiculo.Codigo;
            obj.CodigoAcerto = veiculo.CodigoAcerto;
            obj.Placa = veiculo.Placa;
            obj.NumeroAtuacao = veiculo.NumeroAtuacao;
            obj.NumeroInfracao = veiculo.NumeroInfracao;

            data.push(obj);
        });
    }

    _gridAcertoViagemInfracao.CarregarGrid(data);
}

function ValidarEstornarSaldosViagem(e, sender) {
    var valorAlimentacaoSaldo = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAlimentacaoSaldo.val());
    if (isNaN(valorAlimentacaoSaldo))
        valorAlimentacaoSaldo = 0;

    var valorAdiantamentoSaldo = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAdiantamentoSaldo.val());
    if (isNaN(valorAdiantamentoSaldo))
        valorAdiantamentoSaldo = 0;

    if (((valorAlimentacaoSaldo >= 0) && (valorAdiantamentoSaldo >= 0)) || _CONFIGURACAO_TMS.ExibirSaldoPrevistoAcertoViagem) {
        exibirConfirmacao("Confirmação", "Realmente deseja reabrir o acerto de viagem?", function () {
            EstornarAcertoViagem(e, sender);
        });
    } else {
        var msgAviso = "Atenção para o resultado do saldo do motorista: </br>";

        if (valorAlimentacaoSaldo < 0)
            msgAviso += " Favor reverta manualmente o pagamento lançado sobre o valor faltante de alimentação. </br>";

        if (valorAdiantamentoSaldo < 0)
            msgAviso += " Favor reverta manualmente o pagamento lançado sobre o valor faltante de adiantamento. </br>";

        msgAviso += "Realmente deseja reabrir o acerto de viagem?";

        if (!_finalizandoAcertoViagem) {
            exibirConfirmacao("Confirmação", msgAviso, function () {
                EstornarAcertoViagem(e, sender);
            });
        }
    }
}

function ValidarSaldosViagem(e, sender) {
    var valorAlimentacaoSaldo = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAlimentacaoSaldo.val());
    if (isNaN(valorAlimentacaoSaldo))
        valorAlimentacaoSaldo = 0;

    var valorAdiantamentoSaldo = Globalize.parseFloat(_fechamentoAcertoViagem.ValorAdiantamentoSaldo.val());
    if (isNaN(valorAdiantamentoSaldo))
        valorAdiantamentoSaldo = 0;

    //validar se o valor está negativo e se não tem config informada
    if (_CONFIGURACAO_TMS.HabilitarFormaRecebimentoTituloAoMotorista) {
        var valorSaldoMotorista = Globalize.parseFloat(_fechamentoAcertoViagem.SaldoMotorista.val());
        if (isNaN(valorSaldoMotorista))
            valorSaldoMotorista = 0;
        if (valorSaldoMotorista <= 0) {
            if (_fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.val() == EnumFormaRecebimentoMotoristaAcerto.NadaFazer) {
                exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor informe uma forma de recebimento do motorista para finalizar o acerto.");
                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                return;
            } else if (_fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.val() == EnumFormaRecebimentoMotoristaAcerto.CriarTitulo) {
                _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.requiredClass("form-control");
                _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.requiredClass("form-control");
                var tudoCerto = true;
                if (_fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.codEntity() === 0) {
                    _fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.requiredClass("form-control is-invalid");
                    tudoCerto = false;
                }
                if (_fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.val() === "" || _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.val() === undefined || _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.val() === "  /  /     ") {
                    _fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.requiredClass("form-control is-invalid");
                    tudoCerto = false;
                }
                if (!tudoCerto) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor informe a data de vencimento e o tipo de movimento na forma de recebimento do motorista.");
                    _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                    return;
                }
            }
        }
    }

    var continuarFinalizacaoSemTacografo = false;
    if (_CONFIGURACAO_TMS.HabilitarLancamentoTacografo) {
        var registros = _gridAcertoViagemTacografo.NumeroRegistros();
        if (registros == null || registros == 0) {
            exibirConfirmacao("ATENÇÃO", "Não foi informado o(s) tacógrafo(s) do motorista, deseja continuar sem informar?", function () {
                continuarFinalizacaoSemTacografo = true;
                SeguirComFinalizacaoAcerto(e, sender, valorAlimentacaoSaldo, valorAdiantamentoSaldo, continuarFinalizacaoSemTacografo);
            }, function () {
                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
            });
        }
        else
            continuarFinalizacaoSemTacografo = true;
        SeguirComFinalizacaoAcerto(e, sender, valorAlimentacaoSaldo, valorAdiantamentoSaldo, continuarFinalizacaoSemTacografo);
    }
    else {
        continuarFinalizacaoSemTacografo = true;
        SeguirComFinalizacaoAcerto(e, sender, valorAlimentacaoSaldo, valorAdiantamentoSaldo, continuarFinalizacaoSemTacografo);
    }
}

function SeguirComFinalizacaoAcerto(e, sender, valorAlimentacaoSaldo, valorAdiantamentoSaldo, continuarFinalizacaoSemTacografo) {
    if ((valorAlimentacaoSaldo === 0 && valorAdiantamentoSaldo === 0) || _CONFIGURACAO_TMS.ExibirSaldoPrevistoAcertoViagem) {
        if (!_finalizandoAcertoViagem && continuarFinalizacaoSemTacografo) {
            exibirConfirmacao("Confirmação", "Realmente deseja finalizar o acerto de viagem?", function () {
                FinalizarAcertoViagem(e, sender);
            });
        }
        else if (!continuarFinalizacaoSemTacografo) {
            _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
        }
    } else if (continuarFinalizacaoSemTacografo) {
        var msgAviso = "Atenção para o resultado do saldo do motorista: </br>";
        if (_CONFIGURACAO_TMS.HabilitarControlarOutrasDespesas) {
            if (valorAlimentacaoSaldo < 0 || valorAdiantamentoSaldo < 0) {
                msgAviso += "O valor de saldo de alimentação/adiantamento está negativo.</br>";
                msgAviso += "Favor avalie os valores lançados ou realize um adiantamento ao motorista.</br>";
                exibirMensagem(tipoMensagem.aviso, "Aviso", msgAviso);
                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                return;
            }
        }
        if (valorAlimentacaoSaldo > 0)
            msgAviso += " O valor restante de alimentação será lançado no saldo do motorista. </br>";
        else if (valorAlimentacaoSaldo < 0)
            msgAviso += " O valor faltante de alimentação será lançado em um novo pagamento ao motorista. </br>";

        if (valorAdiantamentoSaldo > 0)
            msgAviso += " O valor restante de adiantamento será lançado no saldo do motorista. </br>";
        else if (valorAdiantamentoSaldo < 0)
            msgAviso += " O valor faltante de adiantamento será lançado em um novo pagamento ao motorista. </br>";

        msgAviso += "Realmente deseja finalizar o acerto de viagem?";

        if (!_finalizandoAcertoViagem) {
            exibirConfirmacao("Confirmação", msgAviso, function () {
                FinalizarAcertoViagem(e, sender);
            });
        }
    }
    else
        _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
}

function FinalizarAcertoViagem(e, sender) {
    _finalizandoAcertoViagem = true;
    preencherListasSelecao();
    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Fechamento);

    LimparCampoEntity(_acertoViagem.SegmentoVeiculo);
    LimparCampoEntity(_acertoViagem.Cheque);

    _acertoViagem.SegmentoVeiculo.val(_fechamentoAcertoViagem.SegmentoVeiculo.val());
    _acertoViagem.SegmentoVeiculo.codEntity(_fechamentoAcertoViagem.SegmentoVeiculo.codEntity());

    _acertoViagem.Cheque.val(_fechamentoAcertoViagem.Cheque.val());
    _acertoViagem.Cheque.codEntity(_fechamentoAcertoViagem.Cheque.codEntity());

    _acertoViagem.ValorTotalAlimentacaoRepassado.val(_fechamentoAcertoViagem.ValorTotalAlimentacaoRepassado.val());
    _acertoViagem.ValorAlimentacaoRepassado.val(_fechamentoAcertoViagem.ValorAlimentacaoRepassado.val());
    _acertoViagem.ValorAlimentacaoComprovado.val(_fechamentoAcertoViagem.ValorAlimentacaoComprovado.val());
    _acertoViagem.ValorAlimentacaoSaldo.val(_fechamentoAcertoViagem.ValorAlimentacaoSaldo.val());
    _acertoViagem.ValorTotalAdiantamentoRepassado.val(_fechamentoAcertoViagem.ValorTotalAdiantamentoRepassado.val());
    _acertoViagem.ValorAdiantamentoRepassado.val(_fechamentoAcertoViagem.ValorAdiantamentoRepassado.val());
    _acertoViagem.ValorAdiantamentoComprovado.val(_fechamentoAcertoViagem.ValorAdiantamentoComprovado.val());
    _acertoViagem.ValorAdiantamentoSaldo.val(_fechamentoAcertoViagem.ValorAdiantamentoSaldo.val());
    _acertoViagem.SaldoPrevistoAlimentacaoMotorista.val(_fechamentoAcertoViagem.SaldoPrevistoAlimentacaoMotorista.val());
    _acertoViagem.SaldoPrevistoOutrasDepesasMotorista.val(_fechamentoAcertoViagem.SaldoPrevistoOutrasDepesasMotorista.val());

    _acertoViagem.FormaRecebimentoMotoristaAcerto.val(_fechamentoAcertoViagem.FormaRecebimentoMotoristaAcerto.val());
    _acertoViagem.DataVencimentoMotoristaAcerto.val(_fechamentoAcertoViagem.DataVencimentoMotoristaAcerto.val());
    _acertoViagem.ObservacaoMotoristaAcerto.val(_fechamentoAcertoViagem.ObservacaoMotoristaAcerto.val());
    _acertoViagem.TipoMovimentoMotoristaAcerto.codEntity(_fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.codEntity());
    _acertoViagem.TipoMovimentoMotoristaAcerto.val(_fechamentoAcertoViagem.TipoMovimentoMotoristaAcerto.codEntity());

    _acertoViagem.ObservacaoAcertoMotorista.val(_fechamentoAcertoViagem.ObservacaoAcertoMotorista.val());
    _acertoViagem.Titulo.codEntity(_fechamentoAcertoViagem.Titulo.codEntity());
    _acertoViagem.Titulo.val(_fechamentoAcertoViagem.Titulo.codEntity());
    _acertoViagem.Banco.codEntity(_fechamentoAcertoViagem.Banco.codEntity());
    _acertoViagem.Banco.val(_fechamentoAcertoViagem.Banco.codEntity());


    Salvar(_acertoViagem, "AcertoFechamento/AtualizarFechamento", function (arg) {
        _finalizandoAcertoViagem = false;
        if (arg.Success) {
            if (arg.Data) {
                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Acerto de viagem finalizada com sucesso.");

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Fechamento);
            } else {
                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorioFinalizacaoAcerto);
}

function EstornarAcertoViagem(e, sender) {
    var reAbrirAcerto = (_fechamentoAcertoViagem.RetornarOutraDespesa.text() == "Reabrir Acerto");
    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.OutrasDespesas);
    preencherListasSelecao();

    LimparCampoEntity(_acertoViagem.SegmentoVeiculo);
    LimparCampoEntity(_acertoViagem.Cheque);

    _acertoViagem.SegmentoVeiculo.val(_fechamentoAcertoViagem.SegmentoVeiculo.val());
    _acertoViagem.SegmentoVeiculo.codEntity(_fechamentoAcertoViagem.SegmentoVeiculo.codEntity());

    _acertoViagem.Cheque.val(_fechamentoAcertoViagem.Cheque.val());
    _acertoViagem.Cheque.codEntity(_fechamentoAcertoViagem.Cheque.codEntity());

    _acertoViagem.ValorTotalAlimentacaoRepassado.val(_fechamentoAcertoViagem.ValorTotalAlimentacaoRepassado.val());
    _acertoViagem.ValorAlimentacaoRepassado.val(_fechamentoAcertoViagem.ValorAlimentacaoRepassado.val());
    _acertoViagem.ValorAlimentacaoComprovado.val(_fechamentoAcertoViagem.ValorAlimentacaoComprovado.val());
    _acertoViagem.ValorAlimentacaoSaldo.val(_fechamentoAcertoViagem.ValorAlimentacaoSaldo.val());
    _acertoViagem.ValorTotalAdiantamentoRepassado.val(_fechamentoAcertoViagem.ValorTotalAdiantamentoRepassado.val());
    _acertoViagem.ValorAdiantamentoRepassado.val(_fechamentoAcertoViagem.ValorAdiantamentoRepassado.val());
    _acertoViagem.ValorAdiantamentoComprovado.val(_fechamentoAcertoViagem.ValorAdiantamentoComprovado.val());
    _acertoViagem.ValorAdiantamentoSaldo.val(_fechamentoAcertoViagem.ValorAdiantamentoSaldo.val());
    _acertoViagem.SaldoPrevistoAlimentacaoMotorista.val(_fechamentoAcertoViagem.SaldoPrevistoAlimentacaoMotorista.val());
    _acertoViagem.SaldoPrevistoOutrasDepesasMotorista.val(_fechamentoAcertoViagem.SaldoPrevistoOutrasDepesasMotorista.val());

    Salvar(_acertoViagem, "AcertoFechamento/AtualizarFechamento", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Fechamento do acerto de viagem salvo com sucesso.");
                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Fechamento);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function carregarConteudosFechamentoHTML(calback) {
    $.get("Content/Static/Acerto/FechamentoAcertoViagem.html?dyn=" + guid(), function (data) {
        _HTMLFechamentoAcertoViagem = data;
        calback();
    });
}

function CarregarDadosSaldoMoedaEstrangeira() {
    if (_acertoViagem.Codigo.val() != null && _acertoViagem.Codigo.val() > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/DadosSaldoMoedaEstrangeira", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    var dataFechamento = { Data: arg.Data };
                    PreencherObjetoKnout(_detalheSaldoMoedaEstrangeira, dataFechamento);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}

function CarregarDadosFechamentoAcerto() {
    if (_acertoViagem.Codigo.val() != null && _acertoViagem.Codigo.val() > 0) {
        var data = {
            Codigo: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoFechamento/DadosFechamentoAcerto", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _gridDescontoMotorista.CarregarGrid();
                    _gridBonificacoesMotorista.CarregarGrid();
                    _gridDevolucoesMoedaEstrangeira.CarregarGrid();
                    _gridVariacoesCambial.CarregarGrid();
                    _gridAdiantamentos.CarregarGrid();

                    var dataFechamento = { Data: arg.Data };
                    PreencherObjetoKnout(_fechamentoAcertoViagem, dataFechamento);
                    RecarregarGridCheques();
                    RecarregarGridFolgas();
                    if (_acertoViagem.Situacao.val() === EnumSituacoesAcertoViagem.Fechado) {
                        if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
                            _fechamentoAcertoViagem.VisualizarRecibo.enable(true);
                            _fechamentoAcertoViagem.VisualizarRecibo.visible(true);
                        } else
                            _fechamentoAcertoViagem.VisualizarRecibo.visible(false);
                    } else {
                        _fechamentoAcertoViagem.VisualizarRecibo.visible(false);
                    }
                    assinado(arg.Data.Assinado);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    }
}

function ConverterConversaoMoeda() {
    var valorMoedaCotacao = Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorCotacao.val());
    var valorOriginal = Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorOrigem.val());
    if (valorOriginal > 0 && valorMoedaCotacao > 0) {
        _detalheSaldoMoedaEstrangeira.ValorFinal.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
    }
}

function ConverterConversaoMoedaValorFinal() {
    var valorFinal = Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorFinal.val());
    var valorOriginal = Globalize.parseFloat(_detalheSaldoMoedaEstrangeira.ValorOrigem.val());
    if (valorOriginal > 0 && valorFinal > 0) {
        _detalheSaldoMoedaEstrangeira.ValorCotacao.val(Globalize.format(valorFinal / valorOriginal, "n4"));
    }
}

function CalcularMoedaEstrangeiraDespesaBonificacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novaBonificacao.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novaBonificacao.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novaBonificacao.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorDespesaBonificacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorDespesaBonificacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novaBonificacao.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novaBonificacao.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novaBonificacao.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalDespesaBonificacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novaBonificacao.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novaBonificacao.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novaBonificacao.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function CalcularMoedaEstrangeiraDespesaDesconto() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _novoDesconto.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _novoDesconto.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _novoDesconto.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorDespesaDesconto();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorDespesaDesconto() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoDesconto.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoDesconto.ValorOriginalMoedaEstrangeira.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoDesconto.Valor.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalDespesaDesconto() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_novoDesconto.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_novoDesconto.Valor.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _novoDesconto.ValorOriginalMoedaEstrangeira.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function CalcularMoedaEstrangeiraDevolucao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _fechamentoAcertoViagem.MoedaCotacaoBancoCentral.val(), DataBaseCRT: _fechamentoAcertoViagem.DataBaseCRT.val() }, function (r) {
            if (r.Success) {
                _fechamentoAcertoViagem.ValorMoedaCotacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorDevolucao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorDevolucao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_fechamentoAcertoViagem.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_fechamentoAcertoViagem.ValorDevolucao.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _fechamentoAcertoViagem.ValorOriginal.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalDevolucao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_fechamentoAcertoViagem.ValorMoedaCotacao.val());
        var valorOriginal = Globalize.parseFloat(_fechamentoAcertoViagem.ValorOriginal.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _fechamentoAcertoViagem.ValorDevolucao.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}


function CalcularMoedaEstrangeiraVariacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        executarReST("Cotacao/ConverterMoedaEstrangeira", { MoedaCotacaoBancoCentral: _fechamentoAcertoViagem.MoedaCotacaoBancoCentralVariacao.val(), DataBaseCRT: _fechamentoAcertoViagem.DataBaseCRTVariacao.val() }, function (r) {
            if (r.Success) {
                _fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.val(Globalize.format(r.Data, "n10"));
                ConverterValorVariacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function ConverterValorVariacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.val());
        var valorOriginal = Globalize.parseFloat(_fechamentoAcertoViagem.ValorVariacao.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _fechamentoAcertoViagem.ValorOriginalVariacao.val(Globalize.format(valorOriginal * valorMoedaCotacao, "n2"));
        }
    }
}

function ConverterValorOriginalVariacao() {
    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) {
        var valorMoedaCotacao = Globalize.parseFloat(_fechamentoAcertoViagem.ValorMoedaCotacaoVariacao.val());
        var valorOriginal = Globalize.parseFloat(_fechamentoAcertoViagem.ValorOriginalVariacao.val());
        if (valorOriginal > 0 && valorMoedaCotacao > 0) {
            _fechamentoAcertoViagem.ValorVariacao.val(Globalize.format(valorOriginal / valorMoedaCotacao, "n2"));
        }
    }
}

function editarDescontoMotorista(descontoMotoristaGrid) {
    _novoDesconto.Codigo.val(descontoMotoristaGrid.Codigo);
    BuscarPorCodigo(_novoDesconto, "AcertoFechamento/BuscarDescontoPorCodigo", function (arg) {
        _novoDesconto.Atualizar.visible(true);
        _novoDesconto.Adicionar.visible(false);
        Global.abrirModal('divAdicionarDescontoMotorista');
    }, null);
}

function AtualizarDescontoMotoristaClick(e, sender) {
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }

    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    Salvar(_novoDesconto, "AcertoFechamento/AtualizarDescontoMotorista", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                LimparCampos(e);
                _gridDescontoMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
                Global.fecharModal('divAdicionarDescontoMotorista');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function editarBonificacaoMotorista(bonificacaoMotoristaGrid) {
    _novaBonificacao.Codigo.val(bonificacaoMotoristaGrid.Codigo);
    BuscarPorCodigo(_novaBonificacao, "AcertoFechamento/BuscarBonificacaoPorCodigo", function (arg) {
        _novaBonificacao.Atualizar.visible(true);
        _novaBonificacao.Adicionar.visible(false);
        Global.abrirModal('divAdicionarBonificacaoMotorista');
    }, null);
}

function AtualizarBonificacaoMotoristaClick(e, sender) {
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }

    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    Salvar(_novaBonificacao, "AcertoFechamento/AtualizarBonificacaoMotorista", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                LimparCampos(e);
                _gridBonificacoesMotorista.CarregarGrid();
                CarregarDadosFechamentoAcerto();
                Global.fecharModal('divAdicionarBonificacaoMotorista');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}