/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPlanejamentoPedidoTMS.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumDataPlanejamentoPedidoTMS.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumPedidosVinculadosCarga.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="PlanejamentoPedidoTMSDefinicaoVeiculo.js" />
/// <reference path="PlanejamentoPedidoTMSDisponibilidade.js" />
/// <reference path="../../Consultas/Carga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridPlanejamentoPedidoTMS;
var _pesquisaPlanejamentoPedidoTMS;
var _planejamentoPedidoTMSEmail;
var _pedidoGrupo = false;
var _detalhePedido;
var _detalhesAviso, _gridDetalhesAviso;
var _tratativas, _gridTratativas;
var _knoutAlterarDataPrevisaoSaida;
var _enviarEscala;
var _planejamentoPedidoPlacaCarregamento;
var _PermissoesPersonalizadasCarga;


var _operadorLogistica = {
    PermiteAdicionarComplementosDeFrete: null
};

var _tipoPropriedadeVeiculo = [
    { text: "Todos", value: "A" },
    { text: "Própria", value: "P" },
    { text: "Terceiros", value: "T" }
];

var _aceitaContratarTerceiros = [
    { text: "Todos", value: "" },
    { text: "Sim", value: EnumSimNaoPesquisa.Sim },
    { text: "Não", value: EnumSimNaoPesquisa.Nao }
];

/*
 * Declaração das Classes
 */

var PesquisaPlanejamentoPedidoTMS = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, val: ko.observable(Global.Data(EnumTipoOperacaoDate.Add, 5, 'D')), def: Global.Data(EnumTipoOperacaoDate.Add, 5, 'D') });
    this.NumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número Pedido no Embarcador:", issue: 902 });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: "Número do Pedido:", configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(true) });
    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: "Número da Carga:" });

    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoPedido.Aberto), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.Aberto });
    this.SituacaoPlanejamentoPedidoTMS = PropertyEntity({ text: "Situação de Planejamento: ", getType: typesKnockout.selectMultiple, val: ko.observable(EnumSituacaoPlanejamentoPedidoTMS.Pendente), options: EnumSituacaoPlanejamentoPedidoTMS.obterOpcoes(), def: EnumSituacaoPlanejamentoPedidoTMS.Pendente });

    this.TipoDataAgrupamento = PropertyEntity({ val: ko.observable(EnumDataPlanejamentoPedidoTMS.DataCarregamento), options: EnumDataPlanejamentoPedidoTMS.obterOpcoes(), def: EnumDataPlanejamentoPedidoTMS.DataCarregamento, text: "Tipo da Data: " });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: "Tipo de Remetente: ", issue: 306, required: true, visible: ko.observable(true), eventChange: tipoPessoaChange });
    this.TipoPropriedadeVeiculo = PropertyEntity({ val: ko.observable("A"), def: "A", options: _tipoPropriedadeVeiculo, text: "Tipo Propriedade Veículo: " });
    this.VinculoCarga = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Pedidos vinculados a carga:", options: EnumPedidosVinculadosCarga.obterOpcoes(), val: ko.observable([]), def: [] });

    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", issue: 145, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.CidadePoloOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Polo de Origem:", issue: 831, idBtnSearch: guid() });
    this.CidadePoloDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Polo de Destino:", issue: 831, idBtnSearch: guid() });
    this.PaisOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País de Origem:", idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "País de Destino:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", issue: 121, idBtnSearch: guid() });
    this.TipoOperacaoDiferenteDe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação diferente de:", issue: 121, idBtnSearch: guid() });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado Origem:", idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado Destino:", idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });
    this.Fronteira = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Fronteira:", idBtnSearch: guid() });
    this.Gestor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Gestor:", idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", idBtnSearch: guid() });
    this.SegmentoVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento de Veículo:", idBtnSearch: guid() });
    this.CategoriaOS = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Categoria OS:", options: EnumCategoriaOS.obterOpcoes(), val: ko.observable([]), def: [] });
    this.TipoOSConvertido = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Tipo OS Convertido:", options: EnumTipoOSConvertido.obterOpcoes(), val: ko.observable([]), def: [] });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.AceitaContratarTerceiros = PropertyEntity({ val: ko.observable(""), def: "", options: _aceitaContratarTerceiros, text: "Aceita contratar terceiros? " });


    this.CargaPerigosa = PropertyEntity({ text: "Carga Perigosa:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(9), def: 9 });
    this.AceiteMotorista = PropertyEntity({ text: "Aceite de escala do Motorista: ", val: ko.observable(EnumAceiteMotorista.Todos), options: EnumAceiteMotorista.obterOpcoesPesquisa(), def: EnumAceiteMotorista.Todos, required: true, visible: ko.observable(true) });


    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _pesquisaPlanejamentoPedidoTMS.ExibirFiltros.visibleFade(false);
            limparSelecionados();
            _gridPlanejamentoPedidoTMS.CarregarGrid();

            loadGridPlanejamentoPedidoTMSDisponibilidade();

        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            }
            else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.EnviarEmailTomador = PropertyEntity({
        eventClick: enviarEmailTomadorClick, type: types.event, text: "Enviar e-mail para o Tomador", idGrid: guid(), visible: ko.observable(false)
    });
    this.GerarCarga = PropertyEntity({
        eventClick: gerarCargaClick, type: types.event, text: "Gerar Carga", idGrid: guid(), visible: ko.observable(false)
    });
    this.EnviarEmailCheckList = PropertyEntity({
        eventClick: enviarEmailCheckListClick, type: types.event, text: "Enviar e-mail para o Check List", idGrid: guid(), visible: ko.observable(false)
    });
    this.EnviarEmailColeta = PropertyEntity({
        eventClick: enviarEmailColetaClick, type: types.event, text: "Enviar Ordem de Coleta ao Tomador", idGrid: guid(), visible: ko.observable(false)
    });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todos", visible: ko.observable(true) });

    //Aba Disponibilidade
    this.ExibirDisponibilidade = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: ExibirPlanejamentoPedidoTMSDisponibilidadeClick, text: ko.observable("Exibir Disponibilidade"), visible: ko.observable(true) });
    this.TituloDisponibilidade = PropertyEntity({ text: ko.observable("Disponibilidade") });
    this.VeiculosDisponiveis = PropertyEntity({ text: ko.observable("") });
    this.VeiculosAlocados = PropertyEntity({ text: ko.observable("") });

    this.NumeroFrotaDisponibilidade = PropertyEntity({ text: "Nº Frota: ", maxlength: 30 });
    this.MotoristaDisponibilidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.PesquisarDisponibilidade = PropertyEntity({
        eventClick: function () {
            _gridPlanejamentoPedidoTMSDisponibilidade.CarregarGrid(callbackGridPlanejamentoDisponibilidade);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

var PlanejamentoPedidoTMSEmail = function () {
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: confirmarEmailClick, text: "Confirmar", visible: ko.observable(true) });

    this.Email = PropertyEntity({ maxlength: 1000, getType: typesKnockout.multiplesEmails, required: true, text: "*E-mail:" });
    this.Assunto = PropertyEntity({ maxlength: 1000, required: true, text: "*Assunto:" });
};

var DetalhesAvisoPlanejamentoPedidoTMS = function () {
    this.Fechar = PropertyEntity({ type: types.event, eventClick: fecharDetalhesAvisoClick, text: "Fechar", visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Avisos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var TratativasPlanejamentoPedidoTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarNovaInformacaoTratativaClick, text: "Salvar Nova Informação", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: fecharTratativasClick, text: "Fechar", visible: ko.observable(true) });

    this.NovaInformacao = PropertyEntity({ maxlength: 5000, required: true, text: "*Nova informação:" });

    this.Grid = PropertyEntity({ type: types.local });
    this.Tratativas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
};

var AlterarDataPrevisaoSaida = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataPrevisaoSaida = PropertyEntity({ text: "*Data previsão de saída:", getType: typesKnockout.dateTime, required: ko.observable(true) });

    this.Enviar = PropertyEntity({ type: types.event, eventClick: enviarDataPrevisaoSaidaClick, text: "Enviar" });
};

