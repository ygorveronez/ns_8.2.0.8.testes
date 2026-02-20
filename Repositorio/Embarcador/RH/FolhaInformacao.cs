using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class FolhaInformacao : RepositorioBase<Dominio.Entidades.Embarcador.RH.FolhaInformacao>
    {
        public FolhaInformacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.FolhaInformacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaInformacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.FolhaInformacao BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaInformacao>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.RH.FolhaInformacao> Consultar(string descricao, string codigoIntegracao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaInformacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaInformacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            return result.Count();
        }
    }
}
