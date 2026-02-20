/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Consultas/TiposCausadoresOcorrencia.js" />
/// <reference path="../../Consultas/CausasMotivoChamado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamado.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Enumeradores/EnumResponsavelChamado.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="Chamado.js" />
/// <reference path="ChamadoData.js" />
/// <reference path="ChamadoPerfilAcesso.js" />
/// <reference path="Anexos.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoNFe.js" />
/// <reference path="../../Enumeradores/EnumTratativaDevolucao.js" />
/// <reference path="../../Enumeradores/EnumTipoColetaEntregaDevolucao.js" />
/// <reference path="NFeDevolucaoAbertura.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _abertura;
var _crudAbertura;
var _buscaCliente;
var _buscaTomador;
var _buscaDestinatario;
var _buscaMotorista;
var preenchendoAbertura = false;
var _controleEntregaDevolucaoChamadoEtapa1;
var _configuracaoTratativaDevolucao;
var _koNfdAbertura;
var _koTabs;
var _configuracaoChamado = [];

var Abertura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CargaEntrega = PropertyEntity({ codEntity: ko.observable(0), type: types.entity });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Carga.getFieldDescription(), issue: 195, enable: ko.observable(true), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Pedido.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), val: ko.observable(""), required: ko.observable(false) });
    this.PedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PedidoEmbarcador, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), val: ko.observable(""), required: ko.observable(false) });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.MotivoChamado.getFieldDescription(), issue: 926, enable: ko.observable(true), idBtnSearch: guid(), eventChange: motivoChamadoChange, required: ko.observable(true) });
    this.ValorDaCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.ValorDaCarga.getFieldDescription(), enable: ko.observable(false), required: ko.observable(false), visible: ko.observable(false) });
    this.PesoDaCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.PesoDaCarga.getFieldDescription(), enable: ko.observable(false), required: ko.observable(false), visible: ko.observable(false) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Cliente.getFieldDescription(), enable: ko.observable(false), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Tomador.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.TiposCausadoresOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false) });
    this.CausasMotivoChamado = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausasMotivoAtendimento.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false) });
    this.NotaFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: ko.observable("Nota Fiscal"), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Destinatario.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Valor.getFieldDescription()), required: ko.observable(false), visible: ko.observable(false) });
    this.ValorReferencia = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ValorReferencia.getFieldDescription()), required: ko.observable(false), visible: ko.observable(false) });
    this.ValorDesconto = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.ValorDesconto.getFieldDescription(), required: ko.observable(false), visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid(), required: ko.observable(false) });
    this.ValorCarga = PropertyEntity({ getType: typesKnockout.decimal, issue: 79660, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ValorCarga.getFieldDescription()), required: ko.observable(false), visible: ko.observable(false) });
    this.ValorDescarga = PropertyEntity({ getType: typesKnockout.decimal, issue: 79660, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ValorDescarga.getFieldDescription()), required: ko.observable(false), visible: ko.observable(false) });

    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosChamadosClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Anexos, visible: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Observacao.getFieldDescription(), issue: 593, enable: ko.observable(true), maxlength: 4000 });
    this.ResponsavelChamado = PropertyEntity({ val: ko.observable(EnumResponsavelChamado.Backhall), options: EnumResponsavelChamado.obterOpcoesPesquisa(), enable: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.ResponsavelChamado.getFieldDescription(), def: EnumResponsavelChamado.Backhall, visible: ko.observable(false) });
    this.NumeroPallet = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.NumeroPallet.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.QuantidadeItens = PropertyEntity({ getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), text: Localization.Resources.Cargas.ControleEntrega.QuantidadeItens.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.TipoCliente = PropertyEntity({ val: ko.observable(EnumTipoTomador.Destinatario), options: EnumTipoTomador.obterOpcoesTipoCliente(), def: EnumTipoTomador.Destinatario, text: Localization.Resources.Cargas.ControleEntrega.TipoCliente.getRequiredFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.RetencaoBau = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.RetencaoBau, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.DataRetencaoInicio = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.DataRetencaoInicio.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataRetencaoFim = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.DataRetencaoFim.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataReentrega = PropertyEntity({ type: types.map, getType: typesKnockout.dateTime, val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.DataReentrega.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.Motorista.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.PagoPeloMotorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PagoPeloMotorista, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataChegada.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataSaida.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.SaldoDescontadoMotorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SaldoDescontar.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 10, enable: ko.observable(false), required: ko.observable(false), visible: ko.observable(false) });
    this.TotalPagamentoMotorista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TotalPagamento.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 10, enable: ko.observable(false), required: ko.observable(false), visible: ko.observable(false) });

    this.ClienteResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.ClienteResponsavel.getRequiredFieldDescription(), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.GrupoPessoasResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.GrupoPessoasResponsavel.getRequiredFieldDescription(), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });
    this.TipoPessoaResponsavel = PropertyEntity({ val: ko.observable(""), options: EnumTipoPessoaGrupo.obterOpcoes(), def: "", text: Localization.Resources.Cargas.ControleEntrega.TipoPessoaResponsavel.getFieldDescription(), required: false, enable: ko.observable(true), visible: ko.observable(false), eventChange: tipoPessoaResponsavelChange });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Modelo Veicular da Entrega:"), required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.NomeMotoristaCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroMotoristaCarga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeMotivoChamado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Quantidade.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), val: ko.observable("") });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(false), def: false });
    this.MotivoDaDevolucao = PropertyEntity({ val: ko.observable(false), def: false, required: ko.observable(false), visible: ko.observable(false) });

    this.TipoDevolucao.val.subscribe(function (novoValor) {
        _abertura.MotivoDaDevolucao.required(false);
        _abertura.MotivoDaDevolucao.visible(false);

        if (novoValor == EnumTipoColetaEntregaDevolucao.Total) {
            _abertura.MotivoDaDevolucao.visible(true);
            _abertura.MotivoDaDevolucao.required(true);
        }
    });
    this.MotivoDaDevolucao.required.subscribe(function (required) {
        _analise.MotivoDaDevolucao.text(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getFieldDescription());
        if (required)
            _analise.MotivoDaDevolucao.text(Localization.Resources.Chamado.ChamadoOcorrencia.MotidoDaDevolucao.getRequiredFieldDescription());
    });

    this.RealMotivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.RealMotivo.getRequiredFieldDescription(), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), idBtnSearch: guid() });

    this.ClienteNovaEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Chamado.ChamadoOcorrencia.Cliente.getRequiredFieldDescription(), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(false), idBtnSearch: guid() });

    this.Carga.codEntity.subscribe(function (novoCodigo) {
        if (novoCodigo == 0) {
            _abertura.Cliente.enable(false);
            LimparCampo(_abertura.Cliente);

            _abertura.Tomador.enable(false);
            LimparCampo(_abertura.Tomador);

            _abertura.Destinatario.enable(false);
            LimparCampo(_abertura.Destinatario);

            _abertura.Motorista.enable(false);
            LimparCampo(_abertura.Motorista);

            TrocaComponentesModalBuscas(_motivoChamadoConfiguracao.PermiteAtendimentoSemCarga);

            if (_abertura.Codigo.val() == 0)
                _crudAbertura.Abrir.visible(true);

            _controleEntregaDevolucaoChamadoEtapa1.limpar();
            $('#controle-entrega-devolucao-chamado-container-etapa-1').hide();
        }
    });

    this.TipoCliente.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoTomador.Remetente) {
            _abertura.Destinatario.visible(false);
            _abertura.Destinatario.required(false);
            LimparCampo(_abertura.Destinatario);
        }
        else {
            _abertura.Destinatario.visible(true);
            _abertura.Destinatario.required(false);
        }
    });

    this.PagoPeloMotorista.val.subscribe(function (novoValor) {
        if (!_motivoChamadoConfiguracao.PermiteAdicionarValorComoAdiantamentoMotorista) {
            _abertura.SaldoDescontadoMotorista.visible(novoValor);
            _abertura.TotalPagamentoMotorista.visible(novoValor);
            BuscarSaldoDescontadoMotorista();
        }
    });

    this.Motorista.codEntity.subscribe(function () {
        BuscarSaldoDescontadoMotorista();
    });

    this.Cliente.val.subscribe(function (valor) {
        if (valor) {
            $('#liTabNotaFiscal').show();
            _notaFiscal.NFe.enable(true);
        }
    });


};

