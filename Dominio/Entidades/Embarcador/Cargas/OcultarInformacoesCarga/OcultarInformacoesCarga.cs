using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCULTAR_INFORMACOES_CARGA", EntityName = "OcultarInformacoesCarga", Name = "Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga", NameType = typeof(OcultarInformacoesCarga))]

    public class OcultarInformacoesCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OIC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "OIC_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rota", Column = "OIC_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorNotaFiscal", Column = "OIC_VALOR_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorNotaFiscal { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProduto", Column = "OIC_VALOR_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarRota", Column = "OIC_VISUALIZAR_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarRota { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Usuarios", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCULTAR_INFORMACOES_CARGA_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PerfisAcesso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCULTAR_INFORMACOES_CARGA_PERFIL_ACESSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PerfilAcesso", Column = "PAC_CODIGO")]
        public virtual ICollection<Usuarios.PerfilAcesso> PerfisAcesso { get; set; }
    }
}
