namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TEMPO_CHAMADO", EntityName = "ConfiguracaoTempoChamado", Name = "Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado", NameType = typeof(ConfiguracaoTempoChamado))]
    public class ConfiguracaoTempoChamado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CTC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAtendimento", Column = "CTC_TEMPO_ATENDIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}