var CRUDAbertura = function () {
    this.Abrir = PropertyEntity({ eventClick: AbrirChamadoOcorrenciaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Abrir, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarChamadoOcorrenciaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Atualizar, visible: ko.observable(false) });
    this.PerfilAcesso = PropertyEntity({ eventClick: abrirModalPerfilAcessoChamadoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.PerfilAcesso, visible: ko.observable(false) });
};

var NfdDevolucaoAbertura = function () {
    //Aba NF-e de Devolução
    this.NFeDevolucaoAbertura = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridNFeDevolucaoAbertura = PropertyEntity({ type: types.local, id: guid() });

    this.ChaveNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Chave.getFieldDescription(), maxlength: 44, required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Numero.getRequiredFieldDescription(), getType: typesKnockout.int, required: ko.observable(true), enable: ko.observable(true) });
    this.SerieNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Serie.getRequiredFieldDescription(), getType: typesKnockout.string, required: ko.observable(true), enable: ko.observable(true) });
    this.DataEmissaoNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataEmissao.getRequiredFieldDescription(), getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true) });
    this.ValorTotalProdutosNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorProduto.getFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(false), enable: ko.observable(true) });
    this.ValorTotalNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorTotalNF.getFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(false), enable: ko.observable(true) });
    this.PesoDevolvidoNFeDevolucaoAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PesoDevolvido.getRequiredFieldDescription(), getType: typesKnockout.decimal, required: ko.observable(true), enable: ko.observable(true) });

    this.NumeroNFeOrigemAbertura = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroNFeOrigem.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarNFeDevolucaoAbertura = PropertyEntity({ eventClick: adicionarNFeDevolucaoAberturaClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarNFe, enable: ko.observable(true), required: ko.observable(true) });
    this.ImportarXmlNotaFiscalDevolucaoAbertura = PropertyEntity({ eventClick: notaFiscalAberturaClick, type: types.event, text: Localization.Resources.Chamado.ChamadoOcorrencia.AdicionarXML, type: types.abrirModal, required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
}

var Tabs = function () {
    this.ExibirNFeDevolucao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_motivoChamadoConfiguracao.PermiteInformarNFD), def: _motivoChamadoConfiguracao.PermiteInformarNFD });

}

//*******EVENTOS*******

function loadAbertura(telaChamado) {
    carregarConfiguracaoChamado(function () {
        _abertura = new Abertura();
        KoBindings(_abertura, "knockoutAbertura");

        if (telaChamado)
            HeaderAuditoria("Chamado", _abertura);

        _crudAbertura = new CRUDAbertura();
        KoBindings(_crudAbertura, "knockoutCRUDAbertura");

        _koNfdAbertura = new NfdDevolucaoAbertura();
        KoBindings(_koNfdAbertura, "knockoutNfdAbertura");

        _koTabs = new Tabs();
        KoBindings(_koTabs, "knockoutTabsAbertura");

        loadAnexosChamados();
        loadOcorrenciaChamado();
        loadChamadoData();
        loadChamadoPerfilAcesso();
        loadChamadoNotaFiscal();

        _controleEntregaDevolucaoChamadoEtapa1 = new ControleEntregaDevolucaoContainer("controle-entrega-devolucao-chamado-container-etapa-1");

        let situacoesFiltro;
        if (_CONFIGURACAO_TMS.FiltrarCargasSemDocumentosParaChamados)
            situacoesFiltro = [EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.PendeciaDocumentos, EnumSituacoesCarga.AgIntegracao,
            EnumSituacoesCarga.AgNFe, EnumSituacoesCarga.AgTransportador, EnumSituacoesCarga.CalculoFrete, EnumSituacoesCarga.Nova];
        else
            situacoesFiltro = [EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.PendeciaDocumentos, EnumSituacoesCarga.AgIntegracao];

        BuscarCargas(_abertura.Carga, RetornoConsultaCarga, null, situacoesFiltro, null, null, null, null, null, !_CONFIGURACAO_TMS.GerarOcorrenciaParaCargaAgrupada);
        BuscarXMLNotaFiscal(_abertura.NotaFiscal, null, _abertura.Carga);
        BuscarMotivoChamado(_abertura.MotivoChamado, retornoMotivoChamado, null, _abertura.Carga);
        BuscarGruposPessoas(_abertura.GrupoPessoasResponsavel);
        BuscarTipoOcorrencia(_abertura.RealMotivo, null, null, null, null, null, null, null, true);
        BuscarModelosVeicularesCarga(_abertura.ModeloVeicularCarga);
        BuscarXMLNotaFiscal(_koNfdAbertura.NumeroNFeOrigemAbertura, null, _abertura?.Carga);
        BuscarTiposCausadoresOcorrencia(_abertura.TiposCausadoresOcorrencia);
        BuscarCausasMotivoChamado(_abertura.CausasMotivoChamado, _abertura.MotivoChamado);

        _buscaCliente = new BuscarClientesCarga(_abertura.Cliente, retornoBuscaCliente, _abertura.Carga, !_CONFIGURACAO_TMS.GerarOcorrenciaParaCargaAgrupada);
        _buscaTomador = new BuscarTomadoresCarga(_abertura.Tomador, null, _abertura.Carga);
        _buscaDestinatario = new BuscarDestinatariosCarga(_abertura.Destinatario, retornoBuscaDestinatario, _abertura.Carga);
        _buscaMotorista = new BuscarMotoristasCarga(_abertura.Motorista, null, _abertura.Carga);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
            _abertura.Valor.visible(true);
            _crudAbertura.PerfilAcesso.visible(true);
            BuscarClientes(_abertura.ClienteResponsavel);
        }
        else if (_CONFIGURACAO_TMS.ExigirClienteResponsavelPeloAtendimento) {
            _abertura.ClienteResponsavel.required(true);
            _abertura.ClienteResponsavel.visible(true);
            _abertura.ClienteResponsavel.cssClass("col-12 col-md-6");
            BuscarRemetentesDestinatariosCarga(_abertura.ClienteResponsavel, null, _abertura.Carga, !_CONFIGURACAO_TMS.GerarOcorrenciaParaCargaAgrupada);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
            _abertura.Pedido.visible(true);
            _abertura.Carga.visible(false);
            _abertura.Carga.required(false);
            _abertura.Pedido.required(true);
            BuscarPedidos(_abertura.Pedido, RetornoConsultaPedido);
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {

            if (_configuracaoChamado.PermitirGerarAtendimentoPorPedido) {
                _abertura.PedidoEmbarcador.visible(true);
                _abertura.PedidoEmbarcador.required(false);

                _abertura.Carga.visible(true);
                _abertura.Carga.required(false);

                BuscarPedidosEmbarcador(_abertura.PedidoEmbarcador, RetornoConsultaPedido, null, _abertura.Carga);
            }
        }

        $("#" + _koNfdAbertura.ChaveNFeDevolucaoAbertura.id).mask("00000000000000000000000000000000000000000000", { selectOnFocus: true, clearIfNotMatch: true });

        loadNFeDevolucaoAbertura();
    });
}

