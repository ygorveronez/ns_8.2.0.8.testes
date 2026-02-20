/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/PagamentoMotoristaTipo.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Enumeradores/EnumTipoInclusaoPedagioBaseCalculoICMS.js" />
/// <reference path="../../Enumeradores/EnumVersaoNFe.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumRegimeEspecial.js" />
/// <reference path="Certificado.js" />
/// <reference path="CodigosComercialDistribuidor.js" />
/// <reference path="CondicaoPagamento.js" />
/// <reference path="Configuracao.js" />
/// <reference path="ConfiguracaoCIOT.js" />
/// <reference path="ConfiguracaoNFSe.js" />
/// <reference path="ConfiguracaoTipoOperacao.js" />
/// <reference path="Filiais.js" />
/// <reference path="EstadosFeeder.js" />
/// <reference path="FiliaisEmbarcador.js" />
/// <reference path="ImpostosCIOT.js" />
/// <reference path="IntegracaoKrona.js" />
/// <reference path="LayoutEDI.js" />
/// <reference path="OpenTech.js" />
/// <reference path="Migrate.js" />
/// <reference path="Pallets.js" />
/// <reference path="Permissao.js" />
/// <reference path="PermissaoTransportador.js" />
/// <reference path="Seguro.js" />
/// <reference path="Serie.js" />
/// <reference path="DadoBancario.js" />
/// <reference path="TransportadorFiliais.js" />
/// <reference path="Usuario/Usuario.js" />
/// <reference path="IntelipostDadosIntegracao.js" />
/// <reference path="IntelipostTipoOcorrencia.js" />
/// <reference path="TransportadorAnexo.js" />
/// <reference path="RotaFreteValePedagio.js" />
/// <reference path="TransportadorComponentesCTesImportados.js" />
/// <reference path="InscricaoST.js" />
/// <reference path="TransportadorNFSe.js" />
/// <reference path="AutomacaoNFSManual.js" />
/// <reference path="CodigosIntegracao.js" />
/// <reference path="LeituraFTP.js" />
/// <reference path="TermoQuitacao.js" />
/// <reference path="../../Enumeradores/EnumRegimeTributarioCTe.js" />
/// <reference path="Repom.js" />
/// <reference path="Electrolux.js" />
/// <reference path="Migrate.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _chaptcha;
var _configuracaoLayoutEDI;
var _cookies;
var _crudTransportador;
var _gridTransportador;
var _pesquisaDadosReceita;
var _pesquisaTransportador;
var _transportador;
var _politicaSenha;
var _configuracaoEmissaoCTeOpcoesTipoIntegracao;
var _PermissoesPersonalizadas;

var _fusoHorario = [
    { text: "(UTC-02:00) Coordinated Universal Time-02", value: "UTC-02" },
    { text: "(UTC-03:00) Brasilia (Horario de Verão)", value: "E. South America Standard Time" },
    { text: "(UTC-03:00) Araguaina (Horario de Verão)", value: "Tocantins Standard Time" },
    { text: "(UTC-03:00) Cayenne, Fortaleza", value: "SA Eastern Standard Time" },
    { text: "(UTC-04:00) Cuiaba (Horario de Verão)", value: "Central Brazilian Standard Time" },
    { text: "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", value: "SA Western Standard Time" },
    { text: "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", value: "SA Pacific Standard Time" }
];

var _statusChar = [
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _versaoNFe = [
    { text: "3.10", value: EnumVersaoNFe.Versao310 },
    { text: "4.00", value: EnumVersaoNFe.Versao400 }
];

//var _tipoEmpresa = [
//    { text: "Física", value: "F" },
//    { text: "Jurídica", value: "J" },
//    { text: "Exterior", value: "E" }
//];

/*
 * Declaração das Classes
 */

var PesquisaTransportador = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.RazaoSocial.getFieldDescription(), cssClass: ko.observable("col col-6") });
    this.NomeFantasia = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NomeFantasia.getFieldDescription() });
    this.CNPJ = PropertyEntity({ text: _CONFIGURACAO_TMS.SistemaEstrangeiro ? Localization.Resources.Transportadores.Transportador.Identificacao.getFieldDescription() : Localization.Resources.Transportadores.Transportador.CNPJCPF.getFieldDescription(), getType: _CONFIGURACAO_TMS.SistemaEstrangeiro ? typesKnockout.string : typesKnockout.cpfCnpj });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.SistemaEmissor = PropertyEntity({ val: ko.observable(EnumSistemaEmissor.Todos), options: EnumSistemaEmissor.obterOpcoes(), def: EnumSistemaEmissor.Todos, visible: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.TipoEmissao.getFieldDescription() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getFieldDescription(), getType: typesKnockout.string });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTransportador.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Transportadores.Transportador.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PesquisaDadosReceita = function () {

    this.ImagemCaptcha = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DigiteCodigoImagemLado.getFieldDescription(), src: ko.observable(""), maxlength: 6, required: true });
    this.EnviarCatptch = PropertyEntity({ eventClick: enviarCNPJClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.Consultar, visible: ko.observable(true) });
    this.Captcha = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.Captcha.getFieldDescription()), enable: ko.observable(true), maxlength: 6, visible: ko.observable(true) });

    this.BuscarNovoCaptcha = PropertyEntity({ eventClick: validarCNPJClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.Continuar, visible: ko.observable(true) });
};

var Transportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.RazaoSocial = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription()), required: true, maxlength: 80 });
    this.CodigoIntegracao = PropertyEntity({ text: _CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador ? ko.observable(Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getRequiredFieldDescription()) : ko.observable("Código Integração: "), issue: 15, required: ko.observable(_CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador ? true : false), maxlength: 50 });
    this.NomeFantasia = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NomeFantasia.getRequiredFieldDescription(), required: true });
    this.TipoEmpresa = PropertyEntity({ val: ko.observable("J"), options: ObterTiposEmpresa(), text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), def: "J", enable: ko.observable(true), eventChange: tipoEmpresaChange, visible: ko.observable(true) });
    this.SistemaEstrangeiro = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.SistemaEstrangeiro), def: _CONFIGURACAO_TMS.SistemaEstrangeiro });
    this.CNPJ = PropertyEntity({ text: ko.observable(_CONFIGURACAO_TMS.SistemaEstrangeiro ? Localization.Resources.Transportadores.Transportador.Identificacao.getFieldDescription() : Localization.Resources.Transportadores.Transportador.CNPJ.getRequiredFieldDescription()), issue: 4, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro), getType: _CONFIGURACAO_TMS.SistemaEstrangeiro ? typesKnockout.string : typesKnockout.cnpj, enable: ko.observable(true), visible: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.CPF.getRequiredFieldDescription()), required: false, getType: typesKnockout.cpf, enable: ko.observable(true), visible: ko.observable(false) });
    this.InscricaoEstadual = PropertyEntity({ text: ko.observable((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : "*") + Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription()), issue: 744, maxlength: 20, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro) });
    this.InscricaoMunicipal = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.InscricaoMunicipal.getFieldDescription(), issue: 750, maxlength: 20, required: false, visible: true });
    this.CNAE = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CNAE.getFieldDescription(), issue: 746, maxlength: 20, required: false });
    this.Suframa = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Suframa.getFieldDescription(), issue: 742, maxlength: 20 });
    this.Setor = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Setor.getFieldDescription(), maxlength: 20 });
    this.CEP = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CEP.getRequiredFieldDescription(), maxlength: 10, required: true });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.Cidade.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.ModeloDocumentoFiscalCargaPropria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Transportadores.Transportador.EmitirOutroModeloDocumento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Endereco.getRequiredFieldDescription(), required: true });
    this.Numero = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Numero.getRequiredFieldDescription(), required: true });
    this.Bairro = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Bairro.getRequiredFieldDescription(), required: true });
    this.Complemento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Complemento.getFieldDescription(), maxlength: 100 });
    this.Telefone = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Telefone.getRequiredFieldDescription(), issue: 749, required: true, getType: typesKnockout.phone });
    this.Contato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.NomeContato.getFieldDescription()), issue: 747 });
    this.TelefoneContato = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TelefoneContato.getFieldDescription(), issue: 748, getType: typesKnockout.phone });
    this.NomeContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NomeContador.getFieldDescription() });
    this.CRCContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CRCContador.getFieldDescription(), maxlength: 20 });
    this.TelefoneContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.TelefoneContador.getFieldDescription(), getType: typesKnockout.phone });
    this.Contador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.Contador.getFieldDescription(), idBtnSearch: guid() });
    this.PossuiCertificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.PossuiCertificado });
    this.DataInicialCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataInicioCertificado.getRequiredFieldDescription(), getType: typesKnockout.date, required: false });
    this.DataFinalCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataFinalCertificado.getRequiredFieldDescription(), getType: typesKnockout.date, required: false });
    this.SerieCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SerieCertificado.getRequiredFieldDescription(), required: false });
    this.SenhaCertificado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.SenhaCertificado.getRequiredFieldDescription(), required: false });
    this.LiberacaoParaPagamentoAutomatico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: true, text: Localization.Resources.Transportadores.Transportador.IntegracaoAutomaticaCteGold, visible: _CONFIGURACAO_TMS.PermitirAutomatizarPagamentoTransportador });
    this.OptanteSimplesNacional = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.SimplesNacional, issue: 752 });
    this.OptanteSimplesNacionalComExcessoReceitaBruta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.ExcessoSublimiteReceitaBruta, visible: ko.observable(false) });
    this.ExigeEtiquetagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.ExigeEtiquetagem, visible: ko.observable(false) });
    this.EmissaoDocumentosForaDoSistema = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EsteTransportadorFaraEmissaoDocumentosForaMultiEmbarcador, visible: ko.observable(false) });
    this.EmissaoMDFeForaDoSistema = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EsteTransportadorFaraEmissaoMdfesForaMultiEmbarcador, issue: 753, visible: ko.observable(false) });
    this.EmissaoCRTForaDoSistema = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EmissaoCRTForaDoSistema, visible: ko.observable(false) });
    this.PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.PercentualToleranciaDiferencaValoresEntreCteEsperadPreCteEecebido.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true), val: ko.observable(0), maxlength: 5 });

    this.RegimenTributario = PropertyEntity({ val: ko.observable(EnumRegimenTributacao.NaoSelecionado), options: EnumRegimenTributacao.ObterOpcoes(), text: Localization.Resources.Transportadores.Transportador.RegimenTributario, visible: ko.observable(false) });
    this.RegimeTributarioCTe = PropertyEntity({ val: ko.observable(EnumRegimeTributarioCTe.NaoSelecionado), options: EnumRegimeTributarioCTe.obterOpcoesNaoSelecionado(), text: Localization.Resources.Transportadores.Transportador.RegimeTributarioCTe.getFieldDescription(), visible: ko.observable(true) });


    this.EmissaoDocumentosForaDoSistema.val.subscribe(function (valor) {
        visibilidadeAbaComponentesCTesImportados(valor);
    });

    this.EmiteMDFe20IntraEstadual = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EsteTransportadorFaraEmissaoMdfeVersaoDoisZeroViagensIntraestaduais, issue: 754, visible: ko.observable(false) });
    this.PermiteEmitirSubcontratacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.PermitirEmitirCTesSubcontratacaoCTtesDesteTransportadorOndeSeraTomador, issue: 755, visible: ko.observable(false) });
    this.UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.UsarTransportadorFilialEmissoraPadraoEedespachosIniciadosEstado, visible: ko.observable(false) });
    this.DataUltimaConsultaSintegra = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataUltimaConsultaSintegra.getFieldDescription(), getType: typesKnockout.date, enable: false, visible: ko.observable(false) });
    this.DataProximaConsultaSintegra = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataProximaConsultaSintegra.getFieldDescription(), getType: typesKnockout.date, enable: true, visible: ko.observable(false) });
    this.CodigoCentroCusto = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoCentroCusto.getFieldDescription(), maxlength: 100, visible: ko.observable(true) });
    this.CodigoEmpresa = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoEmpresa.getFieldDescription(), maxlength: 50, visible: ko.observable(true) });
    this.CodigoEstabelecimento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoEstabelecimento.getFieldDescription(), maxlength: 50, visible: ko.observable(true) });
    this.CodigoDocumento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoDocumentacao.getFieldDescription(), required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataInicioAtividade = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.DataInicioAtividade.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.PontuacaoFixa = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.PontuacaoFixa.getFieldDescription()), getType: typesKnockout.int, required: false, visible: ko.observable(true) });
    this.COTM = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.COTM.getFieldDescription(), maxlength: 20, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFeAdmin) });
    this.IntegrarComGerenciadoraDeRisco = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EssaTransportadoraFaraIntegracaoGerenciadoraRisco, issue: 1382, visible: ko.observable(_PossuiIntegracaoGerenciadoraRisco) });
    this.EmpresaMobile = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Transportador.EssaEmpresaUtilizaAplicativoMultiMobile, def: false, visible: ko.observable(false) });
    this.OrdenarCargasMobileCrescente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Transportador.OrdenarCargasMobileCrescente, def: false, visible: ko.observable(!_CONFIGURACAO_TMS.OrdenarCargasMobileCrescente) });
    this.RecusarIntegracaoPODUnilever = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Transportador.RecusarIntegracaoPODUnilever, def: false, visible: _CONFIGURACAO_TMS.PossuiIntegracaoUnilever });
    this.CompraValePedagio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.ComprarValePedagio, visible: ko.observable(false) });
    this.CompraValePedagio.val.subscribe(alterouCompraValePedagio);
    this.IntegrarCorreios = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.IntegrarCorreios, visible: ko.observable(false) });

    this.GerarPedidoAoReceberCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.GerarPedidoReceberIntegracaoCarga, visible: ko.observable(true) });
    this.GerarLoteEscrituracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.GerarLoteEscrituracao, visible: ko.observable(true) });
    this.GerarLoteEscrituracaoCancelamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.GerarLoteEscrituracaoParaCancelamentos, visible: ko.observable(true) });
    this.ProvisionarDocumentos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.ProvisionarDocumentos, visible: ko.observable(true) });
    this.EmpresaPropria = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.TransportadoraParaCargasPropriasEmiteApenasMdfe, visible: ko.observable(true) });
    this.ValidarMotoristaTeleriscoAoConfirmarTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: false, text: Localization.Resources.Transportadores.Transportador.ValidarMotoristaTelerisco, visible: ko.observable(false) });

    this.TransportadorFerroviario = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.TransportadorFerroviario, visible: ko.observable(true) });
    this.PermitirUtilizarCadastroAgendamentoColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.PermitirUtilizarCadastroAgendamentoColeta, visible: ko.observable(true) });
    this.BloquearTransportador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.BloquearTransportador, visible: ko.observable(false) });

    this.NotificarDestinatarioAgendamentoColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.NotificarDestinatarioAgendamentoColeta, visible: ko.observable(false) });
    this.EmpresaRetiradaProduto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false });

    this.BloquearTransportador.val.subscribe(function (valor) {
        if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador, _PermissoesPersonalizadas))
            return;

        _transportador.MotivoBloqueio.visible(valor);
    });

    this.EmpresaRetiradaProduto.val.subscribe(function (valor) {
        _transportador.NotificarDestinatarioAgendamentoColeta.visible(valor);
    });

    this.MotivoBloqueio = PropertyEntity({ maxlength: 150, text: Localization.Resources.Transportadores.Transportador.MotivoBloqueio.getFieldDescription(), visible: ko.observable(false) });

    this.RegistroANTT = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Transportador.RNTRC.getRequiredFieldDescription()), issue: 660, maxlength: 8, required: ko.observable(!_CONFIGURACAO_TMS.SistemaEstrangeiro), cssClass: ko.observable("col col-2") });
    this.FusoHorario = PropertyEntity({ val: ko.observable("E. South America Standard Time"), options: _fusoHorario, def: "E. South America Standard Time", text: Localization.Resources.Transportadores.Transportador.FusoHorario.getRequiredFieldDescription(), issue: 65, required: true });
    this.TipoAmbiente = PropertyEntity({ val: ko.observable(EnumTipoAmbiente.Homologacao), options: EnumTipoAmbiente.obterOpcoes(), def: EnumTipoAmbiente.Homologacao, text: Localization.Resources.Transportadores.Transportador.TipoAmbiente.getRequiredFieldDescription(), issue: 68, required: true });
    this.TipoInclusaoPedagioBaseCalculoICMS = PropertyEntity({ val: ko.observable(EnumTipoInclusaoPedagioBaseCalculoICMS.UtilizarPadrao), options: EnumTipoInclusaoPedagioBaseCalculoICMS.obterOpcoes(), def: EnumTipoInclusaoPedagioBaseCalculoICMS.UtilizarPadrao, text: Localization.Resources.Transportadores.Transportador.IcvsPedagio.getRequiredFieldDescription(), issue: 751 });
    this.Status = PropertyEntity({ val: ko.observable("A"), options: _statusChar, def: "A", text: Localization.Resources.Transportadores.Transportador.Situacao.getRequiredFieldDescription(), issue: 557, required: true });
    this.Email = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroTransportador || _CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas ? "*" : "") + Localization.Resources.Transportadores.Transportador.Email.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, required: _CONFIGURACAO_TMS.ExigirEmailPrincipalCadastroTransportador, maxlength: 1000 });
    this.EmailEnvioCanhoto = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailEnvioCanhoto.getFieldDescription(), maxlength: 1000 });
    this.EmailEnvioCTeRejeitado = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailEnvioCTeRejeitado.getFieldDescription(), maxlength: 1000 });
    this.EmailAdministrativo = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailAdministrativo.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000 });
    this.EmailContador = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EmailContador.getFieldDescription(), issue: 30, getType: typesKnockout.multiplesEmails, maxlength: 1000 });

    this.EnviarEmail = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailAdministrativo = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });
    this.EnviarEmailContador = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: " XML" });

    this.DiasRotatividadePallets = PropertyEntity({ val: ko.observable(""), def: "" });
    this.TempoDelayHorasParaIniciarEmissao = PropertyEntity({ val: ko.observable(""), def: "" });
    this.HoraCorteCarregamento = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.HoraParaCorteCarregamentoNoDia.getFieldDescription(), getType: typesKnockout.time, visible: ko.observable(true) });

    this.AliquotaICMSNegociado = PropertyEntity({ val: ko.observable(""), def: "" });

    this.CodigosComercialDistribuidor = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.ProximoNumeroNFe = PropertyEntity({ val: ko.observable(1), def: 1, text: Localization.Resources.Transportadores.Transportador.ProximoNumeroNFe.getFieldDescription(), required: false, getType: typesKnockout.int, visible: ko.observable(false) });
    this.ProximoNumeroNFCe = PropertyEntity({ val: ko.observable(1), def: 1, text: Localization.Resources.Transportadores.Transportador.ProximoNumeroNFe.getFieldDescription(), required: false, getType: typesKnockout.int, visible: ko.observable(false) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.ArquivoNFe.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false), enable: ko.observable(true) });
    this.ImportarXML = PropertyEntity({ eventClick: importarXMLClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.ImportarXmlNFeAnterior, visible: ko.observable(false) });
    this.CaminhoLogoSistema = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CaminhoLogoSistema.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.RegimeEspecial = PropertyEntity({ val: ko.observable(EnumRegimeEspecial.Nenhum), options: EnumRegimeEspecial.obterOpcoes(), text: Localization.Resources.Transportadores.Transportador.RegimeTributarioEspecial.getFieldDescription(), visible: ko.observable(true) });

    this.IMO = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.IMO, visible: ko.observable(true) });
    this.DataValidadeIMO = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataValidadeIMO, getType: typesKnockout.date, visible: ko.observable(true), val: ko.observable("") });

    this.IdTokenNFCe = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.TokenNFCe.getFieldDescription(), maxlength: 6, required: false, getType: typesKnockout.string, visible: ko.observable(false) });
    this.IdCSCNFCe = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.CscNFCe.getFieldDescription(), required: false, getType: typesKnockout.string, visible: ko.observable(false) });
    this.StatusFinanceiro = PropertyEntity({ val: ko.observable(EnumStatusFinanceiroTransportador.Normal), options: EnumStatusFinanceiroTransportador.obterOpcoes(), def: EnumStatusFinanceiroTransportador.Normal, text: Localization.Resources.Transportadores.Transportador.Statusfinanceiro.getRequiredFieldDescription(), required: false, visible: ko.observable(false) });
    this.AliquotaICMSSimples = PropertyEntity({ val: ko.observable("0,00"), def: "", text: Localization.Resources.Transportadores.Transportador.AliquotaIcmsSimples.getFieldDescription(), required: false, getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.SerieRPS = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Transportadores.Transportador.SerieRPS.getFieldDescription(), required: false, getType: typesKnockout.string, visible: ko.observable(false), maxlength: 10 });
    this.VersaoNFe = PropertyEntity({ val: ko.observable(EnumVersaoNFe.Versao400), options: _versaoNFe, def: EnumVersaoNFe.Versao400, text: Localization.Resources.Transportadores.Transportador.VersaoNFe.getRequiredFieldDescription(), required: false, visible: ko.observable(false) });

    this.TipoIntegracaoValePedagio = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.OperadoraValePedagio.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: _configuracaoEmissaoCTeOpcoesTipoIntegracao, def: [], visible: ko.observable(false) });

    this.ConfiguracaoTipoOperacaos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Series = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ImpostoRendaCIOT = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable() });
    this.ImpostoCIOTBaseCalculoIR = PropertyEntity({ val: function () { return _impostoRendaCIOT.BaseCalculo.val.apply(null, arguments); } });
    this.ImpostoINSSCIOT = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable() });
    this.ImpostoCIOTBaseCalculoINSS = PropertyEntity({ val: function () { return _impostoINSSCIOT.BaseCalculo.val.apply(null, arguments); } });
    this.ImpostoCIOTTetoRetencaoINSS = PropertyEntity({ val: function () { return _impostoINSSCIOT.TetoRetencao.val.apply(null, arguments); } });
    this.ImpostoCIOTAliquotaSEST = PropertyEntity({ val: function () { return _impostoSESTCIOT.Aliquota.val.apply(null, arguments); } });
    this.ImpostoCIOTAliquotaSENAT = PropertyEntity({ val: function () { return _impostoSENATCIOT.Aliquota.val.apply(null, arguments); } });

    //this.LayoutsEDI = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ConfiguracaoLayoutEDI = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.Operadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.EstadosFeeder = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Filiais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.FiliaisEmbarcador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.IntelipostDadosIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.IntelipostTipoOcorrencia = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.RotasFreteValePedagio = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.InscricoesST = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Permissoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.PerfilAcessoTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(0) });
    this.TransportadorAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.HabilitaSincronismoDocumentosDestinados = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.FormulariosTransportador = PropertyEntity({ type: types.listEntity, codEntity: ko.observable(0), list: new Array() });
    this.ModulosTransportador = PropertyEntity({ type: types.listEntity, codEntity: ko.observable(0), list: new Array() });

    this.Configuracao = PropertyEntity({ getType: typesKnockout.dynamic });
    this.DadoBancario = PropertyEntity({ getType: typesKnockout.dynamic });
    this.LeituraFTP = PropertyEntity({ getType: typesKnockout.dynamic });
    this.TermoQuitacao = PropertyEntity({ getType: typesKnockout.dynamic });
    this.ComponentesCTesImportados = PropertyEntity({ getType: typesKnockout.dynamic });

    this.ValidarCNPJ = PropertyEntity({ eventClick: validarCNPJClick, type: types.event, text: Localization.Resources.Gerais.Geral.Validar, visible: ko.observable(true), enable: ko.observable(true) });

    this.DiasRotatividadePallets.val.subscribe(function (novoValor) {
        _pallets.DiasRotatividadePallets.val(novoValor);
    });

    this.TempoDelayHorasParaIniciarEmissao.val.subscribe(function (novoValor) {
        _configuracao.TempoDelayHorasParaIniciarEmissao.val(novoValor);
    });

    this.HoraCorteCarregamento.val.subscribe(function (novoValor) {
        _configuracao.HoraCorteCarregamento.val(novoValor);
    });

    this.AliquotaICMSNegociado.val.subscribe(function (novoValor) {
        _configuracao.AliquotaICMSNegociado.val(novoValor);
    });

    this.UsarTipoOperacaoApolice = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.LiberarEmissaoSemAverbacao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.LiberarParaCargasParaEmissaoMesmoAverbarDocumentos });

    this.OpenTech = PropertyEntity({ getType: typesKnockout.map });
    this.CodigoClienteOpenTech = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoPASOpenTech = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Migrate = PropertyEntity({ getType: typesKnockout.map });

    this.TransportadorFiliais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.TransportadorNFSe = PropertyEntity({ getType: typesKnockout.dynamic });
    this.AutomacaoEmissaoNFSManual = PropertyEntity({ getType: typesKnockout.dynamic });
    this.CodigosIntegracao = PropertyEntity({ getType: typesKnockout.dynamic });

    this.Pais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "País", idBtnSearch: guid(), visible: ko.observable(true) });

    this.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro });

    this.Repom = PropertyEntity({ getType: typesKnockout.dynamic });
    this.Electrolux = PropertyEntity({ getType: typesKnockout.dynamic });

    this.Contribuinte = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.Contribuinte, visible: ko.observable(true) });
    this.DataValidadeContribuinte = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.DataValidadeContribuinte, getType: typesKnockout.date, visible: ko.observable(false), val: ko.observable("") });
    this.ValidarTransportadorContribuinte = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.ValidarTransportadorContribuinte, visible: ko.observable(true) });
    this.EquiparadoTAC = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.EquiparadoTac, visible: ko.observable(true) });
    this.NaoGerarSMNaBrk = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.NaoGerarSMNaBrk, visible: ko.observable(true) });
    this.IgnorarDocumentosDuplicadosNaEmissaoCTe = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Transportadores.Transportador.IgnorarDocumentosDuplicadosNaEmissaoCTe, visible: ko.observable(true) });    
    this.NaoPermitirInformarInicioEFimPreTrip = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.PermitirInformarInicioEFimPreTrip, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.NaoPermitirReenviarIntegracaoDasCargasAppTrizy = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.PermitirReenviarIntegracaoDasCargasAppTrizy, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.NaoGerarIntegracaoSuperAppTrizy = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NaoGerarIntegracaoComOSuperAppTrizy, getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MostrarOcorrenciasFiliaisMatriz = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.MostrarOcorrenciasFiliaisMatriz, getType: typesKnockout.bool, val: ko.observable(false), def: false });
};

