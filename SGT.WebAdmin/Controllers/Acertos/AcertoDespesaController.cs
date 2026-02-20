using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize("Acertos/AcertoViagem")]

    public class AcertoDespesaController : BaseController
    {
        #region Construtores

        public AcertoDespesaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AtualizarDespesas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
            //bool temPermissao = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem);
            
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                if (acertoViagem.Situacao != SituacaoAcertoViagem.EmAntamento)
                    return new JsonpResult(false, "A situação atual do acerto não permite lançamento.");

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                acertoViagem.DespesaSalvo = true;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.OutrasDespesas, this.Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualiar as outras despesas.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]

        public async Task<IActionResult> PesquisarOutrasDespesas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int.TryParse(Request.Params("CodigoAcerto"), out int codigoAcerto);
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigoVeiculo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("CodigoMoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("Data", "DataHora", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Produto / Serviço", "Produto", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CodigoFornecedor", false);
                grid.AdicionarCabecalho("Localidade", "Localidade", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pago Por", "PagoPor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("FaturamentoFornecedor", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("NumeroDocumento", false);
                grid.AdicionarCabecalho("Justificativa", false);
                grid.AdicionarCabecalho("CodigoJustificativa", false);
                grid.AdicionarCabecalho("TipoDespesa", false);
                grid.AdicionarCabecalho("CodigoTipoDespesa", false);
                grid.AdicionarCabecalho("DataEHora", false);
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentral", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("Vlr. Moeda", "ValorMoedaCotacao", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Vlr. Orig. Moeda", "ValorOriginalMoedaEstrangeira", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("DespesaPagaPeloAdiantamento", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DataHora")
                    propOrdenar = "Data";

                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> listaAcertoOutraDespesa = repAcertoOutraDespesa.BuscarPorVeiculoAcerto(codigoAcerto, codigoVeiculo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoOutraDespesa.ContarBuscarPorVeiculoAcerto(codigoAcerto, codigoVeiculo));
                var dynOutraDespesa = (from obj in listaAcertoOutraDespesa
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoVeiculo = obj.Veiculo.Codigo,
                                           CodigoMoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value : MoedaCotacaoBancoCentral.Real,
                                           DataHora = obj.Data.ToString("dd/MM/yyyy"),
                                           Produto = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                                           CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                                           Fornecedor = obj.Pessoa != null ? obj.Pessoa.Nome : obj.NomeFornecedor,
                                           CodigoFornecedor = obj.Pessoa != null ? obj.Pessoa.CPF_CNPJ : 0,
                                           Localidade = obj.Pessoa != null ? obj.Pessoa.Localidade.DescricaoCidadeEstado : string.Empty,
                                           PagoPor = obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista ? "Motorista" : obj.TipoPagamento == TipoPagamentoAcertoDespesa.Empresa ? "Empresa" : obj.Pessoa != null ?
                                                    obj.Pessoa.Modalidades != null && obj.Pessoa.Modalidades.Count > 0 && obj.Pessoa.Modalidades[0].Codigo > 0 ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ) != null ?
                                                    repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ).PagoPorFatura ? "Faturamento" : "Motorista" : "Motorista" : "Motorista" : "Motorista",
                                           Quantidade = obj.Quantidade.ToString("n2"),
                                           Valor = obj.Valor.ToString("n2"),
                                           FaturamentoFornecedor = obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista ? false : obj.Pessoa != null ?
                                                    obj.Pessoa.Modalidades != null && obj.Pessoa.Modalidades.Count > 0 && obj.Pessoa.Modalidades[0].Codigo > 0 ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ) != null ?
                                                    repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ).PagoPorFatura : false : false : false,
                                           obj.Observacao,
                                           obj.NumeroDocumento,
                                           Justificativa = obj.Justificativa != null ? obj.Justificativa.Descricao : string.Empty,
                                           TipoDespesa = obj.TipoDespesa != null ? obj.TipoDespesa.Descricao : string.Empty,
                                           CodigoJustificativa = obj.Justificativa != null ? obj.Justificativa.Codigo : 0,
                                           CodigoTipoDespesa = obj.TipoDespesa != null ? obj.TipoDespesa.Codigo : 0,
                                           DataEHora = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                                           MoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : string.Empty,
                                           DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                           ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                           ValorOriginalMoedaEstrangeira = obj.ValorOriginalMoedaEstrangeira.ToString("n2"),
                                           obj.DespesaPagaPeloAdiantamento
                                       }).ToList();

                grid.AdicionaRows(dynOutraDespesa);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirOutrasDespesasDeNota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirOutrasDespesasDeNota " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoVeiculo, codigoDocumentoEntrada, codigoJustificativa, codigoDespesa, codigoTiposDespesas;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("DocumentoEntrada"), out codigoDocumentoEntrada);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);
                int.TryParse(Request.Params("CodigoDespesa"), out codigoDespesa);
                int.TryParse(Request.Params("TipoDespesa"), out codigoTiposDespesas);

                if (codigoVeiculo == 0)
                    return new JsonpResult(false, "Nenhum veículo encontrado, por favor verifique o lançamento das cargas.");
                if (codigoDocumentoEntrada == 0)
                    return new JsonpResult(false, "Nenhuma nota selecionada.");
                if (codigoAcerto == 0)
                    return new JsonpResult(false, "Nenhum acerto selecionado.");
                if (codigoJustificativa == 0)
                    return new JsonpResult(false, "Nenhuma justificativa selecionada.");

                unitOfWork.Start();
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);
                for (int i = 0; i < itens.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa despesa = new Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa();
                    despesa.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);

                    if (despesa.AcertoViagem.Situacao != SituacaoAcertoViagem.EmAntamento)
                        return new JsonpResult(false, "A situação atual do acerto não permite lançamento.");

                    despesa.Data = itens[i].DocumentoEntrada.DataEmissao;
                    despesa.Observacao = "REF. DOCUMENTO DE ENTRADA " + itens[i].DocumentoEntrada.Numero.ToString();
                    despesa.NumeroDocumento = (int)itens[i].DocumentoEntrada.Numero;
                    despesa.Pessoa = itens[i].DocumentoEntrada.Fornecedor;
                    if (despesa.Pessoa != null)
                    {
                        if (despesa.Pessoa.Nome.Length > 44)
                            despesa.NomeFornecedor = despesa.Pessoa.Nome.Substring(0, 44);
                        else
                            despesa.NomeFornecedor = despesa.Pessoa.Nome;
                    }

                    despesa.Produto = itens[i].Produto;
                    despesa.Quantidade = itens[i].Quantidade;
                    if (despesa.Quantidade <= 0)
                        despesa.Quantidade = 1;
                    despesa.Valor = itens[i].ValorUnitario;
                    despesa.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    despesa.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                    despesa.TipoDespesa = repTipoDespesa.BuscarPorCodigo(codigoTiposDespesas);
                    despesa.DespesaPagaPeloAdiantamento = false;

                    if (ConfiguracaoEmbarcador.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto)
                    {
                        if (despesa.AcertoViagem.DataInicial > despesa.Data)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(true, false, "Não é possível lançar uma despesa fora do período do acerto.");
                        }
                        if (despesa.AcertoViagem.DataFinal.HasValue && despesa.AcertoViagem.DataFinal.Value < despesa.Data)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(true, false, "Não é possível lançar uma despesa fora do período do acerto.");
                        }
                    }

                    repAcertoOutraDespesa.Inserir(despesa, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, despesa.AcertoViagem, null, "Inserido a despesa de nota " + despesa.Descricao + ", veículo " + despesa.Veiculo.Placa, unitOfWork);
                }
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir nova despesa.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirOutrasDespesasDeNota " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InserirOutrasDespesas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoVeiculo, codigoProduto, codigoJustificativa = 0, codigoDespesa = 0, codigoTiposDespesas;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Produto"), out codigoProduto);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);
                int.TryParse(Request.Params("CodigoDespesa"), out codigoDespesa);
                int.TryParse(Request.Params("TipoDespesa"), out codigoTiposDespesas);

                if (codigoVeiculo == 0)
                    return new JsonpResult(false, "Nenhum veículo encontrado, por favor verifique o lançamento das cargas.");

                double cnpjFornecedor = 0;
                double.TryParse(Request.Params("Fornecedor"), out cnpjFornecedor);

                DateTime data;
                DateTime.TryParse(Request.Params("DataDespesa"), out data);

                decimal valor, quantidade = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);
                decimal.TryParse(Request.Params("Quantidade"), out quantidade);

                string numeroDocumento = Request.Params("NumeroDocumento");
                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Acerto.TipoDespesaAcerto repTipoDespesa = new Repositorio.Embarcador.Acerto.TipoDespesaAcerto(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa despesa;
                if (codigoDespesa > 0)
                    despesa = repAcertoOutraDespesa.BuscarPorCodigo(codigoDespesa, true);
                else
                    despesa = new Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa();
                despesa.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);

                if (despesa.AcertoViagem.Situacao != SituacaoAcertoViagem.EmAntamento)
                    return new JsonpResult(false, "A situação atual do acerto não permite lançamento.");

                despesa.Data = data;
                despesa.Observacao = observacao;
                int nDocumento = 0;
                int.TryParse(Utilidades.String.OnlyNumbers(numeroDocumento), out nDocumento);
                despesa.NumeroDocumento = nDocumento;
                despesa.Pessoa = repCliente.BuscarPorCPFCNPJ(cnpjFornecedor);
                if (despesa.Pessoa != null)
                {
                    if (despesa.Pessoa.Nome.Length > 44)
                        despesa.NomeFornecedor = despesa.Pessoa.Nome.Substring(0, 44);
                    else
                        despesa.NomeFornecedor = despesa.Pessoa.Nome;
                }
                if (codigoProduto > 0)
                    despesa.Produto = repProduto.BuscarPorCodigo(0, codigoProduto);
                else
                    despesa.Produto = null;
                despesa.Quantidade = quantidade;
                if (despesa.Quantidade <= 0)
                    despesa.Quantidade = 1;
                despesa.Valor = valor;
                despesa.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                despesa.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);
                despesa.TipoDespesa = repTipoDespesa.BuscarPorCodigo(codigoTiposDespesas);
                despesa.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                despesa.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                despesa.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                despesa.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
                despesa.DespesaPagaPeloAdiantamento = Request.GetBoolParam("DespesaPagaPeloAdiantamento");

                if (ConfiguracaoEmbarcador.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto)
                {
                    if (despesa.AcertoViagem.DataInicial > despesa.Data)
                        return new JsonpResult(true, false, "Não é possível lançar uma despesa fora do período do acerto.");
                    if (despesa.AcertoViagem.DataFinal.HasValue && despesa.AcertoViagem.DataFinal.Value < despesa.Data)
                        return new JsonpResult(true, false, "Não é possível lançar uma despesa fora do período do acerto.");
                }

                unitOfWork.Start();
                if (codigoDespesa > 0)
                    repAcertoOutraDespesa.Atualizar(despesa, Auditado);
                else
                    repAcertoOutraDespesa.Inserir(despesa, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, despesa.AcertoViagem, null, "Inserido a despesa " + despesa.Descricao + ", veículo " + despesa.Veiculo.Placa, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir nova despesa. " + ex.Message);
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RmoverOutrasDespesas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RmoverOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);


                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa despesa = repAcertoOutraDespesa.BuscarPorCodigo(codigo);

                if (despesa.AcertoViagem.Situacao != SituacaoAcertoViagem.EmAntamento)
                    return new JsonpResult(false, "A situação atual do acerto não permite lançamento.");

                unitOfWork.Start();
                Servicos.Auditoria.Auditoria.Auditar(Auditado, despesa.AcertoViagem, null, "Removida a despesa " + despesa.Descricao + ".", unitOfWork);
                repAcertoOutraDespesa.Deletar(despesa, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar a despesa.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RmoverOutrasDespesas " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}


