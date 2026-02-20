using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeFornecedorPessoas : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>
    {
        public ModalidadeFornecedorPessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas BuscarPorModalidade(int codigoModalidade, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas> lstModalidadeFornecedorPessoas = null)
        {
            if (lstModalidadeFornecedorPessoas != null)
                return lstModalidadeFornecedorPessoas.Where(obj=> obj.ModalidadePessoas.Codigo == codigoModalidade).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var result = from obj in query where obj.ModalidadePessoas.Codigo == codigoModalidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas BuscarPorCliente(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var result = from obj in query where obj.ModalidadePessoas.Cliente.CPF_CNPJ == cpfCnpj select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas> BuscarPorCliente(List<double> listaCpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            query = from obj in query where listaCpfCnpj.Contains(obj.ModalidadePessoas.Cliente.CPF_CNPJ) select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas> BuscarPorModalidades(List<int> codigoModalidades)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var result = from obj in query where codigoModalidades.Contains(obj.ModalidadePessoas.Codigo) select obj;
            return result.Fetch(obj => obj.ModalidadePessoas).ToList();
        }

        public void DeletarPorModalidade(int codigoModalidade)
        {
            this.SessionNHiBernate.CreateQuery("DELETE ModalidadeFornecedorPessoas o WHERE o.ModalidadePessoas.Codigo = :codigoModalidade").SetParameter("codigoModalidade", codigoModalidade).ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> BuscarPostosConveniados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            query = query.Where(c => c.PostoConveniado == true && c.ModalidadePessoas.Cliente.Longitude != "" && c.ModalidadePessoas.Cliente.Longitude != null && c.ModalidadePessoas.Cliente.Latitude != "" && c.ModalidadePessoas.Cliente.Latitude != null);
            return query.Select(c => c.ModalidadePessoas).Fetch(obj => obj.Cliente).ToList();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaOficinaPorCliente(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var result = from obj in query where obj.EmpresaOficina != null && obj.ModalidadePessoas.Cliente.CPF_CNPJ == cpfCnpj select obj.EmpresaOficina;
            return result.FirstOrDefault();
        }

        #endregion
    }
}