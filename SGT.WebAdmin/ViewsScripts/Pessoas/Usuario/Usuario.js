/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/Turno.js" />
/// <reference path="../../Consultas/PerfilAcesso.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Consultas/Licenca.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/PerfilAcessoMobile.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../Enumeradores/EnumControleAlertaForma.js" />
/// <reference path="../../Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../Enumeradores/EnumTipoUsuario.js" />
/// <reference path="../../Enumeradores/EnumTipoLogradouro.js" />
/// <reference path="../../Enumeradores/EnumTipoConta.js" />
/// <reference path="../../Enumeradores/EnumCorRaca.js" />
/// <reference path="../../Enumeradores/EnumEscolaridade.js" />
/// <reference path="../../Enumeradores/EnumEstadoCivil.js" />
/// <reference path="../../Enumeradores/EnumTipoComercial.js" />
/// <reference path="../../Enumeradores/EnumAposentadoriaFuncionario.js" />
/// <reference path="../../Enumeradores/EnumFormaAutenticacaoUsuario.js" />
/// <reference path="../../Enumeradores/StatusUsuario.js" />
/// <reference path="PerfilAcesso.js" />
/// <reference path="PermissaoUsuario.js" />
/// <reference path="PermissaoUsuarioMobile.js" />
/// <reference path="Anexo.js" />
/// <reference path="UsuarioLicencas.js" />
/// <reference path="UsuarioFoto.js" />
/// <reference path="UsuarioDadoBancario.js" />
/// <reference path="UsuarioContato.js" />
/// <reference path="Representacoes.js" />
/// <reference path="UsuarioEmpresa.js" />
/// <reference path="UsuarioCentroResultado.js" />
/// <reference path="UsuarioEPIs.js" />
/// <reference path="UsuarioMeta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridUsuario;
var _pesquisaUsuario;
var _PoliticaSenha;
var _usuario, _PermissoesPersonalizadas;

