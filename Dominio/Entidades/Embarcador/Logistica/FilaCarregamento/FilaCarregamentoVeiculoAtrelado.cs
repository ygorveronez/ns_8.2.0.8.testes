using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_ATRELADO", EntityName = "FilaCarregamentoVeiculoAtrelado", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado", NameType = typeof(FilaCarregamentoVeiculoAtrelado))]
    public class FilaCarregamentoVeiculoAtrelado : EntidadeBase, IEquatable<FilaCarregamentoVeiculoAtrelado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoMotorista", Column = "FLM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoMotorista FilaCarregamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Column = "FLV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        public virtual bool Equals(FilaCarregamentoVeiculoAtrelado other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
