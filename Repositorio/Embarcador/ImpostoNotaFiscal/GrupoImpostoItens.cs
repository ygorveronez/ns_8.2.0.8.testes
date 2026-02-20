using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ImpostoNotaFiscal
{
    public class GrupoImpostoItens : RepositorioBase<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens>
    {
        public GrupoImpostoItens(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens BuscarPorEstadosAtividade(string ufDestino, string ufOrigem, int codigoAtividade, int codigoGrupoImposto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens>();
            var result = from obj in query where obj.UFDestino == ufDestino && obj.UFOrigem == ufOrigem && obj.Atividade.Codigo == codigoAtividade && obj.GrupoImposto.Codigo == codigoGrupoImposto select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens> BuscarPorGrupo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens>();
            var result = from obj in query where obj.GrupoImposto.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
