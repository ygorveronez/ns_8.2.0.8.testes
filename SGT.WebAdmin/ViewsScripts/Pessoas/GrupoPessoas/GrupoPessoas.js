/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/PessoaClassificacao.js" />
/// <reference path="../../Consultas/RateioFormula.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFinanceira.js" />
/// <reference path="Cliente.js" />
/// <reference path="ValidacaoNFe.js" />
/// <reference path="GrupoPessoaObservacaoNfe.js" />
/// <reference path="GrupoPessoasPerfilChamado.js" />
/// <reference path="GrupoPessoasAnexo.js" />
/// <reference path="GrupoPessoasOcorrencia.js" />
/// <reference path="GrupoPessoasConfiguracaoEmail.js" />
/// <reference path="GrupoPessoasAdicional.js" />
/// <reference path="GrupoPessoasFornecedor.js" />
/// <reference path="GrupoPessoasLeituraDinamicaXML.js" />
/// <reference path="../../Enumeradores/EnumPrioridadeGrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoPessoas,
    _grupoPessoas,
    _grupoPessoasCRUD,
    _pesquisaGrupoPessoas,
    _configuracaoEmissaoCTe,
    _configuracaoLayoutEDI,
    _configuracaoFatura;

var _PermissoesPersonalizadas;
var _gridListaRaizCNPJ;

var RaizCNPJMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.RaizCNPJ = PropertyEntity({ type: types.map, val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.text });
    this.AdicionarPessoasMesmaRaiz = PropertyEntity({ type: types.map, val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.text });
};