function RetornoConsultaCarga(data) {
    _abertura.Carga.val(data.CodigoCargaEmbarcador);
    _abertura.Carga.codEntity(data.Codigo);
    _abertura.Cliente.enable(true);
    _abertura.Tomador.enable(true);
    _abertura.Destinatario.enable(true);
    _abertura.Motorista.enable(true);

    TrocaComponentesModalBuscas(false);

    _buscaCliente.CarregarClienteCargaSelecionada();
    _buscaTomador.CarregarTomadorCargaSelecionada();
    _buscaDestinatario.CarregarDestinatarioCargaSelecionada();

    if (_motivoChamadoConfiguracao.PermiteSelecionarMotorista)
        _buscaMotorista.CarregarMotoristaCargaSelecionada();

    if (data.NaoPermitirGerarAtendimento)
        _crudAbertura.Abrir.visible(false);
    else if (_abertura.Codigo.val() == 0)
        _crudAbertura.Abrir.visible(true);

    obterDetalhesCarga();
}

function RetornoConsultaPedido(data) {
    if (data.NaoPermitirGerarAtendimento)
        _crudAbertura.Abrir.visible(false);
    else if (_abertura.Codigo.val() == 0)
        _crudAbertura.Abrir.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) 
        obterDetalhesPedidoEmbarcador();
    else
        obterDetalhesPedido();
       
}

function motivoChamadoChange() {
    if (_abertura.MotivoChamado.val() == "") {
        _abertura.MotivoChamado.codEntity(0);
        DefinirEtapa2();
        DefinirEtapa3();
    }
}



