namespace Dominio.ObjetosDeValor.NFSe
{
    public class Documentos
    {
        public string ChaveNFE { get; set; }

        public string Numero { get; set; }

        public string Serie { get; set; }

        public decimal Valor { get; set; }

        public decimal Peso { get; set; }

        public string DataEmissao { get; set; }

        public CTe.Cliente EmitenteNFe { get; set; }

        public CTe.Cliente DestinatarioNFe { get; set; }
    }
}
