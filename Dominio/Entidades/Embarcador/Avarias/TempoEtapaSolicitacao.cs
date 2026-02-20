using System;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TEMPO_ETAPA_SOLICITACAO", EntityName = "TempoEtapaSolicitacao", Name = "Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao", NameType = typeof(TempoEtapaSolicitacao))]
    public class TempoEtapaSolicitacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAvaria", Column = "SAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria SolicitacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TES_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TES_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Entrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TES_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Saida { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Tempo na etapa da avaria " + (this.SolicitacaoAvaria?.Descricao ?? "");
            }
        }

        public virtual string DescricaoEtapa
        {
            get
            {
                switch (Etapa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Criacao:
                        return "Criação";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Autorizacao:
                        return "Autorização";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao.Aprovada:
                        return "Aprovada";
                    default:
                        return "";
                }
            }
        }
    }
}
