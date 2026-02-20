using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Login
{
    public class Login : ServicoBase
    {        
        public Login(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public bool ValidarAcessoEmpresaCommerce(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                try
                {
                    WSValidacaoCommerce.ValidacaoClient validacaoWS = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<WSValidacaoCommerce.ValidacaoClient, WSValidacaoCommerce.IValidacao>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Commerce_Validacao);
                    if (validacaoWS.ExisteCliente(empresa.CNPJ))
                    {
                        DateTime? dataVencimento = validacaoWS.RetornaDataVencimento(empresa.CNPJ);
                        if (dataVencimento != null)
                        {
                            if (empresa.StatusFinanceiro != "B" && dataVencimento.Value.Date <= DateTime.Now.Date)
                            {
                                empresa.StatusFinanceiro = "B";
                                repEmpresa.Atualizar(empresa);
                            }
                            else if (empresa.StatusFinanceiro == "B" && dataVencimento.Value.Date > DateTime.Now.Date)
                            {
                                empresa.StatusFinanceiro = "N";
                                repEmpresa.Atualizar(empresa);
                            }
                        }
                    }
                    else
                        validacaoWS.InsereDadosCliente(empresa.CNPJ, empresa.NomeFantasia, DateTime.Now.Date.AddYears(1));

                    return true;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return false;
                }
            }
            else
                return true;
        }

        public bool BloquearDesbloquearEmpresaCommerce(Dominio.Entidades.Empresa empresa, string statusFinanceiroAnterior, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                try
                {
                    if (statusFinanceiroAnterior != empresa.StatusFinanceiro)
                    {
                        WSValidacaoCommerce.ValidacaoClient validacaoWS = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<WSValidacaoCommerce.ValidacaoClient, WSValidacaoCommerce.IValidacao>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Commerce_Validacao);
                        if (!validacaoWS.ExisteCliente(empresa.CNPJ))
                            validacaoWS.InsereDadosCliente(empresa.CNPJ, empresa.NomeFantasia, DateTime.Now.Date.AddYears(1));

                        if (empresa.StatusFinanceiro == "B")
                            validacaoWS.BloquearCliente(empresa.CNPJ);
                        else if (empresa.StatusFinanceiro == "N")
                            validacaoWS.DesbloquearCliente(empresa.CNPJ);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return false;
                }
            }
            else
                return true;
        }

        public void InativarUsuariosAposXDiasSemAcesso(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            unitOfWork.Start();

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.PoliticaSenha repositorioPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

            Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repositorioPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(tipoServicoMultisoftware);

            if ((politicaSenha?.InativarUsuarioAposDiasSemAcessarSistema ?? 0) <= 0)
                return;

            List<Dominio.Entidades.Usuario> usuarios = repositorioUsuario.BuscarListaUsuariosParaInativacaoAposXDiasSemAcesso(politicaSenha.InativarUsuarioAposDiasSemAcessarSistema);

            if (usuarios.Count == 0)
                return;

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
            {
                usuario.DataHoraBloqueio = DateTime.Now;
                usuario.TentativasInvalidas = 0;
                usuario.UsuarioAcessoBloqueado = true;
                usuario.Status = "I";

                repositorioUsuario.Atualizar(usuario);

                Auditoria.Auditoria.AuditarSemDadosUsuario(usuario, $"Usuário inativado por politica de senha após {politicaSenha.InativarUsuarioAposDiasSemAcessarSistema} dias sem acessar o sistema", unitOfWork);
            }

            unitOfWork.CommitChanges();
        }

        // NÃO REMOVER, é usado no Login e Logout
        public void SalvarLogAcesso(Dominio.Entidades.Usuario usuario, string userHostAddress, Dominio.Enumeradores.TipoLogAcesso tipoLogAcesso, Dominio.ObjetosDeValor.UsuarioInterno usuarioInterno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.LogAcesso repositorioLogAcesso = new Repositorio.LogAcesso(unitOfWork);

            Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

            logAcesso.Data = DateTime.Now;
            logAcesso.IPAcesso = userHostAddress;
            logAcesso.SessionID = usuario.Session;
            logAcesso.Tipo = tipoLogAcesso;
            logAcesso.Usuario = usuario;

            if (usuarioInterno != null)
            {
                logAcesso.Login = usuarioInterno.Email;
                logAcesso.Senha = string.Empty;
            }
            else
            {
                logAcesso.Login = usuario.Login;
                logAcesso.Senha = usuario.Senha;
            }

            repositorioLogAcesso.Inserir(logAcesso);
        }
    }
}