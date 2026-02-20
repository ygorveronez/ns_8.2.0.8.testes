using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Integracao.DigitalCom
{
    public class IntegracaoDigitalCom
    {
        #region Atributos

        public readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo _configuracaoVeiculo;

        #endregion

        #region Construtores

        public IntegracaoDigitalCom(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private IIntegracaoDigitalCom ObterIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = null)
        {
            ObterConfiguracaoVeiculo(configuracaoVeiculo);

            if (_configuracaoVeiculo?.ValidarTAGDigitalCom ?? false)
                return new IntegracaoDigitalComRest(_unitOfWork, _configuracaoVeiculo);

            return new IntegracaoDigitalComServico(_unitOfWork);
        }

        private void ObterConfiguracaoVeiculo(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo)
        {
            _configuracaoVeiculo ??= configuracaoVeiculo;

            if (_configuracaoVeiculo != null)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(_unitOfWork);

            _configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            if (!(cargaDadosTransporteIntegracao?.Carga?.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false) && cargaDadosTransporteIntegracao.Carga.ValorFrete <= 0m && cargaDadosTransporteIntegracao.Carga?.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositoriIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now.AddMinutes(15);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Carga sem valor de frete";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositoriIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            ObterIntegracaoDigitalCom().IntegrarCarga(cargaDadosTransporteIntegracao);
        }

        public bool PermitirGerarIntegracao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo)
        {
            return ObterIntegracaoDigitalCom(configuracaoVeiculo).PermitirGerarIntegracao();
        }

        #endregion Métodos Públicos
    }
}
