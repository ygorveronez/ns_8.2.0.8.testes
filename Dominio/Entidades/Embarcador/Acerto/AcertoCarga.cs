using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_CARGA", EntityName = "AcertoCarga", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoCarga", NameType = typeof(AcertoCarga))]
    public class AcertoCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcerto", Column = "ACC_PERCENTUAL_ACERTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioAcerto", Column = "ACC_PEDAGIO_ACERTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PedagioAcerto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancadoManualmente", Column = "ACC_LANCADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBrutoCarga", Column = "ACC_VALOR_BRUTO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBrutoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSCarga", Column = "ACC_VALOR_ICMS_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaFracionada", Column = "ACC_CARGA_FRACIONADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaFracionada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBonificacaoCliente", Column = "ACC_VALOR_BONIFICACAO_CLIENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBonificacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioAcertoCredito", Column = "ACC_PEDAGIO_ACERTO_CREDITO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PedagioAcertoCredito { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga.CodigoCargaEmbarcador;
            }
        }

        public virtual string DescricaoPercentualAcerto()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='true'></span> " + this.PercentualAcerto.ToString("n2");
        }

        public virtual string DescricaoPercentualAcertoCompleto()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='false'></span> " + this.PercentualAcerto.ToString("n2");
        }

        public virtual string DescricaoPedagioAcerto()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='true'></span> " + this.PedagioAcerto.ToString("n2");
        }

        public virtual string DescricaoPedagioAcertoNaoPermiteAlterar()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='false'></span> " + this.PedagioAcerto.ToString("n2");
        }

        public virtual string DescricaoPedagioAcertoCredito()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='true'></span> " + this.PedagioAcertoCredito.ToString("n2");
        }

        public virtual string DescricaoPedagioAcertoNaoPermiteAlterarCredito()
        {
            return "<span class='spnTipoValorItem' data-codigo-item='" + this.Carga.Codigo + "' data-permite-alterar='false'></span> " + this.PedagioAcertoCredito.ToString("n2");
        }

        public virtual bool Equals(AcertoCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
