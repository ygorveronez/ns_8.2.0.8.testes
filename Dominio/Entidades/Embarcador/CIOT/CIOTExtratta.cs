namespace Dominio.Entidades.Embarcador.CIOT
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_EXTRATTA", EntityName = "CIOTExtratta", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTExtratta", NameType = typeof(CIOTExtratta))]
	public class CIOTExtratta : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEX_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_URL_API", TypeType = typeof(string), Length = 200, NotNull = false)]
		public virtual string URLAPI { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_CNPJ_APLICACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string CNPJAplicacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string Token { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_UTLIZAR_CNPJ_APLICACAO_PREENCHIMENTO_CNPJ_EMPRESA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_PREFIXO_CAMPO_NUMEROCONTROLE", TypeType = typeof(string), Length = 3, NotNull = false)]
		public virtual string PrefixoCampoNumeroControle { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_FORCAR_CIOT_NAO_EQUIPARADO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool ForcarCIOTNaoEquiparado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_UTLIZAR_TIPO_GERACAO_CIOT_PREENCHIMENTO_HABILITARCONTRATOCIOTAGREGADO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "CEX_ENVIAR_QUANTIDADES_MAIORES_QUE_ZERO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool EnviarQuantidadesMaioresQueZero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_NAO_REALIZAR_QUITACAO_DA_VIAGEM_NO_ENCERRAMENTO_DO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoRealizarQuitacaoViagemEncerramentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEX_ENVIAR_CARRETA_VIAGEM_V2", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCarretaViagemV2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeUsuario", Column = "CEX_NOME_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NomeUsuario { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoUsuario", Column = "CEX_DOCUMENTO_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string DocumentoUsuario { get; set; }

		public virtual string Descricao
		{
			get
			{
				return Codigo.ToString();
			}
		}
	}
}
