/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/PerfilAcessoMobile.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Cargo.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
/// <reference path="../../Enumeradores/EnumControleAlertaForma.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../Enumeradores/EnumTipoParentesco.js" />
/// <reference path="../../Enumeradores/EnumTipoLogradouro.js" />
/// <reference path="../../Enumeradores/EnumTipoResidencia.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />
/// <reference path="../../Enumeradores/EnumCorRaca.js" />
/// <reference path="../../Enumeradores/EnumEscolaridade.js" />
/// <reference path="../../Enumeradores/EnumEstadoCivil.js" />
/// <reference path="../../Enumeradores/EnumAposentadoriaFuncionario.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumTipoEndereco.js" />
/// <reference path="../../Enumeradores/EnumTipoEmail.js" />
/// <reference path="Integracoes.js" />
/// <reference path="MotoristaLicencas.js" />
/// <reference path="MotoristaFoto.js" />
/// <reference path="MotoristaDadoBancario.js" />
/// <reference path="MotoristaContato.js" />
/// <reference path="PermissaoMotoristaMobile.js" />
/// <reference path="MotoristaSituacaoColaborador.js" />
/// <reference path="MotoristaTransportador.js" />
/// <reference path="CamposObrigatorios.js" />
/// <reference path="MotoristaEPIs.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _categoriaHabilitacao = [
    { text: "Nenhuma", value: "" },
    { text: "A", value: "A" },
    { text: "B", value: "B" },
    { text: "C", value: "C" },
    { text: "D", value: "D" },
    { text: "E", value: "E" },
    { text: "AB", value: "AB" },
    { text: "AC", value: "AC" },
    { text: "AD", value: "AD" },
    { text: "AE", value: "AE" }
];

var _gridMotorista;
var _motorista;
var _pesquisaMotorista;
var _pesquisaEndereco;
var _PermissoesPersonalizadas;
var _mensagemMotoristaVinculado;

var PesquisaEndereco = function () {
    this.Logradouro = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Endereco.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Bairro.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CEP.getFieldDescription()), required: false });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoIBGE = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CodigoIBGE.getFieldDescription()), required: false });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarEnderecosClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), visible: ko.observable(true) });

    this.Enderecos = PropertyEntity({ type: types.local, idGrid: guid() });

    this.CancelarEndereco = PropertyEntity({ eventClick: cancelarEnderecoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(true) });
};

