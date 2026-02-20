using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ObjetoValorClientes
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public double CPFCNPJ { get; set; }
        public bool Excluir { get; set; }
    }

    public class LSTranslogIntegracaoController : ApiController
    {
        #region Configuracoes
        [AcceptVerbs("POST")]
        public ActionResult SalvarConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracaoIntegracaoLsTranslog = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracaoIntegracaoLsTranslog.BuscaPorEmpresa(this.EmpresaUsuario.Codigo);

                if (configuracao == null)
                    return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");

                string usuario = Request.Params["Usuario"] ?? string.Empty;
                string senha = Request.Params["Senha"] ?? string.Empty;

                configuracao.Login = usuario;
                configuracao.Senha = senha;
                SalvarClientesTranslog(configuracao, unitOfWork);

                repConfiguracaoIntegracaoLsTranslog.Atualizar(configuracao);

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDadosSalvos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracaoIntegracaoLsTranslog = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unitOfWork);

                unitOfWork.Start();
                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracaoIntegracaoLsTranslog.BuscaPorEmpresa(this.EmpresaUsuario.Codigo);
                if (configuracao == null)
                {
                    configuracao = new Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog()
                    {
                        Empresa = this.EmpresaUsuario,
                        Login = "",
                        Senha = ""
                    };
                    repConfiguracaoIntegracaoLsTranslog.Inserir(configuracao);
                }
                unitOfWork.CommitChanges();

                List<ObjetoValorClientes> clientes = new List<ObjetoValorClientes>();
                if (configuracao.Clientes != null)
                    clientes = (from o in configuracao.Clientes
                                select new ObjetoValorClientes
                                {
                                    Id = o.Codigo,
                                    Nome = o.Cliente.Nome,
                                    Tipo = o.Cliente.Tipo,
                                    CPFCNPJ = o.Cliente.CPF_CNPJ,
                                    Excluir = false
                                }).ToList();

                var retorno = new
                {
                    Usuario = configuracao.Login,
                    Senha = configuracao.Senha,
                    Clientes = clientes
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void SalvarClientesTranslog(Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ConfiguracaoIntegracaoLsTranslogClientes repConfiguracaoIntegracaoLsTranslogClientes = new Repositorio.ConfiguracaoIntegracaoLsTranslogClientes(unitOfWork);
            List<ObjetoValorClientes> clientes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoValorClientes>>(Request.Params["Clientes"]);

            foreach (ObjetoValorClientes objCliente in clientes)
            {
                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes cliente = repConfiguracaoIntegracaoLsTranslogClientes.BuscaPorCodigo(objCliente.Id);
                Dominio.Entidades.Cliente clienteSelecionado = repCliente.BuscarPorCPFCNPJ(objCliente.CPFCNPJ);

                if (cliente == null && clienteSelecionado != null)
                {
                    cliente = new Dominio.Entidades.ConfiguracaoIntegracaoLsTranslogClientes()
                    {
                        ConfiguracaoIntegracaoLsTranslog = configuracao,
                        Cliente = clienteSelecionado
                    };
                    repConfiguracaoIntegracaoLsTranslogClientes.Inserir(cliente);
                }
                else if (objCliente.Excluir)
                {
                    repConfiguracaoIntegracaoLsTranslogClientes.Deletar(cliente);
                }
            }
        }
        #endregion

        #region Integracoes
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoLsTranslog repIntegracaoLsTranslog = new Repositorio.IntegracaoLsTranslog(unitOfWork);

                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);
                int.TryParse(Request.Params["NumeroNota"], out int numeroNota);

                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                string identificador = Request.Params["Identificador"];

                Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog? tipoDocumento = null;
                Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusEnvio = null;
                Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusConsulta = null;

                if (Enum.TryParse(Request.Params["TipoDocumento"], out Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog tipoDocumentoAux))
                    tipoDocumento = tipoDocumentoAux;
                if (Enum.TryParse(Request.Params["StatusEnvio"], out Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog statusEnvioAux))
                    statusEnvio = statusEnvioAux;
                if (Enum.TryParse(Request.Params["StatusConsulta"], out Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog statusConsultaAux))
                    statusConsulta = statusConsultaAux;

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                List<Dominio.Entidades.IntegracaoLsTranslog> integracoes = repIntegracaoLsTranslog.Consultar(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, tipoDocumento, statusEnvio, statusConsulta, identificador, numeroNota, inicioRegistros, 50);
                int count = repIntegracaoLsTranslog.ContarConsulta(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, tipoDocumento, statusEnvio, statusConsulta, identificador, numeroNota);

                var lista = from obj in integracoes
                            select new
                            {
                                obj.Codigo,
                                TipoDocumento = obj.DescricaoTipoDocumento,
                                Numero = obj.TipoDocumento == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.CTe ? obj.CTe.Numero : obj.TipoDocumento == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFSe ? obj.NFSe.Numero : obj.NFe != null ? int.Parse(obj.NFe.Numero) : 0,
                                Data = obj.Data.ToString("dd/MM/yyyy"),
                                Remetente = obj.TipoDocumento == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.CTe ? obj.CTe.Remetente.Nome : obj.TipoDocumento == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFSe ? obj.NFSe.Tomador.Nome : obj.NFe != null ? obj.NFe.Emitente.Nome : string.Empty,
                                StatusEnvio = obj.StatusEnvio == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro ? obj.DescricaoStatusEnvio + ": " + obj.ObservacaoEnvio : obj.DescricaoStatusEnvio,
                                StatusConsulta = obj.StatusConsulta == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro ? obj.DescricaoStatusConsulta + ": " + obj.ObservacaoRetorno : obj.DescricaoStatusConsulta
                            };

                return Json(lista, true, null, new string[] { "Código", "Documento|10", "Número|10", "Data|10", "Remetente|30", "Envio|15", "Consulta|15" }, count);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoLsTranslog repIntegracaoLsTranslog = new Repositorio.IntegracaoLsTranslog(unitOfWork);
                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.IntegracaoLsTranslog integracao = repIntegracaoLsTranslog.BuscaPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (integracao == null)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");

                svcLsTranslog.EnviarDocumento(codigo, unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar a integracao.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoLsTranslog repIntegracaoLsTranslog = new Repositorio.IntegracaoLsTranslog(unitOfWork);
                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.IntegracaoLsTranslog integracao = repIntegracaoLsTranslog.BuscaPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (integracao == null)
                    return Json<bool>(false, false, "Não foi possível buscar os dados.");

                svcLsTranslog.ConsultarDocumento(codigo, unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar a integracao.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarLogs()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoLsTranslogLog repIntegracaoLsTranslogLog = new Repositorio.IntegracaoLsTranslogLog(unitOfWork);

                int.TryParse(Request.Params["Integracao"], out int integracao);
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                List<Dominio.Entidades.IntegracaoLsTranslogLog> integracoes = repIntegracaoLsTranslogLog.ConsultarLogs(this.EmpresaUsuario.Codigo, integracao, inicioRegistros, 50);
                int count = repIntegracaoLsTranslogLog.ContarConsultaLogs(this.EmpresaUsuario.Codigo, integracao);

                var lista = from obj in integracoes
                            select new
                            {
                                obj.Codigo,
                                Mensagem = obj.Mensagem ?? string.Empty,
                                Tipo = obj.DescricaoTipo,
                                DataHora = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                                obj.Identificador,
                                obj.NumeroNFe,
                                Status = obj.DescricaoStatus
                            };

                return Json(lista, true, null, new string[] { "Código", "Mensagem", "Tipo|10", "Data Hora|20", "Identificador|30", "Nota Fiscal|15", "Status|15" }, count);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoLsTranslogLog repIntegracaoLsTranslogLog = new Repositorio.IntegracaoLsTranslogLog(unitOfWork);

                int.TryParse(Request.Params["Log"], out int logIntegracao);
                string tipo = Request.Params["Tipo"] ?? string.Empty;

                if (tipo != "Envio" && tipo != "Retorno")
                    tipo = "Retorno";

                Dominio.Entidades.IntegracaoLsTranslogLog log = repIntegracaoLsTranslogLog.BuscaPorCodigo(this.EmpresaUsuario.Codigo, logIntegracao);
                if (log == null)
                    return Json<bool>(false, false, "Log não encontrado.");

                byte[] data = null;
                if (tipo == "Envio")
                    data = System.Text.Encoding.Default.GetBytes(log.Envio ?? string.Empty);
                else
                    data = System.Text.Encoding.Default.GetBytes(log.Retorno ?? string.Empty);

                if (data == null)
                    return Json<bool>(false, false, "Log não encontrado.");

                return Arquivo(data, "application/json", string.Concat(log.Identificador.ToString(), '-', tipo, ".json"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do log.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}