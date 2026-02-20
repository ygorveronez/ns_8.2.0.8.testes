using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMercadoLivre : LongRunningProcessBase<IntegracaoMercadoLivre>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcIntegracaoMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

            svcIntegracaoMercadoLivre.ConsultarHandlingUnitsPendentes(_tipoServicoMultisoftware, null, unitOfWork, cancellationToken);
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre);
        }
    }
}