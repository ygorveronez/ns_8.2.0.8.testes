namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_RETIFICACAO_COLETA", EntityName = "MotivoRetificacaoColeta", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta", NameType = typeof(MotivoRetificacaoColeta))]
    public class MotivoRetificacaoColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MRC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MRC_OBSERVACAO", TypeType = typeof(string), Length = 450, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAplicacaoColetaEntrega", Column = "MRC_TIPO_APLICACAO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega TipoAplicacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MRC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReabrirEntregaZerarDataFim", Column = "MRC_REABRIR_ENTREGA_ZERAR_DATA_FIM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReabrirEntregaZerarDataFim { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}

