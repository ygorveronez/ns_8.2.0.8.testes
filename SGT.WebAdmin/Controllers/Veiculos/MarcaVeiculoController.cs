using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/MarcaVeiculo")]
    public class MarcaVeiculoController : BaseController
    {
		#region Construtores

		public MarcaVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoModelo = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("Modelo")) && int.Parse(Request.Params("Modelo")) > 0)
                    codigoModelo = int.Parse(Request.Params("Modelo"));

                string tipoVeiculo = "";
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoVeiculo")))
                    tipoVeiculo = Utilidades.String.OnlyNumbers(Request.Params("TipoVeiculo"));

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.TipoDeVeiculo, "DescricaoTipoVeiculo", 15, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("DescricaoModelo", false);
                grid.AdicionarCabecalho("CodigoModelo", false);

                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                List<Dominio.Entidades.MarcaVeiculo> listaMarca = repMarcaVeiculo.Consulta(descricao, ativo, codigoModelo, tipoVeiculo, codigoEmpresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMarcaVeiculo.ContaConsulta(descricao, ativo, codigoModelo, tipoVeiculo, codigoEmpresa));
                var lista = (from p in listaMarca
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoTipoVeiculo,
                                 p.DescricaoStatus,
                                 DescricaoModelo = p.Modelos.Count() == 1 ? p.Modelos[0].Descricao : string.Empty,
                                 CodigoModelo = p.Modelos.Count() == 1 ? p.Modelos[0].Codigo : 0
                             }).ToList();
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.Entidades.MarcaVeiculo marcaVeiculo = new Dominio.Entidades.MarcaVeiculo();

                marcaVeiculo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                marcaVeiculo.Status = bool.Parse(Request.Params("Ativo")) == true ? "A" : "I";
                marcaVeiculo.Descricao = Request.Params("Descricao");
                marcaVeiculo.TipoVeiculo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo)int.Parse(Request.Params("TipoVeiculo"));
                if (codigoEmpresa > 0)
                    marcaVeiculo.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                repMarcaVeiculo.Inserir(marcaVeiculo, Auditado);

                SalvarModelos(marcaVeiculo, unitOfWork);

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

                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.MarcaVeiculo marcaVeiculo = repMarcaVeiculo.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), 0);
                marcaVeiculo.Initialize();

                marcaVeiculo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                marcaVeiculo.Status = bool.Parse(Request.Params("Ativo")) == true ? "A" : "I";
                marcaVeiculo.Descricao = Request.Params("Descricao");
                marcaVeiculo.TipoVeiculo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo)int.Parse(Request.Params("TipoVeiculo"));

                SalvarModelos(marcaVeiculo, unitOfWork);

                repMarcaVeiculo.Atualizar(marcaVeiculo, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
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
                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                Dominio.Entidades.MarcaVeiculo marcaVeiculo = repMarcaVeiculo.BuscarPorCodigo(codigo, 0);
                var dynMarcaVeiculo = new
                {
                    marcaVeiculo.Codigo,
                    marcaVeiculo.CodigoIntegracao,
                    marcaVeiculo.Descricao,
                    Status = marcaVeiculo.Status == "A" ? 1 : 2,
                    marcaVeiculo.TipoVeiculo,
                    Modelos = (from obj in marcaVeiculo.Modelos
                               select new
                               {
                                   obj.Codigo,
                                   obj.CodigoIntegracao,
                                   CodigoFIPE = !string.IsNullOrWhiteSpace(obj.CodigoFIPE) ? obj.CodigoFIPE : string.Empty,
                                   obj.Descricao,
                                   obj.DescricaoPossuiArla32,
                                   obj.DescricaoStatus,
                                   MediaPadrao = obj.MediaPadrao.ToString("n4"),
                                   MediaPadraoVazio = obj.MediaPadraoVazio.ToString("n4"),
                                   AlturaEmMetros = obj.AlturaEmMetros.ToString("n2"),
                                   MediaMinima = obj.MediaMinima.ToString("n4"),
                                   MediaMaxima = obj.MediaMaxima.ToString("n4"),
                                   obj.NumeroEixo,
                                   PossuiArla32 = obj.PossuiArla32 ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao,
                                   SimNao = obj.PossuiArla32 ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao,
                                   Status = obj.Status == "A" ? 1 : 2,
                                   Produto = new { Codigo = obj.Produto != null ? obj.Produto.Codigo : 0, Descricao = obj.Produto != null ? obj.Produto.Descricao : "" },
                                   DescricaoProduto = obj.Produto != null ? obj.Produto.Descricao : "",
                                   CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0
                               }).ToList()
                };
                return new JsonpResult(dynMarcaVeiculo);
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
                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
                Dominio.Entidades.MarcaVeiculo marcaVeiculo = repMarcaVeiculo.BuscarPorCodigo(codigo, true);
                repMarcaVeiculo.Deletar(marcaVeiculo, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #endregion

        #region Métodos Privados

        private void SalvarModelos(Dominio.Entidades.MarcaVeiculo marcaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic dynModelos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Modelos"));

            if (marcaVeiculo.Modelos != null && marcaVeiculo.Modelos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var modelo in dynModelos)
                    if (modelo.Codigo != null)
                        codigos.Add((int)modelo.Codigo);

                List<Dominio.Entidades.ModeloVeiculo> modelosVeiculoDeletar = (from obj in marcaVeiculo.Modelos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < modelosVeiculoDeletar.Count; i++)
                {
                    Dominio.Entidades.ModeloVeiculo modeloVeiculo = modelosVeiculoDeletar[i];
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, marcaVeiculo, "Removeu o modelo de veículo " + modeloVeiculo.Descricao, unitOfWork);
                    repModeloVeiculo.Deletar(modeloVeiculo, Auditado);
                }
            }
            else
                marcaVeiculo.Modelos = new List<Dominio.Entidades.ModeloVeiculo>();

            foreach (var dynModelo in dynModelos)
            {
                int.TryParse((string)dynModelo.Codigo, out int codigoModelo);
                Dominio.Entidades.ModeloVeiculo modeloVeiculo = codigoModelo > 0 ? repModeloVeiculo.BuscarPorCodigo(codigoModelo, true) : null;

                if (modeloVeiculo == null)
                    modeloVeiculo = new Dominio.Entidades.ModeloVeiculo();

                modeloVeiculo.CodigoFIPE = (string)dynModelo.CodigoFIPE;
                modeloVeiculo.Descricao = (string)dynModelo.Descricao;
                if (!string.IsNullOrWhiteSpace((string)dynModelo.MediaPadraoVazio))
                    modeloVeiculo.MediaPadraoVazio = decimal.Parse((string)dynModelo.MediaPadraoVazio);            
                if (!string.IsNullOrWhiteSpace((string)dynModelo.AlturaEmMetros))
                    modeloVeiculo.AlturaEmMetros = decimal.Parse((string)dynModelo.AlturaEmMetros);
                if (!string.IsNullOrWhiteSpace((string)dynModelo.MediaPadrao))
                    modeloVeiculo.MediaPadrao = decimal.Parse((string)dynModelo.MediaPadrao);
                if (!string.IsNullOrWhiteSpace((string)dynModelo.MediaMinima))
                    modeloVeiculo.MediaMinima = decimal.Parse((string)dynModelo.MediaMinima);
                if (!string.IsNullOrWhiteSpace((string)dynModelo.MediaMaxima))
                    modeloVeiculo.MediaMaxima = decimal.Parse((string)dynModelo.MediaMaxima);
                if (!string.IsNullOrWhiteSpace((string)dynModelo.NumeroEixo))
                    modeloVeiculo.NumeroEixo = int.Parse((string)dynModelo.NumeroEixo);
                if (!string.IsNullOrWhiteSpace((string)dynModelo.SimNao))
                {
                    if ((int)dynModelo.SimNao == 1)
                        modeloVeiculo.PossuiArla32 = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim;
                    else
                        modeloVeiculo.PossuiArla32 = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao;
                }
                if (!string.IsNullOrWhiteSpace((string)dynModelo.Status))
                {
                    if ((int)dynModelo.Status == 1)
                        modeloVeiculo.Status = "A";
                    else
                        modeloVeiculo.Status = "I";
                }
                if (!string.IsNullOrWhiteSpace((string)dynModelo.CodigoProduto))
                {
                    if ((int)dynModelo.CodigoProduto > 0)
                    {
                        Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                        modeloVeiculo.Produto = repProduto.BuscarPorCodigo(0, (int)dynModelo.CodigoProduto);
                    }
                }

                modeloVeiculo.MarcaVeiculo = marcaVeiculo;
                modeloVeiculo.Empresa = marcaVeiculo.Empresa;

                if (modeloVeiculo.Codigo > 0)
                    repModeloVeiculo.Atualizar(modeloVeiculo, Auditado);
                else
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, marcaVeiculo, "Adicionou o modelo de veículo " + modeloVeiculo.Descricao, unitOfWork);
                    repModeloVeiculo.Inserir(modeloVeiculo, Auditado);
                }
            }
        }

        #endregion
    }
}
