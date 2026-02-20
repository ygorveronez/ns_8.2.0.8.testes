using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar;
using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos
{
    public class SemParar : ServicoBase
    {        
        public SemParar(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool ComprarValePedagioMDFe(ref Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Credencial credencial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
            Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
            Repositorio.ReboqueMDFe repReboquesMDFe = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
            valePedagioMDFeCompra.TentativaReenvio++;

            try
            {

                SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                int quantidadeEixosConjunto = 0;
                Dominio.Entidades.Veiculo veiculoTracao = null;
                Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);

                if (veiculoMDFe == null)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = "Veículo do MDFe não informado.";

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                veiculoTracao = repVeiculo.BuscarPorPlaca(valePedagioMDFeCompra.MDFe.Empresa.Codigo, veiculoMDFe.Placa);
                if (veiculoTracao == null)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = "Veículo " + veiculoTracao.Placa + " não cadastrado.";

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                if (veiculoTracao.TipoDoVeiculo == null)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = "Veículo " + veiculoTracao.Placa + " sem tipo configurado.";

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                quantidadeEixosConjunto = veiculoTracao.TipoDoVeiculo.NumeroEixos;

                Dominio.Entidades.Veiculo veiculoReboque = null;
                List<Dominio.Entidades.ReboqueMDFe> reboquesMDFe = repReboquesMDFe.BuscarPorMDFe(valePedagioMDFeCompra.MDFe.Codigo);

                if (reboquesMDFe != null && reboquesMDFe.Count > 0)
                {
                    for (int i = 0; i < reboquesMDFe.Count; i++)
                    {
                        veiculoReboque = repVeiculo.BuscarPorPlaca(valePedagioMDFeCompra.MDFe.Empresa.Codigo, reboquesMDFe[i].Placa);
                        if (veiculoReboque == null)
                        {
                            valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                            valePedagioMDFeCompra.Mensagem = "Veículo Reboque " + veiculoReboque.Placa + " não cadastrado.";
                            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                            return false;
                        }

                        if (veiculoReboque.TipoDoVeiculo == null)
                        {
                            valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                            valePedagioMDFeCompra.Mensagem = "Veículo Reboque " + veiculoReboque.Placa + " sem tipo configurado.";
                            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                            return false;
                        }

                        quantidadeEixosConjunto += veiculoReboque.TipoDoVeiculo.NumeroEixos;
                    }
                }

                valePedagioMDFeCompra.QuantidadeEixos = quantidadeEixosConjunto;

                string rota = valePedagioMDFeCompra.DescricaoRota;
                SemPararValePedagio.Viagem viagem = null;
                ConsultarTransportador(credencial, valePedagioMDFeCompra, veiculoTracao, unidadeDeTrabalho);

                viagem = valePedagioClient.comprarViagem(rota, veiculoTracao.Placa ?? "", valePedagioMDFeCompra.QuantidadeEixos, DateTime.Now, DateTime.Now.AddDays(8), "", "", "", credencial.Sessao);

                SalvarXMLIntegracao(inspector.LastRequestXML, inspector.LastResponseXML, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unidadeDeTrabalho);

                if (viagem.status != 0)
                {
                    valePedagioMDFeCompra.Mensagem = ObterMensagemRetorno(viagem.status);
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso;
                valePedagioMDFeCompra.NumeroComprovante = viagem.numero.ToString();
                valePedagioMDFeCompra.Mensagem = "Ag Recibo";
                valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSemParar");

                valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = "9999 - " + (ex.Message.Length > 900 ? ex.Message.Substring(0, 900) : ex.Message);

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }
        }

        public bool ConsultarIdVpo(Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Credencial credencial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                jsonRequisicao = $"{valePedagioMDFeCompra.UrlIntegracaoRest}/v1/integracao-antt/vpo/numero-viagem/{valePedagioMDFeCompra.NumeroComprovante}";
                System.Net.Http.HttpClient requisicao = CriarRequisicao(valePedagioMDFeCompra.UrlIntegracaoRest, credencial);

                System.Net.Http.HttpResponseMessage retornoRequisicao = requisicao.GetAsync(jsonRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    RetornoErroValePedagio retornoErro = JsonConvert.DeserializeObject<RetornoErroValePedagio>(jsonRetorno);

                    throw new ServicoException(retornoErro.Erro?.Descricao ?? $"Erro ao consultar ID VPO. StatusCode: {(int)retornoRequisicao.StatusCode}");
                }

                if (!retornoRequisicao.IsSuccessStatusCode || retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Erro ao consultar ID VPO. StatusCode: {(int)retornoRequisicao.StatusCode}");

                RetornoConsultaIdVpo idsVpos = JsonConvert.DeserializeObject<RetornoConsultaIdVpo>(jsonRetorno);

                if (idsVpos.Dados?.CodigosVpos?.Count <= 0)
                    throw new ServicoException("Falha na consulta do ID VPO");

                valePedagioMDFeCompra.Mensagem = "Consulta do ID VPO realizada com sucesso!";
                valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT = idsVpos.Dados.CodigosVpos?.FirstOrDefault() ?? string.Empty;
                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso;
                valePedagioMDFeCompra.DataIntegracao = DateTime.Now;
                valePedagioMDFeCompra.TentativaReenvio++;

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                SalvarXMLIntegracao(ConverterParaXmlSeJson(jsonRequisicao, "Request"), ConverterParaXmlSeJson(jsonRetorno, "Response"), valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unitOfWork);

                return true;
            }
            catch (ServicoException excecao)
            {

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = excecao.Message;

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                SalvarXMLIntegracao(ConverterParaXmlSeJson(jsonRequisicao, "Request"), ConverterParaXmlSeJson(jsonRetorno, "Response"), valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unitOfWork);

                return false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSemParar");

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = "9999 - " + (excecao.Message.Length > 900 ? excecao.Message.Substring(0, 900) : excecao.Message);

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }
        }

        public bool CancelarCompraValePedagioMDFe(ref Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = Autenticar(valePedagioMDFeCompra, unidadeDeTrabalho);

                if (!credencial.Autenticado)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                    valePedagioMDFeCompra.Mensagem = credencial.Retorno;

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                int status = valePedagioClient.cancelarViagem(long.Parse(valePedagioMDFeCompra.NumeroComprovante), credencial.Sessao);

                SalvarXMLIntegracao(inspector.LastRequestXML, inspector.LastResponseXML, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.CancelarCompraValePedagio, unidadeDeTrabalho);

                if (status != 0)
                {
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                    valePedagioMDFeCompra.Mensagem = "Não foi possúvel efetuar o cancelamento";

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    return false;
                }

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Cancelado;
                valePedagioMDFeCompra.Mensagem = "Vale Pedágio Cancelado com Sucesso";

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSemParar");

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                valePedagioMDFeCompra.Mensagem = "9999 - " + (ex.Message.Length > 900 ? ex.Message.Substring(0, 900) : ex.Message);

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }

        }

        public bool CancelarCompraValePedagioMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);

            List<Dominio.Entidades.ValePedagioMDFeCompra> listaValePedagioMDFeCompra = repValePedagioMDFeCompra.BuscarPorMDFeTipoStatus(mdfe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao, Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso);

            bool retornoCancelmento = false;

            if (listaValePedagioMDFeCompra != null && listaValePedagioMDFeCompra.Count > 0)
            {
                foreach (Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra in listaValePedagioMDFeCompra)
                {
                    Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCancelamentoCompra = new Dominio.Entidades.ValePedagioMDFeCompra();

                    try
                    {
                        if (valePedagioMDFeCompra.Integradora != Dominio.Enumeradores.IntegradoraValePedagio.SemParar)
                        {
                            break;
                        }

                        valePedagioMDFeCancelamentoCompra.MDFe = mdfe;
                        valePedagioMDFeCancelamentoCompra.NumeroComprovante = valePedagioMDFeCompra.NumeroComprovante;
                        valePedagioMDFeCancelamentoCompra.CodigoEmissaoValePedagioANTT = valePedagioMDFeCompra.CodigoEmissaoValePedagioANTT;
                        valePedagioMDFeCancelamentoCompra.CNPJFornecedor = valePedagioMDFeCompra.CNPJFornecedor;
                        valePedagioMDFeCancelamentoCompra.CNPJResponsavel = valePedagioMDFeCompra.CNPJResponsavel;
                        valePedagioMDFeCancelamentoCompra.IBGEInicio = valePedagioMDFeCompra.IBGEInicio;
                        valePedagioMDFeCancelamentoCompra.IBGEFim = valePedagioMDFeCompra.IBGEFim;
                        valePedagioMDFeCancelamentoCompra.Integradora = valePedagioMDFeCompra.Integradora;
                        valePedagioMDFeCancelamentoCompra.Tipo = Dominio.Enumeradores.TipoIntegracaoValePedagio.Cancelamento;
                        valePedagioMDFeCancelamentoCompra.IntegracaoUsuario = valePedagioMDFeCompra.IntegracaoUsuario;
                        valePedagioMDFeCancelamentoCompra.IntegracaoSenha = valePedagioMDFeCompra.IntegracaoSenha;
                        valePedagioMDFeCancelamentoCompra.IntegracaoToken = valePedagioMDFeCompra.IntegracaoToken;
                        valePedagioMDFeCancelamentoCompra.TipoCompra = valePedagioMDFeCompra.TipoCompra;
                        valePedagioMDFeCancelamentoCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente;


                        repValePedagioMDFeCompra.Inserir(valePedagioMDFeCancelamentoCompra);

                        retornoCancelmento = this.CancelarCompraValePedagioMDFe(ref valePedagioMDFeCancelamentoCompra, unidadeDeTrabalho);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "IntegracaoSemParar");

                        valePedagioMDFeCancelamentoCompra.MDFe = mdfe;
                        valePedagioMDFeCancelamentoCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento;
                        valePedagioMDFeCompra.Mensagem = "9999 - " + (ex.Message.Length > 900 ? ex.Message.Substring(0, 900) : ex.Message);

                        repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCancelamentoCompra);

                        retornoCancelmento = false;
                    }
                }

                return retornoCancelmento;
            }

            return true;
        }

        public bool ObterReciboCompraValePedagio(ref Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Credencial credencial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                SemPararValePedagio.Recibo recibo = ObterReciboViagem(valePedagioMDFeCompra.NumeroComprovante, credencial, out jsonRequisicao, out jsonRetorno, unitOfWork);

                if (recibo.status == 2 || recibo.status == 0)
                {
                    valePedagioMDFeCompra.Valor = recibo.total ?? 0;
                    valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.Sucesso;
                    valePedagioMDFeCompra.Mensagem = "1 - Recibo obtido com Sucesso";

                    repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                    SalvarXMLIntegracao(jsonRequisicao, jsonRetorno, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unitOfWork);

                    return true;
                }

                if (recibo.status == 7 || recibo.status == 15)
                {
                    string mensagem = "";

                    if (recibo.status == 15)
                        mensagem = $"15 - {ObterMensagemRetorno(recibo.status)}";
                    else
                        mensagem = "7 - Compra recusada";

                    throw new ServicoException(mensagem);
                }

                throw new ServicoException("Ag retorno sem parar");
            }
            catch (ServicoException excecao)
            {
                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = excecao.Message;

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                SalvarXMLIntegracao(jsonRequisicao, jsonRetorno, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unitOfWork);

                return false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSemParar");

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = "9999 - " + (excecao.Message.Length > 900 ? excecao.Message.Substring(0, 900) : excecao.Message);

                repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);

                return false;
            }
        }

        public ReportResult GerarImpressaoValePedagio(int codigoValePedagioMDFe)
        {
            return ReportRequest.WithType(ReportType.ValePedagioSempararMDFe)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoValePedagioMDFe", codigoValePedagioMDFe)
                .CallReport();
        }

        public SemPararValePedagio.Recibo ObterReciboViagem(string numeroValePedagio, Credencial credencial, out string request, out string response, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            SemPararValePedagio.Recibo recibo = valePedagioClient.obterReciboViagem(!string.IsNullOrWhiteSpace(numeroValePedagio) ? long.Parse(numeroValePedagio) : 0, credencial.Sessao);

            request = inspector.LastRequestXML;
            response = inspector.LastResponseXML;

            return recibo;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string cnpj = !string.IsNullOrWhiteSpace(valePedagioMDFeCompra?.CNPJResponsavel) ? valePedagioMDFeCompra.CNPJResponsavel : valePedagioMDFeCompra?.CNPJFornecedor;
            SemPararValePedagio.Identificador identificador = valePedagioClient.autenticarUsuario(cnpj, valePedagioMDFeCompra.IntegracaoUsuario, valePedagioMDFeCompra.IntegracaoSenha);

            if (identificador.status == 0)
            {
                credencial.Autenticado = true;
                credencial.Retorno = "Autenticado com sucesso";
                credencial.Sessao = identificador.sessao;
            }
            else
            {
                credencial.Autenticado = false;
                credencial.Retorno = ObterMensagemRetorno(identificador.status);

            }

            return credencial;
        }

        private void ConsultarTransportador(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {

                dynamic requisicaoConsultarTransportador = new
                {
                    placa = veiculo.Placa,
                    cpfCnpjTransportador = (veiculo.Tipo == "T" && veiculo.Proprietario != null) ? veiculo.Proprietario.CPF_CNPJ_SemFormato : valePedagioMDFeCompra.MDFe.Empresa?.CNPJ_SemFormato
                };

                jsonRequisicao = JsonConvert.SerializeObject(requisicaoConsultarTransportador, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                System.Net.Http.StringContent conteudoRequisicao = new System.Net.Http.StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                string urlBase = $"{valePedagioMDFeCompra.UrlIntegracaoRest}/v1/integracao-rntrc-embarcador";
                System.Net.Http.HttpClient requisicao = CriarRequisicao(urlBase, credencial);

                System.Net.Http.HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlBase, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                SalvarXMLIntegracao(jsonRequisicao, jsonRetorno, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.ComprarValePedagio, unitOfWork);
            }
            catch (ServicoException excecao)
            {
                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoSemParar");

                valePedagioMDFeCompra.Status = Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra;
                valePedagioMDFeCompra.Mensagem = "Ocorreu uma falha ao realizar a consulta de rntrc-embarcador do vale pedágio";
            }

            repValePedagioMDFeCompra.Atualizar(valePedagioMDFeCompra);
        }

        public string ObterMensagemRetorno(int status)
        {
            string mensagem = "";

            if (status == 1)
                mensagem = "CNPJ, login ou senha inválidos";
            else if (status == 3)
                mensagem = "Sessão expirada ou inválida (Timeout da sessão ultrapassado ou código de sessão inválido)";
            else if (status == 4)
                mensagem = "Veículo não disponível (Veículo não encontrado no sistema ou com restrições)";
            else if (status == 5)
                mensagem = "Placa inválida";
            else if (status == 7)
                mensagem = "Veículo com múltiplos Tags (Veículo possui mais de um Tag ativo)";
            else if (status == 8)
                mensagem = "Viagem não encontrada (Numero de Vale Pedágio não encontrado no sistema)";
            else if (status == 9)
                mensagem = "Usuário sem permissão a este serviço (Usuário precisa estar cadastrado para acessar a funcionalidade)";
            else if (status == 10)
                mensagem = "Prazo inválido (Data vazia ou valor de início maior que final)";
            else if (status == 11)
                mensagem = "Prazo máximo extrapolado (Prazo máximo para extratos é de 90 dias.Para vigência de ValePedágio, 15 dias.)";
            else if (status == 12)
                mensagem = "Rota Inválida (Rota não encontrada no sistema para este embarcador)";
            else if (status == 13)
                mensagem = "Número de eixos inválido (Numero deve ser entre 2 a 10.E nos serviços de obter valores praças deve ser entre 2 a 15.)";
            else if (status == 14)
                mensagem = "Saldo insuficiente (Crédito disponível na conta menor que o valor da viagem a ser comprada.)";
            else if (status == 15)
                mensagem = "Recibo não disponível (O recibo do vale pedágio só é retornado caso a viagem não tenha sido cancelada ou encerrada.)";
            else if (status == 49)
                mensagem = "Viagem não pode ser cancelada (Viagem confirmada (impressa) a mais de 3 hrs ou com alguma passagem já reconhecida não pode mais ser cancelada.)";
            else if (status == 51)
                mensagem = "Praça(s) inválida(s) (Para reemissão (transferência) de viagem. Lista de praças deve ter o formato 99 - 99 - 99 -...)";
            else if (status == 52)
                mensagem = "Não foi encontrado o transportador (Transportador não foi encontrado no sistema)";
            else if (status == 53)
                mensagem = "Viagem não pode ser reemitida (Viagem expirada ou viagem que ainda não tenha sido confirmada(impressa) não pode ser reemitida.)";
            else if (status == 54)
                mensagem = "Viagem parcialmente reemitida (Houve alguma(s) praça(s) de pedágio selecionada(s) que não pode(puderam) ser reemitida(s))";
            else if (status == 55)
                mensagem = "Viagem não pode ser nula (Valor da viagem deve ser maior que R$ 0,00)";
            else if (status == 58)
                mensagem = "Nome de rota já existente (Usuário tentou cadastrar uma rota com um nome já existente)";
            else if (status == 59)
                mensagem = "Rota inexistente (Usuário tentou pesquisar uma rota que não existe)";
            else if (status == 62)
                mensagem = "Mais de um resultado encontrado... (Era esperado no máximo um valor de retorno, mas foi encontrado mais de um.)";
            else if (status == 66)
                mensagem = "Praça não encontrada (Usuário tentou cadastrar uma rota e uma ou mais praças não foram encontradas ou não são atendidas.)";
            else if (status == 77)
                mensagem = "Tipo de rodagem indisponível (Tipos de rodagem existentes são: 0 – Simples 1 – Dupla)";
            else if (status == 79)
                mensagem = "Praça incompatível com a tecnologia do equipamento do veículo (Usuário tentou cadastrar uma ou mais praças cujo equipamento do veículo não é compatível com o da praça.)";
            else if (status == 83)
                mensagem = "Compra de viagem não processada (Usuário tentou efetuar a compra de uma viagem que por algum motivo não ocorreu (ausência / insuficiência de saldo, parâmetros inválidos, etc..))";
            else if (status == 84)
                mensagem = "Pontos de Parada Inválido, alguns dos pontos de paradas estão com código inválido. (Esse status será utilizado tanto com código do IBGE quanto lat / long)";
            else if (status == 85)
                mensagem = "Pontos não possui ligação (Utilizado quando algum dos pontos não possui ligação com os outros)";
            else if (status == 411)
                mensagem = "Preenchimento do parâmetro observacao1 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido)";
            else if (status == 412)
                mensagem = "Preenchimento do parâmetro observacao2 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 413)
                mensagem = "Preenchimento do parâmetro observacao3 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 414)
                mensagem = "Preenchimento do parâmetro observacao4 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 415)
                mensagem = "Preenchimento do parâmetro observacao5 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 416)
                mensagem = "Preenchimento do parâmetro observacao6 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 421)
                mensagem = "Texto informado no parâmetro observacao1 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 422)
                mensagem = "Texto informado no parâmetro observacao2 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 423)
                mensagem = "Texto informado no parâmetro observacao3 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 424)
                mensagem = "Texto informado no parâmetro observacao4 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 425)
                mensagem = "Texto informado no parâmetro observacao5 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 426)
                mensagem = "Texto informado no parâmetro observacao6 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 431)
                mensagem = "Texto informado no parâmetro observacao1 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 432)
                mensagem = "Texto informado no parâmetro observacao2 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 433)
                mensagem = "Texto informado no parâmetro observacao3 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 434)
                mensagem = "Texto informado no parâmetro observacao4 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 435)
                mensagem = "Texto informado no parâmetro observacao5 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 436)
                mensagem = "Texto informado no parâmetro observacao6 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 451)
                mensagem = "Texto informado no parâmetro observacao1 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 452)
                mensagem = "Texto informado no parâmetro observacao2 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 453)
                mensagem = "Texto informado no parâmetro observacao3 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 454)
                mensagem = "Texto informado no parâmetro observacao4 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 455)
                mensagem = "Texto informado no parâmetro observacao5 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 456)
                mensagem = "Texto informado no parâmetro observacao6 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 461)
                mensagem = "Parâmetro observacao1 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 462)
                mensagem = "Parâmetro observacao2 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 463)
                mensagem = "Parâmetro observacao3 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 464)
                mensagem = "Parâmetro observacao4 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 465)
                mensagem = "Parâmetro observacao5 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 466)
                mensagem = "Parâmetro observacao6 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 808)
                mensagem = "Cadastrar rotas sem praça (Quando o usuário tenta cadastrar uma rota que não tem praças de pedágio ou as mesmas não são atendidas.)";
            else if (status == 999)
                mensagem = "Erro no serviço (Erro diverso que não foi previsto) - Retorno do Sem Parar";
            else
                mensagem = "Não foi possível efetur a comprar no Sem Parar (Codigo de Retorno Sem Parar " + status.ToString() + ")";

            return mensagem.Length <= 400 ? mensagem : mensagem.Substring(0, 399);
        }

        private void SalvarXMLIntegracao(string lastRequestXML, string lastResponseXML, Dominio.Entidades.ValePedagioMDFeCompra valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ValePedagioMDFeCompraXML repValePedagioMDFeCompraXML = new Repositorio.ValePedagioMDFeCompraXML(unidadeDeTrabalho);

            Dominio.Entidades.ValePedagioMDFeCompraXML log = new Dominio.Entidades.ValePedagioMDFeCompraXML()
            {
                ValePedagioMDFeCompra = valePedagioMDFeCompra,
                Tipo = tipo,
                DataHora = DateTime.Now,
                Requisicao = lastRequestXML,
                Resposta = lastResponseXML
            };

            repValePedagioMDFeCompraXML.Inserir(log);
        }

        private string ConverterParaXmlSeJson(string conteudo, string rootName)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return conteudo;

            if (conteudo.StartsWith("<"))
                return conteudo;

            conteudo = conteudo.Trim();

            string arquivoJson = JsonConvert.SerializeObject(conteudo, Formatting.Indented);

            if (arquivoJson.StartsWith("{") || arquivoJson.StartsWith("["))
            {
                try
                {
                    var doc = JsonConvert.DeserializeXNode(arquivoJson, rootName);
                    return doc.ToString();
                }
                catch
                {
                    return $"<{rootName}><![CDATA[{arquivoJson}]]></{rootName}>";
                }
            }

            return $"<{rootName}><![CDATA[{arquivoJson}]]></{rootName}>";
        }

        private System.Net.Http.HttpClient CriarRequisicao(string urlBase, Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Net.Http.HttpClient requisicao = new System.Net.Http.HttpClient();

            requisicao.BaseAddress = new Uri(urlBase);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("sessao", credencial.Sessao.ToString());

            requisicao.Timeout = TimeSpan.FromMinutes(3);

            return requisicao;
        }
    }
}
