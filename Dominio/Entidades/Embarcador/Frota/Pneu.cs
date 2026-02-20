using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU", EntityName = "Frota.Pneu", Name = "Dominio.Entidades.Embarcador.Frota.Pneu", NameType = typeof(Pneu))]
    public class Pneu : EntidadeBase, IEquatable<Pneu>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoNota", Column = "PNU_DESCRICAO_NOTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DescricaoNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DTO", Column = "PNU_DTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DTO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_KM_ATUAL_RODADO", TypeType = typeof(int), NotNull = true)]
        public virtual int KmAtualRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_KM_ANTERIOR_RODADO", TypeType = typeof(int), NotNull = false)]
        public virtual int KmAnteriorRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_KM_RODADO_ENTRE_SULCOS", TypeType = typeof(int), NotNull = false)]
        public virtual int KmRodadoEntreSulcos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFogo", Column = "PNU_NUMERO_FOGO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string NumeroFogo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sulco", Column = "PNU_SULCO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Sulco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SulcoAnterior", Column = "PNU_SULCO_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SulcoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_TIPO_AQUISICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu TipoAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAquisicao", Column = "PNU_VALOR_AQUISICAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCustoAtualizado", Column = "PNU_VALOR_CUSTO_ATUALIZADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCustoAtualizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCustoKmAtualizado", Column = "PNU_VALOR_CUSTO_KM_ATUALIZADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCustoKmAtualizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_VIDA_ATUAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.VidaPneu), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.VidaPneu VidaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Almoxarifado", Column = "AMX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Almoxarifado Almoxarifado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.BandaRodagemPneu", Column = "PBR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BandaRodagemPneu BandaRodagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServicoFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.DocumentoEntradaItem DocumentoEntradaItem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.ModeloPneu", Column = "PML_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloPneu Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimentacaoEstoque", Column = "PNU_DATA_MOVIMENTACAO_ESTOQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMovimentacaoEstoque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataMovimentacaoReforma", Column = "PNU_DATA_MOVIMENTACAO_REFORMA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMovimentacaoReforma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdensServico", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(Ordem.OSE_NUMERO AS varchar(20)) + ' (' +  ISNULL(Fornecedor.CLI_NOME, 'Fornec. ñ informado') + ' - ' + FORMAT(ISNULL(Fornecedor.CLI_CGCCPF, 0),'##############') + ')' 
                                                                                                FROM T_FROTA_ORDEM_SERVICO Ordem
                                                                                                LEFT OUTER JOIN T_CLIENTE Fornecedor on Fornecedor.CLI_CGCCPF = Ordem.CLI_CGCCPF
                                                                                                WHERE Ordem.OSE_SITUACAO in (0,1,3,4,7,8,9,10) AND Ordem.PNU_CODIGO = PNU_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string OrdensServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculoPneu", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_PNEU")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNU_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoPneu", Column = "VPN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.VeiculoPneu> VeiculoPneu { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculoEstepe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_ESTEPE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PNU_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoEstepe", Column = "VES_CODIGO")]
        public virtual ICollection<Dominio.Entidades.VeiculoEstepe> VeiculoEstepe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PNU_CALIBRAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Calibragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Milimitragem1", Column = "PNU_MILIMITRAGEM_1", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Milimitragem1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Milimitragem2", Column = "PNU_MILIMITRAGEM_2", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Milimitragem2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Milimitragem3", Column = "PNU_MILIMITRAGEM_3", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Milimitragem3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Milimitragem4", Column = "PNU_MILIMITRAGEM_4", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Milimitragem4 { get; set; }

        public virtual string Descricao
        {
            get { return NumeroFogo; }
        }

        public virtual decimal KmRodadoPorSulco
        {
            get { return (SulcoGasto > 0m) ? Math.Round(KmRodadoEntreSulcos / SulcoGasto, 2, MidpointRounding.AwayFromZero) : 0m; }
        }

        public virtual decimal SulcoGasto
        {
            get { return (Sulco > SulcoAnterior) ? Sulco - SulcoAnterior : 0m; }
        }

        public virtual bool Equals(Pneu other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
