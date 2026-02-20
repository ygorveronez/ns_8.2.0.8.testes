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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/PedidoTipoPagamento.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumOrgaoEmissorRG.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFinanceira.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="../../Enumeradores/EnumEstadoCivil.js" />
/// <reference path="../../Enumeradores/EnumSexo.js" />
/// <reference path="../../Enumeradores/EnumTipoClienteIntegracaoLBC.js" />
/// <reference path="Pessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pessoaAdicional;

//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)
//                                                        DEIXAR OS CAMPOS VISÍVEIS APENAS NO SISTEMA QUE SOLICITA (TMS, EMBARCADOR)

var PessoaAdicional = function () {
    this.NomeSocio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NomeSocio.getFieldDescription(), maxlength: 100 });
    this.CPFSocio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CPFSocio.getFieldDescription(), getType: typesKnockout.cpf });
    this.Profissao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Profissao.getFieldDescription(), maxlength: 200 });
    this.TituloEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TituloEleitoral.getFieldDescription(), maxlength: 100 });
    this.ZonaEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ZonaEleitoral.getFieldDescription(), maxlength: 100 });
    this.SecaoEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SecaoEleitoral.getFieldDescription(), maxlength: 100 });
    this.NumeroCEI = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NumeroCeiCadastroEspecificoDoInss.getFieldDescription(), maxlength: 100 });
    this.ObservacaoInterna = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ObservacaoInterna.getFieldDescription(), maxlength: 5000 });

    this.CodigoCompanhia = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoDaCompanhia.getFieldDescription()), required: false, maxlength: 50, visible: ko.observable(true), enable: ko.observable(true) });
    this.ContaFornecedorEBS = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.ContaContabil.getFieldDescription()), required: false, maxlength: 50, visible: ko.observable(true), enable: ko.observable(true) });
    this.CodigoDocumento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoDocumentacao.getFieldDescription(), required: ko.observable(false), maxlength: 50, visible: ko.observable(false), enable: ko.observable(true) });

    this.OrgaoEmissaoRG = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.OrgaoEmissorRG.getFieldDescription(), val: ko.observable(""), options: EnumOrgaoEmissorRG.obterOpcoes(), def: "" });

    this.LocalidadeNascimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.LocalidadeNascimento.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });

    this.DataNascimento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date, text: Localization.Resources.Pessoas.Pessoa.DataNascimento.getFieldDescription(), required: ko.observable(false) });
    this.PISPASEP = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Pessoas.Pessoa.PisPasep.getFieldDescription(), required: ko.observable(false) });
    this.SituacaoFinanceira = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SituacaoFinanceira.getFieldDescription(), val: ko.observable(EnumSituacaoFinanceira.Liberada), options: EnumSituacaoFinanceira.obterOpcoes(), def: EnumSituacaoFinanceira.Liberada, enable: ko.observable(true), visible: ko.observable(false) });

    this.GerarPedidoColeta = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.GerarPedidoDeColeta, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.GerarPedidoBloqueado = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.GerarPedidoBloqueado, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.InstituicaoGovernamental = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.InstituicaoGovernamental, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ExigirNumeroControleCliente = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExigirNumeroDeControleDoCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ReplicarNumeroControleCliente = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ReplicarNumeroDeControleDoClienteParaTodasAsNotasDaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExigirNumeroNumeroReferenciaCliente = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExigirNumeroDeReferenciaDoCliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.EnviarAutomaticamenteDocumentacaoCarga = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.EnviarAutomaticamenteDocumentacaoDeCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.ReplicarNumeroReferenciaTodasNotasCarga = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ReplicarNumeroDeReferenciaDoClienteParaTodasAsNotasDaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DigitalizacaoCanhotoInteiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DigitalizacaoDoCanhotoSeraUmaImagemDaNotaInteira, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EnviarDocumentacaoCTeAverbacaoSegundaInstancia = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AtivarEnvioDaDocumentacaoDeCTeAverbacaoParaASegundaInstancia, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.NaoAplicarChecklistMultiMobile = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoAplicarChecklistMultiMobile, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.ClientePai = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.EmpresaPai.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.RecebedorColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Recebedor.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.Pessoa.Transportador.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PedidoTipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.CondicaoDoPedido.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoOperacaoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.Pessoa.TipoDeOperacaoPadrao.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoEmissaoCTeDocumentosExclusivo = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoes(), text: Localization.Resources.Pessoas.Pessoa.RateioDosDocumentosExclusivo.getFieldDescription(), def: EnumTipoEmissaoCTeDocumentos.NaoInformado, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(false), issue: 400 });
    this.RateioFormulaExclusivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.Pessoa.FormulaDoRateioDoFreteExclusivo.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.CodigoPortuario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoPortuario.getFieldDescription(), maxlength: 50, visible: ko.observable(false) });
    this.Celular = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Celular.getFieldDescription(), issue: 27, getType: typesKnockout.phone });
    this.SenhaLiberacaoMobile = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SenhaLiberacaoMobile.getFieldDescription(), maxlength: 100, visible: ko.observable(_CONFIGURACAO_TMS.ExibirSenhaCadastroPessoa) });
    this.SenhaConfirmacaoColetaEntrega = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SenhaParaConfirmacaoDeEntrega.getFieldDescription(), maxlength: 50, getType: typesKnockout.int, def: "", configInt: { precision: 0, allowZero: false }, visible: ko.observable(false) });
    this.CodigoSap = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoSAP.getFieldDescription(), maxlength: 100, visible: ko.observable(false) });
    this.Referencia = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Referencia.getFieldDescription(), maxlength: 100, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFluxoPedidoEcommerce) });
    this.ValorMinimoCarga = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pessoas.Pessoa.ValorMinimoCarga.getFieldDescription(), required: false, visible: ko.observable(false) });
    this.ValorMinimoEntrega = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pessoas.Pessoa.ValorMinimoMercadoriaEntrega.getFieldDescription(), required: ko.observable(false), visible: ko.observable(false) });

    this.FronteiraAlfandega = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FronteiraAlfandega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    //this.TempoMedioPermanenciaFronteira = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TempoMedioDePermanenciaNaFronteira.getFieldDescription(), getType: typesKnockout.time, visible: ko.observable(false) });
    this.TempoMedioPermanenciaFronteira = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TempoMedioDePermanenciaNaFronteira.getFieldDescription(), getType: typesKnockout.mask, mask: "000:00", visible: ko.observable(false), required: ko.observable(false) });
    this.CodigoAduaneiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoDaAduana.getFieldDescription(), getType: typesKnockout.string, string: ko.observable(false) });
    this.CodigoURFAduaneiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoUrfIntercionalizacao, getType: typesKnockout.string, visible: ko.observable(false) });
    this.CodigoRAAduaneiro = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoRaIntercionalizacao, getType: typesKnockout.string, visible: ko.observable(false) });
    this.CodigoAduanaDestino = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoDaAduanaDoDestino.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(false) });

    this.ExigeQueEntregasSejamAgendadas = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExigeQueSuasEntregasSejamAgendadas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.AreaRedex = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AreaDeRedex, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.Armador = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Armador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });
    this.PermiteAgendarComViagemIniciada = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PermiteAgendarComViagemIniciada, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.Sexo = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Sexo.getFieldDescription()), val: ko.observable(EnumSexo.NaoInformado), options: EnumSexo.obterOpcoes(), def: EnumSexo.NaoInformado, enable: ko.observable(true) });
    this.EstadoCivil = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.EstadoCivil.getFieldDescription()), val: ko.observable(EnumEstadoCivil.Outros), options: EnumEstadoCivil.obterOpcoes(), def: EnumEstadoCivil.Outros, enable: ko.observable(true) });
    this.CotacaoEspecial = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 13, text: Localization.Resources.Pessoas.Pessoa.CotacaoEspecial.getFieldDescription(), visible: ko.observable(true) });
    this.TipoFornecedor = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDoFornecedor.getFieldDescription(), maxlength: 100 });
    this.CodigoCategoriaTrabalhador = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CategoriaDoTrabalhador.getFieldDescription(), maxlength: 100 });
    this.Funcao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Funcao.getFieldDescription(), maxlength: 100 });
    this.PagamentoEmBanco = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.PagamentoEmBanco.getFieldDescription(), maxlength: 100 });
    this.FormaPagamentoeSocial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FormaPagamentoESocial.getFieldDescription(), maxlength: 100 });
    this.BancoDOC = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.BancoParaDOC.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.EstadoRG = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.EstadoRG.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.ArmazemResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.ArmazemResponsavel.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CondicaoPagamentoPadrao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.CondicaoPagamentoPadrao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoIntegracaoValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.TipoIntegracaoValePedagio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoClienteIntegracaoLBC = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoClienteIntegracao, val: ko.observable(EnumTipoClienteIntegracaoLBC.Nenhum), options: EnumTipoClienteIntegracaoLBC.obterOpcoes(), def: EnumTipoClienteIntegracaoLBC.Nenhum, enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoAutonomo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoAutonomo.getFieldDescription(), maxlength: 100 });
    this.CodigoReceita = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoReceita.getFieldDescription(), maxlength: 100 });
    this.TipoPagamentoBancario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoPagamentoBancario.getFieldDescription(), maxlength: 100 });
    this.NaoDescontaIRRF = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.NaoDescontaIRRF.getFieldDescription(), maxlength: 100 });
    this.CodigoAlternativo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CodigoAlternativo.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.ExigeEtiquetagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Pessoa.ExigeEtiquetagem, visible: ko.observable(false) });
    this.EhPontoDeApoio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Pessoa.PontoDeApoio, visible: ko.observable(false) });
    this.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Pessoa.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento, visible: ko.observable(true) });
    this.NaoComprarValePedagio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Pessoa.NaoComprarValePedagio, visible: ko.observable(true) });
    this.MesoRegiao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, idBtnSearch: guid() });
    this.Regiao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Regiao, idBtnSearch: guid() });
    this.FazParteGrupoEconomico = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.FazParteGrupoEconomico, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.RegraPallet = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RegraPallet.getFieldDescription()), val: ko.observable(EnumRegraPallet.Nenhuma), options: EnumRegraPallet.obterOpcoes(), def: EnumRegraPallet.Nenhuma, enable: ko.observable(true), visible: ko.observable(false) });
    this.CanalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.CanalEntrega.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.RKST = PropertyEntity({ text: "RKST: ", required: ko.observable(false), maxlength: 10 });
    this.MDGCode = PropertyEntity({ text: "MDG Code: ", required: ko.observable(false), maxlength: 11 });
    this.CMDID = PropertyEntity({ text: "CMD ID: ", required: ko.observable(false), maxlength: 10 });

    
    this.GerarPedidoColeta.val.subscribe(function (valor) {
        _pessoaAdicional.RecebedorColeta.visible(valor === true);
        _pessoaAdicional.RecebedorColeta.required(valor === true);
    });

    this.ExigirNumeroNumeroReferenciaCliente.val.subscribe(function (novoValor) {
        _pessoaAdicional.ReplicarNumeroReferenciaTodasNotasCarga.visible(novoValor === true);
        if (novoValor === false)
            _pessoaAdicional.ReplicarNumeroReferenciaTodasNotasCarga.val(false);
    });

    this.ExigirNumeroControleCliente.val.subscribe(function (novoValor) {
        _pessoaAdicional.ReplicarNumeroControleCliente.visible(novoValor === true);
        if (novoValor === false)
            _pessoaAdicional.ReplicarNumeroControleCliente.val(false);
    });

    this.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento.val.subscribe(function (valor) {
        _pessoaAdicional.ValorMinimoEntrega.visible(valor === true);
        _pessoaAdicional.ValorMinimoEntrega.required(valor === true);
        if (valor === false) {
            _pessoaAdicional.ValorMinimoEntrega.val(false);
            _pessoaAdicional.ValorMinimoEntrega.required(valor === true);
        }
    });

    this.CotacaoEspecial.val.subscribe(function (value) {
        const valorNumerico = parseFloat(value);
        if (!isNaN(valorNumerico) && valorNumerico > 100) {
            this.CotacaoEspecial.val(100);
            exibirMensagem(tipoMensagem.atencao, "Valor excedente", "O valor informado não pode ser superior a 100");
        }
    }.bind(this));
};

