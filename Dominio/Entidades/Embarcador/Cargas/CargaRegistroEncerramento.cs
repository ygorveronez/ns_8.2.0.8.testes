using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_REGISTRO_ENCERRAMENTO", EntityName = "CargaRegistroEncerramento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento", NameType = typeof(CargaRegistroEncerramento))]
    public class CargaRegistroEncerramento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEncerramento", Column = "CRE_DATA_ENCERRAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaEncerramento", Column = "CRE_NOTA_ENCERRAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NotaEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CRE_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicao", Column = "CRE_MOTIVO_REJEICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CRE_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EncerrouMDFes", Column = "CRE_ENCERROU_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrouMDFes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EncerrarSemIntegracao", Column = "CRE_ENCERRAR_SEM_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EncerrarSemIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Carga?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.Descricao(); }
        }
    }
}