var PesquisaGrupoPessoas = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Descricao.getFieldDescription() });
    this.RaizCNPJ = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.RaizCNPJ.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Pessoas.GrupoPessoas.Situacao.getFieldDescription() });
    this.TipoGrupoPessoas = PropertyEntity({ val: ko.observable(EnumTipoGrupoPessoas.Ambos), options: EnumTipoGrupoPessoas.obterOpcoes(), def: EnumTipoGrupoPessoas.Ambos, text: Localization.Resources.Pessoas.GrupoPessoas.TipoGrupo.getFieldDescription() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.Pessoa.getFieldDescription(), idBtnSearch: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoPessoas.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var GrupoPessoas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Descricao.getRequiredFieldDescription(), issue: 586, required: true, enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.CodigoIntegracao.getFieldDescription(), required: false, visible: true, issue: 15, maxlength: 50, enable: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Pessoas.GrupoPessoas.Situacao.getRequiredFieldDescription(), issue: 557, required: true, enable: ko.observable(true) });
    this.TipoGrupoPessoas = PropertyEntity({ val: ko.observable(EnumTipoGrupoPessoas.Clientes), options: EnumTipoGrupoPessoas.obterOpcoes(), def: EnumTipoGrupoPessoas.Clientes, text: Localization.Resources.Pessoas.GrupoPessoas.TipoGrupo.getRequiredFieldDescription(), required: true, enable: ko.observable(true) });
    this.NaoImportarDocumentosDestinadosTransporte = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValidaPlacaNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValidaDestinoNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ArmazenaProdutosXMLNFE = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValidaOrigemNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValidaEmitenteNFe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.LerNumeroPedidoDaObservacaoDaNota = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExigirRotaParaEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExigirRotaFreteCargaDocumentos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ExigirNumeroControleCliente = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExigirNumeroControleCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ReplicarNumeroControleCliente = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ReplicarNumeroControleClienteTodasNotas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExigirNumeroNumeroReferenciaCliente = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExigirNumeroReferenciaCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarAutomaticamenteDocumentacaoCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ReplicarNumeroReferenciaTodasNotasCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ReplicarNumeroReferenciaClienteNotas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarPedidoColeta = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.GerarPedidoColeta, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ControlaPagamentos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ControlaPagamentos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarOcorrenciaControleEntrega = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.GerarOcorrenciaEventosControleColeta, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PermitirConsultarOcorrenciaControleEntregaWebService = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.PermitirConsultarOcorrenciasWeb, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarNumeroConhecimentoNoBoleto = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.GerarNumeroDocumentoBoleto, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.GerarNumeroFaturaNoBoleto = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.GerarNumeroDocumentoFatura, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.NaoAlterarDocumentoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoAlterarDocumentoIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });


    this.Contato = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Contato.getFieldDescription(), required: false, visible: true, maxlength: 500, enable: ko.observable(true), visible: ko.observable(false) });
    this.TelefoneContato = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Telefone.getFieldDescription(), required: false, getType: typesKnockout.phone, enable: ko.observable(true), visible: ko.observable(false) });

    this.RecebedorColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.Recebedor.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.Transportador.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacaoColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.TipoOperacao.getFieldDescription(), issue: 121, idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.ExigirRotaCalculoFreteParaEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExigirRotaCalculoFreteEmissao, issue: 2368, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.UtilizaMultiEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.UtilizaMultiEmbarcador), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.Classificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.GrupoPessoas.Classificacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Bloqueado = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Bloqueado, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.MotivoBloqueio = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.MotivoBloqueio.getFieldDescription(), enable: ko.observable(true) });

    this.LerPDFNotaFiscalRecebidaPorEmail = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), text: Localization.Resources.Pessoas.GrupoPessoas.LerPDFNotaFiscalRecebida, enable: ko.observable(true) });

    this.UtilizaMetaEmissao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), text: Localization.Resources.Pessoas.GrupoPessoas.UtilizaMetasEmissao, enable: ko.observable(true) });
    this.MetaEmissaoMensal = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false), text: Localization.Resources.Pessoas.GrupoPessoas.MetaMensal.getFieldDescription(), enable: ko.observable(true) });
    this.MetaEmissaoAnual = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false), text: Localization.Resources.Pessoas.GrupoPessoas.MetaAnual.getFieldDescription(), enable: ko.observable(true) });

    this.EmailEnvioNovoVeiculo = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EmailEnvioNovosVeiculos.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000, visible: ko.observable(true), enable: ko.observable(false) });
    this.EnviarNovoVeiculoEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.GrupoPessoas.Email, enable: ko.observable(true) });

    this.Email = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Email.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000, visible: ko.observable(true), enable: ko.observable(true) });
    this.EnviarXMLCTePorEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML", enable: ko.observable(true) });

    this.RegraCotacaoFeeder = PropertyEntity({ val: ko.observable(EnumRegraCotacaoFeeder.Nenhuma), options: EnumRegraCotacaoFeeder.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.RegraCotacaoFeeder.getFieldDescription(), def: EnumRegraCotacaoFeeder.Nenhuma, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false) });
    this.TipoEmissaoCTeDocumentosExclusivo = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoes(), text: Localization.Resources.Pessoas.GrupoPessoas.RateioDocumentosExclusivo.getFieldDescription(), def: EnumTipoEmissaoCTeDocumentos.NaoInformado, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false), issue: 400 });
    this.RateioFormulaExclusivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.GrupoPessoas.FormulaRateioFreteExclusivo.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });
    this.ConfiguracaoEmissaoCTe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.ConfiguracaoLayoutEDI = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.ConfiguracaoFatura = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });

    this.Clientes = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarPessoasForaRaizCNPJ, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.ListaRaizCNPJ = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.GridRaizCNPJ = PropertyEntity({ type: types.local });
    this.RaizCNPJ = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.RaizCNPJ.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), idBtnSearch: guid(), validaEscritaBusca: false });
    this.AddPessoasComRaiz = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarTodasPessoasMesmaRaiz, def: true, visible: true, enable: ko.observable(true) });
    this.AdicionarRaiz = PropertyEntity({ eventClick: AdicionarRaizCNPJClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: ko.observable(true) });

    this.ModelosReboque = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Ocorrencias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.EmailsDocumentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.LeituraDinamicaXML = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaContatos = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Contatos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ListaVendedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.ListaMensagemAlerta = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.AutorizadosDownloadDFe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });


    this.Comprovantes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ExigirComprovantesLiberacaoPagamentoContratoFrete = PropertyEntity({ val: ko.observable(true), def: false, type: types.map, getType: typesKnockout.bool, text: "traduzir ExigirComprovantesLiberaçãoPagamentoContratoFrete", visible: ko.observable(true), enable: ko.observable(true) });


    this.InformarProdutoPredominante = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.ProdutoPredominante = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DefinirProdutoPredominanteGrupo, required: false, enable: ko.observable(false), visible: ko.observable(true) });

    this.ObservacaoNfe = PropertyEntity({ maxlength: 2000, enable: ko.observable(true) });
    this.FormulasObservacaoNfe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.NCMPalletsNFe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.TabelaValores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.PerfilChamado = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.ObservacoesCTes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.DadosAdicionais = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Fornecedor = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Prioridade = PropertyEntity({ val: ko.observable(), options: EnumPrioridadeGrupoPessoas.obterOpcoes(), text: "Prioridade", required: false, enable: ko.observable(true) });

    //VALIDAÇÕES
    this.NaoImportarDocumentosDestinadosTransporte.val.subscribe(function (novoValor) {
        _documentoDestinado.NaoImportarDocumentosDestinadosTransporte.val(novoValor);
    });

    this.ExigirNumeroNumeroReferenciaCliente.val.subscribe(function (novoValor) {
        _grupoPessoas.ReplicarNumeroReferenciaTodasNotasCarga.visible(novoValor === true);
        if (novoValor === false)
            _grupoPessoas.ReplicarNumeroReferenciaTodasNotasCarga.val(false);
    });

    this.ExigirNumeroControleCliente.val.subscribe(function (novoValor) {
        _grupoPessoas.ReplicarNumeroControleCliente.visible(novoValor === true);
        if (novoValor === false)
            _grupoPessoas.ReplicarNumeroControleCliente.val(false);
    });

    this.ValidaPlacaNFe.val.subscribe(function (novoValor) {
        _validacaoNFe.ValidaPlacaNFe.val(novoValor);
    });
    this.ValidaDestinoNFe.val.subscribe(function (novoValor) {
        _validacaoNFe.ValidaDestinoNFe.val(novoValor);
    });

    this.ArmazenaProdutosXMLNFE.val.subscribe(function (novoValor) {
        _validacaoNFe.ArmazenaProdutosXMLNFE.val(novoValor);
    });
    this.ValidaOrigemNFe.val.subscribe(function (novoValor) {
        _validacaoNFe.ValidaOrigemNFe.val(novoValor);
    });

    this.ValidaEmitenteNFe.val.subscribe(function (novoValor) {
        _validacaoNFe.ValidaEmitenteNFe.val(novoValor);
    });

    //this.LerNumeroPedidoDaObservacaoDaNota.val.subscribe(function (novoValor) {
    //    _validacaoNFe.LerNumeroPedidoDaObservacaoDaNota.val(novoValor);
    //});

    this.InformarProdutoPredominante.val.subscribe(function (novoValor) {
        _grupoPessoas.ProdutoPredominante.enable(novoValor);
    });

    this.EnviarNovoVeiculoEmail.val.subscribe(function (novoValor) {
        _grupoPessoas.EmailEnvioNovoVeiculo.enable(novoValor);
    });

    this.GerarPedidoColeta.val.subscribe(function (valor) {
        _grupoPessoas.RecebedorColeta.visible(valor === true);
        _grupoPessoas.RecebedorColeta.required(valor === true);
        _grupoPessoas.TipoOperacaoColeta.visible(valor === true);
    });

    this.UtilizaMultiEmbarcador.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liConfiguracaoMultiEmbarcador").show();
        else
            $("#liConfiguracaoMultiEmbarcador").hide();
    });

    //Tab Tipo Integrações
    this.TipoIntegracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

};

