using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy.GrupoMotorista
{
    public class GrupoMotoristaCreateResponse
    {
        public string message { get; set; }
        public Segmentation? segmentation { get; set; }
        public string? error { get; set; }
    }

    public class Segmentation
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string tenant { get; set; }
        public string _id { get; set; }
        public string createdBy { get; set; }
        public bool active { get; set; }
        public int __v { get; set; }
        public object[] queueLog { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public long code { get; set; }
        public string updatedBy { get; set; }
    }

}
