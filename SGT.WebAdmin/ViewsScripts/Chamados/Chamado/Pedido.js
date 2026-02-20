/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="Adicional.js" />
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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ClienteOutroEndereco.js" />
/// <reference path="Importacao.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="PercursoMDFe.js" />
/// <reference path="../../Consultas/SerieTransportador.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="NotaParcial.js" />
/// <reference path="Resumo.js" />
/// <reference path="Etapa.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="Produto.js" />
/// <reference path="Integracao.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../Enumeradores/EnumFormaAverbacaoCTE.js" />
/// <reference path="../../Enumeradores/EnumTipoObrigacaoUsoTerminal.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumFormaPreenchimentoCentroResultadoPedido.js" />
/// <reference path="clientes.js" />
/// <reference path="Motorista.js" />
/// <reference path="Reboque.js" />
/// <reference path="CamposObrigatorios.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPedido = null, _pedido = null, _CRUDPedido = null, _pesquisaPedido = null, _gridMotoristas = null, _gridHistoricoPedido = null, _PermissoesPersonalizadas = null, _gridReboques = null;
var _buscarRotasFrete;

var PesquisaPedido = function () {
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Pedidos.Pedido.TipodeRemetente.getFieldDescription(), issue: 306, required: true, eventChange: TipoPessoaPesquisaChange, visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col-12 col-md-6") });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPedido.Aberto), options: EnumSituacaoPedido.obterOpcoesPesquisa(), def: EnumSituacaoPedido.Aberto, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataColeta = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataColeta.getFieldDescription(), getType: typesKnockout.date });
    this.NumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroPedidoEmbarcador.getFieldDescription(), issue: 902 });
    this.NumeroPedido = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroPedido.getFieldDescription(), configInt: { precision: 0, allowZero: true }, getType: typesKnockout.int, visible: ko.observable(false) });
    this.NotaFiscal = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroNF.getFieldDescription(), configInt: { precision: 0, allowZero: false }, getType: typesKnockout.int, visible: ko.observable(true), cssClass: ko.observable("") });

    this.PedidoEmpresaResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.EmpresaResponsavel.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PedidoCentroCusto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CentroCusto.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroBooking.getFieldDescription() });
    this.NumeroOS = PropertyEntity({ val: ko.observable(""), def: "", text: "Nº O.S.:" });
    this.NumeroEXP = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroEXP.getFieldDescription(), maxlength: 150, visible: ko.observable(false) });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Container.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });


    this.CodigoCargaEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroCarga.getFieldDescription() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Recebedor.getFieldDescription(), idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Expedidor.getFieldDescription(), idBtnSearch: guid() });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Tomador.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Veiculo.getFieldDescription(), issue: 143, idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), issue: 145, idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.CidadePoloOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.PoloOrigem.getFieldDescription(), issue: 831, idBtnSearch: guid() });
    this.CidadePoloDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.PoloDestino.getFieldDescription(), issue: 831, idBtnSearch: guid() });
    this.PaisOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.PaisOrigem.getFieldDescription(), idBtnSearch: guid() });
    this.PaisDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.PaisDestino.getFieldDescription(), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.Filial.getRequiredFieldDescription(), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.TipoCarga.getFieldDescription(), idBtnSearch: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.CanalEntrega.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.FuncionarioVendedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Vendedor.getFieldDescription(), idBtnSearch: guid() });
    this.ProcImportacao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.ProcImportação.getFieldDescription(), cssClass: ko.observable("") });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pedidos.Pedido.Transportador.getFieldDescription()), idBtnSearch: guid() });
    this.Safra = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.Safra.getFieldDescription() });
    this.PercentualAdiantamentoTerceiro = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.Adiantamento.getFieldDescription() });
    this.PercentualMinimoAdiantamentoTerceiro = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.MinimoAdiantamento.getFieldDescription() });
    this.PercentualMaximoAdiantamentoTerceiro = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.MáximoAdiantamento.getFieldDescription() });
    this.NumeroCotacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroCotacao.getFieldDescription(), configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.NumeroProtocoloIntegracaoPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "", text: Localization.Resources.Pedidos.Pedido.NumeroProtocoloIntegraçãoPedido.getFieldDescription(), configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 10, visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPedido.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Pedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TelaResumida = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.TelaPedidosResumido === true), def: _CONFIGURACAO_TMS.TelaPedidosResumido === true });
    this.CanceladoAposVinculoCarga = PropertyEntity({ getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroPedido.getFieldDescription(), maxlength: 50, visible: ko.observable(false), required: false, enable: ko.observable(false), val: ko.observable(""), def: ko.observable("") });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroPedidoEmbarcador.getFieldDescription(), maxlength: 50, visible: true, required: false, enable: ko.observable(true) });
    this.DataColeta = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataColeta, getType: typesKnockout.dateTime, required: false, issue: 2, visible: ko.observable(true) });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.DataCarregamento.getFieldDescription(), getType: typesKnockout.dateTime, enable: false, visible: ko.observable(false), eventClick: alterarDataCarregamentoMontagemCarga });
    this.EncaixarHorario = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Pedidos.Pedido.TipodeRemetente.getRequiredFieldDescription(), issue: 306, required: true, eventChange: TipoPessoaChange, visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), eventChange: remetenteBlur, required: true, text: Localization.Resources.Gerais.Geral.Remetente.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), issue: 52, cssClass: ko.observable("col-12 col-md-6") });
    this.Remetentes = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: Localization.Resources.Gerais.Geral.Remetente.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), issue: 52, cssClass: ko.observable("col-12 col-md-6") });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), eventChange: remetenteBlur, required: false, text: Localization.Resources.Gerais.Geral.Remetente.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), issue: 58, cssClass: ko.observable("col-12 col-md-6") });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Origem.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 16, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), eventChange: destinatarioBlur, required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Destinatario.getRequiredFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatarios = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Destinatarios.getRequiredFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.TipoCarga.getRequiredFieldDescription(), issue: 53, idBtnSearch: guid(), cssClass: ko.observable("col-12 col-md-3"), visible: ko.observable(true) });
    this.ClienteDeslocamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.ClienteDeslocamento.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.Veiculo.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 143, visible: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.ProdutoEmbarcador.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 59, visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), eventChange: tipoOperacaoBlur, required: false, text: Localization.Resources.Pedidos.Pedido.TipoOperacao.getRequiredFieldDescription(), issue: 121, idBtnSearch: guid(), enable: ko.observable(true), destinatarioObrigatorio: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador, permitirMultiplosDestinatariosPedido: false, permitirMultiplosRemetentesPedido: false });
    this.Rota = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.Rota.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pedidos.Pedido.ModeloVeicularSolicitadoPeloEmbarcador.getFieldDescription(), issue: 44, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModelosVeicularesCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.ModeloVeicularSolicitadoPeloEmbarcador.getRequiredFieldDescription()), issue: 44, idBtnSearch: guid(), visible: ko.observable(false) });

    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio === true, text: ko.observable((_CONFIGURACAO_TMS.CentroResultadoPedidoObrigatorio === true ? "*" : "") + Localization.Resources.Pedidos.Pedido.CentroResultados.getFieldDescription()), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), enable: ko.observable(_CONFIGURACAO_TMS.NaoPermitirAlterarCentroResultadoPedido === false) });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Filial.getFieldDescription()), issue: 70, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Empresa.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PesoTotalCarga = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.PesoPedido.getFieldDescription(), required: false, visible: ko.observable(false), issue: 24747 });
    this.PesoLiquidoTotal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.Pedidos.Pedido.PesoLiquido.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.CubagemTotal = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, text: Localization.Resources.Pedidos.Pedido.CubagemPedido.getFieldDescription(), issue: 805, required: false, visible: ko.observable(false) });
    this.PalletsFracionado = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, text: Localization.Resources.Pedidos.Pedido.NumeroPallets.getFieldDescription(), required: false, visible: ko.observable(true) });

    this.TempoDeDoca = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 0, allowZero: false, allowNegative: false }, text: Localization.Resources.Pedidos.Pedido.TempoDeDoca.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.NumeroDoca = PropertyEntity({ text: Localization.Resources.Pedidos.Pedido.NumeroDoca.getFieldDescription(), maxlength: 50, visible: ko.observable(false), required: false, val: ko.observable(""), def: ko.observable("") });


    this.ObservacaoAbaPedido = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), maxlength: 2000, visible: ko.observable(false) });
    this.ObservacaoEmissaRemetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), visible: ko.observable(false), val: ko.observable("") });
    this.ObservacaoEmissaoDestinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), visible: ko.observable(false), val: ko.observable("") });

    //Multimodal
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Recebedor.getFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(true), codigoIBGE: "", eventChange: recebedorBlur });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Expedidor.getFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(true), codigoIBGE: "", eventChange: expedidorBlur });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pedidos.Pedido.Terceiro.getFieldDescription()), issue: 52, idBtnSearch: guid(), visible: ko.observable(false) });
    //

    this.Motoristas = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pedidos.Pedido.InformarMotorista, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145 });
    this.Reboques = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pedidos.Pedido.InformarReboque, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 0 });

    this.Fronteira = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), visible: ko.observable(false), text: ko.observable(Localization.Resources.Pedidos.Pedido.Fronteira.getFieldDescription()), idBtnSearch: guid(), issue: 309 });

    this.ColetaEmProdutorRural = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.UsarOutroEnderecoOrigem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LocalidadeClienteOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.BairroOrigem = PropertyEntity();
    this.CEPOrigem = PropertyEntity();
    this.NumeroOrigem = PropertyEntity();
    this.ComplementoOrigem = PropertyEntity();
    this.EnderecoOrigem = PropertyEntity();
    this.Telefone1Origem = PropertyEntity();
    this.RGIE1Origem = PropertyEntity();
    this.CidadePoloOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.PaisOrigem = PropertyEntity();
    this.IBGEOrigem = PropertyEntity();
    this.UFOrigem = PropertyEntity();
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), idBtnSearch: guid(), required: false, issue: 16, visible: ko.observable(true) });
    this.UsarOutroEnderecoDestino = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.LocalidadeClienteDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.BairroDestino = PropertyEntity();
    this.CEPDestino = PropertyEntity();
    this.NumeroDestino = PropertyEntity();
    this.ComplementoDestino = PropertyEntity();
    this.EnderecoDestino = PropertyEntity();
    this.Telefone1Destino = PropertyEntity();
    this.RGIE1Destino = PropertyEntity();
    this.CidadePoloDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.PaisDestino = PropertyEntity();
    this.IBGEDestino = PropertyEntity();
    this.UFDestino = PropertyEntity();

    this.ListaMotoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: false });
    this.ListaReboques = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PassagemPercursoEstado = PropertyEntity({ type: types.listEntity, list: new Array(), options: _estados, val: ko.observable("AC"), def: "AC", codEntity: ko.observable("AC"), defCodEntity: "AC", text: Localization.Resources.Pedidos.Pedido.EstadoPassagem.getRequiredFieldDescription() });

    this.NumeroBL = PropertyEntity();
    this.NumeroNavio = PropertyEntity();
    this.Porto = PropertyEntity();
    this.TipoTerminalImportacao = PropertyEntity();
    this.EnderecoEntregaImportacao = PropertyEntity();
    this.BairroEntregaImportacao = PropertyEntity();
    this.CEPEntregaImportacao = PropertyEntity();
    this.LocalidadeEntregaImportacao = PropertyEntity();
    this.DataVencimentoArmazenamentoImportacao = PropertyEntity();
    this.ArmadorImportacao = PropertyEntity();
    this.NumeroDI = PropertyEntity();
    this.CodigoImportacao = PropertyEntity();
    this.CodigoReferencia = PropertyEntity();
    this.ValorCarga = PropertyEntity();
    this.Volume = PropertyEntity();
    this.Peso = PropertyEntity();
    this.CodigoPorto = PropertyEntity();
    this.CodigoTipoTerminalImportacao = PropertyEntity();
    this.CodigoLocalidadeEntregaImportacao = PropertyEntity();
    this.GridDI = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ProdutosEmbarcador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ONUsProdutosEmbarcador = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Navio = PropertyEntity();
    this.CodigoNavio = PropertyEntity();
    this.PortoDestino = PropertyEntity();
    this.CodigoPortoDestino = PropertyEntity();
    this.TerminalOrigem = PropertyEntity();
    this.CodigoTerminalOrigem = PropertyEntity();
    this.TerminalDestino = PropertyEntity();
    this.CodigoTerminalDestino = PropertyEntity();
    this.DirecaoViagemMultimodal = PropertyEntity();
    this.Container = PropertyEntity();
    this.CodigoContainer = PropertyEntity();
    this.LacreContainerUmMultimodal = PropertyEntity();
    this.LacreContainerDoisMultimodal = PropertyEntity();
    this.LacreContainerTresMultimodal = PropertyEntity();
    this.TaraContainerMultimodal = PropertyEntity();
    this.ContainerTipoReserva = PropertyEntity();
    this.CodigoContainerTipoReserva = PropertyEntity();
    this.GridTransbordo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridComponente = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridDestinatarioBloqueado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ListaProdutos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaONUs = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaClientes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: "", val: ko.observable("") });

    this.SituacaoPedido = PropertyEntity();
    this.DescricaoSituacaoPedido = PropertyEntity();
    this.DescricaoEtapaPedido = PropertyEntity();
    this.NomesMotoristas = PropertyEntity();

    this.ListaComponentesFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ComponentesFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.ListaAcrescimoDesconto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.AcrescimoDesconto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.CodigoMotoristaVeiculo = PropertyEntity();
    this.NomeMotoristaVeiculo = PropertyEntity();

    this.Stages = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });


    this.TipoOperacao.codEntity.subscribe(function (novoValor) {
        if (novoValor <= 0) {
            ExibirVeiculoEMotorista(true);
            SetarCamposObrigatoriosPedido();
        }
    });
};

