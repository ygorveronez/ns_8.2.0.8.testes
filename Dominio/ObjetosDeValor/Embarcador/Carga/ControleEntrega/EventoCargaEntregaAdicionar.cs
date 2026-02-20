using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class EventoCargaEntregaAdicionar
    {
        public Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        
        public List<int> CodigosCargaPedidos { get; set; }
        
        public DateTime DataOcorrencia { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        public Entidades.Usuario Usuario { get; set; }

        public bool ValidarNotasFiscaisFinalizadas { get; set; }

        public string Pacote { get; set; }

        public int Volumes { get; set; }
    }
}
