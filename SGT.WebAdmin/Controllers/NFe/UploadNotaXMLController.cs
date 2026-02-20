using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFe
{
    public class UploadNotaXMLController : BaseController
    {
		#region Construtores

		public UploadNotaXMLController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Metodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> ProcessarXmlParaVinculacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Servicos.Embarcador.Pedido.NotaFiscal servicoNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.NFe.NFe servicoNFE = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                    Stream nota = file.InputStream;

                    if (extensao != ".xml")
                        throw new ControllerException("A extensão do arquivo é inválida.");

                    var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(nota);

                    if (objetoNFe == null)
                        throw new ControllerException("O xml informado não é uma NF-e, por favor verifique.");
                    System.IO.StreamReader stReaderXML = new StreamReader(nota);
                    string xmlNota = stReaderXML.ReadToEnd();
                    bool retorno = new Servicos.Embarcador.NFe.NFe(unitOfWork).BuscarDadosNotaFiscal(out string erro, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, configuracaoTMS.GerarCargaDeNotasRecebidasPorEmail, false, TipoServicoMultisoftware, configuracaoTMS.ImportarEmailCliente, configuracaoTMS.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false);

                    if (!retorno)
                        throw new ControllerException(erro);

                    if (xmlNotaFiscal.Codigo > 0)
                        repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                    else
                        repositorioXmlNotaFiscal.Inserir(xmlNotaFiscal);

                    servicoNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, TipoServicoMultisoftware, Auditado, true, false);
                    servicoNFE.SalvarProdutosNota(xmlNota, xmlNotaFiscal, Auditado, TipoServicoMultisoftware, unitOfWork);

                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = null;
                    Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                    if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave))
                        documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChave(xmlNotaFiscal.Chave);

                    if (documentoDestinado == null)
                    {
                        System.IO.StreamReader stReaderXML2 = new StreamReader(nota);
                        servicoNFE.BuscarDadosNotaFiscalDestinada(stReaderXML2, unitOfWork);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Nota enviada com sucesso");
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);

            }
            catch (Exception exception)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, " xml não está no padrão correto");
            }
        }

        #endregion

        #region Metodos Privados

        #endregion
    }
}