var GrupoPessoasCrud = function () {
    this.Bloquear = PropertyEntity({ eventClick: BloquearClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Bloquear, visible: ko.observable(false) });
    this.Desbloquear = PropertyEntity({ eventClick: DesbloquearClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Desbloquear, visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Pessoas.GrupoPessoas.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "GrupoPessoas/Importar",
        UrlConfiguracao: "GrupoPessoas/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O032_GrupoPessoas,
        CallbackImportacao: function () {
            _gridGrupoPessoas.CarregarGrid();
        }
    });
};

//*******EVENTOS*******

function loadGrupoPessoas() {
    var habilitarConfiguracao = true;
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes, _PermissoesPersonalizadas))
        habilitarConfiguracao = false;

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoEmissao, _PermissoesPersonalizadas))
        $("#liEmissao").show();

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoFatura, _PermissoesPersonalizadas))
        $("#liConfiguracaoFatura").show();

    _grupoPessoas = new GrupoPessoas();
    KoBindings(_grupoPessoas, "knockoutCadastroGrupoPessoas");

    _grupoPessoasCRUD = new GrupoPessoasCrud();
    KoBindings(_grupoPessoasCRUD, "knockoutCadastroGrupoPessoasCRUD");

    HeaderAuditoria("GrupoPessoas", _grupoPessoas);

    $("#" + _grupoPessoas.RaizCNPJ.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });

    _pesquisaGrupoPessoas = new PesquisaGrupoPessoas();
    KoBindings(_pesquisaGrupoPessoas, "knockoutPesquisaGrupoPessoas", false, _pesquisaGrupoPessoas.Pesquisar.id);
    $("#" + _pesquisaGrupoPessoas.RaizCNPJ.id).mask("00.000.000", { selectOnFocus: true, clearIfNotMatch: true });

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: ExcluirRaizCNPJ }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "AdicionarPessoasMesmaRaiz", visible: false },
        { data: "RaizCNPJ", title: Localization.Resources.Pessoas.GrupoPessoas.RaizCNPJ, width: "80%" }
    ];

    _gridListaRaizCNPJ = new BasicDataTable(_grupoPessoas.GridRaizCNPJ.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridListaRaizCNPJ();

    new BuscarClientes(_pesquisaGrupoPessoas.Cliente);
    new BuscarClientes(_grupoPessoas.RaizCNPJ, retornoBuscaCliente);
    new BuscarTransportadores(_grupoPessoas.Transportador);
    new BuscarRateioFormulas(_grupoPessoas.RateioFormulaExclusivo);
    new BuscarPessoaClassificacao(_grupoPessoas.Classificacao);

    buscarGrupoPessoass();

    loadCliente();
    loadModeloReboque();
    loadDocumentoDestinado();
    loadValidacaoNFe();
    LoadContatoGrupoPessoas();
    loadGrupoPessoasVendedor();
    LoadAutorizadoDownloadDFe();
    loadGrupoPessoaObservacaoNfe();
    LoadGrupoPessoaImportacaoNFe();
    loadGrupoPessoasMensagemAlerta();
    LoadModeloVeicularEmbarcador();
    LoadTipoCargaEmbarcador();
    LoadConfiguracaoMultiEmbarcador();
    LoadBloqueioGrupoPessoas();
    LoadGrupoPessoasPerfilChamado();
    LoadDocumentoEmitidoEmbarcador();
    LoadCTeSubcontratacao();
    LoadObservacaoCTe();
    LoadConfiguracaoGeralCTe();
    loadAnexo();
    loadGrupoPessoasOcorrencia();
    loadGrupoPessoasConfigEmail();
    loadGrupoPessoasAdicional();
    loadGrupoPessoasFornecedor();
    loadConfiguracaoLogo();
    LoadGrupoPessoasTipoIntegracoes();
    loadComprovante();
    loadGrupoPessoasLeituraDinamicaXML();

    new BuscarClientes(_grupoPessoas.RecebedorColeta);
    new BuscarTiposOperacao(_grupoPessoas.TipoOperacaoColeta);

    _configuracaoEmissaoCTe = new ConfiguracaoEmissaoCTe("divConfiguracaoEmissaoCTe", _grupoPessoas.ConfiguracaoEmissaoCTe, null, _grupoPessoas.GrupoPessoas, habilitarConfiguracao);
    _configuracaoLayoutEDI = new ConfiguracaoLayoutEDI("divConfiguracaoLayoutEDI", _grupoPessoas.ConfiguracaoLayoutEDI);
    _configuracaoFatura = new ConfiguracaoFatura("divConfiguracaoFatura", _grupoPessoas.ConfiguracaoFatura, function () {

        _configuracaoFatura.Configuracao.ArmazenaCanhotoFisicoCTe.def = _CONFIGURACAO_TMS.PadraoArmazenamentoFisicoCanhotoCTe;
        _configuracaoFatura.Configuracao.ArmazenaCanhotoFisicoCTe.val(_CONFIGURACAO_TMS.PadraoArmazenamentoFisicoCanhotoCTe);

        configurarLayoutGrupoPessoasPorTipoSistema();

    });

    _configuracaoEmissaoCTe.VisualizarCamposGrupoPessoa();

    configurarLayoutGrupoPessoasPorTiposIntegracao();
}

