using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualImpressaoController : BaseController
    {
		#region Construtores

		public CargaMDFeManualImpressaoController(Conexao conexao) : base(conexao) { }

		#endregion


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarDocumentosParaImpressao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeTrabalho.Start();
                int codigoCargaMDFeManual = 0;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);
                Servicos.Embarcador.Carga.Impressao serCargaImpressao = new Servicos.Embarcador.Carga.Impressao(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargaMDFeManual.CTes.Select(o => o.Carga).Distinct().ToList();
                cargas.AddRange(cargaMDFeManual.Cargas.Select(o => o).Distinct().ToList());
                cargas = cargas.Distinct().ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = (from obj in cargas where obj.Filial != null && (obj.Filial.NumeroUnidadeImpressao > 0 || !string.IsNullOrWhiteSpace(obj.NumeroImpressora) ) select obj).FirstOrDefault();
                
                if (carga != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Solicitou impressão dos documentos", unidadeTrabalho);
                    string retorno = serCargaImpressao.EnviarDocumentosParaImpressao(carga);

                    if (string.IsNullOrWhiteSpace(retorno))
                    {
                        serCargaImpressao.EnviarMDFeParaImpressao(carga);
                        unidadeTrabalho.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unidadeTrabalho.Rollback();
                        return new JsonpResult(false, true, retorno);
                    }
                }
                else
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Nenhuma das filiais das cargas possui a unidade de impressão configurada.");
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar ao enviar para impressão.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLotePDF()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaMDFeManual = 0;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download dos PDFs não está disponível. Contate o suporte técnico.");
                
                List<int> mdfes = (from obj in cargaMDFeManual.MDFeManualMDFes select obj.MDFe.Codigo).ToList();

                if (mdfes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFs(0, new List<int>(), mdfes, new List<int>(), unidadeTrabalho, TipoServicoMultisoftware);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(cargaMDFeManual.Codigo.ToString()), "_Documentos.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de PDF.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
