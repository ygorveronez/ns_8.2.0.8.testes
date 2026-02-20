namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech
{
    public class ParametrosConfiguracaoRastreamento
    {
        public string url { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public string solicitanteId { get; set; }
        public string solicitanteSenha { get; set; }
        public string URI { get; set; }
        public string CNPJConsulta { get; set; }
        public long UltimoSequencial { get; set; }
        public bool BuscarDadosVeiculos { get; set; }
        public Enumeradores.EnumTecnologiaGerenciadora? rastreadorId { get; set; }
    }
}
