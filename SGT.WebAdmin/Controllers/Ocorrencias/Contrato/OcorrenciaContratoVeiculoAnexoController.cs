using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "BuscarAnexosVeiculoContratoPorCodigo", "DownloadAnexo" }, "Ocorrencias/Ocorrencia", "Ocorrencias/AutorizacaoOcorrencia")]
    public class OcorrenciaContratoVeiculoAnexoController : BaseController
    {
        #region Construtores

        public OcorrenciaContratoVeiculoAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 7, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo> listaCargaOcorrenciaAnexos = repCargaOcorrenciaAnexos.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
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

        public async Task<IActionResult> BuscarAnexosVeiculoContratoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repOcorrenciaContratoVeiculoAnexo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);

                int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo> listaVeiculoContratoOcorrenciaAnexos = repOcorrenciaContratoVeiculoAnexo.BuscarPorCodigoOcorrencia(ocorrencia);


                var lista = new
                {
                    Anexos = (from p in listaVeiculoContratoOcorrenciaAnexos
                              select new
                              {
                                  p.Codigo,
                                  CodigoVeiculo = p.OcorrenciaContratoVeiculo.Veiculo.Codigo,
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
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repVeiculoContratoOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("CodigoOcorrencia"), out int ocorrencia);
                int.TryParse(Request.Params("CodigoVeiculo"), out int veiculo);

                // Usa essa informacao para permitir inserir anexos numa ocorrencia finalizada
                string hash = !string.IsNullOrWhiteSpace(Request.Params("HashAnexos")) ? Request.Params("HashAnexos") : string.Empty;

                if (!this.InsereArquivosOcorrenciaVeiculoContrato(veiculo, ocorrencia, hash, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo> ocorrenciasAnexos = repVeiculoContratoOcorrenciaAnexos.BuscarPorCodigoOcorrencia(ocorrencia);

                var dynAnexos = (from obj in ocorrenciasAnexos
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoVeiculo = obj.OcorrenciaContratoVeiculo.Veiculo.Codigo,
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
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repOcorrenciaContratoVeiculoAnexo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo contratoVeiculoAnexo = repOcorrenciaContratoVeiculoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (contratoVeiculoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(contratoVeiculoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, contratoVeiculoAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", contratoVeiculoAnexo.NomeArquivo);
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
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repOcorrenciaContratoVeiculoAnexo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo contratoVeiculoAnexo = repOcorrenciaContratoVeiculoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (contratoVeiculoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (contratoVeiculoAnexo.OcorrenciaContratoVeiculo.Ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao)
                    return new JsonpResult(false, "Situação da Ocorrência não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(contratoVeiculoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, contratoVeiculoAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoVeiculoAnexo.OcorrenciaContratoVeiculo.Ocorrencia, null, "Excluiu o anexo " + contratoVeiculoAnexo.NomeArquivo + ".", unitOfWork);
                repOcorrenciaContratoVeiculoAnexo.Deletar(contratoVeiculoAnexo, Auditado);

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

        private dynamic InsereArquivosOcorrenciaVeiculoContrato(int veiculo, int ocorrencia, string hash, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo repOcorrenciaContratoVeiculoAnexo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo repOcorrenciaContratoVeiculo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo contratoVeiculo = repOcorrenciaContratoVeiculo.BuscarPorCodigoVeiculoEOcorrencia(veiculo, ocorrencia);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (contratoVeiculo == null)
            {
                erro = "Ocorrência não localizada para anexar arquivo.";
                return false;
            }

            if (contratoVeiculo.Ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && contratoVeiculo.Ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao)
            {
                if (!hash.Equals(contratoVeiculo.Ocorrencia.DataAlteracao.ToString("ddMMyyyyHHmm")))
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

                // Salva na pasta
                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                // Insere no banco
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo contratoVeiculoAnexo = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculoAnexo
                {
                    OcorrenciaContratoVeiculo = contratoVeiculo,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repOcorrenciaContratoVeiculoAnexo.Inserir(contratoVeiculoAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoVeiculoAnexo.OcorrenciaContratoVeiculo.Ocorrencia, null, "Adicionou o anexo " + contratoVeiculoAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
