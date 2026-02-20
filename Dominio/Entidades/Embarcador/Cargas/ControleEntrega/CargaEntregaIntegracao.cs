using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_INTEGRACAO ", EntityName = "CargaEntregaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaEntregaIntegracao", NameType = typeof(CargaEntregaIntegracao))]
    public class CargaEntregaIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao>
    {
        public CargaEntregaIntegracao()
        {
            this.ProblemaIntegracao = "";
            this.DataIntegracao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEI_CODIGO_AGRUPADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoAgrupador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_ENTREGA_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaEntregaIntegracaoArquivo", Column = "CEA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(CargaEntregaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}

