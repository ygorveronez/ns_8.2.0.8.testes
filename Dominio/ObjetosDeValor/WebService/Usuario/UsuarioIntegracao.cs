using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Usuario
{
    public class UsuarioIntegracao
    {
        public string CodigoIntegracao { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public bool RedefinirSenha { get; set; }
        public int IBGEMunicipio { get; set; }
        public string Situacao { get; set; }
        public string Sistema { get; set; }
        public string CodigoIntegracaoPerfilAcesso { get; set; }
        public bool PermissaoAdministrador { get; set; }
        public bool NotificacaoExpedicao { get; set; }
        public bool LiberarAuditoria { get; set; }
        public bool NotificacaoEmail { get; set; }
        public bool ExibidoAprovacoesTransportador { get; set; }
        public bool OperadorLogistica { get; set; }
        public bool SupervisorLogistica { get; set; }
        public bool PermitirInformarComplementoFrete { get; set; }
        public bool PermitirVisualizarValorFreteTransportadores { get; set; }
        public List<string> CodigosIntegracaoFilialOperadorLogistica { get; set; }

    }
}
