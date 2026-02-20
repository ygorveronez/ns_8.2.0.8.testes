namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega
{
    public class RelacaoEntregaDocumento
    {
        public int CodigoCarga { get; set; }
        public int CodigoCTe { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCTe { get; set; }
        public string NomeRemetente { get; set; }
        public string NomeDestinatario { get; set; }
        public decimal QtdPeso { get; set; }
        public decimal QtdVolume { get; set; }
        public string Notas { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string NumeroDestinatario { get; set; }
        public string ComplementoDestinatario { get; set; }
        public string CEPDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string EstadoDestinatario { get; set; }
        public string FoneDestinatario { get; set; }
        public int Ordem { get; set; }
        public string Instrucao { get; set; }
        public string Setor { get; set; }
        public byte[] CodigoBarras { get; set; }
        public string CodigoBarrasCompleto { get; set; }
        public string EmpresaFilial { get; set; }
        public int QtdNF { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal ValorFrete { get; set; }
    }
}
