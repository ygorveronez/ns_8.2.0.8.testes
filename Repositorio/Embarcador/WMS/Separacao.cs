using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.WMS
{
    public class Separacao : RepositorioBase<Dominio.Entidades.Embarcador.WMS.Separacao>
    {
        public Separacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.Separacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Separacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.Separacao> _Consultar(int separador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao? situacao, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Separacao>();

            var result = from obj in query select obj;

            // Filtros
            if (separador > 0)
                result = result.Where(o => o.Funcionarios.Any(f => f.Usuario.Codigo == separador));

            if (situacao.HasValue)
                result = result.Where(o => o.Selecao.SituacaoSelecaoSeparacao == situacao);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Selecao.Data.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Selecao.Data.Date <= dataFinal);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.Separacao> Consultar(int separador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao? situacao, DateTime dataInicial, DateTime dataFinal, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(separador, situacao, dataInicial, dataFinal);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int separador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao? situacao, DateTime dataInicial, DateTime dataFinal)
        {
            var result = _Consultar(separador, situacao, dataInicial, dataFinal);

            return result.Count();
        }
    }
}
