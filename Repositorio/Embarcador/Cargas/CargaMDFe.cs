using Dominio.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>
    {
        public CargaMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaEmitidosEmOutroSistema(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) && o.Carga.Codigo == codigoCarga && o.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            return query.Fetch(o => o.MDFe).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFe BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.MDFe.Codigo == codigoMDFe);
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorMDFeAsync(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.MDFe.Codigo == codigoMDFe);
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.MDFe.Codigo == codigoMDFe);
            return result.Select(p => p.Carga).FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarMDFePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(p => p.MDFe).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFe BuscarPorMDFeECarga(int codigoMDFe, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.MDFe.Codigo == codigoMDFe && p.Carga.Codigo == carga);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCarga(int codigoCarga, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorAutorizadosCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) select obj.Codigo;

            return result.Count();
        }

        public List<int> BuscarCodigosMDFePorAutorizadosCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga && (p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));
            return result.Select(o => o.MDFe.Codigo).ToList();
        }

        public List<string> BuscarChavesMDFeAutorizadosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga && (p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));
            return result.Select(o => o.MDFe.Chave).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorAutorizadosCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga && (p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            query = query.Where(p => p.MDFe.Codigo == codigoMDFe);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(p => query.Any(o => o.Carga == p.Carga));

            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Pedido> queryPedido = queryCargaPedido.Select(obj => obj.Pedido);

            return queryPedido.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarAutorizadosPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            return query
                .Fetch(obj => obj.MDFe)
                .ThenFetch(obj => obj.Serie)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarAutorizadosPorProtocoloCarga(List<int> protocolosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => protocolosCarga.Contains(o.Carga.Protocolo) && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            return query
                .Fetch(obj => obj.MDFe)
                .ThenFetch(obj => obj.Serie)
                .ToList();
        }

        public int ContarAutorizadosPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarTodosPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            query = query.Where(o => o.Carga == carga || (o.Carga.Protocolo != null && queryCargas.Any(cargaAgrupada => cargaAgrupada.Protocolo == carga.Codigo && cargaAgrupada.CargaAgrupamento.Codigo != null && cargaAgrupada.CargaAgrupamento.Codigo == o.Carga.Protocolo)));

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                query = query.Where(obj => !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarTodosPorProtocoloCarga(int protocoloCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(obj => obj.Carga.Protocolo == protocoloCarga && !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga);

            return consultaCargaMDFe.Fetch(cargaMDFe => cargaMDFe.MDFe).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga);

            return consultaCargaMDFe.Fetch(cargaMDFe => cargaMDFe.MDFe).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaECancelamento(int codigoCarga, int codigoCancelamentoCarga)
        {
            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga && cargaMDFe.CargaCancelamento.Codigo == codigoCancelamentoCarga);

            return consultaCargaMDFe.Fetch(cargaMDFe => cargaMDFe.MDFe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaSemParcial(int codigoCarga, bool filtrarMDFePorStatus)
        {
            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe =>
                    cargaMDFe.Carga.Codigo == codigoCarga &&
                    cargaMDFe.MDFeAnteriorCargaParcial == false &&
                    cargaMDFe.CargaCancelamento == null
            );

            if (filtrarMDFePorStatus)
                consultaCargaMDFe = consultaCargaMDFe.Where(cargaMDFe => cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado);

            return consultaCargaMDFe.Fetch(obj => obj.MDFe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaDesc(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.OrderByDescending(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaVeiculosMDFeNaoEncerrados(int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query where obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento select obj;
            result = result.Where(p => p.Carga.Veiculo.Codigo == veiculo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaVeiculosMDFeNaoEncerrados(string placaVeiculo)
        {
            placaVeiculo = placaVeiculo.ToUpper();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => (o.MDFe.Veiculos.Any(v => v.Placa.ToUpper() == placaVeiculo) || o.MDFe.Reboques.Any(r => r.Placa.ToUpper() == placaVeiculo)) && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorEmEncerramento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            var result = from obj in query select obj;
            result = result.Where(p => p.EmEncerramento == true && p.Carga.CargaFechada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);
            return result.ToList();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, Dominio.Enumeradores.StatusMDFe[] statusMDFe, int minutosLimiteEmissao = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && statusMDFe.Contains(obj.MDFe.Status) select obj;

            if (minutosLimiteEmissao > 0)
                result = result.Where(o => o.MDFe.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> Consultar(int codigoCarga, int codigoCancelamentoCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            query = query.Where(p => p.Carga.Codigo == codigoCarga && p.MDFe != null && (p.CargaCancelamento.Codigo == codigoCancelamentoCarga || p.CargaCancelamento == null));
            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoCarga, int codigoCancelamentoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();
            query = query.Where(p => p.Carga.Codigo == codigoCarga && p.MDFe != null && (p.CargaCancelamento.Codigo == codigoCancelamentoCarga || p.CargaCancelamento == null));
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> ConsultarMDFe(int carga, Dominio.Enumeradores.StatusMDFe statusMDFe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = from obj in query
                         where obj.Carga.Codigo == carga && obj.MDFe != null &&
                                (obj.CargaCancelamento == null || obj.CargaCancelamento.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Carga)
                         select obj;

            if (statusMDFe != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(obj => obj.MDFe.Status == statusMDFe);

            return result.Fetch(o => o.MDFe).ThenFetch(o => o.EstadoCarregamento)
                         .Fetch(o => o.MDFe).ThenFetch(o => o.EstadoDescarregamento)
                         .Fetch(o => o.MDFe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.MDFeManual)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaMDFe(int carga, Dominio.Enumeradores.StatusMDFe statusMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = from obj in query
                         where obj.Carga.Codigo == carga && obj.MDFe != null &&
                                (obj.CargaCancelamento == null || obj.CargaCancelamento.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Carga)
                         select obj;

            if (statusMDFe != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(obj => obj.MDFe.Status == statusMDFe);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarMDFesPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = from obj in query
                    where
                       obj.MDFe.DataEmissao.Value.Date >= dataInicial.Date
                       && obj.MDFe.DataEmissao.Value.Date <= dataFinal.Date
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.MDFe.Empresa.Codigo == empresa);

            var result = query.OrderBy(obj => obj.Codigo).Select(obj => obj);
            return result.Distinct().Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarMDFesPorPeriodoEmpresa(DateTime dataInicial, DateTime dataFinal, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = from obj in query
                    where
                       obj.MDFe.DataEmissao.Value.Date >= dataInicial.Date
                       && obj.MDFe.DataEmissao.Value.Date <= dataFinal.Date
                    select obj;

            if (empresa > 0)
                query = query.Where(obj => obj.MDFe.Empresa.Codigo == empresa);

            var result = query.OrderBy(obj => obj.Codigo).Select(obj => obj);

            return result.Distinct().Count();
        }

        public bool ExisteMDFeInvalidoParaCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                                     o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado &&
                                     o.MDFe.DataAutorizacao < DateTime.Now.AddDays(-1));

            return query.Any();
        }

        public bool ExisteMDFeEmCancelamento(int codigoCarga, int codigoCargaCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe =>
                    cargaMDFe.MDFe != null &&
                    cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    cargaMDFe.Carga.Codigo == codigoCarga
                );

            if (codigoCargaCancelamento > 0)
                consultaCargaMDFe = consultaCargaMDFe.Where(cargaMDFe => cargaMDFe.CargaCancelamento.Codigo == codigoCargaCancelamento);

            return consultaCargaMDFe.Any();
        }

        public bool ExisteMDFeComRejeicaoNoCancelamento(int codigoCarga, int codigoCargaCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe =>
                    cargaMDFe.Carga.Codigo == codigoCarga &&
                    cargaMDFe.MDFe != null &&
                    cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                    cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado &&
                    cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao
                );

            if (codigoCargaCancelamento > 0)
                consultaCargaMDFe = consultaCargaMDFe.Where(cargaMDFe => cargaMDFe.CargaCancelamento.Codigo == codigoCargaCancelamento);

            return consultaCargaMDFe.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarAutorizadosEmitidoNoMultiCTePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe =>
                    cargaMDFe.MDFe != null &&
                    cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                    cargaMDFe.Carga.Codigo == codigoCarga
                );

            return consultaCargaMDFe.ToList();
        }

        public List<string> BuscarNumeroCargaAutorizadoPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Carga.CodigoCargaEmbarcador).ToList();
        }

        public bool ExisteAutorizadoPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public List<int> BuscarCodigosPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemCancelamento(int codigoCarga)
        {
            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga && cargaMDFe.CargaCancelamento == null);

            return consultaCargaMDFe.Select(cargaMDFe => cargaMDFe.Codigo).ToList();
        }

        public int BuscarCodigoPorCargaECTe(int codigoCarga, int codigoCTe)
        {
            var consultaDocumentoMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>()
                .Where(documento => documento.CTe.Codigo == codigoCTe);

            var consultaCargaMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga && consultaDocumentoMDFe.Any(documento => documento.MunicipioDescarregamento.MDFe.Codigo == cargaMDFe.MDFe.Codigo));

            return consultaCargaMDFe.Select(cargaMDFe => cargaMDFe.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorCargaEEstadoDestino(int codigoCarga, string estadoDestino)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.MDFe.EstadoDescarregamento.Sigla == estadoDestino);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFe BuscarPorCodigoComFetch(int codigoCargaMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Codigo == codigoCargaMDFe);

            return query.Fetch(o => o.MDFe).ThenFetch(o => o.Empresa).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> BuscarPorChavesCTes(IEnumerable<string> chavesCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                     o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.MDFe.MunicipiosDescarregamento.Any(c => c.Documentos.Any(d => chavesCTes.Contains(d.Chave) || chavesCTes.Contains(d.CTe.Chave))));

            return query.Timeout(99999).ToList();
        }

        public bool ExistePorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            query = query.Where(o => o.MDFe != null &&
                                     o.Carga == carga);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> BuscarMDFesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(p => p.Carga.Codigo == codigoCarga);

            return query.Select(p => p.MDFe).ToList();
        }

        public Task<List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>> BuscarMDFesPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(p => p.Carga.Codigo == codigoCarga);

            return query.Select(p => p.MDFe).ToListAsync(cancellationToken);
        }

        public Task<bool> ExisteMDFeNaoAutorizado(int codigoCarga, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(p => p.Carga.Codigo == codigoCarga && p.MDFe.Status != StatusMDFe.Autorizado);

            return query.AnyAsync(cancellationToken);
        }

        public bool ExisteMDFeNaoEncerradoPorCarga(int codigoCarga)
        {
            var existeNaoEncerrado = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>()
                .Where(cargaMDFe => cargaMDFe.Carga.Codigo == codigoCarga &&
                                    cargaMDFe.MDFe != null &&
                                    cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                .Any(); // Retorna true se existir pelo menos um

            return existeNaoEncerrado;
        }

        public Task<bool> ExisteStatusMDFePorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

            var result = query.AnyAsync(p => p.Carga.Codigo == codigoCarga &&
                                new[]
                                {
                                    Dominio.Enumeradores.StatusMDFe.Encerrado,
                                    Dominio.Enumeradores.StatusMDFe.Autorizado,
                                    Dominio.Enumeradores.StatusMDFe.Enviado,
                                    Dominio.Enumeradores.StatusMDFe.Cancelado
                                }.Contains(p.MDFe.Status), CancellationToken);

            return result; 
        }

        #endregion
    }
}
