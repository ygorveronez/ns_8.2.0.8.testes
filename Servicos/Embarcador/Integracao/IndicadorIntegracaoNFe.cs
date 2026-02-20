using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public sealed class IndicadorIntegracaoNFe
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Contrutores

        public IndicadorIntegracaoNFe(Repositorio.UnitOfWork unitOfWork) : this (unitOfWork, configuracaoEmbarcador: null) { }

        public IndicadorIntegracaoNFe(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarIntegracaoAutomaticaComSucesso(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido == null) || (cargaPedido.Carga == null))
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.IndicarIntegracaoNFe)
                return;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
            {
                Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe indicadorIntegracaoNFe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe()
                {
                    DataIntegracao = DateTime.Now,
                    Filial = pedidoXMLNotaFiscal.CargaPedido.Carga.Filial,
                    PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                    NumeroCarga = pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe.Sucesso,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe.Automatico,
                    XMLNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal
                };

                repositorioIndicadorIntegracaoNFe.Inserir(indicadorIntegracaoNFe);
            }
        }

        public void AdicionarIntegracaoAutomaticaRejeitada(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, string motivoRejeicao)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.IndicarIntegracaoNFe)
                return;

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = !string.IsNullOrWhiteSpace(cargaIntegracao.Filial?.CodigoIntegracao) ? repositorioFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial.CodigoIntegracao) : null;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe indicadorIntegracaoNFe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe()
            {
                DataIntegracao = DateTime.Now,
                Filial = filial,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe.Rejeitada,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe.Automatico,
                MotivoRejeicao = motivoRejeicao,
                NumeroCarga = cargaIntegracao.NumeroCarga
            };

            repositorioIndicadorIntegracaoNFe.Inserir(indicadorIntegracaoNFe);
        }

        public void AdicionarIntegracaoAutomaticaRejeitada(Dominio.Entidades.Embarcador.Cargas.Carga carga, string motivoRejeicao)
        {
            if (carga == null)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.IndicarIntegracaoNFe)
                return;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe indicadorIntegracaoNFe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe()
            {
                DataIntegracao = DateTime.Now,
                Filial = carga.Filial,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe.Rejeitada,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe.Automatico,
                MotivoRejeicao = motivoRejeicao,
                NumeroCarga = carga.CodigoCargaEmbarcador
            };

            repositorioIndicadorIntegracaoNFe.Inserir(indicadorIntegracaoNFe);
        }

        public void AdicionarIntegracaoPorEmailComSucesso(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, string emailRemetente)
        {
            if (pedidoXMLNotaFiscal == null)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.IndicarIntegracaoNFe)
                return;

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe indicadorIntegracaoNFe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe()
            {
                DataIntegracao = DateTime.Now,
                EmailRemetente = emailRemetente,
                Filial = pedidoXMLNotaFiscal.CargaPedido.Carga.Filial,
                PedidoXMLNotaFiscal = pedidoXMLNotaFiscal,
                NumeroCarga = pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe.Sucesso,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe.PorEmail,
                XMLNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal
            };

            repositorioIndicadorIntegracaoNFe.Inserir(indicadorIntegracaoNFe);
        }

        public void AdicionarIntegracaoPorEmailRejeitada(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, string emailRemetente, string motivoRejeicao)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.IndicarIntegracaoNFe)
                return;

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato);

            Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe repositorioIndicadorIntegracaoNFe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe indicadorIntegracaoNFe = new Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe()
            {
                DataIntegracao = DateTime.Now,
                EmailRemetente = emailRemetente,
                Filial = filial,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIndicadorIntegracaoNFe.Rejeitada,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracaoNFe.PorEmail,
                MotivoRejeicao = motivoRejeicao,
                XMLNotaFiscal = xmlNotaFiscal
            };

            repositorioIndicadorIntegracaoNFe.Inserir(indicadorIntegracaoNFe);
        }

        #endregion
    }
}
