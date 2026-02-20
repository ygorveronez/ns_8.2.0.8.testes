using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Raster
{
    public class IntegracaoRaster
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public IntegracaoRaster() { }

        public IntegracaoRaster(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoRaster || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRaster))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Raster.";
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            string tipoAmbiente = "Homologacao";

            if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                tipoAmbiente = "Producao";

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            bool preSMjaIntegrada = configuracaoIntegracao.GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster;
            if (preSMjaIntegrada)
            {// confirma a geração da integração pois por falta de dados na etapa pode que a integração não tenha cido gerada neste caso ira gerar preSM agora
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
                if (cargaDadosTransporteIntegracao == null)
                    preSMjaIntegrada = false;
            }

            try
            {
                if (IntegrarMotoristas(out string mensagem, cargaIntegracao, null, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    IntegrarVeiculo(out mensagem, cargaIntegracao, null, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    IntegrarCarretas(out mensagem, cargaIntegracao, null, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    (preSMjaIntegrada || IntegrarPreSM(out mensagem, cargaIntegracao, null, tipoAmbiente, configuracaoIntegracao, unitOfWork)) &&
                    (preSMjaIntegrada || EfetivarPreSM(out mensagem, cargaIntegracao, null, tipoAmbiente, configuracaoIntegracao, unitOfWork)) &&
                    (!configuracaoIntegracao.GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao || SetRevisaoPreSm(out mensagem, cargaIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork))
                   )
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integração realizada com sucesso.";
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = mensagem;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (configuracaoIntegracao.NotificarFalhaIntegracaoRaster && cargaIntegracao.Carga.Operador != null)
                    {
                        Servicos.Embarcador.Notificacao.Notificacao svcNotificacao = new Notificacao.Notificacao();
                        svcNotificacao.GerarNotificacao(cargaIntegracao.Carga.Operador, cargaIntegracao.Carga.Codigo, "Cargas/Carga", string.Format(Localization.Resources.Cargas.Carga.OcorreramProblemasIntegracaoCarga, cargaIntegracao.Carga.CodigoCargaEmbarcador), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, tipoServicoMultisoftware, unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Erro geral ao integrar.";

                try
                {
                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaIntegracao, ex.Message, "Sem resposta", "txt", cargaIntegracao.ProblemaIntegracao);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }
            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoRaster || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRaster))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Raster.";
                cargaDadosTransporteIntegracao.NumeroTentativas += 1;
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

                return;
            }

            string tipoAmbiente = "Homologacao";

            if (cargaDadosTransporteIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                tipoAmbiente = "Producao";

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                if (IntegrarMotoristas(out string mensagem, null, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    IntegrarVeiculo(out mensagem, null, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    IntegrarCarretas(out mensagem, null, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    IntegrarPreSM(out mensagem, null, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork) &&
                    (
                        configuracaoIntegracao.GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster || EfetivarPreSM(out mensagem, null, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork))
                    )
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (configuracaoIntegracao.NotificarFalhaIntegracaoRaster && cargaDadosTransporteIntegracao.Carga.Operador != null)
                    {
                        Servicos.Embarcador.Notificacao.Notificacao svcNotificacao = new Notificacao.Notificacao();
                        svcNotificacao.GerarNotificacao(cargaDadosTransporteIntegracao.Carga.Operador, cargaDadosTransporteIntegracao.Carga.Codigo, "Cargas/Carga", string.Format(Localization.Resources.Cargas.Carga.OcorreramProblemasIntegracaoCarga, cargaDadosTransporteIntegracao.Carga.CodigoCargaEmbarcador), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, tipoServicoMultisoftware, unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro geral ao integrar.";

                try
                {
                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, ex.Message, "Sem resposta", "txt", cargaDadosTransporteIntegracao.ProblemaIntegracao);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public static void IntegrarCargaFrete(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaFreteIntegracao.Carga.Codigo, cargaFreteIntegracao.TipoIntegracao.Codigo);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaFreteIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoRaster || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRaster))
            {
                cargaFreteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaFreteIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Raster.";
                cargaFreteIntegracao.NumeroTentativas += 1;
                cargaFreteIntegracao.DataIntegracao = DateTime.Now;

                repCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);

                return;
            }

            string tipoAmbiente = "Homologacao";

            if (cargaFreteIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                tipoAmbiente = "Producao";

            cargaFreteIntegracao.NumeroTentativas += 1;
            cargaFreteIntegracao.DataIntegracao = DateTime.Now;

            Servicos.Embarcador.Carga.CargaFreteIntegracao servicoCargaFreteIntegracao = new Servicos.Embarcador.Carga.CargaFreteIntegracao();

            try
            {
                if (EfetivarPreSM(out string mensagem, cargaCargaIntegracao, cargaDadosTransporteIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork, cargaFreteIntegracao))
                {
                    cargaFreteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaFreteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso.";
                    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                    cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = false;
                }
                else
                {
                    servicoCargaFreteIntegracao.AplicarFalha(cargaFreteIntegracao, mensagem);

                    if (configuracaoIntegracao.NotificarFalhaIntegracaoRaster && cargaFreteIntegracao.Carga.Operador != null)
                    {
                        Servicos.Embarcador.Notificacao.Notificacao svcNotificacao = new Notificacao.Notificacao();
                        svcNotificacao.GerarNotificacao(cargaFreteIntegracao.Carga.Operador, cargaFreteIntegracao.Carga.Codigo, "Cargas/Carga", string.Format(Localization.Resources.Cargas.Carga.OcorreramProblemasIntegracaoCarga, cargaFreteIntegracao.Carga.CodigoCargaEmbarcador), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, tipoServicoMultisoftware, unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                servicoCargaFreteIntegracao.AplicarFalha(cargaFreteIntegracao, "Erro geral ao integrar.");

                try
                {
                    ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
                    servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, ex.Message, "Sem resposta", "txt", cargaFreteIntegracao.ProblemaIntegracao);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }
            repCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
        }

        public static void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            cargaCancelamentoIntegracao.NumeroTentativas += 1;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporte = null;

            if (cargaIntegracao == null || string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo))
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                cargaDadosTransporte = repCargaDadosTransporteIntegracao.ExisteComProtocoloPorCargaETipoIntegracaoRetornoCargaDadosTransporte(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
                if (cargaDadosTransporte == null)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Integração de SM não foi realizada na carga.";
                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                    return;
                }
            }

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoRaster || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRaster))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Raster.";

                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            string tipoAmbiente = "Homologacao";

            if (cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                tipoAmbiente = "Producao";

            string mensagem = "";

            if (cargaDadosTransporte == null)
            {
                if (CancelarSM(out mensagem, cargaCancelamentoIntegracao, cargaIntegracao, tipoAmbiente, configuracaoIntegracao, unitOfWork))
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                else
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                if (CancelarPreSM(out mensagem, cargaCancelamentoIntegracao, cargaDadosTransporte, tipoAmbiente, configuracaoIntegracao, unitOfWork))
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                else
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;

            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static bool ConsultarProprietarioVeiculo(out string mensagem, string cnpj, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""getProprietario""";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaProprietario integracaoConsultaProprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaProprietario()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                CNPJ = cnpj
            };


            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""getProprietario""", Method.POST);
            request.AddJsonBody(integracaoConsultaProprietario);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoProprietario = (int?)retorno.result[0].Proprietario?.Codigo ?? 0;
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0 && codigoProprietario > 0)
                    mensagem = "Proprietário do veículo já existe.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }



            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response.Content, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);


            return sucesso;
        }

        private static bool ConsultarVeiculo(out string mensagem, string placa, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""getVeiculo""";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaVeiculo integracaoConsultaVeiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaVeiculo()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                Placa = placa
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""getVeiculo""", Method.POST);
            request.AddJsonBody(integracaoConsultaVeiculo);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                    mensagem = "Veículo já existe.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response.Content, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);




            return sucesso;
        }

        private static bool ConsultarCarreta(out string mensagem, string placa, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""getCarreta""";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaCarreta integracaoConsultaCarreta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaCarreta()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                Placa = placa
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""getCarreta""", Method.POST);
            request.AddJsonBody(integracaoConsultaCarreta);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                    mensagem = "Carreta já existe.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response.Content, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);



            return sucesso;
        }

        private static bool ConsultarMotorista(out string mensagem, string cpf, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""getMotorista""";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaMotorista integracaoConsultaMotorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaMotorista()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                CPF = cpf
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""getMotorista""", Method.POST);
            request.AddJsonBody(integracaoConsultaMotorista);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoMotorista = (int?)retorno.result[0].Motorista?.Codigo ?? 0;
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0 && codigoMotorista > 0)
                    mensagem = "Motorista já existe.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response.Content, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);


            return sucesso;
        }

        private static bool IntegrarMotoristas(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao?.Carga ?? cargaDadosTransporteIntegracao?.Carga;

            foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
            {
                if (ConsultarMotorista(out mensagem, motorista.CPF, ambiente, configuracaoIntegracao, unitOfWork, cargaIntegracao, cargaDadosTransporteIntegracao))
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoMotorista integracaoMotorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoMotorista()
                {
                    Ambiente = ambiente,
                    Login = configuracaoIntegracao.UsuarioRaster,
                    Senha = configuracaoIntegracao.SenhaRaster,
                    TipoRetorno = "JSON",
                    Motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoMotoristaDados()
                    {
                        CPF = motorista.CPF,
                        Nome = motorista.Nome,
                        Sexo = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "F" : "M",
                        UFEmissCNH = motorista.Localidade?.Estado?.Sigla ?? string.Empty,
                        CodIBGECidadeNatal = motorista.Localidade?.CodigoIBGE ?? 0,
                        DataNascimento = motorista.DataNascimento,
                        CodIBGECidade = motorista.Localidade?.CodigoIBGE ?? 0,
                        CodProfissao = 1
                    }
                };

                var client = new RestClient(configuracaoIntegracao.URLRaster);
                var request = new RestRequest(@"/""setMotorista""", Method.POST);
                request.AddJsonBody(integracaoMotorista);
                IRestResponse response = client.Execute(request);

                bool sucesso = true;
                string jsonRetorno = response.Content;
                var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
                if (response.IsSuccessful)
                {
                    int codigoErro = ((int)retorno.result[0].CodErro);
                    mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                    if (codigoErro == 0)
                        mensagem = "Motorista integrado com sucesso.";
                    else
                        sucesso = false;
                }
                else
                {
                    mensagem = retorno.error.ToString();
                    sucesso = false;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    Data = DateTime.Now,
                    Mensagem = mensagem
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (cargaIntegracao != null)
                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                else if (cargaDadosTransporteIntegracao != null)
                    cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                if (!sucesso)
                    return false;
            }

            mensagem = string.Empty;
            return true;
        }

        public void IntegrarAtualizacaoRaster(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonRetorno = string.Empty;
            string mensagem = string.Empty;

            try
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoRaster = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoRaster = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaIntegracao.Carga.Codigo, tipoIntegracaoRaster.Codigo);


                if (cargaJanelaDescarregamento == null)
                    throw new ServicoException("Não existe janela de descarregamento para a carga informada.");

                if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoRaster || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaRaster) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLRaster))
                    throw new ServicoException("Não existe configuração de integração disponível para a Raster Atualização.");

                string ambiente = "Homologacao";

                if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    ambiente = "Producao";

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoRasterAtualizacao integracaoRasterAtualizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoRasterAtualizacao()
                {
                    Ambiente = ambiente,
                    Login = configuracaoIntegracao.UsuarioRaster,
                    Senha = configuracaoIntegracao.SenhaRaster,
                    TipoRetorno = "JSON",
                    DadosAgendamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.DadosAgendamento()
                    {
                        CodSolicitacao = cargaIntegracaoRaster.PreProtocolo,
                        PrevisaoChegada = cargaJanelaDescarregamento.InicioDescarregamento.ToString("yyyy-MM-dd HH:mm:ss"),
                        StatusAgendamento = SituacaCargaJanelaDescarregamentoHelper.ObterDescricao(cargaJanelaDescarregamento.Situacao)
                    }
                };

                RestClient client = new RestClient(configuracaoIntegracao.URLRaster);
                RestRequest request = new RestRequest(@"/""setAgendamentoViagem""", Method.POST);

                request.AddJsonBody(integracaoRasterAtualizacao);
                IRestResponse response = client.Execute(request);

                jsonRetorno = response.Content;
                jsonRequest = request.Body.Value.ToString();

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);

                if (retorno == null || !response.IsSuccessful)
                    throw new ServicoException("Erro ao integrar atualização Raster: " + (retorno != null ? retorno.error.ToString() : string.Empty));

                int codigoErro = (int)retorno.result[0].CodErro;
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                    mensagem = "Integração realizada com sucesso.";

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = mensagem;
            }
            catch (ServicoException ex)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = ex.Message;
                repCargaIntegracao.Atualizar(cargaIntegracao);
            }
            catch (Exception ex)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Erro ao integrar com a Raster";
                repCargaIntegracao.Atualizar(cargaIntegracao);

                Servicos.Log.TratarErro(ex, "RasterAtualizacao");
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = cargaIntegracao.PreProtocolo + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private static bool IntegrarVeiculo(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao?.Carga ?? cargaDadosTransporteIntegracao?.Carga;

            if (carga.Veiculo == null)
            {
                mensagem = "Nenhum veículo informado na carga.";
                return true;
            }

            if (!IntegrarProprietarioVeiculo(out mensagem, cargaIntegracao, cargaDadosTransporteIntegracao, carga.Veiculo, ambiente, configuracaoIntegracao, unitOfWork))
                return false;

            if (ConsultarVeiculo(out mensagem, carga.Veiculo.Placa_Formatada, ambiente, configuracaoIntegracao, unitOfWork, cargaIntegracao, cargaDadosTransporteIntegracao))
                return true;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoVeiculo integracaoVeiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoVeiculo()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                Veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoVeiculoDados()
                {
                    Placa = carga.Veiculo.Placa_Formatada,
                    CodIBGECidade = (carga.Veiculo.Tipo == "P" ? carga.Empresa.Localidade.CodigoIBGE : carga.Veiculo.Proprietario?.Localidade?.CodigoIBGE) ?? 0,
                    CodTipoVeiculo = carga.Veiculo.ModeloVeicularCarga?.CodigoIntegracaoGerenciadoraRisco?.ToInt() ?? 0,
                    CodMarca = carga.Veiculo.Marca?.CodigoIntegracao?.ToInt() ?? 0,
                    CodCor = 0,
                    AnoFabricacao = carga.Veiculo.AnoFabricacao,
                    AnoModelo = carga.Veiculo.AnoModelo,
                    CNPJProprietario = carga.Veiculo.Tipo == "P" ? carga.Empresa.CNPJ : carga.Veiculo.Proprietario?.CPF_CNPJ_SemFormato,
                    PossuiRastreador = "N"
                }
            };

            if (carga.Veiculo.PossuiRastreador)
            {
                integracaoVeiculo.Veiculo.PossuiRastreador = "S";
                integracaoVeiculo.Veiculo.TecnoRasPrincipal = carga.Veiculo.TecnologiaRastreador?.CodigoIntegracao?.ToInt() ?? 0;
                integracaoVeiculo.Veiculo.ModelRasPrincipal = carga.Veiculo.TipoComunicacaoRastreador?.CodigoIntegracao?.ToInt() ?? 0;
                integracaoVeiculo.Veiculo.TermiRasPrincipal = carga.Veiculo.NumeroEquipamentoRastreador;
            }

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setVeiculo""", Method.POST);
            request.AddJsonBody(integracaoVeiculo);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                    mensagem = "Veículo integrado com sucesso.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            return sucesso;
        }

        private static bool IntegrarProprietarioVeiculo(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Veiculo veiculo, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""setProprietario""";

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao?.Carga ?? cargaDadosTransporteIntegracao?.Carga;

            if (carga == null)
            {
                mensagem = "Carga não encontrada.";
                return true;
            }

            if (veiculo == null)
            {
                mensagem = "Veículo não informado na carga.";
                return true;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoProprietario integracaoProprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoProprietario()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON"
            };

            if (veiculo.Tipo == "P")
            {
                if (carga.Empresa == null)
                {
                    mensagem = "Empresa não informada na carga.";
                    return true;
                }

                if (ConsultarProprietarioVeiculo(out mensagem, carga.Empresa.CNPJ, ambiente, configuracaoIntegracao, unitOfWork, cargaIntegracao, cargaDadosTransporteIntegracao))
                    return true;

                integracaoProprietario.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoProprietarioDados()
                {
                    Razao = carga.Empresa.RazaoSocial,
                    CNPJ = carga.Empresa.CNPJ.ToLong(),
                    Endereco = carga.Empresa.Endereco,
                    Numero = Utilidades.String.Left(carga.Empresa.Numero, 5),
                    Bairro = carga.Empresa.Bairro,
                    CodIBGECidade = carga.Empresa.Localidade?.CodigoIBGE ?? 0,
                    CEP = carga.Empresa.CEP,
                    Telefone = carga.Empresa.Telefone
                };
            }
            else
            {
                if (ConsultarProprietarioVeiculo(out mensagem, veiculo.Proprietario?.CPF_CNPJ_SemFormato, ambiente, configuracaoIntegracao, unitOfWork, cargaIntegracao, cargaDadosTransporteIntegracao))
                    return true;

                integracaoProprietario.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoProprietarioDados()
                {
                    Razao = veiculo.Proprietario?.Nome ?? string.Empty,
                    CNPJ = (long)(veiculo?.Proprietario?.CPF_CNPJ ?? 0),
                    Endereco = veiculo.Proprietario?.Endereco ?? string.Empty,
                    Numero = Utilidades.String.Left(veiculo.Proprietario?.Numero ?? string.Empty, 5),
                    Bairro = veiculo.Proprietario?.Bairro ?? string.Empty,
                    CodIBGECidade = veiculo.Proprietario?.Localidade?.CodigoIBGE ?? 0,
                    CEP = veiculo.Proprietario?.CEP ?? string.Empty,
                    Telefone = veiculo.Proprietario?.Telefone1 ?? string.Empty
                };
            }

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setProprietario""", Method.POST);
            request.AddJsonBody(integracaoProprietario);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)

            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                    mensagem = "Proprietário do veículo integrado com sucesso.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            return sucesso;
        }

        private static bool IntegrarCarretas(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao?.Carga ?? cargaDadosTransporteIntegracao?.Carga;

            if (carga.VeiculosVinculados.Count() == 0)
            {
                mensagem = "Nenhum reboque informado na carga.";
                return true;
            }

            foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
            {
                if (!IntegrarProprietarioVeiculo(out mensagem, cargaIntegracao, null, veiculo, ambiente, configuracaoIntegracao, unitOfWork))
                    return false;

                if (ConsultarCarreta(out mensagem, veiculo.Placa_Formatada, ambiente, configuracaoIntegracao, unitOfWork, cargaIntegracao, cargaDadosTransporteIntegracao))
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCarreta integracaoCarreta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCarreta()
                {
                    Ambiente = ambiente,
                    Login = configuracaoIntegracao.UsuarioRaster,
                    Senha = configuracaoIntegracao.SenhaRaster,
                    TipoRetorno = "JSON",
                    Carreta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCarretaDados()
                    {
                        Placa = veiculo.Placa_Formatada,
                        CodIBGECidade = (veiculo.Tipo == "P" ? carga.Empresa?.Localidade.CodigoIBGE : veiculo.Proprietario?.Localidade?.CodigoIBGE) ?? 0,
                        CodTipoCarreta = veiculo.ModeloVeicularCarga?.CodigoIntegracaoGerenciadoraRisco?.ToInt() ?? 0,
                        CodMarca = veiculo.Marca?.CodigoIntegracao?.ToInt() ?? 0,
                        CodCor = 0,
                        AnoFabricacao = veiculo.AnoFabricacao,
                        AnoModelo = veiculo.AnoModelo,
                        CNPJProprietario = veiculo.Tipo == "P" ? carga.Empresa?.CNPJ : veiculo.Proprietario?.CPF_CNPJ_SemFormato,
                        PossuiRastreador = "N"
                    }
                };

                if (veiculo.PossuiRastreador)
                {
                    integracaoCarreta.Carreta.PossuiRastreador = "S";
                    integracaoCarreta.Carreta.TecnologiaRastreador = cargaIntegracao.Carga.Veiculo.TecnologiaRastreador?.CodigoIntegracao?.ToInt() ?? 0;
                    integracaoCarreta.Carreta.ModeloRastreador = cargaIntegracao.Carga.Veiculo.TipoComunicacaoRastreador?.CodigoIntegracao?.ToInt() ?? 0;
                    integracaoCarreta.Carreta.TerminalRastreador = cargaIntegracao.Carga.Veiculo.NumeroEquipamentoRastreador;
                }

                var client = new RestClient(configuracaoIntegracao.URLRaster);
                var request = new RestRequest(@"/""setCarreta""", Method.POST);
                request.AddJsonBody(integracaoCarreta);
                IRestResponse response = client.Execute(request);

                bool sucesso = true;
                string jsonRetorno = response.Content;
                var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
                if (response.IsSuccessful)
                {
                    int codigoErro = ((int)retorno.result[0].CodErro);
                    mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                    if (codigoErro == 0)
                        mensagem = "Carreta integrada com sucesso.";
                    else
                        sucesso = false;
                }
                else
                {
                    mensagem = retorno.error.ToString();
                    sucesso = false;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    Data = DateTime.Now,
                    Mensagem = carga.Veiculo.Placa + " - " + mensagem
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (cargaIntegracao != null)
                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                else if (cargaDadosTransporteIntegracao != null)
                    cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                if (!sucesso)
                    return false;
            }

            mensagem = string.Empty;
            return true;
        }

        private static bool IntegrarPreSM(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao = null)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao?.Carga ?? cargaDadosTransporteIntegracao?.Carga ?? cargaFreteIntegracao?.Carga;

            if (carga.Veiculo == null)
            {
                mensagem = "Nenhum veículo informado na carga.";
                return true;
            }

            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            Dominio.Entidades.Veiculo carreta1 = carga.VeiculosVinculados.Count() > 0 ? carga.VeiculosVinculados.ElementAt(0) : null;
            Dominio.Entidades.Veiculo carreta2 = carga.VeiculosVinculados.Count() > 1 ? carga.VeiculosVinculados.ElementAt(1) : null;
            Dominio.Entidades.Veiculo carreta3 = carga.VeiculosVinculados.Count() > 2 ? carga.VeiculosVinculados.ElementAt(2) : null;

            Dominio.Entidades.Usuario motorista1 = carga.Motoristas.Count() > 0 ? carga.Motoristas.ElementAt(0) : null;
            Dominio.Entidades.Usuario motorista2 = carga.Motoristas.Count() > 1 ? carga.Motoristas.ElementAt(1) : null;

            int codigo = 0;

            if (cargaFreteIntegracao != null)
            {

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                var cargaDadosTransporte = repCargaDadosTransporteIntegracao.ExisteComProtocoloPorCargaETipoIntegracaoRetornoCargaDadosTransporte(cargaFreteIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
                if (cargaDadosTransporte == null)
                {
                    mensagem = "Integração de SM não foi realizada na carga.";
                    cargaFreteIntegracao.ProblemaIntegracao = mensagem;
                    return false;
                }
                codigo = (cargaDadosTransporte?.PreProtocolo).ToNullableInt() ?? 0;
                if (codigo <= 0)
                {
                    mensagem = "Pre protocolo da de SM não pode ser vazio.";
                    cargaFreteIntegracao.ProblemaIntegracao = mensagem;
                    return false;
                }
            }
            else
                codigo = (cargaIntegracao?.PreProtocolo ?? cargaDadosTransporteIntegracao?.PreProtocolo).ToNullableInt() ?? 0;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SM integracaoPreSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SM()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                PreSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDados()
                {
                    Codigo = codigo,
                    Engate = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDadosEngate()
                    {
                        CodFilial = carga.TipoOperacao.CodigoFilialRaster,
                        CodPerfilSeguranca = carga.TipoOperacao.CodigoPerfilSegurancaRaster,
                        CPFMotorista1 = motorista1?.CPF,
                        VincMotorista1 = ObterVinculoMotorista(motorista1),
                        CPFMotorista2 = motorista2?.CPF,
                        VincMotorista2 = ObterVinculoMotorista(motorista2),
                        PlacaVeiculo = veiculo?.Placa_Formatada,
                        VincVeiculo = ObterVinculoVeiculo(veiculo),
                        PlacaCarreta1 = carreta1?.Placa_Formatada,
                        VincCarreta1 = ObterVinculoVeiculo(carreta1),
                        PlacaCarreta2 = carreta2?.Placa_Formatada,
                        VincCarreta2 = ObterVinculoVeiculo(carreta2),
                        PlacaCarreta3 = carreta3?.Placa_Formatada,
                        VincCarreta3 = ObterVinculoVeiculo(carreta3),
                        InfAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMInfAdicionais>(),
                        CNPJTransportador = carga.Empresa?.CNPJ_Formatado
                    },
                    Detalhamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamento()
                    {
                        ColetasEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntrega>(),
                    },
                    Rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoRota()
                    {
                        CodRota = carga.Rota?.CodigoIntegracao?.ToNullableInt() ?? 0
                    },
                    CheckList = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoCheckList()
                    {
                        SolicitarCheckList = "NAO"
                    },
                    LiberacaoEngate = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoLiberacaoEngate()
                    {
                        SolicitarPesquisa = "NAO"
                    }
                }
            };
            integracaoPreSM.PreSM.Engate.InfAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMInfAdicionais()
            {
                Codigo = 11,
                Observacao = carga.DataCriacaoCarga.ToString("yyyy-MM-dd HH:mm:ss")
            });

            integracaoPreSM.PreSM.Engate.InfAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMInfAdicionais()
            {
                Codigo = 12,
                Observacao = carga.TipoOperacao.Descricao
            });

            integracaoPreSM.PreSM.Engate.InfAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMInfAdicionais()
            {
                Codigo = 13,
                Observacao = carga.TipoDeCarga.Descricao
            });

            if (carga.TipoDeCarga != null && carga.TipoDeCarga.ControlaTemperatura && carga.TipoDeCarga.FaixaDeTemperatura != null)
                integracaoPreSM.PreSM.Engate.CodFaixaTemperatura = carga.TipoDeCarga.FaixaDeTemperatura.CodigoIntegracao.ToNullableInt() ?? 0;

            List<Dominio.Entidades.Cliente> clientesColeta = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteColeta != null).Select(cargaPedido => cargaPedido.ClienteColeta).Distinct().ToList();
            List<Dominio.Entidades.Cliente> clientesEntrega = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteEntrega != null).Select(cargaPedido => cargaPedido.ClienteEntrega).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente clienteColeta in clientesColeta)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorClienteColeta = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteColeta?.CPF_CNPJ == clienteColeta.CPF_CNPJ).ToList();
                DateTime? dataInicialColeta = cargaPedidosPorClienteColeta.Where(cargaPedido => cargaPedido.Pedido.DataInicialColeta.HasValue).Min(cargaPedido => cargaPedido.Pedido.DataInicialColeta);
                DateTime? dataFinalColeta = cargaPedidosPorClienteColeta.Where(cargaPedido => cargaPedido.Pedido.DataFinalColeta.HasValue).Min(cargaPedido => cargaPedido.Pedido.DataFinalColeta);
                DateTime dataSaida = dataFinalColeta ?? (dataInicialColeta ?? DateTime.MinValue).AddHours(2);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntrega coleta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntrega()
                {
                    Tipo = "COLETA",
                    CodIBGECidade = clienteColeta.Localidade.CodigoIBGE,
                    DataHoraSaida = dataSaida,
                    DataHoraChegada = carga.DataInicialPrevisaoCarregamento ?? DateTime.MinValue,
                    Cliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaCliente()
                    {
                        Razao = clienteColeta.Nome,
                        CNPJ = (long)clienteColeta.CPF_CNPJ,
                        Endereco = clienteColeta.Endereco,
                        Numero = Utilidades.String.Left(clienteColeta.Numero, 5),
                        Bairro = clienteColeta.Bairro,
                        CodIBGECidade = clienteColeta.Localidade.CodigoIBGE,
                        CEP = clienteColeta.CEP,
                        Telefone = clienteColeta.Telefone1,
                        Latitude = clienteColeta.Latitude.ToNullableDecimal() ?? 0m,
                        Longitude = clienteColeta.Longitude.ToNullableDecimal() ?? 0m
                    }
                };

                integracaoPreSM.PreSM.Detalhamento.ColetasEntregas.Add(coleta);
            }

            foreach (Dominio.Entidades.Cliente clienteEntrega in clientesEntrega)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorClienteEntrega = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteEntrega?.CPF_CNPJ == clienteEntrega.CPF_CNPJ).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = cargaPedidosPorClienteEntrega.SelectMany(cargaPedido => cargaPedido.NotasFiscais).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedidoNotasFiscais.Select(pedidoNotaFiscal => pedidoNotaFiscal.XMLNotaFiscal).Distinct().ToList();
                List<int> codigosPedidoNotasFiscais = pedidoNotasFiscais.Select(pedidoNotaFiscal => pedidoNotaFiscal.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = carga.CargaCTes.Where(cargaCTe => cargaCTe.NotasFiscais.Any(notaFiscalCte => codigosPedidoNotasFiscais.Contains(notaFiscalCte.PedidoXMLNotaFiscal.Codigo))).ToList();
                DateTime dataEntrega = cargaPedidosPorClienteEntrega.Where(cargaPedido => cargaPedido.Pedido.PrevisaoEntrega.HasValue).Min(cargaPedido => cargaPedido.Pedido.PrevisaoEntrega) ?? DateTime.MinValue;

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProdutoDocumento> documentos = (
                    from obj in cargaCTes
                    select new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProdutoDocumento()
                    {
                        Tipo = "CTE",
                        Numero = obj.CTe.Chave,
                        Valor = obj.CTe.ValorAReceber,
                        Peso = obj.CTe.Peso,
                        PesoCubado = obj.CTe.PesoCubado,
                        Volume = obj.CTe.Volumes,
                        Cubagem = obj.CTe.FatorCubagem
                    }
                ).ToList();

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProdutoDocumento()
                {
                    Tipo = "CARGA",
                    Numero = carga.CodigoCargaEmbarcador.ToString(),
                    Valor = (carga.DadosSumarizados?.ValorTotalProdutos ?? 0) > 0 ? carga.DadosSumarizados?.ValorTotalProdutos ?? 0 : carga.DadosSumarizados?.ValorTotalMercadoriaPedidos ?? 0,
                    Peso = carga.DadosSumarizados?.PesoTotal ?? 0,
                    PesoCubado = carga.Pedidos?.Sum(x => x.Pedido.PesoCubado) ?? 0,
                    Volume = carga.DadosSumarizados?.VolumesTotal ?? 0,
                    Cubagem = carga.DadosSumarizados?.CubagemTotal ?? 0
                });

                documentos.AddRange((
                    from obj in notasFiscais
                    select new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProdutoDocumento()
                    {
                        Tipo = "NOTAFISCAL",
                        Numero = obj.Chave,
                        Valor = obj.ValorTotalProdutos,
                        Peso = obj.Peso,
                        PesoCubado = obj.PesoCubado,
                        Volume = obj.Volumes,
                        Cubagem = obj.FatorCubagem
                    }
                ).ToList());

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntrega()
                {
                    Tipo = "ENTREGA",
                    CodIBGECidade = clienteEntrega.Localidade.CodigoIBGE,
                    DataHoraSaida = dataEntrega.AddHours(2),
                    DataHoraChegada = dataEntrega,
                    Cliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaCliente()
                    {
                        Razao = clienteEntrega.Nome,
                        CNPJ = (long)clienteEntrega.CPF_CNPJ,
                        Endereco = clienteEntrega.Endereco,
                        Numero = Utilidades.String.Left(clienteEntrega.Numero, 5),
                        Bairro = clienteEntrega.Bairro,
                        CodIBGECidade = clienteEntrega.Localidade.CodigoIBGE,
                        CEP = clienteEntrega.CEP,
                        Telefone = clienteEntrega.Telefone1,
                        Latitude = clienteEntrega.Latitude.ToNullableDecimal() ?? 0m,
                        Longitude = clienteEntrega.Longitude.ToNullableDecimal() ?? 0m
                    },
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProduto>()
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.SMDetalhamentoColetaEntregaProduto()
                        {
                            NCMProduto = carga.TipoDeCarga?.NCM,
                            Valor = notasFiscais.Sum(notaFiscal => notaFiscal.Valor),
                            Documentos = documentos
                        }
                    }
                };

                integracaoPreSM.PreSM.Detalhamento.ColetasEntregas.Add(entrega);
            }

            string preProtocolo = null;
            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setPRESM""", Method.POST);
            string jsonString = JsonConvert.SerializeObject(integracaoPreSM, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
            request.AddParameter(new JsonParameter("", jsonString));
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = (int)retorno.result[0].CodErro;
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                {
                    preProtocolo = (string)retorno.result[0].PreSM.Codigo;
                    mensagem = $"Pré SM integrada com sucesso, protocolo {preProtocolo}.";
                }
                else if (codigoErro == 138) // Pre-solicitacao de monitoramento ja foi efetivada
                {
                    preProtocolo = (string)retorno.result[0].PreSM.Codigo;
                    mensagem = $"{mensagem}, protocolo {preProtocolo}.";
                }
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno?.error?.ToString() ?? "Erro, rotorno vazio.";
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
            {
                cargaIntegracao.ProblemaIntegracao = mensagem;
                cargaIntegracao.PreProtocolo = preProtocolo;
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            else if (cargaDadosTransporteIntegracao != null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                cargaDadosTransporteIntegracao.PreProtocolo = preProtocolo;
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            else if (cargaFreteIntegracao != null)
            {
                cargaFreteIntegracao.ProblemaIntegracao = mensagem;
                //cargaFreteIntegracao.Protocolo = preProtocolo;
                cargaFreteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }

            return sucesso;
        }

        private static bool EfetivarPreSM(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao = null)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            int preProtocolo = (cargaIntegracao?.PreProtocolo ?? cargaDadosTransporteIntegracao?.PreProtocolo).ToNullableInt() ?? 0;

            if (cargaFreteIntegracao != null)
                carga = cargaFreteIntegracao.Carga;
            else if (cargaDadosTransporteIntegracao != null)
                carga = cargaDadosTransporteIntegracao.Carga;
            else if (cargaIntegracao != null)
                carga = cargaIntegracao.Carga;

            if (carga.Veiculo == null)
            {
                mensagem = "Nenhum veículo informado na carga.";
                return true;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoEfetivaSM integracaoEfetivaSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoEfetivaSM()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                CodPreSolicitacao = preProtocolo,
                JaPassouRaioOrigem = "N"
            };

            string protocolo = null;

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setEfetivaPreSM""", Method.POST);
            request.AddJsonBody(integracaoEfetivaSM);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = (int)retorno.result[0].CodErro;
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                {
                    protocolo = (string)retorno.result[0].CodSolicitacao;
                    mensagem = $"Solicitação de monitoramento integrada com sucesso, protocolo {protocolo}.";
                }
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaFreteIntegracao != null)
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
                if (cargaDadosTransporteIntegracao != null)
                {
                    cargaDadosTransporteIntegracao.Protocolo = protocolo;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                }

                cargaFreteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                cargaFreteIntegracao.ProblemaIntegracao = mensagem;
            }
            else if (cargaDadosTransporteIntegracao != null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                cargaDadosTransporteIntegracao.Protocolo = protocolo;
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            else if (cargaIntegracao != null)
            {
                cargaIntegracao.Protocolo = protocolo;
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                cargaIntegracao.ProblemaIntegracao = mensagem;
            }

            return sucesso;
        }

        private static bool CancelarSM(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCancelaSM integracaoCancelaSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCancelaSM()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                CodSolicitacao = cargaIntegracao.Protocolo.ToNullableInt() ?? 0,
                Motivo = cargaCancelamentoIntegracao.CargaCancelamento.MotivoCancelamento
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setCancelaSM""", Method.POST);
            request.AddJsonBody(integracaoCancelaSM);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;
                string cancelou = (string)retorno.result[0].Cancelou;

                if (codigoErro == 0 && cancelou == "SIM")
                    mensagem = "Cancelamento realizado com sucesso.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = cargaIntegracao.Carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            return sucesso;
        }

        private static bool CancelarPreSM(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTRansporteIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCancelaPreSM integracaoCancelaPreSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoCancelaPreSM()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                CodPreSolicitacao = cargaDadosTRansporteIntegracao.PreProtocolo.ToNullableInt() ?? 0,
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setCancelaPreSM""", Method.POST);
            request.AddJsonBody(integracaoCancelaPreSM);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = ((int)retorno.result[0].CodErro);
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;
                string cancelou = (string)retorno.result[0].Cancelou;

                if (codigoErro == 0 && cancelou == "SIM")
                    mensagem = "Cancelamento setCancelaPreSM realizado com sucesso.";
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno.error.ToString();
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = cargaDadosTRansporteIntegracao.Carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            return sucesso;
        }

        private static string ObterVinculoVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return null;

            if (veiculo.Tipo == "P")
                return "F";
            else
                return veiculo.TipoProprietario == Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado ? "A" : "T";
        }

        private static string ObterVinculoMotorista(Dominio.Entidades.Usuario usuario)
        {
            if (usuario == null)
                return null;

            if (usuario.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio)
                return "F";
            else
                return "T";
        }

        private static bool ConsultarTabela(out string mensagem, string tabela, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlWebService = configuracaoIntegracao.URLRaster + @"/""getTabela""";

            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaTabela integracaoConsultaTabela = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.IntegracaoConsultaTabela()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                NomeTabela = "PERFIL_SEGURANCA"
            };

            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""getTabela""", Method.POST);
            request.AddJsonBody(integracaoConsultaTabela);
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);


            //bool sucesso = true;

            //if (response.IsSuccessStatusCode)
            //{
            //    //int codigoErro = ((int)retorno.result[0].CodErro);
            //    //mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

            //    //if (codigoErro == 0)
            //    //    mensagem = "Veículo já existe.";
            //    //else
            //    //    sucesso = false;
            //}
            //else
            //{
            //    //mensagem = retorno.error.ToString();
            //    //sucesso = false;
            //}



            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response.Content, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = ""
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (cargaIntegracao != null)
                cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            else if (cargaDadosTransporteIntegracao != null)
                cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);





            mensagem = string.Empty;
            return true;
        }

        private static bool SetRevisaoPreSm(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string ambiente, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Raster);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaIntegracao.Carga;

            if (carga.Veiculo == null)
            {
                mensagem = "Nenhum veículo informado na carga.";
                return true;
            }

            int codigo = (cargaDadosTransporteIntegracao?.Protocolo ?? (cargaIntegracao?.Protocolo ?? "0")).ToNullableInt() ?? 0;
            Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoPreSM integracaoRevisaoPreSM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoPreSM()
            {
                Ambiente = ambiente,
                Login = configuracaoIntegracao.UsuarioRaster,
                Senha = configuracaoIntegracao.SenhaRaster,
                TipoRetorno = "JSON",
                SM = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDados()
                {
                    Codigo = codigo,
                    Detalhamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamento()
                    {
                        ColetasEntregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntrega>(),
                    },
                    Rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoRota()
                    {
                        CodRota = carga.Rota?.CodigoIntegracao?.ToNullableInt() ?? 0
                    },
                    CheckList = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoCheckList()
                    {
                        SolicitarCheckList = "NAO"
                    }
                }
            };
            List<Dominio.Entidades.Cliente> clientesColeta = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteColeta != null).Select(cargaPedido => cargaPedido.ClienteColeta).Distinct().ToList();
            List<Dominio.Entidades.Cliente> clientesEntrega = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteEntrega != null).Select(cargaPedido => cargaPedido.ClienteEntrega).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente clienteColeta in clientesColeta)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorClienteColeta = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteColeta?.CPF_CNPJ == clienteColeta.CPF_CNPJ).ToList();
                DateTime? dataInicialColeta = cargaPedidosPorClienteColeta.Where(cargaPedido => cargaPedido.Pedido.DataInicialColeta.HasValue).Min(cargaPedido => cargaPedido.Pedido.DataInicialColeta);
                DateTime? dataFinalColeta = cargaPedidosPorClienteColeta.Where(cargaPedido => cargaPedido.Pedido.DataFinalColeta.HasValue).Min(cargaPedido => cargaPedido.Pedido.DataFinalColeta);
                DateTime dataSaida = dataFinalColeta ?? (dataInicialColeta ?? DateTime.MinValue).AddHours(2);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntrega coleta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntrega()
                {
                    Tipo = "COLETA",
                    CodIBGECidade = clienteColeta.Localidade.CodigoIBGE,
                    DataHoraSaida = dataSaida,
                    DataHoraChegada = carga.DataInicialPrevisaoCarregamento ?? DateTime.MinValue,
                    Cliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaCliente()
                    {
                        Razao = clienteColeta.Nome,
                        CNPJ = (long)clienteColeta.CPF_CNPJ,
                        Endereco = clienteColeta.Endereco,
                        Numero = Utilidades.String.Left(clienteColeta.Numero, 5),
                        Bairro = clienteColeta.Bairro,
                        CodIBGECidade = clienteColeta.Localidade.CodigoIBGE,
                        CEP = clienteColeta.CEP,
                        Telefone = clienteColeta.Telefone1,
                        Latitude = clienteColeta.Latitude.ToNullableDecimal() ?? 0m,
                        Longitude = clienteColeta.Longitude.ToNullableDecimal() ?? 0m
                    }
                };

                integracaoRevisaoPreSM.SM.Detalhamento.ColetasEntregas.Add(coleta);
            }

            foreach (Dominio.Entidades.Cliente clienteEntrega in clientesEntrega)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorClienteEntrega = carga.Pedidos.Where(cargaPedido => cargaPedido.ClienteEntrega?.CPF_CNPJ == clienteEntrega.CPF_CNPJ).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotasFiscais = cargaPedidosPorClienteEntrega.SelectMany(cargaPedido => cargaPedido.NotasFiscais).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedidoNotasFiscais.Select(pedidoNotaFiscal => pedidoNotaFiscal.XMLNotaFiscal).Distinct().ToList();
                List<int> codigosPedidoNotasFiscais = pedidoNotasFiscais.Select(pedidoNotaFiscal => pedidoNotaFiscal.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = carga.CargaCTes.Where(cargaCTe => cargaCTe.NotasFiscais.Any(notaFiscalCte => codigosPedidoNotasFiscais.Contains(notaFiscalCte.PedidoXMLNotaFiscal.Codigo))).ToList();
                DateTime dataEntrega = cargaPedidosPorClienteEntrega.Where(cargaPedido => cargaPedido.Pedido.PrevisaoEntrega.HasValue).Min(cargaPedido => cargaPedido.Pedido.PrevisaoEntrega) ?? DateTime.MinValue;

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProdutoDocumento> documentos = (
                    from obj in cargaCTes
                    select new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProdutoDocumento()
                    {
                        Tipo = "CTE",
                        Numero = obj.CTe.Chave,
                        Valor = obj.CTe.ValorAReceber,
                        Peso = obj.CTe.Peso,
                        PesoCubado = obj.CTe.PesoCubado,
                        Volume = obj.CTe.Volumes,
                        Cubagem = obj.CTe.FatorCubagem
                    }
                ).ToList();

                documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProdutoDocumento()
                {
                    Tipo = "CARGA",
                    Numero = carga.CodigoCargaEmbarcador.ToString(),
                    Valor = (carga.DadosSumarizados?.ValorTotalProdutos ?? 0) > 0 ? carga.DadosSumarizados?.ValorTotalProdutos ?? 0 : carga.DadosSumarizados?.ValorTotalMercadoriaPedidos ?? 0,
                    Peso = carga.DadosSumarizados?.PesoTotal ?? 0,
                    PesoCubado = carga.Pedidos?.Sum(x => x.Pedido.PesoCubado) ?? 0,
                    Volume = carga.DadosSumarizados?.VolumesTotal ?? 0,
                    Cubagem = carga.DadosSumarizados?.CubagemTotal ?? 0
                });

                documentos.AddRange((
                    from obj in notasFiscais
                    select new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProdutoDocumento()
                    {
                        Tipo = "NOTAFISCAL",
                        Numero = obj.Chave,
                        Valor = obj.ValorTotalProdutos,
                        Peso = obj.Peso,
                        PesoCubado = obj.PesoCubado,
                        Volume = obj.Volumes,
                        Cubagem = obj.FatorCubagem
                    }
                ).ToList());

                Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntrega()
                {
                    Tipo = "ENTREGA",
                    CodIBGECidade = clienteEntrega.Localidade.CodigoIBGE,
                    DataHoraSaida = dataEntrega.AddHours(2),
                    DataHoraChegada = dataEntrega,
                    Cliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaCliente()
                    {
                        Razao = clienteEntrega.Nome,
                        CNPJ = (long)clienteEntrega.CPF_CNPJ,
                        Endereco = clienteEntrega.Endereco,
                        Numero = Utilidades.String.Left(clienteEntrega.Numero, 5),
                        Bairro = clienteEntrega.Bairro,
                        CodIBGECidade = clienteEntrega.Localidade.CodigoIBGE,
                        CEP = clienteEntrega.CEP,
                        Telefone = clienteEntrega.Telefone1,
                        Latitude = clienteEntrega.Latitude.ToNullableDecimal() ?? 0m,
                        Longitude = clienteEntrega.Longitude.ToNullableDecimal() ?? 0m
                    },
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProduto>()
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Raster.RevisaoSMDetalhamentoColetaEntregaProduto()
                        {
                            NCMProduto = carga.TipoDeCarga?.NCM,
                            Valor = notasFiscais.Sum(notaFiscal => notaFiscal.Valor),
                            Documentos = documentos
                        }
                    }
                };

                integracaoRevisaoPreSM.SM.Detalhamento.ColetasEntregas.Add(entrega);
            }

            string preProtocolo = null;
            var client = new RestClient(configuracaoIntegracao.URLRaster);
            var request = new RestRequest(@"/""setRevisaoPreSm""", Method.POST);
            string jsonString = JsonConvert.SerializeObject(integracaoRevisaoPreSM, new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddTHH:mm:ss" });
            request.AddParameter(new JsonParameter("", jsonString));
            IRestResponse response = client.Execute(request);

            bool sucesso = true;
            string jsonRetorno = response.Content;
            var retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (response.IsSuccessful)
            {
                int codigoErro = (int)retorno.result[0].CodErro;
                mensagem = codigoErro + " - " + (string)retorno.result[0].MsgErro;

                if (codigoErro == 0)
                {
                    preProtocolo = (string)retorno.result[0].SM.Codigo;
                    mensagem = $"Revisão Pré SM integrada com sucesso, protocolo {preProtocolo}.";
                }
                else if (codigoErro == 138) // Pre-solicitacao de monitoramento ja foi efetivada
                {
                    preProtocolo = (string)retorno.result[0].SM.Codigo;
                    mensagem = $"{mensagem}, protocolo {preProtocolo}.";
                }
                else
                    sucesso = false;
            }
            else
            {
                mensagem = retorno?.error?.ToString() ?? "Erro, rotorno vazio.";
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request.Body.Value.ToString(), "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                Data = DateTime.Now,
                Mensagem = carga.Veiculo.Placa + " - " + mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ProblemaIntegracao = mensagem;
            cargaIntegracao.PreProtocolo = preProtocolo;
            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            return sucesso;
        }

        #endregion Métodos Privados
    }
}
