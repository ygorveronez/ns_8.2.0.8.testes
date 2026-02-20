using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "BuscarAnexosCargaPorCodigo", "DownloadAnexo", "DownloadAnexoCarga", "AnexarArquivos" }, "Ocorrencias/AceiteDebitoAnexo")]
    public class AceiteDebitoAnexoController : BaseController
    {
        #region Construtores

        public AceiteDebitoAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo repAceiteDebitoAnexo = new Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int ocorrencia);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 7, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo> listaAceiteDebitoAnexo = repAceiteDebitoAnexo.Consultar(ocorrencia, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAceiteDebitoAnexo.ContarConsulta(ocorrencia));


                var lista = (from p in listaAceiteDebitoAnexo
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

        public async Task<IActionResult> BuscarAnexosCargaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos repCargaOcorrenciaSumarizadaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos(unitOfWork);

                int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos> listaAceiteDebitoAnexo = repCargaOcorrenciaSumarizadaAnexos.BuscarPorCodigoOcorrencia(ocorrencia);


                var lista = new
                {
                    Anexos = (from p in listaAceiteDebitoAnexo
                              select new
                              {
                                  p.Codigo,
                                  CodigoVeiculo = p.CargaSumarizado.Veiculo.Codigo,
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

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo repAceiteDebitoAnexo = new Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("Codigo"), out int codigo);

                if (!this.InsereArquivosOcorrencia(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo> anexos = repAceiteDebitoAnexo.BuscarPorCodigoAceite(codigo);

                var dynAnexos = from obj in anexos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo,
                                };

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo repAceiteDebitoAnexo = new Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo AceiteDebitoAnexo = repAceiteDebitoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (AceiteDebitoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(AceiteDebitoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, AceiteDebitoAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", AceiteDebitoAnexo.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo repAceiteDebitoAnexo = new Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo AceiteDebitoAnexo = repAceiteDebitoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (AceiteDebitoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (AceiteDebitoAnexo.AceiteDebito.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.AgAceite)
                    return new JsonpResult(false, "Situação do Aceite não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(AceiteDebitoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, AceiteDebitoAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, AceiteDebitoAnexo.AceiteDebito, null, "Excluiu o anexo " + AceiteDebitoAnexo.NomeArquivo + ".", unitOfWork);
                repAceiteDebitoAnexo.Deletar(AceiteDebitoAnexo);

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
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "AceiteDebito" });
        }

        private dynamic InsereArquivosOcorrencia(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo repAceiteDebitoAnexo = new Repositorio.Embarcador.Ocorrencias.AceiteDebitoAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite = repAceiteDebito.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (aceite == null)
            {
                erro = "Aceite não localizada para anexar arquivo.";
                return false;
            }

            if (aceite.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.AgAceite)
            {
                erro = "Situação do Aceite não permite anexar arquivos.";
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

                // Salva na pasta
                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                // Insere no banco
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo aceiteDebitoAnexo = new Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo
                {
                    AceiteDebito = aceite,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repAceiteDebitoAnexo.Inserir(aceiteDebitoAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, aceiteDebitoAnexo.AceiteDebito, null, "Adicionou o anexo " + aceiteDebitoAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
