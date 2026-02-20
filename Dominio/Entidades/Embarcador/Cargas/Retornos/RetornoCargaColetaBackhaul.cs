using System;

namespace Dominio.Entidades.Embarcador.Cargas.Retornos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_CARGA_COLETA_BACKHAUL", EntityName = "RetornoCargaColetaBackhaul", Name = "Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCargaColetaBackhaul", NameType = typeof(RetornoCargaColetaBackhaul))]
    public class RetornoCargaColetaBackhaul : EntidadeBase, IEquatable<RetornoCargaColetaBackhaul>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "RCB_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoCargaColetaBackhaul), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoCargaColetaBackhaul Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RetornoCarga", Column = "RCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RetornoCarga RetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCancelamentoRetornoCargaColetaBackhaul", Column = "RMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoCancelamentoRetornoCargaColetaBackhaul MotivoCancelamento { get; set; }

        public virtual string Descricao
        {
            get { return RetornoCarga.Carga.CodigoCargaEmbarcador; }
        }

        public virtual bool Equals(RetornoCargaColetaBackhaul other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
