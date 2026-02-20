using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class CTeCanhotoIntegracao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private readonly string _urlAcessoCliente;

        #endregion

        #region Construtores

        public CTeCanhotoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void ProcessarIntegracoesPendentes()
        {
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            int tentativas = 5;
            int intervaloTempoRejeitadas = 5;
            List<int> integracoesPendentes = repCTeCanhotoIntegracao.BuscarIntegracoesPendentes(tentativas, intervaloTempoRejeitadas, 100);
            int total = integracoesPendentes.Count();
            for (int i = 0; i < total; i++)
            {
                ProcessarIntegracaoPendente(integracoesPendentes[i]);

                _unitOfWork.FlushAndClear();
            }
        }

        public void ProcessarIntegracaoPendente(int codigo)
        {
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao cteCanhotoIntegracao = repCTeCanhotoIntegracao.BuscarPorCodigo(codigo);

            switch (cteCanhotoIntegracao?.TipoIntegracao.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                    ProcessarIntegracao_Unilever(cteCanhotoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cebrace:
                    ProcessarIntegracaoCebrace(cteCanhotoIntegracao);
                    break;
            }
        }

        #endregion

        #region Métodos Privados

        private void ProcessarIntegracao_Unilever(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork).Buscar();
            if (configuracaoIntegracaoUnilever.IntegrarCanhoto)
                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork).EnviarCteCanhoto(integracao);
        }

        private void ProcessarIntegracaoCebrace(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace configuracaoIntegracaoCebrace = new Repositorio.Embarcador.Configuracoes.IntegracaoCebrace(_unitOfWork).BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoCebrace?.PossuiIntegracao ?? false)
                new Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace(_unitOfWork).EnviarCteCanhoto(integracao);
        }

        #endregion
    }
}
