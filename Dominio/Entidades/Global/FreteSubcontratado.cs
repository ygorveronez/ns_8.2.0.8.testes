using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_SUBCONTRATADO", EntityName = "FreteSubcontratado", Name = "Dominio.Entidades.FreteSubcontratado", NameType = typeof(FreteSubcontratado))]
    public class FreteSubcontratado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FRS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "FRS_PARCEIRO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Parceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Filial", Column = "FRS_FILIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTe", Column = "FRS_NUMERO_CTE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNFe", Column = "FRS_NUMERO_NFE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrada", Column = "FRS_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "FRS_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "FRS_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "FRS_TIPO", TypeType = typeof(Enumeradores.TipoFreteSubcontratado), NotNull = true)]
        public virtual Enumeradores.TipoFreteSubcontratado Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "FRS_PESO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FRS_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTotal", Column = "FRS_FRETE_TOTAL", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorFreteTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "FRS_ICMS", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteLiquido", Column = "FRS_FRETE_LIQUIDO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTaxaAdicional", Column = "FRS_TAXA_ADICIONAL", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorTaxaAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTDA", Column = "FRS_VALOR_TDA", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorTDA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTDE", Column = "FRS_VALOR_TDE", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorTDE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCarroDedicado", Column = "FRS_VALOR_CARRO_DEDICADO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorCarroDedicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissao", Column = "FRS_VALOR_COMISSAO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal ValorComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "FRS_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), NotNull = false, Scale = 6, Precision = 18)]
        public virtual decimal PercentualComissao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FRS_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecebedorDocumento", Column = "FRS_RECEBEDOR_DOCUMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string RecebedorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "FRS_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FRS_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FRS_STATUS", TypeType = typeof(Enumeradores.StatusFreteSubcontratado), NotNull = false)]
        public virtual Enumeradores.StatusFreteSubcontratado Status { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoFreteSubcontratado.Coleta:
                        return "Coleta";
                    case Enumeradores.TipoFreteSubcontratado.Devolucao:
                        return "Devolução";
                    case Enumeradores.TipoFreteSubcontratado.Entrega:
                        return "Entrega";
                    case Enumeradores.TipoFreteSubcontratado.Reentrega:
                        return "Reentrega";
                    default:
                        return string.Empty;
                }
            }
        }

    }
}
