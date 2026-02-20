using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoLoteCliente : ServicoBase
    {
        #region Construtores
        
        public IntegracaoLoteCliente(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public IntegracaoLoteCliente(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public static void GerarIntegracoesLoteCliente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);
            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> lotesAgIntegracao = repLoteCliente.BuscarLotesAgIntegracao(5);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            for (var i = 0; i < lotesAgIntegracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente = lotesAgIntegracao[i];

                bool integracaoFinalizada = true;

                IniciarIntegracoesComEDI(loteCliente, unitOfWork);

                if (!VerificarSePossuiIntegracao(loteCliente, unitOfWork) || !loteCliente.Integracoes.Any() || loteCliente.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (repLoteClienteIntegracaoEDI.ContarPorLoteClienteESituacaoDiff(loteCliente.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                        loteCliente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.Finalizado;

                    integracaoFinalizada = true;
                }

                if (integracaoFinalizada)
                {
                    loteCliente.GerouIntegracoes = true;

                    repLoteCliente.Atualizar(loteCliente);
                }
            }
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {

            List<int> lotesCliente = await new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).VerificarIntegracoesPendentesLoteClienteAsync();

            lotesCliente = lotesCliente.Distinct().ToList();

            foreach (int codigoLoteCliente in lotesCliente)
                AtualizarSituacaoClienteIntegracao(codigoLoteCliente, _unitOfWork);
        }

        public void AtualizarSituacaoClienteIntegracao(int codigoLoteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);
            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente = repLoteCliente.BuscarPorCodigo(codigoLoteCliente, false);

            if (loteCliente.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.AgIntegracao ||
                loteCliente.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.FalhaIntegracao)
            {
                if (repLoteClienteIntegracaoEDI.ContarPorLoteCliente(loteCliente.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    loteCliente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.FalhaIntegracao;

                    repLoteCliente.Atualizar(loteCliente);
                }
                else if (repLoteClienteIntegracaoEDI.ContarPorLoteCliente(loteCliente.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    loteCliente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.Finalizado;

                    repLoteCliente.Atualizar(loteCliente);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private static void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(loteCliente, unitOfWork);
        }

        private static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

            if (repLoteClienteIntegracaoEDI.ContarPorLoteClienteESituacaoDiff(loteCliente.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        #endregion
    }
}
