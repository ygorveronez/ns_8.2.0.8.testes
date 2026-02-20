using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Link
{
    
    public class LinkController : BaseController
    {
		#region Construtores

		public LinkController(Conexao conexao) : base(conexao) { }

		#endregion

        
        [AllowAnonymous]             
        public FileResult BuscarFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil configConfirmaFacil = new Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil(unitOfWork).Buscar();
            if (configConfirmaFacil == null || !configConfirmaFacil.PossuiIntegracao)
            {
                return null;
            }
            else 
            { 
                try
                {
                    string id = Request.QueryString.ToString();
                    if (string.IsNullOrEmpty(id))
                        return null;
                        
                    id = id.Substring(1);
                    string parametro = Servicos.Criptografia.Descriptografar(id, "CT3##MULT1@#$S0FTW4R3");

                    if (!string.IsNullOrWhiteSpace(parametro))
                    {
                        int codigo = parametro.ToInt();
                        Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo = repAnexo.BuscarPorCodigo(codigo, true);

                        if (anexo != null)
                        {
                            string caminhoOcorrencia = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");
                            string extensao,nome = "";
                            
                            if (!string.IsNullOrWhiteSpace(caminhoOcorrencia))
                            {                                
                                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoOcorrencia);
                                extensao = Path.GetExtension(caminhoOcorrencia);  
                                nome = Path.GetFileName(caminhoOcorrencia);
                                return File(arquivoBinario, $"application/{extensao}", nome);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return null;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
        }
    }
}
