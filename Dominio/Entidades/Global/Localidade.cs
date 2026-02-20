using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCALIDADES", EntityName = "Localidade", Name = "Dominio.Entidades.Localidade", NameType = typeof(Localidade))]
    public class Localidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LOC_CODIGO")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LOC_DESCRICAO", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCidade", Column = "LOC_CODIGO_CIDADE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoCidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "LOC_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "LOC_DATAATU", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIBGE", Column = "LOC_IBGE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIBGE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoLocalidadeEmbarcador", Column = "LOC_CODIGO_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoLocalidadeEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_POLO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePolo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNFSeGoiania", Column = "LOC_CODIGO_NFSE_GOIANIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoNFSeGoiania { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "OutrasDescricoes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOCALIDADES_OUTRAS_DESCRICOES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "LOC_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 80)]
        public virtual ICollection<string> OutrasDescricoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "LOC_LATITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "LOC_LONGITUDE", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "LOC_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "LOC_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoZonaTarifaria", Column = "LOC_COD_ZONA_TARIFARIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string CodigoZonaTarifaria { get; set; }

        /// <summary>
        /// LatitudeEntrega e LongitudeEntrega utilizado para atender clientes da frimesa entrega de produtos no nordeste.. onde a entrega ocorre por barco e não é possível roteirizar
        /// quando um cliente for da cidade que possui as informações de lat e lng entrega informado, deve ser roteirizado até esses pontos.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "LatitudeEntrega", Column = "LOC_LATITUDE_ENTREGA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LatitudeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongitudeEntrega", Column = "LOC_LONGITUDE_ENTREGA", TypeType = typeof(decimal), NotNull = false, Scale = 10, Precision = 18)]
        public virtual decimal? LongitudeEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoURF", Column = "LOC_CODIGO_URF", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoURF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRA", Column = "LOC_CODIGO_RA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoRA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Poligono", Column = "LOC_POLIGONO", TypeType = typeof(string), Length = 150000, NotNull = false)]
        public virtual string Poligono { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "LOC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RKST", Column = "LOC_RKST", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string RKST { get; set; }

        public virtual int CodigoIBGESemUf {
            get
            {
                string codigoIBGE = CodigoIBGE.ToString();

                if (codigoIBGE.Length > 2)
                    return codigoIBGE.Substring(2).ToInt();

                return CodigoIBGE;
            }
        }

        public virtual string DescricaoCidadeEstado
        {
            get
            {
                if (this.CodigoIBGE != 9999999 || this.Pais == null)
                    return this.Descricao + " - " + this.Estado?.Sigla ?? "";
                else
                {
                    if (this.Pais.Abreviacao != null)
                        return this.Descricao + " - " + this.Pais.Abreviacao;
                    else
                        return this.Descricao + " - " + this.Pais.Nome;
                }
            }
        }

        public virtual string DescricaoCidadeEstadoPais
        {
            get
            {
                return $@"{DescricaoCidadeEstado} - {Pais?.Descricao ?? string.Empty}";
            }
        }
    }
}
