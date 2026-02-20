using Dominio.ObjetosDeValor.Embarcador.AcessoMotorista;

namespace SGT.WebAdmin.Models
{
    public class AcessoMotorista
    {
        public string Acesso { get; set; }
        public string Pager { get; set; }
        public bool ExibirNumeroPager { get; set; }
        public AcessoMotoristaDadosCarga DadosCarga { get; set; }
        public string ImagemBase64 { get; set; }
    }
}