function configurarLayoutGrupoPessoasPorTiposIntegracao() {
    executarReST("TipoIntegracao/BuscarTodos", { Tipos: JSON.stringify([EnumTipoIntegracao.SaintGobain]) }, function (r) {
        if (!r.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, r.Msg);

        if (!Boolean(r.Data))
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, r.Msg);

        for (var i = 0; i < r.Data.length; i++) {
            switch (r.Data[i].Codigo) {
                case EnumTipoIntegracao.SaintGobain:
                    _grupoPessoas.ControlaPagamentos.visible(true);
                    break;
            }
        }
    });
}

function configurarLayoutGrupoPessoasPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _grupoPessoas.UtilizaMetaEmissao.visible(true);
        _grupoPessoas.Transportador.visible(false);
        _grupoPessoas.GerarNumeroConhecimentoNoBoleto.visible(true);
        _grupoPessoas.GerarNumeroFaturaNoBoleto.visible(true);
        _grupoPessoas.EmailEnvioNovoVeiculo.visible(true);
        _grupoPessoas.EnviarAutomaticamenteDocumentacaoCarga.visible(true);


        _validacaoNFe.ValidaPlacaNFe.visible(true);
        _validacaoNFe.ValidaDestinoNFe.visible(true);
        _validacaoNFe.ValidaOrigemNFe.visible(true);
        _validacaoNFe.ValidaEmitenteNFe.visible(true);
        $("#liTabComprovante").show();
        $("#liTabModeloReboque").show();
        $("#liTabDocumentosDestinados").show();
        $("#liTabContatos").show();
        $("#liTabAutorizadoDownloadDFe").show();
        $("#liTabPerfilChamado").show();
        $("#liTabValidacaoNFe").show();
        $("#liTabknockoutObservacaoNfe").show();
        $("#liTabknockoutMensagemAlerta").show();
        $("#liTabDocumentoEmitidoEmbarcador").removeClass("d-none");
        $("#liCTeSubcontratacao").removeClass("d-none");
        $("#liTabObservacaoCTe").removeClass("d-none");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabAutorizadoDownloadDFe").show();
        //_validacaoNFe.LerNumeroPedidoDaObservacaoDaNota.visible(true);
        _grupoPessoas.ExigirRotaCalculoFreteParaEmissaoDocumentos.visible(false);
        _grupoPessoas.GerarNumeroConhecimentoNoBoleto.visible(false);
        _grupoPessoas.GerarNumeroFaturaNoBoleto.visible(false);
        _grupoPessoas.EmailEnvioNovoVeiculo.visible(false);
        $("#liTabObservacaoCTe").removeClass("d-none");
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _grupoPessoas.Email.visible(true);
        _grupoPessoas.Classificacao.visible(false);
        _grupoPessoas.Contato.visible(true);
        _grupoPessoas.TelefoneContato.visible(true);
        _grupoPessoas.ProdutoPredominante.visible(false);
        _grupoPessoas.ExigirRotaParaEmissaoDocumentos.visible(false);
        _grupoPessoas.GerarPedidoColeta.visible(false);
        _grupoPessoas.GerarNumeroConhecimentoNoBoleto.visible(false);
        _grupoPessoas.GerarNumeroFaturaNoBoleto.visible(false);
        _grupoPessoas.ExigirRotaCalculoFreteParaEmissaoDocumentos.visible(false);
        _grupoPessoas.UtilizaMultiEmbarcador.visible(false);
        _grupoPessoas.EmailEnvioNovoVeiculo.visible(false);

        _grupoPessoas.RegraCotacaoFeeder.visible(false);
        _grupoPessoas.TipoEmissaoCTeDocumentosExclusivo.visible(false);
        _grupoPessoas.RateioFormulaExclusivo.visible(false);

        $("#liEmissao").hide();
        $("#liCadastroLayoutEDI").hide();
        $("#liTabOcorrencias").hide();
    } else {
        $("#liTabValidacaoNFe").show();
        $("#liTabknockoutObservacaoNfe").show();
        $("#liTabknockoutMensagemAlerta").show();
    }

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _grupoPessoas.RegraCotacaoFeeder.visible(true);
        _grupoPessoas.TipoEmissaoCTeDocumentosExclusivo.visible(true);
        _grupoPessoas.RateioFormulaExclusivo.visible(true);
        _grupoPessoas.EmailEnvioNovoVeiculo.visible(false);
    }

    ValidarPermissaoPersonalizada();
}

