using System;


namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_INF_CARGA", EntityName = "InformacaoCargaPreCTE", Name = "Dominio.Entidades.InformacaoCargaPreCTE", NameType = typeof(InformacaoCargaPreCTE))]
    public class InformacaoCargaPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        /// <summary>
        /// 00 - M3
        /// 01 - KG
        /// 02 - TON
        /// 03 - UN
        /// 04 - LT
        /// 05 - MMBTU
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedida", Column = "PIC_UN", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PIC_TIPO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PIC_QTD", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        public virtual decimal ValorDoFretePorUnidadeTransportada
        {
            get
            {
                return this.Quantidade > 0 ? (this.PreCTE.ValorFrete / this.Quantidade) : 0m;
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
    }
}
