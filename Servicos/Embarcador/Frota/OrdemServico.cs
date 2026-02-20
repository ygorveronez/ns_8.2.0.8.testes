using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Frota
{
    public class OrdemServico : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota,
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico,
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico
    >
    {
        #region Construtores

        public OrdemServico(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public static Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota AbrirOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

            if (objetoOrdemServico.Veiculo == null && objetoOrdemServico.Equipamento == null && objetoOrdemServico.Pneu == null && objetoOrdemServico.PneuEnvioReforma == null)
                throw new ServicoException("É necessário selecionar um veículo ou um equipamento ou um pneu para gerar a ordem de serviço.");

            if (objetoOrdemServico.Operador == null)
                throw new ServicoException("Usuário não foi informado.");

            if (objetoOrdemServico.TipoOrdemServico != null)
            {
                if (objetoOrdemServico.TipoOrdemServico.ObrigarInformarLocalDeArmazenamentoOS && objetoOrdemServico.LocalManutencao == null)
                    throw new ServicoException("Tipo informado exige que informe o Local de Manutenção.");

                if (objetoOrdemServico.TipoOrdemServico.ObrigarInformarCondicaoPagamento && string.IsNullOrWhiteSpace(objetoOrdemServico.CondicaoPagamento))
                    throw new ServicoException("Tipo informado exige condição de pagamento!");
            }

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota()
            {
                DataProgramada = objetoOrdemServico.DataProgramada,
                LocalManutencao = objetoOrdemServico.LocalManutencao,
                Motorista = objetoOrdemServico.Motorista,
                Numero = repOrdemServico.BuscarUltimoNumero(objetoOrdemServico.Empresa?.Codigo ?? 0) + 1,
                Observacao = objetoOrdemServico.Observacao,
                CondicaoPagamento = objetoOrdemServico.CondicaoPagamento,
                Operador = objetoOrdemServico.Operador,
                Situacao = SituacaoOrdemServicoFrota.EmDigitacao,
                DataAlteracao = objetoOrdemServico.DataManutencao > DateTime.MinValue ? objetoOrdemServico.DataManutencao : DateTime.Now,
                TipoManutencao = TipoManutencaoOrdemServicoFrota.PreventivaECorretiva,
                Veiculo = objetoOrdemServico.Veiculo,
                Equipamento = objetoOrdemServico.Equipamento,
                Horimetro = objetoOrdemServico.Horimetro,
                QuilometragemVeiculo = objetoOrdemServico.QuilometragemVeiculo,
                TipoOrdemServico = objetoOrdemServico.TipoOrdemServico,
                Empresa = objetoOrdemServico.Empresa,
                LancarServicosManualmente = objetoOrdemServico.LancarServicosManualmente,
                GrupoServico = objetoOrdemServico.GrupoServico,
                CentroResultado = objetoOrdemServico.CentroResultado,
                Responsavel = objetoOrdemServico.Responsavel,
                Pneu = objetoOrdemServico.Pneu,
                PneuEnvioReforma = objetoOrdemServico.PneuEnvioReforma,
                DataLimiteExecucao = objetoOrdemServico.DataLimiteExecucao,
                Prioridade = objetoOrdemServico.Prioridade,
                TipoLocalManutencao = objetoOrdemServico.TipoLocalManutencao,
            };

            if (objetoOrdemServico.Veiculo == null && objetoOrdemServico.Equipamento == null)
                ordemServico.Situacao = SituacaoOrdemServicoFrota.EmManutencao;

            if (objetoOrdemServico.Pneu != null)
            {
                Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Pneu pneu = repPneu.BuscarPorCodigo(objetoOrdemServico.Pneu.Codigo);
                pneu.OrdemServicoFrota = ordemServico;
            }

            if (ordemServico.LocalManutencao != null)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(ordemServico.LocalManutencao.CPF_CNPJ);

                if (modalidadeFornecedor != null && modalidadeFornecedor.Oficina && modalidadeFornecedor.TipoOficina.HasValue)
                    ordemServico.TipoOficina = modalidadeFornecedor.TipoOficina;
            }

            repOrdemServico.Inserir(ordemServico, auditado);

            if (objetoOrdemServico.ValorOrcado > 0)
            {
                Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrdemServicoOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento ordemServicoFrotaOrcamento = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento();
                {
                    ordemServicoFrotaOrcamento.OrdemServico = ordemServico;
                    ordemServicoFrotaOrcamento.ValorTotalOrcado = objetoOrdemServico.ValorOrcado;
                    ordemServicoFrotaOrcamento.ValorTotalPreAprovado = 0;
                    ordemServicoFrotaOrcamento.Parcelas = 1;
                }
                repOrdemServicoOrcamento.Inserir(ordemServicoFrotaOrcamento);
            }

            
            if (objetoOrdemServico.Veiculo == null && objetoOrdemServico.Equipamento == null)
            {
                Servicos.Embarcador.Frota.OrdemServicoOrcamento.GerarOrcamentoInicial(ordemServico, null, unitOfWork);

                if (!objetoOrdemServico.TipoOrdemServico?.LancarServicosOSManualmente ?? false)
                    Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarManutencaoDoServicoEspecifico(ordemServico, objetoOrdemServico.ServicoVeiculo, objetoOrdemServico.Custo, unitOfWork);
            }
            else
            {
                if (!(objetoOrdemServico.TipoOrdemServico?.LancarServicosOSManualmente ?? false))
                {
                    if (objetoOrdemServico.CadastrandoVeiculoEquipamento)
                        Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarManutencoesGrupoServico(ordemServico, unitOfWork);
                    else if (objetoOrdemServico.ServicoVeiculo != null)
                        Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarManutencaoDoServicoEspecifico(ordemServico, objetoOrdemServico.ServicoVeiculo, objetoOrdemServico.Custo, unitOfWork);
                    else if (objetoOrdemServico.Servicos?.Count > 0)
                        Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarManutencaoDeServicosEspecificos(ordemServico, objetoOrdemServico.Servicos, unitOfWork);
                    else
                        Servicos.Embarcador.Frota.OrdemServicoManutencao.GerarManutencoesVeiculo(ordemServico, unitOfWork);

                    if (objetoOrdemServico.Veiculo != null)
                        Servicos.Embarcador.Frota.OrdemServicoManutencao.IniciarManutencaoVeiculo(objetoOrdemServico.Veiculo.Codigo, ordemServico, unitOfWork, auditado);
                }
            }

            SalvarLogAlteracao(ordemServico, objetoOrdemServico.Operador, unitOfWork);

            return ordemServico;
        }

        public static object ObterDetalhesOrdemServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico)
        {
            return new
            {
                ordemServico.Codigo,
                DataProgramada = ordemServico.DataProgramada.ToString("dd/MM/yyyy HH:mm"),
                ordemServico.DescricaoSituacao,
                ordemServico.DescricaoTipoManutencao,
                LocalManutencao = new
                {
                    Codigo = ordemServico.LocalManutencao?.CPF_CNPJ ?? 0,
                    Descricao = ordemServico.LocalManutencao != null ? ordemServico.LocalManutencao.Nome + " (" + ordemServico.LocalManutencao.Localidade.DescricaoCidadeEstado + ")" : string.Empty
                },
                Motorista = new
                {
                    Codigo = ordemServico.Motorista?.Codigo,
                    Descricao = ordemServico.Motorista?.Nome
                },
                ordemServico.Numero,
                ordemServico.Observacao,
                ordemServico.CondicaoPagamento,
                Operador = new
                {
                    ordemServico.Operador.Codigo,
                    Descricao = ordemServico.Operador.Nome
                },
                ordemServico.QuilometragemVeiculo,
                ordemServico.Horimetro,
                ordemServico.Situacao,
                ordemServico.SituacaoAnteriorCancelamento,
                ordemServico.TipoManutencao,
                Veiculo = new
                {
                    Codigo = ordemServico.Veiculo?.Codigo ?? 0,
                    Descricao = ordemServico.Veiculo?.DescricaoComMarcaModelo ?? string.Empty
                },
                Equipamento = new
                {
                    Codigo = ordemServico.Equipamento?.Codigo ?? 0,
                    Descricao = ordemServico.Equipamento?.DescricaoComMarcaModelo ?? string.Empty
                },
                ordemServico.Motivo,
                Tipo = new
                {
                    Codigo = ordemServico.TipoOrdemServico?.Codigo ?? 0,
                    Descricao = ordemServico.TipoOrdemServico?.Descricao ?? string.Empty
                },
                ordemServico.LancarServicosManualmente,
                GrupoServico = new { Codigo = ordemServico.GrupoServico?.Codigo ?? 0, Descricao = ordemServico.GrupoServico?.Descricao ?? string.Empty },
                CentroResultado = new { Codigo = ordemServico.CentroResultado?.Codigo ?? 0, Descricao = ordemServico.CentroResultado?.Descricao ?? string.Empty },
                Responsavel = new
                {
                    Codigo = ordemServico.Responsavel?.Codigo,
                    Descricao = ordemServico.Responsavel?.Nome
                },
                DataLimiteExecucao = ordemServico.DataLimiteExecucao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                ordemServico.Prioridade,
                TipoLocalManutencao = new
                {
                    Codigo = ordemServico.TipoLocalManutencao?.Codigo ?? 0,
                    Descricao = ordemServico.TipoLocalManutencao?.Descricao ?? string.Empty
                },
            };
        }

        public static object ObterDetalhesFechamentoOrdemServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);

            decimal valorTotalProdutosDocumento = repFechamentoProduto.BuscarValorDocumentoPorOrdemServico(ordemServico.Codigo);
            decimal valorOrcado = ordemServico.Orcamento?.ValorTotalOrcado ?? 0;

            return new
            {
                OrdemServico = ordemServico.Codigo,
                DataFechamento = ordemServico.DataFechamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                DataFechamentoEditavel = ordemServico.DataProgramada.ToString("dd/MM/yyyy HH:mm:ss"),
                Operador = ordemServico.OperadorFechamento?.Nome ?? string.Empty,
                ValorOrcado = valorOrcado.ToString("n2"),
                ValorRealizado = valorTotalProdutosDocumento.ToString("n2"),
                Desconto = ordemServico.Desconto.ToString("n2"),
                ValorTotal = (valorOrcado - ordemServico.Desconto).ToString("n2"),
                ordemServico.TipoOficina,
                DiferencaValorOrcadoRealizado = (valorOrcado - valorTotalProdutosDocumento).ToString("n2")
            };
        }

        public static void AtualizarTipoManutencaoOrdemServico(ref Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota> tiposManutencaoDisponiveis = repServicoOrdemServico.BuscarTipoManutencaoPorOrdemServico(ordemServico.Codigo);

            if (ordemServico.TipoOrdemServico?.OSCorretiva ?? false)
            {
                ordemServico.TipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.Corretiva;
            }
            else
            {
                if (tiposManutencaoDisponiveis.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva) && tiposManutencaoDisponiveis.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota.Preventiva))
                    ordemServico.TipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.PreventivaECorretiva;
                else if (tiposManutencaoDisponiveis.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota.Corretiva))
                    ordemServico.TipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.Corretiva;
                else
                    ordemServico.TipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.Preventiva;
            }

            repOrdemServico.Atualizar(ordemServico);
        }

        public static bool VincularDocumentoEntradaAOrdemServico(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, out string mensagemErro, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento repDocumentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repProdutoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamentoOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico repDocumentoEntradaItemOrdemServico = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico(unidadeTrabalho);

            if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Finalizado)
            {
                mensagemErro = null;
                return true;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServicoItens = (from obj in itensDocumentoEntrada where obj.OrdemServico != null select obj.OrdemServico).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServicoItem in ordensServicoItens)
            {

                if (!repDocumentoOrdemServico.ContemRegistroDuplicado(ordemServicoItem.Codigo, documentoEntrada.Codigo))
                {

                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento documentoVinculado = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento()
                    {
                        DocumentoEntrada = documentoEntrada,
                        OrdemServico = ordemServicoItem
                    };

                    repDocumentoOrdemServico.Inserir(documentoVinculado);
                }

                RefazerProdutosFechamento(ordemServicoItem.Codigo, unidadeTrabalho);

                bool permiteFinalizarOS = true;
                if (configuracao.NaoFinalizarDocumentoEntradaOSValorDivergente)
                {
                    if (ordensServicoItens.Count > 1)//Quando tiver mais que uma OS distinta selecionada nos itens, valida o valor por item em vez do total da nota
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensComAOrdem = itensDocumentoEntrada.Where(o => o.OrdemServico.Codigo == ordemServicoItem.Codigo).ToList();

                        decimal valorOrcado = repOrcamentoOrdemServicoFrota.BuscarValorTotalOrcadoPorOrdemServico(ordemServicoItem.Codigo);
                        decimal valorCustoTotal = itensComAOrdem.Sum(o => o.ValorCustoTotal);
                        if (valorCustoTotal > valorOrcado)
                        {
                            mensagemErro = $"Custo total dos itens vinculados a OS {ordemServicoItem.Numero} é maior que o orçado nela, não sendo permitido finalizar.";
                            return false;
                        }

                        if (valorCustoTotal < valorOrcado)
                            permiteFinalizarOS = false;
                    }
                    else
                    {
                        decimal valorOrcado = repOrcamentoOrdemServicoFrota.BuscarValorTotalOrcadoPorOrdemServico(ordemServicoItem.Codigo);
                        decimal valorTotalNotasDaOS = repDocumentoEntrada.BuscarValorTotalPorOrdemServico(ordemServicoItem.Codigo);

                        if (valorTotalNotasDaOS > valorOrcado)
                        {
                            mensagemErro = $"Valor Total do(s) Documento(s) ({valorTotalNotasDaOS.ToString("n2")}) é maior que o orçado na OS ({valorOrcado.ToString("n2")}), não sendo permitido finalizar.";
                            return false;
                        }

                        if (valorTotalNotasDaOS < valorOrcado)
                        {
                            permiteFinalizarOS = false;
                            documentoEntrada.EncerrarOrdemServico = false;
                        }
                    }
                }

                bool todosProdutosConformeOrcado = repProdutoOrdemServico.TodosProdutosConformeOrcado(ordemServicoItem.Codigo);
                bool encerrarOrdemServico = documentoEntrada.EncerrarOrdemServico.HasValue && documentoEntrada.EncerrarOrdemServico.Value;
                bool encerrarOrdemServicoPorItem = itensDocumentoEntrada.Where(o => o.OrdemServico != null && o.OrdemServico.Codigo == ordemServicoItem.Codigo).Any(o => o.EncerrarOrdemServico);

                if ((todosProdutosConformeOrcado || encerrarOrdemServico || encerrarOrdemServicoPorItem) && permiteFinalizarOS)
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServicoFrota.BuscarPorCodigo(ordemServicoItem.Codigo);
                    if (ordemServico.Situacao == SituacaoOrdemServicoFrota.EmManutencao || ordemServico.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal)
                    {
                        string erro = string.Empty;
                        if (!FinalizarOrdemServico(out erro, ref ordemServico, usuario, unidadeTrabalho, tipoServicoMultisoftware, auditado))
                        {
                            mensagemErro = erro;
                            return false;
                        }
                    }
                }
            }

            //Lote de Ordens de serviços dos itens
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico> itensOSDocumentoEntrada = repDocumentoEntradaItemOrdemServico.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            ordensServicoItens = itensOSDocumentoEntrada.Select(o => o.OrdemServico).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServicoItem in ordensServicoItens)
            {
                if (!repDocumentoOrdemServico.ContemRegistroDuplicado(ordemServicoItem.Codigo, documentoEntrada.Codigo))
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento documentoVinculado = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento()
                    {
                        DocumentoEntrada = documentoEntrada,
                        OrdemServico = ordemServicoItem
                    };

                    repDocumentoOrdemServico.Inserir(documentoVinculado);
                }

                RefazerProdutosFechamento(ordemServicoItem.Codigo, unidadeTrabalho);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServicoFrota.BuscarPorCodigo(ordemServicoItem.Codigo);
                if (ordemServico.Situacao == SituacaoOrdemServicoFrota.EmManutencao || ordemServico.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal)
                {
                    string erro = string.Empty;
                    if (!FinalizarOrdemServico(out erro, ref ordemServico, usuario, unidadeTrabalho, tipoServicoMultisoftware, auditado))
                    {
                        mensagemErro = erro;
                        return false;
                    }
                }
            }

            mensagemErro = null;
            return true;
        }

        public static bool DesvincularDocumentoEntradaDoFechamento(int codigoDocumentoEntrada, out string mensagemErro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento repDocumentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);

            if (documentoEntrada.Situacao == SituacaoDocumentoEntrada.Finalizado)
            {
                mensagemErro = null;
                return true;
            }

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> documentosVinculados = repDocumentoOrdemServico.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento documentoVinculado in documentosVinculados)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = documentoVinculado.OrdemServico;
                repDocumentoOrdemServico.Deletar(documentoVinculado);

                RefazerProdutosFechamento(ordemServico.Codigo, unidadeTrabalho);
            }

            mensagemErro = null;
            return true;
        }

        public static void RefazerProdutosFechamento(int codigoOrdemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repProdutoFechamentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento repDocumentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repProdutoOrcamentoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServico.BuscarPorCodigo(codigoOrdemServico);

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> documentosOrdemServico = repDocumentoOrdemServico.BuscarPorOrdemServico(ordemServico.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentos = repItemDocumentoEntrada.BuscarPorDocumentoEntradaEOrdemServico(documentosOrdemServico.Select(o => o.DocumentoEntrada.Codigo).ToArray(), ordemServico.Codigo);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> itensOrdemServico = repProdutoOrcamentoOrdemServico.BuscarPorOrdemServico(ordemServico.Codigo);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> produtosFechamentoExcluir = repProdutoFechamentoOrdemServico.BuscarPorOrdemServicoEOrigem(ordemServico.Codigo, TipoLancamento.Automatico);

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto produtoExcluir in produtosFechamentoExcluir)
                repProdutoFechamentoOrdemServico.Deletar(produtoExcluir);

            IEnumerable<Dominio.Entidades.Produto> produtosFechamento = itensDocumentos.Select(o => o.Produto).Distinct();
            produtosFechamento = produtosFechamento.Union(itensOrdemServico.Select(o => o.Produto).Distinct());

            bool sempreConsiderarValorOrcadoFechamentoOrdemServico = itensDocumentos?.FirstOrDefault()?.DocumentoEntrada?.Fornecedor?.SempreConsiderarValorOrcadoFechamentoOrdemServico ?? false;
            if (!sempreConsiderarValorOrcadoFechamentoOrdemServico && ordemServico != null && ordemServico.LocalManutencao != null && ordemServico.LocalManutencao.SempreConsiderarValorOrcadoFechamentoOrdemServico)
                sempreConsiderarValorOrcadoFechamentoOrdemServico = true;

            foreach (Dominio.Entidades.Produto produto in produtosFechamento)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto produtoFechamento = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto()
                {
                    Autorizado = true,
                    Garantia = false,
                    OrdemServico = ordemServico,
                    Produto = produto,
                    Origem = TipoLancamento.Automatico
                };

                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto> produtoItensOrdemServico = itensOrdemServico.Where(o => o.Produto == produto).ToList();
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> produtoItensDocumentos = itensDocumentos.Where(o => o.Produto == produto).ToList();
                bool naturezaDeGarantia = produtoItensDocumentos.FirstOrDefault()?.NaturezaOperacao?.Garantia ?? false;

                produtoFechamento.QuantidadeDocumento = sempreConsiderarValorOrcadoFechamentoOrdemServico ? produtoItensOrdemServico.Sum(o => o.Quantidade) : produtoItensDocumentos.Sum(o => o.Quantidade);
                produtoFechamento.QuantidadeOrcada = produtoItensOrdemServico.Sum(o => o.Quantidade);
                produtoFechamento.ValorDocumento = naturezaDeGarantia ? 0 : sempreConsiderarValorOrcadoFechamentoOrdemServico ? produtoItensOrdemServico.Sum(o => o.ValorTotal) : produtoItensDocumentos.Sum(o => o.ValorTotalLiquido);
                produtoFechamento.ValorOrcado = produtoItensOrdemServico.Sum(o => o.ValorTotal);
                produtoFechamento.ValorUnitario = naturezaDeGarantia ? 0 : sempreConsiderarValorOrcadoFechamentoOrdemServico ? (produtoItensOrdemServico.Sum(o => o.Valor) / produtoItensOrdemServico.Count) : (produtoItensDocumentos.Sum(o => o.ValorCustoUnitario) / produtoItensDocumentos.Count);

                produtoFechamento.LocalArmazenamento = itensDocumentos.Where(o => o.Produto == produto && o.LocalArmazenamento != null).Select(o => o.LocalArmazenamento).FirstOrDefault();
                if (produtoFechamento.LocalArmazenamento == null)
                    produtoFechamento.LocalArmazenamento = produto.LocalArmazenamentoProduto;

                if (!itensOrdemServico.Any(o => o.Produto == produto))
                    produtoFechamento.Situacao = SituacaoProdutoFechamentoOrdemServicoFrota.NaoOrcado;
                else if (produtoFechamento.ValorDocumento != produtoFechamento.ValorOrcado || produtoFechamento.QuantidadeDocumento != produtoFechamento.QuantidadeOrcada)
                    produtoFechamento.Situacao = SituacaoProdutoFechamentoOrdemServicoFrota.DiferenteOrcado;
                else
                    produtoFechamento.Situacao = SituacaoProdutoFechamentoOrdemServicoFrota.ConformeOrcado;

                repProdutoFechamentoOrdemServico.Inserir(produtoFechamento);
            }
        }

        public static void SalvarLogAlteracao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaLog repLog = new Repositorio.Embarcador.Frota.OrdemServicoFrotaLog(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaLog log = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaLog()
            {
                Data = DateTime.Now,
                OrdemServico = ordemServico,
                Situacao = ordemServico.Situacao,
                Usuario = usuario
            };

            repLog.Inserir(log);
        }

        public static bool FinalizarOrdemServico(out string erro, ref Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, DateTime? dataFechamento = null)
        {
            if (ordemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao && ordemServico.Situacao != SituacaoOrdemServicoFrota.AgNotaFiscal)
            {
                erro = "Para finalizar a ordem de serviço é necessário que ela esteja em manutenção.";
                return false;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Financeiro.ProcessoMovimento();
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaoProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaoProduto.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> produtosAdicionadosManualmente = repFechamentoProduto.BuscarPorOrdemServicoEOrigem(ordemServico.Codigo, TipoLancamento.Manual);

            ordemServico.DataAlteracao = DateTime.Now;
            ordemServico.Situacao = SituacaoOrdemServicoFrota.Finalizada;
            ordemServico.DataFechamento = dataFechamento ?? DateTime.Now;
            ordemServico.OperadorFechamento = usuario;

            repOrdemServico.Atualizar(ordemServico);

            DateTime dataMovimentacao = ordemServico.DataFechamento ?? ordemServico.DataProgramada;
            bool controlarEstoqueNegativo = configuracaoTMS.ControlarEstoqueNegativo || (ordemServico.Empresa?.ControlarEstoqueNegativo ?? false);
            bool naoPermitirRealizarFechamentoOrdemServicoCustoZerado = configuracaoVeiculo?.NaoPermitirRealizarFechamentoOrdemServicoCustoZerado ?? false;

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto produtoAdicionadoManualmente in produtosAdicionadosManualmente)
            {
                if (controlarEstoqueNegativo && !servicoEstoque.ValidarProdutoComEstoque(out erro, produtoAdicionadoManualmente.Produto, produtoAdicionadoManualmente.QuantidadeDocumento, ordemServico.LocalManutencao, ordemServico.Empresa, configuracaoTMS, produtoAdicionadoManualmente.LocalArmazenamento, configuracaoProduto.RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico, dataMovimentacao))
                    return false;

                if (naoPermitirRealizarFechamentoOrdemServicoCustoZerado && (produtoAdicionadoManualmente.ValorUnitario == 0 || produtoAdicionadoManualmente.ValorDocumento == 0))
                {
                    erro = $"Processo Abortado! O produto {produtoAdicionadoManualmente.Produto.Descricao} não possui valor.";
                    return false;
                }

                if (!servicoEstoque.MovimentarEstoque(out erro, produtoAdicionadoManualmente.Produto, produtoAdicionadoManualmente.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Saida, "OS", ordemServico.Numero.ToString(), produtoAdicionadoManualmente.ValorUnitario, ordemServico.Empresa, dataMovimentacao, tipoServicoMultisoftware, ordemServico.LocalManutencao, produtoAdicionadoManualmente.LocalArmazenamento))
                    return false;

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && produtoAdicionadoManualmente.FinalidadeProduto != null && produtoAdicionadoManualmente.FinalidadeProduto.TipoMovimentoUso != null)
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, produtoAdicionadoManualmente.FinalidadeProduto.TipoMovimentoUso, dataMovimentacao, produtoAdicionadoManualmente.ValorDocumento, ordemServico.Numero.ToString(), "Referente à OS " + ordemServico.Numero.ToString() + ", item " + produtoAdicionadoManualmente.Produto.Descricao + ".", unidadeTrabalho, TipoDocumentoMovimento.Outros, tipoServicoMultisoftware))
                        return false;

                if (!servicoEstoque.MovimentarEstoqueReserva(out erro, produtoAdicionadoManualmente.Produto, produtoAdicionadoManualmente.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Saida, ordemServico.Empresa, DateTime.Now, tipoServicoMultisoftware, ordemServico.LocalManutencao, produtoAdicionadoManualmente.LocalArmazenamento))
                    return false;

                if (!AtualizarValorUnitarioProduto(out erro, produtoAdicionadoManualmente, ordemServico.Empresa?.Codigo, unidadeTrabalho))
                    return false;
            }

            if (ordemServico.Veiculo != null)
                Servicos.Embarcador.Frota.OrdemServicoManutencao.FinalizarManutencaoVeiculo(ordemServico.Veiculo.Codigo, ordemServico, unidadeTrabalho, auditado, configuracaoTMS.NaoControlarSituacaoVeiculoOrdemServico);

            if (!configuracaoTMS.MovimentarKMApenasPelaGuarita && !configuracaoVeiculo.NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico)
                Servicos.Embarcador.Frota.OrdemServicoManutencao.AtualizarKMVeiculoPneu(ordemServico, unidadeTrabalho, auditado);

            Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, usuario, unidadeTrabalho);

            erro = null;
            return true;
        }

        public static bool ReabrirOrdemServico(out string erro, ref Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ordemServico.Situacao != SituacaoOrdemServicoFrota.Finalizada)
            {
                erro = "Para reabrir a ordem de serviço é necessário que ela esteja finalizada.";
                return false;
            }

            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Financeiro.ProcessoMovimento();
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> produtosAdicionadosManualmente = repFechamentoProduto.BuscarPorOrdemServicoEOrigem(ordemServico.Codigo, TipoLancamento.Manual);

            DateTime dataReversao = ordemServico.DataFechamento ?? ordemServico.DataProgramada;
            ordemServico.DataAlteracao = DateTime.Now;
            ordemServico.Situacao = SituacaoOrdemServicoFrota.EmManutencao;
            ordemServico.DataFechamento = null;
            ordemServico.OperadorFechamento = null;

            repOrdemServico.Atualizar(ordemServico);

            foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto produtoAdicionadoManualmente in produtosAdicionadosManualmente)
            {
                if (!servicoEstoque.MovimentarEstoque(out erro, produtoAdicionadoManualmente.Produto, produtoAdicionadoManualmente.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Entrada, "OS", ordemServico.Numero.ToString(), produtoAdicionadoManualmente.ValorUnitario, ordemServico.Empresa, DateTime.Now, tipoServicoMultisoftware, ordemServico.LocalManutencao, produtoAdicionadoManualmente.LocalArmazenamento))
                    return false;

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && produtoAdicionadoManualmente.FinalidadeProduto != null && produtoAdicionadoManualmente.FinalidadeProduto.TipoMovimentoReversao != null)
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, produtoAdicionadoManualmente.FinalidadeProduto.TipoMovimentoReversao, dataReversao, produtoAdicionadoManualmente.ValorDocumento, ordemServico.Numero.ToString(), "Referente ao estorno da OS " + ordemServico.Numero.ToString() + ", item " + produtoAdicionadoManualmente.Produto.Descricao + ".", unidadeTrabalho, TipoDocumentoMovimento.Outros, tipoServicoMultisoftware))
                        return false;

                if (!servicoEstoque.MovimentarEstoqueReserva(out erro, produtoAdicionadoManualmente.Produto, produtoAdicionadoManualmente.QuantidadeDocumento, Dominio.Enumeradores.TipoMovimento.Entrada, ordemServico.Empresa, DateTime.Now, tipoServicoMultisoftware, ordemServico.LocalManutencao, produtoAdicionadoManualmente.LocalArmazenamento))
                    return false;
            }

            erro = null;
            return true;
        }

        public static byte[] GerarRelatorioDetalhesOS(int codigoOrdemServico, Repositorio.UnitOfWork unidadeTrabalho)
        {
            return ReportRequest.WithType(ReportType.OrdemServico)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoOrdemServico", codigoOrdemServico.ToString())
                .CallReport()
                .GetContentFile();
        }

        public static void ConfirmarExecucaoServicos(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork, decimal valorProdutos = 0, decimal valorMaoObra = 0, Dominio.Entidades.Produto produtoOrcado = null)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);

            if (ordemServico.Situacao != SituacaoOrdemServicoFrota.EmDigitacao && ordemServico.Situacao != SituacaoOrdemServicoFrota.EmManutencao)
                throw new ServicoException("A situação da ordem de serviço não permite a confirmação da execução dos serviços.");

            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicos = repServicoOrdemServico.BuscarPorOrdemServico(ordemServico.Codigo);

            if (servicos.Count <= 0)
                throw new ServicoException("É necessário adicionar uma manutenção para confirmar a execução.");

            if (servicos.Any(o => !o.Servico.PermiteLancamentoSemValor && o.CustoEstimado <= 0m))
                throw new ServicoException("Há manutenções sem custo estimado, não sendo possível confirmar a execução das mesmas.");

            ordemServico.DataAlteracao = DateTime.Now;
            ordemServico.Situacao = SituacaoOrdemServicoFrota.AgAutorizacao;

            repOrdemServico.Atualizar(ordemServico);

            Servicos.Embarcador.Frota.OrdemServicoOrcamento.GerarOrcamentoInicial(ordemServico, servicos, valorProdutos, valorMaoObra, produtoOrcado, unitOfWork);
        }

        public static void GerarFinalizarOrdemServicoCompleta(Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = AbrirOrdemServico(objetoOrdemServico, auditado, unitOfWork);

            //Método: OrdemServico/ConfirmarExecucaoServicos()
            ConfirmarExecucaoServicos(ordemServico, unitOfWork);

            //Método: OrdemServico/AutorizarOrcamento()
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            ordemServico.Situacao = SituacaoOrdemServicoFrota.EmManutencao;
            AtualizarValorOrcado(ordemServico, objetoOrdemServico.ValorOrcado, unitOfWork);
            repOrdemServico.Atualizar(ordemServico);

            //Método: FechamentoOrdemServico/Finalizar()
            string erro = string.Empty;
            if (!Servicos.Embarcador.Frota.OrdemServico.FinalizarOrdemServico(out erro, ref ordemServico, usuario, unitOfWork, tipoServicoMultisoftware, auditado))
                throw new ServicoException(erro);

            Servicos.Auditoria.Auditoria.Auditar(auditado, ordemServico, null, "Finalizado", unitOfWork);
        }

        public static Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota GerarOrdemServicoAteEtapaDeFechamento(Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, decimal valorProdutos = 0, decimal valorMaoObra = 0, Dominio.Entidades.Produto produtoOrcado = null)
        {
            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = AbrirOrdemServico(objetoOrdemServico, auditado, unitOfWork);

            //Método: OrdemServico/ConfirmarExecucaoServicos()
            ConfirmarExecucaoServicos(ordemServico, unitOfWork, valorProdutos, valorMaoObra, produtoOrcado);

            //Método: OrdemServico/AutorizarOrcamento()
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            ordemServico.Situacao = SituacaoOrdemServicoFrota.EmManutencao;
            repOrdemServico.Atualizar(ordemServico);

            return ordemServico;
        }

        public static object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico repositorioAprovacao = new Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(ordemServico.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(ordemServico.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(ordemServico.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                ordemServico.DescricaoSituacao,
                ordemServico.Situacao,
                ordemServico.Codigo
            };
        }

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool bloquearSemRegraAprovacao)
        {
            List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> regras = ObterRegrasAutorizacao(ordemServico);

            if (regras.Count > 0)
                CriarRegrasAprovacao(ordemServico, regras, tipoServicoMultisoftware);
            else if (bloquearSemRegraAprovacao)
                ordemServico.Situacao = SituacaoOrdemServicoFrota.SemRegraAprovacao;
            else
                ordemServico.Situacao = SituacaoOrdemServicoFrota.EmManutencao;
        }

        public bool EnviarEmailOrdemServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(ordemServico.Empresa?.Codigo ?? 0);

            if (string.IsNullOrEmpty(ordemServico.LocalManutencao?.Email))
                return false;

            if (configuracaoEmail == null)
                return false;

            try
            {
                byte[] relatorio = GerarRelatorioDetalhesOS(ordemServico.Codigo, _unitOfWork);
                List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new MemoryStream(relatorio), $"Ordem de Serviço nº {ordemServico.Numero}.pdf", "application/pdf") };

                string mailDestino = ordemServico.LocalManutencao.Email;
                string veiculo = !string.IsNullOrWhiteSpace(ordemServico.Veiculo?.Placa_Formatada) ? " - Veículo: " + ordemServico.Veiculo.Placa_Formatada : "";
                string equipamento = !string.IsNullOrWhiteSpace(ordemServico.Equipamento?.Descricao) ? " - Equipamento: " + ordemServico.Equipamento.Descricao : "";
                string assunto = "Ordem de Serviço nº " + ordemServico.Numero + veiculo + equipamento;

                System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();
                mensagemEmail.Append("Olá, ").AppendLine().AppendLine();
                mensagemEmail.Append($"Segue em anexo a Ordem de Serviço de número: {ordemServico.Numero}.").AppendLine();

                if (!string.IsNullOrWhiteSpace(ordemServico.Veiculo?.Placa_Formatada))
                    mensagemEmail.Append("Veículo: " + ordemServico.Veiculo.Placa_Formatada).AppendLine();

                if (!string.IsNullOrWhiteSpace(ordemServico.Equipamento?.Descricao))
                    mensagemEmail.Append("Equipamento: " + ordemServico.Equipamento.Descricao).AppendLine();

                mensagemEmail.Append("Data Programada: " + ordemServico.DataProgramada.ToDateTimeString()).AppendLine();

                if (ordemServico.Veiculo.KilometragemAtual > 0)
                    mensagemEmail.Append("Km atual: " + ordemServico.Veiculo.KilometragemAtual.ToString()).AppendLine();

                mensagemEmail.Append("Local de Manutenção: " + ordemServico.LocalManutencao.Nome + "(CNPJ: " + ordemServico.LocalManutencao.CPF_CNPJ_Formatado + ")").AppendLine();

                if (ordemServico.DataLimiteExecucao.HasValue)
                    mensagemEmail.Append("Data limite para execução: " + ordemServico.DataLimiteExecucao.ToDateString()).AppendLine(); // SQL-INJECTION-SAFE

                if (ordemServico.Prioridade.HasValue)
                    mensagemEmail.Append("Prioridade: " + ordemServico.Prioridade.Value.ObterDescricao()).AppendLine();

                if (!string.IsNullOrWhiteSpace(ordemServico.Observacao))
                    mensagemEmail.Append("Observação: " + ordemServico.Observacao).AppendLine();

                mensagemEmail.Append("").AppendLine();
                mensagemEmail.Append("E-mail enviado automaticamente. Por favor, não responda.");

                bool statusEnvioEmail = Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, mailDestino, null, null, assunto, mensagemEmail.ToString(), configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, anexos, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);

                return statusEnvioEmail;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }

        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: ordemServico.Codigo,
                URLPagina: "Frota/AutorizacaoOrdemServico",
                titulo: Localization.Resources.Frotas.OrdemServico.TituloOrdemServico,
                nota: string.Format(Localization.Resources.Frotas.OrdemServico.CriadaSolicitacaoAprovacaoOrdemServico, ordemServico.Numero),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico repositorio = new Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico()
                        {
                            OrigemAprovacao = ordemServico,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = ordemServico.DataCriacao,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(ordemServico, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico()
                    {
                        OrigemAprovacao = ordemServico,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = ordemServico.DataCriacao
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            ordemServico.Situacao = existeRegraSemAprovacao ? SituacaoOrdemServicoFrota.AguardandoAprovacao : SituacaoOrdemServicoFrota.EmManutencao;
        }

        private List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico>(_unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrdemServicoFrotaOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico>();

            decimal valorOrcado = repOrdemServicoFrotaOrcamento.BuscarValorTotalOrcadoPorOrdemServico(ordemServico.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico regra in listaRegras)
            {
                if (regra.RegraPorFornecedor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaFornecedor, Dominio.Entidades.Cliente>(regra.AlcadasFornecedor, ordemServico.LocalManutencao?.CPF_CNPJ))
                    continue;

                if (regra.RegraPorOperador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaOperador, Dominio.Entidades.Usuario>(regra.AlcadasOperador, ordemServico.Operador?.Codigo))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AlcadaValor, decimal>(regra.AlcadasValor, valorOrcado))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private static bool AtualizarValorUnitarioProduto(out string erro, Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto produtoFechamento, int? codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoqueProduto = repProdutoEstoque.BuscarPorProduto(produtoFechamento.Produto.Codigo, codigoEmpresa, produtoFechamento.LocalArmazenamento?.Codigo ?? null);

            if (estoqueProduto != null)
            {
                Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unitOfWork);
                produtoFechamento.ValorUnitario = estoqueProduto.CustoMedio;
                produtoFechamento.ValorDocumento = estoqueProduto.CustoMedio * produtoFechamento.QuantidadeDocumento;
                repFechamentoProduto.Atualizar(produtoFechamento);
            }

            erro = string.Empty;
            return true;
        }

        private static void AtualizarValorOrcado(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, decimal valorOrcado, Repositorio.UnitOfWork unitOfWork)
        {
            if (valorOrcado <= 0)
                return;

            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unitOfWork);
            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento = repOrcamento.BuscarPorOrdemServico(ordemServico.Codigo);

            orcamento.ValorTotalOrcado = valorOrcado;
            repOrcamento.Atualizar(orcamento);
        }

        #endregion
    }
}
