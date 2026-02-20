using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DADOS_AVERBACAO", EntityName = "CargaDadosAverbacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao", NameType = typeof(CargaDadosAverbacao))]

    public class CargaDadosAverbacao : EntidadeBase, IEquatable<CargaDadosAverbacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CDA_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_TENTATIVAS_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int tentativasIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "CDA_CODIGO_RETORNO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CDA_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CDA_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "CDA_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CDA_TIPO", TypeType = typeof(Enumeradores.TipoAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.TipoAverbacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CDA_STATUS", TypeType = typeof(Enumeradores.StatusAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.StatusAverbacaoCTe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento SituacaoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_ADICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_IOF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal IOF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SeguradoraAverbacao", Column = "CDA_SEGURADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao SeguradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Averbacao", Column = "CDA_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Averbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguroAverbacao", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Seguros.ApoliceSeguroAverbacao ApoliceSeguroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_PERCENTUAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Forma", Column = "CDA_FORMA", TypeType = typeof(Enumeradores.FormaAverbacaoCTE), NotNull = false)]
        public virtual Enumeradores.FormaAverbacaoCTE Forma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTe", Column = "CDA_CHAVE_CTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFe", Column = "CDA_CHAVE_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarga", Column = "CDA_NUMERO_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NummeroBooking", Column = "CDA_NUMERO_BOOKING", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NummeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOS", Column = "CDA_NUMERO_OS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroOS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        public virtual bool Equals(CargaDadosAverbacao other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaDadosAverbacao)this.MemberwiseClone();
        }
    }
}
