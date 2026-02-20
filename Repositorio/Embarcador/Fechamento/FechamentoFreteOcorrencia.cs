using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia>
    {
        #region Construtores

        public FechamentoFreteOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj.Ocorrencia;

            return result;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaCargaOcorrencia = Consultar(codigoFechamento);

            return consultaCargaOcorrencia.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(fechamento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int fechamento)
        {
            var result = Consultar(fechamento);

            return result.Count();
        }

        #endregion
    }
}
