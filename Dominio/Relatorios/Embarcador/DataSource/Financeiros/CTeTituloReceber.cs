using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class CTeTituloReceber
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string CNPJTransportador { get; set; }
        public string Transportador { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public decimal ValorAReceber { get; set; }
        public string Observacao { get; set; }
        public string TipoOperacao { get; set; }
        private string Status { get; set; }
        public decimal Peso { get; set; }
        public string NotaFiscal { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroCarga { get; set; }
        public string Acrescimos { get; set; }
        public string Decrescimos { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataVencimento { get; set; }
        private DateTime DataLiquidacao { get; set; }
        private StatusTitulo StatusTitulo { get; set; }
        public string ChaveCTe { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoSituacao
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string DescricaoDataEmissao
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataVencimento
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoDataLiquidacao
        {
            get { return DataLiquidacao != DateTime.MinValue ? DataLiquidacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string StatusFormatada
        {
            get
            {
                switch (Status)
                {
                    case "A": return "Autorizado";
                    case "P": return "Pendente";
                    case "E": return "Enviado";
                    case "R": return "Rejeitado";
                    case "C": return "Cancelado";
                    case "I": return "Inutilizado";
                    case "D": return "Denegado";
                    case "S": return "Em Digitação";
                    case "K": return "Em Cancelamento";
                    case "L": return "Em Inutilização";
                    case "Z": return "Anulado";
                    default: return string.Empty;

                }
            }
        }

        #endregion
    }
}