using System.Linq;

namespace Repositorio
{
    public class OpcoesEmpresa : RepositorioBase<Dominio.Entidades.OpcoesEmpresa>
    {
        public OpcoesEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OpcoesEmpresa BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OpcoesEmpresa>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.OpcoesEmpresa BuscarPorNome(string nome, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OpcoesEmpresa>();
            var result = from obj in query where obj.Nome.Equals(nome) && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public string BuscaOpcao(string nome, int codigoEmpresa, string valorPadrao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OpcoesEmpresa>();
            var result = from obj in query where obj.Nome.Equals(nome) && obj.Empresa.Codigo == codigoEmpresa select obj;

            var opcao = result.FirstOrDefault();

            if (opcao != null)
                return opcao.Valor;
            else
                return valorPadrao;
        }
    }
}
