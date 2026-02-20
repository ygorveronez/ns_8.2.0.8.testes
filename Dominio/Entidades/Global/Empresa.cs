using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA", DynamicUpdate = true, EntityName = "Empresa", Name = "Dominio.Entidades.Empresa", NameType = typeof(Empresa))]
    public class Empresa : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public Empresa()
        {
            this.Filiais = new List<Dominio.Entidades.Empresa>();
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "EMP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAlfanumerico", Column = "EMP_CODIGO_ALFANUMERICO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAlfanumerico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "save-update")]
        public virtual ConfiguracaoEmpresa Configuracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "EMP_CONTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Contador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.PagamentoMotorista.PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "EMP_CNPJ", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoEstadual", Column = "EMP_INSCRICAO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string InscricaoEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoMunicipal", Column = "EMP_INSCRICAO_MUNICIPAL", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string InscricaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RazaoSocial", Column = "EMP_RAZAO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string RazaoSocial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "EMP_FANTASIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "EMP_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fax", Column = "EMP_FAX", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Fax { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contato", Column = "EMP_CONTATO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelefoneContato", Column = "EMP_FONECONTATO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TelefoneContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "EMP_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailEnvioCanhoto", Column = "EMP_EMAIL_ENVIO_CANHOTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailEnvioCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailEnvioCTeRejeitado", Column = "EMP_EMAIL_ENVIO_CTE_REJEITADO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailEnvioCTeRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "EMP_ENDERECO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "EMP_COMPLEMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "EMP_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "EMP_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        /// <summary>
        /// F - Física
        /// J - Jurídica
        /// E - Exterior
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "EMP_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "EMP_RESPONSAVEL", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IP", Column = "EMP_IP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "EMP_BAIRRO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "EMP_DATAATU", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Name = "UsuarioAtualizacao", Column = "EMP_USUARIO_ATUALIZACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioAtividade", Column = "EMP_DATA_INICIO_ATIVIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioAtividade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "EMP_DATACADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Name = "UsuarioCadastro", Column = "EMP_USUARIO_CADASTRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PontuacaoFixa", Column = "EMP_PONTUACAO_FIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int PontuacaoFixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "EMP_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Versao", Column = "EMP_VERSAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Versao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaANTT", Column = "EMP_CATEGORIAANTT", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string CategoriaANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegistroANTT", Column = "EMP_REGISTROANTT", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string RegistroANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoANTT", Column = "EMP_DATAEMISSAOANTT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeANTT", Column = "EMP_DATAVALIDADEANTT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "EMP_TIPO_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimeEspecial", Column = "EMP_REGIME_ESPECIAL", TypeType = typeof(Dominio.Enumeradores.RegimeEspecialEmpresa), NotNull = false)]
        public virtual Dominio.Enumeradores.RegimeEspecialEmpresa RegimeEspecial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IMO", Column = "EMP_IMO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeIMO", Column = "EMP_DATA_VALIDADE_IMO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeIMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCredito", Column = "EMP_PERCENTUAL_CREDITO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInclusaoPedagioBaseCalculoICMS", Column = "EMP_TIPO_INCLUSAO_PEDAGIO_BASE_CALCULO_ICMS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInclusaoPedagioBaseCalculoICMS TipoInclusaoPedagioBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoNumeroNFe", Column = "EMP_ULTIMO_NUMERO_NFE", TypeType = typeof(int), NotNull = false)]
        public virtual int UltimoNumeroNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoNumeroNFCe", Column = "EMP_ULTIMO_NUMERO_NFCE", TypeType = typeof(int), NotNull = false)]
        public virtual int UltimoNumeroNFCe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasRotatividadePallets", Column = "EMP_DIAS_ROTATIVIDADE_PALLETS", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasRotatividadePallets { get; set; }

        [Obsolete("Utilizar a propriedade CodigosComercialDistribuidor")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoComercialDistribuidor", Column = "EMP_CODIGO_COMERCIAL_DISTRIBUIDOR", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string CodigoComercialDistribuidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDelayHorasParaIniciarEmissao", Column = "EMP_TEMPO_DELAY_HORAS_INICIAR_EMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDelayHorasParaIniciarEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraCorteCarregamento", Column = "EMP_HORA_CORTE_CARREGAMENTO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraCorteCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TAF", Column = "EMP_TAF", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string TAF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NroRegEstadual", Column = "EMP_NUMERO_REG_ESTADUAL", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NroRegEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CodigosComercialDistribuidor", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_CODIGOS_COMERCIAL_DISTRIBUIDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "EMP_CODIGO_COMERCIAL_DISTRIBUIDOR", TypeType = typeof(string), Length = 25, NotNull = true)]
        public virtual ICollection<string> CodigosComercialDistribuidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialCertificado", Column = "EMP_DATA_CERTIFICADOINI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalCertificado", Column = "EMP_DATA_CERTIFICADOFINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAlertaVencimentoAntt", Column = "EMP_DATA_ULTIMO_ALERTA_VENCIMENTO_ANTT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAlertaVencimentoAntt { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAlertaVencimentoCertificado", Column = "EMP_DATA_ULTIMO_ALERTA_VENCIMENTO_CERTIFICADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAlertaVencimentoCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaCertificado", Column = "EMP_SENHA_CERTIFICADO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCertificado", Column = "EMP_SERIECERTIFICADO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SerieCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCertificado", Column = "EMP_NOME_CERTIFICADO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCertificadoKeyVault", Column = "EMP_NOME_CERTIFICADO_KEY_VAULT", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCertificadoKeyVault { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEmail", Column = "EMP_EMAIL_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContador", Column = "EMP_EMAILCONTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEmailContador", Column = "EMP_EMAILCONTADOR_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEmailContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailAdministrativo", Column = "EMP_EMAILADM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailAdministrativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEmailAdministrativo", Column = "EMP_EMAILADM_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEmailAdministrativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoLogoDacte", Column = "EMP_CAMINHOLOGO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoLogoDacte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoLogoSistema", Column = "EMP_CAMINHOLOGOSISTEMA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoLogoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNAE", Column = "EMP_CNAE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CNAE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Suframa", Column = "EMP_SUFRAMA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Suframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Setor", Column = "EMP_SETOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Setor { get; set; }

        [Obsolete("Migrado para a tabela TransportadorInscricaoST")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Inscricao_ST", Column = "EMP_INSCRICAO_ST", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Inscricao_ST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "EmpresaAdministradora", Column = "EMP_EMPRESA_ADMIN", Class = "Empresa", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaAdministradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "EmpresaPai", Column = "EMP_EMPRESA", Class = "Empresa", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaPai { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeContador", Column = "EMP_NOMECONTADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TelefoneContador", Column = "EMP_FONECONTADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TelefoneContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CRCContador", Column = "EMP_CRCCONTADOR", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CRCContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXML", Column = "EMP_EMAIL_XML", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmailXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXMLHost", Column = "EMP_EMAIL_XML_HOST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmailXMLHost { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXMLPorta", Column = "EMP_EMAIL_XML_PORTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string EmailXMLPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXMLAutentica", Column = "EMP_EMAIL_XML_AUTENTICA", TypeType = typeof(int), NotNull = false)]
        public virtual int EmailXMLAutentica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXMLUsuario", Column = "EMP_EMAIL_XML_USUARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmailXMLUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailXMLSenha", Column = "EMP_EMAIL_XML_SENHA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EmailXMLSenha { get; set; }

        /// <summary>
        /// N - Não Contatou
        /// P - Pendente
        /// S - Sistema Web
        /// C - Call Center
        /// M - Não Emitente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEmissao", Column = "EMP_STATUS_EMISSAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OptanteSimplesNacional", Column = "EMP_OPTANTE_SIMPLES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OptanteSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeEtiquetagem", Column = "EMP_EXIGE_ETIQUETAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeEtiquetagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OptanteSimplesNacionalComExcessoReceitaBruta", Column = "EMP_OPTANTE_SIMPLES_EXCESSO_RECEITA_BRUTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OptanteSimplesNacionalComExcessoReceitaBruta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarEmissaoSemAverbacao", Column = "EMP_LIBERAR_EMISSAO_SEM_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarEmissaoSemAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoDocumentosForaDoSistema", Column = "EMP_EMISSAO_DOCUMENTOS_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoDocumentosForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoMDFeForaDoSistema", Column = "EMP_EMISSAO_MDFE_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoMDFeForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoNFSeForaDoSistema", Column = "EMP_EMISSAO_NFSE_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmissaoNFSeForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmissaoCRTForaDoSistema", Column = "EMP_EMISSAO_CRT_FORA_DO_SISTEMA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmissaoCRTForaDoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoIncrementarNumeroLoteRPSAutomaticamente", Column = "EMP_NAO_INCREMENTAR_NUMERO_LOTE_RPS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncrementarNumeroLoteRPSAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_EMITE_NFSE_OCORRENCIA_FORA_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteNFSeOcorrenciaForaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado", Column = "EMP_PERCENTUAL_DE_TOLERANCIA_DIFERENCA_ENTRE_CTE_EMITIDO_E_ESPERADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDeToleranciaDiferencaEntreCTeEmitidoEEsperado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarComGerenciadoraDeRisco", Column = "EMP_INTEGRAR_COM_GERENCIADORA_RISCO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarComGerenciadoraDeRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_USAR_TIPO_OPERACAO_APOLICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarTipoOperacaoApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteMDFe20IntraEstadual", Column = "EMP_EMITE_CTE_20_INTRAESTADUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteMDFe20IntraEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteEmitirSubcontratacao", Column = "EMP_PERMITE_EMITIR_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmitirSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora", Column = "EMP_USAR_COMO_FILIAL_EMISSORA_PADRO_EM_REDESPACHOS_INICIOADOS_NO_ESTADO_DA_TRANSPORTADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarCondicao", Column = "EMP_ATIVAR_CONDICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarCondicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "EMP_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "EMP_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FusoHorario", Column = "EMP_FUSO_HORARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FusoHorario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "PlanoEmissaoCTe", Column = "PEC_CODIGO", Class = "PlanoEmissaoCTe", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoEmissaoCTe PlanoEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "EmpresaCobradora", Column = "EMP_EMPRESA_COBRADORA", Class = "Empresa", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaCobradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimentoFatura", Column = "EMP_DIA_VENCIMENTO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaVencimentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Skype", Column = "EMP_SKYPE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Skype { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "EMP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLSistema", Column = "EMP_URL_SISTEMA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSistema", Column = "EMP_TIPO_SISTEMA", TypeType = typeof(Dominio.Enumeradores.TipoSistema), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSistema TipoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCTeFora", Column = "EMP_SERIE_CTE_FORA", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieCTeFora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCTeDentro", Column = "EMP_SERIE_CTE_DENTRO", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieCTeDentro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieMDFe", Column = "EMP_SERIE_MDFE", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrimeiroNumeroMDFe", Column = "EMP_PRIMEIRO_NUMERO_MDFE", TypeType = typeof(int), NotNull = false)]
        public virtual int PrimeiroNumeroMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusFinanceiro", Column = "EMP_STATUS_FINANCEIRO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string StatusFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdTokenNFCe", Column = "EMP_TOKEN_NFCE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdTokenNFCe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdCSCNFCe", Column = "EMP_CSC_NFCE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdCSCNFCe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransportador", Column = "EMP_TIPO_TRANSPORTADOR", TypeType = typeof(Dominio.Enumeradores.TipoTransportador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTransportador TipoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSSimples", Column = "EMP_ALIQUOTA_ICMS_SIMPLES", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal AliquotaICMSSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSNegociado", Column = "EMP_ALIQUOTA_ICMS_NEGOCIADO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal AliquotaICMSNegociado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeIDENotas", Column = "EMP_NFSE_ID_ENOTAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NFSeIDENotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CertificadoA3", Column = "EMP_CERTIFICADOA3", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CertificadoA3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssinaturaCapicom", Column = "EMP_ASSINATURA_CAPICOM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AssinaturaCapicom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CasasQuantidadeProdutoNFe", Column = "EMP_CASAS_QUANTIDADE_PRODUTO_NFE", TypeType = typeof(int), NotNull = false)]
        public virtual int CasasQuantidadeProdutoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CasasValorProdutoNFe", Column = "EMP_CASAS_VALOR_PRODUTO_NFE", TypeType = typeof(int), NotNull = false)]
        public virtual int CasasValorProdutoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalculaIBPTNFe", Column = "EMP_CALCULA_IBPT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalculaIBPTNFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "COTM", Column = "EMP_NUMERO_COTM", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string COTM { get; set; }

        /// <summary>
        /// INDICA SE O TRANSPORTADOR É DO EMBARCADOR (PLACA CINZA), NESSE CASO PODE SER USADO POR EXEMPLO PARA NÃO GERAR CT-E E GERAR APENAS O MDF-E POR NOTA.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmpresaPropria", Column = "EMP_EMPRESA_PROPRIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmpresaPropria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_CARGA_PROPRIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalCargaPropria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontuacao", Column = "EMP_PONTUACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int Pontuacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TransportadorLayoutsEDI", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSPORTADOR_LAYOUT_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TransportadorLayoutEDI", Column = "TLY_CODIGO")]
        public virtual IList<Embarcador.Transportadores.TransportadorLayoutEDI> TransportadorLayoutsEDI { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmpresaIntelipostIntegracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_INTELIPOST_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaIntelipostIntegracao", Column = "EII_CODIGO")]
        public virtual IList<Dominio.Entidades.EmpresaIntelipostIntegracao> EmpresaIntelipostIntegracao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmpresaIntelipostTipoOcorrencia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_INTELIPOST_TIPO_OCORRENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaIntelipostTipoOcorrencia", Column = "EIO_CODIGO")]
        public virtual IList<Dominio.Entidades.EmpresaIntelipostTipoOcorrencia> EmpresaIntelipostTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LayoutsEDI", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_LAYOUT_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LayoutEDI", Column = "LAY_CODIGO")]
        public virtual ICollection<Dominio.Entidades.LayoutEDI> LayoutsEDI { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> Filiais { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Matriz", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual IList<Dominio.Entidades.Empresa> Matriz { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracoesTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_CONFIGURACAO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoTipoOperacao", Column = "CTP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao> ConfiguracoesTipoOperacao { get; set; }


        /// <summary>
        /// São as filiis do Multiembarcador que o transportador pode transportar.
        /// </summary>
        [NHibernate.Mapping.Attributes.Set(0, Name = "FiliaisEmbarcadorHabilitado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_FILIAL_EMBARCADOR_HABILITADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> FiliaisEmbarcadorHabilitado { get; set; }

        //[NHibernate.Mapping.Attributes.OneToOne(0, Name = "Canhoto", Class = "Canhoto", PropertyRef = "XMLNotaFiscal", Access = "property")]
        //public virtual Dominio.Entidades.Embarcador.Canhotos.Canhoto Canhoto { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "CentrosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADOR")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamento", Column = "CEC_CODIGO")]
        //public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> CentrosCarregamento { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "CentrosCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_TRANSPORTADORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamentoTransportador", Column = "CTR_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador> CentrosCarregamento { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_EMPRESA_ADMINISTRADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorAdministrador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModulosLiberados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_MODULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MOD_CODIGO_MODULO", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> ModulosLiberados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CondicoesPagamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONDICAO_PAGAMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CondicaoPagamentoTransportador", Column = "CPT_CODIGO")]
        public virtual ICollection<Embarcador.Transportadores.CondicaoPagamentoTransportador> CondicoesPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TransportadorConfiguracoesNFSe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_CONFIGURACAO_NFSE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TransportadorConfiguracaoNFSe", Column = "ECN_CODIGO")]
        public virtual ICollection<Embarcador.Transportadores.TransportadorConfiguracaoNFSe> TransportadorConfiguracoesNFSe { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Column = "PAT_CODIGO", Class = "PerfilAcessoTransportador", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Transportadores.PerfilAcessoTransportador PerfilAcessoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogStatus", Column = "EMP_LOG_STATUS", Type = "StringClob", NotNull = false)]
        public virtual string LogStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogAceiteTermosUso", Column = "EMP_ACEITOU_TERMOS_USO_LOG", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string LogAceiteTermosUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AceitouTermosUso", Column = "EMP_ACEITOU_TERMOS_USO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AceitouTermosUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAceiteTermosUso", Column = "EMP_DATA_ACEITE_TERMOS_USO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAceiteTermosUso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteEmissaoDocumentosDestinados", Column = "EMP_PERMITE_EMISSAO_DOCUMENTOS_DESTINADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteEmissaoDocumentosDestinados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CadastrarProdutoAutomaticamenteDocumentoEntrada", Column = "EMP_CADASTRAR_PRODUTO_AUTOMATICAMENTE_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CadastrarProdutoAutomaticamenteDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DeixarPadraoFinalizadoDocumentoEntrada", Column = "EMP_DEIXAR_PADRA_FINALIZADO_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeixarPadraoFinalizadoDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoNFe", Column = "EMP_VERSAO_NFE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.VersaoNFe), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.VersaoNFe VersaoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitaSincronismoDocumentosDestinados", Column = "EMP_HABILITA_SINCRONISMO_DOCUMENTOS_DESTINADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitaSincronismoDocumentosDestinados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmpresaMobile", Column = "EMP_EMPRESA_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmpresaMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointIntegracaoGPA", Column = "EMP_ENDPOINT_INTEGRACAO_GPA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string EndpointIntegracaoGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoPedidoVenda", Column = "EMP_TIPO_IMPRESSAO_PEDIDO_VENDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoVenda), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoPedidoVenda TipoImpressaoPedidoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TransportadorFiliais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TRANSPORTADOR_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TransportadorFilial", Column = "TFI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial> TransportadorFiliais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarParcelaAutomaticamente", Column = "EMP_GERAR_PARCELA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarParcelaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmitirVendaPrazoNFCe", Column = "EMP_EMITIR_VENDA_PRAZO_NFCE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirVendaPrazoNFCe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO_NFCE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacaoNFCe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLancamentoFinanceiroSemOrcamento", Column = "EMP_TIPO_LANCAMENTO_FINANCEIRO_SEM_ORCAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento TipoLancamentoFinanceiroSemOrcamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaIntegracaoDocumentosDestinado", Column = "EMP_UTILIZA_INTEGRACAO_DOCUMENTOS_DESTINADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaIntegracaoDocumentosDestinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_LIBERACAO_PARA_PAGAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberacaoParaPagamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoSimplesNacional", Column = "EMP_OBSERVACAO_SIMPLES_NACIONAL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoSimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_COMPRA_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_GERAR_PEDIDO_RECEBER_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoAoReceberCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitaLancamentoProdutoLote", Column = "EMP_HABILITA_LANCAMENTO_PRODUTO_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitaLancamentoProdutoLote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "EMP_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "EMP_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "EMP_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjIpef", Column = "EMP_CNPJ_IPEF", TypeType = typeof(string), Length = 18, NotNull = false)]
        public virtual string CnpjIpef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "EMP_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_ANTECIPACAO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AntecipacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_GERAR_LOTE_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLoteEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_GERAR_LOTE_ESCRITURACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLoteEscrituracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_PROVISIONAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisionarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_CODIGO_CENTRO_CUSTO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoCentroCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_CODIGO_ESTABELECIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEstabelecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_CODIGO_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SVMMesmoQueMultimodal", Column = "EMP_SVM_MESMO_QUE_MULTIMODAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SVMMesmoQueMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SVMTerminaisPortuarioOrigemDestino", Column = "EMP_SVM_TERMINAIS_PORTUARIO_ORIGEM_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SVMTerminaisPortuarioOrigemDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SVMBUSPortoOrigemDestino", Column = "EMP_SVM_BUS_PORTO_ORIGEM_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SVMBUSPortoOrigemDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaRateioSVM", Column = "EMP_FORMA_RATEIO_SVM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaRateioSVM? FormaRateioSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "EMP_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [Obsolete("Passou a ser uma lista, TiposIntegracaoValePedagio")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracaoValePedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_TIPO_INTEGRACAO_VALE_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TiposIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO_INTEGRACAO_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeMaximaEmailRPS", Column = "EMP_QUANTIDADE_MAXIMA_EMAIL_RPS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaEmailRPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubtraiDescontoBaseICMS", Column = "EMP_SUBTRAI_DESCONTO_BASE_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SubtraiDescontoBaseICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArmazenarDanfeParaSMS", Column = "EMP_ARMAZENAR_DANFE_PARA_SMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenarDanfeParaSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarEnvioDanfeSMS", Column = "EMP_ATIVAR_ENVIO_DANFE_SMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarEnvioDanfeSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenSMS", Column = "EMP_TOKEN_SMS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TokenSMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarTabelaValorOrdemServicoVenda", Column = "EMP_HABILITAR_TABELA_VALOR_ORDEM_SERVICO_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTabelaValorOrdemServicoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAlterarEmpresaOrdemServicoVenda", Column = "EMP_PERMITE_ALTERAR_EMPRESA_ORDEM_SERVICO_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarEmpresaOrdemServicoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarNumeroInternoOrdemServicoVenda", Column = "EMP_HABILITAR_NUMERO_INTERNO_ORDEM_SERVICO_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarNumeroInternoOrdemServicoVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_VALIDAR_MOTORISTA_TELERISCO_AO_CONFIRMAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarMotoristaTeleriscoAoConfirmarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_EMITIR_TODOS_CTES_COMO_SIMPLES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EmitirTodosCTesComoSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_COBRAR_DOCUMENTOS_DESTINADOS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CobrarDocumentosDestinados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJContabilidade", Column = "EMP_CNPJ_CONTABILIDADE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFContabilidade", Column = "EMP_CPF_CONTABILIDADE", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPFContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaDataVencimentoNaEmissao", Column = "EMP_UTILIZA_DATA_VENCIMENTO_NA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaDataVencimentoNaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarCorreios", Column = "EMP_INTEGRAR_CORREIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_TIPO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TiposIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual", Column = "EMP_BLOQUEAR_FINALIZACAO_PEDIDO_VENDA_DATA_ENTREGA_DIFERENTE_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearFinalizacaoPedidoVendaDataEntregaDiferenteAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarEtiquetaProdutosNFe", Column = "EMP_HABILITAR_ETIQUETA_PRODUTOS_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEtiquetaProdutosNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirImportarApenasPedidoVendaFinalizado", Column = "EMP_PERMITIR_IMPORTAR_APENAS_PEDIDO_VENDA_FINALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarApenasPedidoVendaFinalizado { get; set; }

        //Esta flag serve para criar uma empresa padrão para a retirada de produto. Assim, conseguimos cadastrar os veiculos nesta empresa padrão
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmpresaRetiradaProduto", Column = "EMP_EMPRESA_RETIRADA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmpresaRetiradaProduto { get; set; }

        /// <summary>
        /// Notificar o agendamento da coleta para o destinatário dos pedidos que este transportador fizer.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarDestinatarioAgendamentoColeta", Column = "EMP_NOTIFICAR_DESTINATARIO_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarDestinatarioAgendamentoColeta { get; set; }

        //independente de ser frete municipal ou outro vai gerar 
        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_SEMPRE_EMITIR_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreEmitirNFS { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaAnexo", Column = "ANX_CODIGO")]
        public virtual IList<EmpresaAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarEstoqueNegativo", Column = "EMP_CONTROLAR_ESTOQUE_NEGATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarEstoqueNegativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCertificadoIdoneidade", Column = "EMP_NUMERO_CERTIFICADO_IDONEIDADE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string NumeroCertificadoIdoneidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarSomenteClientesAssociados", Column = "EMP_VISUALIZAR_SOMENTE_CLIENTES_ASSOCIADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarSomenteClientesAssociados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodicidadeEmissaoNFSManual", Column = "EMP_PERIODICIDADE_EMISSAO_NFS_MANUAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade PeriodicidadeEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaSemanaEmissaoNFSManual", Column = "EMP_DIA_SEMANA_EMISSAO_NFS_MANUAL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemanaEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaMesEmissaoNFSManual", Column = "EMP_DIA_MES_EMISSAO_NFS_MANUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaMesEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCreditoC197SPEDFiscal", Column = "EMP_GERAR_CREDITO_C197_SPED_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCreditoC197SPEDFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearTransportador", Column = "EMP_BLOQUEAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoBloqueio", Column = "EMP_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaConsultaSintegra", Column = "EMP_DATA_ULTIMA_CONSULTA_SINTEGRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaSintegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProximaConsultaSintegra", Column = "EMP_DATA_PROXIMA_CONSULTA_SINTEGRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProximaConsultaSintegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaDeducaoValePedagio", Column = "EMP_FORMA_DEDUCAO_VALE_PEDAGIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaDeducaoValePedagio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaDeducaoValePedagio? FormaDeducaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RestringirLocaisCarregamentoAutorizadosMotoristas", Column = "EMP_RESTRINGIR_LOCAIS_CARREGAMENTO_AUTORIZADOS_MOTORISTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RestringirLocaisCarregamentoAutorizadosMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaTransportadoraPadraoContratacao", Column = "EMP_UTILIZAR_TRANSPORTADOR_PADRAO_CONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaTransportadoraPadraoContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "EMP_OBSERVACAO_CTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EstadosFeeder", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_UF_FEEDER")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual IList<Dominio.Entidades.Estado> EstadosFeeder { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMPRESA_FAVORECIDA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFavorecida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_ORDENAR_CARGA_MOBILE_CRESCENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrdenarCargasMobileCrescente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EMP_RECUSAR_INTEGRACAO_POD_UNILEVER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecusarIntegracaoPODUnilever { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaContrato", Column = "EMP_CODIGO_EMPRESA_CONTRATO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.EmpresaContrato EmpresaContrato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "EMP_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimenTributario", Column = "EMP_REGIMEN_TRIBUTARIO", TypeType = typeof(RegimenTributacao), NotNull = false)]
        public virtual RegimenTributacao RegimenTributario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TransportadorFerroviario", Column = "EMP_TRANSPORTADOR_FERROVIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransportadorFerroviario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirUtilizarCadastroAgendamentoColeta", Column = "EMP_PERMITIR_UTILIZAR_CADASTRO_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirUtilizarCadastroAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Operadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_OPERADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EMP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Operadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoServicoCorreios", Column = "EMP_CODIGO_SERVICO_CORREIOS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoServicoCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimeTributarioCTe", Column = "EMP_CODIGO_REGIME_TRIBUTARIO_CTE", TypeType = typeof(RegimeTributarioCTe), NotNull = false)]
        public virtual RegimeTributarioCTe RegimeTributarioCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAvisoPeriodico", Column = "EMP_GERAR_AVISO_PERIODICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAvisoPeriodico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ACadaAvisoPeriodico", Column = "EMP_ACADA_AVISO_PERIODICO", TypeType = typeof(int), NotNull = false)]
        public virtual int ACadaAvisoPeriodico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ACadaTipoTermo", Column = "EMP_ACADA_TIPO_TERMO", TypeType = typeof(int), NotNull = false)]
        public virtual int ACadaTipoTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoAvisoPeriodico", Column = "EMP_PERIODO_AVISO_PERIODICO", TypeType = typeof(DiaSemanaMesAno), NotNull = false)]
        public virtual DiaSemanaMesAno PeriodoAvisoPeriodico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoTipoTermo", Column = "EMP_PERIODO_TIPO_TERMO", TypeType = typeof(DiaSemanaMesAno), NotNull = false)]
        public virtual DiaSemanaMesAno PeriodoTipoTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTermo", Column = "EMP_TIPO_TERMO", TypeType = typeof(TipoTermo), NotNull = false)]
        public virtual TipoTermo TipoTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGeracaoTermo", Column = "EMP_TIPO_GERACAO_TERMO", TypeType = typeof(TipoGeracaoTermo), NotNull = false)]
        public virtual TipoGeracaoTermo TipoGeracaoTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAguardarParaGerarTermo", Column = "EMP_TEMPO_AGUARDAR_PARA_GERAR_TERMO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAguardarParaGerarTermo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimTermoQuitacaoInicial", Column = "EMP_DATA_FIM_TERMINO_QUITACAO_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimTermoQuitacaoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoAvisoTermoQuitacaoGerado", Column = "EMP_DATA_ULTIMO_AVISO_TERMO_QUITACAO_GERADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoAvisoTermoQuitacaoGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro", Column = "EMP_GERAR_CIOT_PARA_TODAS_CARGAS_MESMO_SEM_VEICULO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFSeNacional", Column = "EMP_NFSE_NACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFSeNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contribuinte", Column = "EMP_CONTRIBUINTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Contribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeContribuinte", Column = "EMP_DATA_VALIDADE_CONTRIBUINTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarTransportadorContribuinte", Column = "EMP_VALIDAR_TRANSPORTADOR_CONTRIBUINTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTransportadorContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EquiparadoTAC", Column = "EMP_EQUIPARADO_TAC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EquiparadoTAC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarSMNaBrk", Column = "EMP_NAO_GERAR_SM_NA_BRK", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarSMNaBrk { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IgnorarDocumentosDuplicadosNaEmissaoCTe", Column = "EMP_IGNORAR_DOCUMENTOS_DUPLICADOS_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarDocumentosDuplicadosNaEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirInformarInicioEFimPreTrip", Column = "EMP_INFORMAR_INICIO_E_FIM_PRE_TRIP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInformarInicioEFimPreTrip { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirReenviarIntegracaoDasCargasAppTrizy", Column = "EMP_REENVIAR_INTEGRACAO_DAS_CARGAS_APP_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirReenviarIntegracaoDasCargasAppTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarIntegracaoSuperAppTrizy", Column = "EMP_NAO_GERAR_INTEGRACAO_SUPER_APP_TRIZY", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarIntegracaoSuperAppTrizy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MostrarOcorrenciasFiliaisMatriz", Column = "EMP_MOSTRAR_OCORRENCIAS_FILIAL_MATRIZ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MostrarOcorrenciasFiliaisMatriz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePIX", Column = "EMP_TIPO_CHAVE_PIX", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.TipoChavePix), Length = 200, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Enumerador.TipoChavePix TipoChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIX", Column = "EMP_CHAVE_PIX", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChavePIX { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get
            {
                return $"{(string.IsNullOrWhiteSpace(this.CodigoEmpresa) ? "" : this.CodigoEmpresa + " ")}{(string.IsNullOrWhiteSpace(this.RazaoSocial) ? "" : this.RazaoSocial + " ")}{(this.Localidade != null ? $"({this.Localidade.DescricaoCidadeEstado})" : string.Empty)}";
            }
        }

        public virtual string NomeCNPJ
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(RazaoSocial))
                    descricao += RazaoSocial.Trim();
                if (!string.IsNullOrWhiteSpace(Tipo))
                    descricao += " - " + CNPJ_Formatado;

                return descricao;
            }
        }

        public virtual string NomeCNPJLimitado
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(RazaoSocial))
                    descricao += RazaoSocial.Trim();
                if (!string.IsNullOrWhiteSpace(Tipo))
                    descricao += " - " + CNPJ_Formatado;

                if (!string.IsNullOrEmpty(descricao) && descricao.Length > 60)
                    descricao = descricao.Substring(0, 60);

                return descricao;
            }
        }

        public virtual string CNPJ_Formatado
        {
            get
            {
                return this.Tipo != null && this.Tipo.Equals("F") ? string.Format(@"{0:000\.000\.000\-00}", long.Parse(this.CNPJ)) : this.Tipo != null && this.Tipo.Equals("E") ? string.Empty : string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ));
            }
        }

        public virtual string CNPJ_SemFormato
        {
            get
            {
                return this.Tipo != null && this.Tipo.Equals("F") ? string.Format(@"{0:00000000000}", long.Parse(this.CNPJ)) : this.Tipo != null && this.Tipo.Equals("E") ? string.Empty : string.Format(@"{0:00000000000000}", long.Parse(this.CNPJ));
            }
        }

        public virtual string CNPJ_Identificacao_Exterior
        {
            get { return this.CNPJ.ToString(); }
        }

        public virtual string RaizCnpj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CNPJ_SemFormato) && CNPJ_SemFormato.Length > 7)
                    return CNPJ_SemFormato.Substring(0, 8);
                else
                    return "";
            }
        }

        public virtual string CPF_CNPJ_Formatado_AX
        {
            get
            {
                return this.Tipo != null && this.Tipo.Equals("J") ? string.Format(@"{0:000000000000\-00}", long.Parse(this.CNPJ)) : string.Format(@"{0:000000000\-00}", long.Parse(this.CNPJ));
            }
        }

        public virtual string DescricaoLiberacaoParaPagamentoAutomatico
        {
            get { return LiberacaoParaPagamentoAutomatico ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public virtual string DescricaoOptanteSimplesNacional
        {
            get { return OptanteSimplesNacional ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status == "A")
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual string DescricaoEmissao
        {
            get
            {
                switch (this.TipoAmbiente)
                {
                    case Enumeradores.TipoAmbiente.Homologacao:
                        return "Homologação";
                    case Enumeradores.TipoAmbiente.Producao:
                        return "Produção";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipoAmbiente
        {
            get
            {
                switch (this.TipoAmbiente)
                {
                    case Enumeradores.TipoAmbiente.Homologacao:
                        return "Homologação";
                    case Enumeradores.TipoAmbiente.Producao:
                        return "Produção";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string LocalidadeUF
        {
            get
            {
                return this.Localidade.Descricao + " - " + this.Localidade.Estado.Sigla;
            }
        }

        public virtual int? TempoAtividadeEmAnos
        {
            get
            {
                if (!DataInicioAtividade.HasValue)
                    return null;

                DateTime dataAtual = DateTime.Now;
                int anoInicioAtividade = DataInicioAtividade.Value.Year;

                if ((DataInicioAtividade.Value.Month > dataAtual.Month) || ((DataInicioAtividade.Value.Month == dataAtual.Month) && (DataInicioAtividade.Value.Day > dataAtual.Day)))
                    anoInicioAtividade++;

                int tempoAtividade = (dataAtual.Year - anoInicioAtividade);

                return (tempoAtividade > 0) ? tempoAtividade : 0;
            }
        }

        public virtual string FusoHorarioPadrao
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FusoHorario))
                    return FusoHorario;
                else if (!string.IsNullOrWhiteSpace(EmpresaPai?.FusoHorario ?? string.Empty))
                    return EmpresaPai.FusoHorario;
                else
                    return "E. South America Standard Time";
            }
        }

        #endregion
    }
}
