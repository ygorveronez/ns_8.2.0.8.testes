using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EmpresaContratoController : ApiController
    {
        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("empresacontrato.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nomeEmpresa = Request.Form["NomeEmpresa"];

                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);

                List<Dominio.Entidades.EmpresaContrato> listaEmpresaContrato = new List<Dominio.Entidades.EmpresaContrato>();
                int countContratos = 0;

                Servicos.Log.TratarErro("Consulta Contrato empresa: " + this.EmpresaUsuario.CNPJ);

                if (this.EmpresaUsuario.CNPJ == "13969629000196")
                {
                    listaEmpresaContrato = repEmpresaContrato.Consultar(nomeEmpresa, inicioRegistros, 50);
                    countContratos = repEmpresaContrato.ContarConsulta(nomeEmpresa);
                }
                else
                {
                    listaEmpresaContrato = repEmpresaContrato.Consultar(this.EmpresaUsuario.Codigo, nomeEmpresa, inicioRegistros, 50);
                    countContratos = repEmpresaContrato.ContarConsulta(this.EmpresaUsuario.Codigo, nomeEmpresa);
                }

                var result = from obj in listaEmpresaContrato select new { obj.Codigo, obj.Empresa.CNPJ, obj.Empresa.NomeFantasia };

                return Json(result, true, null, new string[] { "Codigo", "CNPJ|20", "Empresa|60" }, countContratos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as contratos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);
                Dominio.Entidades.EmpresaContrato empresaContrato = repEmpresaContrato.BuscarPorCodigo(codigo);

                if (empresaContrato == null)
                    return Json<bool>(false, false, "Contrato não encontrada.");

                var retorno = new
                {
                    empresaContrato.Codigo,
                    CodigoEmpresa = empresaContrato.Empresa.Codigo,
                    DescricaoEmpresa = string.Concat(empresaContrato.Empresa.CNPJ, " - ", empresaContrato.Empresa.NomeFantasia),
                    empresaContrato.Contrato
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterContratoEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);
                Dominio.Entidades.EmpresaContrato empresaContrato = repEmpresaContrato.BuscarPorEmpresa(this.Usuario.Empresa.Codigo);

                if (empresaContrato == null && this.Usuario.Empresa.EmpresaPai != null)
                    empresaContrato = repEmpresaContrato.BuscarPorEmpresa(this.Usuario.Empresa.EmpresaPai.Codigo);

                if (empresaContrato == null)
                    return Json<bool>(false, false, "Contrato não encontrada.");

                var contrato = empresaContrato.Contrato;

                contrato = contrato.Replace("#RazaoSocialTransportadora#", this.Usuario.Empresa.RazaoSocial);
                contrato = contrato.Replace("#CNPJTransportadora#", this.Usuario.Empresa.CNPJ_Formatado);
                contrato = contrato.Replace("#EnderecoTransportadora#", this.Usuario.Empresa.Endereco);
                contrato = contrato.Replace("#ComplementoTransportadora#", this.Usuario.Empresa.Complemento);
                contrato = contrato.Replace("#BairroTransportadora#", this.Usuario.Empresa.Bairro);
                contrato = contrato.Replace("#CidadeUFTransportadora#", string.Concat(this.Usuario.Empresa.Localidade.Descricao, " - ", this.Usuario.Empresa.Localidade.Estado.Sigla));

                if (this.Usuario.Empresa.EmpresaPai != null)
                {
                    contrato = contrato.Replace("#RazaoSocialEmpresaPai#", this.Usuario.Empresa.EmpresaPai.RazaoSocial);
                    contrato = contrato.Replace("#CNPJEmpresaPai#", this.Usuario.Empresa.EmpresaPai.CNPJ_Formatado);
                    contrato = contrato.Replace("#EnderecoEmpresaPai#", this.Usuario.Empresa.EmpresaPai.Endereco);
                    contrato = contrato.Replace("#ComplementoEmpresaPai#", this.Usuario.Empresa.EmpresaPai.Complemento);

                    contrato = contrato.Replace("#BairroEmpresaPai#", this.Usuario.Empresa.EmpresaPai.Bairro);
                    contrato = contrato.Replace("#CidadeUFEmpresaPai#", string.Concat(this.Usuario.Empresa.EmpresaPai.Localidade.Descricao, " - ", this.Usuario.Empresa.EmpresaPai.Localidade.Estado.Sigla));
                    contrato = contrato.Replace("#ContatoEmpresaPai#", this.Usuario.Empresa.EmpresaPai.Contato);
                }

                contrato = contrato.Replace("#DataAtual#", DateTime.Today.ToLongDateString());
                contrato = contrato.Replace("#DataCadastro#", this.Usuario.Empresa.DataCadastro.Value.ToString("dd/MM/yyyy"));

                if (contrato.Contains("#PlanoDeEmissaoTransportadora#"))
                {
                    string faixasEmissao = string.Empty;

                    Repositorio.FaixaEmissaoCTe repFaixaEmissaoCTe = new Repositorio.FaixaEmissaoCTe(unitOfWork);
                    if (this.Usuario.Empresa.PlanoEmissaoCTe != null)
                    {
                        List<Dominio.Entidades.FaixaEmissaoCTe> listaFaixa = repFaixaEmissaoCTe.BuscarPorPlano(this.Usuario.Empresa.PlanoEmissaoCTe.Codigo);
                        if (listaFaixa != null && listaFaixa.Count > 0)
                        {
                            faixasEmissao = string.Concat("Plano: ", listaFaixa.FirstOrDefault().Plano.Descricao, "\n");
                            foreach (Dominio.Entidades.FaixaEmissaoCTe faixa in listaFaixa)
                            {
                                faixasEmissao = string.Concat(faixasEmissao, "\n", "Até ", faixa.Quantidade, " documentos emitidos Valor: R$", faixa.Valor.ToString("n2"));
                            }
                        }
                    }

                    contrato = contrato.Replace("#PlanoDeEmissaoTransportadora#", faixasEmissao);
                }

                var retorno = new
                {
                    Contrato = contrato,
                    PermiteAceitar = Servicos.Usuario.ObrigatorioTermos(this.Usuario, unitOfWork) && IdUsuarioAdmin == 0
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConcordarTermosDeUso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = this.EmpresaUsuario;

                if (empresa.AceitouTermosUso || IdUsuarioAdmin > 0)
                    return Json<bool>(true, true);

                string logAceite = "Termos de uso aceito por #usuario# (#login#) dia #data# às #hora#. Acessado pelo IP #ip#.";
                string ip = PegaEnderecoIP();
                DateTime dataAceite = DateTime.Now;

                logAceite = logAceite
                    .Replace("#usuario#", this.Usuario.Nome)
                    .Replace("#login#", this.Usuario.Login)
                    .Replace("#data#", dataAceite.ToString("dd/MM/yyyy"))
                    .Replace("#hora#", dataAceite.ToString("HH:mm"))
                    .Replace("#ip#", ip)
                ;

                empresa.AceitouTermosUso = true;
                empresa.LogAceiteTermosUso = logAceite;
                empresa.DataAceiteTermosUso = dataAceite;

                Session["ReloadPages"] = true;

                repEmpresa.Atualizar(empresa);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao aceitar os termos de uso!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoEmpresa = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                string contrato = Request.Params["Contrato"];

                if (string.IsNullOrWhiteSpace(contrato))
                    return Json<bool>(false, false, "Contrato inválida.");

                Repositorio.EmpresaContrato repDespesa = new Repositorio.EmpresaContrato(unitOfWork);

                Dominio.Entidades.EmpresaContrato empresaContrato = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    empresaContrato = repDespesa.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    empresaContrato = new Dominio.Entidades.EmpresaContrato();
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                empresaContrato.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresaContrato.Empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                empresaContrato.Contrato = contrato;

                if (empresaContrato.Codigo > 0)
                    repDespesa.Atualizar(empresaContrato);
                else
                    repDespesa.Inserir(empresaContrato);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a contrato para a empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.EmpresaContrato repDespesa = new Repositorio.EmpresaContrato(unitOfWork);

                Dominio.Entidades.EmpresaContrato empresaContrato = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                        return Json<bool>(false, false, "Permissão para exclusão negada.");

                    empresaContrato = repDespesa.BuscarPorCodigo(codigo);
                }
                else
                    return Json<bool>(false, false, "Contrato não selecionado para exclusão.");

                if (empresaContrato == null)
                    return Json<bool>(false, false, "Contrato não localizado para exclusão.");

                repDespesa.Deletar(empresaContrato);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir a contrato para a empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        protected string PegaEnderecoIP()
        {
            // https://stackoverflow.com/questions/735350/how-to-get-a-users-client-ip-address-in-asp-net
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;
        }

        private int IdUsuarioAdmin
        {
            get
            {
                int codigo = 0;
                if (Session["IdUsuarioAdmin"] != null)
                    codigo = (int)Session["IdUsuarioAdmin"];

                return codigo;
            }
        }
    }
}
