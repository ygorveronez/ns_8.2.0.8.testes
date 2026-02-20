using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "BuscarAnexosCargaPorCodigo", "DownloadAnexo", "DownloadAnexoCarga" }, "Ocorrencias/AutorizacaoOcorrencia", "Ocorrencias/Ocorrencia")]
    public class OcorrenciaAnexosController : BaseController
    {
        #region Construtores

        public OcorrenciaAnexosController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 7, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> listaCargaOcorrenciaAnexos = repCargaOcorrenciaAnexos.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaOcorrenciaAnexos.ContarConsulta(codigo));


                var lista = (from p in listaCargaOcorrenciaAnexos
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

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos> listaCargaOcorrenciaAnexos = repCargaOcorrenciaSumarizadaAnexos.BuscarPorCodigoOcorrencia(ocorrencia);


                var lista = new
                {
                    Anexos = (from p in listaCargaOcorrenciaAnexos
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
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

                // Busca Ocorrencia
                int codigo, cargaVinculada = 0;
                int.TryParse(Request.Params("CodigoOcorrencia"), out codigo);
                int.TryParse(Request.Params("CargaOcorrenciaVinculada"), out cargaVinculada);

                // Usa essa informacao para permitir inserir anexos numa ocorrencia finalizada
                string hash = !string.IsNullOrWhiteSpace(Request.Params("HashAnexos")) ? Request.Params("HashAnexos") : string.Empty;
                string erro;

                if (!this.InsereArquivosOcorrencia(codigo, hash, unitOfWork, out erro))
                {
                    if (erro != "Nenhum arquivo selecionado para envio.")
                        throw new Exception(erro); //return new JsonpResult(false, erro);
                }

                if (cargaVinculada > 0)
                    if (!this.InsereArquivosOcorrencia(cargaVinculada, hash, unitOfWork, out erro))
                        throw new Exception(erro); //return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> ocorrenciasAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(codigo);

                var dynAnexos = from obj in ocorrenciasAnexos
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
                Servicos.Log.TratarErro(ex, "AnexoOcorrencia");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                    int.TryParse(Request.Params("CodigoOcorrencia"), out int codigo);
                    int.TryParse(Request.Params("CargaOcorrenciaVinculada"), out int codigoVinculada);
                    if (codigo > 0)
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);
                        if (ocorrencia != null && ocorrencia.Carga != null && ocorrencia.TipoOcorrencia != null && ocorrencia.TipoOcorrencia.AnexoObrigatorio)
                        {
                            ocorrencia.Inativa = true;
                            ocorrencia.ObservacaoCancelamento = "FALHA AO ENVIAR ANEXOS, OCORRENCIA INATIVADA";
                            repCargaOcorrencia.Atualizar(ocorrencia);
                        }
                    }
                    if (codigoVinculada > 0)
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoVinculada);
                        if (ocorrencia != null && ocorrencia.Carga != null && ocorrencia.TipoOcorrencia != null && ocorrencia.TipoOcorrencia.AnexoObrigatorio)
                        {
                            ocorrencia.Inativa = true;
                            ocorrencia.ObservacaoCancelamento = "FALHA AO ENVIAR ANEXOS, OCORRENCIA INATIVADA";
                            repCargaOcorrencia.Atualizar(ocorrencia);
                        }
                    }
                    return new JsonpResult(false, "Não foi possível enviar anexo, favor lançar ocorrência novamente.");
                }
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnexarArquivosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("CodigoOcorrencia"), out int ocorrencia);
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigo);

                // Usa essa informacao para permitir inserir anexos numa ocorrencia finalizada
                string hash = !string.IsNullOrWhiteSpace(Request.Params("HashAnexos")) ? Request.Params("HashAnexos") : string.Empty;

                if (!this.InsereArquivosOcorrenciaCarga(codigo, ocorrencia, hash, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos> ocorrenciasAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(ocorrencia);

                var dynAnexos = (from obj in ocorrenciasAnexos
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoVeiculo = obj.CargaSumarizado.Veiculo.Codigo,
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
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos cargaOcorrenciaAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (cargaOcorrenciaAnexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(cargaOcorrenciaAnexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaAnexos.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", cargaOcorrenciaAnexos.NomeArquivo);
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

        public async Task<IActionResult> DownloadAnexoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos repCargaOcorrenciaSumarizadaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos cargaOcorrenciaAnexos = repCargaOcorrenciaSumarizadaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (cargaOcorrenciaAnexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(cargaOcorrenciaAnexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaAnexos.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", cargaOcorrenciaAnexos.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos cargaOcorrenciaAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (cargaOcorrenciaAnexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(cargaOcorrenciaAnexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaAnexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrenciaAnexos.CargaOcorrencia, null, "Excluiu o anexo " + cargaOcorrenciaAnexos.NomeArquivo + ".", unitOfWork);
                repCargaOcorrenciaAnexos.Deletar(cargaOcorrenciaAnexos);

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
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia");

            return caminho;
        }

        private dynamic InsereArquivosOcorrencia(int codigo, string hash, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (cargaOcorrencia == null)
            {
                erro = "Ocorrência não localizada para anexar arquivo.";
                return false;
            }

            if (cargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && cargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao && cargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes)
            {
                if (!hash.Equals(cargaOcorrencia.DataAlteracao.ToString("ddMMyyyyHHmm")))
                {
                    erro = "Situação da Ocorrência não permite anexar arquivos.";
                    return false;
                }
            }

            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);

                string fullPath = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo);
                // Salva na pasta
                file.SaveAs(fullPath);
                if (!file.IsFileSaved(fullPath))
                    return false;

                // Insere no banco
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos cargaOcorrenciaAnexos = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos();

                cargaOcorrenciaAnexos.CargaOcorrencia = cargaOcorrencia;
                cargaOcorrenciaAnexos.Descricao = i < descricoes.Length ? descricoes[i] : string.Empty;
                cargaOcorrenciaAnexos.GuidArquivo = guidArquivo;
                cargaOcorrenciaAnexos.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));

                repCargaOcorrenciaAnexos.Inserir(cargaOcorrenciaAnexos);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrenciaAnexos.CargaOcorrencia, null, "Adicionou o anexo " + cargaOcorrenciaAnexos.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }

        private dynamic InsereArquivosOcorrenciaCarga(int codigo, int ocorrencia, string hash, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos repCargaOcorrenciaSumarizadaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado cargaSumarizado = repCargaOcorrenciaSumarizado.BuscarPorCodigoVeiculoEOcorrencia(codigo, ocorrencia);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (cargaSumarizado == null)
            {
                erro = "Ocorrência não localizada para anexar arquivo.";
                return false;
            }

            if (cargaSumarizado.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && cargaSumarizado.CargaOcorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao)
            {
                if (!hash.Equals(cargaSumarizado.CargaOcorrencia.DataAlteracao.ToString("ddMMyyyyHHmm")))
                {
                    erro = "Situação da Ocorrência não permite anexar arquivos.";
                    return false;
                }
            }

            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);
                string fullPath = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo);
                // Salva na pasta
                file.SaveAs(fullPath);
                if (!file.IsFileSaved(fullPath))
                    return false;

                // Insere no banco
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos cargaOcorrenciaAnexos = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizadaAnexos();

                cargaOcorrenciaAnexos.CargaSumarizado = cargaSumarizado;
                cargaOcorrenciaAnexos.Descricao = i < descricoes.Length ? descricoes[i] : string.Empty;
                cargaOcorrenciaAnexos.GuidArquivo = guidArquivo;
                cargaOcorrenciaAnexos.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));

                repCargaOcorrenciaSumarizadaAnexos.Inserir(cargaOcorrenciaAnexos);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrenciaAnexos.CargaSumarizado, null, "Adicionou o anexo " + cargaOcorrenciaAnexos.NomeArquivo + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrenciaAnexos.CargaSumarizado.CargaOcorrencia, null, "Adicionou o anexo " + cargaOcorrenciaAnexos.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