var CRUDTransportador = function () {
    this.ReenviarDadosAcessoaCliente = PropertyEntity({ eventClick: reenviarDadosAcessoaClienteClick, type: types.event, text: Localization.Resources.Transportadores.Transportador.ReenviarDadosAcesso, visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function ObterTiposEmpresa() {
    var p = new promise.Promise();

    _tipoEmpresa = new Array();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        _tipoEmpresa.push({ value: "F", text: Localization.Resources.Transportadores.Transportador.Fisica });
        _tipoEmpresa.push({ value: "J", text: Localization.Resources.Transportadores.Transportador.Juridica });
    }
    else {
        _tipoEmpresa.push({ value: "J", text: Localization.Resources.Transportadores.Transportador.Juridica });
        _tipoEmpresa.push({ value: "E", text: Localization.Resources.Transportadores.Transportador.Exterior });
    }

    return _tipoEmpresa;
}

function BuscarConfiguracao() {
    executarReST("Transportador/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            var data = retorno.Data;

            if (data.TemIntegracaoIntelipost)
                $("#liIntelipostDadosIntegracao").show();

            if (data.TemIntegracaoKrona)
                $("#liTabIntegracaoKrona").show();

            if (data.TemIntegracaoOpenTech) {
                $("#liTabOpenTech").show();
                _openTech.PossuiIntegracaoOpenTech.val(true);
            }
            if (data.TemIntegracaoMigrate) {
                $("#liTabMigrate").show();
            }

            if (data.TemIntegracaoRepom) {
                $("#liTabRepom").show();
                _repom.PossuiIntegracaoRepom.val(true);
            }

            if (data.TemIntegracaoElectrolux) {
                $("#liTabElectrolux").show();
                _electrolux.PossuiIntegracaoElectrolux.val(true);
            }

            if (data.TemIntegracaoRepomRest)
                $("#liTabRotasValePedagio").show();

            if (data.TemIntegracaoSintegra) {
                _transportador.DataUltimaConsultaSintegra.visible(true);
                _transportador.DataProximaConsultaSintegra.visible(true);
            }
        }
    });
}

