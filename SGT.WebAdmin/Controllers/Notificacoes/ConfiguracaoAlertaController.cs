using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/ConfiguracaoAlerta")]
    public class ConfiguracaoAlertaController : BaseController
    {
		#region Construtores

		public ConfiguracaoAlertaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioconfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = new Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta();

                PreencherConfiguracaoAlerta(configuracaoAlerta, unitOfWork);
                ValidarConfiguracaoAlertaDuplicada(configuracaoAlerta, repositorioconfiguracaoAlerta);

                unitOfWork.Start();

                AdicionarUsuarios(configuracaoAlerta, unitOfWork);
                repositorioconfiguracaoAlerta.Inserir(configuracaoAlerta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioconfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioconfiguracaoAlerta.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoAlerta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                PreencherConfiguracaoAlerta(configuracaoAlerta, unitOfWork);
                ValidarConfiguracaoAlertaDuplicada(configuracaoAlerta, repositorioconfiguracaoAlerta);

                unitOfWork.Start();

                AtualizarUsuarios(configuracaoAlerta, unitOfWork);
                repositorioconfiguracaoAlerta.Atualizar(configuracaoAlerta, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioconfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioconfiguracaoAlerta.BuscarPorCodigo(codigo, auditavel: false);

                if (configuracaoAlerta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    configuracaoAlerta.Codigo,
                    Status = configuracaoAlerta.Ativo,
                    configuracaoAlerta.AlertarAposVencimento,
                    configuracaoAlerta.AlertarTransportador,
                    configuracaoAlerta.DiasAlertarAntesVencimento,
                    DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta > 0 ? configuracaoAlerta.DiasRepetirAlerta.ToString() : "",
                    configuracaoAlerta.Tipo,
                    Usuarios = (
                        from usuario in configuracaoAlerta.Usuarios
                        select new
                        {
                            usuario.Codigo,
                            usuario.Descricao
                        }
                    ).ToList(),
                    configuracaoAlerta.CodigosRejeicoes
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarUsuarios(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic usuarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Usuarios"));

            InserirUsuariosAdicionados(configuracaoAlerta, usuarios, unitOfWork);
        }

        private void AtualizarUsuarios(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic usuarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Usuarios"));

            ExcluirUsuariosRemovidos(configuracaoAlerta, usuarios, unitOfWork);
            InserirUsuariosAdicionados(configuracaoAlerta, usuarios, unitOfWork);
        }

        private void ExcluirUsuariosRemovidos(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, dynamic usuarios, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracaoAlerta.Usuarios?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var usuario in usuarios)
                    listaCodigosAtualizados.Add(((string)usuario.Codigo).ToInt());

                List<Dominio.Entidades.Usuario> listaUsuarioRemover = (from usuario in configuracaoAlerta.Usuarios where !listaCodigosAtualizados.Contains(usuario.Codigo) select usuario).ToList();

                foreach (var usuario in listaUsuarioRemover)
                    configuracaoAlerta.Usuarios.Remove(usuario);
            }
        }

        private void InserirUsuariosAdicionados(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, dynamic usuarios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            if (configuracaoAlerta.Usuarios == null)
                configuracaoAlerta.Usuarios = new List<Dominio.Entidades.Usuario>();

            foreach (var usuario in usuarios)
            {
                int codigo = ((string)usuario.Codigo).ToInt();
                Dominio.Entidades.Usuario usuarioAdicionar = repositorioUsuario.BuscarPorCodigo(codigo) ?? throw new ControllerException("Usuário não encontrado");

                if (!configuracaoAlerta.Usuarios.Contains(usuarioAdicionar))
                    configuracaoAlerta.Usuarios.Add(usuarioAdicionar);
            }
        }

        private void PreencherConfiguracaoAlerta(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            configuracaoAlerta.AlertarAposVencimento = Request.GetBoolParam("AlertarAposVencimento");
            configuracaoAlerta.AlertarTransportador = Request.GetBoolParam("AlertarTransportador");
            configuracaoAlerta.Ativo = Request.GetBoolParam("Status");
            configuracaoAlerta.DiasAlertarAntesVencimento = Request.GetIntParam("DiasAlertarAntesVencimento");
            configuracaoAlerta.DiasRepetirAlerta = Request.GetIntParam("DiasRepetirAlerta");
            configuracaoAlerta.Tipo = Request.GetEnumParam<TipoConfiguracaoAlerta>("Tipo");
            configuracaoAlerta.CodigosRejeicoes = Request.GetStringParam("CodigosRejeicoes");
        }

        private Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta()
            {
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetNullableEnumParam<TipoConfiguracaoAlerta>("Tipo")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 50, Models.Grid.Align.left, false);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioconfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
                int totalRegistros = repositorioconfiguracaoAlerta.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta> listaConfiguracaoAlerta = (totalRegistros > 0) ? repositorioconfiguracaoAlerta.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta>();

                var listaConfiguracaoAlertaRetornar = (
                    from configuracaoAlerta in listaConfiguracaoAlerta
                    select new
                    {
                        configuracaoAlerta.Codigo,
                        Tipo = configuracaoAlerta.Tipo.ObterDescricao(),
                        configuracaoAlerta.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaConfiguracaoAlertaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void ValidarConfiguracaoAlertaDuplicada(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta, Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioconfiguracaoAlerta)
        {
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlertaDuplicada = repositorioconfiguracaoAlerta.BuscarPorTipo(configuracaoAlerta.Tipo);

            if ((configuracaoAlertaDuplicada != null) && (configuracaoAlertaDuplicada.Codigo != configuracaoAlerta.Codigo))
                throw new ControllerException($"Já existe uma configuração do tipo {configuracaoAlerta.Tipo.ObterDescricao()} cadastrada");
        }

        #endregion
    }
}
