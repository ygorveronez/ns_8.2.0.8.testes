using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoLavacao : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao>
    {
        public VeiculoLavacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao> BuscarPorVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo select obj;
            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