var PesquisaMotorista = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Transportador.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Nome = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Nome.getFieldDescription()) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CPF.getFieldDescription()), maxlength: 14, getType: _CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro ? typesKnockout.string : typesKnockout.cpf });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getFieldDescription()) });
    this.CargoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Cargo.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroMatricula = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.NumeroMatricula.getFieldDescription() });
    this.TipoMotorista = PropertyEntity({ val: ko.observable(EnumTipoMotorista.Todos), options: EnumTipoMotorista.obterOpcoesPesquisa(), def: EnumTipoMotorista.Todos, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoMotorista.getRequiredFieldDescription()), issue: 640, required: true, visible: ko.observable(false) });
    this.TipoMotoristaAjudante = PropertyEntity({ val: ko.observable(EnumTipoMotoristaAjudante.Todos), options: EnumTipoMotoristaAjudante.obterOpcoesPesquisa(), def: EnumTipoMotoristaAjudante.Todos, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoMotoristaAjudante.getFieldDescription()), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({

        eventClick: function (e) {
            _gridMotorista.CarregarGrid();
        }, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Pesquisar), idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.FiltrosDePesquisa), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Motorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.MotoristaEstrangeiro = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Transportadores.Motorista.MotoristaEstrangeiro, visible: ko.observable(_CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro) });

    this.MotoristaEstrangeiro.val.subscribe(controlarCamposMotoristaEstrangeiro);

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.Transportador.getRequiredFieldDescription()), issue: 637, idBtnSearch: guid(), visible: ko.observable(false), OrdenarCargasMobileCrescente: false });
    this.Status = PropertyEntity({ val: ko.observable("A"), def: "A" });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: ko.observable(Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription()), issue: 557 });
    this.Bloqueado = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.Bloqueado.getRequiredFieldDescription()), visible: ko.observable(false) });

    this.Bloqueado.val.subscribe(function (valor) {
        _motorista.MotivoBloqueio.visible(valor == EnumSimNao.Sim && _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador && _motorista.Bloqueado.visible());
    });

    this.Apelido = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Apelido.getFieldDescription()), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4"), enable: ko.observable(true) });
    this.Nome = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Nome.getRequiredFieldDescription()), issue: 638, required: true, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.CodigoIntegracao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CodigoIntegracao.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 50 });
    this.CodigoIntegracaoContabilidade = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CodigoIntegracaoContabil.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 50 });
    this.CPF = PropertyEntity({ text: ko.observable(_CONFIGURACAO_TMS.Pais == EnumPaises.Brasil ? Localization.Resources.Transportadores.Motorista.CPF.getRequiredFieldDescription() : Localization.Resources.Transportadores.Motorista.CPF.getFieldDescription()), issue: 3, visible: ko.observable(true), required: _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil, getType: typesKnockout.cpf });
    this.CodigoMotoristaEstrangeiro = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Codigo.getFieldDescription()), required: false, getType: typesKnockout.string, maxlength: 20, visible: ko.observable(false) });
    this.RG = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.RG.getFieldDescription()), maxlength: 50 });
    this.PISPASEP = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.PISPASEP.getFieldDescription()), maxlength: 14, required: false });
    this.EmissorRG = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.EmissorRG.getFieldDescription()), val: ko.observable(EnumOrgaoEmissorRG.SSP), options: EnumOrgaoEmissorRG.obterOpcoes(), def: EnumOrgaoEmissorRG.SSP, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-2 col-lg-2") });
    this.EstadoRG = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.EstadoRG.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Telefone = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Telefone.getFieldDescription()), issue: 27, getType: typesKnockout.phone, required: false });
    this.Celular = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Celular.getFieldDescription()), issue: 27, getType: typesKnockout.phone, required: false });
    this.DataNascimento = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataNascimento.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.DataValidadeCNH = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataValidadeCNH.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.DataPrimeiraHabilitacao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataPrimeiraCNH.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.UFEmissaoCNH = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.UFEmissaoCNH.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.DataEmissaoCNH = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataEmissaoCNH.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.DataEmissaoRG = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataEmissaoRG.getFieldDescription()), issue: 2, getType: typesKnockout.date, required: false });
    this.DataAdmissao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataAdmissao.getFieldDescription()), issue: 2, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataDemissao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataDemissao.getFieldDescription()), issue: 2, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataFimPeriodoExperiencia = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataFimPeriodoExperiencia.getFieldDescription()), issue: 2, getType: typesKnockout.date, enable: ko.observable(true) });
    this.DataVencimentoMoop = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.VencimentoMoop.getFieldDescription()), issue: 2, getType: typesKnockout.date });
    this.NumeroCTPS = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroCTPS.getFieldDescription()), maxlength: 100 });
    this.SerieCTPS = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.SerieCTPS.getFieldDescription()), maxlength: 100 });
    this.ValidarCamposReferenteCIOT = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.ValidarCamposReferenteCIOT.val.subscribe(ValidarCamposReferenteCIOTChange);

    this.SaldoDiaria = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.SaldoDiarias.getFieldDescription()), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.SaldoAdiantamento = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.SaldoAdiantamentos.getFieldDescription()), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.DiasTrabalhado = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DiasTrabalhado.getFieldDescription()), getType: typesKnockout.int, required: false, enable: ko.observable(false), visible: ko.observable(true) });
    this.DiasFolgaRetirado = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DiasDeFolgaRetirada.getFieldDescription()), getType: typesKnockout.int, required: false, enable: ko.observable(false), visible: ko.observable(true) });

    this.DataValidadeLiberacaoSeguradora = PropertyEntity({ text: (_CONFIGURACAO_TMS.ExigirDatasValidadeCadastroMotorista ? "*" : "") + Localization.Resources.Transportadores.Motorista.DataDeValidadeDaSeguradora.getFieldDescription(), issue: 639, getType: typesKnockout.date, visible: ko.observable(false), required: _CONFIGURACAO_TMS.ExigirDatasValidadeCadastroMotorista, enable: ko.observable(false) });

    this.NumeroHabilitacao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroCNH.getFieldDescription()), maxlength: 17 });
    this.NumeroRegistroHabilitacao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroRegistroCNH.getFieldDescription()), maxlength: 50 });
    this.CategoriaHabilitacao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CategoriaCNH.getFieldDescription()), options: _categoriaHabilitacao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Email = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Email.getFieldDescription()), issue: 30, getType: typesKnockout.multiplesEmails, cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-2 col-lg-2"), enable: ko.observable(true) });
    this.TipoMotorista = PropertyEntity({ val: ko.observable(EnumTipoMotorista.Proprio), options: EnumTipoMotorista.obterOpcoes(), def: EnumTipoMotorista.Proprio, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoMotorista.getRequiredFieldDescription()), issue: 640, required: true });

    this.TipoResidencia =PropertyEntity({val: ko.observable(EnumTipoResidencia.Nenhum), options: EnumTipoResidencia.obterOpcoes(), def: EnumTipoResidencia.Nenhum, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoResidencia.getFieldDescription()), required: false, enable: ko.observable(true) });

    this.ClienteTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.PessoaAgregado.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.FilialPadrao.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoEndereco = PropertyEntity({ val: ko.observable(EnumTipoEndereco.Comercial), options: EnumTipoEndereco.obterOpcoes(), def: EnumTipoEndereco.Comercial, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoEndereco.getFieldDescription()), required: false });
    this.TipoEmail = PropertyEntity({ val: ko.observable(EnumTipoEmail.Principal), options: EnumTipoEmail.obterOpcoes(), def: EnumTipoEmail.Principal, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoEmail.getFieldDescription()), issue: 29, required: false, enable: ko.observable(true) });

    this.Endereco = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Logradouro.getFieldDescription()), issue: 18, maxlength: 150, required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-7") });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Bairro.getFieldDescription()), maxlength: 150, required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CEP.getFieldDescription()), issue: 117, required: false, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-10 col-lg-3") });
    this.Complemento = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Complemento.getFieldDescription()), maxlength: 150, enable: ko.observable(true) });
    this.NumeroEndereco = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Numero.getFieldDescription()), maxlength: 20, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: ko.observable(Localization.Resources.Transportadores.Motorista.Tipo.getFieldDescription()), required: false, enable: ko.observable(true) });                          
    this.EnderecoDigitado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Transportadores.Motorista.DigitarEndereco), def: ko.observable(false), enable: ko.observable(true) });
    this.NumeroCartao = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroCartao.getFieldDescription()), maxlength: 150, enable: ko.observable(true) });
    this.TipoCartao = PropertyEntity({ val: ko.observable(EnumTipoPessoaCartao.Nenhum), options: EnumTipoPessoaCartao.obterOpcoes(), def: EnumTipoPessoaCartao.Nenhum, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoCartao), required: false, enable: ko.observable(true) });

    this.NaoGeraComissaoAcerto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Transportadores.Motorista.NaoGerarComissaoAorealizarUmAcertoDeViagem), def: false, visible: ko.observable(false) });

    this.Visible2 = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Banco.getRequiredFieldDescription()), idBtnSearch: guid(), required: false });
    this.Agencia = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Agencia.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.Digito = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Digito.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 1 });
    this.NumeroConta = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroConta.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: ko.observable(Localization.Resources.Transportadores.Motorista.Tipo.getRequiredFieldDescription()), required: false });
    this.ObservacaoConta = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.ObservacaoDaConta.getFieldDescription()), maxlength: 5000, visible: ko.observable(true) });
    this.TipoChavePix = PropertyEntity({ val: ko.observable(""), options: EnumTipoChavePix.obterOpcoes(), def: "", text: Localization.Resources.Transportadores.Motorista.TipoChavePix.getFieldDescription(), required: false,visible: ko.observable(true) });
    this.ChavePix = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.ChavePix.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 36 });

    this.AtivarFichaMotorista = PropertyEntity({
        val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Transportadores.Motorista.AtivarLancamentoDaFichaDoMotorista), def: ko.observable(false), visible: ko.observable(false),
        eventChange: AtivarFichaMotoristaClick
    });

    this.UsuarioMobile = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.CadastrarMotoristaMobileAutomaticamente), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Motorista.EsseMotoristaUtilizaAplicativoMultiMobile, issue: 641, def: _CONFIGURACAO_TMS.CadastrarMotoristaMobileAutomaticamente, visible: ko.observable(false) });
    this.OrdenarCargasMobileCrescente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Motorista.OrdenarCargasMobileCrescente, def: false, visible: ko.observable(false) });
    this.ExigeContraSenha = PropertyEntity({ val: ko.observable(_ExigeContraSenha), getType: typesKnockout.bool, def: _ExigeContraSenha, getType: typesKnockout.bool });
    this.ContraSenhaMobile = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NovaContraSenhaMobile.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 80 });
    this.NaoBloquearAcessoSimultaneo = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.CadastrarMotoristaMobileAutomaticamente), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Motorista.PermitirQueMotoristaMudeDeAparelhoSemBloquearLogin, issue: 641, def: _CONFIGURACAO_TMS.CadastrarMotoristaMobileAutomaticamente, visible: ko.observable(false) });

    this.SN_Numero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Transportadores.Motorista.SemNumero), def: ko.observable(false) });
    this.ConsultarCEP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.NaoSeiCEP), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Latitude = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Latitude.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Longitude.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 20 });

    this.WhatsApp = PropertyEntity({ eventClick: whatsAppClick, type: types.event, text: "", icone: "fa fa-whatsapp", visible: ko.observable(true) });

    this.Empresa.val.subscribe(function (novoValor) {
        if (novoValor == "" && !_CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro) {
            _motorista.MotoristaEstrangeiro.val(false);
            _motorista.MotoristaEstrangeiro.visible(false);
        }

        if (novoValor == "") {
            _motorista.MotoristaEstrangeiro.val(false);
            _motorista.Empresa.OrdenarCargasMobileCrescente = false;
            validadeOrdemAppMotorista();
        }


    });

    //Geolocalização motorista local folga
    this.GeoLocalizacaoRaioLocalidade = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    this.RaioGeoLocalizacaoLocalidade = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.decimal, text: Localization.Resources.Transportadores.Motorista.RaioGeolocalizacaoLocalidade, enable: false });
    this.BuscarLatitudeLongitude = PropertyEntity({ eventClick: BuscarLatitudeLongitude, type: types.event, text: Localization.Resources.Transportadores.Motorista.BuscarLatitudeLongitude, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoArea = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.TipoArea, val: ko.observable(true), options: EnumTipoArea.obterOpcoes(), def: EnumTipoArea.Raio, enable: ko.observable(true) });
    this.TipoArea.val.subscribe(function () { setarTipoArea(); });
    this.Area = PropertyEntity();
    this.RaioEmMetros = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.RaioMetros.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 10, enable: ko.observable(true) });
    this.RaioEmMetros.val.subscribe(function () { setarRaioEmMetros(); });
    this.AlvoEstrategico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Transportadores.Motorista.AlvoEstrategico, visible: ko.observable(true), enable: ko.observable(true) });
    this.LatitudeTransbordo = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20, enable: ko.observable(true) });
    this.LongitudeTransbordo = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20, enable: ko.observable(true) });
    this.AtualizarPontoApoioMaisProximoAutomaticamente = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.AtualizarPontoApoioAutomaticamente.getFieldDescription(), val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, visible: ko.observable(true) });
    this.PontoDeApoio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.PontoDeApoio.getFieldDescription()), idBtnSearch: guid() });
    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasClick, type: types.event, text: Localization.Resources.Transportadores.Motorista.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });
    this.PontoTransbordo = PropertyEntity({ eventClick: PontoTransbordoClick, type: types.event, val: ko.observable(false) });
    this.PrecisaoCoordenadas = PropertyEntity({ visible: ko.observable(false) });
    this.Map = PropertyEntity();
    //Geolocalização motorista local folga - fim

    this.PlanoAcertoViagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.PlanoDoMotorista.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.SituacaoColaborador = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.SituacaoColaborador.getFieldDescription()), val: ko.observable(EnumSituacaoColaborador.Trabalhando), options: EnumSituacaoColaborador.obterOpcoes(), def: EnumSituacaoColaborador.Trabalhando, enable: ko.observable(false) });

    this.PISAdministrativo = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.PISAdministrativo.getFieldDescription()), maxlength: 200 });
    this.Cargo = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Cargo.getFieldDescription()), maxlength: 200, enable: ko.observable(true) });
    this.CBO = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CBO.getFieldDescription()), maxlength: 200, enable: ko.observable(true) });
    this.NumeroMatricula = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroMatricula.getFieldDescription()), maxlength: 200, enable: ko.observable(true) });
    this.NumeroProntuario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroProntuario.getFieldDescription()), maxlength: 200, enable: ko.observable(true) });
    this.DataFechamentoAcerto = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataFechamentoAcerto.getFieldDescription()), getType: typesKnockout.date, required: false, enable: ko.observable(false), visible: ko.observable(false) });

    //Aba Dados Adicionais
    this.Observacao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Observacao.getFieldDescription()), maxlength: 5000, enable: ko.observable(true), visible: ko.observable(true) });
    this.TituloEleitoral = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.TituloEleitoral.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.ZonaEleitoral = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.ZonaEleitoral.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.SecaoEleitoral = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.SecaoEleitoral.getFieldDescription()), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataExpedicaoCTPS = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataExpedicaoCTPS.getFieldDescription()), issue: 2, getType: typesKnockout.date, enable: ko.observable(true) });
    this.EstadoCivil = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.EstadoCivil.getFieldDescription()), val: ko.observable(EnumEstadoCivil.Outros), options: EnumEstadoCivil.obterOpcoes(), def: EnumEstadoCivil.Outros, enable: ko.observable(true) });
    this.CorRaca = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CorRaca.getFieldDescription()), val: ko.observable(EnumCorRaca.SemInformacao), options: EnumCorRaca.obterOpcoes(), def: EnumCorRaca.SemInformacao, enable: ko.observable(false) });
    this.Sexo = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Sexo.getFieldDescription()), val: ko.observable(EnumSexo.NaoInformado), options: EnumSexo.obterOpcoes(), def: EnumSexo.NaoInformado, enable: ko.observable(true) });
    this.Escolaridade = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Escolaridade.getFieldDescription()), val: ko.observable(EnumEscolaridade.SemInstrucaoFormal), options: EnumEscolaridade.obterOpcoes(), def: EnumEscolaridade.SemInstrucaoFormal, enable: ko.observable(true) });
    this.EstadoCTPS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.EstadoCTPS.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.LocalidadeNascimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.LocalidadeNascimento.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.CentroDeResultado.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.RenachCNH = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.RenachCNH.getFieldDescription()), maxlength: 11, enable: ko.observable(true) });
    this.PossuiControleDisponibilidade = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.ControlarDisponibilidade, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.CargoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Cargo.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Aposentadoria = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Aposentadoria.getFieldDescription()), val: ko.observable(EnumAposentadoriaFuncionario.NaoInformado), options: EnumAposentadoriaFuncionario.obterOpcoes(), def: EnumAposentadoriaFuncionario.NaoInformado, enable: ko.observable(true) });
    this.DataSuspensaoInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataSuspensaoInicio.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExibirCamposSuspensaoMotorista) });
    this.DataSuspensaoFim = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataSuspensaoFim.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExibirCamposSuspensaoMotorista) });
    this.MotivoBloqueio = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.MotivoBloqueio.getFieldDescription()), maxlength: 500, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExibirCamposSuspensaoMotorista) });
    this.SenhaGR = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.SenhaGR.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataValidadeGR = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataValidadeGR.getFieldDescription()), getType: typesKnockout.date });
    this.LocalidadeMunicipioEstadoCNH = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.LocalidadeMunicipioEstadoCNH.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoSegurancaCNH = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.CodigoSegurancaCNH.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.Gestor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.Gestor.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.NumeroCartaoValePedagio = PropertyEntity({ type: types.int, required: false, maxlength: 100, text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroCartaoValePedagio.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.FiliacaoMotoristaMae = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.FiliacaoMotoristaMae.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.FiliacaoMotoristaPai = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.FiliacaoMotoristaPai.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });


    this.Ajudante = PropertyEntity({ getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Transportadores.Motorista.Ajudante.getFieldDescription()), val: ko.observable(false), def: false, visible: ko.observable(_PermiteInformarAjudanteNaCarga) });
    
    //Aba Licenças
    this.CodigoLicenca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Transportadores.Motorista.Licenca), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription()), maxlength: 200, required: false });
    this.Numero = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroLicenca.getRequiredFieldDescription()), maxlength: 20, required: false });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Transportadores.Motorista.DataEmissao.getRequiredFieldDescription()), required: false });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Transportadores.Motorista.DataVencimento.getRequiredFieldDescription()), required: false });
    this.FormaAlerta = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.GerarAlertaAoResponsavel.getFieldDescription()), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [] });
    this.StatusLicenca = PropertyEntity({ val: ko.observable(EnumStatusLicenca.Vigente), options: EnumStatusLicenca.obterOpcoes(), def: EnumStatusLicenca.Vigente, text: ko.observable(Localization.Resources.Gerais.Geral.Status.getRequiredFieldDescription()) });

    this.BloquearCriacaoPedidoLicencaVencida = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.BloquearCriacaoDoPedidoSeLicencaEstiverVencida), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.GridMotoristaLicencas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.BloquearCriacaoPlanejamentoPedidoLicencaVencida = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.BloquearInsercaoNaTelaPlanejamentoDePedidosSeLicencaEstiverVencida), val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.AdicionarLicenca = PropertyEntity({ eventClick: adicionarMotoristaLicencaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarLicenca = PropertyEntity({ eventClick: atualizarMotoristaLicencaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirLicenca = PropertyEntity({ eventClick: excluirMotoristaLicencaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarLicenca = PropertyEntity({ eventClick: LimparCamposMotoristaLicencas, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba LiberacaoGR
    this.CodigoLiberacaoGR = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LicencaLiberacaoGR = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Transportadores.Motorista.Licenca), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DescricaoLiberacaoGR = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription()), maxlength: 200, required: false });
    this.NumeroLiberacaoGR = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Numero.getRequiredFieldDescription()), maxlength: 20, required: false });
    this.DataEmissaoLiberacaoGR = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Transportadores.Motorista.DataEmissao.getRequiredFieldDescription()), required: false });
    this.DataVencimentoLiberacaoGR = PropertyEntity({ getType: typesKnockout.date, text: ko.observable(Localization.Resources.Transportadores.Motorista.DataVencimento.getRequiredFieldDescription()), required: false });

    this.GridMotoristaLiberacoesGR = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarLiberacaoGR = PropertyEntity({ eventClick: adicionarMotoristaLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarLiberacaoGR = PropertyEntity({ eventClick: atualizarMotoristaLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirLiberacaoGR = PropertyEntity({ eventClick: excluirMotoristaLiberacaoGRClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarLiberacaoGR = PropertyEntity({ eventClick: LimparCamposMotoristaLiberacoesGR, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });


    //Aba Foto
    this.ArquivoFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Arquivo.getFieldDescription()), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.AdicionarFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.ArquivoFotoRemover = PropertyEntity({ eventClick: removerFotoMotorista, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Remover), visible: ko.observable(true) });
    this.FotoMotorista = PropertyEntity({});
    this.GaleriaMotorista = PropertyEntity({ val: ko.observable([]) });

    this.ArquivoFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            enviarFotoMotorista();
    });

    this.AdicionarFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            enviarFotoAdicionalMotorista();
    });

    this.Codigo.val.subscribe(function (codigoNovo) {
        if (codigoNovo > 0)
            $("#liTabFoto").show();
        else
            $("#liTabFoto").hide();
    });

    //Aba Permissões Mobile
    this.PerfilAcessoMobile = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.PerfilDeAcessoMobile.getFieldDescription()), issue: 597, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FormulariosUsuarioMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosUsuarioMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.UsuarioAdministradorMobile = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: ko.observable(Localization.Resources.Transportadores.Motorista.EsteUsuarioTemPermissoesParaTodosOsMenusDoAplicativo), visible: ko.observable(true) });
    this.UsuarioMobile.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liTabPermissoesMobile").show();
        else
            $("#liTabPermissoesMobile").hide();

        validadeOrdemAppMotorista();
    });

    //Aba Contatos
    this.GridMotoristaContatos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.NomeContato = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Nome.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 150, val: ko.observable("") });
    this.TelefoneContato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Telefone.getFieldDescription()), getType: typesKnockout.phone, required: false, visible: ko.observable(true), maxlength: 200, val: ko.observable("") });
    this.EmailContato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Email.getFieldDescription()), getType: typesKnockout.multiplesEmails, required: false, visible: ko.observable(true), maxlength: 1000, val: ko.observable("") });
    this.TipoParentescoContato = PropertyEntity({ val: ko.observable(EnumTipoParentesco.Nenhum), options: EnumTipoParentesco.obterOpcoes(), def: EnumTipoParentesco.Nenhum, text: ko.observable(Localization.Resources.Transportadores.Motorista.TipoParentesco.getFieldDescription()), required: false, enable: ko.observable(true) });
    this.CodigoContato = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CPFContato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.CPF.getFieldDescription()), getType: typesKnockout.cpf, required: false });
    this.DataNascimentoContato = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.DataNascimento.getFieldDescription()), getType: typesKnockout.date, required: false });

    this.AdicionarContato = PropertyEntity({ eventClick: adicionarMotoristaContatoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarContato = PropertyEntity({ eventClick: atualizarMotoristaContatoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirContato = PropertyEntity({ eventClick: excluirMotoristaContatoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarContato = PropertyEntity({ eventClick: LimparCamposMotoristaContatos, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba Dados Bancários
    this.GridMotoristaDadoBancarios = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.CodigoDadoBancario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BancoDadoBancario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Transportadores.Motorista.Banco.getRequiredFieldDescription()), idBtnSearch: guid(), required: false });
    this.AgenciaDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Agencia.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.DigitoDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.Digito.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 1 });
    this.NumeroContaDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.NumeroConta.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.TipoContaDadoBancario = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: ko.observable(Localization.Resources.Transportadores.Motorista.Tipo.getRequiredFieldDescription()), required: false });
    this.ObservacaoContaDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.ObservacaoDaConta.getFieldDescription()), maxlength: 5000, visible: ko.observable(true) });
    this.TipoChavePixDadoBancario = PropertyEntity({ val: ko.observable(""), options: EnumTipoChavePix.obterOpcoes(), def: "", text: Localization.Resources.Transportadores.Motorista.TipoChavePix.getFieldDescription(), required: false, visible: ko.observable(true) });
    this.ChavePixDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Transportadores.Motorista.ChavePix.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 36 });

    this.AdicionarDadoBancario = PropertyEntity({ eventClick: adicionarMotoristaDadoBancarioClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarDadoBancario = PropertyEntity({ eventClick: atualizarMotoristaDadoBancarioClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirDadoBancario = PropertyEntity({ eventClick: excluirMotoristaDadoBancarioClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarDadoBancario = PropertyEntity({ eventClick: LimparCamposMotoristaDadoBancarios, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba Transportadoras
    this.Transportadoras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridTransportadoras = PropertyEntity({ type: types.local });
    this.Transportadora = PropertyEntity({ type: types.event, text: Localization.Resources.Transportadores.Motorista.AdicionarTransportador, idBtnSearch: guid() });

    //Aba Situações Colaborador
    this.GridSituacoesColaborador = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    //Aba Locais de Carregamento Autorizados
    this.LocaisCarregamentosAutorizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridLocaisCarregamentoAutorizados = PropertyEntity({ type: types.local });
    this.LocaisCarregamentoAutorizados = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid() });
    this.RestringirLocaisCarregamentoAutorizadosMotoristas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });

    //Aba EPIs
    this.GridEPIs = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.CodigoEPI = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Motorista.EPI.getRequiredFieldDescription(), idBtnSearch: guid(), required: false });
    this.DataRepasse = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Transportadores.Motorista.DataRepasse.getFieldDescription() });
    this.SerieEPI = PropertyEntity({ text: Localization.Resources.Transportadores.Motorista.SerieEPI.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 30 });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Transportadores.Motorista.QuantidadeEPI.getRequiredFieldDescription(), val: ko.observable(0), def: 0, maxlength: 11, required: false });

    this.AdicionarEPI = PropertyEntity({ eventClick: adicionarMotoristaEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarEPI = PropertyEntity({ eventClick: atualizarMotoristaEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.ExcluirEPI = PropertyEntity({ eventClick: excluirMotoristaEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.CancelarEPI = PropertyEntity({ eventClick: LimparCamposMotoristaEPI, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    //CRUD
    this.Duplicar = PropertyEntity({ eventClick: duplicarClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.DuplicarCadastro), visible: ko.observable(_PermiteDuplicarCadastroMotorista) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Atualizar), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Excluir), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Cancelar), visible: ko.observable(false) });
    this.ZerarSaldo = PropertyEntity({ eventClick: zerarSaldoClick, type: types.event, text: ko.observable(Localization.Resources.Transportadores.Motorista.ZerarSaldoMotorista), visible: ko.observable(false) });

    this.DuplicarCadastro = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "Motorista/Importar",
        UrlConfiguracao: "Motorista/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O019_Motorista,
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        },
        CallbackImportacao: function () {
            _gridMotorista.CarregarGrid();
        }
    });    
};

