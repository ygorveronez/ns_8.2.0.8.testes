using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CargaJanelaCarregamentoChecklist", "Logistica/JanelaCarregamentoTransportador")]
    public class CargaJanelaCarregamentoChecklistController : BaseController
    {
        #region Construtores

        public CargaJanelaCarregamentoChecklistController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> SalvarChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklist repChecklist = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklist(unitOfWork);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento == null)
                    throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoEncontrada);

                dynamic dynChecklist = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Checklist"));

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist checklist = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist();
                checklist.CargaJanelaCarregamento = cargaJanelaCarregamento;

                int codigo = int.Parse((string)dynChecklist?.Codigo);
                bool inserir = codigo == 0;

                if (!inserir)
                    checklist = repChecklist.BuscarPorCodigo(codigo, false);
                else
                    repChecklist.Inserir(checklist);

                checklist.UltimaCarga = PreencherChecklistCarga(dynChecklist.UltimaCarga, checklist, unitOfWork);
                checklist.PenultimaCarga = PreencherChecklistCarga(dynChecklist.PenultimaCarga, checklist, unitOfWork);
                checklist.AntepenultimaCarga = PreencherChecklistCarga(dynChecklist.AntepenultimaCarga, checklist, unitOfWork);

                repChecklist.Atualizar(checklist);
                unitOfWork.Flush();

                if (!InsereArquivosChecklistCarga(checklist.UltimaCarga.Codigo, unitOfWork, HttpContext.GetFiles("ArquivoUltima"), Request.TryGetArrayParam<string>("DescricaoUltima"), out string erro) && inserir)
                    throw new ControllerException(erro);

                if (!InsereArquivosChecklistCarga(checklist.PenultimaCarga.Codigo, unitOfWork, HttpContext.GetFiles("ArquivoPenultima"), Request.TryGetArrayParam<string>("DescricaoPenultima"), out erro) && inserir)
                    throw new ControllerException(erro);

                if (!InsereArquivosChecklistCarga(checklist.AntepenultimaCarga.Codigo, unitOfWork, HttpContext.GetFiles("ArquivoAntepenultima"), Request.TryGetArrayParam<string>("DescricaoAntepenultima"), out erro) && inserir)
                    throw new ControllerException(erro);

                cargaJanelaCarregamento.CargaJanelaCarregamentoChecklist = checklist;
                repCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.SalvouChecklist + ".", unitOfWork);

                return new JsonpResult(new { Checklist = servicoCargaJanelaCarregamento.ObterChecklist(cargaJanelaCarregamento.Codigo, unitOfWork) }, true, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ChecklistSalvoComSucesso);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoChecklist");
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoChecklist");
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarChecklist);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos repChecklistAnexos = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos(unitOfWork);

                int.TryParse(Request.Params("ChecklistCargaVinculada"), out int codigo);

                unitOfWork.Start();

                if (!InsereArquivosChecklistCarga(codigo, unitOfWork, HttpContext.GetFiles("Arquivo"), Request.TryGetArrayParam<string>("Descricao"), out string erro))
                    throw new ControllerException(erro);

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos> checklistAnexos = repChecklistAnexos.BuscarPorCargaJanelaCarregamentoChecklistCarga(codigo);

                return new JsonpResult(new
                {
                    Anexos = from obj in checklistAnexos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.NomeArquivo,
                             }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoChecklist");
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAnexarArquivo);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos repChecklistAnexos = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos anexo = repChecklistAnexos.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoBuscarOsDados);

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarAnexo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
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
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos repChecklistAnexos = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos anexo = repChecklistAnexos.BuscarPorCodigo(codigo, false);

                if (anexo == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoBuscarOsDados);

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoDeletarAnexo);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.CargaJanelaCarregamentoChecklistCarga.CargaJanelaCarregamentoChecklist.CargaJanelaCarregamento, null, Localization.Resources.Transportadores.Transportador.ExcluiuAnexo + anexo.NomeArquivo + ".", unitOfWork);
                repChecklistAnexos.Deletar(anexo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaJanelaCarregamentoChecklistCarga");

            return caminho;
        }

        private bool InsereArquivosChecklistCarga(int codigo, Repositorio.UnitOfWork unitOfWork, IList<Servicos.DTO.CustomFile> arquivos, string[] descricoes, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga repChecklistCarga = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos repChecklistAnexos = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga cargaJanelaCarregamentoChecklistCarga = repChecklistCarga.BuscarPorCodigo(codigo, false);

            if (arquivos.Count <= 0)
            {
                erro = Localization.Resources.Gerais.Geral.NenhumArqSelecionado;
                return false;
            }

            if (cargaJanelaCarregamentoChecklistCarga == null)
            {
                erro = Localization.Resources.Logistica.JanelaCarregamentoTransportador.ChecklistCargaNaoLocalizadaParaAnexarArquivo;
                return false;
            }

            for (int i = 0; i < arquivos.Count(); i++)
            {
                Servicos.DTO.CustomFile file = arquivos[i];
                string nomeArquivo = file.FileName;
                string extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);

                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos checklistAnexos = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCargaAnexos
                {
                    CargaJanelaCarregamentoChecklistCarga = cargaJanelaCarregamentoChecklistCarga,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repChecklistAnexos.Inserir(checklistAnexos);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, checklistAnexos.CargaJanelaCarregamentoChecklistCarga.CargaJanelaCarregamentoChecklist.CargaJanelaCarregamento, null, Localization.Resources.Transportadores.Transportador.AdicionouAnexo + checklistAnexos.NomeArquivo + " " + Localization.Resources.Logistica.JanelaCarregamentoTransportador.Checklist + ".", unitOfWork);
            }

            return true;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga PreencherChecklistCarga(dynamic dynChecklistCarga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklist checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga repChecklistCarga = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga checklistCarga = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga();


            int codigo = int.Parse((string)dynChecklistCarga?.Codigo);

            if (codigo > 0)
                checklistCarga = repChecklistCarga.BuscarPorCodigo(codigo, false);

            DateTime.TryParseExact((string)dynChecklistCarga.Data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime data);
            checklistCarga.DataChecklist = data;
            checklistCarga.Codigo = int.Parse((string)dynChecklistCarga?.Codigo);

            if (checklistCarga.Reboques?.Count > 0)
                checklistCarga.Reboques.Clear();
            else
                checklistCarga.Reboques = new List<Dominio.Entidades.Veiculo>();

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(int.Parse((string)dynChecklistCarga?.Veiculo));
            if (veiculo == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoInformadoNaoFoiLocalizado);
            checklistCarga.Veiculo = veiculo;

            Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(int.Parse((string)dynChecklistCarga?.Reboque));
            if (reboque == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoInformadoNaoFoiLocalizado);
            checklistCarga.Reboques.Add(reboque);

            Dominio.Entidades.Veiculo segundoReboque = repVeiculo.BuscarPorCodigo(int.Parse((string)dynChecklistCarga?.SegundoReboque));
            if (segundoReboque == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoInformadoNaoFoiLocalizado);
            checklistCarga.Reboques.Add(segundoReboque);

            Dominio.Entidades.Veiculo terceiroReboque = repVeiculo.BuscarPorCodigo(int.Parse((string)dynChecklistCarga?.TerceiroReboque));
            if (terceiroReboque == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.OVeiculoInformadoNaoFoiLocalizado);
            checklistCarga.Reboques.Add(terceiroReboque);

            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(int.Parse((string)dynChecklistCarga?.GrupoProduto));
            if (grupoProduto == null)
                throw new ControllerException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.GrupoProdutoNaoEncontrado);
            checklistCarga.GrupoProduto = grupoProduto;

            checklistCarga.RegimeLimpeza = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumRegimeLimpeza)int.Parse((string)dynChecklistCarga?.RegimeLimpeza);

            if (codigo > 0)
                repChecklistCarga.Atualizar(checklistCarga);
            else
            {
                checklistCarga.CargaJanelaCarregamentoChecklist = checklist;
                repChecklistCarga.Inserir(checklistCarga);
            }


            return checklistCarga;
        }
    }
}
