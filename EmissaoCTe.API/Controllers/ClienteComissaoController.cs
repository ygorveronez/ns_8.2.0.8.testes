using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ClienteComissaoController : ApiController
    {

        #region Variáveis Globais
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("clientecomissao.aspx") select obj).FirstOrDefault();
        }
        #endregion

        #region Métodos Globais
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                int codigo, inicioRegistros;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["InicioRegistros"], out inicioRegistros);
                
                string nomeParceiro = Request.Params["NomeParceiro"];
                string nomeCidade = Request.Params["NomeCidade"];

                Repositorio.ClienteComissao repClienteComissao = new Repositorio.ClienteComissao(unitOfWork);
                List<Dominio.Entidades.ClienteComissao> listaClienteComissao = repClienteComissao.Consultar(this.EmpresaUsuario.Codigo, nomeParceiro, nomeCidade, inicioRegistros, 50);
                int countRegistros = repClienteComissao.ContarConsulta(this.EmpresaUsuario.Codigo, nomeParceiro);

                var retorno = (from obj in listaClienteComissao
                               select new
                               {
                                   obj.Codigo,
                                   Parceiro = obj.Parceiro.CPF_CNPJ + " " + obj.Parceiro.Nome,
                                   Localidade = obj.Localidade.Descricao,
                                   Status = obj.Status == Dominio.Enumeradores.StatusComissaoCliente.Ativo ? "Ativo" : "Inativo"
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Parceiro|50", "Localidade|20","Status|10" }, countRegistros);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as comissões clientes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        [AcceptVerbs("POST")]
        public ActionResult ConsultarDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                double cnpjCpfParceiro = 0F;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjCpfParceiro);

                int codigoLocalidade = 0;
                int.TryParse(Request.Params["Localidade"], out codigoLocalidade);

                Repositorio.ClienteComissao repClienteComissao = new Repositorio.ClienteComissao(unitOfWork);
                Dominio.Entidades.ClienteComissao clienteComissao = repClienteComissao.BuscaPorParceiroLocalidade(this.EmpresaUsuario.Codigo, cnpjCpfParceiro, codigoLocalidade);

                object retorno = null;

                if (clienteComissao != null)
                {
                    retorno = new
                    {
                        ValorMinimo = clienteComissao.MinimoComissao,
                        PercentualComissao = clienteComissao.PercentualComissao,
                        CodigoLocalidade =  clienteComissao.Localidade.Codigo,
                        ValorTDA = clienteComissao.ValorTDA
                    };
                }
                return Json(retorno, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as comissões clientes.");
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
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ClienteComissao repClienteComissao = new Repositorio.ClienteComissao(unitOfWork);

                Dominio.Entidades.ClienteComissao comissao = repClienteComissao.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (comissao == null)
                    return Json<bool>(false, false, "Comissão não encontrado.");

                var retorno = new
                {
                    comissao.Codigo,
                    Parceiro = comissao.Parceiro.CPF_CNPJ + " " + comissao.Parceiro.Nome,
                    CPFCNPJParceiro = comissao.Parceiro.CPF_CNPJ,
                    Localidade = comissao.Localidade.Descricao + " " + comissao.Localidade.Estado.Sigla,
                    CodigoLocalidade = comissao.Localidade.Codigo,
                    PercComissao = comissao.PercentualComissao,
                    ValorMinimo = comissao.MinimoComissao,
                    Status = comissao.Status,
                    ValorTDA = comissao.ValorTDA
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes das comissões clientes.");
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
                int codigo, codigoLocalidade = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Localidade"], out codigoLocalidade);

                decimal percComissao, valorMinimo, valorTDA = 0;
                decimal.TryParse(Request.Params["PercComissao"], out percComissao);
                decimal.TryParse(Request.Params["ValorMinimo"], out valorMinimo);
                decimal.TryParse(Request.Params["ValorTDA"], out valorTDA);

                double cnpjParceiro;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjParceiro);

                Dominio.Enumeradores.StatusComissaoCliente status;
                Enum.TryParse<Dominio.Enumeradores.StatusComissaoCliente>(Request.Params["Status"], out status);

                Dominio.Entidades.ClienteComissao clienteComissao = null;

                Repositorio.ClienteComissao repClienteComissao = new Repositorio.ClienteComissao(unitOfWork);

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Cliente Comissão negada!");

                    clienteComissao = repClienteComissao.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Cliente Comissão negada!");

                    clienteComissao = new Dominio.Entidades.ClienteComissao();
                }

                clienteComissao.Empresa = this.EmpresaUsuario;
                clienteComissao.PercentualComissao = percComissao;
                clienteComissao.MinimoComissao = valorMinimo;
                clienteComissao.ValorTDA = valorTDA;
                clienteComissao.Status = status;

                Repositorio.Cliente repParceiro = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                clienteComissao.Parceiro = repParceiro.BuscarPorCPFCNPJ(cnpjParceiro);
                clienteComissao.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);                

                unitOfWork.Start(System.Data.IsolationLevel.Serializable);

                if (codigo > 0)
                {
                    repClienteComissao.Atualizar(clienteComissao);
                }
                else
                {
                    repClienteComissao.Inserir(clienteComissao);
                }

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Cliente Comissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}