function validadeOrdemAppMotorista() {
    if (_motorista.UsuarioMobile.val() && !_CONFIGURACAO_TMS.OrdenarCargasMobileCrescente && !_motorista.Empresa.OrdenarCargasMobileCrescente && _motorista.Empresa.val() != "")
        _motorista.OrdenarCargasMobileCrescente.visible(true);
    else
        _motorista.OrdenarCargasMobileCrescente.visible(false);
}

//*******EVENTOS*******

function loadMotorista() {
    _motorista = new Motorista();
    KoBindings(_motorista, "knockoutCadastroMotorista");

    HeaderAuditoria("Usuario", _motorista);

    _pesquisaMotorista = new PesquisaMotorista();
    KoBindings(_pesquisaMotorista, "knockoutPesquisaMotorista", false, _pesquisaMotorista.Pesquisar.id);

    loadMotoristaDadoBancario();
    loadMotoristaContato();
    loadMotoristaLicenca();
    loadMotoristaLiberacaoGR();
    loadMotoristaIntegracoes();
    loadMotoristaSituacaoColaborador();
    loadMotoristaEPIs();
    loadGeolocalizacaoMotorista();

    $("#" + _motorista.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _motorista.PISPASEP.id).mask("000.00000.00-0", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _motorista.EnderecoDigitado.id).click(digitarEndereco);
    $("#" + _motorista.SN_Numero.id).click(verificarSNNumero);

    $("#" + _motorista.NumeroHabilitacao.id).mask("09999999999999999", { selectOnFocus: true, clearIfNotMatch: true });

    new BuscarLocalidades(_motorista.Localidade);
    new BuscarTransportadores(_motorista.Empresa, RetornoEmpresa);
    new BuscarPlanoConta(_motorista.PlanoAcertoViagem, null, null, null, EnumAnaliticoSintetico.Analitico);
    new BuscarTransportadores(_pesquisaMotorista.Empresa, null, null, null, null, null, null, null, null, true);
    new BuscarEnderecos(_motorista.ConsultarCEP, null, null, selecionarEnderecoClick);
    new BuscarBanco(_motorista.Banco);
    new BuscarBanco(_motorista.BancoDadoBancario);
    new BuscarClientes(_motorista.ClienteTerceiro);
    new BuscarEstados(_motorista.EstadoRG);
    new BuscarEstados(_motorista.UFEmissaoCNH);
    new BuscarFilial(_motorista.Filial);
    new BuscarPerfilAcessoMobile(_motorista.PerfilAcessoMobile, PerfilAcessoMobileOnChange);
    new BuscarEstados(_motorista.EstadoCTPS);
    new BuscarLocalidades(_motorista.LocalidadeNascimento);
    new BuscarCentroResultado(_motorista.CentroResultado);
    new BuscarCargos(_motorista.CargoMotorista);
    new BuscarCargos(_pesquisaMotorista.CargoMotorista);
    new BuscarLocais(_motorista.PontoDeApoio);
    new BuscarLocalidades(_motorista.LocalidadeMunicipioEstadoCNH);
    new BuscarFuncionario(_motorista.Gestor);

    buscarMotoristas();
    buscarPaginasMobile();

    loadAnexos();
    loadTransportador();
    loadLocaisCarregamentoAutorizados();

    _motorista.SN_Numero.val(false);
    _motorista.NumeroEndereco.val("S/N");
    _motorista.NumeroEndereco.enable(false);
    _motorista.Duplicar.visible(_PermiteDuplicarCadastroMotorista && false);

    ConfigurarCamposPorTipoServico();

    desabilitaCamposEndereco();

    if (_CONFIGURACAO_TMS.IniciarCadastroFuncionarioMotoristaSempreInativo) {
        _motorista.Status.val("I");
        _motorista.Ativo.val(false);
    }

    ValidarCamposReferenteCIOTChange();

    SetarCamposObrigatoriosMotorista();

    configurarTabsPorTipoSistema();
}

