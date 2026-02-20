using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoPacote : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>
    {
        public CargaPedidoPacote(UnitOfWork unitOfWork, CancellationToken cancellationToken = default) : base(unitOfWork, cancellationToken) { }       

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote BuscarPorCodigo(int codigo)
        {
            var consultaCargaPedidoPacote = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(obj => obj.Codigo == codigo);

            return consultaCargaPedidoPacote
                .Fetch(p => p.Pacote)
                .ThenFetch(p => p.CTeTerceiroXML)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarCargaPedidoPacotePorPacotes(List<int> codigosPacotes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => codigosPacotes.Contains(o.Pacote.Codigo));

            return query.Fetch(o => o.Pacote).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarCargaPedidoPacotePendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.Pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || o.Pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
            return query.Fetch(o => o.Pacote).ToList();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote BuscarCargaPedidoPacotePorPacote(int pacote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.Pacote.Codigo == pacote);
            return query.Fetch(o => o.Pacote).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarCargaPedidoPacotePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Fetch(o => o.Pacote)
                        .Fetch(o => o.CargaPedido).ThenFetch(o => o.Carga)
                        .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote BuscarDocumentoPrincipal(int codigoCargaPedido, int totalDocumentoAdicionaisPorCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.DocumentoPrincipal && o.TotalDocumentosAdicionais < totalDocumentoAdicionaisPorCTeTerceiro);

            return query
                .Fetch(o => o.Pacote)
                .ThenFetch(p => p.CTeTerceiroXML)
                .ThenFetch(p => p.CTeTerceiro)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarCargaPedidoPacotePorLoggiKey(string loggiKey)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.Pacote.LogKey == loggiKey);

            return query.Fetch(obj => obj.CargaPedido).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarCargaPedidoPacoteLoggiKey(List<string> loggiKeys, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = loggiKeys.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotesRetornar = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                cargaPedidoPacotesRetornar.AddRange(query.Where(o => o.CargaPedido == cargaPedido && loggiKeys.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Pacote.LogKey)).Fetch(o => o.Pacote).ToList());

            return cargaPedidoPacotesRetornar;
        }

        public int BuscarQuantidadePacotesPorCargaPedidos(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => codigosCargaPedido.Contains(o.CargaPedido.Codigo));

            return query.Select(o => o.Pacote).Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Pacote> BuscarPacotesPorCargaPedidos(List<int> codigosCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => codigosCargaPedidos.Contains(o.CargaPedido.Codigo));

            return query.Select(o => o.Pacote).ToList();
        }

        public decimal BuscarQuantidadePacoteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Pacote).Distinct().Count();
        }


        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.QuantidadePacotesCargaPorCargaPedido>> BuscarQuantidadePacoteCargaPorCargaPedidoAsync(int carga, CancellationToken cancellationToken)
        {
            string sqlQuery = $@" 
                               SELECT T.CPE_CODIGO, QTD_PACOTES,QTD_CTES_ANTERIORES,
                                  CAST((QTD_PACOTES*100.0)/case when QTD_CTES_ANTERIORES = 0 then 999999999 else QTD_CTES_ANTERIORES end    AS decimal(18, 2)) PERCENTUAL 
                                  FROM (select cp.CPE_CODIGO, SUM(case when cpp.CPE_CODIGO is null then 0 else 1 end) QTD_PACOTES from T_CARGA_PEDIDO cp 
                                      LEFT join T_CARGA_PEDIDO_PACOTE cpp on cpp.CPE_CODIGO = cp.CPE_CODIGO
                                      where  cp.CAR_CODIGO = :carga
                                      group by cp.CPE_CODIGO ) T
                                  LEFT JOIN (
                                      select cp.CPE_CODIGO, SUM(case when c.CPE_CODIGO is null then 0 else 1 end) QTD_CTES_ANTERIORES from T_CARGA_PEDIDO cp
                                      LEFT JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO c on cp.CPE_CODIGO = c.CPE_CODIGO
                                      where  cp.CAR_CODIGO = :carga 
                                      group by cp.CPE_CODIGO) AS T2 ON T.CPE_CODIGO = T2.CPE_CODIGO  ";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sqlQuery).SetInt32("carga", carga);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.QuantidadePacotesCargaPorCargaPedido)));

            return (await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.QuantidadePacotesCargaPorCargaPedido>(cancellationToken)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoSemPacotePorCarga(int codigoCarga)
        {
            var queryCargaPedidoPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga)
                .Select(o => o.CargaPedido.Codigo);

            var queryPedidoCTeParaSubContratacao = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga)
                .Select(o => o.CargaPedido.Codigo).Distinct();

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga &&
                            !o.PedidoSemNFe &&
                            !queryCargaPedidoPacote.Contains(o.Codigo) &&
                            !queryPedidoCTeParaSubContratacao.Contains(o.Codigo)
                );

            return queryCargaPedido.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroCargaPedidoSemPacotePorCarga(int codigoCarga)
        {
            var queryCargaPedidoPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga)
                .Select(o => o.CargaPedido.Codigo);

            var queryPedidoCTeParaSubContratacao = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga)
                .Select(o => o.CargaPedido.Codigo).Distinct();

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga &&
                            !o.PedidoSemNFe &&
                            !queryCargaPedidoPacote.Contains(o.Codigo) &&
                            !queryPedidoCTeParaSubContratacao.Contains(o.Codigo)
                );

            return queryCargaPedido.FirstOrDefault();
        }
    }
}
