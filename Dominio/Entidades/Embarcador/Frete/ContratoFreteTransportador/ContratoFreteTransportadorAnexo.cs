namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_ANEXOS", EntityName = "ContratoFreteTransportadorAnexo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo", NameType = typeof(ContratoFreteTransportadorAnexo))]
    public class ContratoFreteTransportadorAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_ANEXO_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }


        public virtual bool Equals(ContratoFreteTransportadorAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
