using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoInformacao
    {
        public DateTime? DataCargaAceita { get; set; }

        public DateTime DataEntrada { get; set; }

        public int PosicaoEntrada { get; set; }

        public List<Entidades.Veiculo> Reboques { get; set; }

        public Entidades.Veiculo Tracao { get; set; }
    }
}
