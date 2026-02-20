using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_FRETE_INTEGRACAO", EntityName = "CargaFreteIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao", NameType = typeof(CargaFreteIntegracao))]
    public class CargaFreteIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaCancelamentoCargaIntegracao>
    {
        public CargaFreteIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Stage Stage { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "ChavesNotasIntegradas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_FRETE_INTEGRACAO_NOTAS_INTEGRADAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CFI_CHAVE_NOTA_INTEGRADA", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> ChavesNotasIntegradas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_FRETE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(CargaCancelamentoCargaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
