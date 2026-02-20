using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Extensions;


namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "DownloadAnexo", "BuscarAnexosDescartePorCodigo" }, "WMS/DescarteLoteProdutoEmbarcador")]
    public class DescarteLoteProdutoEmbarcadorAnexoController : BaseController
    {
		#region Construtores

		public DescarteLoteProdutoEmbarcadorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 7, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo> anexos = repDescarteLoteProdutoEmbarcadorAnexo.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repDescarteLoteProdutoEmbarcadorAnexo.ContarConsulta(codigo));


                var lista = (from p in anexos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NomeArquivo
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAnexosDescartePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

                int.TryParse(Request.Params("DescarteLote"), out int descarte);

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo> anexos = repDescarteLoteProdutoEmbarcadorAnexo.BuscarPorDescarteLote(descarte);


                var lista = new
                {
                    Anexos = (from p in anexos
                              select new
                              {
                                  p.Codigo,
                                  p.Descricao,
                                  p.NomeArquivo
                              }).ToList()
                };

                return new JsonpResult(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

                int.TryParse(Request.Params("CodigoDescarteLote"), out int lote);

                if (!this.InsereArquivosDescarteLote(lote, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo> anexos = repDescarteLoteProdutoEmbarcadorAnexo.BuscarPorDescarteLote(lote);

                var dynAnexos = (from obj in anexos
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.Descricao,
                                     obj.NomeArquivo,
                                 }).ToList();

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo ao descarte.");
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
                // Repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo anexo = repDescarteLoteProdutoEmbarcadorAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, FileExtensions.ConvertArquivoRetorno(bArquivo, ""), anexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo anexo = repDescarteLoteProdutoEmbarcadorAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (!PodeAnexar(anexo.Lote))
                    return new JsonpResult(false, "Situação do Descarte não permite excluir arquivos.");

                anexo.Ativo = false;

                // Remove do banco
                unitOfWork.Start();
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Lote, null, "Inativou o anexo " + anexo.NomeArquivo + ".", unitOfWork);
                repDescarteLoteProdutoEmbarcadorAnexo.Atualizar(anexo);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "DescarteLote" });
        }

        private dynamic InsereArquivosDescarteLote(int descarteLote, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador repDescarteLoteProdutoEmbarcador = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo repDescarteLoteProdutoEmbarcadorAnexo = new Repositorio.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte = repDescarteLoteProdutoEmbarcador.BuscarPorCodigo(descarteLote);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (descarte == null)
            {
                erro = "Descarte não localizado para anexar arquivo.";
                return false;
            }

            if (!PodeAnexar(descarte))
            {
                erro = "Situação do Descarte não permite anexar arquivos.";
                return false;
            }

            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);
                string caminhoAbsoluto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo);

                // Salva na pasta
                file.SaveAs(caminhoAbsoluto);

                // Insere no banco
                Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo anexo = new Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcadorAnexo
                {
                    Lote = descarte,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo))),
                    Ativo = true
                };

                repDescarteLoteProdutoEmbarcadorAnexo.Inserir(anexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Lote, null, "Adicionou o anexo " + anexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }

        private bool PodeAnexar(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador lote)
        {
            return lote.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao || lote.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.SemRegra;
        }
    }
}
