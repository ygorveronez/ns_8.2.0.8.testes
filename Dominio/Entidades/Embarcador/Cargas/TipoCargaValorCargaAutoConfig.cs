using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_VALOR_CARGA_AUTO_CONFIG", EntityName = "TipoCargaValorCargaAutoConfig", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig", NameType = typeof(TipoCargaValorCargaAutoConfig))]
    public class TipoCargaValorCargaAutoConfig : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCargaModeloVeicularAutoConfig", Column = "TMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCargaModeloVeicularAutoConfig TipoCargaModeloVeicularAutoConfig { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "TPV_TIPO_VALOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorAutomatizacaoTipoCargaValor), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorAutomatizacaoTipoCargaValor TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TPV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado UFDestino { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.TipoCarga?.Descricao ?? string.Empty) + " - " + Valor.ToString("n2");
            }
        }
        public virtual bool Equals(TipoCargaValorCargaAutoConfig other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
