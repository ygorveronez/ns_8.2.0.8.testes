using System;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACEITE_LOTE", EntityName = "AceiteLote", Name = "Dominio.Entidades.Embarcador.Avarias.AceiteLote", NameType = typeof(AceiteLote))]
    public class AceiteLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario ResponsavelRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALO_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALO_OBSERAVCAO", TypeType = typeof(string), Length = 1500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Lote", Column = "LAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.Lote Lote { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Lote.Descricao;
            }
        }

    }
}