function ObterTiposIntegracao() {
    var p = new promise.Promise();

    executarReST("TipoIntegracao/BuscarTodos", {
        Tipos: JSON.stringify([
            EnumTipoIntegracao.NaoInformada,
            EnumTipoIntegracao.SemParar,
            EnumTipoIntegracao.Target,
            EnumTipoIntegracao.PagBem,
            EnumTipoIntegracao.Repom,
            EnumTipoIntegracao.DBTrans,
            EnumTipoIntegracao.Pamcard,
            EnumTipoIntegracao.QualP,
            EnumTipoIntegracao.EFrete,
            EnumTipoIntegracao.Extratta,
            EnumTipoIntegracao.DigitalCom,
            EnumTipoIntegracao.RepomFrete,
            EnumTipoIntegracao.NDDCargo])
    }, function (r) {
        if (r.Success) {
            _configuracaoEmissaoCTeOpcoesTipoIntegracao = new Array();
            _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada = new Array();

            for (var i = 0; i < r.Data.length; i++) {
                _configuracaoEmissaoCTeOpcoesTipoIntegracaoComNaoInformada.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });

                if (r.Data[i].Codigo != EnumTipoIntegracao.NaoInformada)
                    _configuracaoEmissaoCTeOpcoesTipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }

        p.done();
    });

    return p;
}


