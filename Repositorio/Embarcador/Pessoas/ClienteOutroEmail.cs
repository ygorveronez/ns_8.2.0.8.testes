using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteOutroEmail : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>
    {
        public ClienteOutroEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> BuscarPorCNPJCPFCliente(double cnpjcpfCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpjcpfCliente select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> BuscarPorCNPJCPFClienteTipo(double cnpjcpfCliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail tipoEmail)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpjcpfCliente && obj.TipoEmail == tipoEmail select obj;
            return result.ToList();
        }

        public bool ContemEmailCliente(double cnpjcpfCliente, string email)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpjcpfCliente && obj.Email == email && obj.EmailStatus == "A" select obj;
            return result.Any();
        }

    }
}