function retornoBuscaCliente(data) {
    if (data != null && data.CPF_CNPJ != "") {
        _grupoPessoas.RaizCNPJ.val(data.CPF_CNPJ.replace(/[^0-9]/g, '').substring(0, 8));
    }
}

function preencherListasSelecao() {
    var pessoas = new Array();
    $.each(_clienteGrupo.Clientes.basicTable.BuscarRegistros(), function (i, cliente) {
        pessoas.push({ Cliente: cliente });
    });
    _grupoPessoas.Clientes.val(JSON.stringify(pessoas));

    var modelosReboque = new Array();

    _grupoPessoas.ModelosReboque.val(JSON.stringify(_gridModeloReboque.BuscarRegistros()));

    _grupoPessoas.Ocorrencias.val(JSON.stringify(arrayOcorrencias));
    _grupoPessoas.EmailsDocumentos.val(JSON.stringify(arrayEmailDocumentos));
    _grupoPessoas.LeituraDinamicaXML.val(JSON.stringify(arrayGrupoPessoasLeituraDinamicaXML));

    var autorizadosDownloadDFe = new Array();
    $.each(_autorizadoDownloadDFe.Pessoa.basicTable.BuscarRegistros(), function (i, autorizadoDownloadDFe) {
        autorizadosDownloadDFe.push({ Pessoa: autorizadoDownloadDFe });
    });
    _grupoPessoas.AutorizadosDownloadDFe.val(JSON.stringify(autorizadosDownloadDFe));
    _grupoPessoas.Comprovantes.val(obterListaTipoComprovanteSalvar());
    _grupoPessoas.ListaContatos.val(JSON.stringify(_grupoPessoas.Contatos.val()));
    _grupoPessoas.FormulasObservacaoNfe.val(obterListaObservacaoNfeFormulaSalvar());
    _grupoPessoas.NCMPalletsNFe.val(obterListaNCMPalletSalvar());
    _grupoPessoas.PerfilChamado.val(JSON.stringify(RetornarObjetoPesquisa(_grupoPessoasPerfilChamado)));
    _grupoPessoas.DadosAdicionais.val(JSON.stringify(RetornarObjetoPesquisa(_grupoPessoasAdicional)));
    _grupoPessoas.TipoIntegracoes.val(JSON.stringify(_gridTipoIntegracoes.BuscarRegistros()));

    var fornecedorDATA = {
        Fornecedor: RetornarObjetoPesquisa(_grupoPessoasFornecedor),
        TabelaMultiplosVencimentos: _grupoPessoasFornecedor != null ? _grupoPessoasFornecedor.TabelaMultiplosVencimentos.list : []
    };
    _grupoPessoas.Fornecedor.val(JSON.stringify(fornecedorDATA));
    atualizaCamposComprovante();
}

