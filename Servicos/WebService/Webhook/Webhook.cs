using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;

namespace Servicos.WebService.Webhook
{
    public class Webhook : ServicoBase
    {
        #region Propriedades Privadas        
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;                
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;

        #endregion

        #region Construtores
        
        public Webhook(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork)
        {                                    
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ReceberEventoCTe(RetornoEventoCTe retornoEventoCTe)
        {
            string mensagemErro = string.Empty;

            if (!Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech).ReceberEventoCte(out mensagemErro, null, retornoEventoCTe, _auditado, _tipoServicoMultisoftware, _unitOfWork))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        #endregion

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
