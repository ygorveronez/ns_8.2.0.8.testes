using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>
    {
        #region Construtores

        public CargaEntregaNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaEntregaNotaFiscal(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal BuscarPorCargaENFe(int carga, int xmlNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == xmlNotaFiscal && !obj.CargaEntrega.Coleta);
            return result.Fetch(obj => obj.CargaEntrega).FirstOrDefault();
        }

        public async Task<bool> BuscarPorCargaLiberacaoEntregaAsync(int codigoCarga, int codigoCargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga &&
                       obj.CargaEntrega.Situacao != SituacaoEntrega.Entregue &&
                       !obj.CargaEntrega.Coleta &&
                       obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Canhoto != null &&
                       obj.CargaEntrega.Codigo == codigoCargaEntrega
                );


            bool existeNaoConforme = await query.AnyAsync(obj =>
                obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Canhoto.SituacaoCanhoto != SituacaoCanhoto.Justificado &&
                obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado, CancellationToken);

            bool existemRegistros = await query.AnyAsync(CancellationToken);

            return existemRegistros && !existeNaoConforme;
        }

        //public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaEPedido(int carga, int cargaPedido)
        //{
        //    var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
        //    var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga
        //    && obj.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
        //    && obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido && !obj.CargaEntrega.Coleta);
        //    return result
        //        .Fetch(obj => obj.CargaEntrega)
        //        .Fetch(obj => obj.PedidoXMLNotaFiscal)
        //        .ThenFetch(obj => obj.XMLNotaFiscal)
        //        .ThenFetch(obj => obj.Canhoto)
        //        .ToList();
        //}

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarTodasPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarTodasCargaEntregaPorXMLNotasFiscais(List<int> codigosXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => codigosXMLNotaFiscal.Contains(obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo));
            return result
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public void ExcluirCargaEntregaNotaFiscalPorCargaPedido(int codigoCargaPedido)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaNotaFiscal obj WHERE obj.PedidoXMLNotaFiscal in (SELECT pedidoXMLNotaFiscal.Codigo from PedidoXMLNotaFiscal pedidoXMLNotaFiscal WHERE pedidoXMLNotaFiscal.CargaPedido.Codigo = :CargaPedido)")
                             .SetInt32("CargaPedido", codigoCargaPedido)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosPorCarga(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaNotaFiscal obj WHERE obj.CargaEntrega in (select cargaEntrega.Codigo from CargaEntrega cargaEntrega where cargaEntrega.Carga.Codigo = :Carga)")
                             .SetInt32("Carga", codigoCarga)
                             .SetTimeout(120)
                             .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> BuscarNotaFiscalPorCargaEntrega(int cargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.PedidoXMLNotaFiscal);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto).ToList();
        }

        public decimal BuscarPesoPorCargaEntregaNumerosNotas(int cargaEntrega, List<int> numerosNotas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<decimal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega && numerosNotas.Contains(obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero)).Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso);
            return result.Sum();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTePorCargaEntrega(int cargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.PedidoXMLNotaFiscal);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> queryCTe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryCTe = queryCTe.Where(c => result.Any(p => p == c.PedidoXMLNotaFiscal));

            return queryCTe.Where(c => c.CargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal).Select(c => c.CargaCTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaEntrega(int cargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega);
            return result.Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaEntregaComLimiteDeRegistros(int cargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega);
            return result.Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto).Take(100).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarNotasDevolucaoTotalPorCargaEntrega(int cargaEntrega, int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega &&
                                            obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Devolvida &&
                                            (obj.Chamado == null || obj.Chamado.Codigo == codigoChamado));

            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal BuscarPorCargaENumeroNotaFiscal(int codigoCarga, List<int> numerosNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga && numerosNotaFiscal.Contains(obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero) && !obj.CargaEntrega.Coleta);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);
            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Filial)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaFetchSimples(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);
            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }
        public Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>> BuscarPorCargasFetchSimplesAsync(List<int> cargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => cargas.Contains(obj.CargaEntrega.Carga.Codigo));
            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ToListAsync();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaSemFetch(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaEntregas(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            query = query.Where(obj => codigos.Contains(obj.CargaEntrega.Codigo));

            return query
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarCargaPedidoPorNotas(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            return SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(x => codigos.Contains(x.Codigo))
                .Fetch(x => x.PedidoXMLNotaFiscal)
                .ThenFetch(pxn => pxn.XMLNotaFiscal)
                .Fetch(x => x.PedidoXMLNotaFiscal)
                .ThenFetch(pxn => pxn.CargaPedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCargaEntregaCodigoCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorNumeroNotaFiscalESerie(int numeroNota, string serie, double CNPJEmissor = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            result = result.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNota && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie == serie && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true);

            if (CNPJEmissor > 0)
                result = result.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ == CNPJEmissor);

            result = result.Where(obj => !obj.CargaEntrega.Coleta);

            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.CargaEntrega).ThenFetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarPorCodigoNotaFiscal(int codigoNota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            result = result.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNota);

            return result
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> BuscarNotasDevolucaoTotalOuParcialPorCargaEntrega(int cargaEntrega, int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega &&
                                            (obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Devolvida ||
                                            obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.DevolvidaParcial) &&
                                            (obj.Chamado == null || obj.Chamado.Codigo == codigoChamado));

            return result.ToList();
        }

        public bool ExisteCanhotosPendentesPorCargaEntrega(int codigoCargaEntrega, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> consultaCargaEntregaNotaFiscal = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            consultaCargaEntregaNotaFiscal = consultaCargaEntregaNotaFiscal.Where(obj => consultaCanhoto.Where(canhoto =>
                ((canhoto.TipoCanhoto == TipoCanhoto.NFe && canhoto.XMLNotaFiscal.Codigo == obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) ||
                (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso != null && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == obj.PedidoXMLNotaFiscal.Codigo)))
                && canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.AgAprovocao && canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado
                ).Any());

            return consultaCargaEntregaNotaFiscal.Any();
        }

        /// <summary>
        /// Metodo usado específicamente na criacao das entregas, nao utilizar.
        /// </summary>
        /// <param name="ListaCargaEntregaNotaFiscal"></param>
        /// <param name="cargaEntrega"></param>
        public async Task InsertSQLListaCargaEntregaPedido(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> ListaCargaEntregaNotaFiscal, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (ListaCargaEntregaNotaFiscal != null && ListaCargaEntregaNotaFiscal.Count > 0 && cargaEntrega != null)
            {
                int take = 1000;
                int start = 0;

                while (start < ListaCargaEntregaNotaFiscal.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> Listatemp = ListaCargaEntregaNotaFiscal.Skip(start).Take(take).ToList();

                    string parameros = "( :CEN_CODIGO_[X], :PNF_CODIGO_[X] )";
                    string sqlQuery = @"
                        INSERT INTO T_CARGA_ENTREGA_NOTA_FISCAL ( CEN_CODIGO, PNF_CODIGO) values " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < Listatemp.Count; i++)
                        sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                    NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                    query.SetParameter("CEN_CODIGO_0", cargaEntrega.Codigo);
                    query.SetParameter("PNF_CODIGO_0", Listatemp[0].PedidoXMLNotaFiscal.Codigo);

                    for (int i = 1; i < Listatemp.Count; i++)
                    {
                        query.SetParameter("CEN_CODIGO_" + i.ToString(), cargaEntrega.Codigo);
                        query.SetParameter("PNF_CODIGO_" + i.ToString(), Listatemp[i].PedidoXMLNotaFiscal.Codigo);
                    }

                    await query.ExecuteUpdateAsync();
                    start += take;
                }
            }
        }


        public void DeletarSQLListaCargaEntregaPedido(List<int> CodigoListaCargaEntregaNotaFiscal)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaNotaFiscal obj WHERE obj.Codigo in (:CEF_CODIGO)")
                                     .SetParameterList("CEF_CODIGO", CodigoListaCargaEntregaNotaFiscal)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaNotaFiscal obj WHERE obj.Codigo in (:CEF_CODIGO)")
                                     .SetParameterList("CEF_CODIGO", CodigoListaCargaEntregaNotaFiscal)
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

        #endregion
    }
}