function AdicionarRaizCNPJClick(e, sender) {
    var valido = true;

    valido = _grupoPessoas.RaizCNPJ.val() != "";
    _grupoPessoas.RaizCNPJ.requiredClass("form-control ");

    if (valido) {
        var raizValida = false;
        var raizCompleta = _grupoPessoas.RaizCNPJ.val().replace(/[^0-9]/g, '');
        if (raizCompleta.length === 8)
            raizValida = true;

        if (!raizValida) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.RaizCNPJIncompleta, Localization.Resources.Pessoas.GrupoPessoas.RaizDigitadaNaoPossui8Caracteres);
            return;
        }
        else
            _grupoPessoas.RaizCNPJ.val(raizCompleta);

        var existe = false;
        $.each(_grupoPessoas.ListaRaizCNPJ.list, function (i, listaRaiz) {
            if (listaRaiz.RaizCNPJ.val == _grupoPessoas.RaizCNPJ.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.RaizCNPJJaExistente, Localization.Resources.Pessoas.GrupoPessoas.RaizXJaEstaCadastrada.format(_grupoPessoas.RaizCNPJ.val()));
            return;
        }

        var obj = new RaizCNPJMap();

        obj.Codigo.val = 0;
        obj.AdicionarPessoasMesmaRaiz.val = _grupoPessoas.AddPessoasComRaiz.val();
        obj.RaizCNPJ.val = _grupoPessoas.RaizCNPJ.val();
        _grupoPessoas.ListaRaizCNPJ.list.push(obj);

        RecarregarGridListaRaizCNPJ();
        limparCamposListaRaiz();
        $("#" + _grupoPessoas.RaizCNPJ.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        _grupoPessoas.RaizCNPJ.requiredClass("form-control is-invalid");
    }
}

function RecarregarGridListaRaizCNPJ() {
    var data = new Array();

    $.each(_grupoPessoas.ListaRaizCNPJ.list, function (i, listaRaizCNPJ) {
        var listaRaizCNPJGrid = new Object();

        listaRaizCNPJGrid.Codigo = listaRaizCNPJ.Codigo.val;
        listaRaizCNPJGrid.RaizCNPJ = listaRaizCNPJ.RaizCNPJ.val;
        listaRaizCNPJGrid.AdicionarPessoasMesmaRaiz = listaRaizCNPJ.AdicionarPessoasMesmaRaiz.val;

        data.push(listaRaizCNPJGrid);
    });

    _gridListaRaizCNPJ.CarregarGrid(data);
}

function ExcluirRaizCNPJ(data) {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Confirmacao, Localization.Resources.Pessoas.GrupoPessoas.RealmenteDesejaExcluirRaizX.format(data.RaizCNPJ), function () {
        $.each(_grupoPessoas.ListaRaizCNPJ.list, function (i, listaRaiz) {
            if (data.RaizCNPJ == listaRaiz.RaizCNPJ.val) {
                _grupoPessoas.ListaRaizCNPJ.list.splice(i, 1);
                return false;
            }
        });

        RecarregarGridListaRaizCNPJ();
    });
}

function BloquearClick() {
    AbrirTelaBloqueioGrupoPessoasClick();
}

function DesbloquearClick() {
    DesbloquearGrupoPessoasClick();
}

function adicionarClick(e, sender) {
    if (!validarCamposObrigatoriosGrupoPessoasAdicional()) {
        return false;
    }
    preencherListasSelecao();
    $("#myTab a:first").tab("show");
    Salvar(_grupoPessoas, "GrupoPessoas/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.CadastradoSucesso);

                enviarArquivosAnexados(arg.Data.Codigo);
                _gridGrupoPessoas.CarregarGrid();
                limparCamposGrupoPessoas();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, arg.Msg);
            }
        } else {
            resetarTabs();
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!validarCamposObrigatoriosGrupoPessoasAdicional()) {
        return false;
    }
    preencherListasSelecao();
    $("#myTab a:first").tab("show");
    Salvar(_grupoPessoas, "GrupoPessoas/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.AtualizadoSucesso);
                _gridGrupoPessoas.CarregarGrid();
                limparCamposGrupoPessoas();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Confirmacao, Localization.Resources.Pessoas.GrupoPessoas.RealmenteDesejaExcluirGrupoPessoasX.format(_grupoPessoas.Descricao.val()), function () {
        ExcluirPorCodigo(_grupoPessoas, "GrupoPessoas/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.ExcluidoSucesso);
                    _gridGrupoPessoas.CarregarGrid();
                    limparCamposGrupoPessoas();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposGrupoPessoas();
}

//*******MÉTODOS*******

function buscarGrupoPessoass() {
    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: editarGrupoPessoas, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoPessoas = new GridView(_pesquisaGrupoPessoas.Pesquisar.idGrid, "GrupoPessoas/Pesquisa", _pesquisaGrupoPessoas, menuOpcoes, null);
    _gridGrupoPessoas.CarregarGrid();
}

function editarGrupoPessoas(grupoPessoasGrid) {
    limparCamposGrupoPessoas();
    _grupoPessoas.Codigo.val(grupoPessoasGrid.Codigo);
    BuscarPorCodigo(_grupoPessoas, "GrupoPessoas/BuscarPorCodigo", function (arg) {
        recarregarGridClientes();
        RecarregarGridListaRaizCNPJ();
        recarregarGridModeloReboque();
        LimparCampos(_grupoPessoasOcorrencia);
        RecarregarGridAutorizadoDownloadDFe();
        RecarregarGridContato();
        recarregarGridGrupoPessoasVendedores();
        recarregarGridGrupoPessoasMensagemAlerta();
        RecarregarGridObservacaoCTe();
        preencherCampoObservacaoNfe();
        preencherCamposValidacaoNFe();
        recarregarGridComprovante();
        preencherCamposComprovante();

        _configuracaoEmissaoCTe.SetarValores(arg.Data.ConfiguracaoEmissaoCTe);
        _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);
        _configuracaoFatura.SetarValores(arg.Data.ConfiguracaoFatura);

        _pesquisaGrupoPessoas.ExibirFiltros.visibleFade(false);

        _grupoPessoasCRUD.Atualizar.visible(true);
        _grupoPessoasCRUD.Cancelar.visible(true);
        _grupoPessoasCRUD.Excluir.visible(true);
        _grupoPessoasCRUD.Adicionar.visible(false);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermitirBloquearOuDesbloquearGrupoDePessoas, _PermissoesPersonalizadas)) {
            _grupoPessoasCRUD.Bloquear.visible(!arg.Data.Bloqueado);
            _grupoPessoasCRUD.Desbloquear.visible(arg.Data.Bloqueado);
        } else {
            _grupoPessoasCRUD.Bloquear.visible(false);
            _grupoPessoasCRUD.Desbloquear.visible(false);
        }

        if (arg.Data.Bloqueado) {
            $("#txtMotivoBloqueio").text(arg.Data.MotivoBloqueio);
            $("#divMotivoBloqueioGrupoPessoas").removeClass("d-none");
        }

        arrayOcorrencias = arg.Data.Ocorrencias;
        arrayEmailDocumentos = arg.Data.EmailDocumentos;
        arrayGrupoPessoasLeituraDinamicaXML = arg.Data.LeituraDinamicaXML;
        RecarregarGridOcorrencias();
        RecarregarGridEmailDocumentos();
        RecarregarGridGrupoPessoasLeituraDinamicaXML();

        _clienteGrupo.ClientesComRaiz.visible(true);
        ValidarPermissaoPersonalizada();

        if (arg.Data.DadosAdicionais != null)
            PreencherObjetoKnout(_grupoPessoasAdicional, { Data: arg.Data.DadosAdicionais });
        if (arg.Data.PerfilChamado !== null && arg.Data.PerfilChamado !== undefined)
            PreencherObjetoKnout(_grupoPessoasPerfilChamado, { Data: arg.Data.PerfilChamado });
        RecarregarGridTabelaGrupoPessoasPerfilChamado();
        _anexo.Anexos.val(arg.Data.Anexos);
        if (arg.Data.Fornecedor != null) {
            PreencherDadosGrupoPessoasFornecedor(arg.Data.Fornecedor);
            recarregarGridTabelaMultiplosVencimentos();
        }
        preencherLogoGrupoPessoas(arg.Data.LogoGrupoPessoas);
        RecarregarTipoIntegracoes();
    }, null);
}