var DetalhePedido = function (pedido) {
    this.DT_RowColor = PropertyEntity();
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataCriacao = PropertyEntity({ text: "Data Criação do Pedido: ", val: ko.observable("") });
    this.DataCarregamentoPedido = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.DataAgendamento = PropertyEntity({ text: "Data Agendamento: ", val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: "Nº Pedido: ", val: ko.observable(""), visible: ko.observable(false) });
    this.Tomador = PropertyEntity({ text: "Tomador: ", val: ko.observable(""), visible: ko.observable(false) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido Embarcador: ", val: ko.observable("") });
    this.Origem = PropertyEntity({ text: "Origem: ", val: ko.observable("") });
    this.ExibirOrigem = PropertyEntity({ val: ko.observable(false) });
    this.Destino = PropertyEntity({ text: "Destino: ", val: ko.observable("") });
    this.Remetente = PropertyEntity({ text: "Remetente: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: "Destinatário: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ text: "Expedidor: ", val: ko.observable(""), visible: ko.observable(false) });
    this.Recebedor = PropertyEntity({ text: "Recebedor: ", val: ko.observable(""), visible: ko.observable(false) });
    this.DestinoRecebedor = PropertyEntity({ text: "Destino: ", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable("") });
    this.ValorTotalPedido = PropertyEntity({ text: "Valor: ", val: ko.observable(""), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ text: "Transportador: ", val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: "Tipo de Operação: ", val: ko.observable("") });
    this.TipoOperacaoCodigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(false) });
    this.TotalPallets = PropertyEntity({ text: "Pallets: ", val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: "Cubagem (M³): ", val: ko.observable("") });
    this.Restricao = PropertyEntity({ text: "Restrição de Entrega: ", val: ko.observable("") });
    this.ObservacaoRestricao = PropertyEntity({ text: "Observação da Restrição: ", val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação do Pedido: ", val: ko.observable(""), visible: ko.observable(false) });
    this.ObservacaoInterna = PropertyEntity({ text: "Observação Interna: ", val: ko.observable("") });
    this.ObservacaoTipoOperacao = PropertyEntity({ text: "Observação Tipo de Operação: ", val: ko.observable("") });
    this.DataInicialColeta = PropertyEntity({ text: "Data da Coleta: ", val: ko.observable("") });
    this.DataPrevisaoSaida = PropertyEntity({ text: "Data Previsão de Saída: ", val: ko.observable("") });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data Prevista de Entrega/Retorno: ", val: ko.observable("") });
    this.ExigirPreCargaMontagemCarga = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false) });
    this.Temperatura = PropertyEntity({ text: "Temperatura: ", val: ko.observable("") });
    this.Vendedor = PropertyEntity({ text: "Vendedor: ", val: ko.observable("") });
    this.Ordem = PropertyEntity({ text: "Ordem: ", val: ko.observable("") });
    this.PortoSaida = PropertyEntity({ text: "Porto de Saída: ", val: ko.observable("") });
    this.PortoChegada = PropertyEntity({ text: "Porto de Chegada: ", val: ko.observable("") });
    this.Companhia = PropertyEntity({ text: "Companhia: ", val: ko.observable("") });
    this.NumeroNavio = PropertyEntity({ text: "Número do Navio: ", val: ko.observable("") });
    this.Reserva = PropertyEntity({ text: "Reserva: ", val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.Resumo = PropertyEntity({ text: "Resumo: ", val: ko.observable("") });
    this.TipoEmbarque = PropertyEntity({ text: "Tipo de Embarque: ", val: ko.observable("") });
    this.DeliveryTerm = PropertyEntity({ text: "Delivery Term: ", val: ko.observable("") });
    this.IdAutorizacao = PropertyEntity({ text: "Id de Autorização: ", val: ko.observable("") });
    this.DataETA = PropertyEntity({ text: "Data ETA: ", val: ko.observable(""), visible: !_CONFIGURACAO_TMS.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga });
    this.DataInclusaoBooking = PropertyEntity({ text: "Inclusão do Booking: ", val: ko.observable("") });
    this.DataInclusaoPCP = PropertyEntity({ text: "Inclusão do PCP: ", val: ko.observable("") });
    this.Selecionado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.SemLatLng = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedidoPrioritario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiAjudante = PropertyEntity({ text: "Possui Ajudante? ", val: ko.observable("") });
    this.PossuiAjudanteCarga = PropertyEntity({ text: "Possui Ajudante na Carga? ", val: ko.observable("") });
    this.PossuiAjudanteDescarga = PropertyEntity({ text: "Possui Ajudante na Descarga? ", val: ko.observable("") });
    this.PossuiCarga = PropertyEntity({ text: "Possui Carga? ", val: ko.observable("") });
    this.PossuiDescarga = PropertyEntity({ text: "Possui Descarga? ", val: ko.observable("") });
    this.ModeloVeicularCarga = PropertyEntity({ text: "Modelo Veicular de Carga: ", val: ko.observable("") });
    this.FormaPagamento = PropertyEntity({ text: "Forma de Pagamento: ", val: ko.observable("") });
    this.DiasDePrazoFatura = PropertyEntity({ text: "Dias de prazo do faturamento: ", val: ko.observable("") });
    this.QuantidadeVolumes = PropertyEntity({ text: "Qtde. Volumes: ", val: ko.observable("") });
    this.InfoPedido = PropertyEntity({ type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5"), visibleSemLatLng: ko.observable(false) });
    this.ValorFreteNegociado = PropertyEntity({ text: "Valor Frete Negociado: ", val: ko.observable(""), visible: ko.observable(true) });
    this.TipoPropriedadeVeiculo = PropertyEntity({ text: "Tipo Propriedade Veículo: ", val: ko.observable("") });
    this.LocalPaletizacao = PropertyEntity({ text: "Local de paletização: ", val: ko.observable(""), visible: ko.observable(false) });

    this.Email = PropertyEntity({ text: ko.observable("*E-mail: "), issue: 30, required: true, getType: typesKnockout.email, maxlength: 1000 });

    this.Operador = PropertyEntity({ text: "Operador: ", val: ko.observable("") });

    if (pedido != undefined && pedido != null) {
        PreencherObjetoKnout(this, { Data: pedido });

        this.Peso.val(pedido.PesoSaldoRestante);
        this.NumeroPedido.visible(true);
        this.InfoPedido.val(pedido.PedidoPrioritario);
    }
};

var EnviarEscala = function () {
    this.CodigoPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MotoristaEscala = PropertyEntity({ text: "Motorista: ", val: ko.observable(""), enable: ko.observable(false) });
    this.DataComparecerEscala = PropertyEntity({ text: "Comparecer no Dia: ", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.AceiteMotorista = PropertyEntity({ val: ko.observable(EnumAceiteMotorista.Enviado), options: EnumAceiteMotorista.obterOpcoes(), def: EnumAceiteMotorista.Enviado, text: "Aceite do Motorista: ", required: true, visible: ko.observable(true) });

    this.Enviar = PropertyEntity({ type: types.event, eventClick: salvarAceiteMotorista, text: "Enviar", visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridPlanejamentoPedidoTMS() {
    var opcaoDefinirVeiculo = { descricao: "Definir Veículo", id: guid(), metodo: definirVeiculoClick, icone: "", visibilidade: controlarVisibilidadeDefinirVeiculo };
    var opcaoDefinirPlacaCarregamento = { descricao: "Definir Placa de Carregamento", id: guid(), metodo: definirPlacaCarregamentoClick, icone: "", visibilidade: controlarVisibilidadeDefinirPlacaCarregamento };
    var opcaoRemoverVeiculo = { descricao: "Remover Veículo", id: guid(), metodo: removerVeiculoClick, icone: "", visibilidade: controlarVisibilidadeRemoverVeiculo };
    var opcaoSubstituirVeiculo = { descricao: "Substituir Veículo", id: guid(), metodo: substituirVeiculoClick, icone: "", visibilidade: controlarVisibilidadeSubstituirVeiculo };
    var opcaoDefinirMotorista = { descricao: "Definir Motorista", id: guid(), metodo: definirMotoristaClick, icone: "", visibilidade: controlarVisibilidadeDefinirMotorista };
    var opcaoSubstituirMotorista = { descricao: "Substituir Motorista", id: guid(), metodo: substituirMotoristaClick, icone: "", visibilidade: controlarVisibilidadeSubstituirMotorista };
    var opcaoRemoverMotorista = { descricao: "Remover Motorista", id: guid(), metodo: removerMotoristaClick, icone: "", visibilidade: controlarVisibilidadeRemoverMotorista };
    var opcaoSituacaoPendente = { descricao: "Pendente", id: guid(), metodo: situacaoPendenteClick, icone: "", visibilidade: controlarVisibilidadeSituacaoPendente };
    var opcaoSituacaoCheckListOK = { descricao: "Check List OK", id: guid(), metodo: situacaoCheckListOKClick, icone: "", visibilidade: controlarVisibilidadeSituacaoCheckListOK };
    var opcaoDetalhePedido = { descricao: "Detalhe do Pedido", id: guid(), metodo: detalhePedidoClick, icone: "" };
    var opcaoSimularFrete = { descricao: "Simular Frete", id: guid(), metodo: simularFreteClick, icone: "", visibilidade: controlarVisibilidadeSimularFrete };
    var opcaoAvisoMotorista = { descricao: "Aviso ao Motorista", id: guid(), metodo: registrarAvisoAoMotoristaClick, icone: "" };
    var opcaoMotoristaCiente = { descricao: "Motorista Ciente", id: guid(), metodo: registrarMotoristaCienteClick, icone: "" };
    var opcaoDetalhesAviso = { descricao: "Detalhes do Aviso", id: guid(), metodo: detalhesAvisoClick, icone: "" };
    var opcaoTratativas = { descricao: "Tratativas", id: guid(), metodo: detalhesTratativasClick, icone: "" };
    var opcaoDefinirModeloVeicular = { descricao: "Definir Modelo Veicular", id: guid(), metodo: definirModeloVeicularClick, icone: "", visibilidade: controlarVisibilidadeDefinirModeloVeicular };
    var opcaoSubstituirModeloVeicular = { descricao: "Substituir Modelo Veicular", id: guid(), metodo: substituirModeloVeicularClick, icone: "", visibilidade: controlarVisibilidadeSubstituirModeloVeicular };
    var opcaoAbrirWhatsApp = { descricao: "Abrir WhatsApp Motorista", id: guid(), metodo: abrirWhatsAppMotoristaClick, icone: "", visibilidade: controlarVisibilidadeAbrirWhatsApp };
    var opcaoSubstituirTipoOperacao = { descricao: "Tipo de Operação", id: guid(), metodo: substituirTipoOperacaoClick, icone: "" };
    var opcaoOrdemColeta = { descricao: "Imprimir Ordem de Coleta", id: guid(), metodo: ordemColetaClick, icone: "" };
    var opcaoSituacaoDevolucao = { descricao: "Devolução", id: guid(), metodo: situacaoDevolucaoClick, icone: "" };
    var opcaoAlterarDataPrevisaoSaida = { descricao: "Alterar Data Previsão Saída", id: guid(), metodo: abrirModalAlterarDataPrevisaoSaidaClick, icone: "" };
    var opcaoSubstituirTipoCarga = { descricao: "Tipo de Carga", id: guid(), metodo: substituirTipoCargaClick, icone: "" };
    var opcaoEnviarEscala = { descricao: "Enviar Escala", id: guid(), metodo: buscarAceiteMotorista, icone: "" };
    var dadosCarga = { descricao: "Dados da Carga", id: guid(), metodo: visualizaDadosCargaClick, icone: "" };
    var configuracoesExportacao = { url: "PlanejamentoPedidoTMS/ExportarPesquisa", titulo: "Planejamento Pedido TMS" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5,
        opcoes: [opcaoDefinirVeiculo, opcaoDefinirPlacaCarregamento, opcaoSubstituirVeiculo, opcaoRemoverVeiculo, opcaoDefinirMotorista, opcaoSubstituirMotorista, opcaoRemoverMotorista, opcaoSituacaoPendente, opcaoSituacaoCheckListOK, opcaoDetalhePedido, opcaoSimularFrete,
            opcaoAvisoMotorista, opcaoMotoristaCiente, opcaoDetalhesAviso, opcaoTratativas, opcaoDefinirModeloVeicular, opcaoSubstituirModeloVeicular, opcaoAbrirWhatsApp, opcaoSubstituirTipoOperacao, opcaoOrdemColeta,
            opcaoSituacaoDevolucao, opcaoAlterarDataPrevisaoSaida, opcaoSubstituirTipoCarga, opcaoEnviarEscala, dadosCarga]
    };

    var quantidadePorPagina = 200;

    var editarColuna = {
        permite: true,
        callback: editarColunaRetorno,
        atualizarRow: false
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaPlanejamentoPedidoTMS.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    _gridPlanejamentoPedidoTMS = new GridViewExportacao("grid-pesquisa-pedidos", "PlanejamentoPedidoTMS/Pesquisa", _pesquisaPlanejamentoPedidoTMS, menuOpcoes, configuracoesExportacao, null, quantidadePorPagina, multiplaEscolha, null, editarColuna, gridPlanejamentoPedidoTMSCallbackColumnDefault);
    _gridPlanejamentoPedidoTMS.SetPermitirRedimencionarColunas(true);
    _gridPlanejamentoPedidoTMS.SetGroup({ enable: true, propAgrupa: "DataEstado", dirOrdena: orderDir.asc });
    _gridPlanejamentoPedidoTMS.SetPermitirEdicaoColunas(true);
    _gridPlanejamentoPedidoTMS.SetSalvarPreferenciasGrid(true);
    _gridPlanejamentoPedidoTMS.CarregarGrid();

    loadGridPlanejamentoPedidoTMSDisponibilidade();

    loadPlanejamentoVeiculoDefinicaoPLacaCarregamento();

}


function loadPlanejamentoPedidoTMS() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLComponenteControleEntrega(function () {
                _pesquisaPlanejamentoPedidoTMS = new PesquisaPlanejamentoPedidoTMS();
                KoBindings(_pesquisaPlanejamentoPedidoTMS, "knockoutPesquisaPlanejamentoPedidoTMS", false, _pesquisaPlanejamentoPedidoTMS.Pesquisar.id);

                _planejamentoPedidoTMSEmail = new PlanejamentoPedidoTMSEmail();
                KoBindings(_planejamentoPedidoTMSEmail, "knockoutPlanejamentoPedidoTMSEmail");

                _detalhePedido = new DetalhePedido({});
                KoBindings(_detalhePedido, "knoutDetalhePedido");

                _detalhesAviso = new DetalhesAvisoPlanejamentoPedidoTMS();
                KoBindings(_detalhesAviso, "knockoutDetalhesAvisoPlanejamentoPedidoTMS");

                _tratativas = new TratativasPlanejamentoPedidoTMS();
                KoBindings(_tratativas, "knockoutTratativasPlanejamentoPedidoTMS");

                _knoutAlterarDataPrevisaoSaida = new AlterarDataPrevisaoSaida();
                KoBindings(_knoutAlterarDataPrevisaoSaida, "knoutDataPrevisaoSaida");

                new BuscarClientes(_pesquisaPlanejamentoPedidoTMS.Remetente);
                new BuscarGruposPessoas(_pesquisaPlanejamentoPedidoTMS.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
                new BuscarLocalidades(_pesquisaPlanejamentoPedidoTMS.Origem);
                new BuscarClientes(_pesquisaPlanejamentoPedidoTMS.Destinatario);
                new BuscarClientes(_pesquisaPlanejamentoPedidoTMS.Tomador);
                new BuscarVeiculos(_pesquisaPlanejamentoPedidoTMS.Veiculo);
                new BuscarMotorista(_pesquisaPlanejamentoPedidoTMS.Motorista);
                new BuscarLocalidades(_pesquisaPlanejamentoPedidoTMS.Destino);
                new BuscarLocalidadesPolo(_pesquisaPlanejamentoPedidoTMS.CidadePoloOrigem);
                new BuscarLocalidadesPolo(_pesquisaPlanejamentoPedidoTMS.CidadePoloDestino);
                new BuscarPaises(_pesquisaPlanejamentoPedidoTMS.PaisOrigem);
                new BuscarPaises(_pesquisaPlanejamentoPedidoTMS.PaisDestino);
                new BuscarTiposdeCarga(_pesquisaPlanejamentoPedidoTMS.TipoCarga);
                new BuscarTiposOperacao(_pesquisaPlanejamentoPedidoTMS.TipoOperacao);
                new BuscarTiposOperacao(_pesquisaPlanejamentoPedidoTMS.TipoOperacaoDiferenteDe);
                new BuscarMotorista(_pesquisaPlanejamentoPedidoTMS.MotoristaDisponibilidade);
                new BuscarEstados(_pesquisaPlanejamentoPedidoTMS.EstadoOrigem);
                new BuscarEstados(_pesquisaPlanejamentoPedidoTMS.EstadoDestino);
                new BuscarCentroResultado(_pesquisaPlanejamentoPedidoTMS.CentroResultado);
                new BuscarFuncionario(_pesquisaPlanejamentoPedidoTMS.FuncionarioResponsavel);
                new BuscarClientes(_pesquisaPlanejamentoPedidoTMS.Fronteira, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
                new BuscarFuncionario(_pesquisaPlanejamentoPedidoTMS.Gestor);
                new BuscarModelosVeicularesCarga(_pesquisaPlanejamentoPedidoTMS.ModeloVeicularCarga);
                new BuscarSegmentoVeiculo(_pesquisaPlanejamentoPedidoTMS.SegmentoVeiculo);
                new BuscarOperador(_pesquisaPlanejamentoPedidoTMS.Operador);

                loadPlanejamentoVeiculoDefinicaoVeiculo();
                loadGridPlanejamentoPedidoTMS();
                LoadGridDetalhesAviso();
                LoadGridTratativas();
                loadEnviarEscala();
                loadPlanejamentoPedidoTMSTipoOperacaoAnexo();
                loadSimularFrete();
            });
        });
    });
}




/*
 * Declaração das Funções Associadas a Eventos
 */
function definirVeiculoClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.codEntity(pedidoSelecionado.TipoOperacaoCodigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.val(pedidoSelecionado.TipoOperacao);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Veiculo.idBtnSearch).click();
}
function definirPlacaCarregamentoClick(pedidoSelecionado) {
    exibirModalPlanejamentoVeiculoDefinicaoPlacaCarregamento(pedidoSelecionado.Codigo);
}
function substituirVeiculoClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.codEntity(pedidoSelecionado.TipoOperacaoCodigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.val(pedidoSelecionado.TipoOperacao);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Veiculo.idBtnSearch).click();
}

function removerVeiculoClick(pedidoSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o veículo do pedido?", function () {
        executarReST("PlanejamentoPedidoTMS/RemoverVeiculo", { Codigo: pedidoSelecionado.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _gridPlanejamentoPedidoTMS.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Veículo removido com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

function definirMotoristaClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Motorista.idBtnSearch).click();
}
function visualizaDadosCargaClick(pedidoSelecionado) {
    dadosCargaClick(pedidoSelecionado.CodigoCarga);
}

function substituirMotoristaClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.Motorista.idBtnSearch).click();
}

function removerMotoristaClick(pedidoSelecionado) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o motorista do pedido?", function () {
        executarReST("PlanejamentoPedidoTMS/RemoverMotorista", { Codigo: pedidoSelecionado.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _gridPlanejamentoPedidoTMS.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Motorista removido com sucesso.");
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}
function dadosCargaClick(CodCarga) {
    var data = { Carga: CodCarga };

    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="close" data-dismiss="modal"');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
                _cargaAtual = knoutCarga;
                Global.abrirModal('divModalDetalhesCargaFDS');
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
function detalhePedidoClick(pedidoSelecionado) {
    executarReST("MontagemCargaPedido/BuscarPorCodigoPedido", { Codigo: pedidoSelecionado.Codigo, PlanejamentoPedido: true }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhePedido, { Data: retorno.Data.DetalhesPedido });

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
                    _detalhePedido.ValorFreteNegociado.val("");

                _detalhePedido.ModeloVeicularCarga.val(retorno.Data.DetalhesPedido.ModeloVeicularCarga.Descricao);
                PreencherGridAnexos(retorno.Data);

                Global.abrirModal('modalDetalhePedido');
                $("#modalDetalhePedido").one('hidden.bs.modal', function () {
                    LimparCampos(_detalhePedido);

                    $("a[href='#knoutDetalhePedido']").click();
                });
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function situacaoPendenteClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedidoTMS(pedidoSelecionado, EnumSituacaoPlanejamentoPedidoTMS.Pendente);
}

function situacaoCheckListOKClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedidoTMS(pedidoSelecionado, EnumSituacaoPlanejamentoPedidoTMS.CheckListOK);
}

function situacaoDevolucaoClick(pedidoSelecionado) {
    alterarSituacaoPlanejamentoPedidoTMS(pedidoSelecionado, EnumSituacaoPlanejamentoPedidoTMS.Devolucao);
}

function registrarAvisoAoMotoristaClick(pedidoSelecionado) {
    registrarAvisoAoMotoristaPlanejamentoPedidoTMS(pedidoSelecionado, false);
}

function registrarMotoristaCienteClick(pedidoSelecionado) {
    registrarAvisoAoMotoristaPlanejamentoPedidoTMS(pedidoSelecionado, true);
}

function ordemColetaClick(pedidoSelecionado) {
    var data = { Codigo: pedidoSelecionado.Codigo, Carregamento: false, OrdemColeta: false, PlanejamentoPedido: true };
    executarReST("Pedido/GerarRelatorio", data, function (arg) {
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

function substituirTipoOperacaoClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.idBtnSearch).click();
}

function substituirTipoCargaClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.TipoCarga.idBtnSearch).click();
}

function abrirWhatsAppMotoristaClick(pedidoSelecionado) {
    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirWhatsAppMotorista, _PermissoesPersonalizadas)) {
        window.open('https://web.whatsapp.com/send?phone=55' + string.OnlyNumbers(pedidoSelecionado.NumeroCelularMotorista));
    } else
        exibirMensagem(tipoMensagem.atencao, "Sem Permissão", "Usuário não possui permissão para completar essa ação.");
}

function definirModeloVeicularClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.ModeloVeicular.idBtnSearch).click();
}

