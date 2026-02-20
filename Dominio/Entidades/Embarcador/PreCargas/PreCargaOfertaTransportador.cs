using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CARGA_OFERTA_TRANSPORTADOR", EntityName = "PreCargaOfertaTransportador", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador", NameType = typeof(PreCargaOfertaTransportador))]
    public class PreCargaOfertaTransportador : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCargaOferta", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargaOferta PreCargaOferta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "POT_TIPO", TypeType = typeof(TipoPreCargaOfertaTransportador), NotNull = true)]
        public virtual TipoPreCargaOfertaTransportador Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "POT_SITUACAO", TypeType = typeof(SituacaoPreCargaOfertaTransportador), NotNull = true)]
        public virtual SituacaoPreCargaOfertaTransportador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioLimiteConfirmacao", Column = "POT_HORARIO_LIMITE_CONFIRMACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HorarioLimiteConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueada", Column = "POT_BLOQUEADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Bloqueada { get; set; }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual string ObterCorLinha()
        {
            return Bloqueada ? "#afb8bf" : Tipo.ObterCorLinha();
        }

        #endregion Métodos Públicos
    }
}
