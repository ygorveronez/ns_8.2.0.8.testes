using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Frota.Programacao
{
    [CustomAuthorize("Frota/ProgramacaoVeiculo")]
    public class ProgramacaoVeiculoController : BaseController
    {
		#region Construtores

		public ProgramacaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                int totalRegistros = 0, novoLimite = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, ref novoLimite);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);
                if (novoLimite > 0)
                    grid.limite = novoLimite;

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                int totalRegistros = 0, novoLimite = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, ref novoLimite);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);
                if (novoLimite > 0)
                    grid.limite = novoLimite;

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo programacaoVeiculo = repProgramacaoVeiculo.BuscarPorCodigo(codigo);

                // Valida
                if (programacaoVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    programacaoVeiculo.Codigo,
                    Veiculo = programacaoVeiculo.Veiculo != null ? new { Codigo = programacaoVeiculo.Veiculo.Codigo, Descricao = programacaoVeiculo.Veiculo.Placa } : null,
                    Motorista = programacaoVeiculo.Motorista != null ? new { Codigo = programacaoVeiculo.Motorista.Codigo, Descricao = programacaoVeiculo.Motorista.Nome } : null,
                    ProgramacaoLicenciamento = programacaoVeiculo.ProgramacaoLicenciamento != null ? new { Codigo = programacaoVeiculo.ProgramacaoLicenciamento.Codigo, Descricao = programacaoVeiculo.ProgramacaoLicenciamento.Descricao } : null,
                    ProgramacaoAlocacao = programacaoVeiculo.ProgramacaoAlocacao != null ? new { Codigo = programacaoVeiculo.ProgramacaoAlocacao.Codigo, Descricao = programacaoVeiculo.ProgramacaoAlocacao.Descricao } : null,
                    ProgramacaoEspecialidade = programacaoVeiculo.ProgramacaoEspecialidade != null ? new { Codigo = programacaoVeiculo.ProgramacaoEspecialidade.Codigo, Descricao = programacaoVeiculo.ProgramacaoEspecialidade.Descricao } : null,
                    programacaoVeiculo.Pallets,
                    ProgramacaoSituacao = programacaoVeiculo.ProgramacaoSituacao != null ? new { Codigo = programacaoVeiculo.ProgramacaoSituacao.Codigo, Descricao = programacaoVeiculo.ProgramacaoSituacao.Descricao } : null,
                    Cliente = programacaoVeiculo.Cliente != null ? new { Codigo = programacaoVeiculo.Cliente.CPF_CNPJ, Descricao = programacaoVeiculo.Cliente.Descricao } : null,
                    Destino = programacaoVeiculo.Localidade != null ? new { Codigo = programacaoVeiculo.Localidade.Codigo, Descricao = programacaoVeiculo.Localidade.Descricao } : null,
                    DataInicio = programacaoVeiculo.DataInicio != null && programacaoVeiculo.DataInicio.HasValue ? programacaoVeiculo.DataInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataTermino = programacaoVeiculo.DataTermino != null && programacaoVeiculo.DataTermino.HasValue ? programacaoVeiculo.DataTermino.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSumarizadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Parametros
                string numeroFrota = Request.GetStringParam("NumeroFrota");

                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Reboque"), out int codigoReboque);
                int.TryParse(Request.Params("ModeloVeicular"), out int codigoModeloVeicular);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out int codigoProgramacaoSituacao);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoLicenciamento"), out int codigoProgramacaoLicenciamento);
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out int codigoProgramacaoAlocacao);
                int.TryParse(Request.Params("ProgramacaoEspecialidade"), out int codigoProgramacaoEspecialidade);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>();
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Veiculo);
                finalidades.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao.Todos);

                // Busca informacoes
                int totalRegistros = repProgramacaoVeiculo.ContarConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, codigoProgramacaoSituacao, codigoMotorista, codigoProgramacaoLicenciamento, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade);
                List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> situacoesMotorista = repProgramacaoSituacao.BuscarSituacoes(finalidades);
                // Formata retorno
                var retorno = new
                {
                    Status = situacoesMotorista != null ? (from obj in situacoesMotorista
                                                           select new
                                                           {
                                                               Descricao = obj.Descricao,
                                                               Quantidade = repProgramacaoVeiculo.ContarConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, obj.Codigo, codigoMotorista, codigoProgramacaoLicenciamento, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade).ToString("n0")
                                                           }).ToList() : null

                };
                var todos = new
                {
                    Descricao = "Todos",
                    Quantidade = totalRegistros.ToString("n0")
                };
                retorno.Status.Add(todos);

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados sumarizados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoEspecialidade repProgramacaoEspecialidade = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoEspecialidade(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao repProgramacaoAlocacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoAlocacao(unitOfWork);
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoLicenciamento repProgramacaoLicenciamento = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoLicenciamento(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo programacao = null;
                if (codigo > 0)
                    programacao = repProgramacaoVeiculo.BuscarPorCodigo(codigo, true);
                else
                    programacao = new Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo();

                // Valida

                int codigoProgramacaoAlocacao = 0, codigoMotorista = 0, codigoProgramacaoEspecialidade = 0, codigoProgramacaoSituacao = 0, codigoVeiculo = 0, codigoProgramacaoLicenciamento = 0, codigoDestino = 0;
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("ProgramacaoLicenciamento"), out codigoProgramacaoLicenciamento);
                int.TryParse(Request.Params("ProgramacaoAlocacao"), out codigoProgramacaoAlocacao);
                int.TryParse(Request.Params("ProgramacaoEspecialidade"), out codigoProgramacaoEspecialidade);
                int.TryParse(Request.Params("ProgramacaoSituacao"), out codigoProgramacaoSituacao);
                int.TryParse(Request.Params("Destino"), out codigoDestino);

                decimal pallets = 0;
                decimal.TryParse(Request.Params("Pallets"), out pallets);

                double cnpjCliente = 0;
                double.TryParse(Request.Params("Cliente"), out cnpjCliente);

                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                DateTime? dataTermino = Request.GetNullableDateTimeParam("DataTermino");

                unitOfWork.Start();

                programacao.Cliente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
                programacao.DataInicio = dataInicio.HasValue ? dataInicio : null;
                programacao.DataTermino = dataTermino.HasValue ? dataTermino : null;
                programacao.Empresa = this.Usuario.Empresa;
                programacao.Localidade = codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoDestino) : null;
                programacao.Motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;
                programacao.Pallets = pallets;
                programacao.ProgramacaoAlocacao = codigoProgramacaoAlocacao > 0 ? repProgramacaoAlocacao.BuscarPorCodigo(codigoProgramacaoAlocacao) : null;
                programacao.ProgramacaoEspecialidade = codigoProgramacaoEspecialidade > 0 ? repProgramacaoEspecialidade.BuscarPorCodigo(codigoProgramacaoEspecialidade) : null;
                programacao.ProgramacaoLicenciamento = codigoProgramacaoLicenciamento > 0 ? repProgramacaoLicenciamento.BuscarPorCodigo(codigoProgramacaoLicenciamento) : null;
                programacao.ProgramacaoSituacao = codigoProgramacaoSituacao > 0 ? repProgramacaoSituacao.BuscarPorCodigo(codigoProgramacaoSituacao) : null;
                programacao.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

                if (codigo > 0)
                    repProgramacaoVeiculo.Atualizar(programacao, Auditado);
                else
                    repProgramacaoVeiculo.Inserir(programacao, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicioGrid, int limiteGrid, Repositorio.UnitOfWork unitOfWork, ref int novoLimite)
        {
            Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo repProgramacaoVeiculo = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoVeiculo(unitOfWork);

            string numeroFrota = Request.GetStringParam("NumeroFrota");

            int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
            int.TryParse(Request.Params("Reboque"), out int codigoReboque);
            int.TryParse(Request.Params("ModeloVeicular"), out int codigoModeloVeicular);
            int.TryParse(Request.Params("ProgramacaoSituacao"), out int codigoProgramacaoSituacao);
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            int.TryParse(Request.Params("ProgramacaoLicenciamento"), out int codigoProgramacaoLicenciamento);
            int.TryParse(Request.Params("ProgramacaoAlocacao"), out int codigoProgramacaoAlocacao);
            int.TryParse(Request.Params("ProgramacaoEspecialidade"), out int codigoProgramacaoEspecialidade);

            bool.TryParse(Request.Params("HabilitarPainel"), out bool habilitadoPainel);

            int.TryParse(Request.Params("inicio"), out int inicio);
            int.TryParse(Request.Params("limite"), out int limite);
            if (inicio <= 0)
                inicio = inicioGrid;

            if (!habilitadoPainel)
                limite = limite * 2;
            if (limite <= 0)
                limite = limiteGrid;
            else
                novoLimite = limite;

            PropOrdena(ref propOrdenar);

            List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo> listaGrid = repProgramacaoVeiculo.ConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, codigoProgramacaoSituacao, codigoMotorista, codigoProgramacaoLicenciamento, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProgramacaoVeiculo.ContarConsultarProgramacaoVeiculo(numeroFrota, codigoVeiculo, codigoReboque, codigoModeloVeicular, codigoProgramacaoSituacao, codigoMotorista, codigoProgramacaoLicenciamento, codigoProgramacaoAlocacao, codigoProgramacaoEspecialidade);

            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Veiculo = obj.Veiculo.Placa,
                             Reboques = obj.Veiculo.VeiculosVinculados != null && obj.Veiculo.VeiculosVinculados.Count > 0 ? string.Join(", ", obj.Veiculo.VeiculosVinculados.Select(o => o.Placa)) : string.Empty,
                             Motorista = obj.Motorista?.Nome ?? string.Empty,
                             Cliente = obj.Cliente?.Nome ?? string.Empty,
                             Localidade = obj.Localidade?.Descricao ?? string.Empty,
                             DataInicio = obj.DataInicio != null && obj.DataInicio.HasValue ? obj.DataInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             DataTermino = obj.DataTermino != null && obj.DataTermino.HasValue ? obj.DataTermino.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             Situacao = obj.ProgramacaoSituacao?.Descricao ?? "",
                             DT_RowColor = obj.ProgramacaoSituacao != null ? obj.ProgramacaoSituacao.DescricaoCor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco,
                             DT_FontColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Black
                         }).ToList();

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo");
            grid.Prop("Veiculo").Nome("Placa").Align(Models.Grid.Align.left).Tamanho(10);
            grid.Prop("Reboques").Nome("Reboques").Align(Models.Grid.Align.left).Tamanho(10).Ord(false);
            grid.Prop("Motorista").Nome("Motorista").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("Cliente").Nome("Cliente").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("Localidade").Nome("Destino").Align(Models.Grid.Align.left).Tamanho(15);
            grid.Prop("DataInicio").Nome("Início").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("DataTermino").Nome("Término").Align(Models.Grid.Align.center).Tamanho(10);
            grid.Prop("Situacao").Nome("Situação").Align(Models.Grid.Align.center).Tamanho(10);

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "NumeroFrota") propOrdenar = "Veiculo.NumeroFrota";
            else if (propOrdenar == "Veiculo") propOrdenar = "Veiculo.Placa";
            else if (propOrdenar == "ModeloVeicularCarga") propOrdenar = "Veiculo.ModeloVeicularCarga.Descricao";
            else if (propOrdenar == "Motorista") propOrdenar = "Motorista.Nome";
            else if (propOrdenar == "ProgramacaoLicenciamento") propOrdenar = "ProgramacaoLicenciamento.Descricao";
            else if (propOrdenar == "ProgramacaoAlocacao") propOrdenar = "ProgramacaoAlocacao.Descricao";
            else if (propOrdenar == "ProgramacaoEspecialidade") propOrdenar = "ProgramacaoEspecialidade.Descricao";
            else if (propOrdenar == "ProgramacaoSituacao") propOrdenar = "ProgramacaoSituacao.Descricao";
            else if (propOrdenar == "Cliente") propOrdenar = "Cliente.Nome";
            else if (propOrdenar == "Localidade") propOrdenar = "Localidade.Descricao";
        }

        #endregion
    }
}
