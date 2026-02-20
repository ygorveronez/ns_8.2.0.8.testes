namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO_BLOCO", EntityName = "MontagemCarregamentoBloco", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco", NameType = typeof(MontagemCarregamentoBloco))]
    public class MontagemCarregamentoBloco : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SessaoRoteirizador", Column = "SRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador SessaoRoteirizador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        /// <summary>
        /// Contem o peso total dos pedido do cliente.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "MCB_PESO_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalletTotal", Column = "MCB_QUANTIDADE_PALLET_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal QuantidadePalletTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubicoTotal", Column = "MCB_METRO_CUBICO_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal MetroCubicoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumesTotal", Column = "MCB_VOLUMES_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal VolumesTotal { get; set; }

        /// <summary>
        /// Valor TOTAL DOS PEDIDOS simulados.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "MCB_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        /// <summary>
        /// Valor TOTAL DOS Fretes simulados.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFretes", Column = "MCB_VALOR_TOTAL_FRETES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalFretes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LeadTimeFretes", Column = "MCB_LEAD_TIME_FRETES", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTimeFretes { get; set; }

        /// <summary>
        /// Grava o transportador vencedor da simulação de frete... após simunado..
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CRG_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCarregamentoBloco Situacao { get; set; }

        /// <summary>
        /// Iremos gravar a Observação em caso de Erro na Simulação do Frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MCB_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 5000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }
    }
}
