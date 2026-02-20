using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;
using Dominio.ObjetosDeValor.Embarcador.PortalCliente;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using SGTAdmin.Controllers;
using System.Net;

namespace SGT.WebAdmin.Controllers.PortalCliente
{
    public class PortalClienteController : BaseController
    {
        public PortalClienteController(Conexao conexao) : base(conexao) { }

        public async Task<IActionResult> Index(string token)
        {
            string caminhoBaseViews = "~/Views/PortalCliente/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {

                    PortalClienteView dataTitulo = ObterDadosRenderizacao(token, unitOfWork);

                    if (dataTitulo == null)
                        return View(caminhoBaseViews + "Erro.cshtml");

                    return View(caminhoBaseViews + "Detalhe.cshtml", dataTitulo);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "Erro.cshtml");
            }

        }

        public async Task<IActionResult> DownloadBoleto([FromBody] PortalClienteDownloadDocumentos parametro)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (parametro.CodigoTitulo <= 0)
                    return new JsonpResult(false, "Título não encontrado");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(parametro.CodigoTitulo);

                if (titulo == null || string.IsNullOrWhiteSpace(titulo.CaminhoBoleto))
                    return new JsonpResult(false, "Este título não possui PDF disponível para download.");

                if (Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto), "application/pdf", titulo.Codigo.ToString() + ".pdf");
                else
                    return new JsonpResult(false, "O arquivo do boleto " + titulo.CaminhoBoleto + " não foi encontrado.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF do boleto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDanfe([FromBody] PortalClienteDownloadDocumentos parametros)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (parametros == null || string.IsNullOrWhiteSpace(parametros.ChaveNfe))
                    return new JsonpResult(false, false, "Não foi encontrado o arquivo PDF da nota selecionada.");

                string caminhoDanfe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

                var caminhoArquivoDanfe = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDanfe, parametros.ChaveNfe) + ".pdf";
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoDanfe))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivoDanfe), "application/pdf", parametros.ChaveNfe + ".pdf");
                else
                    return new JsonpResult(false, "Não foi encontrado o arquivo DANFE da nota selecionada.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da DANFE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXML([FromBody] PortalClienteDownloadDocumentos parametros)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repositorioNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos xmlNotaFiscal = repositorioNotaFiscalArquivos.BuscarPorNota(parametros.CodigoNotaFiscal); 

                if (xmlNotaFiscal == null || string.IsNullOrWhiteSpace(xmlNotaFiscal.XMLDistribuicao))
                    return new JsonpResult(false, "Não foi encontrado o arquivo XML da nota selecionada.");

                byte[] xml = System.Text.Encoding.UTF8.GetBytes(xmlNotaFiscal.XMLDistribuicao);

                if (xml == null)
                    return new JsonpResult(false, "Não foi encontrado o arquivo XML da nota selecionada.");

                return Arquivo(xml, "text/xml", string.Concat("Nota_" + parametros.ChaveNfe, ".xml"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        private PortalClienteView ObterDadosRenderizacao(string portalClienteCodigo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Titulo titulo = ObterTitulo(portalClienteCodigo, repTitulo);

            if (titulo == null)
                return null;

            PortalClienteView portalClienteView = new PortalClienteView
            {
                CaminhoLogo = Empresa.CaminhoLogoSistema,
                RazaoSocial = titulo.Empresa?.RazaoSocial,
                NomeFantasia = titulo.Empresa?.NomeFantasia,
                CNPJ = titulo.Empresa?.CNPJ_Formatado,
                DestinatarioNome = titulo.Pessoa?.Nome,
                DataEmissao = titulo.DataEmissao?.ToString("dd/MM/yyyy"),
                ValorNota = titulo.ValorTotalCalculado,
            };
            
            bool possuiNotaFiscalEletronica = !string.IsNullOrWhiteSpace(titulo.NotaFiscal?.Chave);
            if (possuiNotaFiscalEletronica)
                return PreencherComDadosDaNotaFiscalEletronica(portalClienteView, titulo, repTitulo);
            else
                return PreencherComDadosDoTitulo(portalClienteView, titulo);

        }

        private Titulo ObterTitulo(string portalClienteCodigo, Repositorio.Embarcador.Financeiro.Titulo repTitulo)
        {
            return repTitulo.BuscarPorPortalClienteCodigo(portalClienteCodigo);
        }

        private PortalClienteView PreencherComDadosDaNotaFiscalEletronica(PortalClienteView portalClienteView, Titulo titulo, Repositorio.Embarcador.Financeiro.Titulo repTitulo)
        {
            portalClienteView.ChaveNFe = titulo.NotaFiscal.Chave;
            portalClienteView.CodigoNotaFiscal = titulo.NotaFiscal.Codigo;
            portalClienteView.NumeroNotaFiscal = titulo.NotaFiscal.Numero.ToString();
            portalClienteView.DataEmissao = titulo.NotaFiscal.DataEmissao?.ToString("dd/MM/yyyy");
            portalClienteView.ValorNota = titulo.NotaFiscal.ValorTotalNota;
            portalClienteView.Parcelas = ObterParcelasNfe(titulo.NotaFiscal.Codigo, repTitulo);

            return portalClienteView;
        }

        private PortalClienteView PreencherComDadosDoTitulo(PortalClienteView portalClienteView, Titulo titulo)
        {
            string statusTitulo = StatusTituloHelper.ObterDescricao(titulo.StatusTitulo);
            portalClienteView.Parcelas = new List<ParcelaView>();

            ParcelaView parcela = new ParcelaView
            {
                ParcelaId = titulo.Codigo,
                Sequencia = 1,
                Vencimento = titulo.DataVencimento?.ToString("dd/MM/yyyy"),
                Situacao = statusTitulo,
                Valor = titulo.ValorTotalCalculado
            };
            
            portalClienteView.Parcelas.Add(parcela);

            return portalClienteView;
        }

        private List<ParcelaView> ObterParcelasNfe(int codigoNfe, Repositorio.Embarcador.Financeiro.Titulo repTitulo)
        {
            List<Titulo> titulosNota = repTitulo.BuscarPorNota(codigoNfe);
            List<ParcelaView> parcelas = new List<ParcelaView>();

            foreach (Titulo parcela in titulosNota)
            {
                string statusTitulo = StatusTituloHelper.ObterDescricao(parcela.StatusTitulo);

                ParcelaView novaParcela = new ParcelaView
                {
                    ParcelaId = parcela.Codigo,
                    Vencimento = parcela.DataVencimento?.ToString("dd/MM/yyyy"),
                    Sequencia = parcela.Sequencia,
                    Situacao = statusTitulo,
                    Valor = parcela.ValorOriginal,
                };

                parcelas.Add(novaParcela);
                    
            }

            return parcelas;
        }

    }
}
