using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/CompraPallets")]
    public class CompraPalletsController : BaseController
    {
        #region Construtores

        public CompraPalletsController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> ImportacaoXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Valida
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("XML");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                int contagemErrosImportacao = 0;
                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    // Salva na pasta
                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    if (!ProcessaXML(file, unitOfWork))
                        contagemErrosImportacao++;
                }

                return new JsonpResult(new
                {
                    Erros = contagemErrosImportacao
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar importar os XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = repCompraPallets.BuscarPorCodigo(codigo, true);

                // Valida
                if (compraPallets == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (compraPallets.XMLNotaFiscal == null)
                    return new JsonpResult(false, true, "XML não disponível para download.");

                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(compraPallets.XMLNotaFiscal.XML);

                return Arquivo(bytes, "text/xml", compraPallets.XMLNotaFiscal.Chave + ".xml");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
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
                Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = repCompraPallets.BuscarPorCodigo(codigo);

                // Valida
                if (compraPallets == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    compraPallets.Codigo,
                    compraPallets.Numero,
                    Fornecedor = compraPallets.Fornecedor != null ? new { compraPallets.Fornecedor.Codigo, compraPallets.Fornecedor.Descricao } : null,
                    Quantidade = compraPallets.Quantidade.ToString(),
                    Valor = compraPallets.Valor.ToString("n2"),
                    Filial = compraPallets.Filial != null ? new { compraPallets.Filial.Codigo, compraPallets.Filial.Descricao } : null,
                    Setor = compraPallets.Setor != null ? new { compraPallets.Setor.Codigo, compraPallets.Setor.Descricao } : null,
                    Transportador = compraPallets.Transportador != null ? new { compraPallets.Transportador.Codigo, compraPallets.Transportador.Descricao } : null,
                    compraPallets.Situacao,
                    PossuiXML = compraPallets.XMLNotaFiscal != null,
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
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = new Dominio.Entidades.Embarcador.Pallets.CompraPallets();

                // Preenche entidade com dados
                PreencheEntidade(ref compraPallets, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(compraPallets, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCompraPallets.Inserir(compraPallets, Auditado);

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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = repCompraPallets.BuscarPorCodigo(codigo, true);

                // Valida
                if (compraPallets == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref compraPallets, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(compraPallets, out string erro))
                    return new JsonpResult(false, true, erro);

                compraPallets.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets.Finalizado;

                // Persiste dados
                repCompraPallets.Atualizar(compraPallets, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada;

                InserirMovimentacaoEstoque(compraPallets, tipo, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a compra de pallets.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = repCompraPallets.BuscarPorCodigo(codigo, true);

                // Valida
                if (compraPallets == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (compraPallets.Fechamento != null && compraPallets.Fechamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Finalizado)
                    return new JsonpResult(false, true, "Não é possível cancelar a compra de pallet pois o fechamento " + compraPallets.Fechamento.Numero + " já foi finalizado.");

                compraPallets.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets.Cancelado;
                compraPallets.DataCancelamento = DateTime.Now;

                // Persiste dados
                repCompraPallets.Atualizar(compraPallets, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.FilialSaida;

                InserirMovimentacaoEstoque(compraPallets, tipo, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
            grid.Prop("Fornecedor").Nome("Fornecedor").Tamanho(25).Align(Models.Grid.Align.left);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.Prop("Transportador").Nome("Empresa/Filial").Tamanho(25).Align(Models.Grid.Align.left);
            else
            {
                grid.Prop("Filial").Nome("Filial").Tamanho(25).Align(Models.Grid.Align.left);
                grid.Prop("Setor").Nome("Setor").Tamanho(20).Align(Models.Grid.Align.left);
            }

            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("DataFinalizacao").Nome("Data Finalização").Tamanho(15).Align(Models.Grid.Align.center);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);

            // Dados do filtro
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");
            int filial = Request.GetIntParam("Filial");
            int numero = Request.GetIntParam("Numero");
            double fornecedor = Request.GetDoubleParam("Fornecedor");
            int transportador = Request.GetIntParam("Transportador");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets>("Situacao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.CompraPallets> listaGrid = repCompraPallets.Consultar(dataInicio, dataFim, filial, numero, fornecedor, situacao, transportador, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCompraPallets.ContarConsulta(dataInicio, dataFim, filial, numero, fornecedor, situacao, transportador);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Fornecedor = obj.Fornecedor?.Descricao,
                            Filial = obj.Filial?.Descricao,
                            Transportador = obj.Transportador?.Descricao,
                            Setor = obj.Setor?.Descricao,
                            Situacao = obj.SituacaoDescricao,
                            DataFinalizacao = obj.DataFinalizacao?.ToString("dd/MM/yyyy") ?? string.Empty
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            bool isTMS = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

            // Vincula dados
            compraPallets.Fornecedor = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("Fornecedor"));
            compraPallets.Filial = isTMS ? null : repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            compraPallets.Setor = isTMS ? null : repSetor.BuscarPorCodigo(Request.GetIntParam("Setor"));
            compraPallets.Transportador = isTMS ? repositorioTransportador.BuscarPorCodigo(Request.GetIntParam("Transportador")) : null;
            compraPallets.DataFinalizacao = DateTime.Now;

            // Dados Criacao 
            if (compraPallets.Codigo == 0)
            {
                compraPallets.Numero = Request.GetIntParam("Numero");
                compraPallets.Quantidade = Request.GetIntParam("Quantidade");
                compraPallets.Valor = Request.GetDecimalParam("Valor");
                compraPallets.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets.AgFinalizacao;
                compraPallets.DataCriacao = DateTime.Now;
                compraPallets.Usuario = this.Usuario;
            }
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets, out string msgErro)
        {
            msgErro = "";

            if (compraPallets.Fornecedor == null)
            {
                msgErro = "Fornecedor é obrigatório.";
                return false;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (compraPallets.Transportador == null)
                {
                    msgErro = "Empresa/Filial é obrigatória.";
                    return false;
                }
            }
            else
            {
                if (compraPallets.Filial == null)
                {
                    msgErro = "Filial é obrigatória.";
                    return false;
                }
            }

            if (compraPallets.Numero == 0)
            {
                msgErro = "Número é obrigatório.";
                return false;
            }

            if (compraPallets.Quantidade == 0)
            {
                msgErro = "Quantidade é obrigatória.";
                return false;
            }

            if (compraPallets.Valor == 0)
            {
                msgErro = "Valor é obrigatório.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Fornecedor") propOrdenar = "Fornecedor.Nome";
            else if (propOrdenar == "Filial") propOrdenar = "Filial.Descricao";
            else if (propOrdenar == "Transportador") propOrdenar = "Transportador.RazaoSocial";
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "CompraPallets", "ImportacaoXML" });
        }

        private void InserirMovimentacaoEstoque(Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipoOperacaoMovimentacao, UnitOfWork unitOfWork)
        {
            var servicoEstoque = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork);
            var movimentacao = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = compraPallets.Filial?.Codigo ?? 0,
                CodigoSetor = compraPallets.Setor?.Codigo ?? 0,
                CodigoTransportador = compraPallets.Transportador?.Codigo ?? 0,
                Quantidade = compraPallets.Quantidade,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = tipoOperacaoMovimentacao
            };

            servicoEstoque.InserirMovimentacao(movimentacao);
        }

        private bool ProcessaXML(Servicos.DTO.CustomFile file, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pallets.CompraPallets repCompraPallets = new Repositorio.Embarcador.Pallets.CompraPallets(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Servicos.Embarcador.NFe.NFe srvNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            System.IO.StreamReader streamReader = new System.IO.StreamReader(file.InputStream);
            string xml = streamReader.ReadToEnd();

            if (!srvNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, true, TipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                return false;

            if (repCompraPallets.BuscarPorChaveXML(xmlNotaFiscal.Chave) != null)
                return false;

            Dominio.Entidades.Empresa transportador = null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

            if (xmlNotaFiscal.Destinatario != null)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    transportador = repositorioTransportador.BuscarEmpresaPorCNPJ(xmlNotaFiscal.Destinatario.CPF_CNPJ_SemFormato);
                else
                    filial = repFilial.BuscarPorCNPJ(xmlNotaFiscal.Destinatario.CPF_CNPJ_SemFormato);
            }

            Dominio.Entidades.Embarcador.Pallets.CompraPallets compraPallets = new Dominio.Entidades.Embarcador.Pallets.CompraPallets()
            {
                XMLNotaFiscal = xmlNotaFiscal,

                Numero = xmlNotaFiscal.Numero,
                Quantidade = (int)xmlNotaFiscal.QuantidadePallets,
                Valor = xmlNotaFiscal.ValorTotalProdutos / (xmlNotaFiscal.QuantidadePallets > 0 ? xmlNotaFiscal.QuantidadePallets : 1),
                Fornecedor = xmlNotaFiscal.Emitente,
                Filial = filial,
                DataCriacao = DateTime.Now,
                Transportador = transportador,
                Usuario = this.Usuario,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCompraPallets.AgFinalizacao,
            };
            xmlNotaFiscal.DataRecebimento = DateTime.Now;
            repXMLNotaFiscal.Inserir(xmlNotaFiscal, Auditado);
            repCompraPallets.Inserir(compraPallets, Auditado);
            srvNFe.SalvarProdutosNota(xml, xmlNotaFiscal, Auditado, TipoServicoMultisoftware, unitOfWork);
            return true;
        }

        #endregion
    }
}