var CRUDPedido = function () {
    this.ImportarDocas = PropertyEntity({ eventClick: ImportarDocasClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.ImportarDocas, visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.DescricaoPedido, visible: ko.observable(false), enable: ko.observable(true) });
    this.OrdemColeta = PropertyEntity({ eventClick: imprimirOrdemColetaClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.OrdemColeta, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarPedidoAposVinculoCarga = PropertyEntity({ eventClick: cancelarPedidoAposVinculoCargaClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.CancelarPedidoAposVinculoCarga, visible: ko.observable(false), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: ko.observable(true) });
    this.VisualizarHistorico = PropertyEntity({ eventClick: visualizarHistoricoPedidoClick, type: types.event, text: Localization.Resources.Pedidos.Pedido.VisualizarHistorico, visible: ko.observable(true), enable: ko.observable(true) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Pedido/Importar",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O005_Pedidos,
        CallbackImportacao: function () {
            _gridPedido.CarregarGrid();
        },
        CallbackPreProcessamento: callbackImportacaoAtrasadaPreProcessamento,
        ParametrosRequisicao: function () {
            return {
                MotivosImportacaoAtrasada: _parametrosMotivosImporacaoAtrasado
            }
        }
    });



    this.ImportarISISReturn = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pedidos.Pedido.ImportarISIS,
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Pedido/ImportarISISReturn",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacaoISISReturn",
        CodigoControleImportacao: EnumCodigoControleImportacao.O046_IsisReturn
    });

    this.ImportarOcorrenciasDePedidosPorPlanilhas = PropertyEntity({
        type: types.local,
        text: "Importar Ocorrências",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Pedido/ImportarOcorrenciaPedido",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacaoOcorrenciaPedido",
        CodigoControleImportacao: EnumCodigoControleImportacao.O053_ImportacaoPedidoOcorrencia,
        CallbackImportacao: function () {
            _gridPedido.CarregarGrid();
        },
        CallbackPreProcessamento: callbackImportacaoAtrasadaPreProcessamento,
        ParametrosRequisicao: function () {
            return {
                MotivosImportacaoAtrasada: _parametrosMotivosImporacaoAtrasado
            }
        }
    });

    this.ImportarAtualizacaoPedido = PropertyEntity({
        type: types.local,
        text: "Importar Atualização Pedido",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Pedido/ImportarAtualizacaoPedido",
        UrlConfiguracao: "Pedido/ConfiguracaoImportacaoAtualizacaoPedido",
        CodigoControleImportacao: EnumCodigoControleImportacao.O069_ImportarAtualizacaoPedido,
    });
};

//*******EVENTOS*******

