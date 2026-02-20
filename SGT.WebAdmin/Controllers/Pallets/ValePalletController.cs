using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/ValePallet", "Chamados/ChamadoOcorrencia", "GestaoPatio/FluxoPatio")]
    public class ValePalletController : BaseController
    {
        #region Construtores

        public ValePalletController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
                Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet = repValePallet.BuscarPorCodigo(codigo);

                // Valida
                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    valePallet.Codigo,
                    valePallet.Situacao,

                    Lancamento = new
                    {
                        valePallet.Numero,
                        Devolucao = new { valePallet.Devolucao.Codigo, valePallet.Devolucao.Descricao },
                        Representante = valePallet.Representante != null ? new { valePallet.Representante.Codigo, valePallet.Representante.Descricao } : null,
                        Responsavel = new { Codigo = valePallet.Chamado?.Responsavel?.Codigo ?? 0, Descricao = valePallet.Chamado?.Responsavel?.Descricao ?? string.Empty },
                        valePallet.Quantidade,
                    },

                    Recolhimento = new
                    {
                        Data = valePallet.DataRecolhimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        NFe = valePallet.NotaFiscalRecolhimento > 0 ? valePallet.NotaFiscalRecolhimento.ToString() : string.Empty,
                        Transportador = valePallet.Transportador != null ? new { valePallet.Transportador.Codigo, valePallet.Transportador.Descricao } : null,
                        Quantidade = valePallet.QuantidadeRecolhimento > 0 ? valePallet.QuantidadeRecolhimento.ToString() : string.Empty,
                    },

                    Resumo = new
                    {
                        valePallet.Numero,
                        Data = valePallet.DataLancamento.ToString("dd/MM/yyyy"),
                        Filial = valePallet.Devolucao.Filial?.Descricao ?? string.Empty,
                        Situacao = valePallet.DescricaoSituacao,
                        Cliente = valePallet.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao ?? string.Empty,
                        valePallet.Quantidade,
                        Pedido = ObterPedidoCargaCliente(valePallet.Devolucao.CargaPedido?.Carga, valePallet.Devolucao.XMLNotaFiscal?.Destinatario, unitOfWork)
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet = repValePallet.BuscarPorCodigo(codigo);

                // Valida
                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] bytes = null;

                return Arquivo(bytes, "text/xml", "ValePallet" + valePallet.Numero.ToString() + ".pdf");
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet = new Dominio.Entidades.Embarcador.Pallets.ValePallet();

                // Preenche entidade com dados
                PreencheEntidade(ref valePallet, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(valePallet, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repValePallet.Inserir(valePallet, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    valePallet.Codigo
                });
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
            var unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var valePallet = repositorioValePallet.BuscarPorCodigo(codigo, true);

                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (valePallet.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.AgRecolhimento)
                    return new JsonpResult(false, true, "Não é possível finalizar o vale pallet nessa situação.");

                var devolucaoValePallet = ObterDevolucaoValePallet(valePallet.Codigo, unitOfWork);

                if ((devolucaoValePallet == null) || (devolucaoValePallet.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada))
                    return new JsonpResult(false, true, "Não é possível finalizar o vale pallet sem uma devolução de vale pallet aprovada.");

                PreencheEntidadeRecolhimento(ref valePallet, unitOfWork);

                if (!ValidaEntidadeRecolhimento(valePallet, out string erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.Start();

                repositorioValePallet.Atualizar(valePallet, Auditado);

                if (!InserirMovimentacaoEstoquePallet(valePallet, unitOfWork, out string msg))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(true, false, msg);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    valePallet.Codigo
                });
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

        public async Task<IActionResult> Cancelar()
        {
            var unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var valePallet = repositorioValePallet.BuscarPorCodigo(codigo, true);

                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (valePallet.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.Cancelado)
                    return new JsonpResult(false, true, "Não é possível cancelar um vale pallet nessa situação.");

                var devolucaoValePallet = ObterDevolucaoValePallet(valePallet.Codigo, unitOfWork);

                if (devolucaoValePallet != null)
                    return new JsonpResult(false, true, "Não é possível cancelar um vale pallet que contém uma devolução de vale pallet associada.");

                unitOfWork.Start();

                valePallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.Cancelado;
                valePallet.DataCancelamento = DateTime.Now;

                repositorioValePallet.Atualizar(valePallet, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    valePallet.Codigo
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o registro.");
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
                Repositorio.Embarcador.Pallets.ValePallet repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

                int codigoValePallet = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet = repositorioValePallet.BuscarPorCodigo(codigoValePallet);

                if (valePallet == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioValePallet.Deletar(valePallet, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);

            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaValePallet()
            {
                CodigoChamado = Request.GetIntParam("Chamado"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Numero = Request.GetIntParam("Numero"),
                NumeroNfe = Request.GetIntParam("NFe"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet>("Situacao")
            };

            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> listaGrid = repositorioValePallet.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repositorioValePallet.ContarConsulta(filtrosPesquisa);

            var lista = (
                from obj in listaGrid
                select new
                {
                    obj.Codigo,
                    obj.Numero,
                    Cliente = obj.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao,
                    NotaFiscal = obj.Devolucao.XMLNotaFiscal?.Numero.ToString() ?? String.Empty,
                    Filial = obj.Devolucao.Filial?.Descricao ?? string.Empty,
                    Data = obj.DataLancamento.ToString("dd/MM/yyyy"),
                    Quantidade = obj.Quantidade.ToString(),
                    Situacao = obj.DescricaoSituacao,
                }
            );

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("NotaFiscal").Nome("NF-e").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Cliente").Nome("Cliente").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("Data").Nome("Data").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        private bool InserirMovimentacaoEstoquePallet(Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet, UnitOfWork unitOfWork, out string msg)
        {
            msg = "";
            var servicoEstoque = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork);

            if (valePallet.Devolucao.XMLNotaFiscal == null)
            {
                msg = "";
                return false;
            }

            if (valePallet.Devolucao.XMLNotaFiscal?.Destinatario == null)
            {
                msg = "A nota fiscal vinculada a devolução não possui destinatário. Impossível realizar a movimentação de estoque";
                return false;
            }

            var cpfCnpjCliente = valePallet.Devolucao.XMLNotaFiscal?.Destinatario?.CPF_CNPJ ?? 0d;
            var MovimentacaoSaidaCliente = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = valePallet.Devolucao.Filial?.Codigo ?? 0,
                CpfCnpjCliente = cpfCnpjCliente,
                DevolucaoPallet = valePallet.Devolucao,
                Quantidade = valePallet.QuantidadeRecolhimento,
                TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.ClienteFilial,
                CodigoGrupoPessoas = valePallet.Devolucao?.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
            };

            servicoEstoque.InserirMovimentacao(MovimentacaoSaidaCliente);

            return true;
        }

        private Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet ObterDevolucaoValePallet(int codigoValePallet, UnitOfWork unitOfWork)
        {
            var repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);

            return repositorio.BuscarPorValePallet(codigoValePallet);
        }

        private string ObterPedidoCargaCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliente, UnitOfWork unitOfWork)
        {
            if (carga == null)
                return string.Empty;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<string> pedidos = repCargaPedido.BuscarPorCargaEClienteDestinatario(carga.Codigo, cliente?.CPF_CNPJ ?? 0);

            return String.Join(", ", pedidos);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Data")
                return "DataLancamento";

            if (propriedadeOrdenar == "Filial")
                return "Devolucao.Filial.Descricao";

            if (propriedadeOrdenar == "NotaFiscal")
                return "Devolucao.XMLNotaFiscal.Numero";

            if (propriedadeOrdenar == "Cliente")
                return "Devolucao.XMLNotaFiscal.Destinatario.Nome";

            return propriedadeOrdenar;
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);
            Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            valePallet.Devolucao = repositorioDevolucaoPallet.BuscarPorCodigo(Request.GetIntParam("Devolucao"));
            valePallet.Representante = repRepresentante.BuscarPorCodigo(Request.GetIntParam("Representante"));
            valePallet.Chamado = repChamado.BuscarPorCodigo(Request.GetIntParam("Chamado"));
            valePallet.Quantidade = Request.GetIntParam("Quantidade");

            if (valePallet.Codigo == 0)
            {
                valePallet.Numero = repValePallet.BuscarProximoNumero();
                valePallet.DataLancamento = DateTime.Now;
                valePallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.AgDevolucao;
                valePallet.Usuario = this.Usuario;
            }
        }

        private void PreencheEntidadeRecolhimento(ref Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            // Vincula dados
            valePallet.NotaFiscalRecolhimento = Request.GetIntParam("NFe");
            valePallet.Transportador = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Transportador"));
            valePallet.DataRecolhimento = Request.GetNullableDateTimeParam("Data");
            valePallet.QuantidadeRecolhimento = Request.GetIntParam("Quantidade");
            valePallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePallet.Recolhido;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet, out string msgErro)
        {
            msgErro = "";

            if (valePallet.Devolucao == null)
            {
                msgErro = "Devolução é obrigatória.";
                return false;
            }

            if (valePallet.Devolucao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Cancelado)
            {
                msgErro = "A devolução informada está cancelada.";
                return false;
            }

            //if (valePallet.Devolucao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.Entregue)
            //{
            //    msgErro = "A devolução informada está finalizada.";
            //    return false;
            //}

            if (valePallet.Quantidade == 0)
            {
                msgErro = "Quantidade é obrigatória.";
                return false;
            }

            if (valePallet.Devolucao.QuantidadePallets < valePallet.Quantidade)
            {
                msgErro = "Quantidade deve ser menor ou igual a disponível para devolução.";
                return false;
            }

            return true;
        }

        private bool ValidaEntidadeRecolhimento(Dominio.Entidades.Embarcador.Pallets.ValePallet valePallet, out string msgErro)
        {
            msgErro = "";

            if (!valePallet.DataRecolhimento.HasValue)
            {
                msgErro = "Data é obrigatória.";
                return false;
            }

            if (valePallet.QuantidadeRecolhimento == 0)
            {
                msgErro = "Quantidade é obrigatória.";
                return false;
            }

            if (valePallet.QuantidadeRecolhimento > valePallet.Quantidade)
            {
                msgErro = "Quantidade do recolhimento deve ser menor ou igual a quantidade do lançamento.";
                return false;
            }

            return true;
        }

        #endregion
    }
}
