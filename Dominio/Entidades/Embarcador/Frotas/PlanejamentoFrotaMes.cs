using System;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANEJAMENTO_FROTA_MES", EntityName = "PlanejamentoFrotaMes", Name = "Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes", NameType = typeof(PlanejamentoFrotaMes))]
    public class PlanejamentoFrotaMes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PFM_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        public virtual string Descricao
        { 
            get
            {
                return $"{Data:MM/yyyy} - {Filial.Descricao}";
            }
        }
    }
}
