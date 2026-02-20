/// <reference path="CTeAverbacao.js" />
/// <reference path="MICDTA.js" />
/// <autosync enabled="true" />
/// <reference path="../../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.js" />
/// <reference path="../../../../wwwroot/js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../Global/SignalR/SignalR.js" />
/// <reference path="../../../Configuracao/EmissaoCTe/EmissaoCTe.js" />
/// <reference path="../../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../../CTe/CTe/ComponentePrestacaoServico.js" />
/// <reference path="../../../CTe/CTe/CTe.js" />
/// <reference path="../../../CTe/CTe/Documento.js" />
/// <reference path="../../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../../Enumeradores/EnumTipoChavePix.js" />
/// <reference path="../../../CTe/CTe/DocumentoTransporteAnteriorEletronico.js" />
/// <reference path="../../../CTe/CTe/DocumentoTransporteAnteriorPapel.js" />
/// <reference path="../../../CTe/CTe/Duplicata.js" />
/// <reference path="../../../CTe/CTe/DuplicataAutomatica.js" />
/// <reference path="../../../CTe/CTe/InformacaoCarga.js" />
/// <reference path="../../../CTe/CTe/Motorista.js" />
/// <reference path="../../../CTe/CTe/Observacao.js" />
/// <reference path="../../../CTe/CTe/ObservacaoGeral.js" />
/// <reference path="../../../CTe/CTe/Participante.js" />
/// <reference path="../../../CTe/CTe/ProdutoPerigoso.js" />
/// <reference path="../../../CTe/CTe/QuantidadeCarga.js" />
/// <reference path="../../../CTe/CTe/Rodoviario.js" />
/// <reference path="../../../CTe/CTe/Seguro.js" />
/// <reference path="../../../CTe/CTe/TotalServico.js" />
/// <reference path="../../../CTe/CTe/Veiculo.js" />
/// <reference path="../Documentos/ValePedagio.js" />s
/// <reference path="../DadosEmissao/Configuracao.js" />
/// <reference path="../DadosEmissao/DadosEmissao.js" />
/// <reference path="../DadosEmissao/Geral.js" />
/// <reference path="../DadosEmissao/Lacre.js" />
/// <reference path="../DadosEmissao/LocaisPrestacao.js" />
/// <reference path="../DadosEmissao/Observacao.js" />
/// <reference path="../DadosEmissao/Passagem.js" />
/// <reference path="../DadosEmissao/Percurso.js" />
/// <reference path="../DadosEmissao/Rota.js" />
/// <reference path="../DadosEmissao/Seguro.js" />
/// <reference path="../DadosTransporte/DadosTransporte.js" />
/// <reference path="../DadosTransporte/Motorista.js" />
/// <reference path="../DadosTransporte/Tipo.js" />
/// <reference path="../DadosTransporte/Transportador.js" />
/// <reference path="MDFe.js" />
/// <reference path="NFS.js" />
/// <reference path="PreCTe.js" />
/// <reference path="../DocumentosEmissao/CargaPedidoDocumentoCTe.js" />
/// <reference path="../DocumentosEmissao/ConsultaReceita.js" />
/// <reference path="../DocumentosEmissao/CTe.js" />
/// <reference path="../DocumentosEmissao/Documentos.js" />
/// <reference path="../DocumentosEmissao/DropZone.js" />
/// <reference path="../DocumentosEmissao/EtapaDocumentos.js" />
/// <reference path="../DocumentosEmissao/NotaFiscal.js" />
/// <reference path="../Frete/Complemento.js" />
/// <reference path="../Frete/Componente.js" />
/// <reference path="../Frete/EtapaFrete.js" />
/// <reference path="../Frete/Frete.js" />
/// <reference path="../Frete/SemTabela.js" />
/// <reference path="../Frete/TabelaCliente.js" />
/// <reference path="../Frete/TabelaComissao.js" />
/// <reference path="../Frete/TabelaRota.js" />
/// <reference path="../Frete/TabelaSubContratacao.js" />
/// <reference path="../Frete/TabelaTerceiros.js" />
/// <reference path="../Impressao/Impressao.js" />
/// <reference path="../Integracao/Integracao.js" />
/// <reference path="../Integracao/IntegracaoCarga.js" />
/// <reference path="../Integracao/IntegracaoCTe.js" />
/// <reference path="../Integracao/IntegracaoEDI.js" />
/// <reference path="../Terceiro/ContratoFrete.js" />
/// <reference path="../DadosCarga/SignalR.js" />
/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="../DadosCarga/DataCarregamento.js" />
/// <reference path="../DadosCarga/Leilao.js" />
/// <reference path="../DadosCarga/Operador.js" />
/// <reference path="../../../Consultas/Tranportador.js" />
/// <reference path="../../../Consultas/Localidade.js" />
/// <reference path="../../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/Motorista.js" />
/// <reference path="../../../Consultas/Veiculo.js" />
/// <reference path="../../../Consultas/GrupoPessoa.js" />
/// <reference path="../../../Consultas/TipoOperacao.js" />
/// <reference path="../../../Consultas/Filial.js" />
/// <reference path="../../../Consultas/Cliente.js" />
/// <reference path="../../../Consultas/Usuario.js" />
/// <reference path="../../../Consultas/CIOT.js" />
/// <reference path="../../../Consultas/TipoCarga.js" />
/// <reference path="../../../Consultas/RotaFrete.js" />
/// <reference path="../../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../Enumeradores/EnumTipoFreteEscolhido.js" />
/// <reference path="../../../Enumeradores/EnumTipoOperacaoEmissao.js" />
/// <reference path="../../../Enumeradores/EnumMotivoPendenciaFrete.js" />
/// <reference path="../../../Enumeradores/EnumTipoContratacaoCarga.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoContratoFrete.js" />
/// <reference path="../../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../../Enumeradores/EnumTipoEmissaoCTeParticipantes.js" />
/// <reference path="../../../Enumeradores/EnumSituacaoRetornoDadosFrete.js" />
/// <reference path="DropZone.js" />

//*******MAPEAMENTO*******

var _HTMLCTE;
var _gridCargaCTe;
var _gridDocumentoParaMessiaoNFSManual;
var _gridCargaPreCTe;
var _gridCargaCTeAverbacao;
var _gridCargaNFe;
var _gridCargaMICDTA;
var _gridCargaIntegracaoValePedagio;
var _gridCargaIntegracaoValePedagioPreCTe;
var _gridCargaIntegracaoDespesa;
var _gridCargaGuiasRecolhimento;
var _gridCargaCIOT;
var _gridCargaCIOTPreCTe;
var _cargaCTe;
var _anexoEtapaCTe;
var _gridAnexoEtapaCTe;
var _PermissoesEdicaoDoCTe;
var _informarContainer;
var _alterarObservacao;
var _cartaCorrecaoCTe = null;
var _protocoloIntegracao;

