using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DAMDFE
    {
        public int Codigo { get; set; }

        public byte[] CodigoBarras { get; set; }

        public byte[] Logomarca { get; set; }

        public byte[] MarcaDAgua { get; set; }

        public byte[] QRCode { get; set; }

        public string ProtocoloAutorizacao { get; set; }

        public DateTime? DataAutorizacao { get; set; }

        public string Chave { get; set; }

        private string _chaveFormatada;
        public string ChaveFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._chaveFormatada))
                {
                    if (this.Chave != null && this.Chave.Length == 44)
                    {
                        this._chaveFormatada = this.Chave;

                        _chaveFormatada = _chaveFormatada.Insert(4, " ");
                        _chaveFormatada = _chaveFormatada.Insert(9, " ");
                        _chaveFormatada = _chaveFormatada.Insert(14, " ");
                        _chaveFormatada = _chaveFormatada.Insert(19, " ");
                        _chaveFormatada = _chaveFormatada.Insert(24, " ");
                        _chaveFormatada = _chaveFormatada.Insert(29, " ");
                        _chaveFormatada = _chaveFormatada.Insert(34, " ");
                        _chaveFormatada = _chaveFormatada.Insert(39, " ");
                        _chaveFormatada = _chaveFormatada.Insert(44, " ");
                        _chaveFormatada = _chaveFormatada.Insert(49, " ");
                    }
                    else
                    {
                        this._chaveFormatada = string.Empty;
                    }
                }

                return this._chaveFormatada;
            }
        }

        public string Modelo { get; set; }

        public int Serie { get; set; }

        public int Numero { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string UFCarregamento { get; set; }

        public string UFDescarregamento { get; set; }

        public string CIOT { get; set; }

        public int QuantidadeCTe { get; set; }

        public int QuantidadeCTRC { get; set; }

        public int QuantidadeNFe { get; set; }

        public int QuantidadeNF { get; set; }

        public decimal PesoTotal { get; set; }

        public string Observacoes { get; set; }

        public decimal ValorTotalCarga { get; set; }

        public string UnidadeMedida { get; set; }

        #region Dados do Emitente

        public string CNPJEmitente { get; set; }

        public string IEEmitente { get; set; }

        public string LogradouroEmitente { get; set; }

        public string RazaoSocialEmitente { get; set; }

        public string NumeroEmitente { get; set; }

        public string ComplementoEmitente { get; set; }

        public string BairroEmitente { get; set; }

        public string UFEmitente { get; set; }

        public string MunicipioEmitente { get; set; }

        public string CEPEmitente { get; set; }

        public string RNTRCEmitente { get; set; }

        #endregion
    }
}
