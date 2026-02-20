using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_INFRACAO", EntityName = "TipoInfracao", Name = "Dominio.Entidades.Embarcador.Frota.TipoInfracao", NameType = typeof(TipoInfracao))]
    public class TipoInfracao : EntidadeBase, IEquatable<TipoInfracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TIN_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCTB", Column = "TIN_CODIGO_CTB", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoCTB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TIN_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIN_NIVEL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelInfracaoTransito), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelInfracaoTransito Nivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pontos", Column = "TIN_PONTOS", TypeType = typeof(int), NotNull = true)]
        public virtual int Pontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIN_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoInfracaoTransito), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoInfracaoTransito Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TIN_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoFichaMotorista", Column = "TIN_GERAR_MOVIMENTO_FICHA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoFichaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancarDescontoMotorista", Column = "TIN_LANCAR_DESCONTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarDescontoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirReplicarInformacao", Column = "TIN_PERMITE_REPLICAR_INFORMACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReplicarInformacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoComissaoMotorista", Column = "TIN_DESCONTO_COMISSAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DescontoComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReduzirPercentualComissaoMotorista", Column = "TIN_REDUZIR_PERCENTUAL_COMISSAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReduzirPercentualComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoComissaoMotorista", Column = "TIN_PERCENTUAL_REDUCAO_COMISSAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarTituloFinanceiro", Column = "TIN_NAO_GERAR_TITULO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarTituloFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_FICHA_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoFichaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TITULO_EMPRESA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoTituloEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarRenavamVeiculoObservacao", Column = "TIN_ADICIONAR_RENAVAM_VEICULO_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarRenavamVeiculoObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoObrigarInformarCidade", Column = "TIN_NAO_OBRIGAR_INFORMAR_CIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarCidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoObrigarInformarLocal", Column = "TIN_NAO_OBRIGAR_INFORMAR_LOCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarInformarLocal { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(TipoInfracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
