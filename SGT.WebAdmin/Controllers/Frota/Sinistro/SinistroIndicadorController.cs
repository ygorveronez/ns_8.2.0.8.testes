using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroIndicadorController : BaseController
    {
		#region Construtores

		public SinistroIndicadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> EnviarParaAcompanhamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSinistro = Request.GetIntParam("Sinistro");

                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro = repositorioSinistroDados.BuscarPorCodigo(codigoSinistro, true);

                if (sinistro == null)
                    return new JsonpResult(false, true, "Sinistro não encontrado.");

                if (sinistro.Etapa != EtapaSinistro.IndicacaoPagador)
                    return new JsonpResult(false, true, "Sinistro não está mais na etapa do indicador!");

                if (sinistro.PossuiTitulo)//Validação para quando volta etapa, vai que habilitam o botão e tentam novamente
                    return new JsonpResult(false, true, "Títulos já foram gerados, não sendo possível gerar novamente!");

                if (sinistro.FolhaLancamento != null)
                    return new JsonpResult(false, true, "Folha já foi gerada, não sendo possível gerar novamente!");

                unidadeTrabalho.Start();

                PreencherEtapaIndicador(sinistro, unidadeTrabalho);

                SalvarParcelas(sinistro, unidadeTrabalho);
                SalvarNotas(sinistro, unidadeTrabalho);

                GerarTitulos(sinistro, unidadeTrabalho);
                GerarFolhaLancamento(sinistro, unidadeTrabalho);

                sinistro.Etapa = EtapaSinistro.Acompanhamento;

                repositorioSinistroDados.Atualizar(sinistro, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new { sinistro.Codigo }, true, "Sucesso");
            }
            catch (BaseException ex)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a etapa do indicador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEtapaIndicador(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            IndicadorPagadorSinistro indicadorPagador = Request.GetEnumParam<IndicadorPagadorSinistro>("IndicadorPagador");

            sinistro.IndicadorPagador = indicadorPagador;

            if (indicadorPagador == IndicadorPagadorSinistro.SeguroProprio || indicadorPagador == IndicadorPagadorSinistro.SeguroTerceiro || indicadorPagador == IndicadorPagadorSinistro.EmpresaNota)
                return;

            Repositorio.Embarcador.Financeiro.TipoMovimento repositorioTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.RH.FolhaInformacao repositorioFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unidadeTrabalho);
            Repositorio.Usuario repositorioFuncionario = new Repositorio.Usuario(unidadeTrabalho);

            if (IndicadorGeraTitulo(indicadorPagador))
            {
                int codigoTipoMovimento = Request.GetIntParam("TipoMovimento");

                double cnpjPessoa = Request.GetDoubleParam("Pessoa");

                sinistro.DataEmissaoTitulo = Request.GetDateTimeParam("DataEmissao");
                sinistro.DataVencimentoTitulo = Request.GetDateTimeParam("DataVencimento");
                sinistro.ValorOriginalTitulo = Request.GetDecimalParam("ValorOriginal");
                sinistro.TipoDocumentoTitulo = Request.GetStringParam("TipoDocumento");
                sinistro.NumeroDocumentoTitulo = Request.GetStringParam("NumeroDocumento");
                if (indicadorPagador != IndicadorPagadorSinistro.Terceiro && indicadorPagador != IndicadorPagadorSinistro.SeguroTerceiroReembolso)
                {
                    sinistro.LinhaDigitavelBoleto = Request.GetStringParam("LinhaDigitavel");
                    sinistro.NossoNumeroBoleto = Request.GetStringParam("CodigoDeBarras");
                }
                sinistro.FormaTitulo = Request.GetEnumParam<FormaTitulo>("FormaTitulo");
                sinistro.ObservacaoTitulo = Request.GetStringParam("Observacao");

                sinistro.TipoMovimento = codigoTipoMovimento > 0 ? repositorioTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
                sinistro.PessoaTitulo = cnpjPessoa > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjPessoa) : null;

                if (sinistro.DataEmissaoTitulo > sinistro.DataVencimentoTitulo)
                    throw new ControllerException("A data de emissão não pode ser maior que a data de vencimento.");

                if (sinistro.TipoMovimento == null)
                    throw new ControllerException("Obrigatório informar o Tipo de Movimento.");

                if (sinistro.PessoaTitulo == null)
                    throw new ControllerException("Obrigatório informar a Pessoa.");
            }
            else if (sinistro.IndicadorPagador == IndicadorPagadorSinistro.MotoristaFolha)
            {
                int codigoFuncionario = Request.GetIntParam("Funcionario");
                int codigoFolhaInformacao = Request.GetIntParam("FolhaInformacao");

                sinistro.DescricaoFolhaLancamento = Request.GetStringParam("Descricao");
                sinistro.NumeroEventoFolhaLancamento = Request.GetIntParam("NumeroEvento");
                sinistro.NumeroContratoFolhaLancamento = Request.GetIntParam("NumeroContrato");
                sinistro.DataInicialFolhaLancamento = Request.GetDateTimeParam("DataInicial");
                sinistro.DataFinalFolhaLancamento = Request.GetDateTimeParam("DataFinal");
                sinistro.BaseFolhaLancamento = Request.GetDecimalParam("Base");
                sinistro.ReferenciaFolhaLancamento = Request.GetDecimalParam("Referencia");
                sinistro.ValorFolhaLancamento = Request.GetDecimalParam("Valor");
                sinistro.DataCompetenciaFolhaLancamento = Request.GetNullableDateTimeParam("DataCompetencia");

                sinistro.FolhaInformacao = codigoFolhaInformacao > 0 ? repositorioFolhaInformacao.BuscarPorCodigo(codigoFolhaInformacao) : null;
                sinistro.FuncionarioFolhaLancamento = codigoFuncionario > 0 ? repositorioFuncionario.BuscarPorCodigo(codigoFuncionario) : null;

                if (sinistro.DataCompetenciaFolhaLancamento.HasValue && sinistro.DataCompetenciaFolhaLancamento.Value > DateTime.MinValue && sinistro.DataCompetenciaFolhaLancamento.Value.Date < DateTime.Now.Date)
                    throw new ControllerException("A data de competência não pode ser retroativa.");
            }
        }

        private void SalvarNotas(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (sinistro.IndicadorPagador != IndicadorPagadorSinistro.EmpresaNota)
                return;

            Repositorio.Embarcador.Frota.SinistroNota repositorioSinistroNota = new Repositorio.Embarcador.Frota.SinistroNota(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repositorioDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);

            dynamic dynNotas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Notas"));

            if (dynNotas.Count == 0)
                throw new ControllerException("Nenhuma nota foi selecionada!");

            List<Dominio.Entidades.Embarcador.Frota.SinistroNota> notas = repositorioSinistroNota.BuscarPorFluxoSinistro(sinistro.Codigo);
            if (notas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynNota in dynNotas)
                    if (dynNota.Codigo != null)
                        codigos.Add((int)dynNota.Codigo);

                List<Dominio.Entidades.Embarcador.Frota.SinistroNota> notasDeletar = (from obj in notas where !codigos.Contains(obj.DocumentoEntrada.Codigo) select obj).ToList();

                for (var i = 0; i < notasDeletar.Count; i++)
                    repositorioSinistroNota.Deletar(notasDeletar[i]);
            }

            foreach (var dynNota in dynNotas)
            {
                int codigo = ((string)dynNota.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Frota.SinistroNota sinistroNota = codigo > 0 ? repositorioSinistroNota.BuscarPorFluxoSinistroEDocumentoEntrada(sinistro.Codigo, codigo) : null;

                if (sinistroNota == null)
                {
                    sinistroNota = new Dominio.Entidades.Embarcador.Frota.SinistroNota()
                    {
                        Sinistro = sinistro,
                        DocumentoEntrada = repositorioDocumentoEntrada.BuscarPorCodigo(codigo)
                    };
                    repositorioSinistroNota.Inserir(sinistroNota);
                }
            }
        }

        private void SalvarParcelas(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (!IndicadorGeraTitulo(sinistro.IndicadorPagador.Value))
                return;

            Repositorio.Embarcador.Frota.SinistroParcela repositorioSinistroParcela = new Repositorio.Embarcador.Frota.SinistroParcela(unidadeTrabalho);

            dynamic dynParcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parcelas"));

            List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> parcelas = repositorioSinistroParcela.BuscarPorFluxoSinistro(sinistro.Codigo);
            if (parcelas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynParcela in dynParcelas)
                    if (dynParcela.Codigo != null)
                        codigos.Add((int)dynParcela.Codigo);

                List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> parcelasDeletar = (from obj in parcelas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < parcelasDeletar.Count; i++)
                    repositorioSinistroParcela.Deletar(parcelasDeletar[i]);
            }

            decimal valor = 0m;
            foreach (var dynParcela in dynParcelas)
            {
                int codigo = ((string)dynParcela.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Frota.SinistroParcela sinistroParcela = codigo > 0 ? repositorioSinistroParcela.BuscarPorCodigo(codigo, false) : null;

                if (sinistroParcela == null)
                    sinistroParcela = new Dominio.Entidades.Embarcador.Frota.SinistroParcela();

                sinistroParcela.DataVencimento = ((string)dynParcela.DataVencimento).ToDateTime();
                sinistroParcela.Sequencia = ((string)dynParcela.Parcela).ToInt();
                sinistroParcela.Valor = (decimal)dynParcela.Valor;

                sinistroParcela.DataEmissao = sinistro.DataEmissaoTitulo.Value;
                sinistroParcela.Forma = sinistro.FormaTitulo.Value;
                sinistroParcela.Sinistro = sinistro;

                if (sinistroParcela.Codigo > 0)
                    repositorioSinistroParcela.Atualizar(sinistroParcela);
                else
                    repositorioSinistroParcela.Inserir(sinistroParcela);

                valor += sinistroParcela.Valor;
            }

            if (sinistro.ValorOriginalTitulo != valor)
                throw new ControllerException("A soma das parcelas divergente do Valor Original, favor verificar.");
        }

        private void GerarTitulos(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (!IndicadorGeraTitulo(sinistro.IndicadorPagador.Value))
                return;

            Repositorio.Embarcador.Frota.SinistroParcela repositorioSinistroParcela = new Repositorio.Embarcador.Frota.SinistroParcela(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unidadeTrabalho);

            if (repFechamentoDiario.VerificarSeExistePorDataFechamento(0, sinistro.DataEmissaoTitulo.Value))
                throw new ControllerException("Já existe um fechamento diário igual ou posterior à data " + sinistro.DataEmissaoTitulo.Value.ToDateString() + ", não sendo possível realizar o lançamento do título.");

            List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> parcelas = repositorioSinistroParcela.BuscarPorFluxoSinistro(sinistro.Codigo);
            if (parcelas.Count == 0)
            {
                InserirTitulo(sinistro, sinistro.DataVencimentoTitulo.Value, 1, sinistro.ValorOriginalTitulo, null, unidadeTrabalho);
                return;
            }

            foreach (Dominio.Entidades.Embarcador.Frota.SinistroParcela parcela in parcelas)
                InserirTitulo(sinistro, parcela.DataVencimento, parcela.Sequencia, parcela.Valor, parcela, unidadeTrabalho);
        }

        private void InserirTitulo(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, DateTime dataVencimento, int sequencia, decimal valorOriginal, Dominio.Entidades.Embarcador.Frota.SinistroParcela parcela, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);

            IndicadorPagadorSinistro indicadorPagador = sinistro.IndicadorPagador.Value;

            string observacao = $"Referente ao fluxo de sinistro nº { sinistro.Numero }";
            observacao += !string.IsNullOrWhiteSpace(sinistro.ObservacaoTitulo) ? " - " + sinistro.ObservacaoTitulo + "." : ".";

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.TipoTitulo = indicadorPagador == IndicadorPagadorSinistro.Terceiro || indicadorPagador == IndicadorPagadorSinistro.SeguroTerceiroReembolso ? TipoTitulo.Receber : TipoTitulo.Pagar;
            titulo.DataEmissao = sinistro.DataEmissaoTitulo.Value;
            titulo.DataVencimento = dataVencimento;
            titulo.DataProgramacaoPagamento = titulo.DataVencimento;
            titulo.Pessoa = sinistro.PessoaTitulo;
            titulo.GrupoPessoas = sinistro.PessoaTitulo.GrupoPessoas;
            if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.Sequencia = sequencia;
            titulo.ValorOriginal = valorOriginal;
            titulo.ValorPendente = valorOriginal;
            titulo.StatusTitulo = StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = Usuario;
            titulo.Observacao = observacao;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Fluxo de Sinistro";
            titulo.NumeroDocumentoTituloOriginal = sinistro.Numero.ToString();
            titulo.FormaTitulo = sinistro.FormaTitulo.Value;
            titulo.NossoNumero = sinistro.NossoNumeroBoleto;
            titulo.LinhaDigitavelBoleto = sinistro.LinhaDigitavelBoleto;
            titulo.TipoMovimento = sinistro.TipoMovimento;
            titulo.SinistroParcela = parcela;

            if (titulo.TipoTitulo == TipoTitulo.Pagar)
            {
                if (!ConfiguracaoEmbarcador.NaoValidarCodigoBarrasBoletoTituloAPagar && (!string.IsNullOrWhiteSpace(titulo.NossoNumero) || !string.IsNullOrWhiteSpace(titulo.LinhaDigitavelBoleto)) &&
                    Utilidades.String.OnlyNumbers(titulo.NossoNumero).Length != 44)
                    throw new ControllerException("O código de barras do boleto deve ter 44 dígitos.");

                if (!string.IsNullOrWhiteSpace(titulo.NossoNumero))
                {
                    if (repTitulo.ContemTituloNossoNumeroDuplicado(titulo.Codigo, titulo.NossoNumero))
                        throw new ControllerException("Já existe um título a pagar lançado com o mesmo número de boleto para o pagamento eletrônico.");
                }
            }

            repTitulo.Inserir(titulo);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionado pelo Fluxo de Sinistro.", unidadeTrabalho);

            if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.NumeroDocumentoTituloOriginal, titulo.Observacao, unidadeTrabalho, TipoDocumentoMovimento.Outros, TipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.GrupoPessoas, titulo.DataEmissao.Value))
                throw new ControllerException(erro);
        }

        private void GerarFolhaLancamento(Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (sinistro.IndicadorPagador != IndicadorPagadorSinistro.MotoristaFolha)
                return;

            Repositorio.Embarcador.RH.FolhaLancamento repositorioFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento()
            {
                Descricao = sinistro.DescricaoFolhaLancamento,
                NumeroEvento = sinistro.NumeroEventoFolhaLancamento,
                NumeroContrato = sinistro.NumeroContratoFolhaLancamento,
                DataInicial = sinistro.DataInicialFolhaLancamento.Value,
                DataFinal = sinistro.DataFinalFolhaLancamento.Value,
                Base = sinistro.BaseFolhaLancamento,
                Referencia = sinistro.ReferenciaFolhaLancamento,
                Valor = sinistro.ValorFolhaLancamento,
                DataCompetencia = sinistro.DataCompetenciaFolhaLancamento,
                FolhaInformacao = sinistro.FolhaInformacao,
                Funcionario = sinistro.FuncionarioFolhaLancamento
            };

            repositorioFolhaLancamento.Inserir(folhaLancamento);

            sinistro.FolhaLancamento = folhaLancamento;

            Servicos.Embarcador.RH.FolhaLancamento svcFolhaLancamento = new Servicos.Embarcador.RH.FolhaLancamento(unidadeTrabalho);
            if (ConfiguracaoEmbarcador.GerarTituloFolhaPagamento)
                svcFolhaLancamento.GerarTituloFolhaLancamento(folhaLancamento, Usuario, TipoServicoMultisoftware, Empresa.TipoAmbiente);
            else
                svcFolhaLancamento.GerarMovimentoFinanceiroFolhaLancamento(folhaLancamento, TipoServicoMultisoftware);
        }

        private bool IndicadorGeraTitulo(IndicadorPagadorSinistro indicadorPagador)
        {
            return indicadorPagador == IndicadorPagadorSinistro.Empresa || indicadorPagador == IndicadorPagadorSinistro.EmpresaMotorista ||
                indicadorPagador == IndicadorPagadorSinistro.Terceiro || indicadorPagador == IndicadorPagadorSinistro.SeguroTerceiroReembolso;
        }

        #endregion
    }
}
