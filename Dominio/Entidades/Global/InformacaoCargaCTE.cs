using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_INF_CARGA", EntityName = "InformacaoCargaCTE", Name = "Dominio.Entidades.InformacaoCargaCTE", NameType = typeof(InformacaoCargaCTE))]
    public class InformacaoCargaCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        /// <summary>
        /// 00 - M3
        /// 01 - KG
        /// 02 - TON
        /// 03 - UN
        /// 04 - LT
        /// 05 - MMBTU
        /// 99 - M3 ST (Unidade de medida feita para a Sintravir)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedida", Column = "ICA_UN", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ICA_TIPO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "ICA_QTD", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        public virtual decimal ValorDoFretePorUnidadeTransportada
        {
            get
            {
                return this.Quantidade > 0 ? (this.CTE.ValorFrete / this.Quantidade) : 0m;
            }
        }

        public virtual string DescricaoUnidadeMedida
        {
            get
            {
                Dominio.Enumeradores.UnidadeMedida un;
                Enum.TryParse<Dominio.Enumeradores.UnidadeMedida>(UnidadeMedida, out un);
                return un.ToString("G");
            }
        }

        public virtual Dominio.Enumeradores.UnidadeMedida EnumUnidadeDeMedida
        {
            get
            {
                switch (this.UnidadeMedida)
                {
                    case "00":
                        return Dominio.Enumeradores.UnidadeMedida.M3;
                    case "01":
                        return Dominio.Enumeradores.UnidadeMedida.KG;
                    case "02":
                        return Dominio.Enumeradores.UnidadeMedida.TON;
                    case "03":
                        return Dominio.Enumeradores.UnidadeMedida.UN;
                    case "04":
                        return Dominio.Enumeradores.UnidadeMedida.LT;
                    case "05":
                        return Dominio.Enumeradores.UnidadeMedida.MMBTU;
                    case "99":
                        return Dominio.Enumeradores.UnidadeMedida.M3_ST;
                    default:
                        return Dominio.Enumeradores.UnidadeMedida.KG;
                }
            }
        }

        public virtual InformacaoCargaCTE Clonar()
        {
            return (InformacaoCargaCTE)this.MemberwiseClone();
        }
    }
}
