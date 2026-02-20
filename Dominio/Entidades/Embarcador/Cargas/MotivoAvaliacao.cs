namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_AVALIACAO", EntityName = "MotivoAvaliacao", Name = "Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao", NameType = typeof(MotivoAvaliacao))]
    public class MotivoAvaliacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMA_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela", Column = "TMA_GERAR_ATENDIMENTO_AUTOMATICO_QUANDO_AVALIACAO_FOR_UMA_ESTRELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(MotivoAvaliacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
