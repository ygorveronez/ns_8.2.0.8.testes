using Dominio.Entidades.Embarcador.Fatura;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APLICAR_ACRESCIMO_DESCONTO_TITULO", EntityName = "AplicarAcrescimoDescontoNoTitulo", Name = "Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo", NameType = typeof(AplicarAcrescimoDescontoNoTitulo))]
    public class AplicarAcrescimoDescontoNoTitulo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "AAD_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; } 
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "AAD_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverProvisao", Column = "AAD_REMOVER_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverProvisao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "AAD_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        public virtual bool Equals(AplicarAcrescimoDescontoNoTitulo other)
        {
            return other.Codigo == this.Codigo ? true : false;
        }
    }
}
