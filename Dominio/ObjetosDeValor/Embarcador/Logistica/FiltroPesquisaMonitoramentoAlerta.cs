using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaMonitoramentoAlerta
    {
        public string CodigoCargaEmbarcador { get; set; }
        public string Placa { get; set; }
        public int Transportador { get; set; }
        public int Motorista { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus AlertaMonitorStatus { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }
        public bool ApenasComPosicaoTardia { get; set; }
        public List<int> Filiais { get; set; }
        public List<double> Recebedores { get; set; }
        public bool FiltrarCargasPorParteDoNumero { get; set; }

    }
}
