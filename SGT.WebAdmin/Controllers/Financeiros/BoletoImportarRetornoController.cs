using System;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using SGTAdmin.Controllers;
using System.Collections.Generic;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/BoletoImportarRetorno")]
    public class BoletoImportarRetornoController : BaseController
    {
        #region Construtores

        public BoletoImportarRetornoController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                DataSet ds = new DataSet();
                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];

                    if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosImportacaoBoleto))
                        return new JsonpResult(false, "Não está configurado o caminho para importação do retorno, favor contate o suporte!");

                    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosImportacaoBoleto;
                    
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString() + Path.GetFileName(file.FileName));
                    file.SaveAs(caminho);

                    Repositorio.Embarcador.Financeiro.BoletoRetornoArquivo repBoletoRetornoArquivo = new Repositorio.Embarcador.Financeiro.BoletoRetornoArquivo(unitOfWork);
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo boleto = new Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoArquivo()
                    {
                        Arquivo = caminho,
                        BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Nenhum,
                        DataGeracao = DateTime.Now,
                        Usuario = this.Usuario
                    };
                    repBoletoRetornoArquivo.Inserir(boleto);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, "Sucesso");
                }
                else
                {
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o arquivo. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