//*******EVENTOS*******

function loadPessoaAdicional() {
    _pessoaAdicional = new PessoaAdicional();
    KoBindings(_pessoaAdicional, "knockoutAdicionais");

    $("#" + _pessoaAdicional.PISPASEP.id).mask("000.00000.00-0", { selectOnFocus: true, clearIfNotMatch: true });

    $("#" + _pessoaAdicional.AreaRedex.id).click(desabilitarAreaRedex);
    $("#" + _pessoaAdicional.Armador.id).click(habilitarAbaDadosArmador);

    new BuscarClientes(_pessoaAdicional.ClientePai);
    new BuscarClientes(_pessoaAdicional.RecebedorColeta);
    new BuscarTransportadores(_pessoaAdicional.Transportador);
    new BuscarRateioFormulas(_pessoaAdicional.RateioFormulaExclusivo);
    new BuscarPedidoTipoPagamento(_pessoaAdicional.PedidoTipoPagamento);
    new BuscarTiposOperacao(_pessoaAdicional.TipoOperacaoPadrao);
    new BuscarBanco(_pessoaAdicional.BancoDOC);
    new BuscarEstados(_pessoaAdicional.EstadoRG);
    new BuscarClientes(_pessoaAdicional.ArmazemResponsavel);
    new BuscarCondicaoPagamento(_pessoaAdicional.CondicaoPagamentoPadrao);
    new BuscarMesoRegiao(_pessoaAdicional.MesoRegiao);
    new BuscarRegioes(_pessoaAdicional.Regiao);
    new BuscarTipoIntegracao(_pessoaAdicional.TipoIntegracaoValePedagio);
    new BuscarLocalidades(_pessoaAdicional.LocalidadeNascimento);
    new BuscarCanaisEntrega(_pessoaAdicional.CanalEntrega);

    if (_CONFIGURACAO_TMS.NaoPermitirLiberarSemValePedagio)
        _pessoaAdicional.TipoIntegracaoValePedagio.visible(_CONFIGURACAO_TMS.NaoPermitirLiberarSemValePedagio);


    configurarLayoutPorTipoSistema();
}

