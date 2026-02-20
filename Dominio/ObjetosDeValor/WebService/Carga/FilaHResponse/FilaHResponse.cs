using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{

    public class FilaHResponse
    {
        public List<string> checkinMessage { get; set; }
        public List<FilaHOperationsResponse> operations { get; set; }
        public List<FilaHPlants> plants { get; set; }
        public string requestDate { get; set; }
        public string ticketDiver { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}