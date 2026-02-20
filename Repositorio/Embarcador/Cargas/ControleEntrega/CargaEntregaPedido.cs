using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaPedido : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>
    {
        #region Construtores

        public CargaEntregaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCarga(int carga)
        {
            var query01 = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaEntrega.Carga.Codigo == carga)
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();

            var query02 = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaEntrega.Carga.Codigo == carga)
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();
            foreach (var q1 in query01)
            {
                foreach (var q2 in query02.Where(obj => obj.CargaPedido.Codigo == q1.CargaPedido.Codigo))
                {
                    q1.CargaPedido.Pedido.Expedidor = q2.CargaPedido.Pedido.Expedidor;
                    q1.CargaPedido.Pedido.Recebedor = q2.CargaPedido.Pedido.Recebedor;
                    q1.CargaPedido.Pedido.Tomador = q2.CargaPedido.Pedido.Tomador;
                }
            }
            return query01;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaSemFetch(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);
            return result
                .Fetch(obj => obj.CargaEntrega)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEntregaCodigoCarga(int carga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == carga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargas(List<int> cargas)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(o => cargas.Contains(o.CargaEntrega.Carga.Codigo));

            return consultaCargaEntregaPedido
                .Fetch(o => o.CargaEntrega).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEntregas(IList<int> cargaEntregas)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(o => cargaEntregas.Contains(o.CargaEntrega.Codigo));

            return consultaCargaEntregaPedido
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido).ThenFetch(o => o.Destinatario)
                .ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEntregasFetchRemetente(List<int> cargaEntregas)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(o => cargaEntregas.Contains(o.CargaEntrega.Codigo));

            return consultaCargaEntregaPedido
                .Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido).ThenFetch(o => o.Remetente)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEntregasPendenteIntegracao(int inicio, int limite, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            var result = from obj in query
                         where
                            !obj.CargaEntrega.IntegradoERP && obj.CargaEntrega.Situacao == situacao
                         select obj;

            return result
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public int ContarPorCargaEntregasPendenteIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            var result = from obj in query
                         where
                            !obj.CargaEntrega.IntegradoERP && obj.CargaEntrega.Situacao == situacao
                         select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorCargaEntregaENotaFiscal(int codigoCargaEntrega, int codigoNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            var queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            result = result.Where(o => (from obj in queryPedidoXMLNotaFiscal where obj.XMLNotaFiscal.Codigo == codigoNotaFiscal select obj.CargaPedido.Codigo).Contains(o.CargaPedido.Codigo));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorCargaEntregaERemetenteDestinatario(int codigoCargaEntrega, double remetente, double destinatario)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == remetente && obj.CargaPedido.Pedido.Destinatario.CPF_CNPJ == destinatario);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarPorCargaPedido(List<int> cargaPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => cargaPedidos.Contains(obj.CargaPedido.Codigo));
            return result.Select(obj => obj.CargaEntrega).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorCargaPedido(int cargaPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaPedido.Codigo == cargaPedido);
            return result.Fetch(obj => obj.CargaEntrega).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarCargaEntregaPedidoPorCargaPedido(int cargaPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaPedido.Codigo == cargaPedido);
            return result.Fetch(obj => obj.CargaEntrega).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaPedidos(List<int> cargaPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => cargaPedidos.Contains(obj.CargaPedido.Codigo));

            return result
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.CargaPedido).ThenFetch(o => o.Pedido)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>> BuscarPorCargaPedidosAsync(List<int> cargaPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> resultadoFinal = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            int tamanhoLote = 1000;

            for (int i = 0; i < cargaPedidos.Count; i += tamanhoLote)
            {
                List<int> lote = cargaPedidos.Skip(i).Take(tamanhoLote).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                    .Where(obj => lote.Contains(obj.CargaPedido.Codigo))
                    .Fetch(obj => obj.CargaEntrega)
                    .Fetch(obj => obj.CargaPedido).ThenFetch(o => o.Pedido);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> resultadoLote = await query.ToListAsync();

                resultadoFinal.AddRange(resultadoLote);
            }

            return resultadoFinal;
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega);
            return result
                .Fetch(obj => obj.CargaEntrega)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorCargaEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.CargaPedido);
            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> BuscarPedidosPorNotasAsync(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            return await SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                .Where(x => codigos.Contains(x.Codigo))
                .Select(x => x.PedidoXMLNotaFiscal.CargaPedido)
                .ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroCargaPedidoPorCargaEntrega(int cargaEntrega)
        {
            var consultaCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(o => o.CargaEntrega.Codigo == cargaEntrega);

            return consultaCargaEntregaPedido.Select(o => o.CargaPedido).FirstOrDefault();
        }

        public int ObterVolumesTotais(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega);
            return result.Sum(obj => obj.CargaPedido.Pedido.QtVolumes);
        }

        public List<int> BuscarCodigosCargaPedidoPorCargaEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega);
            return result.Select(obj => obj.CargaPedido.Codigo).ToList();
        }

        public List<int> BuscarCodigosPedidoReentregaAutomaticaPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

            var result = query.Where(obj =>
                            obj.CargaEntrega.Codigo == codigoCargaEntrega &&
                            obj.CargaPedido.Pedido.ReentregaSolicitada &&
                            obj.CargaPedido.Pedido.TipoOperacao != null &&
                            obj.CargaPedido.Pedido.TipoOperacao.ConfiguracaoPedido.EnviarPedidoReentregaAutomaticamenteRoteirizar
            );

            return result.Select(obj => obj.CargaPedido.Pedido.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> BuscarPorCargaEPedidoPedido(int codigoCarga, int codigoPedido)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaPedido.CargaOrigem.Protocolo == codigoCarga && obj.CargaPedido.Pedido.Protocolo == codigoPedido);
            return result.Fetch(obj => obj.CargaEntrega).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarPedidosPorCargaEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.CargaPedido);
            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.GrupoPessoas)
                  .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.GrupoPessoas)
                  .Fetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.GrupoPessoas)
                  .Fetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.GrupoPessoas)
                  .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.CargaPedido.Pedido);
            return result
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.GrupoPessoas)
                .ToList();
        }
        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorEntregas(List<int> cargaEntregas)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => cargaEntregas.Contains(obj.CargaEntrega.Codigo)).Select(obj => obj.CargaPedido.Pedido);
            return result
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoSemColetaPacotePorCarga(int codigoCargaEspelho, int codigoCargaAtual)
        {
            var queryCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCargaEspelho
                                    && obj.CargaEntrega.Coleta
                                    && obj.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue
                                    && obj.CargaEntrega.Carga.TipoOperacao.ConfiguracaoControleEntrega != null
                                    && obj.CargaEntrega.Carga.TipoOperacao.ConfiguracaoControleEntrega.ExigirInformarNumeroPacotesNaColetaTrizy
                                    && obj.CargaEntrega.QuantidadePacotesColetados == 0
                ).Select(o => o.CargaPedido.Pedido.Codigo).Distinct();

            var queryCargaPedidoPacote = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCargaAtual)
                .Select(o => o.CargaPedido.Codigo).Distinct();

            var queryPedidoCTeParaSubContratacao = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCargaAtual)
                .Select(o => o.CargaPedido.Codigo).Distinct();

            var result = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().
                Where(obj => obj.Carga.Codigo == codigoCargaAtual
                          && queryCargaEntregaPedido.Contains(obj.Pedido.Codigo)
                          && !queryCargaPedidoPacote.Contains(obj.Codigo)
                          && !queryPedidoCTeParaSubContratacao.Contains(obj.Codigo)
                );

            return result.ToList();
        }

        public void ExcluirPorCarga(int carga)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaPedido obj WHERE obj.CargaEntrega in (select cargaEntrega.Codigo from CargaEntrega cargaEntrega where cargaEntrega.Carga.Codigo = :Carga)")
                             .SetInt32("Carga", carga)
                             .ExecuteUpdate();
        }

        public void ExcluirPorCargaPedido(int cargaEntregaPedido)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaEntregaPedido obj WHERE obj.CargaPedido = :CargaPedido")
                             .SetInt32("CargaPedido", cargaEntregaPedido)
                             .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ConsultarDocumentosPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaEntrega = ConsultaDocumentosPorCarga(codigoCarga);

            return ObterLista(consultaCargaEntrega, parametrosConsulta);
        }

        public int ContarConsultaDocumentosPorCarga(int codigoCarga)
        {
            var consultaCargaEntrega = ConsultaDocumentosPorCarga(codigoCarga);

            return consultaCargaEntrega.Count();
        }

        /// <summary>
        /// Metodo usado específicamente na montagem da carga, por favor nao usar.
        /// </summary>
        /// <param name="ListacargaPedido"></param>
        /// <param name="cargaEntrega"></param>
        public void InsertSQLListaCargaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ListacargaPedido, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (ListacargaPedido != null && ListacargaPedido.Count > 0 && cargaEntrega != null)
            {
                int take = 1000;
                int start = 0;

                while (start < ListacargaPedido.Count)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> Listatemp = ListacargaPedido.Skip(start).Take(take).ToList();

                    string parameros = "( :CEN_CODIGO_[X], :CPE_CODIGO_[X] )";
                    string sqlQuery = @"
                        INSERT INTO T_CARGA_ENTREGA_PEDIDO ( CEN_CODIGO, CPE_CODIGO) values " + parameros.Replace("[X]", "0");

                    for (int i = 1; i < Listatemp.Count; i++)
                        sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

                    var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                    query.SetParameter("CEN_CODIGO_0", cargaEntrega.Codigo);
                    query.SetParameter("CPE_CODIGO_0", Listatemp[0].Codigo);

                    for (int i = 1; i < Listatemp.Count; i++)
                    {
                        query.SetParameter("CEN_CODIGO_" + i.ToString(), cargaEntrega.Codigo);
                        query.SetParameter("CPE_CODIGO_" + i.ToString(), Listatemp[i].Codigo);
                    }

                    query.ExecuteUpdate();
                    start += take;
                }
            }
        }

        public int VerificarCargaEntregaPedidoPossuiDiferentesEntregas(int cargaPedido, int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaPedido.Codigo == cargaPedido && obj.CargaEntrega.Codigo != cargaEntrega);
            return result.Count();
        }

        public List<int> BuscarCodigosPedidosCorreiosPendenteEntrega(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> listaSituacoesEntrege, int numeroRegistrosPorVez)
        {
            List<int> retornoCodigos = new List<int>();

            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result = query.Where(obj => obj.CargaPedido.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
                                            && obj.CargaPedido.Pedido.NumeroRastreioCorreios != string.Empty && obj.CargaPedido.Pedido.NumeroRastreioCorreios != null
                                            && !listaSituacoesEntrege.Contains(obj.CargaEntrega.Situacao)
                                            && obj.CargaPedido.Pedido.DataUltimaConsultaCorreios == null
                                            && obj.CargaPedido.Pedido.Empresa.CodigoServicoCorreios != null && obj.CargaPedido.Pedido.Empresa.CodigoServicoCorreios != string.Empty);

            List<int> codigosSemDataConsulta = result.Skip(0).Take(numeroRegistrosPorVez).Select(o => o.CargaPedido.Pedido.Codigo).ToList();

            var query2 = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            var result2 = query2.Where(obj => obj.CargaPedido.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
                                            && obj.CargaPedido.Pedido.NumeroRastreioCorreios != string.Empty && obj.CargaPedido.Pedido.NumeroRastreioCorreios != null
                                            && !listaSituacoesEntrege.Contains(obj.CargaEntrega.Situacao)
                                            && DateTime.Now.AddMinutes(-30) >= obj.CargaPedido.Pedido.DataUltimaConsultaCorreios
                                            && obj.CargaPedido.Pedido.Empresa.CodigoServicoCorreios != null && obj.CargaPedido.Pedido.Empresa.CodigoServicoCorreios != string.Empty);

            List<int> codigosComDataConsulta = result2.Skip(0).Take(numeroRegistrosPorVez).Select(o => o.CargaPedido.Pedido.Codigo).ToList();

            retornoCodigos.AddRange(codigosSemDataConsulta);
            retornoCodigos.AddRange(codigosComDataConsulta);

            return retornoCodigos;
        }

        public List<int> BuscarCodigosPedidosPorCargaEntrega(int cargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaEntrega.Codigo == cargaEntrega).Select(obj => obj.CargaPedido.Pedido.Codigo);

            return query.ToList();
        }

        public List<(int CodigoCargaEntrega, int CodigoCargaPedido)> BuscarCodigoCargaEntregaECodigoCargaPedidosPorCargaEntrega(List<int> codigosCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => codigosCargaEntrega.Contains(obj.CargaEntrega.Codigo));

            return query
                .Select(o => ValueTuple.Create(o.CargaEntrega.Codigo, o.CargaPedido.Codigo))
                .ToList();
        }

        public decimal ObterPrecoUnitarioPedidoProdutoPorCargaEntrega(int codigoCargaEntrega, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            var result = query.Where(obj => obj.Produto.Codigo == codigoProduto);

            var queryCargaEntregaPedido = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            result = result.Where(o => queryCargaEntregaPedido.Where(p => p.CargaEntrega.Codigo == codigoCargaEntrega && p.CargaPedido.Pedido.Codigo == o.Pedido.Codigo).Any());

            return result.Sum(o => (decimal?)o.PrecoUnitario) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorProtocoloPedido(int protocoloPedido)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .OrderByDescending(e => e.Codigo)
                .FirstOrDefault(obj => obj.CargaPedido.Pedido.Protocolo == protocoloPedido);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido BuscarPorProtocoloPedido(int protocoloPedido, bool finalizado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => obj.CargaPedido.Pedido.Protocolo == protocoloPedido);

            if (finalizado)
                query = query.Where(o => o.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue);
            else
                query = query.Where(o => o.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida &&
                o.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado);

            //LastOrDefault não funciona no NHibernate
            return query.OrderByDescending(e => e.Codigo).FirstOrDefault();
        }

        public bool ExistePedidoAindaNaoEntregue(List<int> codigosPedidos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(obj => codigosPedidos.Contains(obj.CargaPedido.Pedido.Codigo));
            query = query.Where(o => o.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue && o.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida && o.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado);

            return query.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> ConsultaDocumentosPorCarga(int codigoCarga)
        {
            var consultaCargaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>()
                .Where(o => o.CargaEntrega.Carga.Codigo == codigoCarga);

            return consultaCargaEntrega;
        }

        #endregion
    }
}
