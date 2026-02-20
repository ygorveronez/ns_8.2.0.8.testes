using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class VincularRotasSemParar : LongRunningProcessBase<VincularRotasSemParar>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VincularRotas(unitOfWork);
        }

        public override bool CanRun()
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return false; 

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);
        }

        private void VincularRotas(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Frete.AjusteTabelaFrete servicoAjusteTabelFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
            servicoAjusteTabelFrete.VincularRotasSemParar();
        }
    }
}