function ValidarCamposReferenteCIOTChange() {
    var validarCamposReferenteCIOT = _motorista.ValidarCamposReferenteCIOT.val();

    if (_CONFIGURACAO_TMS.ExigirDatasValidadeCadastroMotorista || validarCamposReferenteCIOT) {
        _motorista.DataValidadeCNH.text("*" + _motorista.DataValidadeCNH.text());
        _motorista.DataValidadeCNH.required = true;
    } else {
        _motorista.DataValidadeCNH.text(_motorista.DataValidadeCNH.text().replace("*", ""));
        _motorista.DataValidadeCNH.required = false;
    }

    if (validarCamposReferenteCIOT) {
        _motorista.DataNascimento.text("*" + _motorista.DataNascimento.text());
        _motorista.Telefone.text("*" + _motorista.Telefone.text());
        _motorista.PISPASEP.text("*" + _motorista.PISPASEP.text());
        _motorista.DataNascimento.required = true;
        _motorista.Telefone.required = true;
        _motorista.PISPASEP.required = true;
    } else {
        _motorista.DataNascimento.text(_motorista.DataNascimento.text().replace("*", ""));
        _motorista.Telefone.text(_motorista.Telefone.text().replace("*", ""));
        _motorista.PISPASEP.text(_motorista.PISPASEP.text().replace("*", ""));
        _motorista.DataNascimento.required = false;
        _motorista.Telefone.required = false;
        _motorista.PISPASEP.required = false;
    }
}

