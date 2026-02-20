using Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas
{
    public class AtualizacaoGrupoMotoristas : ISalvamentoGrupoMotoristas
    {
        public Entidades.Embarcador.Logistica.GrupoMotoristas GrupoMotoristas { get { return ResultadoConsulta.GrupoMotoristas; } }
        public List<RelacionamentoGrupoMotoristas> Motoristas { get; set; }
        public List<Enumeradores.TipoIntegracao> TiposIntegracao { get; set; }
        public RetornoGrupoMotoristas ResultadoConsulta { get; set; }

        public AtualizacaoGrupoMotoristas(RetornoGrupoMotoristas grupoMotoristasAlterado, 
                                           List<RelacionamentoGrupoMotoristas> novosMotoristas, 
                                           List<Enumeradores.TipoIntegracao> novosTiposIntegracao)
        {
            ResultadoConsulta = grupoMotoristasAlterado;
            Motoristas = novosMotoristas;
            TiposIntegracao = novosTiposIntegracao;
        }
    }
}
