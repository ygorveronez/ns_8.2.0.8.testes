using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public class EtiquetaVolumeLRMaster : IEtiquetaCarga
    {
        #region Cabeçalho
        public string NomeTransportadora { get; set; }
        public string EnderecoTransportadora { get; set; }
        public string Bairro { get; set; }        
        public string CidadeTransportadora { get; set; }
        public string UFTransportadora { get; set; }
        public string CEPTransportadora { get; set; }
        public string CNPJTransportadora { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Telefone { get; set; }
        public string RNTC { get; set; }
        public string UFLocalidade { get; set; }
        public string Localidade { get; set; }

        public string NumeroNota { get; set; }
        public string FilialDistribuidora { get; set; }
        public string Rota { get; set; }
        public string FilialEmissora { get; set; }
        public DateTime DataEmissaoNF { get; set; }
        public string Endereco { get; set; }
        public string CidadeEstado { get; set; }
        public string CEP { get; set; }
        public string Serie { get; set; }
        #endregion

        #region Remetente
        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string PaisRemetente { get; set; }
        public string CEPRemetente { get; set; }
        public string InscricaoRemetente { get; set; }
        public string TelefoneRemetente { get; set; }
        #endregion

        #region Destinatário
        public string Destinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string PaisDestinatario { get; set; }
        public string CEPDestinatario { get; set; }
        public string InscricaoDestinatario { get; set; }
        public string TelefoneDestinatario { get; set; }
        #endregion

        #region Carga
        public string Carga { get; set; }
        public string NumeroPedido { get; set; }        
        public decimal VolumeTotal { get; set; }
        public decimal Volume { get; set; }
        public decimal MetrosCubicos { get; set; }
        public decimal Peso { get; set; }
        public string Sam { get; set; }
        public string Cross { get; set; }
        public string CodigoEtiqueta { get; set; }
        public byte[] CodigoBarras { get; set; }
        public DateTime DataEtiqueta { get; set; }

        public string Transbordo { get; set; }
        public string Item { get; set; }
        #endregion

        public string NumeroContainer { get; set; }
        public string NumeroNotaFiscal { get; set; }
    }
}
