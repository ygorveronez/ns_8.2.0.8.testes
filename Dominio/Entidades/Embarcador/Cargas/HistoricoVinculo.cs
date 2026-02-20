using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VINCULO", EntityName = "HistoricoVinculo", Name = "Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo", NameType = typeof(HistoricoVinculo))]
    public class HistoricoVinculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "THV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "VeiculoTracao", Class = "Veiculo", Column = "VEI_CODIGO_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo VeiculoTracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculoReboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_HISTORICO_VINCULO_REBOQUES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "THV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> VeiculoReboques { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_HISTORICO_VINCULO_MOTORISTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "THV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalVinculo", Column = "THV_LOCAL_VINCULO", TypeType = typeof(LocalVinculo), NotNull = false)]
        public virtual LocalVinculo LocalVinculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraVinculo", Column = "THV_DATA_HORA_VINCULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraVinculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraDesvinculo", Column = "THV_DATA_HORA_DESVINCULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraDesvinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Name = "FilaCarregamento", Column = "FLV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.FilaCarregamentoVeiculo FilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "THV_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

       
    }
}
