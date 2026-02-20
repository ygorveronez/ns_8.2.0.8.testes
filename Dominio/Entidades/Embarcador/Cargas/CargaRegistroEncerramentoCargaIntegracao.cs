using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_REGISTRO_ENCERRAMENTO_CARGA_INTEGRACAO", EntityName = "CargaRegistroEncerramentoCargaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao", NameType = typeof(CargaRegistroEncerramentoCargaIntegracao))]
    public class CargaRegistroEncerramentoCargaIntegracao : Integracao.Integracao , IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaRegistroEncerramentoCargaIntegracao>
    {
        public CargaRegistroEncerramentoCargaIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaRegistroEncerramento", Column = "CRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento CargaRegistroEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_REGISTRO_ENCERRAMENTO_CARGA_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CEI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo;
            }
        }

        public virtual bool Equals(CargaRegistroEncerramentoCargaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
