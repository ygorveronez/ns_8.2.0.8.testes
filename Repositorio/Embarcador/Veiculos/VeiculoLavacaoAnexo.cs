using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoLavacaoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo>
    {
        public VeiculoLavacaoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo> BuscarPorLavacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigo select obj;
            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
