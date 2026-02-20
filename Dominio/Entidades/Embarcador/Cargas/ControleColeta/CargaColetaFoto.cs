using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleColeta
{
    [Obsolete("O controle de coleta não existe mais na aplicação")]
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_COLETA_FOTO", EntityName = "CargaColetaFoto", Name = "Dominio.Entidades.Embarcador.Cargas.ControleColeta.CargaColetaFoto", NameType = typeof(CargaColetaFoto))]
    public class CargaColetaFoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaColeta", Column = "CCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaColeta CargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        public virtual string Descricao => "Foto - " + (this.CargaColeta?.Descricao ?? string.Empty);
    }
}
