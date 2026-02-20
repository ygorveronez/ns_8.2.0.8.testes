namespace Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas
{
    public interface IEntidadeRelacionamentoGrupoMotoristas : IFuturoRelacionamentoGrupoMotoristas
    {
        Entidades.Embarcador.Logistica.GrupoMotoristas GrupoMotoristas { get; set; }
    }
}
