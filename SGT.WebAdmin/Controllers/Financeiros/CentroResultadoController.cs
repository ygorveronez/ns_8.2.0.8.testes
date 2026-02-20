using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "ProximaNumeracao" }, "Financeiros/CentroResultado")]
    public class CentroResultadoController : BaseController
    {
		#region Construtores

		public CentroResultadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.CentroResultado.NumeroDoCentro, "Plano", 25, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoAnaliticoSintetico", 15, Models.Grid.Align.left, false, true, true);
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false, true, true);

                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> listaCentroResultado = repCentroResultado.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCentroResultado.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaCentroResultado
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.BuscarDescricao,
                                 p.Plano,
                                 p.DescricaoAnaliticoSintetico,
                                 p.DescricaoAtivo
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado filtrosPesquisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número do Centro", "Plano", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoAnaliticoSintetico", 15, Models.Grid.Align.left, false);
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> listaCentroResultado = repCentroResultado.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCentroResultado.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaCentroResultado
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.BuscarDescricao,
                                 p.Plano,
                                 p.DescricaoAnaliticoSintetico,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new(unitOfWork);

                int codigoSegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo");

                bool ativo = Request.GetBoolParam("Ativo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico>("AnaliticoSintetico");

                string descricao = Request.Params("Descricao");
                string plano = Request.Params("Plano");
                string planoContabilidade = Request.Params("PlanoContabilidade");
                string codigoCompanhia = Request.Params("CodigoCompanhia");

                if (repCentroResultado.ContemCentroResultado(0, plano))
                    return new JsonpResult(false, true, "Esta numeração de centro de resultado já está sendo utilizada.");

                if (plano.Length > 15)
                    return new JsonpResult(false, true, "Esta numeração de centro de resultado não está no formato ideal.");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = new()
                {
                    SegmentoVeiculo = codigoSegmentoVeiculo > 0 ? await repSegmentoVeiculo.BuscarPorCodigoAsync(codigoSegmentoVeiculo, true) : null,
                    Ativo = ativo,
                    Descricao = descricao,
                    Plano = plano,
                    PlanoContabilidade = planoContabilidade,
                    CodigoCompanhia = codigoCompanhia,
                    AnaliticoSintetico = tipo
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    centroResultado.Empresa = await repEmpresa.BuscarPorCodigoAsync(Usuario.Empresa.Codigo, cancellationToken);

                SalvarVeiculos(centroResultado, unitOfWork);
                SalvarTiposOperacao(centroResultado, unitOfWork);

                await repCentroResultado.InserirAsync(centroResultado, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoSegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo");

                bool ativo = Request.GetBoolParam("Ativo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico>("AnaliticoSintetico");

                string descricao = Request.Params("Descricao");
                string plano = Request.Params("Plano");
                string planoContabilidade = Request.Params("PlanoContabilidade");
                string codigoCompanhia = Request.Params("CodigoCompanhia");

                if (repCentroResultado.ContemCentroResultado(codigo, plano))
                    return new JsonpResult(false, true, "Esta numeração de centro de resultado já está sendo utilizada.");

                if (plano.Length > 15)
                    return new JsonpResult(false, true, "Esta numeração de centro de resultado não está no formato ideal.");

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = await repCentroResultado.BuscarPorCodigoAsync(codigo, true);

                await unitOfWork.StartAsync(cancellationToken);

                centroResultado.SegmentoVeiculo = codigoSegmentoVeiculo > 0 ? await repSegmentoVeiculo.BuscarPorCodigoAsync(codigoSegmentoVeiculo, true) : null;
                centroResultado.Ativo = ativo;
                centroResultado.Descricao = descricao;
                centroResultado.Plano = plano;
                centroResultado.PlanoContabilidade = planoContabilidade;
                centroResultado.CodigoCompanhia = codigoCompanhia;
                centroResultado.AnaliticoSintetico = tipo;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    centroResultado.Empresa = await repEmpresa.BuscarPorCodigoAsync(Usuario.Empresa.Codigo, true);

                SalvarVeiculos(centroResultado, unitOfWork);
                SalvarTiposOperacao(centroResultado, unitOfWork);

                await repCentroResultado.AtualizarAsync(centroResultado, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroPai = repCentroResultado.BuscarPlanoPai(centroResultado.Plano);

                var dynCentroResultado = new
                {
                    centroResultado.Codigo,
                    centroResultado.Descricao,
                    Plano = new
                    {
                        Codigo = centroResultado?.Plano ?? "",
                        Descricao = centroResultado?.Plano ?? ""
                    },
                    centroResultado.PlanoContabilidade,
                    centroResultado.CodigoCompanhia,
                    centroResultado.Ativo,
                    centroResultado.AnaliticoSintetico,
                    CentroResultadoSintetico = centroPai != null ? "(" + centroPai.Plano + ") - " + centroPai.Descricao : string.Empty,
                    SegmentoVeiculo = new
                    {
                        Codigo = centroResultado.SegmentoVeiculo?.Codigo ?? 0,
                        Descricao = centroResultado.SegmentoVeiculo?.Descricao ?? string.Empty
                    },
                    Veiculos = (from obj in centroResultado.Veiculos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Placa,
                                    ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao,
                                    obj.NumeroFrota
                                }).ToList(),
                    TiposOperacao = (from obj in centroResultado.TiposOperacao
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList()
                };

                return new JsonpResult(dynCentroResultado);
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
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorCodigo(codigo);
                repCentroResultado.Deletar(centroResultado, Auditado);
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

        public async Task<IActionResult> ProximaNumeracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                string plano = Request.Params("Plano");

                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                int tamanhoPlano = 0;
                if (plano.Length == 0)
                    tamanhoPlano = 1;
                else if (plano.Length == 1)
                    tamanhoPlano = 3;
                else if (plano.Length == 3)
                    tamanhoPlano = 6;
                else if (plano.Length == 6)
                    tamanhoPlano = 9;
                else if (plano.Length == 9)
                    tamanhoPlano = 12;
                else if (plano.Length == 12)
                    tamanhoPlano = 15;
                else
                    tamanhoPlano = 0;

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroPai = repCentroResultado.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> listaCentroResultados = repCentroResultado.BuscarProximoPlanoAnalitico(plano, tamanhoPlano);

                string proximoPlano = plano;
                if (listaCentroResultados.Count > 0 && !string.IsNullOrWhiteSpace(listaCentroResultados[0].Plano))
                    proximoPlano = listaCentroResultados[0].Plano;
                else
                {
                    if (proximoPlano.Length == 1)
                        proximoPlano = proximoPlano + ".0";
                    else
                        proximoPlano = proximoPlano + ".00";
                }

                if (proximoPlano.Length == 1)
                    proximoPlano = Convert.ToString(Convert.ToInt32(proximoPlano) + 1);
                else if (proximoPlano.Length == 3)
                    proximoPlano = proximoPlano.Substring(0, 2) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(2, 1)) + 1);
                else if (proximoPlano.Length == 6)
                    proximoPlano = proximoPlano.Substring(0, 4) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(4, 2)) + 1).PadLeft(2, '0');
                else if (proximoPlano.Length == 9)
                    proximoPlano = proximoPlano.Substring(0, 7) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(7, 2)) + 1).PadLeft(2, '0');
                else if (proximoPlano.Length == 12)
                    proximoPlano = proximoPlano.Substring(0, 10) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(10, 2)) + 1).PadLeft(2, '0');
                else if (proximoPlano.Length == 15)
                    proximoPlano = proximoPlano.Substring(0, 13) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(13, 2)) + 1).PadLeft(2, '0');
                else
                    proximoPlano = "";


                var dynRetorno = new
                {
                    Plano = proximoPlano,
                    CentroResultadoSintetico = centroPai != null ? "(" + centroPai.Plano + ") - " + centroPai.Descricao : string.Empty
                };
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a próxima numeração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCentroResultadoAnalitico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                decimal percentual = 0;
                decimal.TryParse(Request.Params("Percentual"), out percentual);

                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centro = repCentroResultado.BuscarPorCodigo(codigo);

                decimal percentualRetorno = percentual;
                List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centros = null;
                if (centro.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    centros = repCentroResultado.BuscarPlanoFilho(centro.Plano);
                    percentualRetorno = (percentualRetorno / centros.Count());
                }
                else
                {
                    centros = new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
                    centros.Add(centro);
                }

                var dynRetorno = new
                {
                    Centros = centros != null && centros.Count > 0 ? ((from obj in centros
                                                                       select new
                                                                       {
                                                                           obj.Codigo,
                                                                           CentroResultado = obj.Descricao,
                                                                           CodigoCentroResultado = obj.Codigo,
                                                                           Percentual = new { val = percentualRetorno.ToString("n2"), tipo = "decimal" }
                                                                       }).ToList()) : null
                };
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os centros analíticos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado()
            {
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                Descricao = Request.GetStringParam("Descricao"),
                Plano = Request.GetStringParam("Plano"),
                Ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos),
                Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico>("Tipo"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                CodigoUsuario = Request.GetBoolParam("SomenteDoUsuario") ? Usuario.Codigo : 0
            };
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            if (centroResultado.Veiculos == null)
                centroResultado.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic veiculo in veiculos)
                    codigos.Add((int)veiculo);

                List<Dominio.Entidades.Veiculo> veiculosDeletar = centroResultado.Veiculos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Veiculo veiculoDeletar in veiculosDeletar)
                    centroResultado.Veiculos.Remove(veiculoDeletar);
            }

            foreach (var veiculo in veiculos)
            {
                if (!centroResultado.Veiculos.Any(o => o.Codigo == (int)veiculo))
                {
                    Dominio.Entidades.Veiculo veic = repVeiculo.BuscarPorCodigo((int)veiculo);

                    centroResultado.Veiculos.Add(veic);
                }
            }
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            if (centroResultado.TiposOperacao == null)
                centroResultado.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacaoDeletar = centroResultado.TiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposOperacaoDeletar)
                    centroResultado.TiposOperacao.Remove(tipoOperacaoDeletar);
            }

            foreach (var tipoOperacao in tiposOperacao)
            {
                if (!centroResultado.TiposOperacao.Any(o => o.Codigo == (int)tipoOperacao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOp = repTipoOperacao.BuscarPorCodigo((int)tipoOperacao);

                    centroResultado.TiposOperacao.Add(tipoOp);
                }
            }
        }

        #endregion
    }
}
