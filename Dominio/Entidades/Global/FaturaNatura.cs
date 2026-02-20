using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATURA_FATURA", EntityName = "FaturaNatura", Name = "Dominio.Entidades.FaturaNatura", NameType = typeof(FaturaNatura))]
    public class FaturaNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FAN_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPreFatura", Column = "FAN_NUMERO_PRE_FATURA", TypeType = typeof(long), NotNull = true)]
        public virtual long NumeroPreFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPreFatura", Column = "FAN_DATA_PRE_FATURA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataPreFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "FAN_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "FAN_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FAN_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FAN_STATUS", TypeType = typeof(ObjetosDeValor.Enumerador.StatusFaturaNatura), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusFaturaNatura Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Sacado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NATURA_FATURA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemFaturaNatura", Column = "FAI_CODIGO")]
        public virtual IList<ItemFaturaNatura> Itens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NaturaXMLs", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_NATURA_FATURA_XML")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NaturaXML", Column = "NAX_CODIGO")]
        public virtual ICollection<NaturaXML> NaturaXMLs { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (Status)
                {
                    case ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida:
                        return "Emitida";
                    case ObjetosDeValor.Enumerador.StatusFaturaNatura.Paga:
                        return "Paga";
                    case ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Enumerador.StatusFaturaNatura.Cancelada:
                        return "Cancelada";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
