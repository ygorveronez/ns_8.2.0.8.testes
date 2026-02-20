using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CHAMADO", EntityName = "ConfiguracaoTipoOperacaoChamado", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado", NameType = typeof(ConfiguracaoTipoOperacaoChamado))]
	public class ConfiguracaoTipoOperacaoChamado : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCH_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirGerarAtendimento", Column = "CCH_NAO_PERMITIR_GERAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool NaoPermitirGerarAtendimento { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "PermitirSelecionarApenasAlgunsMotivosAtendimento", Column = "CCH_PERMITIR_SELECIONAR_APENAS_ALGUNS_MOTIVOS_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool PermitirSelecionarApenasAlgunsMotivosAtendimento { get; set; }
		
		[NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarRetornoGeradoParaFinalizacaoAtendimento", Column = "CCH_NAO_VALIDAR_RETORNO_GERADO_PARA_FINALIZACAO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool NaoValidarRetornoGeradoParaFinalizacaoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarAutomaticamenteAtendimentoNfeEntregue", Column = "CCH_FINALIZAR_AUTOMATICAMENTE_ATENDIMENTO_NFE_ENTREGUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAutomaticamenteAtendimentoNfeEntregue { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "MotivosChamados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CHAMADO_MOTIVO_CHAMADO")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "CCH_CODIGO")]
		[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotivoChamado", Column = "MCH_CODIGO")]
		public virtual ICollection<Chamados.MotivoChamado> MotivosChamados { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CHAMADO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> Transportadores { get; set; }

        public virtual string Descricao
		{
			get { return "Configurações de Chamado"; }
		}
	}
}
