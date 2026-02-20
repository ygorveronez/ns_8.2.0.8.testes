using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EixoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("eixosdeveiculos.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros, codigoVeiculo = 0;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];
                string tipo = Request.Params["Tipo"];

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                List<Dominio.Entidades.EixoVeiculo> listaEixoVeiculo = new List<Dominio.Entidades.EixoVeiculo>();
                int countEixoVeiculo = 0;

                if (veiculo != null && veiculo.TipoDoVeiculo != null)
                {
                    var consulta = from obj in veiculo.TipoDoVeiculo.EixosDoVeiculo where obj.Status.Equals("A") select obj;
                    if (!string.IsNullOrWhiteSpace(descricao))
                        consulta = consulta.Where(o => o.Descricao.Contains(descricao));

                    if (tipo.Equals("E"))
                    {
                        Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                        List<Dominio.Entidades.Pneu> listaPneus = repPneu.BuscarPorEixosEVeiculo((from obj in consulta select obj.Codigo).ToList(), codigoVeiculo);
                        consulta = consulta.Where(o => !(from obj in listaPneus select obj.Eixo.Codigo).ToList().Contains(o.Codigo));
                    }
                    else if (tipo.Equals("S"))
                    {
                        Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                        List<Dominio.Entidades.Pneu> listaPneus = repPneu.BuscarPorEixosEVeiculo((from obj in consulta select obj.Codigo).ToList(), codigoVeiculo);
                        consulta = consulta.Where(o => (from obj in listaPneus select obj.Eixo.Codigo).ToList().Contains(o.Codigo));
                    }

                    listaEixoVeiculo = consulta.Skip(inicioRegistros).Take(50).ToList();
                    countEixoVeiculo = consulta.Count();
                }
                var retorno = from obj in listaEixoVeiculo select new { obj.Codigo, obj.Status, obj.Dianteiro, obj.Tipo, obj.Descricao, DescricaoDianteiro = obj.Dianteiro ? "Sim" : "Não", OrdemEixo = obj.OrdemEixo.ToString("n0"), obj.DescricaoTipo, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Dianteiro", "Tipo", "Descrição|42", "Dianteiro|12", "Ordem|12", "Tipo|12", "Status|12" }, countEixoVeiculo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os eixos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterPneu()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoVeiculo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                Dominio.Entidades.Pneu pneu = repPneu.BuscarPorEixoEVeiculo(codigo, codigoVeiculo, this.EmpresaUsuario.Codigo);
                if (pneu != null)
                {
                    var retorno = new { Codigo = pneu.Codigo, Serie = pneu.Serie, ModeloPneu = pneu.ModeloPneu.Descricao };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "O pneu do eixo não foi encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o pneu do eixo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

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

                Repositorio.EixoVeiculo repEixoVeiculo = new Repositorio.EixoVeiculo(unitOfWork);
                var listaEixoVeiculo = repEixoVeiculo.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countEixoVeiculo = repEixoVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaEixoVeiculo select new { obj.Codigo, obj.Status, obj.Dianteiro, obj.Tipo, obj.Descricao, DescricaoDianteiro = obj.Dianteiro ? "Sim" : "Não", OrdemEixo = obj.OrdemEixo.ToString("n0"), obj.DescricaoTipo, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Dianteiro", "Tipo", "Descrição|42", "Dianteiro|12", "Ordem|12", "Tipo|12", "Status|12" }, countEixoVeiculo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os eixos.");
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

                Repositorio.EixoVeiculo repEixoVeiculo = new Repositorio.EixoVeiculo(unitOfWork);
                Dominio.Entidades.EixoVeiculo eixo = repEixoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (eixo != null)
                {
                    var retorno = new
                    {
                        eixo.Codigo,
                        eixo.Status,
                        eixo.Dianteiro,
                        eixo.Tipo,
                        eixo.Descricao,
                        OrdemEixo = eixo.OrdemEixo.ToString("n0"),
                        eixo.Posicao,
                        InternoExterno = eixo.Interno_Externo
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Eixo não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os eixos.");
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
                int codigo, ordemEixo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["OrdemEixo"], out ordemEixo);
                bool dianteiro;
                bool.TryParse(Request.Params["Dianteiro"], out dianteiro);
                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];
                string posicao = Request.Params["Posicao"];
                string internoExterno = Request.Params["InternoExterno"];
                string tipo = Request.Params["Tipo"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Tipo inválido.");
                if (string.IsNullOrWhiteSpace(posicao))
                    return Json<bool>(false, false, "Posição no veículo inválida.");
                if (string.IsNullOrWhiteSpace(internoExterno))
                    return Json<bool>(false, false, "Posição no eixo inválida.");

                Repositorio.EixoVeiculo repEixoVeiculo = new Repositorio.EixoVeiculo(unitOfWork);
                Dominio.Entidades.EixoVeiculo eixoVeiculo;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    eixoVeiculo = new Dominio.Entidades.EixoVeiculo();
                    eixoVeiculo.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    eixoVeiculo = repEixoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                eixoVeiculo.Descricao = descricao;
                eixoVeiculo.Empresa = this.EmpresaUsuario;
                eixoVeiculo.Dianteiro = dianteiro;
                eixoVeiculo.OrdemEixo = ordemEixo;
                eixoVeiculo.Tipo = tipo;
                eixoVeiculo.Interno_Externo = internoExterno;
                eixoVeiculo.Posicao = posicao;
                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    eixoVeiculo.Status = status;

                if (codigo > 0)
                    repEixoVeiculo.Atualizar(eixoVeiculo);
                else
                    repEixoVeiculo.Inserir(eixoVeiculo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o eixo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
