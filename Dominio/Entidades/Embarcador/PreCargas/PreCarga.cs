using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PRE_CARGA", EntityName = "PreCarga", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCarga", NameType = typeof(PreCarga))]
    public class PreCarga : Cargas.CargaBase, IEquatable<PreCarga>
    {
        public PreCarga() { }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public override int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPreCarga", Column = "PCA_NUMERO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_NUMERO_CARGA_INTERNO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroPreCargaInterno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDadosSumarizados", Column = "CDS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaDadosSumarizados DadosSumarizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DATA_ATUALIZACAO_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacaoImportacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PCA_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_CALCULANDO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalculandoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoPendencia", Column = "PCA_MOTIVO_PENDENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "PCA_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PCA_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoAlteracaoData", Column = "PCA_MOTIVO_ALTERACAO_DATA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoAlteracaoData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PENDENCIA_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenciaCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "PCA_DISTANCIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_ADICIONADA_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionadaManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PROGRAMACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DOCA_CARREGAMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string DocaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_CARGA_RETORNO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CargaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PROBLEMA_VINCULAR_CARGA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ProblemaVincularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_TENTATIVAS_VINCULAR_FILA_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasVincularFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_USUARIO_CANCELAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoProgramacaoCarga", Column = "CPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga ConfiguracaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SugestaoProgramacaoCarga", Column = "SPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga SugestaoProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "CAR_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Veiculo Veiculo { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EscalaVeiculoEscalado", Column = "EVE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado EscalaVeiculoEscalado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.AreaVeiculoPosicao LocalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinatarios", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRE_CARGA_DESTINATARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRE_CARGA_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "PED_CODIGO")]
        public virtual ICollection<Usuario> Motoristas { get; set; }

        public override ICollection<Usuario> ListaMotorista
        {
            get { return Motoristas; }
            set { Motoristas = value; }
        }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRE_CARGA_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public override ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pedido", Column = "PED_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPreCarga", Column = "PCA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga SituacaoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaixaTemperatura", Column = "FTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura FaixaTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_AGUARDANDO_GERACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoGeracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoPreCarga", Column = "PCA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "PCA_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

		#endregion Propriedades

		#region Propriedades com Regras

		public virtual string Descricao
        {
            get { return this.NumeroPreCarga; }
        }

        #endregion Propriedades com Regra

        #region Propriedade - Datas Fluxo Pre Carga

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioViagem", Column = "CAR_DATA_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "CAR_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoInicioViagem", Column = "CAR_DATA_PREVISAO_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoFimViagem", Column = "CAR_DATA_PREVISAO_FIM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PREVISAO_CHEGADA_DOCA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoChegadaDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PREVISAO_CHEGADA_DESTINATARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoChegadaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_PREVISAO_SAIDA_DESTINATARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoSaidaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCA_DATA_CARRETA_INFORMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarretaInformada { get; set; }

        /// <summary>
        /// Campo requisitado pela DANONE, inserido manualmente na tela.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntregaManual", Column = "PCA_DATA_PREVISAO_ENTREGA_MANUAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntregaManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaDePreCarga", Column = "PCA_CARGA_DE_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaDePreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "PCA_PRE_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoImportacaoPedidoAtrasada", Column = "MIA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada MotivoImportacaoPedidoAtrasada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioCarregamentoInformadoNoPedido", Column = "PCA_HORARIO_CARREGAMENTO_INFORMADO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioCarregamentoInformadoNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PCA_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallet", Column = "PCA_QUANTIDADE_PALET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePallet { get; set; }


        #endregion Propriedade - Datas Fluxo Pre Carga

        #region Métodos Públicos

        public virtual bool Equals(PreCarga other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos

        #region Métodos Sobrescritos

        public override bool IsCarga()
        {
            return false;
        }

        protected override string ObterDescricaoEntidade()
        {
            return "pré carga";
        }

        protected override string ObterNumero()
        {
            return NumeroPreCarga;
        }

        #endregion Métodos Sobrescritos
    }
}
