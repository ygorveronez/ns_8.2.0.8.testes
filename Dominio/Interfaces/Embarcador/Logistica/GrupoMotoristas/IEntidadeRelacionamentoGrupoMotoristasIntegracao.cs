namespace Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas
{
    public interface IEntidadeRelacionamentoGrupoMotoristasIntegracao : IFuturoRelacionamentoGrupoMotoristas, Entidade.IEntidade
    {
        Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao GrupoMotoristasIntegracao { get; set; }
    }
}
