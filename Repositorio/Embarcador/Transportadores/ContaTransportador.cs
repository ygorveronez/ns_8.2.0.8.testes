using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class ContaTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.ContaTransportador>
    {
        public ContaTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.ContaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Transportadores.ContaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaContaTransportador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ContaTransportador>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAgencia))
                result = result.Where(obj => obj.NumeroAgencia.Equals(filtrosPesquisa.NumeroAgencia));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroConta))
                result = result.Where(obj => obj.NumeroConta.Equals(filtrosPesquisa.NumeroConta));

            if (filtrosPesquisa.CodigoBanco > 0)
                result = result.Where(o => o.Banco.Codigo == filtrosPesquisa.CodigoBanco);

            return result;
        }

        #endregion
    }
}