function loadTransportador() {
    ObterTiposIntegracao().then(function () {

        _transportador = new Transportador();
        KoBindings(_transportador, "knockoutCadastroTransportador");

        _crudTransportador = new CRUDTransportador();
        KoBindings(_crudTransportador, "knockoutCRUDTransportador");

        _pesquisaTransportador = new PesquisaTransportador();
        KoBindings(_pesquisaTransportador, "knockoutPesquisaTransportador", false, _pesquisaTransportador.Pesquisar.id);

        HeaderAuditoria(Localization.Resources.Transportadores.Transportador.Empresa, _transportador);

        $("#" + _transportador.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
        $("#" + _transportador.RegistroANTT.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
            _transportador.Contato.text(Localization.Resources.Transportadores.Transportador.ContatoSkype.getFieldDescription());
            _transportador.CaminhoLogoSistema.visible(true);
            _transportador.StatusFinanceiro.visible(true);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
            $("#liTabCodigosComercialDistribuidor").show();
            $("#liTabDadosBancarios").hide();

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador, _PermissoesPersonalizadas))
                _transportador.BloquearTransportador.visible(true);
        }

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
            $("#lblTitle").text(Localization.Resources.Transportadores.Transportador.Empresas);
            $("#lblPesquisa").text(Localization.Resources.Transportadores.Transportador.PesquisarEmpresas);
            $("#lblAba").text(Localization.Resources.Transportadores.Transportador.Empresa);
            $("#lblCadastro").text(Localization.Resources.Transportadores.Transportador.RegistrarEmpresa);
            _transportador.RegistroANTT.text(Localization.Resources.Transportadores.Transportador.RNTRC);
            _transportador.RegistroANTT.required(false);

            _transportador.ProximoNumeroNFe.visible(false);
            _transportador.ProximoNumeroNFCe.visible(false);
            _transportador.ImportarXML.visible(true);
            _transportador.Arquivo.visible(true);

            _transportador.IdTokenNFCe.visible(true);
            _transportador.IdCSCNFCe.visible(true);
            _transportador.SerieRPS.visible(true);
            _transportador.AliquotaICMSSimples.visible(true);
            //_transportador.VersaoNFe.visible(true);
            //_transportador.TipoEmpresa.visible(true);

            _transportador.CodigoCentroCusto.visible(false);
            _transportador.GerarLoteEscrituracao.visible(false);
            _transportador.GerarLoteEscrituracaoCancelamento.visible(false);
            _transportador.ProvisionarDocumentos.visible(false);
            _transportador.TipoIntegracaoValePedagio.visible(false);
            _transportador.EmpresaPropria.visible(false);
            _transportador.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(false);
            _transportador.GerarPedidoAoReceberCarga.visible(false);
            _transportador.ModeloDocumentoFiscalCargaPropria.visible(false);
            _transportador.PontuacaoFixa.visible(false);

        }
        else {
            $("#lblTitle").text(Localization.Resources.Transportadores.Transportador.DescricaoTransportador);
            $("#lblPesquisa").text(Localization.Resources.Transportadores.Transportador.PesquisarTransportadores);
            $("#lblAba").text(Localization.Resources.Transportadores.Transportador.DescricaoTransportador);
            $("#lblCadastro").text(Localization.Resources.Transportadores.Transportador.CadastrarTransportador);
        }

        new BuscarLocalidades(_transportador.Localidade);
        new BuscarModeloDocumentoFiscal(_transportador.ModeloDocumentoFiscalCargaPropria, null, null, true);
        new BuscarPaises(_transportador.Pais);

        buscarTransportadors();

        loadCodigoComercialDistribuidorTransportador();
        loadSerie();
        loadFilial();
        loadEstadoFeeder();
        loadFilialEmbarcador();
        loadIntelipostDadosIntegracao();
        loadIntelipostTipoOcorrencia();
        loadPermissao();
        loadPermissaoTransportador();
        loadConfiguracoesCIOT();
        loadConfiguracoesMultimodal();
        loadConfiguracaoLogo();
        loadConfiguracoes();
        loadTransportadorNFSe();
        loadPallets();
        loadSeguro();
        LoadOpenTech();
        LoadMigrate();
        LoadRepom();
        LoadElectrolux();
        loadTransportadorFilial();
        loadImpostosCIOT();
        loadCondicaoPagamento();
        loadIntegracaoKrona();
        loadDadoBancario();
        carregarConteudosHTML();
        configurarIntegracoesDisponiveis();
        loadAnexoTransportador();
        loadComponentesCTeImportados();
        loadRotaFreteValePedagio();
        loadInscricaoST();
        loadTransportadorCodigosIntegracao();
        loadCertificado();
        loadOperadores();
        verificarSeExisteEscalationList();
        loadLeituraFTP();
        loadTermoQuitacao();

        _transportador.IntegrarComGerenciadoraDeRisco.val.subscribe(IntegrarComGerenciadoraDeRiscoChange);
        _transportador.OptanteSimplesNacional.val.subscribe(OptanteSimplesNacionalChange);
        _transportador.Contribuinte.val.subscribe(ContribuinteChange);

        _configuracaoLayoutEDI = new ConfiguracaoLayoutEDI("divConfiguracaoLayoutEDI", _transportador.ConfiguracaoLayoutEDI);

        BuscarConfiguracao();

        loadConfiguracaoTipoOperacao(function () {
            if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
                $("#liTabConfiguracaoMultimodal").show();
                $("#liTabEstadosFeeder").show();
            }

            $("#liTabTransportadorFiliais").hide();
            $("#liTabConfiguracaoCIOT").hide();
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
                $("#liTabConfiguracoes").hide();
                $("#liTabDadosBancarios").show();
                $("#liTabCertificado").show();
                $("#liTabPermissoes").hide();
                $("#liTabPermissoesTransportador").hide();
                $("#liTabUsuarios").hide();
                $("#liTabSeries").show();
                $("#liTabLayoutsEDI").hide();
                $("#liTabFiliais").hide();
                $("#liTabFiliaisEmbarcador").hide();
                $("#liTabConfiguracaoNFSe").hide();
                $("#liTabTransportadorFiliais").show();
                $("#liTabIntegracaoIntelipost").hide();
                $("#liTabOperadores").hide();
                alterarEstadoCadastroCertificado();
                $("#knockoutCadastroConfiguracaoTipoOperacao").addClass('d-none');
                _transportador.EmpresaPropria.visible(false);
                _transportador.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(false);
            }
            else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
                $("#liTabConfiguracoes").hide();
                $("#liTabDadosBancarios").show();
                $("#liTabCertificado").show();
                $("#liTabPermissoes").show();
                $("#liTabPermissoesTransportador").show();
                $("#liTabUsuarios").hide();
                $("#liTabSeries").show();
                $("#liTabLayoutsEDI").hide();
                $("#liTabFiliais").hide();
                $("#liTabFiliaisEmbarcador").hide();
                $("#liTabConfiguracaoNFSe").hide();
                $("#liTabTransportadorFiliais").show();
                $("#liTabIntegracaoIntelipost").hide();
                alterarEstadoCadastroCertificado();
                $("#knockoutCadastroConfiguracaoTipoOperacao").addClass('d-none');
                _transportador.EmpresaPropria.visible(false);
                _transportador.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(false);
            }
            else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                $("#liTabFiliaisEmbarcador").show();
                $("#liTabConfiguracaoCIOT").show();
                $("#liTabLeituraFTP").show();
                _pesquisaTransportador.Descricao.cssClass("col col-4");
                _pesquisaTransportador.SistemaEmissor.visible(true);
                _transportador.EmissaoDocumentosForaDoSistema.visible(true);
                _transportador.EmissaoMDFeForaDoSistema.visible(true);
                _transportador.EmissaoCRTForaDoSistema.visible(true);
                _transportador.PermiteEmitirSubcontratacao.visible(true);
                _transportador.EmpresaMobile.visible(true);
                _transportador.ExigeEtiquetagem.visible(true);
            }
            else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                $("#liTabDadosBancarios").show();
                //$("#knockoutCadastroConfiguracaoTipoOperacao").addClass('hide');
                _transportador.EmpresaPropria.visible(false);
                $("#liTabLeituraFTP").show();
            }

        });

        if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
            definirCamposMinimosObrigatorios();
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function OptanteSimplesNacionalChange() {
    if (_transportador.OptanteSimplesNacional.val()) {
        _transportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(true);
        _configuracao.ObservacaoSimplesNacional.visible(true);
        _transportador.RegimenTributario.visible(true);
        _transportador.RegimeEspecial.visible(false);
    } else {
        _transportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(false);
        _configuracao.ObservacaoSimplesNacional.visible(false);
        _transportador.RegimenTributario.visible(false);
        _transportador.RegimeEspecial.visible(true);
    }
}

function tipoEmpresaChange(e, sender) {
    if (_transportador.TipoEmpresa.val() == "F") {
        _transportador.RazaoSocial.text(Localization.Resources.Gerais.Geral.Nome.getRequiredFieldDescription());
        _transportador.InscricaoEstadual.text((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription()));
        _transportador.InscricaoEstadual.required(!_CONFIGURACAO_TMS.SistemaEstrangeiro);
        _transportador.CodigoIntegracao.text(_CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador ? Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getRequiredFieldDescription() : Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getFieldDescription());
        _transportador.CNPJ.enable(false);
        _transportador.CNPJ.visible(false);
        _transportador.CNPJ.required(false);
        _transportador.CPF.visible(true);
        _transportador.CPF.required = true;
        _transportador.CodigoIntegracao.required(_CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador);
    } else if (_transportador.TipoEmpresa.val() == "E") {
        _transportador.CNPJ.text(_CONFIGURACAO_TMS.SistemaEstrangeiro ? Localization.Resources.Transportadores.Transportador.Identificacao.getFieldDescription() : Localization.Resources.Transportadores.Transportador.CNPJ.getFieldDescription());
        _transportador.InscricaoEstadual.text(Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription());
        _transportador.InscricaoEstadual.required(false);
        _transportador.RazaoSocial.text(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription());
        _transportador.CodigoIntegracao.text(Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getRequiredFieldDescription());
        _transportador.CNPJ.enable(false);
        _transportador.CNPJ.visible(true);
        _transportador.CNPJ.required(false);
        _transportador.ValidarCNPJ.enable(false);
        _transportador.CPF.visible(false);
        _transportador.CPF.required = false;
        _transportador.CodigoIntegracao.required(true);
    } else {
        _transportador.RazaoSocial.text(Localization.Resources.Transportadores.Transportador.RazaoSocial.getRequiredFieldDescription());
        _transportador.CNPJ.text("*" + (_CONFIGURACAO_TMS.SistemaEstrangeiro ? Localization.Resources.Transportadores.Transportador.Identificacao.getFieldDescription() : Localization.Resources.Transportadores.Transportador.CNPJ.getFieldDescription()));
        _transportador.InscricaoEstadual.text((_CONFIGURACAO_TMS.SistemaEstrangeiro ? "" : "*") + Localization.Resources.Transportadores.Transportador.InscricaoEstadual.getFieldDescription());
        _transportador.InscricaoEstadual.required(!_CONFIGURACAO_TMS.SistemaEstrangeiro);
        _transportador.CodigoIntegracao.text(_CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador ? Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getRequiredFieldDescription() : Localization.Resources.Transportadores.Transportador.CodigoIntegracao.getFieldDescription());
        _transportador.CNPJ.enable(true);
        _transportador.CNPJ.visible(true);
        _transportador.CNPJ.required(!_CONFIGURACAO_TMS.SistemaEstrangeiro);
        _transportador.ValidarCNPJ.enable(true);
        _transportador.CPF.visible(false);
        _transportador.CPF.required = false;
        _transportador.CodigoIntegracao.required(_CONFIGURACAO_TMS.ExigirCodigoIntegracaoTransportador);
    }
}

