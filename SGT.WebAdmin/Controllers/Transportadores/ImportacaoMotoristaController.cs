using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/ImportacaoMotorista")]
    public class ImportacaoMotoristaController : BaseController
    {
		#region Construtores

		public ImportacaoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresaOrigem, codigoEmpresaDestino = 0;
                int.TryParse(Request.Params("TransportadorOrigem"), out codigoEmpresaOrigem);
                int.TryParse(Request.Params("TransportadorDestino"), out codigoEmpresaDestino);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaOrigem = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaOrigem, this.Empresa.Codigo);
                Dominio.Entidades.Empresa empresaDestino = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaDestino, this.Empresa.Codigo);

                if (empresaOrigem == null)
                    return new JsonpResult(false, "Empresa de origem dos motoristas não encontrada!");

                if (empresaDestino == null)
                    return new JsonpResult(false, "Empresa de destino dos motoristas não encontrada!");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                List<Dominio.Entidades.Usuario> usuariosOrigem = repUsuario.BuscarPorEmpresa(empresaOrigem.Codigo, "M");
                List<Dominio.Entidades.Usuario> usuariosDestino = repUsuario.BuscarPorEmpresa(empresaDestino.Codigo, "M");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    usuariosDestino = repUsuario.BuscarTodosCadastros();

                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Usuario usuarioOrigem in usuariosOrigem)
                {
                    Dominio.Entidades.Usuario usuario = (from obj in usuariosDestino where Utilidades.String.OnlyNumbers(obj.CPF).Equals(Utilidades.String.OnlyNumbers(usuarioOrigem.CPF)) select obj).FirstOrDefault();

                    if (usuario == null)
                        usuario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? repUsuario.BuscarMotoristaPorCPF(usuarioOrigem.CPF) : repUsuario.BuscarMotoristaPorCPF(empresaDestino.Codigo, usuarioOrigem.CPF); ;

                    if (usuario == null) usuario = new Dominio.Entidades.Usuario();
                    else usuario.Initialize();

                    usuario.Categoria = usuarioOrigem.Categoria;
                    usuario.Complemento = usuarioOrigem.Complemento;
                    usuario.CPF = usuarioOrigem.CPF;
                    usuario.DataAdmissao = usuarioOrigem.DataAdmissao;
                    usuario.DataHabilitacao = usuarioOrigem.DataHabilitacao;
                    usuario.DataNascimento = usuarioOrigem.DataNascimento;
                    usuario.DataVencimentoHabilitacao = usuarioOrigem.DataVencimentoHabilitacao;
                    usuario.Email = usuarioOrigem.Email;
                    usuario.Empresa = empresaDestino;
                    usuario.Endereco = usuarioOrigem.Endereco;
                    usuario.EstadoCivil = usuarioOrigem.EstadoCivil;
                    usuario.Localidade = usuarioOrigem.Localidade;
                    usuario.Moop = usuarioOrigem.Moop;
                    usuario.Nome = usuarioOrigem.Nome;
                    usuario.NumeroCartao = usuarioOrigem.NumeroCartao;
                    usuario.NumeroHabilitacao = usuarioOrigem.NumeroHabilitacao;
                    usuario.PIS = usuarioOrigem.PIS;
                    usuario.RG = usuarioOrigem.RG;
                    usuario.Setor = usuarioOrigem.Setor;
                    usuario.Status = "A";
                    usuario.Telefone = usuarioOrigem.Telefone;
                    usuario.Tipo = "M";
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                    usuario.TipoSanguineo = usuarioOrigem.TipoSanguineo;

                    if (usuario.Codigo > 0)
                        repUsuario.Atualizar(usuario, Auditado);
                    else
                        repUsuario.Inserir(usuario, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, "Motorista importado entre transportadoras.", unidadeDeTrabalho);
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true, "Importação realizada com sucesso!");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao importar os motoristas, atualize a página e tente novamente.");
            }
        }
    }
}
