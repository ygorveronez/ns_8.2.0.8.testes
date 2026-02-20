using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_PRIORIDADE_CARGA_AUTO_CONFIG", EntityName = "TipoCargaPrioridadeCargaAutoConfig", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig", NameType = typeof(TipoCargaPrioridadeCargaAutoConfig))]
    public class TipoCargaPrioridadeCargaAutoConfig : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCargaModeloVeicularAutoConfig", Column = "TMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCargaModeloVeicularAutoConfig TipoCargaModeloVeicularAutoConfig { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "TPC_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.TipoDeCarga?.Descricao ?? string.Empty) + " - " + Posicao.ToString();
            }
        }

        public virtual bool Equals(TipoCargaPrioridadeCargaAutoConfig other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
