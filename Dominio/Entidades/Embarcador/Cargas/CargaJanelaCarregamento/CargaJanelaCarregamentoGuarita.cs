using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_GUARITA", EntityName = "CargaJanelaCarregamentoGuarita", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita", NameType = typeof(CargaJanelaCarregamentoGuarita))]
    public class CargaJanelaCarregamentoGuarita : EntidadeCargaBase, IEntidade
    {
        public CargaJanelaCarregamentoGuarita() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

        [Obsolete("Não utilizar. O campo foi descontinuado")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JanelaDescarregamento", Column = "JDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento JanelaDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoPatio.FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        /// <summary>
        /// usado como uma flag, se verdadeiro exibe na tela de guarita, se falso, não exibe, por exemplo, após estar na guarita a carga é movida para as excedentes da janela o sistema seta para false essa flag
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioEntradaDefinido", Column = "JCG_HORARIO_ENTRADA_DEFINIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioEntradaDefinido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaGuaritaLiberada", Column = "JCG_ETAPA_GUARITA_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaGuaritaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaSaidaGuaritaLiberada", Column = "JCG_ETAPA_SAIDA_GUARITA_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaSaidaGuaritaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JCG_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChegadaGuarita", Column = "JCG_TIPO_CHEGADA_GUARITA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoChegadaGuarita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoChegadaGuarita TipoChegadaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramadaParaChegada", Column = "CJC_DATA_PROGRAMADA_PARA_VEICULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataProgramadaParaChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChegadaVeiculo", Column = "CJC_CHEGADA_VEICULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataChegadaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntregaGuarita", Column = "CJC_DATA_ENTRADA_GUARITA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntregaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalCarregamento", Column = "CJC_DATA_FINAL_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoVeiculo", Column = "CJC_LIBERACAO_VEICULO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaidaGuarita", Column = "CJC_DATA_SAIDA_GUARITA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaidaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_PRODUTOR_RURAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PesagemProdutorRural { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_PEDIDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PesagemPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_INICIAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PesagemInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_QUANTIDADE_CAIXAS", TypeType = typeof(int), NotNull = false)]
        public virtual int PesagemQuantidadeCaixas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_NUMERO_NF_PRODUTOR", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNfProdutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_FINAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PesagemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_PORCENTAGEM_PERDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PorcentagemPerda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_LOTE_INTERNO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string LoteInterno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_LOTE_INTERNO_DOIS", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string LoteInternoDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_NUMERO_LACRE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NumeroLacre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_PESAGEM_PRESSAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesagemPressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_CHEGADA_DENEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaDenegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_OBSERVACAO_CHEGADA_DENEGADA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoChegadaDenegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_POSSUI_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_OBSERVACAO_DEVOLUCAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCG_DOCA_CHEGADA_GUARITA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DocaChegadaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCG_SENHA_CHEGADA_GUARITA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaChegadaGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemPesagemInicial", Column = "JCG_ORIGEM_PESAGEM_INICIAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemPesagemGuarita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemPesagemGuarita? OrigemPesagemInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemPesagemFinal", Column = "JCG_ORIGEM_PESAGEM_FINAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemPesagemGuarita), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemPesagemGuarita? OrigemPesagemFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_PESAGEM_INICIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioPesagemInicial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_PESAGEM_FINAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioPesagemFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialBalanca", Column = "FBA_CODIGO_PESAGEM_INICIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.FilialBalanca BalancaPesagemInicial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialBalanca", Column = "FBA_CODIGO_PESAGEM_FINAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.FilialBalanca BalancaPesagemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CJC_QUANTIDADE_LITROS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeLitros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPesagemCarga", Column = "CJC_SITUACAO_PESAGEM_CARGA", TypeType = typeof(SituacaoPesagemCarga), NotNull = false)]
        public virtual SituacaoPesagemCarga? SituacaoPesagemCarga { get; set; }

        public virtual string Descricao
        {
            get { return CargaJanelaCarregamento?.Descricao ?? string.Empty; }
        }
    }
}
