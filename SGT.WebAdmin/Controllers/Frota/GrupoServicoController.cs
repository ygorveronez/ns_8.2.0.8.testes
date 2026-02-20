using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/GrupoServico")]
    public class GrupoServicoController : BaseController
    {
		#region Construtores

		public GrupoServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = new Dominio.Entidades.Embarcador.Frota.GrupoServico();

                PreencherEntidade(grupoServico, unitOfWork);

                unitOfWork.Start();

                repositorioGrupoServico.Inserir(grupoServico, Auditado);

                SalvarServicosVeiculo(grupoServico, unitOfWork);
                SalvarMarcasVeiculo(grupoServico, unitOfWork);
                SalvarModelosVeiculo(grupoServico, unitOfWork);
                SalvarMarcasEquipamento(grupoServico, unitOfWork);
                SalvarModelosEquipamento(grupoServico, unitOfWork);
                SalvarLocaisManutencao(grupoServico, unitOfWork);

                AtualizarTotalizadorListas(grupoServico, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = repositorioGrupoServico.BuscarPorCodigo(codigo, true);

                if (grupoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(grupoServico, unitOfWork);

                unitOfWork.Start();

                SalvarMarcasVeiculo(grupoServico, unitOfWork);
                SalvarModelosVeiculo(grupoServico, unitOfWork);
                SalvarMarcasEquipamento(grupoServico, unitOfWork);
                SalvarModelosEquipamento(grupoServico, unitOfWork);
                SalvarLocaisManutencao(grupoServico, unitOfWork);

                repositorioGrupoServico.Atualizar(grupoServico, Auditado);

                SalvarServicosVeiculo(grupoServico, unitOfWork);

                AtualizarTotalizadorListas(grupoServico, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);
                Repositorio.Embarcador.Frota.GrupoServicoLocalManutencao repGrupoServicoLocalManutencao = new Repositorio.Embarcador.Frota.GrupoServicoLocalManutencao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = repositorioGrupoServico.BuscarPorCodigo(codigo, true);

                if (grupoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao> locaisManutencao = repGrupoServicoLocalManutencao.BuscarPorGrupoServico(grupoServico.Codigo);

                var dynGrupoServico = new
                {
                    grupoServico.Codigo,
                    grupoServico.Descricao,
                    Status = grupoServico.Ativo,
                    grupoServico.CodigoIntegracao,
                    grupoServico.Observacao,
                    grupoServico.KmFinal,
                    grupoServico.KmInicial,
                    grupoServico.DiaInicial,
                    grupoServico.DiaFinal,
                    grupoServico.TipoVeiculoEquipamento,
                    TipoOrdemServico = new { Codigo = grupoServico.TipoOrdemServico?.Codigo ?? 0, Descricao = grupoServico.TipoOrdemServico?.Descricao ?? string.Empty },
                    ServicosVeiculo = (from obj in grupoServico.ServicosVeiculo
                                       select new
                                       {
                                           obj.Codigo,
                                           obj.Tipo,
                                           obj.ToleranciaDias,
                                           obj.ToleranciaKM,
                                           obj.ValidadeDias,
                                           obj.ValidadeKM,
                                           obj.ValidadeHorimetro,
                                           obj.ToleranciaHorimetro,
                                           ServicoVeiculo = new
                                           {
                                               obj.ServicoVeiculoFrota.Codigo,
                                               obj.ServicoVeiculoFrota.Descricao
                                           }
                                       }).ToList(),
                    MarcasVeiculo = (from obj in grupoServico.MarcasVeiculo
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.Descricao
                                     }).ToList(),
                    MarcasEquipamento = (from obj in grupoServico.MarcasEquipamento
                                         select new
                                         {
                                             obj.Codigo,
                                             obj.Descricao
                                         }).ToList(),
                    ModelosVeiculo = (from obj in grupoServico.ModelosVeiculo
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Descricao
                                      }).ToList(),
                    ModelosEquipamento = (from obj in grupoServico.ModelosEquipamento
                                          select new
                                          {
                                              obj.Codigo,
                                              obj.Descricao
                                          }).ToList(),
                    LocaisManutencao = (from obj in locaisManutencao
                                        select new
                                        {
                                            Codigo = obj.Codigo,
                                            CNPJCPF = obj.Cliente.CPF_CNPJ,
                                            obj.Cliente.Descricao,
                                            Localidade = obj.Cliente.Localidade.DescricaoCidadeEstado,
                                            obj.Cliente.Latitude,
                                            obj.Cliente.Longitude
                                        }).ToList()
                };

                return new JsonpResult(dynGrupoServico);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico = repositorioGrupoServico.BuscarPorCodigo(codigo, true);

                if (grupoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioGrupoServico.Deletar(grupoServico, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseje mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

            int codigoOrdemServico = Request.GetIntParam("TipoOrdemServico");

            grupoServico.Ativo = Request.GetBoolParam("Status");
            grupoServico.Descricao = Request.Params("Descricao");
            grupoServico.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            grupoServico.Observacao = Request.Params("Observacao");
            grupoServico.DiaInicial = Request.GetIntParam("DiaInicial");
            grupoServico.DiaFinal = Request.GetIntParam("DiaFinal");
            grupoServico.KmInicial = Request.GetIntParam("KmInicial");
            grupoServico.KmFinal = Request.GetIntParam("KmFinal");
            grupoServico.TipoVeiculoEquipamento = Request.GetEnumParam<VeiculoEquipamento>("TipoVeiculoEquipamento");
            grupoServico.TipoOrdemServico = codigoOrdemServico > 0 ? repTipoOrdemServico.BuscarPorCodigo(codigoOrdemServico) : null;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("CodigoTipoOrdemServico", false);
                grid.AdicionarCabecalho("DescricaoTipoOrdemServico", false);

                Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.Frota.GrupoServico> listaGrupoServico = repositorioGrupoServico.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorioGrupoServico.ContarConsulta(filtrosPesquisa);

                var retorno = (from grupo in listaGrupoServico
                               select new
                               {
                                   grupo.Codigo,
                                   grupo.Descricao,
                                   grupo.DescricaoAtivo,
                                   CodigoTipoOrdemServico = grupo.TipoOrdemServico?.Codigo ?? 0,
                                   DescricaoTipoOrdemServico = grupo.TipoOrdemServico?.Descricao ?? string.Empty
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGrupoServico()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoTipoOrdemServico = Request.GetIntParam("Tipo"),
                Status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                CpfCnpjLocalManutencao = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? Usuario.Cliente?.CPF_CNPJ ?? 0d : 0d,
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void SalvarServicosVeiculo(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.GrupoServicoServicoVeiculo repGrupoServicoServicoVeiculo = new Repositorio.Embarcador.Frota.GrupoServicoServicoVeiculo(unitOfWork);

            dynamic dynServicosVeiculo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ServicosVeiculo"));

            if (grupoServico.ServicosVeiculo != null && grupoServico.ServicosVeiculo.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var servicoVeiculo in dynServicosVeiculo)
                    if (servicoVeiculo.Codigo != null)
                        codigos.Add((int)servicoVeiculo.Codigo);

                List<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo> servicoRemover = (from obj in grupoServico.ServicosVeiculo where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < servicoRemover.Count; i++)
                    repGrupoServicoServicoVeiculo.Deletar(servicoRemover[i]);
            }
            else
                grupoServico.ServicosVeiculo = new List<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo>();

            foreach (var dynServicoVeiculo in dynServicosVeiculo)
            {
                int.TryParse((string)dynServicoVeiculo.Codigo, out int codigo);

                Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo servico = codigo > 0 ? repGrupoServicoServicoVeiculo.BuscarPorCodigo(codigo) : null;
                if (servico == null)
                    servico = new Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo();

                int codigoServico = ((string)dynServicoVeiculo.ServicoVeiculo.Codigo).ToInt();

                servico.GrupoServico = grupoServico;
                servico.ServicoVeiculoFrota = repServicoVeiculoFrota.BuscarPorCodigo(codigoServico);

                servico.Tipo = ((string)dynServicoVeiculo.Tipo).ToEnum<TipoServicoVeiculo>();
                servico.ToleranciaDias = ((string)dynServicoVeiculo.ToleranciaDias).ToInt();
                servico.ValidadeDias = ((string)dynServicoVeiculo.ValidadeDias).ToInt();
                servico.ToleranciaKM = ((string)dynServicoVeiculo.ToleranciaKM).ToInt();
                servico.ValidadeKM = ((string)dynServicoVeiculo.ValidadeKM).ToInt();
                servico.ValidadeHorimetro = ((string)dynServicoVeiculo.ValidadeHorimetro).ToInt();
                servico.ToleranciaHorimetro = ((string)dynServicoVeiculo.ToleranciaHorimetro).ToInt();

                if (servico.Codigo > 0)
                    repGrupoServicoServicoVeiculo.Atualizar(servico);
                else
                    repGrupoServicoServicoVeiculo.Inserir(servico);
            }
        }

        private void SalvarMarcasVeiculo(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);

            dynamic dynMarcasVeiculo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("MarcasVeiculo"));

            if (grupoServico.MarcasVeiculo != null && grupoServico.MarcasVeiculo.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var marcaVeiculo in dynMarcasVeiculo)
                    if (marcaVeiculo.Codigo != null)
                        codigos.Add((int)marcaVeiculo.Codigo);

                List<Dominio.Entidades.MarcaVeiculo> entidadeRemover = (from obj in grupoServico.MarcasVeiculo where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < entidadeRemover.Count; i++)
                    grupoServico.MarcasVeiculo.Remove(entidadeRemover[i]);
            }
            else
                grupoServico.MarcasVeiculo = new List<Dominio.Entidades.MarcaVeiculo>();

            foreach (var dynMarcaVeiculo in dynMarcasVeiculo)
            {
                int.TryParse((string)dynMarcaVeiculo.Codigo, out int codigo);

                Dominio.Entidades.MarcaVeiculo marcaVeiculo = codigo > 0 ? (from obj in grupoServico.MarcasVeiculo where obj.Codigo == codigo select obj).FirstOrDefault() : null;
                if (marcaVeiculo == null)
                {
                    marcaVeiculo = repMarcaVeiculo.BuscarPorCodigo(codigo, false);
                    grupoServico.MarcasVeiculo.Add(marcaVeiculo);
                }
            }
        }

        private void SalvarModelosVeiculo(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);

            dynamic dynModelosVeiculo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeiculo"));

            if (grupoServico.ModelosVeiculo != null && grupoServico.ModelosVeiculo.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var modeloVeiculo in dynModelosVeiculo)
                    if (modeloVeiculo.Codigo != null)
                        codigos.Add((int)modeloVeiculo.Codigo);

                List<Dominio.Entidades.ModeloVeiculo> entidadeRemover = (from obj in grupoServico.ModelosVeiculo where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < entidadeRemover.Count; i++)
                    grupoServico.ModelosVeiculo.Remove(entidadeRemover[i]);
            }
            else
                grupoServico.ModelosVeiculo = new List<Dominio.Entidades.ModeloVeiculo>();

            foreach (var dyModeloVeiculo in dynModelosVeiculo)
            {
                int.TryParse((string)dyModeloVeiculo.Codigo, out int codigo);

                Dominio.Entidades.ModeloVeiculo modeloVeiculo = codigo > 0 ? (from obj in grupoServico.ModelosVeiculo where obj.Codigo == codigo select obj).FirstOrDefault() : null;
                if (modeloVeiculo == null)
                {
                    modeloVeiculo = repModeloVeiculo.BuscarPorCodigo(codigo, false);
                    grupoServico.ModelosVeiculo.Add(modeloVeiculo);
                }
            }
        }

        private void SalvarMarcasEquipamento(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.MarcaEquipamento repMarcaEquipamento = new Repositorio.Embarcador.Veiculos.MarcaEquipamento(unitOfWork);

            dynamic dynMarcasEquipamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("MarcasEquipamento"));

            if (grupoServico.MarcasEquipamento != null && grupoServico.MarcasEquipamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var marcaEquipamento in dynMarcasEquipamento)
                    if (marcaEquipamento.Codigo != null)
                        codigos.Add((int)marcaEquipamento.Codigo);

                List<Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento> entidadeRemover = (from obj in grupoServico.MarcasEquipamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < entidadeRemover.Count; i++)
                    grupoServico.MarcasEquipamento.Remove(entidadeRemover[i]);
            }
            else
                grupoServico.MarcasEquipamento = new List<Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento>();

            foreach (var dyMarcaEquipamento in dynMarcasEquipamento)
            {
                int.TryParse((string)dyMarcaEquipamento.Codigo, out int codigo);

                Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento marcaEquipamento = codigo > 0 ? (from obj in grupoServico.MarcasEquipamento where obj.Codigo == codigo select obj).FirstOrDefault() : null;
                if (marcaEquipamento == null)
                {
                    marcaEquipamento = repMarcaEquipamento.BuscarPorCodigo(codigo, false);
                    grupoServico.MarcasEquipamento.Add(marcaEquipamento);
                }
            }
        }

        private void SalvarModelosEquipamento(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.ModeloEquipamento repModeloEquipamento = new Repositorio.Embarcador.Veiculos.ModeloEquipamento(unitOfWork);

            dynamic dynModelosEquipamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosEquipamento"));

            if (grupoServico.ModelosEquipamento != null && grupoServico.ModelosEquipamento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var marcaEquipamento in dynModelosEquipamento)
                    if (marcaEquipamento.Codigo != null)
                        codigos.Add((int)marcaEquipamento.Codigo);

                List<Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento> entidadeRemover = (from obj in grupoServico.ModelosEquipamento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < entidadeRemover.Count; i++)
                    grupoServico.ModelosEquipamento.Remove(entidadeRemover[i]);
            }
            else
                grupoServico.ModelosEquipamento = new List<Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento>();

            foreach (var dyMarcaEquipamento in dynModelosEquipamento)
            {
                int.TryParse((string)dyMarcaEquipamento.Codigo, out int codigo);

                Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento modeloEquipamento = codigo > 0 ? (from obj in grupoServico.ModelosEquipamento where obj.Codigo == codigo select obj).FirstOrDefault() : null;
                if (modeloEquipamento == null)
                {
                    modeloEquipamento = repModeloEquipamento.BuscarPorCodigo(codigo, false);
                    grupoServico.ModelosEquipamento.Add(modeloEquipamento);
                }
            }
        }

        private void SalvarLocaisManutencao(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.GrupoServicoLocalManutencao repGrupoServicoLocalManutencao = new Repositorio.Embarcador.Frota.GrupoServicoLocalManutencao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynLocaisManutencao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LocaisManutencao"));

            List<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao> locaisManutencao = repGrupoServicoLocalManutencao.BuscarPorGrupoServico(grupoServico.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (locaisManutencao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic localManutencao in dynLocaisManutencao)
                {
                    int codigo = ((string)localManutencao.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao> listaDeletar = (from obj in locaisManutencao where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (var deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Local Manutenção",
                        De = $"{deletar.Cliente.Descricao}",
                        Para = ""
                    });

                    repGrupoServicoLocalManutencao.Deletar(deletar);
                }
            }

            foreach (dynamic localManutencao in dynLocaisManutencao)
            {
                int codigo = ((string)localManutencao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao grupoServicoLocalManutencao = codigo > 0 ? repGrupoServicoLocalManutencao.BuscarPorCodigo(codigo, false) : null;

                if (grupoServicoLocalManutencao == null)
                {
                    grupoServicoLocalManutencao = new Dominio.Entidades.Embarcador.Frota.GrupoServicoLocalManutencao();

                    double CnpjCpfCliente = ((string)localManutencao.CNPJCPF).ToDouble();

                    grupoServicoLocalManutencao.GrupoServico = grupoServico;
                    grupoServicoLocalManutencao.Cliente = repCliente.BuscarPorCPFCNPJ(CnpjCpfCliente);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Local Manutenção",
                        De = "",
                        Para = $"{grupoServicoLocalManutencao.Cliente.Descricao}"
                    });

                    repGrupoServicoLocalManutencao.Inserir(grupoServicoLocalManutencao);
                }
            }

            grupoServico.SetExternalChanges(alteracoes);
        }

        private void AtualizarTotalizadorListas(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.GrupoServico repositorioGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);

            grupoServico.PossuiMarcasEquipamento = grupoServico.MarcasEquipamento?.Count > 0;
            grupoServico.PossuiMarcasVeiculo = grupoServico.MarcasVeiculo?.Count > 0;
            grupoServico.PossuiModelosEquipamento = grupoServico.ModelosEquipamento?.Count > 0;
            grupoServico.PossuiModelosVeiculo = grupoServico.ModelosVeiculo?.Count > 0;

            repositorioGrupoServico.Atualizar(grupoServico);
        }

        #endregion
    }
}