function PerfilAcessoMobileOnChange(itemGrid) {
    executarReST("PerfilAcessoMobile/BuscarPorPerfilMobile", itemGrid, function (e) {
        if (e.Success) {
            if (e.Data) {
                limparPermissoesModulosFormulariosMobile();
                PreencherObjetoKnout(_motorista, e);
                setarPermissoesModulosFormulariosMobile();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, e.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
}

function RetornoEmpresa(data) {
    _motorista.Empresa.val(data.Descricao);
    _motorista.Empresa.codEntity(data.Codigo);
    _motorista.ValidarCamposReferenteCIOT.val(data.HabilitarCIOT);
    _motorista.RestringirLocaisCarregamentoAutorizadosMotoristas.val(data.RestringirLocaisCarregamentoAutorizadosMotoristas);
    _motorista.Empresa.OrdenarCargasMobileCrescente = data.OrdenarCargasMobileCrescente;
    controlarExibicaoLocaisCarregamentoAutorizados();

    bloquearCampoCPFPorTipoEmpresa(data.Tipo);
    validadeOrdemAppMotorista();
}

function AtivarFichaMotoristaClick(e) {
    if (_motorista.Visible2.visibleFade() == true) {
        _motorista.Visible2.visibleFade(false);
        _motorista.Banco.required = false;
        _motorista.Agencia.required = false;
        _motorista.Digito.required = false;
        _motorista.NumeroConta.required = false;
        _motorista.TipoConta.required = false;
    } else {
        _motorista.Visible2.visibleFade(true);
        _motorista.Banco.required = true;
        _motorista.Agencia.required = true;
        _motorista.Digito.required = false;
        _motorista.NumeroConta.required = true;
        _motorista.TipoConta.required = true;
    }
}

function ConfigurarCamposPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        ConfigurarCamposNFe();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        ConfigurarCamposTMS();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        ConfigurarCamposEmbarcador();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        ConfigurarCamposMultiCTe();
    }

    BloqueioCamposMotoristaLGPD();
}

function ConfigurarCamposNFe() {
    _pesquisaMotorista.Empresa.visible(false);
    _motorista.Empresa.visible(false);
    _motorista.Empresa.required = false;
    _motorista.AtivarFichaMotorista.visible(true);

    _motorista.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-8 col-lg-8");
    _motorista.CEP.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");

    $("#liTabTransportadoras").hide();
}

function ConfigurarCamposTMS() {
    _pesquisaMotorista.Empresa.visible(false);
    _motorista.Empresa.visible(false);
    _motorista.Empresa.required = false;
    _pesquisaMotorista.TipoMotorista.visible(true);

    _motorista.Nome.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    _motorista.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    _motorista.Bairro.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _motorista.CEP.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _motorista.Email.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _motorista.CodigoIntegracao.visible(true);
    _motorista.CodigoIntegracaoContabilidade.visible(true);
    _motorista.AtivarFichaMotorista.visible(!_CONFIGURACAO_TMS.HabilitarFichaMotoristaTodos);

    if (_CONFIGURACAO_TMS.HabilitarFichaMotoristaTodos) {
        _motorista.AtivarFichaMotorista.val(true);
        AtivarFichaMotoristaClick();
    }

    _motorista.NaoGeraComissaoAcerto.visible(true);

    _motorista.UsuarioMobile.visible(true);
    _motorista.NaoBloquearAcessoSimultaneo.visible(true);

    _motorista.CentroResultado.visible(true);
    _motorista.BloquearCriacaoPedidoLicencaVencida.visible(true);
    _motorista.BloquearCriacaoPlanejamentoPedidoLicencaVencida.visible(true);
    _motorista.PossuiControleDisponibilidade.visible(true);
    _motorista.DataFechamentoAcerto.visible(true);
    _motorista.Bloqueado.visible(true);
    _motorista.NumeroCartaoValePedagio.visible(true);

    $("#liTabTransportadoras").hide();
    $("#liTabConfiguracao").show();
}

function ConfigurarCamposEmbarcador() {
    _motorista.Empresa.visible(true);
    _motorista.Empresa.required = true;
    _motorista.DataValidadeLiberacaoSeguradora.visible(true);

    _motorista.Nome.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    _motorista.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-5 col-lg-5");
    _motorista.CEP.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _motorista.EstadoRG.cssClass("col col-xs-12 col-sm-6 col-md-4");
    _motorista.EmissorRG.cssClass("col col-xs-12 col-sm-6 col-md-2 col-lg-2");


    _motorista.UsuarioMobile.visible(true);
    _motorista.NaoBloquearAcessoSimultaneo.visible(true);


    _motorista.CodigoIntegracao.visible(true);
    _motorista.Filial.visible(true);

    var permiteBloquearMotorista = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Motorista_PermiteBloquearMotorista, _PermissoesPersonalizadas);

    _motorista.Bloqueado.visible(permiteBloquearMotorista);
    _motorista.DataValidadeLiberacaoSeguradora.enable(true);

    $("#liTabConfiguracao").hide();
}

function ConfigurarCamposMultiCTe() {
    _pesquisaMotorista.Empresa.visible(false);
    _motorista.Empresa.visible(false);
    _motorista.Empresa.required = false;
    _motorista.DataValidadeLiberacaoSeguradora.visible(true);

    _motorista.DataValidadeLiberacaoSeguradora.enable(!_CONFIGURACAO_TMS.NaoPermitirTransportadoAlterarDataValidadeSeguradora);

    _motorista.Nome.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    _motorista.Email.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    _motorista.CEP.cssClass("col col-xs-12 col-sm-6 col-md-4 col-lg-3");
    _motorista.Endereco.cssClass("col col-xs-12 col-sm-12 col-md-5 col-lg-5");
    _motorista.Bairro.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");

    $("#liTabTransportadoras").hide();

    if (_CONFIGURACAO_TMS.ExibirConfiguracoesPortalTransportador) {
        _motorista.UsuarioMobile.visible(true);
        _motorista.NaoBloquearAcessoSimultaneo.visible(true);
    }
}

function BloqueioCamposMotoristaLGPD() {
    if (_CONFIGURACAO_TMS.BloquearCamposMotoristaLGPD) {
        $("#liTabAdicionais").hide();

        _motorista.Apelido.enable(false);
        _motorista.TipoEmail.enable(false);
        _motorista.Email.enable(false);
        _motorista.DataAdmissao.enable(false);
        _motorista.DataFimPeriodoExperiencia.enable(false);
        _motorista.DataDemissao.enable(false);
        _motorista.Cargo.enable(false);
        _motorista.CBO.enable(false);
        _motorista.NumeroMatricula.enable(false);
        _motorista.NumeroProntuario.enable(false);



    }
}

function adicionarClick(e, sender) {
    if (!validarCamposMotorista())
        return;

    resetarTabs();
    buscarPermissoesFormulariosMobile();
    preencherListasSelecaoMotorista();

    Salvar(e, "Motorista/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirModalLGPD) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    carregarModalMotoristaLGPD();
                }
                else {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridMotorista.CarregarGrid();
                    limparCamposMotorista();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function duplicarClick(e, sender) {
    if (!validarCamposMotorista())
        return;

    resetarTabs();
    buscarPermissoesFormulariosMobile();
    preencherListasSelecaoMotorista();

    _motorista.Codigo.val(0);
    _motorista.DuplicarCadastro.val(true);

    Salvar(e, "Motorista/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirModalLGPD) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    carregarModalMotoristaLGPD();
                }
                else {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridMotorista.CarregarGrid();
                    limparCamposMotorista();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function zerarSaldoClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaZerarSaldoDoMotorista.format(_motorista.Nome.val()), function () {
        Salvar(_motorista, "Motorista/ZerarSaldoMotorista", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Transportadores.Motorista.SaldoZeradoComSucesso);
                    _gridMotorista.CarregarGrid();
                    limparCamposMotorista();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        }, null);
    });
}

function atualizarClick(e, sender) {
    if (!validarCamposMotorista())
        return;

    resetarTabs();
    buscarPermissoesFormulariosMobile();
    preencherListasSelecaoMotorista();

    //ABA MAPA
    setarCoordenadas();
    setarRaioEmMetros();
    SetarAreaGeoLocalizacao();

    if (!_motorista.Ativo.val()) {
        _mensagemMotoristaVinculado = "";
        ValidarMotoristaPlanejamentoFrota(e).then(function () {
            if (_mensagemMotoristaVinculado != "") {
                exibirConfirmacao("Motorista Vinculado a um planejamento", _mensagemMotoristaVinculado, function () {
                    AtualizarMotorista(e, sender);
                });
            } else
                AtualizarMotorista(e, sender);
        });

    } else
        AtualizarMotorista(e, sender);


}

function AtualizarMotorista(e, sender) {
    Salvar(e, "Motorista/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirModalLGPD) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    carregarModalMotoristaLGPD();
                }
                else {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    _gridMotorista.CarregarGrid();
                    limparCamposMotorista();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}


function ValidarMotoristaPlanejamentoFrota(e) {
    var p = new promise.Promise();

    var data = { Codigo: e.Codigo.val() }
    executarReST("Motorista/ValidarMotoristaPlanejamentoFrota", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirConfirmacaoMotoristaPlanejamentoFuturo) {
                    _mensagemMotoristaVinculado = arg.Msg;
                } else
                    _mensagemMotoristaVinculado = "";
            } else {
                _mensagemMotoristaVinculado = "";
            }
        } else {
            _mensagemMotoristaVinculado = "";
        }

        p.done();
    });

    return p;
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Transportadores.Motorista.RealmenteDesejaExcluirMotorista.format(_motorista.Nome.val()), function () {
        ExcluirPorCodigo(_motorista, "Motorista/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridMotorista.CarregarGrid();
                    limparCamposMotorista();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestão, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposMotorista();
}

function preencherListasSelecaoMotorista() {
    _motorista.Transportadoras.val(JSON.stringify(_motorista.Transportadora.basicTable.BuscarRegistros()));
    _motorista.LocaisCarregamentosAutorizados.val(JSON.stringify(_motorista.LocaisCarregamentoAutorizados.basicTable.BuscarRegistros()));
}

//*******MÉTODOS*******

function validarCamposMotorista() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        var permiteBloquearMotorista = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Motorista_PermiteBloquearMotorista, _PermissoesPersonalizadas);
        if (permiteBloquearMotorista && _motorista.Bloqueado.val() && string.IsNullOrWhiteSpace(_motorista.MotivoBloqueio.val())) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Motorista.NecessarioInformarMotivoBloqueio)
            _motorista.MotivoBloqueio.requiredClass("form-control is-invalid");
            return false;
        }
    }

    return true;
}

