using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class NotasEmitidasAdministrativo
    {
        public int CodigoEmpresaPai { get; set; }
        public string NomeEmpresaPai { get; set; }
        public string CNPJEmpresaPai { get; set; }
        public int CodigoEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public int QtdInutilizadas { get; set; }
        public int QtdCanceladas { get; set; }
        public int QtdEmitidas { get; set; }
        public int QtdProcessadas { get; set; }
        public int QtdDenegadas { get; set; }
        public int QtdRejeitadas { get; set; }
        public int QtdAutorizadas { get; set; }
        public int QtdCCe { get; set; }
        public int QtdNFSe { get; set; }
        public int QtdBoletos { get; set; }
        public DateTime DataCadastro { get; set; }
        public int QtdNFDestinada { get; set; }
        public int QtdDocumentoEntrada { get; set; }
        public int QtdMDFe { get; set; }
        public int TotalDocumentos { get; set; }
        public decimal ValorPlano { get; set; }
    }
}
