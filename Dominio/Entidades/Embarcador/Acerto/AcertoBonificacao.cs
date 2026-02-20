using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_BONIFICACAO", EntityName = "AcertoBonificacao", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao", NameType = typeof(AcertoBonificacao))]
    public class AcertoBonificacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "ABO_MOTIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBonificacao", Column = "ABO_VALOR_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ABO_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoBonificacao", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.TipoBonificacao TipoBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "ABO_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "ABO_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "ABO_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "ABO_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Justificativa.Descricao + " - " + this.ValorBonificacao.ToString("n2");
            }
        }
        public virtual bool Equals(AcertoBonificacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
