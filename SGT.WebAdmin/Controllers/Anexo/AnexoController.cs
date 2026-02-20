using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Anexo
{
    public abstract class AnexoController<TAnexo, TEntidadeAnexo> : BaseController
        where TAnexo : Dominio.Entidades.Embarcador.Anexo.Anexo<TEntidadeAnexo>, new()
        where TEntidadeAnexo : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Construtores

        public AnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.RepositorioBase<TEntidadeAnexo> repositorioEntidade = new Repositorio.RepositorioBase<TEntidadeAnexo>(unitOfWork);
                TEntidadeAnexo entidade = repositorioEntidade.BuscarPorCodigo(codigo, auditavel: false);

                if (entidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!IsPermitirAdicionarAnexo(entidade))
                    return new JsonpResult(false, true, "Situação não permite adicionar arquivos.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                string caminho = ObterCaminhoArquivos(unitOfWork);

                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile arquivo = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                    TAnexo anexo = new TAnexo()
                    {
                        EntidadeAnexo = entidade,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName)))
                    };

                    PreecherInformacoesAdicionais(anexo, unitOfWork);

                    repositorioAnexo.Inserir(anexo, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, $"Adicionou o arquivo {anexo.NomeArquivo}.", unitOfWork);
                }

                List<TAnexo> anexos = repositorioAnexo.BuscarPorEntidade(entidade.Codigo);

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                        TipoAnexo = new { Codigo = 0, Descricao = string.Empty }
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                TAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                byte[] arquivoBinario = servicoAnexo.DownloadAnexo(anexo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                TAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!IsPermitirExcluirAnexo(anexo.EntidadeAnexo))
                    return new JsonpResult(false, true, "Não é permitido excluir o anexo.");

                Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork, Auditado);

                servicoAnexo.ExcluirAnexo(anexo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirTodosAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);

                TEntidadeAnexo entidadeAnexo = repositorioAnexo.BuscarEntidadePorCodigo(codigo);

                if (entidadeAnexo == null)
                    return new JsonpResult(false, true, "Anexo não encontrado.");

                if (!IsPermitirExcluirAnexo(entidadeAnexo))
                    return new JsonpResult(false, true, "Situação não permite deletar os anexos.");

                Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork, Auditado);

                unitOfWork.Start();

                servicoAnexo.ExcluirAnexos(entidadeAnexo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                List<TAnexo> anexos;

                if (codigo > 0)
                {
                    Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                    anexos = repositorioAnexo.BuscarPorEntidade(codigo);
                }
                else
                    anexos = new List<TAnexo>();

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, true);

                int codigo = Request.GetIntParam("Codigo");
                int totalRegistros = 0;
                List<TAnexo> anexos = null;

                if (codigo > 0)
                {
                    string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                    Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);

                    totalRegistros = repositorioAnexo.ContarConsulta(codigo);
                    anexos = (totalRegistros > 0) ? repositorioAnexo.Consultar(codigo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<TAnexo>();
                }
                else
                    anexos = new List<TAnexo>();

                var anexosRetornar = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                ).ToList();

                grid.AdicionaRows(anexosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os anexos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> DownloadAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<TAnexo> anexos;
            try
            {
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
                servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                anexos = repositorioAnexo.BuscarPorEntidade(codigo);


                if (anexos == null || anexos.Count == 0)
                    return new JsonpResult(false, true, "Registros não encontrados");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                foreach (var item in anexos)
                {
                    string caminho = servicoAnexo.ObterCaminhoArquivos(unitOfWork);
                    string extensao = System.IO.Path.GetExtension(item.NomeArquivo).ToLower();
                    string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, item.GuidArquivo + extensao);
                    string nomeArquivo = item.NomeArquivo + "_" + item.GuidArquivo + extensao;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    {
                        string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, item.NomeArquivo);
                        byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);
                        conteudoCompactar.Add(nomeArquivo, arquivoBinario);
                    }
                }

                if (conteudoCompactar?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar as imagens para realizar o download.");

                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

                arquivoCompactado.Dispose();

                if (arquivoCompactadoBinario == null)
                    return new JsonpResult(false, true, "Não foi possível gerar o arquivo.");

                return Arquivo(arquivoCompactadoBinario, "application/zip", $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Protegidos

        protected string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(TEntidadeAnexo).Name });
        }

        #endregion

        #region Métodos Protegidos com Permissão de Sobrescrita

        protected virtual bool IsPermitirAdicionarAnexo(TEntidadeAnexo entidade)
        {
            return true;
        }

        protected virtual bool IsPermitirExcluirAnexo(TEntidadeAnexo entidade)
        {
            return true;
        }

        protected virtual void PreecherInformacoesAdicionais(TAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {

        }

        #endregion
    }
}