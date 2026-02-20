using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_DESTINO", EntityName = "DestinoDoAcertoDeViagem", Name = "Dominio.Entidades.DestinoDoAcertoDeViagem", NameType = typeof(DestinoDoAcertoDeViagem))]
    public class DestinoDoAcertoDeViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCarga", Column = "ATC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoDeViagem", Column = "ACE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoDeViagem AcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ACD_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemInicial", Column = "ACD_KM_INICIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KilometragemInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemFinal", Column = "ACD_KM_FIM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KilometragemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "ACD_DATAINI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "ACD_DATAFIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "ACD_LOC_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "ACD_LOC_DESTINO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoCarga", Column = "ACD_PESO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "ACD_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "ACD_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrosDescontos", Column = "ACD_OUTROS_DESCONTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutrosDescontos { get; set; }

        public virtual int NumeroAcerto
        {
            get
            {
                return AcertoDeViagem?.Numero ?? 0;
            }
        }

        public virtual string DescricaoCTe
        {
            get
            {
                if (this.CTe != null)
                    return string.Concat(this.CTe.Numero, " - ", this.CTe.Serie.Numero);
                else
                    return string.Empty;
            }
        }

        public virtual string DescricaoOrigem
        {
            get
            {
                if (this.Origem != null)
                    return string.Concat(this.Origem.Descricao, " / ", this.Origem.Estado.Sigla);
                else
                    return string.Empty;
            }
        }

        public virtual string DescricaoDestino
        {
            get
            {
                if (this.Destino != null)
                    return string.Concat(this.Destino.Descricao, " / ", this.Destino.Estado.Sigla);
                else
                    return string.Empty;
            }
        }

        public virtual string DescricaoCarga
        {
            get
            {
                if (this.TipoCarga != null)
                    return this.TipoCarga.Descricao;
                else
                    return string.Empty;
            }
        }

        // Usado na visualizao do acerto
        public virtual string DescricaoDataDestino
        {
            get
            {
                DateTime? dataDestino = this.CTe?.DataEmissao ?? this.DataInicial;
                if (this.AcertoDeViagem.Empresa.CNPJ == "18805855000155" || this.AcertoDeViagem.Empresa.CNPJ == "12656321000128")
                    dataDestino = this.DataInicial;

                return dataDestino.HasValue ? dataDestino.Value.ToString("dd/MM/yyyy") : "";
            }
        }
    }
}
