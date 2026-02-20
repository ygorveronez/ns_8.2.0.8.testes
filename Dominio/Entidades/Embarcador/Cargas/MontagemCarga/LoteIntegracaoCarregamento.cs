using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_INTEGRACAO_CARREGAMENTO", EntityName = "LoteIntegracaoCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento", NameType = typeof(LoteIntegracaoCarregamento))]

    public class LoteIntegracaoCarregamento : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<LoteIntegracaoCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Carregamentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_INTEGRACAO_CARREGAMENTO_CARREGAMENTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carregamento", Column = "CRG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> Carregamentos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_INTEGRACAO_CARREGAMENTO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LIC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }


        public virtual bool Equals(LoteIntegracaoCarregamento other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
