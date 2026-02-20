using System;

namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_BAIXA_PALLETS", EntityName = "CancelamentoBaixaPallets", Name = "Dominio.Entidades.Embarcador.Pallets.CancelamentoBaixaPallets", NameType = typeof(CancelamentoBaixaPallets))]
    public class CancelamentoBaixaPallets : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DevolucaoPallet Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBP_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CBP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Cancelamento Baixa - " + this.Devolucao.Descricao;
            }
        }
    }
}