function ContribuinteChange() {
    if (_transportador.Contribuinte.val()) {
        _transportador.DataValidadeContribuinte.visible(true);
    } else {
        _transportador.DataValidadeContribuinte.visible(false);
    }
}

function importarXMLClick(e, sender) {

    var file = document.getElementById(_transportador.Arquivo.id);
    var documentos = new Array();
    var fileCount = file.files.length;

    for (var i = 0; i < fileCount; i++) {
        var formData = new FormData();
        formData.append("upload", file.files[i]);

        _transportador.Arquivo.requiredClass("form-control");

        if (_transportador.Arquivo.val() != "") {
            enviarArquivo("Transportador/ImportarNFe?callback=?", { Codigo: _transportador.Codigo.val() }, formData, function (arg) {
                if (arg.Success) {
                    documentos.push({ Count: i });
                    if (documentos.length == fileCount) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);

                        _transportador.Arquivo.requiredClass("form-control");
                        _transportador.Arquivo.val("");
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    return;
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
            if (_transportador.Arquivo.val() == "")
                _transportador.Arquivo.requiredClass("form-control is-invalid");
        }
    }
}

function adicionarClick() {
    if (!validarCamposCadastroTransportador())
        return;
    preencherListasOperadores();
    executarReST("Transportador/Adicionar", obterTransportadorSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
                _gridTransportador.CarregarGrid();
                limparCamposTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function reenviarDadosAcessoaClienteClick(e, sender) {
    var data = { Codigo: _transportador.Codigo.val() };
    executarReST("Transportador/ReenviarDadosAcessoaCliente", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Transportador.EnviadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function atualizarClick() {
    if (!validarCamposCadastroTransportador())
        return;
    preencherListasOperadores();
    executarReST("Transportador/Atualizar", obterTransportadorSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridTransportador.CarregarGrid();
                limparCamposTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Transportador.RealmenteDesejaExcluirTransportador.format(_transportador.RazaoSocial.val()), function () {
        ExcluirPorCodigo(_transportador, "Transportador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridTransportador.CarregarGrid();
                    limparCamposTransportador();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }

            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTransportador();
}

function enviarCNPJClick(e) {
    if ($("#" + _transportador.CNPJ.id).val().match(/\d/g) != null && $("#" + _transportador.CNPJ.id).val().match(/\d/g).join("").length == 14) {
        if (_pesquisaDadosReceita != null && _pesquisaDadosReceita.Captcha.val() != null && _pesquisaDadosReceita.Captcha.val() != "") {
            var data = { CNPJ: _transportador.CNPJ.val(), Captcha: _pesquisaDadosReceita.Captcha.val(), Cookies: JSON.stringify(_cookies) };
            iniciarRequisicao();
            executarReST("Transportador/InformarCaptchaConsultaCNPJ", data, function (arg) {
                if (arg.Success) {
                    var argDados = { Data: arg.Data };
                    if (argDados.Data != null) {
                        PreencherObjetoKnout(_transportador, argDados);
                    }
                    Global.fecharModal('divModalConsultaPessoa');
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                    validarCNPJClick();
                }
            });
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.PorFavorDigiteCaptchaAntesConsultarDadosReceita);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.PorFavorDigiteCNPJValidoAntesConsultarDadosReceita);
        Global.fecharModal('divModalConsultaPessoa');
    }
}

function validarCNPJClick(e) {
    //if ($("#" + _transportador.CNPJ.id).val().match(/\d/g) != null && $("#" + _transportador.CNPJ.id).val().match(/\d/g).join("").length == 14) {
    //    if (ValidarCNPJ(_transportador.CNPJ.val(), _transportador.CNPJ.required)) {
    //        _transportador.CNPJ.requiredClass("form-control");
    //        _pesquisaDadosReceita.ImagemCaptcha.val("");
    //        _pesquisaDadosReceita.Captcha.val("");
    //        var data = { CNPJ: _transportador.CNPJ.val() };
    //        executarReST("Transportador/RequisicaoConsultaCNPJ", data, function (arg) {
    //            if (arg.Success) {
    //                if (arg.Data.Empresa != null) {
    //                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Transportador já cadastrado.")
    //                    var e = { Data: arg.Data.Empresa };
    //                    PreencherObjetoKnout(_transportador, e);
    //                    preecherRetornoEditarTransportador();
    //                }

    //                if (arg.Data.chaptcha !== false) {
    //                    _chaptcha = arg.Data.chaptcha;
    //                    _cookies = arg.Data.Cookies;
    //                    $('#divModalConsultaPessoa').modal({ keyboard: true, backdrop: 'static' });

    //                    _pesquisaDadosReceita.ImagemCaptcha.src(_chaptcha);
    //                    $("#" + _pesquisaDadosReceita.Captcha.id).focus();
    //                } else {
    //                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
    //                }
    //            } else {
    //                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //            }
    //        });
    //    } else {
    //        _transportador.CNPJ.requiredClass("form-control is-invalid");
    //        exibirMensagem(tipoMensagem.atencao, "Atenção", "O número do CNPJ não é válido. Verifique se o mesmo foi digitado corretamente!");
    //    }
    //} else {
    //    exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor digite um CNPJ válido antes de consultar os dados na receita!");
    //}

    //if ($("#" + _transportador.CNPJ.id).val().match(/\d/g) != null && $("#" + _transportador.CNPJ.id).val().match(/\d/g).join("").length == 14) {
    //    if (ValidarCNPJ(_transportador.CNPJ.val(), _transportador.CNPJ.required)) {
    //        _transportador.CNPJ.requiredClass("form-control");
    //        var data = { CNPJ: _transportador.CNPJ.val() };
    //        iniciarControleManualRequisicao();
    //        executarReST("Transportador/VerificarCNPJCadastrado", data, function (argTransp) {
    //            if (argTransp.Success) {
    //                if (argTransp.Data != null) {
    //                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Transportador já cadastrado.")
    //                    var e = { Data: argTransp.Data };
    //                    PreencherObjetoKnout(_transportador, e);
    //                    preecherRetornoEditarTransportador();
    //                }
    //                executarReST("Transportador/ConsultaCNPJCentralizada", data, function (arg) {
    //                    if (arg.Success) {
    //                        var argDados = { Data: arg.Data };
    //                        if (argDados.Data != null) {
    //                            PreencherObjetoKnout(_transportador, argDados);
    //                        }
    //                    } else {
    //                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    //                    }
    //                    finalizarControleManualRequisicao();
    //                });
    //            } else {
    //                exibirMensagem(tipoMensagem.falha, "Falha", argTransp.Msg);
    //                finalizarControleManualRequisicao();
    //            }
    //        });
    //    } else {
    //        _transportador.CNPJ.requiredClass("form-control is-invalid");
    //        exibirMensagem(tipoMensagem.atencao, "Atenção", "O número do CNPJ não é válido. Verifique se o mesmo foi digitado corretamente!");
    //    }
    //} else {
    //    exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor digite um CNPJ válido antes de consultar os dados na receita!");
    //}

    if ($("#" + _transportador.CNPJ.id).val().match(/\d/g) != null && $("#" + _transportador.CNPJ.id).val().match(/\d/g).join("").length == 14) {
        if (ValidarCNPJ(_transportador.CNPJ.val(), _transportador.CNPJ.required)) {
            _transportador.CNPJ.requiredClass("form-control");
            var data = { CNPJ: _transportador.CNPJ.val() };
            iniciarControleManualRequisicao();
            executarReST("Transportador/VerificarCNPJCadastrado", data, function (argTransp) {
                if (argTransp.Success) {
                    if (argTransp.Data != null) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Transportadores.Transportador.TransportadorCadastrado)
                        var e = { Data: argTransp.Data };
                        PreencherObjetoKnout(_transportador, e);
                        preecherRetornoEditarTransportador();
                    }
                    executarReST("Transportador/ConsultaCNPJReceitaWS", data, function (arg) {
                        if (arg.Success) {
                            var argDados = { Data: arg.Data };
                            if (argDados.Data != null) {
                                PreencherObjetoKnout(_transportador, argDados);
                            }
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                        }
                        finalizarControleManualRequisicao();
                    });
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, argTransp.Msg);
                    finalizarControleManualRequisicao();
                }
            });
        } else {
            _transportador.CNPJ.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.NumeroCNPJValidoVerifiqueMesmoDigitadoCorretamente);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.PorFavorDigiteCNPJValidoAntesConsultarDadosReceita);
    }
}

/*
 * Declaração das Funções
 */

function buscarTransportadors() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarTransportador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTransportador = new GridView(_pesquisaTransportador.Pesquisar.idGrid, "Transportador/Pesquisa", _pesquisaTransportador, menuOpcoes, null);
    _gridTransportador.CarregarGrid();
}

function editarTransportador(transportadorGrid) {
    _transportador.Codigo.val(transportadorGrid.Codigo);
    _EditouTransportador = true;
    BuscarPorCodigo(_transportador, "Transportador/BuscarPorCodigo", function (arg) {
        preecherRetornoEditarTransportador(arg);
        preencherCondicaoPagamento(arg.Data.CondicaoPagamento);
        preencherIntegracaoKrona(arg.Data.IntegracaoKrona);
        preencherLogoEmpresa(arg.Data.LogoEmpresa);
        preencherGridComponentesCTeImportados(arg.Data.ComponentesCTesImportados);
        preencherGridTransportadorCodigosIntegracao(arg.Data.CodigosIntegracao);
        preencherRepom(arg.Data.Repom);
        preencherMigrate(arg.Data.Migrate);
        preencherElectrolux(arg.Data.Electrolux);
        recarregarGridOperadores();

        if (_transportador.OptanteSimplesNacional.val())
            _transportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(true);
        else
            _transportador.OptanteSimplesNacionalComExcessoReceitaBruta.visible(false);

        _configuracaoLayoutEDI.SetarValores(arg.Data.ConfiguracaoLayoutEDI);

        _anexo.Anexos.val(arg.Data.Anexos);
    }, null);
}

function exibirMensagemCamposObrigatorio() {
    resetarTabs();
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function preecherRetornoEditarTransportador() {
    var configuracao = { Data: _transportador.Configuracao.val() };
    var dadoBancario = { Data: _transportador.DadoBancario.val() };
    var automacaoEmissaoNFSManual = { Data: _transportador.AutomacaoEmissaoNFSManual.val() };
    var leituraFTP = { Data: _transportador.LeituraFTP.val() };
    var termoQuitacao = { Data: _transportador.TermoQuitacao.val() };
    var repom = { Data: _transportador.Repom.val() };
    var electrolux = { Data: _transportador.Electrolux.val() };
    var migrate = { Data: _transportador.Migrate.val() };

    PreencherObjetoKnout(_dadoBancario, dadoBancario);
    PreencherObjetoKnout(_configuracao, configuracao);
    PreencherObjetoKnout(_automacaoNFSManual, automacaoEmissaoNFSManual);
    PreencherObjetoKnout(_transportadorNFSe, { Data: _transportador.TransportadorNFSe.val() });
    PreencherObjetoKnout(_LeituraFTP, leituraFTP);
    PreencherObjetoKnout(_termoQuitacao, termoQuitacao);
    PreencherObjetoKnout(_repom, repom);
    PreencherObjetoKnout(_electrolux, electrolux);
    PreencherObjetoKnout(_migrate, migrate);

    _configuracao.Empresa.val(_transportador.Codigo.val());
    _configuracao.Empresa.codEntity(_transportador.Codigo.val());

    _configuracao.TipoIntegracao.val(configuracao.TipoIntegracao)

    _configuracaoTipoOperacao.Empresa.val(_transportador.Codigo.val());
    _configuracaoTipoOperacao.Empresa.codEntity(_transportador.Codigo.val());

    _pesquisaTransportador.ExibirFiltros.visibleFade(false);

    _crudTransportador.Atualizar.visible(true);
    _crudTransportador.Cancelar.visible(true);
    _crudTransportador.Excluir.visible(true);
    _crudTransportador.Adicionar.visible(false);

    _transportador.TipoEmpresa.enable(false);
    _transportador.CNPJ.enable(false);
    _transportador.ValidarCNPJ.enable(false);
    _transportador.CPF.enable(false);
    tipoEmpresaChange();

    resetarTabs();

    limparCamposConfiguracaoTipoOperacao();
    recarregarGridCodigoComercialDistribuidorTransportador();
    recarregarGridSerie();
    recarregarGridConfiguracaoTipoOperacao();
    recarregarGridEstadoFeeder();
    recarregarGridFilial();
    recarregarGridFilialEmbarcador();
    recarregarPermissoesTransportador();
    recarregarGridTransportadorFilial();
    RecarregarGridImpostoRendaCIOT();
    RecarregarGridImpostoINSSCIOT();
    recarregarGridIntelipostDadosIntegracao();
    recarregarGridIntelipostTipoOcorrencia();
    recarregarGridOperadores();
    recarregarGridRotaFreteValePedagio();
    recarregarGridInscricaoST();
    configurarPeriodicidade();

    alterarEstadoCadastroCertificado();
    alterarEstadoCadastroConfiguracaoNFSe();
    alterarEstadoCadastroUsuario();
    alterarEstadoCadastroPallets();

    _permissaoTransportador.TransportadorAdministrador.val(_transportador.TransportadorAdministrador.val());
    _permissaoTransportador.HabilitaSincronismoDocumentosDestinados.val(_transportador.HabilitaSincronismoDocumentosDestinados.val());
    _permissaoTransportador.PerfilAcessoTransportador.val(_transportador.PerfilAcessoTransportador.val());
    _permissaoTransportador.PerfilAcessoTransportador.codEntity(_transportador.PerfilAcessoTransportador.codEntity());

    _seguro.LiberarEmissaoSemAverbacao.val(_transportador.LiberarEmissaoSemAverbacao.val());

    SetarPermissoesModulosFormularios();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _crudTransportador.ReenviarDadosAcessoaCliente.visible(true);

        $("#liTabSeguros").show();
        _seguro.Transportador.val(_transportador.Codigo.val());
        _seguro.UsarTipoOperacaoApolice.val(_transportador.UsarTipoOperacaoApolice.val());
        _gridSeguro.CarregarGrid();
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        $("#liTabSeguros").show();
        _seguro.Transportador.val(_transportador.Codigo.val());
        _seguro.UsarTipoOperacaoApolice.val(_transportador.UsarTipoOperacaoApolice.val());
        _gridSeguro.CarregarGrid();
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFeAdmin)
        _crudTransportador.ReenviarDadosAcessoaCliente.visible(true);

    $("#liTabConfiguracoes").show();
    $("#liTabConfiguracaoLogo").show();

    if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
        definirCamposMinimosObrigatorios();
}

function limparCamposTransportador() {
    _crudTransportador.ReenviarDadosAcessoaCliente.visible(false);
    _crudTransportador.Atualizar.visible(false);
    _crudTransportador.Cancelar.visible(false);
    _crudTransportador.Excluir.visible(false);
    _crudTransportador.Adicionar.visible(true);

    LimparCampos(_transportador);
    tipoEmpresaChange();
    _transportador.TipoEmpresa.enable(true);
    _transportador.CNPJ.enable(true);
    _transportador.ValidarCNPJ.enable(true);
    _transportador.CPF.enable(true);
    _transportador.Arquivo.val("");

    limparCamposSerie();
    limparCamposConfiguracaoTipoOperacao();
    limparCamposFilial();
    limparCamposFilialEmbarcador();
    limparPermissoes();
    limparPermissoesTransportador();
    limparCamposConfiguracoes();
    limparCamposCodigoComercialDistribuidorTransportador();
    limparCamposConfiguracoesCIOT();
    limparCamposTransportadorFilial();
    limparCamposCondicaoPagamento();
    limparCamposIntegracaoKrona();
    limparCamposTransportadorNFSe();
    limparCamposDadoBancario();
    limparCamposConfiguracoesMultimodal();
    limparConfiguracaoLogo();
    limparCamposRotaFreteValePedagio();
    limparCamposInscricaoST();
    limparCamposTransportadorComponenteCTeImportado();
    limparCamposTransportadorCodigosIntegracao();
    limparCamposOperadores();
    limparCamposLeituraFTP();
    limparCamposTermoQuitacao();
    LimparRepom();
    LimparElectrolux();
    LimparMigrate();

    recarregarGridCodigoComercialDistribuidorTransportador();
    recarregarGridSerie();
    recarregarGridConfiguracaoTipoOperacao();
    recarregarGridEstadoFeeder();
    recarregarGridFilial();
    recarregarGridFilialEmbarcador();
    recarregarGridTransportadorFilial();
    recarregarGridIntelipostDadosIntegracao();
    recarregarGridIntelipostTipoOcorrencia();
    recarregarGridOperadores();

    _configuracaoLayoutEDI.Limpar();

    alterarEstadoCadastroUsuario();
    alterarEstadoCadastroCertificado();
    alterarEstadoCadastroConfiguracaoNFSe();
    alterarEstadoCadastroPallets();
    LimparSeguroTransportador();

    if (_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas)
        definirCamposMinimosObrigatorios();

    resetarTabs();
}

function resetarTabs() {
    $("#tabTransportador a:first").tab("show");
}

function carregarConteudosHTML(callback) {
    $.get("Content/Static/Pessoa/ConsultaPessoa.html?dyn=" + guid(), function (data) {
        $("#ConsultaDadosReceita").html(data);
        _pesquisaDadosReceita = new PesquisaDadosReceita();
        KoBindings(_pesquisaDadosReceita, "knoutConsultaPessoa");
    });
}

function obterTransportadorSalvar() {
    var permissaoTransportador = RetornarObjetoPesquisa(_permissaoTransportador);

    BuscarPermissoesFormularios();

    _transportador.PerfilAcessoTransportador.val(_permissaoTransportador.PerfilAcessoTransportador.val());
    _transportador.PerfilAcessoTransportador.codEntity(_permissaoTransportador.PerfilAcessoTransportador.codEntity());
    _transportador.TransportadorAdministrador.val(permissaoTransportador.TransportadorAdministrador);
    _transportador.HabilitaSincronismoDocumentosDestinados.val(permissaoTransportador.HabilitaSincronismoDocumentosDestinados);
    _transportador.LiberarEmissaoSemAverbacao.val(_seguro.LiberarEmissaoSemAverbacao.val());
    _transportador.Configuracao.val(JSON.stringify(RetornarObjetoPesquisa(_configuracao)));
    _transportador.DadoBancario.val(JSON.stringify(RetornarObjetoPesquisa(_dadoBancario)));
    _transportador.ComponentesCTesImportados.val(JSON.stringify(_componenteCTeImportado.ComponentesCTesImportados.val()));
    _transportador.TransportadorNFSe.val(JSON.stringify(RetornarObjetoPesquisa(_transportadorNFSe)));
    _transportador.AutomacaoEmissaoNFSManual.val(JSON.stringify(RetornarObjetoPesquisa(_automacaoNFSManual)));
    _transportador.CodigosIntegracao.val(JSON.stringify(_gridTransportadorCodigosIntegracao.BuscarRegistros()));
    _transportador.LeituraFTP.val(JSON.stringify(RetornarObjetoPesquisa(_LeituraFTP)));
    _transportador.TermoQuitacao.val(JSON.stringify(RetornarObjetoPesquisa(_termoQuitacao)));
    _transportador.Repom.val(JSON.stringify(RetornarObjetoPesquisa(_repom)));
    _transportador.Electrolux.val(JSON.stringify(RetornarObjetoPesquisa(_electrolux)));
    _transportador.Migrate.val(JSON.stringify(RetornarObjetoPesquisa(_migrate)));

    var transportador = RetornarObjetoPesquisa(_transportador);

    preencherCondicaoPagamentoSalvar(transportador);
    preencherIntegracaoKronaSalvar(transportador);

    return transportador;
}

function configurarIntegracoesDisponiveis() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (r) {
        if (r.Success && r.Data) {
            if (r.Data.TiposExistentes != null && r.Data.TiposExistentes.length > 0) {

                if (r.Data.TiposExistentes.some(function (o) {
                    return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.Target || o == EnumTipoIntegracao.Repom || o == EnumTipoIntegracao.PagBem || o == EnumTipoIntegracao.DBTrans ||
                        o == EnumTipoIntegracao.Pamcard || o == EnumTipoIntegracao.QualP || o == EnumTipoIntegracao.EFrete || o == EnumTipoIntegracao.Extratta || o == EnumTipoIntegracao.DigitalCom || o == EnumTipoIntegracao.RepomFrete ||
                        o == EnumTipoIntegracao.NDDCargo;
                }))
                    _transportador.CompraValePedagio.visible(true);

                if (_CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.SemParar || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Target || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Repom
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.PagBem || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.DBTrans || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Pamcard
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.QualP || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.EFrete || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.Extratta
                    || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.DigitalCom || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.RepomFrete || _CONFIGURACAO_TMS.TipoIntegracaoValePedagio == EnumTipoIntegracao.NDDCargo) {
                    _transportador.CompraValePedagio.val(true);
                    _transportador.TipoIntegracaoValePedagio.val([_CONFIGURACAO_TMS.TipoIntegracaoValePedagio]);
                    $("#" + _transportador.TipoIntegracaoValePedagio.id).trigger("change");
                }

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Telerisco; }))
                    _transportador.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(true);
                else
                    _transportador.ValidarMotoristaTeleriscoAoConfirmarTransportador.visible(false);

                if (r.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Correios; }))
                    _transportador.IntegrarCorreios.visible(true);
                else
                    _transportador.IntegrarCorreios.visible(false);
            }
        }
    });
}

