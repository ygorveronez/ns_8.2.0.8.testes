using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/AdvertenciaTransportador")]
    public class AdvertenciaTransportadorController : BaseController
    {
		#region Construtores

		public AdvertenciaTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertenciaTransportador = new Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador();

                PreencherAdvertenciaTransportador(advertenciaTransportador, unitOfWork);

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(unitOfWork);

                repositorio.Inserir(advertenciaTransportador, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorioAdvertenciaTransportador = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertencia = repositorioAdvertenciaTransportador.BuscarPorCodigo(codigo, false);

                if (advertencia == null)
                    return new JsonpResult(false, "Registro não encontrado");

                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> listaFechamentoPontuacaoTransportador = repositorioFechamentoPontuacaoTransportador.BuscarPorTransportadores(new List<int> { advertencia.Transportador.Codigo });

                if (!PermiteExcluirOuAlterarAdvertencia(advertencia, listaFechamentoPontuacaoTransportador))
                    return new JsonpResult(true, "Não é possível excluir o registro pois o mesmo já foi utilizado para contagem de pontuação.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, advertencia, "Deletou o registro", unitOfWork);

                repositorioAdvertenciaTransportador.Deletar(advertencia);

                unitOfWork.CommitChanges();
                
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o registro.");
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                DateTime? data = Request.GetNullableDateTimeParam("Data");
                int codigoMotivo = Request.GetIntParam("Motivo");
                string observacao = Request.GetStringParam("Observacao");
                
                Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorioAdvertenciaTransportador = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertencia = repositorioAdvertenciaTransportador.BuscarPorCodigo(codigo, false);

                if (advertencia == null)
                    return new JsonpResult(false, "Registro não encontrado");
                
                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> listaFechamentoPontuacaoTransportador = repositorioFechamentoPontuacaoTransportador.BuscarPorTransportadores(new List<int> { advertencia.Transportador.Codigo });

                if (!PermiteExcluirOuAlterarAdvertencia(advertencia, listaFechamentoPontuacaoTransportador))
                    return new JsonpResult(true, "Não é possível alterar o registro pois o mesmo já foi utilizado para contagem de pontuação.");

                Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorioMotivo = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivo = codigoMotivo > 0 ? repositorioMotivo.BuscarPorCodigo(codigoMotivo, false) : null;

                if (data.HasValue)
                {
                    if (listaFechamentoPontuacaoTransportador.Any(obj => obj.FechamentoPontuacao.Ano == data.Value.Year && (int)obj.FechamentoPontuacao.Mes == data.Value.Month))
                        return new JsonpResult(true, $"Já existe um fechamento de pontuação pertencente a data {data.Value.ToString("dd/MM/yyyy")}.");

                    advertencia.Data = data.Value;
                }

                if (motivo != null)
                    advertencia.Motivo = motivo;

                if (!string.IsNullOrWhiteSpace(observacao))
                    advertencia.Observacao = observacao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, advertencia, "Atualizou o registro", unitOfWork);
                
                repositorioAdvertenciaTransportador.Atualizar(advertencia);

                unitOfWork.CommitChanges();
                
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o registro.");
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

        private void PreencherAdvertenciaTransportador(Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertenciaTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador repositorioMotivo = new Repositorio.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador(unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Frete.Pontuacao.MotivoAdvertenciaTransportador motivo = repositorioMotivo.BuscarPorCodigo(Request.GetIntParam("Motivo"), auditavel: false) ?? throw new ControllerException("O motivo da advertência deve ser informado.");
            Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo(Request.GetIntParam("Transportador"), auditavel: false) ?? throw new ControllerException("O transportador da advertência deve ser informado.");

            advertenciaTransportador.Data = DateTime.Now;
            advertenciaTransportador.Motivo = motivo;
            advertenciaTransportador.Observacao = Request.GetStringParam("Observacao");
            advertenciaTransportador.Pontuacao = motivo.Pontuacao;
            advertenciaTransportador.Transportador = transportador;
            advertenciaTransportador.Usuario = this.Usuario;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoMotivo = Request.GetIntParam("Motivo"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("CodigoMotivo", false);
                grid.AdicionarCabecalho("PermiteAlterarOuExcluirAdvertencia", false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CNPJ Transportador", "CnpjTransportador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo", "Motivo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pontuação", "Pontuacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 25, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorio = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador> listaAdvertenciaTransportador = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador>();

                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(unitOfWork);
                List<Dominio.Entidades.Empresa> transportadores = listaAdvertenciaTransportador.Select(obj => obj.Transportador).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> listaFechamentoPontuacaoTransportador = repositorioFechamentoPontuacaoTransportador.BuscarPorTransportadores(transportadores.Select(obj => obj.Codigo).ToList());

                var listaAdvertenciaTransportadorRetornar = (
                    from advertencia in listaAdvertenciaTransportador
                    select new
                    {
                        advertencia.Codigo,
                        Transportador = advertencia.Transportador.Descricao,
                        CodigoTransportador = advertencia.Transportador.Codigo,
                        advertencia.CnpjTransportador,
                        Motivo = advertencia.Motivo.Descricao,
                        CodigoMotivo = advertencia.Motivo.Codigo,
                        Data = advertencia.Data.ToString("dd/MM/yyyy HH:mm"),
                        advertencia.Pontuacao,
                        advertencia.Observacao,
                        PermiteAlterarOuExcluirAdvertencia = PermiteExcluirOuAlterarAdvertencia(advertencia, (from obj in listaFechamentoPontuacaoTransportador where obj.Transportador.Codigo == advertencia.Transportador.Codigo select obj).ToList())
                    }
                ).ToList();

                grid.AdicionaRows(listaAdvertenciaTransportadorRetornar);
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
            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            if (propriedadeOrdenar == "Motivo")
                return "Motivo.Descricao";

            return propriedadeOrdenar;
        }

        private bool PermiteExcluirOuAlterarAdvertencia(Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador advertencia, List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> listaFechamentoPontuacaoTransportador)
        {
            DateTime dataInicialFechamento = new DateTime(advertencia.Data.Year, advertencia.Data.Month, 1);

            return listaFechamentoPontuacaoTransportador
                .Where(obj => obj.FechamentoPontuacao.Ano == dataInicialFechamento.Year &&
                              (int)obj.FechamentoPontuacao.Mes == dataInicialFechamento.Month)
                .ToList()
                .Count == 0;
        }

        #endregion
    }
}
