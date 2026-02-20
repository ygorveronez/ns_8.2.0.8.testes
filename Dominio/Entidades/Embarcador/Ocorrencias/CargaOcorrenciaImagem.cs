using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_IMAGEM", EntityName = "CargaOcorrenciaImagem", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem", NameType = typeof(CargaOcorrenciaImagem))]
    public class CargaOcorrenciaImagem : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivo", Column = "COI_GUID_NOME_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidNomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "COI_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        public virtual bool Equals(CargaOcorrenciaImagem other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
