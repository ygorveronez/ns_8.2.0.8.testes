using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using Repositorio.Embarcador.Frotas.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrota : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>
    {
        public OrdemServicoFrota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OrdemServicoFrota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> BuscarPorSituacao(List<SituacaoOrdemServicoFrota> situacoesControle, TipoOficina tipoOficina)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            if (situacoesControle?.Count > 0)
                query = query.Where(obj => situacoesControle.Contains(obj.Situacao));

            query = query.Where(obj => obj.TipoOficina == tipoOficina && obj.AlertaEnviado != true);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota BuscarPorNumero(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.Numero == numero);

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ConsultarFinalizadasPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            var result = from obj in query where (obj.Integrado == false || obj.Integrado == null) && (obj.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal || obj.Situacao == SituacaoOrdemServicoFrota.Finalizada) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarFinalizadasPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            var result = from obj in query where (obj.Integrado == false || obj.Integrado == null) && (obj.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal || obj.Situacao == SituacaoOrdemServicoFrota.Finalizada) select obj;

            return result.Count();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, UnitOfWork unitOfWork)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.TipoOficina == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOficina.Interna && o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual);

            return query.Select(o => o.Codigo).ToList();
        }

        public int ContarPorSegmentoVeiculoESituacao(int codigoSegmentoVeiculo, SituacaoOrdemServicoFrota situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.Situacao == situacao);

            if (codigoSegmentoVeiculo > 0)
                query = query.Where(o => o.Veiculo.SegmentoVeiculo.Codigo == codigoSegmentoVeiculo);
            else
                query = query.Where(o => o.Veiculo != null && o.Veiculo.SegmentoVeiculo != null);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> BuscarPorVeiculo(int codigoVeiculo, SituacaoOrdemServicoFrota situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo && o.Situacao == situacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> BuscarPorVeiculos(List<int> codigosVeiculos, List<SituacaoOrdemServicoFrota> situacoesOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => codigosVeiculos.Contains(o.Veiculo.Codigo) && situacoesOS.Contains(o.Situacao));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota BuscarOSAbertaPorPneu(int codigoPneu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(o => o.Pneu.Codigo == codigoPneu && o.PneuEnvioReforma != null && (o.Situacao == SituacaoOrdemServicoFrota.EmManutencao || o.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Fetch(o => o.LocalManutencao)
                        .Fetch(o => o.Veiculo).ThenFetch(o => o.CentroResultado)
                        .Fetch(o => o.Equipamento)
                        .Fetch(o => o.Operador)
                        .Fetch(o => o.Motorista)
                        .Fetch(o => o.Orcamento)
                        .OrderBy(propOrdenar + " " + dirOrdenar)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public bool ContemOrdemEmAndamento(int codigoVeiculo, int codigoEquipamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();
            query = query.Where(o => o.Situacao != SituacaoOrdemServicoFrota.Finalizada && o.Situacao != SituacaoOrdemServicoFrota.Cancelada &&
                                     o.Situacao != SituacaoOrdemServicoFrota.Rejeitada && o.Situacao != SituacaoOrdemServicoFrota.AprovacaoRejeitada);

            if (codigoVeiculo > 0 && codigoEquipamento > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo || o.Equipamento.Codigo == codigoEquipamento);
            else if (codigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo);
            else if (codigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == codigoEquipamento);

            return query.Any();
        }

        public bool ContemProdutoLancado(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(obj => obj.ProdutosFechamento.Any(o => o.OrdemServico.Codigo == codigo));

            return query.Any();
        }

        public bool ContemDocumentoEntrada(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            query = query.Where(obj => obj.DocumentosFechamento.Any(o => o.OrdemServico.Codigo == codigo));

            return query.Any();
        }

        public bool ContemTempoServico(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            query = query.Where(obj => obj.OrdemServico.Codigo == codigo);

            return query.Any();
        }

        public bool ContemOrdemServicoEmManutencaoEmAbertoVeiculo(int codigoIgnorar, int codigoVeiculo, List<SituacaoOrdemServicoFrota> situacoesOS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();            
            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculo && situacoesOS.Contains(o.Situacao) && o.Codigo != codigoIgnorar);

            return query.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServico filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Veiculo.Placa == filtrosPesquisa.Placa);

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial && o.Numero <= filtrosPesquisa.NumeroFinal);
            else if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(o => o.Numero == filtrosPesquisa.NumeroInicial);
            else if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(o => o.Numero == filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoEquipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == filtrosPesquisa.CodigoEquipamento);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataProgramada.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataProgramada.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoMotorista > 0)
                query = query.Where(o => o.Motorista.Codigo == filtrosPesquisa.CodigoMotorista);

            if (filtrosPesquisa.CodigoOperador > 0)
                query = query.Where(o => o.Operador.Codigo == filtrosPesquisa.CodigoOperador);

            if (filtrosPesquisa.Situacao != null && filtrosPesquisa.Situacao.Count > 0)
                query = query.Where(o => filtrosPesquisa.Situacao.Contains(o.Situacao));

            if (filtrosPesquisa.TipoManutencao.HasValue)
                query = query.Where(o => o.TipoManutencao == filtrosPesquisa.TipoManutencao);

            if (filtrosPesquisa.CpfCnpjLocalManutencao > 0d || filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                query = query.Where(o => o.LocalManutencao.CPF_CNPJ == filtrosPesquisa.CpfCnpjLocalManutencao);

            if (filtrosPesquisa.TipoOrdemServico.HasValue)
                query = query.Where(o => o.TipoOficina == filtrosPesquisa.TipoOrdemServico.Value);

            if (filtrosPesquisa.CodigoServico > 0)
            {
                var queryServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();
                query = query.Where(o => queryServico.Any(s => s.Servico.Codigo == filtrosPesquisa.CodigoServico && s.OrdemServico.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.CodigoGrupoServico > 0)
                query = query.Where(o => o.GrupoServico.Codigo == filtrosPesquisa.CodigoGrupoServico);

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                query = query.Where(o => o.CentroResultado.Codigo == filtrosPesquisa.CodigoCentroResultado);

            if (filtrosPesquisa.CodigosEmpresa != null && filtrosPesquisa.CodigosEmpresa.Count > 0)
                query = query.Where(o => o.Veiculo.Empresas.Any(e => filtrosPesquisa.CodigosEmpresa.Contains(e.Codigo)));

            if (filtrosPesquisa.Prioridade.HasValue)
                query = query.Where(o => o.Prioridade == filtrosPesquisa.Prioridade);

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroFogoPneu))
                query = query.Where(o => filtrosPesquisa.NumeroFogoPneu.Contains(o.Pneu.NumeroFogo));

            return query;
        }

        #endregion

        #region Relatório de Manutenção de Veículo

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo> RelatorioManutencaoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string queryVeiculoRealizado = "";
            string queryVeiculo = "";
            string queryServico = "";
            string queryServicosPendentesVeiculo = "";
            string queryServicosPendentesEquipamento = "";
            string queryLocalManutencaoVeiculo = "";
            string queryEquipamentoRealizado = "";
            string queryEquipamento = "";
            string queryLocalManutencaoEquipamento = "";
            string queryValidacao = "";
            string queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            string queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

            if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente && filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO))) AND (ISNULL(OSS.HorimetroUltimaExecucao, 0) > 0)) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0) ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }
            else if (filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO)))) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0)";
            }
            else if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null ";
                queryServicosPendentesEquipamento = @" where OSS.CodigoOrdemServico is not null ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                List<int> codigosVeiculos = new List<int>();
                codigosVeiculos.Add(filtrosPesquisa.CodigoVeiculo);
                if (filtrosPesquisa.CodigosReboques?.Count > 0)
                    codigosVeiculos.AddRange(filtrosPesquisa.CodigosReboques);

                queryVeiculoRealizado = $" AND os.VEI_CODIGO in ({ string.Join(", ", codigosVeiculos) })";
                queryVeiculo = $" AND V.VEI_CODIGO in ({ string.Join(", ", codigosVeiculos) })";

                if (filtrosPesquisa.CodigoEquipamento == 0)
                    queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0 || filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
            {
                List<int> codigosEquipamentos = new List<int>();
                if (filtrosPesquisa.CodigoEquipamento > 0)
                    codigosEquipamentos.Add(filtrosPesquisa.CodigoEquipamento);
                if (filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
                    codigosEquipamentos.AddRange(filtrosPesquisa.CodigosEquipamentosAcoplados);

                queryEquipamentoRealizado = $" AND os.EQP_CODIGO in ({ string.Join(", ", codigosEquipamentos) })";
                queryEquipamento = $" AND V.EQP_CODIGO in ({ string.Join(", ", codigosEquipamentos) })";

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigosServico?.Count > 0)
                queryServico += " and S.SEV_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosServico) + ")";

            if (filtrosPesquisa.TiposManutencao?.Count > 0)
                queryServico += " and S.SEV_TIPO_MANUTENCAO in (" + string.Join(", ", filtrosPesquisa.TiposManutencao.Select(o => o.ToString("D"))) + ")";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PropriedadeVeiculo))
                queryVeiculo += " AND V.VEI_TIPO = '" + filtrosPesquisa.PropriedadeVeiculo + "'";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryVeiculo += " AND V.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa;

            if (filtrosPesquisa.CodigoModeloVeiculo > 0 || filtrosPesquisa.CodigoMarcaVeiculo > 0 || filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
            {
                if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                    queryVeiculo += " AND V.VMO_CODIGO = " + filtrosPesquisa.CodigoModeloVeiculo;

                if (filtrosPesquisa.CodigoMarcaVeiculo > 0)
                    queryVeiculo += " AND V.VMA_CODIGO = " + filtrosPesquisa.CodigoMarcaVeiculo;

                if (filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
                    queryServico += " and V.VSE_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSegmentoVeiculo) + ")";

                queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }
            else if (filtrosPesquisa.CodigoModeloEquipamento > 0 || filtrosPesquisa.CodigoMarcaEquipamento > 0)
            {
                if (filtrosPesquisa.CodigoModeloEquipamento > 0)
                    queryEquipamento += " AND V.EMO_CODIGO = " + filtrosPesquisa.CodigoModeloEquipamento;

                if (filtrosPesquisa.CodigoMarcaEquipamento > 0)
                    queryEquipamento += " AND V.EQM_CODIGO = " + filtrosPesquisa.CodigoMarcaEquipamento;

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CnpjCpfLocalManutencao > 0)
            {
                queryLocalManutencaoVeiculo = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.VEI_CODIGO = V.VEI_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";

                queryLocalManutencaoEquipamento = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.EQP_CODIGO = V.EQP_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";
            }

            if (filtrosPesquisa.CodigoFuncionarioResponsavel > 0)
            {
                queryVeiculo += " AND V.FUN_CODIGO_RESPONSAVEL = " + filtrosPesquisa.CodigoFuncionarioResponsavel;
                queryValidacao += " AND 29 != null";
            }

            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                queryVeiculo += " AND V.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;
                queryValidacao += " AND 30 != null";
            }

            if (filtrosPesquisa.VisualizarSomenteVeiculosAtivos)
            {
                queryVeiculo += " AND V.VEI_ATIVO = 1";
            }



            //SELECT
            var joinGrupoServico = "";
            var selectAgrupamento = @"  S.SEV_VALIDADE_KM, 
                                        S.SEV_TOLERANCIA_KM, 
                                        S.SEV_VALIDADE_DIAS, 
                                        S.SEV_TOLERANCIA_DIAS, 
                                        S.SEV_TIPO,
                                        S.SEV_TOLERANCIA_HORIMETRO,
				                        S.SEV_VALIDADE_HORIMETRO";

            var selectExecucaoUnica = @"S.SEV_VALIDADE_KM ValidadeKM, 
                                        S.SEV_TOLERANCIA_KM ToleranciaKM, 
                                        S.SEV_VALIDADE_DIAS ValidadeDias, 
                                        S.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                                        S.SEV_TIPO TipoServico,
                                        S.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        S.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,";

            string queryGrupoServicoEquipamento = "";
            string queryGrupoServicoVeiculo = "";
            if (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico)
            {
                joinGrupoServico = @"join T_GRUPO_SERVICO_SERVICO_VEICULO GrupoServicoVeiculo on GrupoServicoVeiculo.SEV_CODIGO = S.SEV_CODIGO
                                     join T_GRUPO_SERVICO Grupo on Grupo.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO ";

                queryServico += " AND Grupo.GSF_ATIVO = 1";

                queryGrupoServicoEquipamento = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} >= Grupo.GSF_KM_INICIAL
                                           AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} <= Grupo.GSF_KM_FINAL
                                           ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate())))
				                           AND (
                                            ((V.EMO_CODIGO IN (SELECT MoEq.EMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EMO_CODIGO = v.EMO_CODIGO))
                                               or (Grupo.GSF_POSSUI_MODELOS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
				                            and ((V.EQM_CODIGO in (SELECT MoEq.EQM_CODIGO FROM T_GRUPO_SERVICO_MARCA_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EQM_CODIGO = v.EQM_CODIGO))
                                               or (Grupo.GSF_POSSUI_MARCAS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
                                               ) ";

                queryGrupoServicoVeiculo = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND { (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} >= Grupo.GSF_KM_INICIAL
                                       AND {(filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} <= Grupo.GSF_KM_FINAL
                                       ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.VEI_DATACOMPRA, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.VEI_DATACOMPRA, getdate())))
				                       AND (
                                        ((V.VMO_CODIGO IN (SELECT MoEq.VMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMO_CODIGO = v.VMO_CODIGO))
                                           or (Grupo.GSF_POSSUI_MODELOS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
				                        and ((V.VMA_CODIGO in (SELECT MoEq.VMA_CODIGO FROM T_GRUPO_SERVICO_MARCA_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMA_CODIGO = v.VMA_CODIGO))
                                           or (Grupo.GSF_POSSUI_MARCAS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
                                           )";

                selectAgrupamento = @"  GrupoServicoVeiculo.GSV_VALIDADE_KM SEV_VALIDADE_KM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM SEV_TOLERANCIA_KM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS SEV_VALIDADE_DIAS, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS SEV_TOLERANCIA_DIAS, 
                                        GrupoServicoVeiculo.GSV_TIPO SEV_TIPO,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO SEV_TOLERANCIA_HORIMETRO,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO SEV_VALIDADE_HORIMETRO";

                selectExecucaoUnica = @"GrupoServicoVeiculo.GSV_VALIDADE_KM ValidadeKM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM ToleranciaKM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS ValidadeDias, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS ToleranciaDias, 
                                        GrupoServicoVeiculo.GSV_TIPO TipoServico,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO ValidadeHorimetro,";
            }

            string query = @"   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                TT.VEI_CODIGO CodigoVeiculo,
                TT.VEI_KMATUAL KmAtualVeiculo, 
                TT.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
                TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
                OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
                TT.SituacaoUltimaOS,
				TT.MarcaVeiculo,
				TT.ModeloVeiculo,
                TT.SegmentoVeiculo,
                TT.ResponsavelVeiculo,
		        TT.CentroResultado

                FROM (
                SELECT S.SEV_CODIGO, 
                S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,
                V.VEI_CODIGO,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as VEI_KMATUAL, 
                V.VEI_PLACA,
                { selectAgrupamento }
                , ISNULL((select top 1 OSE_SITUACAO from T_FROTA_ORDEM_SERVICO ordem
                          join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO servicoOrdem on servicoOrdem.OSE_CODIGO = ordem.OSE_CODIGO
                          where ordem.OSE_SITUACAO != 6 and ordem.VEI_CODIGO = V.VEI_CODIGO and servicoOrdem.SEV_CODIGO = S.SEV_CODIGO order by ordem.OSE_CODIGO desc), -1) SituacaoUltimaOS,
                MarcaVeiculo.VMA_DESCRICAO MarcaVeiculo,
				ModeloVeiculo.VMO_DESCRICAO ModeloVeiculo,
                SegmentoVeiculo.VSE_DESCRICAO SegmentoVeiculo,
                F.FUN_NOME ResponsavelVeiculo,
			    C.CRE_DESCRICAO CentroResultado

                FROM T_VEICULO V
                LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo ON MarcaVeiculo.VMA_CODIGO = V.VMA_CODIGO
				LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
				LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = V.CRE_CODIGO
				LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = V.FUN_CODIGO_RESPONSAVEL
				LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo ON SegmentoVeiculo.VSE_CODIGO = V.VSE_CODIGO
                , T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }                
                WHERE S.SEV_TIPO <> 6 and V.VEI_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) " +
                queryVeiculo + queryServico + queryLocalManutencaoVeiculo + queryGrupoServicoVeiculo + @" 
                ) AS TT

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
                                    os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.VEI_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.VEI_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.VEI_CODIGO is not null " + queryVeiculoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.VEI_CODIGO = TT.VEI_CODIGO AND RowNumber = 1" +

                queryServicosPendentesVeiculo + @"

                UNION ALL

				   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                TT.EQP_HODOMETRO KmAtualVeiculo, 
                '' PlacaVeiculo,
				TT.EQP_CODIGO CodigoEquipamento,
				TT.EQP_DESCRICAO Equipamento,
				TT.EQP_HORIMETRO HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
				TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
				OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
                TT.SituacaoUltimaOS,
				'' MarcaVeiculo,
				'' ModeloVeiculo,
                '' SegmentoVeiculo,
                Funcionario.ResponsavelVeiculo ResponsavelVeiculo,
		        '' CentroResultado

                FROM (
                SELECT S.SEV_CODIGO, 
                S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,                
				V.EQP_CODIGO,
				V.EQP_DESCRICAO,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @" as EQP_HORIMETRO,
				" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + $@" as EQP_HODOMETRO,
                { selectAgrupamento }
                , ISNULL((select top 1 OSE_SITUACAO from T_FROTA_ORDEM_SERVICO ordem
                          join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO servicoOrdem on servicoOrdem.OSE_CODIGO = ordem.OSE_CODIGO
                          where ordem.OSE_SITUACAO != 6 and ordem.EQP_CODIGO = V.EQP_CODIGO and servicoOrdem.SEV_CODIGO = S.SEV_CODIGO order by ordem.OSE_CODIGO desc), -1) SituacaoUltimaOS
                FROM T_EQUIPAMENTO V,
                T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                WHERE S.SEV_TIPO <> 6 and V.EQP_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) " + queryValidacao
                + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryGrupoServicoEquipamento + @" 
                ) AS TT

                OUTER APPLY (SELECT TOP 1 
								F.FUN_NOME ResponsavelVeiculo
							FROM T_EQUIPAMENTO Equipamento 
							JOIN T_VEICULO_EQUIPAMENTO VeiculoEquipamento on VeiculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO 
							JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoEquipamento.VEI_CODIGO
							JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL
							WHERE Equipamento.EQP_CODIGO = TT.EQP_CODIGO) Funcionario

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
									os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.EQP_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.EQP_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.EQP_CODIGO is not null " + queryEquipamentoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.EQP_CODIGO = TT.EQP_CODIGO AND RowNumber = 1" + queryServicosPendentesEquipamento
                +
                @"
                UNION ALL

                SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                V.VEI_CODIGO CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as KmAtualVeiculo, 
                V.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                { selectExecucaoUnica }
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
                NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                -1 SituacaoUltimaOS,
                MarcaVeiculo.VMA_DESCRICAO MarcaVeiculo,
				ModeloVeiculo.VMO_DESCRICAO ModeloVeiculo,
                SegmentoVeiculo.VSE_DESCRICAO SegmentoVeiculo,
                F.FUN_NOME ResponsavelVeiculo,
		        C.CRE_DESCRICAO CentroResultado

                from T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                , T_VEICULO V
                LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo ON MarcaVeiculo.VMA_CODIGO = V.VMA_CODIGO
				LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
				LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo ON SegmentoVeiculo.VSE_CODIGO = V.VSE_CODIGO
				LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = V.CRE_CODIGO
				LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = V.FUN_CODIGO_RESPONSAVEL
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_KM - GrupoServicoVeiculo.GSV_TOLERANCIA_KM)" : "(S.SEV_VALIDADE_KM - S.SEV_TOLERANCIA_KM)") }
                    <= ISNULL(" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + @", 0))" +
                $@"{ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? " AND (ISNULL(V.VEI_KMATUAL, 0) >= ISNULL(Grupo.GSF_KM_INICIAL, 0) AND  ISNULL(V.VEI_KMATUAL, 0) <= ISNULL(Grupo.GSF_KM_FINAL, 0)) " : " ") }                    
                " + queryExecutadoUmaVezVeiculo + queryVeiculo + queryServico + queryLocalManutencaoVeiculo +

                @"
                UNION ALL

				SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + @" as KmAtualVeiculo, 
                '' PlacaVeiculo,				
				V.EQP_CODIGO CodigoEquipamento,
				V.EQP_DESCRICAO Equipamento,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + $@" as HorimetroAtual,
                { selectExecucaoUnica }
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
				NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                -1 SituacaoUltimaOS,
				'' MarcaVeiculo,
				'' ModeloVeiculo,
                '' SegmentoVeiculo,
                Funcionario.ResponsavelVeiculo ResponsavelVeiculo,
		        '' CentroResultado

                from T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                , T_EQUIPAMENTO V
                OUTER APPLY (SELECT TOP 1 
								F.FUN_NOME ResponsavelVeiculo
							FROM T_EQUIPAMENTO Equipamento 
							JOIN T_VEICULO_EQUIPAMENTO VeiculoEquipamento on VeiculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO 
							JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoEquipamento.VEI_CODIGO
							JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL
							WHERE Equipamento.EQP_CODIGO = V.EQP_CODIGO) Funcionario
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO - GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO)" : "(S.SEV_VALIDADE_HORIMETRO - S.SEV_TOLERANCIA_HORIMETRO)") }
                    <= ISNULL(" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @", 0))                
                " + queryExecutadoUmaVezEquipamento + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryValidacao;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo>> RelatorioManutencaoVeiculoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string queryVeiculoRealizado = "";
            string queryVeiculo = "";
            string queryServico = "";
            string queryServicosPendentesVeiculo = "";
            string queryServicosPendentesEquipamento = "";
            string queryLocalManutencaoVeiculo = "";
            string queryEquipamentoRealizado = "";
            string queryEquipamento = "";
            string queryLocalManutencaoEquipamento = "";
            string queryValidacao = "";
            string queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            string queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

            if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente && filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO))) AND (ISNULL(OSS.HorimetroUltimaExecucao, 0) > 0)) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0) ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }
            else if (filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO)))) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0)";
            }
            else if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null ";
                queryServicosPendentesEquipamento = @" where OSS.CodigoOrdemServico is not null ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                List<int> codigosVeiculos = new List<int>();
                codigosVeiculos.Add(filtrosPesquisa.CodigoVeiculo);
                if (filtrosPesquisa.CodigosReboques?.Count > 0)
                    codigosVeiculos.AddRange(filtrosPesquisa.CodigosReboques);

                queryVeiculoRealizado = $" AND os.VEI_CODIGO in ({string.Join(", ", codigosVeiculos)})";
                queryVeiculo = $" AND V.VEI_CODIGO in ({string.Join(", ", codigosVeiculos)})";

                if (filtrosPesquisa.CodigoEquipamento == 0)
                    queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0 || filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
            {
                List<int> codigosEquipamentos = new List<int>();
                if (filtrosPesquisa.CodigoEquipamento > 0)
                    codigosEquipamentos.Add(filtrosPesquisa.CodigoEquipamento);
                if (filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
                    codigosEquipamentos.AddRange(filtrosPesquisa.CodigosEquipamentosAcoplados);

                queryEquipamentoRealizado = $" AND os.EQP_CODIGO in ({string.Join(", ", codigosEquipamentos)})";
                queryEquipamento = $" AND V.EQP_CODIGO in ({string.Join(", ", codigosEquipamentos)})";

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigosServico?.Count > 0)
                queryServico += " and S.SEV_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosServico) + ")";

            if (filtrosPesquisa.TiposManutencao?.Count > 0)
                queryServico += " and S.SEV_TIPO_MANUTENCAO in (" + string.Join(", ", filtrosPesquisa.TiposManutencao.Select(o => o.ToString("D"))) + ")";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PropriedadeVeiculo))
                queryVeiculo += " AND V.VEI_TIPO = '" + filtrosPesquisa.PropriedadeVeiculo + "'";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryVeiculo += " AND V.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa;

            if (filtrosPesquisa.CodigoModeloVeiculo > 0 || filtrosPesquisa.CodigoMarcaVeiculo > 0 || filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
            {
                if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                    queryVeiculo += " AND V.VMO_CODIGO = " + filtrosPesquisa.CodigoModeloVeiculo;

                if (filtrosPesquisa.CodigoMarcaVeiculo > 0)
                    queryVeiculo += " AND V.VMA_CODIGO = " + filtrosPesquisa.CodigoMarcaVeiculo;

                if (filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
                    queryServico += " and V.VSE_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSegmentoVeiculo) + ")";

                queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }
            else if (filtrosPesquisa.CodigoModeloEquipamento > 0 || filtrosPesquisa.CodigoMarcaEquipamento > 0)
            {
                if (filtrosPesquisa.CodigoModeloEquipamento > 0)
                    queryEquipamento += " AND V.EMO_CODIGO = " + filtrosPesquisa.CodigoModeloEquipamento;

                if (filtrosPesquisa.CodigoMarcaEquipamento > 0)
                    queryEquipamento += " AND V.EQM_CODIGO = " + filtrosPesquisa.CodigoMarcaEquipamento;

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CnpjCpfLocalManutencao > 0)
            {
                queryLocalManutencaoVeiculo = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.VEI_CODIGO = V.VEI_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";

                queryLocalManutencaoEquipamento = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.EQP_CODIGO = V.EQP_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";
            }

            if (filtrosPesquisa.CodigoFuncionarioResponsavel > 0)
            {
                queryVeiculo += " AND V.FUN_CODIGO_RESPONSAVEL = " + filtrosPesquisa.CodigoFuncionarioResponsavel;
                queryValidacao += " AND 29 != null";
            }

            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                queryVeiculo += " AND V.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;
                queryValidacao += " AND 30 != null";
            }

            if (filtrosPesquisa.VisualizarSomenteVeiculosAtivos)
            {
                queryVeiculo += " AND V.VEI_ATIVO = 1";
            }



            //SELECT
            var joinGrupoServico = "";
            var selectAgrupamento = @"  S.SEV_VALIDADE_KM, 
                                        S.SEV_TOLERANCIA_KM, 
                                        S.SEV_VALIDADE_DIAS, 
                                        S.SEV_TOLERANCIA_DIAS, 
                                        S.SEV_TIPO,
                                        S.SEV_TOLERANCIA_HORIMETRO,
				                        S.SEV_VALIDADE_HORIMETRO";

            var selectExecucaoUnica = @"S.SEV_VALIDADE_KM ValidadeKM, 
                                        S.SEV_TOLERANCIA_KM ToleranciaKM, 
                                        S.SEV_VALIDADE_DIAS ValidadeDias, 
                                        S.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                                        S.SEV_TIPO TipoServico,
                                        S.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        S.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,";

            string queryGrupoServicoEquipamento = "";
            string queryGrupoServicoVeiculo = "";
            if (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico)
            {
                joinGrupoServico = @"join T_GRUPO_SERVICO_SERVICO_VEICULO GrupoServicoVeiculo on GrupoServicoVeiculo.SEV_CODIGO = S.SEV_CODIGO
                                     join T_GRUPO_SERVICO Grupo on Grupo.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO ";

                queryServico += " AND Grupo.GSF_ATIVO = 1";

                queryGrupoServicoEquipamento = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} >= Grupo.GSF_KM_INICIAL
                                           AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} <= Grupo.GSF_KM_FINAL
                                           ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate())))
				                           AND (
                                            ((V.EMO_CODIGO IN (SELECT MoEq.EMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EMO_CODIGO = v.EMO_CODIGO))
                                               or (Grupo.GSF_POSSUI_MODELOS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
				                            and ((V.EQM_CODIGO in (SELECT MoEq.EQM_CODIGO FROM T_GRUPO_SERVICO_MARCA_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EQM_CODIGO = v.EQM_CODIGO))
                                               or (Grupo.GSF_POSSUI_MARCAS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
                                               ) ";

                queryGrupoServicoVeiculo = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND {(filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} >= Grupo.GSF_KM_INICIAL
                                       AND {(filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} <= Grupo.GSF_KM_FINAL
                                       ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.VEI_DATACOMPRA, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.VEI_DATACOMPRA, getdate())))
				                       AND (
                                        ((V.VMO_CODIGO IN (SELECT MoEq.VMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMO_CODIGO = v.VMO_CODIGO))
                                           or (Grupo.GSF_POSSUI_MODELOS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
				                        and ((V.VMA_CODIGO in (SELECT MoEq.VMA_CODIGO FROM T_GRUPO_SERVICO_MARCA_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMA_CODIGO = v.VMA_CODIGO))
                                           or (Grupo.GSF_POSSUI_MARCAS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
                                           )";

                selectAgrupamento = @"  GrupoServicoVeiculo.GSV_VALIDADE_KM SEV_VALIDADE_KM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM SEV_TOLERANCIA_KM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS SEV_VALIDADE_DIAS, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS SEV_TOLERANCIA_DIAS, 
                                        GrupoServicoVeiculo.GSV_TIPO SEV_TIPO,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO SEV_TOLERANCIA_HORIMETRO,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO SEV_VALIDADE_HORIMETRO";

                selectExecucaoUnica = @"GrupoServicoVeiculo.GSV_VALIDADE_KM ValidadeKM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM ToleranciaKM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS ValidadeDias, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS ToleranciaDias, 
                                        GrupoServicoVeiculo.GSV_TIPO TipoServico,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO ValidadeHorimetro,";
            }

            string query = @"   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                TT.VEI_CODIGO CodigoVeiculo,
                TT.VEI_KMATUAL KmAtualVeiculo, 
                TT.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
                TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
                OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
                TT.SituacaoUltimaOS,
				TT.MarcaVeiculo,
				TT.ModeloVeiculo,
                TT.SegmentoVeiculo,
                TT.ResponsavelVeiculo,
		        TT.CentroResultado

                FROM (
                SELECT S.SEV_CODIGO, 
                S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,
                V.VEI_CODIGO,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as VEI_KMATUAL, 
                V.VEI_PLACA,
                {selectAgrupamento}
                , ISNULL((select top 1 OSE_SITUACAO from T_FROTA_ORDEM_SERVICO ordem
                          join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO servicoOrdem on servicoOrdem.OSE_CODIGO = ordem.OSE_CODIGO
                          where ordem.OSE_SITUACAO != 6 and ordem.VEI_CODIGO = V.VEI_CODIGO and servicoOrdem.SEV_CODIGO = S.SEV_CODIGO order by ordem.OSE_CODIGO desc), -1) SituacaoUltimaOS,
                MarcaVeiculo.VMA_DESCRICAO MarcaVeiculo,
				ModeloVeiculo.VMO_DESCRICAO ModeloVeiculo,
                SegmentoVeiculo.VSE_DESCRICAO SegmentoVeiculo,
                F.FUN_NOME ResponsavelVeiculo,
			    C.CRE_DESCRICAO CentroResultado

                FROM T_VEICULO V
                LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo ON MarcaVeiculo.VMA_CODIGO = V.VMA_CODIGO
				LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
				LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = V.CRE_CODIGO
				LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = V.FUN_CODIGO_RESPONSAVEL
				LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo ON SegmentoVeiculo.VSE_CODIGO = V.VSE_CODIGO
                , T_FROTA_SERVICO_VEICULO S
                {joinGrupoServico}                
                WHERE S.SEV_TIPO <> 6 and V.VEI_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) " +
                queryVeiculo + queryServico + queryLocalManutencaoVeiculo + queryGrupoServicoVeiculo + @" 
                ) AS TT

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
                                    os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.VEI_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.VEI_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.VEI_CODIGO is not null " + queryVeiculoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.VEI_CODIGO = TT.VEI_CODIGO AND RowNumber = 1" +

                queryServicosPendentesVeiculo + @"

                UNION ALL

				   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                TT.EQP_HODOMETRO KmAtualVeiculo, 
                '' PlacaVeiculo,
				TT.EQP_CODIGO CodigoEquipamento,
				TT.EQP_DESCRICAO Equipamento,
				TT.EQP_HORIMETRO HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
				TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
				OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
                TT.SituacaoUltimaOS,
				'' MarcaVeiculo,
				'' ModeloVeiculo,
                '' SegmentoVeiculo,
                Funcionario.ResponsavelVeiculo ResponsavelVeiculo,
		        '' CentroResultado

                FROM (
                SELECT S.SEV_CODIGO, 
                S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,                
				V.EQP_CODIGO,
				V.EQP_DESCRICAO,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @" as EQP_HORIMETRO,
				" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + $@" as EQP_HODOMETRO,
                {selectAgrupamento}
                , ISNULL((select top 1 OSE_SITUACAO from T_FROTA_ORDEM_SERVICO ordem
                          join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO servicoOrdem on servicoOrdem.OSE_CODIGO = ordem.OSE_CODIGO
                          where ordem.OSE_SITUACAO != 6 and ordem.EQP_CODIGO = V.EQP_CODIGO and servicoOrdem.SEV_CODIGO = S.SEV_CODIGO order by ordem.OSE_CODIGO desc), -1) SituacaoUltimaOS
                FROM T_EQUIPAMENTO V,
                T_FROTA_SERVICO_VEICULO S
                {joinGrupoServico}
                WHERE S.SEV_TIPO <> 6 and V.EQP_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) " + queryValidacao
                + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryGrupoServicoEquipamento + @" 
                ) AS TT

                OUTER APPLY (SELECT TOP 1 
								F.FUN_NOME ResponsavelVeiculo
							FROM T_EQUIPAMENTO Equipamento 
							JOIN T_VEICULO_EQUIPAMENTO VeiculoEquipamento on VeiculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO 
							JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoEquipamento.VEI_CODIGO
							JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL
							WHERE Equipamento.EQP_CODIGO = TT.EQP_CODIGO) Funcionario

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
									os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.EQP_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.EQP_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.EQP_CODIGO is not null " + queryEquipamentoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.EQP_CODIGO = TT.EQP_CODIGO AND RowNumber = 1" + queryServicosPendentesEquipamento
                +
                @"
                UNION ALL

                SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                V.VEI_CODIGO CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as KmAtualVeiculo, 
                V.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                {selectExecucaoUnica}
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
                NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                -1 SituacaoUltimaOS,
                MarcaVeiculo.VMA_DESCRICAO MarcaVeiculo,
				ModeloVeiculo.VMO_DESCRICAO ModeloVeiculo,
                SegmentoVeiculo.VSE_DESCRICAO SegmentoVeiculo,
                F.FUN_NOME ResponsavelVeiculo,
		        C.CRE_DESCRICAO CentroResultado

                from T_FROTA_SERVICO_VEICULO S
                {joinGrupoServico}
                , T_VEICULO V
                LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo ON MarcaVeiculo.VMA_CODIGO = V.VMA_CODIGO
				LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
				LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo ON SegmentoVeiculo.VSE_CODIGO = V.VSE_CODIGO
				LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = V.CRE_CODIGO
				LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = V.FUN_CODIGO_RESPONSAVEL
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({(filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_KM - GrupoServicoVeiculo.GSV_TOLERANCIA_KM)" : "(S.SEV_VALIDADE_KM - S.SEV_TOLERANCIA_KM)")}
                    <= ISNULL(" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + @", 0))" +
                $@"{(filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? " AND (ISNULL(V.VEI_KMATUAL, 0) >= ISNULL(Grupo.GSF_KM_INICIAL, 0) AND  ISNULL(V.VEI_KMATUAL, 0) <= ISNULL(Grupo.GSF_KM_FINAL, 0)) " : " ")}                    
                " + queryExecutadoUmaVezVeiculo + queryVeiculo + queryServico + queryLocalManutencaoVeiculo +

                @"
                UNION ALL

				SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + @" as KmAtualVeiculo, 
                '' PlacaVeiculo,				
				V.EQP_CODIGO CodigoEquipamento,
				V.EQP_DESCRICAO Equipamento,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + $@" as HorimetroAtual,
                {selectExecucaoUnica}
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
				NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                -1 SituacaoUltimaOS,
				'' MarcaVeiculo,
				'' ModeloVeiculo,
                '' SegmentoVeiculo,
                Funcionario.ResponsavelVeiculo ResponsavelVeiculo,
		        '' CentroResultado

                from T_FROTA_SERVICO_VEICULO S
                {joinGrupoServico}
                , T_EQUIPAMENTO V
                OUTER APPLY (SELECT TOP 1 
								F.FUN_NOME ResponsavelVeiculo
							FROM T_EQUIPAMENTO Equipamento 
							JOIN T_VEICULO_EQUIPAMENTO VeiculoEquipamento on VeiculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO 
							JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoEquipamento.VEI_CODIGO
							JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL
							WHERE Equipamento.EQP_CODIGO = V.EQP_CODIGO) Funcionario
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({(filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO - GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO)" : "(S.SEV_VALIDADE_HORIMETRO - S.SEV_TOLERANCIA_HORIMETRO)")}
                    <= ISNULL(" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @", 0))                
                " + queryExecutadoUmaVezEquipamento + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryValidacao;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo)));

            return await nhQuery.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.ManutencaoVeiculo>();
        }


        public int ContarRelatorioManutencaoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();
            string queryVeiculoRealizado = "";
            string queryVeiculo = "";
            string queryServico = "";
            string queryServicosPendentesVeiculo = "";
            string queryServicosPendentesEquipamento = "";
            string queryLocalManutencaoVeiculo = "";
            string queryEquipamentoRealizado = "";
            string queryEquipamento = "";
            string queryValidacao = "";
            string queryLocalManutencaoEquipamento = "";
            string queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            string queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO NOT IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

            if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente && filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where OSS.CodigoOrdemServico is not null and (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO))) AND (ISNULL(OSS.HorimetroUltimaExecucao, 0) > 0)) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS))) AND (ISNULL(OSS.QuilometragemUltimaExecucao, 0) > 0)))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0) ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }
            else if (filtrosPesquisa.VisualizarServicosPendentesManutencao)
            {
                queryServicosPendentesVeiculo = @" where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.VEI_KMATUAL >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0)";
                queryServicosPendentesEquipamento = @"where (
				    ((TT.SEV_VALIDADE_KM > 0) AND (TT.EQP_HODOMETRO >= (ISNULL(OSS.QuilometragemUltimaExecucao, 0) + (TT.SEV_VALIDADE_KM - TT.SEV_TOLERANCIA_KM)))) 
				    OR
					((TT.SEV_VALIDADE_HORIMETRO > 0) AND (TT.EQP_HORIMETRO >= (ISNULL(OSS.HorimetroUltimaExecucao, 0) + (TT.SEV_VALIDADE_HORIMETRO - TT.SEV_TOLERANCIA_HORIMETRO)))) 
					OR
				    ((TT.SEV_VALIDADE_DIAS > 0) AND (GETDATE() >= (OSS.DataUltimaExecucao + (TT.SEV_VALIDADE_DIAS - TT.SEV_TOLERANCIA_DIAS)))))				
				    AND (TT.SEV_VALIDADE_KM > 0 OR TT.SEV_VALIDADE_DIAS > 0 or TT.SEV_VALIDADE_HORIMETRO > 0)";
            }
            else if (filtrosPesquisa.VisualizarSomenteServicosExecutadosAnteriormente)
            {
                queryServicosPendentesVeiculo = @" where OSS.CodigoOrdemServico is not null ";
                queryServicosPendentesEquipamento = @" where OSS.CodigoOrdemServico is not null ";

                queryExecutadoUmaVezEquipamento = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.EQP_CODIGO = V.EQP_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";

                queryExecutadoUmaVezVeiculo = @"AND S.SEV_CODIGO IN 
                                                       (SELECT M.SEV_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                          JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                         WHERE OS.VEI_CODIGO = V.VEI_CODIGO AND (OS.OSE_SITUACAO = 5 OR OS.OSE_SITUACAO = 7))";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                List<int> codigosVeiculos = new List<int>();
                codigosVeiculos.Add(filtrosPesquisa.CodigoVeiculo);
                if (filtrosPesquisa.CodigosReboques?.Count > 0)
                    codigosVeiculos.AddRange(filtrosPesquisa.CodigosReboques);

                queryVeiculoRealizado = $" AND os.VEI_CODIGO in ({ string.Join(", ", codigosVeiculos) })";
                queryVeiculo = $" AND V.VEI_CODIGO in ({ string.Join(", ", codigosVeiculos) })";

                if (filtrosPesquisa.CodigoEquipamento == 0)
                    queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0 || filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
            {
                List<int> codigosEquipamentos = new List<int>();
                if (filtrosPesquisa.CodigoEquipamento > 0)
                    codigosEquipamentos.Add(filtrosPesquisa.CodigoEquipamento);
                if (filtrosPesquisa.CodigosEquipamentosAcoplados?.Count > 0)
                    codigosEquipamentos.AddRange(filtrosPesquisa.CodigosEquipamentosAcoplados);

                queryEquipamentoRealizado = $" AND os.EQP_CODIGO in ({ string.Join(", ", codigosEquipamentos) })";
                queryEquipamento = $" AND V.EQP_CODIGO in ({ string.Join(", ", codigosEquipamentos) })";

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigosServico?.Count > 0)
                queryServico += " and S.SEV_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosServico) + ")";

            if (filtrosPesquisa.TiposManutencao?.Count > 0)
                queryServico += " and S.SEV_TIPO_MANUTENCAO in (" + string.Join(", ", filtrosPesquisa.TiposManutencao.Select(o => o.ToString("D"))) + ")";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PropriedadeVeiculo))
            {
                queryVeiculo += " AND V.VEI_TIPO = :V_VEI_TIPO";
                parametros.Add(new ParametroSQL("V_VEI_TIPO", filtrosPesquisa.PropriedadeVeiculo));
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryVeiculo += " AND V.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa;

            if (filtrosPesquisa.CodigoModeloVeiculo > 0 || filtrosPesquisa.CodigoMarcaVeiculo > 0 || filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
            {
                if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                    queryVeiculo += " AND V.VMO_CODIGO = " + filtrosPesquisa.CodigoModeloVeiculo;

                if (filtrosPesquisa.CodigoMarcaVeiculo > 0)
                    queryVeiculo += " AND V.VMA_CODIGO = " + filtrosPesquisa.CodigoMarcaVeiculo;

                if (filtrosPesquisa.CodigosSegmentoVeiculo?.Count > 0)
                    queryServico += " and V.VSE_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSegmentoVeiculo) + ")";

                queryEquipamento = " AND V.EQP_CODIGO IS NULL";
            }
            else if (filtrosPesquisa.CodigoModeloEquipamento > 0 || filtrosPesquisa.CodigoMarcaEquipamento > 0)
            {
                if (filtrosPesquisa.CodigoModeloEquipamento > 0)
                    queryEquipamento += " AND V.EMO_CODIGO = " + filtrosPesquisa.CodigoModeloEquipamento;

                if (filtrosPesquisa.CodigoMarcaEquipamento > 0)
                    queryEquipamento += " AND V.EQM_CODIGO = " + filtrosPesquisa.CodigoMarcaEquipamento;

                if (filtrosPesquisa.CodigoVeiculo == 0)
                    queryVeiculo = " AND V.VEI_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CnpjCpfLocalManutencao > 0)
            {
                queryLocalManutencaoVeiculo = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.VEI_CODIGO = V.VEI_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";

                queryLocalManutencaoEquipamento = @" AND S.SEV_CODIGO IN (SELECT M.SEV_CODIGO
                                                FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO M
                                                JOIN T_FROTA_ORDEM_SERVICO OS ON OS.OSE_CODIGO = M.OSE_CODIGO
                                                WHERE OS.EQP_CODIGO = V.EQP_CODIGO
                                                AND OS.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfLocalManutencao + ")";
            }

            if (filtrosPesquisa.CodigoFuncionarioResponsavel > 0)
            {
                queryVeiculo += " AND V.FUN_CODIGO_RESPONSAVEL = " + filtrosPesquisa.CodigoFuncionarioResponsavel;
                queryValidacao += " AND 24 != null";
            }

            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                queryVeiculo += " AND V.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;
                queryValidacao += " AND 25 != null";
            }




            //SELECT
            var joinGrupoServico = "";
            var selectAgrupamento = @"  S.SEV_VALIDADE_KM, 
                                        S.SEV_TOLERANCIA_KM, 
                                        S.SEV_VALIDADE_DIAS, 
                                        S.SEV_TOLERANCIA_DIAS, 
                                        S.SEV_TIPO,
                                        S.SEV_TOLERANCIA_HORIMETRO,
				                        S.SEV_VALIDADE_HORIMETRO";

            var selectExecucaoUnica = @"S.SEV_VALIDADE_KM ValidadeKM, 
                                        S.SEV_TOLERANCIA_KM ToleranciaKM, 
                                        S.SEV_VALIDADE_DIAS ValidadeDias, 
                                        S.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                                        S.SEV_TIPO TipoServico,
                                        S.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        S.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,";

            string queryGrupoServicoEquipamento = "";
            string queryGrupoServicoVeiculo = "";
            if (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico)
            {
                joinGrupoServico = @"join T_GRUPO_SERVICO_SERVICO_VEICULO GrupoServicoVeiculo on GrupoServicoVeiculo.SEV_CODIGO = S.SEV_CODIGO
                                     join T_GRUPO_SERVICO Grupo on Grupo.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO ";

                queryServico += " AND Grupo.GSF_ATIVO = 1";

                queryGrupoServicoEquipamento = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} >= Grupo.GSF_KM_INICIAL
                                           AND {(filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO")} <= Grupo.GSF_KM_FINAL
                                           ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.EQP_DATA_AQUISICAO, getdate())))
				                           AND (
                                            ((V.EMO_CODIGO IN (SELECT MoEq.EMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EMO_CODIGO = v.EMO_CODIGO))
                                               or (Grupo.GSF_POSSUI_MODELOS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
				                            and ((V.EQM_CODIGO in (SELECT MoEq.EQM_CODIGO FROM T_GRUPO_SERVICO_MARCA_EQUIPAMENTO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.EQM_CODIGO = v.EQM_CODIGO))
                                               or (Grupo.GSF_POSSUI_MARCAS_EQUIPAMENTO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 2)))
                                               ) ";

                queryGrupoServicoVeiculo = $@" AND ((Grupo.GSF_KM_FINAL > 0 AND {(filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} >= Grupo.GSF_KM_INICIAL
                                       AND {(filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL")} <= Grupo.GSF_KM_FINAL
                                       ) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, V.VEI_DATACOMPRA, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, V.VEI_DATACOMPRA, getdate())))
				                       AND (
                                        ((V.VMO_CODIGO IN (SELECT MoEq.VMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMO_CODIGO = v.VMO_CODIGO))
                                           or (Grupo.GSF_POSSUI_MODELOS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
				                        and ((V.VMA_CODIGO in (SELECT MoEq.VMA_CODIGO FROM T_GRUPO_SERVICO_MARCA_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMA_CODIGO = v.VMA_CODIGO))
                                           or (Grupo.GSF_POSSUI_MARCAS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
                                           )";

                selectAgrupamento = @"  GrupoServicoVeiculo.GSV_VALIDADE_KM SEV_VALIDADE_KM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM SEV_TOLERANCIA_KM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS SEV_VALIDADE_DIAS, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS SEV_TOLERANCIA_DIAS, 
                                        GrupoServicoVeiculo.GSV_TIPO SEV_TIPO,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO SEV_TOLERANCIA_HORIMETRO,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO SEV_VALIDADE_HORIMETRO";

                selectExecucaoUnica = @"GrupoServicoVeiculo.GSV_VALIDADE_KM ValidadeKM, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_KM ToleranciaKM, 
                                        GrupoServicoVeiculo.GSV_VALIDADE_DIAS ValidadeDias, 
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS ToleranciaDias, 
                                        GrupoServicoVeiculo.GSV_TIPO TipoServico,
                                        GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				                        GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO ValidadeHorimetro,";
            }

            string query = $@"   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                TT.VEI_CODIGO CodigoVeiculo,
                TT.VEI_KMATUAL KmAtualVeiculo, 
                TT.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
                TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
                OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
                TT.FUN_CODIGO_RESPONSAVEL FuncionarioResponsavel,
			    TT.CRE_CODIGO CentroResultados

                FROM (
                SELECT S.SEV_CODIGO, 
                S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,
                V.VEI_CODIGO,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as VEI_KMATUAL, 
                V.VEI_PLACA,
                V.FUN_CODIGO_RESPONSAVEL,
                V.CRE_CODIGO,
                { selectAgrupamento }
                FROM T_VEICULO V,
                T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                WHERE S.SEV_TIPO <> 6 and V.VEI_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) " +
                queryVeiculo + queryServico + queryLocalManutencaoVeiculo + queryGrupoServicoVeiculo + @" 
                ) AS TT

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
                                    os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.VEI_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.VEI_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.VEI_CODIGO is not null " + queryVeiculoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.VEI_CODIGO = TT.VEI_CODIGO AND RowNumber = 1" +

                queryServicosPendentesVeiculo + $@"

                UNION ALL

				   SELECT TT.SEV_CODIGO CodigoServico,
                TT.SEV_DESCRICAO DescricaoServico,
                TT.SEV_MOTIVO MotivoServico,
                TT.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                TT.EQP_HODOMETRO KmAtualVeiculo, 
                '' PlacaVeiculo,
				TT.EQP_CODIGO CodigoEquipamento,
				TT.EQP_DESCRICAO Equipamento,
				TT.EQP_HORIMETRO HorimetroAtual,
                TT.SEV_VALIDADE_KM ValidadeKM,  
                TT.SEV_TOLERANCIA_KM ToleranciaKM, 
                TT.SEV_VALIDADE_DIAS ValidadeDias, 
                TT.SEV_TOLERANCIA_DIAS ToleranciaDias, 
                TT.SEV_TIPO TipoServico,
				TT.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetro,
				TT.SEV_VALIDADE_HORIMETRO ValidadeHorimetro,
                OSS.OSE_NUMERO NumeroOS,
                OSS.OSS_OBSERVACAO ObservacaoOS,
                OSS.DataUltimaExecucao, 
                OSS.QuilometragemUltimaExecucao,
				OSS.HorimetroUltimaExecucao,
                0 ExecucaoUnica,
			    '' FuncionarioResponsavel,
			    '' CentroResultados

                FROM (
                SELECT S.SEV_CODIGO, 
				S.SEV_DESCRICAO,
                S.SEV_MOTIVO,
                S.SEV_OBSERVACAO,                
				V.EQP_CODIGO,
				V.EQP_DESCRICAO,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @" as EQP_HORIMETRO,
				" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + $@" as EQP_HODOMETRO,
                { selectAgrupamento }
                FROM T_EQUIPAMENTO V,
                T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                WHERE S.SEV_TIPO <> 6 and V.EQP_ATIVO = 1 AND S.SEV_ATIVO = 1 AND (S.SEV_EXECUCAO_UNICA = 0 OR S.SEV_EXECUCAO_UNICA IS NULL) {queryValidacao}"
                + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryGrupoServicoEquipamento + @" 
                ) AS TT

                LEFT OUTER JOIN (SELECT os.OSE_CODIGO CodigoOrdemServico, 
                                    os.OSE_NUMERO,
                                    manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                    manutencao.OSS_OBSERVACAO,
                                    os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                                    os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
									os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                    manutencao.SEV_CODIGO, os.EQP_CODIGO,
                                    ROW_NUMBER() OVER(PARTITION BY manutencao.SEV_CODIGO, os.EQP_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                                    FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao
                                    JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO
                                    WHERE (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) and os.EQP_CODIGO is not null " + queryEquipamentoRealizado + @") AS OSS
                ON OSS.SEV_CODIGO = TT.SEV_CODIGO AND OSS.EQP_CODIGO = TT.EQP_CODIGO AND RowNumber = 1" + queryServicosPendentesEquipamento
                +
                $@"
                UNION ALL

                SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                V.VEI_CODIGO CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + $@" as KmAtualVeiculo, 
                V.VEI_PLACA PlacaVeiculo,
                0 CodigoEquipamento,
				'' Equipamento,
				0 HorimetroAtual,
                { selectExecucaoUnica }
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
                NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                V.FUN_CODIGO_RESPONSAVEL FuncionarioResponsavel,
			    V.CRE_CODIGO CentroResultado
                from T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                , T_VEICULO V
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_KM - GrupoServicoVeiculo.GSV_TOLERANCIA_KM)" : "(S.SEV_VALIDADE_KM - S.SEV_TOLERANCIA_KM)") }
                    <= ISNULL(" + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.VEI_KMATUAL") + @", 0))" +
                $@"{ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? " AND (ISNULL(V.VEI_KMATUAL, 0) >= ISNULL(Grupo.GSF_KM_INICIAL, 0) AND  ISNULL(V.VEI_KMATUAL, 0) <= ISNULL(Grupo.GSF_KM_FINAL, 0)) " : " ") }                    
                " + queryExecutadoUmaVezVeiculo + queryVeiculo + queryServico + queryLocalManutencaoVeiculo +

                $@"
                UNION ALL

				SELECT S.SEV_CODIGO CodigoServico,
                S.SEV_DESCRICAO DescricaoServico,
                S.SEV_MOTIVO MotivoServico,
                S.SEV_OBSERVACAO ObservacaoServico,
                0 CodigoVeiculo,
                " + (filtrosPesquisa.KMAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.KMAtual.ToString("n0")) : "V.EQP_HODOMETRO") + @" as KmAtualVeiculo, 
                '' PlacaVeiculo,				
				V.EQP_CODIGO CodigoEquipamento,
				V.EQP_DESCRICAO Equipamento,
				" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + $@" as HorimetroAtual,
                { selectExecucaoUnica }
                NULL NumeroOS,
                NULL ObservacaoOS,
                NULL DataUltimaExecucao, 
                NULL QuilometragemUltimaExecucao,
				NULL HorimetroUltimaExecucao,
                S.SEV_EXECUCAO_UNICA ExecucaoUnica,
                '' FuncionarioResponsavel,
			    '' CentroResultados
                from T_FROTA_SERVICO_VEICULO S
                { joinGrupoServico }
                ,T_EQUIPAMENTO V
                WHERE S.SEV_TIPO <> 6 and S.SEV_ATIVO = 1
                AND S.SEV_EXECUCAO_UNICA = 1
                AND ({ (filtrosPesquisa.UtilizarValidadeServicoPeloGrupoServicoOrdemServico ? "(GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO - GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO)" : "(S.SEV_VALIDADE_HORIMETRO - S.SEV_TOLERANCIA_HORIMETRO)") }
                    <= ISNULL(" + (filtrosPesquisa.HorimetroAtual > 0 ? Utilidades.String.OnlyNumbers(filtrosPesquisa.HorimetroAtual.ToString("n0")) : "V.EQP_HORIMETRO") + @", 0))                
                " + queryExecutadoUmaVezEquipamento + queryEquipamento + queryServico + queryLocalManutencaoEquipamento + queryValidacao;

            query = "SELECT COUNT(0) as CONTADOR FROM (" + query + ") AS TTT "; 

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.UniqueResult<int>();
        }

        #endregion

        #region Relatório Despesa Ordem Serviço

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico> ConsultarRelatorioDespesaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServico(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico>> ConsultarRelatorioDespesaOrdemServicoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServico(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico>();
        }

        public int ContarConsultaRelatorioDespesaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServico(filtros, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtros, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDespesaOrdemServico(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioDespesaOrdemServico(ref where, ref groupBy, ref joins, filtros);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDespesaOrdemServico(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        if (propOrdena == "MesAnoOS")
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + "AnoOS " + dirOrdena + ", MesOS " + dirOrdena;
                        else
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_FROTA_ORDEM_SERVICO OrdemServico ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioDespesaOrdemServico(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "OrdemServico.OSE_CODIGO Codigo, ";
                        groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += @"OrdemServico.OSE_SITUACAO Situacao, ";
                        groupBy += "OrdemServico.OSE_SITUACAO, ";
                    }
                    break;
                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        if (!joins.Contains(" Tipo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_TIPO Tipo ON Tipo.FOT_CODIGO = OrdemServico.FOT_CODIGO";

                        select += "ISNULL(Tipo.FOT_DESCRICAO, 'NÃO INFORMADO') as Tipo, ";
                        groupBy += "Tipo.FOT_DESCRICAO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Placa, ";
                        groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "Equipamento":
                    if (!select.Contains(" Equipamento, "))
                    {
                        if (!joins.Contains(" Equipamento "))
                            joins += " LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO";

                        select += "Equipamento.EQP_DESCRICAO Equipamento, ";
                        groupBy += "Equipamento.EQP_DESCRICAO, ";
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" GrupoProdutoTMS "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProdutoTMS ON GrupoProdutoTMS.GPR_CODIGO = OrdemServico.EQP_CODIGO";

                        select += "GrupoProdutoTMS.GRP_DESCRICAO GrupoProduto, ";
                        groupBy += "GrupoProdutoTMS.GRP_DESCRICAO, ";
                    }
                    break;
                case "Ano":
                    if (!select.Contains(" Ano, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_ANO Ano, ";
                        groupBy += "Veiculo.VEI_ANO, ";
                    }
                    break;
                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Modelo "))
                            joins += " LEFT JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO";

                        select += "Modelo.VMO_DESCRICAO Modelo, ";
                        groupBy += "Modelo.VMO_DESCRICAO, ";
                    }
                    break;
                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Marca "))
                            joins += " LEFT JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = Veiculo.VMA_CODIGO";

                        select += "Marca.VMA_DESCRICAO Marca, ";
                        groupBy += "Marca.VMA_DESCRICAO, ";
                    }
                    break;
                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao, "))
                    {
                        if (!joins.Contains(" LocalManutencao "))
                            joins += " LEFT JOIN T_CLIENTE LocalManutencao ON LocalManutencao.CLI_CGCCPF = OrdemServico.CLI_CGCCPF";

                        select += "LocalManutencao.CLI_NOME LocalManutencao, ";
                        groupBy += "LocalManutencao.CLI_NOME, ";
                    }
                    break;
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select += "OrdemServico.OSE_DATA_PROGRAMADA Data, ";
                        groupBy += "OrdemServico.OSE_DATA_PROGRAMADA, ";
                    }
                    break;
                case "MesAnoOS":
                    if (!select.Contains(" MesAnoOS, "))
                    {
                        select += "REPLICATE('0', 2 - LEN(CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)))) + CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)) + '-' + CAST(YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(4)) MesAnoOS, ";
                        select += "YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AnoOS, ";
                        select += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA) MesOS, ";
                        groupBy += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA), YEAR(OrdemServico.OSE_DATA_PROGRAMADA), ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "OrdemServico.OSE_NUMERO Numero, ";
                        groupBy += "OrdemServico.OSE_NUMERO, ";
                    }
                    break;
                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN  T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_PRODUTOS) ValorProdutos, ";
                    }
                    break;
                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_MAO_OBRA) ValorServicos, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_ORCADO) ValorTotal, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(P.PRO_DESCRICAO AS NVARCHAR(160))
		                                        FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                                JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
		                                        WHERE PRO_CATEGORIA_PRODUTO <> 9 AND F.OSE_CODIGO = OrdemServico.OSE_CODIGO FOR XML PATH('')), 3, 1000) AS Produto, ";
                    }
                    break;
                case "Servico":
                    if (!select.Contains(" Servico, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(SEV_DESCRICAO AS NVARCHAR(160))
		                                        FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO SVOS
                                                JOIN T_FROTA_SERVICO_VEICULO SV ON SV.SEV_CODIGO = SVOS.SEV_CODIGO
		                                        WHERE SVOS.OSE_CODIGO = OrdemServico.OSE_CODIGO FOR XML PATH('')), 3, 1000) AS Servico, ";
                    }
                    break;
                case "ValorProdutosFechamento":
                    if (!select.Contains(" ValorProdutosFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO <> 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorProdutosFechamento, ";
                    }
                    break;
                case "ValorServicosFechamento":
                    if (!select.Contains(" ValorServicosFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO = 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorServicosFechamento, ";
                    }
                    break;
                case "ValorTotalFechamento":
                    if (!select.Contains(" ValorTotalFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorTotalFechamento, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDespesaOrdemServico(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtros)
        {
            string pattern = "yyyy-MM-dd";

            if (filtros.TipoOrdemServico != null & filtros.TipoOrdemServico.Count > 0)
                where += $" AND OrdemServico.FOT_CODIGO in ({string.Join(",", filtros.TipoOrdemServico)}) ";

            if (filtros.Empresa > 0)
                where += " AND OrdemServico.EMP_CODIGO = '" + filtros.Empresa.ToString() + "' ";

            if (filtros.DataInicial != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) >= '" + filtros.DataInicial.ToString(pattern) + "' ";

            if (filtros.DataFinal != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) <= '" + filtros.DataFinal.ToString(pattern) + "'";

            if (filtros.NumeroOS > 0)
                where += " AND OrdemServico.OSE_NUMERO = " + filtros.NumeroOS;

            if (filtros.Veiculo > 0)
                where += " AND OrdemServico.VEI_CODIGO = " + filtros.Veiculo;

            if (filtros.ModeloVeiculo != null & filtros.ModeloVeiculo.Count > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += $" AND Veiculo.VMO_CODIGO in ({string.Join(",", filtros.ModeloVeiculo)}) ";
            }

            if (filtros.LocalManutencao > 0)
                where += " AND OrdemServico.CLI_CGCCPF = " + filtros.LocalManutencao;

            if (filtros.Situacoes?.Count > 0)
                where += $" AND OrdemServico.OSE_SITUACAO IN ({ string.Join(", ", filtros.Situacoes.Select(o => o.ToString("D"))) })";

            if (filtros.MarcaVeiculo > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += " AND Veiculo.VMA_CODIGO = " + filtros.MarcaVeiculo;
            }

            if (filtros.Servico != null & filtros.Servico.Count > 0)
            {
                where += @" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO 
                                                            WHERE SEV_CODIGO IN (" + string.Join(",", filtros.Servico) + "))";
            }

            if (filtros.Equipamento != null & filtros.Equipamento.Count > 0)
            {
                where += @" AND OrdemServico.EQP_CODIGO IN (" + string.Join(",", filtros.Equipamento) + ")";
            }

            if (filtros.Produto != null & filtros.Produto.Count > 0)
            {
                where += @" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO 
                                                            WHERE PRO_CODIGO IN (" + string.Join(",", filtros.Produto) + "))";
            }
            if (filtros.GrupoProduto != null & filtros.GrupoProduto.Count > 0)
            {
                where += @" AND OrdemServico.EQP_CODIGO IN (SELECT DISTINCT GPR_CODIGO 
                                                            FROM T_GRUPO_PRODUTO_TMS
                                                            WHERE GPR_CODIGO IN (" + string.Join(",", filtros.GrupoProduto) + "))";
            }

        }

        #endregion

        #region Relatório de Ordem de Serviço

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico> ConsultarRelatorioOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery query = new ConsultaOrdemServico().ObterSqlPesquisa(filtros, parametroConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico>> ConsultarRelatorioOrdemServicoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery query = new ConsultaOrdemServico().ObterSqlPesquisa(filtros, parametroConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico)));

            return await query.SetTimeout(300).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico>();
        }

        public int ContarConsultaRelatorioOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery query = new ConsultaOrdemServico().ObterSqlContarPesquisa(filtros, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.UniqueResult<int>();
        }

        #endregion

        #region Relatório Detalhado Despesa Ordem Serviço

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico> ConsultarRelatorioDespesaDetalhadaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaDetalhadaOrdemServico(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico>> ConsultarRelatorioDespesaDetalhadaOrdemServicoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaDetalhadaOrdemServico(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaDetalhadaOrdemServico>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico> ConsultarRelatorioAuditoriaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros)
        {
            string sql = ObterSelectPrincipalConsultaRelatorioAuditoriaOrdemServico(filtros);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico>> ConsultarRelatorioAuditoriaOrdemServicoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros)
        {
            string sql = ObterSelectPrincipalConsultaRelatorioAuditoriaOrdemServico(filtros);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico>> ConsultarRelatorioAuditoriaOrdemServicoServicos(List<int> codigosOrdemServico)
        {
            string sql = ObterSelectServicosRelatorioAuditoriaOrdemServico(codigosOrdemServico);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoServico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto>> ConsultarRelatorioAuditoriaOrdemServicoProdutos(List<int> codigosOrdemServico)
        {
            string sql = ObterSelectProdutosRelatorioAuditoriaOrdemServico(codigosOrdemServico);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.AuditoriaOrdemServicoProduto>();
        }

        public int ContarConsultaRelatorioDespesaDetalhadaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioDespesaDetalhadaOrdemServico(filtros, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectPrincipalConsultaRelatorioAuditoriaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros)
        {
            string select = @"SELECT 
	                                OrdemServico.OSE_CODIGO as Codigo, 
                                    OrdemServico.OSE_NUMERO as Numero,
                                    OrdemServico.OSE_OBSERVACAO as Observacao,
	                                ISNULL(Veiculo.VEI_PLACA, Equipamento.EQP_DESCRICAO) as Placa, 
	                                ISNULL(Modelo.VMO_DESCRICAO, EquipamentoModelo.EMO_DESCRICAO) as Nome, 
	                                ISNULL(Veiculo.VEI_KMATUAL, 0) as QuilometragemAtual, 
	                                ISNULL(Equipamento.EQP_HORIMETRO, 0) as HorimetroAtual,
                                    ISNULL((SELECT SUM(OrdemServicoFechamentoProduto.OFP_VALOR_DOCUMENTO) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamentoProduto WHERE OrdemServicoFechamentoProduto.OSE_CODIGO = OrdemServico.OSE_CODIGO), 0) CustoTotalProduto
                                FROM
	                                T_FROTA_ORDEM_SERVICO OrdemServico
	                                left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO
	                                left join T_VEICULO_MODELO Modelo on Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO
	                                left join T_EQUIPAMENTO Equipamento on Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO
                                    LEFT JOIN T_EQUIPAMENTO_MODELO EquipamentoModelo on EquipamentoModelo.EMO_CODIGO = Equipamento.EMO_CODIGO";

            string where = "";
            string joins = "";
            string groupBy = "";

            SetarWhereRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(ref where, ref groupBy, ref joins, filtros);

            groupBy = @" group by 
	                    OrdemServico.OSE_CODIGO,
                        OrdemServico.OSE_OBSERVACAO,
	                    Veiculo.VEI_PLACA,
                        OrdemServico.OSE_NUMERO,
	                    Modelo.VMO_DESCRICAO,
	                    Veiculo.VEI_KMATUAL,
                        Equipamento.EQP_DESCRICAO,
                        EquipamentoModelo.EMO_DESCRICAO,
	                    Equipamento.EQP_HORIMETRO;";

            string query = select + " WHERE 1 = 1 " + where + groupBy;

            return query;
        }

        private string ObterSelectServicosRelatorioAuditoriaOrdemServico(List<int> codigosOrdemServico)
        {
            string query = @"SELECT 
	                            OrdemServicoVeiculo.OSS_CODIGO as Codigo,
	                            OrdemServico.OSE_CODIGO as CodigoOrdemServico, 
	                            OrdemServico.OSE_NUMERO as Numero,
	                            Servico.SEV_DESCRICAO as Servico,
	                            Servico.SEV_OBSERVACAO as Observacao, 
	                            OrdemServico.OSE_QUILOMETRAGEM_VEICULO as QuilometragemOrdemServico, 
	                            OrdemServico.OSE_HORIMETRO as HorimetroOrdemServico, 
	                            OrdemServico.OSE_DATA_CRIACAO as Data
                            FROM
	                            T_FROTA_ORDEM_SERVICO OrdemServico
	                            left join T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoVeiculo on OrdemServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO
	                            left join T_FROTA_SERVICO_VEICULO Servico on Servico.SEV_CODIGO = OrdemServicoVeiculo.SEV_CODIGO";

            string groupBy = @" group by 
	                            OrdemServico.OSE_CODIGO,
	                            OrdemServicoVeiculo.OSS_CODIGO,
	                            OrdemServico.OSE_NUMERO,
	                            Servico.SEV_DESCRICAO,
	                            Servico.SEV_OBSERVACAO,
	                            OrdemServico.OSE_QUILOMETRAGEM_VEICULO,
	                            OrdemServico.OSE_HORIMETRO,
	                            OrdemServico.OSE_DATA_CRIACAO;";

            string where = $" WHERE 1 = 1 AND OrdemServico.OSE_CODIGO in ({string.Join(", ", codigosOrdemServico)})";

            return query + where + groupBy;
        }

        private string ObterSelectProdutosRelatorioAuditoriaOrdemServico(List<int> codigosOrdemServico)
        {
            string query = @"SELECT 
	                            Produto.PRO_COD_PRODUTO as CodigoProduto,
                                Produto.PRO_CODIGO as Codigo,
	                            Produto.PRO_DESCRICAO as NomeInsumo,
	                            OrdemServico.OSE_CODIGO as CodigoOrdemServico,
	                            OrdemServicoFechamentoProduto.OFP_QUANTIDADE_ORCADA as QuantidadePrevia,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_ORCADO as CustoPrevio,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_DOCUMENTO as CustoReal,
	                            OrdemServicoFechamentoProduto.OFP_QUANTIDADE_DOCUMENTO as QuantidadeReal,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_DOCUMENTO - OrdemServicoFechamentoProduto.OFP_VALOR_ORCADO as Diferenca,
	                            Produto.PRO_UNIDADE_MEDIDA as Unidade
                            FROM
	                            T_PRODUTO Produto
	                            JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamentoProduto on OrdemServicoFechamentoProduto.PRO_CODIGO = Produto.PRO_CODIGO
	                            JOIN T_FROTA_ORDEM_SERVICO OrdemServico on OrdemServico.OSE_CODIGO = OrdemServicoFechamentoProduto.OSE_CODIGO";
            string where = $" WHERE 1 = 1 AND OrdemServico.OSE_CODIGO in ({string.Join(", ", codigosOrdemServico)}) ";
            string groupBy = @"group by 
	                            Produto.PRO_COD_PRODUTO,
	                            Produto.PRO_DESCRICAO,
                                Produto.PRO_CODIGO,
	                            OrdemServicoFechamentoProduto.OFP_QUANTIDADE_ORCADA,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_ORCADO,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_DOCUMENTO,
	                            OrdemServicoFechamentoProduto.OFP_QUANTIDADE_DOCUMENTO,
	                            OrdemServicoFechamentoProduto.OFP_VALOR_ORCADO - OrdemServicoFechamentoProduto.OFP_VALOR_DOCUMENTO,
	                            OrdemServico.OSE_CODIGO,
	                            Produto.PRO_UNIDADE_MEDIDA;";

            return query + where + groupBy;
        }

        private string ObterSelectConsultaRelatorioDespesaDetalhadaOrdemServico(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(ref where, ref groupBy, ref joins, filtros);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        if (propOrdena == "MesAnoOS")
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + "AnoOS " + dirOrdena + ", MesOS " + dirOrdena;
                        else
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_FROTA_ORDEM_SERVICO OrdemServico ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "OrdemServico.OSE_CODIGO Codigo, ";
                        groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += @"OrdemServico.OSE_SITUACAO Situacao, ";
                        groupBy += "OrdemServico.OSE_SITUACAO, ";
                    }
                    break;
                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        if (!joins.Contains(" Tipo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_TIPO Tipo ON Tipo.FOT_CODIGO = OrdemServico.FOT_CODIGO";

                        select += "ISNULL(Tipo.FOT_DESCRICAO, 'NÃO INFORMADO') as Tipo, ";
                        groupBy += "Tipo.FOT_DESCRICAO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Placa, ";
                        groupBy += "Veiculo.VEI_PLACA, OrdemServico.VEI_CODIGO, ";
                    }
                    break;
                case "Equipamento":
                    if (!select.Contains(" Equipamento, "))
                    {
                        if (!joins.Contains(" Equipamento "))
                            joins += " LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO";

                        select += "Equipamento.EQP_DESCRICAO Equipamento, ";
                        groupBy += "Equipamento.EQP_DESCRICAO, ";
                    }
                    break;
                case "Ano":
                    if (!select.Contains(" Ano, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_ANO Ano, ";
                        groupBy += "Veiculo.VEI_ANO, OrdemServico.VEI_CODIGO, ";
                    }
                    break;
                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Modelo "))
                            joins += " LEFT OUTER JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO";

                        select += "Modelo.VMO_DESCRICAO Modelo, ";
                        groupBy += "Modelo.VMO_DESCRICAO, OrdemServico.VEI_CODIGO, ";
                    }
                    break;
                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Marca "))
                            joins += " LEFT OUTER JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = Veiculo.VMA_CODIGO";

                        select += "Marca.VMA_DESCRICAO Marca, ";
                        groupBy += "Marca.VMA_DESCRICAO, OrdemServico.VEI_CODIGO, ";
                    }
                    break;
                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao, "))
                    {
                        if (!joins.Contains(" LocalManutencao "))
                            joins += " LEFT OUTER JOIN T_CLIENTE LocalManutencao ON LocalManutencao.CLI_CGCCPF = OrdemServico.CLI_CGCCPF";

                        select += "LocalManutencao.CLI_NOME LocalManutencao, ";
                        groupBy += "LocalManutencao.CLI_NOME, ";
                    }
                    break;
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select += "OrdemServico.OSE_DATA_PROGRAMADA Data, ";
                        groupBy += "OrdemServico.OSE_DATA_PROGRAMADA, ";
                    }
                    break;
                case "MesAnoOS":
                    if (!select.Contains(" MesAnoOS, "))
                    {
                        select += "REPLICATE('0', 2 - LEN(CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)))) + CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)) + '-' + CAST(YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(4)) MesAnoOS, ";
                        select += "YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AnoOS, ";
                        select += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA) MesOS, ";
                        groupBy += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA), YEAR(OrdemServico.OSE_DATA_PROGRAMADA), ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "OrdemServico.OSE_NUMERO Numero, ";
                        groupBy += "OrdemServico.OSE_NUMERO, ";
                    }
                    break;
                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_PRODUTOS) ValorProdutos, ";
                    }
                    break;
                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_MAO_OBRA) ValorServicos, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_ORCADO) ValorTotal, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!select.Contains(" CodigoProduto, "))
                        {
                            select += @"SUBSTRING((select ', ' + produto.PRO_DESCRICAO from T_PRODUTO produto
			                        INNER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO ordemServicoFechamento ON ordemServicoFechamento.PRO_CODIGO = produto.PRO_CODIGO 
				                        WHERE ordemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO for xml path('')), 3,500) as Produto,";

                            if (!groupBy.Contains("OrdemServico.OSE_CODIGO"))
                                groupBy += "OrdemServico.OSE_CODIGO, ";
                        }
                        else
                        {                            
                            select += @"produto.PRO_DESCRICAO as Produto, ";
                            
                            if (!groupBy.Contains("Produto.PRO_DESCRICAO"))
                                groupBy += "Produto.PRO_DESCRICAO, ";
                        }
                    }
                    break;
                case "Servico":
                    if (!select.Contains(" Servico, "))
                    {
                        if (!joins.Contains(" OrdemServicoServicoVeiculo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoServicoVeiculo ON OrdemServicoServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Servico "))
                            joins += " LEFT OUTER JOIN T_FROTA_SERVICO_VEICULO Servico ON Servico.SEV_CODIGO = OrdemServicoServicoVeiculo.SEV_CODIGO";

                        select += @"Servico.SEV_DESCRICAO Servico,";
                        groupBy += "Servico.SEV_DESCRICAO, ";
                    }
                    break;
                case "ValorProdutosFechamento":
                    if (!select.Contains(" ValorProdutosFechamento, "))
                    {
                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO <> 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorProdutosFechamento, ";

                        SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico("Codigo", 0, ref select, ref groupBy, ref joins, count);
                    }
                    break;
                case "ValorProduto":
                    if (!select.Contains(" ValorProduto, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        select += @"ISNULL(OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, 0) ValorProduto,";
                        groupBy += "OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, ";
                    }
                    break;
                case "ValorServicosFechamento":
                    if (!select.Contains(" ValorServicosFechamento, "))
                    {
                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where PRO_CATEGORIA_PRODUTO = 9 AND OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorServicosFechamento, ";

                        SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico("Codigo", 0, ref select, ref groupBy, ref joins, count);
                    }
                    break;
                case "ValorServico":
                    if (!select.Contains(" ValorServico, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        select += @"ISNULL(OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, 0) ValorProduto,";
                        groupBy += "OrdemServicoFechamento.OFP_VALOR_DOCUMENTO, ";
                    }
                    break;
                case "ValorTotalFechamento":
                    if (!select.Contains(" ValorTotalFechamento, "))
                    {
                        select += @"(SELECT ISNULL(SUM(OFP_VALOR_DOCUMENTO), 0) FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO F
                                    JOIN T_PRODUTO P ON P.PRO_CODIGO = F.PRO_CODIGO
                                    where OSE_CODIGO = OrdemServico.OSE_CODIGO) ValorTotalFechamento, ";

                        SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico("Codigo", 0, ref select, ref groupBy, ref joins, count);
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "OrdemServico.OSE_OBSERVACAO Observacao, ";
                        groupBy += "OrdemServico.OSE_OBSERVACAO, ";
                    }
                    break;
                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" ModeloVeicular "))
                            joins += " LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO";

                        select += "ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ";
                        groupBy += "ModeloVeicular.MVC_DESCRICAO, OrdemServico.VEI_CODIGO, ";
                    }
                    break;
                case "KMExecucao":
                    if (!select.Contains(" KMExecucao, "))
                    {
                        select += "OrdemServico.OSE_QUILOMETRAGEM_VEICULO KMExecucao, ";
                        groupBy += "OrdemServico.OSE_QUILOMETRAGEM_VEICULO, ";
                    }
                    break;
                case "HorimetroExecucao":
                    if (!select.Contains(" HorimetroExecucao, "))
                    {
                        select += "OrdemServico.OSE_HORIMETRO HorimetroExecucao, ";
                        groupBy += "OrdemServico.OSE_HORIMETRO, ";
                    }
                    break;
                case "KMUltimoAbastecimento":
                    if (!select.Contains(" KMUltimoAbastecimento, "))
                    {
                        select += "(SELECT TOP(1) Abastecimento.ABA_KM FROM T_ABASTECIMENTO Abastecimento WHERE Abastecimento.VEI_CODIGO = OrdemServico.VEI_CODIGO order by Abastecimento.ABA_DATA desc) KMUltimoAbastecimento, ";
                    }
                    break;
                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        select += @"ISNULL(Produto.PRO_COD_PRODUTO, CAST(Produto.PRO_CODIGO AS VARCHAR(100))) CodigoProduto, ";
                        groupBy += "Produto.PRO_COD_PRODUTO, Produto.PRO_CODIGO, ";
                    }
                    break;
                case "QuantidadeProduto":
                    if (!select.Contains(" QuantidadeProduto, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        select += @"ISNULL(OrdemServicoFechamento.OFP_QUANTIDADE_DOCUMENTO, 0) QuantidadeProduto,";
                        groupBy += "OrdemServicoFechamento.OFP_QUANTIDADE_DOCUMENTO, ";
                    }
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        if (!joins.Contains(" CentroResultado "))
                            joins += " LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = OrdemServico.CRE_CODIGO";

                        select += "CentroResultado.CRE_DESCRICAO CentroResultado, ";
                        groupBy += "CentroResultado.CRE_DESCRICAO, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " LEFT OUTER JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = OrdemServico.FUN_OPERADOR";

                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;
                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(docEntrada.TDE_NUMERO_LONG AS NVARCHAR(160)) 
                            FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO prods
                            JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_DOCUMENTO docs on docs.OSE_CODIGO = prods.OSE_CODIGO 
                            JOIN T_TMS_DOCUMENTO_ENTRADA docEntrada on docEntrada.TDE_CODIGO = docs.TDE_CODIGO
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM itenEntrada on itenEntrada.TDE_CODIGO = docEntrada.TDE_CODIGO and itenEntrada.PRO_CODIGO = prods.PRO_CODIGO 
                            where docs.OSE_CODIGO = OrdemServico.OSE_CODIGO and prods.OSE_CODIGO = OrdemServico.OSE_CODIGO and prods.PRO_CODIGO = Produto.PRO_CODIGO
                            FOR XML PATH('')), 3, 1000) NotaFiscal, ";
                        groupBy += "Produto.PRO_CODIGO, ";

                        SetarSelectRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico("Codigo", 0, ref select, ref groupBy, ref joins, count);
                    }
                    break;

                case "LocalArmazenamento":
                    if (!select.Contains(" LocalArmazenamento, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";
                        if (!joins.Contains(" LocalArmazenamentoProduto "))
                            joins += " LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamentoProduto ON LocalArmazenamentoProduto.LAP_CODIGO = OrdemServicoFechamento.LAP_CODIGO";

                        select += @"LocalArmazenamentoProduto.LAP_DESCRICAO LocalArmazenamento,";
                        groupBy += "LocalArmazenamentoProduto.LAP_DESCRICAO, ";
                    }
                    break;
                case "Responsavel":
                    if (!select.Contains(" Responsavel, "))
                    {
                        if (!joins.Contains(" Responsavel "))
                            joins += " LEFT OUTER JOIN T_FUNCIONARIO Responsavel ON Responsavel.FUN_CODIGO = OrdemServico.FUN_RESPONSAVEL";

                        select += "Responsavel.FUN_NOME Responsavel, ";
                        groupBy += "Responsavel.FUN_NOME, ";
                    }
                    break;

                case "ObservacaoServicos":
                    if (!select.Contains(" ObservacaoServicos, "))
                    {
                        select += " ISNULL( ";
                        select += "     SUBSTRING(( ";
                        select += "         SELECT '; ' + ";
                        select += "             OrdemServicoServicoVeiculo.OSS_OBSERVACAO ";
                        select += "         FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoServicoVeiculo ";
                        select += "         WHERE OrdemServicoServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO AND TRIM(OSS_OBSERVACAO) <> '' ";
                        select += "         FOR XML PATH('') ";
                        select += "     ), 3, 1000), ";
                        select += " '') ObservacaoServicos, ";

                        if (!groupBy.Contains("OrdemServico.OSE_CODIGO"))
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" OrdemServicoFechamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                        if (!joins.Contains(" GrupoProdutoTMS "))
                            joins += " LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProdutoTMS ON GrupoProdutoTMS.GPR_CODIGO = Produto.GPR_CODIGO";

                        select += "GrupoProduto.GRP_DESCRICAO GrupoProduto,";
                        groupBy += "GrupoProduto.GRP_DESCRICAO, ";
                    }
                    break;

                case "ValorOrcadoEmProdutos":
                    if (!select.Contains(" ValorOrcadoEmProdutos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        if (!joins.Contains(" OrdemServicoServicoVeiculo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoServicoVeiculo ON OrdemServicoServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        if (!joins.Contains(" OrcamentoServico "))
                            joins += "LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO_SERVICO OrcamentoServico on OrcamentoServico.OSO_CODIGO = OrdemServicoOrcamento.OSO_CODIGO AND OrdemServicoServicoVeiculo.OSS_CODIGO = OrcamentoServico.OSS_CODIGO ";

                        select += "SUM(OrcamentoServico.OOS_VALOR_PRODUTOS) ValorOrcadoEmProdutos, ";

                        if (!groupBy.Contains("OrcamentoServico.OOS_VALOR_PRODUTOS"))
                            groupBy += "OrcamentoServico.OOS_VALOR_PRODUTOS, ";
                    }
                    break;

                case "ValorOrcadoEmServicos":
                    if (!select.Contains(" ValorOrcadoEmServicos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO ";

                        if (!joins.Contains(" OrdemServicoServicoVeiculo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO OrdemServicoServicoVeiculo ON OrdemServicoServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO ";

                        if (!joins.Contains(" OrcamentoServico "))
                            joins += "LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO_SERVICO OrcamentoServico on OrcamentoServico.OSO_CODIGO = OrdemServicoOrcamento.OSO_CODIGO AND OrdemServicoServicoVeiculo.OSS_CODIGO = OrcamentoServico.OSS_CODIGO ";

                        select += "SUM(OrcamentoServico.OOS_VALOR_MAO_OBRA) ValorOrcadoEmServicos, ";

                        if (!groupBy.Contains("OrcamentoServico.OOS_VALOR_MAO_OBRA"))
                            groupBy += "OrcamentoServico.OOS_VALOR_MAO_OBRA, ";
                    }
                    break;

                case "Mecanicos":
                    if (!select.Contains(" Mecanicos,"))
                    {
                        select += "SUBSTRING((SELECT ', ' + Mecanico.FUN_NOME " +
                            "FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                            "JOIN T_FUNCIONARIO Mecanico on Mecanico.FUN_CODIGO = TempoExecucao.FUN_MECANICO " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO " +
                            "FOR XML PATH('')), 3, 1000) AS Mecanicos, ";

                        groupBy += "OrdemServico.OSE_CODIGO, ";

                    }
                    break;

                case "TempoPrevisto":
                    if (!select.Contains(" TempoPrevisto,"))
                    {
                        select += "ISNULL((SELECT SUM(ServicoVeiculo.OSS_TEMPO_ESTIMADO) FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO), 0) TempoPrevisto, ";

                        groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;

                case "TempoExecutado":
                    if (!select.Contains(" TempoExecutado,"))
                    {
                        select += "ISNULL((SELECT SUM(TempoExecucao.OTE_TEMPO_EXECUTADO) FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                            "JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                            "WHERE ServicoVeiculo.OSE_CODIGO = OrdemServico.OSE_CODIGO), 0) TempoExecutado, ";

                        groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDespesaDetalhadaOrdemServico(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaDetalhadaOrdemServico filtros)
        {
            string pattern = "yyyy-MM-dd";

            if (filtros.TipoOrdemServico != null & filtros.TipoOrdemServico.Count > 0)
                where += $" AND OrdemServico.FOT_CODIGO in ({string.Join(",", filtros.TipoOrdemServico)}) ";

            if (filtros.Empresa > 0)
                where += " AND OrdemServico.EMP_CODIGO = '" + filtros.Empresa.ToString() + "' ";

            if (filtros.DataInicial != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) >= '" + filtros.DataInicial.ToString(pattern) + "' ";

            if (filtros.DataFinal != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) <= '" + filtros.DataFinal.ToString(pattern) + "'";

            if (filtros.NumeroOS > 0)
                where += " AND OrdemServico.OSE_NUMERO = " + filtros.NumeroOS;

            if (filtros.Veiculo != null && filtros.Veiculo.Count > 0)
                where += $" AND OrdemServico.VEI_CODIGO in ({string.Join(",", filtros.Veiculo)}) ";

            if (filtros.ModeloVeiculo != null & filtros.ModeloVeiculo.Count > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += $" AND Veiculo.VMO_CODIGO in ({string.Join(",", filtros.ModeloVeiculo)}) ";
            }

            if (filtros.LocalManutencao != null && filtros.LocalManutencao.Count > 0)
                where += $" AND OrdemServico.CLI_CGCCPF =  ({string.Join(",", filtros.LocalManutencao)}) ";

            if (filtros.Situacoes?.Count > 0)
                where += $" AND OrdemServico.OSE_SITUACAO IN ({ string.Join(", ", filtros.Situacoes.Select(o => o.ToString("D"))) })";

            if (filtros.MarcaVeiculo != null && filtros.MarcaVeiculo.Count > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += $" AND Veiculo.VMA_CODIGO in ({string.Join(",", filtros.MarcaVeiculo)}) ";
            }

            if (filtros.Servico != null & filtros.Servico.Count > 0)
            {
                where += @" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO 
                                                            WHERE SEV_CODIGO IN (" + string.Join(",", filtros.Servico) + "))";
            }

            if (filtros.Equipamento != null & filtros.Equipamento.Count > 0)
            {
                where += @" AND OrdemServico.EQP_CODIGO IN (" + string.Join(",", filtros.Equipamento) + ")";
            }

            if (filtros.Produto != null & filtros.Produto.Count > 0)
            {
                where += @" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO 
                                                            WHERE PRO_CODIGO IN (" + string.Join(",", filtros.Produto) + "))";
            }

            if (filtros.Responsavel > 0)
            {
                where += " AND OrdemServico.FUN_RESPONSAVEL = " + filtros.Responsavel;
            }

            if (filtros.CodigoGrupoProduto > 0)
            {
                where += " AND GrupoProduto.GPR_CODIGO = " + filtros.CodigoGrupoProduto;

                if (!joins.Contains(" OrdemServicoFechamento "))
                    joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO OrdemServicoFechamento ON OrdemServicoFechamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                if (!joins.Contains(" Produto "))
                    joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = OrdemServicoFechamento.PRO_CODIGO";

                if (!joins.Contains(" GrupoProdutoTMS "))
                    joins += " LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProdutoTMS ON GrupoProdutoTMS.GPR_CODIGO = Produto.GPR_CODIGO";
            }

            if (filtros.CentroResultado?.Count > 0)
                where += $" AND OrdemServico.CRE_CODIGO IN ({string.Join(",", filtros.CentroResultado)}) ";

            if (filtros.Mecanicos?.Count > 0)
            {
                where += $" AND OrdemServico.OSE_CODIGO IN (SELECT ServicoVeiculo.OSE_CODIGO FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO ServicoVeiculo " +
                    $" JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO_TEMPO_EXECUCAO TempoExecucao on TempoExecucao.OSS_CODIGO = ServicoVeiculo.OSS_CODIGO " +
                    $" WHERE TempoExecucao.FUN_MECANICO IN ('{string.Join(", ", filtros.Mecanicos)}')) "; // SQL-INJECTION-SAFE
            }

        }

        #endregion

        #region Relatório Ordem Serviço por Mecânico

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico> ConsultarRelatorioOrdemServicoPorMecanico(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioOrdemServicoPorMecanico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOrdemServicoPorMecanico = new ConsultaOrdemServicoPorMecanico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaOrdemServicoPorMecanico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico)));

            return consultaOrdemServicoPorMecanico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico>> ConsultarRelatorioOrdemServicoPorMecanicoAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioOrdemServicoPorMecanico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOrdemServicoPorMecanico = new ConsultaOrdemServicoPorMecanico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaOrdemServicoPorMecanico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico)));

            return await consultaOrdemServicoPorMecanico.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frotas.OrdemServicoPorMecanico>();
        }

        public int ContarConsultaRelatorioOrdemServicoPorMecanico(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioOrdemServicoPorMecanico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaOrdemServicoPorMecanico = new ConsultaOrdemServicoPorMecanico().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaOrdemServicoPorMecanico.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Relatório Despesa Ordem Serviço Produto

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto> ConsultarRelatorioDespesaOrdemServicoProduto(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServicoProduto filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServicoProduto(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto>> ConsultarRelatorioDespesaOrdemServicoProdutoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServicoProduto filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServicoProduto(filtros, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServicoProduto>();
        }

        public int ContarConsultaRelatorioDespesaOrdemServicoProduto(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServicoProduto filtros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioDespesaOrdemServicoProduto(filtros, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaOrdemServicoProduto(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServicoProduto filtros, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDespesaOrdemServicoProduto(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioDespesaOrdemServicoProduto(ref where, ref groupBy, ref joins, filtros);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDespesaOrdemServicoProduto(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        if (propOrdena == "MesAnoOS")
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + "AnoOS " + dirOrdena + ", MesOS " + dirOrdena;
                        else
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_FROTA_ORDEM_SERVICO OrdemServico ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioDespesaOrdemServicoProduto(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "OrdemServico.OSE_CODIGO Codigo, ";
                        groupBy += "OrdemServico.OSE_CODIGO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += @"OrdemServico.OSE_SITUACAO Situacao, ";
                        groupBy += "OrdemServico.OSE_SITUACAO, ";
                    }
                    break;
                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        if (!joins.Contains(" Tipo "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_TIPO Tipo ON Tipo.FOT_CODIGO = OrdemServico.FOT_CODIGO";

                        select += "ISNULL(Tipo.FOT_DESCRICAO, 'NÃO INFORMADO') as Tipo, ";
                        groupBy += "Tipo.FOT_DESCRICAO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Placa, ";
                        groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "Equipamento":
                    if (!select.Contains(" Equipamento, "))
                    {
                        if (!joins.Contains(" Equipamento "))
                            joins += " LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = OrdemServico.EQP_CODIGO";

                        select += "Equipamento.EQP_DESCRICAO Equipamento, ";
                        groupBy += "Equipamento.EQP_DESCRICAO, ";
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" GrupoProdutoTMS "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProdutoTMS ON GrupoProdutoTMS.GPR_CODIGO = OrdemServico.EQP_CODIGO";

                        select += "GrupoProdutoTMS.GRP_DESCRICAO GrupoProduto, ";
                        groupBy += "GrupoProdutoTMS.GRP_DESCRICAO, ";
                    }
                    break;
                case "Ano":
                    if (!select.Contains(" Ano, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        select += "Veiculo.VEI_ANO Ano, ";
                        groupBy += "Veiculo.VEI_ANO, ";
                    }
                    break;
                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Modelo "))
                            joins += " LEFT JOIN T_VEICULO_MODELO Modelo ON Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO";

                        select += "Modelo.VMO_DESCRICAO Modelo, ";
                        groupBy += "Modelo.VMO_DESCRICAO, ";
                    }
                    break;
                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                        if (!joins.Contains(" Marca "))
                            joins += " LEFT JOIN T_VEICULO_MARCA Marca ON Marca.VMA_CODIGO = Veiculo.VMA_CODIGO";

                        select += "Marca.VMA_DESCRICAO Marca, ";
                        groupBy += "Marca.VMA_DESCRICAO, ";
                    }
                    break;
                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao, "))
                    {
                        if (!joins.Contains(" LocalManutencao "))
                            joins += " LEFT JOIN T_CLIENTE LocalManutencao ON LocalManutencao.CLI_CGCCPF = OrdemServico.CLI_CGCCPF";

                        select += "LocalManutencao.CLI_NOME LocalManutencao, ";
                        groupBy += "LocalManutencao.CLI_NOME, ";
                    }
                    break;
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select += "OrdemServico.OSE_DATA_PROGRAMADA Data, ";
                        groupBy += "OrdemServico.OSE_DATA_PROGRAMADA, ";
                    }
                    break;
                case "MesAnoOS":
                    if (!select.Contains(" MesAnoOS, "))
                    {
                        select += "REPLICATE('0', 2 - LEN(CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)))) + CAST(MONTH(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(2)) + '-' + CAST(YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AS VARCHAR(4)) MesAnoOS, ";
                        select += "YEAR(OrdemServico.OSE_DATA_PROGRAMADA) AnoOS, ";
                        select += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA) MesOS, ";
                        groupBy += "MONTH(OrdemServico.OSE_DATA_PROGRAMADA), YEAR(OrdemServico.OSE_DATA_PROGRAMADA), ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "OrdemServico.OSE_NUMERO Numero, ";
                        groupBy += "OrdemServico.OSE_NUMERO, ";
                    }
                    break;
                case "ValorProdutos":
                    if (!select.Contains(" ValorProdutos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_PRODUTOS) ValorProdutos, ";
                    }
                    break;
                case "ValorServicos":
                    if (!select.Contains(" ValorServicos, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_MAO_OBRA) ValorServicos, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        if (!joins.Contains(" OrdemServicoOrcamento "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO_ORCAMENTO OrdemServicoOrcamento ON OrdemServicoOrcamento.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += "SUM(OrdemServicoOrcamento.OSO_VALOR_TOTAL_ORCADO) ValorTotal, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        select += "Produtos.PRO_DESCRICAO Produto, ";
                        groupBy += "Produtos.PRO_DESCRICAO, ";

                        if (!joins.Contains(" FechamentoProduto "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO FechamentoProduto ON FechamentoProduto.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        if (!joins.Contains(" Produtos "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produtos ON Produtos.PRO_CODIGO = FechamentoProduto.PRO_CODIGO";
                    }
                    break;
                case "ValorProdutosFechamento":
                    if (!select.Contains(" ValorProdutosFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        if (!joins.Contains(" FechamentoProduto "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO FechamentoProduto ON FechamentoProduto.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += @" ISNULL(FechamentoProduto.OFP_VALOR_UNITARIO, 0)  ValorProdutosFechamento, ";

                        if (!groupBy.Contains("FechamentoProduto.OFP_VALOR_UNITARIO"))
                        {
                            groupBy += "FechamentoProduto.OFP_VALOR_UNITARIO, ";
                        }
                    }
                    break;
                case "ValorServicosFechamento":
                    if (!select.Contains(" ValorServicosFechamento, "))
                    {
                        select += @"0.0  ValorServicosFechamento, ";
                    }
                    break;
                case "ValorTotalFechamento":
                    if (!select.Contains(" ValorTotalFechamento, "))
                    {
                        if (!select.Contains(" Codigo, "))
                        {
                            select += "OrdemServico.OSE_CODIGO Codigo, ";
                            groupBy += "OrdemServico.OSE_CODIGO, ";
                        }

                        if (!joins.Contains(" FechamentoProduto "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO FechamentoProduto ON FechamentoProduto.OSE_CODIGO = OrdemServico.OSE_CODIGO";

                        select += @" (ISNULL(FechamentoProduto.OFP_QUANTIDADE_DOCUMENTO, 0) * FechamentoProduto.OFP_VALOR_UNITARIO) ValorTotalFechamento, ";

                        if (!groupBy.Contains("FechamentoProduto.OFP_QUANTIDADE_DOCUMENTO") )
                        {
                            groupBy += "FechamentoProduto.OFP_QUANTIDADE_DOCUMENTO, ";
                        }

                        if (!groupBy.Contains("FechamentoProduto.OFP_VALOR_UNITARIO"))
                        {
                            groupBy += "FechamentoProduto.OFP_VALOR_UNITARIO, ";
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDespesaOrdemServicoProduto(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServicoProduto filtros)
        {
            string pattern = "yyyy-MM-dd";

            if (filtros.TipoOrdemServico != null & filtros.TipoOrdemServico.Count > 0)
                where += $" AND OrdemServico.FOT_CODIGO in ({string.Join(",", filtros.TipoOrdemServico)}) ";

            if (filtros.Empresa > 0)
                where += " AND OrdemServico.EMP_CODIGO = '" + filtros.Empresa.ToString() + "' ";

            if (filtros.DataInicial != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) >= '" + filtros.DataInicial.ToString(pattern) + "' ";

            if (filtros.DataFinal != DateTime.MinValue)
                where += " AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) <= '" + filtros.DataFinal.ToString(pattern) + "'";

            if (filtros.NumeroOS > 0)
                where += " AND OrdemServico.OSE_NUMERO = " + filtros.NumeroOS;

            if (filtros.Veiculo > 0)
                where += " AND OrdemServico.VEI_CODIGO = " + filtros.Veiculo;

            if (filtros.ModeloVeiculo != null & filtros.ModeloVeiculo.Count > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += $" AND Veiculo.VMO_CODIGO in ({string.Join(",", filtros.ModeloVeiculo)}) ";
            }

            if (filtros.LocalManutencao > 0)
                where += " AND OrdemServico.CLI_CGCCPF = " + filtros.LocalManutencao;

            if (filtros.Situacoes?.Count > 0)
                where += $" AND OrdemServico.OSE_SITUACAO IN ({ string.Join(", ", filtros.Situacoes.Select(o => o.ToString("D"))) })";

            if (filtros.MarcaVeiculo > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = OrdemServico.VEI_CODIGO";

                where += " AND Veiculo.VMA_CODIGO = " + filtros.MarcaVeiculo;
            }

            if (filtros.Servico != null & filtros.Servico.Count > 0)
            {
                where += @" AND OrdemServico.OSE_CODIGO IN (SELECT DISTINCT OSE_CODIGO 
                                                            FROM T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO 
                                                            WHERE SEV_CODIGO IN (" + string.Join(",", filtros.Servico) + "))";
            }

            if (filtros.Equipamento != null & filtros.Equipamento.Count > 0)
            {
                where += @" AND OrdemServico.EQP_CODIGO IN (" + string.Join(",", filtros.Equipamento) + ")";
            }

            if (filtros.Produto != null & filtros.Produto.Count > 0)
            {
                where += @" AND Produtos.PRO_CODIGO IN (" + string.Join(",", filtros.Produto) + ")";
            }
            if (filtros.GrupoProduto != null & filtros.GrupoProduto.Count > 0)
            {
                where += @" AND OrdemServico.EQP_CODIGO IN (SELECT DISTINCT GPR_CODIGO 
                                                            FROM T_GRUPO_PRODUTO_TMS
                                                            WHERE GPR_CODIGO IN (" + string.Join(",", filtros.GrupoProduto) + "))";
            }

        }
        #endregion
    }
}