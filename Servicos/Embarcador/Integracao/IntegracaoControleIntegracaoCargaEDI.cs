using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoControleIntegracaoCargaEDI : ServicoBase
    {
        #region Construtores
       
        public IntegracaoControleIntegracaoCargaEDI(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public async Task VerificarIntegracoesPendentesAsync()
        {
            await VerificarControleIntegracaoCargaEDIPendentesAsync();
        }

        #endregion

        #region Métodos Privados

        private async Task VerificarControleIntegracaoCargaEDIPendentesAsync()
        {
            _unitOfWork.FlushAndClear();

            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork);

            var listaIntegracoesPendentes = repositorioControleIntegracaoCargaEDI.BuscarPendenteIntegracaoEnvio(0, 10);

            Servicos.Embarcador.Integracao.FTP.IntegracaoFTP servicoIntegracaoFTP = new FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            foreach (var integracao in listaIntegracoesPendentes)
            {
                if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                    await servicoIntegracaoFTP.EnviarEDIAsync(integracao);
                else if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                {
                    Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEDI(integracao, _unitOfWork);
                }
                else
                {
                    integracao.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
                    integracao.MensagemRetorno = "Tipo de envio não implementado;";
                    await repositorioControleIntegracaoCargaEDI.AtualizarAsync(integracao);
                }
            }
        }

        #endregion

    }
}