function loadPedido() {
    _pedido = new Pedido();
    KoBindings(_pedido, "knockoutCadastroPedido");
    HeaderAuditoria("Pedido", _pedido, null, { Motoristas: "Motorista" });


    var telaPedidosResumido = _CONFIGURACAO_TMS.TelaPedidosResumido === true;
    var permiteAdicionarPessoa = true;
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteIncluirPessoa, _PermissoesPersonalizadas) || telaPedidosResumido)
        permiteAdicionarPessoa = false;

    //Verifica se for TSM, se o usuário tiver permissão para incluir veiculos próprios, terceiros ou ambos
    var tipoPropriedade = "";
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        //Caso seja TMS, vai definir valor padrão como N, desta forma, não trará nenhum veículo se o usuário não tiver nenhuma das permissões
        tipoPropriedade = "N";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio, _PermissoesPersonalizadas))
            tipoPropriedade = "P";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro, _PermissoesPersonalizadas))
            tipoPropriedade = tipoPropriedade == "N" ? "T" : "A";
    }

    new BuscarGruposPessoas(_pedido.GrupoPessoa, RetornoGrupoPessoa, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_pedido.Remetente, retornoRemetente, permiteAdicionarPessoa);
    new BuscarClientes(_pedido.Remetentes);
    new BuscarLocalidades(_pedido.Origem, null, null, RetornoSelecaoOrigem);
    new BuscarClientes(_pedido.Destinatario, retornoDestinatario, permiteAdicionarPessoa);
    new BuscarClientes(_pedido.Destinatarios);
    new BuscarClientes(_pedido.ClienteDeslocamento);
    new BuscarTiposdeCarga(_pedido.TipoCarga, null, _pedido.GrupoPessoa, null, _pedido.Remetente, null, null, null, null, _pedido.TipoOperacao);

    if (_CONFIGURACAO_TMS.PermitirSelecionarReboquePedido)
        new BuscarVeiculos(_pedido.Veiculo, RetornoVeiculoBuscarDetalhes, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, "0", null, null, true, null, tipoPropriedade);
    else
        new BuscarVeiculos(_pedido.Veiculo, RetornoVeiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, tipoPropriedade);

    if (_CONFIGURACAO_TMS.VisualizarTipoOperacaoDoPedidoPorTomador)
        new BuscarTiposOperacao(_pedido.TipoOperacao, RetornoTipoOperacao, _pedido.GrupoPessoa, null, null, null, null, null, null, RetornoTomadorPedido, null, null, _pedido.TipoCarga);
    else
        new BuscarTiposOperacao(_pedido.TipoOperacao, RetornoTipoOperacao, null, null, null, null, null, null, null, null, null, null, _pedido.TipoCarga);

    new BuscarProdutos(_pedido.ProdutoEmbarcador, RetornoProdutoEmbarcador, _pedido.GrupoPessoa, _pedido.Remetente);
    new BuscarModelosVeicularesCarga(_pedido.ModeloVeicularCarga, null, null);
    new BuscarModelosVeicularesCarga(_pedido.ModelosVeicularesCarga, null, null);
    new BuscarFilial(_pedido.Filial);
    new BuscarTransportadores(_pedido.Empresa, retornoEmpresa, null, true);
    new BuscarClientes(_pedido.Fronteira, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _buscarRotasFrete = new BuscarRotasFrete(_pedido.Rota, null, null, _pedido.GrupoPessoa, _pedido.Remetente, null, null, null, null, _pedido.Remetente, _pedido.Destinatario);
    else
        new BuscarRotasFrete(_pedido.Rota, null, null, _pedido.GrupoPessoa, _pedido.Remetente);

    new BuscarClientes(_pedido.Recebedor, retornoRecebedor, permiteAdicionarPessoa);
    new BuscarClientes(_pedido.Expedidor, retornoExpedidor, permiteAdicionarPessoa);
    new BuscarClientes(_pedido.Terceiro, null, permiteAdicionarPessoa);
    new BuscarCentroResultado(_pedido.CentroResultado, null, null, null, EnumAnaliticoSintetico.Analitico);

    _CRUDPedido = new CRUDPedido();
    KoBindings(_CRUDPedido, "knockoutCRUDPedido");

    _pesquisaPedido = new PesquisaPedido();
    KoBindings(_pesquisaPedido, "knockoutPesquisaPedido", false, _pesquisaPedido.Pesquisar.id);

    new BuscarClientes(_pesquisaPedido.Remetente);
    new BuscarGruposPessoas(_pesquisaPedido.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarLocalidades(_pesquisaPedido.Origem);
    new BuscarClientes(_pesquisaPedido.Destinatario);
    new BuscarClientes(_pesquisaPedido.Expedidor);
    new BuscarClientes(_pesquisaPedido.Recebedor);
    new BuscarClientes(_pesquisaPedido.Tomador);
    new BuscarVeiculos(_pesquisaPedido.Veiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, tipoPropriedade);
    new BuscarMotorista(_pesquisaPedido.Motorista);
    new BuscarLocalidades(_pesquisaPedido.Destino);
    new BuscarLocalidadesPolo(_pesquisaPedido.CidadePoloOrigem);
    new BuscarLocalidadesPolo(_pesquisaPedido.CidadePoloDestino);
    new BuscarFilial(_pesquisaPedido.Filial);
    new BuscarPaises(_pesquisaPedido.PaisOrigem);
    new BuscarPaises(_pesquisaPedido.PaisDestino);
    new BuscarTiposdeCarga(_pesquisaPedido.TipoCarga);
    new BuscarCanaisEntrega(_pesquisaPedido.CanalEntrega);
    new BuscarPedidoEmpresaResponsavel(_pesquisaPedido.PedidoEmpresaResponsavel);
    new BuscarPedidoCentroCusto(_pesquisaPedido.PedidoCentroCusto);
    new BuscarContainers(_pesquisaPedido.Container);
    new BuscarTiposOperacao(_pesquisaPedido.TipoOperacao);
    new BuscarFuncionario(_pesquisaPedido.FuncionarioVendedor);
    new BuscarEmpresa(_pesquisaPedido.Transportador);

    loadOrigemDestino();
    loadAdicional();
    loadPercusoMDFe();
    buscarPedidos();
    loadNotaParcial();
    loadCteParcial();
    loadImportacao();
    loadResumoPedido();
    limparResumoPedido();
    loadEtapaPedido();
    setarEtapaInicioPedido();
    loadPedidoAutorizacao();
    loadProduto();
    LoadPedidoCancelamento();
    LoadClientes();
    LoadComponenteFretePedido();
    loadAnexoAnalise();
    loadOcorrenciaPedido();
    LoadGridMotoristas();
    LoadGridReboques();
    loadIntegracaoPedido();
    loadAvaliacaoEntrega();
    loadAcrescimoDesconto();
    loadModalPedidoImportacaoAtraso();
    loadCotacaoPedido();
    LoadStages();
    LoadDocas(_UtilizaIntegracaoDeTemposDoca);
    configurarLayoutPedidoPorTipoSistema();

    if (_FormularioSomenteLeitura) {
        _CRUDPedido.Importar.visible(false);
        _CRUDPedido.Imprimir.enable(false);
        _CRUDPedido.Adicionar.enable(false);
        _CRUDPedido.Excluir.enable(false);
        _CRUDPedido.Cancelar.enable(false);
        _CRUDPedido.Atualizar.enable(false);
    }

    _pedido.ProdutoEmbarcador.visible(false);
    _pedido.ProdutoEmbarcador.required = false;

    $("#tabProdutos").show();
    $("#tabPercursoMDFe").hide();

    if (_CONFIGURACAO_TMS.UtilizarMultiplosModelosVeicularesPedido) {
        _pedido.ModeloVeicularCarga.visible(false);
        _pedido.ModelosVeicularesCarga.visible(true);
        //_pedido.ModelosVeicularesCarga.required(true);
    }
    else {
        _pedido.ModeloVeicularCarga.visible(true);
        _pedido.ModelosVeicularesCarga.visible(false);
        _pedido.ModelosVeicularesCarga.required(false);
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _pedido.Terceiro.visible(true);
        _pedido.Veiculo.visible(false);
        _pedido.ModeloVeicularCarga.visible(false);
        _pedido.TipoCarga.cssClass("col-12 col-md-3");
        $("#divMotoristas").hide();
        $("#tabImportacao").show();
    }
    else {
        $("#divPesquisaMultimodal").hide();
    }

    SetarCamposObrigatoriosPedido();
    SetarCamposTelaResumida();

    obterTiposIntegracao().then(function (data) {
        var visibilidadeImportarISISReturn = data.filter(function (tipo) {
            return tipo.value == EnumTipoIntegracao.Isis;
        }).length > 0;

        _CRUDPedido.ImportarISISReturn.visible(visibilidadeImportarISISReturn);
        _adicional.ISISReturn.visible(visibilidadeImportarISISReturn);
    });
}
function LoadDocas(UtilizaIntegracaoDeTemposDoca) {
    _pedido.NumeroDoca.visible(UtilizaIntegracaoDeTemposDoca);
    _pedido.TempoDeDoca.visible(UtilizaIntegracaoDeTemposDoca);
    _CRUDPedido.ImportarDocas.visible(UtilizaIntegracaoDeTemposDoca);
}


function configurarLayoutPedidoPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pedido.Empresa.visible(true);
        _pedido.Empresa.text(Localization.Resources.Pedidos.Pedido.Transportador.getFieldDescription());
        //_pedido.Filial.visible(true);
        _pedido.Filial.text(Localization.Resources.Pedidos.Pedido.Filial.getRequiredFieldDescription());
        _pedido.Filial.required = true;
        _pedido.PesoTotalCarga.visible(true);
        _pedido.CubagemTotal.visible(true);
        _pedido.PesoTotalCarga.required = true;
        _pedido.TipoPessoa.visible(false);
        _pedido.Origem.visible(false);
        _pesquisaPedido.TipoPessoa.visible(false);
        _pesquisaPedido.Origem.visible(false);
        //_pesquisaPedido.Filial.visible(true);
        _pesquisaPedido.NumeroPedido.visible(false);
        _pesquisaPedido.NumeroEXP.visible(true);
        _pesquisaPedido.Remetente.cssClass("col-12 col-md-6");
        _pesquisaPedido.Transportador.text(Localization.Resources.Pedidos.Pedido.Transportador.getFieldDescription());
        _pesquisaPedido.NotaFiscal.cssClass("col-12 col-md-2");
        _pesquisaPedido.ProcImportacao.cssClass("col-12 col-md-2");
        _pesquisaPedido.NumeroProtocoloIntegracaoPedido.visible(true);

        _pedido.ProdutoEmbarcador.visible(false);
        _pedido.ProdutoEmbarcador.required = false;

        _CRUDPedido.OrdemColeta.visible(false);

        controlarObrigatoriedadeDestinatario();

        $("#tabNotaParcial").show();
        $("#tabCteParcial").show();
        $("#tabOcorrenciasPedido").show();
        $("#tabCotacaoPedido").show(); 

        if (_CONFIGURACAO_TMS.ExibirAuditoriaPedidos)
            $("#tabHistoricoPedido").show();

        executarReST("Pedido/ObterConfiguracoesGeraisPedido", null, function (arg) {
            _CRUDPedido.ImportarOcorrenciasDePedidosPorPlanilhas.visible(arg.Data.ImportarOcorrenciasDePedidosPorPlanilhas);
            _CRUDPedido.ImportarAtualizacaoPedido.visible(arg.Data.AtualizarCamposPedidoPorPlanilha);
        });
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pedido.Filial.visible(_CONFIGURACAO_TMS.Carga.PadraoVisualizacaoOperadorLogistico || _CONFIGURACAO_TMS.GerarJanelaDeCarregamento);
        _pedido.Recebedor.visible(true);
        _pedido.Expedidor.visible(true);
        _pesquisaPedido.NumeroPedido.visible(true);
        _pedido.Empresa.visible(true);
        _pedido.Empresa.text(Localization.Resources.Pedidos.Pedido.EmpresaFilial.getFieldDescription());
        _pesquisaPedido.Remetente.cssClass("col-12 col-md-4");
        _pedido.CentroResultado.visible(true);
        _pesquisaPedido.Transportador.text(Localization.Resources.Pedidos.Pedido.EmpresaFilial.getFieldDescription());
        _pesquisaPedido.NotaFiscal.cssClass("col-12 col-md-3");
        _pesquisaPedido.ProcImportacao.cssClass("col-12 col-md-3");

        if (_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido) {
            _pedido.PalletsFracionado.required = true;
            _pedido.NumeroPedidoEmbarcador.required = true;
            _pedido.Empresa.required = true;
            _pedido.DataColeta.required = true;
            _pedido.Rota.required = true;
        }

        if (_CONFIGURACAO_TMS.PermitirInformarAcrescimoDescontoNoPedido)
            $("#tabAcrescimoDesconto").show();

        $("#tabComponenteFretePedido").show();
    }

    if (_CONFIGURACAO_TMS.NaoPermitirInformarExpedidorNoPedido)
        _pedido.Expedidor.visible(false);
    else
        _pedido.Expedidor.visible(true);
}

function obterTomador() {
    return _pedido.Remetente;
}

function TipoPessoaChange() {
    controlarExibicaoMultiplosRemetentes();
}

function TipoPessoaPesquisaChange() {
    if (_pesquisaPedido.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _pesquisaPedido.Remetente.visible(true);
        _pesquisaPedido.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaPedido.GrupoPessoa);
    }
    else if (_pesquisaPedido.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _pesquisaPedido.GrupoPessoa.visible(true);
        _pesquisaPedido.Remetente.visible(false);
        LimparCampoEntity(_pesquisaPedido.Remetente);
    }
}

function RetornoTomadorPedido() {
    if (_adicional.UsarTipoTomadorPedido.val()) {
        if (_adicional.TipoTomador.val() === EnumTipoTomador.Remetente)
            return _pedido.Remetente;
        else if (_adicional.TipoTomador.val() === EnumTipoTomador.Expedidor)
            return _pedido.Expedidor;
        else if (_adicional.TipoTomador.val() === EnumTipoTomador.Recebedor)
            return _pedido.Recebedor;
        else if (_adicional.TipoTomador.val() === EnumTipoTomador.Destinatario)
            return _pedido.Destinatario;
        else if (_adicional.TipoTomador.val() === EnumTipoTomador.Outros)
            return _adicional.Tomador;
    }
    else {
        if (_adicional.TipoPagamento.val() == EnumTipoPagamento.Pago)
            return _pedido.Remetente;
        else if (_adicional.TipoPagamento.val() == EnumTipoPagamento.A_Pagar)
            return _pedido.Destinatario;
        else if (_adicional.TipoPagamento.val() == EnumTipoPagamento.Outros)
            return _adicional.Tomador;
    }
}

function RetornoProdutoEmbarcador(data, alterarTemperatura) {
    if (alterarTemperatura == null)
        alterarTemperatura = true;

    if (data != null) {
        _pedido.ProdutoEmbarcador.val(data.Descricao);
        _pedido.ProdutoEmbarcador.codEntity(data.Codigo);

        if (alterarTemperatura)
            _adicional.Temperatura.val(data.Temperatura);
    }
}

