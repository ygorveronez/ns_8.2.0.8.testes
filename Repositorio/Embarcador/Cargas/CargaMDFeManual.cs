using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManual : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>
    {
        public CargaMDFeManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual BuscarPorCodigoEIntegracao(int codigo, bool integracaoIntercab)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.Codigo == codigo);
            query = query.Where(o => o.MDFeRecebidoDeIntegracao == integracaoIntercab);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            query = query.Where(o => o.Cargas.Any(obj => obj.Codigo == carga) && o.Situacao != SituacaoMDFeManual.Cancelado);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> BuscarPorPortosTerminais(int codigoViagem, int codigoPortoOrigem, int codigoPortoDestino, int codigoTerminalOrigem, int codigoTerminalDestino)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.TerminalCarregamento.Any(obj => obj.Codigo == codigoTerminalOrigem));
            query = query.Where(o => o.TerminalDescarregamento.Any(obj => obj.Codigo == codigoTerminalDestino));
            query = query.Where(o => o.PortoOrigem.Codigo == codigoPortoOrigem && o.PortoDestino.Codigo == codigoPortoDestino && o.PedidoViagemNavio.Codigo == codigoViagem);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.CTes.Any(obj => obj.Codigo == codigoCargaCTe));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargas(int codigoMDFeManual)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();
            //query = query.Where(o => o.CargaMDFeManual.Codigo == codigoMDFeManual);
            //queryCarga = queryCarga.Where(c => query.Any(m => m.CargaMDFeManual.Cargas.Any(a => a.Codigo == c.Codigo)));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Codigo == codigoMDFeManual);
            queryCargaCTe = queryCargaCTe.Where(c => query.Any(o => o.CTes.Any(e => e.CTe == c.CTe)));
            queryCarga = queryCarga.Where(c => queryCargaCTe.Any(e => e.Carga == c));

            return queryCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> BuscarTodosPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            query = query.Where(o => o.Cargas.Any(obj => obj.Codigo == carga));
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual BuscarPorMDFe(int MDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            query = query.Where(o => o.MDFeManualMDFes.Any(obj => obj.MDFe.Codigo == MDFe) && o.Situacao != SituacaoMDFeManual.Cancelado);
            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> BuscarPorSituacao(SituacaoMDFeManual situacao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            query = query.Where(o => o.Situacao == situacao);
            return query.Skip(inicio).Take(limite).ToList();
        }

        public List<int> BuscarCodigosPorSituacao(SituacaoMDFeManual situacao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            query = query.Where(o => o.Situacao == situacao);
            return query.Skip(inicio).Take(limite).Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual VerificarCargaMDFeManualComCte(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.CTes.Any(cte => cte.Codigo == codigoCTe) && (o.Situacao != SituacaoMDFeManual.Cancelado && o.Situacao != SituacaoMDFeManual.Finalizado));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual VerificarCargaMDFeManualComCarga(int codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            query = query.Where(o => o.Cargas.Any(carga => carga.Codigo == codigoCargas) && (o.Situacao != SituacaoMDFeManual.Cancelado && o.Situacao != SituacaoMDFeManual.Finalizado));

            return query.FirstOrDefault();
        }

        public bool PossuiMFDeAutorizadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || c.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            if (codigoCarga > 0)
            {
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga) || queryCargaCTe.Any(c => o.CTes.Any(e => e.CTe.Codigo == c.CTe.Codigo)));
            }

            return query.Any();
        }

        public bool PossuiMFDePendentePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmDigitacao || c.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || c.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Pendente));

            if (codigoCarga > 0)
            {
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga) || queryCargaCTe.Any(c => o.CTes.Any(e => e.CTe.Codigo == c.CTe.Codigo)));
            }

            return query.Any();
        }

        public bool PossuiMFDePorCargaPorSituacao(int codigoCarga, Dominio.Enumeradores.StatusMDFe statusMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Status == statusMDFe));

            if (codigoCarga > 0)
            {
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga) || queryCargaCTe.Any(c => o.CTes.Any(e => e.CTe.Codigo == c.CTe.Codigo)));
            }

            return query.Any();
        }

        public List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> ConsultarAquaviario(int terminalOrigem, int terminalDestino, int pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManual? situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (terminalOrigem > 0)
                query = query.Where(o => o.TerminalCarregamento.Any(m => m.Codigo == terminalOrigem));

            if (terminalDestino > 0)
                query = query.Where(o => o.TerminalDescarregamento.Any(m => m.Codigo == terminalDestino));

            if (pedidoViagemNavio > 0)
                query = query.Where(o => o.PedidoViagemNavio.Codigo == pedidoViagemNavio);

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
            {
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga) || queryCargaCTe.Any(c => o.CTes.Any(e => e.CTe.Codigo == c.CTe.Codigo)));
            }

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> queryMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            queryMDFe = queryMDFe.Where(o => query.Any(p => p.MDFeManualMDFes.Any(m => m.MDFe == o)));

            return queryMDFe.OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaAquaviario(int terminalOrigem, int terminalDestino, int pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManual? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (terminalOrigem > 0)
                query = query.Where(o => o.TerminalCarregamento.Any(m => m.Codigo == terminalOrigem));

            if (terminalDestino > 0)
                query = query.Where(o => o.TerminalDescarregamento.Any(m => m.Codigo == terminalDestino));

            if (pedidoViagemNavio > 0)
                query = query.Where(o => o.PedidoViagemNavio.Codigo == pedidoViagemNavio);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
            {
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga) || queryCargaCTe.Any(c => o.CTes.Any(e => e.CTe.Codigo == c.CTe.Codigo)));
            }

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            IQueryable<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> queryMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            queryMDFe = queryMDFe.Where(o => query.Any(p => p.MDFeManualMDFes.Any(m => m.MDFe == o)));

            return queryMDFe.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> Consultar(int terminalOrigem, int terminalDestino, int pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManual? situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (terminalOrigem > 0)
                query = query.Where(o => o.TerminalCarregamento.Any(m => m.Codigo == terminalOrigem));

            if (terminalDestino > 0)
                query = query.Where(o => o.TerminalDescarregamento.Any(m => m.Codigo == terminalDestino));

            if (pedidoViagemNavio > 0)
                query = query.Where(o => o.PedidoViagemNavio.Codigo == pedidoViagemNavio);

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            return query.Fetch(o => o.Origem).ThenFetch(o => o.Estado)
                        .Fetch(o => o.Destino).ThenFetch(o => o.Estado)
                        .Fetch(o => o.Veiculo)
                        .Fetch(o => o.Empresa)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int terminalOrigem, int terminalDestino, int pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, SituacaoMDFeManual? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (terminalOrigem > 0)
                query = query.Where(o => o.TerminalCarregamento.Any(m => m.Codigo == terminalOrigem));

            if (terminalDestino > 0)
                query = query.Where(o => o.TerminalDescarregamento.Any(m => m.Codigo == terminalDestino));

            if (pedidoViagemNavio > 0)
                query = query.Where(o => o.PedidoViagemNavio.Codigo == pedidoViagemNavio);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> ConsultarCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            query = query.Where(o => o.Situacao != SituacaoMDFeManual.Cancelado);

            return query.Fetch(o => o.Origem)
                        .Fetch(o => o.Destino)
                        .Fetch(o => o.Veiculo)
                        .OrderBy(propOrdenar + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsultaCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe tipoModalMDFe, int codigoVeiculo, int codigoMotorista, int codigoOrigem, int codigoDestino, int numeroCTe, int numeroMDFe, int codigoCarga, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();

            if (tipoModalMDFe != TipoModalMDFe.Todos)
                query = query.Where(o => o.TipoModalMDFe == tipoModalMDFe);

            if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Reboques.Any(r => r.Codigo == codigoVeiculo));

            if (codigoMotorista > 0)
                query = query.Where(o => o.Motoristas.Any(m => m.Codigo == codigoMotorista));

            if (codigoOrigem > 0)
                query = query.Where(o => o.Origem.Codigo == codigoOrigem);

            if (codigoDestino > 0)
                query = query.Where(o => o.Destino.Codigo == codigoDestino || o.Destinos.Any(det => det.Localidade.Codigo == codigoDestino));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (numeroMDFe > 0)
                query = query.Where(o => o.MDFeManualMDFes.Any(c => c.MDFe.Numero == numeroMDFe));

            if (codigoCarga > 0)
                query = query.Where(o => o.Cargas.Any(c => c.Codigo == codigoCarga));

            if (empresa > 0)
                query = query.Where(o => o.Empresa.Codigo == empresa);

            query = query.Where(o => o.Situacao != SituacaoMDFeManual.Cancelado);

            return query.Count();
        }

        public bool ExistePorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.Select(o => o.Codigo).Any();
        }

        public void RemoverCargaCTe(int codigoCargaCTe)
        {
            string sqlQuery = "DELETE FROM T_CARGA_MDFE_MANUAL_CTE WHERE CCT_CODIGO = :codigoCargaCTe";

            SessionNHiBernate.CreateSQLQuery(sqlQuery).SetInt32("codigoCargaCTe", codigoCargaCTe).ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntergacao(int codigo, int inicio, int limite)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryCargaCTeIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao>();
            var resultCargaCTeIntegracao = from obj in queryCargaCTeIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultCargaCTeIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> BuscarCargaMDFeManualAgIntegracao(bool gerandoIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.AgIntegracao && obj.GerandoIntegracoes == gerandoIntegracoes select obj;

            return result.ToList();
        }

        #endregion
    }
}
