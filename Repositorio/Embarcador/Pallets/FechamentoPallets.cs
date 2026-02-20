using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets
{
    public class FechamentoPallets : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets>
    {

        public FechamentoPallets(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pallets.FechamentoPallets BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets>();
            int? ultimoNumero = query.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets> _Consultar(int numero, DateTime dataInicio, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets>();

            var result = from obj in query select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Date >= dataInicio);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinal.Date <= dataFinal);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets> Consultar(int numero, DateTime dataInicio, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numero, dataInicio, dataFinal, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numero, DateTime dataInicio, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets? situacao)
        {
            var result = _Consultar(numero, dataInicio, dataFinal, situacao);

            return result.Count();
        }
    }
}