function RetornoTipoOperacao(data) {
    _pedido.TipoOperacao.val(data.Descricao);
    _pedido.TipoOperacao.codEntity(data.Codigo);
    _pedido.TipoOperacao.destinatarioObrigatorio = !data.PermiteGerarPedidoSemDestinatario && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
    _pedido.TipoOperacao.permitirMultiplosDestinatariosPedido = data.PermitirMultiplosDestinatariosPedido;
    _pedido.TipoOperacao.permitirMultiplosRemetentesPedido = data.PermitirMultiplosRemetentesPedido;
    _pedido.TipoOperacao.apresentarSaldoProduto = data.ApresentarSaldoProduto;

    if (data.OperacaoDeImportacaoExportacao)
        $("#tabImportacao").show();
    else if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#tabImportacao").hide();
    else if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#tabImportacao").show();

    if (data.TipoServicoMultimodal == EnumTipoServicoMultimodal.VinculadoMultimodalTerceiro || data.TipoServicoMultimodal == EnumTipoServicoMultimodal.VinculadoMultimodalProprio)
        _adicional.PedidoSVM.val(true);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && data.TipoObrigacaoUsoTerminal != null && data.TipoObrigacaoUsoTerminal != undefined) {
        _importacao.TerminalOrigem.text(Localization.Resources.Pedidos.Pedido.TerminalOrigem.getFieldDescription());
        _importacao.TerminalOrigem.required = false;
        _importacao.TerminalDestino.text(Localization.Resources.Pedidos.Pedido.TerminalDestino.getFieldDescription());
        _importacao.TerminalDestino.required = false;

        if (data.TipoObrigacaoUsoTerminal === EnumTipoObrigacaoUsoTerminal.Origem) {
            _importacao.TerminalOrigem.required = true;
            _importacao.TerminalOrigem.text(Localization.Resources.Pedidos.Pedido.TerminalOrigem.getRequiredFieldDescription());
        }
        else if (data.TipoObrigacaoUsoTerminal === EnumTipoObrigacaoUsoTerminal.Destino) {
            _importacao.TerminalDestino.text(Localization.Resources.Pedidos.Pedido.TerminalDestino.getRequiredFieldDescription());
            _importacao.TerminalDestino.required = true;
        }
        else if (data.TipoObrigacaoUsoTerminal === EnumTipoObrigacaoUsoTerminal.OrigemDestino) {
            _importacao.TerminalOrigem.text(Localization.Resources.Pedidos.Pedido.TerminalOrigem.getRequiredFieldDescription());
            _importacao.TerminalDestino.text(Localization.Resources.Pedidos.Pedido.TerminalDestino.getRequiredFieldDescription());
            _importacao.TerminalOrigem.required = true;
            _importacao.TerminalDestino.required = true;
        }
    }

    if (data.TransbordoRodoviario != undefined && data.TransbordoRodoviario != null && data.TransbordoRodoviario === true)
        _adicional.PedidoTransbordo.val(true);

    controlarExibicaoMultiplosDestinatarios();
    controlarExibicaoMultiplosRemetentes();
    controlarExibicaoClienteDeslocamento(data.UtilizarDeslocamentoPedido);

    if (!_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido)
        SetarObrigatoriedadeCamposSecundariosPedido(data.CamposSecundariosObrigatoriosPedido);

    ExibirVeiculoEMotorista(!data.NaoExigeVeiculoParaEmissao);

    SetarCamposObrigatoriosPedido();

    obterDetalhesTipoOperacao();
}

