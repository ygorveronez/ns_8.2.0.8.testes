namespace Dominio.ObjetosDeValor.WebService.Transportadores
{
    public class MotoristaLicenca
    {
        public int Protocolo { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public string DataCriacao { get; set; }
        public string DataVencimento { get; set; }
        public string Status { get; set; }
        public Dominio.ObjetosDeValor.WebService.Configuracoes.Licenca TipoLicenca { get; set; }
        public bool ConfirmadaLeituraPendencia { get; set; }
    }
}
