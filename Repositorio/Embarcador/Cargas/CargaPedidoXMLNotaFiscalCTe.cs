using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoXMLNotaFiscalCTe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>
    {
        #region Construtores

        public CargaPedidoXMLNotaFiscalCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPedidoXMLNotaFiscalCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<string> BuscarNumeroPedidosPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(o => o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador).Distinct().ToList();
        }

        public string BuscarPrimeiroNumeroPedidoPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return query.Select(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador).FirstOrDefault();
        }

        public List<(int, int, SituacaoDigitalizacaoCanhoto)> BuscarDadosCanhotos(List<int> codigoCargas)
        {
            var queri = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            var codigos = queri.Where(x => codigoCargas.Contains(x.Carga.Codigo));

            return codigos.Select(x => ValueTuple.Create(x.Codigo, x.Carga.Codigo, x.SituacaoDigitalizacaoCanhoto)).ToList();
        }

        public List<(int, int)> BuscarDadosMiro(List<int> codigoCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMiro>();

            var codigos = query.Where(o => codigoCTes.Contains(o.CargaCTe.CTe.Codigo)).Select(x => x.PedidoXMLNotaFiscal.Codigo);
            subQuery = subQuery.Where(x => codigos.Contains(x.PedidoXMLNotaFiscal.Codigo));

            return subQuery.Select(x => ValueTuple.Create(x.Codigo, x.PedidoXMLNotaFiscal.CargaPedido.Codigo)).ToList();
        }

        public int ContarCTEsPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);
            return query.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalCTe> BuscarCTEsPorCarga(int carga)
        {
            var queryCargaCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>()
                .Where(o => o.Carga.Codigo == carga &&
                (o.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.TodosDocumentos
                || o.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Documentos))
                .FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == carga && queryCargaCancelamento == null);

            return query.Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoXMLNotaFiscalCTe()
            {
                CargaCTe = obj.CargaCTe.Codigo,
                CargaPedido = obj.PedidoXMLNotaFiscal.CargaPedido.Codigo
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarCargaPedidoXMLNotaFiscalCTePorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == carga);
            return query
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.CargaPedido)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal BuscarXMLNotasFiscaisPorCTeENotaFiscal(int codigoCTe, int xmlNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(o => o.CargaCTe.CTe.Codigo == codigoCTe && o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == xmlNotaFiscal);

            return query.Select(o => o.PedidoXMLNotaFiscal).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotasFiscaisPorCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true);
            var result = query.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal);

            return result
                .Fetch(obj => obj.Canhoto)
                .Fetch(obj => obj.Emitente)
                .Fetch(obj => obj.Destinatario)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotasFiscaisPorCTeEPedido(int codigoCargaCTe, int CodigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.PedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo == CodigoPedido select obj;
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true);
            var result = query.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal);

            return result
                .Fetch(obj => obj.Canhoto)
                .Fetch(obj => obj.Emitente)
                .Fetch(obj => obj.Destinatario)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarXMLNotasFiscaisPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe select obj;
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true);
            var result = query.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal);

            return result
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaCargaPedidoPorCargaCTes(List<int> codigosCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true
                             && codigosCargaCte.Contains(obj.CargaCTe.Codigo)
                         select obj.PedidoXMLNotaFiscal.CargaPedido;

            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigosCargaPedidoPorCargaCTes(List<int> codigosCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true
                             && codigosCargaCte.Contains(obj.CargaCTe.Codigo)
                         select obj.PedidoXMLNotaFiscal.CargaPedido.Codigo;


            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargaPedidoPorCargaCTe(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true
                             && obj.CargaCTe.Codigo == codigoCargaCte
                         select obj.PedidoXMLNotaFiscal.CargaPedido;


            return result.Distinct()
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.CanalEntrega)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.NotasFiscais)
                .ToList();
        }

        public List<int> BuscarCodigosCargaPedidoPorCargaCTe(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva
                             && obj.CargaCTe.Codigo == codigoCargaCte
                         select obj.PedidoXMLNotaFiscal.CargaPedido.Codigo;


            return result.Distinct().ToList();
        }

        public List<int> BuscarStagePorCodigoCTe(List<int> codigoCtes, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var pedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true
                             && codigoCtes.Contains(obj.CargaCTe.CTe.Codigo)
                             && obj.PedidoXMLNotaFiscal.CargaPedido.CTesEmitidos == true
                             && obj.CargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                             && obj.CargaCTe.Carga.Codigo == codigoCarga
                         select obj.PedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo;

            pedidoStage = pedidoStage.Where(x => result.Contains(x.Pedido.Codigo));

            return pedidoStage.Select(x => x.Stage.Codigo).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarCargaCtePorStage(int codigoStage)
        {
            var pedidoStage = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoStage>();
            var consultaCargaPedidoCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            pedidoStage = pedidoStage.Where(s => s.Stage.Codigo == codigoStage);
            consultaCargaPedidoCte = consultaCargaPedidoCte.Where(x => pedidoStage.Any(o => o.Pedido.Codigo == x.PedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo) && x.CargaCTe != null);
            return consultaCargaPedidoCte.Select(c => c.CargaCTe).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiraCargaPedidoPorCargaCTe(int codigoCargaCte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva == true && obj.CargaCTe.Codigo == codigoCargaCte);

            return query.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.CanalEntrega)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .FirstOrDefault();
        }

        public Dominio.Enumeradores.TipoPagamento BuscarTipoPagamentoPedidoPorCTe(int codigoCte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(obj => obj.CargaCTe.CTe.Codigo == codigoCte);

            return query.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Pedido.TipoPagamento).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarTodosCargaPedidoXMLNotaFiscalCTePorCargaCTe(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCte select obj;

            return result
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTe(List<int> codigosCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query where codigosCargaCte.Contains(obj.CargaCTe.Codigo) select obj;

            return result
                .Fetch(obj => obj.CargaCTe)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>> BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTeAsync(List<int> codigosCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query where codigosCargaCte.Contains(obj.CargaCTe.Codigo) select obj;

            return result
                .Fetch(obj => obj.CargaCTe)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarCargaPedidoPorCodigoCte(List<int> codigosCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query where codigosCargaCte.Contains(obj.CargaCTe.CTe.Codigo) select obj.PedidoXMLNotaFiscal.CargaPedido;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarCargaPedidoXMLNotaFiscalCTePorCTe(int codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.CargaCTe.CTe.Status == "A"
                             && obj.CargaCTe.CTe.Codigo == codigoCte
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarCargaPedidoXMLNotaFiscalCTePorCargaCTe(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.CargaCTe.CTe.Status == "A"
                             && obj.CargaCTe.Codigo == codigoCargaCte
                         select obj;

            return result.ToList();
        }

        public int ContarNFSePorCargaPedido(int cargaPedido, int carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            if (carga > 0)
                query = query.Where(obj => obj.CargaCTe.Carga.Codigo == carga);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                query = query.Where(obj => obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe);
            else
                query = query.Where(obj => obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe
                || obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS
                );

            if (configWebService?.NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs ?? false)
                query = query.Where(obj => obj.CargaCTe.LancamentoNFSManual == null);

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result.Distinct().Count();
        }

        public int ContarNFSePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.CargaCTe.Carga.Codigo == codigoCarga && obj.CargaCTe.CTe.ModeloDocumentoFiscal.Numero == "39");

            return query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe).Distinct().Count();
        }

        public int ContarCTesPorCargaPedidoMultiModal(List<int> cargaPedido, List<int> carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryCTesPorCargaPedidoMultiModal(cargaPedido, carga, remetente, destinatario, semComplementos, somenteCTes);

            var result = query.OrderBy(obj => obj.Codigo).Select(obj => obj);
            return result.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarNFSePorCargaPedido(int cargaPedido, int carga, int inicioRegistros, int maximoRegistros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (carga > 0)
                queryCargaCte = queryCargaCte.Where(obj => obj.Carga.Codigo == carga);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                queryCargaCte = queryCargaCte.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe);
            else
                queryCargaCte = queryCargaCte.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS);

            if (configWebService?.NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs ?? false)
                queryCargaCte = queryCargaCte.Where(obj => obj.LancamentoNFSManual == null);

            queryCargaCte = queryCargaCte.Where(obj => query.Any(x => x.CargaCTe.Codigo == obj.Codigo));

            return queryCargaCte.OrderBy(obj => obj.Codigo)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarNFSePorCarga(int codigoCarga, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCte = queryCargaCte.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CTe.ModeloDocumentoFiscal.Numero == "39");
            queryCargaCte = queryCargaCte.Where(obj => query.Any(x => x.CargaCTe.Codigo == obj.Codigo));

            return queryCargaCte.OrderBy(obj => obj.Codigo)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.CentroResultado)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.CentroResultadoDestinatario)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Expedidor)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Recebedor)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.OutrosTomador)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.TomadorPagador)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.TomadorPagador)
                        .ThenFetch(obj => obj.GrupoPessoas)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                        .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarNFSePorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, int inicioRegistros, int maximoRegistros, string codigoTipoOperacao, string situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = from obj in query
                    where
                       obj.CTe.DataEmissao >= dataInicial.Date
                       && obj.CTe.DataEmissao <= dataFinal.Date.AddDays(1)
                       && (obj.CTe.ModeloDocumentoFiscal.Numero == "39" || obj.CTe.ModeloDocumentoFiscal.Numero == "24" || obj.CTe.ModeloDocumentoFiscal.Numero == "NF")
                       && (obj.CTe.Status == "C" || obj.CTe.Status == "A")
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(situacao))
                query = query.Where(obj => obj.CTe.Status == situacao);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(obj => obj.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            var result = query.OrderBy(obj => obj.CTe.Codigo);

            return result
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Fetch(o => o.CTe)
                .ThenFetch(o => o.Serie)
                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarNFSePorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, string codigoTipoOperacao, string situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = from obj in query
                    where
                       obj.CTe.DataEmissao >= dataInicial.Date
                       && obj.CTe.DataEmissao <= dataFinal.Date.AddDays(1)
                       && (obj.CTe.ModeloDocumentoFiscal.Numero == "39" || obj.CTe.ModeloDocumentoFiscal.Numero == "24" || obj.CTe.ModeloDocumentoFiscal.Numero == "NF")
                       && (obj.CTe.Status == "C" || obj.CTe.Status == "A")
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(situacao))
                query = query.Where(obj => obj.CTe.Status == situacao);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(obj => obj.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            return query.Distinct().Count();
        }

        public int ContarAverbacaoCTesPorCargaPedido(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos)
        {
            var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            if (carga > 0)
                query = query.Where(obj => obj.CargaCTe.Carga.Codigo == carga);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            if (remetente > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);

            if (semComplementos)
                query = query.Where(obj => obj.CargaCTe.CargaCTeComplementoInfo == null);

            query = query.Where(obj => queryAverbacao.Any(o => o.CTe == obj.CargaCTe.CTe));

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarAverbacaoCTesPorCargaPedido(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos, int inicioRegistros, int maximoRegistros)
        {
            var queryAverbacao = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            if (carga > 0)
                query = query.Where(obj => obj.CargaCTe.Carga.Codigo == carga);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            if (remetente > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);

            if (semComplementos)
                query = query.Where(obj => obj.CargaCTe.CargaCTeComplementoInfo == null);


            query = query.Where(obj => queryAverbacao.Any(o => o.CTe == obj.CargaCTe.CTe));

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarCTesPorCargaPedido(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.CargaCTe.CTe != null);

            if (carga > 0)
                query = query.Where(obj => obj.CargaCTe.CargaOrigem.Codigo == carga);

            if (cargaPedido > 0)
                query = query.Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido);

            if (remetente > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);

            if (semComplementos)
                query = query.Where(obj => obj.CargaCTe.CargaCTeComplementoInfo == null);

            if (somenteCTes)
                query = query.Where(obj => obj.CargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57");

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result.Distinct().Count();
        }

        public int ContarCTesPorCargaPedido(IList<int> cargaPedidos, IList<int> cargas, double remetente, double destinatario, bool semComplementos, bool somenteCTes)
        {
            StringBuilder sql = ObterSQLBuscarCargaCtePorCargaPedidos("select count(distinct CargaCte.CCT_CODIGO)", cargaPedidos, cargas, remetente, destinatario, semComplementos, somenteCTes);

            NHibernate.ISQLQuery consultaCodigosCargaCte = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            return consultaCodigosCargaCte.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorCargaPedido(int codigoCargaPedido, int codigoCargaOrigem)
        {
            var consultaCargaPedidoXMLNotaFiscalCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(o =>
                    o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido &&
                    o.CargaCTe.CTe != null &&
                    o.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe &&
                    o.CargaCTe.CargaOrigem.Codigo == codigoCargaOrigem &&
                    o.CargaCTe.CargaCTeComplementoInfo == null
                );

            return consultaCargaPedidoXMLNotaFiscalCTe
                .OrderBy(o => o.CargaCTe.Codigo)
                .Select(o => o.CargaCTe)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                .Fetch(o => o.CTe).ThenFetch(o => o.Empresa)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorCargaPedido(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes, int inicioRegistros, int maximoRegistros)
        {
            IList<int> codigosCargaCte = BuscarCodigosCargaCtePorCargaPedido(cargaPedido, carga, remetente, destinatario, semComplementos, somenteCTes, inicioRegistros, maximoRegistros);
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> resultCte = queryCte.Where(obj => codigosCargaCte.Contains(obj.Codigo));

            return resultCte
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                 .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorCargaPedidoMultiModal(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryCTesPorCargaPedidoMultiModal(new List<int> { cargaPedido }, new List<int> { carga }, remetente, destinatario, semComplementos, somenteCTes);

            var result = query.OrderBy(obj => obj.Codigo).Select(obj => obj);
            return result
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Distinct()
                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, int inicioRegistros, int maximoRegistros, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS, bool considerarHora = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryBuscarCTesPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS, considerarHora);

            query = query.OrderBy(obj => obj.CTe.Codigo);

            return query
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Fetch(o => o.CTe)
                .ThenFetch(o => o.Serie)

                .Skip(inicioRegistros).Take(maximoRegistros).Timeout(120).ToList();
        }

        public int ContarCTesPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS, bool considerarHora = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryBuscarCTesPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS, considerarHora);

            query = query.OrderBy(obj => obj.CTe.Codigo);

            return query.Count();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesAlteradosPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, int inicioRegistros, int maximoRegistros, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryBuscarCTesAlteradosPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS);

            query = query.OrderBy(obj => obj.CTe.Codigo);

            return query
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Fetch(o => o.CTe)
                .ThenFetch(o => o.Serie)

                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
        public int ContarCTesAlteradosPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryBuscarCTesAlteradosPorPeriodoEmpresa(dataInicial, dataFinal, empresa, tomador, somentePosIntegracao, codigoTipoOperacao, situacao, configuracaoWS);

            query = query.OrderBy(obj => obj.CTe.Codigo);

            return query.Count();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarOutrosDocsPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = from obj in query
                    where
                       obj.CargaCTe.CTe.DataEmissao.Value.Date >= dataInicial.Date
                       && obj.CargaCTe.CTe.DataEmissao.Value.Date <= dataFinal.Date
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Empresa.Codigo == empresa);

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Distinct().Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarOutrosDocsPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = from obj in query
                    where
                       obj.CargaCTe.CTe.DataEmissao.Value.Date >= dataInicial.Date
                       && obj.CargaCTe.CTe.DataEmissao.Value.Date <= dataFinal.Date
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS
                       && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.CargaCTe.CTe.Empresa.Codigo == empresa);

            var result = query.OrderBy(obj => obj.CargaCTe.Codigo).Select(obj => obj.CargaCTe);
            return result.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTesPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeFilialEmissora == null
                         && obj.CargaCTe.CTe.Status == "A"
                         && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                         select obj.CargaCTe;

            return result.Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> BuscarCargaCTesPorCargaPedidoAsync(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null &&
                         obj.CargaCTe.CargaCTeFilialEmissora == null &&
                         obj.CargaCTe.CTe.Status == "A" &&
                         obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> result = query.Select(obj => obj.CargaCTe).Distinct();

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido
                         select obj;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaPedido(List<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(o => codigosCargaPedido.Contains(o.PedidoXMLNotaFiscal.CargaPedido.Codigo) && o.CargaCTe.CTe != null && o.CargaCTe.CTe.Status == "A");

            return query
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe)
                .Fetch(o => o.PedidoXMLNotaFiscal).ThenFetch(o => o.CargaPedido)
                .OrderByDescending(o => o.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaNaoReplicados(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga && !obj.CargaCTe.ReplicadoCargaFilho
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaCTes(List<int> codigosCargaCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigosCargaCTes.Contains(obj.CargaCTe.Codigo)
                         select obj;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorCargaCTe(int codigsCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.CargaCTe.Codigo == codigsCargaCTe
                         select obj;

            return result.Distinct().ToList();
        }

        public List<int> BuscarNotasPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.CargaCTe.Codigo == codigoCargaCTe
                         select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query where obj.CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == ocorrencia select obj;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTesPorCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigoCargaPedidos.Contains(obj.PedidoXMLNotaFiscal.CargaPedido.Codigo) && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeFilialEmissora == null
                         && obj.CargaCTe.CTe.Status == "A"
                         && (obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ||
                             obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ||
                             obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                         select obj.CargaCTe;

            return result.Distinct().ToList();
        }

        public List<int> BuscarCodigosSemFilialEmissoraCTePorCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigoCargaPedidos.Contains(obj.PedidoXMLNotaFiscal.CargaPedido.Codigo) && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null
                         && obj.CargaCTe.CTe.Status == "A"
                         && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                         select obj.CargaCTe;

            return result.Select(obj => obj.CTe.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosPreCTesSemFilialEmissoraCTePorCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigoCargaPedidos.Contains(obj.PedidoXMLNotaFiscal.CargaPedido.Codigo) && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null
                         && obj.CargaCTe.CTe == null
                         && obj.CargaCTe.PreCTe != null
                         select obj.CargaCTe;

            return result.Select(obj => obj.PreCTe.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosCTePorCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigoCargaPedidos.Contains(obj.PedidoXMLNotaFiscal.CargaPedido.Codigo) && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeFilialEmissora == null
                         && obj.CargaCTe.CTe.Status == "A"
                         && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                         select obj.CargaCTe;

            return result.Select(obj => obj.CTe.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosXmlNotaFiscalComCTeConfirmadoPorCarga(int codigoCarga)
        {
            var consultaNotaFiscalPorCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(notaPorCTe => notaPorCTe.CargaCTe.Carga.Codigo == codigoCarga && notaPorCTe.CargaCTe.SituacaoCheckin == SituacaoCheckin.Confirmado);

            return consultaNotaFiscalPorCTe.Select(notaPorCTe => notaPorCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosXmlNotaFiscalComCTeRecusaPorCarga(int codigoCarga)
        {
            var consultaNotaFiscalPorCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(notaPorCTe => notaPorCTe.CargaCTe.Carga.Codigo == codigoCarga && notaPorCTe.CargaCTe.SituacaoCheckin == SituacaoCheckin.RecusaAprovada);

            return consultaNotaFiscalPorCTe.Select(notaPorCTe => notaPorCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTesSemFilialEmissoraPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null
                         && obj.CargaCTe.CTe.Status == "A"
                         && obj.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                         select obj.CargaCTe;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null
                         select obj.CargaCTe.CTe;

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaPedidoSemCTeFilialEmissora(int codigoCargaPedido, bool naoretornarNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null
                         select obj.CargaCTe.CTe;

            if (naoretornarNFSManual)
                result = result.Where(obj => obj.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS);

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaPedidoCTeFilialEmissora(int codigoCargaPedido, bool naoretornarNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeFilialEmissora == null
                         select obj.CargaCTe.CTe;

            if (naoretornarNFSManual)
                result = result.Where(obj => obj.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS);

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaPedidoDaCarga(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido
                         && obj.CargaCTe.CargaCTeComplementoInfo == null
                         && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null
                         select obj.CargaCTe.CTe;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTesPorCargaPedidoDaCarga(List<int> codigoCargaPedidos, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = MontarConsultaPorCargaPedidos(codigoCargaPedidos);

            //if (!string.IsNullOrWhiteSpace(propOrdenacao))
            //    query = query.OrderBy("CargaCTe." + propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            //if (inicioRegistros > 0)
            //    query = query.Skip(inicioRegistros);

            //if (maximoRegistros > 0)
            //    query = query.Take(maximoRegistros);

            return query.Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Serie)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                .Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal).Select(obj => obj.CargaCTe).Distinct().ToList();
        }

        public int ContarCargaCTesPorCargaPedidoDaCarga(List<int> codigoCargaPedidos)
        {
            var query = MontarConsultaPorCargaPedidos(codigoCargaPedidos);

            return query.Select(obj => obj.CargaCTe).Distinct().Count();
        }

        public int ContarCTesPorNotasFiscaisErro(int[] codigosPedidoXMLNotaFiscal, bool apenasSubcontratacaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where codigosPedidoXMLNotaFiscal.Contains(obj.PedidoXMLNotaFiscal.Codigo) && obj.CargaCTe != null
                         select obj;

            if (apenasSubcontratacaoFilialEmissora)
                result = result.Where(obj => obj.CargaCTe.CargaCTeFilialEmissora != null);

            return result.Count();
        }

        public int ContarCTesPorPedidoCTeParaSubContratacao(List<int> codigosPedidoCTeParaSubContratacao, bool apenasSubcontratacaoFilialEmissora)
        {
            if (codigosPedidoCTeParaSubContratacao.Count < 2000)
            {
                var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
                queryPedido = queryPedido.Where(obj => codigosPedidoCTeParaSubContratacao.Contains(obj.PedidoCTeParaSubContratacao.Codigo));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

                var result = from obj in query
                             where queryPedido.Any(c => c.PedidoXMLNotaFiscal == obj.PedidoXMLNotaFiscal) && obj.CargaCTe != null
                             select obj;

                if (apenasSubcontratacaoFilialEmissora)
                    result = result.Where(obj => obj.CargaCTe.CargaCTeFilialEmissora != null);

                return result.Count();
            }

            int quantidade = 0;
            List<int> listaOriginal = codigosPedidoCTeParaSubContratacao;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
                queryPedido = queryPedido.Where(obj => lote.Contains(obj.PedidoCTeParaSubContratacao.Codigo));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

                var result = from obj in query
                             where queryPedido.Any(c => c.PedidoXMLNotaFiscal == obj.PedidoXMLNotaFiscal) && obj.CargaCTe != null
                             select obj;

                if (apenasSubcontratacaoFilialEmissora)
                    result = result.Where(obj => obj.CargaCTe.CargaCTeFilialEmissora != null);

                quantidade += result.Count();

                indiceInicial += tamanhoLote;
            }

            return quantidade;
        }

        public int ContarCTesPorPedidoCTeParaSubContratacao(int codigoPedidoCTeParaSubContratacao, bool apenasSubcontratacaoFilialEmissora, int codigoCarga = 0)
        {
            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            queryPedido = queryPedido.Where(obj => obj.PedidoCTeParaSubContratacao.Codigo == codigoPedidoCTeParaSubContratacao);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where queryPedido.Any(c => c.PedidoXMLNotaFiscal == obj.PedidoXMLNotaFiscal) && obj.CargaCTe != null && obj.CargaCTe.Carga.Codigo != codigoCarga
                         select obj;

            if (codigoCarga > 0)
                result = result.Where(obj => obj.CargaCTe.Carga.Codigo != codigoCarga);

            if (apenasSubcontratacaoFilialEmissora)
                result = result.Where(obj => obj.CargaCTe.CargaCTeFilialEmissora != null);

            return result.Count();
        }

        public List<int> NumerosCTesPorNotasFiscais(List<int> codigosPedidoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal != null && codigosPedidoXMLNotaFiscal.Contains(obj.PedidoXMLNotaFiscal.Codigo) && obj.CargaCTe != null && obj.CargaCTe.CTe != null
                         select obj;

            return result.Select(p => p.CargaCTe.CTe.Numero).Distinct().ToList();
        }

        public int NumeroCTePorNotaFiscalCarga(int codigoXMLNotaFiscal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = query.Where(obj =>
                obj.Carga.Codigo == codigoCarga &&
                obj.CTe != null &&
                obj.CTe.XMLNotaFiscais.Any(nf => nf.Codigo == codigoXMLNotaFiscal)
            );

            return result.Select(p => p.CTe.Numero).FirstOrDefault();
        }

        public int BuscarCargaCtePorCodigoNota(int codigoXMLNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            var result = from obj in query
                         where obj.PedidoXMLNotaFiscal != null && obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoXMLNotaFiscal && obj.CargaCTe != null && obj.CargaCTe.CTe != null
                         select obj.CargaCTe.Codigo;

            return result.FirstOrDefault();
        }

        public void ExcluirCargaPedidoNotaFiscalCTePorCargaPedido(int codigoCargaPedido)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE CargaPedidoXMLNotaFiscalCTe obj WHERE obj.PedidoXMLNotaFiscal in (SELECT pedidoXMLNotaFiscal.Codigo from PedidoXMLNotaFiscal pedidoXMLNotaFiscal WHERE pedidoXMLNotaFiscal.CargaPedido.Codigo = :CargaPedido)")
                             .SetInt32("CargaPedido", codigoCargaPedido)
                             .ExecuteUpdate();
        }

        public List<(int numeroCte, int codigoNota, bool notaCancelada, bool cteCancelado, string chave)> BuscaDadosCtePorNota(List<int> codigoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = from obj in query where codigoNota.Contains(obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) select obj;

            return query.Select(x => ValueTuple.Create(x.CargaCTe.CTe.Numero, x.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo, x.PedidoXMLNotaFiscal.XMLNotaFiscal.CanceladaPeloEmitente, x.CargaCTe.CTe.Status == "C", x.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave)).ToList();
        }

        public bool VerificarSeExisteCargaCTePorPedidoXMLNotaECargaCTe(int codigoPedidoxmlNotaFiscal, int codigoCargaCte)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.PedidoXMLNotaFiscal.Codigo == codigoPedidoxmlNotaFiscal && obj.CargaCTe.Codigo == codigoCargaCte);

            return query.Count() > 0;
        }

        public List<string> BuscarChaveXMLNotasFiscaisPorCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            query = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;
            query = query.Where(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva);
            return query.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave).ToList();
        }

        public List<int> BuscarProtocoloPedidosPorCTe(int codigoCTe)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            var result = from obj in query
                         where
                             obj.PedidoXMLNotaFiscal.XMLNotaFiscal.nfAtiva
                             && obj.CargaCTe.CTe.Codigo == codigoCTe
                         select obj.PedidoXMLNotaFiscal.CargaPedido.Pedido.Protocolo;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCtePorCargaENotaFiscal(int codigoCarga, int codigoXmlNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Carga.Codigo == codigoCarga && o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoXmlNotaFiscal);

            return query.Select(o => o.CargaCTe.CTe).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos - Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelatorioEmbarque.RelatorioEmbarque> RelatorioRelacaoEmbarque(int codigoCarga)
        {
            string query = @"SELECT 
                Empresa.EMP_RAZAO NomeEmpresa,
                Empresa.EMP_ENDERECO EnderecoEmpresa,
                Empresa.EMP_BAIRRO BairroEmpresa,
                Empresa.EMP_CEP CEPEmpresa,
                Empresa.EMP_CNPJ CNPJEmpresa,
                Empresa.EMP_INSCRICAO IEEmpresa,
                Empresa.EMP_REGISTROANTT ANTTEmpresa,
                LocalidadeEmpresa.LOC_DESCRICAO CidadeEmpresa,
                LocalidadeEmpresa.UF_SIGLA EstadoEmpresa,

                Remetente.CLI_NOME NomeRemetente,
                Remetente.CLI_ENDERECO EnderecoRemetente,
                Remetente.CLI_BAIRRO BairroRemetente,
                Remetente.CLI_CEP CEPRemetente,
                Remetente.CLI_CGCCPF CNPJRemetente,
                Remetente.CLI_IERG IERemetente,
                LocalidadeRemetente.LOC_DESCRICAO CidadeRemetente,
                LocalidadeRemetente.UF_SIGLA EstadoRemetente,

                (SELECT top 1 motorista.FUN_NOME 
                    FROM T_CARGA_MOTORISTA cargaMotorista
                    JOIN T_FUNCIONARIO motorista ON motorista.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA
                    WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO) Motorista,
                (SELECT top 1 motorista.FUN_RG 
                    FROM T_CARGA_MOTORISTA cargaMotorista
                    JOIN T_FUNCIONARIO motorista ON motorista.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA
                    WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO) RGMotorista,
                (SELECT top 1 motorista.FUN_CPF 
                    FROM T_CARGA_MOTORISTA cargaMotorista
                    JOIN T_FUNCIONARIO motorista ON motorista.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA
                    WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO) CPFMotorista,

                SUBSTRING((SELECT DISTINCT ', ' + cargaLacre.CLA_NUMERO
				                from T_CARGA_LACRE cargaLacre
		                 WHERE cargaLacre.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')), 3, 1000) Lacre,

                Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                Carga.CAR_DATA_CRIACAO DataCriacaoCarga,
                Carga.CAR_DATA_FINALIZACAO_EMISSAO DataFinalizacaoEmissao,
                Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador,
                Pedido.PED_CUBAGEM_TOTAL Cubagem,
                CTe.CON_NUM NumeroCTe,
	            XMLNotaFiscal.NF_NUMERO NumeroNota,
	            XMLNotaFiscal.NF_PESO Peso,
	            XMLNotaFiscal.NF_VOLUMES Volumes,
	            XMLNotaFiscal.NF_VALOR ValorNota,
                Destinatario.CLI_CODIGO_INTEGRACAO CodigoIntegracaoDestinatario,
	            LocalidadeDestinatario.LOC_DESCRICAO CidadeDestinatario,
                LocalidadeDestinatario.UF_SIGLA EstadoDestinatario,
                ZonaTransporte.TDE_DESCRICAO ZonaTransporte,
                Operador.FUN_NOME OperadorCarga,
                DadosSumarizados.CDS_OBSERVACAO_RELATORIO_DE_EMBARQUE ObservacaoRelatorioDeEmbarque,
                DadosSumarizados.CDS_VEICULOS Veiculo

            FROM T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal
				JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
				JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
                JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                JOIN T_LOCALIDADES LocalidadeEmpresa ON LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO
                JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO
	            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO                
                JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = XMLNotaFiscal.CLI_CODIGO_DESTINATARIO
	            JOIN T_LOCALIDADES LocalidadeDestinatario ON LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO
				LEFT JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe ON PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
				LEFT JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO
				LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                LEFT OUTER JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
                LEFT OUTER JOIN T_LOCALIDADES LocalidadeRemetente ON LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO
                LEFT OUTER JOIN T_PEDIDO_ADICIONAL PedidoAdicional ON PedidoAdicional.PED_CODIGO = Pedido.PED_CODIGO
                LEFT OUTER JOIN T_TIPO_DETALHE ZonaTransporte ON ZonaTransporte.TDE_CODIGO = PedidoAdicional.TDE_CODIGO_ZONA_TRANSPORTE
                LEFT OUTER JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = Carga.CAR_OPERADOR
            WHERE Carga.CAR_CODIGO = " + codigoCarga;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelatorioEmbarque.RelatorioEmbarque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelatorioEmbarque.RelatorioEmbarque>();
        }

        #endregion

        #region Métodos Privados

        private IList<int> BuscarCodigosCargaCtePorCargaPedido(int cargaPedido, int carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes, int inicioRegistros, int maximoRegistros)
        {
            StringBuilder sql = ObterSQLBuscarCargaCtePorCargaPedidos("select distinct CargaCte.CCT_CODIGO", [cargaPedido], [carga], remetente, destinatario, semComplementos, somenteCTes);

            sql.AppendLine($" order by CargaCte.CCT_CODIGO asc offset {inicioRegistros} rows fetch first {maximoRegistros} rows only");

            var consultaCodigosCargaCte = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            return consultaCodigosCargaCte.List<int>();
        }

        private StringBuilder ObterSQLBuscarCargaCtePorCargaPedidos(string select, IList<int> cargaPedidos, IList<int> cargas, double remetente, double destinatario, bool semComplementos, bool somenteCTes)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder where = new StringBuilder();

            sql.Append($@"
                {select}
                  from T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoCte 
                  join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = CargaPedidoCte.CCT_CODIGO
                  join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO
            ");

            if (cargas?.Count > 0)
                where.Append($" and CargaCte.CAR_CODIGO_ORIGEM in ({string.Join(",", cargas)})");

            if (cargaPedidos?.Count > 0)
            {
                joins.AppendLine(" join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.PNF_CODIGO = CargaPedidoCte.PNF_CODIGO ");
                where.AppendLine($" and PedidoNotaFiscal.CPE_CODIGO in ({string.Join(",", cargaPedidos)})");
            }

            if (remetente > 0)
            {
                joins.AppendLine(" join T_CTE_PARTICIPANTE RemetenteCte on RemetenteCte.PCT_CODIGO = Cte.CON_REMETENTE_CTE ");
                where.AppendLine($" and RemetenteCte.CLI_CODIGO = {remetente}");
            }

            if (destinatario > 0)
            {
                joins.AppendLine(" join T_CTE_PARTICIPANTE DestinatarioCte on DestinatarioCte.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
                where.AppendLine($" and DestinatarioCte.CLI_CODIGO = {destinatario}");
            }

            if (semComplementos)
                where.AppendLine($" and CargaCte.CCC_CODIGO is null ");

            if (somenteCTes)
            {
                joins.AppendLine(" join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = Cte.CON_MODELODOC ");
                where.AppendLine(" and ModeloDocumento.MOD_NUM = '57' ");
            }

            if (joins.Length > 0)
                sql.AppendLine(joins.ToString().Trim());

            if (where.Length > 0)
                sql.AppendLine($" where {where.ToString().Trim().Substring(4)} ");

            return sql;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> MontarConsultaPorCargaPedidos(List<int> codigoCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            query = query.Where(obj => obj.CargaCTe.CargaCTeComplementoInfo == null && obj.CargaCTe.CargaCTeSubContratacaoFilialEmissora == null &&
                (obj.CargaCTe.CargaCancelamento == null || obj.CargaCTe.CargaCancelamento.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Carga));

            if (codigoCargaPedidos.Count == 1)
                query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedidos.FirstOrDefault());
            else
                query = query.Where(o => codigoCargaPedidos.Contains(o.PedidoXMLNotaFiscal.CargaPedido.Codigo));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterQueryBuscarCTesPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS, bool considerarHora = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (considerarHora)
            {
                query = query.Where(obj => obj.CTe.DataEmissao >= dataInicial &&
                                       obj.CTe.DataEmissao <= dataFinal);
            }
            else
            {
                query = query.Where(obj => obj.CTe.DataEmissao >= dataInicial.Date &&
                                       obj.CTe.DataEmissao <= dataFinal.Date.AddDays(1));
            }

            if (configuracaoWS?.RetornarOutrosModelosDeDocumentosNoWSCTe ?? false)
                query = query.Where(o => (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros));
            else
                query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);

            if (empresa > 0)
                query = query.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(situacao))
                query = query.Where(obj => obj.CTe.Status == situacao);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(obj => obj.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            if (tomador > 0)
                query = query.Where(obj => obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador);

            if (somentePosIntegracao)
                query = query.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterQueryBuscarCTesAlteradosPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, double tomador, bool somentePosIntegracao, string codigoTipoOperacao, string situacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => (obj.CTe.DataEmissao >= dataInicial.Date && obj.CTe.DataEmissao <= dataFinal.Date.AddDays(1))
                                        || (obj.CTe.DataCancelamento >= dataInicial.Date && obj.CTe.DataCancelamento <= dataFinal.Date.AddDays(1))
                                        || (obj.CTe.DataAnulacao >= dataInicial.Date && obj.CTe.DataAnulacao <= dataFinal.Date.AddDays(1)));

            query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS && o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe);

            if (empresa > 0)
                query = query.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            if (!string.IsNullOrWhiteSpace(situacao))
                query = query.Where(obj => obj.CTe.Status == situacao);

            if (!string.IsNullOrWhiteSpace(codigoTipoOperacao))
                query = query.Where(obj => obj.Carga.TipoOperacao.CodigoIntegracao == codigoTipoOperacao);

            if (tomador > 0)
                query = query.Where(obj => obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador);

            if (somentePosIntegracao)
                query = query.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos ||
                                           obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterQueryCTesPorCargaPedidoMultiModal(List<int> cargaPedido, List<int> carga, double remetente, double destinatario, bool semComplementos, bool somenteCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe != null);

            if (carga?.Count > 0)
                query = query.Where(obj => carga.Contains(obj.CargaOrigem.Codigo));

            if (cargaPedido?.Count > 0)
                query = query.Where(obj => obj.CargaOrigem.Pedidos.Any(p => cargaPedido.Contains(p.Codigo)));

            if (remetente > 0)
                query = query.Where(obj => obj.CTe.Remetente.Cliente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                query = query.Where(obj => obj.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);

            if (semComplementos)
                query = query.Where(obj => obj.CargaCTeComplementoInfo == null);

            if (somenteCTes)
                query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57");
            return query;
        }

        #endregion
    }
}
