using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_DOCUMENTO_TRANSPORTE", EntityName = "DocumentoTransporte", Name = "Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte", NameType = typeof(DocumentoTransporte))]
    public class DocumentoTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ADT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNF", Column = "ADT_NUMERO_NFE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCte", Column = "ADT_NUMERO_CTE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCte", Column = "ADT_CHAVE_CTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFe", Column = "ADT_CHAVE_NFE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "ADT_PESO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumen", Column = "ADT_VOLUMEN", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumen { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusDocumento", Column = "ADT_STATUS", TypeType = typeof(StatusDocumento), NotNull = false)]
        public virtual StatusDocumento StatusDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ADT_OBSERVACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AgendamentoColeta", Column = "ACO_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual AgendamentoColeta Agendamento { get; set; }

    }
}