function configurarLayoutPorTipoSistema() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _pessoaAdicional.TipoEmissaoCTeDocumentosExclusivo.visible(true);
        _pessoaAdicional.RateioFormulaExclusivo.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pessoaAdicional.CodigoDocumento.visible(true);
        _pessoaAdicional.CodigoPortuario.visible(true);
        _pessoaAdicional.PedidoTipoPagamento.visible(true);
        _pessoaAdicional.SituacaoFinanceira.visible(true);
        _pessoaAdicional.EnviarAutomaticamenteDocumentacaoCarga.visible(true);
        _pessoaAdicional.EnviarDocumentacaoCTeAverbacaoSegundaInstancia.visible(true);
        _pessoaAdicional.EhPontoDeApoio.visible(true);

        _pessoaAdicional.SituacaoFinanceira.enable(true);
        if (!_CONFIGURACAO_TMS.UsuarioAdministrador && !VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pessoa_PermiteAlterarSituacaoFinanceira, _PermissoesPersonalizadas))
            _pessoaAdicional.SituacaoFinanceira.enable(false);

        _pessoaAdicional.FazParteGrupoEconomico.visible(true);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pessoaAdicional.CodigoSap.visible(true);
        _pessoaAdicional.ExigeQueEntregasSejamAgendadas.visible(true);
        _pessoaAdicional.DigitalizacaoCanhotoInteiro.visible(true);
        _pessoaAdicional.AreaRedex.visible(true);
        _pessoaAdicional.Armador.visible(true);
        _pessoaAdicional.SenhaConfirmacaoColetaEntrega.visible(true);
        _pessoaAdicional.ValorMinimoCarga.visible(true);
        _pessoaAdicional.PermiteAgendarComViagemIniciada.visible(true);
        _pessoaAdicional.ExigeEtiquetagem.visible(true);
        _pessoaAdicional.GerarPedidoBloqueado.visible(true);
        _pessoaAdicional.TipoClienteIntegracaoLBC.visible(true);
        _pessoaAdicional.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente.visible(true);
        _pessoaAdicional.RegraPallet.visible(true);
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pessoaAdicional.GerarPedidoColeta.visible(false);
        _pessoaAdicional.ExigirNumeroControleCliente.visible(false);
        _pessoaAdicional.ExigirNumeroNumeroReferenciaCliente.visible(false);
        _pessoaAdicional.FronteiraAlfandega.visible(false);
        _pessoaAdicional.TipoOperacaoPadrao.visible(false);
    }
}