function substituirModeloVeicularClick(pedidoSelecionado) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(pedidoSelecionado.Codigo);
    $("#" + _planejamentoVeiculoDefinicaoVeiculo.ModeloVeicular.idBtnSearch).click();
}

function detalhesAvisoClick(pedidoSelecionado) {
    executarReST("PlanejamentoPedidoTMS/BuscarDetalhesAviso", { Codigo: pedidoSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_detalhesAviso, retorno);
                RecarregarGridDetalhesAviso();

                Global.abrirModal('divModalDetalhesAviso');
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function detalhesTratativasClick(pedidoSelecionado) {
    buscarDetalhesTratativas(pedidoSelecionado.Codigo);
}

function buscarDetalhesTratativas(codigo) {
    executarReST("PlanejamentoPedidoTMS/BuscarDetalhesTratativas", { Codigo: codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_tratativas, retorno);
                RecarregarGridTratativas();

                Global.abrirModal('divModalTrativas');
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function abrirModalAlterarDataPrevisaoSaidaClick(pedidoSelecionado) {
    LimparCampos(_knoutAlterarDataPrevisaoSaida);
    _knoutAlterarDataPrevisaoSaida.Codigo.val(pedidoSelecionado.Codigo);
    _knoutAlterarDataPrevisaoSaida.DataPrevisaoSaida.val(pedidoSelecionado.DataPrevisaoSaida);
    Global.abrirModal('divModalDataPrevisaoSaida');
}

function enviarDataPrevisaoSaidaClick() {
    if (!ValidarCamposObrigatorios(_knoutAlterarDataPrevisaoSaida))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    exibirConfirmacao("Confirmação", "Você realmente deseja alterar a Data de Previsão de Saída do pedido?", function () {
        var dados = {
            Codigo: _knoutAlterarDataPrevisaoSaida.Codigo.val(),
            DataPrevisaoSaida: _knoutAlterarDataPrevisaoSaida.DataPrevisaoSaida.val()
        };

        executarReST("PlanejamentoPedidoTMS/AlterarDataPrevisaoSaida", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalDataPrevisaoSaida');
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                    _gridPlanejamentoPedidoTMS.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}

function tipoPessoaChange(e) {
    if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        e.Remetente.visible(true);
        e.GrupoPessoa.visible(false);

        LimparCampoEntity(e.GrupoPessoa);
    }
    else if (e.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        e.Remetente.visible(false);
        e.GrupoPessoa.visible(true);

        LimparCampoEntity(e.Remetente);
    }
}

/*
 * Declaração das Funções
 */

function gridPlanejamentoPedidoTMSCallbackColumnDefault(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "Tratativa") {
        if (dadosLinha.Tratativa)
            return '<i class="fal fa-pencil" style="font-size: 15px;" title="Tratativa"></i>';

        return "<span></span>";
    }
}

function alterarSituacaoPlanejamentoPedidoTMS(pedidoSelecionado, novaSituacao) {
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        SituacaoPlanejamentoPedidoTMS: novaSituacao
    };

    executarReST("PlanejamentoPedidoTMS/AlterarSituacaoPlanejamentoPedido", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function registrarAvisoAoMotoristaPlanejamentoPedidoTMS(pedidoSelecionado, motoristaCiente) {
    var dados = {
        Codigo: pedidoSelecionado.Codigo,
        MotoristaCiente: motoristaCiente
    };

    executarReST("PlanejamentoPedidoTMS/RegistrarAvisoAoMotorista", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data)
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}
function controlarVisibilidadeDefinirPlacaCarregamento(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo && pedidoSelecionado.NecessitaInformarPlacaCarregamento;
}
function controlarVisibilidadeSimularFrete(pedidoSelecionado){
    return pedidoSelecionado.SituacaoPlanejamentoPedidoTMS != EnumSituacaoPlanejamentoPedidoTMS.Pendente;
}

function controlarVisibilidadeSituacaoPendente(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedidoTMS != EnumSituacaoPlanejamentoPedidoTMS.Pendente;
}

function controlarVisibilidadeSituacaoCheckListOK(pedidoSelecionado) {
    return pedidoSelecionado.SituacaoPlanejamentoPedidoTMS != EnumSituacaoPlanejamentoPedidoTMS.CheckListOK;
}

function controlarVisibilidadeDefinirVeiculo(pedidoSelecionado) {
    return !pedidoSelecionado.PossuiVeiculo;
}

function controlarVisibilidadeSubstituirVeiculo(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo;
}

function controlarVisibilidadeRemoverVeiculo(pedidoSelecionado) {
    return pedidoSelecionado.PossuiVeiculo;
}

function controlarVisibilidadeDefinirMotorista(pedidoSelecionado) {
    return !pedidoSelecionado.PossuiMotorista;
}

function controlarVisibilidadeSubstituirMotorista(pedidoSelecionado) {
    return pedidoSelecionado.PossuiMotorista;
}

function controlarVisibilidadeRemoverMotorista(pedidoSelecionado) {
    return pedidoSelecionado.PossuiMotorista;
}

function controlarVisibilidadeDefinirModeloVeicular(pedidoSelecionado) {
    return !pedidoSelecionado.PossuiModeloVeicular;
}

function controlarVisibilidadeSubstituirModeloVeicular(pedidoSelecionado) {
    return pedidoSelecionado.PossuiModeloVeicular;
}

function controlarVisibilidadeAbrirWhatsApp(pedidoSelecionado) {
    return !string.IsNullOrWhiteSpace(pedidoSelecionado.NumeroCelularMotorista);
}

function editarColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress) {
    if (_CONFIGURACAO_TMS.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta)
        validarSeVeiculoFrotaEmOutroPedidoCarga(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress);
    else
        alterandoColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress);
}

function ValidarFrotaSituacaoMotorista(pedidoSelecionado) {
    if (pedidoSelecionado.NumeroFrota) {
        exibirMensagem(tipoMensagem.aviso, "", "Validando motorista da frota selecionado");
        executarReST("Motorista/ValidarFrotaMotoristaSituacao", { NumeroFrota: pedidoSelecionado.NumeroFrota }, function (arg) {
            if (arg.Success && arg.Data && arg.Data.ExibirConfirmacaoMotoristaSituacao)
                exibirAlerta("Atenção", arg.Msg, "Ok");

            _codigoMotoristaSelecionado = null;
        });
    }
}

function alterandoColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress) {
    var dadosPedidoAtualizar = {
        Codigo: pedidoSelecionado.Codigo,
        Ordem: pedidoSelecionado.Ordem,
        ObservacaoInterna: pedidoSelecionado.ObservacaoInterna,
        NumeroFrota: pedidoSelecionado.NumeroFrota,
        NumeroRota: pedidoSelecionado.NumeroRota,
        TipoDataAgrupamento: _pesquisaPlanejamentoPedidoTMS.TipoDataAgrupamento.val()
    };
    executarReST("PlanejamentoPedidoTMS/AlterarDadosPedido", dadosPedidoAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {

                _gridPlanejamentoPedidoTMS.AtualizarDataRow(linhaSelecionada, retorno.Data, callbackTabPress);

                if (_pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val())
                    _gridPlanejamentoPedidoTMSDisponibilidade.CarregarGrid(callbackGridPlanejamentoDisponibilidade);

                ValidarFrotaSituacaoMotorista(pedidoSelecionado);

                if (retorno.Msg != "")
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
            else {
                _gridPlanejamentoPedidoTMS.DesfazerAlteracaoDataRow(linhaSelecionada);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
        }
        else {
            _gridPlanejamentoPedidoTMS.DesfazerAlteracaoDataRow(linhaSelecionada);
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}

function validarSeVeiculoFrotaEmOutroPedidoCarga(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress) {
    var dadosFrota = {
        NumeroFrota: pedidoSelecionado.NumeroFrota
    };
    executarReST("PlanejamentoPedidoTMS/ValidarSeFrotaVeiculoOutroPedidoOuCarga", dadosFrota, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (!string.IsNullOrWhiteSpace(retorno.Msg))
                    exibirConfirmacao("Atenção!", retorno.Msg, function () { alterandoColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress) }, function () { _gridPlanejamentoPedidoTMS.DesfazerAlteracaoDataRow(linhaSelecionada) });
                else
                    alterandoColunaRetorno(pedidoSelecionado, linhaSelecionada, cabecalho, callbackTabPress);
            }
            else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
                _gridPlanejamentoPedidoTMS.DesfazerAlteracaoDataRow(linhaSelecionada);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
            _gridPlanejamentoPedidoTMS.DesfazerAlteracaoDataRow(linhaSelecionada);
        }
    });
}

function limparSelecionados() {
    _gridPlanejamentoPedidoTMS.AtualizarRegistrosSelecionados([]);
    _gridPlanejamentoPedidoTMS.AtualizarRegistrosNaoSelecionados([]);
    _pesquisaPlanejamentoPedidoTMS.SelecionarTodos.val(false);

    exibirMultiplasOpcoes();
}

function exibirMultiplasOpcoes() {
    var existemRegistrosSelecionados = _gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados().length > 0;

    if (existemRegistrosSelecionados) {

        _pesquisaPlanejamentoPedidoTMS.EnviarEmailTomador.visible(true);
        _pesquisaPlanejamentoPedidoTMS.GerarCarga.visible(true);
        _pesquisaPlanejamentoPedidoTMS.EnviarEmailCheckList.visible(true);
        _pesquisaPlanejamentoPedidoTMS.EnviarEmailColeta.visible(true);
    }
    else {
        _pesquisaPlanejamentoPedidoTMS.EnviarEmailTomador.visible(false);
        _pesquisaPlanejamentoPedidoTMS.GerarCarga.visible(false);
        _pesquisaPlanejamentoPedidoTMS.EnviarEmailCheckList.visible(false);
        _pesquisaPlanejamentoPedidoTMS.EnviarEmailColeta.visible(false);
    }
}

function enviarEmailColetaClick() {
    var dados = {
        ItensSelecionados: ""
    }

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados());

    executarReST("PlanejamentoPedidoTMS/EnviarEmailOrdemColeta", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridPlanejamentoPedidoTMS.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "E-mail(s) enviados com sucesso.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function enviarEmailCheckListClick() {
    Global.abrirModal('divModalEmail');
}

function confirmarEmailClick() {
    if (!ValidarCamposObrigatorios(_planejamentoPedidoTMSEmail)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }
    if (!ValidarMultiplosEmails(_planejamentoPedidoTMSEmail.Email.val())) {
        exibirMensagem(tipoMensagem.atencao, "E-mails Inválidos", "Favor verificar os e-mails informados!");
        return;
    }

    var dados = {
        Email: _planejamentoPedidoTMSEmail.Email.val(),
        Assunto: _planejamentoPedidoTMSEmail.Assunto.val(),
        ItensSelecionados: ""
    }

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados());

    executarReST("PlanejamentoPedidoTMS/EnviarEmailCheckList", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                LimparCampos(_planejamentoPedidoTMSEmail);
                Global.fecharModal("divModalEmail");
                _gridPlanejamentoPedidoTMS.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "E-mail(s) enviados com sucesso.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function enviarEmailTomadorClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaPlanejamentoPedidoTMS);

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados());

    executarReST("PlanejamentoPedidoTMS/EnviarEmailTomadorPedidosSelecionados", dados, function (r) {
        if (r.Success) {
            if (r.Data)
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "E-mail(s) para os Tomadores enviados com sucesso.");
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function gerarCargaClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaPlanejamentoPedidoTMS);

    dados.ItensSelecionados = JSON.stringify(_gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados());

    let arrayNumerosPedidos = new Array();
    var pedidosSelecionados = _gridPlanejamentoPedidoTMS.ObterMultiplosSelecionados();
    for (let x = 0; x < pedidosSelecionados.length; x++) {
        arrayNumerosPedidos.push(pedidosSelecionados[x].NumeroPedidoEmbarcador);
    }

    exibirConfirmacao("Atenção!", "Realmente deseja gerar uma carga para os pedidos " + arrayNumerosPedidos.join(', ') + " ?", function () {
        executarReST("PlanejamentoPedidoTMS/GerarCargasPedidos", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    limparSelecionados();
                    _gridPlanejamentoPedidoTMS.CarregarGrid();
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", r.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        });
    });
}

function salvarNovaInformacaoTratativaClick() {
    if (!ValidarCamposObrigatorios(_tratativas)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    var dados = {
        Codigo: _tratativas.Codigo.val(),
        NovaInformacao: _tratativas.NovaInformacao.val()
    };

    executarReST("PlanejamentoPedidoTMS/RegistrarTratativa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                LimparCampo(_tratativas.NovaInformacao);
                buscarDetalhesTratativas(_tratativas.Codigo.val());
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function LoadGridDetalhesAviso() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroSequencia", visible: false },
        { data: "Registro", title: "Registros", width: "90%" }
    ];

    _gridDetalhesAviso = new BasicDataTable(_detalhesAviso.Grid.id, header, null, { column: 1, dir: orderDir.asc });
}

function LoadGridTratativas() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "NumeroSequencia", visible: false },
        { data: "Registro", title: "Registros", width: "90%" }
    ];

    _gridTratativas = new BasicDataTable(_tratativas.Grid.id, header, null, { column: 1, dir: orderDir.asc });
}

function RecarregarGridDetalhesAviso() {
    var data = new Array();

    $.each(_detalhesAviso.Avisos.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.NumeroSequencia = item.NumeroSequencia.val;
        itemGrid.Registro = item.Registro.val;

        data.push(itemGrid);
    });

    _gridDetalhesAviso.CarregarGrid(data);
}

