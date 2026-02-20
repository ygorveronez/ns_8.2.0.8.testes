namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    //public class Pedido
    //{
    //    public long id { get; set; }
    //    public double latitude { get; set; }
    //    public double longitude { get; set; }
    //    //public double peso_total { get; set; }
    //    public long peso_total { get; set; }
    //    /// <summary>
    //    /// tempo de descarga em minutos
    //    /// </summary>
    //    public int tempo_desc_min { get; set; }

    //    /// <summary>
    //    /// Contem o inicio de recebimento em minutos... Ex: 360 = 6h da manhã
    //    /// </summary>
    //    public int janela_descarga_ini { get; set; }
    //    public int janela_descarga_fim { get; set; }
    //}
    public class Pedido
    {
        public int Codigo { get; set; }

        public int CodigoCanalEntrega { get; set; }
        public string CanalEntreaga { get; set; }

        /// <summary>
        /// Quantidade máxima de entregas do mesmo canal de entrega no mesmo veiculo.
        /// </summary>
        public int LimiteCanalEntrega { get; set; }

        public int Prioridade { get; set; }
    }
}
