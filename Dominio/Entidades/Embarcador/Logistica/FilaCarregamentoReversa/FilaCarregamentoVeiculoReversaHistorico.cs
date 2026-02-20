using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_REVERSA_HISTORICO", EntityName = "FilaCarregamentoVeiculoReversaHistorico", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico", NameType = typeof(FilaCarregamentoVeiculoReversaHistorico))]
    public class FilaCarregamentoVeiculoReversaHistorico : EntidadeBase, IEquatable<FilaCarregamentoVeiculoReversaHistorico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "FRH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FRH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculoReversa", Column = "FVR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculoReversa FilaCarregamentoVeiculoReversa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual bool Equals(FilaCarregamentoVeiculoReversaHistorico other)
        {
            return this.Codigo == other.Codigo;
        }
    }
}
