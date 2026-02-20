using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class AceiteDebito : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito>
    {
        public AceiteDebito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito>();
            var result = from obj in query where obj.Ocorrencia.Codigo == ocorrencia select obj;
            return result.Fetch(obj => obj.Usuario).FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito> _Consultar(int numeroOcorrencia, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito? situacao, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito>();

            var result = from obj in query select obj;

            // Filtros
            if (numeroOcorrencia > 0)
                result = result.Where(o => o.Ocorrencia.NumeroOcorrencia == numeroOcorrencia);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Date <= dataFim);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            if (codigoTransportador > 0)
                result = result.Where(o => o.Ocorrencia.Carga.Empresa.Codigo == codigoTransportador || o.Ocorrencia.ContratoFrete.Transportador.Codigo == codigoTransportador);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito> Consultar(int numeroOcorrencia, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito? situacao, int codigoTransportador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numeroOcorrencia, dataInicio, dataFim, situacao, codigoTransportador);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numeroOcorrencia, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito? situacao, int codigoTransportador)
        {
            var result = _Consultar(numeroOcorrencia, dataInicio, dataFim, situacao, codigoTransportador);

            return result.Count();
        }
    }
}
