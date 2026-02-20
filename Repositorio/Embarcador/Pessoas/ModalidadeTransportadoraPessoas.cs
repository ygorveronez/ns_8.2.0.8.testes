using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeTransportadoraPessoas : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>
    {
        public ModalidadeTransportadoraPessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas BuscarPorModalidade(int codigoModalidade, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas> lstModalidadeTransportadoraPessoas = null)
        {
            if (lstModalidadeTransportadoraPessoas != null)
                return lstModalidadeTransportadoraPessoas.Where(obj => obj.ModalidadePessoas.Codigo == codigoModalidade).FirstOrDefault();


            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();
            var result = from obj in query where obj.ModalidadePessoas.Codigo == codigoModalidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas BuscarPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();
            var result = from obj in query where obj.ModalidadePessoas.Cliente.CPF_CNPJ == cpfCnpj select obj;
            return result.FirstOrDefault();
        }

        public string BuscarRNTCPorPessoa(double cpfCnpj)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>()
                .Where(modalidade => modalidade.ModalidadePessoas.Cliente.CPF_CNPJ == cpfCnpj);

            return query.Select(modalidade => modalidade.RNTRC).FirstOrDefault()?.ToString();
        }


        public void DeletarPorModalidade(int codigoModalidade)
        {
            this.SessionNHiBernate.CreateQuery("DELETE ModalidadeTransportadoraPessoas o WHERE o.ModalidadePessoas.Codigo = :codigoModalidade").SetParameter("codigoModalidade", codigoModalidade).ExecuteUpdate();
        }
    }
}
