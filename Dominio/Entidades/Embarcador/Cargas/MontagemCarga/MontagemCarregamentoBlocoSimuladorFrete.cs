namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO_BLOCO_SIMULADOR_FRETE", EntityName = "MontagemCarregamentoBlocoSimuladorFrete", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFrete", NameType = typeof(MontagemCarregamentoBlocoSimuladorFrete))]
    public class MontagemCarregamentoBlocoSimuladorFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MontagemCarregamentoBloco", Column = "MCB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco Bloco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MSF_TIPO_SIMULACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CentroCarregamentoTipoOperacaoTipo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        /// <summary>
        /// Contém o Grupo que consiste em um bloco de simulação.
        /// Ex: 50.000 toneladas
        ///     Grupo   Qtde    Modelo  Capacidade
        ///     Grupo 1: 1 - CARRETA = 35.000
        ///              1 - VUC     = 15.000
        ///              
        ///     Grupo 2: 2 - 4º EIXO = 25.000
        ///         
        ///     Grupo 3: 1 - 4º EIXO = 25.000
        ///              1 - Truck   = 20.000
        ///              1 - Fiorino =  5.000
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "MSF_GRUPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Grupo { get; set; }

        /// <summary>
        /// Quantidade de viagens do Transportador, Modelo Veicular e Tipo de Operação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "MSF_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ranking", Column = "MSF_RANKING", TypeType = typeof(int), NotNull = true)]
        public virtual int Ranking { get; set; }

        /// <summary>
        /// Valor TOTAL DO FRETE do Bloco de Simulação pelo Transportador, Tipo Operação e Modelo veicular...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "MSF_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        /// <summary>
        /// Valor TOTAL DO FRETE do Bloco de Simulação pelo Transportador, Tipo Operação e Modelo veicular...
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "LeadTime", Column = "MSF_LEAD_TIME", TypeType = typeof(int), NotNull = false)]
        public virtual int LeadTime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeIsca", Column = "MSF_EXIGE_ISCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeIsca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencedor", Column = "MSF_VENCEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Vencedor { get; set; }

        /// <summary>
        /// Valor do Frete * Quantidade do modelo...
        /// </summary>
        public virtual decimal ValorTotalSimulacao { get { return this.ValorTotal * this.Quantidade; } }
    }
}
