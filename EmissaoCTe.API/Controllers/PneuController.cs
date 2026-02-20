using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PneuController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("pneus.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorTipoDeHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros, codigoVeiculo = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                string marcaPneu = Request.Params["MarcaPneu"];
                string modeloPneu = Request.Params["ModeloPneu"];
                string dimensaoPneu = Request.Params["DimensaoPneu"];
                string statusPneu = Request.Params["StatusPneu"];
                string tipo = Request.Params["Tipo"];

                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                IList<Dominio.Entidades.Pneu> listaPneu = new List<Dominio.Entidades.Pneu>();
                int countPneu = 0;
                if (tipo.Equals("E"))
                {
                    listaPneu = repPneu.ConsultarPorHistorico(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, inicioRegistros, 50, 0, tipo);
                    countPneu = repPneu.ContarConsultaPorHistorico(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, 0, tipo);
                }
                else if (tipo.Equals("S"))
                {
                    listaPneu = repPneu.ConsultarPorHistorico(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, inicioRegistros, 50, codigoVeiculo, tipo);
                    countPneu = repPneu.ContarConsultaPorHistorico(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, codigoVeiculo, tipo);
                }
                var retorno = from obj in listaPneu select new { obj.Codigo, Serie = obj.Serie, DataCompra = obj.DataCompra.HasValue ? obj.DataCompra.Value.ToString("dd/MM/yyyy") : string.Empty, DataVenda = obj.DataVenda.HasValue ? obj.DataVenda.Value.ToString("dd/MM/yyyy") : string.Empty, ModeloPneu = obj.ModeloPneu != null ? obj.ModeloPneu.Descricao : string.Empty, obj.DescricaoStatus };
                return Json(retorno, true, null, new string[] { "Código", "Série|15", "Data Compra|15", "Data Venda|15", "Modelo|30", "Status|15" }, countPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os pneus.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterEixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoVeiculo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                Dominio.Entidades.Pneu pneu = repPneu.BuscarPorCodigoEVeiculo(codigo, codigoVeiculo, this.EmpresaUsuario.Codigo);
                if (pneu != null && pneu.Eixo != null)
                {
                    var retorno = new { Codigo = pneu.Eixo.Codigo, Descricao = pneu.Eixo.Descricao };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Eixo do pneu não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar o eixo do pneu.");
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
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                Dominio.Entidades.Pneu pneu = repPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                if (pneu != null)
                {
                    var retorno = new
                    {
                        pneu.Codigo,
                        pneu.Custos,
                        DataCompra = pneu.DataCompra.HasValue ? pneu.DataCompra.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataVenda = pneu.DataVenda.HasValue ? pneu.DataVenda.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DescricaoDimensaoPneu = pneu.DimensaoPneu != null ? pneu.DimensaoPneu.Descricao : string.Empty,
                        CodigoDimensaoPneu = pneu.DimensaoPneu != null ? pneu.DimensaoPneu.Codigo : 0,
                        DescricaoMarcaPneu = pneu.MarcaPneu != null ? pneu.MarcaPneu.Descricao : string.Empty,
                        CodigoMarcaoPneu = pneu.MarcaPneu != null ? pneu.MarcaPneu.Codigo : 0,
                        DescricaoModeloPneu = pneu.ModeloPneu != null ? pneu.ModeloPneu.Descricao : string.Empty,
                        CodigoModeloPneu = pneu.ModeloPneu != null ? pneu.ModeloPneu.Codigo : 0,
                        DescricaoStatusPneu = pneu.StatusPneu != null ? pneu.StatusPneu.Descricao : string.Empty,
                        CodigoStatusPneu = pneu.StatusPneu != null ? pneu.StatusPneu.Codigo : 0,
                        pneu.Observacao,
                        pneu.Receitas,
                        pneu.Serie,
                        pneu.Status,
                        pneu.ValorCompra
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Pneu não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do pneu.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarParaManutencao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string serie = Request.Params["serie"];
                string status  = Request.Params["Status"];

                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);

                List<Dominio.Entidades.Pneu> pneus = repPneu.ConsultarParaManutencao(this.EmpresaUsuario.Codigo, serie, status, inicioRegistros, 12);
                int countPneus = repPneu.ContarConsultaParaManutencao(this.EmpresaUsuario.Codigo, serie, status);

                var retorno = new {
                    Pneus = from obj in pneus select new { obj.Codigo, obj.MarcaPneu.Descricao, obj.Serie },
                    Total = countPneus
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os pneus.");
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
                string marcaPneu = Request.Params["MarcaPneu"];
                string modeloPneu = Request.Params["ModeloPneu"];
                string dimensaoPneu = Request.Params["DimensaoPneu"];
                string statusPneu = Request.Params["StatusPneu"];
                string status = Request.Params["Status"];
                string serie = Request.Params["Serie"];
                
                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                var listaPneu = repPneu.Consultar(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, status, serie, inicioRegistros, 50);
                int countPneu = repPneu.ContarConsulta(this.EmpresaUsuario.Codigo, marcaPneu, modeloPneu, dimensaoPneu, statusPneu, status, serie);

                var retorno = from obj in listaPneu select new { obj.Codigo, Serie = obj.Serie, DataCompra = obj.DataCompra.HasValue ? obj.DataCompra.Value.ToString("dd/MM/yyyy") : string.Empty, DataVenda = obj.DataVenda.HasValue ? obj.DataVenda.Value.ToString("dd/MM/yyyy") : string.Empty, ModeloPneu = obj.ModeloPneu != null ? obj.ModeloPneu.Descricao : string.Empty, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Série|15", "Data Compra|15", "Data Venda|15", "Modelo|30", "Status|15" }, countPneu);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os pneus.");
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
                int codigo, codigoMarca, codigoModelo, codigoStatus, codigoDimensao = 0;
                int.TryParse(Request.Params["CodigoDimensao"], out codigoDimensao);
                int.TryParse(Request.Params["CodigoStatus"], out codigoStatus);
                int.TryParse(Request.Params["CodigoModelo"], out codigoModelo);
                int.TryParse(Request.Params["CodigoMarca"], out codigoMarca);
                int.TryParse(Request.Params["Codigo"], out codigo);
                DateTime dataCompra, dataVenda;
                DateTime.TryParseExact(Request.Params["DataCompra"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCompra);
                DateTime.TryParseExact(Request.Params["DataVenda"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVenda);
                decimal valorCompra, custos, receitas = 0;
                decimal.TryParse(Request.Params["ValorCompra"], out valorCompra);
                decimal.TryParse(Request.Params["Custos"], out custos);
                decimal.TryParse(Request.Params["Receitas"], out receitas);
                string observacao = Request.Params["Observacao"];
                string status = Request.Params["Status"];
                string serie = Request.Params["Serie"];

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                Repositorio.Pneu repPneu = new Repositorio.Pneu(unitOfWork);
                Repositorio.DimensaoPneu repDimensaoPneu = new Repositorio.DimensaoPneu(unitOfWork);
                Repositorio.MarcaPneu repMarcaPneu = new Repositorio.MarcaPneu(unitOfWork);
                Repositorio.ModeloPneu repModeloPneu = new Repositorio.ModeloPneu(unitOfWork);
                Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unitOfWork);
                Dominio.Entidades.Pneu pneu;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    pneu = new Dominio.Entidades.Pneu();
                    pneu.Status = "A";
                    pneu.StatusPneu = repStatusPneu.BuscarPorCodigo(codigoStatus, this.EmpresaUsuario.Codigo);
                    if (pneu.StatusPneu == null || pneu.StatusPneu.Tipo != "A")
                        return Json<bool>(false, false, "Status do pneu inválido.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    pneu = repPneu.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                pneu.Custos = custos;

                if (dataCompra != DateTime.MinValue)
                    pneu.DataCompra = dataCompra;
                else
                    pneu.DataCompra = null;

                if (dataVenda != DateTime.MinValue)
                    pneu.DataVenda = dataVenda;
                else
                    pneu.DataVenda = null;

                pneu.DimensaoPneu = repDimensaoPneu.BuscarPorCodigo(codigoDimensao, this.EmpresaUsuario.Codigo);
                pneu.Empresa = this.EmpresaUsuario;
                pneu.MarcaPneu = repMarcaPneu.BuscarPorCodigo(codigoMarca, this.EmpresaUsuario.Codigo);
                pneu.ModeloPneu = repModeloPneu.BuscarPorCodigo(codigoModelo, this.EmpresaUsuario.Codigo);
                pneu.Observacao = observacao;
                pneu.Receitas = receitas;
                pneu.Serie = serie;
                pneu.ValorCompra = valorCompra;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    pneu.Status = status;

                if (codigo > 0)
                    repPneu.Atualizar(pneu);
                else
                    repPneu.Inserir(pneu);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o pneu.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
