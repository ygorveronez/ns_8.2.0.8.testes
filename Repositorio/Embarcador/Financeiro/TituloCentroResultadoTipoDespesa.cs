using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloCentroResultadoTipoDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa>
    {
        public TituloCentroResultadoTipoDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> BuscarPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa>();

            var result = from obj in query where obj.Titulo.Codigo == codigoTitulo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa> BuscarPorTitulos(List<int> codigoTitulos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa>();

            var result = from obj in query where codigoTitulos.Contains(obj.Titulo.Codigo) select obj;

            return result.ToList();
        }

        #endregion
    }
}
