using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class ImportacaoTicketLog : LongRunningProcessBase<ImportacaoTicketLog>
    {

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (ObterPermissaoExecucao(unitOfWork))
            {
                try
                {
                    VerificarEProcessarAbastecimentos(unitOfWork, _stringConexao);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    await unitOfWork.DisposeAsync();
                }
            }
        }


        private void VerificarEProcessarAbastecimentos(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            var retorno = new Servicos.AbastecimentoTicketLog(unitOfWork).ObterAbastecimentosTicktLog(unitOfWork);

            if (retorno.Sucesso)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfiguracao = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unitOfWork);
                Repositorio.AbastecimentoTicketLog repoAbastecimentoTicketLog = new Repositorio.AbastecimentoTicketLog(unitOfWork);
                Repositorio.Abastecimento repoAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Cliente repoCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Produto repoProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Veiculo repoVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog configAbastecimentoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog(unitOfWork).Buscar();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.SistemaImportacao,
                    Texto = "Importação Abastecimento TicketLog"
                };

                var listaAbastecimentosTicketLog = new List<Dominio.Entidades.AbastecimentoTicketLog>();

                foreach (var t in retorno.Transacoes)
                {
                    if (t.DataTransacao.ToDateTime() > DateTime.Now)
                        continue;

                    try
                    {
                        if (!repoAbastecimentoTicketLog.VerificarSeJaExisteAbastecimentoImportacaoWS(t.CodigoTransacao))
                        {
                            var abastTicketLog = new Dominio.Entidades.AbastecimentoTicketLog
                            {
                                DataIntegracao = DateTime.Now,
                                CodigoTransacao = t.CodigoTransacao.ToInt(),
                                ValorTransacao = t.ValorTransacao,
                                ValorLitroDecimal = t.ValorLitro.ToDecimal(),
                                DataTransacao = DateTime.Parse(t.DataTransacao),
                                CnpjEstabelecimento = t.CnpjEstabelecimento,
                                Litros = t.Litros,
                                LitrosDecimal = t.Litros.ToDecimal(),
                                TipoCombustivel = t.TipoCombustivel,
                                Placa = t.Placa,
                                ValorLitro = t.ValorLitro,
                                ValorTransacaoDecimal = t.ValorTransacao.ToDecimal(),
                                Quilometragem = t.Quilometragem,
                                QuilometragemInt = t.Quilometragem.ToInt()
                            };

                            repoAbastecimentoTicketLog.Inserir(abastTicketLog, auditado);
                            var abastecimento = new Dominio.Entidades.Abastecimento
                            {
                                AbastecimentoTicketLog = abastTicketLog,
                                Documento = abastTicketLog.CodigoTransacao.ToString(),
                                DataAlteracao = abastTicketLog.DataIntegracao,
                                Data = abastTicketLog.DataTransacao,
                                TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.Integracao,
                                Litros = abastTicketLog.LitrosDecimal,
                                ValorUnitario = abastTicketLog.ValorLitroDecimal,
                                Posto = repoCliente.BuscarPorCPFCNPJ(abastTicketLog.CnpjEstabelecimento.ToDouble()),
                                Veiculo = repoVeiculo.BuscarPorPlaca(0, abastTicketLog.Placa),
                                Produto = repoProduto.BuscarPorCodigoProduto(abastTicketLog.TipoCombustivel)
                            };

                            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                abastecimento.Empresa = new Repositorio.Empresa(unitOfWork).BuscarPorCodigo(_codigoEmpresa);

                            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracaoAbastecimento = repConfiguracao.BuscarPorCodigo(abastecimento.ConfiguracaoAbastecimento?.Codigo ?? 0);
                            abastecimento.ConfiguracaoAbastecimento = configuracaoAbastecimento;
                            abastecimento.TipoMovimentoPagamentoExterno = configAbastecimentoIntegracao?.ConfiguracaoAbastecimentoTicketLog?.TipoMovimento;
                            abastecimento.TipoMovimento = configAbastecimentoIntegracao?.ConfiguracaoAbastecimentoTicketLog?.TipoMovimento;
                            abastecimento.GerarContasAPagarParaAbastecimentoExternos = configAbastecimentoIntegracao?.ConfiguracaoAbastecimentoTicketLog?.GerarContasAPagarParaAbastecimentoExternos ?? false;
                            if (abastecimento.Posto != null)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
                                if (modalidadeFornecedorPessoas != null)
                                {
                                    if (modalidadeFornecedorPessoas.Oficina && modalidadeFornecedorPessoas.TipoOficina.HasValue && modalidadeFornecedorPessoas.TipoOficina.Value == TipoOficina.Interna)
                                    {
                                        abastecimento.TipoMovimentoPagamentoExterno = null;
                                        abastecimento.GerarContasAPagarParaAbastecimentoExternos = false;
                                    }
                                }
                            }

                            if (abastecimento.Produto != null && abastecimento.Posto != null && abastecimento.Data.HasValue && abastecimento.ValorUnitario > 0)
                            {
                                Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
                                decimal valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivel(abastecimento.Produto.Codigo, abastecimento.Posto.CPF_CNPJ, abastecimento.Data.Value);
                                if (valorTabelaFornecedor > 0)
                                    abastecimento.ValorUnitario = valorTabelaFornecedor;
                            }

                            if (abastecimento.Veiculo != null)
                            {
                                if (abastecimento.Veiculo.Motoristas?.Count > 1)
                                    abastecimento.Motorista = abastecimento.Veiculo.Motoristas?.First(x => x.Principal)?.Motorista ?? null;
                                else
                                    abastecimento.Motorista = abastecimento.Veiculo.Motoristas?.First()?.Motorista ?? null;

                                if (abastecimento.Veiculo.TipoVeiculo == "1")
                                {
                                    abastecimento.Equipamento = abastecimento.Veiculo.Equipamentos?.Where(c => c.EquipamentoAceitaAbastecimento == true)?.FirstOrDefault() ?? null;
                                    abastecimento.Horimetro = abastTicketLog.QuilometragemInt;
                                }
                                else
                                    abastecimento.Kilometragem = abastTicketLog.Quilometragem.ToDecimal();

                                if (abastecimento.Equipamento != null && abastecimento.Horimetro <= 0 && abastecimento.Kilometragem > 0)
                                {
                                    abastecimento.Horimetro = (int)abastecimento.Kilometragem;
                                    abastecimento.Kilometragem = 0;
                                }
                            }

                            if (abastecimento.Equipamento != null && abastecimento.Horimetro > 0)
                            {
                                abastecimento.Veiculo = null;
                                abastecimento.Kilometragem = 0;
                            }


                            if (abastTicketLog.TipoCombustivel.ToLower().Contains("arla"))
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                            else
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;

                            if (abastecimento.Veiculo?.Tipo == "T")
                            {
                                abastecimento.Situacao = "I";
                                abastecimento.MotivoInconsistencia += " Veículo de terceiro.";
                            }
                            else
                                abastecimento.Situacao = "A";

                            repoAbastecimento.Inserir(abastecimento, auditado);

                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, configuracaoAbastecimento, configuracaoTMS);                           
                            repoAbastecimento.Atualizar(abastecimento, auditado);
                        };

                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                }
            }
        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TicketLog);
        }


        #region Métodos Privados
        //Verifica se a hora atual está na lista de horas de consulta na configuracao da integracao
        private static bool ObterPermissaoExecucao(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog configuracaoIntegracaoTicketLog = new Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog(unitOfWork).Buscar();

            if (configuracaoIntegracaoTicketLog != null && !string.IsNullOrWhiteSpace(configuracaoIntegracaoTicketLog.HorasConsultaTicketLog))
                foreach (var h in configuracaoIntegracaoTicketLog.HorasConsultaTicketLog.Split(';'))
                    if (int.Parse(h.Split(':')[0]) == DateTime.Now.Hour)
                        return true;

            return false;
        }

        #endregion
    }
}