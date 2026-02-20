using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteSubContratacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao>
    {
        public TabelaFreteClienteSubContratacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao BuscarPorTabelaETerceiro(int codigoTabelaFreteCliente, double cnpjTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao>();

            var result = from obj in query where obj.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente && obj.Pessoa.CPF_CNPJ == cnpjTerceiro select obj;

            return result.FirstOrDefault();
        }

    }
}
