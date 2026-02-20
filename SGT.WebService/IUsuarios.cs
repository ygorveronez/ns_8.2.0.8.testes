using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUsuarios" in both code and config file together.
    [ServiceContract]
    public interface IUsuarios
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>> BuscarUsuarios(string sistema, string situacao, int? inicio, int? limite);
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario>> ObterUsuarios();
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.Perfil>> BuscarPerfil(string sistema, string situacao);
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso>> ConsultarPerfisDeAcesso();
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.UsuarioFilial>> ObterRestricoesUsuarios();
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcessoUsuario>> ObterRelacaoUsuarioPerfilAcesso();
        
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilPagina>> ObterFormulariosPorPerfilAcesso(int protocoloPerfil);
        
        [OperationContract]
        Retorno<string> CriarUsuario(Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario usuario);

        [OperationContract]
        Retorno<string> AlterarUsuario(Dominio.ObjetosDeValor.Embarcador.Usuarios.Usuario usuario);

        [OperationContract]
        Retorno<string> RemoverPerfilUsuario(string login, string sistema);

    }
}