var CargaCTEs = function () {
    let self = this;

    this.ValePedagio = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ValePedagio });
    this.CtesSendoGerados = PropertyEntity({ text: Localization.Resources.Cargas.Carga.AguardeOsCtesDaTransportadoraEstaoSendoGerados });
    this.Averbacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Averbacao });
    this.MICDTA = PropertyEntity({ text: "MIC/DTA" });
    this.NFe = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NFe });
    this.DocumentosNfsManual = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DocumentosParaNfsManual });
    this.Ciot = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CIOT });
    this.Despesas = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Despesas });
    this.GuiaRecolhimento = PropertyEntity({ text: "Guia de Recolhimento" });
    this.CtesSendoEmitidos = PropertyEntity({ text: Localization.Resources.Cargas.Carga.OsCtesEstaoSendoEmitidos });
    this.Download = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Download });
    this.Anexos = PropertyEntity({ eventClick: MostrarModalAnexoEtapaCTe, type: types.event, text: Localization.Resources.Gerais.Geral.Anexos, visible: ko.observable(true) });

    this.CTesSubContratacaoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.CTesSemSubContratacaoFilialEmissora = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.CTesFactura = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaMercosul = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaPortoPortoTimelineHabilitado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaPortaPortaTimelineHabilitado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaSVM = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusCTe.TODOS), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoDoCte.getFieldDescription(), options: EnumStatusCTe.obterOpcoes(), def: EnumStatusCTe.TODOS });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDaNotaFiscal.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroDocumento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoDocumento.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.QuantidadeDocumentosEmitidos = PropertyEntity({ val: ko.observable(""), def: "" });
    this.QuantidadeDocumentosTotal = PropertyEntity({ val: ko.observable(""), def: "" });
    this.EmitindoCTes = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ErroEmissaoCTes = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(false) });
    this.SucessoEmissaoCTes = PropertyEntity({ val: ko.observable(Localization.Resources.Cargas.Carga.OsCtesForamEnviadosParaEmissaoComSucesso), def: Localization.Resources.Cargas.Carga.OsCtesForamEnviadosParaEmissaoComSucesso, visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.EmitirNovamenteRejeitados = PropertyEntity({
        eventClick: reemitirCTesRejeitadosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReemitirCtesRejeitados, idGrid: guid(), visible: ko.observable(false)
    });
    this.SincronizarTodosDocumento = PropertyEntity({
        eventClick: sincronizarTodosDocumentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.SincronizarTodosDocumento, idGrid: guid(), visible: ko.observable(false)
    });
    this.EmitirNovamenteAverbacoesCTe = PropertyEntity({
        eventClick: reaverbarRejeitadosClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReaverbarRejeitados, idGrid: guid(), visible: ko.observable(false)
    });
    this.EmitirNovamenteAverbacoesPendentesCTe = PropertyEntity({
        eventClick: reaverbarPendentesClick, type: types.event, text: Localization.Resources.Cargas.Carga.ReaverbarPendentes, idGrid: guid(), visible: ko.observable(false)
    });
    this.DownloadLoteXMLCTe = PropertyEntity({
        eventClick: DownloadLoteXMLCTeClick, type: types.event, text: Localization.Resources.Cargas.Carga.XmlDosCtesNfses, idGrid: guid(), visible: ko.observable(false)
    });
    this.DownloadLoteDACTE = PropertyEntity({
        eventClick: DownloadLoteDACTEClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfDosCtesNfses, idGrid: guid(), visible: ko.observable(false)
    });
    this.DownloadLoteDocumentos = PropertyEntity({
        eventClick: DownloadLoteDocumentosCTeClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfTodosDocumentos, idGrid: guid(), visible: ko.observable(false)
    });
    this.ConfirmarEmissaoCTes = PropertyEntity({
        eventClick: confirmarEmissaoCTeClick, type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarEmissaoDosCtes, visible: ko.observable(true), enable: ko.observable(true)
    });
    this.LiberarSemTodosPreCTes = PropertyEntity({
        eventClick: liberarSemTodosPreCTesClick, type: types.event, text: Localization.Resources.Cargas.Carga.LiberarCargaSemReceberTodosOsCtes, visible: ko.observable(false), enable: ko.observable(true)
    });

    this.LiberarCargaComAverbacoesRejeitados = PropertyEntity({
        eventClick: LiberarCargaComAverbacoesRejeitadosClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.Carga.LiberarCargaMesmoComAsAverbacoesRejeitadas), visible: ko.observable(false), enable: ko.observable(false)
    });

    this.CTe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visible: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.Carga.Cte) });
    this.PreCTe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visible: ko.observable(false) });
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.DownloadTodosPreCte = PropertyEntity({
        eventClick: DownloadPreCtesClick, type: types.event, text: "Download Documentos", idGrid: guid(), visible: ko.observable(false)
    });

    this.DownloadPreviaCusto = PropertyEntity({
        eventClick: DownloadPreviaCustoClick, type: types.event, text: "Download Prévia Custo", idGrid: guid(), visible: ko.observable(false)
    });

    this.NotFis = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), visible: ko.observable(false) });

    this.PesquisarPreCTe = PropertyEntity({
        eventClick: function (e) {
            _gridCargaPreCTe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridPreCTe: guid(), visible: ko.observable(true)
    });


    //****AVERBACAO****
    this.Apolice = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Apolice.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.SituacaoAverbacao = PropertyEntity({ val: ko.observable(EnumStatusAverbacaoCTe.Todos), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoDaAverbacao.getFieldDescription(), options: EnumStatusAverbacaoCTe.obterOpcoesPesquisa(), def: EnumStatusAverbacaoCTe.Todos });
    this.PesquisarCTeAverbacoes = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    //****MIC DTA****    
    this.NumeroMICDTA = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.string });
    this.PesquisarMICDTA = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMICDTA.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });

    //****NFE****    
    this.PesquisarNFe = PropertyEntity({
        eventClick: function (e) {
            _gridCargaNFe.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });
    this.DownloadLoteDANFE = PropertyEntity({
        eventClick: DownloadLoteDANFEClick, type: types.event, text: Localization.Resources.Cargas.Carga.PdfDasNfes, idGrid: guid(), visible: ko.observable(true)
    });

    //****DOC para Emissao NFS Manual****
    this.NumeroDoc = PropertyEntity({ text: Localization.Resources.Cargas.Carga.NumeroDoc.getFieldDescription(), val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.PesquisarDocumentosParaEmissaoNFSManual = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoParaMessiaoNFSManual.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true)
    });


    //****VALE PEDÁGIO****
    this.SituacaoIntegracaoValePedagio = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoIntegracao.getFieldDescription(), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), def: "" });
    this.PesquisarIntegracaoValePedagio = PropertyEntity({ eventClick: function (e) { _gridCargaIntegracaoValePedagio.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarComProblemaValePedagio = PropertyEntity({ eventClick: LiberarComProblemaValePedagioClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });
    this.LiberarSemValePedagio = PropertyEntity({ eventClick: LiberarComProblemaValePedagioClickSemFalha, enable: ko.observable(true), type: types.event, text: "Liberar Sem Vale Pedagio", visible: ko.observable(false) });

    //****VALE PEDÁGIO PRE CTE****
    this.SituacaoIntegracaoValePedagioPreCte = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), text: Localization.Resources.Cargas.Carga.SituacaoIntegracao.getFieldDescription(), options: EnumSituacaoIntegracaoCarga.obterOpcoesPesquisa(), def: "" });
    this.PesquisarIntegracaoValePedagioPreCte = PropertyEntity({ eventClick: function (e) { _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarComProblemaValePedagioPreCte = PropertyEntity({ eventClick: LiberarComProblemaValePedagioClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });

    //****DESPESAS****
    this.PesquisarIntegracaoDespesa = PropertyEntity({ eventClick: function (e) { _gridCargaIntegracaoDespesa.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarComProblemaPagamentoMotorista = PropertyEntity({ eventClick: liberarComProblemaPagamentoMotoristaClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });

    //****GUIA RECOLHIMENTO****
    this.PesquisarGuiaRecolhimento = PropertyEntity({ eventClick: function (e) { _gridCargaGuiasRecolhimento.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ReenviarTodosRejeitados = PropertyEntity({ eventClick: reenviarTodasGuiasRejeitados, enable: ko.observable(true), type: types.event, text: "Reenviar todos rejeitados", visible: ko.observable(true) });
    this.LiberarSemGNRE = PropertyEntity({ eventClick: liberarSemGNRE, enable: ko.observable(true), type: types.event, text: "Liberar sem GNRE", visible: ko.observable(obterVisibilidadeCampo()) });
    this.NroCte = PropertyEntity({ enable: ko.observable(true), text: "Numero CT-e", visible: ko.observable(true) });

    //****CIOT****
    this.EntidadeCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Buscar CIOT:"), idBtnSearch: guid() });
    this.CIOT = PropertyEntity({ text: "CIOT:", maxlength: 12, required: false, cssClass: ko.observable("col col-3"), visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorAntigoCIOT = PropertyEntity({ val: ko.observable("") });

    this.ValorFrete = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Frete: ", required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(_cargaAtual.VeiculoPropriedadeTerceiro.val()) });
    this.ValorAdiantamento = PropertyEntity({ getType: typesKnockout.decimal, text: "Valor Adiantamento: ", required: false, maxlength: 12, cssClass: ko.observable("col col-3"), enable: ko.observable(true), visible: ko.observable(_cargaAtual.VeiculoPropriedadeTerceiro.val()) });
    this.FormaPagamento = PropertyEntity({ text: "Forma de Pagamento:", options: EnumFormaPagamentoCIOT.obterOpcoes(), val: ko.observable(EnumFormaPagamentoCIOT.NaoSelecionado), def: EnumFormaPagamentoCIOT.NaoSelecionado, visible: ko.observable(_cargaAtual.VeiculoPropriedadeTerceiro.val()) });
    this.TipoPagamento = PropertyEntity({ text: "Tipo :", options: EnumTipoPagamentoMDFe.ObterOpcoes(), val: ko.observable(EnumTipoPagamentoMDFe.NaoSelecionado), def: EnumTipoPagamentoMDFe.NaoSelecionado, visible: ko.observable(_cargaAtual.VeiculoPropriedadeTerceiro.val()) });
    this.TipoChavePIX = PropertyEntity({ text: "Tipo Chave PIX :", options: EnumTipoChavePix.obterOpcoesComVazio(), val: ko.observable(EnumTipoChavePix.Nenhum), def: EnumTipoChavePix.Nenhum, visible: ko.observable(false) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: "Data Vencimento: ", required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(_cargaAtual.VeiculoPropriedadeTerceiro.val()) });
    this.CNPJInstituicaoPagamento = PropertyEntity({ text: ko.observable("CNPJ Instituição de Pagamento: "), maxlength: 20, required: ko.observable(false), getType: typesKnockout.cnpj, visible: ko.observable(false) });
    this.ContaCIOT = PropertyEntity({ text: ko.observable("Banco: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });
    this.AgenciaCIOT = PropertyEntity({ text: ko.observable("Agencia: "), required: ko.observable(false), maxlength: 20, visible: ko.observable(false) });

    this.ChavePIXCPFCNPJ = PropertyEntity({ text: ko.observable("Chave PIX (CPF/CNPJ): "), maxlength: 18, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.cpfCnpj });
    this.ChavePIXEmail = PropertyEntity({ text: ko.observable("Chave PIX (E-mail): "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.email });
    this.ChavePIXCelular = PropertyEntity({ text: ko.observable("Chave PIX (Celular): "), maxlength: 15, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.phone });
    this.ChavePIXAleatoria = PropertyEntity({ text: ko.observable("Chave PIX (Aleatória): "), maxlength: 200, required: ko.observable(false), visible: ko.observable(false), getType: typesKnockout.text });

    this.PesquisarCIOT = PropertyEntity({ eventClick: function (e) { _gridCargaCIOT.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarCargaComCIOTRejeitados = PropertyEntity({ eventClick: LiberarCargaComCIOTRejeitadosClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });
    this.GerarCIOTManualmente = PropertyEntity({ eventClick: GerarCIOTManualmente, enable: ko.observable(true), type: types.event, text: "Salvar CIOT", visible: ko.observable(true) });

    //****CIOT PRE CTE****
    this.EntidadeCIOTPreCte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("CIOT:"), idBtnSearch: guid() });

    this.PesquisarCIOTPreCte = PropertyEntity({ eventClick: function (e) { _gridCargaCIOTPreCTe.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarCargaComCIOTRejeitadosPreCte = PropertyEntity({ eventClick: LiberarCargaComCIOTRejeitadosClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });

    this.TipoChavePIX.val.subscribe(function (tipoPix) {
        ObterCampoPixCte(self, self.TipoPagamento.val(), tipoPix, null);
    });

    this.TipoPagamento.val.subscribe(function (tipoPagamento) {
        const tipoBanco = tipoPagamento === EnumTipoPagamentoMDFe.Banco
        const tipoIpef = tipoPagamento === EnumTipoPagamentoMDFe.Ipef

        self.ContaCIOT.visible(tipoBanco);
        self.ContaCIOT.required(tipoBanco);
        self.AgenciaCIOT.visible(tipoBanco);
        self.AgenciaCIOT.required(tipoBanco);

        self.CNPJInstituicaoPagamento.visible(tipoIpef);
        self.CNPJInstituicaoPagamento.required(tipoIpef);

        ObterCampoPixCte(self, tipoPagamento, self.TipoChavePIX.val(), null);
    });

    this.FormaPagamento.val.subscribe(function (novoValor) {
        if (novoValor === EnumFormaPagamentoCIOT.AVista) {
            _cargaCTe.DataVencimento.visible(false);
            _cargaCTe.ValorAdiantamento.val("");
            _cargaCTe.ValorAdiantamento.visible(false);
            _cargaCTe.DataVencimento.required(false);
            _cargaCTe.DataVencimento.val("");
        } else if (novoValor === EnumFormaPagamentoCIOT.APrazo) {
            _cargaCTe.DataVencimento.visible(true);
            _cargaCTe.DataVencimento.required(true);
            _cargaCTe.ValorAdiantamento.visible(true);
        } else {
            _cargaCTe.DataVencimento.visible(false);
            _cargaCTe.DataVencimento.required(false);
            _cargaCTe.DataVencimento.required(false);
            _cargaCTe.ValorAdiantamento.visible(false);
        }
    });

    this.PesquisarCIOT = PropertyEntity({ eventClick: function (e) { _gridCargaCIOT.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarCargaComCIOTRejeitados = PropertyEntity({ eventClick: LiberarCargaComCIOTRejeitadosClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });
    this.GerarCIOTManualmente = PropertyEntity({ eventClick: GerarCIOTManualmente, enable: ko.observable(true), type: types.event, text: "Gerar CIOT", visible: ko.observable(true) });

    //****CIOT PRE CTE****
    this.EntidadeCIOTPreCte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("CIOT:"), idBtnSearch: guid() });

    this.PesquisarCIOTPreCte = PropertyEntity({ eventClick: function (e) { _gridCargaCIOTPreCTe.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), idGridNFS: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.LiberarCargaComCIOTRejeitadosPreCte = PropertyEntity({ eventClick: LiberarCargaComCIOTRejeitadosClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.Carga.AvancarEtapa, visible: ko.observable(true) });
};

var InformarContainer = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Container.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.LacreContainerUm = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerUm.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.LacreContainerDois = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerDois.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });
    this.LacreContainerTres = PropertyEntity({ text: Localization.Resources.Cargas.Carga.LacreContainerTres.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 100 });

    this.Atualizar = PropertyEntity({ eventClick: atualizarContainerCTeClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(true) });
};

var AlterarObservacao = function () {
    this.CodigoCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaCTe = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), required: false, visible: ko.observable(true), maxlength: 1000 });

    this.Atualizar = PropertyEntity({ eventClick: atualizarObservacaoCTeClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(true) });
};

var ProtocoloIntegracao = function () {
    this.CodigoProtocolo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Protocolo.getFieldDescription(), val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: true });
};

//*******EVENTOS*******

function ObterCampoPixCte(objeto, tipoPagamento, tipoChavePix, valor) {
    objeto.TipoChavePIX.visible(false);
    objeto.ChavePIXCPFCNPJ.visible(false);
    objeto.ChavePIXCPFCNPJ.required(false);
    objeto.ChavePIXEmail.visible(false);
    objeto.ChavePIXEmail.required(false);
    objeto.ChavePIXCelular.visible(false);
    objeto.ChavePIXCelular.required(false);
    objeto.ChavePIXAleatoria.visible(false);
    objeto.ChavePIXAleatoria.required(false);

    if (tipoPagamento === EnumTipoPagamentoMDFe.PIX) {
        objeto.TipoChavePIX.visible(true);
        objeto.TipoChavePIX.val(tipoChavePix);

        switch (tipoChavePix) {
            case EnumTipoChavePix.CPFCNPJ:
                if (valor) objeto.ChavePIXCPFCNPJ.val(valor);
                objeto.ChavePIXCPFCNPJ.visible(true);
                objeto.ChavePIXCPFCNPJ.required(true);
                break;
            case EnumTipoChavePix.Email:
                if (valor) objeto.ChavePIXEmail.val(valor);
                objeto.ChavePIXEmail.visible(true);
                objeto.ChavePIXEmail.required(true);
                break;
            case EnumTipoChavePix.Celular:
                if (valor) objeto.ChavePIXCelular.val(valor);
                objeto.ChavePIXCelular.visible(true);
                objeto.ChavePIXCelular.required(true);
                break;
            case EnumTipoChavePix.Aleatoria:
                if (valor) objeto.ChavePIXAleatoria.val(valor);
                objeto.ChavePIXAleatoria.visible(true);
                objeto.ChavePIXAleatoria.required(true);
                break;
        }
    }
}
function GerarCIOTManualmente() {
    let chavePIX = "";
    switch (_cargaCTe.TipoChavePIX.val()) {
        case EnumTipoChavePix.CPFCNPJ:
            chavePIX = _cargaCTe.ChavePIXCPFCNPJ.val();
            break;
        case EnumTipoChavePix.Email:
            chavePIX = _cargaCTe.ChavePIXEmail.val();
            break;
        case EnumTipoChavePix.Celular:
            chavePIX = _cargaCTe.ChavePIXCelular.val();
            break;
        case EnumTipoChavePix.Aleatoria:
            chavePIX = _cargaCTe.ChavePIXAleatoria.val();
            break;
    }

    var data = {
        Carga: _cargaAtual.Codigo.val(),
        FormaPagamento: _cargaCTe.FormaPagamento.val(),
        TipoPagamento: _cargaCTe.TipoPagamento.val(),
        ValorAdiantamento: _cargaCTe.ValorAdiantamento.val(),
        ValorFrete: _cargaCTe.ValorFrete.val(),
        DataVencimento: _cargaCTe.DataVencimento.val(),
        CNPJInstituicaoPagamento: _cargaCTe.CNPJInstituicaoPagamento.val(),
        ContaCIOT: _cargaCTe.ContaCIOT.val(),
        AgenciaCIOT: _cargaCTe.AgenciaCIOT.val(),
        ChavePIXCIOT: chavePIX,
        TipoChavePIX: _cargaCTe.TipoChavePIX.val(),
        CIOT: _cargaCTe.CIOT.val(),
        CIOTAntigo: _cargaCTe.ValorAntigoCIOT.val(),
    }

    if (!ValidarCamposObrigatorios(_cargaCTe))
        return;

    const valorCIOT = _cargaCTe.CIOT.val();

    if (valorCIOT && valorCIOT.trim() && valorCIOT.length !== 12) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "O Campo CIOT deve conter 12 digitos");
        return _cargaCTe.CIOT.requiredClass("form-control  is-invalid");
    }

    exibirConfirmacao(Localization.Resources.Cargas.Carga.AvancarEtapa, "Deseja gerar ciot manualmente?", function () {
        executarReST("CargaCTe/GerarLiberacaoCIOT", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    CarregarGridCIOT();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function buscarCargasCTe(callback, menuOpcoes) {

    _cargaCTe.CargaMercosul.val(_cargaAtual.Mercosul.val());
    _cargaCTe.CargaPortoPortoTimelineHabilitado.val(_cargaAtual.CargaPortoPortoTimelineHabilitado.val());
    _cargaCTe.CargaPortaPortaTimelineHabilitado.val(_cargaAtual.CargaPortaPortaTimelineHabilitado.val());
    _cargaCTe.CargaSVM.val(_cargaAtual.CargaSVM.val());

    if (_cargaAtual.CargaTipoInformarDadosNotaCte.val()) {
        _gridCargaCTe = new GridView(_cargaCTe.Pesquisar.idGrid, "CargaCTe/ConsultarDocumentoCargaCTe", _cargaCTe, null);
        _gridCargaCTe.CarregarGrid(callback);
        $("#OpcoesPesquisaCte").hide();
    }
    else {
        var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: false };
        _gridCargaCTe = new GridView(_cargaCTe.Pesquisar.idGrid, "CargaCTe/ConsultarCargaCTe", _cargaCTe, menuOpcoes, null, null, null, null, null, null, null, editarColuna);
        _gridCargaCTe.CarregarGrid(callback);
    }
}

function EmitirCartaCorrecaoCTeClick(data) {
    if (_cartaCorrecaoCTe == null)
        _cartaCorrecaoCTe = new CartaCorrecaoCTe();

    _cartaCorrecaoCTe.Load(data.CodigoCTE);
}

function alterarObservacaoCTeClick(data) {
    LimparDadosAlterarObservacao();

    _alterarObservacao.CodigoCTe.val(data.CodigoCTE);
    _alterarObservacao.CodigoCargaCTe.val(data.Codigo);
    _alterarObservacao.Observacao.val(data.Observacao);

    Global.abrirModal("divModalAlterarObservacao");
}

function abrirAnexosContribuinteClick(data) {
    loadContribuinteAnexo();
    executarReST("CargaCTe/BuscarAnexosContribuinte", { CargaCTe: data.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _contribuinteAnexo.Anexos.val(retorno.Data.Anexos)
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 6000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

    _dadosAdicionaisContribuinteAnexo.CodigoCargaCTe.val(data.Codigo);
    _dadosAdicionaisContribuinteAnexo.PermiteGerenciarAnexos.val(data.SituacaoDocumentoTransportador != "Aprovado");
    AbrirModalAnexoContribuinteClick();
}

function aprovarDocumentoContribuinteClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteAprovarDocumentos, function () {
        executarReST("CargaCTe/AprovarDocumentoContribuinte", { Codigo: data.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosAprovadosComSucesso);
                    if (retorno.Data.TodosDocumentosAprovados)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.TodosDocumentosForamAprovadosComSucesso);

                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 6000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function reprovarDocumentoContribuinteClick(data) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteReprovarDocumentos, function () {
        executarReST("CargaCTe/ReprovarDocumentoContribuinte", { Codigo: data.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentosReprovadosComSucesso);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 6000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function informarContainerCTeClick(data) {
    LimparDadosInformacaoContainer();

    _informarContainer.CodigoCTe.val(data.CodigoCTE);
    _informarContainer.CodigoCargaCTe.val(data.Codigo);

    Global.abrirModal("divModalInformarContainer");
}

function protocoloIntegracaoClick(data) {
    LimparDadosProtocoloIntegracao();

    _protocoloIntegracao.CodigoProtocolo.val(data.CodigoCTE);

    Global.abrirModal("divModalProtocoloIntegracao");
}

function atualizarObservacaoCTeClick(e, sender) {
    Salvar(e, "CargaCTe/AlterarObservacao", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("divModalAlterarObservacao");
                LimparDadosAlterarObservacao();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AlteracaoDaObservacaoRealizadaComSucesso);
                _gridCargaCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarContainerCTeClick(e, sender) {
    Salvar(e, "CargaCTe/InformarContainer", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("divModalInformarContainer");
                LimparDadosInformacaoContainer();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ContainerAlteradoComSucessoFavorAcompanheSuaCartaDeCorrecao);
                _gridCargaCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 200000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function desativarEditarGridCTe() {
    var editarColuna = { permite: false, callback: null, atualizarRow: false };
    _gridCargaCTe.SetarEditarColunas(editarColuna);
}

function detalhesCTeClick(e, sender) {
    var codigo = parseInt(e.CodigoCTE);
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
            var objetoCTe = ObterObjetoCTe(instancia);
            SalvarCTe(objetoCTe, e.Codigo, instancia);
        }
    }, permissoesSomenteLeituraCTe);
}

function editarCTeClick(e, sender) {

    var permissoesSomenteLeituraCTe = [EnumPermissoesEdicaoCTe.Nenhuma];

    var possuiPermissao = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_EditarCTe, _PermissoesPersonalizadasCarga);

    iniciarControleManualRequisicao();

    var codigo = parseInt(e.CodigoCTE);

    if (e.SituacaoCTe == EnumStatusCTe.EMDIGITACAO) {
        var instancia = new EmissaoCTe(codigo, function () {
            finalizarControleManualRequisicao();
            instancia.CRUDCTe.Emitir.visible(false);
            instancia.CRUDCTe.Salvar.eventClick = function () {
                var objetoCTe = ObterObjetoCTe(instancia);
                SalvarCTe(objetoCTe, e.Codigo, instancia);
            };
        }, possuiPermissao ? (e.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros ? _PermissoesEdicaoDoCTe.OutrosDocumentosEmDigitacao : _PermissoesEdicaoDoCTe.EmDigitacao) : permissoesSomenteLeituraCTe);
    } else if (e.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        var instancia = new EmissaoCTe(codigo, function () {
            finalizarControleManualRequisicao();
            instancia.CRUDCTe.Salvar.visible(false);
            instancia.CRUDCTe.Emitir.visible(true);
            instancia.CRUDCTe.Emitir.eventClick = function () {
                var objetoCTe = ObterObjetoCTe(instancia);
                EmitirCTeRejeicao(objetoCTe, e.Codigo, instancia);
            };
        }, possuiPermissao ? _PermissoesEdicaoDoCTe.Rejeicao : permissoesSomenteLeituraCTe);
    }
}

function EmitirCTeRejeicao(cte, cargaCTe, instancia) {
    var dados = { CTe: cte, CodigoCargaCTe: cargaCTe };
    executarReST("CargaCTe/EmitirCargaCTeRejeição", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                instancia.FecharModal();
                _gridCargaCTe.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CteAlteradoComSucesso);
                exibirReenvioCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function SalvarCTe(cte, cargaCTe, instancia) {
    var dados = { CTe: cte, CodigoCargaCTe: cargaCTe }
    executarReST("CargaCTe/AlterarCargaCTe", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                instancia.FecharModal();
                var carga = arg.Data;
                _gridCargaCTe.CarregarGrid();
                IniciarBindKnoutCarga(_cargaAtual, carga);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CteAlteradoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExibirInformacaoEmissaoCTesSubContratacaoFilialEmissora(e) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == e.DivCarga.id) {
        if ((!e.EmiteMDFeFilialEmissora.val() && e.AgGeracaoCTesAnteriorFilialEmissora.val() || e.EmEmissaoCTeSubContratacaoFilialEmissora.val()) && _cargaCTe != null) {
            _cargaCTe.CTesSubContratacaoFilialEmissora.visible(true);
        } else if (_cargaCTe != null) {
            _cargaCTe.CTesSubContratacaoFilialEmissora.visible(false);
        }
    }

    //DEVE BUSCAR OS CARGA_CTES DE FACTURAS
    if (_cargaCTe != null) {
        if (_cargaAtual.Mercosul.val()) {
            _cargaCTe.CTesSemSubContratacaoFilialEmissora.val(false);
            _cargaCTe.CTesFactura.val(true);
            _cargaCTe.CargaMercosul.val(true);
            _cargaCTe.CTe.text("CRT");

            $("#tabCTes_" + e.DadosCTes.id + "_li span").text("CRT");
        } else {
            _cargaCTe.CTesFactura.val(false);
        }
    }

}

function retoronoSefazClick(e, sender) {
    $('#PMensagemRetornoSefaz').html(e.RetornoSefaz);
    Global.abrirModal("divModalRetornoSefaz");
}

function buscarCTesClick(e, sender, ctesFilialEmissora) {
    _cargaAtual = e;
    ocultarTodasAbas(e);
    let strKnoutCargaCTe = "knoutCargaCTe" + e.EtapaCTeNFs.idGrid;
    let html = _HTMLCTE.replace("#knoutCargaCTe", strKnoutCargaCTe).replace(/#DadosCTes/g, _cargaAtual.DadosCTes.id);

    $("#" + e.EtapaCTeNFs.idGrid).html(html);
    _cargaCTe = new CargaCTEs();

    _informarContainer = new InformarContainer();
    KoBindings(_informarContainer, "knoutModalInformarContainer");

    _alterarObservacao = new AlterarObservacao();
    KoBindings(_alterarObservacao, "knoutModalAlterarObservacao");

    _protocoloIntegracao = new ProtocoloIntegracao();
    KoBindings(_protocoloIntegracao, "knoutModalProtocoloIntegracao");

    _anexoEtapaCTe = new AnexoEtapaCTe();
    KoBindings(_anexoEtapaCTe, "knockoutAnexoEtapaCTe");

    BuscarContainers(_informarContainer.Container);

    $("#tabCTes_" + e.DadosCTes.id + "_li span").text(e.EtapaCTeNFs.text());

    $("#" + _cargaCTe.ChavePIXCelular.id).mask("(00) 00000-0000", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _cargaCTe.ChavePIXCPFCNPJ.id).mask('000.000.000-000', {
        selectOnFocus: true,
        translation: {
            '0': { pattern: /[0-9]/ }
        },
        onKeyPress: function (val, e, field, options) {
            var cleanVal = val.replace(/\D/g, '');
            var masks = ['000.000.000-000', '00.000.000/0000-00'];
            var mask = (cleanVal.length > 11) ? masks[1] : masks[0];

            field.mask(mask, {
                selectOnFocus: true,
                translation: {
                    '0': { pattern: /[0-9]/ }
                }
            });
        }
    });

    $("#" + _cargaCTe.ChavePIXEmail.id).on('input blur', function () {
        var email = this.value;
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email && !emailRegex.test(email)) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }

        _cargaCTe.ChavePIXEmail.val(email);
    });

    if (ctesFilialEmissora != null) {
        _cargaCTe.CTesSemSubContratacaoFilialEmissora.val(ctesFilialEmissora);
        _cargaCTe.CTesSubContratacaoFilialEmissora.val(!ctesFilialEmissora);
        ExibirInformacaoEmissaoCTesSubContratacaoFilialEmissora(e);
    }
    else {
        if (e.PossuiCTeSubcontratacaoFilialEmissora.val()) {
            _cargaCTe.CTesSemSubContratacaoFilialEmissora.val(false);
            _cargaCTe.CTesSubContratacaoFilialEmissora.val(true);
        }
        else {
            _cargaCTe.CTesSemSubContratacaoFilialEmissora.val(false);
            _cargaCTe.CTesSubContratacaoFilialEmissora.val(false);
        }
    }

    _cargaCTe.Carga.val(_cargaAtual.Codigo.val());
    KoBindings(_cargaCTe, strKnoutCargaCTe);

    BuscarCIOT(_cargaCTe.EntidadeCIOT);
    BuscarCIOT(_cargaCTe.EntidadeCIOTPreCte);

    loadGridAnexoEtapaCTe();
    LocalizeCurrentPage();

    if (_CONFIGURACAO_TMS.EmitirNFeRemessaNaCarga)
        $("#tabNFes_" + e.DadosCTes.id + "_li").show();
    else
        $("#tabNFes_" + e.DadosCTes.id + "_li").hide();

    if ((e.PossuiIntegracaoValePedagio.val() && !_cargaCTe.CTesSemSubContratacaoFilialEmissora.val()) || _cargaAtual.NaoPermitirLiberarSemValePedagio.val())
        $("#tabIntegracaoValePedagio_" + e.DadosCTes.id + "_li").show();
    else
        $("#tabIntegracaoValePedagio_" + e.DadosCTes.id + "_li").hide();

    if (e.ProblemaIntegracaoCIOT.val())
        $("#DadosGeracaoCIOT").show();
    else
        $("#DadosGeracaoCIOT").hide();

    if (e.possuiNFSManual.val() && (!ctesFilialEmissora || !e.PossuiCTeSubcontratacaoFilialEmissora.val())) {
        $("#tabDocumentoParaEmissaoNFSManual_" + e.DadosCTes.id + "_li").show();
        buscarCargasCargaDocumentoParaEmissaoNFSManual();
    }
    else
        $("#tabDocumentoParaEmissaoNFSManual_" + e.DadosCTes.id + "_li").hide();

    $("#" + _cargaCTe.NumeroDoc.id).removeAttr("disabled");
    $("#" + _cargaCTe.NumeroNF.id).removeAttr("disabled");
    $("#" + _cargaCTe.Status.id).removeAttr("disabled");
    $("#" + _cargaCTe.Pesquisar.id).removeAttr("disabled");

    var agImportacaoCTe = _cargaAtual.AgImportacaoCTe.val();
    var liberadaSemTodosPreCTes = _cargaAtual.LiberadaSemTodosPreCTes.val();

    if (_cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val() && (ctesFilialEmissora || !e.PossuiCTeSubcontratacaoFilialEmissora.val()))
        agImportacaoCTe = false;

    ExibirMensagemCIOTCte(e);

    _isPreCte = false;
    if (agImportacaoCTe || liberadaSemTodosPreCTes) {
        if (agImportacaoCTe && !liberadaSemTodosPreCTes)
            _cargaCTe.CTe.visible(false);
        else {
            _cargaCTe.CTe.visible(true);
            $("#" + e.EtapaCTeNFs.idGrid + " .DivCTEGrid").show();
        }

        buscarPreCTes(function () {
            var exibirBotaoLiberarSemTodosPreCTes = !liberadaSemTodosPreCTes && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirAvancarCargaSemTodosPreCte, _PermissoesPersonalizadasCarga);

            $("#" + e.EtapaCTeNFs.idGrid + " .DivCTEGrid").show();

            _cargaCTe.PreCTe.visible(true);
            _cargaCTe.ConfirmarEmissaoCTes.visible(false);
            _cargaCTe.LiberarSemTodosPreCTes.visible(exibirBotaoLiberarSemTodosPreCTes);

            if (e.PossuiIntegracaoValePedagio.val() && !_cargaCTe.CTesSemSubContratacaoFilialEmissora.val())
                $("#tabIntegracaoValePedagioPreCtes_" + e.DadosCTes.id + "_li").show();
            else
                $("#tabIntegracaoValePedagioPreCtes_" + e.DadosCTes.id + "_li").hide();
        });

        _isPreCte = true;
        buscarCargasValePedagio();
        buscarCargasCIOT(e);
        buscarCargasCTeAverbacao();
        buscarCargasCTe(function (dataRetorno) {

            _cargaCTe.SincronizarTodosDocumento.visible(dataRetorno.data?.some(item => item.HabilitarSincronizarDocumento === true));

            if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && !_cargaAtual.AutorizouTodosCTes.val() && _cargaAtual.PossuiPendencia.val() && _cargaAtual.SituacaoCarga.val() === EnumSituacoesCarga.PendeciaDocumentos) || (_cargaAtual.CTesEmDigitacao.val() && !_cargaAtual.EmitindoCTes.val())) {
                _cargaCTe.ConfirmarEmissaoCTes.visible(true);
            }
            else {
                _cargaCTe.ConfirmarEmissaoCTes.visible(false);
                _cargaCTe.EmitindoCTes.val(_cargaAtual.EmitindoCTes.val());
                _cargaCTe.EmitindoCTes.visible(_cargaAtual.EmitindoCTes.val());
            }

            if (_cargaAtual.AutorizouTodosCTes.val() && !_cargaAtual.EmitindoCTes.val() && !_cargaCTe.CTesFactura.val() && !_cargaAtual.CTesEmDigitacao.val() && !_cargaAtual.AverbandoCTes.val() && (!(_cargaAtual.PossuiPendencia.val() && _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.PendeciaDocumentos) || _cargaAtual.ProblemaMDFe.val())) {
                EtapaCTeNFsAprovada(e);
                _cargaCTe.DownloadLoteXMLCTe.visible(true);
                _cargaCTe.DownloadLoteDACTE.visible(true);
                _cargaCTe.DownloadLoteDocumentos.visible(true);
            }
            else {
                if (_cargaAtual.ProblemaCTE.val()) {
                    if (ctesFilialEmissora == null || !ctesFilialEmissora || !_cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val()) {
                        preencherMotivoPendenciaCTeNFSe(_cargaAtual);
                        _cargaCTe.EmitirNovamenteRejeitados.visible(true);
                    }
                }
                else if (_cargaAtual.PendenciaTransportadorContribuinte.val()) {
                    if (ctesFilialEmissora == null || !ctesFilialEmissora || !_cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val()) {
                        preencherMotivoPendenciaCTeNFSe(_cargaAtual);
                    }
                }
                else if (_cargaAtual.ProblemaAverbacaoCTe.val()) {
                    if (!e.PossuiCTeSubcontratacaoFilialEmissora.val() || ctesFilialEmissora) {
                        preencherMotivoPendenciaAverbacaoCTe(_cargaAtual);
                        _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
                    }
                }
                else if (_cargaAtual.ProblemaIntegracaoValePedagio.val()) {
                    preencherMotivoPendenciaIntegracaoValePedagio(_cargaAtual);
                }
            }

            buscarNotfisCarga(function () {

            });
        }, obterMenuOpcoesCargasCTe());
    }
    else {
        _cargaCTe.LiberarSemTodosPreCTes.visible(false);

        buscarCargasCTe(function (dataRetorno) {

            _cargaCTe.SincronizarTodosDocumento.visible(dataRetorno.data?.some(item => item.HabilitarSincronizarDocumento === true));

            if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && !_cargaAtual.AutorizouTodosCTes.val() && _cargaAtual.PossuiPendencia.val() && _cargaAtual.SituacaoCarga.val() === EnumSituacoesCarga.PendeciaDocumentos) || (_cargaAtual.CTesEmDigitacao.val() && !_cargaAtual.EmitindoCTes.val())) {
                _cargaCTe.ConfirmarEmissaoCTes.visible(true);
            }
            else {
                _cargaCTe.ConfirmarEmissaoCTes.visible(false);
                _cargaCTe.EmitindoCTes.val(_cargaAtual.EmitindoCTes.val());
                _cargaCTe.EmitindoCTes.visible(_cargaAtual.EmitindoCTes.val());
            }

            $("#" + e.EtapaCTeNFs.idGrid + " .DivCTEGrid").show();

            if (_cargaAtual.AutorizouTodosCTes.val() && !_cargaAtual.EmitindoCTes.val() && !_cargaCTe.CTesFactura.val() && !_cargaAtual.CTesEmDigitacao.val() && !_cargaAtual.AverbandoCTes.val() && (!(_cargaAtual.PossuiPendencia.val() && _cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.PendeciaDocumentos) || _cargaAtual.ProblemaMDFe.val())) {
                EtapaCTeNFsAprovada(e);
                _cargaCTe.DownloadLoteXMLCTe.visible(true);
                _cargaCTe.DownloadLoteDACTE.visible(true);
                _cargaCTe.DownloadLoteDocumentos.visible(true);
            }
            else {
                if (_cargaAtual.ProblemaCTE.val()) {
                    if (ctesFilialEmissora == null || !ctesFilialEmissora || !_cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val()) {
                        preencherMotivoPendenciaCTeNFSe(_cargaAtual);
                        _cargaCTe.EmitirNovamenteRejeitados.visible(true);
                    }
                }
                else if (_cargaAtual.PendenciaTransportadorContribuinte.val()) {
                    if (ctesFilialEmissora == null || !ctesFilialEmissora || !_cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val()) {
                        preencherMotivoPendenciaCTeNFSe(_cargaAtual);
                    }
                }
                else if (_cargaAtual.ProblemaAverbacaoCTe.val()) {
                    if (!e.PossuiCTeSubcontratacaoFilialEmissora.val() || ctesFilialEmissora) {
                        preencherMotivoPendenciaAverbacaoCTe(_cargaAtual);
                        _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
                    }
                }
                else if (_cargaAtual.ProblemaIntegracaoValePedagio.val()) {
                    preencherMotivoPendenciaIntegracaoValePedagio(_cargaAtual);
                }
            }

            if (!PermitirAcessarEtapaCTeCarga()) {
                if (_cargaAtual.NaoPermitirAcessarDocumentosAntesCargaEmTransporte.val() && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAcessarEtapasDocumentosQuandoAcessoEstiverRestrito, _PermissoesPersonalizadasCarga)) {
                    _cargaCTe.CTe.visible(true);
                    _cargaCTe.ConfirmarEmissaoCTes.visible(false);
                }
                else {
                    _cargaCTe.CTe.visible(false);
                    _cargaCTe.ConfirmarEmissaoCTes.visible(false);
                }
            }
            else {
                _cargaCTe.CTe.visible(true);
            }

            buscarNotfisCarga(function () {

            });
        }, obterMenuOpcoesCargasCTe());

        buscarCargasValePedagio();
        buscarCargasCTeAverbacao();
        buscarCargasMICDTA();
        buscarCargasNFe();
        buscarCargasCIOT(e);
        buscarCargasDespesa();
        loadGridGuiasRecolhimentoTributarioEstatual();
    }

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos, _PermissoesPersonalizadasCarga))
        _cargaCTe.ConfirmarEmissaoCTes.enable(false);

    CriarComponenteIntegracaoNotifis();
}

var PermitirAcessarEtapaCTeCarga = () => {
    let situacoesPermitidas = [EnumSituacoesCarga.AgImpressaoDocumentos, EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.Cancelada, EnumSituacoesCarga.Anulada];

    return (!_CONFIGURACAO_TMS.NaoPermitirAcessarDocumentosAntesCargaEmTransporte && !_cargaAtual.NaoPermitirAcessarDocumentosAntesCargaEmTransporte.val()) || situacoesPermitidas.includes(_cargaAtual.SituacaoCarga.val()) || _cargaAtual.ProblemaCTE.val() === true || _cargaAtual.ProblemaAverbacaoCTe.val() === true || _cargaAtual.ProblemaIntegracaoValePedagio.val() === true;
}

function liberarSemTodosPreCTesClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.Carga.DesejaRealmenteLiberarCargaSemReceberTodosOsCtes, function () {
        executarReST("CargaPreCTe/LiberarSemTodosPreCTes", { Carga: _cargaCTe.Carga.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CargaLiberadaSemReceberTodosOsCtesComSucesso);
                    _cargaCTe.LiberarSemTodosPreCTes.visible(false);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 6000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function preencherMotivoPendenciaCIOT(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "")
        exibirMotivoPendenciaEtapaDocumentosCarga(knoutCarga, 'DivMensagemCTe');

    //if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCarga)) {
    //    _cargaCTe.LiberarCargaComAverbacoesRejeitados.visible(true);
    //    _cargaCTe.LiberarCargaComAverbacoesRejeitados.enable(true);
    //}

    if (!knoutCarga.PossuiCTeSubcontratacaoFilialEmissora.val() || knoutCarga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        EtapaCTeNFsProblema(knoutCarga);
    else
        EtapaCTeFilialEmissoraProblema(knoutCarga);
}

function preencherMotivoPendenciaAverbacaoCTe(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "")
        exibirMotivoPendenciaEtapaDocumentosCarga(knoutCarga, 'DivMensagemAverbacaoCTe');

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada, _PermissoesPersonalizadasCarga)) {
        _cargaCTe.LiberarCargaComAverbacoesRejeitados.visible(true);
        _cargaCTe.LiberarCargaComAverbacoesRejeitados.enable(true);
    }

    if (!knoutCarga.PossuiCTeSubcontratacaoFilialEmissora.val() || knoutCarga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        EtapaCTeNFsProblema(knoutCarga);
    else
        EtapaCTeFilialEmissoraProblema(knoutCarga);
}

function preencherMotivoPendenciaIntegracaoValePedagio(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "")
        exibirMotivoPendenciaEtapaDocumentosCarga(knoutCarga, 'DivMensagemIntegracaoValePedagio');

    if (!knoutCarga.PossuiCTeSubcontratacaoFilialEmissora.val() || knoutCarga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        EtapaCTeNFsProblema(knoutCarga);
    else
        EtapaCTeFilialEmissoraProblema(knoutCarga);
}

function exibirMotivoPendenciaEtapaDocumentosCarga(knoutCarga, classe) {
    var html =
        '<div class="alert alert-info mb-2 fade show" role="alert">' +
        '    <div class="d-flex align-items-center">' +
        '        <div class="alert-icon">' +
        '            <i class="fal fa-info-circle"></i>' +
        '        </div>' +
        '        <div class="flex-1">' +
        '            <span class="h5">' + Localization.Resources.Cargas.Carga.Pendencia + '</span>' +
        '            <br>' +
        '            ' + knoutCarga.MotivoPendencia.val() +
        '       </div>' +
        '    </div>' +
        '</div>';

    $("#" + knoutCarga.EtapaCTeNFs.idGrid + " ." + classe).html(html).show();
}

function ExibirMensagemCIOTCte(knoutCarga) {
    var rows = [];

    if (knoutCarga.MotivoPendencia.val() != "")
        rows.push(
            '<div class="alert alert-info mb-2 fade show" role="alert">' +
            '    <div class="d-flex align-items-center">' +
            '        <div class="alert-icon">' +
            '            <i class="fal fa-info-circle"></i>' +
            '        </div>' +
            '        <div class="flex-1">' +
            '            <span class="h5">' + Localization.Resources.Cargas.Carga.Pendencia + '</span>' +
            '            <br>' +
            '            ' + knoutCarga.MotivoPendencia.val() +
            '       </div>' +
            '    </div>' +
            '</div>'
        );

    if (rows.length > 0)
        $("#" + knoutCarga.EtapaCTeNFs.idGrid + " ." + "DivMensagemCTe").html(rows.join("")).show();
}

function preencherMotivoPendenciaCTeNFSe(knoutCarga) {
    if (knoutCarga.MotivoPendencia.val() != "")
        exibirMotivoPendenciaEtapaDocumentosCarga(knoutCarga, 'DivMensagemCTe');

    if (!knoutCarga.PossuiCTeSubcontratacaoFilialEmissora.val() || knoutCarga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        EtapaCTeNFsProblema(knoutCarga);
    else
        EtapaCTeFilialEmissoraProblema(knoutCarga);
}

function emitirCTeClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.REJEICAO || e.SituacaoCTe == EnumStatusCTe.FSDA) {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/EmitirNovamente", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTe.CarregarGrid();
                    exibirReenvioCTe();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.SituacaoNaoPermiteEmissao, Localization.Resources.Cargas.Carga.AtualSituacaoDoCteNaoPermiteQueEleSejaEmitidoNovamente.format(e.Status));
    }
}

function sincronizarCTeClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.ENVIADO || e.SituacaoCTe == EnumStatusCTe.EMCANCELAMENTO) {
        var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
        executarReST("CargaCTe/SincronizarDocumentoEmProcessamento", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridCargaCTe.CarregarGrid();
                    exibirReenvioCTe();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function desvincularCTeEGerarCopiaClick(e, knout) {
    if (e.SituacaoCTe == EnumStatusCTe.REJEICAO) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteDesvincularCTeAtualDaCargaGerarUmaCopiaDoMesmoComUmaNovaNumeracao, function () {
            var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
            executarReST("CargaCTe/DesvincularCTeEGerarCopia", data, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        _gridCargaCTe.CarregarGrid();
                        exibirReenvioCTe();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            });
        });
    }
}

function exibirReenvioCTe() {
    _cargaAtual.PossuiPendencia.val(false);
    _cargaAtual.ProblemaCTE.val(false);
    _cargaAtual.MotivoPendencia.val("");

    if ((!_cargaAtual.PossuiCTeSubcontratacaoFilialEmissora.val() && !_cargaAtual.Mercosul.val()) || _cargaAtual.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        EtapaCTeNFsAguardando(_cargaAtual);
    else
        EtapaCTeFilialEmissoraAguardando(_cargaAtual);

    $("#" + _cargaAtual.EtapaCTeNFs.idGrid + " .DivMensagemCTe").hide();
    _cargaCTe.EmitirNovamenteRejeitados.visible(false);
}

function confirmarEmissaoCTeClick(e) {
    emitirCTes({ Carga: e.Carga.val(), Status: EnumStatusCTe.EMDIGITACAO });
}

function DownloadLoteXMLCTeClick(e) {
    executarDownload("CargaCTe/DownloadLoteXML", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() });
}

function DownloadLoteDACTEClick(e) {
    executarDownload("CargaCTe/DownloadLoteDACTE", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() });
}

function DownloadLoteDANFEClick(e) {
    executarDownload("CargaCTe/DownloadLoteDANFE", { Carga: e.Carga.val() });
}

function DownloadLoteDocumentosCTeClick(e) {
    executarReST("CargaCTe/DownloadLoteDocumentos", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SolicitacaoRealizadaComSucessoFavorAguardeArquivoSerGerado, 20000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
    //executarDownload("CargaCTe/DownloadLoteDocumentos", { Carga: e.Carga.val(), CTesSubContratacaoFilialEmissora: _cargaCTe.CTesSubContratacaoFilialEmissora.val(), CTesSemSubContratacaoFilialEmissora: _cargaCTe.CTesSemSubContratacaoFilialEmissora.val() });
}

function reemitirCTesRejeitadosClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteReemitirTodosOsCtesRejeitadosDestaCarga, function () {
        emitirCTes({ Carga: e.Carga.val(), Status: EnumStatusCTe.REJEICAO });
    });
}

function sincronizarTodosDocumentoClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteSincronizarTodosDocumentoDestaCarga, function () {
        sincronizarTodosDocumento({ Carga: e.Carga.val() });
    });
}

function emitirCTes(data) {
    executarReST("CargaCTe/AutorizarEmissaoCTes", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTe.CarregarGrid();
                _cargaCTe.ConfirmarEmissaoCTes.visible(false);
                _cargaCTe.ErroEmissaoCTes.visible(false);
                _cargaCTe.SucessoEmissaoCTes.visible(false);
                _cargaCTe.EmitindoCTes.val(true);
                _cargaCTe.EmitindoCTes.visible(true);
                _cargaAtual.EmitindoCTes.val(true);
                exibirReenvioCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function sincronizarTodosDocumento(data) {
    executarReST("CargaCTe/SincronizarLoteDocumentoEmProcessamento", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaCTe.CarregarGrid();
                exibirReenvioCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function baixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXML", data);
}

function baixarDacteCompClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacteComplemento", data);
}

function baixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function baixarEDIClick(e) {
    var data = { CodigoCTe: e.CodigoCTE };
    executarDownload("CargaCTe/DownloadEDI", data);
}

function baixarXMLMigrateClick(e) {
    var data = { CodigoCTe: e.CodigoCTE, CodigoEmpresa: e.CodigoEmpresa };
    executarDownload("CargaCTe/DownloadXMLMigrate", data);
}

function enviarNFSeClick(e) {

}

//*******MÉTODOS*******

function RecarregarGridProblemaIntegracaoValePedagioSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && (_gridCargaIntegracaoValePedagio != null || _gridCargaIntegracaoValePedagioPreCTe != null)) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible")) {
            if (_gridCargaIntegracaoValePedagio != null)
                _gridCargaIntegracaoValePedagio.CarregarGrid();// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela

            if (_gridCargaIntegracaoValePedagioPreCTe != null)
                _gridCargaIntegracaoValePedagioPreCTe.CarregarGrid();

            preencherMotivoPendenciaIntegracaoValePedagio(knoutCarga);
            //if (_cargaCTe != null) {
            //    _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
            //}
        }
    }
}

function RecarregarGridProblemaCIOTSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && (_gridCargaCIOT != null || _gridCargaCIOTPreCTe != null)) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible")) {
            if (_gridCargaCIOT != null)
                _gridCargaCIOT.CarregarGrid();// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela

            if (_gridCargaCIOTPreCTe != null)
                _gridCargaCIOTPreCTe.CarregarGrid();

            preencherMotivoPendenciaAverbacaoCTe(knoutCarga);
            preencherMotivoPendenciaCIOT(knoutCarga);
            //if (_cargaCTe != null) {
            //    _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
            //}
        }
    }
}

function RecarregarGridProblemaAverbacaoCTeSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaCTeAverbacao != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible")) {
            _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela
            preencherMotivoPendenciaAverbacaoCTe(knoutCarga);
            if (_cargaCTe != null) {
                _cargaCTe.EmitirNovamenteAverbacoesCTe.visible(true);
            }
        }
    }
}

function RecarregarGridProblemaCTeSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaCTe != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible")) {
            _gridCargaCTe.CarregarGrid();// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela
            preencherMotivoPendenciaCTeNFSe(knoutCarga);
            if (_cargaCTe != null) {
                _cargaCTe.EmitirNovamenteRejeitados.visible(true);
            }
        }
    }
}

