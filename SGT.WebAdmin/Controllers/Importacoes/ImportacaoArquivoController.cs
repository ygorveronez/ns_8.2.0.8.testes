using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using Servicos.DTO;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    [AllowAnonymous]
    public class ImportacaoArquivoController : BaseController
    {
        #region Construtores

        public ImportacaoArquivoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> PesquisaConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Grid
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ordem", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                // Repositorios
                Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao repConfiguracaoImportacao = new Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao(unitOfWork);

                // Converte valores
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao controle;
                Enum.TryParse(Request.Params("CodigoControleImportacao"), out controle);

                string descricao = Request.Params("Descricao");
                if (string.IsNullOrWhiteSpace(descricao))
                    descricao = string.Empty;

                // Busca dados
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                List<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao> lista = repConfiguracaoImportacao.Consultar(controle, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoImportacao.ContarConsulta(controle, descricao);

                // Manipula retorno
                var listaGrid = from obj in lista
                                select new
                                {
                                    obj.Codigo,
                                    obj.Ordem,
                                    Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty
                                };

                // Vincula grid
                grid.AdicionaRows(listaGrid.ToList());
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna dados
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar configurações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ObterPrimeiraConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao repConfiguracaoImportacao = new Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao(unitOfWork);

                // Converte valores
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao controle;
                Enum.TryParse(Request.Params("CodigoControleImportacao"), out controle);

                Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao conf = repConfiguracaoImportacao.BuscarPorControle(controle);

                dynamic configuracao = null;

                if (conf != null)
                    configuracao = new
                    {
                        conf.Codigo,
                        conf.Ordem,
                        Descricao = !string.IsNullOrWhiteSpace(conf.Descricao) ? conf.Descricao : string.Empty
                    };

                return new JsonpResult(configuracao);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar configurações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> SalvarLogsErro()
        {
            string xhr = Request.Params("xhr");
            string status = Request.Params("status");
            string error = Request.Params("error");

            Servicos.Log.TratarErro("xhr: " + xhr + Environment.NewLine + "status: " + status + Environment.NewLine + "error: " + error, "LogImportacao");

            return new JsonpResult(true);
        }

        public async Task<IActionResult> SalvarConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao repConfiguracaoImportacao = new Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao(unitOfWork);
                Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao configuracao = new Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao();

                // Converte valores
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao controle;
                Enum.TryParse(Request.Params("CodigoControleImportacao"), out controle);

                string descricao = Request.Params("Descricao");
                if (string.IsNullOrWhiteSpace(descricao))
                    descricao = string.Empty;

                string ordem = Request.Params("Colunas");
                if (string.IsNullOrWhiteSpace(ordem))
                    ordem = string.Empty;

                // Valida
                if (string.IsNullOrWhiteSpace(descricao))
                    return new JsonpResult(true, false, "Descrição não pode ser vazio.");

                if (string.IsNullOrWhiteSpace(ordem)) // Se a ordem esta em branco, o plugin não enviou os dados corretamente
                    return new JsonpResult(true, false, "Ocorreu uma falha ao salvar a ordem dos campos.");

                // Inicia instancia
                unitOfWork.Start();

                // Adiciona dados
                configuracao.CodigoControle = controle;
                configuracao.Descricao = descricao;
                configuracao.Ordem = ordem;

                // Adiciona ao banco
                repConfiguracaoImportacao.Inserir(configuracao, Auditado);

                // Commita alteracoes
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao repConfiguracaoImportacao = new Repositorio.Embarcador.Importacoes.ConfiguracaoImportacao(unitOfWork);

                Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao configuracao = repConfiguracaoImportacao.BuscarPorCodigo(codigo, true);

                if (configuracao == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                unitOfWork.Start();

                repConfiguracaoImportacao.Deletar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConverterArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string erro = string.Empty;

                CustomFile file = HttpContext.GetFile();

                bool manterArquivoServidor = Request.GetBoolParam("ManterArquivoServidor");
                dynamic obj = Servicos.Embarcador.Importacao.Importacao.ConverterArquivo(file, out erro, !manterArquivoServidor, unitOfWork);

                if (obj == null && !string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                else if (obj == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao converter arquivo.");
                else if (obj.Content.Count == 0)
                    return new JsonpResult(false, true, "Nenhum dado recebido.");

                return new JsonpResult(obj);
            }

            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao converter arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }

}
