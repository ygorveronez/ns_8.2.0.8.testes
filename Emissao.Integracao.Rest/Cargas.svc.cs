using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.Serialization;
using Emissao.Integracao.Rest.Base;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Cargas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Cargas.svc or Cargas.svc.cs at the Solution Explorer and start debugging.
    public class Cargas : ICargas
    {
        public Retorno<int> IntegrarXMLCarga(XmlElement xmlCarga)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            string request = string.Empty;
            string identificador = "";

            try
            {
                //Servicos.Log.TratarErro("IntegrarXMLCarga: " + xmlCarga?.OuterXml ?? "");

                try
                {
                    request = xmlCarga?.OuterXml ?? "";
                    ConverterObjetoCargaIntegracao(request, ref identificador); //identificador = ObterNumeroCargaRequest(request);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    if (string.IsNullOrWhiteSpace(identificador))
                        identificador = ObterNumeroCarga(request);

                    var response = new Retorno<int>() { Mensagem = ex.Message, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, response.Mensagem, identificador, request, Utilidades.XML.Serializar(response, true), "xml", unitOfWork);

                    return response;
                }

                if (integradora == null)
                {
                    var resposta = new Retorno<int>() { Mensagem = "Acesso não permitido para o Token enviado.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, identificador, xmlCarga?.OuterXml, Utilidades.XML.Serializar(resposta, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                if (request == null)
                {
                    var resposta = new Retorno<int>() { Mensagem = "Nenhum dado enviado..", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, identificador, request, Utilidades.XML.Serializar(resposta, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                //Buscar Integração anterior e validar request
                if (ValidarIntegracaoAnterior(integradora.Codigo, request, identificador, unitOfWork))
                {
                    if (!string.IsNullOrWhiteSpace(identificador))
                    {
                        var carga = repCarga.BuscarPrimeiraCargaPorCodigoCargaEmbarcadorTodasSituacoes(identificador);
                        if (carga == null && identificador.Length <= 8) //0095513274
                        { 
                            identificador = string.Concat(new string('0', 10 - identificador.Length), identificador);
                            carga = repCarga.BuscarPrimeiraCargaPorCodigoCargaEmbarcadorTodasSituacoes(identificador);
                        }

                        if (carga != null && repCanhoto.ContarNaoPendentesPoPorCarga(carga.Codigo) > 0)
                        {
                            var respostaCanhoto = new Retorno<int>() { Mensagem = "Carga já possui processo de digitalização de canhoto iniciado, não permite atualização.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, respostaCanhoto.Mensagem, identificador, request, Utilidades.XML.Serializar(respostaCanhoto, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                            return respostaCanhoto;
                        }
                    }

                    var retorno = new Retorno<int>() { Mensagem = "Integração em processamento", Status = true, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, retorno.Mensagem, identificador, request, Utilidades.XML.Serializar(retorno, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

                    return retorno;
                }
                else
                {
                    var resposta = new Retorno<int>() { Mensagem = "Integração sem alteração em relação a integração anterior", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    //Alterado para não salvar quando o request é igual ao anterior feito com sucesso.
                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, identificador, request, Utilidades.XML.Serializar(resposta, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                var response = new Retorno<int>() { Mensagem = "Falha genérica ao integrar dados.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, response.Mensagem, identificador, request, Utilidades.XML.Serializar(response, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                return response;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Rest.DadosTransporteCarga dadosTransporte)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            try
            {
                //Servicos.Log.TratarErro("dadosTransporte " + (dadosTransporte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte) : string.Empty));

                Dominio.Entidades.Embarcador.Cargas.Carga carga = !string.IsNullOrWhiteSpace(dadosTransporte?.NumeroCarga ?? string.Empty) ? repCarga.BuscarPrimeiraCargaPorCodigoCargaEmbarcadorTodasSituacoes(dadosTransporte.NumeroCarga) : null;

                string validacaoDadosTransporte = ValidarDadosTransporte(dadosTransporte);
                if (!string.IsNullOrWhiteSpace(validacaoDadosTransporte))
                {
                    var resposta = new Retorno<bool>() { Mensagem = validacaoDadosTransporte, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, dadosTransporte?.NumeroCarga, dadosTransporte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte) : null, Utilidades.XML.Serializar(resposta, true), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                if (integradora == null)
                {
                    var resposta = new Retorno<bool>() { Mensagem = "Acesso não permitido para o Token enviado.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, dadosTransporte?.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Utilidades.XML.Serializar(resposta, true), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                if (carga == null)
                {
                    var resposta = new Retorno<bool>() { Mensagem = "Carga não encontrada com número " + dadosTransporte.NumeroCarga, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, dadosTransporte?.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Utilidades.XML.Serializar(resposta, true), "json", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                if (carga.Empresa == null)
                {
                    var resposta = new Retorno<bool>() { Mensagem = "Carga não possui transportador.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, dadosTransporte?.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Utilidades.XML.Serializar(resposta, true), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorPlaca(carga.Empresa.Codigo, dadosTransporte.VeiculoTracao.Placa);
                if (veiculoTracao == null)
                {
                    var resposta = new Retorno<bool>() { Mensagem = "Veículo tração (" + dadosTransporte.VeiculoTracao.Placa + ") não possui cadastro.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, resposta.Mensagem, dadosTransporte?.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Utilidades.XML.Serializar(resposta, true), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return resposta;
                }

                Dominio.Entidades.Usuario motorista = this.BuscarMotorista(carga.Empresa, dadosTransporte.Motorista.CPF, dadosTransporte.Motorista.Nome, configuracao.CadastrarMotoristaMobileAutomaticamente, unitOfWork);
                Dominio.Entidades.Usuario motoristaAdicional = dadosTransporte.MotoristaAdicional != null ? this.BuscarMotorista(carga.Empresa, dadosTransporte.MotoristaAdicional.CPF, dadosTransporte.MotoristaAdicional.Nome, configuracao.CadastrarMotoristaMobileAutomaticamente, unitOfWork) : null;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador)
                {
                    Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    Dominio.Entidades.Veiculo reboque1 = dadosTransporte.VeiculoReboque1 != null ? repVeiculo.BuscarPorPlaca(carga.Empresa.Codigo, dadosTransporte.VeiculoReboque1.Placa) : null;
                    Dominio.Entidades.Veiculo reboque2 = dadosTransporte.VeiculoReboque2 != null ? repVeiculo.BuscarPorPlaca(carga.Empresa.Codigo, dadosTransporte.VeiculoReboque2.Placa) : null;

                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte();
                    dadosTrans.Carga = carga;
                    dadosTrans.CodigoTracao = veiculoTracao.Codigo;
                    dadosTrans.CodigoReboque = reboque1?.Codigo ?? 0;
                    dadosTrans.CodigoSegundoReboque = reboque2?.Codigo ?? 0;
                    dadosTrans.CodigoEmpresa = carga.Empresa.Codigo;
                    if (carga.TipoDeCarga != null)
                        dadosTrans.CodigoTipoCarga = carga.TipoDeCarga.Codigo;
                    if (carga.TipoOperacao != null)
                        dadosTrans.CodigoTipoOperacao = carga.TipoOperacao.Codigo;
                    if (carga.ModeloVeicularCarga != null)
                        dadosTrans.CodigoModeloVeicular = carga.ModeloVeicularCarga.Codigo;

                    if (Enum.TryParse(dadosTransporte.TipoCheckin, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin tipoCheckin))
                        dadosTrans.TipoCheckin = tipoCheckin;

                    if (DateTime.TryParseExact(dadosTransporte.DataCheckout, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataCheckout))
                        dadosTrans.DataCheckout = dataCheckout;

                    dadosTrans.ListaCodigoMotorista = new List<int>();
                    dadosTrans.ListaCodigoMotorista.Add(motorista.Codigo);

                    if (motoristaAdicional != null)
                        dadosTrans.ListaCodigoMotorista.Add(motoristaAdicional.Codigo);

                    string mensagemErro = "";

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;
                    auditado.Integradora = integradora;

                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Dados de transporte recebidos via integração", unitOfWork);
                    var retorno = svcCarga.SalvarDadosTransporteCarga(dadosTrans, out mensagemErro, null, false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, string.Empty, null, auditado, unitOfWork);

                    if (retorno == null)
                    {
                        unitOfWork.Rollback();

                        var resposta = new Retorno<bool>() { Mensagem = mensagemErro, Objeto = false, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                        Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, string.Empty, dadosTransporte.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(resposta), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                        return resposta;
                    }

                    unitOfWork.CommitChanges();

                    var response = new Retorno<bool>() { Mensagem = "", Status = true, Objeto = true, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, string.Empty, dadosTransporte.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(response), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                    return response;
                }
                else
                {
                    var response = new Retorno<bool>() { Mensagem = "Situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite atualizar dados de transporte.", Objeto = false, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, string.Empty, dadosTransporte.NumeroCarga, Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(response), "json", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                    return response;
                }



            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<bool>() { Mensagem = "Falha genérica ao integrar dados.", Objeto = false, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ValidarDadosTransporte(Dominio.ObjetosDeValor.WebService.Rest.DadosTransporteCarga dadosTransporte)
        {
            if (dadosTransporte == null)
                return "Nenhum dado enviado.";

            if (string.IsNullOrWhiteSpace(dadosTransporte.NumeroCarga))
                return "Número da carga é obrigatório.";

            if (dadosTransporte.VeiculoTracao == null || string.IsNullOrWhiteSpace(dadosTransporte.VeiculoTracao.Placa))
                return "Veículo Tração é obrigatório.";

            if (dadosTransporte.Motorista == null || string.IsNullOrWhiteSpace(dadosTransporte.Motorista.CPF) || string.IsNullOrWhiteSpace(dadosTransporte.Motorista.Nome))
                return "Nome e CPF do motorista são obrigatórios.";

            if (dadosTransporte.Motorista != null && !Utilidades.Validate.ValidarCPF(Utilidades.String.OnlyNumbers(dadosTransporte.Motorista.CPF)))
                return "CPF do motorista inválido.";

            if (dadosTransporte.MotoristaAdicional != null && (string.IsNullOrWhiteSpace(dadosTransporte.Motorista.CPF) || string.IsNullOrWhiteSpace(dadosTransporte.Motorista.Nome)))
                return "Nome e CPF do motorista adicional são obrigatórios.";

            if (dadosTransporte.MotoristaAdicional != null && !Utilidades.Validate.ValidarCPF(Utilidades.String.OnlyNumbers(dadosTransporte.MotoristaAdicional.CPF)))
                return "CPF do motorista adicional inválido.";

            if (!string.IsNullOrWhiteSpace(dadosTransporte.DataCheckout))
            {
                if (!DateTime.TryParseExact(dadosTransporte.DataCheckout, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime DataCheckout))
                    return "A data checkout não está no formato correto (dd/MM/yyyy).";
            }
            else
                return "Data Checkout é obrigatória.";

            //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin tipoCheckin = null;
            if (!Enum.TryParse(dadosTransporte.TipoCheckin, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin tipoCheckin))
                return "TipoChekin inválido.";

            //if (dadosTransporte.TipoCheckin == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin.Invalido)
            //    return "TipoChekin inválido.";

            return string.Empty;
        }

        private Dominio.Entidades.Usuario BuscarMotorista(Dominio.Entidades.Empresa empresa, string cpfMotorista, string nomeMotorista, bool cadastrarMotoristaMobileAutomaticamente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(empresa.Codigo, Utilidades.String.OnlyNumbers(cpfMotorista), "M");

            if (motorista == null)
            {
                motorista = new Dominio.Entidades.Usuario();
                motorista.CPF = cpfMotorista;
                motorista.Nome = nomeMotorista;
                motorista.Tipo = "M";
                motorista.Status = "A";
                motorista.Localidade = empresa.Localidade;
                motorista.Empresa = empresa;

                repUsuario.Inserir(motorista);

                if (cadastrarMotoristaMobileAutomaticamente)
                {
                    string adminStringConexao = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
                    string host = System.Configuration.ConfigurationManager.AppSettings["host"];
                    if (!string.IsNullOrWhiteSpace(adminStringConexao) && !string.IsNullOrWhiteSpace(host))
                    {
                        using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
                        {
                            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repositorioClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(adminUnitOfWork);
                            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repositorioClienteURLAcesso.BuscarPorURL(host);
                            if (clienteAcesso != null)
                            {
                                motorista.NaoBloquearAcessoSimultaneo = true;
                                Servicos.Usuario.ConfigurarUsuarioMobile(ref motorista, null, clienteAcesso, adminUnitOfWork);
                                repUsuario.Atualizar(motorista);
                            }
                        }
                    }
                }
            }

            return motorista;
        }

        private Dominio.Entidades.WebService.Integradora ValidarToken()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string tokenBaerer = headers["Authorization"];
                if (string.IsNullOrWhiteSpace(tokenBaerer))
                    return null;

                Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorTokenIntegracao(tokenBaerer);

                return integradora;
            }
        }

        private string ObterNumeroCargaRequest(string xmlCarga)
        {
            Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse cargaUnilever = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse));
            using (StringReader reader = new StringReader(xmlCarga))
            {
                cargaUnilever = (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse)serializer.Deserialize(reader);
            }

            if (cargaUnilever.ExOutputTab.Item.Stage.ItemStage.Count <= 0)
                throw new Exception("Não encontrado nenhum Stage no XML, verifique o arquivo e envie novamente.");

            return cargaUnilever.ExOutputTab.Item.Tknum;
        }

        private bool ValidarIntegracaoAnterior(int codigoIntegradora, string requestAtual, string numeroIdentificador, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroIdentificador))
                    return true;

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoRetorno = repIntegradoraIntegracaoRetorno.BuscarUltimaPorIdentificador(numeroIdentificador, codigoIntegradora, 0, null);
                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoSucesso = repIntegradoraIntegracaoRetorno.BuscarUltimaPorIdentificador(numeroIdentificador, codigoIntegradora, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                if (integradoraIntegracaoRetorno != null && integradoraIntegracaoSucesso != null)
                {
                    string numeroCarga = string.Empty;
                    string requestAnterior = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integradoraIntegracaoRetorno.ArquivoRequisicao);
                    string jsonCargaAnterior = Newtonsoft.Json.JsonConvert.SerializeObject(ConverterObjetoCargaIntegracao(requestAnterior, ref numeroCarga));

                    string jsonCargaAtual = Newtonsoft.Json.JsonConvert.SerializeObject(ConverterObjetoCargaIntegracao(requestAtual, ref numeroCarga));

                    return jsonCargaAtual != jsonCargaAnterior;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return true;
            }
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> ConverterObjetoCargaIntegracao(string xmlCarga, ref string numeroCarga)
        {
            Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse cargaUnilever = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse));
            using (StringReader reader = new StringReader(xmlCarga))
            {
                cargaUnilever = (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse)serializer.Deserialize(reader);
            }

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> listaCargaIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();

            if (cargaUnilever.ExOutputTab.Item.Stage.ItemStage.Count <= 0)
                throw new Exception("Não encontrado nenhum Stage no XML, verifique o arquivo e envie novamente.");

            //if (cargaUnilever.ExOutputTab.Item.GlobalStat != 6 && cargaUnilever.ExOutputTab.Item.GlobalStat != 7)
            //    throw new Exception("GlobalStat igual a "+ cargaUnilever.ExOutputTab.Item.GlobalStat + "), carga não gerada.");

            numeroCarga = cargaUnilever.ExOutputTab.Item.Tknum;

            string codigoTipoDeOperacao = cargaUnilever.ExOutputTab.Item.Shtyp;

            List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemStage> listaStage = cargaUnilever.ExOutputTab.Item.Stage.ItemStage;

            for (int i = 0; i < listaStage.Count; i++)
            {
                List<string> numeroPedidos = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Vbeln.ItemVbeln; //Pedidos
                List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemNfe> notas = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Nfe.ItemNfe;

                for (int j = 0; j < numeroPedidos.Count; j++)
                {
                    DateTime dataCorte = DateTime.MinValue;
                    if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["DataCorteCargas"])) 
                        DateTime.TryParseExact(System.Configuration.ConfigurationManager.AppSettings["DataCorteCargas"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCorte);

                    DateTime.TryParseExact(cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].PricinCalcDat, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataArquivo);
                    if (dataArquivo > DateTime.MinValue && dataCorte > DateTime.MinValue && dataArquivo < dataCorte)
                        throw new Exception("Data " + cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].PricinCalcDat + " menor que a data de corte definida. Integração ignorada.");

                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                    cargaIntegracao.NumeroCarga = cargaUnilever.ExOutputTab.Item.Tknum;
                    cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao
                    {
                        CodigoIntegracao = codigoTipoDeOperacao
                    };
                    cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador
                    {
                        CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].LoadType
                    };
                    cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular
                    {
                        CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].EquipType
                    };
                    cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa
                    {
                        CNPJ = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Stcd1
                    };
                    cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                    Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                    produto.CodigoProduto = "1";
                    produto.DescricaoProduto = "DIVERSOS";
                    produto.CodigoGrupoProduto = "1";
                    produto.DescricaoGrupoProduto = "DIVERSOS";
                    cargaIntegracao.Produtos.Add(produto);

                    cargaIntegracao.NumeroPedidoEmbarcador = numeroPedidos[j];
                    List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemNfe> notasPedidos = notas.Where(o => o.Vbeln == cargaIntegracao.NumeroPedidoEmbarcador).ToList();
                    if (notasPedidos.Count > 0)
                    {
                        cargaIntegracao.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                        {
                            CPFCNPJ = notasPedidos[0].Stcd1Is,
                            CodigoAtividade = 3,
                            Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                            {
                                Bairro = notasPedidos[0].City2Is,
                                CEP = Utilidades.String.OnlyNumbers(notasPedidos[0].PostCode1Is),
                                Cidade = new Dominio.ObjetosDeValor.Localidade
                                {
                                    IBGE = notasPedidos[0].ZdestCitcodeIs
                                },
                                Logradouro = notasPedidos[0].StreetIs,
                                Numero = notasPedidos[0].HouseNum1Is,
                                Telefone = Utilidades.String.OnlyNumbers(notasPedidos[0].TelNumberIs)
                            },
                            RGIE = notasPedidos[0].Stcd3Is,
                            NomeFantasia = notasPedidos[0].Name1Is,
                            RazaoSocial = notasPedidos[0].Name1Is,
                            AtualizarEnderecoPessoa = true
                        };

                        cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                        cargaIntegracao.PesoBruto = 0;
                        for (int n = 0; n < notasPedidos.Count; n++)
                        {
                            cargaIntegracao.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                            {
                                CPFCNPJ = notasPedidos[n].Stcd1,
                                //CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].DestCustomer,
                                CodigoAtividade = 3,
                                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                                {
                                    Bairro = notasPedidos[n].City2,
                                    CEP = Utilidades.String.OnlyNumbers(notasPedidos[n].PostCode1),
                                    Cidade = new Dominio.ObjetosDeValor.Localidade
                                    {
                                        IBGE = notasPedidos[n].ZdestCitcode
                                    },
                                    Logradouro = notasPedidos[n].Street,
                                    Numero = notasPedidos[n].HouseNum1,
                                    Telefone = Utilidades.String.OnlyNumbers(notasPedidos[n].TelNumber)
                                },
                                RGIE = notasPedidos[n].Stcd3,
                                NomeFantasia = notasPedidos[n].Name1,
                                RazaoSocial = notasPedidos[n].Name1,
                                AtualizarEnderecoPessoa = true
                            };

                            cargaIntegracao.PesoBruto += notasPedidos[n].Brgew;


                            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                            notaFiscal.Serie = notasPedidos[n].Series;
                            notaFiscal.Chave = notasPedidos[n].Zfield;
                            notaFiscal.Numero = notasPedidos[n].Nfenum;
                            notaFiscal.Valor = notasPedidos[n].NfeTotal;
                            notaFiscal.PesoBruto = notasPedidos[n].Brgew;
                            notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

                            DateTime.TryParseExact(notasPedidos[n].Docdat, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissao);
                            if (dataEmissao.Date > DateTime.MinValue)
                                notaFiscal.DataEmissao = dataEmissao.ToString("dd/MM/yyyy");

                            cargaIntegracao.NotasFiscais.Add(notaFiscal);
                        }
                    }
                    else
                    {
                        throw new Exception("Pedido " + cargaIntegracao.NumeroPedidoEmbarcador + " sem notas");
                    }

                    if (cargaIntegracao.Remetente != null)
                    {
                        cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial
                        {
                            CodigoIntegracao = cargaIntegracao.Remetente.CPFCNPJ
                        };
                    }

                    listaCargaIntegracao.Add(cargaIntegracao);
                }

                if (codigoTipoDeOperacao == "ZA02")
                    break;
            }

            return listaCargaIntegracao;
        }

        private string ObterNumeroCarga(string xmlCarga)
        {
            try
            {
                string tagInicial = "<Tknum>";
                string tagFinal = "</Tknum>";
                int Pos1 = xmlCarga.IndexOf(tagInicial) + tagInicial.Length;
                int Pos2 = xmlCarga.IndexOf(tagFinal);
                return xmlCarga.Substring(Pos1, Pos2 - Pos1);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível encontrar numero da carga, XML = " + xmlCarga);
                return string.Empty;
            }
        }

    }
}