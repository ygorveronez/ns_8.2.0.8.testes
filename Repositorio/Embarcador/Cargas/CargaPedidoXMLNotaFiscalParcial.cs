using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoXMLNotaFiscalParcial : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>
    {
        public CargaPedidoXMLNotaFiscalParcial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarParaVincularPorProcesso(int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = query.Where(o => o.VincularNotaFiscalPorProcesso &&
                                     o.XMLNotaFiscal == null &&
                                     (!o.DataUltimaTentativaVinculo.HasValue || o.DataUltimaTentativaVinculo.Value < DateTime.Now.AddMinutes(-5)) &&
                                     o.NumeroTentativasVinculo < 60 &&
                                    (o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                     o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                               );

            return query.OrderBy(o => o.Codigo).Fetch(o => o.CargaPedido).ThenFetch(o => o.Pedido).ThenFetch(o => o.Remetente).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorNumeroOuNumeroPedidoSemCarga(int numero, int numeroPedido, string numeroDT, double emitenteNF, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            query = query.Where(obj => ((obj.Numero == numero) || (obj.Chave == chave) || (obj.Pedido == numeroDT && numero == 0) || (obj.Numero == numeroPedido && numeroPedido > 0)) &&
                                (obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova ||
                                obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete));

            if (emitenteNF > 0)
                query = query.Where(obj => obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == emitenteNF);

            return query
                .Timeout(7000)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            query = query.Where(obj => obj.Chave == chave &&
                                (obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova));

            return query
                .Timeout(7000)
                .Fetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorNumeroSemCarga(int numero, double cpfCnpjEmitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = query.Where(o => o.CargaPedido.Pedido.Remetente.CPF_CNPJ == cpfCnpjEmitente && o.Numero == numero && o.XMLNotaFiscal == null && (o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe || o.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova));

            return query.Fetch(obj => obj.CargaPedido)
                        .ThenFetch(obj => obj.Carga)
                        .ThenFetch(obj => obj.TipoOperacao)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial BuscarNotaParcialPorNumero(int numero, double emitenteNF, double expedidor, double recebedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            var result = (from obj in query where obj.Numero == numero && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj);

            if (emitenteNF > 0)
                result = result.Where(obj => obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == emitenteNF);

            if (expedidor > 0)
                result = result.Where(obj => obj.CargaPedido.Expedidor.CPF_CNPJ == expedidor);
            else
                result = result.Where(obj => obj.CargaPedido.Expedidor == null);

            if (recebedor > 0)
                result = result.Where(obj => obj.CargaPedido.Recebedor.CPF_CNPJ == recebedor);
            else
                result = result.Where(obj => obj.CargaPedido.Recebedor == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial BuscarPrimeiroPorCargaPedidoENumero(int codigoCargaPedido, int numero)
        {
            var consultaNotaFiscalParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>()
                .Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido && obj.Numero == numero);

            return consultaNotaFiscalParcial.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial BuscarPorNumeroFatura(string numeroFatura)
        {
            var consultaNotaFiscalParcial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>()
                .Where(obj => obj.NumeroFatura == numeroFatura);

            return consultaNotaFiscalParcial.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial BuscarPorNumeroOuNumeroPedidoECargaPedido(int numero, int numeroPedido, string numeroDT, double emitenteNF, int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            var result = from obj in query
                         where (obj.Numero == numero || (obj.Pedido == numeroDT && numero == 0) || (obj.Numero == numeroPedido && numeroPedido > 0)) && obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null &&
                                (obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                obj.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                         )
                         select obj;

            if (emitenteNF > 0)
                result = result.Where(obj => obj.CargaPedido.Pedido.Remetente.CPF_CNPJ == emitenteNF);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> ConsultarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = from obj in query where obj.CargaPedido.Codigo == cargaPedido select obj;

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = from obj in query where obj.CargaPedido.Carga.Protocolo == codigoCarga select obj;

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<int> BuscarNumerosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>()
                .Where(obj => obj.CargaPedido.Carga.Codigo == codigoCarga);

            return query
                .OrderBy(obj => obj.Codigo)
                .Select(obj => obj.Numero)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> BuscarPorCargasPedido(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = from obj in query where codigos.Contains(obj.CargaPedido.Codigo) select obj;

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> ConsultarPorNFe(int codigoNFe, int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.XMLNotaFiscal.Codigo == codigoNFe && obj.CargaPedido.Codigo == cargaPedido select obj;

            return result.ToList();

        }

        public List<int> BuscarCodigoNotasPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == carga && obj.XMLNotaFiscal != null select obj;

            return result.Select(obj => obj.XMLNotaFiscal.Codigo).ToList();

        }

        public bool VerificarSeExisteNotaParcialSemNotaParaCargaPedido(int cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido && o.XMLNotaFiscal == null && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe.Autorizado);
            return query.Any();
        }

        public List<int> BuscarCodigoPedidoQueExisteNotaParcialSemNotaParaCargaPedido(int carga)
        {
            // seguir regras de negocio do metodo VerificarSeExisteNotaParcialSemNotaParaCargaPedido
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal == null && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusNfe.Autorizado);
            return query.Select(obj => obj.CargaPedido.Codigo).ToList();
        }



        public bool VerificarSeExisteNotaParcialSemNota(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga && o.XMLNotaFiscal == null);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> ConsultarSemNota(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null select obj;

            return result.ToList();

        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>> ConsultarSemNotaAsync(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null select obj;

            return result.ToListAsync();

        }

        public bool ExisteCargaParcialSemNotaParaEstaCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal == null select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial ConsultarPorChave(int cargaPedido, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.Chave == chave select obj;

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> Consultar(int cargaPedido, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            var result = from obj in query where obj.CargaPedido.Codigo == cargaPedido && obj.XMLNotaFiscal == null select obj;

            return result.Count();
        }

        public void ReprocessarNotasParciaisPorCarga(int codigoCarga)
        {
            string sqlQuery = "UPDATE CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial SET cargaPedidoXMLNotaFiscalParcial.NumeroTentativasVinculo = 0 WHERE cargaPedidoXMLNotaFiscalParcial.Codigo IN (SELECT c.Codigo FROM CargaPedidoXMLNotaFiscalParcial c WHERE c.CargaPedido.Carga.Codigo =: codigoCarga AND c.XMLNotaFiscal = null)";

            if (UnitOfWork.IsActiveTransaction())
                UnitOfWork.Sessao.CreateQuery(sqlQuery).SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery(sqlQuery).SetInt32("codigoCarga", codigoCarga).ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public bool PossuiPorCargaPedidoENotaParcial(int codigoCargaPedido, int numero)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.Numero == numero);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial BuscarPorChaveNota(string chaveNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();

            query = query.Where(nfe => nfe.Chave == chaveNota);

            return query.Fetch(x => x.CargaPedido).ThenFetch(x => x.Carga).FirstOrDefault();
        }

        #endregion
    }
}
