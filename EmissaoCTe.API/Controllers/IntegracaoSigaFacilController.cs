using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoSigaFacilController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("integracaosigafacil.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);
                int.TryParse(Request.Params["InicioRegistros"], out int inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string placa = Request.Params["Placa"] ?? string.Empty;
                string ciot = Request.Params["CIOT"] ?? string.Empty;

                Dominio.ObjetosDeValor.Enumerador.StatusCIOT? status = null;
                if (Enum.TryParse(Request.Params["Status"], out Dominio.ObjetosDeValor.Enumerador.StatusCIOT statusAux))
                    status = statusAux;

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unitOfWork);

                List<Dominio.Entidades.CIOTSigaFacil> ciots = repCIOT.Consultar(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, placa, ciot, status, inicioRegistros, 50);
                int countCIOTs = repCIOT.ContarConsulta(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, placa, ciot, status);

                var retorno = (dynamic)null;
                string[] campos;

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura || this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad)
                {
                    retorno = (from obj in ciots
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   Transportador = obj.Transportador != null ? obj.Transportador.Nome : string.Empty,
                                   Motorista = obj.Motorista != null ? obj.Motorista.Nome : string.Empty,
                                   Placa = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                   Status = obj.DescricaoStatus,
                                   RetornoAutorizacao = obj.CodigoMensagemRetorno,
                                   RetornoCancelamento = obj.CodigoMensagemRetornoCancelamento
                               }).ToList();
                }
                else
                {
                    retorno = (from obj in ciots
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   Origem = obj.Origem?.Estado.Sigla + " / " + obj.Origem?.Descricao,
                                   Destino = obj.Destino?.Estado.Sigla + " / " + obj.Destino?.Descricao,
                                   Placa = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                   Status = obj.DescricaoStatus,
                                   RetornoAutorizacao = obj.CodigoMensagemRetorno,
                                   RetornoCancelamento = obj.CodigoMensagemRetornoCancelamento
                               }).ToList();
                }


                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura)
                    campos = new string[] { "Codigo", "Número|10", "Data Emissão|10", "Transportador|13", "Motorista|13", "Veículo|8", "Status|10", "Retorno Auto.|12", "Retorno Can/Enc|12" };
                else
                    campos = new string[] { "Codigo", "Número|10", "Data Emissão|10", "Origem|13", "Destino|13", "Veículo|8", "Status|10", "Retorno Auto.|12", "Retorno Can/Enc|12" };

                return Json(retorno, true, null, campos, countCIOTs);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CIOTs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.CIOTSigaFacilLogXML repCIOTSigaFacilLogXML = new Repositorio.CIOTSigaFacilLogXML(unitOfWork);

                int.TryParse(Request.Params["InicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["CIOT"], out int ciot);

                List<Dominio.Entidades.CIOTSigaFacilLogXML> integracoes = repCIOTSigaFacilLogXML.Consultar(this.EmpresaUsuario.Codigo, ciot, inicioRegistros, 50);
                int count = repCIOTSigaFacilLogXML.ContarConsulta(this.EmpresaUsuario.Codigo, ciot);

                var retorno = from obj in integracoes
                              select new
                              {
                                  obj.Codigo,
                                  DataHora = obj.DataHora.ToString("dd/MM/yyyy HH:mm"),
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Data e Hora|90" }, count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.CIOTSigaFacilLogXML repCIOTSigaFacilLogXML = new Repositorio.CIOTSigaFacilLogXML(unitOfWork);

                int.TryParse(Request.Params["Integracao"], out int integracao);
                string tipo = Request.Params["Tipo"] ?? "";

                if (tipo != "Requisicao" && tipo != "Resposta")
                    tipo = "Resposta";

                Dominio.Entidades.CIOTSigaFacilLogXML log = repCIOTSigaFacilLogXML.BuscarPorCodigo(this.EmpresaUsuario.Codigo, integracao);
                if (log == null)
                    return Json<bool>(false, false, "XML não encontrado.");

                byte[] data = null;
                if (tipo == "Requisicao")
                    data = System.Text.Encoding.Default.GetBytes(log.Requisicao);
                else
                    data = System.Text.Encoding.Default.GetBytes(log.Resposta);

                if (data == null)
                    return Json<bool>(false, false, "XML não encontrado.");

                if (log.CIOT.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad)
                    return Arquivo(data, "text/xml", string.Concat(log.CIOT.Numero.ToString(), '-', tipo, ".json"));
                else
                    return Arquivo(data, "text/xml", string.Concat(log.CIOT.Numero.ToString(), '-', tipo, ".xml"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificaVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigoVeiculo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.CIOTSigaFacil repCIOTSigaFacil = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.CIOTSigaFacil ciot = new Dominio.Entidades.CIOTSigaFacil();

                if (veiculo != null)
                {
                    ciot = repCIOTSigaFacil.BuscarPorNumero(veiculo.CIOT, this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto);
                    // Possui CIOT aberto com esse numero
                    if (ciot != null)
                        return Json(new { Numero = ciot.Numero.ToString() }, true, "");
                }

                // Nao possui CIOT aberto
                return Json<bool>(false, false, "");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params["DataInicial"], out dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out dataFinal);

                bool semCIOT;
                bool.TryParse(Request.Params["SemCIOT"], out semCIOT);

                string status = Request.Params["Status"];
                string placa = Request.Params["Placa"];
                string numeroCIOT = Request.Params["CIOT"];

                if (!string.IsNullOrWhiteSpace(numeroCIOT) && numeroCIOT.Length < 12)
                    numeroCIOT = Utilidades.String.Left(numeroCIOT, 12);

                string[] arrStatus;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = null;
                int countCTes = 0;

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.TipoEmpresaCIOT == Dominio.Enumeradores.TipoEmpresaCIOT.Embarcador)
                {
                    countCTes = repCTe.ContarConsultaCTesEmbarcador(this.EmpresaUsuario.EmpresaPai.Codigo, this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.TipoAmbiente, new string[] { "R", "S" }, numeroInicial, numeroFinal);
                    listaCTes = repCTe.ConsultarCTesEmbarcador(this.EmpresaUsuario.EmpresaPai.Codigo, this.EmpresaUsuario.CNPJ, this.EmpresaUsuario.TipoAmbiente, new string[] { "R", "S" }, numeroInicial, numeroFinal, inicioRegistros, 50);
                }
                else
                {
                    if (status == "A") arrStatus = new string[] { "A" };
                    else arrStatus = new string[] { "R", "S" };

                    listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, arrStatus, numeroInicial, numeroFinal, numeroCIOT, semCIOT, placa, dataInicial, dataFinal, inicioRegistros, 50);
                    countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, arrStatus, numeroInicial, numeroFinal, numeroCIOT, semCIOT, placa, dataInicial, dataFinal);
                }

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                                   cte.Numero,
                                   Serie = cte.Serie,
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais.Nome) : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais.Nome) : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : string.Empty,
                                   Valor = string.Format("{0:n2}", cte.Valor),
                                   cte.DescricaoStatus,
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Núm.|8", "Série|8", "Emissão|10", "Início|15", "Destinatário|13", "Término|15", "Valor|10", "Status|8" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodosOsCTes()
        {
            /* Essa funcao apenas retorna os CODIGOS dos CTES para posteriormente buscar as informacoes (evita duplicidade de codigo no JS) */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params["DataInicial"], out dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out dataFinal);

                bool semCIOT;
                bool.TryParse(Request.Params["SemCIOT"], out semCIOT);

                string status = Request.Params["Status"];
                string placa = Request.Params["Placa"];
                string numeroCIOT = Request.Params["CIOT"];

                if (!string.IsNullOrWhiteSpace(numeroCIOT) && numeroCIOT.Length > 12)
                    numeroCIOT = Utilidades.String.Left(numeroCIOT, 12);

                string[] arrStatus;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = null;
                int countCTes = 0;

                if (status == "A") arrStatus = new string[] { "A" };
                else arrStatus = new string[] { "R", "S" };

                countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, arrStatus, numeroInicial, numeroFinal, numeroCIOT, semCIOT, placa, dataInicial, dataFinal);
                listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, arrStatus, numeroInicial, numeroFinal, numeroCIOT, semCIOT, placa, dataInicial, dataFinal, 0, countCTes);

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao selecionar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarInformacoesAbertura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (!ValidaConfiguracoes(out string erro))
                    return Json<bool>(false, false, erro);

                int.TryParse(Request.Params["Codigo"], out int codigo);


                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                    return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");

                unidadeDeTrabalho.Start();

                PreencherEntidadeEncerramento(ref ciot, unidadeDeTrabalho);
                repCIOT.Atualizar(ciot);

                unidadeDeTrabalho.CommitChanges();


                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao abrir o CIOT.");
            }
        }

        /*[AcceptVerbs("POST")]
        public ActionResult Salvar2()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa não está configurada para a emissão de CIOT.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null)
                    return Json<bool>(false, false, "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.");
                }
                else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.");
                }

                int codigo, codigoNaturezaCarga, codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoNaturezaCarga"], out codigoNaturezaCarga);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                double cpfCnpjTransportador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJTransportador"]), out cpfCnpjTransportador);

                DateTime dataInicioViagem, dataTerminoViagem;
                DateTime.TryParseExact(Request.Params["DataInicioViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioViagem);
                DateTime.TryParseExact(Request.Params["DataTerminoViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataTerminoViagem);

                Dominio.Entidades.CIOTSigaFacil ciot = null;
                Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente();

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Servicos.EFrete svcEFrete = new Servicos.EFrete(Conexao.StringConexao);

                // Verifica Natureza da carga
                if (codigoNaturezaCarga <= 0)
                {
                    return Json<bool>(false, false, "Natureza da carga é obrigatório.");
                }

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    ciot = repCIOT.BuscarPorCodigo(codigo);

                    if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto)
                        return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                    if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                        return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    ciot = new Dominio.Entidades.CIOTSigaFacil();
                    ciot.TipoIntegradora = this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value;
                }

                ciot.NaturezaCarga = repNaturezaCargaANTT.BuscarPorCodigo(codigoNaturezaCarga);
                ciot.DataInicioViagem = dataInicioViagem;
                ciot.DataTerminoViagem = dataTerminoViagem;
                ciot.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;
                ciot.Transportador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);
                ciot.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                ciot.TipoPagamento = this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT.Value;
                ciot.CategoriaTransportador = Dominio.Enumeradores.CategoriaTransportadorANTT.TAC;

                dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;

                if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && (string.IsNullOrWhiteSpace(ciot.Motorista.NumeroCartao) || ciot.Motorista.NumeroCartao.Length != 16))
                    return Json<bool>(false, false, "O número do cartão do motorista é inválido (deve possuir 16 dígitos).");

                if (ciot.NumeroCartaoTransportador != null && ciot.NumeroCartaoTransportador != "")
                {
                    if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && ciot.NumeroCartaoTransportador.Length != 16)
                        return Json<bool>(false, false, "O número do cartão do transportador é inválido (deve possuir 16 dígitos).");
                }

                if (ciot.Veiculo == null || ciot.Veiculo.RNTRC <= 0)
                    return Json<bool>(false, false, "O RNTRC do veículo é inválido.");

                unidadeDeTrabalho.Start();

                if (ciot.Codigo > 0)
                {
                    repCIOT.Atualizar(ciot);
                }
                else
                {
                    ciot.Empresa = this.EmpresaUsuario;
                    int proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete);
                    int proximoNumeroEfrete = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);

                    if (proximoNumeroEfrete < proximoNumero)
                        proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);
                    else
                        //if (proximoNumero == 0)
                        proximoNumero = proximoNumeroEfrete;
                    //proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, ciot.Empresa.Configuracao?.TipoIntegradoraCIOT);

                    ciot.Numero = proximoNumero + 1;
                    ciot.DataEmissao = DateTime.Now;
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

                    repCIOT.Inserir(ciot);
                }

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente || ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                {
                    // ABRIR CIOT
                    if (!svcEFrete.AbrirCIOT(ciot.Codigo, unidadeDeTrabalho))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                        repCIOT.Atualizar(ciot);
                    }

                    // Quando abrir um CIOT, deve atualizar o valor do CIOT no cadastro do veiculo
                    this.AtualizarCIOTVeiculo(ciot, ciot.NumeroCIOT);
                }
                else
                {
                    // ENCERRAR O CIOT
                    this.SalvarInformacoesAbertura();

                    // Chama o WS para encerrar o ciot
                    if (!svcEFrete.EncerrarCIOTAberto(ciot.Codigo, unidadeDeTrabalho))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;
                        repCIOT.Atualizar(ciot);
                    }

                    // Remover CIOT do veiculo
                    this.AtualizarCIOTVeiculo(ciot, string.Empty);
                }
                return Json(true, true);
            }
            catch (Exception ex)
            {
                if (unidadeDeTrabalho.Transacao != null && unidadeDeTrabalho.Transacao.IsActive)
                    unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao abrir o CIOT.");
            }
        }*/

        [AcceptVerbs("POST")]
        public ActionResult SalvarAbertura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa não está configurada para a emissão de CIOT.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null && this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT != Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad))
                    return Json<bool>(false, false, "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.");
                }
                else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.");
                }

                int codigo, codigoNaturezaCarga, codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoNaturezaCarga"], out codigoNaturezaCarga);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                double cpfCnpjTransportador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJTransportador"]), out cpfCnpjTransportador);

                DateTime dataInicioViagem, dataTerminoViagem;
                DateTime.TryParseExact(Request.Params["DataInicioViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioViagem);
                DateTime.TryParseExact(Request.Params["DataTerminoViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataTerminoViagem);

                Dominio.Entidades.CIOTSigaFacil ciot = null;
                Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente();

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Servicos.EFrete svcEFrete = new Servicos.EFrete(unidadeDeTrabalho);

                // Verifica Natureza da carga
                if (codigoNaturezaCarga <= 0)
                {
                    return Json<bool>(false, false, "Natureza da carga é obrigatório.");
                }

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    ciot = repCIOT.BuscarPorCodigo(codigo);

                    if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto)
                        return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                    if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                        return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    ciot = new Dominio.Entidades.CIOTSigaFacil();
                    ciot.TipoIntegradora = this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value;
                }

                ciot.NaturezaCarga = repNaturezaCargaANTT.BuscarPorCodigo(codigoNaturezaCarga);
                ciot.DataInicioViagem = dataInicioViagem;
                ciot.DataTerminoViagem = dataTerminoViagem;
                ciot.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(0, codigoMotorista);
                ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;
                ciot.Transportador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);
                ciot.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                ciot.TipoPagamento = Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito;
                ciot.CategoriaTransportador = Dominio.Enumeradores.CategoriaTransportadorANTT.TAC; //CIOT da eFrete do tipo Abertura tem que ser TAC pois o Tipo padrão exige enviar as viagens e pagamentos na abertura

                dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;

                if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && (string.IsNullOrWhiteSpace(ciot.Motorista.NumeroCartao) || ciot.Motorista.NumeroCartao.Length != 16))
                    return Json<bool>(false, false, "O número do cartão do motorista é inválido (deve possuir 16 dígitos).");

                if (ciot.NumeroCartaoTransportador != null && ciot.NumeroCartaoTransportador != "")
                {
                    if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && ciot.NumeroCartaoTransportador.Length != 16)
                        return Json<bool>(false, false, "O número do cartão do transportador é inválido (deve possuir 16 dígitos).");
                }

                if (ciot.Veiculo == null || ciot.Veiculo.RNTRC <= 0)
                    return Json<bool>(false, false, "O RNTRC do veículo é inválido.");

                unidadeDeTrabalho.Start();

                if (ciot.Codigo > 0)
                {
                    repCIOT.Atualizar(ciot);
                }
                else
                {
                    ciot.Empresa = this.EmpresaUsuario;
                    int proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete);
                    int proximoNumeroEfrete = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);

                    if (proximoNumeroEfrete < proximoNumero)
                        proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);
                    else
                        //if (proximoNumero == 0)
                        proximoNumero = proximoNumeroEfrete;
                    //proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, ciot.Empresa.Configuracao?.TipoIntegradoraCIOT);

                    ciot.Numero = proximoNumero + 1;
                    ciot.DataEmissao = DateTime.Now;
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

                    repCIOT.Inserir(ciot);
                }

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente || ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                {
                    // ABRIR CIOT
                    if (!svcEFrete.AbrirCIOT(ciot.Codigo))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                        repCIOT.Atualizar(ciot);
                    }

                    // Quando abrir um CIOT, deve atualizar o valor do CIOT no cadastro do veiculo
                    this.AtualizarCIOTVeiculo(ciot, ciot.NumeroCIOT, unidadeDeTrabalho);
                }
                else
                {
                    // ENCERRAR O CIOT
                    this.SalvarInformacoesAbertura();

                    // Chama o WS para encerrar o ciot
                    if (!svcEFrete.EncerrarCIOTAberto(ciot.Codigo))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;
                        repCIOT.Atualizar(ciot);
                    }

                    // Remover CIOT do veiculo
                    this.AtualizarCIOTVeiculo(ciot, string.Empty, unidadeDeTrabalho);
                }
                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao abrir o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarAberturaTruckPad()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            bool salvouCIOT = false;
            Dominio.Entidades.CIOTSigaFacil ciot = null;

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa não está configurada para a emissão de CIOT.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null)
                    return Json<bool>(false, false, "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.");
                }
                else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.");
                }

                int codigo, codigoNaturezaCarga, codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoNaturezaCarga"], out codigoNaturezaCarga);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                double cpfCnpjTransportador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJTransportador"]), out cpfCnpjTransportador);

                DateTime dataInicioViagem, dataTerminoViagem;
                DateTime.TryParseExact(Request.Params["DataInicioViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioViagem);
                DateTime.TryParseExact(Request.Params["DataTerminoViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataTerminoViagem);

                Dominio.Entidades.DadosCliente dadosCliente = new Dominio.Entidades.DadosCliente();

                Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Servicos.TruckPad svcTruckPad = new Servicos.TruckPad(unidadeDeTrabalho);

                if (codigoNaturezaCarga <= 0)
                {
                    return Json<bool>(false, false, "Natureza da carga é obrigatório.");
                }

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    ciot = repCIOT.BuscarPorCodigo(codigo);

                    if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto)
                        return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                    if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                        return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    ciot = new Dominio.Entidades.CIOTSigaFacil();
                    ciot.TipoIntegradora = this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value;
                }

                ciot.NaturezaCarga = repNaturezaCargaANTT.BuscarPorCodigo(codigoNaturezaCarga);
                ciot.DataInicioViagem = dataInicioViagem;
                ciot.DataTerminoViagem = dataTerminoViagem;
                ciot.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;
                ciot.Transportador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);
                ciot.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                ciot.TipoPagamento = this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT.Value;
                ciot.CategoriaTransportador = Dominio.Enumeradores.CategoriaTransportadorANTT.TAC; //CIOT da eFrete do tipo Abertura tem que ser TAC pois o Tipo padrão exige enviar as viagens e pagamentos na abertura

                dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;

                if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && (string.IsNullOrWhiteSpace(ciot.Motorista.NumeroCartao) || ciot.Motorista.NumeroCartao.Length != 16))
                    return Json<bool>(false, false, "O número do cartão do motorista é inválido (deve possuir 16 dígitos).");

                if (ciot.NumeroCartaoTransportador != null && ciot.NumeroCartaoTransportador != "")
                {
                    if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && ciot.NumeroCartaoTransportador.Length != 16)
                        return Json<bool>(false, false, "O número do cartão do transportador é inválido (deve possuir 16 dígitos).");
                }

                if (ciot.Veiculo == null || ciot.Veiculo.RNTRC <= 0)
                    return Json<bool>(false, false, "O RNTRC do veículo é inválido.");

                unidadeDeTrabalho.Start();

                if (ciot.Codigo > 0)
                {
                    repCIOT.Atualizar(ciot);
                }
                else
                {
                    ciot.Empresa = this.EmpresaUsuario;
                    int proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete);
                    int proximoNumeroEfrete = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);

                    if (proximoNumeroEfrete < proximoNumero)
                        proximoNumero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo);
                    else
                        proximoNumero = proximoNumeroEfrete;
                    ciot.Numero = proximoNumero + 1;
                    ciot.DataEmissao = DateTime.Now;
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

                    repCIOT.Inserir(ciot);
                }

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                salvouCIOT = true;

                if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente || ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                {
                    // ABRIR CIOT
                    if (!svcTruckPad.AbrirCIOT(ciot.Codigo, unidadeDeTrabalho))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                        repCIOT.Atualizar(ciot);
                    }

                    // Quando abrir um CIOT, deve atualizar o valor do CIOT no cadastro do veiculo
                    this.AtualizarCIOTVeiculo(ciot, ciot.NumeroCIOT, unidadeDeTrabalho);
                }
                else
                {
                    // ENCERRAR O CIOT
                    this.SalvarInformacoesAbertura();

                    // Chama o WS para encerrar o ciot
                    if (!svcTruckPad.EncerrarCIOTAberto(ciot.Codigo, unidadeDeTrabalho))
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;
                        repCIOT.Atualizar(ciot);
                    }

                    // Remover CIOT do veiculo
                    this.AtualizarCIOTVeiculo(ciot, string.Empty, unidadeDeTrabalho);
                }
                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                if (!salvouCIOT)
                    return Json<bool>(false, false, ex.Message);
                else
                {
                    if (ciot != null)
                    {
                        ciot.MensagemRetorno = ex.Message;
                        repCIOT.Atualizar(ciot);
                    }
                    return Json(true, true, ex.Message);
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarCTesAbertura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa não está configurada para a emissão de CIOT.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null)
                    return Json<bool>(false, false, "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.");
                }
                else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.");
                }
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão de alteração negada!");

                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                    return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");

                unidadeDeTrabalho.Start();

                this.SalvarDocumentosDoCIOT(ciot, unidadeDeTrabalho);
                repCIOT.Atualizar(ciot);
                repEmpresa.Atualizar(this.EmpresaUsuario);

                unidadeDeTrabalho.CommitChanges();

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os documentos do CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Cancelar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                string motivoCancelamento = Request.Params["MotivoCancelamento"];

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                if (
                    ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado &&
                    ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto &&
                    ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento &&
                    ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado
                )
                    return Json<bool>(false, false, "O status do CIOT não permite o cancelamento do mesmo.");

                if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado)
                    return Json<bool>(false, false, "O CIOT já foi cancelado.");

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                    ciot.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                    repEmpresa.Atualizar(this.EmpresaUsuario);
                }

                ciot.MotivoCancelamento = motivoCancelamento;

                this.CancelarCIOT(ciot.Codigo, unidadeDeTrabalho);
                this.AtualizarCIOTVeiculo(ciot, string.Empty, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Encerrar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    return Json<bool>(false, false, "O status do CIOT não permite o encerramento do mesmo.");

                this.EncerrarCIOT(ciot.Codigo, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Emitir()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa não está configurada para a emissão de CIOT.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null)
                    return Json<bool>(false, false, "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.");

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.");
                }
                else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                        return Json<bool>(false, false, "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.");
                }

                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                    return Json<bool>(false, false, "O status do CIOT não permite a emissão do mesmo.");

                if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                    return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                {
                    ciot.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                    repCIOT.Atualizar(ciot);

                    List<Dominio.Entidades.CTeCIOTSigaFacil> documentos = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

                    foreach (Dominio.Entidades.CTeCIOTSigaFacil documento in documentos)
                    {
                        documento.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                        repCTeCIOT.Atualizar(documento);
                    }

                    repEmpresa.Atualizar(this.EmpresaUsuario);
                }

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
                {
                    Dominio.Entidades.Usuario motorista = repMotorista.BuscarMotoristaPorCPF(this.EmpresaUsuario.Codigo, ciot.Motorista.CPF);

                    if (motorista == null)
                        return Json<bool>(false, false, "Motorista não possui cadastro.");

                    if (motorista.Nome == null || motorista.Nome == "")
                        return Json<bool>(false, false, "Motorista sem Endereço cadastrado.");

                    if (motorista.NumeroHabilitacao == null || motorista.NumeroHabilitacao == "")
                        return Json<bool>(false, false, "Motorista sem CNH cadastrada.");

                    if (motorista.DataNascimento == null || motorista.DataNascimento == DateTime.MinValue)
                        return Json<bool>(false, false, "Motorista sem data de nascimento cadastrada.");

                    if (motorista.Bairro == null || motorista.Bairro == "")
                        return Json<bool>(false, false, "Motorista sem Bairro cadastrada.");

                    if (motorista.CEP == null || motorista.CEP == "")
                        return Json<bool>(false, false, "Motorista sem CEP cadastrada.");

                    if (motorista.Endereco == null || motorista.Endereco == "")
                        return Json<bool>(false, false, "Motorista sem Endereço cadastrado.");

                    if (motorista.Telefone == null || motorista.Telefone == "")
                        return Json<bool>(false, false, "Motorista sem Telefone cadastrado.");

                    if (motorista.Localidade == null)
                        return Json<bool>(false, false, "Motorista sem Municipio cadastrado.");

                    Dominio.Entidades.Cliente transportador = repCliente.BuscarPorCPFCNPJ(ciot.Transportador.CPF_CNPJ);

                    if (transportador == null)
                        return Json<bool>(false, false, "Transportador não possui cadastro.");

                    if (transportador.Bairro == null || transportador.Bairro == "")
                        return Json<bool>(false, false, "Transportador sem Bairro cadastrada.");

                    if (transportador.CEP == null || transportador.CEP == "")
                        return Json<bool>(false, false, "Transportador sem CEP cadastrada.");

                    if (transportador.Endereco == null || transportador.Endereco == "")
                        return Json<bool>(false, false, "Transportador sem Endereço cadastrado.");

                    if (transportador.Telefone1 == null || transportador.Telefone1 == "")
                        return Json<bool>(false, false, "Transportador sem Telefone cadastrado.");

                    if (transportador.Localidade == null)
                        return Json<bool>(false, false, "Transportador sem Municipio cadastrado.");

                }

                this.EmitirCIOT(ciot.Codigo, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não encontrado.");

                Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);

                string unidade = repInformacaoCarga.ObterUnidade(cte.Codigo);
                decimal peso = repInformacaoCarga.ObterPesoKg(cte.Codigo);
                if (peso == 0)
                    peso = repInformacaoCarga.ObterPesoTotal(cte.Codigo);
                string especie = "";

                switch (unidade)
                {
                    case "00":
                        especie = "M3";
                        break;
                    case "01":
                        especie = "KG";
                        break;
                    case "02":
                        especie = "TO";
                        break;
                    case "03":
                        especie = "UN";
                        break;
                    case "04":
                        especie = "LT";
                        break;
                    default:
                        especie = "KG";
                        break;
                }

                var retorno = new
                {
                    ValorFrete = cte.ValorAReceber.ToString("n2"),
                    ValorTotalMercadoria = cte.ValorTotalMercadoria.ToString("n2"),
                    ValorMercadoriaKG = (cte.ValorTotalMercadoria / peso).ToString("n2"),
                    ValorFreteKG = (cte.ValorAReceber / peso).ToString("n2"),
                    PesoArredondado = peso.ToString("n0"),
                    Peso = peso.ToString("n2"),
                    Especie = especie
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesCTesSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int[] codigosCTes = JsonConvert.DeserializeObject<int[]>(Request.Params["Documentos"]);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigosCTes);

                if (ctes == null)
                    return Json<bool>(false, false, "CT-e não encontrado.");

                var retorno = (from obj in ctes
                               select new
                               {
                                   CodigoCTe = obj.Codigo,
                                   NumeroCTe = obj.Numero,
                                   SerieCTe = obj.Serie.Numero,
                                   ValorFrete = obj.ValorAReceber.ToString("n2"),
                                   ValorTotalMercadoria = obj.ValorTotalMercadoria.ToString("n2"),
                                   ValorMercadoriaKG = repInformacaoCarga.ObterPesoTotal(obj.Codigo) > 0 ? (obj.ValorTotalMercadoria / repInformacaoCarga.ObterPesoTotal(obj.Codigo)).ToString("n2") : "0,00",
                                   PesoArredondado = repInformacaoCarga.ObterPesoKg(obj.Codigo) > 0 ? repInformacaoCarga.ObterPesoKg(obj.Codigo).ToString("n0") : repInformacaoCarga.ObterPesoTotal(obj.Codigo).ToString("n0"),
                                   Peso = repInformacaoCarga.ObterPesoKg(obj.Codigo) > 0 ? repInformacaoCarga.ObterPesoKg(obj.Codigo).ToString("n2") : repInformacaoCarga.ObterPesoTotal(obj.Codigo).ToString("n2"),
                                   Especie = repInformacaoCarga.ObterPesoKg(obj.Codigo) > 0 ? "KG" : this.PegaEspeciaPorUnidade(obj.Codigo, unitOfWork)
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string PegaEspeciaPorUnidade(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            string especie;
            switch (repInformacaoCarga.ObterUnidade(codigo))
            {
                case "00": especie = "M3"; break;
                case "01": especie = "KG"; break;
                case "02": especie = "TO"; break;
                case "03": especie = "UN"; break;
                case "04": especie = "LT"; break;
                default: especie = "KG"; break;
            }

            return especie;
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CIOTCidadesPedagio repCIOTCidadesPedagio = new Repositorio.CIOTCidadesPedagio(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                List<Dominio.Entidades.CTeCIOTSigaFacil> documentos = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);
                List<Dominio.Entidades.CIOTCidadesPedagio> cidadesPedagio = repCIOTCidadesPedagio.BuscaPorCIOT(ciot.Codigo);

                var retorno = new
                {
                    ciot.CategoriaTransportador,
                    CodigoNaturezaCarga = ciot.NaturezaCarga != null ? ciot.NaturezaCarga.Codigo : 0,
                    ciot.Codigo,
                    ciot.CodigoRetorno,
                    DataEmissao = ciot.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                    DataInicioViagem = ciot.DataInicioViagem.ToString("dd/MM/yyyy"),
                    DataTerminoViagem = ciot.DataTerminoViagem.ToString("dd/MM/yyyy"),
                    UFDestino = ciot.Destino?.Estado.Sigla,
                    CodigoDestino = ciot.Destino?.Codigo,
                    ciot.DocumentosObrigatorios,
                    ciot.MensagemRetorno,
                    CodigoMotorista = ciot.Motorista != null ? ciot.Motorista.Codigo : 0,
                    DescricaoMotorista = ciot.Motorista != null ? ciot.Motorista.CPF_Formatado + " - " + ciot.Motorista.Nome : string.Empty,
                    MotivoCancelamento = !string.IsNullOrWhiteSpace(ciot.MotivoCancelamento) ? ciot.MotivoCancelamento : string.Empty,
                    ciot.NSU,
                    ciot.Numero,
                    ciot.NumeroCartaoMotorista,
                    ciot.NumeroCIOT,
                    CodigoVerificadorCIOT = !string.IsNullOrWhiteSpace(ciot.CodigoVerificadorCIOT) ? ciot.CodigoVerificadorCIOT : string.Empty,
                    ciot.NumeroContrato,
                    UFOrigem = ciot.Origem?.Estado.Sigla,
                    CodigoOrigem = ciot.Origem?.Codigo,
                    ciot.RegraAdiantamento,
                    ciot.RegraQuitacao,
                    ValorEstimado = ciot.ValorEstimado.ToString("n2"),
                    ValorAdiantamentoAbertura = ciot.ValorAdiantamentoAbertura.ToString("n2"),
                    ciot.PossuiAdiantamentoAbertura,
                    ciot.TipoViagem,
                    Status = ciot.Status.ToString("G"),
                    CPFCNPJTransportador = ciot.Transportador.CPF_CNPJ_Formatado,
                    DescricaoTransportador = ciot.Transportador.CPF_CNPJ_Formatado + " - " + ciot.Transportador.Nome,
                    CodigoVeiculo = ciot.Veiculo.Codigo,
                    PlacaVeiculo = ciot.Veiculo.Placa,
                    TipoFavorecido = ciot.TipoFavorecido,
                    PedagioIdaVolta = ciot.PedagioIdaVolta,
                    CidadesPedagio = (from obj in cidadesPedagio
                                      select new Dominio.ObjetosDeValor.CIOTCidadePedagio()
                                      {
                                          Codigo = obj.Localidade.Codigo,
                                          Cidade = obj.Localidade.Descricao + " - " + obj.Localidade.Estado.Sigla,
                                          Excluir = false
                                      }).ToList(),
                    Documentos = (from obj in documentos
                                  select new Dominio.ObjetosDeValor.DocumentoSigaFacil()
                                  {
                                      Codigo = obj.Codigo,
                                      CodigoCTe = obj.CTe.Codigo,
                                      EspecieMercadoria = obj.EspecieMercadoria,
                                      Excluir = false,
                                      ExigePesoChegada = (int)obj.ExigePesoChegada,
                                      NumeroCTe = obj.CTe.Numero,
                                      PesoBruto = obj.PesoBruto,
                                      PesoLotacao = obj.PesoLotacao,
                                      QuantidadeMercadoria = obj.QuantidadeMercadoria,
                                      RecalculoFrete = (int)obj.RecalculoFrete,
                                      SerieCTe = obj.CTe.Serie.Numero,
                                      TipoPeso = obj.TipoPeso,
                                      TipoQuebra = (int)obj.TipoQuebra,
                                      TipoTolerancia = (int)obj.TipoTolerancia,
                                      Tolerancia = obj.PercentualTolerancia,
                                      ToleranciaSuperior = obj.PercentualToleranciaSuperior,
                                      ValorAdiantamento = obj.ValorAdiantamento,
                                      ValorCartaoPedagio = obj.ValorCartaoPedagio,
                                      ValorFrete = obj.ValorFrete,
                                      ValorINSS = obj.ValorINSS,
                                      ValorIRRF = obj.ValorIRRF,
                                      ValorMercadoriaKG = obj.ValorMercadoriaKG,
                                      ValorOutrosDescontos = obj.ValorOutrosDescontos,
                                      ValorPedagio = obj.ValorPedagio,
                                      ValorAbastecimento = obj.ValorAbastecimento,
                                      ValorSeguro = obj.ValorSeguro,
                                      ValorSENAT = obj.ValorSENAT,
                                      ValorSEST = obj.ValorSEST,
                                      ValorTarifaEmissaoCartao = obj.ValorTarifaEmissaoCartao,
                                      ValorTarifaFrete = obj.ValorTarifaFrete,
                                      ValorTotalMercadoria = obj.ValorTotalMercadoria
                                  }).ToList(),
                    Encerramento = new
                    {
                        ciot.ValorFrete,
                        ciot.PesoBruto,
                        ciot.ValorTarifaFrete,
                        ciot.ValorTotalMercadoria,
                        ciot.ValorMercadoriaKG,
                        ciot.TipoQuebra,
                        ciot.TipoTolerancia,
                        ciot.PercentualTolerancia,
                        ciot.ValorAdiantamento,
                        ciot.ValorSeguro,
                        ciot.ValorPedagio,
                        ciot.ValorIRRF,
                        ciot.ValorINSS,
                        ciot.ValorSEST,
                        ciot.ValorSENAT,
                        ciot.ValorAbastecimento,
                        ciot.ValorOperacao,
                        ciot.ValorQuitacao,
                        ciot.ValorBruto
                    },
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesAbertura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CIOTCidadesPedagio repCIOTCidadesPedagio = new Repositorio.CIOTCidadesPedagio(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                List<Dominio.Entidades.CTeCIOTSigaFacil> documentos = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

                var retorno = new
                {
                    CodigoNaturezaCarga = ciot.NaturezaCarga != null ? ciot.NaturezaCarga.Codigo : 0,
                    ciot.Codigo,
                    DataEmissao = ciot.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                    DataInicioViagem = ciot.DataInicioViagem.ToString("dd/MM/yyyy"),
                    DataTerminoViagem = ciot.DataTerminoViagem.ToString("dd/MM/yyyy"),
                    CodigoMotorista = ciot.Motorista != null ? ciot.Motorista.Codigo : 0,
                    DescricaoMotorista = ciot.Motorista != null ? ciot.Motorista.CPF_Formatado + " - " + ciot.Motorista.Nome : string.Empty,
                    ciot.Numero,
                    ciot.NumeroCIOT,
                    CodigoVerificadorCIOT = !string.IsNullOrWhiteSpace(ciot.CodigoVerificadorCIOT) ? ciot.CodigoVerificadorCIOT : string.Empty,
                    CPFCNPJTransportador = ciot.Transportador.CPF_CNPJ_Formatado,
                    DescricaoTransportador = ciot.Transportador.CPF_CNPJ_Formatado + " - " + ciot.Transportador.Nome,
                    CodigoVeiculo = ciot.Veiculo.Codigo,
                    PlacaVeiculo = ciot.Veiculo.Placa,
                    Status = ciot.Status.ToString("G"),
                    MotivoCancelamento = !string.IsNullOrWhiteSpace(ciot.MotivoCancelamento) ? ciot.MotivoCancelamento : string.Empty,
                    Encerramento = new
                    {
                        ciot.ValorFrete,
                        ciot.PesoBruto,
                        ciot.ValorTarifaFrete,
                        ciot.ValorTotalMercadoria,
                        ciot.ValorMercadoriaKG,
                        ciot.TipoQuebra,
                        ciot.TipoTolerancia,
                        ciot.PercentualTolerancia,
                        ciot.ValorAdiantamento,
                        ciot.ValorSeguro,
                        ciot.ValorPedagio,
                        ciot.ValorIRRF,
                        ciot.ValorINSS,
                        ciot.ValorSEST,
                        ciot.ValorSENAT,
                        ciot.ValorOperacao,
                        ciot.ValorQuitacao,
                        ciot.ValorAbastecimento,
                        ciot.ValorBruto
                    },
                    Documentos = (from obj in documentos
                                  select new Dominio.ObjetosDeValor.DocumentoSigaFacil()
                                  {
                                      Codigo = obj.Codigo,
                                      CodigoCTe = obj.CTe.Codigo,
                                      NumeroCTe = obj.CTe.Numero,
                                      SerieCTe = obj.CTe.Serie.Numero,
                                      QuantidadeMercadoria = obj.QuantidadeMercadoria,
                                      EspecieMercadoria = obj.EspecieMercadoria,
                                      PesoBruto = obj.PesoBruto,
                                      ValorFrete = obj.ValorFrete,
                                      Excluir = false,
                                  }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadContratoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão de acesso negada!");

                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);
                string arquivoRelatorio = "ContratoTransporteSigaFacil";

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado &&
                    ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    return Json<bool>(false, false, "O status do CIOT não permite a emissão do contrato de transporte.");

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura)
                    arquivoRelatorio = "ContratoTransporteEFrete";
                else if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura || ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad)
                    arquivoRelatorio = "ContratoTransportePancard";

                List<Dominio.ObjetosDeValor.Relatorios.ContratoTransporteRodoviario> contrato = repCIOT.RelatorioContratoTransporte(codigoCIOT);
                List<Dominio.ObjetosDeValor.Relatorios.DocumentoContratoTransporteRodoviario> documentos = repCTeCIOT.RelatorioContratoTransporte(codigoCIOT);

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CIOT", contrato));
                dataSources.Add(new ReportDataSource("Documentos", documentos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + arquivoRelatorio + ".rdlc", "PDF", null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "ContratoTransporte." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o contrato de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRecibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCIOT = 0;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);
                string arquivoRelatorio = "ReciboPagamentoAutonomo";

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unitOfWork);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);
                Repositorio.MotoristaCTE repMotorista = new Repositorio.MotoristaCTE(unitOfWork);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                List<Dominio.Entidades.CTeCIOTSigaFacil> documentos = repCTeCIOT.BuscarCTesPorCIOT(codigoCIOT);

                if (documentos.Count == 0)
                    return Json<bool>(false, false, "CT-es não encontrados. Atualize a página e tente novamente.");

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura)
                    arquivoRelatorio = "ReciboPagamentoAutonomoEFrete";
                else if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura)
                    arquivoRelatorio = "ReciboPagamentoAutonomoPancard";

                Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista
                {
                    CEPEmpresa = this.EmpresaUsuario.CEP,
                    CidadeEmpresa = this.EmpresaUsuario.Localidade.Descricao,
                    CNPJEmpresa = this.EmpresaUsuario.CNPJ,
                    ComplementoEmpresa = this.EmpresaUsuario.Complemento,
                    EnderecoEmpresa = this.EmpresaUsuario.Endereco,
                    FoneEmpresa = this.EmpresaUsuario.Telefone,
                    IEEmpresa = this.EmpresaUsuario.InscricaoEstadual,
                    NomeEmpresa = this.EmpresaUsuario.RazaoSocial,
                    NumeroEmpresa = this.EmpresaUsuario.Numero,
                    UFEmpresa = this.EmpresaUsuario.Localidade.Estado.Sigla,

                    CidadeMotorista = ciot.Motorista.Localidade.Descricao,
                    CNHMotorista = ciot.Motorista.NumeroHabilitacao,
                    CPFMotorista = ciot.Motorista.CPF,
                    RGMotorista = ciot.Motorista.RG,
                    DataNascimentoMotorista = ciot.Motorista.DataNascimento.HasValue ? ciot.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    NomeMotorista = ciot.Motorista.Nome,
                    UFMotorista = ciot.Motorista.Localidade.Estado.Sigla,
                    EnderecoMotorista = ciot.Motorista.Endereco,
                    PisPasep = ciot.Motorista.PIS,

                    CidadeInicioPrestacao = ciot.Origem?.Descricao,
                    UFInicioPrestacao = ciot.Origem?.Estado.Sigla,
                    CidadeTerminoPrestacao = ciot.Destino?.Descricao,
                    UFTerminoPrestacao = ciot.Destino?.Estado.Sigla,
                    DataEmissaoCTe = ciot.DataEmissao,
                    NumeroCTe = ciot.Numero.ToString(),
                    SerieCTe = ""
                };

                if (
                    this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFreteAbertura ||
                    this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura ||
                    this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad
                    )
                {
                    recibo.ValorFrete = ciot.ValorFrete;
                    recibo.ValorINSS = ciot.ValorINSS;
                    recibo.ValorIR = ciot.ValorIRRF;
                    recibo.ValorSESTSENAT = ciot.ValorSEST + ciot.ValorSENAT;
                    recibo.ValorAdiantamento = ciot.ValorAdiantamento;
                    recibo.ValorSeguro = ciot.ValorSeguro;
                    recibo.ValorPedagio = ciot.ValorPedagio;
                    recibo.ValorOperacao = ciot.ValorOperacao;
                    recibo.ValorQuitacao = ciot.ValorQuitacao;
                    recibo.ValorOperacaoDescricao = Utilidades.Conversor.DecimalToWords(recibo.ValorOperacao);
                    recibo.ValorQuitacaoDescricao = Utilidades.Conversor.DecimalToWords(recibo.ValorQuitacao);
                }
                else
                {
                    recibo.ValorFrete = (documentos).Sum(o => o.ValorFrete); //+ (documentos).Sum(o => o.ValorINSS) + (documentos).Sum(o => o.ValorIRRF) + (documentos).Sum(o => o.ValorSEST) + (documentos).Sum(o => o.ValorSENAT);
                    recibo.ValorINSS = (documentos).Sum(o => o.ValorINSS);
                    recibo.ValorIR = (documentos).Sum(o => o.ValorIRRF);
                    recibo.ValorSESTSENAT = (documentos).Sum(o => o.ValorSEST) + (documentos).Sum(o => o.ValorSENAT);
                    recibo.ValorAdiantamento = 0;
                    recibo.SaldoAPagar = (documentos).Sum(o => o.ValorFrete) - recibo.ValorINSS - recibo.ValorIR - recibo.ValorSESTSENAT;
                    recibo.SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(recibo.SaldoAPagar);
                }

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura)
                {
                    recibo.SaldoAPagar = ciot.ValorBruto;
                    recibo.ValorLiquido = ciot.ValorFrete - (ciot.ValorIRRF + ciot.ValorINSS + ciot.ValorSEST + ciot.ValorSENAT) - ciot.ValorAdiantamento;
                    recibo.SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(recibo.ValorLiquido + ciot.ValorAdiantamento);
                }

                if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.TruckPad)
                {
                    recibo.SaldoAPagar = ciot.ValorBruto;
                    recibo.ValorLiquido = ciot.ValorFrete - (ciot.ValorIRRF + ciot.ValorINSS + ciot.ValorSEST + ciot.ValorSENAT) - ciot.ValorAdiantamento;
                    recibo.SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(recibo.ValorLiquido + ciot.ValorAdiantamento);
                }

                recibo.CEPProprietarioVeiculo = ciot.Transportador.CEP;
                recibo.CidadeProprietarioVeiculo = ciot.Transportador.Localidade.Descricao;
                recibo.EnderecoProprietarioVeiculo = ciot.Transportador.Endereco;
                recibo.NomeProprietarioVeiculo = ciot.Transportador.CPF_CNPJ_Formatado + " " + ciot.Transportador.Nome;
                recibo.UFProprietarioVeiculo = ciot.Transportador.Localidade.Estado.Sigla;

                recibo.Observacao = "Referente a CIOT " + (string.IsNullOrWhiteSpace(ciot.CodigoVerificadorCIOT) ? ciot.NumeroCIOT : ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT);

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Recibo", new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista>() { recibo }));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + arquivoRelatorio + ".rdlc", "PDF", null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Recibo." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o recibo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCIOT;
                int.TryParse(Request.Params["CodigoCIOT"], out codigoCIOT);

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    return Json<bool>(false, false, "CIOT não encontrado. Atualize a página e tente novamente.");

                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    return Json<bool>(false, false, "O status do CIOT não permite a emissão dos CT-es.");

                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                List<Dominio.Entidades.CTeCIOTSigaFacil> documentos = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                foreach (Dominio.Entidades.CTeCIOTSigaFacil documento in documentos)
                {
                    if (documento.CTe.Status == "R" || documento.CTe.Status == "S")
                    {
                        unidadeDeTrabalho.Start();

                        documento.CTe.CIOT = Utilidades.String.Left(ciot.NumeroCIOT, 12);
                        documento.CTe.ObservacoesGerais = String.Concat("CIOT ", Utilidades.String.Left(ciot.NumeroCIOT, 12), "  ", documento.CTe.ObservacoesGerais);

                        repCTe.Atualizar(documento.CTe);

                        if (svcCTe.Emitir(documento.CTe.Codigo, documento.CTe.Empresa.Codigo, unidadeDeTrabalho))
                        {
                            unidadeDeTrabalho.CommitChanges();

                            if (documento.CTe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                FilaConsultaCTe.GetInstance().QueueItem(1, documento.CTe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();
                        }
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir os CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ImportarCTeCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "Empresa sem configuração para emissão de CIOT.");

                //if (this.EmpresaUsuario.Configuracao.TipoEmpresaCIOT == Dominio.Enumeradores.TipoEmpresaCIOT.Embarcador)
                //    return Json<bool>(false, false, "Importação de XML disponível apenas para CIOT do tipo Embarcador.");

                if (Request.Files.Count > 0)
                {
                    if (Request.Files.Count > 1)
                        return Json<bool>(false, false, "Somente é possível importar 1 CTe.");

                    HttpPostedFileBase file = Request.Files[0];
                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        string path = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoUploadXMLNotasFiscais"], this.EmpresaUsuario.Codigo.ToString());

                        path = Utilidades.IO.FileStorageService.Storage.Combine(path, "CTe Anterior");

                        Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(path, file.FileName), file.InputStream);

                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        Servicos.CIOT svcCIOT = new Servicos.CIOT(unitOfWork);

                        Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                        Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        unitOfWork.Start();

                        object retorno = svcCTe.GerarCTeAnterior(file.InputStream, this.EmpresaUsuario.Codigo, string.Empty, string.Empty, unitOfWork);// svcCTe.GerarCTeAnteriorCIOT(file.InputStream, this.EmpresaUsuario.Codigo, unitOfWork);

                        unitOfWork.CommitChanges();

                        if (retorno != null)
                        {
                            return Json<bool>(true, true);
                            //Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;

                            //double cnpjTransportador = 0;
                            //double.TryParse(cte.ObservacoesAvancadas, out cnpjTransportador);

                            //if (cnpjTransportador == 0)
                            //    double.TryParse(Utilidades.String.OnlyNumbers(cte.Empresa.CNPJ), out cnpjTransportador);

                            //Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjTransportador);
                            //if (cliente == null)
                            //    return Json<bool>(false, false, "Transportador " + cte.ObservacoesAvancadas + " não está cadastrado como cliente.");

                            //unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                            //object retornoCiot = svcCIOT.GerarCIOTPorCTe(cte, cliente.CPF_CNPJ, unitOfWork);

                            //unitOfWork.CommitChanges();

                            //if (retornoCiot != null)
                            //{
                            //    Dominio.Entidades.CIOTSigaFacil ciot = (Dominio.Entidades.CIOTSigaFacil)retornoCiot;

                            //    return Json(new { ciot.Codigo }, true);
                            //}
                            //else
                            //{
                            //    return Json<bool>(false, false, "CIOT não foi gerado.");
                            //}
                        }
                        else
                        {
                            return Json<bool>(false, false, "Conhecimento de transporte não importado.");
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, string.Concat("A extensão do arquivo '", file.FileName, "' é inválida. Somente a extensão XML é aceita."));
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha importar/gerar CTe para o CIOT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarInformacoesPamCard()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Na primeira vez que esse metodo e executado, o CIOT eh criado e setado como salvo
                // Na segunda vez, ele eh encerrado
                if (!ValidaConfiguracoes(out string erro))
                    return Json<bool>(false, false, erro);

                // Repositório
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigo);
                if (ciot == null)
                    return Json<bool>(false, false, "Nenhum registro encontrado.");
                if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    return Json<bool>(false, false, "Status do CIOT não permite essa operação.");

                PreencherEntidade(ref ciot, unidadeDeTrabalho);
                PreencherEntidadeEncerramento(ref ciot, unidadeDeTrabalho);

                if (!ValidaEntidade(ciot, out erro))
                    return Json<bool>(false, false, erro);

                unidadeDeTrabalho.Start();

                repCIOT.Atualizar(ciot);

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);
                this.SalvarDocumentosDoCIOT(ciot, unidadeDeTrabalho);
                this.SalvarCidadesPedagio(ciot, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                repCIOT.Atualizar(ciot);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarAberturaPamCard()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Na primeira vez que esse metodo e executado, o CIOT eh criado e setado como salvo
                // Na segunda vez, ele eh encerrado
                if (!ValidaConfiguracoes(out string erro))
                    return Json<bool>(false, false, erro);

                // Repositório
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                bool atualizandoCIOT = codigo > 0;
                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                {
                    ciot = new Dominio.Entidades.CIOTSigaFacil();
                    ciot.DataEmissao = DateTime.Now;
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;
                    ciot.TipoIntegradora = this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value;
                    ciot.Numero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                    ciot.TipoPagamento = this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT.Value;
                    ciot.Empresa = this.EmpresaUsuario;
                }
                else
                {
                    if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                        return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                    if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                        return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");
                }

                PreencherEntidade(ref ciot, unidadeDeTrabalho);
                PreencherEntidadeEncerramento(ref ciot, unidadeDeTrabalho);

                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;

                if (!ValidaEntidade(ciot, out erro))
                    return Json<bool>(false, false, erro);

                unidadeDeTrabalho.Start();

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                    ciot.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                if (ciot.Codigo > 0)
                    repCIOT.Atualizar(ciot);
                else
                    repCIOT.Inserir(ciot);

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);
                this.SalvarDocumentosDoCIOT(ciot, unidadeDeTrabalho);
                this.SalvarCidadesPedagio(ciot, unidadeDeTrabalho);

                //repEmpresa.Atualizar(this.EmpresaUsuario);

                unidadeDeTrabalho.CommitChanges();

                if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente || ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                {
                    try
                    {
                        // ABRIR CIOT
                        this.EmitirCIOT(ciot.Codigo, unidadeDeTrabalho);
                    }
                    catch (Exception ex)
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                        ciot.MensagemRetorno = "Não foi possível comunicar com a integradora.";

                        Servicos.Log.TratarErro("Falha ao Abrir CIOT(" + ciot.Codigo.ToString() + ") PamCard");
                        Servicos.Log.TratarErro(ex);
                    }

                    if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    {
                        // Quando abrir um CIOT, deve atualizar o valor do CIOT no cadastro do veiculo
                        this.AtualizarCIOTVeiculo(ciot, ciot.NumeroCIOT, unidadeDeTrabalho);
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto;
                    }
                }
                else
                {
                    // ENCERRAR O CIOT
                    this.SalvarInformacoesAbertura();

                    // Chama o WS para atualizar encerrar o ciot
                    this.SalvarCIOT(ciot.Codigo, unidadeDeTrabalho);

                    if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado || ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento)
                    {
                        try
                        {
                            this.EncerrarCIOT(ciot.Codigo, unidadeDeTrabalho);
                        }
                        catch (Exception ex)
                        {
                            ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;
                            ciot.MensagemRetorno = "Não foi possível comunicar com a integradora.";

                            Servicos.Log.TratarErro("Falha ao Encerrar CIOT(" + ciot.Codigo.ToString() + ") PamCard");
                            Servicos.Log.TratarErro(ex);
                        }

                        if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                        {
                            // Remover CIOT do veiculo
                            ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado;
                            this.AtualizarCIOTVeiculo(ciot, string.Empty, unidadeDeTrabalho);

                        }
                    }

                }
                repCIOT.Atualizar(ciot);
                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (!ValidaConfiguracoes(out string erro))
                    return Json<bool>(false, false, erro);

                // Repositório
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigo);

                if (ciot == null)
                {
                    ciot = new Dominio.Entidades.CIOTSigaFacil();
                }
                else
                {
                    if (ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente && ciot.Status != Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                        return Json<bool>(false, false, "O status do CIOT não permite a alteração do mesmo.");

                    if (ciot.TipoIntegradora != this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value)
                        return Json<bool>(false, false, "A integradora do CIOT selecionado para edição difere da configurada na empresa.");
                }

                PreencherEntidade(ref ciot, unidadeDeTrabalho);

                ciot.DataEmissao = DateTime.Now;
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;
                ciot.TipoIntegradora = this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT.Value;

                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, ciot.Transportador.CPF_CNPJ);
                if (dadosCliente != null && dadosCliente.NumeroCartao != null && dadosCliente.NumeroCartao != "")
                    ciot.NumeroCartaoTransportador = dadosCliente.NumeroCartao;

                if (!ValidaEntidade(ciot, out erro))
                    return Json<bool>(false, false, erro);

                unidadeDeTrabalho.Start();

                if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                    ciot.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                if (ciot.Codigo > 0)
                    repCIOT.Atualizar(ciot);
                else
                {
                    ciot.Numero = repCIOT.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                    repCIOT.Inserir(ciot);
                }

                this.SalvarCliente(ciot.Motorista, unidadeDeTrabalho);
                this.SalvarDocumentosDoCIOT(ciot, unidadeDeTrabalho);
                this.SalvarCidadesPedagio(ciot, unidadeDeTrabalho);

                repEmpresa.Atualizar(this.EmpresaUsuario);

                unidadeDeTrabalho.CommitChanges();

                this.EmitirCIOT(ciot.Codigo, unidadeDeTrabalho);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o CIOT.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
        private bool ValidaConfiguracoes(out string erro)
        {
            erro = string.Empty;

            int.TryParse(Request.Params["Codigo"], out int codigo);

            if (codigo > 0 && (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A"))
                erro = "Permissão de alteração negada!";
            if (codigo == 0 && (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A"))
                erro = "Permissão de inclusão negada!";

            if (this.EmpresaUsuario.Configuracao == null)
                erro = "A empresa não está configurada para a emissão de CIOT.";

            if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == null || this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == null)
                erro = "Acesse as configurações da empresa e configure a Integradora e o Tipo de Pagamento.";

            if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
            {
                if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoContratanteSigaFacil) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.ChaveCriptograficaSigaFacil))
                    erro = "Acesse as configurações da empresa e configure o Código da Contratante e a Chave Criptográfica para utilizar a emissão de CIOT da Siga Fácil.";
            }
            else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete)
            {
                if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoIntegradorEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioEFrete) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaEFrete))
                    erro = "Acesse as configurações da empresa e configure o Código do Integrador, Usuário e Senha para utilizar a emissão de CIOT da e-Frete.";
            }

            return string.IsNullOrEmpty(erro);
        }

        private bool ValidaEntidade(Dominio.Entidades.CIOTSigaFacil ciot, out string erro)
        {
            erro = string.Empty;

            if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && (string.IsNullOrWhiteSpace(ciot.Motorista.NumeroCartao) || ciot.Motorista.NumeroCartao.Length != 16))
                erro = "O número do cartão do motorista é inválido (deve possuir 16 dígitos).";

            if (ciot.NumeroCartaoTransportador != null && ciot.NumeroCartaoTransportador != "")
            {
                if (this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao && ciot.NumeroCartaoTransportador.Length != 16)
                    erro = "O número do cartão do transportador é inválido (deve possuir 16 dígitos).";
            }

            if (ciot.Veiculo == null || (ciot.Veiculo.Tipo == "T" && ciot.Veiculo.RNTRC <= 0))
                erro = "O RNTRC do veículo é inválido.";

            if (ciot.NaturezaCarga == null)
                erro = "Natureza da carga é obrigatório.";

            else if (this.EmpresaUsuario.Configuracao.TipoIntegradoraCIOT == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.PamCardAbertura)
            {
                if (string.IsNullOrWhiteSpace(ciot.Motorista.Endereco))
                    erro = "Endereço no cadastro do Motorista é obrigatório.";

                if (string.IsNullOrWhiteSpace(ciot.Motorista.Bairro))
                    erro = "Bairro no cadastro do Motorista é obrigatório.";

                if (string.IsNullOrWhiteSpace(ciot.Motorista.CEP))
                    erro = "CEP no cadastro do Motorista é obrigatório.";

                if (string.IsNullOrWhiteSpace(ciot.Motorista.Telefone))
                    erro = "Telefone no cadastro do Motorista é obrigatório.";

                if (string.IsNullOrWhiteSpace(ciot.Motorista.RG))
                    erro = "RG no cadastro do Motorista é obrigatório.";

                if (ciot.ValorEstimado <= 0)
                    erro = "Valor Estimado é obrigatório.";
            }

            return string.IsNullOrEmpty(erro);
        }

        private void PreencherEntidade(ref Dominio.Entidades.CIOTSigaFacil ciot, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            // Repositório
            Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            // Converte dados
            int.TryParse(Request.Params["CodigoNaturezaCarga"], out int codigoNaturezaCarga);
            int.TryParse(Request.Params["CodigoOrigem"], out int codigoOrigem);
            int.TryParse(Request.Params["CodigoDestino"], out int codigoDestino);
            int.TryParse(Request.Params["CodigoMotorista"], out int codigoMotorista);
            int.TryParse(Request.Params["CodigoVeiculo"], out int codigoVeiculo);

            decimal.TryParse(Request.Params["ValorEstimado"], out decimal valorEstimado);
            decimal.TryParse(Request.Params["ValorAdiantamentoAbertura"], out decimal valorAdiantamentoAbertura);

            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJTransportador"]), out double cpfCnpjTransportador);

            Enum.TryParse<Dominio.Enumeradores.CategoriaTransportadorANTT>(Request.Params["CategoriaTransportador"], out Dominio.Enumeradores.CategoriaTransportadorANTT categoriaTransportador);
            Enum.TryParse<Dominio.Enumeradores.DocumentosObrigatorios>(Request.Params["DocumentosObrigatorios"], out Dominio.Enumeradores.DocumentosObrigatorios documentosObrigatorios);
            Enum.TryParse<Dominio.Enumeradores.RegraQuitacaoAdiantamento>(Request.Params["RegraAdiantamento"], out Dominio.Enumeradores.RegraQuitacaoAdiantamento regraAdiantamento);
            Enum.TryParse<Dominio.Enumeradores.RegraQuitacaoQuitacao>(Request.Params["RegraQuitacao"], out Dominio.Enumeradores.RegraQuitacaoQuitacao regraQuitacao);
            Enum.TryParse<Dominio.Enumeradores.TipoViagem>(Request.Params["TipoViagem"], out Dominio.Enumeradores.TipoViagem tipoViagem);
            Enum.TryParse<Dominio.Enumeradores.TipoFavorecido>(Request.Params["TipoFavorecido"], out Dominio.Enumeradores.TipoFavorecido tipoFavorecido);
            Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["PedagioIdaVolta"], out Dominio.Enumeradores.OpcaoSimNao pedagioIdaVolta);

            DateTime.TryParseExact(Request.Params["DataInicioViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioViagem);
            DateTime.TryParseExact(Request.Params["DataTerminoViagem"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataTerminoViagem);

            Dominio.Entidades.NaturezaCargaANTT natureza = repNaturezaCargaANTT.BuscarPorCodigo(codigoNaturezaCarga);

            ciot.CategoriaTransportador = categoriaTransportador;
            ciot.NaturezaCarga = natureza;
            ciot.DataInicioViagem = dataInicioViagem;
            ciot.DataTerminoViagem = dataTerminoViagem;
            ciot.Destino = repLocalidade.BuscarPorCodigo(codigoDestino);
            ciot.DocumentosObrigatorios = documentosObrigatorios;
            ciot.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(0, codigoMotorista);
            ciot.NumeroCartaoMotorista = ciot.Motorista.NumeroCartao;
            ciot.Origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
            ciot.RegraAdiantamento = regraAdiantamento;
            ciot.RegraQuitacao = regraQuitacao;
            ciot.TipoViagem = tipoViagem;
            ciot.ValorEstimado = valorEstimado;
            ciot.ValorAdiantamentoAbertura = valorAdiantamentoAbertura;
            ciot.PossuiAdiantamentoAbertura = valorAdiantamentoAbertura > 0;
            if (ciot.PossuiAdiantamentoAbertura)
                ciot.ValorAdiantamento = valorAdiantamentoAbertura;
            ciot.Transportador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTransportador);
            ciot.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
            ciot.TipoPagamento = this.EmpresaUsuario.Configuracao.TipoPagamentoCIOT.Value;
            ciot.TipoFavorecido = tipoFavorecido;
            ciot.PedagioIdaVolta = pedagioIdaVolta;
            ciot.Empresa = this.EmpresaUsuario;
        }

        private void PreencherEntidadeEncerramento(ref Dominio.Entidades.CIOTSigaFacil ciot, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            // Converte dados
            Enum.TryParse(Request.Params["TipoQuebra"], out Dominio.Enumeradores.TipoQuebra tipoQuebra);
            Enum.TryParse(Request.Params["TipoTolerancia"], out Dominio.Enumeradores.TipoTolerancia tipoTolerancia);
            decimal.TryParse(Request.Params["ValorFrete"], out decimal valorFrete);
            decimal.TryParse(Request.Params["ValorTarifa"], out decimal valorTarifa);
            decimal.TryParse(Request.Params["PesoTotal"], out decimal pesoTotal);
            decimal.TryParse(Request.Params["ValorMercadoria"], out decimal valorMercadoria);
            decimal.TryParse(Request.Params["ValorMercadoriaPorKG"], out decimal valorMercadoriaPorKG);
            decimal.TryParse(Request.Params["PercentualTolerancia"], out decimal percentualTolerancia);
            decimal.TryParse(Request.Params["ValorAdiantamento"], out decimal valorAdiantamento);
            decimal.TryParse(Request.Params["ValorSeguro"], out decimal valorSeguro);
            decimal.TryParse(Request.Params["ValorPedagio"], out decimal valorPedagio);
            decimal.TryParse(Request.Params["ValorIRRF"], out decimal valorIRRF);
            decimal.TryParse(Request.Params["ValorINSS"], out decimal valorINSS);
            decimal.TryParse(Request.Params["ValorSEST"], out decimal valorSEST);
            decimal.TryParse(Request.Params["ValorSENAT"], out decimal valorSENAT);
            decimal.TryParse(Request.Params["TotalOperacao"], out decimal totalOperacao);
            decimal.TryParse(Request.Params["TotalQuitacao"], out decimal totalQuitacao);
            decimal.TryParse(Request.Params["ValorAbastecimento"], out decimal totalAbastecimento);
            decimal.TryParse(Request.Params["ValorBruto"], out decimal totalBruto);

            string codigoVerificadorCIOT = Request.Params["CodigoVerificadorCIOT"];

            // Atualiza informacoes
            ciot.ValorFrete = valorFrete;
            ciot.ValorTarifaFrete = valorTarifa;
            ciot.PesoBruto = pesoTotal;
            ciot.ValorTotalMercadoria = valorMercadoria;
            ciot.ValorMercadoriaKG = valorMercadoriaPorKG;
            ciot.TipoQuebra = tipoQuebra;
            ciot.TipoTolerancia = tipoTolerancia;
            ciot.PercentualTolerancia = percentualTolerancia;
            if (!ciot.PossuiAdiantamentoAbertura)
                ciot.ValorAdiantamento = valorAdiantamento;
            ciot.ValorSeguro = valorSeguro;
            ciot.ValorPedagio = valorPedagio;
            ciot.ValorIRRF = valorIRRF;
            ciot.ValorINSS = valorINSS;
            ciot.ValorSEST = valorSEST;
            ciot.ValorSENAT = valorSENAT;
            ciot.ValorAbastecimento = totalAbastecimento;
            ciot.ValorOperacao = totalOperacao;
            ciot.ValorQuitacao = totalQuitacao;
            ciot.ValorBruto = totalBruto;
            ciot.CodigoVerificadorCIOT = Utilidades.String.Left(codigoVerificadorCIOT, 4);
        }

        #region Métodos Privados
        private void SalvarCliente(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            double cpfCnpj = 0f;
            double.TryParse(Utilidades.String.OnlyNumbers(motorista.CPF), out cpfCnpj);

            if (cpfCnpj > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

                    cliente = new Dominio.Entidades.Cliente
                    {
                        Atividade = repAtividade.BuscarPorCodigo(7),
                        Bairro = motorista.Bairro,
                        CEP = motorista.CEP,
                        Complemento = motorista.Complemento,
                        CPF_CNPJ = cpfCnpj,
                        DataCadastro = DateTime.Now,
                        DataNascimento = motorista.DataNascimento,
                        Email = motorista.Email,
                        Endereco = motorista.Endereco,
                        EstadoRG = motorista.EstadoRG,
                        IE_RG = motorista.RG,
                        Localidade = motorista.Localidade,
                        Nome = motorista.Nome,
                        OrgaoEmissorRG = motorista.OrgaoEmissorRG,
                        Sexo = motorista.Sexo,
                        Telefone1 = motorista.Telefone,
                        Tipo = motorista.CPF.Length == 14 ? "J" : "F"
                    };

                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    cliente.Ativo = true;
                    repCliente.Inserir(cliente);
                }
            }
        }

        private void SalvarCidadesPedagio(Dominio.Entidades.CIOTSigaFacil ciot, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.CIOTCidadePedagio> cidadesPedagio = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.CIOTCidadePedagio>>(Request.Params["CidadesPedagio"]);

            if (cidadesPedagio != null)
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CIOTCidadesPedagio repCIOTCidadesPedagio = new Repositorio.CIOTCidadesPedagio(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                if (ciot.Codigo > 0)
                {
                    List<Dominio.Entidades.CIOTCidadesPedagio> listaCidadesPedagio = new List<Dominio.Entidades.CIOTCidadesPedagio>();
                    listaCidadesPedagio = repCIOTCidadesPedagio.BuscaPorCIOT(ciot.Codigo);

                    foreach (Dominio.Entidades.CIOTCidadesPedagio cidade in listaCidadesPedagio)
                        repCIOTCidadesPedagio.Deletar(cidade);
                }

                for (var i = 0; i < cidadesPedagio.Count; i++)
                {
                    if (!cidadesPedagio[i].Excluir)
                    {
                        Dominio.Entidades.CIOTCidadesPedagio cidadePedagio = new Dominio.Entidades.CIOTCidadesPedagio();

                        cidadePedagio.CIOT = ciot;
                        cidadePedagio.Localidade = repLocalidade.BuscarPorCodigo(cidadesPedagio[i].Codigo);

                        repCIOTCidadesPedagio.Inserir(cidadePedagio);
                    }
                }
            }
        }

        private void SalvarDocumentosDoCIOT(Dominio.Entidades.CIOTSigaFacil ciot, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.DocumentoSigaFacil> documentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DocumentoSigaFacil>>(Request.Params["Documentos"]);

            if (documentos != null)
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);

                for (var i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.CTeCIOTSigaFacil documento = repCTeCIOT.BuscarPorCodigo(documentos[i].Codigo);

                    if (!documentos[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.CTeCIOTSigaFacil();

                        string numeroNotaFiscal = repDocumentosCTe.BuscarNumeroNotaFiscal(documentos[i].CodigoCTe);
                        int.TryParse(numeroNotaFiscal, out int notaFiscal);

                        documento.CIOT = ciot;
                        documento.CTe = repCTe.BuscarPorCodigo(documentos[i].CodigoCTe);
                        documento.EspecieMercadoria = documentos[i].EspecieMercadoria;
                        documento.ExigePesoChegada = (Dominio.Enumeradores.ExigePesoChegada)documentos[i].ExigePesoChegada;
                        documento.NumeroNotaFiscal = notaFiscal;
                        documento.PercentualTolerancia = documentos[i].Tolerancia;
                        documento.PercentualToleranciaSuperior = documentos[i].ToleranciaSuperior;
                        documento.PesoBruto = documentos[i].PesoBruto;
                        documento.PesoLotacao = documentos[i].PesoLotacao;
                        documento.QuantidadeMercadoria = documentos[i].QuantidadeMercadoria;
                        documento.RecalculoFrete = (Dominio.Enumeradores.RecalculoFrete)documentos[i].RecalculoFrete;
                        documento.TipoPeso = documentos[i].TipoPeso;
                        documento.TipoQuebra = (Dominio.Enumeradores.TipoQuebra)documentos[i].TipoQuebra;
                        documento.TipoTolerancia = (Dominio.Enumeradores.TipoTolerancia)documentos[i].TipoTolerancia;
                        documento.ValorAdiantamento = documentos[i].ValorAdiantamento;
                        documento.ValorCartaoPedagio = documentos[i].ValorCartaoPedagio;
                        documento.ValorFrete = documentos[i].ValorFrete;
                        documento.ValorINSS = documentos[i].ValorINSS;
                        documento.ValorIRRF = documentos[i].ValorIRRF;
                        documento.ValorMercadoriaKG = documentos[i].ValorMercadoriaKG;
                        documento.ValorOutrosDescontos = documentos[i].ValorOutrosDescontos;
                        documento.ValorPedagio = documentos[i].ValorPedagio;
                        documento.ValorAbastecimento = documentos[i].ValorAbastecimento;
                        documento.ValorSeguro = documentos[i].ValorSeguro;
                        documento.ValorSENAT = documentos[i].ValorSENAT;
                        documento.ValorSEST = documentos[i].ValorSEST;
                        documento.ValorTarifaEmissaoCartao = documentos[i].ValorTarifaEmissaoCartao;
                        documento.ValorTarifaFrete = documentos[i].ValorTarifaFrete;
                        documento.ValorTotalMercadoria = documentos[i].ValorTotalMercadoria;

                        if (ciot.TipoIntegradora == Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.SigaFacil)
                            documento.NSU = this.EmpresaUsuario.Configuracao.ProximoNSUSigaFacil;

                        if (documento.Codigo > 0)
                        {
                            repCTeCIOT.Atualizar(documento);
                        }
                        else
                        {
                            repCTeCIOT.Inserir(documento);
                        }
                    }
                    else if (documento != null)
                    {
                        repCTeCIOT.Deletar(documento);
                    }
                }
            }
        }

        private void EmitirCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);

            svcCIOT.Emitir(codigoCIOT, unidadeDeTrabalho);
        }

        private void SalvarCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);

            svcCIOT.Salvar(codigoCIOT, unidadeDeTrabalho);
        }

        private void CancelarCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);

            svcCIOT.Cancelar(codigoCIOT, unidadeDeTrabalho);
        }

        private void EncerrarCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);

            svcCIOT.Encerrar(codigoCIOT, unidadeDeTrabalho);
        }

        private void AtualizarCIOTVeiculo(Dominio.Entidades.CIOTSigaFacil ciot, string numeroCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(ciot.Veiculo.Codigo);

                // So limpa o numero do CIOT quando o veiculo possui o numero do CIOT passado
                if ((string.IsNullOrWhiteSpace(numeroCIOT) && (ciot.Veiculo.CIOT == ciot.NumeroCIOT)) || !string.IsNullOrWhiteSpace(numeroCIOT))
                {
                    string[] log = new string[]
                    {
                        "--- ALTERACAO CIOT VEICULO ---",
                        "De: " + ciot.Veiculo.CIOT,
                        "Para: " + numeroCIOT,
                        "Veiculo: " + ciot.Veiculo.Placa,
                        "Empresa: " + ciot.Empresa.CNPJ_SemFormato
                    };
                    Servicos.Log.TratarErro(String.Join(Environment.NewLine, log), "CIOT");

                    veiculo.CIOT = numeroCIOT.Length > 12 ? Utilidades.String.Left(numeroCIOT, 12) : numeroCIOT;
                    repVeiculo.Atualizar(ciot.Veiculo);
                }
                else if (string.IsNullOrWhiteSpace(numeroCIOT))
                {
                    string[] log = new string[]
                    {
                        "--- TENTATIVA LIMPEZA CIOT VEICULO ---",
                        "CIOT: " + ciot.Veiculo.CIOT,
                        "Veiculo: " + ciot.Veiculo.Placa,
                        "Empresa: " + ciot.Empresa.CNPJ_SemFormato
                    };
                    Servicos.Log.TratarErro(String.Join(Environment.NewLine, log), "CIOT");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

    }
}

