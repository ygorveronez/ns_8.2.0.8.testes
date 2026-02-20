using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO_PARCELA", EntityName = "InfracaoParcela", Name = "Dominio.Entidades.Embarcador.Frota.InfracaoParcela", NameType = typeof(InfracaoParcela))]
    public class InfracaoParcela : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Infracao Infracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFP_PARCELA", TypeType = typeof(int), NotNull = true)]
        public virtual int Parcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFP_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IFP_VALOR_APOS_VENCIMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAposVencimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{Parcela} - {DataVencimento.ToString("dd/MM/yyyy")} - {Valor.ToString("n2")}";
            }
        }
    }
}
