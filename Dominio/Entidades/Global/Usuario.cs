using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO", EntityName = "Usuario", DynamicUpdate = true, Name = "Dominio.Entidades.Usuario", NameType = typeof(Usuario))]
    public class Usuario : EntidadeBase, IEquatable<Usuario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// F - Física 
        /// J - Jurídica
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "FUN_FISJUR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "FUN_CPF", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "FUN_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_CODIGO_INTEGRACAO_CONTABILIDADE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Session", Column = "FUN_SESSION", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Session { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Turno", Column = "TUR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Filiais.Turno Turno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "FUN_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Apelido", Column = "FUN_APELIDO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Apelido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataNascimento", Column = "FUN_DATANASC", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNascimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeLiberacaoSeguradora", Column = "FUN_DATA_VALIDADE_LIBERACAO_SEGURADORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeLiberacaoSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAdmissao", Column = "FUN_DATAADMISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAdmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimPeriodoExperiencia", Column = "FUN_DATA_FIM_PERIODO_EXPERIENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimPeriodoExperiencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RG", Column = "FUN_RG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string RG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrgaoEmissorRG", Column = "FUN_ORGAO_EMISSOR_RG", TypeType = typeof(ObjetosDeValor.Enumerador.OrgaoEmissorRG), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.OrgaoEmissorRG? OrgaoEmissorRG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_RG", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoRG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_DATA_EMISSAO_RG", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoRG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sexo", Column = "FUN_SEXO", TypeType = typeof(ObjetosDeValor.Enumerador.Sexo), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.Sexo? Sexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "FUN_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_CELULAR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Celular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Salario", Column = "FUN_SALARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Salario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoCivil", Column = "FUN_ESTADOCIVIL", TypeType = typeof(EstadoCivil), NotNull = false)]
        public virtual EstadoCivil? EstadoCivil { get; set; }

        /// <summary>
        /// M = Motorista
        /// U = Usuario
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "FUN_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Login", Column = "FUN_LOGIN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAlteracaoSenhaObrigatoria", Column = "FUN_DATA_ULTIMA_ALTERACAO_SENHA_OBRIGATORIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAlteracaoSenhaObrigatoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "FUN_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaCriptografada", Column = "FUN_SENHA_CRIPTOGRAFADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SenhaCriptografada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracaoEmbarcador", Column = "FUN_PENDENTE_INTEGRACAO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAcessoBloqueado", Column = "FUN_USUARIO_ACESSO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioAcessoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraBloqueio", Column = "FUN_DATA_HORA_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativasInvalidas", Column = "FUN_TENTATIVAS_INVALIDAS_ACESSO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasInvalidas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "FUN_ENDERECO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "FUN_BAIRRO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "FUN_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "FUN_COMPLEMENTO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSanguineo", Column = "FUN_TIPOSANGUINEO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string TipoSanguineo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroHabilitacao", Column = "FUN_NUMHABILITACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RenachHabilitacao", Column = "FUN_RENACH", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string RenachHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_DATA_PRIMEIRA_HABILITACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrimeiraHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHabilitacao", Column = "FUN_DATAHABILITACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoHabilitacao", Column = "FUN_VECTOHABILITACAO", TypeType = typeof(DateTime), Length = 80, NotNull = false)]
        public virtual DateTime? DataVencimentoHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAlertaVencimentoCnh", Column = "FUN_DATA_ULTIMO_ALERTA_VENCIMENTO_CNH", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAlertaVencimentoCnh { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_EMISSAO_CNH", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado UFEmissaoCNH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Categoria", Column = "FUN_CATEGORIA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "FUN_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Moop", Column = "FUN_MOOP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Moop { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PIS", Column = "FUN_PIS", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string PIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FUN_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueado", Column = "FUN_BLOQUEADO", TypeType = typeof(bool), Length = 1, NotNull = false)]
        public virtual bool Bloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAdministrador", Column = "FUN_USUARIO_ADMINISTRADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioAdministrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioMultisoftware", Column = "FUN_USUARIO_MULTISOFTWARE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAtendimento", Column = "FUN_USUARIO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioCallCenter", Column = "FUN_USUARIO_CALLCENTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioCallCenter { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcesso", Column = "FUN_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAcesso), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAcesso TipoAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "FUN_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRemocaoVinculo", Column = "FUN_DATA_REMOCAO_VINCULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRemocaoVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_ACERTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoAcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_MODULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> ModulosLiberados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FormulariosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_FORMULARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FuncionarioFormulario", Column = "FMO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> FormulariosLiberados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Series", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_SERIE_FUNC")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaSerie", Column = "ESE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.EmpresaSerie> Series { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Empresas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual IList<Empresa> Empresas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartao", Column = "FUN_NUMERO_CARTAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroCartao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCartao", Column = "FUN_TIPO_CARTAO", TypeType = typeof(TipoPessoaCartao), NotNull = false)]
        public virtual TipoPessoaCartao? TipoCartao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEndereco", Column = "FUN_NUMERO_ENDERECO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLogradouro", Column = "FUN_TIPO_LOGRADOURO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro? TipoLogradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "FUN_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "FUN_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoDigitado", Column = "FUN_ENDERECO_DIGITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnderecoDigitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmail", Column = "FUN_EMAIL_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmail), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmail TipoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEndereco", Column = "FUN_ENDERECO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco TipoEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlterarSenhaAcesso", Column = "FUN_ALTERAR_SENHA_ACESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarSenhaAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAuditar", Column = "FUN_PERMITE_AUDITAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAuditar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PerfilPermissao", Column = "PP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PerfilPermissao PerfilPermissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PerfilAcesso", Column = "PAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Usuarios.PerfilAcesso PerfilAcesso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_FORNECEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssociarUsuarioCliente", Column = "FUN_ASSOCIAR_USUARIO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AssociarUsuarioCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CLIENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Representacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_REPRESENTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Representacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMotorista", Column = "FUN_TIPO_MOTORISTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista TipoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJEmbarcador", Column = "FUN_CNPJ_EMBARCADOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGeraComissaoAcerto", Column = "FUN_NAO_GERA_COMISSAO_ACERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGeraComissaoAcerto { get; set; }

        /// <summary>
        /// Cuidado ao usar esse atributo. Geralmente ele deve ser usado em conjunto com a Configuração Global HabilitarFichaMotoristaTodos, que foi encapsulado no Serviço do Motorista
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarFichaMotorista", Column = "FUN_ATIVAR_FICHA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarFichaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "FUN_BANCO_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiliacaoMotoristaMae", Column = "FUN_FILIACAO_MOTORISTA_MAE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FiliacaoMotoristaMae { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiliacaoMotoristaPai", Column = "FUN_FILIACAO_MOTORISTA_PAI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FiliacaoMotoristaPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "FUN_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "FUN_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "FUN_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_NOTIFICADO_PELA_EXPEDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificadoExpedicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_ENVIAR_NOTIFICACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNotificacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarGraficosIniciais", Column = "FUN_VISUALIZAR_GRAFICOS_INICIAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarGraficosIniciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_CODIGO_MOBILE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_NAO_BLOQUEAR_ACESSO_SIMULTANEO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBloquearAcessoSimultaneo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_ULTIMO_ACESSO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimoAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Callcenter", Column = "FUN_CALLCENTER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Callcenter { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoBloqueio", Column = "FUN_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSuspensaoInicio", Column = "FUN_SUSPENSAO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSuspensaoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSuspensaoFim", Column = "FUN_SUSPENSAO_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSuspensaoFim { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDemissao", Column = "FUN_DATA_DEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDemissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTPS", Column = "FUN_NUMERO_CTPS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCTPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCTPS", Column = "FUN_SERIE_CTPS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SerieCTPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoConta", Column = "FUN_OBSERVACAO_CONTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaComissao", Column = "FUN_DIA_COMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaComissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_COMISSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCaixa", Column = "FUN_SITUACAO_CAIXA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa SituacaoCaixa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CaixaFuncionario", Column = "CAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario CaixaFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoMoop", Column = "FUN_VECTO_MOOP", TypeType = typeof(DateTime), Length = 80, NotNull = false)]
        public virtual DateTime? DataVencimentoMoop { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoMotorista", Column = "PMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista ProgramacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoColaborador", Column = "FUN_SITUACAO_COLABORADOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador? SituacaoColaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PISAdministrativo", Column = "FUN_PIS_ADMINISTRATIVO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PISAdministrativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cargo", Column = "FUN_CARGO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Cargo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CBO", Column = "FUN_CBO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CBO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMatricula", Column = "FUN_NUMERO_MATRICULA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroMatricula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDiaria", Column = "FUN_SALDO_DIARIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal SaldoDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoAdiantamento", Column = "FUN_SALDO_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal SaldoAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicialAcesso", Column = "FUN_HORA_INICIAL_ACESSO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraInicialAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinalAcesso", Column = "FUN_HORA_FINAL_ACESSO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraFinalAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAdministradorMobile", Column = "FUN_USUARIO_ADMINISTRADOR_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UsuarioAdministradorMobile { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PerfilAcessoMobile", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Usuarios.PerfilAcessoMobile PerfilAcessoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirUsuarioAprovacao", Column = "FUN_EXIBIR_USUARIO_APROVACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ExibirUsuarioAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroProntuario", Column = "FUN_NUMERO_PRONTUARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroProntuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FUN_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_NASCIMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeNascimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_TITULO_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TituloEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_ZONA_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ZonaEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_SECAO_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SecaoEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorRaca", Column = "FUN_COR_RACA", TypeType = typeof(CorRaca), NotNull = false)]
        public virtual CorRaca? CorRaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Escolaridade", Column = "FUN_ESCOLARIDADE", TypeType = typeof(Escolaridade), NotNull = false)]
        public virtual Escolaridade? Escolaridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComercial", Column = "FUN_TIPO_COMERCIAL", TypeType = typeof(TipoComercial), NotNull = false)]
        public virtual TipoComercial? TipoComercial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_CTPS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoCTPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataExpedicaoCTPS", Column = "FUN_DATA_EXPEDICAO_CTPS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataExpedicaoCTPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_DIAS_TRABALHADO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasTrabalhado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_DIAS_FOLGA_RETIRADO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasFolgaRetirado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_VERSAO_APP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string VersaoAPP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_SUPERVISOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Supervisor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_GERENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Gerente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITE_SALVAR_NOVO_RELATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSalvarNovoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITE_TORNAR_RELATORIO_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteTornarRelatorioPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITE_SALVAR_CONFIGURACOES_RELATORIOS_PARA_TODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSalvarConfiguracoesRelatoriosParaTodos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarOcorrenciaEntrega", Column = "FUN_NOTIFICAR_OCORRENCIA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarOcorrenciaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimitarOperacaoPorEmpresa", Column = "FUN_LIMITAR_OPERACAO_POR_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LimitarOperacaoPorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRegistroHabilitacao", Column = "FUN_NUMERO_REGISTRO_HABILITACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroRegistroHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberadosMobile", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_MODULO_MOBILE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> ModulosLiberadosMobile { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaIntegracao", Column = "INT_CODIGO")]
        public virtual IList<Embarcador.Transportadores.MotoristaIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Licencas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaLicenca", Column = "MLI_CODIGO")]
        public virtual IList<Embarcador.Transportadores.MotoristaLicenca> Licencas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LiberacoesGR", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_LIBERACAO_GR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaLiberacaoGR", Column = "MLG_CODIGO")]
        public virtual IList<Embarcador.Transportadores.MotoristaLiberacaoGR> LiberacoesGR { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DadosBancarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTORISTA_DADO_BANCARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotoristaDadoBancario", Column = "MDB_CODIGO")]
        public virtual IList<Embarcador.Transportadores.MotoristaDadoBancario> DadosBancarios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contatos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FuncionarioContato", Column = "FCO_CODIGO")]
        public virtual IList<Embarcador.Usuarios.FuncionarioContato> Contatos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "SituacoesColaborador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLABORADOR_LANCAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ColaboradorLancamento", Column = "CLS_CODIGO")]
        public virtual IList<Embarcador.Usuarios.Colaborador.ColaboradorLancamento> SituacoesColaborador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFechamentoAcerto", Column = "FUN_DATA_FECHAMENTO_ACERTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFechamentoAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_POSSUI_CONTROLE_DISPONIBILIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiControleDisponibilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_TOKEN_CONFIRMACAO_EMAIL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenConfirmacaoConta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CentrosResultado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroResultado", Column = "CRE_CODIGO")]
        public virtual ICollection<Embarcador.Financeiro.CentroResultado> CentrosResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cargo", Column = "CRG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.Cargo CargoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_AREA_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente AreaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aposentadoria", Column = "FUN_APOSENTADORIA", TypeType = typeof(Aposentadoria), NotNull = false)]
        public virtual Aposentadoria? Aposentadoria { get; set; }

        /*
         * Se ativa, permite o usuário assinar a Anuência (gerada pela Conciliacação de Transportador. Ver ConciliacaoTransportadorController).
         */
        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAssinarAnuencia", Column = "FUN_PERMITE_ASSINAR_ANUENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAssinarAnuencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAutenticacaoUsuario", Column = "FUN_FORMA_AUTENTICACAO", TypeType = typeof(FormaAutenticacaoUsuario), NotNull = false)]
        public virtual FormaAutenticacaoUsuario? FormaAutenticacaoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAssumirOcorrencia", Column = "FUN_PERMITE_ASSUMIR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAssumirOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaGR", Column = "FUN_SENHA_GR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_MOTORISTA_ESTRANGEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaEstrangeiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_VISUALIZAR_TITULOS_PAGAMENTO_SALARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarTitulosPagamentoSalario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeGR", Column = "FUN_DATA_VALIDADE_GR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_ORDENAR_CARGA_MOBILE_CRESCENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdenarCargasMobileCrescente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cargo", Column = "FUN_CODIGO_CARGO_SETOR_TURNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.Cargo CargoSetorTurno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCustoViagem", Column = "CCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem CentroDeCustoSetorTurno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelEscalationList", Column = "FUN_NIVEL_ESCALATION_LIST", TypeType = typeof(EscalationList), NotNull = false)]
        public virtual EscalationList? NivelEscalationList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITE_INSERIR_DICAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInserirDicas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_CNH", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeMunicipioEstadoCNH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoSegurancaCNH", Column = "FUN_CODIGO_SEGURANCA_CNH", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoSegurancaCNH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITIR_APROVAR_NAO_CONFORMIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAprovarNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_GESTOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Gestor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_NUMEROCARTAOVALEPEDAGIO", TypeType = typeof(string), NotNull = false)]
        public virtual string NumeroCartaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_AJUDANTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Ajudante { get; set; }

        [Obsolete("Migrado para uma lista.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CLIENTE_SETOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteSetor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesSetor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_CLIENTE_SETOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> ClientesSetor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesProvedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FUNCIONARIO_CLIENTE_PROVEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FUN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> ClientesProvedores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_REGIAO_ACESSO_DASH_OPERADOR_NORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegiaoAcessoDashOperadorNorte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_REGIAO_ACESSO_DASH_OPERADOR_NORDESTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegiaoAcessoDashOperadorNordeste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_REGIAO_ACESSO_DASH_OPERADOR_SUL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegiaoAcessoDashOperadorSul { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_REGIAO_ACESSO_DASH_OPERADOR_SUDESTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegiaoAcessoDashOperadorSudeste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_REGIAO_ACESSO_DASH_OPERADOR_CENTRO_OESTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegiaoAcessoDashOperadorCentroOeste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_NOTIFICAR_ETAPAS_BIDDING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarEtapasBidding { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_PERMITIR_ASSUMIR_ATENDIMENTO_MANEIRA_SOBREPOSTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAssumirAtendimentoManeiraSobreposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_USUARIO_UTILIZA_SEGREGACAO_POR_PROVEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsuarioUtilizaSegregacaoPorProvedor { get; set; }

        [Obsolete("Campo não vai ser mais utilizado na Entidade Usuario e sim na Entidade Setor")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_NOTIFICAR_CENARIO_POS_ENTREGA_IMPROCEDENTE_GESTAO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCenarioPosEntregaImprocedenteGestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoResidencia", Column = "FUN_TIPO_RESIDENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoResidencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoResidencia? TipoResidencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FUN_CONTROLE_DUPLICIDADE_UK", TypeType = typeof(int), NotNull = false)]
        public virtual int ControleDuplicidadeUK { get; set; }


        #region Propriedades Virtuais

        public virtual Dominio.ObjetosDeValor.UsuarioInterno UsuarioInterno { get; set; }

        public virtual string DescricaoUsuarioLogado
        {
            get
            {
                if (UsuarioInterno != null)
                    return $"{UsuarioInterno.Nome} ({Nome})";
                else
                    return Nome;
            }
        }

        public virtual string DescricaoUsuarioInterno
        {
            get
            {
                if (UsuarioInterno == null)
                    return string.Empty;

                return $"{UsuarioInterno.Nome} ({UsuarioInterno.Email})";
            }
        }

        public virtual string Descricao
        {
            get
            {
                if (this.TipoAcesso == Enumeradores.TipoAcesso.Embarcador)
                    return $"{Nome}";
                else
                    return $"{Nome} ({CPF_Formatado})";
            }
        }

        public virtual string DescricaoTelefone
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Telefone))
                    return this.Nome + " (" + this.Telefone + ")";
                else if (!string.IsNullOrWhiteSpace(this.Celular))
                    return this.Nome + " (" + this.Celular + ")";
                else
                    return this.Nome;
            }
        }

        public virtual string DescricaoTelefoneEmail
        {
            get
            {
                StringBuilder descricao = new StringBuilder(this.Nome);

                if (!string.IsNullOrWhiteSpace(this.Telefone))
                    descricao.Append($" ({this.Telefone})");

                if (!string.IsNullOrWhiteSpace(this.Email))
                    descricao.Append($" - {this.Email}");

                return descricao.ToString();
            }
        }

        public virtual string CPF_Formatado
        {
            get
            {
                return CPF_CNPJ_Formatado;
            }
        }

        public virtual bool UsuarioDaMultisoftware
        {
            get
            {
                return this.UsuarioMultisoftware || this.UsuarioCallCenter || this.UsuarioAtendimento;
            }
        }


        public virtual string CPF_CNPJ_Formatado
        {
            get
            {
                if (MotoristaEstrangeiro)
                    return CPF;

                string cpf = Utilidades.String.OnlyNumbers(CPF);

                long.TryParse(cpf, out long cpfCnpj);

                if (cpf.Length > 11)
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
            }
        }

        public virtual string CPF_CNPJ_Formatado_Com_Asterisco
        {
            get
            {
                if (MotoristaEstrangeiro)
                    return CPF;

                string cpf = CPF_CNPJ_Formatado;

                return cpf.Remove(4, 7).Insert(4, "***.***");
            }
        }

        public virtual string Telefone_Formatado
        {
            get { return Telefone.ObterTelefoneFormatado(); }
        }

        public virtual string Celular_Formatado
        {
            get { return Celular.ObterTelefoneFormatado(); }
        }

        public virtual string DescricaoTipoContaBanco
        {
            get { return TipoContaBanco?.ObterDescricao() ?? ""; }
        }

        public virtual string DescricaoStatus
        {
            get { return Status == "A" ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo; }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case "U":
                        return "Usuário";
                    case "M":
                        return "Motorista";
                    default:
                        return "";
                }
            }
        }

        public virtual bool PermiteVisualizarTitulosPagamentoSalario
        {
            get { return UsuarioAdministrador ? true : ((PerfilAcesso?.VisualizarTitulosPagamentoSalario ?? false) || VisualizarTitulosPagamentoSalario); }
        }

        public virtual bool Equals(Usuario other)
        {
            return (this.Codigo == other.Codigo);
        }

        #endregion
    }
}
