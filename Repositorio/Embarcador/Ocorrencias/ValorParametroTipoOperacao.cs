using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao>
    {
        public ValorParametroTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao BuscarPorTipoOperacaoEValorParametro(int codigoTipoOperacao, int codigoValorParametroOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroTipoOperacao>();
            var result = from obj in query where obj.TipoOperacao.Codigo == codigoTipoOperacao && obj.ValorParametroOcorrencia.Codigo == codigoValorParametroOcorrencia select obj;
            return result.FirstOrDefault();
        }
    }
}