function validarCamposPessoaAdicional() {
    var tudoCerto = ValidarCamposObrigatorios(_pessoaAdicional);
    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.InformeOsCamposObrigatoriosDaAbaAdicionais);
        $("#liTabAdicionais a").tab("show");
        return tudoCerto;
    }

    if (_pessoa.TipoTransportador.val() && _pessoa.TipoPessoa.val() === EnumTipoPessoa.Fisica) {

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
            _pessoaAdicional.PISPASEP.required(false);
        }

        tudoCerto = ValidarCamposObrigatorios(_pessoaAdicional);

        if (!tudoCerto) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Pessoas.Pessoa.InformeOsCamposObrigatoriosDaAbaAdicionaisParaTransportadorTerceiro);
            $("#liTabAdicionais a").tab("show");
            return tudoCerto;
        }
    }

    return tudoCerto;
}

function limparCamposPessoaAdicional() {
    LimparCampos(_pessoaAdicional);
    _pessoaAdicional.PISPASEP.required(false);
    _pessoaAdicional.DataNascimento.required(false);
}

function habilitarAbaDadosArmador() {
    if (_pessoaAdicional.Armador.val())
        $("#liTabDadosArmador").show();
    else
        $("#liTabDadosArmador").hide();
}
function limitarValor(_, event) {
    const valorString = event.target.value.replace(/\./g, '').replace(',', '.');

    const valor = parseFloat(valorString);

    if (!isNaN(valor) && valor > 100) {
        event.target.value = 100;
        exibirMensagem(tipoMensagem.atencao, "Valor excedente", "O valor informado não pode ser superior a 100");
    }
}