function limparCamposGrupoPessoas() {
    $("#divMotivoBloqueioGrupoPessoas").addClass("d-none");

    resetarTabs();

    _grupoPessoasCRUD.Atualizar.visible(false);
    _grupoPessoasCRUD.Cancelar.visible(false);
    _grupoPessoasCRUD.Excluir.visible(false);
    _grupoPessoasCRUD.Adicionar.visible(true);
    _grupoPessoasCRUD.Bloquear.visible(false);
    _grupoPessoasCRUD.Desbloquear.visible(false);
    _grupoPessoasOcorrencia.Adicionar.visible(true);
    _grupoPessoasOcorrencia.Excluir.visible(false);
    _grupoPessoasOcorrencia.Atualizar.visible(false);

    LimparCampos(_grupoPessoasOcorrencia);
    arrayOcorrencias = []
    RecarregarGridOcorrencias();
    arrayEmailDocumentos = [];
    RecarregarGridEmailDocumentos();
    arrayGrupoPessoasLeituraDinamicaXML = [];
    RecarregarGridGrupoPessoasLeituraDinamicaXML();

    _grupoPessoas.Clientes.list = new Array();
    LimparCampos(_grupoPessoas);

    _configuracaoEmissaoCTe.Limpar();
    _configuracaoLayoutEDI.Limpar();
    _configuracaoFatura.Limpar();
    limparCamposModeloReboque();
    LimparCamposAutorizadoDownloadDFe();
    limparCamposClientes();
    limparCamposValidacaoNFe();
    LimparCamposContato();
    LimparCamposGrupoPessoasVendedores();
    LimparCamposGrupoPessoasMensagemAlerta();
    limparCamposObservacaoNfe();
    limparCamposComprovante();
    _grupoPessoas.ListaRaizCNPJ.list = new Array();
    _grupoPessoas.ListaVendedores.list = new Array();
    _grupoPessoas.ListaMensagemAlerta.list = new Array();

    RecarregarGridListaRaizCNPJ();
    recarregarGridClientes();
    recarregarGridModeloReboque();
    RecarregarGridAutorizadoDownloadDFe();
    RecarregarGridContato();
    recarregarGridGrupoPessoasVendedores();
    recarregarGridGrupoPessoasMensagemAlerta();
    _clienteGrupo.ClientesComRaiz.visible(false);
    ValidarPermissaoPersonalizada();
    LimparTabelaGrupoPessoasPerfilChamado();
    LimparCampos(_grupoPessoasPerfilChamado);
    LimparCamposObservacaoCTe();
    limparCamposConfiguracaoMultiEmbarcador();
    RecarregarGridObservacaoCTe();
    limparCamposAnexo();
    limparCamposGrupoPessoasAdicional();
    LimparCamposGrupoPessoasFornecedor();
    limparConfiguracaoLogo();
    LimparCamposTipoIntegracoes()
}

