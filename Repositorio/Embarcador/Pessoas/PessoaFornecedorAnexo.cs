using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaFornecedorAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo>
    {
        public PessoaFornecedorAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo BuscarPorModalidadeECodigo(int codigoModalidade, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo>();

            var result = from obj in query
                         where
                         obj.EntidadeAnexo.Codigo == codigoModalidade && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        #endregion
    }
}