function obterDetalhesTipoOperacao() {
    if (_pedido.TipoOperacao.codEntity() > 0 && _CONFIGURACAO_TMS.FormaPreenchimentoCentroResultadoPedido === EnumFormaPreenchimentoCentroResultadoPedido.TipoOperacao && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        executarReST("Pedido/ObterDetalhesTipoOperacaoPedido", { TipoOperacao: _pedido.TipoOperacao.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var centroResultado = arg.Data.CentroResultado;
                    if (centroResultado !== null) {
                        _pedido.CentroResultado.codEntity(centroResultado.Codigo);
                        _pedido.CentroResultado.val(centroResultado.Descricao);
                    }
                    var produtosEmbarcador = arg.Data.ProdutosEmbaracador;
                    if (produtosEmbarcador != null) {
                        _produtoEmbarcador.ProdutosEmbarcador.list = produtosEmbarcador.map(function (p) {
                            p.Codigo = guid();
                            return p;
                        });
                        _gridProduto.CarregarGrid(_produtoEmbarcador.ProdutosEmbarcador.list);
                    } else {
                        _produtoEmbarcador.ProdutosEmbarcador.list = new Array();
                        _gridProduto.CarregarGrid([]);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}


function ExibirVeiculoEMotorista(exibir) {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        return;

    if (exibir === true) {
        $("#divMotoristas").show();
        _pedido.Veiculo.visible(true);

        _pedido.TipoCarga.cssClass("col-12 col-md-3");

        if (_CONFIGURACAO_TMS.PermitirSelecionarReboquePedido === true)
            $("#divReboques").show();
        else
            $("#divReboques").hide();
    } else {
        $("#divMotoristas").hide();
        $("#divReboques").hide();

        _pedido.ListaMotoristas.val([]);
        RecarregarListaMotoristas();

        _pedido.ListaReboques.val([]);
        RecarregarListaReboques();

        _pedido.Veiculo.val("");
        _pedido.Veiculo.codEntity(0);
        _pedido.Veiculo.visible(false);

        _pedido.TipoCarga.cssClass("col-12 col-md-3");
    }
}

function SetarObrigatoriedadeCamposSecundariosPedido(obrigatorio) {
    _pedido.PalletsFracionado.required = obrigatorio;
    _pedido.NumeroPedidoEmbarcador.required = obrigatorio;
    _pedido.Empresa.required = obrigatorio;
    _pedido.DataColeta.required = obrigatorio;
    _pedido.Rota.required = obrigatorio;

    _adicional.ObservacaoInterna.required = obrigatorio;

    if (obrigatorio && _pedido.Codigo.val() === 0) {
        _adicional.TipoPagamento.val("");
        _adicional.Requisitante.val("");
        _adicional.Ajudante.val(obrigatorio);
        _adicional.PossuiDescarga.val(obrigatorio);
        _adicional.PossuiCarregamento.val(obrigatorio);
    }

    _adicional.Requisitante.required = obrigatorio;
    _adicional.TipoPagamento.required = obrigatorio;
    _adicional.PesoTotalCargaTMS.required = obrigatorio;
    _adicional.QtdEntregas.required = obrigatorio;
    _adicional.CubagemTotalTMS.required = obrigatorio;
    _adicional.QtVolumes.required = obrigatorio;
    _adicional.PedidoTipoPagamento.required = obrigatorio;

    _adicional.QuantidadeNotasFiscais.visible(obrigatorio);
    _adicional.QuantidadeNotasFiscais.required = obrigatorio;
}

function RetornoVeiculoBuscarDetalhes(data) {
    if (data != null) {
        if (_CONFIGURACAO_TMS.FormaPreenchimentoCentroResultadoPedido === EnumFormaPreenchimentoCentroResultadoPedido.Veiculo) {
            _pedido.CentroResultado.codEntity(data.CodigoCentroResultado);
            _pedido.CentroResultado.val(data.CentroResultado);
        }

        executarReST("Veiculo/ObterDetalhesPorVeiculo", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    var dados = r.Data;
                    _pedido.Veiculo.val(dados.Placa);
                    _pedido.Veiculo.codEntity(dados.CodigoVeiculo);

                    if (dados.ModeloVeicularCargaReboque && !(_CONFIGURACAO_TMS.NaoSelecionarModeloVeicularAutomaticamente)) {
                        _pedido.ModeloVeicularCarga.val(dados.ModeloVeicularCargaReboque.ModeloVeicularReboque.Descricao);
                        _pedido.ModeloVeicularCarga.codEntity(dados.ModeloVeicularCargaReboque.ModeloVeicularReboque.Codigo);
                    } else {
                        _pedido.ModeloVeicularCarga.val(dados.ModeloVeicularCargaTracao.Descricao);
                        _pedido.ModeloVeicularCarga.codEntity(dados.ModeloVeicularCargaTracao.Codigo);
                    }

                    if (dados.VeiculosVinculados != null) {
                        var reboques = dados.VeiculosVinculados.map(function (o) { return { Codigo: o.CodigoVeiculo, Placa: o.Placa, ModeloVeicular: o.ModeloVeicularCarga } });

                        _gridReboques.CarregarGrid(reboques);
                    }

                    if (dados.Motoristas.length > 0) {
                        var dataGrid = _gridMotoristas.BuscarRegistros();

                        $.each(dados.Motoristas, function (i, motorista) {
                            var motoristaInserir = { Codigo: motorista.CodigoMotorista, CPF: motorista.CPF, Nome: motorista.Nome, Veiculo: "", CodigoVeiculo: 0 };

                            for (var j = 0; j < dataGrid.length; j++) {
                                if (dataGrid[j].Codigo == motoristaInserir.Codigo)
                                    return;
                            }

                            if (i == 0) {
                                _pedido.CodigoMotoristaVeiculo.val(motoristaInserir.Codigo);
                                _pedido.NomeMotoristaVeiculo.val(motoristaInserir.Nome);
                            }

                            RetornoInserirMotorista([motoristaInserir]);
                        });
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    }
}

function RetornoVeiculo(data) {
    if (data != null) {
        if (_CONFIGURACAO_TMS.FormaPreenchimentoCentroResultadoPedido === EnumFormaPreenchimentoCentroResultadoPedido.Veiculo) {
            _pedido.CentroResultado.codEntity(data.CodigoCentroResultado);
            _pedido.CentroResultado.val(data.CentroResultado);
        }

        executarReST("Veiculo/ObterDetalhesVeiculo", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    var dados = r.Data;
                    _pedido.Veiculo.val(dados.Placa);
                    _pedido.Veiculo.codEntity(dados.Codigo);
                    console.log(_CONFIGURACAO_TMS);

                    if (!(_CONFIGURACAO_TMS.NaoSelecionarModeloVeicularAutomaticamente)) {
                        if (dados.ModeloVeicularCargaReboque) {
                            _pedido.ModeloVeicularCarga.val(dados.ModeloVeicularCargaReboque.ModeloVeicularReboque.Descricao);
                            _pedido.ModeloVeicularCarga.codEntity(dados.ModeloVeicularCargaReboque.ModeloVeicularReboque.Codigo);
                        } else {
                            _pedido.ModeloVeicularCarga.val(dados.ModeloVeicularCargaTracao.Descricao);
                            _pedido.ModeloVeicularCarga.codEntity(dados.ModeloVeicularCargaTracao.Codigo);
                        }
                    }


                    var motoristas = new Array();

                    if (dados.Motoristas.length > 0) {
                        $.each(dados.Motoristas, function (i, motorista) {
                            var obj = new Object();
                            obj.Codigo = motorista.CodigoMotorista;
                            obj.CPF = motorista.CPF;
                            obj.Nome = motorista.Nome;
                            Veiculo = "";
                            CodigoVeiculo = 0;
                            motoristas.push(obj);
                        });
                    }
                    else if (dados.MotoristasTracao.length > 0) {
                        $.each(dados.MotoristasTracao, function (i, motorista) {
                            var obj = new Object();
                            obj.Codigo = motorista.CodigoMotorista;
                            obj.CPF = motorista.CPF;
                            obj.Nome = motorista.Nome;
                            Veiculo = "";
                            CodigoVeiculo = 0;
                            motoristas.push(obj);
                        });
                    }

                    if (motoristas.length > 0) {
                        var dataGrid = _gridMotoristas.BuscarRegistros();

                        $.each(motoristas, function (i, motoristaInserir) {
                            for (var j = 0; j < dataGrid.length; j++) {
                                if (dataGrid[j].Codigo == motoristaInserir.Codigo)
                                    return;
                            }

                            if (i == 0) {
                                _pedido.CodigoMotoristaVeiculo.val(motoristaInserir.Codigo);
                                _pedido.NomeMotoristaVeiculo.val(motoristaInserir.Nome);
                            }

                            RetornoInserirMotorista([motoristaInserir]);
                        });
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });

    }
}

function remetenteBlur() {
    if (_pedido.Remetente.val() == "")
        limparLocalidadeOrigem();
}

function destinatarioBlur() {
    if (_pedido.Destinatario.val() == "")
        limparLocalidadeDestino();
}

function tipoOperacaoBlur() {
    if (_pedido.TipoOperacao.val() == "") {
        _pedido.TipoOperacao.destinatarioObrigatorio = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
        _pedido.TipoOperacao.permitirMultiplosDestinatariosPedido = false;
        _pedido.TipoOperacao.permitirMultiplosRemetentesPedido = false;
        _pedido.TipoOperacao.apresentarSaldoProduto = false;

        controlarExibicaoMultiplosDestinatarios();
        controlarExibicaoMultiplosRemetentes();
        controlarExibicaoClienteDeslocamento(false);
        controlarExibicaoSaldoProdutoPedido();
    }
}

function ValidarCotacaoTabelaFrete(cb) {
    Salvar(_pedido, "Pedido/ValidarCotacaoTabelaFrete", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.TabelaFreteCompativel)
                    cb();
                else {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.TabelaFrete, cb);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, exibirCamposObrigatorio);
}

function ValidarPedidoDuplicado(cb) {
    Salvar(_pedido, "Pedido/ValidaPedidoDuplicado", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (!arg.Data.PedidoDuplicado)
                    cb();
                else {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.PedidoDuplicado, cb);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null, exibirCamposObrigatorio);
}

function ValidaPedidoSemMotoristaVeiculo(cb) {
    var dados = {
        CodigoVeiculo: _pedido.Veiculo.codEntity(),
        ContemMotoristas: contemMotoristas()
    };
    executarReST("Pedido/ValidaPedidoSemMotoristaVeiculo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (!arg.Data.SolicitaConfirmacao)
                    cb();
                else {
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.ConfirmacaoCerteza.format(arg.Data.MensagemConfirmacao), cb);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function adicionarClick() {
    var _HandleAdicionar = function () {
        executarReST("Pedido/Adicionar", obterPedidoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {

                    var dadosRetorno = "";
                    if (!string.IsNullOrWhiteSpace(retorno.Data.Numero))
                        dadosRetorno = Localization.Resources.Pedidos.Pedido.DescricaoPedido + " " + retorno.Data.Numero;
                    if (!string.IsNullOrWhiteSpace(retorno.Data.NumeroCarga))
                        dadosRetorno += ", " + Localization.Resources.Gerais.Geral.Carga + " " + retorno.Data.NumeroCarga;

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.Pedido.SucessoCadastradoComSucesso.format(dadosRetorno));
                    enviarArquivosAnexados(retorno.Data.Codigo);
                    _gridPedido.CarregarGrid();
                    RecarregarGridComponenteFretePedido();

                    if (_adicional.Cotacao.val() && retorno.Data.Codigo > 0)
                        editarPedido(retorno.Data.Codigo);
                    else
                        limparCamposPedido();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    };

    var valorFreteNegociado = Globalize.parseFloat(_adicional.ValorFreteNegociado.val());

    if (isNaN(valorFreteNegociado))
        valorFreteNegociado = 0;

    if (_CONFIGURACAO_TMS.ValidarTabelaFreteNoPedido && valorFreteNegociado <= 0)
        ValidarCotacaoTabelaFrete(_HandleAdicionar);
    else
        _HandleAdicionar();
}

function atualizarClick() {
    executarReST("Pedido/Atualizar", obterPedidoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridPedido.CarregarGrid();
                RecarregarGridComponenteFretePedido();

                if (_adicional.Cotacao.val())
                    editarPedido(retorno.Data.Codigo);
                else
                    limparCamposPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function ImportarDocasClick() {
    var _HandleImportarDocas = function () {
        executarReST("Pedido/ImportarDocas", obterPedidoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _pedido.TempoDeDoca.val(retorno.Data.TempoDeDoca);
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.Pedido.ImportarDocasOK);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    };
    _HandleImportarDocas();
}



function salvarClick() {
    if (verificarOrigemDestino() && verificarDadosAdicionais() && SetarPassagens() && VerificarDadosImportacao()) {
        if (!ValidarCamposObrigatorios(_pedido)) {
            exibirCamposObrigatorio();
            return;
        } else if (_pedido.ListaMotoristas.required && !contemMotoristas()) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.InformeMotorista);
            return;
        }

        var _HandlePedidoDuplicado = function () {
            var _HandlePedidoSemMotoristaVeiculo = function () {
                if (_pedido.Codigo.val() > 0)
                    atualizarClick();
                else
                    adicionarClick();
            };

            if (_CONFIGURACAO_TMS.SolicitarConfirmacaoPedidoSemMotoristaVeiculo)
                ValidaPedidoSemMotoristaVeiculo(_HandlePedidoSemMotoristaVeiculo);
            else
                _HandlePedidoSemMotoristaVeiculo();
        };

        if (_CONFIGURACAO_TMS.SolicitarConfirmacaoPedidoDuplicado)
            ValidarPedidoDuplicado(_HandlePedidoDuplicado);
        else
            _HandlePedidoDuplicado();


    }
}

function cancelarPedidoAposVinculoCargaClick() {
    executarReST("Pedido/SalvarStatusCanceladoAposVinculoCarga", { Codigo: _pedido.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.Pedido.StatusAlteradoComSucesso);
                editarPedido({ Codigo: _pedido.Codigo.val() });
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function excluirClick(e, sender) {

    if (_CONFIGURACAO_TMS.NaoPermitirExclusaoPedido) {
        AbrirTelaCancelamentoPedido();
        return;
    }

    var callbackExclusao = function () {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
        _gridPedido.CarregarGrid();
        limparCamposPedido();
        finalizarControleManualRequisicao();
    };

    var _handleCancelamento = function () {
        executarReST("Pedido/CancelarPorCodigo", { Codigo: _pedido.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    callbackExclusao();
                }
                else {
                    finalizarControleManualRequisicao();
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                finalizarControleManualRequisicao();
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    };

    var _handleExclusao = function () {
        ExcluirPorCodigo(_pedido, "Pedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {

                    if (arg.Data.PossuiDependencias)
                        _handleCancelamento();
                    else
                        callbackExclusao();
                }
                else {
                    finalizarControleManualRequisicao();
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                finalizarControleManualRequisicao();
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    };

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaExcluirOPedido.format(_pedido.NumeroPedidoEmbarcador.val()), function () {

        iniciarControleManualRequisicao();
        _handleExclusao();
    });
}

function imprimirOrdemColetaClick(e, sender) {
    var data = { Codigo: _pedido.Codigo.val(), Carregamento: false, OrdemColeta: true };

    executarReST("Pedido/GerarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function imprimirClick(e, sender) {
    var data = { Codigo: _pedido.Codigo.val(), Carregamento: false };

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        executarDownload("Pedido/GerarRelatorio", data);
    } else {
        executarReST("Pedido/GerarRelatorio", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    BuscarProcessamentosPendentes();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AguardeQueSeuRelatorioEstaSendoGerado);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        });
    }
}

function cancelarClick(e) {
    limparCamposPedido();
}

function expedidorBlur() {
    if (_pedido.Expedidor.val() == "") {
        _pedido.Expedidor.codigoIBGE = "";
        verificarFronteira();
    }
}

function recebedorBlur() {
    if (_pedido.Recebedor.val() == "") {
        _pedido.Recebedor.codigoIBGE = "";
        verificarFronteira();
    }
}

//*******MÉTODOS*******

function controlarObrigatoriedadeDestinatario() {
    if (_pedido.TipoOperacao.destinatarioObrigatorio) {
        _pedido.Destinatario.required = _pedido.Destinatario.visible();
        _pedido.Destinatario.text(Localization.Resources.Pedidos.Pedido.Destinatario.getRequiredFieldDescription());
        _pedido.Destinatarios.required = _pedido.Destinatarios.visible();
        _pedido.Destinatarios.text(Localization.Resources.Pedidos.Pedido.Destinatario.getRequiredFieldDescription());
    }
    else {
        _pedido.Destinatario.required = false;
        _pedido.Destinatario.text(Localization.Resources.Pedidos.Pedido.Destinatario.getFieldDescription());
        _pedido.Destinatarios.required = false;
        _pedido.Destinatarios.text(Localization.Resources.Pedidos.Pedido.Destinatario.getFieldDescription());
    }
}

function controlarExibicaoClienteDeslocamento(exibir) {
    if (exibir) {
        _pedido.ClienteDeslocamento.visible(true);
        _pedido.ClienteDeslocamento.required = true;
    }
    else {
        _pedido.ClienteDeslocamento.visible(false);
        _pedido.ClienteDeslocamento.required = false;

        LimparCampo(_pedido.ClienteDeslocamento);
    }
}

function controlarExibicaoMultiplosDestinatarios() {
    var exibir = _pedido.TipoOperacao.permitirMultiplosDestinatariosPedido && (_pedido.Codigo.val() == 0);

    if (exibir) {
        _pedido.Destinatario.visible(false);
        _pedido.Destinatarios.visible(true);

        LimparCampo(_pedido.Destinatario);
        limparLocalidadeDestino();
    }
    else {
        _pedido.Destinatario.visible(true);
        _pedido.Destinatarios.visible(false);

        LimparCampo(_pedido.Destinatarios);
    }

    controlarObrigatoriedadeDestinatario();
}

function controlarExibicaoMultiplosRemetentes() {
    if (_pedido.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        _pedido.GrupoPessoa.required = false;
        _pedido.GrupoPessoa.visible(false);

        LimparCampoEntity(_pedido.GrupoPessoa);

        var exibir = _pedido.TipoOperacao.permitirMultiplosRemetentesPedido && (_pedido.Codigo.val() == 0);

        if (exibir) {
            _pedido.Remetente.visible(false);
            _pedido.Remetente.required = false;
            _pedido.Remetentes.visible(true);
            _pedido.Remetentes.required = true;

            LimparCampo(_pedido.Remetente);
            limparLocalidadeOrigem();
        }
        else {
            _pedido.Remetente.visible(true);
            _pedido.Remetente.required = true;
            _pedido.Remetentes.visible(false);
            _pedido.Remetentes.required = false;

            LimparCampo(_pedido.Remetentes);
        }
    }
    else if (_pedido.TipoPessoa.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _pedido.Remetente.required = false;
        _pedido.Remetente.visible(false);
        _pedido.Remetentes.required = false;
        _pedido.Remetentes.visible(false);

        LimparCampoEntity(_pedido.Remetente);
        LimparCampoMultiplesEntities(_pedido.Remetentes);
        limparLocalidadeOrigem();

        _pedido.GrupoPessoa.required = true;
        _pedido.GrupoPessoa.visible(true);
    }

    controlarExibicaoOrigem();
}

function controlarExibicaoOrigem() {
    if (_pedido.Remetentes.visible()) {
        _pedido.Origem.required = false;
        _pedido.Origem.visible(false);

        LimparCampo(_pedido.Origem);
    }
    else {
        _pedido.Origem.required = true;
        _pedido.Origem.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador);
    }
}

function RetornoGrupoPessoa(data) {

    if (data.Bloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.GrupoPessoasBloqueadas, Localization.Resources.Pedidos.Pedido.PessoasBloqueadas.format(data.MotivoBloqueio));

        _pedido.GrupoPessoa.codEntity(0);
        _pedido.GrupoPessoa.val('');

        return;
    }

    _pedido.GrupoPessoa.codEntity(data.Codigo);
    _pedido.GrupoPessoa.val(data.Descricao);

    if (data.ObservacaoEmissaoCarga !== null && data.ObservacaoEmissaoCarga !== undefined && data.ObservacaoEmissaoCarga !== "") {
        _pedido.ObservacaoEmissaRemetente.visible(true);
        _pedido.ObservacaoEmissaRemetente.val(data.ObservacaoEmissaoCarga);
    } else {
        _pedido.ObservacaoEmissaRemetente.visible(false);
        _pedido.ObservacaoEmissaRemetente.val("");
    }
}

function sugerirDataColeta() {

    if (_pedido.DataColeta.val() != "")
        return;

    executarReST("Pedido/ObterDataColeta", { Remetente: _pedido.Remetente.codEntity() }, function (retorno) {
        if ((retorno.Success) && (retorno.Data)) {
            if (retorno.Data.DataColeta)
                _pedido.DataColeta.val(retorno.Data.DataColeta);
        }
    });

}

function retornoRemetente(data) {

    if (data.GrupoPessoasBloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.GrupoPessoasBloqueadas, Localization.Resources.Pedidos.Pedido.PessoasBloqueadas.format(data.GrupoPessoasMotivoBloqueio));

        _pedido.Remetente.codEntity(0);
        _pedido.Remetente.val('');

        return;
    }

    if (data.Bloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.PessoaBloqueada, Localization.Resources.Pedidos.Pedido.ClienteBloqueado.format(data.MotivoBloqueio));

        _pedido.Remetente.codEntity(0);
        _pedido.Remetente.val('');

        return;
    }

    _pedido.Remetente.codEntity(data.Codigo);
    _pedido.Remetente.val(data.Descricao);

    if (_pedido.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
        preecherLocalidadeCliente(_pedido.Remetente, _localidadeOrigem, "R");
        $("#tabOrigem").show();
    }
    else
        $("#tabOrigem").hide();

    if (data.ObservacaoEmissaoCarga !== null && data.ObservacaoEmissaoCarga !== undefined && data.ObservacaoEmissaoCarga !== "") {
        _pedido.ObservacaoEmissaRemetente.visible(true);
        _pedido.ObservacaoEmissaRemetente.val(data.ObservacaoEmissaoCarga);
    }
    else {
        _pedido.ObservacaoEmissaRemetente.visible(false);
        _pedido.ObservacaoEmissaRemetente.val("");
    }

    if (_CONFIGURACAO_TMS.ExibirPedidoDeColeta) {
        if (!_adicional.DisponibilizarPedidoParaColeta.val())
            _adicional.DisponibilizarPedidoParaColeta.val(data.GerarPedidoColeta);

        if (_adicional.RecebedorColeta.codEntity() == 0) {
            _adicional.RecebedorColeta.codEntity(data.CodigoRecebedorColeta);
            _adicional.RecebedorColeta.entityDescription(data.DescricaoRecebedorColeta);
            _adicional.RecebedorColeta.val(data.DescricaoRecebedorColeta);
        }
    }

    //if (_adicional.Transportador.codEntity() == 0) {
    //    _adicional.Transportador.codEntity(data.CodigoTransportador);
    //    _adicional.Transportador.entityDescription(data.DescricaoTransportador);
    //    _adicional.Transportador.val(data.DescricaoTransportador);
    //}


    if (_adicional.DisponibilizarPedidoParaColeta.val())
        sugerirDataColeta();

    preencherRotaFreteAutomaticamente();
    obterCondicaoPedidoPorTomador();
}

function RetornoSelecaoOrigem(data) {
    if (data != null) {
        _pedido.Origem.val(data.Cidade);
        _pedido.Origem.codEntity(data.Codigo);

        _pedido.UFOrigem.val(data.Estado);
        _percursosEntreEstados.EstadoOrigem.val(data.Estado);
        mapaOrigemDestinoChange();
    }
}

function retornoDestinatario(data) {

    if (data.GrupoPessoasBloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.GrupoPessoasBloqueadas, Localization.Resources.Pedidos.Pedido.GrupoPessoasClienteBloqueadas.format(data.GrupoPessoasMotivoBloqueio));

        _pedido.Destinatario.codEntity(0);
        _pedido.Destinatario.val('');

        return;
    }

    if (data.Bloqueado) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pedidos.Pedido.PessoaBloqueada, Localization.Resources.Pedidos.Pedido.ClienteBloqueado.format(data.MotivoBloqueio));

        _pedido.Destinatario.codEntity(0);
        _pedido.Destinatario.val('');

        return;
    }

    _pedido.Destinatario.codEntity(data.Codigo);
    _pedido.Destinatario.val(data.Descricao);

    preecherLocalidadeCliente(_pedido.Destinatario, _localidadeDestino, "D");

    $("#tabDestino").show();

    if (data.ObservacaoEmissaoCarga !== null && data.ObservacaoEmissaoCarga !== undefined && data.ObservacaoEmissaoCarga !== "") {
        _pedido.ObservacaoEmissaoDestinatario.visible(true);
        _pedido.ObservacaoEmissaoDestinatario.val(data.ObservacaoEmissaoCarga);
    }
    else {
        _pedido.ObservacaoEmissaoDestinatario.visible(false);
        _pedido.ObservacaoEmissaoDestinatario.val("");
    }

    preencherRotaFreteAutomaticamente();
    obterCondicaoPedidoPorTomador();
}

function retornoExpedidor(registroSelecionado) {
    _pedido.Expedidor.codEntity(registroSelecionado.Codigo);
    _pedido.Expedidor.val(registroSelecionado.Descricao);
    _pedido.Expedidor.codigoIBGE = registroSelecionado.CodigoIBGE;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS &&
        _CONFIGURACAO_TMS.ConfiguracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido === true)
        preecherLocalidadeCliente(_pedido.Expedidor, _localidadeOrigem, "R");

    verificarFronteira();
    obterCondicaoPedidoPorTomador();
}

function retornoRecebedor(registroSelecionado) {
    _pedido.Recebedor.codEntity(registroSelecionado.Codigo);
    _pedido.Recebedor.val(registroSelecionado.Descricao);
    _pedido.Recebedor.codigoIBGE = registroSelecionado.CodigoIBGE;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS &&
        _CONFIGURACAO_TMS.ConfiguracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido === true)
        preecherLocalidadeCliente(_pedido.Recebedor, _localidadeDestino, "D");

    verificarFronteira();
    obterCondicaoPedidoPorTomador();
}

function retornoEmpresa(registroSelecionado) {
    _pedido.Empresa.codEntity(registroSelecionado.Codigo);
    _pedido.Empresa.val(registroSelecionado.Descricao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.GerarJanelaDeCarregamento) {
        var data = { CNPJ: registroSelecionado.CNPJ };

        executarReST("Filial/VerificarCNPJCadastrado", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _pedido.Filial.val(arg.Data.Descricao);
                    _pedido.Filial.codEntity(arg.Data.Codigo);
                }
            }
        });
    }
}

