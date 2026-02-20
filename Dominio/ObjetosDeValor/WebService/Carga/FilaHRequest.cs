using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class FilaHRequest
    {
        public double DriverCPF { get; set; }
        public string DriverName { get; set; }
        public string DriverTicket { get; set; }
        public string RequestDate { get; set; }
        public List<FilaHPlants> plants { get; set; }
        public List<FilaHOperations> operations { get; set; }
    }
}