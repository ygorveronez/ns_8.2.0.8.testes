using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AJUDA", EntityName = "Ajuda", Name = "Dominio.Entidades.Ajuda", NameType = typeof(Ajuda))]
    public class Ajuda : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AJU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_DESCRICAO", TypeType = typeof(string), Length = 200 , NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_LINK_VIDEO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string LinkVideo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AJU_TIPO_AJUDA", TypeType = typeof(Dominio.Enumeradores.TipoAjuda), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAjuda TipoAjuda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "AJU_NOME_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "AJU_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        /// <summary>
        /// Retorna o link montado do YouTube
        /// </summary>
        public virtual string LinkVideoYouTube

        {
            get
            {
                return "https://www.youtube.com/watch?v=" + (this.LinkVideo ?? string.Empty);
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipoAjuda
        {
            get
            {
                switch (this.TipoAjuda)
                {
                    case Enumeradores.TipoAjuda.Arquivo:
                        return "Arquivo";
                    case Enumeradores.TipoAjuda.Video:
                        return "Video";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
