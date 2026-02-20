using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga;

public class DadosCarga
{
    public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
    public int CodigoTipoCarga { get; set; }
    public int CodigoModeloVeiculo { get; set; }
    public int CodigoTipoContainer { get; set; }
    public string Justificativa { get; set; }
    public int CodigoTransportador { get; set; }
    public int CodigoVeiculo { get; set; }
    public int CodigoReboque { get; set; }
    public int CodigoSegundoReboque { get; set; }
    public int CodigoTerceiroReboque { get; set; }
    public bool AvancoAutomatico { get; set; }
    public Dominio.Entidades.Usuario Usuario { get; set; }
    public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
    public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado { get; set; }
    public List<int> CodigosMotoristas { get; set; }
}
