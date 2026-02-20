using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteContratoFreteAcrescimoDescontoAutomatico : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico>
    {
        public ClienteContratoFreteAcrescimoDescontoAutomatico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> BuscarPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico> BuscarContratoFreteAcrescimoDescontoPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj.AcrescimoDescontoAutomatico;
            return result.ToList();
        }
    }
}
