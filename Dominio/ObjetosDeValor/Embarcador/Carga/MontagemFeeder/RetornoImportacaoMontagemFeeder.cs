namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder
{
    public class RetornoImportacaoMontagemFeeder
    {
        public bool Sucesso { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalCargas { get; set; }
        public string Mensagem { get; set; }
    }
}
