using Dominio.Excecoes.Embarcador;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.RH
{
    public class FolhaLancamento
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FolhaLancamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> ProcessarArquivoFolha(Stream stream, string fileExtension, out int qtdRegistrosNaoImportado)
        {
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> listaFolhaLancamento = new List<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = null;
            qtdRegistrosNaoImportado = 0;

            if (fileExtension.ToLower() == ".xlsx")
            {
                ExcelPackage package = new ExcelPackage(stream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                var cellValue = "";
                for (var i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (i == 1)//Pula o cabeçalho
                        continue;

                    int posicaoEvento = 1;
                    int posicaoDescricaoEvento = 2;
                    int posicaoCodigoFuncionario = 3;
                    int posicaoValorBase = 9;
                    int posicaoValorReferencia = 10;
                    int posicaoValor = 11;
                    int posicaoDataCompetencia = -1;

                    if (configuracaoTMS.GerarTituloFolhaPagamento)
                    {
                        posicaoDataCompetencia = 2;
                        posicaoEvento = 3;
                        posicaoDescricaoEvento = 4;
                        posicaoCodigoFuncionario = 5;
                        posicaoValorBase = 13;
                        posicaoValorReferencia = 13;
                        posicaoValor = 13;
                    }

                    for (var a = 1; a <= worksheet.Dimension.End.Column; a++)
                    {
                        try
                        {
                            cellValue = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, a].Text).Replace("R$", "");
                        }
                        catch (Exception)
                        {
                            cellValue = "";
                        }

                        if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (folhaLancamento == null)
                                folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento();

                            if (posicaoDataCompetencia > 0 && a == posicaoDataCompetencia)//Data Competencia
                            {
                                folhaLancamento.DataCompetencia = DateTime.Parse(cellValue);
                            }
                            else if (a == posicaoEvento)//Evento
                            {
                                folhaLancamento.NumeroEvento = int.Parse(cellValue);
                                folhaLancamento.FolhaInformacao = repFolhaInformacao.BuscarPorCodigoIntegracao(cellValue);

                                if (folhaLancamento.FolhaInformacao == null)
                                {
                                    qtdRegistrosNaoImportado++;
                                    folhaLancamento = null;
                                    break;
                                }
                            }
                            else if (a == posicaoDescricaoEvento)//Descrição Evento
                                folhaLancamento.Descricao = cellValue;
                            else if (a == posicaoCodigoFuncionario)//Código Funcionário
                            {
                                folhaLancamento.NumeroContrato = int.Parse(cellValue);
                                folhaLancamento.Funcionario = repFuncionario.BuscarPorCodigoIntegracao(cellValue);

                                if (folhaLancamento.Funcionario == null)
                                {
                                    qtdRegistrosNaoImportado++;
                                    folhaLancamento = null;
                                    break;
                                }
                            }
                            if (a == posicaoValorBase)//Valor Base
                                folhaLancamento.Base = decimal.Parse(cellValue);
                            if (a == posicaoValorReferencia)//Valor Referência
                                folhaLancamento.Referencia = decimal.Parse(cellValue);
                            if (a == posicaoValor)//Valor
                            {
                                folhaLancamento.Valor = decimal.Parse(cellValue);

                                if (folhaLancamento.Valor == 0)
                                {
                                    qtdRegistrosNaoImportado++;
                                    folhaLancamento = null;
                                    break;
                                }
                                else
                                {
                                    listaFolhaLancamento.Add(folhaLancamento);
                                    folhaLancamento = null;
                                }
                            }
                        }
                    }
                }
            }
            else if (fileExtension.ToLower() == ".txt")
            {
                StreamReader streamReader = new StreamReader(stream);
                int linha = 0;
                var cellValue = "";

                while ((cellValue = streamReader.ReadLine()) != null)
                {
                    string columnEvento = cellValue.Substring(0, 8).Trim();//Evento  
                    string columnEventoDescricao = cellValue.Substring(8, 52).Trim();//DESCREVENTO                                         
                    string columnCodigoFuncionario = cellValue.Substring(60, 23).Trim();//Contrato do Empregado                      
                    string columnBase = cellValue.Substring(188, 15).Trim();//Base           
                    string columnReferencia = cellValue.Substring(203, 12).Trim();//Referência  
                    string columnValor = cellValue.Substring(215, 10).Trim();//Valor     

                    if (linha == 0)
                    {
                        if (!columnEvento.Equals("Evento") && !columnEventoDescricao.Equals("DESCREVENTO") && !columnCodigoFuncionario.Equals("Contrato do Empregado")
                            && !columnBase.Equals("Base") && !columnReferencia.Equals("Referência") && !columnValor.Equals("Valor"))
                            break;
                    }
                    else
                    {
                        if (folhaLancamento == null)
                            folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento();

                        folhaLancamento.NumeroEvento = columnEvento.ToInt();
                        folhaLancamento.FolhaInformacao = repFolhaInformacao.BuscarPorCodigoIntegracao(columnEvento);
                        folhaLancamento.Descricao = columnEventoDescricao;
                        folhaLancamento.NumeroContrato = columnCodigoFuncionario.ToInt();
                        folhaLancamento.Funcionario = repFuncionario.BuscarPorCodigoIntegracao(columnCodigoFuncionario);
                        folhaLancamento.Base = Math.Round(columnBase.ToDecimal(), 2);
                        folhaLancamento.Referencia = columnReferencia.ToDecimal();
                        folhaLancamento.Valor = columnValor.ToDecimal();

                        if (folhaLancamento.FolhaInformacao == null || folhaLancamento.Funcionario == null || folhaLancamento.Valor == 0)
                        {
                            qtdRegistrosNaoImportado++;
                            folhaLancamento = null;
                        }
                        else
                        {
                            listaFolhaLancamento.Add(folhaLancamento);
                            folhaLancamento = null;
                        }
                    }

                    linha += 1;
                }
            }

            return listaFolhaLancamento;
        }

        public void GerarTituloFolhaLancamento(Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            if (folhaLancamento.DataCompetencia.HasValue && folhaLancamento.Titulo == null && folhaLancamento.Valor > 0 && folhaLancamento.FolhaInformacao != null && folhaLancamento.FolhaInformacao.Justificativa != null && folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa != null)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                DateTime? dataVencimento = folhaLancamento.DataCompetencia;

                Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(folhaLancamento.Funcionario.CPF));
                if (pessoa == null)
                {
                    if (folhaLancamento.Funcionario.Localidade == null)
                        throw new ServicoException("Funcionário está com o endereço incompleto, favor ajustar antes de prosseguir.");

                    pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(folhaLancamento.Funcionario, _unitOfWork);
                    repCliente.Inserir(pessoa);
                }

                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                titulo.DataEmissao = DateTime.Now;
                titulo.DataVencimento = dataVencimento;
                titulo.DataProgramacaoPagamento = dataVencimento;
                titulo.Pessoa = pessoa;
                titulo.GrupoPessoas = pessoa.GrupoPessoas;
                titulo.Sequencia = 1;
                titulo.ValorOriginal = folhaLancamento.Valor;
                titulo.ValorPendente = folhaLancamento.Valor;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                titulo.DataAlteracao = DateTime.Now;
                titulo.Observacao = string.Concat("Referente à Folha do Funcionário ", folhaLancamento.Funcionario.Descricao).Trim();
                titulo.Empresa = folhaLancamento.Funcionario.Empresa;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "Folha Funcionário";
                titulo.NumeroDocumentoTituloOriginal = folhaLancamento.Codigo.ToString();
                titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                titulo.TipoMovimento = folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa;
                titulo.Usuario = usuario;
                titulo.DataLancamento = DateTime.Now;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = tipoAmbiente;

                repTitulo.Inserir(titulo);

                if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, folhaLancamento.Codigo.ToString(), titulo.Observacao, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value))
                    throw new ServicoException(erro);

                folhaLancamento.Titulo = titulo;
                repFolhaLancamento.Atualizar(folhaLancamento);
            }
        }

        public void GerarMovimentoFinanceiroFolhaLancamento(Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(_unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);

            if (folhaLancamento.DataCompetencia.HasValue && !folhaLancamento.GerouMovimentoFinanceiro && folhaLancamento.Valor > 0 && folhaLancamento.FolhaInformacao != null && folhaLancamento.FolhaInformacao.Justificativa != null && folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa != null)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out string erro, folhaLancamento.FolhaInformacao.Justificativa.TipoMovimentoUsoJustificativa, DateTime.Now, folhaLancamento.Valor,
                    folhaLancamento.Codigo.ToString(), string.Concat("Referente à Folha do Funcionário ", folhaLancamento.Funcionario.Descricao).Trim(),
                    _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, 0, null, null, null, DateTime.Now))
                    throw new ServicoException(erro);

                folhaLancamento.GerouMovimentoFinanceiro = true;
                repFolhaLancamento.Atualizar(folhaLancamento);
            }
        }

        #endregion
    }
}
