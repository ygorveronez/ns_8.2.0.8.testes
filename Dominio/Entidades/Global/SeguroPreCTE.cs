namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_SEGURO", EntityName = "SeguroPreCTE", Name = "Dominio.Entidades.SeguroPreCTE", NameType = typeof(SeguroPreCTE))]
    public class SeguroPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PSE_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoSeguro), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoSeguro Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "PSE_NOMESEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "PSE_NUMAPOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAverbacao", Column = "PSE_NUMAVERBACAO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string NumeroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PSE_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoSeguro.Destinatario:
                        return "Destinatário";
                    case Enumeradores.TipoSeguro.Emitente_CTE:
                        return "Emitente";
                    case Enumeradores.TipoSeguro.Expedidor:
                        return "Expedidor";
                    case Enumeradores.TipoSeguro.Recebedor:
                        return "Recebedor";
                    case Enumeradores.TipoSeguro.Remetente:
                        return "Remetente";
                    case Enumeradores.TipoSeguro.Tomador_Servico:
                        return "Tomador";
                    default:
                        return "";
                }
            }
        }
    }
}