function PreecherAberturaDevolucao(codigoChamado, codigoCargaEntrega) {
    executarReST("ChamadoAnalise/BuscarPorAnaliseDevolucaoCodigo", { Codigo: codigoChamado }, function (arg) {
        if (arg.Data != null) {
            _koNfdAbertura.NFeDevolucaoAbertura.list = recursiveObjetoRetorno(arg.Data.NFeDevolucaoAnalise);
            recarregarGridNFeDevolucaoAbertura();
            ControleCamposNFeDevolucaoAbertura();
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function retornoBuscaCliente(data) {
    _abertura.Cliente.codEntity(data.Codigo);
    _abertura.Cliente.val(data.Descricao);

    preencherDadosBancariosClienteObservacao(data, false);
}

function retornoBuscaDestinatario(data) {
    _abertura.Destinatario.codEntity(data.Codigo);
    _abertura.Destinatario.val(data.Descricao);

    preencherDadosBancariosClienteObservacao(data, true);
}

function preencherDadosBancariosClienteObservacao(data, destinatario) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS || data.CodigoBanco == 0)
        return;

    if (destinatario && !_motivoChamadoConfiguracao.BuscaContaBancariaDestinatario)
        return;
    else if (!destinatario && _motivoChamadoConfiguracao.BuscaContaBancariaDestinatario)
        return;

    var observacaoAtual = _abertura.Observacao.val();
    var observacao = data.Banco + " - " + data.Localidade + "\nCNPJ/CPF: " + data.CPF_CNPJ + "\nAG: " + data.Agencia + "\nCC: " + data.NumeroConta;

    if (string.IsNullOrWhiteSpace(observacaoAtual))
        _abertura.Observacao.val(observacao);
    else
        _abertura.Observacao.val(observacaoAtual + "\n" + observacao);
}

function obterDetalhesCarga() {
    if (_abertura.Carga.codEntity() > 0) {
        executarReST("ChamadoOcorrencia/ObterDetalhesCargaChamado", { Carga: _abertura.Carga.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var data = arg.Data;
                    _abertura.ValorReferencia.val(data.ValorDescarga);
                    _abertura.PesoDaCarga.val(data.PesoDaCarga);
                    _abertura.ValorDaCarga.val(data.ValorDaCarga);
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
    else
        _abertura.ValorReferencia.val("");
}

function obterDetalhesPedido() {
    if (_abertura.Pedido.codEntity() > 0) {
        executarReST("ChamadoOcorrencia/ObterDetalhesPedidoChamado", { Pedido: _abertura.Pedido.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    let data = arg.Data;
                    PreencherObjetoKnout(_abertura, { Data: data });

                    if (data.Carga.Codigo > 0) {
                        obterDetalhesCarga();
                    }
                    if (!_motivoChamadoConfiguracao.PermiteSelecionarMotorista) {
                        LimparCampo(_abertura.Motorista);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function obterDetalhesPedidoEmbarcador() {

    if (_abertura.PedidoEmbarcador.codEntity() > 0) {
        executarReST("ChamadoOcorrencia/ObterDetalhesPedidoChamado", { Pedido: _abertura.PedidoEmbarcador.codEntity() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    let data = arg.Data;
                    PreencherObjetoKnout(_abertura, { Data: data });
                    if (data.Carga.Codigo > 0) {
                        obterDetalhesCarga();
                    }
                    if (!_motivoChamadoConfiguracao.PermiteSelecionarMotorista) {
                        LimparCampo(_abertura.Motorista);
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }

}

function retornoMotivoChamado(dataRow) {
    ObterConfiguracaoMotivoChamado(dataRow.Codigo, function () {
        _abertura.MotivoChamado.codEntity(dataRow.Codigo);
        _abertura.MotivoChamado.val(dataRow.DescricaoComTipo);
        _koTabs.ExibirNFeDevolucao.val(_motivoChamadoConfiguracao.PermiteInformarNFD);

        if (dataRow.ExigeValor) {
            _abertura.Valor.visible(true);
            _abertura.Valor.required(true);
            _abertura.Valor.text(Localization.Resources.Cargas.ControleEntrega.Valor.getRequiredFieldDescription());
        }
        else if (dataRow.ExigeValorNaLiberacao && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
            _abertura.Valor.visible(true);
            _abertura.Valor.required(false);
            _abertura.Valor.text(Localization.Resources.Cargas.ControleEntrega.Valor.getFieldDescription());
        }
        else {
            _abertura.Valor.visible(false);
            _abertura.Valor.required(false);
            _abertura.Valor.text(Localization.Resources.Cargas.ControleEntrega.Valor.getFieldDescription());
        }

        if (_motivoChamadoConfiguracao.ApresentarValorPesoDaCarga) {
            _abertura.PesoDaCarga.visible(true);
            _abertura.ValorDaCarga.visible(true);
        }
        _abertura.TipoCliente.visible(false);
        _abertura.DataReentrega.visible(false);
        _abertura.DataReentrega.required(false);
        _abertura.RetencaoBau.visible(false);
        _abertura.DataRetencaoInicio.visible(false);
        _abertura.DataRetencaoInicio.required(false);
        _abertura.DataRetencaoFim.visible(false);
        _abertura.DataRetencaoFim.required(false);
        _abertura.Cliente.visible(false);
        _abertura.Tomador.visible(false);
        _abertura.Destinatario.visible(false);
        _abertura.Destinatario.required(false);

        if (_motivoChamadoConfiguracao.MotivoDevolucao) {
            _abertura.Cliente.visible(true);
            _abertura.Cliente.enable(true);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _abertura.Tomador.enable(true);
                _abertura.Tomador.visible(true);
                _abertura.Destinatario.visible(true);
                _abertura.Destinatario.enable(true);
            }
        }
        else if (_motivoChamadoConfiguracao.MotivoReentrega) {
            _abertura.TipoCliente.visible(false);
            _abertura.TipoCliente.val(EnumTipoTomador.Destinatario);
            _abertura.Destinatario.enable(true);
            _abertura.Destinatario.visible(true);
            _abertura.Destinatario.required(true);
            _abertura.DataReentrega.visible(true);
            _abertura.DataReentrega.required(true);
        }
        else if (_motivoChamadoConfiguracao.MotivoRetencao) {
            _abertura.TipoCliente.visible(true);
            _abertura.TipoCliente.val(EnumTipoTomador.Destinatario);
            _abertura.Destinatario.enable(true);
            _abertura.Destinatario.visible(true);
            _abertura.Destinatario.required(true);
            _abertura.RetencaoBau.visible(true);
            _abertura.DataRetencaoInicio.visible(true);
            _abertura.DataRetencaoInicio.required(true);
            _abertura.DataRetencaoFim.visible(true);
            _abertura.DataRetencaoFim.required(true);
        }
        else if (_motivoChamadoConfiguracao.MotivoReentregaMesmaCarga) {
            _abertura.TipoCliente.visible(false);
            _abertura.TipoCliente.val(EnumTipoTomador.Destinatario);
            if (_abertura.Destinatario.val() == "")
                _abertura.Destinatario.enable(true);
            else
                _abertura.Destinatario.enable(false)
            _abertura.Destinatario.visible(true);
            _abertura.Destinatario.required(true);
        }
        else {
            _abertura.Cliente.enable(true);
            _abertura.Cliente.visible(true);
            _abertura.Tomador.enable(true);
            _abertura.Tomador.visible(true);
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _abertura.Destinatario.visible(true);
                _abertura.Destinatario.enable(true);
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Fornecedor)
                LimparCampo(_abertura.Destinatario);
        }

        if (_gridNotaFiscal.BuscarRegistros().length > 1) {
            _abertura.Destinatario.visible(false);
            _abertura.Destinatario.required(false);
            _abertura.Cliente.visible(true);
        }

        _abertura.ValorReferencia.visible(_motivoChamadoConfiguracao.ReferentePagamentoDescarga);
        _abertura.ValorDesconto.visible(_motivoChamadoConfiguracao.PermiteInformarDesconto);
        _abertura.PagoPeloMotorista.visible(_motivoChamadoConfiguracao.PermiteAdicionarValorComoDespesaMotorista);
        _abertura.SaldoDescontadoMotorista.visible(VisibleSaldoDescontadoMotorista());
        _abertura.TotalPagamentoMotorista.visible(VisibleSaldoDescontadoMotorista());

        _abertura.TipoPessoaResponsavel.visible(_motivoChamadoConfiguracao.ObrigarInformarResponsavelAtendimento);
        _abertura.TipoPessoaResponsavel.val(_motivoChamadoConfiguracao.ObrigarInformarResponsavelAtendimento ? EnumTipoPessoaGrupo.Pessoa : "");
        tipoPessoaResponsavelChange();

        _abertura.Motorista.visible(false);
        if (_motivoChamadoConfiguracao.PermiteSelecionarMotorista) {
            _abertura.Motorista.visible(true);

            if (_abertura.Carga.codEntity() > 0)
                _buscaMotorista.CarregarMotoristaCargaSelecionada();
            else
                _abertura.Motorista.enable(false);
        }
        else
            LimparCampo(_abertura.Motorista);

        ControleCamposAberturaSemCarga(true);

        DefinirEtapa2();
        DefinirEtapa3();

        BuscarSaldoDescontadoMotorista();
        buscarDatasMotivoChamado();
        MostrarJustificativaOcorrencia(dataRow.PermiteInserirJustificativaOcorrencia);

        if (_motivoChamadoConfiguracao.InformarQuantidade) {
            _abertura.QuantidadeMotivoChamado.visible(true);
            _abertura.QuantidadeMotivoChamado.required(true);
        }

        if (_motivoChamadoConfiguracao.ObrigarRealMotivo) {
            _abertura.RealMotivo.visible(true);
            _abertura.RealMotivo.required(true);
        }
        else {
            _abertura.RealMotivo.visible(false);
            _abertura.RealMotivo.required(false);
            _abertura.RealMotivo.codEntity(0);
            _abertura.RealMotivo.val("");
        }

        if (_motivoChamadoConfiguracao.ExigeInformarModeloVeicularAberturaChamado) {
            _abertura.ModeloVeicularCarga.visible(true);
            _abertura.ModeloVeicularCarga.required(true);
        } else {
            _abertura.ModeloVeicularCarga.visible(false);
            _abertura.ModeloVeicularCarga.required(false);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
            _abertura.Pedido.visible(true);
            _abertura.Carga.visible(false);
            _abertura.Carga.required(false);
            _abertura.Tomador.enable(false);
            _abertura.Cliente.enable(false);
            _abertura.Pedido.required(true);
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) { 

            if (_configuracaoChamado.PermitirGerarAtendimentoPorPedido) {
                _abertura.PedidoEmbarcador.visible(true);
                _abertura.PedidoEmbarcador.required(false);
                _abertura.Carga.visible(true);
                _abertura.Carga.required(false);
            }
        }

        if (_motivoChamadoConfiguracao.PermitirInformarCausadorOcorrencia) {
            _abertura.CausasMotivoChamado.visible(true);
            _abertura.CausasMotivoChamado.enable(true);
            _abertura.TiposCausadoresOcorrencia.visible(true);
            _abertura.TiposCausadoresOcorrencia.enable(true);
        } else {
            _abertura.CausasMotivoChamado.visible(false);
            _abertura.CausasMotivoChamado.enable(false);
            _abertura.TiposCausadoresOcorrencia.visible(false);
            _abertura.TiposCausadoresOcorrencia.enable(false);
        }

        if (dataRow.ValidaValorCarga || dataRow.ValidaValorDescarga) {
            BuscarValorCargaDescarga();
        }

        if (_CONFIGURACAO_TMS.OcultarTomadorNoAtendimento) {
            _abertura.Tomador.visible(false);
        }

    });
}

function ValidarChamadoDuplicado() {
    var data = {
        Codigo: _abertura.Codigo.val(),
        Carga: _abertura.Carga.codEntity(),
        MotivoChamado: _abertura.MotivoChamado.codEntity(),
        Cliente: _abertura.Cliente.codEntity(),
        Valor: _abertura.Valor.val(),
        Motorista: _abertura.Motorista.codEntity()
    };
    executarReST("ChamadoOcorrencia/ValidaChamadoDuplicado", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.AtendimentoDuplicado)
                    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.AtendimentoDuplicado, Localization.Resources.Cargas.ControleEntrega.JaExisteAtendimentoLancadoMesmaCarga, AdicionarChamadoOcorrenciaClick);
                else if (arg.Data.ChamadoMesmoMotorista)
                    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.AtendimentoDuplicado, "Já existe um pagamento em aberto para esse motorista. Tem certeza que deseja continuar?", AdicionarChamadoOcorrenciaClick);
                else
                    AdicionarChamadoOcorrenciaClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AbrirChamadoOcorrenciaClick() {

    if (!validaRegrasAbertura())
        return;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        ValidarChamadoDuplicado();
    else
        AdicionarChamadoOcorrenciaClick();
}

function AdicionarChamadoOcorrenciaClick() {
    var data = { CodigoMotivoChamado: _abertura.MotivoChamado.codEntity(), CodigoCarga: _abertura.Carga.codEntity() };

    if (!_controleEntregaDevolucaoChamadoEtapa1._validarDadosPreenchidosDevolucao())
        return;

    executarReST("ChamadoOcorrencia/ObterConfiguracoesMotivoChamado", data, function (retorno) {
        if (retorno.Success && retorno.Data) {

            _notaFiscal.TipoDevolucao.val() == 1 ? _abertura.TipoDevolucao.val(true) : _abertura.TipoDevolucao.val(false);

            let envioAbertura = RetornarObjetoPesquisa(_abertura);
            envioAbertura["Datas"] = obterDatas();
            envioAbertura["PerfisAcesso"] = obterPerfisAcesso();
            envioAbertura["NotasFiscais"] = obterNotasFiscais();
            envioAbertura["ItensDevolver"] = _controleEntregaDevolucaoChamadoEtapa1.obter();

            let nfeDevolucao = obterNotasFiscaisDevolucaoAbertura();
            envioAbertura["NFeDevolucaoAbertura"] = JSON.stringify(nfeDevolucao);
            envioAbertura["MotivoDaDevolucao"] = _notaFiscal.MotivoDaDevolucao.codEntity();

            if (retorno.Data.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga && retorno.Data.ExisteChamadoComMesmoMotivoChamadoCarga) {
                permitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga(envioAbertura, retorno.Data.NumerosChamadosExistentes);
            } else {
                AbrirChamadoOcorrencia(envioAbertura);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function formatarObjeto(obj) {
    let resultado = {};

    for (let key in obj) {
        if (obj.hasOwnProperty(key) && obj[key].hasOwnProperty('val')) {
            resultado[key] = obj[key].val;
        }
    }

    return resultado;
}

function obterNotasFiscaisDevolucaoAbertura() {
    return _koNfdAbertura.NFeDevolucaoAbertura.list.map(notaFiscal => formatarObjeto(notaFiscal));
}

function AtualizarChamadoOcorrenciaClick(e, sender) {
    if (!validaRegrasAbertura())
        return;

    var envioAbertura = RetornarObjetoPesquisa(_abertura);
    envioAbertura["Datas"] = obterDatas();
    envioAbertura["NotasFiscais"] = obterNotasFiscais();

    let nfeDevolucao = obterNotasFiscaisDevolucaoAbertura();
    envioAbertura["NFeDevolucaoAbertura"] = JSON.stringify(nfeDevolucao);
    executarReST("ChamadoOcorrencia/Atualizar", envioAbertura, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.Sucesso, Localization.Resources.Cargas.ControleEntrega.AtendimentoAtualizadoSucesso);
                _chamado.Codigo.val(arg.Data);
                buscarChamadoPorCodigo(arg.Data);
                recarregarGridChamados();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function validaRegrasAbertura() {
    var valido = ValidarCamposObrigatorios(_abertura);
    var validoData = validarDatasInformadas();
    if (!valido || !validoData) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.ControleEntrega.CamposObrigatorios, Localization.Resources.Cargas.ControleEntrega.PorfavorInformeCamposObrigatorios);
        return false;
    }
    else if (_motivoChamadoConfiguracao.ObrigarAnexo && !isPossuiAnexoChamado()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.ControleEntrega.AnexosObrigatorios, Localization.Resources.Cargas.ControleEntrega.PorfavorAdicionePeloMenosAnexo);
        return false;
    }

    if (_notaFiscal && _notaFiscal.TipoDevolucao && _notaFiscal.TipoDevolucao.val() == 1 &&
        _motivoChamadoConfiguracao.ObrigarPreenchimentoNFD && !(_koNfdAbertura && _koNfdAbertura.NFeDevolucaoAbertura.list.length > 0)) {
        exibirMensagem(tipoMensagem.atencao, "NF-e de Devolução", "É obrigatório preencher pelo menos uma Nota de Devolução para o motivo selecionado.");
        return false;
    }

    return true;
}

function tipoPessoaResponsavelChange() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS)
        return;

    if (_abertura.TipoPessoaResponsavel.val() === EnumTipoPessoaGrupo.Pessoa) {
        _abertura.ClienteResponsavel.visible(true);
        _abertura.ClienteResponsavel.required(true);
        _abertura.GrupoPessoasResponsavel.visible(false);
        _abertura.GrupoPessoasResponsavel.required(false);

        LimparCampoEntity(_abertura.GrupoPessoasResponsavel);
    }
    else if (_abertura.TipoPessoaResponsavel.val() === EnumTipoPessoaGrupo.GrupoPessoa) {
        _abertura.GrupoPessoasResponsavel.visible(true);
        _abertura.GrupoPessoasResponsavel.required(true);
        _abertura.ClienteResponsavel.visible(false);
        _abertura.ClienteResponsavel.required(false);

        LimparCampoEntity(_abertura.ClienteResponsavel);
    }
    else {
        _abertura.GrupoPessoasResponsavel.visible(false);
        _abertura.GrupoPessoasResponsavel.required(false);
        _abertura.ClienteResponsavel.visible(false);
        _abertura.ClienteResponsavel.required(false);
        _abertura.TipoPessoaResponsavel.visible(false);

        LimparCampoEntity(_abertura.ClienteResponsavel);
        LimparCampoEntity(_abertura.GrupoPessoasResponsavel);
        LimparCampo(_abertura.TipoPessoaResponsavel);
    }
}

function BuscarSaldoDescontadoMotorista() {
    if (preenchendoAbertura)
        return;

    if ((_chamado.Situacao.val() === EnumSituacaoChamado.Todas || _chamado.Situacao.val() === EnumSituacaoChamado.SemRegra || _chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa)
        && _abertura.Motorista.codEntity() > 0 && _abertura.MotivoChamado.codEntity() > 0 && Boolean(_abertura.Valor.val()) && Globalize.parseFloat(_abertura.Valor.val()) > 0) {

        var data = { Motorista: _abertura.Motorista.codEntity(), MotivoChamado: _abertura.MotivoChamado.codEntity(), Valor: _abertura.Valor.val() };
        executarReST("ChamadoOcorrencia/BuscarSaldoDescontadoMotorista", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _abertura.SaldoDescontadoMotorista.val(arg.Data.SaldoDescontado);
                    _abertura.TotalPagamentoMotorista.val(arg.Data.TotalPagamento);
                } else {
                    _abertura.SaldoDescontadoMotorista.val("0,00");
                    _abertura.TotalPagamentoMotorista.val(_abertura.Valor.val());
                }
            } else {
                _abertura.SaldoDescontadoMotorista.val("0,00");
                _abertura.TotalPagamentoMotorista.val(_abertura.Valor.val());

                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    } else {
        _abertura.SaldoDescontadoMotorista.val("");
        _abertura.TotalPagamentoMotorista.val("");
    }
}

function BuscarValorCargaDescarga() {
    LimpaValores();

    var data = { Carga: _abertura.Carga.codEntity() };
    executarReST("ChamadoOcorrencia/BuscarValorCargaDescarga", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _abertura.ValorCarga.val(arg.Data.valorCarga);
                _abertura.ValorCarga.visible(true);
                _abertura.ValorCarga.enable(false);
                _abertura.ValorDescarga.val(arg.Data.valorDescarga);
                _abertura.ValorDescarga.visible(true);
                _abertura.ValorDescarga.enable(false);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }


    });
}

function LimpaValores() {
    _abertura.ValorCarga.visible(false);
    _abertura.ValorCarga.val(null);

    _abertura.ValorDescarga.visible(false);
    _abertura.ValorDescarga.val(null);
}

//*******MÉTODOS*******

function EditarAbertura(data) {
    preenchendoAbertura = true;
    _abertura.Codigo.val(data.Codigo);
    if (data.Abertura != null) {
        PreencherObjetoKnout(_abertura, { Data: data.Abertura });
    }

    _notaFiscal.NFe.enable(true);

    var status = false;
    if (_chamado.Situacao.val() === EnumSituacaoChamado.SemRegra || _chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa)
        status = true;

    preencherEnvioDataMotivoChamado(data.Datas, status);
    preencherPerfilAcessoChamado(data);
    ControleCamposAbertura(status);
    preencherXMLNotasFiscais(data);

    preenchendoAbertura = false;

    if (data.Abertura.ApresentarValorPesoDaCarga) {
        _abertura.ValorDaCarga.visible(true);
        _abertura.PesoDaCarga.visible(true);
    }

    if (data.Abertura.ExibirQuantidadeMotivoChamado) {
        _abertura.QuantidadeMotivoChamado.visible(true);
        _abertura.QuantidadeMotivoChamado.enable(false);
    }

    if (data.Abertura.ModeloVeicularCarga.Codigo > 0) {
        _abertura.ModeloVeicularCarga.visible(true);
        _abertura.ModeloVeicularCarga.enable(false);
    }

    if (_motivoChamadoConfiguracao.PermitirInformarCausadorOcorrencia) {
        _abertura.CausasMotivoChamado.visible(true);
        _abertura.CausasMotivoChamado.enable(true);
        _abertura.TiposCausadoresOcorrencia.visible(true);
        _abertura.TiposCausadoresOcorrencia.enable(true);
    } else {
        _abertura.CausasMotivoChamado.visible(false);
        _abertura.CausasMotivoChamado.enable(false);
        _abertura.TiposCausadoresOcorrencia.visible(false);
        _abertura.TiposCausadoresOcorrencia.enable(false);
    }   
}

function ControleCamposAbertura(status) {
    _abertura.Carga.enable(status);
    _abertura.MotivoChamado.enable(status);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador && _configuracaoChamado.PermitirGerarAtendimentoPorPedido)
    {
        _abertura.PedidoEmbarcador.enable(status);
    }

    var cargaInformada = _abertura.Carga.codEntity() > 0;
    var motivoInformado = _abertura.MotivoChamado.codEntity() > 0;

    _abertura.Cliente.enable(status && cargaInformada && !_motivoChamadoConfiguracao.MotivoDevolucao);
    _abertura.Tomador.enable(status && cargaInformada && !_motivoChamadoConfiguracao.MotivoDevolucao);
    _abertura.Destinatario.enable(status && cargaInformada && !_motivoChamadoConfiguracao.MotivoDevolucao);
    _abertura.Motorista.enable(status && cargaInformada);
    _abertura.Motorista.visible(motivoInformado && _motivoChamadoConfiguracao.PermiteSelecionarMotorista);
    _abertura.ClienteResponsavel.enable(status);
    _abertura.Observacao.enable(status);
    _abertura.ResponsavelChamado.enable(status);
    _abertura.Valor.enable(status);
    _abertura.NumeroPallet.enable(status);
    _abertura.QuantidadeItens.enable(status);
    _abertura.Valor.enable(status);
    _abertura.Valor.visible(motivoInformado && (_motivoChamadoConfiguracao.ExigeValor || (_motivoChamadoConfiguracao.ExigeValorNaLiberacao && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe)));
    _abertura.ValorReferencia.enable(status);
    _abertura.ValorReferencia.visible(motivoInformado && _motivoChamadoConfiguracao.ReferentePagamentoDescarga);
    _abertura.ValorDesconto.enable(status);
    _abertura.ValorDesconto.visible(motivoInformado && _motivoChamadoConfiguracao.PermiteInformarDesconto);
    _abertura.PagoPeloMotorista.enable(status);
    _abertura.PagoPeloMotorista.visible(motivoInformado && _motivoChamadoConfiguracao.PermiteAdicionarValorComoDespesaMotorista);
    _abertura.SaldoDescontadoMotorista.visible(motivoInformado && VisibleSaldoDescontadoMotorista());
    _abertura.TotalPagamentoMotorista.visible(motivoInformado && VisibleSaldoDescontadoMotorista());

    _abertura.DataEntradaRaio.enable(status);
    _abertura.DataEntradaRaio.visible(motivoInformado && _motivoChamadoConfiguracao.PermiteAlterarDatasCargaEntrega && _abertura.CargaEntrega.codEntity() > 0);
    _abertura.DataSaidaRaio.enable(status);
    _abertura.DataSaidaRaio.visible(motivoInformado && _motivoChamadoConfiguracao.PermiteAlterarDatasCargaEntrega && _abertura.CargaEntrega.codEntity() > 0);

    _abertura.Destinatario.visible(false);
    _abertura.Destinatario.required(false);
    _abertura.TipoCliente.visible(false);
    _abertura.RetencaoBau.visible(false);
    _abertura.DataRetencaoInicio.visible(false);
    _abertura.DataRetencaoInicio.required(false);
    _abertura.DataRetencaoFim.visible(false);
    _abertura.DataRetencaoFim.required(false);
    _abertura.DataReentrega.visible(false);
    _abertura.DataReentrega.required(false);

    _abertura.TipoPessoaResponsavel.enable(status);
    _abertura.TipoPessoaResponsavel.visible(motivoInformado && _motivoChamadoConfiguracao.ObrigarInformarResponsavelAtendimento);
    _abertura.GrupoPessoasResponsavel.enable(status);
    tipoPessoaResponsavelChange();

    _abertura.RealMotivo.enable(status);

    if (_chamado.Codigo.val() == 0) {
        _crudAbertura.Abrir.visible(true);
        return;
    }

    _crudAbertura.Abrir.visible(false);
    var chamadoEmAbertoEmTratativa = _chamado.Situacao.val() === EnumSituacaoChamado.Aberto || _chamado.Situacao.val() === EnumSituacaoChamado.EmTratativa;

    if (_motivoChamadoConfiguracao.MotivoDevolucao) {
        _abertura.Carga.enable(false);
        _abertura.Pedido.enable(false);
        _abertura.PedidoEmbarcador.enable(false);
        _abertura.Cliente.enable(false);
        _crudAbertura.Atualizar.visible(chamadoEmAbertoEmTratativa);
    }
    else if (_motivoChamadoConfiguracao.MotivoReentrega) {
        _crudAbertura.Atualizar.visible(chamadoEmAbertoEmTratativa);
        _abertura.Carga.enable(false);
        _abertura.Pedido.enable(false);
        _abertura.PedidoEmbarcador.enable(false);
        _abertura.Cliente.visible(false);
        _abertura.TipoCliente.visible(false);
        _abertura.Destinatario.visible(true);
        _abertura.Destinatario.required(true);
        _abertura.DataReentrega.visible(true);
        _abertura.DataReentrega.required(true);

        _abertura.Destinatario.enable(chamadoEmAbertoEmTratativa);
        _abertura.DataReentrega.enable(chamadoEmAbertoEmTratativa);
        _abertura.TipoCliente.enable(chamadoEmAbertoEmTratativa);
    }
    else if (_motivoChamadoConfiguracao.MotivoRetencao) {
        _crudAbertura.Atualizar.visible(chamadoEmAbertoEmTratativa);
        _abertura.Carga.enable(false);
        _abertura.Pedido.enable(false);
        _abertura.PedidoEmbarcador.enable(false);
        _abertura.Cliente.visible(false);
        _abertura.TipoCliente.visible(true);
        if (_abertura.TipoCliente.val() === EnumTipoTomador.Remetente) {
            _abertura.Destinatario.visible(false);
            _abertura.Destinatario.required(false);
        }
        else {
            _abertura.Destinatario.visible(true);
            _abertura.Destinatario.required(true);
        }
        _abertura.RetencaoBau.visible(true);
        _abertura.DataRetencaoInicio.visible(true);
        _abertura.DataRetencaoInicio.required(true);
        _abertura.DataRetencaoFim.visible(true);
        _abertura.DataRetencaoFim.required(true);

        _abertura.Destinatario.enable(chamadoEmAbertoEmTratativa);
        _abertura.TipoCliente.enable(chamadoEmAbertoEmTratativa);
        _abertura.RetencaoBau.enable(chamadoEmAbertoEmTratativa);
        _abertura.DataRetencaoInicio.enable(chamadoEmAbertoEmTratativa);
        _abertura.DataRetencaoFim.enable(chamadoEmAbertoEmTratativa);
    }
    else if (_CONFIGURACAO_TMS.TipoChamado === EnumTipoChamado.PadraoEmbarcador) {
        _crudAbertura.Atualizar.visible(status);
    }
    else if (_motivoChamadoConfiguracao.ExigeValorNaLiberacao) {
        _crudAbertura.Atualizar.visible(chamadoEmAbertoEmTratativa || _chamado.Situacao.val() === EnumSituacaoChamado.SemRegra);
        _abertura.Valor.enable(chamadoEmAbertoEmTratativa || _chamado.Situacao.val() === EnumSituacaoChamado.SemRegra);
    }
    else if (_configuracaoChamado.PermitirAtualizarChamadoStatus) {
        _crudAbertura.Atualizar.visible(status);
    }

    if (_motivoChamadoConfiguracao.ObrigarRealMotivo) {
        _abertura.RealMotivo.visible(true);
        _abertura.RealMotivo.required(true);
    }
    else {
        _abertura.RealMotivo.visible(false);
        _abertura.RealMotivo.required(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _abertura.Cliente.visible(true);
        _abertura.Tomador.visible(true);
        _abertura.Destinatario.visible(true);
    }

    ControleCamposAberturaSemCarga(status);

    if (status.QuantidadeMotivoChamado != null && status.QuantidadeMotivoChamado != "")
        _abertura.QuantidadeMotivoChamado.val(status.QuantidadeMotivoChamado);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Fornecedor) {
        _abertura.Pedido.visible(true);
        _abertura.Carga.visible(false);
        _abertura.Carga.required(false);
        _abertura.Pedido.required(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {

        if (_configuracaoChamado.PermitirGerarAtendimentoPorPedido) {
            _abertura.PedidoEmbarcador.visible(true);
            _abertura.PedidoEmbarcador.required(false);
            _abertura.PedidoEmbarcador.visible(true);
            _abertura.PedidoEmbarcador.required(false);
        }
    }

    if (_CONFIGURACAO_TMS.OcultarTomadorNoAtendimento) {
        _abertura.Tomador.visible(false);
    } 
}

function ControleCamposAberturaSemCarga(status) {
    if (_motivoChamadoConfiguracao.PermiteAtendimentoSemCarga) {
        _abertura.Carga.required(false);
        _abertura.Motorista.enable(status && _abertura.Motorista.visible());
        _abertura.Motorista.required(status && _abertura.Motorista.visible());

        TrocaComponentesModalBuscas(_abertura.Carga.codEntity() > 0 ? false : true);
    } else {
        _abertura.Carga.required(true);
        _abertura.Motorista.required(false);

        TrocaComponentesModalBuscas(false);
    }
}

function TrocaComponentesModalBuscas(atendimentoSemCarga) {
    if (atendimentoSemCarga) {
        if (_buscaCliente.CarregarClienteCargaSelecionada) {
            _buscaCliente.Destroy();
            _buscaCliente = new BuscarClientes(_abertura.Cliente);
        }
        if (_buscaTomador.CarregarTomadorCargaSelecionada) {
            _buscaTomador.Destroy();
            _buscaTomador = new BuscarClientes(_abertura.Tomador);
        }
        if (_buscaDestinatario.CarregarDestinatarioCargaSelecionada) {
            _buscaDestinatario.Destroy();
            _buscaDestinatario = new BuscarClientes(_abertura.Destinatario);
        }
        if (_buscaMotorista.CarregarMotoristaCargaSelecionada) {
            _buscaMotorista.Destroy();
            _buscaMotorista = new BuscarMotoristas(_abertura.Motorista);
        }
    } else {
        if (!_buscaCliente.CarregarClienteCargaSelecionada) {
            _buscaCliente.Destroy();
            _buscaCliente = new BuscarClientesCarga(_abertura.Cliente, retornoBuscaCliente, _abertura.Carga, !_CONFIGURACAO_TMS.GerarOcorrenciaParaCargaAgrupada);
        }
        if (!_buscaTomador.CarregarTomadorCargaSelecionada) {
            _buscaTomador.Destroy();
            _buscaTomador = new BuscarTomadoresCarga(_abertura.Tomador, null, _abertura.Carga);
        }
        if (!_buscaDestinatario.CarregarDestinatarioCargaSelecionada) {
            _buscaDestinatario.Destroy();
            _buscaDestinatario = new BuscarDestinatariosCarga(_abertura.Destinatario, retornoBuscaDestinatario, _abertura.Carga);
        }
        if (!_buscaMotorista.CarregarMotoristaCargaSelecionada) {
            _buscaMotorista.Destroy();
            _buscaMotorista = new BuscarMotoristasCarga(_abertura.Motorista, null, _abertura.Carga);
        }
    }
}

function VisibleSaldoDescontadoMotorista() {
    return _motivoChamadoConfiguracao.PermiteAdicionarValorComoAdiantamentoMotorista || (_abertura.PagoPeloMotorista.val() && _abertura.PagoPeloMotorista.visible());
}

function LimparCamposAbertura() {
    LimparCampos(_abertura);
    _abertura.Anexo.visible(true);
    _abertura.QuantidadeMotivoChamado.visible(false);
    _abertura.QuantidadeMotivoChamado.enable(true);
    _abertura.ModeloVeicularCarga.visible(false);
    _abertura.ModeloVeicularCarga.enable(true);
    _crudAbertura.Atualizar.visible(false);
    _notaFiscal.NFe.enable(false);
    ControleCamposAbertura(true);
    limparCamposChamadoData();
    limparCamposNotaFiscal();
    limparCamposChamadoPerfilAcesso();
    _controleEntregaDevolucaoChamadoEtapa1.limpar();
    $('#controle-entrega-devolucao-chamado-container-etapa-1').hide();
}

function permitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga(envioAbertura, numerosChamadosExistentes) {
    exibirConfirmacao("Confirmação", "Existe ao menos um chamado com o mesmo motivo e carga, identificado pelo(s) número(s): " + numerosChamadosExistentes + ". Deseja realmente criar um novo semelhante?", function () {
        AbrirChamadoOcorrencia(envioAbertura);
    });
};

function AbrirChamadoOcorrencia(envioAbertura) {
    executarReST("ChamadoOcorrencia/Adicionar", envioAbertura, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.Sucesso, Localization.Resources.Cargas.ControleEntrega.AtendimentoAbertoSucesso);
                _chamado.Codigo.val(arg.Data);

                EnviarArquivosAnexadosChamado(function () {
                    buscarChamadoPorCodigo(arg.Data);
                });

                recarregarGridChamados();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function carregarConfiguracaoChamado(callback) {

    executarReST("ChamadoOcorrencia/BuscarConfiguracoesChamado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _configuracaoChamado = arg.Data;
                callback();
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}