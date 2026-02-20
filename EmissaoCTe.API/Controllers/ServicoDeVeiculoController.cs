using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ServicoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("servicosdeveiculos.aspx") select obj).FirstOrDefault();
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
                string descricao = Request.Params["Descricao"];
                string status = Request.Params["Status"];

                Repositorio.ServicoVeiculo repServicoVeiculo = new Repositorio.ServicoVeiculo(unitOfWork);
                var listaServicosVeiculos = repServicoVeiculo.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countServicosVeiculos = repServicoVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaServicosVeiculos select new { obj.Codigo, obj.Status, obj.Descricao, KMTroca = obj.KMTroca.ToString("n0"), DiasTroca = obj.DiasTroca.ToString("n0"), KMAvisoManutencao = obj.KMAvisoManutencao.ToString("n0"), DiasAvisoManutencao = obj.DiasAvisoManutencao.ToString("n0"), obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Descrição|40", "KM Troca|10", "Dias Troca|10", "KM Aviso|10", "Dias Aviso|10", "Status|10" }, countServicosVeiculos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os serviços de veículos.");
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
                int codigo, kmTroca, diasTroca, diasAvisoManutencao, kmAvisoManutencao, codigoPlanoDeConta = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoPlanoDeConta"], out codigoPlanoDeConta);
                int.TryParse(Request.Params["KMTroca"], out kmTroca);
                int.TryParse(Request.Params["DiasTroca"], out diasTroca);
                int.TryParse(Request.Params["DiasAvisoManutencao"], out diasAvisoManutencao);
                int.TryParse(Request.Params["KMAvisoManutencao"], out kmAvisoManutencao);
                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                Repositorio.ServicoVeiculo repServicoVeiculo = new Repositorio.ServicoVeiculo(unitOfWork);
                Dominio.Entidades.ServicoVeiculo servicoVeiculo;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    servicoVeiculo = new Dominio.Entidades.ServicoVeiculo();
                    servicoVeiculo.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    servicoVeiculo = repServicoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                Repositorio.PlanoDeConta repPlanoDeConta = new Repositorio.PlanoDeConta(unitOfWork);

                servicoVeiculo.PlanoDeConta = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoDeConta);
                servicoVeiculo.Descricao = descricao;
                servicoVeiculo.Empresa = this.EmpresaUsuario;
                servicoVeiculo.DiasTroca = diasTroca;
                servicoVeiculo.KMTroca = kmTroca;
                servicoVeiculo.DiasAvisoManutencao = diasAvisoManutencao;
                servicoVeiculo.KMAvisoManutencao = kmAvisoManutencao;
                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    servicoVeiculo.Status = status;

                if (codigo > 0)
                    repServicoVeiculo.Atualizar(servicoVeiculo);
                else
                    repServicoVeiculo.Inserir(servicoVeiculo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o serviço de veículo.");
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

                Repositorio.ServicoVeiculo repServicoVeiculo = new Repositorio.ServicoVeiculo(unitOfWork);
                Dominio.Entidades.ServicoVeiculo servicoVeiculo = repServicoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (servicoVeiculo != null)
                {
                    var retorno = new
                    {
                        servicoVeiculo.Codigo,
                        servicoVeiculo.Descricao,
                        DiasAvisoManutencao = servicoVeiculo.DiasAvisoManutencao.ToString("n0"),
                        DiasTroca = servicoVeiculo.DiasTroca.ToString("n0"),
                        KMAvisoManutencao = servicoVeiculo.KMAvisoManutencao.ToString("n0"),
                        KMTroca = servicoVeiculo.KMTroca.ToString("n0"),
                        DescricaoPlanoDeConta = servicoVeiculo.PlanoDeConta != null ? string.Concat(servicoVeiculo.PlanoDeConta.Conta, " - ", servicoVeiculo.PlanoDeConta.Descricao) : string.Empty,
                        CodigoPlanoDeConta = servicoVeiculo.PlanoDeConta != null ? servicoVeiculo.PlanoDeConta.Codigo : 0,
                        servicoVeiculo.Status
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Serviço não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do serviço.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterServicosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if ((this.UsuarioAdministrativo != null) || (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeServicosVeiculos))
                    return Json<bool>(false, false, "Serviços de veículos sem configuração para exibição na pagina inicial.");

                Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unitOfWork);

                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioServicosVeiculos> listaServicos = repHistoricoVeiculo.RelatorioServicos(this.EmpresaUsuario.Codigo, 0, 0, DateTime.MinValue, DateTime.MinValue);
                
                var listaCodigoVeiculo = (from obj in listaServicos select obj.CodigoVeiculo).Distinct();

                List<object> listaServicosAgrupados = new List<object>();
                
                foreach (int codigoVeiculo in listaCodigoVeiculo)
                {
                    listaServicosAgrupados.Add(new { CodigoVeiculo = codigoVeiculo, Servicos = from obj in listaServicos where obj.CodigoVeiculo == codigoVeiculo select obj });
                }

                return Json(listaServicosAgrupados, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os serviços de veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
