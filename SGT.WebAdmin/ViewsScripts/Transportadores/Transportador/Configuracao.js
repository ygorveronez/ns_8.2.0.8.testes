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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="Transportador.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedidoVenda.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamentoFinanceiroSemOrcamento.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumPerfilEmpresa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />

var _configuracao;
var _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada;

var _casasQuantidade = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 }
];

var _casasValor = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 }
];

var _tipoImpressaoPedidoVenda = [
    { text: "Pedido", value: EnumTipoImpressaoPedidoVenda.Pedido },
    { text: "Contrato", value: EnumTipoImpressaoPedidoVenda.Contrato }
];

var _tipoLancamentoFinanceiroSemOrcamento = [
    { text: "Liberar", value: EnumTipoLancamentoFinanceiroSemOrcamento.Liberar },
    { text: "Avisar", value: EnumTipoLancamentoFinanceiroSemOrcamento.Avisar },
    { text: "Bloquear", value: EnumTipoLancamentoFinanceiroSemOrcamento.Bloquear }
];

var _EditouTransportador = false;

var Configuracao = function () {
    this.PrincipalFilialEmissoraTMS = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.EssaFilialPrincipalEmpresaEmissao, issue: 203, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EmpresaPadraoLancamentoGuarita = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmpresaPadraoLancamentoGuarita, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.EmitirEstadoOrigemForMesmoDaFilial = PropertyEntity({ val: ko.observable(false), issue: 202, text: Localization.Resources.Transportadores.Transportador.QuandoEstadoOrigemCargaForMesmoDestaFilialEmitirConhecimentosEla, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.SerieIntraestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieCTeDentroEstado.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieInterestadual = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieCTeForaEstado.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.SerieMDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.SerieMDFe.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.PagamentoMotoristaTipo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.TipoPagamentoMotorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), def: EnumTipoEmissaoIntramunicipal.NaoEspecificado, text: ko.observable(Localization.Resources.Transportadores.Transportador.DocumentoEmitidoParaFretesMunicipais.getFieldDescription()), required: true, visible: ko.observable(true) });
    this.SempreEmitirNFS = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.SempreGerarNotasServicoMesmoOperacoesForaMunicipio, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    
    this.TipoEmissaoIntramunicipal.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoEmissaoIntramunicipal.SempreNFSe || novoValor == EnumTipoEmissaoIntramunicipal.SempreNFSManual)
            _configuracao.SempreEmitirNFS.visible(true);
        else {
            _configuracao.SempreEmitirNFS.visible(false);
            _configuracao.SempreEmitirNFS.val(false);
        }
    });

    this.SempreEmitirNFS.val.subscribe(function (novoValor) {
        if (novoValor == true) {
            if (!_EditouTransportador) {
                exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.RealmenteDesejaEmitirNFSTodosTransportadesFeitosTransportador, function () {
                    _configuracao.TipoEmissaoIntramunicipal.text(Localization.Resources.Transportadores.Transportador.DocumentoEmitidoTodosFretes.getFieldDescription());
                }, function () {
                    _configuracao.SempreEmitirNFS.val(false);
                });
            } else {
                _configuracao.TipoEmissaoIntramunicipal.text(Localization.Resources.Transportadores.Transportador.DocumentoEmitidoTodosFretes.getFieldDescription());
            }
        } else {
            _configuracao.TipoEmissaoIntramunicipal.text(Localization.Resources.Transportadores.Transportador.DocumentoEmitidoParaFretesMunicipais.getFieldDescription());
        }
        _EditouTransportador = false;
    });

    //Configurações por Empresa do MultiNFe
    this.CalculaIBPTNFe = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.CalcularImpostoIBPTParaNotasFiscaisSaida, def: true, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.GerarParcelaAutomaticamente = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.GerarParcelaAutomaticamente, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EmitirVendaPrazoNFCe = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.EmiteVendaPrazoViaNFCe, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.HabilitaLancamentoProdutoLote = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.HabilitaLancamentoProdutoLote, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.SubtraiDescontoBaseICMS = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.SubtraiDescontoBaseIcmsNFe, def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ArmazenarDanfeParaSMS = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.ArmazenarDisponibilizarDanfeViaLink, def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AtivarEnvioDanfeSMS = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.AtivarEnvioLinkDanfeSMS, def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.HabilitarTabelaValorOrdemServicoVenda = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.HabilitarTabelaValorMaoObraOrdemServicoVenda, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.UtilizaDataVencimentoNaEmissao = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.UtilizarDataVencimentoDataEmissaoMovimentosFinanceirosTitulosProvisao, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.HabilitarEtiquetaProdutosNFe = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.HabilitarImpressaoEtiquetaNFe, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermitirImportarApenasPedidoVendaFinalizado = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.PermitirImportarApenasPedidosFinalizadosNFeNFSe, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.CadastrarProdutoAutomaticamenteDocumentoEntrada = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.CadastrarProdutoAutomaticamenteDocumentoEntrada, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DeixarPadraoFinalizadoDocumentoEntrada = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.DeixarPadraoFinalizadoDocumentoEntrada, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.ControlarEstoqueNegativo = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.PossuiControleParaNaoPermitirProdutosEstoqueNegativo, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.VisualizarSomenteClientesAssociados = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.VisualizarSomenteClientesAssociadosEmpresa, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PermiteAlterarEmpresaOrdemServicoVenda = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.PermiteAlterarEmpresaOrdemServicoVenda, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.HabilitarNumeroInternoOrdemServicoVenda = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.HabilitarNumeroInternoOrdemServicoVenda, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.TokenSMS = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TokenSmsComtele.getFieldDescription(), maxlength: 100, visible: ko.observable(false) });
    this.AtivarEnvioDanfeSMS.val.subscribe(function (novoValor) {
        _configuracao.TokenSMS.visible(novoValor);
    });

    this.CasasQuantidadeProdutoNFe = PropertyEntity({ val: ko.observable(4), options: _casasQuantidade, def: 4, text: Localization.Resources.Transportadores.Transportador.CasasDecimaisParaQuantidadeNFe.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.CasasValorProdutoNFe = PropertyEntity({ val: ko.observable(5), options: _casasValor, def: 5, text: Localization.Resources.Transportadores.Transportador.CasasDecimaisParaValorUnitarioNFe.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.TipoImpressaoPedidoVenda = PropertyEntity({ val: ko.observable(_tipoImpressaoPedidoVenda.Pedido), options: _tipoImpressaoPedidoVenda, def: _tipoImpressaoPedidoVenda.Pedido, text: Localization.Resources.Transportadores.Transportador.TipoImpressaoPedidoVenda.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.TipoLancamentoFinanceiroSemOrcamento = PropertyEntity({ val: ko.observable(_tipoLancamentoFinanceiroSemOrcamento.Liberar), options: _tipoLancamentoFinanceiroSemOrcamento, def: _tipoLancamentoFinanceiroSemOrcamento.Liberar, text: Localization.Resources.Transportadores.Transportador.TipoLançamentoFinanceiroSemOrcamento.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0) });
    this.TipoPagamentoRecebimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.ContaPagamentoRecebimento.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.NaturezaDaOperacaoNFCe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.NaturezaOperacaoPadraoNFCe.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.TipoMovimento.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(false) });
    //Fim Configurações por Empresa do MultiNFe

    this.TempoDelayHorasParaIniciarEmissao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Transportadores.Transportador.TempoHorasParaEmitirDocumentosAutomaticamenteSomenteCargasSegundoTrecho.getFieldDescription(), def: "", maxlength: 2, getType: typesKnockout.int, visible: ko.observable(false) });
    this.HoraCorteCarregamento = PropertyEntity({ text: "Hora para Corte Carregamento no dia: ", getType: typesKnockout.time, visible: ko.observable(false) });
    this.TipoEmissao = PropertyEntity({ val: ko.observable(1), def: 1, visible: ko.observable(false) }); //Usado apenas para buscar natureza tipo saída

    this.QuantidadeMaximaEmailRPS = PropertyEntity({ val: ko.observable(0), def: 0, type: types.int, text: Localization.Resources.Transportadores.Transportador.QuantidadeMaximaEmailRPS.getFieldDescription(), maxlength: 9, visible: ko.observable(true) });
    this.AliquotaICMSNegociado = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Transportadores.Transportador.AliquotaIcmsNegociada.getFieldDescription(), maxlength: 9, visible: ko.observable(true) });
    this.NumeroCertificadoIdoneidade = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.NumeroCertificadoIdoneidade.getFieldDescription(), maxlength: 3, visible: ko.observable(false) });

    this.CodigoServicoCorreios = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.CodigoServicoCorreios.getFieldDescription(), maxlength: 100, visible: _CONFIGURACAO_TMS.PossuiIntegracaoCorreios });

    this.FraseSecretaNFSe = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.FraseNFSe.getFieldDescription(), maxlength: 200, visible: ko.observable(true) });
    this.SenhaNFSe = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.SenhaPrefeituraNFSe.getFieldDescription(), maxlength: 200, visible: ko.observable(true) });


    this.CSTPISCOFINS = PropertyEntity({ type: types.map, val: ko.observable(""), def: "", getType: typesKnockout.string, text: Localization.Resources.Transportadores.Transportador.CstPisCofins.getFieldDescription(), maxlength: 5, visible: ko.observable(true) });
    this.AliquotaPIS = PropertyEntity({ val: ko.observable(""), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Transportadores.Transportador.AliquotaPIS.getFieldDescription(), maxlength: 5, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.AliquotaCOFINS = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, text: Localization.Resources.Transportadores.Transportador.AliquotaCOFINS.getFieldDescription(), maxlength: 5, visible: ko.observable(true), configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.CNPJContabilidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CNPJContabilidade.getFieldDescription(), getType: typesKnockout.cnpj, visible: ko.observable(true) });
    this.CPFContabilidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CPFContabilidade.getFieldDescription(), getType: typesKnockout.cpf, visible: ko.observable(true) });
    this.PerfilSPEDFiscal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.PerfilSpedFiscal.getFieldDescription(), val: ko.observable(EnumPerfilEmpresa.A), options: EnumPerfilEmpresa.obterOpcoes(), def: EnumPerfilEmpresa.A, visible: ko.observable(true) });
    this.GerarCreditoC197SPEDFiscal = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.GerarCreditoC197SPEDFiscal, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.FormaDeducaoValePedagio = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.FormaDeducaoValePedagio.getFieldDescription(), options: EnumFormaDeducaoValePedagio.obterOpcoes(), val: ko.observable(EnumFormaDeducaoValePedagio.NaoAplicado), def: EnumFormaDeducaoValePedagio.NaoAplicado, visible: ko.observable(false) });
    this.RestringirLocaisCarregamentoAutorizadosMotoristas = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.RestringirLocaisCarregamentoAutorizadosMotoristas.getFieldDescription(), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.TransportadoraPadraoContratacao = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.UtilizaTransportadoraPadraoContratacao.getFieldDescription(), def: false, getType: typesKnockout.bool, visible: _CONFIGURACAO_TMS.ExisteTransportadorPadraoContratacao });

    this.VersaoCTe = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.VersaoCTe.getFieldDescription(), val: ko.observable(EnumVersaoCTe.Versao400), options: EnumVersaoCTe.obterOpcoes(), def: EnumVersaoCTe.Versao400, visible: ko.observable(true) });

    //FTP
    this.UtilizaIntegracaoDocumentosDestinado = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.HabilitarIntegracaoDocumentosDestinadosViaFTP, def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EnderecoFTP = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Endereco.getRequiredFieldDescription(), maxlength: 150, enable: ko.observable(true), visible: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Usuario.getRequiredFieldDescription(), maxlength: 50, enable: ko.observable(true), visible: ko.observable(true) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Senha.getRequiredFieldDescription(), maxlength: 50, enable: ko.observable(true), visible: ko.observable(true) });
    this.Porta = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Porta.getRequiredFieldDescription(), maxlength: 10, def: "21", val: ko.observable("21"), enable: ko.observable(true), visible: ko.observable(true) });
    this.DiretorioInput = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiretarioINPUT.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(true) });
    this.DiretorioOutput = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiretorioOUTPUT.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(true) });
    this.DiretorioXML = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DiretorioXML.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(true) });
    this.Passivo = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.FTPPassivo.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.UtilizarSFTP = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SFTP, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.SSL = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SSL, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.TestarConexaoFTP = PropertyEntity({ eventClick: TestarConexaoFTP, type: types.event, text: Localization.Resources.Transportadores.Transportador.TestarConexao, visible: ko.observable(true) });
    //Fim FTP

    this.TempoDelayHorasParaIniciarEmissao.val.subscribe(function (novoValor) {
        _transportador.TempoDelayHorasParaIniciarEmissao.val(novoValor);
    });

    this.HoraCorteCarregamento.val.subscribe(function (novoValor) {
        _transportador.HoraCorteCarregamento.val(novoValor);
    });

    this.AliquotaICMSNegociado.val.subscribe(function (novoValor) {
        _transportador.AliquotaICMSNegociado.val(novoValor);
    });

    this.TipoIntegracao = PropertyEntity({ val: ko.observable(_configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada[0].value), options: _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada, text: Localization.Resources.Transportadores.Transportador.TipoIntegracao, def: _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada[0].value, required: ko.observable(false), visible: ko.observable(false), issue: 267 });
    this.HabilitarCIOT = _configuracaoCIOT.HabilitarCIOT;
    this.ObrigatoriedadeCIOTEmissaoMDFe = _configuracaoCIOT.ObrigatoriedadeCIOTEmissaoMDFe;
    this.EncerrarCIOTPorViagem = _configuracaoCIOT.EncerrarCIOTPorViagem;
    this.TipoIntegracaoCIOT = _configuracaoCIOT.TipoIntegracaoCIOT;
    this.CodigoIntegracaoEfrete = _configuracaoCIOT.CodigoIntegracaoEfrete;

    this.FormaRateioSVM = _configuracaoMultimodal.FormaRateioSVM;
    this.SVMMesmoQueMultimodal = _configuracaoMultimodal.SVMMesmoQueMultimodal;
    this.SVMTerminaisPortuarioOrigemDestino = _configuracaoMultimodal.SVMTerminaisPortuarioOrigemDestino;
    this.SVMBUSPortoOrigemDestino = _configuracaoMultimodal.SVMBUSPortoOrigemDestino;

    this.ObservacaoSimplesNacional = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ObservacaoSimplesNacional.getFieldDescription(), maxlength: 2000, enable: ko.observable(true), visible: ko.observable(false) });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ObservacaoCTe.getFieldDescription(), maxlength: 2000, enable: ko.observable(true), visible: ko.observable(true) });
    this.TagValorCreditoICMS = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracao.ObservacaoSimplesNacional.id, "#ValorCreditoICMS"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.ValorCreditoICMS });
    this.TagAliquotaSimples = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracao.ObservacaoSimplesNacional.id, "#AliquotaSimples"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.AliquotaSimples });

    this.TagCNPJEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracao.ObservacaoCTe.id, "#CNPJEmpresa"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.TagCNPJEmpresa });
    this.TagRazaoSocialEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracao.ObservacaoCTe.id, "#RazaoSocialEmpresa"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.TagRazaoSocialEmpresa });
    this.TagLocalidadeEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(_configuracao.ObservacaoCTe.id, "#LocalidadeEmpresa"); }, type: types.event, text: Localization.Resources.Transportadores.Transportador.TagLocalidadeEmpresa });

    this.NaoComprarValePedagioCargaTransbordo = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NaoComprarValePedagioParaCargasTransbordo, def: false, getType: typesKnockout.bool, visible: ko.observable(true) })
    this.EnviarNovoImposto = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EnviarNovoImposto, def: false, getType: typesKnockout.bool, visible: ko.observable(true) })
    this.ReduzirPISCOFINSBaseCalculoIBSCBS = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ReduzirPISCOFINSBaseCalculoIBSCBS, def: false, getType: typesKnockout.bool, visible: ko.observable(true) })
};

