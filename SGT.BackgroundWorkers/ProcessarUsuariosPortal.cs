using SGT.BackgroundWorkers.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class ProcessarUsuariosPortal : LongRunningProcessBase<ProcessarUsuariosPortal>
    {

        public Repositorio.UnitOfWork _unitOfWork;
        public AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;

        #region Métodos Protegidos
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ExecutarCadastroUsuariosPortal(unitOfWork);
        }
        #endregion

        #region Métodos publicos
        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repPortal = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortal = repPortal.BuscarPrimeiroRegistro();

                if (configuracaoPortal.HabilitarAcessoTodosClientes)
                    return true;
                else
                    return false;
            }
        }

        public Dominio.Entidades.Usuario CriarUsuarioFromPessoaEGerarConfirmacaoEmailPortal(Dominio.Entidades.Cliente pessoa, string senha, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(unitOfWork.StringConexao);

            if (politicaSenha != null && politicaSenha.HabilitarCriptografia)
            {
                senha = Servicos.Criptografia.GerarHashSHA256(senha);
            }

            Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario
            {
                CPF = pessoa.CPF_CNPJ_SemFormato,
                ClienteFornecedor = pessoa,
                Cliente = pessoa,
                Nome = pessoa.Nome,
                Telefone = pessoa.Telefone1,
                Localidade = pessoa.Localidade,
                Endereco = pessoa.Endereco,
                Complemento = pessoa.Complemento,
                Email = pessoa.Email,
                Login = pessoa.CPF_CNPJ_SemFormato,
                Senha = senha,
                SenhaCriptografada = politicaSenha != null && politicaSenha.HabilitarCriptografia,
                Status = "A",
                TipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor,
                UsuarioAdministrador = true
            };

            repUsuario.Inserir(usuario);

            return usuario;
        }

        public void ExecutarCadastroUsuariosPortal(Repositorio.UnitOfWork unitOfWork)
        {
            var repPessoas = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repositorioConfiguracaoPortalMultiClifor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiClifor = repositorioConfiguracaoPortalMultiClifor.BuscarConfiguracaoPadrao();

            int batchSize = 100;
            int offset = 0;

            while (true)
            {

                var politicaSenha = ObterPoliticaSenha(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, unitOfWork);

                var pessoasParaCadastrar = repPessoas.BuscarPessoasSemCadastroUsuario(batchSize);

                if (!pessoasParaCadastrar.Any())
                    break;

                try
                {
                    foreach (var pessoa in pessoasParaCadastrar)
                    {
                        unitOfWork.Start();
                        string senhaGerada = configuracaoPortalMultiClifor.SenhaPadraoAcessoPortal;

                        var usuario = CriarUsuarioFromPessoaEGerarConfirmacaoEmailPortal(
                            pessoa, senhaGerada, politicaSenha, unitOfWork
                        );
                        unitOfWork.CommitChanges();
                    }

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                }

            }
        }
        #endregion

        #region Métodos privados

        private Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha ObterPoliticaSenha(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            var repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

            return repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(tipoServicoMultisoftware);
        }

        #endregion Métodos privados
    }
}
