using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OcorrenciaDeFuncionarioController : ApiController
    {

        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ocorrenciasdefuncionarios.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nomeFuncionario = Request.Params["NomeFuncionario"];
                string tipoOcorrencia = Request.Params["DescricaoTipoOcorrencia"];
                string status = Request.Params["Status"];

                Repositorio.OcorrenciaDeFuncionario repOcorrenciaDeFuncionario = new Repositorio.OcorrenciaDeFuncionario(unitOfWork);
                var listaOcorrencia = repOcorrenciaDeFuncionario.Consultar(this.EmpresaUsuario.Codigo, nomeFuncionario, tipoOcorrencia, status, inicioRegistros, 50);
                int countOcorrencia = repOcorrenciaDeFuncionario.ContarConsulta(this.EmpresaUsuario.Codigo, nomeFuncionario, tipoOcorrencia, status);

                var retorno = from obj in listaOcorrencia select new { obj.Codigo, DataOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy"), Funcionario = string.Concat(obj.Funcionario.CPF, " - ", obj.Funcionario.Nome), TipoOcorrencia = obj.TipoDeOcorrencia.Descricao, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Data Ocorrência|15", "Funcionário|30", "Tipo Ocorrência|30", "Status|15" }, countOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as ocorrências.");
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

                Repositorio.OcorrenciaDeFuncionario repOcorrencia = new Repositorio.OcorrenciaDeFuncionario(unitOfWork);
                Dominio.Entidades.OcorrenciaDeFuncionario ocorrencia = repOcorrencia.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (ocorrencia != null)
                {
                    var retorno = new
                    {
                        ocorrencia.Codigo,
                        DataDaOcorrencia = ocorrencia.DataDaOcorrencia.ToString("dd/MM/yyyy"),
                        DataDeCadastro = ocorrencia.DataDeCadastro.ToString("dd/MM/yyyy"),
                        ocorrencia.Descricao,
                        ocorrencia.DescricaoStatus,
                        CodigoFuncionario = ocorrencia.Funcionario.Codigo,
                        CPFFuncionario = ocorrencia.Funcionario.CPF,
                        NomeFuncionario = ocorrencia.Funcionario.Nome,
                        ocorrencia.Status,
                        DescricaoTipoOcorrencia = ocorrencia.TipoDeOcorrencia.Descricao,
                        CodigoTipoOcorrencia = ocorrencia.TipoDeOcorrencia.Codigo,
                        PlacaVeiculo = ocorrencia.Veiculo != null ? ocorrencia.Veiculo.Placa : string.Empty,
                        CodigoVeiculo = ocorrencia.Veiculo != null ? ocorrencia.Veiculo.Codigo : 0,
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Ocorrência não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar a ocorrência.");
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
                int codigo, codigoTipoOcorrencia, codigoVeiculo, codigoFuncionario = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoTipoDeOcorrencia"], out codigoTipoOcorrencia);
                int.TryParse(Request.Params["CodigoFuncionario"], out codigoFuncionario);

                DateTime dataOcorrencia;
                DateTime.TryParseExact(Request.Params["DataDaOcorrencia"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataOcorrencia);

                if (dataOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");

                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.OcorrenciaDeFuncionario repOcorrencia = new Repositorio.OcorrenciaDeFuncionario(unitOfWork);
                Dominio.Entidades.OcorrenciaDeFuncionario ocorrencia;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    ocorrencia = new Dominio.Entidades.OcorrenciaDeFuncionario();
                    ocorrencia.Status = "A";
                    ocorrencia.DataDeCadastro = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    ocorrencia = repOcorrencia.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.TipoDeOcorrencia repTipoOcorrencia = new Repositorio.TipoDeOcorrencia(unitOfWork);

                ocorrencia.Descricao = descricao;

                ocorrencia.Funcionario = repUsuario.BuscarPorCodigo(codigoFuncionario);
                if (ocorrencia.Funcionario == null)
                    return Json<bool>(false, false, "Funcionário não encontrado.");

                ocorrencia.TipoDeOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia, this.EmpresaUsuario.Codigo);
                if (ocorrencia.TipoDeOcorrencia == null)
                    return Json<bool>(false, false, "Tipo de ocorrência não encontrado.");

                ocorrencia.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                ocorrencia.DataDaOcorrencia = dataOcorrencia;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    ocorrencia.Status = status;

                if (codigo > 0)
                    repOcorrencia.Atualizar(ocorrencia);
                else
                    repOcorrencia.Inserir(ocorrencia);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}
