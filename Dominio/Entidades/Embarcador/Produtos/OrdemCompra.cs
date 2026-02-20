using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_PRINCIPAL", DynamicUpdate = true, EntityName = "OrdemDeCompraPrincipal", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemDeCompraPrincipal", NameType = typeof(OrdemDeCompraPrincipal))]
    public class OrdemDeCompraPrincipal : EntidadeBase, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleIntegracaoEmbarcador", Column = "OPR_CONTROLE_INTEGRACAO_EMBARCADOR", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string ControleIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ORDEM_DE_COMPRA_PRINCIPAL_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OPR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "OPR_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataIntegracao { get ; set ; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "OPR_PROBLEMA_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ProblemaIntegracao { get ; set; }
    }
}
