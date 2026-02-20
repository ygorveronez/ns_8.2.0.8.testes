using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_HORIMETRO", EntityName = "HistoricoHorimetro", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoHorimetro", NameType = typeof(HistoricoHorimetro))]
    public class HistoricoHorimetro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HIH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroAtual", Column = "HIH_HORIMETRO_ATUAL", TypeType = typeof(int), NotNull = true)]        
        public virtual int HorimetroAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "HIH_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "HIH_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }
    }
}