using Dominio.Interfaces.Embarcador.Entidade;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRAVAMENTO_CHAVE", EntityName = "TravamentoChave", Name = "Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave", NameType = typeof(TravamentoChave))]
    public class TravamentoChave : EntidadeCargaBase,IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCH_DATA_TRAVAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataTravamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCH_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaTravaChaveLiberada", Column = "TCH_TRAVA_LIBERACAO_CHAVE_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaTravaChaveLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaLiberacaoChaveLiberada", Column = "TCH_ETAPA_LIBERACAO_CHAVE_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaLiberacaoChaveLiberada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCarga", Column = "CLC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCarga CheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCH_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTravamentoChave), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTravamentoChave Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaletesPBR", Column = "TCH_PALETES_PBR", TypeType = typeof(int), NotNull = false)]
        public virtual int PaletesPBR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaletesChep", Column = "TCH_PALETES_CHEP", TypeType = typeof(int), NotNull = false)]
        public virtual int PaletesChep { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TravamentoChaveAssinaturaMotorista", Column = "TAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TravamentoChaveAssinaturaMotorista TravamentoChaveAssinaturaMotorista { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "TCH_PREVISTO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? InicioPrevisto { get; set; }

        [Obsolete("Não utilizar, será deletada. Migrada para a observação da entidade FluxoGestaoPatioEtapas")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "TCH_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacoes { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoTravamentoChave.Liberada:
                        return "Liberada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoTravamentoChave.Travada:
                        return "Travada";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return "Controle de Chave - " + (this.Carga?.Descricao  ?? this.PreCarga?.Descricao ?? string.Empty);
            }
        }
    }
}