using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Usuario
{
    public class DetalheUsuario
    {
        public string CodigoIntegracao { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Situacao { get; set; }
        public bool NotificacaoExpedicao { get; set; }
        public bool LiberarAuditoria { get; set; }
        public bool NotificacaoEmail { get; set; }
        public bool ExibidoAprovacoesTransportador { get; set; }
        public bool PermissaoAdministrador { get; set; }
        public bool OperadorLogistica { get; set; }
        public bool SupervisorLogistica { get; set; }
        public bool PermitirInformarComplementoFrete { get; set; }
        public bool PermitirVisualizarValorFreteTransportadores { get; set; }

        public DetalhePerfilUsuario PerfilAcesso { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Filial.Filial FilialUsuario { get; set; }
        public List< Dominio.ObjetosDeValor.Embarcador.Filial.Filial> FiliaisConfiguradas { get; set; }
    }
}