var PesquisaEndereco = function () {
    this.Logradouro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Endereco.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Bairro.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.CEP.getFieldDescription()), required: false });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Loclaization.Resources.Pessoas.Usuario.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoIBGE = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.CodigoIBGE.getFieldDescription()), required: false });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarEnderecosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: ko.observable(true) });

    this.Enderecos = PropertyEntity({ type: types.local, idGrid: guid() });

    this.CancelarEndereco = PropertyEntity({ eventClick: cancelarEnderecoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};

var PesquisaUsuario = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Nome.getFieldDescription() });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Todas), options: EnumTipoPessoa.obterOpcoesPesquisa(false), text: Localization.Resources.Pessoas.Usuario.Tipo.getFieldDescription(), def: EnumTipoPessoa.Todas, enable: ko.observable(true) });
    this.CPF = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CPFOuCNPJ.getFieldDescription(), getType: typesKnockout.cpfCnpj });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Login.getFieldDescription() });
    this.PerfilAcesso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.PerfilDeAcesso.getFieldDescription(), issue: 597, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumTipoUsuario.Funcionarios), options: EnumTipoUsuario.obterOpcoesPesquisa(), def: EnumTipoUsuario.Funcionarios, text: Localization.Resources.Pessoas.Usuario.TipoUsuario.getFieldDescription() });
    this.NumeroMatricula = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NumeroMatricula.getFieldDescription() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CodigoIntegracao.getFieldDescription(), required: false, maxlength: 50 });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.Cidade.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridUsuario.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Usuario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable("A"), def: "A" });
    this.Nome = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Nome.getRequiredFieldDescription(), issue: 586, required: true });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoa.Fisica), eventChange: tipoPessoaChange, options: EnumTipoPessoa.obterOpcoes(false), text: Localization.Resources.Pessoas.Usuario.Tipo.getRequiredFieldDescription(), def: EnumTipoPessoa.Fisica, enable: ko.observable(true) });
    this.CPF = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.CPFBarraCNPJ.getRequiredFieldDescription()), required: true, getType: typesKnockout.string });
    this.RG = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RG.getFieldDescription(), maxlength: 50 });
    this.Telefone = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Telefone.getFieldDescription(), issue: 17, getType: typesKnockout.phone });
    this.DataNascimento = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataNascimento.getFieldDescription(), issue: 2, getType: typesKnockout.date });
    this.DataAdmissao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataAdmissao.getFieldDescription(), issue: 2, getType: typesKnockout.date });
    this.DataFimPeriodoExperiencia = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataFimPeriodoExperiencia.getFieldDescription(), issue: 2, getType: typesKnockout.date });
    this.DataDemissao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataDemissao.getFieldDescription(), issue: 2, getType: typesKnockout.date });
    this.NumeroCTPS = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NumeroCTPS.getFieldDescription(), maxlength: 100 });
    this.SerieCTPS = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.SerieCTPS.getFieldDescription(), maxlength: 100 });
    this.Salario = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Salario.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CodigoIntegracao.getFieldDescription(), required: false, maxlength: 50 });

    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.SetorFuncionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.Setor.getFieldDescription(), issue: 1774, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Turno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.Turno.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CargoSetorTurno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Cargo.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.CentroDeCustoSetorTurno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.CentroDeCustoSetorTurno.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });

    this.NivelEscalationList = PropertyEntity({ val: ko.observable(EnumEscalationList.Nenhum), options: EnumEscalationList.obterOpcoesCadastro(), text: Localization.Resources.Pessoas.Usuario.NivelEscalationList.getFieldDescription(), def: EnumEscalationList.Nenhum, enable: ko.observable(true), visible: ko.observable(false) });
    this.ClienteSetor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Cliente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarArvoreDecisaoEscalationList) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.Cidade.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Endereco = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Logradouro.getFieldDescription()), issue: 19, maxlength: 150, required: false, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Bairro.getFieldDescription()), maxlength: 150, required: false, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CEP.getFieldDescription(), issue: 117, required: false, enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Complemento.getFieldDescription()), maxlength: 150, enable: ko.observable(true) });
    this.NumeroEndereco = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Numero.getFieldDescription()), maxlength: 20, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: Localization.Resources.Pessoas.Usuario.Tipo.getFieldDescription(), required: false, enable: ko.observable(true) });
    this.EnderecoDigitado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.DigitarEndereco, def: ko.observable(false), enable: ko.observable(true) });
    this.SN_Numero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.SemNumero, def: ko.observable(false) });
    this.ConsultarCEP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Pessoas.Usuario.NaoSeiCEP, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Latitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });

    this.Email = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Email.getRequiredFieldDescription(), issue: 5, required: true, getType: typesKnockout.multiplesEmails, cssClass: ko.observable("col-10") });
    this.Status = PropertyEntity({ val: ko.observable(EnumStatusUsuario.Ativo), options: EnumStatusUsuario.obterOpcoes(), def: EnumStatusUsuario.Ativo, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557, required: true });
    this.NotificadoExpedicao = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioSeraNotificadoPelaExpedicao, issue: 662, visible: ko.observable(true) });
    this.NotificacaoPorEmail = PropertyEntity({ val: ko.observable(true), getType: typesKnockout.bool, def: true, text: Localization.Resources.Pessoas.Usuario.NotificarUsuarioPorEmail, issue: 1131 });
    this.AssociarUsuarioCliente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Usuario.AssociarEsteUsuarioUmCliente, issue: 1921, visible: ko.observable(false) });

    this.UsuarioMobile = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.EsseFuncionarioUtilizaAplicaticavoMultiMobile, issue: 641, def: false, visible: ko.observable(true) });
    this.ExigeContraSenha = PropertyEntity({ val: ko.observable(_ExigeContraSenha), getType: typesKnockout.bool, def: _ExigeContraSenha, getType: typesKnockout.bool });
    this.ContraSenhaMobile = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NovaContraSenhaMobile.getFieldDescription(), required: false, visible: ko.observable(false), maxlength: 80 });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Cliente.getRequiredFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.ContaDoUsuario.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoMovimentoComissao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.TipMovimentoParaComissao.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.DiaComissao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Pessoas.Usuario.DiaDaComissao.getFieldDescription(), maxlength: 2, required: ko.observable(false), visible: ko.observable(true) });

    this.Operador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), eventChange: mudarOperadorOnChange, options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: false, text: Localization.Resources.Pessoas.Usuario.UsuarioOperadorLogistico.getFieldDescription(), issue: 602 });
    this.OperadorSupervisor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.UsuarioSupervisor, visible: ko.observable(false) });
    this.PermiteAdicionarComplementosDeFrete = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.PermitirQueEsteUsuarioInformeComplementosDeFrete, visible: ko.observable(false) });
    this.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, text: Localization.Resources.Pessoas.Usuario.PermitirQueEsteUsuarioVisualizeValorDeFreteDosTransportadoresInteressdaos, visible: ko.observable(false) });
    this.PermitirAssumirCargasControleEntrega = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, text: Localization.Resources.Pessoas.Usuario.PermitirQueEsteUsuarioAssumaCargasComoAnalistaNoControleDeEntrega, visible: ko.observable(false) });

    this.UsuarioAcessoBloqueado = PropertyEntity({ val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Enumeradores.UsuarioAcessoSistema.Bloqueado, Localization.Resources.Enumeradores.UsuarioAcessoSistema.Liberado), def: false, text: Localization.Resources.Pessoas.Usuario.AcessoAoSistema.getFieldDescription(), issue: 661 });
    this.LiberarAuditoria = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.LiberarAuditoria, visible: ko.observable(false) });
    this.ExibirUsuarioAprovacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, text: Localization.Resources.Pessoas.Usuario.EsseUsuarioSeraExibidoNasAprovacoesParaOsTransportadores, visible: ko.observable(false) });
    this.LimitarOperacaoPorEmpresa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.LimitarOperacoesDoOperadorDeterminadasEmpresas, visible: ko.observable(false) });

    this.Login = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DescricaoUsuario.getRequiredFieldDescription(), issue: 657, required: true, maxlength: 50, visible: ko.observable(true), cssClass: ko.observable("col-4"), cssGroupClass: ko.observable("") });
    this.Senha = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Senha.getRequiredFieldDescription(), issue: 6, required: true, maxlength: 30, visible: ko.observable(true) });
    this.ConfimarSenha = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Confirmacao.getRequiredFieldDescription(), required: false, type: types.local, maxlength: 30, visible: ko.observable(true) });

    this.RedefinarSenha = PropertyEntity({ eventClick: redefinirSenhaClick, type: types.event, text: Localization.Resources.Pessoas.Usuario.RedefinirSenha, visible: ko.observable(false) });

    this.SituacaoColaborador = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.SituacaoColaborador.getFieldDescription(), val: ko.observable(EnumSituacaoColaborador.Trabalhando), options: EnumSituacaoColaborador.obterOpcoes(), def: EnumSituacaoColaborador.Trabalhando, enable: ko.observable(false) });

    this.PISAdministrativo = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.PISAdmin.getFieldDescription(), maxlength: 200 });
    this.Cargo = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Cargo.getFieldDescription(), maxlength: 200 });
    this.CBO = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CBO.getFieldDescription(), maxlength: 200 });
    this.NumeroMatricula = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NumeroMatricula.getFieldDescription(), maxlength: 200 });

    //Aba Dados Adicionais
    this.Observacao = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Observacoes.getFieldDescription(), maxlength: 5000, enable: ko.observable(true), visible: ko.observable(true) });
    this.TituloEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.TituloEleitoral.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.ZonaEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.ZonaEleitoral.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.SecaoEleitoral = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.SecaoEleitoral.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataExpedicaoCTPS = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataExpedicaoCTPS.getFieldDescription(), issue: 2, getType: typesKnockout.date, enable: ko.observable(true) });
    this.EstadoCivil = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.EstadoCivil.getFieldDescription(), val: ko.observable(EnumEstadoCivil.Outros), options: EnumEstadoCivil.obterOpcoes(), def: EnumEstadoCivil.Outros, enable: ko.observable(true) });
    this.CorRaca = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CorBarraRaca.getFieldDescription(), val: ko.observable(EnumCorRaca.SemInformacao), options: EnumCorRaca.obterOpcoes(), def: EnumCorRaca.SemInformacao, enable: ko.observable(false) });
    this.Escolaridade = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Escolaridade.getFieldDescription(), val: ko.observable(EnumEscolaridade.SemInstrucaoFormal), options: EnumEscolaridade.obterOpcoes(), def: EnumEscolaridade.SemInstrucaoFormal, enable: ko.observable(true) });
    this.TipoComercial = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Comercial.getFieldDescription(), val: ko.observable(EnumTipoComercial.Selecione), options: EnumTipoComercial.obterOpcoes(true), def: EnumTipoComercial.Selecione, enable: ko.observable(false), eventChange: tipoComercialChange, visible: ko.observable(false) });
    this.TipoComercialCheck = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });
    this.EstadoCTPS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.EstadoCTPS.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Gerente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Gerente.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Supervisor.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.LocalidadeNascimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.LocalidadeNascimento.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.NotificarOcorrenciaEntrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Usuario.NotificarVendedorPorOcorrenciasDeEntregaPorEmail, visible: ko.observable(false) });
    this.PermiteAssumirOcorrencia = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Usuario.PermitirAssumirOcorrencias, visible: ko.observable(false) });
    this.Aposentadoria = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.Aposentadoria.getFieldDescription(), val: ko.observable(EnumAposentadoriaFuncionario.NaoInformado), options: EnumAposentadoriaFuncionario.obterOpcoes(), def: EnumAposentadoriaFuncionario.NaoInformado, enable: ko.observable(true) });
    this.FormaAutenticacaoUsuario = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.FormaAutenticacao.getFieldDescription(), val: ko.observable(EnumFormaAutenticacaoUsuario.Todas), options: EnumFormaAutenticacaoUsuario.obterOpcoes(), def: EnumFormaAutenticacaoUsuario.Todas, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.LoginAD) });
    this.AreaContainer = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.AreaContainer.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PermiteInserirDicas = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.PermiteInserirDicas, val: ko.observable(false), def: false });
    this.PermitirAprovarNaoConformidade = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.PermitirAprovarNaoConformidade, val: ko.observable(false), def: false });
    this.NotificarEtapasBidding = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.NotificarEtapasBidding, val: ko.observable(false), def: false });
    this.PermitirAssumirAtendimentoManeiraSobreposta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Usuario.PermitirAssumirAtendimentoManeiraSobreposta, val: ko.observable(false), def: false });

    this.UsuarioAtendimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Usuario.UsuarioAtendimento, visible: ko.observable(false) });
    this.UsuarioCallCenter = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Pessoas.Usuario.UsuarioCallCenter, visible: ko.observable(false) });
    this.UsuarioMultisoftware = PropertyEntity({ val: ko.observable(false) });
    this.UsuarioUtilizaSegregacaoPorProvedor = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.UsuarioUtilizaSegregacaoPorProvedor, visible: ko.observable(_HabilitarFuncionalidadesProjetoGollum) });

    //Aba Permissões
    this.PerfilAcesso = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.PerfilDeAcesso.getFieldDescription(), issue: 597, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FormulariosUsuario = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosUsuario = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.HoraInicialAcesso = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.HorarioInicialAcesso.getFieldDescription(), getType: typesKnockout.time });
    this.HoraFinalAcesso = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.HorarioFinalAcesso.getFieldDescription(), getType: typesKnockout.time });
    this.UsuarioAdministrador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesDeAdministradorAcessoCompletoAoSistema, visible: ko.observable(true) });
    this.PermiteSalvarNovoRelatorio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesParaSalvarUmNovoRelatorio, visible: ko.observable(true) });
    this.PermiteTornarRelatorioPadrao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesDeTornarUmRelatorioComoPadrao, visible: ko.observable(true) });
    this.PermiteSalvarConfiguracoesRelatoriosParaTodos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesDeSalcarConfiguracaoParaTodosOsOutrosUsuarios, visible: ko.observable(true) });
    this.VisualizarGraficosIniciais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesDeVisualizaGraficosDoFaturamentoNaTelaInicial, visible: ko.observable(false) });
    this.VisualizarTitulosPagamentoSalario = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.VisualizarTitulosDePagamentoDeSalario, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.VisualizarControleDashRegiaoOperador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.RegiaoNorte = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RegiaoNorte, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegiaoNordeste = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RegiaoNordeste, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegiaoSul = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RegiaoSul, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegiaoSudeste = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RegiaoSudeste, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.RegiaoCentroOeste = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.RegiaoCentroOeste, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });

    //Aba Permissões Mobile
    this.PerfilAcessoMobile = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.PerfilDeAcessoMobile.getFieldDescription(), issue: 597, idBtnSearch: guid(), visible: ko.observable(true) });
    this.FormulariosUsuarioMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.ModulosUsuarioMobile = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0) });
    this.UsuarioAdministradorMobile = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true, text: Localization.Resources.Pessoas.Usuario.EsteUsuarioTemPermissoesParaTodosOsMenusDoAplicativo, visible: ko.observable(true) });
    this.UsuarioMobile.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liTabPermissoesMobile").show();
        else
            $("#liTabPermissoesMobile").hide();
    });


    //Aba Licenças
    this.CodigoLicenca = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: 200, required: false });
    this.Numero = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NumeroLicenca.getRequiredFieldDescription(), maxlength: 20, required: false });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Usuario.DataEmissao.getRequiredFieldDescription(), required: false });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Usuario.DataVencimento.getRequiredFieldDescription(), required: false });
    this.FormaAlerta = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.GerarAlertaAoResponsavel.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [] });
    this.StatusLicenca = PropertyEntity({ val: ko.observable(EnumStatusLicenca.Vigente), options: EnumStatusLicenca.obterOpcoes(), def: EnumStatusLicenca.Vigente, text: Localization.Resources.Gerais.Geral.Status.getRequiredFieldDescription() });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Pessoas.Usuario.Licenca), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GridUsuarioLicencas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });

    this.AdicionarLicenca = PropertyEntity({ eventClick: adicionarUsuarioLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarLicenca = PropertyEntity({ eventClick: atualizarUsuarioLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirLicenca = PropertyEntity({ eventClick: excluirUsuarioLicencaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarLicenca = PropertyEntity({ eventClick: LimparCamposUsuarioLicencas, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba Dados Bancários
    this.GridUsuarioDadoBancarios = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.CodigoDadoBancario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BancoDadoBancario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Pessoas.Usuario.Banco.getRequiredFieldDescription()), idBtnSearch: guid(), required: false });
    this.AgenciaDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Agencia.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.DigitoDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Digito.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 1 });
    this.NumeroContaDadoBancario = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.NumeroConta.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 50 });
    this.TipoContaDadoBancario = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: Localization.Resources.Pessoas.Usuario.Tipo.getRequiredFieldDescription(), required: false });
    this.ObservacaoContaDadoBancario = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.ObservacaoDaconta.getFieldDescription(), maxlength: 5000, visible: ko.observable(true) });

    this.AdicionarDadoBancario = PropertyEntity({ eventClick: adicionarUsuarioDadoBancarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarDadoBancario = PropertyEntity({ eventClick: atualizarUsuarioDadoBancarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirDadoBancario = PropertyEntity({ eventClick: excluirUsuarioDadoBancarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarDadoBancario = PropertyEntity({ eventClick: LimparCamposUsuarioDadoBancarios, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba Foto
    this.ArquivoFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.Arquivo.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.ArquivoFotoRemover = PropertyEntity({ eventClick: removerFotoUsuario, type: types.event, text: Localization.Resources.Gerais.Geral.Remover, visible: ko.observable(true) });
    this.FotoUsuario = PropertyEntity({});

    this.ArquivoFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            enviarFotoUsuario();
    });

    this.Codigo.val.subscribe(function (codigoNovo) {
        if (codigoNovo > 0)
            $("#liTabFoto").show();
        else
            $("#liTabFoto").hide();
    });

    //Aba Contatos
    this.GridUsuarioContatos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.NomeContato = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Nome.getRequiredFieldDescription()), required: false, visible: ko.observable(true), maxlength: 150, val: ko.observable("") });
    this.TelefoneContato = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Telefone.getFieldDescription()), getType: typesKnockout.phone, required: false, visible: ko.observable(true), maxlength: 200, val: ko.observable("") });
    this.EmailContato = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Usuario.Email.getFieldDescription()), getType: typesKnockout.multiplesEmails, required: false, visible: ko.observable(true), maxlength: 1000, val: ko.observable("") });
    this.TipoParentescoContato = PropertyEntity({ val: ko.observable(EnumTipoParentesco.Nenhum), options: EnumTipoParentesco.obterOpcoes(), def: EnumTipoParentesco.Nenhum, text: Localization.Resources.Pessoas.Usuario.TipoParentesco.getFieldDescription(), required: false, enable: ko.observable(true) });
    this.CodigoContato = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CPFContato = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.CPF.getFieldDescription(), getType: typesKnockout.cpf, required: false });
    this.DataNascimentoContato = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.DataNascimento.getFieldDescription(), getType: typesKnockout.date, required: false });

    this.AdicionarContato = PropertyEntity({ eventClick: adicionarUsuarioContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarContato = PropertyEntity({ eventClick: atualizarUsuarioContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirContato = PropertyEntity({ eventClick: excluirUsuarioContatoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarContato = PropertyEntity({ eventClick: LimparCamposUsuarioContatos, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });

    //Aba Empresas
    this.Empresas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridEmpresas = PropertyEntity({ type: types.local });
    this.Empresa = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.Usuario.AdicionarEmpresaBarraFilial, idBtnSearch: guid() });

    //Aba Representações
    this.GridRepresentacoes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.AdicionarRepresentacoes = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.Usuario.AdicionarClientes, idBtnSearch: guid() });

    //Aba Centros Resultado
    this.CentrosResultado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridCentrosResultado = PropertyEntity({ type: types.local });
    this.CentroResultado = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.Usuario.AdicionarCentroDeResultado, idBtnSearch: guid() });

    //Aba EPIs
    this.GridEPIs = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.CodigoEPI = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EPI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Usuario.EPI.getRequiredFieldDescription(), idBtnSearch: guid(), required: false });
    this.DataRepasse = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Usuario.DataRepasseEPI.getFieldDescription() });
    this.SerieEPI = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.SerieEPI.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 30 });
    this.Quantidade = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Pessoas.Usuario.QuantidadeEPI.getRequiredFieldDescription(), val: ko.observable(0), def: 0, maxlength: 11, required: false });

    this.AdicionarEPI = PropertyEntity({ eventClick: adicionarUsuarioEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarEPI = PropertyEntity({ eventClick: atualizarUsuarioEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.ExcluirEPI = PropertyEntity({ eventClick: excluirUsuarioEPIClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.CancelarEPI = PropertyEntity({ eventClick: LimparCamposUsuarioEPI, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    //Aba Meta
    this.GridMetas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true) });
    this.CodigoMeta = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicialMeta = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Usuario.DataInicialMeta.getRequiredFieldDescription() });
    this.DataFinalMeta = PropertyEntity({ getType: typesKnockout.date, text: Localization.Resources.Pessoas.Usuario.DataFinalMeta.getRequiredFieldDescription() });
    this.PercentualMeta = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Pessoas.Usuario.PercentualMeta.getRequiredFieldDescription(), val: ko.observable(0), def: 0, maxlength: 11, required: false });
    this.TipoMetaVendaDireta = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.TipoMetaVendaDireta.getRequiredFieldDescription(), val: ko.observable(EnumTipoMetaVendaDireta.Agendamento), options: EnumTipoMetaVendaDireta.obterOpcoes(), def: EnumTipoMetaVendaDireta.Agendamento, enable: ko.observable(true), visible: ko.observable(true) });
    this.StatusMeta = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.StatusMeta.getRequiredFieldDescription(), val: ko.observable(true), options: _status, def: true });

    this.AdicionarMeta = PropertyEntity({ eventClick: adicionarUsuarioMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AtualizarMeta = PropertyEntity({ eventClick: atualizarUsuarioMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.ExcluirMeta = PropertyEntity({ eventClick: excluirUsuarioMetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.CancelarMeta = PropertyEntity({ eventClick: LimparCamposUsuarioMeta, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    //Aba Provedor Usuário
    this.ProvedoresUsuarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.GridProvedoresUsuarios = PropertyEntity({ type: types.local });
    this.ProvedorUsuario = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.Usuario.AdicionarProvedor, idBtnSearch: guid() });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        ManterArquivoServidor: true,
        UrlImportacao: "Usuario/Importar",
        UrlConfiguracao: "Usuario/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O077_PlanejamentoVolume,
        CallbackImportacao: function () {
            _gridUsuario.CarregarGrid();
        },
        ParametrosRequisicao: function () {
            return {
                Inserir: true,
                Atualizar: true
            };
        }
    });
};

//*******EVENTOS*******

function loadUsuario() {
    executarReST("PoliticaSenha/BuscarPoliticaSenhaPorServicoMultiSoftware", null, function (arg) {
        if (arg.Success) {
            _PoliticaSenha = arg.Data;

            _usuario = new Usuario();
            KoBindings(_usuario, "knockoutCadastroUsuario");

            _usuario.Login.val("");
            _usuario.Senha.val("");

            _pesquisaUsuario = new PesquisaUsuario();
            KoBindings(_pesquisaUsuario, "knockoutPesquisaUsuario", false, _pesquisaUsuario.Pesquisar.id);

            HeaderAuditoria("Usuario", _usuario);

            new BuscarLocalidades(_usuario.Localidade);
            new BuscarLocalidades(_pesquisaUsuario.Localidade);
            new BuscarPerfilAcesso(_usuario.PerfilAcesso, PerfilAcessoOnChange);
            new BuscarPerfilAcesso(_pesquisaUsuario.PerfilAcesso);
            new BuscarClientes(_usuario.Cliente);
            new BuscarFilial(_usuario.Filial);
            new BuscarSetorFuncionario(_usuario.SetorFuncionario);
            new BuscarTurno(_usuario.Turno, null, null, _usuario.Filial, _usuario.SetorFuncionario);
            new BuscarPlanoConta(_usuario.PlanoConta);
            new BuscarTipoMovimento(_usuario.TipoMovimentoComissao);
            new BuscarLicenca(_usuario.Licenca);
            new BuscarPerfilAcessoMobile(_usuario.PerfilAcessoMobile, PerfilAcessoMobileOnChange);
            new BuscarEnderecos(_usuario.ConsultarCEP, null, null, selecionarEnderecoClick);
            new BuscarBanco(_usuario.BancoDadoBancario);
            new BuscarEstados(_usuario.EstadoCTPS);
            new BuscarLocalidades(_usuario.LocalidadeNascimento);
            new BuscarClientes(_usuario.AreaContainer);
            new BuscarClientes(_usuario.ClienteSetor);
            new BuscarFuncionario(_usuario.Gerente, null, null, null, null, null, null, EnumTipoComercial.Gerente);
            new BuscarFuncionario(_usuario.Supervisor, null, null, null, null, null, null, EnumTipoComercial.Supervisor);
            new BuscarCargos(_usuario.CargoSetorTurno);
            new BuscarCentroCustoViagem(_usuario.CentroDeCustoSetorTurno);

            buscarUsuarios();
            buscarPaginas();
            buscarPaginasMobile();

            _usuario.SN_Numero.val(false);
            _usuario.NumeroEndereco.val("S/N");
            _usuario.NumeroEndereco.enable(false);

            configurarLayoutUsuarioPorTipoSistema();

            if (_CONFIGURACAO_TMS.HabilitarArvoreDecisaoEscalationList)
                _usuario.NivelEscalationList.visible(true)

            if (_CONFIGURACAO_TMS.IniciarCadastroFuncionarioMotoristaSempreInativo) {
                _usuario.Status.val("I");
            }

            $("#" + _usuario.AssociarUsuarioCliente.id).click(verificarAssociarUsuarioCliente);
            $("#" + _usuario.LimitarOperacaoPorEmpresa.id).click(verificarLimitarOperacaoPorEmpresa);
            $("#" + _usuario.UsuarioUtilizaSegregacaoPorProvedor.id).click(verificarUsuarioUtilizaSegregacaoPorProvedor);
            $("#" + _usuario.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
            $("#" + _usuario.EnderecoDigitado.id).click(digitarEndereco);
            $("#" + _usuario.SN_Numero.id).click(verificarSNNumero);
            $("#" + _usuario.TipoComercialCheck.id).click(habilitaCampoComercial);

            loadUsuarioDadoBancario();
            loadPerfilAcesso();
            loadUsuarioLicenca();
            loadAnexos();
            loadUsuarioContato();
            loadEmpresa();
            loadRepresentacao();
            loadCentroResultado();
            tipoPessoaChange();
            loadUsuarioEPIs();
            loadUsuarioMetas();
            loadProvedorUsuario();
            VisibilidadePermissoes();
            desabilitaCamposEndereco();
            limparCamposUsuario();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function configurarLayoutUsuarioPorTipoSistema() {
    $('#liTabRepresentacoes').hide();
    $("#liTabMeta").hide();
    $("#liTabEmpresas").hide();

    if (!_CONFIGURACAO_TMS.UsaPermissaoControladorRelatorios) {
        _usuario.PermiteSalvarNovoRelatorio.visible(false);
        _usuario.PermiteTornarRelatorioPadrao.visible(false);
        _usuario.PermiteSalvarConfiguracoesRelatoriosParaTodos.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _usuario.LimitarOperacaoPorEmpresa.visible(true);
        _usuario.VisualizarTitulosPagamentoSalario.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _usuario.ExibirUsuarioAprovacao.visible(true);
        _usuario.TipoComercial.visible(true);
        _usuario.PermiteAssumirOcorrencia.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFeAdmin) {
        $("#liTabLogistica").hide();
        $("#liTabSetorTurno").hide();
        $("#liTabMeta").show();
        _usuario.VisualizarGraficosIniciais.visible(true);
        _usuario.NotificadoExpedicao.visible(false);
        _usuario.UsuarioMobile.visible(false);
        _usuario.PerfilAcesso.visible(false);
        _pesquisaUsuario.PerfilAcesso.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _usuario.AssociarUsuarioCliente.visible(true);
        _usuario.LiberarAuditoria.visible(true);
    }

    if (!_CONFIGURACAO_TMS.PermiteVisualizarTitulosPagamentoSalario) {
        _usuario.Salario.visible(false);
        _usuario.Email.cssClass("col-12");
    }
}

function adicionarClick(e, sender) {
    if ((_usuario.Senha.val() == _usuario.ConfimarSenha.val()) || _PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
        buscarPermissoesFormularios();
        buscarPermissoesFormulariosMobile();
        preencherListasSelecaoUsuario();

        Salvar(e, "Usuario/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                    _gridUsuario.CarregarGrid();
                    limparCamposUsuario();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    } else {
        resetarTabs();
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Usuario.ValidacaoSenha, Localization.Resources.Pessoas.Usuario.ConfirmacaoDeveSerIgualSenha);
    }
}

function atualizarClick(e, sender) {
    if ((_usuario.Senha.val() == _usuario.ConfimarSenha.val()) || _PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
        buscarPermissoesFormularios();
        buscarPermissoesFormulariosMobile();
        preencherListasSelecaoUsuario();

        Salvar(e, "Usuario/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    _gridUsuario.CarregarGrid();
                    limparCamposUsuario();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    } else {
        resetarTabs();
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Usuario.ValidacaoSenha, Localization.Resources.Pessoas.Usuario.ConfirmacaoDeveSerIgualSenha);
    }
}

function cancelarClick(e) {
    resetarTabs();
    limparCamposUsuario();
}

function preencherListasSelecaoUsuario() {
    _usuario.Empresas.val(JSON.stringify(_usuario.Empresa.basicTable.BuscarRegistros()));
    _usuario.CentrosResultado.val(JSON.stringify(_usuario.CentroResultado.basicTable.BuscarRegistros()));
    _usuario.ProvedoresUsuarios.val(JSON.stringify(_usuario.ProvedoresUsuarios.basicTable.BuscarRegistros()));
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Usuario.RealmenteDesejaExcluirUsuario.format(_usuario.Nome.val()), function () {
        ExcluirPorCodigo(_usuario, "Usuario/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridUsuario.CarregarGrid();
                    limparCamposUsuario();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function PerfilAcessoMobileOnChange(itemGrid) {
    executarReST("PerfilAcessoMobile/BuscarPorPerfilMobile", itemGrid, function (e) {
        if (e.Success) {
            if (e.Data) {
                limparPermissoesModulosFormulariosMobile();
                PreencherObjetoKnout(_usuario, e);
                setarPermissoesModulosFormulariosMobile();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, e.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
        }
    });
}

function mudarOperadorOnChange(e, sender) {
    if (e.Operador.val()) {
        e.PermiteAdicionarComplementosDeFrete.visible(true);
        e.OperadorSupervisor.visible(true);
        e.PermitirVisualizarValorFreteTransportadoresInteressadosCarga.visible(true);
        e.PermitirAssumirCargasControleEntrega.visible(true);
    }
    else {
        e.PermiteAdicionarComplementosDeFrete.visible(false);
        e.OperadorSupervisor.visible(false);
        e.PermitirVisualizarValorFreteTransportadoresInteressadosCarga.visible(false);
        e.PermitirAssumirCargasControleEntrega.visible(false);
    }
}

function redefinirSenhaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pessoas.Usuario.RealmenteDesejaRedefinirSenhaDesteUsuarioLembrandoQueAposRedefinicaoDeSenhaIraPerderSenhaAtualSeraObrigadoDefinirUmaNovaSenhaEmSeuProximoAcesso, function () {
        Salvar(e, "Usuario/RedefinirSenha", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Usuario.SenhaRedefinidaComSucesso);
                } else {
                    exibirMensagem("aviso", Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, exibirCamposObrigatorio);
    });
}

function verificarAssociarUsuarioCliente(e, sender) {
    if (_usuario.AssociarUsuarioCliente.val()) {
        _usuario.Cliente.visible(true);
        _usuario.Cliente.required(true);
    } else {
        _usuario.Cliente.visible(false);
        _usuario.Cliente.required(false);
    }
}

function verificarLimitarOperacaoPorEmpresa(e, sender) {
    if (_usuario.LimitarOperacaoPorEmpresa.val())
        $("#liTabEmpresas").show();
    else
        $("#liTabEmpresas").hide();
}

function verificarUsuarioUtilizaSegregacaoPorProvedor(e, sender) {
    if (_usuario.UsuarioUtilizaSegregacaoPorProvedor.val())
        $("#liTabProvedoresUsuario").show();
    else
        $("#liTabProvedoresUsuario").hide();
}

//*******MÉTODOS*******

function buscarUsuarios() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarUsuario, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridUsuario = new GridView(_pesquisaUsuario.Pesquisar.idGrid, "Usuario/Pesquisa", _pesquisaUsuario, menuOpcoes, null);
    _gridUsuario.CarregarGrid();
}

function editarUsuario(usuarioGrid) {
    limparCamposUsuario();
    _usuario.Codigo.val(usuarioGrid.Codigo);
    BuscarPorCodigo(_usuario, "Usuario/BuscarPorCodigo", function (arg) {
        setarPermissoesModulosFormularios();
        setarPermissoesModulosFormulariosMobile();
        _pesquisaUsuario.ExibirFiltros.visibleFade(false);
        _usuario.ConfimarSenha.val(_usuario.Senha.val());
        _usuario.Atualizar.visible(true);
        _usuario.Cancelar.visible(true);
        _usuario.Excluir.visible(true);
        _usuario.Adicionar.visible(false);
        _usuario.Senha.required = false;
        if (_PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
            _usuario.Login.cssGroupClass("input-group");
            _usuario.RedefinarSenha.visible(true);
        }

        if (arg.Data.UsuarioMultisoftware) {
            _usuario.UsuarioAtendimento.visible(true);
            _usuario.UsuarioCallCenter.visible(true);
        }

        if (arg.Data.UsuarioLogadoAtendimento)
            _usuario.UsuarioCallCenter.visible(true);

        if (_usuario.Operador.val()) {
            _usuario.OperadorSupervisor.visible(true);
            _usuario.PermiteAdicionarComplementosDeFrete.visible(true);
            _usuario.PermitirVisualizarValorFreteTransportadoresInteressadosCarga.visible(true);
        }
        else {
            _usuario.OperadorSupervisor.visible(false);
            _usuario.PermiteAdicionarComplementosDeFrete.visible(false);
            _usuario.PermiteAdicionarComplementosDeFrete.visible(false);
            _usuario.PermitirVisualizarValorFreteTransportadoresInteressadosCarga.visible(false);
        }

        $('#liTabRepresentacoes').hide();
        if (_usuario.TipoComercial.val() != 0) {
            _usuario.TipoComercial.enable(true);
            $("#" + _usuario.TipoComercialCheck.id).prop("checked", true);
            $('#liTabRepresentacoes').show();
        }

        verificaNumero(_usuario);
        veriticaEnderecoDigitado(_usuario);
        verificaDigitarEndereco();
        verificarAssociarUsuarioCliente();
        verificarLimitarOperacaoPorEmpresa();
        verificarUsuarioUtilizaSegregacaoPorProvedor();

        recarregarGridUsuarioDadoBancarios();
        recarregarGridUsuarioLicencas();
        recarregarGridUsuarioContatos();
        recarregarGridEmpresas();
        recarregarGridProvedoresUsuarios();
        recarregarGridRepresentacoes();
        recarregarGridCentrosResultado();
        recarregarGridEPIs();
        recarregarGridMetas();
        EditarListarAnexos(arg);

        tipoPessoaChange();
        habilitaCampoComercial();
        VisibilidadePermissoes();
    }, null);
}

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function limparCamposUsuario() {
    limparPermissoesModulosFormularios();
    limparPermissoesModulosFormulariosMobile();
    _usuario.ConfimarSenha.val("");
    _usuario.Atualizar.visible(false);
    _usuario.Cancelar.visible(false);
    _usuario.Excluir.visible(false);
    _usuario.Adicionar.visible(true);
    _usuario.OperadorSupervisor.visible(false);
    _usuario.PermiteAdicionarComplementosDeFrete.visible(false);
    _usuario.PermitirVisualizarValorFreteTransportadoresInteressadosCarga.visible(false);
    _usuario.Login.cssGroupClass("");
    _usuario.RedefinarSenha.visible(false);
    _usuario.Cliente.visible(false);
    _usuario.Cliente.required(false);
    _usuario.UsuarioAtendimento.visible(false);
    _usuario.UsuarioCallCenter.visible(false);

    LimparCamposUsuarioLicencas();

    if (!_PoliticaSenha.ExigirTrocaSenhaPrimeiroAcesso) {
        _usuario.Senha.required = true;
    }
    else {
        _usuario.Login.val("");
        _usuario.Senha.val("");
        _usuario.Senha.visible(false);
        _usuario.Senha.required = false;
        _usuario.ConfimarSenha.visible(false);
        _usuario.Login.cssClass("col-8");
    }

    LimparCampos(_usuario);
    LimparCamposEmpresa();
    LimparCamposCentroResultado();
    limparAnexosTela();
    $("#liTabAnexos").hide();

    recarregarGridUsuarioDadoBancarios();
    recarregarGridUsuarioLicencas();
    recarregarGridUsuarioContatos();
    recarregarGridEmpresas();
    recarregarGridProvedoresUsuarios();
    recarregarGridRepresentacoes();
    recarregarGridCentrosResultado();
    recarregarGridEPIs();
    recarregarGridMetas();
    resetarTabs();

    $("#" + _usuario.EnderecoDigitado.id).prop("checked", false);
    $("#" + _usuario.SN_Numero.id).prop("checked", true);
    $("#" + _usuario.TipoComercialCheck.id).prop("checked", false);

    _usuario.TipoComercial.enable(false);
    habilitaCampoComercial();
    verificarLimitarOperacaoPorEmpresa();
    verificarUsuarioUtilizaSegregacaoPorProvedor();

    verificarSNNumero;
    _usuario.SN_Numero.val(false);
    _usuario.NumeroEndereco.val("S/N");
    _usuario.NumeroEndereco.enable(false);
    desabilitaCamposEndereco();

    if (_CONFIGURACAO_TMS.IniciarCadastroFuncionarioMotoristaSempreInativo) {
        _usuario.Status.val("I");
    }

    VisibilidadePermissoes();
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function habilitaCampoComercial() {
    _usuario.TipoComercial.enable($("#" + _usuario.TipoComercialCheck.id).prop("checked"));
    if (_usuario.TipoComercial.enable()) {
        $('#liTabRepresentacoes').show();
    } else {
        $('#liTabRepresentacoes').hide();
    }
    tipoComercialChange();
}

//*******EVENTOS DE ENDEREÇO*******

function habilitaCamposEndereco() {
    _usuario.Bairro.enable(true);
    _usuario.Endereco.enable(true);
    _usuario.Localidade.enable(true);
}

function desabilitaCamposEndereco() {
    _usuario.Bairro.enable(false);
    _usuario.Endereco.enable(false);
    _usuario.Localidade.enable(false);
}

function consultaEnderecoCEP(e) {
    if ($("#" + _usuario.CEP.id).val().match(/\d/g) != null && $("#" + _usuario.CEP.id).val().match(/\d/g).join("").length == 8) {
        var data = { CEP: $("#" + _usuario.CEP.id).val() };
        executarReST("Localidade/BuscarEnderecoPorCEP", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null && arg.Data.DescricaoCidadeEstado != null && arg.Data.DescricaoCidadeEstado != "" && arg.Data.CodigoCidade > 0) {
                    _usuario.EnderecoDigitado.val(false);
                    _usuario.Bairro.val(arg.Data.Bairro);
                    _usuario.Endereco.val(arg.Data.Logradouro);
                    _usuario.Localidade.codEntity(arg.Data.CodigoCidade);
                    _usuario.Localidade.val(arg.Data.DescricaoCidadeEstado);
                    _usuario.Latitude.val(arg.Data.Latitude);
                    _usuario.Longitude.val(arg.Data.Longitude);
                    _usuario.TipoLogradouro.val(arg.Data.EnumTipoLogradouro);

                    $("#" + _usuario.NumeroEndereco.id).focus();
                    verificaDigitarEndereco();
                    verificaEnderecoUnico(arg.Data.DescricaoCidade);

                } else if (_usuario.EnderecoDigitado.val() == false) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Usuario.ConsultaDeCEP, Localization.Resources.Pessoas.Usuario.CEPInformadoNaoExisteNaBaseDeDados);
                    _usuario.EnderecoDigitado.val(true);
                    _usuario.Bairro.val("");
                    _usuario.Endereco.val("");
                    LimparCampoEntity(_usuario.Localidade);
                    verificaDigitarEndereco();
                } else if (_usuario.EnderecoDigitado.val() == true) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Usuario.ConsultaDeCEP, Localization.Resources.Pessoas.Usuario.CEPInformadoNaoExisteNaBaseDeDados);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function verificaNumero(e, sender) {
    if (_usuario.NumeroEndereco.val() == "S/N" || _usuario.NumeroEndereco.val() == "") {
        _usuario.SN_Numero.val(false);
        _usuario.NumeroEndereco.val("S/N");
        _usuario.NumeroEndereco.enable(false);
    } else {
        _usuario.SN_Numero.val(true);
        _usuario.NumeroEndereco.enable(true);
    }
}

function verificarSNNumero(e, sender) {
    if (_usuario.SN_Numero.val() == false) {
        _usuario.NumeroEndereco.val("S/N");
        _usuario.NumeroEndereco.enable(false);
    } else {
        if (_usuario.NumeroEndereco.val("S/N"))
            _usuario.NumeroEndereco.val("");
        _usuario.NumeroEndereco.enable(true);
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
        _usuario.EnderecoDigitado.val(false);
        _usuario.CEP.val(enderecoSelecionado.CEP);
        _usuario.Bairro.val(enderecoSelecionado.Bairro);
        _usuario.Endereco.val(enderecoSelecionado.Logradouro);
        _usuario.Localidade.codEntity(enderecoSelecionado.CodigoCidade);
        _usuario.Localidade.val(enderecoSelecionado.Descricao);
        _usuario.Latitude.val(enderecoSelecionado.Latitude);
        _usuario.Longitude.val(enderecoSelecionado.Longitude);
        if (enderecoSelecionado.TipoLogradouro != null && enderecoSelecionado.TipoLogradouro != "") {
            if (enderecoSelecionado.TipoLogradouro == "Rua")
                _usuario.TipoLogradouro.val(1);
            else if (enderecoSelecionado.TipoLogradouro == "Avenida")
                _usuario.TipoLogradouro.val(2);
            else if (enderecoSelecionado.TipoLogradouro == "Rodovia")
                _usuario.TipoLogradouro.val(3);
            else if (enderecoSelecionado.TipoLogradouro == "Estrada")
                _usuario.TipoLogradouro.val(4);
            else if (enderecoSelecionado.TipoLogradouro == "Praca")
                _usuario.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Praça")
                _usuario.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Travessa")
                _usuario.TipoLogradouro.val(6);
            else
                _usuario.TipoLogradouro.val(99);
        }
        $("#" + _usuario.Complemento.id).focus();
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
                        { data: "Descricao", title: Localization.Resources.Pessoas.Usuario.Cidade, width: "20%", className: "text-align-left" },
                        { data: "Logradouro", title: Localization.Resources.Pessoas.Usuario.Endereco, width: "20%", className: "text-align-left" },
                        { data: "Bairro", title: Localization.Resources.Pessoas.Usuario.Bairro, width: "15%" },
                        { data: "CEP", title: Localization.Resources.Pessoas.Usuario.CEP, width: "10%", className: "text-align-right" },
                        { data: "CodigoIBGE", title: Localization.Resources.Pessoas.Usuario.CodigoIBGE, width: "10%", className: "text-align-right" },
                        { data: "TipoLogradouro", title: Localization.Resources.Pessoas.Usuario.Tipo, width: "10%", className: "text-align-left" },
                        { data: "CodigoCidade", visible: false },
                        { data: "Latitude", visible: false },
                        { data: "Longitude", visible: false },
                        { data: "DescricaoCidade", visible: false }
                    ];
                    var gridEnderecos = new BasicDataTable(_pesquisaEndereco.Enderecos.idGrid, header, menuOpcoes);
                    gridEnderecos.CarregarGrid(endereco);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Usuario.PorFavorInformeAoMenosUmDosCamposParaRealizarConsulta);
}

function veriticaEnderecoDigitado(e, sender) {
    _usuario.EnderecoDigitado.val(_usuario.EnderecoDigitado.val());
}

function cancelarEnderecoClick(e, sender) {
    Global.fecharModal('divModalConsultaEndereco');
}

function verificaEnderecoUnico(nomeCidade) {
    if (_usuario.Endereco.val() == null || _usuario.Endereco.val() == "" || removeAcento(_usuario.Endereco.val().toUpperCase()) == removeAcento(nomeCidade.toUpperCase())) {
        _usuario.Bairro.enable(true);
        _usuario.Endereco.enable(true);
        $("#" + _usuario.Endereco.id).focus();
    } else {
        _usuario.Bairro.enable(false);
        _usuario.Endereco.enable(false);
    }
}

function carregarConteudosHTML(callback) {
    $.get("Content/Static/Localidade/Localidade.html?dyn=" + guid(), function (data) {F
        $("#ConsultaEndereco").html(data);
        _pesquisaEndereco = new PesquisaEndereco();
        KoBindings(_pesquisaEndereco, "knoutConsultaEndereco");
        new BuscarLocalidadesBrasil(_pesquisaEndereco.Localidade);
        $("#" + _pesquisaEndereco.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    });
}

function verificaDigitarEndereco() {
    if (_usuario.EnderecoDigitado.val()) {
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

function tipoPessoaChange() {
    var getType = typesKnockout.string;
    var text = _usuario.CPF.text;
    if (_usuario.TipoPessoa.val() == EnumTipoPessoa.Fisica) {
        getType = typesKnockout.cpf;
        text = _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil ? Localization.Resources.Pessoas.Usuario.CPF.getRequiredFieldDescription() : Localization.Resources.Pessoas.Usuario.CPF.getFieldDescription();
        _usuario.CPF.required = _CONFIGURACAO_TMS.Pais == EnumPaises.Brasil;
    } else if (_usuario.TipoPessoa.val() == EnumTipoPessoa.Juridica) {
        getType = typesKnockout.cnpj;
        text = Localization.Resources.Pessoas.Usuario.CNPJ.getRequiredFieldDescription();
    }
    SetarGetType(_usuario.CPF, getType)
    _usuario.CPF.text(text);
}

function tipoComercialChange() {
    var mostrarNotificarOcorrenciaEntrega = $("#" + _usuario.TipoComercialCheck.id).prop("checked") && _usuario.TipoComercial.val() == EnumTipoComercial.Vendedor;
    _usuario.NotificarOcorrenciaEntrega.visible(mostrarNotificarOcorrenciaEntrega);
    if (!$("#" + _usuario.TipoComercialCheck.id).prop("checked")) {
        LimparCampo(_usuario.TipoComercial);
    }
}

function VisibilidadePermissoes() {
    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Usuarios_PermitirEditarAsPermissoesDeAcesso, _PermissoesPersonalizadas) && !_CONFIGURACAO_TMS.UsuarioAdministrador) {
        $('#divTreeView').css("display", "none");
        _usuario.PerfilAcesso.visible(false);
    }

    if (_usuario.UsuarioAdministrador.val() && (_usuario.PerfilAcesso.codEntity() == 0 || _usuario.PerfilAcesso.val() == ''))
        $('#divTreeView').css("display", "none");

    _usuario.VisualizarControleDashRegiaoOperador.val(_VisualizarDashRegiao);
}

