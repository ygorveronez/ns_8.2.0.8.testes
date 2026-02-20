using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteAreaRedex : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex>
    {

        public ClienteAreaRedex(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex>()
                .Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> BuscarPorCNPJCPFCliente(double cnpjCpfCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex>()
                .Where(o => o.Cliente.CPF_CNPJ == cnpjCpfCliente);

            return query.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex> BuscarPorListaCargaEntrega(List<int> CodigosCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteAreaRedex>();
            var queryCargaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            var result = query.Where(o => (from obj in queryCargaEntrega where CodigosCargaEntrega.Contains(obj.Codigo) select obj.Cliente.CPF_CNPJ).Contains(o.Cliente.CPF_CNPJ));

            return result.ToList();
        }
    }
}
