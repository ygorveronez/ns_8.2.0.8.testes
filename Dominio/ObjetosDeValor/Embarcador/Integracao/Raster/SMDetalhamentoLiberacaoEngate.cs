namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class SMDetalhamentoLiberacaoEngate
    {
        /// <summary>
        /// Tipo de pesquisa que será solicitada automaticamente caso não seja encontrada uma pesquisa anterior. Opções: NAO, NORMAL, EXPRESSA 
        /// </summary>
        public string SolicitarPesquisa { get; set; }
        public string DadosPesqVeiculo { get; set; }
        public string DadosPesqMotorista1 { get; set; }
        public string DadosPesqMotorista2 { get; set; }
        public string DadosPesqAjudante { get; set; }
        public string DadosPesqCarreta1 { get; set; }
        public string DadosPesqCarreta2 { get; set; }
        public string DadosPesqCarreta3 { get; set; }
    }
}
