using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_EVENTO", EntityName = "CargaIntegracaoEvento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEvento", NameType = typeof(CargaIntegracaoEvento))]
    public class CargaIntegracaoEvento : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaIntegracaoEvento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_EVENTO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CIE_MENSAGEM", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "CIE_ETAPA_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaCarga), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaCarga Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnvioSucesso", Column = "CIE_ENVIO_SUCESSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnvioSucesso { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"{this.Codigo} - {this.ProblemaIntegracao}";
            }
        }

        public virtual bool Equals(CargaIntegracaoEvento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
