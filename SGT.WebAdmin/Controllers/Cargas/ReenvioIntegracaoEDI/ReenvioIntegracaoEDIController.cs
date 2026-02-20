using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ReenvioIntegracaoEDI
{
    [CustomAuthorize("Cargas/ReenvioIntegracaoEDI")]
    public class ReenvioIntegracaoEDIController : BaseController
    {
		#region Construtores

		public ReenvioIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI repReenvioIntegracaoEDI = new Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI reenvio = repReenvioIntegracaoEDI.BuscarPorCodigo(codigo);

                // Valida
                if (reenvio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    reenvio.Codigo,
                    DataEnvio = reenvio.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                    Usuario = reenvio.Usuario.Nome,

                    Cargas = (from o in reenvio.Cargas select FormataCarga(o)).ToList(),
                    LayoutsEDI = (from o in reenvio.Layouts
                                  select new
                                  {
                                      o.Codigo,
                                      o.Descricao,
                                      o.DescricaoTipo,
                                  }).ToList(),
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                // Instancia repositorios
                Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI repReenvioIntegracaoEDI = new Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI reenvio = new Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI();

                // Preenche entidade com dados
                PreencheEntidade(ref reenvio, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(reenvio, out string erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.Start();
                repReenvioIntegracaoEDI.Inserir(reenvio, Auditado);

                ProcessarReenvio(reenvio, unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCarga();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarParaProcessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoCarga();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, cargas, ((dicionario) =>
                {
                    dicionario.TryGetValue("Carga", out dynamic dynNumeroCarga);
                    string numeroCarga = (string)dynNumeroCarga;

                    return repCarga.BuscarPorCodigoEmbarcador(numeroCarga);
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                cargas = cargas.Distinct().ToList();

                retorno.Importados = cargas.Count();
                retorno.Retorno = (from obj in cargas select FormataCarga(obj)).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Usuario").Nome("Usuário").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("DataEnvio").Nome("Data Envio").Tamanho(15).Align(Models.Grid.Align.center);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI repReenvioIntegracaoEDI = new Repositorio.Embarcador.Cargas.ReenvioIntegracaoEDI(unitOfWork);

            // Dados do filtro
            int layout = Request.GetIntParam("LayoutEDI");
            int usuario = Request.GetIntParam("Usuario");
            string carga = Request.GetStringParam("Carga");
            DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
            DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI> listaGrid = repReenvioIntegracaoEDI.Consultar(layout, usuario, carga, dataInicial, dataFinal, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repReenvioIntegracaoEDI.ContarConsulta(layout, usuario, carga, dataInicial, dataFinal);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Usuario = obj.Usuario.Nome,
                            obj.DataEnvio
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI reenvio, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

            List<Dominio.Entidades.LayoutEDI> layouts = new List<Dominio.Entidades.LayoutEDI>();
            List<dynamic> dynLayouts = Request.GetListParam<dynamic>("LayoutsEDI");
            foreach (dynamic dynLayout in dynLayouts)
            {
                int codigoLayout = ((string)dynLayout.Codigo).ToInt();
                Dominio.Entidades.LayoutEDI layout = repLayoutEDI.BuscarPorCodigo(codigoLayout);

                if (layout != null)
                    layouts.Add(layout);
            }


            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<dynamic> dynCargas = Request.GetListParam<dynamic>("Cargas");
            foreach (dynamic dynCarga in dynCargas)
            {
                int codigoCarga = ((string)dynCarga.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga != null)
                    cargas.Add(carga);
            }


            // Vincula dados
            reenvio.DataEnvio = DateTime.Now;
            reenvio.Usuario = this.Usuario;
            reenvio.Layouts = layouts.Distinct().ToList();
            reenvio.Cargas = cargas.Distinct().ToList();
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI reenvio, out string msgErro)
        {
            msgErro = "";

            if (reenvio.Layouts == null || reenvio.Layouts.Count == 0)
            {
                msgErro = "Nenhum Layout EDI selecionado.";
                return false;
            }

            if (reenvio.Cargas == null || reenvio.Cargas.Count == 0)
            {
                msgErro = "Nenhuma Carga selecionada.";
                return false;
            }

            return true;
        }

        private void ProcessarReenvio(Dominio.Entidades.Embarcador.Cargas.ReenvioIntegracaoEDI reenvio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> integracoes = repCargaEDIIntegracao.BuscarDadosParaReintegracaoEmMassa(reenvio.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasIteracao = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao integracao in integracoes)
            {
                integracao.IniciouConexaoExterna = false;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repCargaEDIIntegracao.Atualizar(integracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, "Reenviou Integração por reenvio em massa.", unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = integracao.Carga;
                carga.PossuiPendencia = false;
                repCarga.Atualizar(carga);

                cargasIteracao.Add(carga);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasIteracao.Distinct())
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Reenviou Integração EDI da Carga por reenvio em massa.", unitOfWork);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoCarga()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Carga", Propriedade = "Carga", Tamanho = 150, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Usuario") propOrdenar = "Usuario.Nome";
        }

        private dynamic FormataCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new
            {
                carga.Codigo,
                carga.CodigoCargaEmbarcador,
                Transportador = carga.Empresa?.Descricao ?? "",
                Filial = carga.Filial?.Descricao ?? "",
                Veiculo = carga.Veiculo?.Placa ?? ""
            };
        }
        #endregion
    }
}
