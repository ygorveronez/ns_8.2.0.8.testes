using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_OUTRO_ENDERECO", EntityName = "ClienteOutroEndereco", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco", NameType = typeof(ClienteOutroEndereco))]
    public class ClienteOutroEndereco : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "COE_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "COE_ENDERECO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "COE_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "COE_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "COE_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "COE_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "COE_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmbarcador", Column = "COE_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string CodigoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "COE_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEndereco", Column = "COE_ENDERECO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco TipoEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoDigitado", Column = "COE_ENDERECO_DIGITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnderecoDigitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLogradouro", Column = "COE_TIPO_LOGRADOURO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro? TipoLogradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "COE_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "COE_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeoLocalizacaoStatus", Column = "COE_GEOLOCALIZACAO_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus GeoLocalizacaoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArea", Column = "COE_TIPO_AREA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArea), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArea TipoArea { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "COE_AREA", Type = "StringClob", NotNull = false)]
        public virtual string Area { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaioEmMetros", Column = "COE_RAIO_METROS", TypeType = typeof(int), NotNull = false)]
        public virtual int? RaioEmMetros { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Endereco;
            }
        }

        public virtual string EnderecoCompletoCidadeeEstado
        {
            get
            {
                return this.Endereco + ", " + this.Bairro + ", " + this.Numero + " " + this.Localidade.DescricaoCidadeEstado;
            }
        }

        public virtual bool Equals(ClienteOutroEndereco other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
