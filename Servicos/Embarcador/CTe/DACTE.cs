using Dominio.Excecoes.Embarcador;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public sealed class DACTE
    {
        #region Atributos

        private readonly Servicos.CTe _svcCTe;

        private readonly Servicos.NFSe _svcNFSe;

        private readonly Servicos.DACTE _svcDACTE;

        private readonly CCe _svcCCe;

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public DACTE(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _svcCTe = new Servicos.CTe(_unitOfWork);
            _svcNFSe = new Servicos.NFSe(_unitOfWork);
            _svcDACTE = new Servicos.DACTE(_unitOfWork);
            _svcCCe = new CCe(_unitOfWork);
        }

        #endregion

        public string ObterNomeArquivoPDF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            string nomeArquivoPDF = CTe.ObterNomeArquivoDownloadCTe(cte, "pdf");

            if (!string.IsNullOrWhiteSpace(nomeArquivoPDF))
                return nomeArquivoPDF;

            string caminhoRelatorios = ObterCaminhoRelatorios(unitOfWork);
            string nomeArquivoFisico = ObterNomeArquivoFisico(cte);
            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";

            return System.IO.Path.GetFileName(caminhoPDF);
        }

        public byte[] ObterPDF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "Z" && cte.Status != "F")
                return null;

            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(_unitOfWork);

            string caminhoRelatorios = ObterCaminhoRelatorios(unitOfWork);
            string nomeArquivoFisico = ObterNomeArquivoFisico(cte);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";
            byte[] pdf;

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                pdf = GerarDocumentoNFS(cte);
            else if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                pdf = _svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoEmbarcador.GerarPDFCTeCancelado);
            else
                pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

            if (pdf == null)
                return null;

            if (configuracaoEmbarcador.ImprimirDACTEeCartaCorrecaoJunto)
            {
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);

                if (cce != null)
                {
                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivoCCe = _svcCCe.ObterRelatorio(cce, _unitOfWork);
                    List<byte[]> sourceFiles = new List<byte[]>
                    {
                        pdf,
                        arquivoCCe.Arquivo
                    };

                    byte[] pdfAgrupado = _svcDACTE.MergeFiles(sourceFiles);

                    pdf = pdfAgrupado;
                }
            }

            return pdf;
        }

        private byte[] GerarDocumentoNFS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                return _svcNFSe.ObterDANFSECTe(cte.Codigo, null, true);

            return _svcNFSe.ObterDANFSECTe(cte.Codigo);
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador != null) return _configuracaoEmbarcador;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            _configuracaoEmbarcador = configuracao;

            return configuracao;
        }

        private string ObterCaminhoRelatorios(Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoRelatorios = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                throw new ServicoException("Caminho de Relatórios não está definido na aplicação.");

            return caminhoRelatorios;
        }

        private string ObterNomeArquivoFisico(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            string nomeArquivoFisico = cte.Chave;

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                nomeArquivoFisico = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();

            if (ObterConfiguracaoEmbarcador().GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                nomeArquivoFisico += "_Canc";

            if (cte.Status == "F")
                nomeArquivoFisico += "_FSDA";

            return nomeArquivoFisico;
        }

    }
}
