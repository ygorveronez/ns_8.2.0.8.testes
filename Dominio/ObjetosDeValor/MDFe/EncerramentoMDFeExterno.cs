namespace Dominio.ObjetosDeValor.MDFe
{
    public class EncerramentoMDFeExterno
    {
        public string Chave { get; set; }

        public string Protocolo { get; set; }

        public string CnpjEmpresa { get; set; }

        public string DataHoraEncerramento { get; set; }

        public int CodigoIBGEMunicipalEncerramento { get; set; }

        public string Token { get; set; }

        public string CnpjEmpresaAdministradora { get; set; }
    }
}