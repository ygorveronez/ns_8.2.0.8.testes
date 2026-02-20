using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_IRREGULARIDADE", EntityName = "MotivoIrregularidade", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade", NameType = typeof(MotivoIrregularidade))]
    public class MotivoIrregularidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTI_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTI_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMotivo", Column = "MTI_TIPO_MOTIVO", TypeType = typeof(TipoMotivoIrregularidade), NotNull = false)]
        public virtual TipoMotivoIrregularidade TipoMotivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Irregularidade", Column = "IRR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade { get; set; }

    }
}
