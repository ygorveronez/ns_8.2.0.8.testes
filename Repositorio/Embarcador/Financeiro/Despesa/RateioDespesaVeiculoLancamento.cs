using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Financeiro.Despesa
{
    public class RateioDespesaVeiculoLancamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento>
    {
        public RateioDespesaVeiculoLancamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio> BuscarVeiculosParaRateio(IEnumerable<int> codigosVeiculos, IEnumerable<int> codigosSegmentosVeiculos, IEnumerable<int> codigosCentroResultado, DateTime dataInicial, DateTime dataFinal, bool ratearPeloPercentualFaturadoDoVeiculoNoPeriodo)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append($"declare @dataInicial datetime = '{dataInicial.ToString("yyyy-MM-dd")}', @dataFinal datetime = '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}';");
            sql.Append("SELECT Veiculo.VEI_CODIGO Codigo, Veiculo.CRE_CODIGO CodigoCentroResultado ");

            if (ratearPeloPercentualFaturadoDoVeiculoNoPeriodo)
            {
                sql.Append(@",  (ISNULL((SELECT SUM(CTe.CON_VALOR_RECEBER) 
                                FROM T_CTE CTe 
                                WHERE 
                                CTe.CON_TIPO_CTE <> 2 AND 
                                CTe.CON_STATUS = 'A' AND 
                                CTe.CON_DATAHORAEMISSAO >= @dataInicial AND 
                                CTe.CON_DATAHORAEMISSAO < @dataFinal AND 
                                exists (SELECT VeiculoCTe.CON_CODIGO FROM T_CTE_VEICULO VeiculoCTe WHERE VeiculoCTe.VEI_CODIGO = Veiculo.VEI_CODIGO AND VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO)), 0)
                                +
                                ISNULL((SELECT SUM(Pedido.PED_VALOR_FRETE_NEGOCIADO) 
                                FROM T_PEDIDO Pedido 
                                WHERE 
                                Pedido.PED_COLETA_EM_PRODUTOR_RURAL = 1 AND
                                Pedido.PED_DATA_INICIAL_COLETA >= @dataInicial AND 
                                Pedido.PED_DATA_INICIAL_COLETA < @dataFinal AND 
                                Pedido.PED_SITUACAO = 1 AND
                                exists (SELECT PedidoVeiculo.PED_CODIGO FROM T_PEDIDO_VEICULO PedidoVeiculo WHERE PedidoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO AND PedidoVeiculo.PED_CODIGO = Pedido.PED_CODIGO)), 0)) 
                                ValorReceber ");
            }

            sql.Append("FROM T_VEICULO Veiculo WHERE Veiculo.VEI_ATIVO = 1 ");

            if (ratearPeloPercentualFaturadoDoVeiculoNoPeriodo) //se rateia pelo faturamento, pega apenas as trações, pois não existe um valor de frete rateado por tração e reboque
                sql.Append("AND Veiculo.VEI_TIPOVEICULO = '0' ");

            if (codigosVeiculos?.Count() > 0)
                sql.Append($"AND Veiculo.VEI_CODIGO in ({string.Join(", ", codigosVeiculos)}) ");

            if (codigosSegmentosVeiculos?.Count() > 0)
                sql.Append($"AND Veiculo.VSE_CODIGO in ({string.Join(", ", codigosSegmentosVeiculos)}) ");

            if (codigosCentroResultado?.Count() > 0)
                sql.Append($"AND Veiculo.CRE_CODIGO in ({string.Join(", ", codigosCentroResultado)}) ");

            if (ratearPeloPercentualFaturadoDoVeiculoNoPeriodo)
            {
                sql.Append(@"AND (ISNULL((SELECT SUM(CTe.CON_VALOR_RECEBER) 
                                FROM T_CTE CTe 
                                WHERE 
                                CTe.CON_TIPO_CTE <> 2 AND 
                                CTe.CON_STATUS = 'A' AND 
                                CTe.CON_DATAHORAEMISSAO >= @dataInicial AND 
                                CTe.CON_DATAHORAEMISSAO < @dataFinal AND 
                                exists (SELECT VeiculoCTe.CON_CODIGO FROM T_CTE_VEICULO VeiculoCTe WHERE VeiculoCTe.VEI_CODIGO = Veiculo.VEI_CODIGO AND VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO)), 0)
                                +
                                ISNULL((SELECT SUM(Pedido.PED_VALOR_FRETE_NEGOCIADO) 
                                FROM T_PEDIDO Pedido 
                                WHERE 
                                Pedido.PED_COLETA_EM_PRODUTOR_RURAL = 1 AND
                                Pedido.PED_DATA_INICIAL_COLETA >= @dataInicial AND 
                                Pedido.PED_DATA_INICIAL_COLETA < @dataFinal AND 
                                Pedido.PED_SITUACAO = 1 AND
                                exists (SELECT PedidoVeiculo.PED_CODIGO FROM T_PEDIDO_VEICULO PedidoVeiculo WHERE PedidoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO AND PedidoVeiculo.PED_CODIGO = Pedido.PED_CODIGO)), 0)) > 0 ");
            }

            NHibernate.ISQLQuery query = SessionNHiBernate.CreateSQLQuery(sql.ToString());

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio)));

            return query.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio>();
        }

        public void DeletarPorRateioDespesaVeiculo(long codigoRateioDespesaVeiculo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RateioDespesaVeiculoLancamentoDia WHERE Codigo IN (SELECT c.Codigo FROM RateioDespesaVeiculoLancamentoDia c WHERE c.Lancamento.RateioDespesa.Codigo = :codigoRateioDespesaVeiculo)").SetInt64("codigoRateioDespesaVeiculo", codigoRateioDespesaVeiculo).SetTimeout(5000).ExecuteUpdate();
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM RateioDespesaVeiculoLancamento c WHERE c.RateioDespesa.Codigo = :codigoRateioDespesaVeiculo").SetInt64("codigoRateioDespesaVeiculo", codigoRateioDespesaVeiculo).SetTimeout(5000).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RateioDespesaVeiculoLancamentoDia WHERE Codigo IN (SELECT c.Codigo FROM RateioDespesaVeiculoLancamentoDia c WHERE c.Lancamento.RateioDespesa.Codigo = :codigoRateioDespesaVeiculo)").SetInt64("codigoRateioDespesaVeiculo", codigoRateioDespesaVeiculo).SetTimeout(5000).ExecuteUpdate();
                        UnitOfWork.Sessao.CreateQuery("DELETE FROM RateioDespesaVeiculoLancamento c WHERE c.RateioDespesa.Codigo = :codigoRateioDespesaVeiculo").SetInt64("codigoRateioDespesaVeiculo", codigoRateioDespesaVeiculo).SetTimeout(5000).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento> Consultar(long codigoRateioDespesaVeiculo, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento>();

            query = query.Where(o => o.RateioDespesa.Codigo == codigoRateioDespesaVeiculo);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Fetch(o => o.Veiculo).ThenFetch(o => o.SegmentoVeiculo).Fetch(o => o.CentroResultado).ToList();
        }

        public int ContarConsulta(long codigoRateioDespesaVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento>();

            query = query.Where(o => o.RateioDespesa.Codigo == codigoRateioDespesaVeiculo);

            return query.Count();
        }

        public decimal ObterTotalPorRateio(long codigoRateioDespesaVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento>();

            query = query.Where(o => o.RateioDespesa.Codigo == codigoRateioDespesaVeiculo);

            return query.Sum(o => o.Valor);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio> BuscarSomenteVeiculos(IEnumerable<int> codigosCentroResultado, IEnumerable<int> codigosVeiculos) {

            bool contentVeiculos = codigosVeiculos?.Count() > 0;
            bool contentCentroRestultado = codigosCentroResultado?.Count() > 0;
       
            StringBuilder sql = new StringBuilder();

            sql.Append($"SELECT Veiculo.VEI_CODIGO Codigo, Veiculo.CRE_CODIGO CodigoCentroResultado FROM T_VEICULO Veiculo WHERE Veiculo.VEI_ATIVO = 1 ");

            if (contentVeiculos)
                sql.Append($" AND Veiculo.VEI_CODIGO in ({string.Join(", ", codigosVeiculos)}) ");

            if (contentCentroRestultado)
                sql.Append($" AND Veiculo.CRE_CODIGO in ({string.Join(", ", codigosCentroResultado)}) ");


            NHibernate.ISQLQuery query = SessionNHiBernate.CreateSQLQuery(sql.ToString());

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio)));

            return query.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoRateio>();
        }

        #endregion
    }
}
