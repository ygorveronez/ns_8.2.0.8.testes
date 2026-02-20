using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>
    {
        public CargaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos


        //public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPendentesIntegracao(int inicio, int limite, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega, bool retornarSomenteCanhotoDigitalizado)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

        //    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query
        //                                                                                          where
        //                                                                                             !obj.IntegradoERP
        //                                                                                          select obj;

        //    if (situacoesEntrega.Count > 0)
        //        result = result.Where(o => situacoesEntrega.Contains(o.Situacao));

        //    IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> queryCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

        //    if (retornarSomenteCanhotoDigitalizado)
        //    {
        //        result = result.Where(o => !o.NotasFiscais.Any(p => queryCanhoto.Any(q => q.XMLNotaFiscal.Codigo == p.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo &&
        //                                                                                 q.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado))
        //                                && o.NotasFiscais.Where(p => queryCanhoto.Any(q => q.XMLNotaFiscal.Codigo == p.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)).Count() > 0);
        //    }

        //    return result
        //        .Skip(inicio)
        //        .Take(limite)
        //        .ToList();
        //}

        public List<int> BuscarCodigosEntregasPendentesIntegracao(int inicio, int limite, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query
                                                                                                  where
                                                                                                     !obj.IntegradoERP
                                                                                                  select obj;

            if (situacoesEntrega.Count > 0)
                result = result.Where(o => situacoesEntrega.Contains(o.Situacao));

            return result
                .Skip(inicio)
                .Take(limite).Select(o => o.Codigo)
                .ToList();
        }


        public IList<int> BuscarCodigosEntregasPendentesIntegracaoSomenteDigitalizados(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            //SE COLOCAR SKIP TAKE AZEDA O PE DA GALINHA
            string sql = @"select distinct entrega.cen_codigo ";

            sql = sql + ObterQueryEntregasPendentesIntegracaoSomenteDigitalizados(situacoesEntrega);

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta.List<int>();
        }

        public int ContarCodigosEntregasPendentesIntegracaoSomenteDigitalizados(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            string sql = @"select count(distinct entrega.cen_codigo) ";

            sql = sql + ObterQueryEntregasPendentesIntegracaoSomenteDigitalizados(situacoesEntrega);

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta.UniqueResult<int>();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPendentesIntegracao(int inicio, int limite, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query
                                                                                                  where
                                                                                                     !obj.IntegradoERP
                                                                                                  select obj;

            if (situacoesEntrega.Count > 0)
                result = result.Where(o => situacoesEntrega.Contains(o.Situacao));

            return result
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public int ContarPendentesIntegracao(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query
                                                                                                  where
                                                                                                     !obj.IntegradoERP && situacoesEntrega.Contains(obj.Situacao)
                                                                                                  select obj;

            return result.Count();
        }


        public bool ContemControleNaoFinalizado(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Situacao == SituacaoEntrega.NaoEntregue select obj;
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Codigo == codigo);
            return result
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.MotivoRejeicao)
                .Fetch(obj => obj.MotivoRetificacaoColeta)
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.ResponsavelFinalizacaoManual)
                .Fetch(obj => obj.MotivoAvaliacao)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Codigo == codigo);

            query = MontarFetchs(query);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCodigos(IList<int> codigos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaRetorno = new();
            int loteTamanho = 2000;
            int total = codigos.Count;
            for (int i = 0; i < total; i += loteTamanho)
            {
                var listaPaginada = codigos.Skip(i).Take(loteTamanho).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => listaPaginada.Contains(obj.Codigo));

                listaRetorno.AddRange(result
                    .Fetch(obj => obj.Cliente)
                    .ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.MotivoRejeicao)
                    .Fetch(obj => obj.MotivoRetificacaoColeta)
                    .Fetch(obj => obj.Carga)
                    .ThenFetch(obj => obj.Filial)
                    .Fetch(obj => obj.ResponsavelFinalizacaoManual)
                    .Fetch(obj => obj.MotivoAvaliacao)
                    .ToList());
            }
            return listaRetorno;
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorIdTrizy(string idTrizy)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.IdTrizy == idTrizy);
            return result
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.MotivoRejeicao)
                .Fetch(obj => obj.MotivoRetificacaoColeta)
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.ResponsavelFinalizacaoManual)
                .Fetch(obj => obj.MotivoAvaliacao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCodigosCargaPedido(List<int> codigosPedido)
        {
            if (codigosPedido == null || !codigosPedido.Any())
                return new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => codigosPedido.Contains(o.CargaPedido.Codigo)));

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Pedidos)
                .ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorIdTrizyAsync(string idTrizy, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.IdTrizy == idTrizy);

            query = MontarFetchs(query);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCargaPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Codigo == codigoPedido));
            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Cliente)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaPedidoAsync(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj =>
                    obj.Pedidos.Any(o => o.CargaPedido.Codigo == codigoPedido) &&
                    obj.Situacao != SituacaoEntrega.Rejeitado &&
                    obj.Situacao != SituacaoEntrega.Entregue
                );

            return query
                .Fetch(obj => obj.Carga)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregasPorCargaPedido(int codigoCargaPedido)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(x => x.Pedidos.Any(p => p.CargaPedido.Codigo == codigoCargaPedido))
                .Fetch(x => x.Cliente)
                .ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorPedido(int codigoPedido, double cliente, bool coleta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Pedido.Codigo == codigoPedido) && obj.Cliente.CPF_CNPJ == cliente && obj.Coleta == coleta && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);
            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Cliente)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntregaLiberadaPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Pedido.Codigo == codigoPedido) && obj.DataFim == null);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorPedido(int codigoPedido, int codigoCarga, bool coleta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Pedido.Codigo == codigoPedido) && obj.Carga.Codigo == codigoCarga && obj.Coleta == coleta && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);
            return result
                .Fetch(obj => obj.Carga)
                .FirstOrDefault();
        }

        public void AtualizarOrdensInferiores(int codigo, int novaOrdem, int ordemAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaEntrega
                             set Ordem = (Ordem - 1)
                           where Carga.Codigo = :codigoCarga
                             and Ordem <= :ordemAtual
                             and Ordem > :ordemAnterior"
                    )
                    .SetParameter("codigoCarga", codigo)
                    .SetParameter("ordemAnterior", ordemAnterior)
                    .SetParameter("ordemAtual", novaOrdem)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaEntrega
                             set Ordem = (Ordem - 1)
                           where Carga.Codigo = :codigoCarga
                             and Ordem <= :ordemAtual
                             and Ordem > :ordemAnterior"
                    )
                    .SetParameter("codigoCarga", codigo)
                    .SetParameter("ordemAnterior", ordemAnterior)
                    .SetParameter("ordemAtual", novaOrdem)
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

        public void AtualizarOrdensSuperiores(int codigo, int novaOrdem, int ordemAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaEntrega
                             set Ordem = (Ordem + 1)
                           where Carga.Codigo = :codigoCarga
                             and Ordem >= :ordemAtual
                             and Ordem < :ordemAnterior"
                    )
                    .SetParameter("codigoCarga", codigo)
                    .SetParameter("ordemAnterior", ordemAnterior)
                    .SetParameter("ordemAtual", novaOrdem)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaEntrega
                             set Ordem = (Ordem + 1)
                           where Carga.Codigo = :codigoCarga
                             and Ordem >= :ordemAtual
                             and Ordem < :ordemAnterior"
                    )
                    .SetParameter("codigoCarga", codigo)
                    .SetParameter("ordemAnterior", ordemAnterior)
                    .SetParameter("ordemAtual", novaOrdem)
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

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCodigoFetchCarga(int codigoEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Codigo == codigoEntrega);

            return query
                .Fetch(obj => obj.Carga)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarTransferenciaPorPedido(int codigoPedido, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            result = result.Where(obj => obj.CargaPedido.Pedido.Codigo == codigoPedido);
            result = result.Where(obj => obj.CargaEntrega.Cliente.CPF_CNPJ != cliente);
            result = result.Where(obj => obj.CargaEntrega.Coleta == false);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return result
                .Select(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Cliente)
                .OrderByDescending(obj => obj.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregaPorPedido(int codigoPedido, double cliente = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            result = result.Where(obj => obj.CargaPedido.Pedido.Codigo == codigoPedido && obj.CargaPedido.Recebedor == null);
            if (cliente > 0)
                result = result.Where(obj => obj.CargaEntrega.Cliente.CPF_CNPJ == cliente);
            result = result.Where(obj => obj.CargaEntrega.Coleta == false);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return result
                .Select(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Cliente)
                .OrderByDescending(obj => obj.Codigo)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarEntregaPorCargaPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o => o.Pedidos.Any(pedido => pedido.CargaPedido.Codigo == codigoPedido) && o.Coleta == false);

            return consultaCargaEntrega
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorPedidoRastreio(int codigoPedido, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            result = result.Where(obj => obj.CargaPedido.Pedido.Codigo == codigoPedido && obj.CargaPedido.Recebedor == null);
            result = result.Where(obj => obj.CargaEntrega.Cliente.CPF_CNPJ == cliente);
            result = result.Where(obj => obj.CargaEntrega.Coleta == false);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != SituacaoCarga.Cancelada);
            result = result.Where(obj => obj.CargaEntrega.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            //result = result.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Pedido.Codigo == codigoPedido && o.CargaPedido.Recebedor == null));
            //result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);
            //result = result.Where(obj => obj.Coleta == false);
            //result = result.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return result
                .Select(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Cliente)
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Pedidos.Any(o => o.CargaPedido.Pedido.Codigo == codigoPedido) && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return result.FirstOrDefault();
        }

        public int BuscarProximaOrdemPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);

            int? retorno = result.Max(o => (int?)o.Ordem);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }

        public bool CodigoRastreioExiste(string codigoRastreamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query where obj.CodigoRastreio == codigoRastreamento select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCodigoRastreio(string codigoRastreio)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            result = result.Where(obj => obj.CodigoRastreio == codigoRastreio);

            return result
                .Fetch(obj => obj.Cliente)
                .FirstOrDefault();
        }

        public bool PossuiNotaCoberturaPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);

            return result.Any(o => o.PossuiNotaCobertura);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorProtocoloCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Protocolo == carga);
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPrimeiroPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCarga(int carga, double cnpjForneceador = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);

            if (cnpjForneceador > 0)
                result = result.Where(x => x.Cliente.CPF_CNPJ == cnpjForneceador);

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>> BuscarPorCargaAsync(int carga, CancellationToken cancellationToken, double cnpjForneceador = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == carga);

            if (cnpjForneceador > 0)
                query = query.Where(x => x.Cliente.CPF_CNPJ == cnpjForneceador);

            return query
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                    .ThenFetch(obj => obj.Localidade)
                        .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                    .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregasSemProdutoPorCarga(int carga)
        {

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && !obj.ProdutosDevolucao.Any(x => x.XMLNotaFiscal != null));

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregasSemNotaPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && !obj.NotasFiscais.Any());
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarEntregaAnteriorPorCarga(int carga, int ordem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Ordem < ordem);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorPedidos(List<int> pedidos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            query = query.Where(obj => obj.Pedidos.Any(o => pedidos.Contains(o.CargaPedido.Pedido.Codigo)) && obj.Coleta == false && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaOrigem(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.CargaOrigem.Codigo == carga);
            return result
                .Fetch(obj => obj.Carga).ToList();
        }

        /// <summary>
        ///  Busca somente as entregas, deixando de lado as coletas
        /// </summary>
        /// <param name="carga"></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregasPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && !obj.Coleta); // Filtra pra pegar apenas as que não são coletas
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarResumoEntregas(int carga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == carga);

            query = query
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsultaResumoEntregas(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == carga);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargas(List<int> cargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o => cargas.Contains(o.Carga.Codigo));

            return consultaCargaEntrega
                .Fetch(o => o.Carga)
                .Fetch(o => o.Pedidos)
                .Fetch(o => o.Cliente)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargasComFetchs(List<int> cargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o => cargas.Contains(o.Carga.Codigo));

            return consultaCargaEntrega
                .Fetch(o => o.Pedidos)
                .Fetch(o => o.Cliente)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.Filial)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.Empresa)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.TiposComprovante)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoEmissaoDocumento)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoDocumentoEmissao)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoCalculoFrete)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoEmissao)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoControleEntrega)
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                    .ThenFetch(carga => carga.TipoOperacao)
                        .ThenFetch(tipoOperacao => tipoOperacao.ConfiguracaoCanhoto)
                .ToList();
        }

        public List<(int Carga, DateTime? DataEntradaRaio)> BuscarDatasEntradaRaioPorCargas(List<int> codigosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o =>
                    codigosCarga.Contains(o.Carga.Codigo) &&
                    o.Cliente != null &&
                    o.Coleta == true &&
                    (((bool?)o.ColetaEquipamento).HasValue == false || o.ColetaEquipamento == false)
                );

            return consultaCargaEntrega
                .OrderBy(o => o.Ordem)
                .Select(o => ValueTuple.Create(o.Carga.Codigo, o.DataEntradaRaio))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaeSituacao(int carga, List<SituacaoEntrega> situacoes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && situacoes.Contains(obj.Situacao));
            return result
                .Fetch(o => o.Cliente)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaNaoRealizada(int carga)
        {

            List<SituacaoEntrega> situacoes = new List<SituacaoEntrega> {
                SituacaoEntrega.EmCliente,
                SituacaoEntrega.Revertida,
                SituacaoEntrega.NaoEntregue
            };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && situacoes.Contains(obj.Situacao));
            return result.Fetch(obj => obj.Cliente).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaNaoRealizada(List<int> codigosCargas)
        {
            List<SituacaoEntrega> situacoes = new List<SituacaoEntrega> {
                SituacaoEntrega.EmCliente,
                SituacaoEntrega.Revertida,
                SituacaoEntrega.NaoEntregue
            };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => codigosCargas.Contains(obj.Carga.Codigo) && situacoes.Contains(obj.Situacao));
            return result
                .Fetch(obj => obj.Cliente)
                .ToList();
        }

        public int BuscarCodigoUltimaCargaEntrega(int codigoCarga)
        {
            IQueryable<int> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(entrega => entrega.Carga.Codigo == codigoCarga && entrega.Coleta == false)
                .OrderByDescending(entrega => entrega.Ordem)
                .Select(entrega => entrega.Codigo);

            return consultaCargaEntrega.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarUltimaCargaEntrega(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);
            return result.OrderByDescending(o => o.Ordem).Take(1).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarUltimaCargaEntregaRealizada(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);
            return result.OrderByDescending(o => o.DataFim).Take(1).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarUltimaCargaEntregaEntregaRealizada(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && !obj.Coleta && obj.DataFim.HasValue);
            return result.OrderByDescending(o => o.DataFim).Take(1).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarUltimaCargaEntregaRealizadaPorOrdem(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.DataFim.HasValue);
            return result.OrderByDescending(o => o.Ordem).Take(1).FirstOrDefault();
        }

        public DateTime? BuscarDataPrevistaMaximaPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(cargaEntrega => cargaEntrega.Carga.Codigo == carga && cargaEntrega.DataPrevista.HasValue);

            return query.Max(cargaEntrega => cargaEntrega.DataPrevista);
        }

        public bool BuscarExistePorCarga(int carga)
        {
            return SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Any(obj => obj.Carga.Codigo == carga);
        }

        public bool BuscarExisteColetaPorCargaeSituacao(int carga, List<SituacaoEntrega> situacoes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && situacoes.Contains(obj.Situacao) && obj.Coleta);
            return result.Count() > 0;
        }

        public int ContarEntregasPendentes(int carga, List<SituacaoEntrega> situacoes, bool validarApenasEntregas = false, string uf = "")
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && situacoes.Contains(obj.Situacao));

            if (validarApenasEntregas)
                result = result.Where(obj => !obj.Coleta);

            if (!string.IsNullOrWhiteSpace(uf))
            {
                result = result.Where(obj =>
                    (obj.Cliente.Localidade.Estado.Sigla ?? obj.Localidade.Estado.Sigla) == uf);
            }


            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCargaECliente(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Cliente.CPF_CNPJ == cliente);
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaEClienteAsync(int codigoCarga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && obj.Cliente.CPF_CNPJ == cliente);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorClienteECarga(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Cliente.CPF_CNPJ == cliente && !obj.Coleta);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorClienteRecebedor(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga && obj.CargaPedido.Recebedor.CPF_CNPJ == cliente && !obj.CargaEntrega.Coleta);
            return result.Select(obj => obj.CargaEntrega).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorClienteRecebedorAsync(int codigoCarga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga && obj.CargaPedido.Recebedor.CPF_CNPJ == cliente && !obj.CargaEntrega.Coleta);

            return query.Select(obj => obj.CargaEntrega).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCargaEClientePedido(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga && obj.CargaPedido.Pedido.Destinatario.CPF_CNPJ == cliente && !obj.CargaEntrega.Coleta);
            return result.Select(obj => obj.CargaEntrega).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarColetaNaOrigemPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == carga && obj.Coleta && !obj.ColetaEquipamento);

            return consultaCargaEntrega.OrderBy(obj => obj.Ordem).FirstOrDefault();
        }

        public bool ExisteColetaNaoEntregueNaOrigemPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == carga && obj.Coleta && !obj.ColetaEquipamento && obj.Situacao == SituacaoEntrega.NaoEntregue);

            return consultaCargaEntrega.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaFilhoOrigemEntregaPorCargaOrigem(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.CargaOrigem.Codigo == carga);

            return consultaCargaEntrega.Select(x => x.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarDestinosEntregasPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && !obj.Coleta);
            return result.Fetch(obj => obj.Cliente).OrderBy(obj => obj.Ordem).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarReentregaPorClienteECarga(int carga, double cliente, bool reentrega = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Cliente.CPF_CNPJ == cliente && obj.Reentrega == reentrega);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidosPorReentregaClienteECarga(int carga, double cliente, bool filtrarReentregas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga && obj.CargaEntrega.Cliente.CPF_CNPJ == cliente);

            if (filtrarReentregas)
                query = query.Where(obj => obj.CargaEntrega.Reentrega == true);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = query.Select(obj => obj.CargaPedido);

            return result.ToList();
        }

        public bool VerificarCargaPedidosComReentregaSolicitada(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga && obj.CargaEntrega.Cliente.CPF_CNPJ == cliente);

            bool result = query.Where(obj => obj.CargaPedido.ReentregaSolicitada == true).Any();

            return result;
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarEntregaOuColetaPorClienteECarga(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Cliente.CPF_CNPJ == cliente);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = result.OrderBy(obj => obj.Ordem).ToList();

            // Prioriza a entrega ainda não finalizada
            int total = entregas.Count;
            if (total == 1)
            {
                return entregas.First();
            }
            else if (total > 1)
            {
                for (int i = 0; i < total; i++)
                {
                    if (entregas[i].DataConfirmacao == null)
                    {
                        return entregas[i];
                    }
                }

                // Se todas já foram entregues, retorna a última
                return entregas.Last();
            }
            return null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregaOuColetaPorCargaEClientes(int carga, List<double> clientes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && clientes.Contains(obj.Cliente.CPF_CNPJ));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaeCliente(int carga, double cliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Cliente.CPF_CNPJ == cliente);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCargaENomeCliente(int carga, string nomeCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && nomeCliente == obj.Cliente.Nome);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntreguePorPeriodo(string codigoIntegracaoDestino, DateTime? dataPrevista, DateTime? dataFimPrevista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Cliente.CodigoIntegracao == codigoIntegracaoDestino && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Coleta == false && listaSituacaoEmAberto.Contains(obj.Situacao) && obj.DataPrevista >= dataPrevista.Value && obj.DataFimPrevista <= dataFimPrevista.Value);
            result = result.Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Veiculo);
            return result.ToList();
        }

        public bool ExisteCargaEntregaFinalizadaManualmentePorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.OrigemSituacao == OrigemSituacaoEntrega.UsuarioMultiEmbarcador);
            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorCargaENotaFiscal(int codigoCarga, int codigoNotaFiscal)
        {
            return ObterQueryPorCargaENotaFiscal(codigoCarga, codigoNotaFiscal).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaENotaFiscalAsync(int codigoCarga, int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(obj =>
                        obj.CargaEntrega.Carga.Codigo == codigoCarga &&
                        obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal &&
                        !obj.CargaEntrega.Coleta &&
                        obj.CargaEntrega.Situacao != SituacaoEntrega.Rejeitado &&
                        obj.CargaEntrega.Situacao != SituacaoEntrega.Entregue
                );

            return query.Select(obj => obj.CargaEntrega)
                .Fetch(obj => obj.Carga)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorNotaFiscal(int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            query = query.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.XMLNotaFiscal.Codigo == codigoNotaFiscal select obj.CargaPedido.Carga.Codigo).Contains(o.Carga.Codigo));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPorNotaFiscalUltimaEntrega(int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
             .Where(notaFiscal => notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal);

            return query.OrderByDescending(obj => obj.Codigo).Select(x => x.CargaEntrega).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarUltimaEntregaConfirmadaPorNotaFiscal(int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
             .Where(notaFiscal => notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal &&
                                  notaFiscal.CargaEntrega.Situacao == SituacaoEntrega.Entregue);

            return query.OrderByDescending(obj => obj.Codigo).Select(x => x.CargaEntrega).FirstOrDefault();
        }

        public DateTime? BuscarPrevisaoUltimaEntregaPorCarga(int codigoCarga)
        {
            int codigoCargaEntrega = BuscarCodigoUltimaCargaEntrega(codigoCarga);

            if (codigoCargaEntrega <= 0)
                return null;

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(entregaPedido => entregaPedido.CargaEntrega.Codigo == codigoCargaEntrega);

            return consultaCargaEntregaPedido
                .Select(entregaPedido => entregaPedido.CargaPedido.Pedido.PrevisaoEntrega)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> Consultar(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int notaFiscal = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = Consultar(codigoCarga);

            if (notaFiscal > 0)
            {
                //var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                //result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.XMLNotaFiscal.Numero == notaFiscal select obj.CargaPedido.Carga.Codigo).Contains(o.Carga.Codigo));

                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
                result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == notaFiscal select obj.CargaEntrega.Codigo).Contains(o.Codigo));
            }

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = Consultar(codigoCarga);

            return result.Count();
        }

        public int ContarConsultaColeta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa)
        {
            var sql = QueryConsularColeta(filtrosPesquisa, true);
            NHibernate.ISQLQuery consulta = sql.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaColeta> ConsultarColeta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sql = QueryConsularColeta(filtrosPesquisa, false, parametroConsulta);
            NHibernate.ISQLQuery consulta = sql.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaColeta)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaColeta>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarParadasPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = ConsultarParadasPorCarga(codigoCarga);

            return ObterLista(consultaCargaEntrega, parametrosConsulta);
        }
        public Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>> ConsultarParadasPorCargaAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = ConsultarParadasPorCarga(codigoCarga);

            return ObterListaAsync(consultaCargaEntrega, parametrosConsulta);
        }

        public int ContarConsultaParadasPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = ConsultarParadasPorCarga(codigoCarga);

            return consultaCargaEntrega.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer> ConsultarMovimentacaoAreaContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            bool temUnion = false;
            string sqlQuery = ObterQueryConsultarMovimentacaoAreaContainer(filtrosPesquisa, false, ref temUnion);

            if (temUnion)
            {
                string sqlQueryUnion = ObterQueryConsultarMovimentacaoAreaContainerUnion(filtrosPesquisa, false);

                sqlQueryUnion = $"{sqlQueryUnion} ORDER BY CargaEntrega.CEN_CODIGO OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;";

                sqlQuery += sqlQueryUnion;
            }
            else
            {
                sqlQuery = $"{sqlQuery} ORDER BY CargaEntrega.CEN_CODIGO OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;";
            }

            NHibernate.ISQLQuery hibernateQuery = SessionNHiBernate.CreateSQLQuery(sqlQuery);
            hibernateQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer)));

            return hibernateQuery.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Container.MovimentacaoAreaContainer>();
        }

        public int ContarConsultaMovimentacaoAreaContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtrosPesquisa)
        {
            bool temUnion = false;
            string sqlQuery = ObterQueryConsultarMovimentacaoAreaContainer(filtrosPesquisa, true, ref temUnion);

            if (temUnion)
            {
                string sqlQueryUnion = ObterQueryConsultarMovimentacaoAreaContainerUnion(filtrosPesquisa, true);
                sqlQuery += sqlQueryUnion;
            }

            NHibernate.ISQLQuery hibernateQuery = SessionNHiBernate.CreateSQLQuery(sqlQuery);

            if (temUnion)
            {
                IList<int> valores = hibernateQuery.SetTimeout(600).List<int>();
                return valores.Sum(x => x);
            }
            else
                return hibernateQuery.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntregaPedido> ConsultarPedidosPorCargaEntrega(int codigoCargaEntrega)
        {
            string sql = @"
                SELECT 
                    Pedido.PED_CODIGO Codigo,
                    Funcionario.FUN_NOME Vendedor,
                    Pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido,
                    Pedido.PED_PREVISAO_ENTREGA PrevisaoEntrega,
                    Pedido.PED_CODIGO_PEDIDO_CLIENTE CodigoPedidoCliente,
                    CargaDadosSumarizados.CDS_VOLUMES_TOTAL QuantidadeVolumes,
                    CargaDadosSumarizados.CDS_NUMERO_ORDEM NumeroOrdem,
                    CanalEntrega.CNE_DESCRICAO CanalEntrega,
	                SUBSTRING((SELECT', ' + convert(varchar,NotaFiscal.NF_NUMERO) + (CASE WHEN NotaFiscal.NF_SERIE IS NULL OR NotaFiscal.NF_SERIE = '' THEN '' ELSE '-' END) + NotaFiscal.NF_SERIE  AS [text()]      
		                FROM t_xml_nota_fiscal NotaFiscal
		                JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
		                WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
		                ORDER BY NotaFiscal.NF_NUMERO
                        FOR XML PATH ('')), 3, 2000) NotasFiscais,
                    (select sum(NotaFiscal.NF_VALOR)      
	                    FROM t_xml_nota_fiscal NotaFiscal
	                    JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
	                    WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO) as ValorNF,
                    (CASE WHEN CargaPedido.PED_REENTREGA_SOLICITADA = 1 THEN 'Sim' ELSE 'Não' END) Reentrega,
                    CargaEntregaPedido.CEN_CODIGO CodigoCargaEntrega,
                    CargaEntregaPedido.CEP_CODIGO CodigoCargaEntregaPedido,
                    SUBSTRING((SELECT DISTINCT ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR 
		                FROM T_CARGA Carga
                        LEFT JOIN T_CARGA_PEDIDO CargaPedidoLista ON CargaPedidoLista.PED_CODIGO = Pedido.PED_CODIGO
                        WHERE Carga.CAR_CODIGO = CargaPedidoLista.CAR_CODIGO
                        FOR XML PATH ('')), 3, 2000) CodigosCargasPedido,
                        Pedido.PED_DATA_ETA as DataAbate
                FROM 
                    t_carga_entrega_pedido CargaEntregaPedido
                JOIN 
                    t_carga_pedido CargaPedido ON CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO
                JOIN 
                    t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                LEFT JOIN 
                    t_funcionario Funcionario ON Pedido.FUN_CODIGO_VENDEDOR = Funcionario.FUN_CODIGO
                LEFT JOIN
                    t_carga Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
                LEFT JOIN
                    T_carga_dados_sumarizados CargaDadosSumarizados ON CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
                LEFT JOIN 
                    T_CANAL_ENTREGA CanalEntrega on CanalEntrega.CNE_CODIGO = Pedido.CNE_CODIGO
                WHERE 
                    CargaEntregaPedido.cen_codigo = :codigoCargaEntrega
                ORDER BY
	                Pedido.PED_PREVISAO_ENTREGA, PEDIDO.PED_NUMERO_PEDIDO_EMBARCADOR";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameter("codigoCargaEntrega", codigoCargaEntrega);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntregaPedido)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntregaPedido>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaNotaVendedorPedido> ConsultarVendedoresNotasPorCarga(int codigoCarga)
        {
            string sql = @"
                 SELECT 
                    Pedido.PED_CODIGO Codigo,
	                Pedido.FUN_CODIGO_VENDEDOR CodigoVendedor,
                    Funcionario.FUN_NOME Vendedor,
                    Pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido,
                    Pedido.PED_PREVISAO_ENTREGA PrevisaoEntrega,
					 SUBSTRING((SELECT', ' + convert(varchar,NotaFiscal.NF_NUMERO) + (CASE WHEN NotaFiscal.NF_SERIE IS NULL OR NotaFiscal.NF_SERIE = '' THEN '' ELSE '-' END) + NotaFiscal.NF_SERIE  AS [text()]      
		                FROM t_xml_nota_fiscal NotaFiscal
		                JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
		                WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
		                ORDER BY NotaFiscal.NF_NUMERO
                        FOR XML PATH ('')), 3, 2000) NotaFiscal,(
	                select sum(NotaFiscal.NF_VALOR)      
	                FROM t_xml_nota_fiscal NotaFiscal
	                JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
	                WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO) as ValorNF,(
	                select max(NotaFiscal.NFX_CODIGO)      
	                FROM t_xml_nota_fiscal NotaFiscal
	                JOIN t_pedido_xml_nota_fiscal PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO
	                WHERE PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO) as CodigoNota
                FROM 
                    t_carga_entrega_pedido CargaEntregaPedido
                JOIN 
                    t_carga_pedido CargaPedido ON CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO
                inner JOIN 
                    t_pedido Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                INNER JOIN 
                    t_funcionario Funcionario ON Pedido.FUN_CODIGO_VENDEDOR = Funcionario.FUN_CODIGO
                WHERE
                    CargaPedido.CAR_CODIGO = :codigoCarga
                ORDER BY
	                Pedido.PED_PREVISAO_ENTREGA, PEDIDO.PED_NUMERO_PEDIDO_EMBARCADOR";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameter("codigoCarga", codigoCarga);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaNotaVendedorPedido)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaNotaVendedorPedido>();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPrimeiroPedidoDaEntrega(int codigoEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Codigo == codigoEntrega);

            return result.Select(o => o.Pedidos.Select(p => p.CargaPedido.Pedido).FirstOrDefault()).FirstOrDefault();
        }

        public List<string> BuscarCodigoIntegracaoDestinatariosCargaEntrega(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.Coleta && obj.Cliente != null && obj.Cliente.CodigoIntegracao != null && obj.Cliente.CodigoIntegracao != "");

            return result.OrderBy(o => o.Ordem).Select(o => o.Cliente.CodigoIntegracao).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarEntregasTransbordo(List<int> codigosCargas, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            INhFetchRequest<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega, Dominio.Entidades.Embarcador.Cargas.Carga> consultaEntregas = ConsultarEntregasTransbordo(codigosCargas)
                .Fetch(x => x.Cliente).ThenFetch(x => x.Localidade)
                .Fetch(x => x.Carga);

            return ObterLista(consultaEntregas, parametrosConsulta);
        }

        public int ContarConsultaEntregasTransbordo(List<int> codigosCargas)
        {
            return ConsultarEntregasTransbordo(codigosCargas).Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntregaTipoDevolucaoEmAberto(int numeroRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            result = result.Where(obj => DateTime.Now > obj.DataFimPrevista
            && obj.Situacao == SituacaoEntrega.NaoEntregue
            && obj.Carga.TipoOperacao.GerarOcorrenciaPedidoEntregueForaPrazo
            && obj.Carga.TipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo != null
            );

            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> resultOcorrencia = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>()
                .Where(obj => obj.CargaEntrega != null);

            result = result.Where(obj => !resultOcorrencia.Any(o => o.CargaEntrega.Codigo == obj.Codigo));

            return result
                .OrderByDescending(obj => obj.Codigo)
                .Take(numeroRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntregaParqueamentoPorCargas(List<int> cargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o => cargas.Contains(o.Carga.Codigo) && o.Parqueamento);

            return consultaCargaEntrega
                .Fetch(o => o.Cliente)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarFronteirasPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Fronteira);
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Pais)
                .OrderBy(obj => obj.Ordem)
                .ToList();
        }

        public bool ExisteFronteirasPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga && obj.Fronteira);
            return result.Count() > 0 ? true : false;
        }

        public bool CargaPossuiControleEntrega(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = query.Where(obj => obj.Carga.Codigo == carga);
            return result.Count() > 0 ? true : false;
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPrimeiraCargaEntregaColetaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && obj.Coleta &&
                        obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                        obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPrimeiraCargaEntregaEntregaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && !obj.Coleta &&
                        obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                        obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarPrimeiraPorPedidoECarga(int codigoPedido, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> queryCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            int codigoCargaPedido = queryCargaPedido.Where(cp => cp.Pedido.Codigo == codigoPedido && cp.Carga.Codigo == codigoCarga).Select(cp => cp.Codigo).FirstOrDefault();

            return queryCargaEntregaPedido.Where(cep => cep.CargaPedido.Codigo == codigoCargaPedido).Select(cep => cep.CargaEntrega).FirstOrDefault();
        }

        public bool RejeitouTodasColetasPorCarga(int codigoCarga, int codigoCargaPedidoAtual)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> queryColetas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && obj.Coleta && obj.Codigo != codigoCargaPedidoAtual);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = queryColetas.Where(obj => obj.Situacao != SituacaoEntrega.Rejeitado);

            return !result.Any();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> BuscarObjetoDeValorPorCarga(int codigoCarga)
        {
            string sql = @"
                    SELECT 
                        CargaEntrega.CEN_CODIGO Codigo,
                        CargaEntrega.CEN_ORDEM OrdemPrevista,
                        CargaEntrega.CEN_ORDEM_REALIZADA OrdemRealizada,
                        CargaEntrega.CLI_CODIGO_ENTREGA CodigoCliente,
                        CargaEntrega.CEN_DATA_INICIO_ENTREGA DataInicioEntregaRealizada,
                        CargaEntrega.CEN_DATA_ENTREGA DataFimEntregaRealizada,
                        CargaEntrega.CEN_COLETA Coleta,
                        CargaEntrega.CEN_DATA_ENTRADA_RAIO DataEntradaRaio,
                        CargaEntrega.CEN_DATA_SAIDA_RAIO DataSaidaRaio,
                        CargaEntrega.CEN_SITUACAO Situacao
                    FROM 
                        T_CARGA_ENTREGA CargaEntrega
                    WHERE
                        CargaEntrega.CAR_CODIGO = :codigoCarga
                    ORDER BY
	                    CargaEntrega.CEN_ORDEM";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameter("codigoCarga", codigoCarga);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega>();
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega> ObterModeloDadosEntregas(DateTime dataAtualizacaoInicial, DateTime dataAtualizacaoFinal, string numeroCarga, string numeroPedido)
        {
            int limiteRegistros = 1000;
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega> cargaEntregas = new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega>();
            List<SituacaoCarga> situacoesCargaNaoFaturada = SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(cargaEntrega => cargaEntrega.Carga.CargaFechada && !situacoesCargaNaoFaturada.Contains(cargaEntrega.Carga.SituacaoCarga));

            if (dataAtualizacaoInicial != DateTime.MinValue)
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => cargaEntrega.Carga.DataAtualizacaoCarga >= dataAtualizacaoInicial);

            if (dataAtualizacaoFinal != DateTime.MinValue)
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => cargaEntrega.Carga.DataAtualizacaoCarga <= dataAtualizacaoFinal);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => cargaEntrega.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => cargaEntrega.Pedidos.Any(pedido => pedido.CargaPedido.Pedido.NumeroPedidoEmbarcador == numeroPedido));

            List<(int ProtocoloCarga, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Entrega Entrega)> dadosEntregas = consultaCargaEntrega
            .WithOptions(opcoes => { opcoes.SetTimeout(600); })
            .Select(cargaEntrega => ValueTuple.Create(
                cargaEntrega.Carga.Protocolo, new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Entrega()
                {
                    Codigo = cargaEntrega.Codigo,
                    SequenciaPrevista = (cargaEntrega.Ordem + 1).ToString(),
                    SequenciaRealizada = SituacaoEntregaHelper.ObterSituacaoEntregaFinalizada(cargaEntrega.Situacao) ? (cargaEntrega.OrdemRealizada + 1).ToString() : null,
                    Tipo = cargaEntrega.Fronteira ? TipoCargaEntrega.Fronteira.ObterDescricao() : (cargaEntrega.Coleta ? TipoCargaEntrega.Coleta.ObterDescricao() : TipoCargaEntrega.Entrega.ObterDescricao()),
                    Situacao = cargaEntrega.Situacao.ObterDescricao(),
                    DataInicio = cargaEntrega.DataInicio,
                    DataFim = cargaEntrega.DataFim,
                    DataEntradaRaio = cargaEntrega.DataEntradaRaio,
                    DataSaidaRaio = cargaEntrega.DataSaidaRaio,
                    Latitude = cargaEntrega.ClienteOutroEndereco != null ? cargaEntrega.ClienteOutroEndereco.Latitude : cargaEntrega.Cliente.Latitude,
                    Longitude = cargaEntrega.ClienteOutroEndereco != null ? cargaEntrega.ClienteOutroEndereco.Longitude : cargaEntrega.Cliente.Longitude,
                    LatitudeChegada = (cargaEntrega.LatitudeConfirmacaoChegada != null) ? cargaEntrega.LatitudeConfirmacaoChegada.ToString() : null,
                    LongitudeChegada = (cargaEntrega.LongitudeConfirmacaoChegada != null) ? cargaEntrega.LongitudeConfirmacaoChegada.ToString() : null,
                    LatitudeFinalizada = (cargaEntrega.LatitudeFinalizada != null) ? cargaEntrega.LatitudeFinalizada.ToString() : null,
                    LongitudeFinalizada = (cargaEntrega.LongitudeFinalizada != null) ? cargaEntrega.LongitudeFinalizada.ToString() : null,
                    Distancia = cargaEntrega.Distancia,
                    DataProgramada = cargaEntrega.DataPrevista,
                    DatasPrevistas = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaDatasPrevistas()
                    {
                        SaidaRaio = cargaEntrega.DataSaidaRaio,
                        ChegadaPrevista = cargaEntrega.DataPrevista,
                        ChegadaPrevistaReprogramada = cargaEntrega.DataReprogramada,
                    },
                    Cliente = (cargaEntrega.Cliente == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Cliente()
                    {
                        CpfCnpj = cargaEntrega.Cliente.Tipo == "J" ? String.Format(@"{0:00000000000000}", cargaEntrega.Cliente.CPF_CNPJ) : String.Format(@"{0:00000000000}", cargaEntrega.Cliente.CPF_CNPJ),
                        Nome = cargaEntrega.Cliente.Nome,
                        IE = cargaEntrega.Cliente.IE_RG,
                        GrupoPessoas = (cargaEntrega.Cliente.GrupoPessoas == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.GrupoPessoas()
                        {
                            CodigoIntegracao = cargaEntrega.Cliente.GrupoPessoas.CodigoIntegracao,
                            Descricao = cargaEntrega.Cliente.GrupoPessoas.Descricao
                        },
                        Endereco = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Endereco()
                        {
                            Logradouro = cargaEntrega.Cliente.Endereco,
                            Bairro = cargaEntrega.Cliente.Bairro,
                            Numero = cargaEntrega.Cliente.Numero,
                            Latitude = cargaEntrega.Cliente.Latitude,
                            Longitude = cargaEntrega.Cliente.Longitude,
                            Localidade = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                            {
                                Descricao = cargaEntrega.Cliente.Localidade.Descricao,
                                CodigoIbge = cargaEntrega.Cliente.Localidade.CodigoIBGE,
                                Regiao = (cargaEntrega.Cliente.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                                {
                                    Descricao = cargaEntrega.Cliente.Localidade.Regiao.Descricao,
                                    CodigoIntegracao = cargaEntrega.Cliente.Localidade.Regiao.CodigoIntegracao
                                },
                                Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                                {
                                    Descricao = cargaEntrega.Cliente.Localidade.Estado.Nome,
                                    Sigla = cargaEntrega.Cliente.Localidade.Estado.Sigla,
                                }
                            }
                        }
                    },

                    ClienteOutroEndereco = (cargaEntrega.ClienteOutroEndereco == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Endereco()
                    {
                        Logradouro = cargaEntrega.ClienteOutroEndereco.Endereco,
                        Bairro = cargaEntrega.ClienteOutroEndereco.Bairro,
                        Numero = cargaEntrega.ClienteOutroEndereco.Numero,
                        Latitude = cargaEntrega.ClienteOutroEndereco.Latitude,
                        Longitude = cargaEntrega.ClienteOutroEndereco.Longitude,
                        Localidade = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                        {
                            Descricao = cargaEntrega.ClienteOutroEndereco.Localidade.Descricao,
                            CodigoIbge = cargaEntrega.ClienteOutroEndereco.Localidade.CodigoIBGE,
                            Regiao = (cargaEntrega.ClienteOutroEndereco.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                            {
                                Descricao = cargaEntrega.ClienteOutroEndereco.Localidade.Regiao.Descricao,
                                CodigoIntegracao = cargaEntrega.ClienteOutroEndereco.Localidade.Regiao.CodigoIntegracao
                            },
                            Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                            {
                                Descricao = cargaEntrega.ClienteOutroEndereco.Localidade.Estado.Nome,
                                Sigla = cargaEntrega.ClienteOutroEndereco.Localidade.Estado.Sigla,

                            }
                        }
                    },
                    Localidade = (cargaEntrega.Localidade == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Localidade()
                    {
                        Descricao = cargaEntrega.Localidade.Descricao,
                        CodigoIbge = cargaEntrega.Localidade.CodigoIBGE,
                        Regiao = (cargaEntrega.Localidade.Regiao == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Regiao()
                        {
                            Descricao = cargaEntrega.Localidade.Regiao.Descricao,
                            CodigoIntegracao = cargaEntrega.Localidade.Regiao.CodigoIntegracao
                        },
                        Estado = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Estado()
                        {
                            Descricao = cargaEntrega.Localidade.Estado.Nome,
                            Sigla = cargaEntrega.Localidade.Estado.Sigla,
                        }
                    }
                }
            ))
            .ToList();

            if (dadosEntregas.Count > 0)
            {
                List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaPedido EntregaPedido)> dadosEntregaPedidos = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaPedido EntregaPedido)>();
                List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal NotasFiscais)> dadosNotasFicais = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal NotasFiscais)>();
                List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Canhoto Canhoto)> dadosCanhotos = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Canhoto Canhoto)>();
                List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckList EntregaCheckList)> dadosEntregaCheckList = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckList EntregaCheckList)>();

                for (int registroInicial = 0; registroInicial < dadosEntregas.Count; registroInicial += limiteRegistros)
                {
                    List<int> protocolosEntregas = dadosEntregas.Select(dadosEntrega => dadosEntrega.Entrega.Codigo).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> consultaCargaEntregaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                        .Where(cargaEntregaPedido => protocolosEntregas.Contains(cargaEntregaPedido.CargaEntrega.Codigo));

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> consultaCargaEntregaNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                       .Where(cargaEntregaPedido => protocolosEntregas.Contains(cargaEntregaPedido.CargaEntrega.Codigo));

                    dadosEntregaPedidos.AddRange(consultaCargaEntregaPedido
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(cargaEntregaPedido => ValueTuple.Create(
                            cargaEntregaPedido.CargaEntrega.Codigo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.EntregaPedido()
                            {
                                Protocolo = cargaEntregaPedido.CargaPedido.Pedido.Protocolo,
                                Numero = cargaEntregaPedido.CargaPedido.Pedido.NumeroPedidoEmbarcador,
                            }
                        ))
                        .ToList()
                    );

                    dadosNotasFicais.AddRange(consultaCargaEntregaNotaFiscal
                       .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                       .Select(cargaEntregaNotaFiscal => ValueTuple.Create(
                           cargaEntregaNotaFiscal.CargaEntrega.Codigo,
                           new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.NotaFiscal()
                           {
                               NumeroNotaFiscal = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal != null ? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero : 0,
                               SerieNotaFiscal = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal != null ? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie : "",
                               Valor = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal != null ? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor : 0,
                               Chave = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal != null ? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave : "",
                           }
                       ))
                       .ToList()
                   );

                    dadosCanhotos.AddRange(this.SessionNHiBernate
                        .CreateSQLQuery($@"
                            select CEN_CODIGO CodigoEntrega, CNF_NUMERO NumeroCanhoto, CNF_DATA_DIGITALIZACAO DataDigitalizacao
                              from (
                                       select EntregaNotaFiscal.CEN_CODIGO, Canhoto.CNF_NUMERO, Canhoto.CNF_DATA_DIGITALIZACAO
                                         from T_CARGA_ENTREGA_NOTA_FISCAL EntregaNotaFiscal
                                         join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.PNF_CODIGO = EntregaNotaFiscal.PNF_CODIGO
                                         join T_CANHOTO_NOTA_FISCAL Canhoto on Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.NFe} and Canhoto.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
                                        union
                                       select EntregaNotaFiscal.CEN_CODIGO, Canhoto.CNF_NUMERO, Canhoto.CNF_DATA_DIGITALIZACAO
                                         from T_CARGA_ENTREGA_NOTA_FISCAL EntregaNotaFiscal
                                         join T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL CanhotoAvulsoNotaFiscal on CanhotoAvulsoNotaFiscal.PNF_CODIGO = EntregaNotaFiscal.PNF_CODIGO
                                         join T_CANHOTO_AVULSO CanhotoAvulso on CanhotoAvulso.CAV_CODIGO = CanhotoAvulsoNotaFiscal.CAV_CODIGO
                                         join T_CANHOTO_NOTA_FISCAL Canhoto on Canhoto.CNF_TIPO_CANHOTO = {(int)TipoCanhoto.Avulso} and Canhoto.CAV_CODIGO = CanhotoAvulso.CAV_CODIGO
                                   ) CanhotosEntrega
                             where CEN_CODIGO in ({string.Join(", ", protocolosEntregas)})
                        ")
                        .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoEntrega, int NumeroCanhoto, DateTime? DataDigitalizacao)).GetConstructors().FirstOrDefault()))
                        .SetTimeout(600)
                        .List<(int CodigoEntrega, int NumeroCanhoto, DateTime? DataDigitalizacao)>()
                        .Select(dadosCanhoto => ValueTuple.Create(
                            dadosCanhoto.CodigoEntrega,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Canhoto()
                            {
                                Numero = dadosCanhoto.NumeroCanhoto,
                                DataDigitalizacao = dadosCanhoto.DataDigitalizacao
                            }
                        ))
                        .ToList()
                    );

                    List<(int CodigoEntrega, int CodigoCheckList, string DescricaoCheckList)> dadosCheckList = new List<(int CodigoEntrega, int CodigoCheckList, string DescricaoCheckList)>();

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList> consultaCargaEntregaCheckList = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>()
                        .Where(cargaEntregaCheckList => protocolosEntregas.Contains(cargaEntregaCheckList.CargaEntrega.Codigo));

                    dadosCheckList.AddRange(consultaCargaEntregaCheckList
                        .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(cargaEntregaCheckList => ValueTuple.Create(
                            cargaEntregaCheckList.CargaEntrega.Codigo,
                            cargaEntregaCheckList.Codigo,
                            cargaEntregaCheckList.Descricao
                        ))
                        .ToList()
                    );

                    if (dadosCheckList.Count > 0)
                    {
                        List<(int CodigoCheckList, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckListPergunta PerguntaCheckList)> dadosEntregaCheckListPergunta = new List<(int CodigoEntrega, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckListPergunta PerguntaCheckList)>();

                        for (int registroInicialCheckList = 0; registroInicialCheckList < dadosCheckList.Count; registroInicialCheckList += limiteRegistros)
                        {
                            List<int> codigosCheckListEntregas = dadosCheckList.Select(dadoCheckList => dadoCheckList.CodigoCheckList).Skip(registroInicialCheckList).Take(limiteRegistros).ToList();

                            dadosEntregaCheckListPergunta.AddRange(this.SessionNHiBernate
                            .CreateSQLQuery($@"
                                        select
                                            pergunta.CEC_CODIGO CodigoCheckList,        
                                            pergunta.CEP_DESCRICAO Pergunta,
                                            (CASE 
                                                WHEN pergunta.CEP_TIPO = 0 then ''
                                                WHEN pergunta.CEP_TIPO = 1 then (CASE WHEN pergunta.CEP_RESPOSTA_SIM_NAO = 1 THEN 'Sim' ELSE 'Não' END)
                                                WHEN pergunta.CEP_TIPO = 2 then (alternativas.descricao)
                                                WHEN pergunta.CEP_TIPO = 4 then (alternativas.descricao)
                                                WHEN pergunta.CEP_TIPO = 5 then (alternativas.descricao)
                                                ELSE pergunta.CEP_RESPOSTA
                                            END) Resposta
                                        from 
                                            T_CARGA_ENTREGA_CHECKLIST_PERGUNTA pergunta
                                            left join (select CEP_CODIGO, STRING_AGG(CEA_DESCRICAO, ', ') descricao FROM T_CARGA_ENTREGA_CHECKLIST_PERGUNTA_ALTERNATIVA where CEA_MARCADO = 1 GROUP BY CEP_CODIGO) AS alternativas ON alternativas.CEP_CODIGO = pergunta.CEP_CODIGO
                                        where
                                            CEC_CODIGO in ({string.Join(", ", codigosCheckListEntregas)})
                            ")
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoCheckList, string Pergunta, string Resposta)).GetConstructors().FirstOrDefault()))
                            .SetTimeout(600)
                            .List<(int CodigoCheckList, string Pergunta, string Resposta)>()
                            .Select(cargaEntregaCheckList => ValueTuple.Create(
                                cargaEntregaCheckList.CodigoCheckList,
                                new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckListPergunta()
                                {
                                    Pergunta = cargaEntregaCheckList.Pergunta,
                                    Resposta = cargaEntregaCheckList.Resposta
                                }
                            ))
                            .ToList());
                        }

                        dadosEntregaCheckList.AddRange(dadosCheckList.Select(dadosCheckList => ValueTuple.Create(
                                dadosCheckList.CodigoEntrega,
                                new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CheckList()
                                {
                                    Descricao = dadosCheckList.DescricaoCheckList,
                                    Perguntas = dadosEntregaCheckListPergunta
                                        .Where(checkListPergunta => checkListPergunta.CodigoCheckList == dadosCheckList.CodigoCheckList)
                                        .Select(dadosPerguntaCheckList => dadosPerguntaCheckList.PerguntaCheckList)
                                        .ToList()
                                }
                            )).ToList());
                    }
                }

                List<int> protocolosCargas = dadosEntregas.Select(dadosEntrega => dadosEntrega.ProtocoloCarga).Distinct().ToList();

                foreach (int protocoloCarga in protocolosCargas)
                {
                    List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Entrega> entregas = dadosEntregas.Where(dadosEntrega => dadosEntrega.ProtocoloCarga == protocoloCarga).Select(dadosEntrega => dadosEntrega.Entrega).ToList();

                    foreach (Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Entrega entrega in entregas)
                    {
                        entrega.Pedidos = dadosEntregaPedidos.Where(cargaEntregaPedido => cargaEntregaPedido.CodigoEntrega == entrega.Codigo).Select(cargaEntregaPedido => cargaEntregaPedido.EntregaPedido).ToList();
                        entrega.NotasFiscais = dadosNotasFicais.Where(cargaEntregaPedido => cargaEntregaPedido.CodigoEntrega == entrega.Codigo).Select(cargaEntregaPedido => cargaEntregaPedido.NotasFiscais).ToList();
                        entrega.Canhotos = dadosCanhotos.Where(dadosCanhoto => dadosCanhoto.CodigoEntrega == entrega.Codigo).Select(dadosCanhoto => dadosCanhoto.Canhoto).ToList();
                        entrega.CheckLists = dadosEntregaCheckList.Where(dadosCheckListEntrega => dadosCheckListEntrega.CodigoEntrega == entrega.Codigo).Select(dadosCheckListEntrega => dadosCheckListEntrega.EntregaCheckList).ToList();
                    }

                    cargaEntregas.Add(new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.CargaEntrega()
                    {
                        ProtocoloCarga = protocoloCarga,
                        Entregas = entregas
                    });
                }
            }

            return cargaEntregas;
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento> ObterDadosMonitoramentoPorCarga(Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>> monitoramentosPorCarga, Dictionary<int, List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Historico>> historicosPorMonitoramento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento> listaDadosMonitoramento = new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento>();
            foreach (var carga in cargas)
            {
                List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento> listaMonitoramentos = new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento>();

                if (!monitoramentosPorCarga.TryGetValue(carga.Codigo, out var monitoramentosDaCarga))
                    monitoramentosDaCarga = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();

                foreach (var monitoramento in monitoramentosDaCarga)
                {
                    historicosPorMonitoramento.TryGetValue(monitoramento.Codigo, out var historicoDoMonitoramento);

                    listaMonitoramentos.Add(new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Monitoramento
                    {
                        Codigo = monitoramento.Codigo,
                        DataCriacao = monitoramento.DataCriacao,
                        DataInicio = monitoramento.DataInicio,
                        Critico = monitoramento.Critico,
                        Situacao = monitoramento.Carga.SituacaoCarga,
                        NomeMotorista = carga.NomePrimeiroMotorista,
                        PlacaTracao = carga.Veiculo.Placa_Formatada ?? string.Empty,
                        PlacaReboque = carga.VeiculosVinculados?.FirstOrDefault()?.Placa_Formatada ?? string.Empty,
                        DataPosicaoAtual = monitoramento.UltimaPosicao?.Data,
                        LatitudePosicaoAtual = monitoramento.UltimaPosicao?.Latitude,
                        LongitudePosicaoAtual = monitoramento.UltimaPosicao?.Longitude,
                        PercentualViagem = monitoramento.PercentualViagem,
                        KmTotal = monitoramento.DistanciaPrevista,
                        TipoOperacao = monitoramento.Carga.TipoOperacao.Descricao,
                        Distancias = new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Distancia
                        {
                            Prevista = monitoramento.DistanciaPrevista,
                            Realizada = monitoramento.DistanciaRealizada,
                            AteOrigem = monitoramento.DistanciaAteOrigem,
                            AteDestino = monitoramento.DistanciaAteDestino
                        },
                        Historicos = historicoDoMonitoramento ?? new List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Historico>()
                    });
                }

                listaDadosMonitoramento.Add(new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.DadosMonitoramento
                {
                    ProtocoloCarga = carga.Codigo,
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    Monitoramentos = listaMonitoramentos
                });
            }

            return listaDadosMonitoramento;
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>> ConsultarCargaEntregaFinalizacaoAssincronaAsync(FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntregaFinalizacaoAssincrona = ConsultarCargaEntregaFinalizacaoAssincrona(filtroPesquisa);

            return consultaCargaEntregaFinalizacaoAssincrona
                .OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar)
                .Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros)
                .ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaCargaEntregaFinalizacaoAssincronaAsync(FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntregaFinalizacaoAssincrona = ConsultarCargaEntregaFinalizacaoAssincrona(filtroPesquisa);

            return consultaCargaEntregaFinalizacaoAssincrona.CountAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ObterEntregasAtingiramData(List<int> codigosFilial, List<int> codigosTipoOperacao, List<int> codigosTransportador, int codigoTipoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal tipoCobrancaMultimodal, int limiteRegistros)
        {
            List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(cargaEntrega => cargaEntrega.DataAgendamentoEntregaTransportador != null && cargaEntrega.DataAgendamentoEntregaTransportador <= DateTime.Now && listaSituacaoEmAberto.Contains(cargaEntrega.Situacao));

            if (codigoTipoOcorrencia > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> consultaOcorrenciaColetaEntrega = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>()
                    .Where(ocorrenciaColetaEntrega => ocorrenciaColetaEntrega.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);

                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => !consultaOcorrenciaColetaEntrega.Select(ocorrenciaColetaEntrega => ocorrenciaColetaEntrega.CargaEntrega.Codigo).Any(codigoCargaEntrega => codigoCargaEntrega == cargaEntrega.Codigo));
            }

            if (tipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTeMultimodal)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> consultaCargaCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => consultaCargaCTe.Select(cargaCTe => cargaCTe.Carga.Codigo).Any(codigoCarga => codigoCarga == cargaEntrega.Carga.Codigo));
            }

            if (codigosFilial.Count > 0)
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => codigosFilial.Contains(cargaEntrega.Carga.Filial.Codigo));

            if (codigosTipoOperacao.Count > 0)
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => codigosTipoOperacao.Contains(cargaEntrega.Carga.TipoOperacao.Codigo));

            if (codigosTransportador.Count > 0)
                consultaCargaEntrega = consultaCargaEntrega.Where(cargaEntrega => codigosTransportador.Contains(cargaEntrega.Carga.Empresa.Codigo));

            return consultaCargaEntrega
                .Fetch(cargaEntrega => cargaEntrega.Carga)
                .Take(limiteRegistros)
                .ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega> BuscarReprocessarGeracaoOcorrenciaEventosEntrega()
        {
            string sql = @"
                            select
                                OcorrenciaColetaEntrega.CEN_CODIGO CodigoEntrega,
                                OcorrenciaColetaEntrega.OCE_DATA_OCORRENCIA DataOcorrencia,
                                Ocorrencia.OCO_CODIGO CodigoOcorrencia,
                                OcorrenciaColetaEntrega.OCE_LATITUDE Latitude,
                                OcorrenciaColetaEntrega.OCE_LONGITUDE Longitude,
                                OcorrenciaColetaEntrega.OCE_ORIGEM_OCORRENCIA OrigemOcorrencia,
                                CargaEntregaEvento.CEE_CODIGO CodigoCargaEntregaEvento

                            from T_OCORRENCIA_COLETA_ENTREGA OcorrenciaColetaEntrega
	                            join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = OcorrenciaColetaEntrega.CEN_CODIGO
	                            join T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = OcorrenciaColetaEntrega.OCO_CODIGO
                                left join t_carga_entrega_evento CargaEntregaEvento
                                    on ((CargaEntregaEvento.CEN_CODIGO = OcorrenciaColetaEntrega.CEN_CODIGO and Ocorrencia.OCO_CODIGO in (2,3,10,9,23,24)) or
			                            (CargaEntregaEvento.CAR_CODIGO = CargaEntrega.CAR_CODIGO and Ocorrencia.OCO_CODIGO in (5,25,17)))
                                    and CargaEntregaEvento.OCO_CODIGO = OcorrenciaColetaEntrega.OCO_CODIGO
                                left join T_CARGA_ENTREGA_EVENTO_INTEGRACAO CargaEntregaEventoIntegracao
                                    on CargaEntregaEventoIntegracao.CEE_CODIGO = CargaEntregaEvento.CEE_CODIGO

                            where OcorrenciaColetaEntrega.OCE_DATA_OCORRENCIA >= '2025-04-01 00:00:00'
                                and OcorrenciaColetaEntrega.OCE_DATA_OCORRENCIA <= '2025-05-31 00:00:00'
	                            and OcorrenciaColetaEntrega.OCO_CODIGO in (5, 2, 3, 10, 24, 25)
                                and (CargaEntregaEvento.CEE_CODIGO is null or CargaEntregaEventoIntegracao.INT_CODIGO is null)
                                
                            order by OcorrenciaColetaEntrega.OCE_DATA_OCORRENCIA desc

                            ";
            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega>();
        }

        #endregion

        #region Métodos Tendência Entrega

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> BuscarEntregasControleTendenciaAtraso(Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfig, TendenciaEntrega FiltroDescricaoTendenciaEntrega, bool coleta, Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga ConfigAlertaAtrasoDescarga)
        {

            string sqlSelect = @"
                select entrega.CEN_CODIGO CodCargaEntrega, carga.CAR_CODIGO CodCarga, carga.CAR_CODIGO_CARGA_EMBARCADOR CodCargaEmbarcador, STR(entrega.CLI_CODIGO_ENTREGA, 14, 0) CPF_CNPJ, entrega.CEN_DATA_ENTREGA_PREVISTA DataEntregaPrevista, entrega.CEN_DATA_ENTREGA_REPROGRAMADA DataEntregaReprogramada, DATEDIFF(minute, entrega.CEN_DATA_ENTREGA_PREVISTA, entrega.CEN_DATA_ENTREGA_REPROGRAMADA) TempoDiferenca, entrega.CEN_DATA_AGENDAMENTO as DataAgendamentoEntrega from t_carga carga 
                inner join T_CARGA_ENTREGA entrega on carga.car_codigo = entrega.car_codigo 
                inner join T_MONITORAMENTO Monitoramento on Monitoramento.CAR_CODIGO = carga.CAR_CODIGO and Monitoramento.MON_STATUS in (1)
                left outer join T_FILIAL filial  on carga.FIL_CODIGO = filial.FIL_CODIGO 
                left outer join T_VEICULO veiculo on carga.CAR_VEICULO = veiculo.VEI_CODIGO ";

            sqlSelect += MontarConsultaEntregasFiltroTendencias(FiltroDescricaoTendenciaEntrega, acompanhamentoEntregaConfig, coleta, ConfigAlertaAtrasoDescarga);

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sqlSelect);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega)));

            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega>();
        }

        private string MontarConsultaEntregasFiltroTendencias(TendenciaEntrega FiltroDescricaoTendenciaEntrega, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfig, bool coleta, Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga ConfigAlertaAtrasoDescarga)
        {
            string sqlWhere = @" where carga.CAR_CARGA_FECHADA = 1
                                    and carga.CAR_SITUACAO not in (13, 18)
                                    and carga.CAR_VEICULO is not null
                                    and entrega.CEN_SITUACAO in (0,1) ";
            string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string dataReferencia = ConfigAlertaAtrasoDescarga.DataBaseAlerta == DataBaseAlerta.DataAgendamento ? "entrega.CEN_DATA_AGENDAMENTO" : "entrega.CEN_DATA_ENTREGA_PREVISTA";
            string dataBase = ConfigAlertaAtrasoDescarga.ConsiderarDataDeEntradaNoDestinoComoDataBase ? "Coalesce(entrega.CEN_DATA_ENTRADA_RAIO, entrega.CEN_DATA_ENTREGA_REPROGRAMADA)" : "entrega.CEN_DATA_ENTREGA_REPROGRAMADA";
            if (coleta)
            {
                switch (FiltroDescricaoTendenciaEntrega)
                {
                    case TendenciaEntrega.Adiantado:

                        sqlWhere += $@"
                              and entrega.CEN_COLETA = 1 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Adiantado}) and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                              and {dataBase} < dateadd(minute, {-acompanhamentoEntregaConfig.SaidaEmTempo.TotalMinutes}, {dataReferencia}) ";

                        return sqlWhere;

                    case TendenciaEntrega.Nohorario:
                        sqlWhere += $@"
                              and entrega.CEN_COLETA = 1 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Nohorario})  and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                              and {dataBase} BETWEEN dateadd(minute, {-acompanhamentoEntregaConfig.SaidaAtraso3.TotalMinutes}, {dataReferencia}) and dateadd(minute, {acompanhamentoEntregaConfig.SaidaEmTempo.TotalMinutes}, {dataReferencia})";

                        return sqlWhere;

                    case TendenciaEntrega.Atrasado:
                        sqlWhere += $@"
                            and entrega.CEN_COLETA = 1 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Atrasado})  and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                             and ({dataBase} > dateadd(minute, {acompanhamentoEntregaConfig.SaidaAtraso3.TotalMinutes}, {dataReferencia})  or ({dataReferencia} < '{dataAtual}' and COALESCE(entrega.CEN_DATA_ENTRADA_RAIO, entrega.CEN_DATA_INICIO_ENTREGA) IS NULL))";
                        return sqlWhere;
                    default:
                        return "";
                }
            }
            else
            {
                switch (FiltroDescricaoTendenciaEntrega)
                {

                    case TendenciaEntrega.Adiantado:
                        sqlWhere += $@"
                              and entrega.CEN_COLETA = 0 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Adiantado})  and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                              and {dataBase} < dateadd(minute, {-acompanhamentoEntregaConfig.DestinoEmTempo.TotalMinutes}, {dataReferencia}) ";

                        return sqlWhere;

                    case TendenciaEntrega.Nohorario:
                        sqlWhere += $@"
                              and entrega.CEN_COLETA = 0 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Nohorario})  and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                              and {dataBase} BETWEEN dateadd(minute, {-acompanhamentoEntregaConfig.DestinoEmTempo.TotalMinutes}, {dataReferencia}) and dateadd(minute, {acompanhamentoEntregaConfig.DestinoAtraso3.TotalMinutes}, {dataReferencia})";

                        return sqlWhere;

                    case TendenciaEntrega.Atrasado:
                        sqlWhere += $@"
                              and entrega.CEN_COLETA = 0 and (entrega.CEN_TENDENCIA is null or entrega.CEN_TENDENCIA <> {(int)TendenciaEntrega.Atrasado})  and entrega.CEN_DATA_ENTREGA_REPROGRAMADA is not null
                             and ({dataBase} > dateadd(minute, {acompanhamentoEntregaConfig.DestinoAtraso3.TotalMinutes}, {dataReferencia}) or ({dataReferencia} < '{dataAtual}' and COALESCE(entrega.CEN_DATA_ENTRADA_RAIO, entrega.CEN_DATA_INICIO_ENTREGA) IS NULL))";

                        return sqlWhere;
                    default:
                        return "";
                }
            }
        }

        public void AtualizarStatusTendenciasEntregas(TendenciaEntrega FiltroDescricaoTendenciaEntrega, IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> listAtualizar)
        {
            int loteTamanho = 100;
            int total = listAtualizar.Count;
            for (int i = 0; i < total; i += loteTamanho)
            {
                var listaPaginada = listAtualizar.Skip(i).Take(loteTamanho).ToList();

                string sqlUpdate = MontarUpdateEntregasFiltroTendencias(FiltroDescricaoTendenciaEntrega, listaPaginada);

                UnitOfWork.Sessao.CreateQuery(sqlUpdate).ExecuteUpdate();
            }
        }

        private string MontarUpdateEntregasFiltroTendencias(TendenciaEntrega FiltroDescricaoTendenciaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> listaAtualizar)
        {
            string sqlUpdate = "";

            List<int> codigosAtualizar = listaAtualizar.Select(obj => obj.CodCargaEntrega).ToList();

            switch (FiltroDescricaoTendenciaEntrega)
            {
                case TendenciaEntrega.Adiantado:
                    sqlUpdate += $" update CargaEntrega set Tendencia = {(int)TendenciaEntrega.Adiantado} where Codigo in ({string.Join(", ", codigosAtualizar)}) "; // SQL-INJECTION-SAFE
                    return sqlUpdate;
                case TendenciaEntrega.Nohorario:
                    sqlUpdate += $" update CargaEntrega set Tendencia = {(int)TendenciaEntrega.Nohorario} where Codigo in ({string.Join(", ", codigosAtualizar)}) "; // SQL-INJECTION-SAFE
                    return sqlUpdate;
                case TendenciaEntrega.Atrasado:
                    sqlUpdate += $" update CargaEntrega set Tendencia = {(int)TendenciaEntrega.Atrasado} where Codigo in ({string.Join(", ", codigosAtualizar)}) "; // SQL-INJECTION-SAFE
                    return sqlUpdate;
                default:
                    return sqlUpdate;
            }
        }

        #endregion

        #region Resumo Roteiro

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro> BuscarResumoRoteiro(int codigoCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sqlSelect = @"SELECT T.Etapa, T.Inicio, T.Fim, T.Total, (T.Freetime * 60) Freetime, 
                                CASE 
                                WHEN Total > (T.Freetime * 60) then Total - (T.Freetime * 60)
                                ELSE 0
                                END Exedente 
                                FROM  (
                                SELECT CargaEntrega.CEN_ORDEM_REALIZADA Ordem,
                                CASE 
                                WHEN CEN_COLETA = 1 THEN 'Coleta'
                                WHEN CEN_FRONTEIRA = 1 THEN 'Fronteira ' + Cliente.CLI_NOME
                                WHEN CEN_PARQUEAMENTO = 1 THEN 'Parqueamento'
                                ELSE 'Entrega'
                                END Etapa,
                                CargaEntrega.CEN_DATA_INICIO_ENTREGA Inicio,
                                CargaEntrega.CEN_DATA_FIM_ENTREGA Fim,
                                datediff(minute, CargaEntrega.CEN_DATA_INICIO_ENTREGA , CargaEntrega.CEN_DATA_FIM_ENTREGA) Total,
                                CASE
                                WHEN CEN_COLETA = 1 THEN ISNULL(Rota.ROF_TEMPO_CARREGAMENTO_TICKS, 0) / 36000000000
                                WHEN CEN_FRONTEIRA = 1 THEN ISNULL((SELECT TOP(1) Fronteira.RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA FROM T_ROTA_FRETE_FRONTEIRA Fronteira where Fronteira.ROF_CODIGO = Rota.ROF_CODIGO and Fronteira.CLI_CGCCPF = Cliente.CLI_CGCCPF), 0) / 60
                                WHEN CEN_PARQUEAMENTO = 1 THEN ISNULL((SELECT TOP(1) pp.RPD_TEMPO_ESTIMADO_PERMANENCIA_MINUTOS /60 FROM T_ROTA_FRETE_PONTO_PRE_DEFINIDO pp WHERE pp.ROF_CODIGO = Rota.ROF_CODIGO AND pp.RPD_LOCAL_DE_PARQUEAMENTO = 1), ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000.0)
                                ELSE ISNULL(Rota.ROF_TEMPO_DESCARGA_TICKS, 0) / 36000000000
                                END Freetime
                                    FROM T_CARGA_ENTREGA CargaEntrega
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                                    JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                    LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = Carga.ROF_CODIGO
                                WHERE CargaEntrega.CAR_CODIGO = " + codigoCarga.ToString("D") + @") AS T
                                order by Ordem ";

            //if (!string.IsNullOrWhiteSpace(propOrdena))
            //    sqlSelect += " order by " + propOrdena + " " + dirOrdena;

            if (maximoRegistros > 0)
                sqlSelect += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sqlSelect);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro)));

            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro>();
        }

        public int ContarBuscarResumoRoteiro(int codigoCarga)
        {
            string sqlSelect = @"SELECT   COUNT(0) as CONTADOR 
                                    FROM T_CARGA_ENTREGA CargaEntrega
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                                    JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                    LEFT OUTER JOIN T_ROTA_FRETE Rota ON Rota.ROF_CODIGO = Carga.ROF_CODIGO
                                WHERE CargaEntrega.CAR_CODIGO = " + codigoCarga.ToString("D");

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(sqlSelect);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega> ConsultarCargaEntregaQualidadeEntrega(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, TipoConsultaQualidadeEntrega tipoConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega> query = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega>();

            ISQLQuery consultaCargaEntregaQualidadeEntrega = ConsultarCargaEntregaQualidadeEntregas(filtroPesquisa, parametrosConsulta);
            consultaCargaEntregaQualidadeEntrega.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega)));
            return consultaCargaEntregaQualidadeEntrega.SetTimeout(900).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega> ObterResumoQualidadeEntregas(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa, TipoConsultaQualidadeEntrega tipoConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega> query = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaQualidadeEntrega>();

            ISQLQuery consultaCargaEntregaQualidadeEntrega = ConsultarCargaEntregaQualidadeEntregas(filtroPesquisa, null, tipoConsulta);
            consultaCargaEntregaQualidadeEntrega.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega)));
            return consultaCargaEntregaQualidadeEntrega.SetTimeout(900).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega>();
        }

        public int ContarConsultaCargaEntregaQualidadeEntrega(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa)
        {
            ISQLQuery consultaCargaEntregaQualidadeEntrega = ConsultarCargaEntregaQualidadeEntregas(filtroPesquisa, null, TipoConsultaQualidadeEntrega.TotalGeral);

            return consultaCargaEntregaQualidadeEntrega.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega> ContarResumoQualidadeEntregas(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtrosPesquisa)
        {
            string filtro = GetFiltroPesquisaConsultaQualidadeEntrega(filtrosPesquisa);
            string sqlSelect = @"SELECT 
                                     COUNT(DISTINCT CASE WHEN CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA = 1 THEN XMLNotaFiscal.NFX_CODIGO END) AS QtdDisponivelParaConsulta,
                                        COUNT(DISTINCT CASE WHEN CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA = 0 THEN XMLNotaFiscal.NFX_CODIGO END) AS QtdNaoDisponivelParaConsulta,
                                        COUNT(DISTINCT XMLNotaFiscal.NFX_CODIGO) AS QtdTotalParaConsulta";

            string sqlFrom = GetSqlFromConsultaQualidadeEntrega();
            string stringQqlQuery = $"{sqlSelect} {sqlFrom} {filtro}";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(stringQqlQuery);

            query = (ISQLQuery)query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(
                typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega)));

            return query.SetTimeout(900)
                        .List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ResumoConsultaQualidadeEntrega>();
        }
        #endregion

        #region AvaliacaoEntrega

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaComValorNF> BuscaCargaEntregaComValorNF(int codigoCarga)
        {
            string sqlSelect = @" SELECT 
                    cast( P.CLI_CODIGO_REMETENTE  as float) as Remetente,
                    cast( P.CLI_CODIGO_TOMADOR  as float) as Tomador,
                    cast( P.CLI_CODIGO_RECEBEDOR  as float) as Recebedor,
                    cast( P.CLI_CODIGO_EXPEDIDOR  as float) as Expedidor,
                    MAX(CP.PED_PREVISAO_ENTREGA)PrevisaoEntrega,
                    cast(sum(NF.NF_VALOR) as float) as ValorNf
                    FROM T_CARGA_ENTREGA CE
                    INNER JOIN T_CARGA_ENTREGA_PEDIDO CEP ON CEP.CEN_CODIGO = CE.CEN_CODIGO 
                    INNER JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CEP.CPE_CODIGO  
                    INNER JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO  
                    INNER JOIN T_PEDIDO_XML_NOTA_FISCAL NFP ON NFP.CPE_CODIGO = CP.CPE_CODIGO 
                    INNER JOIN T_XML_NOTA_FISCAL NF ON NF.NFX_CODIGO=NFP.NFX_CODIGO 
                    WHERE CE.CAR_CODIGO = " + codigoCarga.ToString("D") + @" AND CEN_COLETA = 0
                    GROUP BY P.CLI_CODIGO_REMETENTE,P.CLI_CODIGO_TOMADOR,P.CLI_CODIGO_RECEBEDOR,P.CLI_CODIGO_EXPEDIDOR ";
            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sqlSelect);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaComValorNF)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaComValorNF>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarAvaliacaoEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = ConsultarAvaliacaoEntrega(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaAvaliacaoEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = ConsultarAvaliacaoEntrega(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> Consultar(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query;
        }

        private ISQLQuery ConsultarCargaEntregaQualidadeEntregas(
     FiltroPesquisaQualidadeEntrega filtro,
     Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros = null,
     TipoConsultaQualidadeEntrega tipoConsulta = TipoConsultaQualidadeEntrega.Listagem)
        {
            var cte = MontarCTEDadosAgrupados(filtro);
            string selectFinal;
            string clausulaFinal = string.Empty;

            switch (tipoConsulta)
            {
                case TipoConsultaQualidadeEntrega.Totalizador:
                    selectFinal = @"
                    SELECT 
                        COUNT(CASE WHEN DisponivelParaConsulta = 1 THEN 1 END) AS QtdDisponivelParaConsulta,
                        COUNT(CASE WHEN DisponivelParaConsulta = 0 THEN 1 END) AS QtdNaoDisponivelParaConsulta,
                        COUNT(*) AS QtdTotalParaConsulta
                    FROM DadosAgrupados";
                    break;

                case TipoConsultaQualidadeEntrega.TotalGeral:
                    selectFinal = "SELECT COUNT(*) AS TotalRegistros FROM DadosAgrupados";
                    break;

                case TipoConsultaQualidadeEntrega.Listagem:
                default:
                    selectFinal = "SELECT * FROM DadosAgrupados";

                    if (!string.IsNullOrWhiteSpace(parametros?.PropriedadeOrdenar))
                    {
                        clausulaFinal += $" ORDER BY {parametros.PropriedadeOrdenar} {parametros.DirecaoOrdenar}";
                    }

                    if (parametros?.LimiteRegistros > 0)
                    {
                        clausulaFinal += $" OFFSET {parametros.InicioRegistros} ROWS FETCH NEXT {parametros.LimiteRegistros} ROWS ONLY";
                    }
                    break;
            }

            string sql = $"{cte}\n{selectFinal}{clausulaFinal}";
            return this.SessionNHiBernate.CreateSQLQuery(sql);
        }

        private string MontarCTEDadosAgrupados(FiltroPesquisaQualidadeEntrega filtro)
        {
            string filtrosWhere = GetFiltroPesquisaConsultaQualidadeEntrega(filtro);
            string joins = GetSqlFromConsultaQualidadeEntrega();

            return $@"
                    WITH DadosAgrupados AS (
                        SELECT 
                            XMLNotaFiscal.NFX_CODIGO as CodigoNotaFiscal,
                            MAX(CargaEntrega.CEN_CODIGO) AS CodigoCargaEntrega,
                            MAX(CargaEntrega.CEN_DATA_ENTRADA_RAIO) AS DataEntradaRaio,
                            MAX(CargaEntrega.CEN_DATA_SAIDA_RAIO) AS DataSaidaRaio,
                            MAX(XMLNotaFiscal.NF_NUMERO) AS NumeroNotaFiscal,
                            MAX(XMLNotaFiscal.NF_SERIE) AS SerieNotaFiscal,
                            MAX(PedidoXMLNotaFiscal.PNF_TIPO_NOTA_FISCAL) AS TipoNotaFiscal,
                            MAX(XMLNotaFiscal.NF_DATA_EMISSAO) AS DataEmissaoNotaFiscal,
                            MAX(XMLNotaFiscal.NF_CHAVE) AS ChaveNotaFiscal,
                            MAX(Carga.CAR_CODIGO_CARGA_EMBARCADOR) AS NumeroCarga,
                            MAX(Empresa.EMP_FANTASIA) AS NomeFantasiaTransportador,
                            MAX(Empresa.EMP_CNPJ) AS CNPJTransportador,
                            MAX(TipoDeCarga.TCG_DESCRICAO) AS DescricaoTipoDeCarga,
                            MAX(Destinatario.CLI_NOME) AS NomeDestinatario,
                            MAX(Destinatario.CLI_CGCCPF) AS CPFCNPJDestinatario,
                            MAX(Carga.CAR_PROTOCOLO) AS ProtocoloCarga,
                            MAX(Filial.FIL_DESCRICAO) AS DescricaoFilial,
                            MAX(Filial.FIL_CNPJ) AS CNPJFilial,
                            MAX(CanhotoNotaFiscal.CNF_CODIGO) AS CodigoCanhoto,
                            MAX(CanhotoNotaFiscal.CNF_SITUACAO_CANHOTO) AS SituacaoCanhoto,
                            MAX(CanhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO) AS SituacaoDigitalizacaoCanhoto,
                            MAX(CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO) AS DataDigitalizacaoCanhoto,
                            MAX(CargaEntrega.CEN_DATA_ENTREGA) AS DataEntregaCliente,
                            MAX(XMLNotaFiscal.NF_SITUACAO_ENTREGA) AS SituacaoNotaFiscal,
                            MAX(CentroResultado.CRE_DESCRICAO) AS CentroResultado,
                            MAX(Origem.LOC_DESCRICAO) AS OrigemDescricao,
                            MAX(Destino.LOC_DESCRICAO) AS DestinoDescricao,
                            MAX(ClienteComplementar.CLC_ESCRITORIO_VENDAS) AS EscritorioVendas,
                            MAX(ClienteComplementar.CLC_MATRIZ) AS MatrizVendas,
                            MAX(XMLNotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA) AS TipoNotaFiscalIntegrada,
                            MAX(CanhotoNotaFiscal.CNF_SITUACAO_PGTO_CANHOTO) AS SituacaoPgtoCanhoto,
                            CAST(MAX(CAST(CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA AS INT)) AS BIT) AS DisponivelParaConsulta,
                            MAX(CargaEntrega.CEN_DATA_ENTREGA_ORIGINAL) AS DataEntregaOriginal
                        {joins}
                        {filtrosWhere}
                        GROUP BY XMLNotaFiscal.NFX_CODIGO
                    )";
        }

        private string GetFiltroPesquisaConsultaQualidadeEntrega(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaQualidadeEntrega filtroPesquisa)
        {
            string sqlQueryWhere = " WHERE 1=1 AND CargaEntrega.CEQ_CODIGO IS NOT NULL AND CargaEntrega.CEN_COLETA = 0 AND Transbordo.TRB_CODIGO is null";

            if (filtroPesquisa.NumeroNF > 0)
                sqlQueryWhere += $" AND  XMLNotaFiscal.NF_NUMERO = {filtroPesquisa.NumeroNF}";

            if (!string.IsNullOrEmpty(filtroPesquisa.Carga))
                sqlQueryWhere += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtroPesquisa.Carga}'";

            if (filtroPesquisa.Filiais.Count > 0)
                sqlQueryWhere += $" AND Filial.FIL_CODIGO IN ({string.Join(", ", filtroPesquisa.Filiais)})";

            if (filtroPesquisa.DataInicioCriacaoCarga != DateTime.MinValue)
                sqlQueryWhere += $" AND CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{filtroPesquisa.DataInicioCriacaoCarga.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (filtroPesquisa.DataFimCriacaoCarga != DateTime.MinValue)
                sqlQueryWhere += $" AND CAST(Carga.CAR_DATA_CRIACAO AS DATE) < '{filtroPesquisa.DataFimCriacaoCarga.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (filtroPesquisa.DataInicioEmissaoNF != DateTime.MinValue)
                sqlQueryWhere += $" AND XMLNotaFiscal.NF_DATA_EMISSAO >= '{filtroPesquisa.DataInicioEmissaoNF.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (filtroPesquisa.DataFimEmissaoNF != DateTime.MinValue)
                sqlQueryWhere += $" AND XMLNotaFiscal.NF_DATA_EMISSAO <= '{filtroPesquisa.DataFimEmissaoNF.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (filtroPesquisa.TipoCanhoto != TipoCanhoto.Todos)
                sqlQueryWhere += $" AND CanhotoNotaFiscal.CNF_TIPO_CANHOTO = {(int)filtroPesquisa.TipoCanhoto}";

            if (filtroPesquisa.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Todas)
                sqlQueryWhere += $" AND CanhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = {(int)filtroPesquisa.SituacaoDigitalizacaoCanhoto}";

            if (filtroPesquisa.DisponivelParaConsulta != null)
            {
                if (filtroPesquisa.DisponivelParaConsulta ?? false)
                    sqlQueryWhere += $" AND CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA = 1";
                else
                    sqlQueryWhere += $" AND CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA = 0";
            }

            return sqlQueryWhere;
        }

        private string GetSqlFromConsultaQualidadeEntrega()
        {
            string sqlQueryFrom = @" FROM 
                                T_CARGA_ENTREGA CargaEntrega
                                JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO AND Carga.CAR_SITUACAO NOT IN (13, 18) AND Carga.CAR_CARGA_FECHADA = 1
                                JOIN T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal ON CargaEntregaNotaFiscal.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal ON PedidoXMLNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO
                                JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO
                                JOIN T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal ON CanhotoNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                                LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga ON TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO
                                LEFT JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = XMLNotaFiscal.CLI_CODIGO_DESTINATARIO
                                LEFT JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = XMLNotaFiscal.CLI_CODIGO_REMETENTE
                                LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO
                                LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = PedidoXMLNotaFiscal.CRE_CODIGO
                                LEFT JOIN T_LOCALIDADES Origem on Origem.LOC_CODIGO = Remetente.LOC_CODIGO
                                LEFT JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = Destinatario.LOC_CODIGO
                                LEFT JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar on ClienteComplementar.CLI_CODIGO = Destinatario.CLI_CGCCPF
								LEFT JOIN T_TRANSBORDO Transbordo on Transbordo.CAR_CODIGO = Carga.CAR_CODIGO";

            return sqlQueryFrom;
        }

        private SQLDinamico QueryConsularColeta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            StringBuilder sql = new StringBuilder();

            var parametros = new List<ParametroSQL>();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"select
                        cargaEntrega.CEN_CODIGO as Codigo,
                        cargaEntrega.CEN_SITUACAO as SituacaoEntrega,
                        carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador,
                            (select
                                [Codigo] = Pedido.PED_CODIGO,
			                    [NumeroPedidoEmbarcador] = Pedido.PED_NUMERO_PEDIDO_EMBARCADOR,
			                    [CodigoRemetente] = cast(Pedido.CLI_CODIGO_REMETENTE as bigint),
			                    [DescricaoRemetente] = Remetente.CLI_NOME,
			                    [CodigoDestinatario] = cast(Pedido.CLI_CODIGO as bigint),
			                    [DescricaoDestinatario] = Destinatario.CLI_NOME
                            from T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
                                inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaEntregaPedido.CPE_CODIGO 
                                inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
			                    inner join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
			                    inner join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO
                            where CargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO
                            order by Pedido.PED_CODIGO
		                    FOR JSON PATH ) as DadosPedidos,
                        modeloVeicularCarga.MVC_NUMERO_REBOQUES as NumeroReboques,
                        tipoOperacao.TOP_EXIGE_PLACA_TRACAO as ExigePlacaTracao
                ");

            sql.Append(" from T_CARGA_ENTREGA cargaEntrega ");
            sql.Append(@"
                left join T_CARGA carga on cargaEntrega.CAR_CODIGO = carga.CAR_CODIGO
                left join T_EMPRESA empresaCarga on empresaCarga.EMP_CODIGO = carga.EMP_CODIGO
                left join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.TOP_CODIGO = carga.TOP_CODIGO
				left join T_MODELO_VEICULAR_CARGA modeloVeicularCarga on modeloVeicularCarga.MVC_CODIGO = carga.MVC_CODIGO
            ");

            sql.Append("WHERE 1 = 1 ");

            sql.Append($" AND cargaEntrega.CEN_COLETA = 1");
            sql.Append($" AND carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada}");
            sql.Append($" AND carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                sql.Append($" AND carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.CodigoTransportador > 0)
                sql.Append($" AND empresaCarga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
            {
                sql.Append(@$"
                    and (
                        exists (
                            select
                                cargaEntregaPedido.CEP_CODIGO 
                            from
                                T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido
                            inner join T_CARGA_PEDIDO   cargaPedido  on cargaPedido.CPE_CODIGO       = cargaEntregaPedido.CPE_CODIGO 
                            inner join T_PEDIDO         pedido       on cargaPedido.PED_CODIGO        = pedido.PED_CODIGO 
                            where
                                pedido.PED_NUMERO_PEDIDO_EMBARCADOR = :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR
                                and cargaEntregaPedido.CEN_CODIGO = cargaEntrega.CEN_CODIGO  
                        )
                    )
                ");

                parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", filtrosPesquisa.NumeroPedidoEmbarcador));
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                sql.Append(@$"
                    and (
                        exists (
                            select
                                cargaEntregaNotaFiscal.CEF_CODIGO 
                            from
                                T_CARGA_ENTREGA_NOTA_FISCAL cargaEntregaNotaFiscal 
                            left join T_PEDIDO_XML_NOTA_FISCAL pedidoXmlNotaFiscal on cargaEntregaNotaFiscal.PNF_CODIGO=pedidoXmlNotaFiscal.PNF_CODIGO 
                            left join T_XML_NOTA_FISCAL xmlNotaFiscal on pedidoXmlNotaFiscal.NFX_CODIGO=xmlNotaFiscal.NFX_CODIGO 
                            where
                                xmlNotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNotaFiscal} 
                                and cargaEntregaNotaFiscal.CEN_CODIGO = cargaEntrega.CEN_CODIGO 
                        )
                    ) 
                "); // SQL-INJECTION-SAFE
            }

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.AppendLine($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.AppendLine($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return new SQLDinamico(sql.ToString(), parametros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarParadasPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaEntrega;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarEntregasTransbordo(List<int> codigosCargas)
        {
            List<SituacaoEntrega> situacoes = new List<SituacaoEntrega> {
                SituacaoEntrega.EmCliente,
                SituacaoEntrega.Revertida,
                SituacaoEntrega.NaoEntregue
            };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>()
                .Where(obj => codigosCargas.Contains(obj.Carga.Codigo))
                .Where(obj => (!((bool?)obj.Fronteira).HasValue || !obj.Fronteira) || (!((bool?)obj.Coleta).HasValue || !obj.Coleta))
                .Where(obj => situacoes.Contains(obj.Situacao));

            return query;
        }

        private string ObterQueryConsultarMovimentacaoAreaContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtrosPesquisa, bool contarConsulta, ref bool temUnion)
        {
            temUnion = false;
            StringBuilder sqlQuery = new StringBuilder("select ");

            if (contarConsulta)
                sqlQuery.Append("count(1) ");
            else
                sqlQuery.Append(@"
                    CargaEntrega.CEN_CODIGO [Codigo],
                    CargaEntrega.CEN_SITUACAO [Situacao],
                    CargaEntrega.CEN_COLETA [Coleta],
                    CargaEntrega.CEN_FRONTEIRA [Fronteira],
                    CargaEntrega.CEN_COLETA_EQUIPAMENTO [ColetaEquipamento],
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR [Carga],
                    Carga.CAR_CODIGO [CodigoCarga],
                    Cliente.CLI_ARMADOR [Armador],
                    ClienteLocal.CLI_CGCCPF [LocalRetiradaContainer],
                    ContainerRetirar.CTR_CODIGO [CodigoContainerRetirar],
                    ContainerRetirar.CTR_DESCRICAO [DescricaoContainerRetirar],
                    TipoOperacao.TOP_DESCRICAO [TipoOperacao],
                    TipoContainer.CTI_CODIGO [CodigoTipoContainerCarga],
                    TipoContainer.CTI_DESCRICAO [TipoContainerCarga],
                    Cliente.CLI_CGCCPF [CPFCNPJCliente],
                    Veiculo.VEI_PLACA [Veiculo],
                    substring((
                        select distinct ', ' + _pedidoCargaEntrega.PED_NUMERO_EXP
                          from (
                                   select _pedido.PED_NUMERO_EXP 
                                     from T_PEDIDO _pedido
                                     join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     join T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                    where CargaEntrega.CEN_COLETA = 0
                                      and _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                      and _pedido.PED_NUMERO_EXP is not null
                                    union
                                   select _pedido.PED_NUMERO_EXP 
                                     from T_PEDIDO _pedido
                                     join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                    where CargaEntrega.CEN_COLETA = 1
                                      and _cargaPedido.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                      and _pedido.PED_NUMERO_EXP is not null
                               ) as _pedidoCargaEntrega
                           for xml path('')
                    ), 3, 4000) [NumeroExp]
                ");

            sqlQuery.Append(@"
                from T_CARGA_ENTREGA CargaEntrega
                join T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                left join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO
                left join T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = Carga.CRG_CODIGO
                left join T_RETIRADA_CONTAINER RetiradaContainer on RetiradaContainer.CAR_CODIGO = Carga.CAR_cODIGO
                left join T_CLIENTE ClienteLocal on ClienteLocal.CLI_CGCCPF = RetiradaContainer.CLI_CODIGO_LOCAL
                left join T_CONTAINER ContainerRetirar on ContainerRetirar.CTR_CODIGO = RetiradaContainer.CTR_CODIGO
                left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO
                left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarregamento on ModeloVeicularCarregamento.MVC_CODIGO = Carregamento.MVC_CODIGO
                left join T_CONTAINER_TIPO TipoContainer on TipoContainer.CTI_CODIGO = coalesce(RetiradaContainer.CTI_CODIGO, ModeloVeicularCarga.CTI_CODIGO, ModeloVeicularCarregamento.CTI_CODIGO)
            ");

            sqlQuery.Append($" where Carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Encerrada}, {(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}) ");

            if (filtrosPesquisa.CpfCnpjAreaContainer > 0)
                sqlQuery.Append($"and Cliente.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjAreaContainer} ");

            if (filtrosPesquisa.SituacaoEntrega.HasValue)
            {
                if (filtrosPesquisa.SituacaoEntrega.Value == SituacaoEntrega.NaoEntregue)
                {
                    sqlQuery.Append($"and CargaEntrega.CEN_SITUACAO in (0,1)");
                }
                else
                {
                    sqlQuery.Append($"and CargaEntrega.CEN_SITUACAO = {(int)filtrosPesquisa.SituacaoEntrega.Value} ");
                }

            }


            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                sqlQuery.Append($"and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                sqlQuery.Append($"and TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                sqlQuery.Append($"and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                temUnion = true;
                sqlQuery.Append($"and (Carga.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa} or RetiradaContainer.CLI_CODIGO_LOCAL in ({string.Join(", ", filtrosPesquisa.CpfCnpjEmpresaColeta)})) ");
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_MOTORISTA _cargaMotorista where _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO and _cargaMotorista.CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoOrigem > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_PEDIDO _cargaPedido join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoOrigem}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoDestino > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_PEDIDO _cargaPedido join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoDestino}) "); // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                sqlQuery.Append(
                    $@"and exists (
                        select top(1) 1
                          from T_PEDIDO _pedido
                          join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                          join T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                         where CargaEntrega.CEN_COLETA = 0
                           and _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                           and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'
                         union
                        select top(1) 1
                          from T_PEDIDO _pedido
                          join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                         where CargaEntrega.CEN_COLETA = 1
                           and _cargaPedido.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                           and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'
                    ) "
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContainer))
                sqlQuery.Append($@"and exists(
                                               SELECT 1 FROM T_COLETA_CONTAINER _coletaContainer JOIN T_CONTAINER _container ON _container.CTR_CODIGO = _coletaContainer.CTR_CODIGO      
                                                WHERE (_coletaContainer.CAR_CODIGO = Carga.CAR_CODIGO OR _coletaContainer.CAR_CODIGO_ATUAL = Carga.CAR_CODIGO)
                                                  AND _container.CTR_NUMERO = '{filtrosPesquisa.NumeroContainer}'
                                             ) ");

            if (filtrosPesquisa.TipoCargaEntrega.HasValue)
            {
                if (filtrosPesquisa.TipoCargaEntrega.Value == TipoCargaEntrega.Coleta)
                    sqlQuery.Append($"and CargaEntrega.CEN_COLETA = 1 ");
                else
                    sqlQuery.Append($"and CargaEntrega.CEN_COLETA = 0 ");
            }

            return sqlQuery
                .ToString();
        }

        private string ObterQueryEntregasPendentesIntegracaoSomenteDigitalizados(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega)
        {
            string sql = $@"from T_CARGA_ENTREGA entrega
                inner join T_CARGA_ENTREGA_NOTA_FISCAL notaEntrega on entrega.CEN_CODIGO = notaEntrega.CEN_CODIGO
                inner join T_PEDIDO_XML_NOTA_FISCAL pedXmlNotaFiscal on pedXmlNotaFiscal.PNF_CODIGO = notaEntrega.PNF_CODIGO
                inner join T_CANHOTO_NOTA_FISCAL canhoto on canhoto.NFX_CODIGO = pedXmlNotaFiscal.NFX_CODIGO 
                where   not exists ( select NFX_CODIGO from T_CANHOTO_NOTA_FISCAL canhoto2 where canhoto2.NFX_CODIGO in
                (select NFX_CODIGO from T_PEDIDO_XML_NOTA_FISCAL pedXml inner join T_CARGA_ENTREGA_NOTA_FISCAL entreganota on pedXml.PNF_CODIGO = entreganota.PNF_CODIGO
                    where entreganota.CEN_CODIGO = entrega.cen_codigo ) and canhoto2.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 3)
                and entrega.CEN_SITUACAO in ({string.Join(", ", (from o in situacoesEntrega select (int)o))}) and not entrega.CEN_INTEGRADO_ERP=1";

            return sql;
        }

        private string ObterQueryConsultarMovimentacaoAreaContainerUnion(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaMovimentacaoAreaContainer filtrosPesquisa, bool contarConsulta)
        {
            StringBuilder sqlQuery = new StringBuilder(" UNION select ");

            if (contarConsulta)
                sqlQuery.Append("count(1) ");
            else
                sqlQuery.Append(@"
                    CargaEntrega.CEN_CODIGO [Codigo],
                    CargaEntrega.CEN_SITUACAO [Situacao],
                    CargaEntrega.CEN_COLETA [Coleta],
                    CargaEntrega.CEN_FRONTEIRA [Fronteira],
                    CargaEntrega.CEN_COLETA_EQUIPAMENTO [ColetaEquipamento],
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR [Carga],
                    Carga.CAR_CODIGO [CodigoCarga],
                    Cliente.CLI_ARMADOR [Armador],
                    ClienteLocal.CLI_CGCCPF [LocalRetiradaContainer],
                    ContainerRetirar.CTR_CODIGO [CodigoContainerRetirar],
                    ContainerRetirar.CTR_DESCRICAO [DescricaoContainerRetirar],
                    TipoOperacao.TOP_DESCRICAO [TipoOperacao],
                    TipoContainer.CTI_CODIGO [CodigoTipoContainerCarga],
                    TipoContainer.CTI_DESCRICAO [TipoContainerCarga],
                    Cliente.CLI_CGCCPF [CPFCNPJCliente],
                    Veiculo.VEI_PLACA [Veiculo],
                    substring((
                        select distinct ', ' + _pedidoCargaEntrega.PED_NUMERO_EXP
                          from (
                                   select _pedido.PED_NUMERO_EXP 
                                     from T_PEDIDO _pedido
                                     join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     join T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                                    where CargaEntrega.CEN_COLETA = 0
                                      and _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                      and _pedido.PED_NUMERO_EXP is not null
                                    union
                                   select _pedido.PED_NUMERO_EXP 
                                     from T_PEDIDO _pedido
                                     join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                    where CargaEntrega.CEN_COLETA = 1
                                      and _cargaPedido.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                                      and _pedido.PED_NUMERO_EXP is not null
                               ) as _pedidoCargaEntrega
                           for xml path('')
                    ), 3, 4000) [NumeroExp]
                ");

            sqlQuery.Append(@"
                from T_CARGA_ENTREGA CargaEntrega
                join T_CARGA Carga on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                left join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA
                left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO
                left join T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = Carga.CRG_CODIGO
                left join T_RETIRADA_CONTAINER RetiradaContainer on RetiradaContainer.CAR_CODIGO = Carga.CAR_cODIGO
                left join T_CLIENTE ClienteLocal on ClienteLocal.CLI_CGCCPF = RetiradaContainer.CLI_CODIGO_LOCAL
                left join T_CONTAINER ContainerRetirar on ContainerRetirar.CTR_CODIGO = RetiradaContainer.CTR_CODIGO
                left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO
                left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarregamento on ModeloVeicularCarregamento.MVC_CODIGO = Carregamento.MVC_CODIGO
                left join T_CONTAINER_TIPO TipoContainer on TipoContainer.CTI_CODIGO = coalesce(RetiradaContainer.CTI_CODIGO, ModeloVeicularCarga.CTI_CODIGO, ModeloVeicularCarregamento.CTI_CODIGO)
            ");

            sqlQuery.Append($" where Carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Encerrada}, {(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}) ");

            if (filtrosPesquisa.CpfCnpjAreaContainer > 0)
                sqlQuery.Append($"and Cliente.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjAreaContainer} ");

            if (filtrosPesquisa.SituacaoEntrega.HasValue)
                sqlQuery.Append($"and CargaEntrega.CEN_SITUACAO = {(int)filtrosPesquisa.SituacaoEntrega.Value} ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                sqlQuery.Append($"and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                sqlQuery.Append($"and TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                sqlQuery.Append($"and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_PEDIDO _cargaPedido join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_pedido.cli_codigo in ({string.Join(", ", filtrosPesquisa.CpfCnpjEmpresaColeta)}) or _pedido.CLI_CODIGO_REMETENTE in ({string.Join(", ", filtrosPesquisa.CpfCnpjEmpresaColeta)}))) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoMotorista > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_MOTORISTA _cargaMotorista where _cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO and _cargaMotorista.CAR_MOTORISTA = {filtrosPesquisa.CodigoMotorista}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoOrigem > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_PEDIDO _cargaPedido join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoOrigem}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoDestino > 0)
                sqlQuery.Append($"and exists(select 1 from T_CARGA_PEDIDO _cargaPedido join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoDestino}) "); // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                sqlQuery.Append(
                    $@"and exists (
                        select top(1) 1
                          from T_PEDIDO _pedido
                          join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                          join T_CARGA_ENTREGA_PEDIDO _cargaEntregaPedido on _cargaEntregaPedido.CPE_CODIGO = _cargaPedido.CPE_CODIGO
                         where CargaEntrega.CEN_COLETA = 0
                           and _cargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                           and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'
                         union
                        select top(1) 1
                          from T_PEDIDO _pedido
                          join T_CARGA_PEDIDO _cargaPedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                         where CargaEntrega.CEN_COLETA = 1
                           and _cargaPedido.CAR_CODIGO = CargaEntrega.CAR_CODIGO
                           and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'
                    ) "
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContainer))
                sqlQuery.Append($@"and exists(
                                               SELECT 1 FROM T_COLETA_CONTAINER _coletaContainer JOIN T_CONTAINER _container ON _container.CTR_CODIGO = _coletaContainer.CTR_CODIGO      
                                                WHERE (_coletaContainer.CAR_CODIGO = Carga.CAR_CODIGO OR _coletaContainer.CAR_CODIGO_ATUAL = Carga.CAR_CODIGO)
                                                  AND _container.CTR_NUMERO = '{filtrosPesquisa.NumeroContainer}'
                                             ) ");

            if (filtrosPesquisa.TipoCargaEntrega.HasValue)
            {
                if (filtrosPesquisa.TipoCargaEntrega.Value == TipoCargaEntrega.Coleta)
                    sqlQuery.Append($"and CargaEntrega.CEN_COLETA = 1 ");
                else
                    sqlQuery.Append($"and CargaEntrega.CEN_COLETA = 0 ");
            }

            return sqlQuery
                .ToString();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarAvaliacaoEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaAvaliacaoEntrega filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.CNPJsDestinatario?.Count > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> consultaCargaEntregaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
                result = result.Where(obj => consultaCargaEntregaPedido.Any(o => obj.Codigo == o.CargaEntrega.Codigo && filtrosPesquisa.CNPJsDestinatario.Contains(o.CargaPedido.Pedido.Destinatario.CPF_CNPJ)));
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> consultaCargaEntregaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
                result = result.Where(obj => consultaCargaEntregaPedido.Any(o => obj.Codigo == o.CargaPedido.Codigo && o.CargaPedido.NotasFiscais.Any(n => n.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal)));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroTransporte))
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> consultaCargaEntregaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
                result = result.Where(obj => consultaCargaEntregaPedido.Any(o => obj.Codigo == o.CargaPedido.Codigo && o.CargaPedido.NotasFiscais.Any(n => n.XMLNotaFiscal.NumeroTransporte == filtrosPesquisa.NumeroTransporte)));
            }

            if (filtrosPesquisa.Respondida.HasValue)
            {
                if (filtrosPesquisa.Respondida.Value == 1)
                    result = result.Where(obj => obj.DataAvaliacao.HasValue);
                else
                    result = result.Where(obj => !obj.DataAvaliacao.HasValue);
            }

            if (filtrosPesquisa.SituacaoEntrega.HasValue)
                result = result.Where(obj => obj.Situacao == filtrosPesquisa.SituacaoEntrega.Value);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConsultarCargaEntregaFinalizacaoAssincrona(FiltroPesquisaConsultaCargaEntregaFinalizacaoAssincrona filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> consultaCargaEntregaFinalizacaoAssincrona = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona.Where(cargaEntrega => cargaEntrega.CargaEntregaFinalizacaoAssincrona != null);

            if (filtroPesquisa.CodigoCarga > 0)
                consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona.Where(cargaEntrega => cargaEntrega.Carga.Codigo == filtroPesquisa.CodigoCarga);

            if (filtroPesquisa.CodigoCliente > 0)
                consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona.Where(cargaEntrega => cargaEntrega.Cliente.CPF_CNPJ == filtroPesquisa.CodigoCliente);

            if (filtroPesquisa.DataInicialInclusaoProcessamento != DateTime.MinValue)
            {
                consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona
                    .Where(cargaEntrega => cargaEntrega.CargaEntregaFinalizacaoAssincrona.DataInclusao >= filtroPesquisa.DataInicialInclusaoProcessamento);
            }

            if (filtroPesquisa.DataFinalInclusaoProcessamento != DateTime.MinValue)
            {
                consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona
                    .Where(cargaEntrega => cargaEntrega.CargaEntregaFinalizacaoAssincrona.DataInclusao <= filtroPesquisa.DataFinalInclusaoProcessamento);
            }

            if (filtroPesquisa.SituacaoProcessamento != null)
                consultaCargaEntregaFinalizacaoAssincrona = consultaCargaEntregaFinalizacaoAssincrona.Where(cargaEntrega => cargaEntrega.CargaEntregaFinalizacaoAssincrona.SituacaoProcessamento == filtroPesquisa.SituacaoProcessamento);

            return consultaCargaEntregaFinalizacaoAssincrona;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> MontarFetchs(IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query)
        {
            query.Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
            .Fetch(obj => obj.MotivoRejeicao)
            .Fetch(obj => obj.MotivoRetificacaoColeta)
            .Fetch(obj => obj.Carga)
            .Fetch(obj => obj.ResponsavelFinalizacaoManual)
            .Fetch(obj => obj.MotivoAvaliacao);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ObterQueryPorCargaENotaFiscal(int codigoCarga, int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(cargaEntregaNotaFiscal =>
                        cargaEntregaNotaFiscal.CargaEntrega.Carga.Codigo == codigoCarga &&
                        cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal &&
                        !cargaEntregaNotaFiscal.CargaEntrega.Coleta
                );

            return query.Select(cargaEntregaNotaFiscal => cargaEntregaNotaFiscal.CargaEntrega);
        }

        #endregion

        #region Relatório

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas> ConsultarRelatorioParadas(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new ConsultaParadas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas>();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga> ConsultarRelatorioParadasWebService(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new ConsultaParadas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>();
        }

        public int ContarConsultaRelatorioParadas(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consulta = new ConsultaParadas().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AvaliacaoEntregaPedido>> ConsultarRelatorioAvaliacaoEntregaPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            var consulta = new ConsultaAvaliacaoEntregaPedido().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate)
                .SetResultTransformer(Transformers.AliasToBean<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AvaliacaoEntregaPedido>());

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AvaliacaoEntregaPedido>(cancellationToken);
        }

        public async Task<int> ContarConsultaRelatorioAvaliacaoEntregaPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, CancellationToken cancellationToken)
        {
            NHibernate.ISQLQuery consulta = new ConsultaAvaliacaoEntregaPedido().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consulta.SetTimeout(600).UniqueResultAsync<int>(cancellationToken);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega> ConsultarRelatorioControleEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new ConsultaControleEntrega().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega>();
        }

        public int ContarConsultaRelatorioControleEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consulta = new ConsultaControleEntrega().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas> ConsultarRelatorioConsolidadoEntregas(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new TorreControle.Consulta.ConsultaConsolidadoEntregas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas>();
        }

        public int ContarConsultaRelatorioConsolidadoEntregas(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consulta = new TorreControle.Consulta.ConsultaConsolidadoEntregas().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}