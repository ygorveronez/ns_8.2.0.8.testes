using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_SUARIZADA_ANEXOS", EntityName = "CargaOcorrenciaSumarizadaAnexos", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos", NameType = typeof(CargaOcorrenciaSumarizadaAnexos))]
    public class CargaOcorrenciaSumarizadaAnexos : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrenciaSumarizado", Column = "COS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado CargaSumarizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "COA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "COA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "COA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }


        public virtual bool Equals(CargaOcorrenciaSumarizadaAnexos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
