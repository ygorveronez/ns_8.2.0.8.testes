using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/CanhotoMalote")]
    public class CanhotoMaloteController : BaseController
    {
		#region Construtores

		public CanhotoMaloteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> ObterProximoProtocolo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                int protocolo = repMalote.BuscarProximoProtocolo();
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportdor"));
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa;

                return new JsonpResult(new {
                    ProtocoloMalote = protocolo,
                    OrigemMalote = empresa?.Descricao ?? "",
                    DestinoMalote = new { filial.Codigo, filial.Descricao }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter sequência do Protocolo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

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
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = null;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, ref canhotos, unitOfWork);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Malote malote = new Dominio.Entidades.Embarcador.Canhotos.Malote();

                // Preenche entidade com dados
                PreencheEntidade(ref malote, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(malote, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repMalote.Inserir(malote, Auditado);
                if(!SalvarCanhotosSelecionados(malote, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Nenhum Canhoto Selecionado.");
                }
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
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série", "Serie", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data de Emissao", "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Viagem", "NumeroCarga", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Valor NF-e", "Valor", 10, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 18, Models.Grid.Align.left, false);
            }
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Nota", "DataNotaFiscal", 15, Models.Grid.Align.left, false);
            }
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Digitalização", "DescricaoDigitalizacao", 10, Models.Grid.Align.center, true);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Data Baixa", "DataEnvioCanhoto", 10, Models.Grid.Align.center, false);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, ref List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            // Dados do filtro
            int filial = Request.GetIntParam("Filial");
            int empresa = Request.GetIntParam("Transportador");
            int motorista = Request.GetIntParam("Motorista");

            double emitente = Request.GetDoubleParam("Emitente");
            double terceiro = Request.GetDoubleParam("Terceiro");

            string codigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador");
            List<int> codigosCargaEmbarcador = Request.GetListParam<int>("CodigosCargaEmbarcador");
            List<int> codigosNumero = Request.GetListParam<int>("Numeros");
            int numero = Request.GetIntParam("Numero");

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                empresa = this.Empresa.Codigo;

            TipoCanhoto tipoCanhoto = Request.GetEnumParam<TipoCanhoto>("TipoCanhoto");
            SituacaoCanhoto situacaoCanhoto = Request.GetEnumParam<SituacaoCanhoto>("SituacaoCanhoto");
            SituacaoDigitalizacaoCanhoto situacaoDigitalizacaoCanhoto = Request.GetEnumParam<SituacaoDigitalizacaoCanhoto>("SituacaoDigitalizacaoCanhoto");

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro = new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                TipoCanhoto = tipoCanhoto,
                Situacao = situacaoCanhoto,
                SituacaoDigitalizacaoCanhoto = situacaoDigitalizacaoCanhoto,
                CodigoCargaEmbarcador = codigoCargaEmbarcador,
                CodigosCargaEmbarcador = codigosCargaEmbarcador,
                Motorista = motorista,
                Pessoa = emitente,
                Numeros = codigosNumero,
                Numero = numero,
                Filial = filial,
                Empresa = empresa,
                Terceiro = terceiro,
                DataInicio = dataInicio,
                DataFim = dataFim,
                SemMalote = true,
                ObrigatorioFilial = true,
            };

            // Quando canhoto for null, é método da busca normal
            // Se for diferente de null, é método para vincular canhotos ao malote
            if (canhotos != null)
            {
                totalRegistros = 0;
                canhotos = repCanhoto.ConsultarCanhotosMalote(filtro);
                return null;
            }
            
            // Consulta
            totalRegistros = repCanhoto.ContarConsulta(filtro);
            return repCanhoto.ConsultarDynamicMalotes(filtro, propOrdenar, dirOrdena, inicio, limite);
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Canhotos.Malote malote, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);

            // Vincula dados
            if(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                malote.Empresa = this.Empresa; 
            else
                malote.Empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportador")); 
            malote.Protocolo = repMalote.BuscarProximoProtocolo();
            malote.Data = DateTime.Now;
            malote.DataEnvio = Request.GetDateTimeParam("DataMalote");
            malote.Operador = repUsuario.BuscarPorCodigo(Request.GetIntParam("OperadorMalote"));
            malote.NomeOperador = Request.GetStringParam("OperadorManualMalote");
            malote.Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            malote.Destino = repFilial.BuscarPorCodigo(Request.GetIntParam("DestinoMalote"));
            malote.Situacao = SituacaoMaloteCanhoto.Gerado;
            malote.QuantidadeCanhotos = 0;
        }

        private bool SalvarCanhotosSelecionados(Dominio.Entidades.Embarcador.Canhotos.Malote malote, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Malote repMalote = new Repositorio.Embarcador.Canhotos.Malote(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.MaloteCanhoto repMaloteCanhoto = new Repositorio.Embarcador.Canhotos.MaloteCanhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionadas(unitOfWork);

            int count = canhotos.Count;
            if (count == 0)
                return false;

            for(int i = 0; i < count; i++)
            {
                Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto maloteCanhoto = new Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto()
                {
                    Canhoto = canhotos[i],
                    Malote = malote
                };
                repMaloteCanhoto.Inserir(maloteCanhoto);

                maloteCanhoto.Canhoto.Malote = malote;
                repCanhoto.Atualizar(maloteCanhoto.Canhoto);
            }

            malote.QuantidadeCanhotos = count;
            repMalote.Atualizar(malote);

            return true;
        }

        private List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref totalRegistros, "", "", 0, 0, ref canhotos, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("NaoSelecionados"));
                foreach (var dynItem in listaNaoSelecionados)
                    canhotos.Remove(new Dominio.Entidades.Embarcador.Canhotos.Canhoto() { Codigo = (int)dynItem.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Selecionados"));
                foreach (var dynItem in listaSelecionados)
                    canhotos.Add(repCanhoto.BuscarPorCodigo((int)dynItem.Codigo));
            }

            // Retorna lista
            return canhotos;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Canhotos.Malote malote, out string msgErro)
        {
            msgErro = "";

            if (malote.DataEnvio == DateTime.MinValue)
            {
                msgErro = "Data do Envio é obrigatório.";
                return false;
            }

            if (malote.Filial == null)
            {
                msgErro = "Filial é obrigatória.";
                return false;
            }

            if (malote.NomeOperador.Length == 0)
            {
                msgErro = "Operador é obrigatório.";
                return false;
            }

            if (malote.Empresa == null)
            {
                msgErro = "Origem é obrigatório.";
                return false;
            }

            if (malote.Destino == null)
            {
                msgErro = "Destino é obrigatório.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "DescricaoTipoCanhoto")
                propOrdena = "TipoCanhoto";
            if (propOrdena == "DescricaoDigitalizacao")
                propOrdena = "SituacaoDigitalizacaoCanhoto";
            if (propOrdena == "DescricaoSituacao")
                propOrdena = "SituacaoCanhoto";
            else if (propOrdena == "Emitente")
                propOrdena = "XMLNotaFiscal.Emitente.Nome";
            else if (propOrdena == "Empresa")
                propOrdena += ".RazaoSocial";
            else if (propOrdena == "DataNotaFiscal")
                propOrdena = "XMLNotaFiscal.DataEmissao";
        }
        #endregion
    }
}
