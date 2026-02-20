using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class FechamentoAbastecimentoEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa>
    {
        #region Construtores
        
        public FechamentoAbastecimentoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa>()
                .Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa> BuscarPorFechametoAbastecimento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa>()
                .Where(obj => obj.FechamentoAbastecimento.Codigo == codigoFechamento);

            return query.ToList();
        }
        
        #endregion
    }
}
