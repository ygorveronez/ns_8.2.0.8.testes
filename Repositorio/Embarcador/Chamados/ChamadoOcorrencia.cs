using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>
    {
        public ChamadoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Privados
        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj.Chamado;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosPorOcorrenciaComFetch(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj.Chamado;

            return result
                .Fetch(obj => obj.Cliente)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.Chamado> BuscarChamadosPorOcorrenciaPaginado(List<int> codigosOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> result = new List<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            int take = 2000;
            int start = 0;
            while (start < codigosOcorrencia?.Count)
            {
                List<int> tmp = codigosOcorrencia.Skip(start).Take(take).ToList();

                result.AddRange((from obj in query where tmp.Contains(obj.CargaOcorrencia.Codigo) select obj.Chamado).ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj.CargaOcorrencia;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public string BuscarNumerosOcorrenciasPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj.CargaOcorrencia;
            return string.Join(", ", result.Select(o => o.NumeroOcorrencia));
        }

        public string BuscarNumeroChamadoPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia orderby obj.Chamado.Numero descending select obj.Chamado;

            int numeroChamado = result.Select(o => o.Numero).FirstOrDefault();

            return numeroChamado == 0 ? string.Empty : numeroChamado.ToString();
        }

        public Task<List<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>> BuscarPorChamadoAsync(int codigoChamado)
        {
            var result = _Consultar(codigoChamado);

            return result.OrderByDescending(obj => obj.Chamado.DataCriacao).ToListAsync(CancellationToken);
        }

        public bool ExisteOcorrenciaDuplicadaPorChamado(int codigoChamado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada
            };

            IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            query = query.Where(o => o.Chamado.Codigo == codigoChamado && !situacoes.Contains(o.CargaOcorrencia.SituacaoOcorrencia) && o.Chamado.MotivoChamado.TipoOcorrencia.BloqueiaOcorrenciaDuplicada);

            return query.Any();
        }

        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> _Consultar(int chamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();

            var result = from obj in query where obj.Chamado.Codigo == chamado select obj;

            return result;
        }

        #endregion
    }
}
