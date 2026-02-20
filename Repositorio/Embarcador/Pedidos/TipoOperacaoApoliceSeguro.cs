using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoApoliceSeguro : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>
    {
        public TipoOperacaoApoliceSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> _Consultar(int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();

            var result = from obj in query
                         where
                            obj.TipoOperacao.Codigo == tipoOperacao
                         select obj;

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> BuscarPorCodigosDiferente(int tipoOperacao, List<int> apolices)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();

            var result = from obj in query
                         where
                            obj.TipoOperacao.Codigo == tipoOperacao &&
                            !apolices.Contains(obj.ApoliceSeguro.Codigo)
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> Consultar(int tipoOperacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(tipoOperacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int tipoOperacao)
        {
            var result = _Consultar(tipoOperacao);

            return result.Count();
        }

        public bool TipoOperacaoApoliceSeguroJaExiste(int tipoOperacao, int apoliceSeguro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();

            var result = from obj in query
                         where
                            obj.TipoOperacao.Codigo == tipoOperacao &&
                            obj.ApoliceSeguro.Codigo == apoliceSeguro
                         select obj;

            return result.Count() > 0;
        }
    }
}
