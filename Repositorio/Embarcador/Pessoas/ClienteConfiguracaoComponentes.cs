using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteConfiguracaoComponentes : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes>
    {
        public ClienteConfiguracaoComponentes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes> BuscarPorCliente(double codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteConfiguracaoComponentes>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}