function RecarregarGridTratativas() {
    var data = new Array();

    $.each(_tratativas.Tratativas.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.NumeroSequencia = item.NumeroSequencia.val;
        itemGrid.Registro = item.Registro.val;

        data.push(itemGrid);
    });

    _gridTratativas.CarregarGrid(data);
}

function fecharDetalhesAvisoClick() {
    Global.fecharModal("divModalDetalhesAviso");
}

function fecharTratativasClick() {
    Global.fecharModal("divModalTrativas");
}

function salvarAceiteMotorista(e) {
    var dados = {
        Codigo: _enviarEscala.CodigoPedido.val(),
        MotoristaEscala: _enviarEscala.MotoristaEscala.val(),
        DataComparecerEscala: _enviarEscala.DataComparecerEscala.val(),
        AceiteMotorista: _enviarEscala.AceiteMotorista.val(),
    }

    executarReST("PlanejamentoPedidoTMS/SalvarEnviarEscala", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.fecharModal("divModalEnviarEscala");
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Escala enviada com sucesso.");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function buscarAceiteMotorista(e) {
    _enviarEscala.MotoristaEscala.val(e.Motorista);
    _enviarEscala.CodigoPedido.val(e.Codigo);
    executarReST("PlanejamentoPedidoTMS/BuscarEscala", { Codigo: e.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _enviarEscala.DataComparecerEscala.val(r.Data.DataComparecerEscala ? r.Data.DataComparecerEscala : Global.DataHoraAtual());
                _enviarEscala.AceiteMotorista.val(r.Data.AceiteMotorista ? r.Data.AceiteMotorista : "");
                $("#divModalEnviarEscala").modal("show");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
}

function loadEnviarEscala() {
    _enviarEscala = new EnviarEscala();
    KoBindings(_enviarEscala, "knockoutEnviarEscala");
}