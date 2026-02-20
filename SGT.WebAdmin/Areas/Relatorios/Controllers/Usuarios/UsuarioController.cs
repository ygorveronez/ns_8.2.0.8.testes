using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Usuarios
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Usuarios/Usuario")]
    public class UsuarioController : BaseController
    {
        #region Construtores

        public UsuarioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R111_Usuarios;
        private int UltimaColunaDinanica = 1;
        private int NumeroMaximoEPIs = 20;
        private decimal TamanhoColunasMedia = 6;
        private decimal TamanhoColunasDescritivos = 10;
        private decimal TamanhoColunasPequeno = 4;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Usuários", "Usuarios", "Usuario.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Nome", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await GridPadraoAsync(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Usuarios.Usuario servicoRelatorioUsuario = new Servicos.Embarcador.Relatorios.Usuarios.Usuario(unitOfWork, TipoServicoMultisoftware, Cliente, cancellationToken);

                int totalRegistros = await servicoRelatorioUsuario.ContarRegistrosAsync(filtrosPesquisa, agrupamentos);
                IList<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario> lista = totalRegistros > 0 ? await servicoRelatorioUsuario.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.Usuario>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Grid> GridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pessoas.EPI repEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.EPI> epis = await repEPI.BuscarTodosAtivos();

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Nome").Nome("Nome").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("CPFFormatado").Nome("CPF").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("RG").Nome("RG").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Login").Nome("Login").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Email").Nome("E-mail").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Filial").Nome("Filial").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataCadastroFormatada").Nome("Data de Cadastro").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("PerfilAcesso").Nome("Perfil de Acesso").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("UltimoAcessoFormatada").Nome("Último Acesso").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Telefone").Nome("Telefone").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Cidade").Nome("Cidade").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Endereco").Nome("Endereço").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Complemento").Nome("Complemento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Salario").Nome("Salário").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DataNascimentoFormatada").Nome("Data de Nascimento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Situacao").Nome("Situação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left);
            grid.Prop("DataAdmissaoFormatada").Nome("Data de Admissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("AcessoSistemaFormatado").Nome("Acesso ao Sistema").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Ambiente").Nome("Ambiente").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataUltimaAlteracaoSenhaFormatada").Nome("Última Alteração Senha").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("SituacaoSenha").Nome("Situação Senha").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("AposentadoriaFormatada").Nome("Aposentado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false);
            grid.Prop("DataDemissaoFormatada").Nome("Data Demissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("HoraInicialAcesso").Nome("Horário inicial acesso").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("HoraFinalAcesso").Nome("Horário final acesso").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("UsuarioAdministradorFormatado").Nome("Usuário Admnistrador").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Setor").Nome("Setor").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PermiteAuditarDescricao").Nome("Auditoria").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);

            //Colunas montadas dinamicamente
            for (int i = 0; i < epis.Count; i++)
            {
                if (i < NumeroMaximoEPIs)
                {
                    grid.AdicionarCabecalho(epis[i].Descricao, "QuantidadeEPI" + UltimaColunaDinanica.ToString(), TamanhoColunasPequeno, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, epis[i].Codigo);

                    UltimaColunaDinanica++;
                }
                else
                    break;
            }

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Usuarios.FiltroPesquisaRelatorioUsuario()
            {
                DataCadastroInicial = Request.GetDateTimeParam("DataCadastroInicial"),
                DataCadastroFinal = Request.GetDateTimeParam("DataCadastroFinal"),
                UltimoAcessoInicial = Request.GetDateTimeParam("UltimoAcessoInicial"),
                UltimoAcessoFinal = Request.GetDateTimeParam("UltimoAcessoFinal"),
                CodigoLocalidade = Request.GetIntParam("Localidade"),
                CodigoPerfilAcesso = Request.GetIntParam("PerfilAcesso"),
                Operador = Request.GetNullableBoolParam("Operador"),
                AcessoSistema = Request.GetNullableBoolParam("AcessoSistema"),
                Status = Request.GetStringParam("Status"),
                SituacaoColaborador = Request.GetEnumParam<SituacaoColaborador>("SituacaoColaborador"),
                Ambiente = Request.GetNullableEnumParam<TipoAcesso>("Ambiente"),
                Aposentadoria = Request.GetEnumParam<Aposentadoria>("Aposentadoria"),
                UsuarioMultisoftware = this.Usuario.UsuarioMultisoftware,
                tipoServicoMultisoftware = TipoServicoMultisoftware,
                TipoUsuario = Request.GetEnumParam<TipoUsuario>("TipoUsuario"),
                SomenteUsuariosAtivo = Request.GetBoolParam("SomenteUsuariosAtivo")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa.Codigo;
                filtrosPesquisa.Ambiente = null;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                filtrosPesquisa.Ambiente = null;

            return filtrosPesquisa;
        }

        #endregion
    }
}
