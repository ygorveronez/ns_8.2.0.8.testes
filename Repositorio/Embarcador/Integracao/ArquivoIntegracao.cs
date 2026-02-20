using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class ArquivoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>
    {
        public ArquivoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao BuscarPorNomeArquivo(string nomeArquivo)
        {
            var arquivoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>()
                .Where(o => o.NomeArquivo == nomeArquivo)
                .FirstOrDefault();

            return arquivoIntegracao;
        }
    }
}
