using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.RH
{
    public class ComissaoFuncionarioMotoristaAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento>
    {
        public ComissaoFuncionarioMotoristaAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemAbastecimentoEmComissao(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento select obj;
            return result.Any();
        }
    }
}
