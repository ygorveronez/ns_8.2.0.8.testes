using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELACAO_CTES_ENTREGUES", EntityName = "RelacaoCTesEntregues", Name = "Dominio.Entidades.RelacaoCTesEntregues", NameType = typeof(RelacaoCTesEntregues))]
    public class RelacaoCTesEntregues : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_NUMERO_CONTROLE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string NumeroControle { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataBipagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal Diaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_VALOR_ACRESCIMOS", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorAcrescimos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_VALOR_DESCONTOS", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_TIPO_DIARIA", TypeType = typeof(Dominio.Enumeradores.TipoDiariaRelacaoCTesEntregues), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoDiariaRelacaoCTesEntregues TipoDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCE_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusRelacaoCTesEntregues), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusRelacaoCTesEntregues Status { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "CTes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RELACAO_ENTREGUES_CTES")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "RCE_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        //public virtual ICollection<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RELACAO_CTES_ENTREGUES_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RelacaoCTesEntreguesCTes", Column = "RCC_CODIGO")]
        public virtual IList<RelacaoCTesEntreguesCTes> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Coletas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RELACAO_CTES_ENTREGUES_COLETA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RelacaoCTesEntreguesColeta", Column = "RCO_CODIGO")]
        public virtual IList<RelacaoCTesEntreguesColeta> Coletas { get; set; }

        public virtual string DescricaoStatus {
            get
            {
                switch (Status)
                {
                    case Enumeradores.StatusRelacaoCTesEntregues.Aberto:
                        return "Aberto";
                    case Enumeradores.StatusRelacaoCTesEntregues.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusRelacaoCTesEntregues.Fechado:
                        return "Fechado";
                    default:
                        return "";
                }
            }
        }
        public virtual int DiferencaKm
        {
            get
            {
                return Math.Abs(KmFinal - KmInicial);
            }
        }
    }
}
