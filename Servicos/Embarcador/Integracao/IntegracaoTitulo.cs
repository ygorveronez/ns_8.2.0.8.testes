using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoTitulo
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoTitulo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesDeTitulos(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcao)
        {
            if (titulo.TipoTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        AdicionarTituloIntegracao(titulo, tipoIntegracao, tipoAcao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                        Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(_unitOfWork);
                        var integracaoPortalCabotagem = repIntegracaoPortalCabotagem.BuscarPrimeiroRegistro();

                        if (integracaoPortalCabotagem?.AtivarEnvioPDFBoleto ?? false)
                            AdicionarTituloIntegracao(titulo, tipoIntegracao, tipoAcao, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void IniciarIntegracoesDeTitulosAReceber(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcao)
        {
            if (titulo.Documentos != null && titulo.Documentos?.Count > 0)// apenas para quando não possui documentos fiscais
                return;

            if (titulo.TipoTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdicionarTituloIntegracao(titulo, tipoIntegracao, tipoAcao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                        Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(_unitOfWork);
                        var integracaoPortalCabotagem = repIntegracaoPortalCabotagem.BuscarPrimeiroRegistro();

                        if (integracaoPortalCabotagem?.AtivarEnvioPDFBoleto ?? false)
                            AdicionarTituloIntegracao(titulo, tipoIntegracao, tipoAcao, _unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
                        var configuracaoIntegracaoEMP = repIntegracaoEMP.Buscar();

                        if (configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoBoletoEMP ?? false && (configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoBoletoEMP ?? "") == "A")
                            AdicionarTituloIntegracao(titulo, tipoIntegracao, tipoAcao, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Métodos Públicos Estáticos

        public void VerificarIntegracoesPendentesTitulo()
        {
            int numeroTentativas = 2;
            int minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Financeiro.TituloIntegracao repTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao> titulosIntegracao = repTituloIntegracao.BuscarPendentesIntegracao(numeroRegistrosPorVez, numeroTentativas, minutosACadaTentativa);
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = (from obj in titulosIntegracao select obj.Titulo).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao in titulosIntegracao)
            {
                switch (tituloIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (tituloIntegracao.TipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Criacao)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarTituloFinanceiro(tituloIntegracao);
                        else if (tituloIntegracao.TipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Alteracao)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarTituloFinanceiroAlteracao(tituloIntegracao);
                        else
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarTituloFinanceiroCancelamento(tituloIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if (tituloIntegracao.TipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Criacao)
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork).IntegrarTituloFinanceiro(tituloIntegracao);
                        else
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork).IntegrarTituloFinanceiroCancelamento(tituloIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                        new Servicos.Embarcador.Integracao.PortalCabotagem.IntegracaoPortalCabotagem(_unitOfWork).Integrar(tituloIntegracao.CodigosCTes.FirstOrDefault(), false);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(_unitOfWork).IntegrarTitulo(tituloIntegracao);
                        break;
                    default:
                        break;
                }

                repTituloIntegracao.Atualizar(tituloIntegracao);
            }

        }
        public void AdicionarTituloIntegracao(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(unitOfWork);

            if (repositorioTituloIntegracao.ExistePorTituloETipo(titulo.Codigo, tipoIntegracao, tipoAcaoIntegracao))
                return;

            if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Alteracao || tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
            {
                var integracaoCriacao = repositorioTituloIntegracao.BuscarPorTituloETipoIntegracaoEAcao(titulo.Codigo, tipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Criacao);

                if (integracaoCriacao == null)
                    return;

                if(integracaoCriacao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                {
                    if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
                        return;

                    integracaoCriacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repositorioTituloIntegracao.Atualizar(integracaoCriacao);
                    return;
                }
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoTitulo = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao = new Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao()
            {
                Titulo = titulo,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoTitulo,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                TipoAcaoIntegracao = tipoAcaoIntegracao,
            };

            repositorioTituloIntegracao.Inserir(tituloIntegracao);
        }
        #endregion

        private ServicoSGT.Financeiro.FinanceiroClient ObterClientFinanceiro(string url, string token)
        {
//#if DEBUG
//            url = "http://localhost:5146/";
//#endif
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Financeiro.svc";

            ServicoSGT.Financeiro.FinanceiroClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Financeiro.FinanceiroClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Financeiro.FinanceiroClient(binding, endpointAddress);
            }
            AdicionarHeaders(token, client.InnerChannel);

            return client;
        }

        private void AdicionarHeaders(string token, System.ServiceModel.IContextChannel channel)
        {
            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(channel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);
        }



    }
}
