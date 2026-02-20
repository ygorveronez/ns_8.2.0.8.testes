namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROCEDIMENTO_EMBARQUE", EntityName = "ProcedimentoEmbarque", Name = "Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque", NameType = typeof(ProcedimentoEmbarque))]

    public class ProcedimentoEmbarque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_INTEGRACAO_PROCEDIMENTO_EMBARQUE", TypeType = typeof(int), NotNull = false)]
        public virtual int IntegracaoProcedimentoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_CODIGO_MODELO_CONTRATACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoModeloContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_TEMPO_ENTREGA_ANGELLIRA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEntregaAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRE_NAO_ENVIAR_DATA_INICIO_TERMINO_VIAGEM_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarDataInicioETerminoViagemAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PRE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                string prodecimento = IntegracaoProcedimentoEmbarque.ToString();

                if (this.TipoOperacao != null)
                    prodecimento += " - " + this.TipoOperacao.Descricao;

                if (this.Filial != null)
                    prodecimento += " - " + this.Filial.Descricao;

                return prodecimento;

            }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }


    }
}
