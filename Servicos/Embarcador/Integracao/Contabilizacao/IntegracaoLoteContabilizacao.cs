using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Contabilizacao
{
    public class IntegracaoLoteContabilizacao : ServicoBase
    {
        #region Construtores
        
        public IntegracaoLoteContabilizacao(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public static void GerarIntegracoesLoteContabilizacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> lotesAgIntegracao = repLoteContabilizacao.BuscarLotesAgIntegracao(5);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            for (var i = 0; i < lotesAgIntegracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = lotesAgIntegracao[i];

                bool integracaoFinalizada = true;

                IniciarIntegracoesComEDI(loteContabilizacao, unitOfWork);

                if (!VerificarSePossuiIntegracao(loteContabilizacao, unitOfWork) || !loteContabilizacao.Integracoes.Any() || loteContabilizacao.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (repLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoESituacaoDiff(loteContabilizacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                        loteContabilizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.Finalizado;

                    integracaoFinalizada = true;
                }

                if (integracaoFinalizada)
                {
                    loteContabilizacao.GerouIntegracoes = true;

                    repLoteContabilizacao.Atualizar(loteContabilizacao);
                }
            }
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            List<int> lotesContabilizacao = await new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).VerificarIntegracoesPendentesContabilizacaoAsync();

            lotesContabilizacao = lotesContabilizacao.Distinct().ToList();

            foreach (int codigoLoteContabilizacao in lotesContabilizacao)
                await AtualizarSituacaoContabilizacaoIntegracaoAsync(codigoLoteContabilizacao);
        }


        public async Task AtualizarSituacaoContabilizacaoIntegracaoAsync(int codigoLoteContabilizacao)
        {
            Repositorio.Embarcador.Financeiro.LoteContabilizacao repositorioLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repositorioLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = await repositorioLoteContabilizacao.BuscarPorCodigoAsync(codigoLoteContabilizacao, false);

            if (loteContabilizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.AgIntegracao ||
                loteContabilizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.FalhaIntegracao)
            {
                if (await repositorioLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoAsync(loteContabilizacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    loteContabilizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.FalhaIntegracao;

                    await repositorioLoteContabilizacao.AtualizarAsync(loteContabilizacao);
                }
                else if (await repositorioLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoAsync(loteContabilizacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                    return;
                else
                {
                    loteContabilizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao.Finalizado;

                    await repositorioLoteContabilizacao.AtualizarAsync(loteContabilizacao);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private static void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(loteContabilizacao, unitOfWork);
        }

        private static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

            if (repLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoESituacaoDiff(loteContabilizacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        #endregion
    }
}
