namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTA_TRANSPORTADOR", EntityName = "ContaTransportador", Name = "Dominio.Entidades.Embarcador.Transportadores.ContaTransportador", NameType = typeof(ContaTransportador))]
    public class ContaTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAgencia", Column = "COT_NUMERO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "COT_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "COT_NUMERO_CONTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "COT_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        public virtual string Descricao
        {
            get { return $"{this.Banco.Descricao} - nº {NumeroConta}"; }
        }
    }
}
