using MongoDB.Driver;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoAnalise : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>
    {
        public ChamadoAnalise(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ChamadoAnalise(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarAnalisesAtendimentosPorCargaComEntrega(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.Carga.Codigo == carga && obj.Chamado.CargaEntrega != null && obj.Chamado.MotivoChamado.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Atendimento select obj;
            return result
                .Fetch(
                 obj => obj.Autor
                ).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarAnalisesAtendimentosPorEntrega(int entrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.CargaEntrega.Codigo == entrega && obj.Chamado.MotivoChamado.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Atendimento select obj;
            return result
                .Fetch(
                 obj => obj.Autor
                ).ToList();
        }

        public Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise BuscarUltimaAnalisePorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.Codigo == codigo orderby obj.DataCriacao descending select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarUltimaAnalisePorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.Codigo == codigo orderby obj.DataCriacao descending select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarAprovacaoValorChamadoPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.Codigo == codigo && obj.LiberadoValorCargaDescarga == true select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarPorChamado(int chamado)
        {
            var result = _Consultar(chamado);
            result = result.OrderByDescending(obj => obj.DataCriacao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> Consultar(int chamado, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(chamado);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int chamado)
        {
            var result = _Consultar(chamado);

            return result.Count();
        }

        public bool UsuarioAptoAEscalarAtendimento(int codigoChamado, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
            var result = query.Where(chamado => chamado.Chamado.Codigo == codigoChamado).OrderByDescending(chamado => chamado.DataCriacao).FirstOrDefault();

            return result.Autor?.Codigo == codigoUsuario && result.Chamado?.Responsavel?.Codigo == codigoUsuario;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> _Consultar(int chamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();

            var result = from obj in query where obj.Chamado.Codigo == chamado select obj;

            return result;
        }

        #endregion
    }
}
