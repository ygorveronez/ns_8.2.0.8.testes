using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_INTEGRACAO", EntityName = "GestaoDevolucaoIntegracao", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao", NameType = typeof(GestaoDevolucaoIntegracao))]
    public class GestaoDevolucaoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<GestaoDevolucaoIntegracaoArquivo>, IEquatable<GestaoDevolucaoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoGestaoDevolucao", Column = "GDI_TIPO_INTEGRACAO_GESTAO_DEVOLUCAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao TipoIntegracaoGestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DEVOLUCAO_INTEGRACAO_ARQUIVOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GestaoDevolucaoIntegracaoArquivo", Column = "DIA_CODIGO")]
        public virtual ICollection<GestaoDevolucaoIntegracaoArquivo> ArquivosTransacao { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return this.GestaoDevolucao.Descricao;
            }
        }

        public virtual bool Equals(GestaoDevolucaoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}