namespace Dominio.ObjetosDeValor
{
    public class ConfiguracaoEmpresaClienteSerie
    {
        public string CnpjCliente { get; set; }

        public int CodigoSerie { get; set; }

        public int Serie { get; set; }

        public bool Excluir { get; set; }

        public string TipoCliente { get; set; }
        
        public string DescricaoTipoCliente { get; set; }

        public string RaizCNPJ { get; set; }
    }
}
