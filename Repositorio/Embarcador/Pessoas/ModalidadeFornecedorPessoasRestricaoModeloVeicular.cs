using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeFornecedorPessoasRestricaoModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>
    {
        public ModalidadeFornecedorPessoasRestricaoModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> BuscarPorModalidade(int codigoModalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            var result = from obj in query where obj.ModalidadeFornecedorPessoa.ModalidadePessoas.Codigo == codigoModalidade select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> BuscarPorModalidades(List<int> codigosModalidades)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            var result = from obj in query where codigosModalidades.Contains(obj.ModalidadeFornecedorPessoa.ModalidadePessoas.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> BuscarPorModalidadeFornecedorPessoa(int codigoModalidadeFornecedorPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular>();
            var result = from obj in query where obj.ModalidadeFornecedorPessoa.Codigo == codigoModalidadeFornecedorPessoa select obj;
            return result.ToList();
        }
    }
}
