using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_FATURAMENTO", EntityName = "FaturamentoCIOT", Name = "Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT", NameType = typeof(FaturamentoCIOT))]
    public class FaturamentoCIOT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Vencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_NUMERO", TypeType = typeof(long), NotNull = false)]
        public virtual long Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_TAXA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Taxa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_TARIFA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Tarifa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_TIPO", TypeType = typeof(FaturamentoEFreteTipo), NotNull = false)]
        public virtual FaturamentoEFreteTipo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_STATUS", TypeType = typeof(FaturamentoEFreteStatus), NotNull = false)]
        public virtual FaturamentoEFreteStatus Status { get; set; }



        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_FATURAMENTO_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturamentoCIOTItem", Column = "CFI_CODIGO")]
        public virtual IList<FaturamentoCIOTItem> Itens { get; set; }

        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "Outros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_FATURAMENTO_OUTRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturamentoCIOTOutro", Column = "CFO_CODIGO")]
        public virtual IList<FaturamentoCIOTOutro> Outros { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pagamentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CIOT_FATURAMENTO_PAGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturamentoCIOTPagamento", Column = "CFP_CODIGO")]
        public virtual IList<FaturamentoCIOTPagamento> Pagamentos { get; set; }
    }
}
