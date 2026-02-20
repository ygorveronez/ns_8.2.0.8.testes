using System.Linq;

namespace Repositorio
{
    public class ClienteDadosIntegracao : RepositorioBase<Dominio.Entidades.ClienteDadosIntegracao>
    {
        public ClienteDadosIntegracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ClienteDadosIntegracao BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteDadosIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ClienteDadosIntegracao BuscaPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteDadosIntegracao>();

            var result = from obj in query where obj.Tipo == tipoIntegracao && obj.Cliente.CPF_CNPJ == cliente select obj;

            return result.FirstOrDefault();
        }


    }
}
