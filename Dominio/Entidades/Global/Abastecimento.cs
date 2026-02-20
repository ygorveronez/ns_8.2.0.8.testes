using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO", EntityName = "Abastecimento", Name = "Dominio.Entidades.Abastecimento", NameType = typeof(Abastecimento))]
    public class Abastecimento : EntidadeBase, IEquatable<Dominio.Entidades.Abastecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoDeViagem", Column = "ACE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AcertoDeViagem AcertoDeViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntrada", Column = "DOE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntrada DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AbastecimentoTicketLog", Column = "ABA_CODIGO_ABASTECIMENTO_TICKETLOG", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AbastecimentoTicketLog AbastecimentoTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "ABA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecebimentoAbastecimento", Column = "ABA_TIPO_RECEBIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento? TipoRecebimentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ABA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "ABA_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Kilometragem", Column = "ABA_KM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Kilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Litros", Column = "ABA_LITROS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Litros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "ABA_VALOR_UN", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomePosto", Column = "ABA_POSTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomePosto { get; set; }

        /// <summary>
        /// A - ABERTO
        /// F - FECHADO
        /// I - INCONSISTENTE
        /// G - AGRUPADO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ABA_SITUACAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemAnterior", Column = "ABA_KM_ANTERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KilometragemAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Media", Column = "ABA_MEDIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Media { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ABA_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pago", Column = "ABA_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Pago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "ABA_DOCUMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "ABA_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoAbastecimento", Column = "FAB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento FechamentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "ABA_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "ABA_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "ABA_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "ABA_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "ABA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoInconsistencia", Column = "ABA_MOTIVO_INCONSISTENCIA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MotivoInconsistencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LitrosOriginal", Column = "ABA_LITROS_ORIGINAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal LitrosOriginal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemOriginal", Column = "ABA_KM_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KilometragemOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroOriginal", Column = "ABA_HORIMETRO_ORIGINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPrecoDaTabelaDeValoresDoFornecedor", Column = "ABA_UTILIZAR_PRECO_TABELA_VALORES_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrecoDaTabelaDeValoresDoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarContasAPagarParaAbastecimentoExternos", Column = "ABA_GERAR_CONTAS_PAGAR_PARA_ABASTECIMENTO_EXTERNOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarContasAPagarParaAbastecimentoExternos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PAGAMENTO_EXTERNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoPagamentoExterno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoAbastecimento", Column = "ABC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento ConfiguracaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Abastecimentos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ABASTECIMENTO_AGRUPADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ABA_CODIGO_AGRUPADO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Abastecimento", Column = "ABA_CODIGO")]
        public virtual ICollection<Abastecimento> Abastecimentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AcertosViagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_ABASTECIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ACB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoAbastecimento", Column = "ACV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> AcertosViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AbastecimentoAlterado", Column = "ABA_ABASTECIMENTO_ALTERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AbastecimentoAlterado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemAnteriorAlteraecao", Column = "ABA_KM_ANTERIOR_ALTERACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KilometragemAnteriorAlteraecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnterior", Column = "ABA_DATA_ANTERIOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ANTERIOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario MotoristaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ABA_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "ABA_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoDestinadoEmpresa", Column = "DDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Documentos.DocumentoDestinadoEmpresa DocumentoDestinadoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNotaFiscal", Column = "ABA_CHAVE_NOTA_FISCAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ChaveNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Requisicao", Column = "ABA_REQUISICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Requisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BombaAbastecimento", Column = "ABB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento BombaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LiberacaoAbastecimentoAutomatizado", Column = "LAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado LiberacaoAbastecimentoAutomatizado { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAcertos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(C.ACV_NUMERO AS NVARCHAR(20)) FROM T_ACERTO_ABASTECIMENTO A
                                                                                                JOIN T_ACERTO_DE_VIAGEM C ON C.ACV_CODIGO = A.ACV_CODIGO
                                                                                                WHERE A.ABA_CODIGO = ABA_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroAcertos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContagemDuplicado", Formula = @"ISNULL((SELECT Count(*) 
                        FROM T_ABASTECIMENTO A
                        WHERE A.ABA_KM = ABA_KM AND A.VEI_CODIGO = VEI_CODIGO AND A.ABA_LITROS = ABA_LITROS AND A.ABA_SITUACAO <> 'G'
                        GROUP BY A.ABA_KM, A.VEI_CODIGO, A.ABA_LITROS HAVING Count(*) > 1), 0)", TypeType = typeof(long), Lazy = true)]
        public virtual long ContagemDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaCombustivel",
                        Formula = @"ISNULL((SELECT TOP(1)
                                    ((A.ABA_KM - (CASE
				                                    WHEN PP.PRO_DESCRICAO LIKE '%ARLA%' THEN
					                                    ISNULL((SELECT TOP(1) AA.ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_KM DESC), A.ABA_KM)
				                                    ELSE
					                                    ISNULL((SELECT TOP(1) AA.ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_KM DESC), A.ABA_KM)
			                                    END)) / CASE WHEN A.ABA_LITROS <= 0 THEN 1 ELSE A.ABA_LITROS END) 
                                    FROM T_ABASTECIMENTO A 
                                    JOIN T_PRODUTO PP ON PP.PRO_CODIGO = A.PRO_CODIGO
                                    WHERE A.ABA_CODIGO = ABA_CODIGO), 0)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal MediaCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaHorimetro",
                        Formula = @"(CASE WHEN (SELECT count(*) FROM T_ABASTECIMENTO AA WHERE ABA_HORIMETRO > AA.ABA_HORIMETRO AND EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G') > 0 and ABA_LITROS > 0 THEN

                                        CASE WHEN (ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                           WHERE ABA_HORIMETRO > AA.ABA_HORIMETRO AND EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0)) <= 0 THEN 0

                                        ELSE (ABA_LITROS / (ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                                    WHERE ABA_HORIMETRO > AA.ABA_HORIMETRO AND EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0))) END

                                    ELSE 0 END)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal MediaHorimetro { get; set; }

        public virtual string DataFormatadaEDI { get; set; }

        public virtual string ValorTotalEDI { get; set; }

        public virtual string LitrosEDI { get; set; }

        public virtual double CNPJPostoEDI { get; set; }

        public virtual string PlacaVeiculoEDI { get; set; }

        public virtual string CodigoProdutoEDI { get; set; }

        public virtual string DescricaoProdutoEDI { get; set; }

        public virtual string DescricaoPosto
        {
            get
            {
                if (this.Posto == null)
                    return this.NomePosto;
                else
                    return string.Concat(this.Posto.CPF_CNPJ_Formatado, " - ", this.Posto.Nome);
            }
        }

        public virtual string ValorLitroTicketLog
        {
            get
            {
                return AbastecimentoTicketLog?.ValorLitro ?? string.Empty;
            }
        }

        public virtual string ValorTotalTicketLog
        {
            get
            {
                return AbastecimentoTicketLog?.ValorTransacao ?? string.Empty;
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                return this.Litros * this.ValorUnitario;
            }
        }

        public virtual decimal ValorUnitarioMoedaEstrangeira
        {
            get
            {
                return this.Litros > 0 ? this.ValorOriginalMoedaEstrangeira / this.Litros : 0m;
            }
        }

        public virtual string DescricaoVeiculo
        {
            get
            {
                return this.Veiculo != null ? this.Veiculo.Placa : string.Empty;
            }
        }

        public virtual decimal KilometrosPorLitro
        {
            get
            {
                return this.Litros > 0 ? (this.Kilometragem - this.KilometragemAnterior) / this.Litros : 0;
            }
        }

        public virtual decimal LitrosPorKM
        {
            get
            {
                return this.Litros > 0 && this.Kilometragem > 0 ? this.Litros / (this.Kilometragem - this.KilometragemAnterior) : 0;
            }
        }

        public virtual decimal KilometrosRodados
        {
            get
            {
                return this.Kilometragem - this.KilometragemAnterior;
            }
        }

        public virtual bool Equals(Abastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoTipoAbastecimento
        {
            get { return TipoAbastecimento.ObterDescricao(); }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case "A":
                        return "Aberto";
                    case "I":
                        return "Inconsistente";
                    case "F":
                        return "Fechado";
                    case "G":
                        return "Agrupado";
                    case "R":
                        return "Requisição";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Placa + " - " + this.Litros.ToString("n2");
            }
        }

        public virtual int NumeroAcertoDeViagem
        {
            get
            {
                return this.AcertoDeViagem?.Numero ?? 0;
            }
        }
        public virtual Dominio.Entidades.Abastecimento Clonar()
        {
            return (Dominio.Entidades.Abastecimento)this.MemberwiseClone();
        }

        #endregion
    }
}
