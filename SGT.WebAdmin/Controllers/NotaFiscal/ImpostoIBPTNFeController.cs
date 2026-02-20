using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/ImpostoIBPTNFe")]
    public class ImpostoIBPTNFeController : BaseController
    {
		#region Construtores

		public ImpostoIBPTNFeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);

                string descricao = Request.Params("Descricao");
                string ncm = Request.Params("NCM");
                int empresa = this.Usuario.Empresa.Codigo;
                int empresaPai = 0;
                if (this.Usuario.Empresa.EmpresaPai != null)
                    empresaPai = this.Usuario.Empresa.EmpresaPai.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NCM", "NCM", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe> listaImpostoIBPTNFe = repImpostoIBPTNFe.Consultar(empresaPai, descricao, ncm, empresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repImpostoIBPTNFe.ContarConsulta(empresaPai, descricao, ncm, empresa));
                var lista = (from p in listaImpostoIBPTNFe
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                p.NCM
                            }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe impostoIBPTNFe = new Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe();

                decimal nacionalFederal, importadosFederal, estadual, municipal = 0;
                decimal.TryParse(Request.Params("NacionalFederal"), out nacionalFederal);
                decimal.TryParse(Request.Params("ImportadosFederal"), out importadosFederal);
                decimal.TryParse(Request.Params("Estadual"), out estadual);
                decimal.TryParse(Request.Params("Municipal"), out municipal);

                DateTime vigenciaInicio, vigenciaFim;
                DateTime.TryParse(Request.Params("VigenciaInicio"), out vigenciaInicio);
                DateTime.TryParse(Request.Params("VigenciaFim"), out vigenciaFim);

                string ncm = Request.Params("NCM");
                string extensao = Request.Params("Extensao");
                string tipo = Request.Params("Tipo");
                string descricao = Request.Params("Descricao");                
                string versao = Request.Params("Versao");
                string fonte = Request.Params("Fonte");

                int empresa = this.Usuario.Empresa.Codigo;

                impostoIBPTNFe.Descricao = descricao;
                impostoIBPTNFe.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                impostoIBPTNFe.Estadual = estadual;
                impostoIBPTNFe.Extensao = extensao;
                impostoIBPTNFe.Fonte = fonte;
                impostoIBPTNFe.ImportadosFederal = importadosFederal;
                impostoIBPTNFe.Municipal = municipal;
                impostoIBPTNFe.NacionalFederal = nacionalFederal;
                impostoIBPTNFe.NCM = ncm;
                impostoIBPTNFe.Tipo = tipo;
                impostoIBPTNFe.Versao = versao;
                impostoIBPTNFe.VigenciaFim = vigenciaFim;
                impostoIBPTNFe.VigenciaInicio = vigenciaInicio;

                repImpostoIBPTNFe.Inserir(impostoIBPTNFe, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe impostoIBPTNFe = repImpostoIBPTNFe.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                decimal nacionalFederal, importadosFederal, estadual, municipal = 0;
                decimal.TryParse(Request.Params("NacionalFederal"), out nacionalFederal);
                decimal.TryParse(Request.Params("ImportadosFederal"), out importadosFederal);
                decimal.TryParse(Request.Params("Estadual"), out estadual);
                decimal.TryParse(Request.Params("Municipal"), out municipal);

                DateTime vigenciaInicio, vigenciaFim;
                DateTime.TryParse(Request.Params("VigenciaInicio"), out vigenciaInicio);
                DateTime.TryParse(Request.Params("VigenciaFim"), out vigenciaFim);

                string ncm = Request.Params("NCM");
                string extensao = Request.Params("Extensao");
                string tipo = Request.Params("Tipo");
                string descricao = Request.Params("Descricao");                
                string versao = Request.Params("Versao");
                string fonte = Request.Params("Fonte");

                int empresa = this.Usuario.Empresa.Codigo;

                impostoIBPTNFe.Descricao = descricao;
                impostoIBPTNFe.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                impostoIBPTNFe.Estadual = estadual;
                impostoIBPTNFe.Extensao = extensao;
                impostoIBPTNFe.Fonte = fonte;
                impostoIBPTNFe.ImportadosFederal = importadosFederal;
                impostoIBPTNFe.Municipal = municipal;
                impostoIBPTNFe.NacionalFederal = nacionalFederal;
                impostoIBPTNFe.NCM = ncm;
                impostoIBPTNFe.Tipo = tipo;
                impostoIBPTNFe.Versao = versao;
                impostoIBPTNFe.VigenciaFim = vigenciaFim;
                impostoIBPTNFe.VigenciaInicio = vigenciaInicio;

                repImpostoIBPTNFe.Atualizar(impostoIBPTNFe, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe impostoIBPTNFe = repImpostoIBPTNFe.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    impostoIBPTNFe.Codigo,
                    impostoIBPTNFe.Descricao,
                    impostoIBPTNFe.Estadual,
                    impostoIBPTNFe.Extensao,
                    impostoIBPTNFe.Fonte,
                    impostoIBPTNFe.ImportadosFederal,
                    impostoIBPTNFe.Municipal,
                    impostoIBPTNFe.NacionalFederal,
                    impostoIBPTNFe.NCM,
                    impostoIBPTNFe.Tipo,
                    impostoIBPTNFe.Versao,
                    VigenciaFim = impostoIBPTNFe.VigenciaFim.ToString("dd/MM/yyyy"),
                    VigenciaInicio = impostoIBPTNFe.VigenciaInicio.ToString("dd/MM/yyyy")
                };
                return new JsonpResult(dynProcessoMovimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe impostoIBPTNFe = repImpostoIBPTNFe.BuscarPorCodigo(codigo);
                repImpostoIBPTNFe.Deletar(impostoIBPTNFe, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarCSV()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (codigoEmpresa == 0)
                    return new JsonpResult(false, "Favor selecione uma Empresa cadastrada.");

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".csv")
                    {
                        Servicos.Embarcador.NotaFiscal.ImpostoIBPTNFe svcImpostoIBPTNFe = new Servicos.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
                        string retorno = svcImpostoIBPTNFe.ProcessarArquivoImpostoIBPTNFe(file.InputStream, unitOfWork, codigoEmpresa);

                        if (retorno == "")
                        {
                            unitOfWork.CommitChanges();
                            return new JsonpResult(true, "Importação do IBPT foi realizada com sucesso.");
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, retorno);
                        }

                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(true, "Layout do arquivo está fora de padrão.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o IBPT. Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
