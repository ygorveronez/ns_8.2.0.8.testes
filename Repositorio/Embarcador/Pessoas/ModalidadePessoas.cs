using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadePessoas : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>
    {
        public ModalidadePessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> BuscarPorCliente(double cnpj_cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cnpj_cpf select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> BuscarPorClientes(List<double> cnpj_cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            var result = from obj in query where cnpj_cpf.Contains(obj.Cliente.CPF_CNPJ) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, double cnpj_cpf, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> lstModalidadePessoas = null)
        {
            if (lstModalidadePessoas != null)
                return lstModalidadePessoas.Where(obj => obj.TipoModalidade == tipoModalidade && obj.Cliente.CPF_CNPJ == cnpj_cpf).FirstOrDefault();
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>().Where(obj => obj.TipoModalidade == tipoModalidade && obj.Cliente.CPF_CNPJ == cnpj_cpf).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, List<double> cnpj_cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            var result = from obj in query where obj.TipoModalidade == tipoModalidade && cnpj_cpf.Contains(obj.Cliente.CPF_CNPJ) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> Consultar(int codigo, Dominio.Entidades.Cliente cliente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (cliente != null)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ.Equals(cliente.CPF_CNPJ));


            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, Dominio.Entidades.Cliente cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (cliente != null)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ.Equals(cliente.CPF_CNPJ));

            return result.Count();
        }

        public bool ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, double cnpj_cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            var result = from obj in query where obj.TipoModalidade == tipoModalidade && obj.Cliente.CPF_CNPJ == cnpj_cpf select obj;
            return result.Count() > 0;
        }

    }
}
