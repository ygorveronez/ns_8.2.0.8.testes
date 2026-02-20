namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class Manifestacao
    {
        public string identificacaoManifestacao { get; set; }
        public InfoGeral infoGeral { get; set; }
        public Veiculo veiculo { get; set; }
        public Transportador transportador { get; set; }
        public Carga carga { get; set; }
    }
}
