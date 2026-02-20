namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_TIPO", EntityName = "OrdemServicoFrotaTipo", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo", NameType = typeof(OrdemServicoFrotaTipo))]
    public class OrdemServicoFrotaTipo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "FOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_OBRIGAR_INFORMAR_LOCAL_DE_ARMAZENAMENTO_OS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarLocalDeArmazenamentoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_OBRIGAR_INFORMAR_CONDICAO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarCondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_INFORMAR_MOTIVO_LIBERAR_VEICULO_MANUTENCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarMotivoLiberarVeiculoManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_OS_CORRETIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OSCorretiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FOT_LANCAR_SERVICOS_OS_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarServicosOSManualmente { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