function alterouCompraValePedagio() {

    var habilitar = _transportador.CompraValePedagio.val();

    _transportador.TipoIntegracaoValePedagio.visible(habilitar);

    if (!habilitar)
        LimparCampo(_transportador.TipoIntegracaoValePedagio);
}

function validarCamposCadastroTransportador() {
    if (!ValidarCamposObrigatorios(_transportador)) {
        exibirMensagemCamposObrigatorio();
        return false;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        var permiteBloquearTransportador = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Transportador_PermiteBloquearTransportador, _PermissoesPersonalizadas);
        if (permiteBloquearTransportador && _transportador.BloquearTransportador.val() && string.IsNullOrWhiteSpace(_transportador.MotivoBloqueio.val())) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.NecessarioInformarMotivoBloqueio)
            _transportador.MotivoBloqueio.requiredClass("form-control is-invalid");
            return false;
        }
    }

    if (!isPossuiAnexo() && _CONFIGURACAO_TMS.ExigirAnexosNoCadastroDoTransportador && _transportador.Status.val() === "A" && !_CONFIGURACAO_TMS.PermitirCadastrarTransportadorInformacoesMinimas) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Transportador.PorFavorInformeMaisAnexosTransportador);
        return false;
    }

    return true;
}

function definirCamposMinimosObrigatorios() {
    _transportador.NomeFantasia.required = false;
    _transportador.InscricaoEstadual.required(false);
    _transportador.Endereco.required = false;
    _transportador.Numero.required = false;
    _transportador.Bairro.required = false;
    _transportador.Telefone.required = false;
    _transportador.RegistroANTT.required = false;
    _transportador.RegistroANTT.text(Localization.Resources.Transportadores.Transportador.RNTRC.getFieldDescription());

    _transportador.Email.required = true;
}

function preencherListasOperadores() {
    _transportador.Operadores.val(JSON.stringify(_gridOperadores.BuscarRegistros()));
}

function verificarSeExisteEscalationList() {
    executarReST("Transportador/VerificarSeExisteEscalationList", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.EscalationList) {
                $("#liTabOperadores").show()
            } else {
                $("#liTabOperadores").hide()
            }
        }
    });
}