function buscarMotoristas() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarMotorista, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMotorista = new GridView(_pesquisaMotorista.Pesquisar.idGrid, "Motorista/Pesquisa", _pesquisaMotorista, menuOpcoes, null);
    _gridMotorista.CarregarGrid();
}

function editarMotorista(motoristaGrid) {
    limparCamposMotorista();
    _motorista.Codigo.val(motoristaGrid.Codigo);
    BuscarPorCodigo(_motorista, "Motorista/BuscarPorCodigo", function (arg) {
        var data = arg.Data;
        resetarTabs();
        verificaNumero(_motorista);
        veriticaEnderecoDigitado(_motorista);
        verificaDigitarEndereco();
        setarPermissoesModulosFormulariosMobile();
        recarregarGridTransportadoras();
        recarregarMotoristaIntegracoes();
        recarregarGridMotoristaLicencas();
        recarregarGridMotoristaLiberacoesGR();
        recarregarGridMotoristaDadoBancarios();
        recarregarGridMotoristaContatos();
        recarregarGridSituacaoColaborador();
        recarregarGridLocaisCarregamentoAutorizados();
        recarregarGridEPIs();
        EditarListarAnexos(arg);
        _motorista.Empresa.OrdenarCargasMobileCrescente = arg.Data.EmpresaOrdenarCargasMobileCrescente;
        validadeOrdemAppMotorista();
        //ABA MAPA
        setarCoordenadas();
        setarRaioEmMetros();
        SetarAreaGeoLocalizacao();

        _pesquisaMotorista.ExibirFiltros.visibleFade(false);
        _motorista.Atualizar.visible(true);
        _motorista.Cancelar.visible(true);
        _motorista.Excluir.visible(true);

        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Motorista_PermitirDuplicarCadastro, _PermissoesPersonalizadas)) 
            _motorista.Duplicar.visible(_PermiteDuplicarCadastroMotorista && true);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Motorista_PermiteZerarSaldo, _PermissoesPersonalizadas)))
            _motorista.ZerarSaldo.visible(true);

        if (!_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Motorista_NaoPermitirAlterarCentroDeResultado, _PermissoesPersonalizadas))) {
            _motorista.CentroResultado.enable(false);
        }

        _motorista.Adicionar.visible(false);

        if (_motorista.AtivarFichaMotorista.val() || _CONFIGURACAO_TMS.HabilitarFichaMotoristaTodos) {
            _motorista.Visible2.visibleFade(true);
            _motorista.Banco.required = true;
            _motorista.Agencia.required = true;
            _motorista.Digito.required = false;
            _motorista.NumeroConta.required = true;
            _motorista.TipoConta.required = true;
        } else {
            _motorista.Visible2.visibleFade(false);
            _motorista.Banco.required = false;
            _motorista.Agencia.required = false;
            _motorista.Digito.required = false;
            _motorista.NumeroConta.required = false;
            _motorista.TipoConta.required = false;
        }

        // Caso a flag global de usar a foto do app para o motorista esteja ativa, busca a foto do servidor central de autenticação do app (tms.multicte)
        if (!_CONFIGURACAO_TMS.MotoristaUsarFotoDoApp) {
            carregarFotoDoAppMotorista();
        }



        _motorista.GaleriaMotorista.val(arg.Data.GaleriaMotorista);

        if (arg.Data.Empresa != null)
            bloquearCampoCPFPorTipoEmpresa(arg.Data.Empresa.Tipo);
    }, null);
}

