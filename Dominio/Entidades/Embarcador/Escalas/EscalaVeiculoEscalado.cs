using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ESCALA_VEICULO_ESCALADO", EntityName = "EscalaVeiculoEscalado", Name = "Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado", NameType = typeof(EscalaVeiculoEscalado))]
    public class EscalaVeiculoEscalado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EscalaOrigemDestino", Column = "EOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino EscalaOrigemDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraCarregamento", Column = "EVO_HORA_CARREGAMENTO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "EVE_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "Veiculos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ESCALA_VEICULO_ESCALADO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EVO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Veiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ESCALA_VEICULO_ESCALADO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EVO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Motoristas { get; set; }

        public virtual string Descricao
        {
            get { return $"Veículo da escala {EscalaOrigemDestino.Descricao}"; }
        }

        public virtual bool Equals(EscalaVeiculoEscalado other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
