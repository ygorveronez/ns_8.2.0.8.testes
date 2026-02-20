using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class TipoDeVeiculoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("tiposdeveiculos.aspx") select obj).FirstOrDefault();
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

                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
                var listaTiposVeiculos = repTipoVeiculo.Consultar(this.EmpresaUsuario.Codigo, descricao, status, inicioRegistros, 50);
                int countTiposVeiculos = repTipoVeiculo.ContarConsulta(this.EmpresaUsuario.Codigo, descricao, status);

                var retorno = from obj in listaTiposVeiculos select new {
                    obj.Codigo,
                    obj.Status,
                    CodigoIntegracao = string.IsNullOrWhiteSpace(obj.CodigoIntegracao) ? string.Empty : obj.CodigoIntegracao,
                    obj.Descricao,
                    PesoBruto = obj.PesoBruto.ToString("n0"),
                    PesoLiquido = obj.PesoLiquido.ToString("n0"),
                    NumeroEixos = obj.NumeroEixos.ToString("n0"),
                    obj.DescricaoStatus
                };

                return Json(retorno, true, null, new string[] { "Código", "Status", "Código Integração", "Descrição|42", "Peso Bruto|12", "Peso Líquido|12", "Número Eixos|12", "Status|12" }, countTiposVeiculos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os tipos de veículos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unidadeTrabalho.Start();
                int codigo, pesoBruto, pesoLiquido, numeroEixos = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["PesoBruto"], out pesoBruto);
                int.TryParse(Request.Params["PesoLiquido"], out pesoLiquido);
                int.TryParse(Request.Params["NumeroEixos"], out numeroEixos);

                string status = Request.Params["Status"];
                string descricao = Request.Params["Descricao"];
                string codigoIntegracao = Request.Params["CodigoIntegracao"];

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida.");
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unidadeTrabalho);
                Dominio.Entidades.TipoVeiculo tipoVeiculo;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    tipoVeiculo = new Dominio.Entidades.TipoVeiculo();
                    tipoVeiculo.Status = "A";
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    tipoVeiculo = repTipoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }
                tipoVeiculo.Descricao = descricao;
                tipoVeiculo.PesoBruto = pesoBruto;
                tipoVeiculo.PesoLiquido = pesoLiquido;
                tipoVeiculo.CodigoIntegracao = codigoIntegracao;
                tipoVeiculo.Empresa = this.EmpresaUsuario;
                tipoVeiculo.NumeroEixos = numeroEixos;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    tipoVeiculo.Status = status;

                if (!string.IsNullOrWhiteSpace(Request.Params["Eixos"]))
                {
                    if (tipoVeiculo.EixosDoVeiculo == null)
                        tipoVeiculo.EixosDoVeiculo = new List<Dominio.Entidades.EixoVeiculo>();

                    List<Dominio.ObjetosDeValor.Eixo> eixos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Eixo>>(Request.Params["Eixos"]);
                    if (eixos.Count > 0)
                    {
                        Repositorio.EixoVeiculo repEixoVeiculo = new Repositorio.EixoVeiculo(unidadeTrabalho);
                        foreach (Dominio.ObjetosDeValor.Eixo eixo in eixos)
                        {
                            var eixoExistente = (from obj in tipoVeiculo.EixosDoVeiculo where obj.Codigo == eixo.Codigo select obj).FirstOrDefault();
                            if (eixoExistente == null)
                                tipoVeiculo.EixosDoVeiculo.Add(repEixoVeiculo.BuscarPorCodigo(eixo.Codigo, this.EmpresaUsuario.Codigo));
                        }
                        var listaIdEixos = (from obj in eixos select obj.Codigo).ToList();
                        var listaEixosDeletar = (from obj in tipoVeiculo.EixosDoVeiculo where !listaIdEixos.Contains(obj.Codigo) select obj).ToList();
                        foreach (Dominio.Entidades.EixoVeiculo eixo in listaEixosDeletar)
                            tipoVeiculo.EixosDoVeiculo.Remove(eixo);
                    }
                    else
                    {
                        if (tipoVeiculo.EixosDoVeiculo != null && tipoVeiculo.EixosDoVeiculo.Count > 0)
                            tipoVeiculo.EixosDoVeiculo.Clear();
                    }
                }
                else
                {
                    if (tipoVeiculo.EixosDoVeiculo != null && tipoVeiculo.EixosDoVeiculo.Count > 0)
                        tipoVeiculo.EixosDoVeiculo.Clear();
                }

                if (codigo > 0)
                    repTipoVeiculo.Atualizar(tipoVeiculo);
                else
                    repTipoVeiculo.Inserir(tipoVeiculo);

                unidadeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o tipo de veículo.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarEixos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
                Dominio.Entidades.TipoVeiculo tipoVeiculo = repTipoVeiculo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                if (tipoVeiculo != null)
                {
                    var retorno = from obj in tipoVeiculo.EixosDoVeiculo
                                  orderby obj.OrdemEixo ascending, obj.Descricao ascending
                                  select new Dominio.ObjetosDeValor.Eixo
                                  {
                                      Codigo = obj.Codigo,
                                      Descricao = obj.Descricao,
                                      DescricaoDianteiro = obj.Dianteiro ? "Sim" : "Não",
                                      DescricaoStatus = obj.DescricaoStatus,
                                      DescricaoTipo = obj.DescricaoTipo,
                                      Dianteiro = obj.Dianteiro,
                                      OrdemEixo = obj.OrdemEixo.ToString("n0"),
                                      Status = obj.Status,
                                      Tipo = obj.Tipo
                                  };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Veículo não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os eixos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