function RecarregarGridPendenciaContribuinteCTeSignalR(knoutCarga) {
    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaCTe != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible")) {
            _gridCargaCTe.CarregarGrid();// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela
            preencherMotivoPendenciaCTeNFSe(knoutCarga);
        }
    }
}

function RecarregarGridsAutorizadosCTeNFSeSignalR(knoutCarga) {

    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaCTe != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible"))
            _gridCargaCTe.CarregarGrid();// nesse caso atualiza a grid de ctes pois pode ser que o usuário esteja visualizando essa tela
    }

    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaCTeAverbacao != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible"))
            _gridCargaCTeAverbacao.CarregarGrid(callbackGridAverbacaoCTe);
    }

    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaNFe != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible"))
            _gridCargaNFe.CarregarGrid();
    }

    if (_cargaAtual != null && _cargaAtual.DivCarga.id == knoutCarga.DivCarga.id && _gridCargaMICDTA != null) {
        if ($("#" + _cargaAtual.EtapaCTeNFs.idGrid).is(":visible"))
            _gridCargaMICDTA.CarregarGrid();
    }
}

function VisibilidadeMensagemSefaz(data) {
    if (data.RetornoSefaz != "" && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeRejeicao(data) {
    if (data.SituacaoCTe == EnumStatusCTe.REJEICAO || data.SituacaoCTe == EnumStatusCTe.FSDA) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeSincronizarDocumento(data) {
    if (data.HabilitarSincronizarDocumento == true) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDesvincularCTeEGerarCopia(data) {
    if (data.HabilitarDesvincularCTeEGerarCopia == true && _CONFIGURACAO_TMS.PermitirDesvincularGerarCopiaCTeRejeitadoCarga) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeDownloadOutrosDoc(data) {
    if (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadDANFSE(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFS)) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownload(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.FSDA || data.SituacaoCTe == EnumStatusCTe.EMCONTINGENCIA) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoDownloadMigrate(data) {
    if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.REJEICAO) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe && data.SistemaEmissor == 3) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeOpcaoCartaCorrecaoCTe(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadeAlterarObservacao(data) {
    return data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && data.TipoDocumentoEmissao != EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadeInformarContainer(data) {
    return !data.ContemContainer && data.ContainerADefinir && data.SituacaoCTe == EnumStatusCTe.AUTORIZADO && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe;
}

function VisibilidadeInformacaoContribuinte(data, anexo) {
    let visivel = data.SituacaoCTe == EnumStatusCTe.AUTORIZADO &&
        data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe &&
        _cargaAtual.PendenciaTransportadorContribuinte.val();

    if (anexo)
        visivel = visivel || data.SituacaoDocumentoTransportador == "Aprovado";

    return visivel;
}

function VisibilidadeOpcaoDownloadEDI(data) {
    //if ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_ReenviarIntegracoes, _PermissoesPersonalizadasCarga)) {
    //    return true;
    //} else {
    //    return false;
    //}
    return false;
}

function VisibilidadeOpcaoEditar(data) {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe &&
        (((data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO || data.SituacaoCTe == EnumStatusCTe.REJEICAO) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe) ||
            (data.SituacaoCTe == EnumStatusCTe.EMDIGITACAO && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.Outros))) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeProtocoloIntegracao(data) {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && (data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.NFSe || data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe);
}

function EtapaCTeNFsDesabilitada(e) {
    $("#" + e.EtapaCTeNFs.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step");
    e.EtapaCTeNFs.eventClick = function (e) { };
}

function EtapaCTeNFsLiberada(e) {

    $("#" + e.EtapaCTeNFs.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step yellow");
    e.EtapaCTeNFs.eventClick = buscarCTesClick;
    aprovarEtapasAnteriores(e);
}

function EtapaCTeNFsAprovada(e) {
    $("#" + e.EtapaCTeNFs.idTab).attr("data-bs-toggle", "tab");

    if (e.AgNFSManual.val() || e.CargaEmitidaParcialmente.val())
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step cyan");
    else if (e.LiberadaSemTodosPreCTes.val())
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step orange");
    else
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step green");

    e.EtapaCTeNFs.eventClick = buscarCTesClick;
    aprovarEtapasAnteriores(e);
}

function EtapaCTeNFsProblema(e) {
    $("#" + e.EtapaCTeNFs.idTab).attr("data-bs-toggle", "tab");
    if (e.PendenciaTransportadorContribuinte.val())
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step cyan");
    else
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step red");
    e.EtapaCTeNFs.eventClick = buscarCTesClick;
    aprovarEtapasAnteriores(e);
}

function EtapaCTeNFsAguardando(e) {

    $("#" + e.EtapaCTeNFs.idTab).attr("data-bs-toggle", "tab");

    if (!e.AgImportacaoCTe.val())
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step yellow");
    else
        $("#" + e.EtapaCTeNFs.idTab + " .step").attr("class", "step orange");

    e.EtapaCTeNFs.eventClick = buscarCTesClick;
    aprovarEtapasAnteriores(e);
    ExibirInformacaoEmissaoCTesSubContratacaoFilialEmissora(e);
}

function EtapaCTeNFsEdicaoDesabilitada(e) {
    e.EtapaCTeNFs.enable(false);
    EtapaNotaFiscalEdicaoDesabilitada(e);
    EtapaFreteTMSEdicaoDesabilitada(e);
}

function aprovarEtapasAnteriores(e) {
    if (e.PossuiCTeSubcontratacaoFilialEmissora.val()) {
        //if (e.EmiteMDFeFilialEmissora.val())
        //    EtapaMDFeAprovada(e);
        //else
        //    EtapaCTeFilialEmissoraAprovada(e);
        EtapaIntegracaoFilialEmissoraAprovada(e);
    } else {
        if (e.ExigeNotaFiscalParaCalcularFrete.val()) {
            EtapaFreteTMSAprovada(e);
            EtapaFreteTMSEdicaoDesabilitada(e);
        } else {
            EtapaNotaFiscalAprovada(e);
        }
    }

    if (e.Mercosul.val()) {
        EtapaNotaFiscalMercosulAprovada(e);
    }
}

function EtapaCTeFilialEmissoraDesabilitada(e) {
    $("#" + e.EtapaCTeFilialEmissora.idTab).removeAttr("data-bs-toggle");
    $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step");
    e.EtapaCTeFilialEmissora.eventClick = function (e) { };
}

function EtapaCTeFilialEmissoraLiberada(e) {
    $("#" + e.EtapaCTeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step yellow");
    e.EtapaCTeFilialEmissora.eventClick = function (e, sender) { buscarCTesClick(e, sender, true) };
    aprovarEtapasAnterioresEtapaCTeFilialEmissora(e);
}

function EtapaCTeFilialEmissoraAprovada(e) {
    $("#" + e.EtapaCTeFilialEmissora.idTab).attr("data-bs-toggle", "tab");

    if (e.AgNFSManual.val() && !e.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora.val())
        $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step cyan");
    else
        $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step green");

    e.EtapaCTeFilialEmissora.eventClick = function (e, sender) { buscarCTesClick(e, sender, true) };
    aprovarEtapasAnterioresEtapaCTeFilialEmissora(e);
}

function EtapaCTeFilialEmissoraProblema(e) {
    $("#" + e.EtapaCTeFilialEmissora.idTab).attr("data-bs-toggle", "tab");
    $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step red");
    e.EtapaCTeFilialEmissora.eventClick = function (e, sender) { buscarCTesClick(e, sender, true) };
    aprovarEtapasAnterioresEtapaCTeFilialEmissora(e);
}

function EtapaCTeFilialEmissoraAguardando(e) {
    $("#" + e.EtapaCTeFilialEmissora.idTab).attr("data-bs-toggle", "tab");

    if (!e.AgImportacaoCTe.val())
        $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step yellow");
    else
        $("#" + e.EtapaCTeFilialEmissora.idTab + " .step").attr("class", "step orange");

    //se é mercosul e nao esta mais emitindo crt entao esta na etapa do CTE.
    //if (e.Mercosul.val() && !e.EmitindoCRT.val()) {
    //    EtapaCTeFilialEmissoraAprovada(e);
    //}

    e.EtapaCTeFilialEmissora.eventClick = function (e, sender) { buscarCTesClick(e, sender, true) };
    aprovarEtapasAnterioresEtapaCTeFilialEmissora(e);
}

function EtapaCTeFilialEmissoraEdicaoDesabilitada(e) {
    e.EtapaCTeFilialEmissora.enable(false);
    EtapaNotaFiscalEdicaoDesabilitada(e);
    EtapaFreteTMSEdicaoDesabilitada(e);
}

function aprovarEtapasAnterioresEtapaCTeFilialEmissora(e) {
    if (e.ExigeNotaFiscalParaCalcularFrete.val()) {
        EtapaFreteTMSAprovada(e);
        EtapaFreteTMSEdicaoDesabilitada(e);
    } else {
        EtapaNotaFiscalAprovada(e);
    }
}

function LimparDadosInformacaoContainer() {
    LimparCampos(_informarContainer);
}

function LimparDadosAlterarObservacao() {
    LimparCampos(_alterarObservacao);
}

function LimparDadosProtocoloIntegracao() {
    LimparCampo(_protocoloIntegracao);
}

function CriarComponenteIntegracaoNotifis() {

    const componente = new ComponenteIntegracao("#componenteIntegracaoNotifis_" + _cargaAtual.DadosCTes.id, "#integracao" + _cargaAtual.DadosCTes.id);

    // Opções
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };
    menuOpcoes.opcoes.push({
        descricao: Localization.Resources.Cargas.Carga.Reenviar, id: guid(), metodo: (data) => {

            executarReST("ControleGeracaoEDI/Reenviar", { Codigo: data.Codigo }, function (r) {
                if (r.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.ReenvioSolicitadoComSucesso);
                    componente.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
                }
            });

        }, tamanho: "20", icone: "", visibilidade: (data) => data.TipoIntegracao
    });

    menuOpcoes.opcoes.push({
        descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: (data) => {
            executarDownload("CargaPreCTe/DownloadArquivoEDI", { Codigo: data.Codigo, Carga: _cargaCTe.Carga.val() });
        }, tamanho: "20", icone: "", visibilidade: true
    });

    componente.setSituacoes([
        { value: 0, text: 'Todas' },
        { value: 1, text: 'Aguardando integracao' },
        { value: 2, text: 'Integrado' },
        { value: 3, text: 'Falha' },
    ]);

    componente.configurarGrid("ControleGeracaoEDI/Pesquisa?TelaCarga=true", {
        Carga: PropertyEntity({ val: ko.observable(_cargaCTe.Carga.val()), getType: typesKnockout.int }),
        Situacao: PropertyEntity({ val: ko.observable(null), getType: typesKnockout.int })
    }, menuOpcoes);

    componente.setOnPesquisar((gridView, dados, situacao) => {
        dados.Situacao.val(situacao);
        gridView.CarregarGrid();
    });

    componente.setOnObterTotais((integracao) => {
        executarReST("ControleGeracaoEDI/ObterTotais", { Carga: _cargaCTe.Carga.val() }, function (r) {
            if (r.Success) {
                PreencherObjetoKnout(integracao, r);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });

    componente.render();
}

function DownloadPreCtesClick() {
    executarDownload("CargaPreCTe/DownloadTodosPreCte", { Carga: _cargaCTe.Carga.val() })
}

function DownloadPreviaCustoClick() {
    executarDownload("CargaPreCTe/DownloadPreviaCusto", { Carga: _cargaCTe.Carga.val() })
}

function liberarSemGNRE() {
    executarReST("GuiaNacionalRecolhimentoTributoEstual/LiberarComProblemaGNRE", { Carga: _cargaCTe.Carga.val() }, function (r) {
        if (!r.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);

        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Liberação feita com sucesso");
    });
}

function obterVisibilidadeCampo() {
    return VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoGNRE, _PermissoesPersonalizadasCarga) || _CONFIGURACAO_TMS.UsuarioAdministrador;
}

function obterMenuOpcoesCargasCTe() {
    var baixarEDI = { descricao: Localization.Resources.Cargas.Carga.BaixarEdi, id: guid(), metodo: baixarEDIClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadEDI };
    var baixarXMLNFSe = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDANFSE = { descricao: Localization.Resources.Cargas.Carga.BaixarDanfse, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadDANFSE };
    var baixarDACTEComp = { descricao: Localization.Resources.Cargas.Carga.BaixarDacteComp, id: guid(), metodo: baixarDacteCompClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Cargas.Carga.BaixarPdf, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var retornoSefaz = { descricao: Localization.Resources.Cargas.Carga.MensagemSefaz, id: guid(), metodo: retoronoSefazClick, icone: "", visibilidade: VisibilidadeMensagemSefaz };
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCTeClick, icone: "", visibilidade: VisibilidadeOpcaoEditar };
    var visualizar = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("CargaCTe"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
    var emitirCCe = { descricao: Localization.Resources.Cargas.Carga.CartaDeCorrecao, id: guid(), metodo: EmitirCartaCorrecaoCTeClick, icone: "", visibilidade: VisibilidadeOpcaoCartaCorrecaoCTe };
    var informarContainer = { descricao: Localization.Resources.Cargas.Carga.InformarContainer, id: guid(), metodo: informarContainerCTeClick, icone: "", visibilidade: VisibilidadeInformarContainer };
    var alterarObservacao = { descricao: Localization.Resources.Cargas.Carga.AlterarObservacao, id: guid(), metodo: alterarObservacaoCTeClick, icone: "", visibilidade: VisibilidadeAlterarObservacao };
    var emitir = {
        descricao: Localization.Resources.Cargas.Carga.Emitir, id: guid(), metodo: function (datagrid) {
            emitirCTeClick(datagrid, _cargaAtual);
        }, icone: "", visibilidade: VisibilidadeRejeicao
    };
    var protocoloIntegracao = { descricao: Localization.Resources.Cargas.Carga.ProtocoloDeIntegracao, id: guid(), metodo: protocoloIntegracaoClick, icone: "", visibilidade: VisibilidadeProtocoloIntegracao };
    let anexosContribuinte = { descricao: Localization.Resources.Cargas.Carga.AnexosContribuinte, id: guid(), metodo: abrirAnexosContribuinteClick, icone: "", visibilidade: (datagrid) => VisibilidadeInformacaoContribuinte(datagrid, true) };
    let aprovarDocContribuinte = { descricao: Localization.Resources.Cargas.Carga.AprovarDocumentoContribuinte, id: guid(), metodo: aprovarDocumentoContribuinteClick, icone: "", visibilidade: VisibilidadeInformacaoContribuinte };
    let reprovarDocContribuinte = { descricao: Localization.Resources.Cargas.Carga.ReprovarDocumentoContribuinte, id: guid(), metodo: reprovarDocumentoContribuinteClick, icone: "", visibilidade: VisibilidadeInformacaoContribuinte };
    let sincronizarDocumento = { descricao: Localization.Resources.Cargas.Carga.SincronizarDocumento, id: guid(), metodo: function (datagrid) { sincronizarCTeClick(datagrid, _cargaAtual); }, visibilidade: VisibilidadeSincronizarDocumento };
    let DesvincularCTeGerarCopia = { descricao: Localization.Resources.Cargas.Carga.DesvincularCTeEGerarCopia, id: guid(), metodo: function (datagrid) { desvincularCTeEGerarCopiaClick(datagrid, _cargaAtual); }, visibilidade: VisibilidadeDesvincularCTeEGerarCopia };
    let baixarXMLMigrate = { descricao: "Baixar XML Migrate", id: guid(), metodo: baixarXMLMigrateClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMigrate };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [] };

    if (!_cargaAtual.PossuiOcultarInformacoesCarga.val()) {
        menuOpcoes.opcoes.push(baixarDACTE);
        menuOpcoes.opcoes.push(baixarXML);
        menuOpcoes.opcoes.push(baixarXMLMigrate);
        menuOpcoes.opcoes.push(baixarDANFSE);
        menuOpcoes.opcoes.push(baixarXMLNFSe);
        menuOpcoes.opcoes.push(baixarPDF);
        menuOpcoes.opcoes.push(visualizar);
        menuOpcoes.opcoes.push(baixarDACTEComp);
    }

    menuOpcoes.opcoes.push(baixarEDI);
    menuOpcoes.opcoes.push(sincronizarDocumento);
    menuOpcoes.opcoes.push(DesvincularCTeGerarCopia);
    menuOpcoes.opcoes.push(retornoSefaz);
    menuOpcoes.opcoes.push(emitir);
    menuOpcoes.opcoes.push(editar);

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_EmitirCartaCorrecao, _PermissoesPersonalizadasCarga))
        menuOpcoes.opcoes.push(emitirCCe);

    menuOpcoes.opcoes.push(informarContainer);
    menuOpcoes.opcoes.push(alterarObservacao);
    menuOpcoes.opcoes.push(auditar);
    menuOpcoes.opcoes.push(protocoloIntegracao);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        menuOpcoes.opcoes.push(anexosContribuinte);
        menuOpcoes.opcoes.push(aprovarDocContribuinte);
        menuOpcoes.opcoes.push(reprovarDocContribuinte);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermitirAnexoDocumentoNaoContribuinte, _PermissoesPersonalizadasCarga))
        menuOpcoes.opcoes.push(anexosContribuinte);

    return menuOpcoes;
}

var AnexoEtapaCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), type: types.map, getType: typesKnockout.string, maxlength: 150, required: true, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.Arquivo.getRequiredFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true), required: true });

    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, enable: ko.observable(true) });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _anexoEtapaCTe.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
    });

    this.Anexos = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarAnexoEtapaCTeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(!_FormularioSomenteLeitura) });
};