function preencherRotaFreteAutomaticamente() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && !_CONFIGURACAO_TMS.NaoPreencherRotaFreteAutomaticamente && _pedido.Remetente.codEntity() > 0 && _pedido.Destinatario.codEntity() > 0)
        _buscarRotasFrete.ExecuteSearch();
}

function verificarFronteira() {
    var origemExterior = _localidadeOrigem.CodigoIBGE.val() == "9999999";
    var destinoExterior = _localidadeDestino.CodigoIBGE.val() == "9999999";

    if ((origemExterior || destinoExterior) && !_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        var campoFronteiraObrigatorio = false

        if (origemExterior && (_pedido.Expedidor.codEntity() == 0 || _pedido.Expedidor.codigoIBGE == "9999999"))
            campoFronteiraObrigatorio = true;

        if (destinoExterior && (_pedido.Recebedor.codEntity() == 0 || _pedido.Recebedor.codigoIBGE == "9999999"))
            campoFronteiraObrigatorio = true;

        _pedido.Fronteira.required(campoFronteiraObrigatorio);
        _pedido.Fronteira.text(campoFronteiraObrigatorio ? Localization.Resources.Pedidos.Pedido.Fronteira.getRequiredFieldDescription() : Localization.Resources.Pedidos.Pedido.Fronteira.getRequiredFieldDescription());
        _pedido.Fronteira.visible(true);
    }
    else {
        _pedido.Fronteira.visible(false);
        _pedido.Fronteira.required(false);
    }
}

function buscarPedidos() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (pedidoGrid) { editarPedido(pedidoGrid, false); }, tamanho: "10", icone: "" };
    var duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), evento: "onclick", metodo: function (pedidoGrid) { editarPedido(pedidoGrid, true); }, tamanho: "10", icone: "" };

    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_BloquearDuplicarPedido, _PermissoesPersonalizadas) || _CONFIGURACAO_TMS.BloquearDuplicarPedido)
        var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar], tamanho: 10 };
    else
        var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar], tamanho: 10 };

    _gridPedido = new GridView(_pesquisaPedido.Pesquisar.idGrid, "Pedido/Pesquisa", _pesquisaPedido, menuOpcoes, null, null, null);
    _gridPedido.CarregarGrid();
}

