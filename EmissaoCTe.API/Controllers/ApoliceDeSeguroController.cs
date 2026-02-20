using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ApoliceDeSeguroController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("apolicesdeseguros.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string ramo = Request.Params["Ramo"];
                string numeroApolice = Request.Params["NumeroApolice"];
                string nomeSeguradora = Request.Params["NomeSeguradora"];
                string status = Request.Params["Status"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);

                Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);

                List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceSeguro.Consultar(this.EmpresaUsuario.Codigo, 0, nomeSeguradora, numeroApolice, ramo, status, cpfCnpjCliente, inicioRegistros, 50);
                int countApolices = repApoliceSeguro.ContarConsulta(this.EmpresaUsuario.Codigo, 0, nomeSeguradora, numeroApolice, ramo, status, cpfCnpjCliente);

                var retorno = (from obj in listaApolices
                              select new
                              {
                                  obj.Codigo,
                                  obj.NomeSeguradora,
                                  obj.NumeroApolice,
                                  Cliente = obj.Cliente != null ? obj.Cliente.CPF_CNPJ_Formatado + " " + obj.Cliente.Nome : string.Empty ,
                                  DataInicioVigencia = obj.DataInicioVigencia.HasValue ? obj.DataInicioVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DataFimVigencia = obj.DataFimVigencia.HasValue ? obj.DataFimVigencia.Value.ToString("dd/MM/yyyy") : string.Empty
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Seguradora|15", "Apólice|15", "Cliente|30", "Data Inicio|15", "Data Fim|15" }, countApolices);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as apólices de seguro.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.ApoliceDeSeguro repApoliceDeSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
                Dominio.Entidades.ApoliceDeSeguro apolice = repApoliceDeSeguro.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                if (apolice != null)
                {
                    var retorno = new
                    {
                        apolice.Codigo,
                        CPFCNPJCliente = apolice.Cliente != null ? apolice.Cliente.CPF_CNPJ_Formatado : string.Empty,
                        NomeCliente = apolice.Cliente != null ? apolice.Cliente.Nome : string.Empty,
                        CNPJSeguradora = !string.IsNullOrWhiteSpace(apolice.CNPJSeguradora) ? apolice.CNPJSeguradora : string.Empty,
                        apolice.Status,
                        apolice.Ramo,
                        apolice.NumeroApolice,
                        apolice.NomeSeguradora,
                        apolice.Responsavel,
                        DataFimVigencia = apolice.DataFimVigencia.HasValue ? apolice.DataFimVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataInicioVigencia = apolice.DataInicioVigencia.HasValue ? apolice.DataInicioVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                        CNPJResposavelNaObservacaoContribuinte = !string.IsNullOrWhiteSpace(apolice.CNPJResposavelNaObservacaoContribuinte) ? apolice.CNPJResposavelNaObservacaoContribuinte : string.Empty
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Apólice de seguro não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da apólice de seguro.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorClienteParaEmissao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);
                string ramo = Request.Params["Ramo"];
                string numeroApolice = Request.Params["NumeroApolice"];
                string nomeSeguradora = Request.Params["NomeSeguradora"];
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);

                List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceSeguro.ConsultarPorClienteEVigencia(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, cpfCnpjCliente, nomeSeguradora, numeroApolice, ramo, inicioRegistros, 50);
                int countApolices = repApoliceSeguro.ContarConsultaPorClienteEVigencia(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, cpfCnpjCliente, nomeSeguradora, numeroApolice, ramo);

                var retorno = (from obj in listaApolices
                              select new
                              {
                                  obj.Codigo,
                                  CNPJSeguradora = !string.IsNullOrWhiteSpace(obj.CNPJSeguradora) ? obj.CNPJSeguradora : string.Empty,
                                  obj.NomeSeguradora,
                                  obj.NumeroApolice,
                                  obj.Ramo,
                                  DataInicioVigencia = obj.DataInicioVigencia.HasValue ? obj.DataInicioVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DataFimVigencia = obj.DataFimVigencia.HasValue ? obj.DataFimVigencia.Value.ToString("dd/MM/yyyy") : string.Empty
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "CNPJ", "Seguradora|15", "Apólice|15", "Ramo|30", "Data Inicio|15", "Data Fim|15" }, countApolices);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da apólice de seguro.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCliente()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);

                Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);

                List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceSeguro.BuscarPorCliente(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, cpfCnpjCliente);

                var retorno = (from obj in listaApolices
                               select new
                               {
                                   obj.Codigo,
                                   obj.Responsavel,
                                   obj.NomeSeguradora,
                                   CNPJSeguradora = !string.IsNullOrWhiteSpace(obj.CNPJSeguradora) ? obj.CNPJSeguradora : string.Empty,
                                   obj.NumeroApolice,
                                   obj.Ramo,
                                   DataInicioVigencia = obj.DataInicioVigencia.HasValue ? obj.DataInicioVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   DataFimVigencia = obj.DataFimVigencia.HasValue ? obj.DataFimVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   CNPJResposavelNaObservacaoContribuinte = !string.IsNullOrWhiteSpace(obj.CNPJResposavelNaObservacaoContribuinte) ? obj.CNPJResposavelNaObservacaoContribuinte : string.Empty
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da apólice de seguro.");
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
                int codigo, responsavel = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Responsavel"], out responsavel);
                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJCliente"]), out cpfCnpjCliente);
                string nomeSeguradora = Request.Params["NomeSeguradora"];
                string numeroApolice = Request.Params["NumeroApolice"];
                string ramo = Request.Params["Ramo"];
                string status = Request.Params["Status"];
                string cnpjSeguradora = Request.Params["CNPJSeguradora"];
                string cnpjResposavelNaObservacaoContribuinte = Request.Params["CNPJResposavelNaObservacaoContribuinte"];

                DateTime dataInicioVigencia, dataFimVigencia = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicioVigencia"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioVigencia);
                DateTime.TryParseExact(Request.Params["DataFimVigencia"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimVigencia);

                bool.TryParse(Request.Params["CNPJRaiz"], out bool todosCNPJs);

                Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Dominio.Entidades.ApoliceDeSeguro apolice = null;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    apolice = new Dominio.Entidades.ApoliceDeSeguro();
                    apolice.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    apolice = repApoliceSeguro.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                apolice.Cliente = cpfCnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente) : null;
                apolice.Empresa = this.EmpresaUsuario;

                if (dataFimVigencia != DateTime.MinValue)
                    apolice.DataFimVigencia = dataFimVigencia;
                else
                    apolice.DataFimVigencia = null;

                if (dataInicioVigencia != DateTime.MinValue)
                    apolice.DataInicioVigencia = dataInicioVigencia;
                else
                    apolice.DataInicioVigencia = null;

                apolice.NomeSeguradora = nomeSeguradora;
                apolice.NumeroApolice = numeroApolice;
                apolice.Ramo = ramo;
                apolice.CNPJSeguradora = cnpjSeguradora;
                apolice.Responsavel = responsavel;
                apolice.CNPJResposavelNaObservacaoContribuinte = cnpjResposavelNaObservacaoContribuinte;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    apolice.Status = status;

                if (codigo > 0)
                    repApoliceSeguro.Atualizar(apolice);
                else
                    repApoliceSeguro.Inserir(apolice);

                if (todosCNPJs)
                    this.ReplicarParaTodosCNPJs(apolice, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a apólice de seguro.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterInformacoesApoliceAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string mensagemRetorno = string.Empty;

                if (System.Configuration.ConfigurationManager.AppSettings["ExibirAvisoApolicesEmissaoCTe"] == "SIM")
                {
                    Repositorio.ApoliceDeSeguro repApoliceDeSeguro = new Repositorio.ApoliceDeSeguro(unitOfWork);
                    List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceDeSeguro.BuscarPorEmpresaResponsavelVigencia(this.EmpresaUsuario.Codigo, 4, DateTime.Now);

                    if (listaApolices != null && listaApolices.Count() > 0)
                        mensagemRetorno = "Transportador possui apólice própria, favor validar – " + listaApolices.FirstOrDefault().NomeSeguradora + "  " + listaApolices.FirstOrDefault().NumeroApolice + " .";

                    if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.SeguradoraAverbacao != Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido)
                        mensagemRetorno = !string.IsNullOrWhiteSpace(mensagemRetorno) ? string.Concat(mensagemRetorno, " / ", "Transportador possui Averbação Automática.") : "Transportador possui Averbação Automática.";
                }

                var retorno = new
                {
                    Mensagem = mensagemRetorno
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do seguro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void ReplicarParaTodosCNPJs(Dominio.Entidades.ApoliceDeSeguro apolice, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ApoliceDeSeguro repApoliceDeSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            if (apolice != null && apolice.Cliente != null && apolice.Cliente.Tipo == "J")
            {
                List<Dominio.Entidades.Cliente> listaClientes = repCliente.BuscarPorRaizCNPJ(apolice.Cliente.CPF_CNPJ_SemFormato.Substring(0, 8));
                foreach (Dominio.Entidades.Cliente cliente in listaClientes)
                {
                    if (cliente.CPF_CNPJ != apolice.Cliente.CPF_CNPJ)
                    {
                        Dominio.Entidades.ApoliceDeSeguro apoliceCliente = repApoliceDeSeguro.BuscarApolicePorCliente(apolice.Empresa.Codigo, cliente.CPF_CNPJ);
                        if (apoliceCliente == null)
                            apoliceCliente = new Dominio.Entidades.ApoliceDeSeguro();

                        apoliceCliente.Cliente = cliente;
                        apoliceCliente.CNPJSeguradora = apolice.CNPJSeguradora;
                        apoliceCliente.DataFimVigencia = apolice.DataFimVigencia;
                        apoliceCliente.DataInicioVigencia = apolice.DataInicioVigencia;
                        apoliceCliente.Empresa = apolice.Empresa;
                        apoliceCliente.NomeSeguradora = apolice.NomeSeguradora;
                        apoliceCliente.NumeroApolice = apolice.NumeroApolice;
                        apoliceCliente.Ramo = apolice.Ramo;
                        apoliceCliente.Responsavel = apolice.Responsavel;
                        apoliceCliente.Status = apolice.Status;
                        apoliceCliente.CNPJResposavelNaObservacaoContribuinte = apolice.CNPJResposavelNaObservacaoContribuinte;

                        if (apoliceCliente.Codigo > 0)
                            repApoliceDeSeguro.Atualizar(apoliceCliente);
                        else
                            repApoliceDeSeguro.Inserir(apoliceCliente);
                    }
                }
            }
            else
                throw new Exception("Não foi possível selecionar Estado para replicar tabela de frete Valor.");

        }

        #endregion
    }
}
