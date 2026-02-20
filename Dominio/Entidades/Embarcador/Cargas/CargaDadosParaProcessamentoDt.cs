using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DADOS_PARA_PROCESSAMENTO_DT", EntityName = "CargaDadosParaProcessamentoDt", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt", NameType = typeof(CargaDadosParaProcessamentoDt))]
    public class CargaDadosParaProcessamentoDt : EntidadeBase, IEquatable<CargaDadosParaProcessamentoDt>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "CDP_DOCUMENTO", Type = "StringClob", NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }
     
        public virtual bool Equals(CargaDadosParaProcessamentoDt other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
