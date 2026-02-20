namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class parametersAlteraPesoLote
    {
        public string identificador { get; set; }
        public decimal peso_bruto { get; set; }
        /// <summary>
        /// 0-Nao
        /// 1-Sim
        /// </summary>
        public int encerra_lote { get; set; }
        /// <summary>
        /// adicionar
        /// remover
        /// </summary>
        public string operacao { get; set; }
    }
}
