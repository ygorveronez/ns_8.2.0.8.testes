using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class AFRMMControl
    {
        public int Codigo { get; set; }
        public string Filial { get; set; }
        public string CNPJFilial { get; set; }
        public string CodigoFilial { get; set; }
        public string Navio { get; set; }
        public int Viagem { get; set; }
        public string Direcao { get; set; }
        public string POL { get; set; }
        public string POLCodigo { get; set; }
        public string POD { get; set; }
        public string PODCodigo { get; set; }
        public string CTAC { get; set; }
        public int NumeroFiscal { get; set; }
        public string ChaveCTe { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CTM { get; set; }
        public string ChaveCTM { get; set; }
        public string ChaveNFe { get; set; }
        public string StatusCTe { get; set; }
        public string MotivoCancelamento { get; set; }
        public string TipoDocumento { get; set; }
        public string ObservacaoComplementar { get; set; }
        public string ChaveCTeCliente { get; set; }

        public string FilialFormatada
        {
            get
            {
                //if (!string.IsNullOrWhiteSpace(CNPJFilial))
                //    return Filial + " (" + String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJFilial)) + ")";
                //else
                return Filial;
            }
        }
        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
