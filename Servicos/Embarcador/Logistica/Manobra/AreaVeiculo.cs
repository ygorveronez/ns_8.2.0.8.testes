using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Logistica
{
    public sealed class AreaVeiculo
    {
        #region Métodos Privados

        public byte[] ObterPdfTodosQRCodeAreaVeiculoPosicao(List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode> dataSourceAreaVeiculoPosicaoQrCode)
        {
            return ReportRequest.WithType(ReportType.AreaVeiculoPosicoesQrCode)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("dataSourceAreaVeiculoPosicaoQrCodeList", dataSourceAreaVeiculoPosicaoQrCode.ToJson())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Públicos

        public byte[] ObterPdfQRCodeAreaVeiculo(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo)
        {
            Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode dataSourceAreaVeiculoQrCode = new Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode()
            {
                AreaVeiculo = areaVeiculo.Descricao,
                QRCode = Utilidades.QRcode.Gerar(areaVeiculo.QRCode),
                Posicao = ""
            };

            return ObterPdfTodosQRCodeAreaVeiculoPosicao(new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode>() { dataSourceAreaVeiculoQrCode }); ;
        }

        public byte[] ObterPdfQRCodeAreaVeiculoPosicao(Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao posicao)
        {
            Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode dataSourceAreaVeiculoPosicaoQrCode = new Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode()
            {
                AreaVeiculo = posicao.AreaVeiculo.Descricao,
                QRCode = Utilidades.QRcode.Gerar(posicao.QRCode),
                Posicao = posicao.Descricao
            };

            return ObterPdfTodosQRCodeAreaVeiculoPosicao(new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode>() { dataSourceAreaVeiculoPosicaoQrCode });
        }

        public byte[] ObterPdfTodosQRCodeAreaVeiculoPosicao(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode> dataSourceAreaVeiculoPosicaoQrCode = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode>();

            dataSourceAreaVeiculoPosicaoQrCode.Add(new Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode()
            {
                AreaVeiculo = areaVeiculo.Descricao,
                QRCode = Utilidades.QRcode.Gerar(areaVeiculo.QRCode),
                Posicao = ""
            });

            if (areaVeiculo.Posicoes?.Count > 0)
            {
                dataSourceAreaVeiculoPosicaoQrCode.AddRange(
                    from posicao in areaVeiculo.Posicoes
                    select new Dominio.Relatorios.Embarcador.DataSource.Logistica.AreaVeiculoPosicaoQrCode()
                    {
                        AreaVeiculo = posicao.AreaVeiculo.Descricao,
                        QRCode = Utilidades.QRcode.Gerar(posicao.QRCode),
                        Posicao = posicao.Descricao
                    }
                );
            }

            return ObterPdfTodosQRCodeAreaVeiculoPosicao(dataSourceAreaVeiculoPosicaoQrCode);
        }

        public byte[] ObterTodosPdfQRCodeCompactado(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            conteudoCompactar.Add($"QR Code {areaVeiculo.Descricao}.pdf", ObterPdfQRCodeAreaVeiculo(areaVeiculo));

            foreach (Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao posicao in areaVeiculo.Posicoes)
                conteudoCompactar.Add($"QR Code {posicao.DescricaoAcao}.pdf", ObterPdfQRCodeAreaVeiculoPosicao(posicao));

            MemoryStream arquivoTodosPdfQrCode = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoBinarioTodosPdfQrCode = arquivoTodosPdfQrCode.ToArray();

            arquivoTodosPdfQrCode.Dispose();

            return arquivoBinarioTodosPdfQrCode;
        }

        #endregion
    }
}
