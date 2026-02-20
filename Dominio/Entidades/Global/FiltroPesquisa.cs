using System;

namespace Dominio.Entidades.Global
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILTRO_PESQUISA", EntityName = "FiltroPesquisa", Name = "Dominio.Entidades.FiltroPesquisa", NameType = typeof(FiltroPesquisa))]
    public class FiltroPesquisa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dados", Column = "FPE_DADOS", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Dados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FPE_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFiltroPesquisa", Column = "FPE_CODIGO_FILTRO_PESQUISA", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa CodigoFiltroPesquisa { get; set; }

        [Obsolete] //Foi definido como obsoleto pois deve ser usado a descrição do modelo
        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFiltro", Column = "FPE_NOME_FILTRO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NomeFiltro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FiltroPesquisaModelo", Column = "FPM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FiltroPesquisaModelo Modelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusaoFiltro", Column = "FPE_DATA_INCLUSAO_FILTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusaoFiltro { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}


