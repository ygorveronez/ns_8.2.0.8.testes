using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoTMS", "Chamados/ControleChamadoTMS")]
    public class ChamadoTMSAnexosController : BaseController
    {
        #region Construtores

        public ChamadoTMSAnexosController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        #region Anexos Abertura
        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexo repChamadoTMSAnexo = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexo(unitOfWork);

                int.TryParse(Request.Params("Chamado"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (chamado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (chamado.Situacao == SituacaoChamadoTMS.Finalizado || chamado.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || chamado.Situacao == SituacaoChamadoTMS.Cancelado || chamado.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, true, "Situação do chamado não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexo()
                    {
                        EntidadeAnexo = chamado,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    repChamadoTMSAnexo.Inserir(chamadoAnexo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, chamadoAnexo.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexo> chamadoAnexos = repChamadoTMSAnexo.BuscarPorChamado(chamado.Codigo);

                var dynAnexos = from obj in chamadoAnexos
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexo repChamadoTMSAnexo = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexo anexo = repChamadoTMSAnexo.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMSAnexo repChamadoTMSAnexo = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexo anexos = repChamadoTMSAnexo.BuscarPorCodigo(codigo, false);

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Finalizado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Cancelado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, "Situação do chamado não permite excluir arquivos.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.EntidadeAnexo, null, anexos.NomeArquivo + " excluído", unitOfWork);

                repChamadoTMSAnexo.Deletar(anexos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Anexos Email Autorização Cliente
        public async Task<IActionResult> AnexarArquivosAutorizacaoCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail repChamadoTMSAnexoEmail = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail(unitOfWork);

                int.TryParse(Request.Params("Chamado"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (chamado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (chamado.Situacao == SituacaoChamadoTMS.Finalizado || chamado.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || chamado.Situacao == SituacaoChamadoTMS.Cancelado || chamado.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, true, "Situação do chamado não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail chamadoAnexoEmail = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail()
                    {
                        EntidadeAnexo = chamado,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    repChamadoTMSAnexoEmail.Inserir(chamadoAnexoEmail);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, chamadoAnexoEmail.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail> chamadoAnexos = repChamadoTMSAnexoEmail.BuscarPorChamado(chamado.Codigo);

                var dynAnexos = from obj in chamadoAnexos
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexoAutorizacaoCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail repChamadoTMSAnexoEmail = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail anexo = repChamadoTMSAnexoEmail.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexoAutorizacaoCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail repChamadoTMSAnexoEmail = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail anexos = repChamadoTMSAnexoEmail.BuscarPorCodigo(codigo, false);

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Finalizado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Cancelado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, "Situação do chamado não permite excluir arquivos.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.EntidadeAnexo, null, anexos.NomeArquivo + " excluído", unitOfWork);

                repChamadoTMSAnexoEmail.Deletar(anexos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Anexos Adiantamento Motorista
        public async Task<IActionResult> AnexarArquivosAdiantamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista repChamadoTMSAnexoAdiantamentoMotorista = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista(unitOfWork);

                int.TryParse(Request.Params("Chamado"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (chamado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (chamado.Situacao == SituacaoChamadoTMS.Finalizado || chamado.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || chamado.Situacao == SituacaoChamadoTMS.Cancelado || chamado.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, true, "Situação do chamado não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    file.SaveAs(caminho + guidArquivo + extensaoArquivo);

                    Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista chamadoAnexoAdiantamentoMotorista = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista()
                    {
                        EntidadeAnexo = chamado,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    repChamadoTMSAnexoAdiantamentoMotorista.Inserir(chamadoAnexoAdiantamentoMotorista);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, chamadoAnexoAdiantamentoMotorista.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista> chamadoAnexos = repChamadoTMSAnexoAdiantamentoMotorista.BuscarPorChamado(chamado.Codigo);

                var dynAnexos = from obj in chamadoAnexos
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexoAdiantamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista repChamadoTMSAnexoAdiantamentoMotorista = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista anexo = repChamadoTMSAnexoAdiantamentoMotorista.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexoAdiantamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista repChamadoTMSAnexoAdiantamentoMotorista = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoAdiantamentoMotorista anexos = repChamadoTMSAnexoAdiantamentoMotorista.BuscarPorCodigo(codigo, false);

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Finalizado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Cancelado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, "Situação do chamado não permite excluir arquivos.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.EntidadeAnexo, null, anexos.NomeArquivo + " excluído", unitOfWork);

                repChamadoTMSAnexoAdiantamentoMotorista.Deletar(anexos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Anexos Documento de Análise
        public async Task<IActionResult> AnexarArquivosDocumentoAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise repChamadoTMSAnexoDocumentoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise(unitOfWork);

                int.TryParse(Request.Params("Chamado"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (chamado == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (chamado.Situacao == SituacaoChamadoTMS.Finalizado || chamado.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || chamado.Situacao == SituacaoChamadoTMS.Cancelado || chamado.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, true, "Situação do chamado não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise chamadoAnexoDocumentoAnalise = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise()
                    {
                        EntidadeAnexo = chamado,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    repChamadoTMSAnexoDocumentoAnalise.Inserir(chamadoAnexoDocumentoAnalise);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, chamadoAnexoDocumentoAnalise.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise> chamadoAnexos = repChamadoTMSAnexoDocumentoAnalise.BuscarPorChamado(chamado.Codigo);

                var dynAnexos = from obj in chamadoAnexos
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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexoDocumentoAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise repChamadoTMSAnexoDocumentoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise anexo = repChamadoTMSAnexoDocumentoAnalise.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexoDocumentoAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise repChamadoTMSAnexoDocumentoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise anexos = repChamadoTMSAnexoDocumentoAnalise.BuscarPorCodigo(codigo, false);

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Finalizado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.LiberadaOcorrencia
                    || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.Cancelado || anexos.EntidadeAnexo.Situacao == SituacaoChamadoTMS.PagamentoNaoAutorizado)
                    return new JsonpResult(false, "Situação do chamado não permite excluir arquivos.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.EntidadeAnexo, null, anexos.NomeArquivo + " excluído", unitOfWork);

                repChamadoTMSAnexoDocumentoAnalise.Deletar(anexos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #endregion

        #region Métodos Privados

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "ChamadoTMS");

            return caminho;
        }

        #endregion
    }
}
