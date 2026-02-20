using System.Linq;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Email
{
    public class EmailGlobalizadoFornecedorCliente : RepositorioBase<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente>
    {
        public EmailGlobalizadoFornecedorCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente> BuscarPorEmailGlobalizado(int codigoEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente>();

            query = query.Where(o => o.EmailGlobalizadoFornecedor.Codigo == codigoEmail);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente BuscarPorEmailGlobalizadoECliente(int codigoEmail, double cpfCnpjFornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente>();

            query = query.Where(o => o.EmailGlobalizadoFornecedor.Codigo == codigoEmail && o.Fornecedor.CPF_CNPJ == cpfCnpjFornecedor);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientePorEmailGlobalizado(int codigoEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente>();

            query = query.Where(o => o.EmailGlobalizadoFornecedor.Codigo == codigoEmail);

            return query.Select(o => o.Fornecedor).ToList();
        }
        #endregion
    }
}
