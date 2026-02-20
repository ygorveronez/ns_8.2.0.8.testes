using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoNFSe : LongRunningProcessBase<IntegracaoNFSe>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            ConsultarNFSePendenteMigrate();
        }

        #endregion 

        #region Metodos Privados

        private void ConsultarNFSePendenteMigrate()
        {
            try
            {
                int numeroTentativas = 10;
                double minutosACadaTentativa = 3;
                int numeroRegistrosPorVez = 15;
                Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(_unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTePendentes = repCTe.BuscarListaNFSePendentesIntegracaoMigrate(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

                foreach (var cte in listaCTePendentes)
                {
                    if (cte.RPS != null)
                    {
                        cte.RPS.DataUltimaConsulta = DateTime.Now;
                        cte.RPS.QuantidadeTentativaConsulta++;

                        Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate(_unitOfWork);
                        serMigrate.ConsultarRetornoNFSe(cte, _unitOfWork);

                        repRPSNFSe.Atualizar(cte.RPS);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
        
        #endregion
    }
}