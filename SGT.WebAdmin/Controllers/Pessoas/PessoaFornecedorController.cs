using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Pessoas/Pessoa")]
    public class PessoaFornecedorController : AnexoController<Dominio.Entidades.Embarcador.Pessoas.PessoaFornecedorAnexo, Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>
    {
		#region Construtores

		public PessoaFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion

		public async Task<IActionResult> EnviarDadosAcesso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cpfCnpj = Request.GetDoubleParam("Codigo");
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorClienteFornecedor(cliente?.CPF_CNPJ ?? 0, Dominio.Enumeradores.TipoAcesso.Fornecedor);

                if (cliente == null || usuario == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                string nomeCliente = cliente.Nome;
                string emailCliente = cliente.Email;
                string subject = ObterSubjectEmail();
                string body = ObterBodyEmail(usuario);

                if (!Servicos.Email.EnviarEmailAutenticado(emailCliente, subject, body, unitOfWork, out string msgErro, nomeCliente))
                {
                    Servicos.Log.TratarErro(msgErro);
                    return new JsonpResult(true, false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoEnviarEmail);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterBodyEmail(Dominio.Entidades.Usuario usuario)
        {
            return $@"<html>
    <body>
        <div>Olá {usuario.Nome}, as suas informações de acesso são:</div>
        <table border=""0"">
            <tbody>
                <tr>
                    <td><strong>Usuário:</strong></td>
                    <td>{usuario.Login}</td>
                </tr>
                <tr>
                    <td><strong>Senha:</strong></td>
                    <td>{usuario.Senha}</td>
                </tr>
            </tbody>
        </table>
    </body>
</html>";
        }

        private string ObterSubjectEmail()
        {
            return "Informações de acesso";
        }
    }
}