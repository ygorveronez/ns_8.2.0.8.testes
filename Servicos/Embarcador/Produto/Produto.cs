using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Produto
{
    public class Produto
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Produto(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public byte[] ObterPdfTodasEtiquetasProduto(List<Dominio.Entidades.Produto> produtos)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaProduto.BuscarConfiguracaoPadrao();

            if (configuracaoProduto.LayoutEtiquetaProduto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutEtiquetaProduto.CodigoBarras)
                return ObterEtiquetaCodigoDeBarrasProduto(produtos);

            return ObterEtiquetaQrCodeProduto(produtos);
        }

        public byte[] ObterPdfTodasEtiquetasCompactadas(List<Dominio.Entidades.Produto> produtos)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaProduto.BuscarConfiguracaoPadrao();

            if (configuracaoProduto.LayoutEtiquetaProduto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutEtiquetaProduto.CodigoBarras)
                return ObterEtiquetaCodigoDeBarrasCompactada(produtos);

            return ObterEtiquetaQrCodeCompactada(produtos);
        }

        public byte[] ObterEtiquetaProduto(Dominio.Entidades.Produto produto)
        {
            return ObterPdfTodasEtiquetasProduto(new List<Dominio.Entidades.Produto>() { produto });
        }

        public string ContornarLeituraCodigoBarras(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras) || codigoBarras.Length < 2)
                return codigoBarras;

            var catch99 = codigoBarras.Substring(0, 2);
            if (codigoBarras.Length == 13 && catch99 == "99")
            {
                codigoBarras = codigoBarras.Remove(codigoBarras.Length - 1);
                codigoBarras = codigoBarras.Remove(0, 2);
                codigoBarras = codigoBarras.TrimStart('0');
            }
            return codigoBarras;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private byte[] ObterEtiquetaQrCodeProduto(List<Dominio.Entidades.Produto> produtos)
        {
            var produtosInformacoes = produtos.Select(produto => new
            {
                produto.Codigo,
                produto.Descricao,
                produto.LocalArmazenamentoProduto
            }).ToList();

            return ReportRequest.WithType(ReportType.ProdutoEtiqueta)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("Produtos", Newtonsoft.Json.JsonConvert.SerializeObject(produtosInformacoes))
                .CallReport()
                .GetContentFile();
        }

        private byte[] ObterEtiquetaCodigoDeBarrasProduto(List<Dominio.Entidades.Produto> produtos)
        {
            var produtosInformacoes = produtos.Select(produto => new
            {
                produto.Codigo,
                produto.CodigoProduto,
                produto.Descricao
            }).ToList();

            return ReportRequest.WithType(ReportType.ProdutoEtiquetaCodigoBarra)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("Produtos", Newtonsoft.Json.JsonConvert.SerializeObject(produtosInformacoes))
                .CallReport()
                .GetContentFile();
        }

        private byte[] ObterEtiquetaQrCodeCompactada(List<Dominio.Entidades.Produto> produtos)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            foreach (Dominio.Entidades.Produto produto in produtos)
            {
                string nomeArquivo = $"Etiqueta Produto {produto.Codigo}.pdf";

                if (!conteudoCompactar.ContainsKey(nomeArquivo))
                    conteudoCompactar.Add(nomeArquivo, ObterEtiquetaProduto(produto));
            }

            MemoryStream arquivoTodasEtiquetas = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoBinarioTodasEtiquetas = arquivoTodasEtiquetas.ToArray();

            arquivoTodasEtiquetas.Dispose();

            return arquivoBinarioTodasEtiquetas;
        }

        private byte[] ObterEtiquetaCodigoDeBarrasCompactada(List<Dominio.Entidades.Produto> produtos)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            foreach (Dominio.Entidades.Produto produto in produtos)
            {
                string nomeArquivo = $"Etiqueta Produto Codigo Barras {produto.Codigo}.pdf";

                if (!conteudoCompactar.ContainsKey(nomeArquivo))
                    conteudoCompactar.Add(nomeArquivo, ObterEtiquetaProduto(produto));
            }

            MemoryStream arquivoTodasEtiquetas = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoBinarioTodasEtiquetas = arquivoTodasEtiquetas.ToArray();

            arquivoTodasEtiquetas.Dispose();

            return arquivoBinarioTodasEtiquetas;
        }

        

        

        #endregion Métodos Privados

    }
}