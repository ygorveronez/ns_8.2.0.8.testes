using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>
    {
        public CargaPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaPedidoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargasPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where codigoCargaPedidos.Contains(obj.CargaPedido.Codigo) select obj;

            return result
                .Fetch(obj => obj.Produto)
                .Fetch(obj => obj.JustificativaTemperatura)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            return result
                .Fetch(obj => obj.Produto)
                .Fetch(obj => obj.JustificativaTemperatura)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;

            return result
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.Unidade)
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.TipoEmbalagem)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ToList();
        }


        public (string CodigoNCM, string Descricao) BuscarNCMProdutoMaiorValorPorCarga(int codigoCarga)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                                    .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.Produto.CodigoNCM != null && obj.Produto.CodigoNCM != "")
                                    .OrderByDescending(o => o.Quantidade * o.ValorUnitarioProduto)
                                    .WithOptions(o => o.SetTimeout(600))
                                    .Select(x => ValueTuple.Create(
                                        x.Produto.CodigoNCM,
                                        x.Produto.Descricao
                                    ))?.FirstOrDefault() ?? (string.Empty, string.Empty);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargaSemFetch(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto BuscarPrimeiroPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;

            return result
                .Fetch(obj => obj.Produto)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargaPaginado(int codigoCarga, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && (!obj.CargaPedido.PedidoSemNFe || obj.CargaPedido.Pedido.CanalEntrega.LiberarPedidoSemNFeAutomaticamente) select obj;

            return result
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.Unidade)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query
                         where obj.CargaPedido.Pedido.Codigo == pedido
       && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
       && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
       && !obj.CargaPedido.ReentregaSolicitada
       && obj.CargaPedido.CargaPedidoTrechoAnterior == null
                         select obj;

            return result
                .Fetch(obj => obj.Produto)
                .ThenFetch(obj => obj.Unidade)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargas(List<int> codigosCargas)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            int inicioRegistros = 0;
            int limiteRegistros = 1000;

            while (inicioRegistros < codigosCargas?.Count)
            {
                List<int> codigosCargasPorIntervalo = codigosCargas.Skip(inicioRegistros).Take(limiteRegistros).ToList();

                var consultaCargaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                    .Where(cargaPedidoProduto => codigosCargasPorIntervalo.Contains(cargaPedidoProduto.CargaPedido.Carga.Codigo));

                cargaPedidoProdutos
                    .AddRange(
                        consultaCargaPedidoProduto
                            .Fetch(obj => obj.Produto).ThenFetch(obj => obj.Unidade)
                            .Fetch(obj => obj.CargaPedido).ThenFetch(obj => obj.Pedido)
                            .Fetch(obj => obj.Produto).ThenFetch(obj => obj.GrupoProduto)
                            .ToList()
                    );

                inicioRegistros += limiteRegistros;
            }

            return cargaPedidoProdutos;
        }

        public List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> BuscarDadosPorCargas(List<int> codigosCargas)
        {
            List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)> cargaPedidoProdutos = new List<(int CodigoCarga, int CodigoCargaPedido, string CodigoProdutoEmbarcador, string DescricaoProduto, decimal Quantidade)>();
            int inicioRegistros = 0;
            int limiteRegistros = 1000;

            while (inicioRegistros < codigosCargas?.Count)
            {
                List<int> codigosCargasPorIntervalo = codigosCargas.Skip(inicioRegistros).Take(limiteRegistros).ToList();

                var consultaCargaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                    .Where(cargaPedidoProduto => codigosCargasPorIntervalo.Contains(cargaPedidoProduto.CargaPedido.Carga.Codigo));

                cargaPedidoProdutos
                    .AddRange(
                        consultaCargaPedidoProduto
                            .Select(cargaPedidoProduto => ValueTuple.Create(
                                cargaPedidoProduto.CargaPedido.Carga.Codigo,
                                cargaPedidoProduto.CargaPedido.Codigo,
                                cargaPedidoProduto.Produto.CodigoProdutoEmbarcador,
                                cargaPedidoProduto.Produto.Descricao,
                                cargaPedidoProduto.Quantidade
                            ))
                            .ToList()
                    );

                inicioRegistros += limiteRegistros;
            }

            return cargaPedidoProdutos;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorCargaParaImpressaoSintese(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga && obj.Produto.GrupoProduto.ListarProdutosDesteGrupoNoRelatorioDeSinteseDeMateriaisDoPatio == true && obj.PesoUnitario > 0);

            return query.ToList();
        }

        public List<int> BuscarCodigosProdutosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj.Produto.Codigo;

            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigosProdutosPorCargaPedido(int codigoCargaPedido)
        {
            var consultaCargaPedidoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return consultaCargaPedidoProduto
                .Select(o => o.Produto.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            query = query.Where(o => o.CargaPedido.Carga.Protocolo == protocoloCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto BuscarPorPedidoProduto(int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.Codigo == codigoPedidoProduto select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCargaPedido(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();

            var result = from obj in query where obj.CargaPedido.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> BuscarGruposDeProdutosDaCarga(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codCarga select obj.Produto.GrupoProduto;
            return result.Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>> BuscarGruposDeProdutosDaCargaAsync(int codCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codCarga select obj.Produto.GrupoProduto;
            return result.Distinct().ToListAsync();
        }

        public decimal ObterValorTotalPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            return result.Sum(obj => obj.ValorUnitarioProduto * obj.Quantidade);
        }

        public Task<decimal> ObterValorTotalPorCargaPedidosAsync(List<int> codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where codigoCargaPedido.Contains(obj.CargaPedido.Codigo) select obj;
            return result.SumAsync(obj => obj.ValorUnitarioProduto * obj.Quantidade);
        }

        public decimal ObterQuantidadeTotalPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            return result.Sum(obj => obj.Quantidade);
        }

        public Task<decimal> ObterQuantidadeTotalPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga select obj;
            return result.SumAsync(obj => obj.Quantidade);
        }

        public void DeletarPorCargaPedido(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao
                        .CreateQuery("DELETE CargaPedidoProduto obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido")
                        .SetInt32("CodigoCargaPedido", codigoCargaPedido)
                        .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao
                            .CreateQuery("DELETE CargaPedidoProduto obj WHERE obj.CargaPedido.Codigo = :CodigoCargaPedido")
                            .SetInt32("CodigoCargaPedido", codigoCargaPedido)
                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        private string ObterSelectConsultaRelatorioProdutosExpedidos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool count, DateTime dataInicial, DateTime dataFinal, int produto, int grupoProduto, int unidadeMedida, int empresa, List<int> codigosFilial, List<double> codigosRecebedor, int rota, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, List<int> codigosTipoOperacao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            SetarWhereRelatorioConsultaProdutosExpedidos(ref where, ref groupBy, ref joins, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, empresa, codigosFilial, codigosRecebedor, rota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao);

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaProdutosExpedidos(propriedades[i].Propriedade, ref select, ref groupBy, ref joins, count);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaProdutosExpedidos(propAgrupa, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CARGA_PEDIDO_PRODUTO as CargaPedidoProduto " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");

        }

        private void SetarWhereRelatorioConsultaProdutosExpedidos(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int produto, int grupoProduto, int unidadeMedida, int empresa, List<int> codigosFilial, List<double> codigosRecebedor, int rota, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, List<int> codigosTipoOperacao)
        {
            if (!joins.Contains(" CargaPedido "))
                joins += "inner join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO ";

            if (!joins.Contains(" Carga "))
                joins += "inner join T_CARGA as Carga on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ";

            where += " and Carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 ";

            if (dataInicial != DateTime.MinValue)
                where += " and Carga.CAR_DATA_CARREGAMENTO >= '" + dataInicial.ToString("yyyy-MM-dd") + "'";


            if (dataFinal != DateTime.MinValue)
                where += " and Carga.CAR_DATA_CARREGAMENTO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (produto > 0)
            {
                where += " and ProdutoEmbarcador.GRP_CODIGO = " + produto.ToString();

                if (!joins.Contains(" ProdutoEmbarcador "))
                    joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
            }

            if (grupoProduto > 0)
            {
                where += " and GrupoProduto.GPR_CODIGO = " + grupoProduto.ToString();

                if (!joins.Contains(" ProdutoEmbarcador "))
                    joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";

                if (!joins.Contains(" GrupoProduto "))
                    joins += "left join T_GRUPO_PRODUTO AS GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ";
            }

            if (unidadeMedida > 0)
            {
                where += " and UnidadeMedida.UNI_CODIGO = " + unidadeMedida.ToString();

                if (!joins.Contains(" ProdutoEmbarcador "))
                    joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";

                if (!joins.Contains(" UnidadeMedida "))
                    joins += "left join T_UNIDADE_MEDIDA as UnidadeMedida on UnidadeMedida.UNI_CODIGO = ProdutoEmbarcador.UNI_CODIGO ";
            }

            if (empresa > 0)
                where += " and Carga.EMP_CODIGO = " + empresa.ToString();


            if (codigosFilial.Any(codigo => codigo == -1))
            {
                where += ($@" and (Carga.FIL_CODIGO in ({string.Join(", ", codigosFilial)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", codigosRecebedor)})))");
            }
            else if (codigosFilial?.Count > 0)
                where += $" and Carga.FIL_CODIGO in ({string.Join(", ", codigosFilial)})";

            if (rota > 0)
                where += " and Carga.ROF_CODIGO = " + rota.ToString();

            if (codigoOrigem > 0)
                where += " and CargaPedido.LOC_CODIGO_ORIGEM = " + codigoOrigem.ToString();

            if (codigoDestino > 0)
                where += " and CargaPedido.LOC_CODIGO_DESTINO = " + codigoDestino.ToString();

            if (codigosTipoCarga?.Count > 0)
                where += $" and (Carga.TCG_CODIGO in ({string.Join(", ", codigosTipoCarga)}){(codigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})";

            if (codigosTipoOperacao?.Count > 0)
                where += $" and (Carga.TOP_CODIGO in ({string.Join(", ", codigosTipoOperacao)}){(codigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})";

            if (cpfCnpjRemetente > 0)
            {
                where += " and Pedido.CLI_CODIGO_REMETENTE = " + cpfCnpjRemetente.ToString();

                if (!joins.Contains(" Pedido "))
                    joins += "inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ";
            }

            if (cpfCnpjDestinatario > 0)
            {
                where += " and Pedido.CLI_CODIGO = " + cpfCnpjDestinatario.ToString();

                if (!joins.Contains(" Pedido "))
                    joins += "inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ";
            }
        }

        private void SetarSelectRelatorioConsultaProdutosExpedidos(string propriedade, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto"))
                    {
                        select += "ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR as CodigoProduto, ";
                        groupBy += "ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ";
                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto"))
                    {
                        select += "ProdutoEmbarcador.GRP_DESCRICAO as Produto, ";
                        groupBy += "ProdutoEmbarcador.GRP_DESCRICAO, ";
                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
                    }
                    break;
                case "CodigoGrupoProduto":
                    if (!select.Contains(" CodigoGrupoProduto"))
                    {
                        select += "GrupoProduto.GRP_CODIGO_GRUPO_PRODUTO_EMBARCADOR as CodigoGrupoProduto, ";
                        groupBy += "GrupoProduto.GRP_CODIGO_GRUPO_PRODUTO_EMBARCADOR, ";

                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";

                        if (!joins.Contains(" GrupoProduto "))
                            joins += "left join T_GRUPO_PRODUTO AS GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ";
                    }
                    break;
                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto"))
                    {
                        select += "GrupoProduto.GRP_DESCRICAO as GrupoProduto, ";
                        groupBy += "GrupoProduto.GRP_DESCRICAO, ";

                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";

                        if (!joins.Contains(" GrupoProduto "))
                            joins += "left join T_GRUPO_PRODUTO AS GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ";
                    }
                    break;
                case "UnidadeDeMedida":
                    if (!select.Contains(" UnidadeDeMedida"))
                    {
                        select += "UnidadeMedida.UNI_DESCRICAO as UnidadeDeMedida, ";
                        groupBy += "UnidadeMedida.UNI_DESCRICAO, ";

                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";

                        if (!joins.Contains(" UnidadeMedida "))
                            joins += "left join T_UNIDADE_MEDIDA as UnidadeMedida on UnidadeMedida.UNI_CODIGO = ProdutoEmbarcador.UNI_CODIGO ";
                    }
                    break;
                case "Filial":
                    if (!select.Contains(" Filial"))
                    {
                        select += "Filial.FIL_DESCRICAO as Filial, ";
                        groupBy += "Filial.FIL_DESCRICAO, ";

                        if (!joins.Contains(" CargaPedido "))
                            joins += "inner join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoProduto.CPE_CODIGO ";

                        if (!joins.Contains(" Carga "))
                            joins += "inner join T_CARGA as Carga on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Filial "))
                            joins += "inner join T_FILIAL as Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ";
                    }
                    break;
                case "Quantidade":
                    if (!select.Contains(" Quantidade"))
                        select += "sum(CargaPedidoProduto.CPP_QUANTIDADE) as Quantidade, ";
                    break;

                case "PesoUnitarioProduto":
                    if (!select.Contains(" PesoUnitarioProduto, "))
                    {
                        select += "ProdutoEmbarcador.PRO_PESO_UNITARIO as PesoUnitarioProduto, ";
                        groupBy += "ProdutoEmbarcador.PRO_PESO_UNITARIO, ";
                        if (!joins.Contains(" ProdutoEmbarcador "))
                            joins += "inner join T_PRODUTO_EMBARCADOR as ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
                    }
                    break;

                case "PesoTotal":
                    SetarSelectRelatorioConsultaProdutosExpedidos("PesoUnitarioProduto", ref select, ref groupBy, ref joins, count);
                    SetarSelectRelatorioConsultaProdutosExpedidos("Quantidade", ref select, ref groupBy, ref joins, count);
                    break;
            }
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Expedicao.ExpedicaoProduto> ConsultarRelatorioProdutosExpedidos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int produto, int grupoProduto, int unidadeMedida, int empresa, List<int> codigosFilial, List<double> codigosRecebedor, int rota, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, List<int> codigosTipoOperacao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioProdutosExpedidos(propriedades, false, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, empresa, codigosFilial, codigosRecebedor, rota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Expedicao.ExpedicaoProduto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Expedicao.ExpedicaoProduto>();

        }

        public int ContarConsultarRelatorioProdutosExpedidos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int produto, int grupoProduto, int unidadeMedida, int empresa, List<int> codigosFilial, List<double> codigosRecebedor, int rota, int codigoOrigem, int codigoDestino, List<int> codigosTipoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, List<int> codigosTipoOperacao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioProdutosExpedidos(propriedades, true, dataInicial, dataFinal, produto, grupoProduto, unidadeMedida, empresa, codigosFilial, codigosRecebedor, rota, codigoOrigem, codigoDestino, codigosTipoCarga, cpfCnpjRemetente, cpfCnpjDestinatario, codigosTipoOperacao, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido> ConsultarGraficoProdutosExpedidos(DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectGraficoRelatorioProdutosExpedidos(dataInicial, dataFinal, codigoCentroCarregamento));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido>();

        }

        private string ObterSelectGraficoRelatorioProdutosExpedidos(DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            SetarWhereRelatorioGraficoProdutosExpedidos(ref where, ref groupBy, ref joins, dataInicial, dataFinal, codigoCentroCarregamento);

            select += "ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR as CodigoProduto, ";
            select += "ProdutoEmbarcador.GRP_DESCRICAO as Produto, ";
            select += "sum(case when Carga.CAR_SITUACAO < 7 then CargaPedidoProduto.CPP_QUANTIDADE else 0 end) QuantidadeAExpedir, ";
            select += "sum(case when Carga.CAR_SITUACAO > 6 then CargaPedidoProduto.CPP_QUANTIDADE else 0 end) QuantidadeExpedida, ";
            select += "sum(CargaPedidoProduto.CPP_QUANTIDADE) as QuantidadeTotal ";

            joins += "inner join T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
            joins += "inner join T_CARGA_PEDIDO CargaPedido on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO ";
            joins += "inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ";
            joins += "inner join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on  CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ";

            groupBy += " ProdutoEmbarcador.GRP_DESCRICAO, ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR ";

            orderBy += " QuantidadeTotal desc ";

            return ("select " + select + // SQL-INJECTION-SAFE
                   " from T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto " + joins +
                   " where 1=1" + where) +
                   (groupBy.Length > 0 ? " group by " + groupBy : string.Empty) +
                   (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 desc ");

        }

        private void SetarWhereRelatorioGraficoProdutosExpedidos(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {

            where += " and Carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 and ProdutoEmbarcador.PRO_EXIBIR_EXPEDICAO_TEMPO_REAL = 1";

            if (dataInicial != DateTime.MinValue)
                where += " and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '" + dataInicial.ToString("yyyy-MM-dd") + "'";


            if (dataFinal != DateTime.MinValue)
                where += " and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigoCentroCarregamento > 0)
            {
                where += " and CargaJanelaCarregamento.CEC_CODIGO = " + codigoCentroCarregamento.ToString();

            }
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido> ConsultarGraficoUnidadeMedida(DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectGraficoRelatorioUnidadeMedida(dataInicial, dataFinal, codigoCentroCarregamento));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProdutoExpedido>();

        }

        private string ObterSelectGraficoRelatorioUnidadeMedida(DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            SetarWhereRelatorioGraficoUnidadeMedida(ref where, ref groupBy, ref joins, dataInicial, dataFinal, codigoCentroCarregamento);

            select += "UnidadeMedida.UNI_SIGLA as CodigoProduto, ";
            select += "UnidadeMedida.UNI_DESCRICAO as Produto, ";
            select += "sum(case when Carga.CAR_SITUACAO < 7 then CargaPedidoProduto.CPP_QUANTIDADE else 0 end) QuantidadeAExpedir, ";
            select += "sum(case when Carga.CAR_SITUACAO > 6 then CargaPedidoProduto.CPP_QUANTIDADE else 0 end) QuantidadeExpedida, ";
            select += "sum(CargaPedidoProduto.CPP_QUANTIDADE) as QuantidadeTotal ";

            joins += "inner join T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ";
            joins += "inner join T_CARGA_PEDIDO CargaPedido on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO ";
            joins += "inner join T_UNIDADE_MEDIDA UnidadeMedida on UnidadeMedida.UNI_CODIGO = ProdutoEmbarcador.UNI_CODIGO ";
            joins += "inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ";
            joins += "inner join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on  CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ";

            groupBy += " UnidadeMedida.UNI_DESCRICAO, UnidadeMedida.UNI_SIGLA ";

            orderBy += " QuantidadeTotal desc ";

            return ("select " + select + // SQL-INJECTION-SAFE
                   " from T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto " + joins +
                   " where 1=1" + where) +
                   (groupBy.Length > 0 ? " group by " + groupBy : string.Empty) +
                   (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 desc ");

        }

        private void SetarWhereRelatorioGraficoUnidadeMedida(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int codigoCentroCarregamento)
        {

            where += " and Carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 and UnidadeMedida.UNI_EXIBIR_EXPEDICAO_TEMPO_REAL = 1";

            if (dataInicial != DateTime.MinValue)
                where += " and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '" + dataInicial.ToString("yyyy-MM-dd") + "'";


            if (dataFinal != DateTime.MinValue)
                where += " and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigoCentroCarregamento > 0)
            {
                where += " and CargaJanelaCarregamento.CEC_CODIGO = " + codigoCentroCarregamento.ToString();

            }
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto> ConsultarRelatorioCargasProdutos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargasProdutos = new ConsultaCargaProduto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargasProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto)));

            return consultaCargasProdutos.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto>();
        }

        public async Task<List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto>> ConsultarRelatorioCargasProdutosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargasProdutos = new ConsultaCargaProduto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargasProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto)));

            return (List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto>)await consultaCargasProdutos.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto>();
        }

        public int ContarConsultaRelatorioCargasProdutos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCargasProdutos = new ConsultaCargaProduto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCargasProdutos.SetTimeout(600).UniqueResult<int>();
        }

        public async Task<int> ContarConsultaRelatorioCargasProdutosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCargasProdutos = new ConsultaCargaProduto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consultaCargasProdutos.SetTimeout(600).UniqueResultAsync<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProdutoTransportador> ConsultarRelatorioCargasProdutosTransportador(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoTransportador)
        {
            var consultaCargasProdutos = new ConsultaCargaProdutoTransportador(tipoServicoMultisoftware, codigoTransportador).ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargasProdutos.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProdutoTransportador)));

            return consultaCargasProdutos.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProdutoTransportador>();
        }

        public int ContarConsultaRelatorioCargasProdutosTransportador(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProdutoTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoTransportador)
        {
            var consultaCargasProdutos = new ConsultaCargaProdutoTransportador(tipoServicoMultisoftware, codigoTransportador).ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCargasProdutos.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador BuscarProdutoComRegraICMS(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> queryRegraIcmsProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            return query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido &&
                                    queryRegraIcmsProduto.Any(regra => regra.Ativo && regra.ProdutosEmbarcador.Any(prod => prod == o.Produto)))
                        .Select(o => o.Produto)
                        .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> BuscarProdutoComRegraICMSPorCarga(int carga, List<int> produtos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga && produtos.Contains(obj.Produto.Codigo) select obj;
            return result.ToList();
        }

        /// <summary>
        /// Método utilizado específicamente na geracao da carga, por favor não utilizar.
        /// </summary>
        /// <param name="ObjetoMontagemCargaSqlPedidoProduto"></param>
        public void InsertSQL(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto)
        {
            if (ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto != null && ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Count > 0)
            {
                int take = 230;
                int start = 0;
                //tem q executar de 230 em 230 pois o SQLServer só executa 2100 parametros; 230 * 9 = 2070
                while (start < ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Count)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto> listaTemp = ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Skip(start).Take(take).ToList();

                    string parameros = "( :CPE_CODIGO_[X], :PRO_CODIGO_[X], :CPP_PESO_UNITARIO_[X], :CPP_QUANTIDADE_[X], :CPP_QUANTIDADE_CAIXA_[X], :CPP_QUANTIDADE_CAIXAS_VAZIAS_[X], :CPP_QUANTIDADE_PLANEJADA_[X], :CPP_PESO_TOTAL_EMBALAGEM_[X], :CPP_VALOR_UNITARIO_[X] )";
                    string sqlQuery = @"
                        INSERT INTO T_CARGA_PEDIDO_PRODUTO ( CPE_CODIGO, PRO_CODIGO, CPP_PESO_UNITARIO, CPP_QUANTIDADE, CPP_QUANTIDADE_CAIXA, CPP_QUANTIDADE_CAIXAS_VAZIAS, CPP_QUANTIDADE_PLANEJADA, CPP_PESO_TOTAL_EMBALAGEM, CPP_VALOR_UNITARIO ) values " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < listaTemp.Count; i++)
                        sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                    query.SetParameter("CPE_CODIGO_0", listaTemp[0].codigoCargaPedido);
                    query.SetParameter("PRO_CODIGO_0", listaTemp[0].codigoProduto);
                    query.SetParameter("CPP_PESO_UNITARIO_0", listaTemp[0].PesoUnitario);
                    query.SetParameter("CPP_QUANTIDADE_0", listaTemp[0].Quantidade);
                    query.SetParameter("CPP_QUANTIDADE_CAIXA_0", listaTemp[0].QuantidadeCaixa);
                    query.SetParameter("CPP_QUANTIDADE_CAIXAS_VAZIAS_0", listaTemp[0].QuantidadeCaixasVazias);
                    query.SetParameter("CPP_QUANTIDADE_PLANEJADA_0", listaTemp[0].QuantidadePlanejada);
                    query.SetParameter("CPP_PESO_TOTAL_EMBALAGEM_0", listaTemp[0].PesoTotalEmbalagem);
                    query.SetParameter("CPP_VALOR_UNITARIO_0", listaTemp[0].ValorProduto);

                    for (int i = 1; i < listaTemp.Count; i++)
                    {
                        //quando um valor pode ser nulo usar assim
                        //valor = DBNull.Value;
                        //if (!string.IsNullOrEmpty(produtosPedido[i].PROPRIEDADE))
                        //    valor = produtosPedido[i].PROPRIEDADE;
                        //query.SetParameter("PROP_" + i.ToString(), valor);

                        query.SetParameter("CPE_CODIGO_" + i.ToString(), listaTemp[i].codigoCargaPedido);
                        query.SetParameter("PRO_CODIGO_" + i.ToString(), listaTemp[i].codigoProduto);
                        query.SetParameter("CPP_PESO_UNITARIO_" + i.ToString(), listaTemp[i].PesoUnitario);
                        query.SetParameter("CPP_QUANTIDADE_" + i.ToString(), listaTemp[i].Quantidade);
                        query.SetParameter("CPP_QUANTIDADE_CAIXA_" + i.ToString(), listaTemp[i].QuantidadeCaixa);
                        query.SetParameter("CPP_QUANTIDADE_CAIXAS_VAZIAS_" + i.ToString(), listaTemp[i].QuantidadeCaixasVazias);
                        query.SetParameter("CPP_QUANTIDADE_PLANEJADA_" + i.ToString(), listaTemp[i].QuantidadePlanejada);
                        query.SetParameter("CPP_PESO_TOTAL_EMBALAGEM_" + i.ToString(), listaTemp[i].PesoTotalEmbalagem);
                        query.SetParameter("CPP_VALOR_UNITARIO_" + i.ToString(), listaTemp[i].ValorProduto);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }

        }

        /// <summary>
        /// Método utilizado específicamente na geracao da carga, por favor não utilizar.
        /// </summary>
        /// <param name="ObjetoMontagemCargaSqlPedidoProduto"></param>
        public async Task InsertSQLAsync(Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto)
        {
            if (ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto != null && ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Count > 0)
            {
                int take = 230;
                int start = 0;

                //tem q executar de 230 em 230 pois o SQLServer só executa 2100 parametros; 230 * 9 = 2070
                while (start < ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Count)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto> listaTemp = ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Skip(start).Take(take).ToList();

                    string parameros = "( :CPE_CODIGO_[X], :PRO_CODIGO_[X], :CPP_PESO_UNITARIO_[X], :CPP_QUANTIDADE_[X], :CPP_QUANTIDADE_CAIXA_[X], :CPP_QUANTIDADE_CAIXAS_VAZIAS_[X], :CPP_QUANTIDADE_PLANEJADA_[X], :CPP_PESO_TOTAL_EMBALAGEM_[X], :CPP_VALOR_UNITARIO_[X] )";
                    string sqlQuery = @"
                        INSERT INTO T_CARGA_PEDIDO_PRODUTO ( CPE_CODIGO, PRO_CODIGO, CPP_PESO_UNITARIO, CPP_QUANTIDADE, CPP_QUANTIDADE_CAIXA, CPP_QUANTIDADE_CAIXAS_VAZIAS, CPP_QUANTIDADE_PLANEJADA, CPP_PESO_TOTAL_EMBALAGEM, CPP_VALOR_UNITARIO ) values " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < listaTemp.Count; i++)
                        sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                    query.SetParameter("CPE_CODIGO_0", listaTemp[0].codigoCargaPedido);
                    query.SetParameter("PRO_CODIGO_0", listaTemp[0].codigoProduto);
                    query.SetParameter("CPP_PESO_UNITARIO_0", listaTemp[0].PesoUnitario);
                    query.SetParameter("CPP_QUANTIDADE_0", listaTemp[0].Quantidade);
                    query.SetParameter("CPP_QUANTIDADE_CAIXA_0", listaTemp[0].QuantidadeCaixa);
                    query.SetParameter("CPP_QUANTIDADE_CAIXAS_VAZIAS_0", listaTemp[0].QuantidadeCaixasVazias);
                    query.SetParameter("CPP_QUANTIDADE_PLANEJADA_0", listaTemp[0].QuantidadePlanejada);
                    query.SetParameter("CPP_PESO_TOTAL_EMBALAGEM_0", listaTemp[0].PesoTotalEmbalagem);
                    query.SetParameter("CPP_VALOR_UNITARIO_0", listaTemp[0].ValorProduto);

                    for (int i = 1; i < listaTemp.Count; i++)
                    {
                        query.SetParameter("CPE_CODIGO_" + i.ToString(), listaTemp[i].codigoCargaPedido);
                        query.SetParameter("PRO_CODIGO_" + i.ToString(), listaTemp[i].codigoProduto);
                        query.SetParameter("CPP_PESO_UNITARIO_" + i.ToString(), listaTemp[i].PesoUnitario);
                        query.SetParameter("CPP_QUANTIDADE_" + i.ToString(), listaTemp[i].Quantidade);
                        query.SetParameter("CPP_QUANTIDADE_CAIXA_" + i.ToString(), listaTemp[i].QuantidadeCaixa);
                        query.SetParameter("CPP_QUANTIDADE_CAIXAS_VAZIAS_" + i.ToString(), listaTemp[i].QuantidadeCaixasVazias);
                        query.SetParameter("CPP_QUANTIDADE_PLANEJADA_" + i.ToString(), listaTemp[i].QuantidadePlanejada);
                        query.SetParameter("CPP_PESO_TOTAL_EMBALAGEM_" + i.ToString(), listaTemp[i].PesoTotalEmbalagem);
                        query.SetParameter("CPP_VALOR_UNITARIO_" + i.ToString(), listaTemp[i].ValorProduto);
                    }

                    await query.ExecuteUpdateAsync();
                    start += take;
                }
            }

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioTotalizador> RelatorioRomanaeioTotalizador(int codigoCarga)
        {
            string query = $@"SELECT 
                                filial.FIL_DESCRICAO AS Filial,
                                empresa.EMP_RAZAO AS Transportador,
                                veiculo.VEI_PLACA AS Veiculo,
                                funcionario.FUN_NOME AS Motorista,
                                carga.CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga,
                                carga.CAR_DATA_CARREGAMENTO AS DataCarregamento,
                                produto.PRO_CODIGO_PRODUTO_EMBARCADOR AS ProdutoCodigo, 
                                produto.GRP_DESCRICAO AS ProdutoDescricao, 
                                SUM(cargaPedidoProduto.CPP_QUANTIDADE) AS ProdutoQuantidade
                            FROM 
                                T_CARGA_PEDIDO_PRODUTO cargaPedidoProduto
                                JOIN T_PRODUTO_EMBARCADOR produto ON cargaPedidoProduto.PRO_CODIGO = produto.PRO_CODIGO
                                JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedidoProduto.CPE_CODIGO = cargaPedido.CPE_CODIGO
                                JOIN T_CARGA carga ON cargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                                JOIN T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                JOIN T_FILIAL filial ON carga.FIL_CODIGO = filial.FIL_CODIGO
                                LEFT JOIN T_EMPRESA empresa ON carga.EMP_CODIGO = empresa.EMP_CODIGO
                                LEFT JOIN T_CARGA_MOTORISTA cargaMotorista ON carga.CAR_CODIGO = cargaMotorista.CAR_CODIGO
                                LEFT JOIN T_FUNCIONARIO funcionario ON cargaMotorista.CAR_MOTORISTA = funcionario.FUN_CODIGO
                                LEFT JOIN T_VEICULO veiculo ON carga.CAR_VEICULO = veiculo.VEI_CODIGO
                                JOIN T_CLIENTE cliente ON pedido.CLI_CODIGO = cliente.CLI_CGCCPF
                                JOIN T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido ON cargaPedido.CPE_CODIGO = cargaEntregaPedido.CPE_CODIGO
                                JOIN T_CARGA_ENTREGA cargaEntrega ON cargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                            WHERE 
                                cargaPedido.CAR_CODIGO = {codigoCarga}
                            GROUP BY 
                                produto.PRO_CODIGO_PRODUTO_EMBARCADOR, 
                                produto.GRP_DESCRICAO, 
                                filial.FIL_DESCRICAO,
                                empresa.EMP_RAZAO,
                                carga.CAR_CODIGO_CARGA_EMBARCADOR,
                                carga.CAR_DATA_CARREGAMENTO,
                                funcionario.FUN_NOME,
                                veiculo.VEI_PLACA;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioTotalizador)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioTotalizador>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.DetalhesRomaneioDetalhado> DetalhesRomaneioDetalhado(int codigoCarga)
        {
            string query = $@"SELECT
                                 filial.FIL_DESCRICAO AS Filial,
                                 empresa.EMP_RAZAO AS Transportador,
                                 veiculo.VEI_PLACA AS Veiculo,
                                 funcionario.FUN_NOME AS Motorista,
                                 carga.CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga,
                                 FORMAT(carga.CAR_DATA_CARREGAMENTO, 'dd/MM/yyyy HH:mm') AS DataCarregamento,
                                pedido.PED_CODIGO AS PedidoCodigo,
                                CAST(ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS INT)  AS SequenciaCarregamento,
                                pedido.PED_NUMERO_PEDIDO_EMBARCADOR AS PedidoNumero,
                                 cliente.CLI_NOME AS Destinatario,
                                CONCAT(cliente.CLI_ENDERECO, ', ', cliente.CLI_NUMERO, ', ', cliente.CLI_COMPLEMENTO, ', ', cliente.CLI_BAIRRO) AS Endereco
                             FROM
                                 T_CARGA_PEDIDO cargaPedido
                                 JOIN T_CARGA carga ON cargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                                 JOIN T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO
                                 JOIN T_FILIAL filial ON carga.FIL_CODIGO = filial.FIL_CODIGO
                                 LEFT JOIN T_EMPRESA empresa ON carga.EMP_CODIGO = empresa.EMP_CODIGO
                                 LEFT JOIN T_CARGA_MOTORISTA cargaMotorista ON carga.CAR_CODIGO = cargaMotorista.CAR_CODIGO
                                 LEFT JOIN T_FUNCIONARIO funcionario ON cargaMotorista.CAR_MOTORISTA = funcionario.FUN_CODIGO
                                 LEFT JOIN T_VEICULO veiculo ON carga.CAR_VEICULO = veiculo.VEI_CODIGO
                                 JOIN T_CLIENTE cliente ON pedido.CLI_CODIGO = cliente.CLI_CGCCPF
                                 JOIN T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido ON cargaPedido.CPE_CODIGO = cargaEntregaPedido.CPE_CODIGO
                                 JOIN T_CARGA_ENTREGA cargaEntrega ON cargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                             WHERE
                                 cargaPedido.CAR_CODIGO = {codigoCarga}
                                 and cargaEntrega.CAR_CODIGO = {codigoCarga}
                             ORDER BY
                                cargaEntrega.CEN_ORDEM desc;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.DetalhesRomaneioDetalhado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.DetalhesRomaneioDetalhado>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioDetalhadoResumido> RomaneioDetalhadoResumido(int codigoCarga)
        {
            string query = $@"SELECT
                                    filial.FIL_DESCRICAO AS Filial,
                                    empresa.EMP_RAZAO AS Transportador,
                                    veiculo.VEI_PLACA AS Veiculo,
                                    funcionario.FUN_NOME AS Motorista,
                                    carga.CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga,
                                    FORMAT(carga.CAR_DATA_CARREGAMENTO, 'dd/MM/yyyy HH:mm') AS DataCarregamento,
                                    STRING_AGG(pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ', ') WITHIN GROUP (ORDER BY pedido.PED_NUMERO_PEDIDO_EMBARCADOR) AS PedidoNumero,
                                    MAX(pedido.PED_CODIGO) AS PedidoCodigo,
                                    COALESCE(clienteRecebedor.CLI_NOME, cliente.CLI_NOME) AS Destinatario,
                                    CAST(ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS INT) AS SequenciaCarregamento,
                                    CONCAT(
                                        COALESCE(clienteRecebedor.CLI_ENDERECO, cliente.CLI_ENDERECO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_NUMERO, cliente.CLI_NUMERO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_COMPLEMENTO, cliente.CLI_COMPLEMENTO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_BAIRRO, cliente.CLI_BAIRRO)
                                    ) AS Endereco,
                                    COALESCE(clienteRecebedor.CLI_CGCCPF, cliente.CLI_CGCCPF) AS CodigoDestinatario
                                FROM
                                    T_CARGA_PEDIDO cargaPedido
                                    JOIN T_CARGA carga ON cargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                                    JOIN T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO
                                    JOIN T_FILIAL filial ON carga.FIL_CODIGO = filial.FIL_CODIGO
                                    LEFT JOIN T_EMPRESA empresa ON carga.EMP_CODIGO = empresa.EMP_CODIGO
                                    LEFT JOIN T_CARGA_MOTORISTA cargaMotorista ON carga.CAR_CODIGO = cargaMotorista.CAR_CODIGO
                                    LEFT JOIN T_FUNCIONARIO funcionario ON cargaMotorista.CAR_MOTORISTA = funcionario.FUN_CODIGO
                                    LEFT JOIN T_VEICULO veiculo ON carga.CAR_VEICULO = veiculo.VEI_CODIGO
                                    JOIN T_CLIENTE cliente ON pedido.CLI_CODIGO = cliente.CLI_CGCCPF
                                    LEFT JOIN T_CLIENTE clienteRecebedor ON cargaPedido.CLI_CODIGO_RECEBEDOR = clienteRecebedor.CLI_CGCCPF
                                    JOIN T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido ON cargaPedido.CPE_CODIGO = cargaEntregaPedido.CPE_CODIGO
                                    JOIN T_CARGA_ENTREGA cargaEntrega ON cargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                                WHERE
                                    cargaPedido.CAR_CODIGO = {codigoCarga}
                                    and cargaEntrega.CAR_CODIGO = {codigoCarga}
                                GROUP BY
                                    filial.FIL_DESCRICAO,
                                    empresa.EMP_RAZAO,
                                    veiculo.VEI_PLACA,
                                    funcionario.FUN_NOME,
                                    carga.CAR_CODIGO_CARGA_EMBARCADOR,
                                    FORMAT(carga.CAR_DATA_CARREGAMENTO, 'dd/MM/yyyy HH:mm'),
                                    COALESCE(clienteRecebedor.CLI_NOME, cliente.CLI_NOME),
                                    cargaEntrega.cen_ordem,
                                    CONCAT(
                                        COALESCE(clienteRecebedor.CLI_ENDERECO, cliente.CLI_ENDERECO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_NUMERO, cliente.CLI_NUMERO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_COMPLEMENTO, cliente.CLI_COMPLEMENTO),
                                        ', ',
                                        COALESCE(clienteRecebedor.CLI_BAIRRO, cliente.CLI_BAIRRO)
                                    ),
                                    COALESCE(clienteRecebedor.CLI_CGCCPF, cliente.CLI_CGCCPF)
                                ORDER BY
                                    cargaEntrega.CEN_ORDEM DESC;
                                ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioDetalhadoResumido)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.RomaneioDetalhadoResumido>();

        }
        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhadoResumido> ProdutosRomaneioDetalhadoResumido(int codigoCarga)
        {
            string query = $@"WITH AggregatedItems AS (
                                SELECT 
                                    produto.PRO_CODIGO_PRODUTO_EMBARCADOR AS ProdutoCodigo, 
                                    produto.GRP_DESCRICAO AS ProdutoDescricao, 
                                    STRING_AGG(pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ', ') AS PedidoNumero,
                                    SUM(cargaPedidoProduto.CPP_QUANTIDADE) AS ProdutoQuantidade, 
                                    MAX(pedidoProduto.PRP_QUANTIDADE_PALET) AS QuantidadePallet, 
                                    pedido.PED_CODIGO AS PedidoCodigo,
		                            COALESCE(
                                              clienteRecebedor.CLI_CGCCPF, cliente.CLI_CGCCPF
                                            ) AS CodigoDestinatario
                                FROM 
                                    T_CARGA_PEDIDO_PRODUTO cargaPedidoProduto 
                                    JOIN T_PRODUTO_EMBARCADOR produto ON cargaPedidoProduto.PRO_CODIGO = produto.PRO_CODIGO 
                                    JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedidoProduto.CPE_CODIGO = cargaPedido.CPE_CODIGO 
	                                LEFT JOIN T_CLIENTE clienteRecebedor ON cargaPedido.CLI_CODIGO_RECEBEDOR = clienteRecebedor.CLI_CGCCPF 
                                    JOIN T_CARGA carga ON cargaPedido.CAR_CODIGO = carga.CAR_CODIGO 
                                    JOIN T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
		                            JOIN T_CLIENTE cliente ON pedido.CLI_CODIGO = cliente.CLI_CGCCPF
                                    LEFT JOIN T_PEDIDO_PRODUTO pedidoProduto ON cargaPedidoProduto.PRO_CODIGO = pedidoProduto.PRO_CODIGO 
                                                                              AND cargaPedido.PED_CODIGO = pedidoProduto.PED_CODIGO
                                WHERE 
                                    cargaPedido.CAR_CODIGO = {codigoCarga}
                                GROUP BY 
                                    produto.PRO_CODIGO_PRODUTO_EMBARCADOR, 
                                    produto.GRP_DESCRICAO, 
                                    pedido.PED_CODIGO,
		                            cliente.CLI_CGCCPF,
                                    clienteRecebedor.CLI_CGCCPF
                            )
                            SELECT 
                                ProdutoCodigo,
                                ProdutoDescricao,
                                STRING_AGG(PedidoNumero, ', ') AS PedidoNumero,
                                SUM(ProdutoQuantidade) AS ProdutoQuantidade,
                                MAX(QuantidadePallet) AS QuantidadePallet,codigoDestinatario
                            FROM 
                                AggregatedItems
                            GROUP BY 
                                ProdutoCodigo,
                                ProdutoDescricao,
	                            codigoDestinatario;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhadoResumido)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhadoResumido>();

        }
        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhado> ProdutosRomaneioDetalhado(int codigoCarga)
        {
            string query = $@"SELECT 
                                produto.PRO_CODIGO_PRODUTO_EMBARCADOR AS ProdutoCodigo, 
                                produto.GRP_DESCRICAO AS ProdutoDescricao, 
                                SUM(
                                  cargaPedidoProduto.CPP_QUANTIDADE
                                ) AS ProdutoQuantidade, 
                                (
                                  SELECT 
                                    pedidoProduto.PRP_QUANTIDADE_PALET 
                                  FROM 
                                    T_PEDIDO_PRODUTO pedidoProduto 
                                  WHERE 
                                    cargaPedidoProduto.PRO_CODIGO = pedidoProduto.PRO_CODIGO 
                                    and pedidoProduto.PED_CODIGO = pedido.PED_CODIGO
                                ) AS QuantidadePallet, 
                                pedido.PED_CODIGO PedidoCodigo 
                              FROM 
                                T_CARGA_PEDIDO_PRODUTO cargaPedidoProduto 
                                JOIN T_PRODUTO_EMBARCADOR produto ON cargaPedidoProduto.PRO_CODIGO = produto.PRO_CODIGO 
                                JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedidoProduto.CPE_CODIGO = cargaPedido.CPE_CODIGO 
                                JOIN T_CARGA carga ON cargaPedido.CAR_CODIGO = carga.CAR_CODIGO 
                                JOIN T_PEDIDO pedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                              WHERE 
                                cargaPedido.CAR_CODIGO = {codigoCarga} 
                              GROUP BY 
                                cargaPedidoProduto.PRO_CODIGO, 
                                produto.PRO_CODIGO_PRODUTO_EMBARCADOR, 
                                produto.GRP_DESCRICAO, 
                                pedido.PED_CODIGO;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.ProdutosRomaneioDetalhado>();

        }

        public void DeletarCargaPedidoProdutoPorCodigoCargaPedidoViaQuery(int codigoCargaPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_PRODUTO WHERE CPE_CODIGO = {codigoCargaPedido}").ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery($"DELETE FROM T_CARGA_PEDIDO_PRODUTO WHERE CPE_CODIGO = {codigoCargaPedido}").ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }
    }
}