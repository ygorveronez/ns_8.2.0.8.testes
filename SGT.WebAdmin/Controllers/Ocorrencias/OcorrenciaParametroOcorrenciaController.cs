using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/OcorrenciaParametroOcorrencia")]
    public class OcorrenciaParametroOcorrenciaController : BaseController
    {
        #region Construtores

        public OcorrenciaParametroOcorrenciaController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> PesquisaOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaOcorrencias();

                // Busca Dados
                int.TryParse(Request.Params("Codigo"), out int codigo);

                if (codigo > 0)
                {
                    int totalRegistros = 0;
                    var lista = ConsultarOcorrenciasGeradas(codigo, ref totalRegistros, "", grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                    // Seta valores na grid
                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(totalRegistros);
                }
                else
                {
                    var lista = CalcularOcorrenciasGeradas(unitOfWork);

                    // Seta valores na grid
                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(lista.Count());
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Não foi possível encontrar uma tabela de ocorrência para os parâmetros informados.");
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
                Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia repOcorrenciaParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia ocorrencia = repOcorrenciaParametroOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (ocorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    ocorrencia.Codigo,
                    ocorrencia.Numero,
                    Carga = new { ocorrencia.Carga.Codigo, Descricao = ocorrencia.Carga.CodigoCargaEmbarcador },
                    TipoOcorrencia = new { ocorrencia.TipoOcorrencia.Codigo, Descricao = ocorrencia.TipoOcorrencia.Descricao },
                    DataParametro = ocorrencia.Data.ToString("dd/MM/yyyy"),
                    Observacao = ocorrencia.Observacao,
                    ObservacaoCTe = ocorrencia.ObservacaoCTe,

                    Parametros = new
                    {
                        KmInicial = ocorrencia.KmInicial,
                        KmFinal = ocorrencia.KmFinal,
                        Data = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy"),
                        HoraInicial = ocorrencia.HoraInicial.ToString(@"hh\:mm"),
                        HoraFinal = ocorrencia.HoraFinal.ToString(@"hh\:mm"),
                        EnumTipoParametro = ocorrencia.TipoParametro
                    }
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
                // Inicia transacao
                await unitOfWork.StartAsync();

                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia repOcorrenciaParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado repOcorrenciaParametroOcorrenciaGerado = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia();

                // Preenche entidade com dados
                PreencheEntidade(ref ocorrencia, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(ocorrencia, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                await repOcorrenciaParametroOcorrencia.InserirAsync(ocorrencia);

                List<dynamic> valoresAlterados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("ValoresAlterados"));

                if (valoresAlterados == null) valoresAlterados = new List<dynamic>();

                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> ocorrenciasGeradas = CalcularOcorrenciasGeradas(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro ocorrenciaGeradas in ocorrenciasGeradas)
                {
                    bool valorAlterado = false;
                    decimal valor = ocorrenciaGeradas.Valor;

                    foreach (dynamic valorAlterador in valoresAlterados)
                    {
                        if ((int)valorAlterador.Codigo == ocorrenciaGeradas.Codigo)
                        {
                            valorAlterado = true;
                            decimal.TryParse((string)valorAlterador.Valor, out valor);
                            break;
                        }
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado dadoOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado()
                    {
                        OcorrenciaParametroOcorrencia = ocorrencia,
                        Descricao = ocorrenciaGeradas.Descricao,
                        Valor = valor,
                        ValorAlterado = valorAlterado,
                        ComponenteFrete = await repComponenteFrete.BuscarPorCodigoAsync(ocorrenciaGeradas.ComponenteFrete, false)
                    };

                    await repOcorrenciaParametroOcorrenciaGerado.InserirAsync(dadoOcorrencia);

                    // Gera a ocorrencia
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia()
                    {
                        NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                        Carga = ocorrencia.Carga,
                        ComponenteFrete = dadoOcorrencia.ComponenteFrete,
                        DataOcorrencia = ocorrencia.DataOcorrencia,
                        ObservacaoCTe = ocorrencia.ObservacaoCTe,
                        Observacao = dadoOcorrencia.Descricao,
                        SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada,
                        TipoOcorrencia = ocorrencia.TipoOcorrencia,
                        ValorOcorrencia = dadoOcorrencia.Valor,
                        ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga,
                        DataAlteracao = DateTime.Now,
                        DataFinalizacaoEmissaoOcorrencia = DateTime.Now
                    };

                    await GerarOcorrencia(cargaOcorrencia, unitOfWork);
                }

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterParametros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("TipoOcorrencia"), out int tipoOcorrencia);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                DateTime.TryParse(Request.Params("DataParametro"), out DateTime dataParametro);

                if (tipoOcorrencia == 0)
                    return new JsonpResult(false, true, "Tipo Ocorrência é obrigatório.");

                if (codigoCarga == 0)
                    return new JsonpResult(false, true, "Carga é obrigatório.");

                if (dataParametro == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data Parâmetro é obrigatório.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.ObterValorParametroOcorrencia(tipoOcorrencia, carga.Codigo, dataParametro, unitOfWork);

                // Valida
                if (valorParametroOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS registroSaidaVeiculo = null;
                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS registroEntradaVeiculo = null;
                if (carga != null && carga.Veiculo != null)
                {
                    registroSaidaVeiculo = repGuaritaTMS.BuscarRegistroVeiculo(carga.Veiculo.Codigo, dataParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida);
                    registroEntradaVeiculo = repGuaritaTMS.BuscarRegistroVeiculo(carga.Veiculo.Codigo, dataParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada);
                }

                // Formata retorno
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia tipoParametroCalculoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Escolta;

                if (valorParametroOcorrencia.ValorParametroHoraExtraOcorrencia.TipoOcorrencia?.Codigo == tipoOcorrencia)
                    tipoParametroCalculoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.HorasExtra;
                else if (valorParametroOcorrencia.ValorParametroEstadiaOcorrencia.TipoOcorrencia?.Codigo == tipoOcorrencia)
                    tipoParametroCalculoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Estadia;
                else if (valorParametroOcorrencia.ValorParametroPernoiteOcorrencia.TipoOcorrencia?.Codigo == tipoOcorrencia)
                    tipoParametroCalculoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Pernoite;
                else
                    tipoParametroCalculoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Escolta;

                return new JsonpResult(new
                {
                    valorParametroOcorrencia.Codigo,
                    TipoParametroCalculoOcorrencia = tipoParametroCalculoOcorrencia,
                    DataSaidaEntrada = registroSaidaVeiculo != null ? registroSaidaVeiculo.DataSaidaEntrada.ToString("dd/MM/yyyy") : string.Empty,
                    HoraSaida = registroSaidaVeiculo != null ? string.Format("{0:00}:{1:00}", registroSaidaVeiculo.HoraSaidaEntrada.Hours, registroSaidaVeiculo.HoraSaidaEntrada.Minutes) : string.Empty,
                    HoraEntrada = registroEntradaVeiculo != null ? string.Format("{0:00}:{1:00}", registroEntradaVeiculo.HoraSaidaEntrada.Hours, registroEntradaVeiculo.HoraSaidaEntrada.Minutes) : string.Empty
                });
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

        #endregion

        #region Métodos Privados

        private dynamic ConsultarOcorrenciasGeradas(int codigo, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado repOcorrenciaParametroOcorrenciaGerado = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado(unitOfWork);

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado> listaGrid = repOcorrenciaParametroOcorrenciaGerado.Consultar(codigo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repOcorrenciaParametroOcorrenciaGerado.ContarConsulta(codigo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Descricao,
                            obj.Valor
                        };

            return lista.ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> CalcularOcorrenciasGeradas(Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia repValorParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ValorParametroOcorrencia(unitOfWork);

            // Parametros
            int kmInicial = Request.GetIntParam("KmInicial");
            int kmFinal = Request.GetIntParam("KmFinal");
            int carga = Request.GetIntParam("Carga");
            int tabelaParametros = Request.GetIntParam("TabelaParametros");

            TimeSpan horaInicial = Request.GetTimeParam("HoraInicial");
            TimeSpan horaFinal = Request.GetTimeParam("HoraFinal");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia tipoParametro = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia>("EnumTipoParametro");

            // Busca informacoes
            Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia valorParametroOcorrencia = repValorParametroOcorrencia.BuscarPorCodigo(tabelaParametros);

            // Valida
            if (valorParametroOcorrencia == null)
                return new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            // Valida Campos Vazios
            if (tipoParametro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.HorasExtra && (horaInicial == horaFinal))
                return new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            if (tipoParametro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Estadia && (horaInicial == horaFinal))
                return new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            if (tipoParametro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia.Escolta && (horaInicial == horaFinal || kmFinal == 0))
                return new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro>();

            List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro> lista = Servicos.Embarcador.CargaOcorrencia.ValorParametroOcorrencia.CalcularOcorrencias(horaInicial, horaFinal, kmInicial, kmFinal, carga, tipoParametro, valorParametroOcorrencia, unitOfWork);

            int i = 0;

            return (from o in lista
                    select new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaPorParametro
                    {
                        Codigo = ++i,
                        ComponenteFrete = o.ComponenteFrete,
                        Minutos = o.Minutos,
                        TipoParametro = o.TipoParametro,
                        Titulo = o.Titulo,
                        Valor = o.Valor
                    }).ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Data").Nome("Data").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Carga").Nome("Carga").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("TipoOcorrencia").Nome("Tipo Ocorrência").Tamanho(20).Align(Models.Grid.Align.left);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaOcorrencias()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(60).Align(Models.Grid.Align.left).Ord(false);
            grid.Prop("Valor").Nome("Valor").Tamanho(20).Align(Models.Grid.Align.right).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9)).Ord(false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia repOcorrenciaParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia(unitOfWork);

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia> listaGrid = repOcorrenciaParametroOcorrencia.Consultar(propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repOcorrenciaParametroOcorrencia.ContarConsulta();

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Numero,
                            Data = obj.Data.ToString("dd/MM/yyyy"),
                            Carga = obj.Carga.CodigoCargaEmbarcador,
                            TipoOcorrencia = obj.TipoOcorrencia.Descricao,
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia repOcorrenciaParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            // Converte valores
            int carga = Request.GetIntParam("Carga");
            int tipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
            int kmInicial = Request.GetIntParam("KmInicial");
            int kmFinal = Request.GetIntParam("KmFinal");

            DateTime data = Request.GetDateTimeParam("Data");
            DateTime dataOcorrencia = Request.GetDateTimeParam("DataParametro");

            TimeSpan horaInicial = Request.GetTimeParam("HoraInicial");
            TimeSpan horaFinal = Request.GetTimeParam("HoraFinal");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia tipoParametro = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia>("EnumTipoParametro");

            string obs = Request.Params("Observacao") ?? string.Empty;
            string observacaoCTe = Request.Params("ObservacaoCTe") ?? string.Empty;

            // Vincula dados
            ocorrencia.Numero = repOcorrenciaParametroOcorrencia.BuscarProximoNumero();
            ocorrencia.Carga = repCarga.BuscarPorCodigo(carga);
            ocorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            ocorrencia.Data = data;
            ocorrencia.Observacao = obs;
            ocorrencia.DataOcorrencia = dataOcorrencia;
            ocorrencia.HoraInicial = horaInicial;
            ocorrencia.HoraFinal = horaFinal;
            ocorrencia.KmInicial = kmInicial;
            ocorrencia.KmFinal = kmFinal;
            ocorrencia.ObservacaoCTe = observacaoCTe;
            ocorrencia.TipoParametro = tipoParametro;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia ocorrencia, out string msgErro)
        {
            msgErro = "";

            if (ocorrencia.Carga == null)
            {
                msgErro = "Carga é obrigatória.";
                return false;
            }

            if (ocorrencia.Data == DateTime.MinValue)
            {
                msgErro = "Data é obrigatória.";
                return false;
            }

            if (ocorrencia.TipoOcorrencia == null)
            {
                msgErro = "Tipo Ocorrência é obrigatória.";
                return false;
            }

            if (ocorrencia.DataOcorrencia == DateTime.MinValue)
            {
                msgErro = "Data do Parâmetro é obrigatória.";
                return false;
            }

            return true;
        }

        private async Task GerarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            if (ocorrencia.TipoOcorrencia == null)
                throw new ServicoException("É obrigatório selecionar o tipo de ocorrência.");

            ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;

            //if (ocorrencia.OrigemOcorrenciaPorPeriodo && ((!ocorrencia.PeriodoInicio.HasValue || !ocorrencia.PeriodoFim.HasValue) || ocorrencia.PeriodoInicio.Value > ocorrencia.PeriodoFim.Value))
            //{
            //    MensagemRetorno = "Período selecionado é inválido.";
            //    return false;
            //}

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            // Vincula as cargas a ocorrencia
            if (ocorrencia.DTNatura != null)
            {
                cargaCTEs = srvOcorrencia.BuscarCTesPorCargaEDTNatura(ocorrencia.Carga, ocorrencia.DTNatura);

                if (cargaCTEs.Count <= 0)
                    throw new ServicoException("Nenhum CT-e encontrado nesta carga com as notas fiscais deste DT.");

            }
            else
            {
                // Vincula as cargas a ocorrencia

                switch (ocorrencia.OrigemOcorrencia)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga:
                        cargaCTEs = await srvOcorrencia.BuscarCTesSelecionadosOuCargas(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes(), ocorrencia.Carga, ConfiguracaoEmbarcador, unitOfWork, "", true, 0, 0, TipoServicoMultisoftware, Usuario, 0);

                        if (cargaCTEs.Any(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                            throw new ServicoException("Não é possível selecionar uma NFS para gerar a ocorrência.");

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo:
                        ocorrencia.Cargas = srvOcorrencia.BuscarCargasDoPeriodoSelecionado(ocorrencia, unitOfWork, TipoServicoMultisoftware, Usuario, 0, 0);

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato:
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro?.CPF_CNPJ_SemFormato ?? string.Empty);
                        int transportador = empresa?.Codigo ?? 0;

                        ocorrencia.ContratoFrete = repContratoFreteTransportador.BuscarContratoAtivo(transportador, ocorrencia.TipoOcorrencia.Codigo, DateTime.Now.Date);
                        ocorrencia.Tomador = ocorrencia.ContratoFrete?.ClienteTomador;

                        if (ocorrencia.TipoOcorrencia.OcorrenciaComVeiculo)
                        {
                            if (ocorrencia.Veiculo == null)
                                throw new ServicoException("Nenhum CT-e encontrado nesta carga com as notas fiscais deste DT.");

                            ocorrencia.Cargas = srvOcorrencia.BuscarCargasDoPeriodoSelecionadoComVeiculo(ocorrencia, unitOfWork, TipoServicoMultisoftware, Usuario, 0);
                        }

                        break;
                }

                if (cargaCTEs.Count > 50)
                    throw new ServicoException("A quantidade máxima de CT-es permitida para a geração da ocorrência é de 50. Selecione menos CT-es ou faça duas ocorrências caso necessário.");
            }

            //Validação de clientes bloqueados para lançamento de ocorrência
            if (ocorrencia.TipoOcorrencia.ClientesBloqueados != null && ocorrencia.TipoOcorrencia.ClientesBloqueados.Count > 0 && cargaCTEs.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados clienteBloqueado in ocorrencia.TipoOcorrencia.ClientesBloqueados)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                    {
                        if (cargaCTe.CTe != null)
                        {
                            if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato)
                                    throw new ServicoException("Cliente origem " + clienteBloqueado.Cliente.CPF_CNPJ_SemFormato + (!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? " (" + clienteBloqueado.Cliente.CodigoIntegracao + ") " : " ") + clienteBloqueado.Cliente.Nome + " não permitido para lançamento da ocorrência.");
                            }
                            else if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato)
                                    throw new ServicoException("Cliente destino " + clienteBloqueado.Cliente.CPF_CNPJ_SemFormato + (!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? " (" + clienteBloqueado.Cliente.CodigoIntegracao + ") " : " ") + clienteBloqueado.Cliente.Nome + " não permitido para lançamento da ocorrência.");
                            }
                        }
                    }

                }
            }

            srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario);

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
            {
                if (!ocorrencia.ComplementoValorFreteCarga)
                {
                    if (cargaCTEs.Count == 0)
                    {
                        throw new ServicoException("Obrigatório selecionar ao menos um CT-e para gerar a ocorrência.");
                    }
                }
                else
                {
                    if (ocorrencia.ValorOcorrencia <= 0m && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        throw new ServicoException("É obrigatório informar um valor para a ocorrência.");
                    }

                    if (ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        throw new ServicoException("A situação da carga não permite que a ocorrência de complemento do valor do frete seja adicionada.");
                    }
                }
            }

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
            {
                if (ocorrencia.Cargas.Count == 0)
                {
                    throw new ServicoException("O período selecionado não possui nenhuma carga com documento.");
                }

                if (ocorrencia.Cargas.FirstOrDefault().Empresa.EmissaoDocumentosForaDoSistema)
                {
                    throw new ServicoException("A emissão por periodo só é permitida para transportadores que emitem no Multiembarcador.");
                }
            }

            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
            {
                if (ocorrencia.ValorICMS <= 0m && ocorrencia.AliquotaICMS <= 0m)
                {
                    throw new ServicoException("É necessário informar um valor ou alíquota de ICMS para a emissão de complemento de ICMS.");
                }

                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    throw new ServicoException("A emissão de complementos de ICMS não é permitida para ocorrências por período.");
                }

                if (cargaCTEs.Count > 1 && ocorrencia.ValorICMS > 0m)
                {
                    throw new ServicoException("Selecione apenas um CT-e para emissão de complemento de ICMS com valor de ICMS. Caso necessário selecionar mais de um, utilize somente a alíquota.");
                }
            }

            if (ocorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    // Validacao de ocorrencia por periodo
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(ocorrencia, out string erro, unitOfWork, this.Usuario))
                    {
                        throw new ServicoException(erro);
                    }
                }
                else
                {
                    // Validacao de ocorrencia por CTe
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, ocorrencia, out string erro, unitOfWork, TipoServicoMultisoftware))
                    {
                        throw new ServicoException(erro);
                    }
                }
            }

            if (!srvOcorrencia.SetaModeloDocumentoFiscal(ref ocorrencia, cargaCTEs, out string erroModeloDocumento, unitOfWork, TipoServicoMultisoftware))
            {
                throw new ServicoException(erroModeloDocumento);
            }

            if (ocorrencia.OrigemOcorrenciaPorPeriodo)
            {
                // Valida emitente
                if (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSe)
                {
                    if (ocorrencia.Emitente == null)
                    {
                        throw new ServicoException("Emitente não selecionado.");
                    }

                    Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(ocorrencia.Emitente.Codigo);
                    if (transportadorConfiguracaoNFSe == null)
                    {
                        throw new ServicoException("Emitente não possui configuração para emitir NFSe.");
                    }
                }
            }

            //-- Persistencia dos dados
            //-------------------------
            await repOcorrencia.InserirAsync(ocorrencia, Auditado);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
            {
                // Dados Sumarizados carga ocorrencia
                srvOcorrencia.SalvarCargasSumarizadas(ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario, 0, 0, "");

                // Atuliza valor da ocorrencia
                ocorrencia.ValorOcorrencia = srvOcorrencia.CalcularValorOcorrenciaPorTipoOcorrencia(ocorrencia);
            }

            // Detalhes da ocorrencia
            string mensagemRetorno = string.Empty;
            if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, Usuario, ConfiguracaoEmbarcador, Cliente, "", true, false, Auditado))
            {
                throw new ServicoException(mensagemRetorno);
            }

            await repOcorrencia.AtualizarAsync(ocorrencia);
        }

        #endregion
    }
}