function editarPedido(pedidoGrid, duplicar) {
    limparCamposPedido();


    executarReST("Pedido/BuscarPorCodigo", { Codigo: pedidoGrid.Codigo, Duplicar: duplicar }, function (arg) {

        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

        if (!arg.Data)
            return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);

        let dataPrincipal = arg.Data;

        executarReST("Pedido/BuscarPorCodigoParteComplementar1", { Codigo: pedidoGrid.Codigo, Duplicar: duplicar }, (argComplementar) => {
            if (!argComplementar.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);

            if (!argComplementar.Data)
                return exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);

            dataPrincipal = { ...dataPrincipal, ...argComplementar.Data }

            PreencherObjetoKnout(_pedido, { Data: dataPrincipal });

            _integracaoPedido.Codigo.val(_pedido.Codigo.val());
            _gridArquivosPedido.CarregarGrid();
            CarregarIntegracaoPedido();

            _pesquisaPedido.ExibirFiltros.visibleFade(false);

            _CRUDPedido.Atualizar.visible(!duplicar);
            _CRUDPedido.Imprimir.visible(!duplicar);
            _CRUDPedido.Cancelar.visible(!duplicar);
            _CRUDPedido.Excluir.visible(!duplicar);
            _CRUDPedido.Adicionar.visible(duplicar);
            _CRUDPedido.CancelarPedidoAposVinculoCarga.visible(dataPrincipal.PermiteCancelarAposVinculoCarga);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _CRUDPedido.OrdemColeta.visible(!duplicar);

                if (!_CONFIGURACAO_TMS.Carga.PadraoVisualizacaoOperadorLogistico && duplicar) {
                    _pedido.Filial.val("");
                    _pedido.Filial.codEntity(0);
                }
            }

            if (_CONFIGURACAO_TMS.TelaPedidosResumido)
                _pedido.NumeroPedidoEmbarcador.val(_pedido.NumeroPedidoEmbarcador.def);

            _pedido.TipoOperacao.apresentarSaldoProduto = dataPrincipal.TipoOperacao.ApresentarSaldoProduto;

            controlarExibicaoClienteDeslocamento(dataPrincipal.TipoOperacao.UtilizarDeslocamentoPedido);

            if (_pedido.ObservacaoEmissaRemetente.val() !== "" && _pedido.ObservacaoEmissaRemetente.val() !== null && _pedido.ObservacaoEmissaRemetente.val() !== undefined)
                _pedido.ObservacaoEmissaRemetente.visible(true);
            else
                _pedido.ObservacaoEmissaRemetente.visible(false);

            if (_pedido.ObservacaoEmissaoDestinatario.val() !== "" && _pedido.ObservacaoEmissaoDestinatario.val() !== null && _pedido.ObservacaoEmissaoDestinatario.val() !== undefined)
                _pedido.ObservacaoEmissaoDestinatario.visible(true);
            else
                _pedido.ObservacaoEmissaoDestinatario.visible(false);

            if (_pedido.ColetaEmProdutorRural.val())
                _pedido.TipoOperacao.enable(false);

            _pedido.TipoOperacao.destinatarioObrigatorio = dataPrincipal.TipoOperacao.DestinatarioObrigatorio && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
            _pedido.Expedidor.codigoIBGE = dataPrincipal.Expedidor.codigoIBGE;
            _pedido.Recebedor.codigoIBGE = dataPrincipal.Recebedor.codigoIBGE;

            preencherProduto(dataPrincipal.DadosProduto);
            loadGridProdutos();

            controlarObrigatoriedadeDestinatario();

            EditarClientes(dataPrincipal);

            if (_pedido.TipoPessoa.val() === EnumTipoPessoaGrupo.Pessoa) {
                $("#tabOrigem").show();

                _localidadeOrigem.Cliente.visible(true);
                _localidadeOrigem.Cliente.codEntity(_pedido.Remetente.codEntity());
                _localidadeOrigem.Cliente.val(_pedido.Remetente.val());
                _localidadeOrigem.CodigoIBGE.val(_pedido.IBGEOrigem.val());
                _localidadeOrigem.UF.val(_pedido.UFOrigem.val());
                _percursosEntreEstados.EstadoOrigem.val(_pedido.UFOrigem.val());
                mapaOrigemDestinoChange();

                if (_pedido.UsarOutroEnderecoOrigem.val()) {
                    _localidadeOrigem.MudarEndereco.val(true);
                    preecherLocalidadeCliente(_pedido.Remetente, _localidadeOrigem, "R");

                    if (_pedido.LocalidadeClienteOrigem.codEntity() == 0) {
                        _localidadeOrigem.Localidade.val(_pedido.Origem.val());
                        _localidadeOrigem.Localidade.codEntity(_pedido.Origem.codEntity());
                        _localidadeOrigem.LocalidadePolo.val(_pedido.CidadePoloOrigem.val());
                        _localidadeOrigem.Bairro.val(_pedido.BairroOrigem.val());
                        _localidadeOrigem.CEP.val(_pedido.CEPOrigem.val());
                        _localidadeOrigem.Numero.val(_pedido.NumeroOrigem.val());
                        _localidadeOrigem.Complemento.val(_pedido.ComplementoOrigem.val());
                        _localidadeOrigem.Endereco.val(_pedido.EnderecoOrigem.val());
                        _localidadeOrigem.Telefone1.val(_pedido.Telefone1Origem.val());
                        _localidadeOrigem.RGIE.val(_pedido.RGIE1Origem.val());
                        _localidadeOrigem.ClienteOutroEndereco.fadeVisible(true);
                    }
                    else {
                        _localidadeOrigem.ClienteOutroEndereco.codEntity(_pedido.LocalidadeClienteOrigem.codEntity());
                        _localidadeOrigem.ClienteOutroEndereco.val(_pedido.LocalidadeClienteOrigem.val());
                        _localidadeOrigem.ClienteOutroEndereco.fadeVisible(false);
                    }
                }
                else
                    preecherDescricaoEnderecoOrigemUtilizado();
            }

            if (_pedido.Destinatario.codEntity() != "" && _pedido.Destinatario.codEntity() > 0) {
                $("#tabDestino").show();
                _localidadeDestino.Cliente.visible(true);
                _localidadeDestino.Cliente.codEntity(_pedido.Destinatario.codEntity());
                _localidadeDestino.Cliente.val(_pedido.Destinatario.val());
                _localidadeDestino.CodigoIBGE.val(_pedido.IBGEDestino.val());
                _localidadeDestino.UF.val(_pedido.UFDestino.val());
                _percursosEntreEstados.EstadoDestino.val(_pedido.UFDestino.val());
                mapaOrigemDestinoChange();

                if (_pedido.UsarOutroEnderecoDestino.val()) {
                    _localidadeDestino.MudarEndereco.val(true);
                    preecherLocalidadeCliente(_pedido.Destinatario, _localidadeDestino, "D");

                    if (_pedido.LocalidadeClienteDestino.codEntity() == 0) {
                        _localidadeDestino.Localidade.val(_pedido.Destino.val());
                        _localidadeDestino.Localidade.codEntity(_pedido.Destino.codEntity());
                        _localidadeDestino.LocalidadePolo.val(_pedido.CidadePoloDestino.val());
                        _localidadeDestino.Bairro.val(_pedido.BairroDestino.val());
                        _localidadeDestino.CEP.val(_pedido.CEPDestino.val());
                        _localidadeDestino.Numero.val(_pedido.NumeroDestino.val());
                        _localidadeDestino.Complemento.val(_pedido.ComplementoDestino.val());
                        _localidadeDestino.Endereco.val(_pedido.EnderecoDestino.val());
                        _localidadeDestino.Telefone1.val(_pedido.Telefone1Destino.val());
                        _localidadeDestino.RGIE.val(_pedido.RGIE1Destino.val());
                        _localidadeDestino.ClienteOutroEndereco.fadeVisible(true);
                    }
                    else {
                        _localidadeDestino.ClienteOutroEndereco.fadeVisible(false);
                        _localidadeDestino.ClienteOutroEndereco.codEntity(_pedido.LocalidadeClienteDestino.codEntity());
                        _localidadeDestino.ClienteOutroEndereco.val(_pedido.LocalidadeClienteDestino.val());

                        if (dataPrincipal.OutroEnderecoDestino != null) {
                            var outroEndereco = dataPrincipal.OutroEnderecoDestino;
                            _localidadeDestino.DetalheEnderecoOutroDestino.val(outroEndereco.Endereco);
                            _localidadeDestino.DetalheLocalidadeOutroDestino.val(outroEndereco.Localidade);
                            _localidadeDestino.DetalheCidadePoloOutroDestino.val(outroEndereco.CidadePolo);
                            _localidadeDestino.DetalheTelefoneOutroDestino.val(outroEndereco.Telefone);

                            if (outroEndereco.Telefone)
                                _localidadeDestino.DetalheTelefoneOutroDestino.visible(true);

                            if (outroEndereco.CidadePolo)
                                _localidadeDestino.DetalheCidadePoloOutroDestino.visible(true);
                        }
                    }
                }
                else
                    preecherDescricaoEnderecoDestinoUtilizado();
            }

            verificarFronteira();
            preencherDadosAdicionais(dataPrincipal.DadosAdicionais);
            preencherDadosAvaliacao(dataPrincipal.AvaliacaoEntrega);
            carregarGridOcorrenciaPedido(dataPrincipal.Ocorrencias);
            preencherNotaParcial(dataPrincipal.NotasParciais);
            preencherNotasFiscais(dataPrincipal.NotasFiscais);
            preencherCteParcial(dataPrincipal.CtesParciais);
            RecarregarListaMotoristas();
            RecarregarListaReboques();
            RecarregarGridComponenteFretePedido();
            TipoPessoaChange(_pedido);
            RetornoProdutoEmbarcador(dataPrincipal.ProdutoEmbarcador, false);
            carregarGridCotacaoPedido(dataPrincipal.Cotacoes);
            CarregarGridStage(dataPrincipal.Stages);

            _importacao.NumeroContainer.val(_adicional.NumeroContainer.val());
            _importacao.NumeroBL.val(_pedido.NumeroBL.val());
            _importacao.NumeroNavio.val(_pedido.NumeroNavio.val());

            _importacao.Porto.codEntity(_pedido.CodigoPorto.val());
            _importacao.Porto.val(_pedido.Porto.val());

            _importacao.TipoTerminalImportacao.codEntity(_pedido.CodigoTipoTerminalImportacao.val());
            _importacao.TipoTerminalImportacao.val(_pedido.TipoTerminalImportacao.val());

            _importacao.EnderecoEntregaImportacao.val(_pedido.EnderecoEntregaImportacao.val());
            _importacao.BairroEntregaImportacao.val(_pedido.BairroEntregaImportacao.val());
            _importacao.CEPEntregaImportacao.val(_pedido.CEPEntregaImportacao.val());

            _importacao.LocalidadeEntregaImportacao.codEntity(_pedido.CodigoLocalidadeEntregaImportacao.val());
            _importacao.LocalidadeEntregaImportacao.val(_pedido.LocalidadeEntregaImportacao.val());

            _importacao.DataVencimentoArmazenamentoImportacao.val(_pedido.DataVencimentoArmazenamentoImportacao.val());
            _importacao.ArmadorImportacao.val(_pedido.ArmadorImportacao.val());

            _importacao.Navio.val(_pedido.Navio.val());
            _importacao.Navio.codEntity(_pedido.CodigoNavio.val());

            _importacao.PortoDestino.val(_pedido.PortoDestino.val());
            _importacao.PortoDestino.codEntity(_pedido.CodigoPortoDestino.val());

            _importacao.TerminalOrigem.val(_pedido.TerminalOrigem.val());
            _importacao.TerminalOrigem.codEntity(_pedido.CodigoTerminalOrigem.val());

            _importacao.TerminalDestino.val(_pedido.TerminalDestino.val());
            _importacao.TerminalDestino.codEntity(_pedido.CodigoTerminalDestino.val());

            _importacao.DirecaoViagemMultimodal.val(_pedido.DirecaoViagemMultimodal.val());

            _importacao.Container.val(_pedido.Container.val());
            _importacao.Container.codEntity(_pedido.CodigoContainer.val());

            _importacao.LacreContainerUmMultimodal.val(_pedido.LacreContainerUmMultimodal.val());
            _importacao.LacreContainerDoisMultimodal.val(_pedido.LacreContainerDoisMultimodal.val());
            _importacao.LacreContainerTresMultimodal.val(_pedido.LacreContainerTresMultimodal.val());
            _importacao.TaraContainerMultimodal.val(_pedido.TaraContainerMultimodal.val());

            _importacao.ContainerTipoReserva.val(_pedido.ContainerTipoReserva.val());
            _importacao.ContainerTipoReserva.codEntity(_pedido.CodigoContainerTipoReserva.val());

            _importacao.GridDI.list = new Array();
            _importacao.GridDI.list = _pedido.GridDI.val();

            recarregarGridDI();

            _importacao.GridTransbordo.list = new Array();
            _importacao.GridTransbordo.list = _pedido.GridTransbordo.val();

            recarregarGridAcrescimoDesconto();

            _importacao.GridDestinatarioBloqueado.list = new Array();
            _importacao.GridDestinatarioBloqueado.list = _pedido.GridDestinatarioBloqueado.val();

            recarregarGridTransbordo();
            recarregarGridDestinatarioBloqueado();

            _produtoEmbarcador.ProdutosEmbarcador.list = new Array();
            _produtoEmbarcador.ProdutosEmbarcador.list = _pedido.ProdutosEmbarcador.val();

            recarregarGridProduto();

            _produtoEmbarcador.ONUsProdutosEmbarcador.list = new Array();
            _produtoEmbarcador.ONUsProdutosEmbarcador.list = _pedido.ONUsProdutosEmbarcador.val();

            _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
            _percursosEntreEstados.PassagemPercursoEstado.list = _pedido.PassagemPercursoEstado.list;

            if (_percursosEntreEstados.EstadoOrigem.val() != "" && _percursosEntreEstados.EstadoDestino.val() != "") {
                setarOrigemDestinoMapa();
                $("#tabPercursoMDFe").show();
            }
            else
                $("#tabPercursoMDFe").hide();

            if (!duplicar) {
                preecherResumoPedido(dataPrincipal.Resumo);
                setarEtapaInicioPedido();
                setarEtapasPedido();
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {

                if (dataPrincipal.OperacaoDeImportacaoExportacao)
                    $("#tabImportacao").show();
                else if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
                    $("#tabImportacao").hide();

                if (!duplicar) {
                    _pedido.NumeroPedido.visible(true);
                    _pedido.Remetente.cssClass("col-12 col-md-4");
                    _pedido.Remetentes.cssClass("col-12 col-md-4");
                    _pedido.GrupoPessoa.cssClass("col-12 col-md-4");

                }
            }

            $("#tabProdutos").show();

            if (!duplicar)
                preecherPedidoAutorizacao(dataPrincipal.DadosAutorizacao);

            if (!_CONFIGURACAO_TMS.CamposSecundariosObrigatoriosPedido)
                SetarObrigatoriedadeCamposSecundariosPedido(dataPrincipal.CamposSecundariosObrigatoriosPedido);

            ExibirVeiculoEMotorista(!dataPrincipal.NaoExigeVeiculoParaEmissao);
            _anexo.Anexos.val(dataPrincipal.Anexos);
            _anexoTipoOperacao.AnexosTipoOperacao.val(dataPrincipal.AnexosTipoOperacao);

            SetarCamposObrigatoriosPedido();
            SetarCamposTelaResumida();

            if (duplicar)
                limparCamposPedidoDuplicar();

            if (dataPrincipal.LinkRastreio != "" && dataPrincipal.LinkRastreio != null) {

                _ocorrenciasPedido.LinkRastreio.val(dataPrincipal.LinkRastreio);
                _ocorrenciasPedido.LinkRastreio.visible(true);

                _adicional.LinkRastreio.visible(true);
            }

        })

    });
}