function limparCamposListaRaiz() {
    _grupoPessoas.AddPessoasComRaiz.val(true);
    _grupoPessoas.RaizCNPJ.val("");
    _grupoPessoas.RaizCNPJ.requiredClass("form-control ");
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function ValidarPermissaoPersonalizada() {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermiteAlterarApenasObservacoes, _PermissoesPersonalizadas)) {
        $("#liCadastroLayoutEDI").hide();
        $("#liEmissao").hide();
        $("#liTabAutorizadoDownloadDFe").hide();

        SetarEnableCamposKnockout(_grupoPessoas, false);

        SetarEnableCamposKnockout(_configuracaoEmissaoCTe, false);

        SetarEnableCamposKnockout(_configuracaoLayoutEDI, false);
        //_configuracaoLayoutEDI.HabilitarDesabilitarLayoutEDI(false);

        SetarEnableCamposKnockout(_configuracaoFatura, false);
        DestabilitarHabilitarCamposFatura(false);

        SetarEnableCamposKnockout(_modeloVeicularEmbarcador, false);
        SetarEnableCamposKnockout(_modeloReboque, false);

        SetarEnableCamposKnockout(_grupoPessoaObservacaoNfe, true);
        SetarEnableCamposKnockout(_grupoPessoasVendedores, false);
        SetarEnableCamposKnockout(_grupoPessoasMensagemAlerta, true);
        SetarEnableCamposKnockout(_grupoPessoaImportacaoNFe, true);
        SetarEnableCamposKnockout(_contato, false);
        SetarEnableCamposKnockout(_clienteGrupo, false);
        SetarEnableCamposKnockout(_bloqueioGrupoPessoas, false);

        SetarEnableCamposKnockout(_documentoDestinado, false);
        SetarEnableCamposKnockout(_validacaoNFe, true);
    }
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente, _PermissoesPersonalizadas))
        SetarEnableCamposKnockout(_clienteGrupo, false);
}

function DestabilitarHabilitarCamposFatura(enable) {
    _configuracaoFatura.Configuracao.DiaSemana.enable(enable);
    _configuracaoFatura.Configuracao.DiaMes.enable(enable);
    _configuracaoFatura.Configuracao.PermiteFinalSemana.enable(enable);
    _configuracaoFatura.Configuracao.ExigeCanhotoFisico.enable(enable);
    _configuracaoFatura.Configuracao.NaoGerarFaturaAteReceberCanhotos.enable(enable);
    _configuracaoFatura.Configuracao.Banco.enable(enable);
    _configuracaoFatura.Configuracao.Agencia.enable(enable);
    _configuracaoFatura.Configuracao.Digito.enable(enable);
    _configuracaoFatura.Configuracao.NumeroConta.enable(enable);
    _configuracaoFatura.Configuracao.TipoConta.enable(enable);
    _configuracaoFatura.Configuracao.TomadorFatura.enable(enable);
    _configuracaoFatura.Configuracao.ObservacaoFatura.enable(true);
    _configuracaoFatura.Configuracao.TipoPrazoFaturamento.enable(enable);
    _configuracaoFatura.Configuracao.DiasDePrazoFatura.enable(enable);
    _configuracaoFatura.Configuracao.FormaPagamento.enable(enable);
    _configuracaoFatura.Configuracao.GerarTituloPorDocumentoFiscal.enable(enable);
    _configuracaoFatura.Configuracao.GerarTituloAutomaticamente.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturaAutomaticaCte.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturamentoAVista.enable(enable);
    _configuracaoFatura.Configuracao.SomenteOcorrenciasFinalizadoras.enable(enable);
    _configuracaoFatura.Configuracao.FaturarSomenteOcorrenciasFinalizadoras.enable(enable);
    _configuracaoFatura.Configuracao.ArmazenaCanhotoFisicoCTe.enable(enable);
    _configuracaoFatura.Configuracao.AssuntoEmailFatura.enable(true);
    _configuracaoFatura.Configuracao.CorpoEmailFatura.enable(true);
    _configuracaoFatura.Configuracao.GerarBoletoAutomaticamente.enable(enable);
    _configuracaoFatura.Configuracao.EnviarArquivosDescompactados.enable(enable);
    _configuracaoFatura.Configuracao.TipoEnvioFatura.enable(enable);
    _configuracaoFatura.Configuracao.TipoAgrupamentoFatura.enable(enable);
    _configuracaoFatura.Configuracao.DiasSemanaFatura.enable(enable);
    _configuracaoFatura.Configuracao.DiasMesFatura.enable(enable);
    _configuracaoFatura.Configuracao.GerarFaturaPorCte.enable(enable);
}