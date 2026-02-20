using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCadastrosMulti : LongRunningProcessBase<IntegracaoCadastrosMulti>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            if (integracao?.PossuiIntegracaoDeCadastrosMulti ?? false)
            {
                if (integracao.RealizarIntegracaoDePessoaParaPessoa)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastro(out _, _tipoServicoMultisoftware, _auditado, unitOfWork);
                if (integracao.RealizarIntegracaoDeTransportadorParaEmpresa)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroTransportador(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeTipoDeContainer)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroTipoContainer(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeTerminalPortuario)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroTerminalPortuario(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeContainer)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroContainer(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeNavio)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroNavio(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeViagem)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroViagem(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDePorto)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroPorto(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                if (integracao.RealizarIntegracaoDeProdutoEmbarcador)
                    Servicos.Embarcador.Integracao.CadastrosMulti.ImportarCadastroPessoaPessoa.ImportarCadastroProdutoEmbarcador(out _, _tipoServicoMultisoftware, _auditado, unitOfWork, unitOfWorkAdmin.StringConexao);
                
            }
        }                
    }
}