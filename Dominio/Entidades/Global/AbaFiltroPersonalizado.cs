using System;

namespace Dominio.Entidades.Global
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABA_FILTRO_PERSONALIZADO", EntityName = "AbaFiltroPersonalizado", Name = "Dominio.Entidades.AbaFiltroPersonalizado", NameType = typeof(AbaFiltroPersonalizado))]
    public class AbaFiltroPersonalizado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "AFP_DESCRICAO_ABA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cliente", Column = "AFP_CLIENTE", TypeType = typeof(int), NotNull = true)]
        public virtual int Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoFiltroPesquisa", Column = "AFP_TIPO_FILTRO", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa CodigoFiltroPesquisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "AFP_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }
    }
}


