using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeClientePessoas : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>
    {
        public ModalidadeClientePessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas BuscarPorModalidade(int codigoModalidade, List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas> lstModalidadeClientePessoas = null)
        {
            if (lstModalidadeClientePessoas != null)
                return lstModalidadeClientePessoas.Where(obj=> obj.ModalidadePessoas.Codigo == codigoModalidade).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>();
            var result = from obj in query where obj.ModalidadePessoas.Codigo == codigoModalidade select obj;
            return result.FirstOrDefault();
        }

        public void DeletarPorModalidade(int codigoModalidade)
        {
            this.SessionNHiBernate.CreateQuery("DELETE ModalidadeClientePessoas o WHERE o.ModalidadePessoas.Codigo = :codigoModalidade").SetParameter("codigoModalidade", codigoModalidade).ExecuteUpdate();
        }

    }
}

