using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Relatorios
{
    [Area("Relatorios")]
    [AllowAnonymous]
    public class RelatorioController : BaseController
    {
        #region Construtores

        public RelatorioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> DesativarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorios = (Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios)int.Parse(Request.Params("CodigoControleRelatorios"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPorCodigo(codigo);

                if (ConfiguracaoEmbarcador.UsaPermissaoControladorRelatorios)
                {
                    bool permiteSalvarNovoRelatorio = this.Usuario.PerfilAcesso?.PermiteSalvarNovoRelatorio ?? this.Usuario.PermiteSalvarNovoRelatorio;
                    if (!permiteSalvarNovoRelatorio)
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);
                        return new JsonpResult(false, true, "Seu usuário não possui permissão para desativar um relatório.");
                    }
                }

                if (!relatorio.PadraoMultisoftware)
                {
                    if (relatorio.Padrao)
                    {
                        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioMultisoftware = repRelatorio.BuscarPadraoMultisoftware(CodigoControleRelatorios, TipoServicoMultisoftware);
                        relatorioMultisoftware.Padrao = true;
                        repRelatorio.Atualizar(relatorioMultisoftware);
                    }
                    relatorio.Ativo = false;
                    relatorio.Padrao = false;
                    relatorio.DataAlteracao = DateTime.Now;
                    relatorio.Usuario = this.Usuario;

                    repRelatorio.Atualizar(relatorio);
                    await unitOfWork.CommitChangesAsync(cancellationToken);
                    return new JsonpResult(true);
                }
                else
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return new JsonpResult(false, true, "Não é possível desativar o relatório padrão da Multisoftware.");
                }
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o relatorio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRelatorioGerado = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorio = await repRelatorioControleGeracao.BuscarPorCodigoAsync(codigoRelatorioGerado);

                if (Usuario == null)
                    return new JsonpResult(false, "Sem permissão de acesso");

                if (relatorio == null)
                    return new JsonpResult(false, "Relatório não está disponível para download.");

                if (relatorio?.Usuario?.Codigo != Usuario.Codigo)
                {
                    Servicos.Log.TratarErro($"Relatório código {codigoRelatorioGerado} - Usuário logado (código {Usuario.Codigo}) diferente do usuário do relatório (código {relatorio?.Usuario?.Codigo ?? 0}).", "Relatorio");
                    return new JsonpResult(false, "Sem permissão de acesso.");
                }

                if (relatorio.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado)
                {
                    string extencao = "";
                    if (relatorio.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
                        extencao = ".pdf";

                    if (relatorio.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                        extencao = ".xls";

                    if (relatorio.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.CSV)
                        extencao = ".csv";

                    string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), relatorio.GuidArquivo) + extencao;

                    Servicos.Log.TratarErro($"Caminho relatorio download: {caminhoArquivo}", "Relatorio");

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                    {
                        byte[] bufferRelatorio = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(caminhoArquivo, cancellationToken);

                        if (bufferRelatorio != null)
                        {
                            if (relatorio.Relatorio != null && relatorio.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R233_AFRMMControlMercante)
                            {
                                string caminhoArquivoASCII = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), relatorio.GuidArquivo) + "2" + extencao;

                                using (Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoArquivo))
                                using (StreamReader sr = new StreamReader(stream))
                                {
                                    await Utilidades.IO.FileStorageService.Storage.WriteLineAsync(caminhoArquivoASCII, await sr.ReadToEndAsync(), Encoding.ASCII, cancellationToken);
                                }

                                bufferRelatorio = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(caminhoArquivoASCII, default);

                                return Arquivo(bufferRelatorio, "application/pdf", "H6DDPCME" + extencao);
                            }
                            else
                                return Arquivo(bufferRelatorio, "application/" + extencao.Replace(".", ""), relatorio.Titulo.Replace(" ", "_") + "_" + relatorio.DataInicioGeracao.ToString("dd-MM-yyyy_HH-mm") + extencao);
                        }
                        else
                            return new JsonpResult(false, false, "Não foi possível gerar o DACTE, atualize a página e tente novamente.");

                    }
                    else
                    {
                        return new JsonpResult(true, false, "O arquivo não existe mais no servidor, por favor gere um novo relatório.");
                    }
                }
                else
                {
                    if (relatorio.SituacaoGeracaoRelatorio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar)
                        return new JsonpResult(true, false, "Ocorreu uma falha ao gerar esse relatório, por favor tente gerá-lo novamente.");
                    else
                        return new JsonpResult(true, false, "O arquivo não existe mais no servidor, por favor gere um novo relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                int codigoEmpresa = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) ? this.Empresa.Codigo : 0;
                Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorios = Request.GetEnumParam<Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios>("CodigoControleRelatorios");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoControleRelatorios", false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Relatorio.Descricao, "Descricao", 65, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Relatorio.RelatorioPadrao, "DescricaoPadrao", 20, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int totalRegistros = repositorioRelatorio.ContarConsulta(descricao, codigoControleRelatorios, TipoServicoMultisoftware, codigoEmpresa, this.Usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Relatorios.Relatorio> relatorios = (totalRegistros > 0) ? repositorioRelatorio.Consultar(descricao, codigoControleRelatorios, TipoServicoMultisoftware, codigoEmpresa, this.Usuario.Codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Relatorios.Relatorio>();

                var relatoriosRetornar = (
                    from o in relatorios
                    select new
                    {
                        o.Codigo,
                        o.CodigoControleRelatorios,
                        o.Descricao,
                        o.DescricaoPadrao
                    }
                ).ToList();

                grid.AdicionaRows(relatorios);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> SalvarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string jsonRelatorio = Request.Params("Relatorio");

                Models.ServicoRelatorio svcRelatorio = new Models.ServicoRelatorio();

                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = svcRelatorio.SalvarConfiguracaoRelatorio(jsonRelatorio, Usuario, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador.UsaPermissaoControladorRelatorios);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(codigoRelatorio);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Globais
    }
}

