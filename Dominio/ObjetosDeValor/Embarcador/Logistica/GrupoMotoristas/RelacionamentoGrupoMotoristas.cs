namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class RelacionamentoGrupoMotoristas : Interfaces.Embarcador.Logistica.GrupoMotoristas.IRelacionamentoGrupoMotoristas
    {

        public int CodigoRelacionamento { get; set; }

        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public string CPF { get; set; }

    }
}