function limparCamposMotorista() {
    _motorista.Atualizar.visible(false);
    _motorista.Cancelar.visible(false);
    _motorista.Excluir.visible(false);
    _motorista.ZerarSaldo.visible(false);
    _motorista.Adicionar.visible(true);
    _motorista.Duplicar.visible(_PermiteDuplicarCadastroMotorista && false);
    _motorista.DuplicarCadastro.val(false);
    LimparCampos(_motorista);
    LimparCamposTransportador();
    limparCamposMapaRequest()
    LimparCamposMotoristaLicencas();

    recarregarGridTransportadoras();
    recarregarMotoristaIntegracoes();
    recarregarGridMotoristaLicencas();
    recarregarGridMotoristaLiberacoesGR();
    recarregarGridMotoristaDadoBancarios();
    recarregarGridMotoristaContatos();
    recarregarGridSituacaoColaborador();
    recarregarGridLocaisCarregamentoAutorizados();
    recarregarGridEPIs();
    resetarTabs();

    $("#" + _motorista.EnderecoDigitado.id).prop("checked", false);
    $("#" + _motorista.SN_Numero.id).prop("checked", true);
    verificarSNNumero;
    _motorista.SN_Numero.val(false);
    _motorista.NumeroEndereco.val("S/N");
    _motorista.NumeroEndereco.enable(false);

    _motorista.Visible2.visibleFade(false);
    _motorista.AtivarFichaMotorista.val(false);
    _motorista.Banco.required = false;
    _motorista.Agencia.required = false;
    _motorista.Digito.required = false;
    _motorista.NumeroConta.required = false;
    _motorista.TipoConta.required = false;

    if (_CONFIGURACAO_TMS.IniciarCadastroFuncionarioMotoristaSempreInativo) {
        _motorista.Status.val("I");
        _motorista.Ativo.val(false);
    }

    limparPermissoesModulosFormulariosMobile();
    $("#liTabAnexos").hide();
    desabilitaCamposEndereco();
    _motorista.Empresa.OrdenarCargasMobileCrescente = false;
    validadeOrdemAppMotorista();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

//*******EVENTOS DE ENDEREÇO*******

function habilitaCamposEndereco() {
    _motorista.Bairro.enable(true);
    _motorista.Endereco.enable(true);
    _motorista.Localidade.enable(true);
}

function desabilitaCamposEndereco() {
    _motorista.Bairro.enable(false);
    _motorista.Endereco.enable(false);
    _motorista.Localidade.enable(false);
}

function controlarCamposMotoristaEstrangeiro() {
    var motoristaEstrangeiro = _motorista.MotoristaEstrangeiro.val();

    if (motoristaEstrangeiro)
        LimparCampo(_motorista.CPF);
    else
        LimparCampo(_motorista.CodigoMotoristaEstrangeiro);

    _motorista.CPF.required = !motoristaEstrangeiro;
    _motorista.CPF.visible(!motoristaEstrangeiro);
    _motorista.CodigoMotoristaEstrangeiro.visible(motoristaEstrangeiro);
}

function consultaEnderecoCEP(e) {
    if ($("#" + _motorista.CEP.id).val().match(/\d/g) != null && $("#" + _motorista.CEP.id).val().match(/\d/g).join("").length == 8) {
        var data = { CEP: $("#" + _motorista.CEP.id).val() };
        executarReST("Localidade/BuscarEnderecoPorCEP", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null && arg.Data.DescricaoCidadeEstado != null && arg.Data.DescricaoCidadeEstado != "" && arg.Data.CodigoCidade > 0) {
                    _motorista.EnderecoDigitado.val(false);
                    _motorista.Bairro.val(arg.Data.Bairro);
                    _motorista.Endereco.val(arg.Data.Logradouro);
                    _motorista.Localidade.codEntity(arg.Data.CodigoCidade);
                    _motorista.Localidade.val(arg.Data.DescricaoCidadeEstado);
                    _motorista.Latitude.val(arg.Data.Latitude);
                    _motorista.Longitude.val(arg.Data.Longitude);
                    _motorista.TipoLogradouro.val(arg.Data.EnumTipoLogradouro);

                    $("#" + _motorista.NumeroEndereco.id).focus();
                    verificaDigitarEndereco();
                    verificaEnderecoUnico(arg.Data.DescricaoCidade);

                } else if (_motorista.EnderecoDigitado.val() == false) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Motorista.ConsultaDeCEP, Localization.Resources.Transportadores.Motorista.CEPInformadoNaoExisteNaBaseDeDados);
                    _motorista.EnderecoDigitado.val(true);
                    _motorista.Bairro.val("");
                    _motorista.Endereco.val("");
                    LimparCampoEntity(_motorista.Localidade);
                    verificaDigitarEndereco();
                } else if (_motorista.EnderecoDigitado.val() == true) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Motorista.ConsultaDeCEP, Localization.Resources.Transportadores.Motorista.CEPInformadoNaoExisteNaBaseDeDados);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
            }
        });
    }
}

function verificaNumero(e, sender) {
    if (_motorista.NumeroEndereco.val() == "S/N" || _motorista.NumeroEndereco.val() == "") {
        _motorista.SN_Numero.val(false);
        _motorista.NumeroEndereco.val("S/N");
        _motorista.NumeroEndereco.enable(false);
    } else {
        _motorista.SN_Numero.val(true);
        _motorista.NumeroEndereco.enable(true);
    }
}

function verificarSNNumero(e, sender) {
    if (_motorista.SN_Numero.val() == false) {
        _motorista.NumeroEndereco.val("S/N");
        _motorista.NumeroEndereco.enable(false);
    } else {
        if (_motorista.NumeroEndereco.val("S/N"))
            _motorista.NumeroEndereco.val("");
        _motorista.NumeroEndereco.enable(true);
    }
}

function digitarEndereco(e, sender) {
    verificaDigitarEndereco();
}

function abrirModaConsultarCEPClick(e, sender) {
    Global.abrirModal('divModalConsultaEndereco');
}

