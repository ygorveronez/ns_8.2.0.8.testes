using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaOfertaIntegracaoArquivos : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos>
    {
        public CargaOfertaIntegracaoArquivos(UnitOfWork unitOfWork) : this(unitOfWork, default) { }
        public CargaOfertaIntegracaoArquivos(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos
        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos>> BuscarArquivosPorCodigoIntegracaoAsync(int cargaOfertaCodigoIntegracao)
        {
            var buscaintegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracaoArquivos>()
            .Where(arquivos => arquivos.CargaOfertaIntegracao.Codigo == cargaOfertaCodigoIntegracao);

            return buscaintegracoes.ToListAsync(CancellationToken);
        }



        #endregion Métodos Públicos

    }
}