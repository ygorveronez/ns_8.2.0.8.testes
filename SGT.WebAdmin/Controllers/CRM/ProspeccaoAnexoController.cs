using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace SGT.WebAdmin.Controllers.CRM
{
    [CustomAuthorize("CRM/Prospeccao")]
    public class ProspeccaoAnexoController : BaseController
    {
        #region Construtores

        public ProspeccaoAnexoController(Conexao conexao) : base(conexao) { }

        #endregion


        public async Task<IActionResult> PesquisaAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CRM.ProspeccaoAnexo repProspeccaoAnexo = new Repositorio.Embarcador.CRM.ProspeccaoAnexo(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, true);

                // Dados do filtro
                int.TryParse(Request.Params("Prospeccao"), out int codigo);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo> anexos = repProspeccaoAnexo.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProspeccaoAnexo.ContarConsulta(codigo);
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> AnexarArquivos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                await unitOfWork.StartAsync();

                // Repositorios
                Repositorio.Embarcador.CRM.Prospeccao repProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.CRM.ProspeccaoAnexo repProspeccaoAnexo = new Repositorio.Embarcador.CRM.ProspeccaoAnexo(unitOfWork, cancellationToken);

                // Busca Ocorrencia
                int.TryParse(Request.Params("Prospeccao"), out int codigo);

                Dominio.Entidades.Embarcador.CRM.Prospeccao prospeccao = await repProspeccao.BuscarPorCodigoAsync(codigo, false);

                // Valida
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (prospeccao == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (prospeccao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Pendente)
                    return new JsonpResult(false, "Situação não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = CaminhoArquivos(unitOfWork);

                    // Salva na pasta
                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    // Insere no banco
                    Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo anexo = new Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo
                    {
                        Prospeccao = prospeccao,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, prospeccao, null, "Adicionou o arquivo " + anexo.NomeArquivo + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);
                    await repProspeccaoAnexo.InserirAsync(anexo, Auditado);
                }

                // Commita
                await unitOfWork.CommitChangesAsync(cancellationToken);

                // Busca todos anexos
                List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo> anexos = await repProspeccaoAnexo.BuscarPorProspeccaoAsync(prospeccao.Codigo);

                // Retorna arquivos
                var dynAnexos = from obj in anexos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo
                                };

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadAnexo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.CRM.ProspeccaoAnexo repProspeccaoAnexo = new Repositorio.Embarcador.CRM.ProspeccaoAnexo(unitOfWork, cancellationToken);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo anexo = await repProspeccaoAnexo.BuscarPorCodigoAsync(codigo, false);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = await Utilidades.IO.FileStorageService.Storage.ReadAllBytesAsync(arquivo, cancellationToken);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirAnexo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                await unitOfWork.StartAsync();

                // Repositorios
                Repositorio.Embarcador.CRM.ProspeccaoAnexo repProspeccaoAnexo = new Repositorio.Embarcador.CRM.ProspeccaoAnexo(unitOfWork, cancellationToken);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo anexos = await repProspeccaoAnexo.BuscarPorCodigoAsync(codigo, true);

                // Valida
                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.Prospeccao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao.Pendente)
                    return new JsonpResult(false, "Situação não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, anexos.Prospeccao, null, "Removeu o arquivo " + anexos.NomeArquivo + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);
                await repProspeccaoAnexo.DeletarAsync(anexos, Auditado);

                // Commita
                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Prospeccao" });
        }
    }
}
