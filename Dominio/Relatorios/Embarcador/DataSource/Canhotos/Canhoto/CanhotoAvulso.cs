using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto
{
    public class CanhotoAvulso
    {
        public string NomeEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string TelefoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string Filial { get; set; }
        public string Transportador { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        public string notasFiscais { get; set; }
        public int NumeroCanhotoAvulso { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal PesoTotal { get; set; }
        public byte[] QRCode { get; set; }
        public string CNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
    }
}
