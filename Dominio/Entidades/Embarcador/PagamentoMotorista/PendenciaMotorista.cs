using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PENDENCIA_MOTORISTA", EntityName = "PendenciaMotorista", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista", NameType = typeof(PendenciaMotorista))]
    public class PendenciaMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "PEM_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PEM_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PEM_VALOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PEM_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PEM_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS Pendencia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PENDENCIA_MOTORISTA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PendenciaMotoristaAnexo", Column = "APE_CODIGO")]
        public virtual IList<PendenciaMotoristaAnexo> Anexos { get; set; }
        public virtual string Descricao
        {
            get
            {
                return this.Motorista?.Nome;
            }
        }
    }
}
