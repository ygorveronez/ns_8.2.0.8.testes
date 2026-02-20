using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoLoteEscrituracaoCancelamento : ServicoBase
    {

        #region Construtores

        public IntegracaoLoteEscrituracaoCancelamento() : base() { }

        public IntegracaoLoteEscrituracaoCancelamento(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public IntegracaoLoteEscrituracaoCancelamento(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #endregion construtores

        #region Métodos Públicos

        public void IniciarIntegracoesComEDI(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoEDI.AdicionarEDIParaIntegracao(loteEscrituracaoCancelamento, unitOfWork);
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            IntegracaoEDI servicoIntegracaoEDI = new IntegracaoEDI(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> lotesEscrituracaoCancelamento = await servicoIntegracaoEDI.VerificarIntegracoesPendentesEscrituracaoCancelamentoAsync();

            lotesEscrituracaoCancelamento = lotesEscrituracaoCancelamento.Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento in lotesEscrituracaoCancelamento)
                AtualizarSituacaoEscrituracaoIntegracao(loteEscrituracaoCancelamento, _unitOfWork, _tipoServicoMultisoftware);
        }

        public static void AtualizarSituacaoEscrituracaoIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(unitOfWork);

            loteEscrituracaoCancelamento = repLoteEscrituracaoCancelamento.BuscarPorCodigo(loteEscrituracaoCancelamento.Codigo);

            if (loteEscrituracaoCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.AgIntegracao ||
                loteEscrituracaoCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.FalhaIntegracao)
            {
                if (repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                {
                    loteEscrituracaoCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.FalhaIntegracao;

                    repLoteEscrituracaoCancelamento.Atualizar(loteEscrituracaoCancelamento);
                }
                else if (repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracao(loteEscrituracaoCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                {
                    return;
                }
                else
                {
                    loteEscrituracaoCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.Finalizado;

                    repLoteEscrituracaoCancelamento.Atualizar(loteEscrituracaoCancelamento);
                }
            }
        }

        public static bool VerificarSePossuiIntegracao(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unidadeDeTrabalho);

            if (repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracaoESituacaoDiff(loteEscrituracaoCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) > 0)
                return true;

            return false;
        }

        #endregion
    }
}