function selecionarEnderecoClick(enderecoSelecionado) {
    if (enderecoSelecionado != null) {
        Global.fecharModal('divModalConsultaEndereco');
        _motorista.EnderecoDigitado.val(false);
        _motorista.CEP.val(enderecoSelecionado.CEP);
        _motorista.Bairro.val(enderecoSelecionado.Bairro);
        _motorista.Endereco.val(enderecoSelecionado.Logradouro);
        _motorista.Localidade.codEntity(enderecoSelecionado.CodigoCidade);
        _motorista.Localidade.val(enderecoSelecionado.Descricao);
        _motorista.Latitude.val(enderecoSelecionado.Latitude);
        _motorista.Longitude.val(enderecoSelecionado.Longitude);
        if (enderecoSelecionado.TipoLogradouro != null && enderecoSelecionado.TipoLogradouro != "") {
            if (enderecoSelecionado.TipoLogradouro == "Rua")
                _motorista.TipoLogradouro.val(1);
            else if (enderecoSelecionado.TipoLogradouro == "Avenida")
                _motorista.TipoLogradouro.val(2);
            else if (enderecoSelecionado.TipoLogradouro == "Rodovia")
                _motorista.TipoLogradouro.val(3);
            else if (enderecoSelecionado.TipoLogradouro == "Estrada")
                _motorista.TipoLogradouro.val(4);
            else if (enderecoSelecionado.TipoLogradouro == "Praca")
                _motorista.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Praça")
                _motorista.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Travessa")
                _motorista.TipoLogradouro.val(6);
            else
                _motorista.TipoLogradouro.val(99);
        }
        $("#" + _motorista.Complemento.id).focus();
        verificaDigitarEndereco();
        verificaEnderecoUnico(enderecoSelecionado.DescricaoCidade);
    }
}

function pesquisarEnderecosClick(e, sender) {
    var data = {
        Logradouro: _pesquisaEndereco.Logradouro.val(),
        CEP: _pesquisaEndereco.CEP.val(),
        Bairro: _pesquisaEndereco.Bairro.val(),
        Descricao: _pesquisaEndereco.Localidade.val(),
        CodigoIBGE: _pesquisaEndereco.CodigoIBGE.val(),
        CodigoCidade: _pesquisaEndereco.Localidade.codEntity(),
        NomeCidade: _pesquisaEndereco.Localidade.val()
    };

    var realizaConsulta = false;

    realizaConsulta = data.Logradouro != "" || data.CEP != "" || data.Bairro != "" || data.Descricao != "" || data.CodigoIBGE != "" || data.CodigoCidade != "" || data.NomeCidade != "";

    if (realizaConsulta) {
        executarReST("Localidade/BuscarEnderecosCorreio", data, function (arg) {
            if (arg.Success) {
                $.each(arg.Data, function (i, endereco) {

                    var selecionar = { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: guid(), evento: "onclick", metodo: selecionarEnderecoClick, tamanho: "15", icone: "" };
                    var menuOpcoes = new Object();
                    menuOpcoes.tipo = TypeOptionMenu.link;
                    menuOpcoes.opcoes = new Array();
                    menuOpcoes.opcoes.push(selecionar);

                    var header = [
                        { data: "Descricao", title: Localization.Resources.Transportadores.Motorista.Cidade, width: "20%", className: "text-align-left" },
                        { data: "Logradouro", title: Localization.Resources.Transportadores.Motorista.Endereco, width: "20%", className: "text-align-left" },
                        { data: "Bairro", title: Localization.Resources.Transportadores.Motorista.Bairro, width: "15%" },
                        { data: "CEP", title: Localization.Resources.Transportadores.Motorista.CEP, width: "10%", className: "text-align-right" },
                        { data: "CodigoIBGE", title: Localization.Resources.Transportadores.Motorista.CodigoIBGE, width: "10%", className: "text-align-right" },
                        { data: "TipoLogradouro", title: Localization.Resources.Transportadores.Motorista.Tipo, width: "10%", className: "text-align-left" },
                        { data: "CodigoCidade", visible: false },
                        { data: "Latitude", visible: false },
                        { data: "Longitude", visible: false },
                        { data: "DescricaoCidade", visible: false }
                    ];
                    var gridEnderecos = new BasicDataTable(_pesquisaEndereco.Enderecos.idGrid, header, menuOpcoes);
                    gridEnderecos.CarregarGrid(endereco);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.falha, arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Transportadores.Motorista.PorFavorInformeAoMenosUmDosCamposParaRealizarConsulta);
}

function veriticaEnderecoDigitado(e, sender) {
    _motorista.EnderecoDigitado.val(_motorista.EnderecoDigitado.val());
}

function cancelarEnderecoClick(e, sender) {
    Global.fecharModal('divModalConsultaEndereco');
}

function verificaEnderecoUnico(nomeCidade) {
    if (_motorista.Endereco.val() == null || _motorista.Endereco.val() == "" || removeAcento(_motorista.Endereco.val().toUpperCase()) == removeAcento(nomeCidade.toUpperCase())) {
        _motorista.Bairro.enable(true);
        _motorista.Endereco.enable(true);
        $("#" + _motorista.Endereco.id).focus();
    } else {
        _motorista.Bairro.enable(false);
        _motorista.Endereco.enable(false);
    }
}

function carregarConteudosHTML(callback) {
    $.get("Content/Static/Localidade/Localidade.html?dyn=" + guid(), function (data) {
        $("#ConsultaEndereco").html(data);
        _pesquisaEndereco = new PesquisaEndereco();
        KoBindings(_pesquisaEndereco, "knoutConsultaEndereco");
        new BuscarLocalidadesBrasil(_pesquisaEndereco.Localidade);
        $("#" + _pesquisaEndereco.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    });
}

function verificaDigitarEndereco() {
    if (_motorista.EnderecoDigitado.val() == true) {
        habilitaCamposEndereco();
    } else {
        desabilitaCamposEndereco();
    }
}

function removeAcento(strToReplace) {
    str_acento = "áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ";
    str_sem_acento = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC";
    var nova = "";
    for (var i = 0; i < strToReplace.length; i++) {
        if (str_acento.indexOf(strToReplace.charAt(i)) != -1) {
            nova += str_sem_acento.substr(str_acento.search(strToReplace.substr(i, 1)), 1);
        } else {
            nova += strToReplace.substr(i, 1);
        }
    }
    return nova;
}

function BuscarLatitudeLongitude() {
    var lat = parseFloat(String(_motorista.Latitude.val()).replace(',', '.'));
    var long = parseFloat(String(_motorista.Longitude.val()).replace(',', '.'));
    if (!isNaN(lat) != 0 && !isNaN(long) != 0) {
        _motorista.Latitude.val(lat);
        _motorista.Longitude.val(long);
        var position = new google.maps.LatLng(lat, long);
        if (_marker != null) {
            _marker.setPosition(position);
            _map.panTo(position);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Latitude ou Longitude informada está errada!");
    }
}

function bloquearCampoCPFPorTipoEmpresa(tipoEmpresa) {
    if (tipoEmpresa == "E") {
        _motorista.MotoristaEstrangeiro.val(true);
        _motorista.MotoristaEstrangeiro.visible(true);
    } else if (!_CONFIGURACAO_TMS.PermitirCadastrarMotoristaEstrangeiro) {
        _motorista.MotoristaEstrangeiro.val(false);
        _motorista.MotoristaEstrangeiro.visible(false);
    } else {
        _motorista.MotoristaEstrangeiro.val(false);
    }
}

function whatsAppClick() {

    if (_motorista.Celular.val() == "") {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Motorista não possui um número de celular cadastrado.");
        return;
    }

    var celular = obterSomenteNumeros(_motorista.Celular.val());

    var url = "https://api.whatsapp.com/send?phone=55" + celular;
    window.open(url);
}

function obterSomenteNumeros(string) {
    var x = string.replace(/[^0-9]/g, '');
    return parseInt(x);
}

function configurarTabsPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabLiberacaoGR").hide();

    }

}