//*******EVENTOS*******

function loadConfiguracoes() {

    _configuracao = new Configuracao();
    KoBindings(_configuracao, "knockoutCadastroConfiguracao");

    $("#" + _configuracao.TestarConexaoFTP.id + "_erro").addClass("d-none");
    $("#" + _configuracao.TestarConexaoFTP.id + "_sucesso").addClass("d-none");

    BuscarSeriesCTeTransportador(_configuracao.SerieIntraestadual, null, null, null, null, _configuracao.Empresa);
    BuscarSeriesCTeTransportador(_configuracao.SerieInterestadual, null, null, null, null, _configuracao.Empresa);
    BuscarSeriesMDFeTransportador(_configuracao.SerieMDFe, null, null, null, null, _configuracao.Empresa);
    BuscarNaturezasOperacoesNotaFiscal(_configuracao.NaturezaDaOperacaoNFCe, null, null, null, null, null, null, null, _configuracao.TipoEmissao);
    BuscarTipoPagamentoRecebimento(_configuracao.TipoPagamentoRecebimento);
    BuscarTipoMovimento(_configuracao.TipoMovimento);
    BuscarPagamentoMotoristaTipo(_configuracao.PagamentoMotoristaTipo);
    configurarLayoutConfiguracaoTransportadorPorTipoSistema();
}

function configurarLayoutConfiguracaoTransportadorPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _configuracao.PrincipalFilialEmissoraTMS.visible(true);
        _configuracao.EmpresaPadraoLancamentoGuarita.visible(true);
        _configuracao.EmitirEstadoOrigemForMesmoDaFilial.visible(true);
        _configuracao.NumeroCertificadoIdoneidade.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _configuracao.CNPJContabilidade.visible(false);
        _configuracao.CPFContabilidade.visible(false);
        _configuracao.SubtraiDescontoBaseICMS.visible(false);
        _configuracao.ArmazenarDanfeParaSMS.visible(false);
        _configuracao.AtivarEnvioDanfeSMS.visible(false);
        _configuracao.PagamentoMotoristaTipo.visible(true);
        _configuracao.PerfilSPEDFiscal.visible(false);
        _configuracao.GerarCreditoC197SPEDFiscal.visible(false);
        _configuracao.TempoDelayHorasParaIniciarEmissao.visible(true);
        _configuracao.TipoIntegracao.visible(true);
        _configuracao.FormaDeducaoValePedagio.visible(true);
        _configuracao.HoraCorteCarregamento.visible(true);
        _configuracao.RestringirLocaisCarregamentoAutorizadosMotoristas.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _configuracao.EmitirEstadoOrigemForMesmoDaFilial.visible(false);
        _configuracao.SerieIntraestadual.visible(false);
        _configuracao.SerieInterestadual.visible(false);
        _configuracao.SerieMDFe.visible(false);
        _configuracao.CasasQuantidadeProdutoNFe.visible(true);
        _configuracao.CalculaIBPTNFe.visible(true);
        _configuracao.CasasValorProdutoNFe.visible(true);
        _configuracao.TipoImpressaoPedidoVenda.visible(true);
        _configuracao.GerarParcelaAutomaticamente.visible(true);
        _configuracao.EmitirVendaPrazoNFCe.visible(true);
        _configuracao.TipoPagamentoRecebimento.visible(true);
        _configuracao.NaturezaDaOperacaoNFCe.visible(true);
        _configuracao.TipoImpressaoPedidoVenda.visible(true);
        _configuracao.TipoLancamentoFinanceiroSemOrcamento.visible(true);
        _configuracao.UtilizaIntegracaoDocumentosDestinado.visible(true);
        _configuracao.TipoMovimento.visible(true);
        _configuracao.HabilitaLancamentoProdutoLote.visible(true);
        _configuracao.QuantidadeMaximaEmailRPS.visible(false);
        _configuracao.AliquotaICMSNegociado.visible(false);
        _configuracao.HabilitarTabelaValorOrdemServicoVenda.visible(true);
        _configuracao.UtilizaDataVencimentoNaEmissao.visible(true);
        _configuracao.BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual.visible(true);
        _configuracao.HabilitarEtiquetaProdutosNFe.visible(true);
        _configuracao.PermitirImportarApenasPedidoVendaFinalizado.visible(true);
        _configuracao.CadastrarProdutoAutomaticamenteDocumentoEntrada.visible(true);
        _configuracao.DeixarPadraoFinalizadoDocumentoEntrada.visible(true);
        _configuracao.ControlarEstoqueNegativo.visible(true);
        _configuracao.VisualizarSomenteClientesAssociados.visible(true);
        _configuracao.PermiteAlterarEmpresaOrdemServicoVenda.visible(true);
        _configuracao.HabilitarNumeroInternoOrdemServicoVenda.visible(true);
    }
}

function TestarConexaoFTP(e, sender) {
    executarReST("FTP/TestarConexao", {
        Host: _configuracao.EnderecoFTP.val(),
        Porta: _configuracao.Porta.val(),
        Diretorio: _configuracao.DiretorioInput.val(),
        Usuario: _configuracao.Usuario.val(),
        Senha: _configuracao.Senha.val(),
        Passivo: _configuracao.Passivo.val(),
        UtilizarSFTP: _configuracao.UtilizarSFTP.val(),
        SSL: _configuracao.SSL.val()
    }, function (r) {
        if (r.Success) {
            $("#" + _configuracao.TestarConexaoFTP.id + "_sucesso").removeClass("d-none");
            $("#" + _configuracao.TestarConexaoFTP.id + "_erro").addClass("d-none");
        } else {
            $("#" + _configuracao.TestarConexaoFTP.id + "_sucesso").addClass("d-none");
            $("#" + _configuracao.TestarConexaoFTP.id + "_erro").removeClass("d-none");
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function limparCamposConfiguracoes() {
    LimparCampos(_configuracao);
    $("#liTabConfiguracoes").hide();
    Global.ResetarAbas();
}