function loadGridAnexoEtapaCTe() {
    var linhasPorPaginas = 2;
    var opcaoDownload = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: DownloadAnexoEtapaCTeClick, icone: "", visibilidade: true };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: RemoverAnexoEtapaCTeClick, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoDownload, opcaoRemover] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "30%", className: "text-align-left" }
    ];

    _gridAnexoEtapaCTe = new BasicDataTable(_anexoEtapaCTe.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoEtapaCTe.CarregarGrid([]);
}

function RemoverAnexoEtapaCTeClick(registroSelecionado) {
    executarReST("CargaCTeAnexos/ExcluirAnexo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AnexoExcluidoComSucesso);
                removerAnexoLocalCTe(registroSelecionado);
                _gridAnexoEtapaCTe.CarregarGrid(_anexoEtapaCTe.Anexos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerAnexoLocalCTe(registroSelecionado) {
    var listaAnexos = _gridAnexoEtapaCTe.BuscarRegistros();

    listaAnexos.forEach(function (anexo, i) {
        if (registroSelecionado.Codigo == anexo.Codigo) {
            listaAnexos.splice(i, 1);
        }
    });

    _anexoEtapaCTe.Anexos.val(listaAnexos);
}

function adicionarAnexoEtapaCTeClick() {
    if (!ValidarCamposObrigatorios(_anexoEtapaCTe)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var arquivo = document.getElementById(_anexoEtapaCTe.Arquivo.id);

    var anexo = {
        Codigo: guid(),
        Descricao: _anexoEtapaCTe.Descricao.val(),
        NomeArquivo: _anexoEtapaCTe.NomeArquivo.val(),
        Arquivo: arquivo.files[0],
    };

    var formData = new FormData();
    formData.append("Arquivo", anexo.Arquivo);
    formData.append("Descricao", anexo.Descricao);

    enviarArquivo("CargaCTeAnexos/AnexarArquivos", { Codigo: _cargaAtual.Codigo.val() }, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _anexoEtapaCTe.Anexos.val(retorno.Data.Anexos);
                _gridAnexoEtapaCTe.CarregarGrid(_anexoEtapaCTe.Anexos.val());
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ArquivoAnexadoComSucesso);

                LimparCampos(_anexoEtapaCTe);
                LimparCampoArquivoAnexoEtapaCTe();
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function MostrarModalAnexoEtapaCTe() {
    executarReST("Carga/BuscarAnexosDaCargaCTe", { CodigoCarga: _cargaAtual.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                Global.abrirModal("knockoutAnexoEtapaCTe");
                _anexoEtapaCTe.Anexos.val(r.Data.Anexos);
                _gridAnexoEtapaCTe.CarregarGrid(r.Data.Anexos);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });

    $("#knockoutAnexoEtapaCTe").one('hidden.bs.modal', function () {
        LimparCampos(_anexoEtapaCTe);
        LimparCampoArquivoAnexoEtapaCTe();
        _anexoEtapaCTe.Anexos.val([]);
        _gridAnexoEtapaCTe.CarregarGrid([]);
    });
}

function LimparCampoArquivoAnexoEtapaCTe() {
    _anexoEtapaCTe.Arquivo.val("");
    var arquivo = document.getElementById(_anexoEtapaCTe.Arquivo.id);
    arquivo.value = null;
}
function DownloadAnexoEtapaCTeClick(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("CargaNFeAnexos/DownloadAnexo", dados);
}

function LoadCTes() {
    if (!_HTMLCTE) {
        $.get("Content/Static/Carga/CargaCte.html?dyn=" + guid(), function (data) {
            _HTMLCTE = data;
        });
    }
}