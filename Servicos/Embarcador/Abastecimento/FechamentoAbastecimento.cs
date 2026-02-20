using Dominio.Excecoes.Embarcador;
using Repositorio;
using Servicos.Embarcador.Abastecimento.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Abastecimento
{
    public class FechamentoAbastecimento: IFechamentoAbastecimento
    {

        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtor

        public FechamentoAbastecimento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
                                       Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado,
                                       UnitOfWork unitOfWork,
                                       Dominio.Entidades.Usuario usuario = null)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
           
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            _tipoServicoMultisoftware = tipoServicoMultisoftware;

            _auditado = auditado;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Globais

        public bool FecharAbastecimento(Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento, ref string mensagemErro)
        {
            try
            {
                
                // Instacia respositorios
                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(_unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                Servicos.Embarcador.Financeiro.ProcessoMovimento serProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_unitOfWork.StringConexao);
                Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(_unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = repFechamentoAbastecimento.BuscarPorCodigo(parametrosFechamentoAbastecimento.CodigoFechamento, true);

                if (fechamentoAbastecimento == null)
                    throw new ServicoException("Fechamento não encontrado.");

                if (fechamentoAbastecimento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente)
                    throw new ServicoException("A situação do Fechamento não permite essa ação.");

                if (repFechamentoAbastecimento.PossuiQuilometragemZeradaPorFechamento(fechamentoAbastecimento.Codigo))
                    throw new ServicoException("O fechamento possui abastecimentos com quilometragem e horímetro zerados.");

                // Busca os abastecimentos
                int totalAbastecimentos = repFechamentoAbastecimento.ContarConsultarPorFechamento(parametrosFechamentoAbastecimento.CodigoFechamento, "A");

                // Se não houver pedagio, não gera fechamento
                if (totalAbastecimentos == 0)
                    throw new ServicoException("Nenhum abastecimento para gerar fechamento.");

                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repFechamentoAbastecimento.ConsultarPorFechamento(parametrosFechamentoAbastecimento.CodigoFechamento, "A", "Codigo", "", 0, totalAbastecimentos);

                string erro = "";
                int resetUnitOfWork = 0;
                for (int i = 0; i < listaAbastecimentos.Count; i++)
                {
                    // Inicia instancia
                    _unitOfWork.Start();

                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimentos[i].Codigo, true);

                    // Fecha abastecimento
                    abastecimento.Situacao = "F";
                    abastecimento.DataAlteracao = DateTime.Now;
                    abastecimento.Integrado = false;

                    repAbastecimento.Atualizar(abastecimento, _auditado);

                    if (abastecimento.Produto != null && abastecimento.Produto.ProdutoCombustivel.HasValue && abastecimento.Produto.ControlaEstoqueCombustivel.HasValue && abastecimento.Produto.ProdutoCombustivel.Value && abastecimento.Produto.ControlaEstoqueCombustivel.Value)
                    {
                        Dominio.Entidades.Empresa empresa = abastecimento.Empresa;
                        if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && abastecimento.Posto != null)
                            empresa = repEmpresa.BuscarPorCNPJ(abastecimento.Posto.CPF_CNPJ_SemFormato);
                        if (empresa != null && !servicoEstoque.MovimentarEstoque(out erro, abastecimento.Produto, abastecimento.Litros, Dominio.Enumeradores.TipoMovimento.Saida, "ABAST", abastecimento.Codigo.ToString(), abastecimento.ValorUnitario, empresa, abastecimento.Data.Value, _tipoServicoMultisoftware, null, abastecimento.LocalArmazenamento))
                        {
                            _unitOfWork.Rollback();
                            throw new ServicoException(erro);
                        }
                    }

                    if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && abastecimento.TipoMovimento != null)
                    {
                        string tipoMaquina = abastecimento.Veiculo != null ? "veículo" : abastecimento.Equipamento != null ? "equipamento" : "";
                        string descricao = abastecimento.Veiculo?.Placa != null ? abastecimento.Veiculo?.Placa : abastecimento.Equipamento?.Descricao != null ? abastecimento.Equipamento?.Descricao : "";
                        string obsMovimentacao = $"Fechamento de abastecimento do {tipoMaquina} {descricao}";
                        if (!serProcessoMovimento.GerarMovimentacao(out erro, abastecimento.TipoMovimento, abastecimento.Data.Value, abastecimento.ValorTotal, abastecimento.Codigo.ToString(), obsMovimentacao, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, _tipoServicoMultisoftware, 0))
                        {
                            _unitOfWork.Rollback();
                            throw new ServicoException(erro);
                        }
                    }

                    if (abastecimento.GerarContasAPagarParaAbastecimentoExternos && abastecimento.TipoMovimentoPagamentoExterno != null && abastecimento.Posto != null)
                    {
                        if (!GerarTituloPagamentoExterno(abastecimento, _usuario, _unitOfWork))
                        {
                            _unitOfWork.Rollback();
                            throw new ServicoException("Problemas na geração do título a pagar.");
                        }
                    }

                    if (!_configuracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        AtualizarKMVeiculoPneu(abastecimento, _unitOfWork);

                    // Comitta 
                    _unitOfWork.CommitChanges();

                    if (resetUnitOfWork > 20)
                    {
                        _unitOfWork.FlushAndClear();
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                // Finaliza o fechamento               
                _unitOfWork.FlushAndClear();
                _unitOfWork.Start();
                repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(_unitOfWork);
                fechamentoAbastecimento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Finalizado;
                repFechamentoAbastecimento.Atualizar(fechamentoAbastecimento, _auditado);
                _unitOfWork.CommitChanges();

                return true;
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                mensagemErro = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                mensagemErro = ex.Message;
                return false;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public bool GerarFechamentoAbastecimento(Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento, Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento, ref string mensagemErro)
        {
            try
            {
                
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
                Repositorio.Embarcador.Frotas.FechamentoAbastecimentoEmpresa repositorioFechamentoAbastecimentoEmpresa = new Repositorio.Embarcador.Frotas.FechamentoAbastecimentoEmpresa(_unitOfWork);

                //Valida se algum campo está preenchido
                if ((parametrosFechamentoAbastecimento.DataInicio != DateTime.MinValue && parametrosFechamentoAbastecimento.DataFim == DateTime.MinValue) || parametrosFechamentoAbastecimento.DataInicio == DateTime.MinValue && parametrosFechamentoAbastecimento.DataFim != DateTime.MinValue)
                    throw new ServicoException("Favor informar as duas datas para a geração.");
                if (parametrosFechamentoAbastecimento.DataInicio == DateTime.MinValue && parametrosFechamentoAbastecimento.CodigoVeiculo == 0 && parametrosFechamentoAbastecimento.CodigoEquipamento == 0 && parametrosFechamentoAbastecimento.CodigoPosto == 0 && (parametrosFechamentoAbastecimento.CodigosEmpresa == null || parametrosFechamentoAbastecimento.CodigosEmpresa.Count == 0))
                    throw new ServicoException("Favor informar pelo menos um dos campos para a geração.");

                string situacao = "A";

                int codigoEmpresa = 0;

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = _usuario.Empresa.Codigo;

                if (_configuracaoEmbarcador.BloquearFechamentoAbastecimentoSemplaca && parametrosFechamentoAbastecimento.CodigoVeiculo == 0)
                    throw new ServicoException("Favor informe um veículo para gerar o seu fechamento.");

                int countAbastecimento = repAbastecimento.ContarConsultarParaFechamento(parametrosFechamentoAbastecimento.CodigoPosto, parametrosFechamentoAbastecimento.CodigoVeiculo, parametrosFechamentoAbastecimento.DataInicio, parametrosFechamentoAbastecimento.DataFim, situacao, codigoEmpresa, parametrosFechamentoAbastecimento.CodigoEquipamento, parametrosFechamentoAbastecimento.CodigosEmpresa);
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = new List<Dominio.Entidades.Abastecimento>();

                if (countAbastecimento > 0)
                    listaAbastecimentos = repAbastecimento.ConsultarParaFechamento(parametrosFechamentoAbastecimento.CodigoPosto, parametrosFechamentoAbastecimento.CodigoVeiculo, parametrosFechamentoAbastecimento.DataInicio, parametrosFechamentoAbastecimento.DataFim, situacao, codigoEmpresa, parametrosFechamentoAbastecimento.CodigoEquipamento, parametrosFechamentoAbastecimento.CodigosEmpresa, "Codigo", "", 0, countAbastecimento);
                else
                    throw new ServicoException("Não foram encontrados abastecimentos para os filtros selecionados.");

                _unitOfWork.Start();

                fechamentoAbastecimento.Posto = parametrosFechamentoAbastecimento.CodigoPosto > 0 ? repCliente.BuscarPorCPFCNPJ(parametrosFechamentoAbastecimento.CodigoPosto) : null;
                fechamentoAbastecimento.Veiculo = parametrosFechamentoAbastecimento.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(parametrosFechamentoAbastecimento.CodigoVeiculo) : null;
                if (parametrosFechamentoAbastecimento.DataInicio > DateTime.MinValue)
                    fechamentoAbastecimento.DataInicio = parametrosFechamentoAbastecimento.DataInicio;
                if (parametrosFechamentoAbastecimento.DataFim > DateTime.MinValue)
                    fechamentoAbastecimento.DataFim = parametrosFechamentoAbastecimento.DataFim;
                fechamentoAbastecimento.Operador = _usuario;
                fechamentoAbastecimento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAbastecimento.Pendente;
                if (codigoEmpresa > 0)
                    fechamentoAbastecimento.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                fechamentoAbastecimento.Equipamento = parametrosFechamentoAbastecimento.CodigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(parametrosFechamentoAbastecimento.CodigoEquipamento) : null;

                repFechamentoAbastecimento.Inserir(fechamentoAbastecimento, _auditado);

                // Vincula o filtro de empresas ao fechamento
                foreach (int codEmpresa in parametrosFechamentoAbastecimento.CodigosEmpresa)
                {
                    Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa fechamentoAbastecimentoEmpresa = new Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa()
                    {
                        FechamentoAbastecimento = fechamentoAbastecimento,
                        Empresa = repEmpresa.BuscarPorCodigo(codEmpresa)
                    };

                    repositorioFechamentoAbastecimentoEmpresa.Inserir(fechamentoAbastecimentoEmpresa, _auditado);
                }

                // Busca todos abastecimentos para vincular
                // Vincula os abastecimentos ao fechamento
                for (var i = 0; i < listaAbastecimentos.Count(); i++)
                {
                    listaAbastecimentos[i].FechamentoAbastecimento = fechamentoAbastecimento;
                    listaAbastecimentos[i].Integrado = false;

                    repAbastecimento.Atualizar(listaAbastecimentos[i]);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, listaAbastecimentos[i], null, "Gerar Fechamento Abastecimentos", _unitOfWork);
                }

                // Commita
                _unitOfWork.CommitChanges();
                return true;

            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                mensagemErro = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                mensagemErro = ex.Message;
                return false;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private bool GerarTituloPagamentoExterno(Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unitOfWork);

            Dominio.Entidades.Cliente fornecedor = abastecimento.Posto;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fornecedor?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = fornecedor?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (fornecedor != null && fornecedor.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = fornecedor.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = fornecedor.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = fornecedor.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = fornecedor.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (!gerarDuplicataNotaEntrada || parcelasDuplicataNotaEntrada == 0)
            {
                if (GerarTitulo(usuario, 
                                abastecimento, 
                                abastecimento.Posto, 
                                abastecimento.TipoMovimentoPagamentoExterno, 
                                abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, 
                                abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, 
                                abastecimento.ValorTotal, 1, formaTitulo, unitOfWork, unitOfWork.StringConexao))
                    return true;
                else
                    return false;
            }

            int quantidadeParcelas = parcelasDuplicataNotaEntrada;
            decimal valorTotal = abastecimento.ValorTotal;
            decimal valorParcela = Math.Round((valorTotal / quantidadeParcelas), 2);
            decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeParcelas), 2);
            string[] arrayDias = null;

            bool permiteMultiplosVencimentos;
            if (usaGrupoPessoas)
                permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
            else
                permiteMultiplosVencimentos = fornecedor.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

            if (permiteMultiplosVencimentos)
            {
                int diaEmissao = abastecimento.Data.HasValue ? abastecimento.Data.Value.Date.Day : DateTime.Now.Date.Day;
                int diaVencimento = 0;

                if (usaGrupoPessoas)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(fornecedor.CPF_CNPJ, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }

                if (diaVencimento > 0)
                {
                    DateTime novaData = ProximaDataTabelaVencimento(abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, diaVencimento);

                    if (GerarTitulo(usuario, 
                                    abastecimento, 
                                    abastecimento.Posto, 
                                    abastecimento.TipoMovimentoPagamentoExterno, 
                                    abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, 
                                    novaData, abastecimento.ValorTotal, 1, formaTitulo, unitOfWork, unitOfWork.StringConexao))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                var x = intervaloDiasDuplicataNotaEntrada;
                if (x.IndexOf(".") >= 0)
                {
                    arrayDias = x.Split('.');
                    if (arrayDias.Length != quantidadeParcelas)
                    {
                        return false;
                    }
                    for (var i = 0; i < arrayDias.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    arrayDias = new string[1];
                    arrayDias[0] = x;
                    if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                    {
                        return false;
                    }
                }
                var dataVencimento = abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date;

                for (var i = 0; i < quantidadeParcelas; i++)
                {
                    decimal valor = 0;
                    if (i == 0)
                        valor = Math.Round((valorParcela + valorDiferenca), 2);
                    else
                        valor = Math.Round(valorParcela, 2);

                    if (arrayDias.Length > 1)
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                    else
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                    DateTime novaData = dataVencimento;
                    if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                    {
                        try
                        {
                            if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                dataVencimento = dataVencimento.AddMonths(1);

                            novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                        }
                        catch
                        {
                            novaData = dataVencimento;
                        }
                    }
                    dataVencimento = novaData;

                    if (!GerarTitulo(usuario, 
                                     abastecimento, 
                                     abastecimento.Posto, 
                                     abastecimento.TipoMovimentoPagamentoExterno, 
                                     abastecimento.Data.HasValue ? abastecimento.Data.Value.Date : DateTime.Now.Date, 
                                     dataVencimento, abastecimento.ValorTotal, i + 1, formaTitulo, unitOfWork, unitOfWork.StringConexao))
                        return false;
                }
            }
            return true;
        }

        private bool GerarTitulo(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Abastecimento abastecimento, Dominio.Entidades.Cliente fornecedor, Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, DateTime dataEmissao, DateTime dataVencimento, decimal valor, int sequencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

            titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
            titulo.DataEmissao = dataEmissao;
            titulo.DataVencimento = dataVencimento;
            titulo.DataProgramacaoPagamento = dataVencimento;
            titulo.Pessoa = fornecedor;
            titulo.GrupoPessoas = fornecedor.GrupoPessoas;
            if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.Sequencia = sequencia;
            titulo.ValorOriginal = valor;
            titulo.ValorPendente = valor;
            titulo.Desconto = 0;
            titulo.Acrescimo = 0;
            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.Observacao = "Abastecimento Externo";
            titulo.Empresa = abastecimento.Empresa;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Abastecimento";
            titulo.NumeroDocumentoTituloOriginal = !string.IsNullOrEmpty(abastecimento.Documento) ? abastecimento.Documento : Utilidades.String.OnlyNumbers(abastecimento.Codigo.ToString("n0"));
            titulo.FormaTitulo = formaTitulo;
            titulo.NossoNumero = string.Empty;
            titulo.TipoMovimento = tipoMovimento;
            titulo.Provisao = false;
            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = usuario;
            titulo.Abastecimento = abastecimento;

            repTitulo.Inserir(titulo, _auditado);

            if (!svcProcessoMovimento.GerarMovimentacao(out string erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.NumeroDocumentoTituloOriginal, titulo.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, _tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas, titulo.DataEmissao.Value))
                return false;

            return true;
        }

        private DateTime ProximaDataTabelaVencimento(DateTime dataEmissao, int vencimento)
        {
            DateTime novaData;
            DateTime proximoMesAno = dataEmissao.Date;
            int novoDia = vencimento;
            if (proximoMesAno.Day > novoDia)
                proximoMesAno = proximoMesAno.AddMonths(1);
            int diasMes = DateTime.DaysInMonth(proximoMesAno.Year, proximoMesAno.Month);
            if (novoDia > diasMes)
                novoDia = diasMes;

            try
            {
                novaData = new DateTime(proximoMesAno.Year, proximoMesAno.Month, novoDia);
            }
            catch
            {
                novaData = dataEmissao.Date;
            }

            return novaData;
        }

        private void AtualizarKMVeiculoPneu(Dominio.Entidades.Abastecimento abastecimento,  Repositorio.UnitOfWork unitOfWork)
        {
            if ((abastecimento.Kilometragem > 0 || abastecimento.Horimetro > 0) && (abastecimento.Veiculo != null || abastecimento.Equipamento != null))
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                decimal qtdKMRodado = 0;
                Dominio.Entidades.Veiculo veiculo = null;
                //atualiza km do veículo e seus pneus
                if (abastecimento.Kilometragem > 0 && abastecimento.Veiculo != null)
                {
                    veiculo = repVeiculo.BuscarPorCodigo(abastecimento.Veiculo.Codigo);

                    if (veiculo != null && veiculo.KilometragemAtual < abastecimento.Kilometragem)
                    {
                        qtdKMRodado = abastecimento.Kilometragem - (decimal)veiculo.KilometragemAtual;

                        if (veiculo.Pneus != null && veiculo.Pneus.Count > 0 && qtdKMRodado > 0)
                        {
                            foreach (var eixo in veiculo.Pneus)
                            {
                                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                if (pneu != null)
                                {
                                    pneu.KmAnteriorRodado = 0;// pneu.KmAtualRodado;
                                    pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                    if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                        pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                    repPneu.Atualizar(pneu);
                                }
                            }
                        }
                        if (!_configuracaoEmbarcador.MovimentarKMApenasPelaGuarita)
                        {
                            veiculo.KilometragemAnterior = veiculo.KilometragemAtual;
                            veiculo.KilometragemAtual = (int)abastecimento.Kilometragem;
                        }

                        repVeiculo.Atualizar(veiculo, _auditado, null, "Atualizada a Quilometragem Atual do Veículo via Fechamento de Abastecimento");
                    }
                }

                if (abastecimento.Equipamento != null && abastecimento.Horimetro > 0)
                {
                    Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(abastecimento.Equipamento.Codigo);
                    if (equipamento != null)
                    {
                        if (equipamento.TrocaHorimetro)
                        {
                            if (equipamento != null && equipamento.HorimetroAtual < abastecimento.Horimetro)
                            {
                                equipamento.Horimetro = equipamento.Horimetro + ((int)abastecimento.Horimetro - equipamento.HorimetroAtual);
                                equipamento.HorimetroAtual = (int)abastecimento.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                        else
                        {
                            if (equipamento != null && equipamento.Horimetro < abastecimento.Horimetro)
                            {
                                equipamento.Horimetro = (int)abastecimento.Horimetro;
                                repEquipamento.Atualizar(equipamento);
                            }
                        }
                    }
                }

                //atualiza km dos reboques e seus pneus
                if (veiculo != null && veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (var reboque in veiculo.VeiculosVinculados)
                    {
                        if (reboque != null && qtdKMRodado > 0)
                        {
                            if (reboque.Pneus != null && reboque.Pneus.Count > 0 && qtdKMRodado > 0)
                            {
                                foreach (var eixo in reboque.Pneus)
                                {
                                    Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(eixo.Pneu.Codigo);
                                    if (pneu != null)
                                    {
                                        pneu.KmAnteriorRodado = 0;// pneu.KmAtualRodado;
                                        pneu.KmAtualRodado = (int)(pneu.KmAtualRodado + qtdKMRodado);
                                        if (pneu.ValorCustoAtualizado > 0 && pneu.KmAtualRodado > 0)
                                            pneu.ValorCustoKmAtualizado = pneu.ValorCustoAtualizado / pneu.KmAtualRodado;
                                        repPneu.Atualizar(pneu);
                                    }
                                }
                            }
                            if (!_configuracaoEmbarcador.MovimentarKMApenasPelaGuarita && qtdKMRodado > 0)
                            {
                                reboque.KilometragemAnterior = reboque.KilometragemAtual;
                                reboque.KilometragemAtual = reboque.KilometragemAtual + (int)qtdKMRodado;
                            }

                            repVeiculo.Atualizar(reboque, _auditado, null, "Atualizada a Quilometragem Atual do Reboque via Fechamento de Abastecimento");
                        }
                    }
                }
            }
        }

        #endregion
    }
}
