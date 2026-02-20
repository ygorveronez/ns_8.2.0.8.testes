using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Ofertas
{
    [CustomAuthorize("Cargas/ParametrosOfertas")]
    public class ParametrosOfertasController : BaseController
    {
        #region Construtores
        public ParametrosOfertasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertas repoParametrosOfertas = new(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas = await MontarEntidadeParametrosOfertas(cancellationToken);

                await unitOfWork.StartAsync();

                long codigo = await repoParametrosOfertas.InserirAsync(parametrosOfertas, Auditado);

                parametrosOfertas.Codigo = (int)codigo;

                await AdicionarParametrosOfertasDadosOfertas(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasFilial(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasTipoCarga(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasTipoOperacao(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasEmpresa(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasFuncionario(parametrosOfertas, unitOfWork, cancellationToken);
                await AdicionarParametrosOfertasTipoIntegracao(parametrosOfertas, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(new
                {
                    Codigo = codigo,
                    parametrosOfertas.Descricao,
                    parametrosOfertas.CodigoIntegracao,
                    parametrosOfertas.Ativo,
                    parametrosOfertas.GrupoMotoristas,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
                if (ex is (ControllerException))
                    return new JsonpResult(false, true, ex.Message);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao adicionar os parâmetros de ofertas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                if (codigo < 1)
                {
                    throw new ControllerException($"Código de Parametros de Ofertas inválido. Recebido: {codigo}");
                }

                Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta = await ConsultarParametrosOfertas(codigo, unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertas repoParametrosOfertas = new(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas = await repoParametrosOfertas.BuscarPorCodigoAsync(codigo, false);

                parametrosOfertas.Initialize();

                await unitOfWork.StartAsync();

                await AtualizarParametrosOfertasFuncionarios(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasEmpresas(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasTipoOperacao(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasTipoCarga(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasFilial(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasTipoIntegracao(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertasDadosOfertas(parametrosOfertas, resultadoConsulta, unitOfWork, cancellationToken);
                await AtualizarParametrosOfertas(parametrosOfertas, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os Parâmetros de Ofertas.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarGridParametrosOfertas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertas repoParametrosOfertas = new(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertas filtrosPesquisa = MontarFiltroPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 35, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas> listaParametrosOfertas = await repoParametrosOfertas.BuscarAsync(
                    filtrosPesquisa,
                    parametrosConsulta
                );

                grid.setarQuantidadeTotal(await repoParametrosOfertas.ContarConsultaAsync(filtrosPesquisa));


                var lista = (
                    from po in listaParametrosOfertas
                    select new
                    {
                        po.Codigo,
                        po.Descricao,
                        po.CodigoIntegracao,
                        Situacao = po.Ativo ?
                            nameof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoParametrosOfertas.Ativo) :
                            nameof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoParametrosOfertas.Inativo)
                    }
                ).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar os Parâmetros de Oferta.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RecuperarParametrosOfertas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                if (codigo < 1)
                {
                    throw new ControllerException($"Código de Parametros de Ofertas inválido. Recebido: {codigo}");
                }

                return new JsonpResult(await ConsultarParametrosOfertas(codigo, unitOfWork, cancellationToken));
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os Parâmetros de Oferta com seus relacionamentos.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task AdicionarParametrosOfertasTipoIntegracao(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var listaTiposIntegracao = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TiposIntegracao");

            if (listaTiposIntegracao.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao repoParametrosOfertasTipoIntegracao = new(unitOfWork, cancellationToken);

            for (int i = 0; i < listaTiposIntegracao.Length; i++)
            {
                await repoParametrosOfertasTipoIntegracao.InserirAsync(
                    new()
                    {
                        TipoIntegracao = listaTiposIntegracao[i],
                        ParametrosOfertas = parametrosOfertas
                    },
                    Auditado
                );
            }
        }

        private async Task AtualizarParametrosOfertasTipoIntegracao(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entrada = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TiposIntegracao");
            var atual = resultadoConsulta.TiposIntegracao;

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao repoParametrosOfertasTipoIntegracao = new(unitOfWork, cancellationToken);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            var aInserir = entrada.Except(atual).ToList();
            var aRemover = atual.Except(entrada).ToList();

            for (int i = 0; i < aInserir.Count; i++)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Integração",
                    De = "",
                    Para = aInserir[i].ObterDescricao(),
                });

                await repoParametrosOfertasTipoIntegracao.InserirAsync(
                    new()
                    {
                        TipoIntegracao = aInserir[i],
                        ParametrosOfertas = parametrosOfertas,
                    },
                    Auditado
                );
            }

            for (int i = 0; i < aRemover.Count; i++)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Integração",
                    De = aRemover[i].ObterDescricao(),
                    Para = "",
                });
                await repoParametrosOfertasTipoIntegracao.DeletarPorTipo(aRemover[i], resultadoConsulta.ParametrosOfertas.Codigo);
            }

            parametrosOfertas.SetExternalChanges(alteracoes);
        }

        private async Task AdicionarParametrosOfertasFuncionario(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> funcionariosOV = ObterMotorista();

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario repoParametrosOfertasFuncionario = new(unitOfWork, cancellationToken);

            for (int i = 0; i < funcionariosOV.Count; i++)
            {
                Dominio.Entidades.Usuario funcionario = new() { Codigo = funcionariosOV[i].Codigo };

                await repoParametrosOfertasFuncionario.InserirAsync(
                    new()
                    {
                        Funcionario = funcionario,
                        ParametrosOfertas = parametrosOfertas
                    },
                    Auditado
                );
            }
        }

        private async Task AtualizarParametrosOfertasFuncionarios(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario repositorio = new(unitOfWork, cancellationToken);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> entrada = ObterMotorista();

            async Task inserir(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao relacionamento)
            {
                Dominio.Entidades.Usuario funcionario = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                new()
                {
                    Funcionario = funcionario,
                    ParametrosOfertas = parametrosOfertas,
                },
                Auditado
                );
            }

            await AtualizarRelacionamento(repositorio, resultadoConsulta.Funcionarios, entrada, inserir, "Motorista", parametrosOfertas);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> ObterMotorista()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> funcionariosLista = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>("Funcionarios");

            if (funcionariosLista.Count > 1)
                throw new ControllerException("Não é permitido adicionar mais de um motorista.");

            return funcionariosLista;
        }

        private async Task AdicionarParametrosOfertasEmpresa(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var empresasOV = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertas>("Empresas");

            if (empresasOV.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa repoParametrosOfertasEmpresa = new(unitOfWork, cancellationToken);

            for (int i = 0; i < empresasOV.Length; i++)
            {
                if (int.TryParse(empresasOV[i].Codigo, out int codigo))
                {
                    Dominio.Entidades.Empresa empresa = new() { Codigo = codigo };

                    await repoParametrosOfertasEmpresa.InserirAsync(
                        new()
                        {
                            Empresa = empresa,
                            ParametrosOfertas = parametrosOfertas
                        },
                        Auditado
                    );
                }
            }
        }

        private async Task AtualizarParametrosOfertasEmpresas(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entrada = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>("Empresas");
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa repositorio = new(unitOfWork, cancellationToken);

            async Task inserir(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao relacionamento)
            {
                Dominio.Entidades.Empresa empresa = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                    new()
                    {
                        Empresa = empresa,
                        ParametrosOfertas = parametrosOfertas,
                    },
                    Auditado
                );
            }

            await AtualizarRelacionamento(repositorio, resultadoConsulta.Empresas, entrada, inserir, "Empresa", parametrosOfertas);
        }

        private async Task AdicionarParametrosOfertasTipoOperacao(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var tiposOperacaoOV = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertas>("TiposOperacao");

            if (tiposOperacaoOV.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao repoParametrosOfertasTipoOperacao = new(unitOfWork, cancellationToken);

            for (int i = 0; i < tiposOperacaoOV.Length; i++)
            {
                if (int.TryParse(tiposOperacaoOV[i].Codigo, out int codigo))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOpercao = new() { Codigo = codigo };

                    await repoParametrosOfertasTipoOperacao.InserirAsync(
                        new()
                        {
                            TipoOperacao = tipoOpercao,
                            ParametrosOfertas = parametrosOfertas
                        },
                        Auditado
                    );
                }
            }
        }

        private async Task AtualizarParametrosOfertasTipoOperacao(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entrada = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>("TiposOperacao");
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao repositorio = new(unitOfWork, cancellationToken);

            async Task inserir(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao relacionamento)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                    new()
                    {
                        TipoOperacao = tipoOperacao,
                        ParametrosOfertas = parametrosOfertas,
                    },
                    Auditado
                );
            }

            await AtualizarRelacionamento(repositorio, resultadoConsulta.TiposOperacao, entrada, inserir, "Tipo de Operação", parametrosOfertas);
        }

        private async Task AdicionarParametrosOfertasTipoCarga(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var tiposCargaOV = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertas>("TiposCarga");

            if (tiposCargaOV.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga repoParametrosOfertasTipoCarga = new(unitOfWork, cancellationToken);

            for (int i = 0; i < tiposCargaOV.Length; i++)
            {
                if (int.TryParse(tiposCargaOV[i].Codigo, out int codigo))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = new() { Codigo = codigo };

                    await repoParametrosOfertasTipoCarga.InserirAsync(
                        new()
                        {
                            TipoDeCarga = tipoCarga,
                            ParametrosOfertas = parametrosOfertas
                        },
                        Auditado
                    );
                }
            }
        }

        private async Task AtualizarParametrosOfertasTipoCarga(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entrada = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>("TiposCarga");
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga repositorio = new(unitOfWork, cancellationToken);

            async Task inserir(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao relacionamento)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                    new()
                    {
                        TipoDeCarga = tipoCarga,
                        ParametrosOfertas = parametrosOfertas,
                    },
                    Auditado
                );
            }

            await AtualizarRelacionamento(repositorio, resultadoConsulta.TiposCarga, entrada, inserir, "Tipo da Carga", parametrosOfertas);
        }

        private async Task AdicionarParametrosOfertasFilial(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var filiaisOV = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertas>("Filiais");

            if (filiaisOV.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial repoParametrosOfertasFilial = new(unitOfWork, cancellationToken);

            for (int i = 0; i < filiaisOV.Length; i++)
            {
                if (int.TryParse(filiaisOV[i].Codigo, out int codigo))
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = new() { Codigo = codigo };

                    await repoParametrosOfertasFilial.InserirAsync(
                        new()
                        {
                            Filial = filial,
                            ParametrosOfertas = parametrosOfertas
                        },
                        Auditado
                    );
                }
            }
        }

        private async Task AtualizarParametrosOfertasFilial(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entrada = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>("Filiais");
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial repositorio = new(unitOfWork, cancellationToken);

            async Task inserir(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao relacionamento)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = new() { Codigo = relacionamento.Codigo };

                await repositorio.InserirAsync(
                    new()
                    {
                        Filial = filial,
                        ParametrosOfertas = parametrosOfertas,
                    },
                    Auditado
                );
            }

            await AtualizarRelacionamento(repositorio, resultadoConsulta.Filiais, entrada, inserir, "Filial", parametrosOfertas);
        }

        private async Task AdicionarParametrosOfertasDadosOfertas(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var parametrosOfertasDadosOfertas = Request.GetArrayParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOferta>("ParametrosOfertasDadosOfertas");

            if (parametrosOfertasDadosOfertas.Length < 1)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta repoParametrosOfertasDadosOferta = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana repoParametrosOfertasDadosOfertaDiaSemana = new(unitOfWork, cancellationToken);

            for (int i = 0; i < parametrosOfertasDadosOfertas.Length; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta parametrosOfertasDadosOferta = new()
                {
                    ParametrosOfertas = parametrosOfertas,
                    HoraInicio = TimeSpan.ParseExact(parametrosOfertasDadosOfertas[i].HoraInicio, @"hh\:mm", null),
                    HoraTermino = TimeSpan.ParseExact(parametrosOfertasDadosOfertas[i].HoraTermino, @"hh\:mm", null),
                    Raio = parametrosOfertasDadosOfertas[i].Raio,
                };

                long codigo = await repoParametrosOfertasDadosOferta.InserirAsync(parametrosOfertasDadosOferta, Auditado);

                parametrosOfertasDadosOferta.Codigo = (int)codigo;

                List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> listaParametrosOfertasDadosOfertaDiaSemana = MontarEntidadesParametrosOfertasDadosOfertaDiaSemana(
                    parametrosOfertasDadosOferta,
                    parametrosOfertasDadosOfertas[i].DiasSemana,
                    null
                );

                await repoParametrosOfertasDadosOfertaDiaSemana.InserirMuitosAsync(listaParametrosOfertasDadosOfertaDiaSemana, Auditado);
            }
        }

        private async Task AtualizarParametrosOfertasDadosOfertas(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta resultadoConsulta, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var entradas = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOferta>("ParametrosOfertasDadosOfertas");

            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta repoParametrosOfertasDadosOferta = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana repoParametrosOfertasDadosOfertaDiaSemana = new(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> atuais = await repoParametrosOfertasDadosOfertaDiaSemana.BuscarPorCodigosParametrosOfertaDadosOfertaAsync(resultadoConsulta.ParametrosOfertasDadosOfertas.Select(o => o.Codigo).ToList(), cancellationToken);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (entradas.Count == 0 && atuais.Count != 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana registro in atuais)
                {
                    await repoParametrosOfertasDadosOfertaDiaSemana.DeletarPorCodigoDadosOfertaAsync(registro.ParametrosOfertasDadosOferta.Codigo);
                    await repoParametrosOfertasDadosOferta.DeletarPorCodigoAsync(registro.ParametrosOfertasDadosOferta.Codigo);


                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Dados da Oferta",
                        De = $"{string.Join(",", registro.Descricao)} {registro.ParametrosOfertasDadosOferta.HoraInicio} Até {registro.ParametrosOfertasDadosOferta.HoraTermino} Raio {registro.ParametrosOfertasDadosOferta.Raio}",
                        Para = "",
                    });
                }
            }

            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOferta> novos = entradas.FindAll(o => o.Codigo == 0);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOferta> antigos = entradas.FindAll(o => o.Codigo != 0);
            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> aRemover = new();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOferta antigo in antigos)
                aRemover.AddRange(atuais.Where(o => o.ParametrosOfertasDadosOferta.Codigo == antigo.Codigo && !antigo.DiasSemana.Contains(o.DiaSemana)).ToList());


            for (int i = 0; i < aRemover.Count; i++)
            {
                await repoParametrosOfertasDadosOfertaDiaSemana.DeletarAsync(aRemover[i]);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Dados da Oferta",
                    De = $"{string.Join(",", aRemover[i].Descricao)} {aRemover[i].ParametrosOfertasDadosOferta.HoraInicio} Até {aRemover[i].ParametrosOfertasDadosOferta.HoraTermino} Raio {aRemover[i].ParametrosOfertasDadosOferta.Raio}",
                    Para = "",
                });
            }

            List<int> codigosDadosOfertaAtuais = atuais.Select(o => o.ParametrosOfertasDadosOferta.Codigo).ToList();

            for (int i = 0; i < codigosDadosOfertaAtuais.Count; i++)
            {
                if (await repoParametrosOfertasDadosOfertaDiaSemana.BuscarPorCodigoParametrosOfertaDadosOfertaAsync(codigosDadosOfertaAtuais[i], cancellationToken) == null)
                {
                    await repoParametrosOfertasDadosOferta.DeletarPorCodigoAsync(codigosDadosOfertaAtuais[i]);
                }
            }

            for (int i = 0; i < novos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta parametrosOfertasDadosOferta = new()
                {
                    ParametrosOfertas = parametrosOfertas,
                    HoraInicio = TimeSpan.ParseExact(novos[i].HoraInicio, @"hh\:mm", null),
                    HoraTermino = TimeSpan.ParseExact(novos[i].HoraTermino, @"hh\:mm", null),
                    Raio = novos[i].Raio,
                };

                long codigo = await repoParametrosOfertasDadosOferta.InserirAsync(parametrosOfertasDadosOferta, Auditado);

                parametrosOfertasDadosOferta.Codigo = (int)codigo;

                List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> listaParametrosOfertasDadosOfertaDiaSemana = MontarEntidadesParametrosOfertasDadosOfertaDiaSemana(
                    parametrosOfertasDadosOferta,
                    novos[i].DiasSemana,
                    alteracoes
                );

                await repoParametrosOfertasDadosOfertaDiaSemana.InserirMuitosAsync(listaParametrosOfertasDadosOfertaDiaSemana, Auditado);
            }

            parametrosOfertas.SetExternalChanges(alteracoes);
        }

        private async Task AtualizarParametrosOfertas(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            string novaDescricao = Request.GetStringParam("Descricao");
            string novoCodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            bool novaSituacao = Request.GetBoolParam("Ativo");
            int novoGrupoMotorista = Request.GetIntParam("GrupoMotoristas");

            Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas grupoMotoristas = null;
            if (novoGrupoMotorista != 0)
            {
                Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new(unitOfWork, cancellationToken);
                grupoMotoristas = await repositorioGrupoMotoristas.BuscarPorCodigoAsync(novoGrupoMotorista, false);
            }

            parametrosOfertas.Descricao = novaDescricao;
            parametrosOfertas.CodigoIntegracao = novoCodigoIntegracao;
            parametrosOfertas.Ativo = novaSituacao;
            parametrosOfertas.GrupoMotoristas = grupoMotoristas;

            await new Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertas(unitOfWork, cancellationToken).AtualizarAsync(parametrosOfertas, Auditado);
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta> ConsultarParametrosOfertas(int codigo, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertas repoParametrosOfertas = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana repoParametrosOfertasDadosOfertaDia = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao repoParametrosOfertasTipoIntegracao = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao repoParametrosOfertasTipoOperacao = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario repoParametrosOfertasFuncionario = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga repoParametrosOfertasTipoCarga = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa repoParametrosOfertasEmpresa = new(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial repoParametrosOfertasFilial = new(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas = await repoParametrosOfertas.BuscarPorCodigoAsync(codigo, Auditado != null);
            Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao grupoMotoristas = await repoParametrosOfertas.BuscarCodigosDescricaoAsync(codigo, cancellationToken);

            Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertas parametrosOfertasObj = new(null, null, null, null)
            {
                Codigo = parametrosOfertas.Codigo,
                CodigoIntegracao = parametrosOfertas.CodigoIntegracao,
                Descricao = parametrosOfertas.Descricao,
                Ativo = parametrosOfertas.Ativo,
                GrupoMotoristas = grupoMotoristas,
            };

            Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro = new() { CodigoParametrosOfertas = codigo };

            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> listaDadosOfertaDiaSemana = await repoParametrosOfertasDadosOfertaDia.BuscarAsync(
                new Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertasDadosOferta() { CodigoParametrosOfertas = codigo },
                null
            );

            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao> listaPOTipoIntegracao = await repoParametrosOfertasTipoIntegracao.BuscarAsync(filtro);

            var listaPOTipoOperacao = await repoParametrosOfertasTipoOperacao.BuscarCodigosDescricaoAsync(codigo);
            var listaPOFuncionario = await repoParametrosOfertasFuncionario.BuscarCodigosDescricaoAsync(codigo);
            var listaPOTipoCarga = await repoParametrosOfertasTipoCarga.BuscarCodigosDescricaoAsync(codigo);
            var listaPOEmpresa = await repoParametrosOfertasEmpresa.BuscarCodigosDescricaoAsync(codigo);
            var listaPOFilial = await repoParametrosOfertasFilial.BuscarCodigosDescricaoAsync(codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOfertaConsulta> listaConsultaDadosOferta = MontarResultadoConsultaDadosOferta(listaDadosOfertaDiaSemana);

            return new Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasConsulta()
            {
                ParametrosOfertas = parametrosOfertasObj,
                Empresas = [.. listaPOEmpresa.Select(o => o.ComoSuperclasse())],
                Filiais = [.. listaPOFilial],
                Funcionarios = [.. listaPOFuncionario],
                TiposCarga = [.. listaPOTipoCarga],
                TiposOperacao = [.. listaPOTipoOperacao],
                TiposIntegracao = [.. listaPOTipoIntegracao.Select(o => o.TipoIntegracao)],
                ParametrosOfertasDadosOfertas = listaConsultaDadosOferta,
            };
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOfertaConsulta> MontarResultadoConsultaDadosOferta(List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> listaDadosOfertaDiaSemana)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasDadosOfertaConsulta> listaConsultaDadosOferta = [];
            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta> dadosOferta = [.. listaDadosOfertaDiaSemana.Select(o => o.ParametrosOfertasDadosOferta)];

            dadosOferta = [.. dadosOferta.Distinct()];

            for (int i = 0; i < dadosOferta.Count; i++)
            {
                listaConsultaDadosOferta.Add(new()
                {
                    Codigo = dadosOferta[i].Codigo,
                    HoraInicio = dadosOferta[i].HoraInicio,
                    HoraTermino = dadosOferta[i].HoraTermino,
                    Raio = dadosOferta[i].Raio,
                    DiasSemana = [.. listaDadosOfertaDiaSemana.FindAll(o => o.ParametrosOfertasDadosOferta.Codigo == dadosOferta[i].Codigo).Select(o => o.DiaSemana)]
                });
            }

            return listaConsultaDadosOferta;
        }

        private async Task<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas> MontarEntidadeParametrosOfertas(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new(_conexao.StringConexao);

            int codigoGrupoMotoristas = Request.GetIntParam("GrupoMotoristas");

            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repositorioGrupoMotoristas = new(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas grupoMotoristas = await repositorioGrupoMotoristas.BuscarPorCodigoAsync(codigoGrupoMotoristas, false);

            Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas entidade = new()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Ativo = Request.GetBoolParam("Ativo"),
                GrupoMotoristas = grupoMotoristas,
            };

            entidade.Initialize();

            return entidade;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertas MontarFiltroPesquisa()
        {
            var situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoParametrosOfertas>("Situacao");
            bool? ativo = null;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoParametrosOfertas.Ativo)
            {
                ativo = true;
            }
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoParametrosOfertas.Inativo)
            {
                ativo = false;
            }

            return new()
            {
                Descricao = Request.GetNullableStringParam("Descricao"),
                CodigoIntegracao = Request.GetNullableStringParam("CodigoIntegracao"),
                Ativo = ativo,
            };
        }

        private static List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> MontarEntidadesParametrosOfertasDadosOfertaDiaSemana(Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOferta parametrosOfertasDadosOfertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana[] diasDaSemana, List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = null)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> parametrosOfertasDadosOfertaDiaSemanas = [];

            for (int i = 0; i < diasDaSemana.Length; i++)
            {
                parametrosOfertasDadosOfertaDiaSemanas.Add(new Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana()
                {
                    DiaSemana = diasDaSemana[i],
                    ParametrosOfertasDadosOferta = parametrosOfertasDadosOfertas
                });

                if (alteracoes != null)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Dados da Oferta",
                        De = "",
                        Para = $"{diasDaSemana[i].ObterDescricao()} {parametrosOfertasDadosOfertas.HoraInicio} Até {parametrosOfertasDadosOfertas.HoraTermino} Raio {parametrosOfertasDadosOfertas.Raio}",
                    });
                }
            }

            return parametrosOfertasDadosOfertaDiaSemanas;
        }

        private async Task AtualizarRelacionamento(
            Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas repositorio,
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> atual,
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> entrada,
            Func<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao, Task> inserir,
            string nomeEntidadeFracaRelacionamento, Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas parametrosOfertas
            )
        {
            var novos = entrada.FindAll(o => o.CodigoRelacionamento == 0);
            var antigos = entrada.FindAll(o => o.CodigoRelacionamento != 0);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (atual.Count > antigos.Count)
            {
                var aRemover = atual.ExceptBy(antigos.Select(o => o.CodigoEntidadeFraca), o => o.CodigoEntidadeFraca).ToList();

                for (int i = 0; i < aRemover.Count; i++)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = nomeEntidadeFracaRelacionamento,
                        De = aRemover[i].Descricao,
                        Para = "",
                    });

                    await repositorio.DeletarPorCodigoAsync(aRemover[i].CodigoRelacionamento);
                }
            }

            for (int i = 0; i < novos.Count; i++)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = nomeEntidadeFracaRelacionamento,
                    De = "",
                    Para = novos[i].Descricao,
                });

                await inserir(novos[i]);
            }

            parametrosOfertas.SetExternalChanges(alteracoes);
        }

        #endregion
    }
}
