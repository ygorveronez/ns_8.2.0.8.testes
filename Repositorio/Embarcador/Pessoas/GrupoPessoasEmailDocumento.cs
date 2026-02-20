using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasEmailDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail>
    {
        public GrupoPessoasEmailDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail> BuscarTodosPorEntidade(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas entidadeGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail>();
            query = query.Where(obj => obj.GrupoPessoas == entidadeGrupoPessoa);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail BuscarPorTipoDocumento(int tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail>();

            var result = from obj in query where obj.ModeloDocumentoFiscal.Codigo == tipoDocumento select obj;

            return result.FirstOrDefault();
        }
    }
}
