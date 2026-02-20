using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class NFe
    {
        public string Chave { get; set; }

        public string Protocolo { get; set; }

        public DateTime DataEmissao { get; set; }

        public decimal Valor { get; set; }

        public int Numero { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoCubado { get; set; }

        public decimal Volumes { get; set; }

        public string NumeroRomaneio { get; set; }

        public string NumeroPedido { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }

        public string SerieDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Chave))
                {
                    if (int.TryParse(this.Chave.Substring(23, 2), out int serie))
                        return serie.ToString();
                    else
                        return this.Chave.Substring(23, 2);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string NumeroControleCliente { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string PINSuframa { get; set; }
        public string NCMPredominante { get; set; }
        public string CFOP { get; set; }
    }
}
