using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Servicos.ServicoPamCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.Pamcard
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard _integracaoPamcard;

        #endregion Atributos Globais

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            cargaIntegracaoValePedagio.DataIntegracao = DateTime.Now;
            cargaIntegracaoValePedagio.NumeroTentativas++;

            try
            {
                ObterIntegracaoPamcard(cargaIntegracaoValePedagio.Carga, tipoServicoMultisoftware);

                if (_integracaoPamcard == null)
                    throw new ServicoException("Não possui configuração para Pamcard.");

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string codigoRotaEmbarcador = string.Empty;

                if (_integracaoPamcard.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaPamcard.RotaFixa)
                    codigoRotaEmbarcador = cargaIntegracaoValePedagio.CodigoIntegracaoValePedagio;

                Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(carga, _integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio);

                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(filial, carga.Empresa, _unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

                ExecutarRouter(svcPamCard, cargaIntegracaoValePedagio, codigoRotaEmbarcador, tipoServicoMultisoftware, inspector);

                Dictionary<string, string> responseInsertTrip = ExecutarInsertTrip(svcPamCard, cargaIntegracaoValePedagio, codigoRotaEmbarcador, tipoServicoMultisoftware, inspector);

                string viagemID = responseInsertTrip.GetValue<string>("viagem.id");

                ExecutarPayToll(svcPamCard, cargaIntegracaoValePedagio, viagemID, tipoServicoMultisoftware, inspector);

                decimal valorValePedagio = responseInsertTrip.GetValue<string>("viagem.pedagio.valor").ToDecimal();

                cargaIntegracaoValePedagio.IdCompraValePedagio = viagemID;
                cargaIntegracaoValePedagio.NumeroValePedagio = viagemID;
                cargaIntegracaoValePedagio.ValorValePedagio = valorValePedagio;
                cargaIntegracaoValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cargaIntegracaoValePedagio.ProblemaIntegracao = string.Empty;

                repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);

                if (cargaIntegracaoValePedagio.Carga.PossuiPendencia)
                    InformarCargaIntegrandoValePedagio(carga);

                SalvarDadosRetornoCompraValePedagio(responseInsertTrip, carga);
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoTemPedagioNaRota)
            {
                cargaIntegracaoValePedagio.ProblemaIntegracao = excecao.Message;
                cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaIntegracaoValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
            }
            catch (ServicoException ex)
            {
                cargaIntegracaoValePedagio.ProblemaIntegracao = ex.Message;
                cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValePedagio");

                cargaIntegracaoValePedagio.ProblemaIntegracao = "Falha no serviço da Pamcard";
                cargaIntegracaoValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ObterIntegracaoPamcard(cargaValePedagio.Carga, tipoServicoMultisoftware);

            if (_integracaoPamcard == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Pamcard.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            ServicoPamCard.execute execute = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "CancelTrip",
                    fields = ObterCamposCancelamento(cargaValePedagio, tipoServicoMultisoftware, _unitOfWork)
                }
            };

            Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(cargaValePedagio.Carga, _integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio);

            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(filial, cargaValePedagio.Carga.Empresa, _unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoPamCard.executeResponse retorno = null;

            try
            {
                retorno = svcPamCard.execute(execute);

                if (retorno != null)
                {
                    string codigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                    string mensagemRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                }
                else
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao enviar o cancelamento para a Pamcard.";
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao enviar para a Pamcard: " + ex.Message);

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento de Vale Pedágio";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public decimal ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            decimal valorRouter = 0;
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            try
            {
                ObterIntegracaoPamcard(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (_integracaoPamcard == null)
                    throw new ServicoException("Não possui configuração para Pamcard.");

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicoPamCard.execute executeRoter = new ServicoPamCard.execute()
                {
                    arg0 = new ServicoPamCard.requestTO()
                    {
                        context = "Router",
                        fields = ObterCamposConsultaValePedagio(carga)
                    }
                };

                Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(carga, _integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio);

                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(filial, carga.Empresa, _unitOfWork, out inspector);

                ServicoPamCard.executeResponse retornoRouter = null;

                try
                {
                    retornoRouter = svcPamCard.execute(executeRoter);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex.Message);
                    throw new ServicoException("Consulta Rota (Router) não integrada com a Pamcard");
                }

                if (retornoRouter == null)
                    throw new ServicoException("Consulta Rota (Router) não teve redorno da Pamcard");

                string codigo = (from obj in retornoRouter.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();

                if (codigo != "0")
                {
                    string mensagem = (from obj in retornoRouter.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();

                    throw new ServicoException(mensagem ?? "Consulta Rota (Router) não teve redorno da Pamcard");
                }

                string valor = (from obj in retornoRouter.@return where obj.key.Equals("viagem.pedagio.valor") select obj.value).FirstOrDefault();

                Servicos.Log.TratarErro(Newtonsoft.Json.JsonConvert.SerializeObject($"Carga: {cargaValePedagio.Carga.CodigoCargaEmbarcador} JSON: {retornoRouter}"), "ConsultaValePedagio");

                valorRouter = valor.ToDecimal();
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaValePedagio.ProblemaIntegracao = "Falha no serviço da Pamcard";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            if (!string.IsNullOrWhiteSpace(inspector.LastRequestXML) && !string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return valorRouter;
        }

        public void ConsultarIdVpoPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            try
            {
                ObterIntegracaoPamcard(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (_integracaoPamcard == null)
                    throw new ServicoException("Não possui configuração para Pamcard.");

                Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(cargaValePedagio.Carga, _integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio);
                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(filial, cargaValePedagio.Carga.Empresa, _unitOfWork, out inspector);

                executeResponse retornoConsultaIdVpo = ExecutarFindTrip(svcPamCard, cargaValePedagio);

                if (retornoConsultaIdVpo == null)
                    throw new ServicoException("Retorno sem dados ao realizar a consulta do vale pedágio");

                string idVpo = (from obj in retornoConsultaIdVpo.@return where obj.key.Equals("viagem.pedagio.protocolo") select obj.value).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(idVpo) || idVpo == "0")
                    throw new ServicoException("IdVpo não encontrado ao realizar a consulta do vale pedágio");

                cargaValePedagio.CodigoEmissaoValePedagioANTT = idVpo;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a consulta do ID VPO do vale pedágio";
            }

            SalvarXMLIntegracao(ref cargaValePedagio, inspector, "Consulta IdVpo");
            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public byte[] GerarImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                ObterIntegracaoPamcard(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (_integracaoPamcard == null)
                    throw new ServicoException("Não possui configuração para Pamcard.");

                Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(cargaValePedagio.Carga, _integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio);
                ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(filial, cargaValePedagio.Carga.Empresa, _unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspector);

                executeResponse retornoFindTrip = ExecutarFindTrip(svcPamCard, cargaValePedagio);

                if (retornoFindTrip == null)
                    return null;

                return GerarRelatorioReciboValePedagio(retornoFindTrip.@return);
            }
            catch (ServicoException excecao)
            {
                return null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        #region Executar Compra

        private void ExecutarRouter(ServicoPamCard.WSTransacionalClient svcPamCard, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, string codigoRotaEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            ServicoPamCard.execute executeRouter = FormatarRequest("Router", ObterCamposObterRota(cargaIntegracaoValePedagio.Carga, codigoRotaEmbarcador, tipoServicoMultisoftware, _unitOfWork));

            ServicoPamCard.executeResponse retornoRouter = null;

            try
            {
                retornoRouter = svcPamCard.execute(executeRouter);
                SalvarXMLIntegracao(ref cargaIntegracaoValePedagio, inspector, "Consulta Rota");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.Message);
                throw new ServicoException("Consulta Rota (Router) não integrada com a Pamcard");
            }

            if (retornoRouter == null)
                throw new ServicoException("Consulta Rota (Router) não teve retorno da Pamcard");

            Dictionary<string, string> responseRouter = ObterDicionario(retornoRouter.@return);

            string codigoRetornoRouter = responseRouter.GetValue<string>("mensagem.codigo");
            string mensagemRetornoRouter = responseRouter.GetValue<string>("mensagem.descricao");
            string valorRetornoPedagio = responseRouter.GetValue<string>("viagem.pedagio.valor");

            if (codigoRetornoRouter != "0")
                throw new ServicoException($"{codigoRetornoRouter} - {mensagemRetornoRouter}");

            decimal valorValePedagio = valorRetornoPedagio.ToDecimal();

            if (valorValePedagio == 0)
            {
                InformarCargaIntegrandoValePedagio(cargaIntegracaoValePedagio.Carga);

                throw new ServicoException("Rota sem valor de Vale pedágio na Pamcard", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoTemPedagioNaRota);
            }
        }

        private Dictionary<string, string> ExecutarInsertTrip(ServicoPamCard.WSTransacionalClient svcPamCard, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, string codigoRotaEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            ServicoPamCard.execute executeInsertTrip = FormatarRequest("InsertTrip", ObterCamposViagem(cargaIntegracaoValePedagio.Carga, codigoRotaEmbarcador, tipoServicoMultisoftware, _unitOfWork));

            ServicoPamCard.executeResponse retornoInsertTrip = null;

            try
            {
                retornoInsertTrip = svcPamCard.execute(executeInsertTrip);
                SalvarXMLIntegracao(ref cargaIntegracaoValePedagio, inspector, "Compra Pedágio");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.Message);
                throw new ServicoException("Compra vale pedágio (InsertTrip) não integrada com a Pamcard");
            }

            if (retornoInsertTrip == null)
                throw new ServicoException("Compra vale pedágio (InsertTrip) não integrada com a Pamcard");

            Dictionary<string, string> responseInsertTrip = ObterDicionario(retornoInsertTrip.@return);

            string codigoRetornoTrip = responseInsertTrip.GetValue<string>("mensagem.codigo");
            string mensagemRetornoTrip = responseInsertTrip.GetValue<string>("mensagem.descricao");
            string valorRetornoPedagio = responseInsertTrip.GetValue<string>("viagem.pedagio.valor");

            if (codigoRetornoTrip != "0")
                throw new ServicoException($"{codigoRetornoTrip} - {mensagemRetornoTrip}");

            decimal valorValePedagio = valorRetornoPedagio.ToDecimal();

            if (valorValePedagio == 0)
            {
                InformarCargaIntegrandoValePedagio(cargaIntegracaoValePedagio.Carga);

                throw new ServicoException("Viagem sem valor de Vale pedágio na Pamcard", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoTemPedagioNaRota);
            }

            return responseInsertTrip;
        }

        private void ExecutarPayToll(ServicoPamCard.WSTransacionalClient svcPamCard, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, string viagemID, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            ServicoPamCard.execute executePayToll = FormatarRequest("PayToll", ObterCamposPayToll(cargaIntegracaoValePedagio, tipoServicoMultisoftware, _unitOfWork, viagemID));

            ServicoPamCard.executeResponse retornoPayToll = null;

            try
            {
                retornoPayToll = svcPamCard.execute(executePayToll);
                SalvarXMLIntegracao(ref cargaIntegracaoValePedagio, inspector, "Compra Pedágio");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.Message);
                throw new ServicoException("Compra vale pedágio (PayToll) não integrada com a Pamcard");
            }

            if (retornoPayToll == null)
                throw new ServicoException("Compra vale pedágio (PayToll) não integrada com a Pamcard");

            Dictionary<string, string> responsePayToll = ObterDicionario(retornoPayToll.@return);

            string codigoRetornoPayToll = responsePayToll.GetValue<string>("mensagem.codigo");
            string mensagemRetornoPayToll = responsePayToll.GetValue<string>("mensagem.descricao");

            if (codigoRetornoPayToll != "0")
                throw new ServicoException($"{codigoRetornoPayToll} - {mensagemRetornoPayToll}");

            string idVpo = responsePayToll.GetValue<string>("viagem.pedagio.protocolo");

            if (!string.IsNullOrWhiteSpace(idVpo))
                cargaIntegracaoValePedagio.CodigoEmissaoValePedagioANTT = idVpo;
        }

        private ServicoPamCard.executeResponse ExecutarFindTrip(ServicoPamCard.WSTransacionalClient svcPamCard, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, Servicos.Models.Integracao.InspectorBehavior inspector = null)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoPamCard.execute executeFindTrip = new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = "FindTrip",
                    fields = ObterCamposImpressaoValePedagio(cargaIntegracaoValePedagio)
                }
            };

            ServicoPamCard.executeResponse retornoFindTrip = null;

            try
            {
                retornoFindTrip = svcPamCard.execute(executeFindTrip);

                fieldTO[] request = retornoFindTrip.@return;

                if (request.Where(obj => obj.key.Equals("mensagem.codigo")).Select(obj => obj.value).FirstOrDefault() != "0" ||
                    request.Where(obj => obj.key.Equals("mensagem.descricao")).Select(obj => obj.value).FirstOrDefault() != "Operação realizada com sucesso")
                {
                    throw new ServicoException(request.Where(obj => obj.key.Equals("mensagem.descricao")).Select(obj => obj.value).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.Message);
                throw new ServicoException("Consulta do Vale Pedágio não disponível Pamcard");
            }

            return retornoFindTrip;
        }

        #endregion Executar Compra

        #region Obter Campos

        private ServicoPamCard.execute FormatarRequest(string context, ServicoPamCard.fieldTO[] campos)
        {
            return new ServicoPamCard.execute()
            {
                arg0 = new ServicoPamCard.requestTO()
                {
                    context = context,
                    fields = campos
                }
            };
        }

        private ServicoPamCard.fieldTO[] ObterCamposObterRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, string rotaEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                string cnpjContratante = ObterContratante(carga);

                ServicoPamCard.fieldTO numeroContratanteDocumento = new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante };
                ServicoPamCard.fieldTO numeroViagemUnidadeDocumento = new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = carga.Filial?.CNPJ ?? carga.Empresa.CNPJ };

                campos.Add(numeroContratanteDocumento);
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });

                if (configuracaoGeralCarga?.ValidarContratanteOrigemVPIntegracaoPamcard ?? false)
                {
                    bool ehDiferente = numeroContratanteDocumento.value != numeroViagemUnidadeDocumento.value;

                    if (ehDiferente && carga.Filial is null)
                        campos.Add(numeroViagemUnidadeDocumento);
                }
                else
                    campos.Add(numeroViagemUnidadeDocumento);
            }

            (string categoriaVeiculo, string categoriaVeiculoEixoSuspenso) = ObterCategoriaVeiculo(carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = categoriaVeiculo });

            if (!string.IsNullOrWhiteSpace(categoriaVeiculoEixoSuspenso))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria.eixo.suspenso", value = categoriaVeiculoEixoSuspenso });

            if (!string.IsNullOrWhiteSpace(rotaEmbarcador))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.rota.id", value = rotaEmbarcador });
            else
                campos.AddRange(ObterPontosRota(carga));

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.praca", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.rota", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.tempo.percurso", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.roteirizar.tipo", value = "1" });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, string codigoRota, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Usuario motorista = null;

            if (carga.Motoristas?.Count > 0)
                motorista = carga.Motoristas.FirstOrDefault();

            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga _repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = _repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                string cnpjContratante = ObterContratante(carga);

                ServicoPamCard.fieldTO numeroContratanteDocumento = new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante };
                ServicoPamCard.fieldTO numeroViagemUnidadeDocumento = new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = carga.Filial?.CNPJ ?? carga.Empresa.CNPJ };

                campos.Add(numeroContratanteDocumento);
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });

                if (configuracaoGeralCarga?.ValidarContratanteOrigemVPIntegracaoPamcard ?? false)
                {
                    bool ehDiferente = numeroContratanteDocumento.value != numeroViagemUnidadeDocumento.value;

                    if (ehDiferente && carga.Filial is null)
                        campos.Add(numeroViagemUnidadeDocumento);
                }
                else
                    campos.Add(numeroViagemUnidadeDocumento);
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.qtde", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.tipo", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento.qtde", value = "1" });

            if (carga.Veiculo?.Tipo == "T" && carga.Veiculo.Proprietario != null)
            {
                ObterCamposFavorecidoTerceiro(campos, carga.Veiculo.Proprietario);
            }
            else
            {
                if (motorista?.TipoCartao.HasValue ?? false)
                {
                    if (motorista.TipoCartao == TipoPessoaCartao.Fisica)
                        ObterCamposFavorecidoPF(campos, motorista);
                    else
                        ObterCamposFavorecidoPJ(campos, motorista.Empresa, possuiCartao: true);
                }
                else
                    ObterCamposFavorecidoPJ(campos, carga.Empresa, possuiCartao: false);
            }

            if (carga.Empresa.Banco != null)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.banco", value = carga.Empresa.Banco.Numero.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.agencia", value = carga.Empresa.Agencia });

                if (!string.IsNullOrWhiteSpace(carga.Empresa.DigitoAgencia))
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.agencia.digito", value = carga.Empresa.DigitoAgencia });

                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.numero", value = carga.Empresa.NumeroConta });

                if (carga.Empresa.TipoContaBanco == TipoContaBanco.Poupança)
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.tipo", value = "2" });
                else
                    campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.conta.tipo", value = "1" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.solucao.id", value = "6" }); //TAG

            string valorTagEmissorValePedagio = carga.Veiculo?.ModoCompraValePedagioTarget?.ObterValorTagEmissorValePedagio();

            if (!string.IsNullOrWhiteSpace(valorTagEmissorValePedagio))
                campos.Add(new fieldTO { key = "viagem.pedagio.tag.emissor.id", value = valorTagEmissorValePedagio });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.placa", value = carga.Veiculo.Placa });

            (string categoriaVeiculo, string categoriaVeiculoEixoSuspenso) = ObterCategoriaVeiculo(carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = categoriaVeiculo });

            if (!string.IsNullOrWhiteSpace(categoriaVeiculoEixoSuspenso))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria.eixo.suspenso", value = categoriaVeiculoEixoSuspenso });

            DateTime dataPartida = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value : carga.DataCriacaoCarga;

            bool idaVolta = !_integracaoPamcard.NaoEnviarIdaVoltaValePedagio && RotaFreteComVolta(carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.partida", value = dataPartida > DateTime.Now.AddDays(-2) ? dataPartida.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.idavolta", value = idaVolta ? "S" : "N" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.status.id", value = "2" }); //2 = Liberado
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.roteirizar", value = "S" }); //Conforme a lei do Vale-Pedágio a roteirização da viagem é obrigatória, portanto, é necessário que os dados da rota sejam enviados e o parâmetro viagem.pedagio.roteirizar tenha valor S.

            if (!string.IsNullOrWhiteSpace(codigoRota))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.rota.id", value = codigoRota });
            else
                campos.AddRange(ObterPontosRota(carga));

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.qtde", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.tipo", value = "5" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.numero", value = carga.Codigo.ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = "0" });

            return campos.ToArray();
        }

        private List<fieldTO> ObterPontosRota(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            (Dominio.Entidades.Localidade localidadeOrigem, string cepOrigem) = ObterLocalidade(pontosRota.FirstOrDefault());
            (Dominio.Entidades.Localidade localidadeDestino, string cepDestino) = ObterLocalidade(pontosRota.LastOrDefault());

            if (localidadeOrigem == null)
                throw new ServicoException("Não foi possível definir a localidade origem da rota.");

            if (localidadeDestino == null)
                throw new ServicoException("Não foi possível definir a localidade destino da rota.");

            List<fieldTO> campos = new List<fieldTO>();

            campos.Add(new fieldTO() { key = "viagem.origem.cidade.ibge", value = localidadeOrigem.CodigoIBGE.ToString() });
            campos.Add(new fieldTO() { key = "viagem.destino.cidade.ibge", value = localidadeDestino.CodigoIBGE.ToString() });

            if (_integracaoPamcard.EnviarCEPsNaIntegracao)
            {
                campos.Add(new fieldTO() { key = "viagem.origem.cidade.cep", value = cepOrigem });
                campos.Add(new fieldTO() { key = "viagem.destino.cidade.cep", value = cepDestino });
            }

            int pontosPassagem = 0;

            for (int i = 0; i < pontosRota.Count; i++)
            {
                if (i > 0 && i < pontosRota.Count - 1) // Não envia o primeiro e o ultimo ponto                
                {
                    (Dominio.Entidades.Localidade localidade, string cepLocalidade) = ObterLocalidade(pontosRota[i]);
                    (Dominio.Entidades.Localidade localidadeAnterior, string _) = ObterLocalidade(pontosRota[i - 1]);

                    if (localidade != null && (localidadeAnterior == null || localidade.Codigo != localidadeAnterior.Codigo) && localidade.Codigo != localidadeOrigem.Codigo && localidade.Codigo != localidadeDestino.Codigo)
                    {
                        string pontoId = $"viagem.ponto{pontosPassagem + 1}";

                        campos.Add(new fieldTO() { key = pontoId + ".cidade.ibge", value = localidade.CodigoIBGE.ToString() });

                        if (_integracaoPamcard.EnviarCEPsNaIntegracao)
                            campos.Add(new fieldTO() { key = pontoId + ".cidade.cep", value = cepLocalidade });

                        pontosPassagem++;
                    }
                }
            }

            if (pontosPassagem > 0)
                campos.Add(new fieldTO() { key = "viagem.ponto.qtde", value = pontosPassagem.ToString() });

            return campos;
        }

        private ServicoPamCard.fieldTO[] ObterCamposCancelamento(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            string cnpjContratante = ObterContratante(cargaIntegracaoValePedagio.Carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante });
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });

                if (cargaIntegracaoValePedagio.Carga.Filial is null)
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = cargaIntegracaoValePedagio.Carga.Empresa.CNPJ });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = cargaIntegracaoValePedagio.IdCompraValePedagio });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.cancelamento.motivo", value = "CANCELAMENTO GERADO PELO OPERADOR" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.antt.ciot.numero", value = cargaIntegracaoValePedagio.NumeroValePedagio });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposPayToll(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string IdCompraValePedagio)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga _repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = _repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                string cnpjContratante = ObterContratante(cargaIntegracaoValePedagio.Carga);

                ServicoPamCard.fieldTO numeroContratanteDocumento = new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante };
                ServicoPamCard.fieldTO numeroViagemUnidadeDocumento = new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.numero", value = cargaIntegracaoValePedagio.Carga.Filial?.CNPJ ?? cargaIntegracaoValePedagio.Carga.Empresa.CNPJ };

                campos.Add(numeroContratanteDocumento);
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.unidade.documento.tipo", value = "1" });

                if (configuracaoGeralCarga?.ValidarContratanteOrigemVPIntegracaoPamcard ?? false)
                {
                    bool ehDiferente = numeroContratanteDocumento.value != numeroViagemUnidadeDocumento.value;

                    if (ehDiferente && cargaIntegracaoValePedagio.Carga.Filial is null)
                        campos.Add(numeroViagemUnidadeDocumento);
                }
                else
                    campos.Add(numeroViagemUnidadeDocumento);
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = IdCompraValePedagio });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposConsultaValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            string cnpjContratante = ObterContratante(carga);
            (string categoriaVeiculo, string categoriaVeiculoEixoSuspenso) = ObterCategoriaVeiculo(carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = categoriaVeiculo });

            if (!string.IsNullOrWhiteSpace(categoriaVeiculoEixoSuspenso))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria.eixo.suspenso", value = categoriaVeiculoEixoSuspenso });

            campos.AddRange(ObterPontosRota(carga));

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.praca", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.rota", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.uf", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.postos", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.idavolta", value = "N" });

            return campos.ToArray();
        }

        private ServicoPamCard.fieldTO[] ObterCamposImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaValePedagio.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            (Dominio.Entidades.Localidade localidadeOrigem, string cepOrigem) = ObterLocalidade(pontosRota.FirstOrDefault());
            (Dominio.Entidades.Localidade localidadeDestino, string cepDestino) = ObterLocalidade(pontosRota.LastOrDefault());

            if (localidadeOrigem == null)
                throw new ServicoException("Não foi possível definir a localidade origem da rota.");

            if (localidadeDestino == null)
                throw new ServicoException("Não foi possível definir a localidade destino da rota.");

            if (String.IsNullOrWhiteSpace(cargaValePedagio.IdCompraValePedagio))
                throw new ServicoException("Pedágio não possui número de identificação!");

            string tipoVeiculo = cargaValePedagio.Carga.Veiculo?.ModeloVeicularCarga?.TipoVeiculoPamcard;
            if (string.IsNullOrWhiteSpace(tipoVeiculo))
                tipoVeiculo = "4";

            string cnpjContratante = ObterContratante(cargaValePedagio.Carga);

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = cnpjContratante });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id", value = cargaValePedagio.IdCompraValePedagio });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.praca", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.pedagio.obter.rota", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.favorecido", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.documento", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.valores", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.veiculo", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.quitacao", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.uf", value = "S" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.obter.postos", value = "S" });

            return campos.ToArray();
        }

        private void ObterCamposFavorecidoPF(List<ServicoPamCard.fieldTO> campos, Dominio.Entidades.Usuario motorista)
        {
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento1.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento1.numero", value = motorista.CPF });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.nome", value = motorista.Nome.Left(40) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.logradouro", value = motorista.Endereco.Left(40) });

            string enderecoNumero = string.IsNullOrWhiteSpace(motorista.NumeroEndereco.ObterSomenteNumeros()) ? "0" : motorista.NumeroEndereco.ObterSomenteNumeros();
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.numero", value = enderecoNumero });

            if (!string.IsNullOrWhiteSpace(motorista.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.complemento", value = motorista.Complemento.Left(15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.bairro", value = motorista.Bairro.Left(30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(motorista.CEP) });

            ObterCamposTelefone(campos, motorista.Telefone);

            if (!string.IsNullOrWhiteSpace(motorista.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.email", value = motorista.Email.Split(';').FirstOrDefault() });
        }

        private void ObterCamposFavorecidoPJ(List<ServicoPamCard.fieldTO> campos, Dominio.Entidades.Empresa empresa, bool possuiCartao)
        {
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento1.tipo", value = empresa.Tipo == "F" ? "2" : "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento1.numero", value = empresa.CNPJ });

            //campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.documento2.tipo", value = "6" });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.nome", value = empresa.RazaoSocial.Left(40) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.logradouro", value = empresa.Endereco.Left(40) });

            string enderecoNumero = string.IsNullOrWhiteSpace(empresa.Numero.ObterSomenteNumeros()) ? "0" : empresa.Numero.ObterSomenteNumeros();
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.numero", value = enderecoNumero });

            if (!string.IsNullOrWhiteSpace(empresa.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.complemento", value = empresa.Complemento.Left(15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.bairro", value = empresa.Bairro.Left(30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", empresa.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(empresa.CEP) });

            if (!string.IsNullOrWhiteSpace(empresa.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.email", value = empresa.Email.Split(';').FirstOrDefault() });

            if (possuiCartao)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.empresa.Nome", value = empresa.RazaoSocial.Left(40) });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.empresa.cnpj", value = empresa.CNPJ });
            }
        }

        private void ObterCamposFavorecidoTerceiro(List<fieldTO> campos, Dominio.Entidades.Cliente proprietario)
        {
            if (proprietario.Tipo == "F")
            {
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.documento1.tipo", value = "2" });
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.documento1.numero", value = proprietario.CPF_CNPJ_SemFormato });

                //campos.Add(new fieldTO() { key = "viagem.favorecido1.documento3.tipo", value = "5" });

                ObterCamposTelefone(campos, proprietario.Telefone1);
            }
            else if (proprietario.Tipo == "J")
            {
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.documento1.tipo", value = "1" });
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.documento1.numero", value = proprietario.CPF_CNPJ_SemFormato });

                //campos.Add(new fieldTO() { key = $"viagem.favorecido1.documento2.tipo", value = "6" });
            }

            campos.Add(new fieldTO() { key = $"viagem.favorecido1.nome", value = proprietario.Nome.Left(40) });
            campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.logradouro", value = proprietario.Endereco.Left(40) });

            string enderecoNumero = string.IsNullOrWhiteSpace(proprietario.Numero.ObterSomenteNumeros()) ? "0" : proprietario.Numero.ObterSomenteNumeros();
            campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.numero", value = enderecoNumero });

            if (!string.IsNullOrWhiteSpace(proprietario.Complemento))
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.complemento", value = proprietario.Complemento.Left(15) });

            campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.bairro", value = proprietario.Bairro.Left(30) });
            campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", proprietario.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new fieldTO() { key = $"viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(proprietario.CEP) });

            if (!string.IsNullOrWhiteSpace(proprietario.Email))
                campos.Add(new fieldTO() { key = $"viagem.favorecido1.email", value = proprietario.Email.Split(';').FirstOrDefault() });
        }

        private void ObterCamposTelefone(List<ServicoPamCard.fieldTO> campos, string telefone)
        {
            if (!string.IsNullOrWhiteSpace(telefone))
            {
                string telefoneCompleto = Utilidades.String.OnlyNumbers(telefone);
                string ddd = string.Empty;
                string numeroTelefone = string.Empty;

                if (telefoneCompleto.StartsWith("0"))
                {
                    ddd = telefoneCompleto.Substring(1, 2);
                    numeroTelefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                }
                else
                {
                    ddd = telefoneCompleto.Substring(0, 2);
                    numeroTelefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                }

                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.telefone.ddd", value = "0" + ddd });
                campos.Add(new ServicoPamCard.fieldTO() { key = $"viagem.favorecido1.telefone.numero", value = numeroTelefone });
            }
        }

        private ServicoPamCard.WSTransacionalClient ObterClientPamCard(Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior)
        {
            ServicoPamCard.WSTransacionalClient svcPamCard = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoPamCard.WSTransacionalClient, ServicoPamCard.WSTransacional>(TipoWebServiceIntegracao.Pamcard_WSTransacional, out inspectorBehavior);
            string url = svcPamCard.Endpoint.Address.ToString();
            svcPamCard = null;

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            binding.SendTimeout = new TimeSpan(0, 2, 0);

            EndpointAddress endpointAddress = new EndpointAddress(url);
            svcPamCard = new ServicoPamCard.WSTransacionalClient(binding, endpointAddress);

            svcPamCard.Endpoint.EndpointBehaviors.Add(inspectorBehavior);

            if (filial != null)
            {
                if (string.IsNullOrEmpty(filial.NomeCertificado) || string.IsNullOrEmpty(filial.SenhaCertificado))
                    throw new ServicoException("Certificado digital da filial não configurado.");

                svcPamCard.ClientCredentials.ClientCertificate.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(filial.NomeCertificado, filial.SenhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);
            }
            else
            {
                if (string.IsNullOrEmpty(empresa.NomeCertificado) || string.IsNullOrEmpty(empresa.SenhaCertificado))
                    throw new ServicoException("Certificado digital da empresa não configurado.");

                svcPamCard.ClientCredentials.ClientCertificate.Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(empresa.NomeCertificado, empresa.SenhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);
            }

            return svcPamCard;
        }

        #endregion Obter Campos

        private void SalvarXMLIntegracao(ref Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Servicos.Models.Integracao.InspectorBehavior inspector, string mensagemRetorno)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = mensagemRetorno;
            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
        }

        private void SalvarDadosRetornoCompraValePedagio(Dictionary<string, string> responseInsertTrip, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repositorioValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioValePedagioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(_unitOfWork);

            string viagemID = responseInsertTrip.GetValue<string>("viagem.id");
            string valorRetornoPedagio = responseInsertTrip.GetValue<string>("viagem.pedagio.valor");

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
            {
                Carga = carga,
                CodigoFilialCliente = string.Empty,
                CodigoProcessoCliente = string.Empty,
                CodigoViagem = viagemID.ToInt(),
                DataEmissao = DateTime.Now,
                ValorTotalPedagios = valorRetornoPedagio.ToDecimal()
            };

            repositorioValePedagioDadosCompra.Inserir(dadosCompra);

            string quantidadeDePracas = responseInsertTrip.GetValue<string>("viagem.pedagio.qtde");

            int numeroPracas = quantidadeDePracas.ToInt();

            for (int i = 0; i < numeroPracas; i++)
            {
                string nomePraca = responseInsertTrip.GetValue<string>($"viagem.pedagio.praca{i}.nome");
                string valorPedagio = responseInsertTrip.GetValue<string>($"viagem.pedagio.praca{i}.valor");

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca praca = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca
                {
                    CargaValePedagioDadosCompra = dadosCompra,
                    CodigoPraca = 0,
                    ConcessionariaCodigo = 0,
                    ConcessionariaNome = string.Empty,
                    NomePraca = nomePraca,
                    NumeroEixos = 0,
                    Valor = valorPedagio.ToDecimal()
                };

                repositorioValePedagioDadosCompraPraca.Inserir(praca);
            }
        }

        private bool RotaFreteComVolta(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Empresa != null && carga.Rota != null)
            {
                Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio repTransportadorRotaFreteValePedagio = new Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio(_unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio rotaFreteValePedagio = repTransportadorRotaFreteValePedagio.BuscarPorEmpresaERotaFrete(carga.Empresa.Codigo, carga.Rota.Codigo);

                if (rotaFreteValePedagio != null)
                    return rotaFreteValePedagio.TipoRotaFrete == TipoRotaFrete.IdaVolta;
            }

            TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);
            TipoUltimoPontoRoteirizacao tipoUltimoPonto = ultimoPontoPorTipoOperacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;
            TipoRotaFrete tipoRotaFrete = tipoUltimoPonto == TipoUltimoPontoRoteirizacao.AteOrigem ? TipoRotaFrete.IdaVolta : TipoRotaFrete.Ida;

            return tipoRotaFrete == TipoRotaFrete.IdaVolta;
        }

        private byte[] GerarRelatorioReciboValePedagio(fieldTO[] retorno)
        {
            return ReportRequest.WithType(ReportType.PamcardReciboVP)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("retorno", retorno.ToJson())
                .CallReport()
                .GetContentFile();
        }

        public (Dominio.Entidades.Localidade Localidade, string CEP) ObterLocalidade(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoRota)
        {
            if (pontoRota == null)
                return (null, null);

            if (pontoRota.ClienteOutroEndereco != null)
                return ValueTuple.Create(pontoRota.ClienteOutroEndereco.Localidade, pontoRota.ClienteOutroEndereco.CEP);

            if (pontoRota.Cliente?.Localidade != null)
                return ValueTuple.Create(pontoRota.Cliente.Localidade, pontoRota.Cliente.CEP);

            if (pontoRota.Localidade != null)
                return ValueTuple.Create(pontoRota.Localidade, pontoRota.Localidade.CEP);

            return (null, null);
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool considerarFilialMatriz)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

            if (considerarFilialMatriz)
                filial = repositorioFilial.BuscarMatriz();

            if (filial == null)
                filial = carga.Filial;

            return filial;
        }

        private void InformarCargaIntegrandoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            carga.PossuiPendencia = false;
            carga.ProblemaIntegracaoValePedagio = false;
            carga.IntegrandoValePedagio = true;
            carga.MotivoPendencia = "";
            repositorioCarga.Atualizar(carga);
        }

        private Dictionary<string, string> ObterDicionario(ServicoPamCard.fieldTO[] retornoIntegracao)
        {
            Dictionary<string, string> responseIntegracao = retornoIntegracao.ToDictionary(retorno => retorno.key, retorno => retorno.value);

            return responseIntegracao;
        }

        private string ObterContratante(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(carga, considerarFilialMatriz: true);

            string cnpjContratante = filial?.CNPJ ?? carga.Empresa.CNPJ;

            return cnpjContratante;
        }

        private void ObterIntegracaoPamcard(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (_integracaoPamcard != null)
                return;

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _integracaoPamcard = servicoValePedagio.ObterIntegracaoPamcard(carga, tipoServicoMultisoftware);
        }

        private (string categoriaVeiculo, string categoriaVeiculoEixoSuspenso) ObterCategoriaVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string categoriaVeiculo = string.Empty;
            string categoriaVeiculoEixoSuspenso = string.Empty;

            if (!_integracaoPamcard.SomarEixosSuspensosValePedagio)
            {
                categoriaVeiculo = carga.Veiculo?.ModeloVeicularCarga?.TipoVeiculoPamcard ?? carga.ModeloVeicularCarga?.TipoVeiculoPamcard;

                if (string.IsNullOrWhiteSpace(categoriaVeiculo))
                    categoriaVeiculo = "4";

                return (categoriaVeiculo, categoriaVeiculoEixoSuspenso);
            }

            (int numeroEixos, int numeroEixosSuspensos) = ObterNumeroEixos(carga);

            categoriaVeiculo = ObterCategoriaVeiculoPorNumeroEixos(numeroEixos);

            (bool retornoCargaVazio, bool rotaComVolta) = ObterRotaComVolta(carga);

            if (rotaComVolta && retornoCargaVazio && numeroEixosSuspensos > 0)
                categoriaVeiculoEixoSuspenso = ObterCategoriaVeiculoPorNumeroEixos(numeroEixosSuspensos);

            return (categoriaVeiculo, categoriaVeiculoEixoSuspenso);
        }

        private (int numeroEixos, int numeroEixosSuspensos) ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int numeroEixos = 0;
            int numeroEixosSuspensos = 0;

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = carga.Veiculo != null ? carga.Veiculo.ModeloVeicularCarga : carga.ModeloVeicularCarga;

            if (modeloVeicular != null)
            {
                numeroEixos = modeloVeicular.NumeroEixos ?? 0;
                numeroEixosSuspensos = modeloVeicular.NumeroEixosSuspensos ?? 0;

                if (carga.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                    {
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;
                        numeroEixosSuspensos += reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return (numeroEixos, numeroEixosSuspensos);
        }

        private string ObterCategoriaVeiculoPorNumeroEixos(int numeroEixos)
        {
            return numeroEixos switch
            {
                2 => "2",  // Rodado duplo como padrão (caminhão leve, trator etc.)
                3 => "4",  // Caminhão-trator c/ semi-reboque
                4 => "6",  // Caminhão e/ou trator c/ semi-reboque
                5 => "7",  // Caminhão c/ reboque
                6 => "8",  // Caminhão-trator c/ semi-reboque
                7 => "10", // Caminhão-trator c/ semi-reboque
                8 => "11",
                9 => "12",
                10 => "13",
                _ => "4"
            };
        }

        private (bool retornoCargaVazio, bool rotaComVolta) ObterRotaComVolta(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (_integracaoPamcard.ConsiderarRotaFreteDaCargaNoValePedagio)
            {
                bool idaVolta = carga.Rota?.TipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                bool retornoVazio = idaVolta && carga.Rota?.TipoCarregamentoVolta == RetornoCargaTipo.Vazio;

                return (retornoVazio, idaVolta);
            }

            bool retornoOperacaoVazio = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;
            bool idaVoltaOperacao = RotaFreteComVolta(carga);

            return (retornoOperacaoVazio, idaVoltaOperacao);
        }


        #endregion Métodos Privados
    }
}
