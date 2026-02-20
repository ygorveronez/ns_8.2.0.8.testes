using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoRemessaCancelamentoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo>
    {
        public BoletoRemessaCancelamentoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo> BuscarPorTitulos(List<int> codigosTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessaCancelamentoTitulo>();
            var result = from obj in query where codigosTitulo.Contains(obj.Titulo.Codigo) select obj;
            return result.ToList();
        }

        #endregion
    }
}
