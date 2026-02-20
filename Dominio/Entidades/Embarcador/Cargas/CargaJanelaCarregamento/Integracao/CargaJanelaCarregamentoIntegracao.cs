using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_INTEGRACAO", EntityName = "CargaJanelaCarregamentoIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao", NameType = typeof(CargaJanelaCarregamentoIntegracao))]
	public class CargaJanelaCarregamentoIntegracao : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCI_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoViagem", Column = "JCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem CargaJanelaCarregamentoViagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoRetornoRecebimento", Column = "JCI_TIPO_RETORNO_RECEBIMENTO", TypeType = typeof(TipoRetornoRecebimento), NotNull = false)]
		public virtual TipoRetornoRecebimento TipoRetornoRecebimento { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoEvento", Column = "JCI_TIPO_EVENTO", TypeType = typeof(TipoEventoIntegracaoJanelaCarregamento), NotNull = false)]
		public virtual TipoEventoIntegracaoJanelaCarregamento TipoEvento { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "JCI_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracao), NotNull = false)]
		public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "JCI_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataCriacao { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Usuario UsuarioCriacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "JCI_MENSAGEM", Type = "StringClob", NotNull = false)]
		public virtual string Mensagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "JCI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string Protocolo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NovaAnalise", Column = "JCI_NOVA_ANALISE", TypeType = typeof(bool), NotNull = false)]
		public virtual bool NovaAnalise { get; set; }

		[NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_INTEGRACAO_ARQUIVOS")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "JCI_CODIGO")]
		[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaJanelaCarregamentoIntegracaoArquivo", Column = "JIA_CODIGO")]
		public virtual ICollection<CargaJanelaCarregamentoIntegracaoArquivo> ArquivosTransacao { get; set; }

		#region Propriedades com Regras

		public virtual string Descricao
		{
			get { return Codigo.ToString(); }
		}

		public virtual string TipoEventoDescricao
		{
			get
			{
				return TipoEventoIntegracaoJanelaCarregamentoHelper.ObterDescricao(this.TipoEvento);
			}
		}

		public virtual string TipoRetornoRecebimentoDescricao
		{
			get
			{
				return TipoRetornoRecebimentoHelper.ObterDescricao(this.TipoRetornoRecebimento);
			}
		}

		#endregion
	}
}