function limparCamposPedido() {
    _pedido.TipoOperacao.enable(true);

    _CRUDPedido.Atualizar.visible(false);
    _CRUDPedido.Imprimir.visible(false);
    _CRUDPedido.OrdemColeta.visible(false);
    _CRUDPedido.CancelarPedidoAposVinculoCarga.visible(false);
    _CRUDPedido.Cancelar.visible(false);
    _CRUDPedido.Excluir.visible(false);
    _CRUDPedido.Adicionar.visible(true);
    _pedido.Fronteira.visible(false);
    _pedido.Fronteira.required(false);
    $("#tabOrigem").hide();
    $("#tabDestino").hide();
    $("#tabPercursoMDFe").hide();
    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
        $("#tabImportacao").hide();
    LimparCampos(_pedido);
    LimparCampos(_localidadeDestino);
    _localidadeDestino.DetalheTelefoneOutroDestino.visible(false);
    _localidadeDestino.DetalheCidadePoloOutroDestino.visible(false);
    LimparCampos(_localidadeOrigem);
    _localidadeDestino.ClienteOutroEndereco.fadeVisible(true);
    _localidadeOrigem.ClienteOutroEndereco.fadeVisible(true);
    _pedido.TipoOperacao.destinatarioObrigatorio = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
    _pedido.TipoOperacao.permitirMultiplosDestinatariosPedido = false;
    _pedido.TipoOperacao.permitirMultiplosRemetentesPedido = false;
    _pedido.Expedidor.codigoIBGE = "";
    _pedido.Recebedor.codigoIBGE = "";
    _pedido.TipoPessoa.val(EnumTipoPessoaGrupo.Pessoa);
    _pedido.NumeroPedido.visible(false);
    _pedido.Remetente.cssClass("col-12 col-md-6");
    _pedido.Remetentes.cssClass("col-12 col-md-6");
    _pedido.GrupoPessoa.cssClass("col-12 col-md-6");
    _pedido.ObservacaoEmissaRemetente.visible(false);
    _pedido.ObservacaoEmissaoDestinatario.visible(false);

    controlarExibicaoMultiplosDestinatarios();
    controlarExibicaoMultiplosRemetentes();
    controlarExibicaoClienteDeslocamento(false);

    _percursosEntreEstados.PassagemPercursoEstado.list = new Array();
    _percursosEntreEstados.EstadoOrigem.val("");
    _percursosEntreEstados.EstadoDestino.val("");

    limparCamposDadosAdicionais();
    limparCamposDadosAvaliacao();
    limparCamposNotaParcial();
    limparCamposCteParcial();
    limparDI();
    limparResumoPedido();
    limparCamposAnexo();
    limparCamposAcrescimoDesconto();
    setarEtapaInicioPedido();

    RecarregarListaMotoristas();
    RecarregarListaReboques();
    recarregarGridDI();
    recarregarGridTransbordo();
    //recarregarGridComponente();
    recarregarGridDestinatarioBloqueado();
    limparProduto();
    recarregarGridProduto();
    recarregarGridONUs();
    resetarTabs();
    LimparGridComponenteFretePedido()
    LimparCamposComponenteFretePedido();
    RecarregarGridComponenteFretePedido();
    recarregarGridAcrescimoDesconto();

    SetarCamposObrigatoriosPedido();
    SetarCamposTelaResumida();
}

function limparCamposPedidoDuplicar() {
    if (!_CONFIGURACAO_TMS.NaoApagarCamposDatasAoDuplicarPedido) {
        _pedido.DataColeta.val("");
        limparCamposDadosAdicionaisDuplicar();
    }

    if (_CONFIGURACAO_TMS.ApagarCampoRotaAoDuplicarPedido)
        _pedido.Rota.val("");
}

function limparLocalidadeOrigem() {
    LimparCampos(_localidadeOrigem);
    $("#tabOrigem").hide();
    _localidadeOrigem.Cliente.visible(false);
}

function limparLocalidadeDestino() {
    LimparCampos(_localidadeDestino);
    $("#tabDestino").hide();
    _localidadeDestino.Cliente.visible(false);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatório, Localization.Resources.Pedidos.Pedido.InformeCampoObrigatorio);
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function preencherListaProdutos() {
    _pedido.ListaProdutos.list = new Array();
    var produtos = new Array();

    $.each(_produtoEmbarcador.ProdutosEmbarcador.list, function (i, produto) {
        produtos.push({ Produto: produto });
    });
    _pedido.ListaProdutos.val(JSON.stringify(produtos));

    _pedido.ListaONUs.list = new Array();
    var onus = new Array();

    $.each(_produtoEmbarcador.ONUsProdutosEmbarcador.list, function (i, onu) {
        onus.push({ ONU: onu });
    });
    _pedido.ListaONUs.val(JSON.stringify(onus));
}

function preencherListaClientes() {
    _pedido.ListaClientes.list = new Array();
    var clientes = new Array();
    if (!string.IsNullOrWhiteSpace(_clientes.Clientes.val())) {
        $.each(_clientes.Clientes.val(), function (i, cliente) {
            clientes.push({ cliente: cliente });
        });
    }
    _pedido.ListaClientes.val(JSON.stringify(clientes));
}

function preencherListaComponentesFrete() {
    _pedido.ListaComponentesFrete.val(JSON.stringify(_pedido.ComponentesFrete.val()));
}

function preencherListaAcrescimoDesconto() {
    _pedido.ListaAcrescimoDesconto.val(JSON.stringify(_pedido.AcrescimoDesconto.val()));
}

function obterPedidoSalvar() {
    preencherListaMotorista();
    PreencherListaReboques();
    preencherListaProdutos();
    preencherListaClientes();
    preencherListaComponentesFrete();
    preencherListaAcrescimoDesconto();

    var pedido = RetornarObjetoPesquisa(_pedido);

    preencherDadosAdicionaisSalvar(pedido);
    preencherNotaParcialSalvar(pedido);
    preencherCteParcialSalvar(pedido);

    return pedido;
}

function ExibirModalHistoricoPedido() {
    Global.abrirModal('divModalHistoricoPedido');
    $("#divModalHistoricoPedido").one('hidden.bs.modal', function () {

    });
}

function loadGridHistoricoPedido(pedido) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 10;

    _gridHistoricoPedido = new GridView("grid-historico-pedido", "Pedido/ObterHistoricoPedido?pedido=" + pedido, null, null, null, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, undefined, undefined, undefined, undefined, null);
    _gridHistoricoPedido.CarregarGrid();
}

function visualizarHistoricoPedidoClick() {
    ExibirModalHistoricoPedido();
    loadGridHistoricoPedido(_pedido.Codigo.val());
}

