using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Operacional
{
    public class OperadorTipoCargaModelosVeiculares
    {
        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoCarga { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeicularCarga { get; set; }

    }
}
