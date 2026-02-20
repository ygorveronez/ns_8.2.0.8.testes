using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_PROCESSAMENTO_RECEITA_DESPESA", EntityName = "VeiculoProcessamentoReceitaDespesa", Name = "Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa", NameType = typeof(VeiculoProcessamentoReceitaDespesa))]
    public class VeiculoProcessamentoReceitaDespesa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPR_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }
    }
}
