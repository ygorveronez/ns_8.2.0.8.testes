using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Frete
{
    public class ContratoTransporteFrete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ContratoTransporteFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarIntegracaoAnexo(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            GerarRegistroIntegracaoContratoFrete(contratoTransporteFrete, tipoIntegracao, integracaoAnexo: true);
        }

        public void GerarIntegracaoContrato(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contrato, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            GerarRegistroIntegracaoContratoFrete(contrato, tipoIntegracao, integracaoAnexo: false);
        }

        public void GerarRegistroIntegracaoContratoFreteCustoFixo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repostitorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            if (contratoFreteTransportador.Situacao == SituacaoContratoFreteTransportador.Aprovado && contratoFreteTransportador.TipoCargas != null && contratoFreteTransportador.ModelosVeiculares != null)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repostitorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

                if (tipoIntegracao != null)
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repostiorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao contratoFreteTransportadorIntegracao = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        TipoIntegracao = tipoIntegracao,
                        ProblemaIntegracao = string.Empty,
                        NumeroTentativas = 0,
                        SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                        IntegrarCustoFixo = true,
                        ContratoFreteTransportador = contratoFreteTransportador
                    };

                    repostiorioContratoFreteTransportador.Inserir(contratoFreteTransportadorIntegracao);
                }
            }
        }

        public void VerificarIntegracoesAnexosPendentes()
        {
            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repostiorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> registrosAnexosPendentes = repostiorioContratoFreteTransportador.BuscarIntegracoesPendentes(numeroTentativas: 3, minutosACadaTentativa: 5, integracaoAnexos: true);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracaoPendente in registrosAnexosPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.LBC:
                        new Embarcador.Integracao.LBC.IntegracaoLBC(_unitOfWork).IntegrarContratoTransporteFreteAnexos(integracaoPendente);
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repositorioContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao> integracoesPendentesContratoFreteTransportador = repositorioContratoTransporteFreteIntegracao.BuscarIntegracoesPendentes(3, 5, integracaoAnexos: false);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracaoPendente in integracoesPendentesContratoFreteTransportador)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.LBC:
                        new Embarcador.Integracao.LBC.IntegracaoLBC(_unitOfWork).IntegrarContratoTransporteFrete(integracaoPendente);
                        break;
                }
            }
        }

        #endregion Métodos Públicos

        #region Metodos Privados

        private void GerarRegistroIntegracaoContratoFrete(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, bool integracaoAnexo)
        {
            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repostiorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);

            if (!integracaoAnexo)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracao = repostiorioContratoFreteTransportador.BuscarIntegracaoPorCodigoContrato(contratoTransporteFrete.Codigo);

                if (integracao != null)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    repostiorioContratoFreteTransportador.Atualizar(integracao);
                    return;
                }

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoFreteTransportadorIntegracao = new Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao()
                {
                    DataIntegracao = DateTime.Now,
                    TipoIntegracao = tipoIntegracao,
                    ProblemaIntegracao = string.Empty,
                    NumeroTentativas = 0,
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    IntegrarAnexos = integracaoAnexo,
                    ContratoTransporteFrete = contratoTransporteFrete
                };

                repostiorioContratoFreteTransportador.Inserir(contratoFreteTransportadorIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoFreteTransportadorIntegracaoAnexo = new Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao()
            {
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracao,
                ProblemaIntegracao = string.Empty,
                NumeroTentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                IntegrarAnexos = integracaoAnexo,
                ContratoTransporteFrete = contratoTransporteFrete
            };

            repostiorioContratoFreteTransportador.Inserir(contratoFreteTransportadorIntegracaoAnexo);
        }

        #endregion Metodos Privados
    }
}
