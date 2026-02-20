using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{

    sealed class ConsultaMonitoramentoPosicaoFrotaRastreamento : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento>
    {

        #region Atributos privados

        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        #endregion

        #region Construtores

        public ConsultaMonitoramentoPosicaoFrotaRastreamento() : base(tabela: "T_MONITORAMENTO as Monitoramento") { }

        public List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ExtrairMonitoramentoPosicaoFrotaRastreamento(UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            var consultaMonitoramentoPosicaoFrotaRastreamento = this.ObterSqlPesquisa(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta(), propriedades).CriarSQLQuery(unitOfWork.Sessao);
            consultaMonitoramentoPosicaoFrotaRastreamento.SetTimeout(300).SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento)));

            IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento> monitoramentos = consultaMonitoramentoPosicaoFrotaRastreamento.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento>();
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoes = ProcessarMonitoramentoPosicaoFrotaRastreamento(unitOfWork, monitoramentos, filtrosPesquisa, parametrosConsulta, configuracao);

            return posicoes;
        }

        public int ContarMonitoramentoPosicaoFrotaRastreamento(UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = this.ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(unitOfWork.Sessao);
            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados SQL

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("join t_carga Carga on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Monitoramento.VEI_CODIGO ");
        }

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtroPesquisa)
        {

            if (!select.Contains(" Codigo, "))
            {
                select.Append("Monitoramento.MON_CODIGO as Codigo, ");
            }

            if (!select.Contains(" MonitoramentoStatus, "))
            {
                select.Append("Monitoramento.MON_STATUS as MonitoramentoStatus, ");
            }

            if (!select.Contains(" MonitoramentoDataInicio, "))
            {
                select.Append("Monitoramento.MON_DATA_INICIO as MonitoramentoDataInicio, ");
            }

            if (!select.Contains(" MonitoramentoDataFim, "))
            {
                select.Append("Monitoramento.MON_DATA_FIM as MonitoramentoDataFim, ");
            }

            if (!select.Contains(" CargaCodigo, "))
            {
                select.Append("Carga.CAR_CODIGO as CargaCodigo, ");
            }

            if (!select.Contains(" CargaCodigoEmbarcador, "))
            {
                select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CargaCodigoEmbarcador, ");
            }

            if (!select.Contains(" CargaCodigoClienteOrigem, "))
            {
                select.Append(@"isnull((
		            select 
			            top 1 CargaPedido.CLI_CODIGO_EXPEDIDOR
		            from
			            t_carga_pedido CargaPedido
		            where 
			            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
	            ), (
		            select 
			            top 1 Pedido.CLI_CODIGO_REMETENTE
		            from
			            t_carga_pedido CargaPedido
		            join
			            t_pedido Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		            where 
			            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
	            )) as CargaCodigoClienteOrigem, ");
            }

            if (!select.Contains(" VeiculoCodigo, "))
            {
                select.Append("Veiculo.VEI_CODIGO as VeiculoCodigo, ");
            }

            if (!select.Contains(" VeiculoPlaca, "))
            {
                select.Append("Veiculo.VEI_PLACA as VeiculoPlaca, ");
            }

            if (!select.Contains(" VeiculoNumeroEquipamentoRastreador, "))
            {
                select.Append("Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR as VeiculoNumeroEquipamentoRastreador, ");
            }

            if (!select.Contains(" EmpresaCodigo, "))
            {
                select.Append("Empresa.EMP_CODIGO as EmpresaCodigo, ");
            }

            if (!select.Contains(" EmpresaCNPJ, "))
            {
                select.Append("Empresa.EMP_CNPJ as EmpresaCNPJ, ");
            }

            if (!select.Contains(" EmpresaNomeFantasia, "))
            {
                select.Append("Empresa.EMP_FANTASIA as EmpresaNomeFantasia, ");
            }

            if (!select.Contains(" VeiculosVinculados, "))
            {
                select.Append(@"(
                    SELECT ';' + convert(nvarchar, Veiculo1.VEI_CODIGO) + ',' + Veiculo1.VEI_PLACA + ',' + Veiculo1.VEI_NUMERO_EQUIPAMENTO_RASTREADOR AS[text()]
                    FROM t_carga_veiculos_vinculados VeiculosVinculados
                    JOIN t_veiculo Veiculo1 ON Veiculo1.VEI_CODIGO = VeiculosVinculados.VEI_CODIGO and Veiculo1.VEI_POSSUI_RASTREADOR = 1
                    WHERE VeiculosVinculados.CAR_CODIGO = Carga.CAR_CODIGO
                    FOR XML PATH('')
                ) VeiculosVinculados, ");
            }

        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCarga(joins);
            SetarJoinsVeiculo(joins);
            SetarJoinEmpresa(joins);

            where.Append($" and Monitoramento.MON_STATUS in (1,2)");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                where.Append($" and Monitoramento.MON_DATA_INICIO >= convert(datetime, '{filtrosPesquisa.DataInicial.ToString(_dateFormat)}', 102)");
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                where.Append($" and (Monitoramento.MON_DATA_FIM is null or Monitoramento.MON_DATA_FIM <= convert(datetime, '{filtrosPesquisa.DataFinal.ToString(_dateFormat)}', 102))");
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }
        }

        #endregion

        #region Métodos Privados SQL

        private IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> BuscarPosicoesDosVeiculosNoPeriodo(UnitOfWork unitOfWork, List<int> codigosVeiculos, DateTime dataInicio, DateTime dataFim)
        {
            string sql = $@"
                select
	                Posicao.POS_CODIGO as Codigo,
	                Posicao.POS_DATA_VEICULO as DataVeiculo,
                    case
		                when Posicao.POS_EM_ALVO = 1 then (
			                SELECT ',' + convert(varchar, convert(bigint, PosicaoAlvo.CLI_CGCCPF)) AS [text()]  
			                FROM T_POSICAO_ALVO PosicaoAlvo
			                WHERE PosicaoAlvo.POS_CODIGO = Posicao.POS_CODIGO 
			                FOR XML PATH ('')
		                )
		                else null
	                end CodigosClientesEmAlvo,
                    Posicao.VEI_CODIGO CodigoVeiculo
                from
	                T_POSICAO as Posicao
                where
	                Posicao.VEI_CODIGO in ({string.Join(",", codigosVeiculos)})";

            if (dataInicio != DateTime.MinValue)
            {
                sql += $@" 
                    and Posicao.POS_DATA_VEICULO >= convert(datetime, '{dataInicio.ToString(_dateFormat)}', 102)";
            }

            if (dataFim != DateTime.MinValue)
            {
                sql += $@"
                    and Posicao.POS_DATA_VEICULO <= convert(datetime, '{dataFim.ToString(_dateFormat)}', 102)";
            }

            sql += $@"
                order by
                    Posicao.POS_DATA_VEICULO";

            var query = unitOfWork.Sessao.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento)));
            return query.List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>();

        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ProcessarMonitoramentoPosicaoFrotaRastreamento(UnitOfWork unitOfWork, IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento> monitoramentos, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoFrotaRastreamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesRetorno = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>();
            int total = monitoramentos.Count;
            if (total > 0)
            {

                // Extrai os veículos distintos
                List<int> codigosVeiculos = ExtrairCodigosVeiculosDistintos(monitoramentos);

                // Consulta as posições dos veículos
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoes = BuscarPosicoesDosVeiculosNoPeriodo(unitOfWork, codigosVeiculos, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal);

                for (int i = 0; i < total; i++)
                {

                    // Tração
                    List<Dominio.ObjetosDeValor.Veiculo> veiculos = new List<Dominio.ObjetosDeValor.Veiculo>();
                    veiculos.Add(new Dominio.ObjetosDeValor.Veiculo
                    {
                        Codigo = monitoramentos[i].VeiculoCodigo,
                        Placa = monitoramentos[i].VeiculoPlaca,
                        NumeroEquipamentoRastreador = monitoramentos[i].VeiculoNumeroEquipamentoRastreador
                    });

                    // Reboques
                    List<Dominio.ObjetosDeValor.Veiculo> reboques = ExtrairVeiculos(monitoramentos[i].VeiculosVinculados);
                    veiculos = veiculos.Concat(reboques).ToList();

                    int totalVeiculos = veiculos.Count;
                    if (totalVeiculos > 0)
                    {
                        // Entregas da carga
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(monitoramentos[i].CargaCodigo);
                        List<double> codigosClientes = (
                            from entrega in cargaEntregas
                            select entrega.Cliente?.CPF_CNPJ ?? 0
                            ).ToList();

                        // Se o cliente de origem não fazer parte das entrgas, adiciona no início da lista
                        if (!codigosClientes.Contains(monitoramentos[i].CargaCodigoClienteOrigem)) codigosClientes.Insert(0, monitoramentos[i].CargaCodigoClienteOrigem);

                        for (int j = 0; j < totalVeiculos; j++)
                        {

                            // Extrai apenas as posições do veículo
                            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesVeiculo = ExtrairPosicoesDoVeiculo(veiculos[j].Codigo, posicoes);
                            if (posicoesVeiculo.Count > 0)
                            {

                                // Extrai deslocamentos e as permanências entre/em cada um dos clientes destinos da carga
                                List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesDeslocamentos = ExtraiDeslocamentos(unitOfWork, monitoramentos[i], codigosClientes, posicoesVeiculo, veiculos[j], configuracao);
                                posicoesRetorno = posicoesRetorno.Concat(posicoesDeslocamentos).ToList();
                            }
                        }
                    }

                }
            }

            Ordenar(posicoesRetorno, parametrosConsulta);

            return posicoesRetorno;
        }

        private List<int> ExtrairCodigosVeiculosDistintos(IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento> monitoramentos)
        {
            List<int> codigosVeiculos = new List<int>();
            int total = monitoramentos.Count;
            for (int i = 0; i < total; i++)
            {

                // Veículo tração
                if (!codigosVeiculos.Contains(monitoramentos[i].VeiculoCodigo))
                {
                    codigosVeiculos.Add(monitoramentos[i].VeiculoCodigo);
                }

                // Reboques
                List<Dominio.ObjetosDeValor.Veiculo> reboques = ExtrairVeiculos(monitoramentos[i].VeiculosVinculados);
                int totalVeiculos = reboques?.Count ?? 0;
                for (int j = 0; j < totalVeiculos; j++)
                {
                    if (!codigosVeiculos.Contains(reboques[j].Codigo))
                    {
                        codigosVeiculos.Add(reboques[j].Codigo);
                    }
                }

            }
            return codigosVeiculos;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ExtrairPosicoesDoVeiculo(int codigoVeiculo, IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoes)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesVeiculo = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>();
            int total = posicoes?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (codigoVeiculo == posicoes[i].CodigoVeiculo)
                {
                    posicoesVeiculo.Add(posicoes[i]);
                }
            }
            posicoesVeiculo.Sort((x, y) => x.DataVeiculo.CompareTo(y.DataVeiculo));
            return posicoesVeiculo;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> ExtraiDeslocamentos(UnitOfWork unitOfWork, Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoPosicaoFrotaRastreamento monitoramento, List<double> codigosClientes, List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesVeiculo, Dominio.ObjetosDeValor.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesDeslocamentos = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>();

            List<double> codigosTodosClientesAlvos = new List<double>();
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesVeiculoPermanencias = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento>();

            int totalPosicoesVeiculo = posicoesVeiculo.Count;
            for (int i = 0; i < totalPosicoesVeiculo; i++)
            {
                List<double> codigosClientesEntregasEmAlvo = ExtrairCodigosClientes(posicoesVeiculo[i].CodigosClientesEmAlvo, codigosClientes);
                int totalClientesEmAlvo = codigosClientesEntregasEmAlvo.Count;
                if (totalClientesEmAlvo > 0)
                {
                    codigosTodosClientesAlvos = codigosTodosClientesAlvos.Concat(codigosClientesEntregasEmAlvo).ToList();

                    // Início da permanência no alvo
                    for (int j = 0; j < totalClientesEmAlvo; j++)
                    {
                        if (!PossuiPermanenciaEmAberto(codigosClientesEntregasEmAlvo[j], posicoesVeiculoPermanencias))
                        {
                            posicoesVeiculoPermanencias.Add(new Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento
                            {
                                Codigo = posicoesVeiculo[i].Codigo,
                                Placa = veiculo.Placa,
                                Rastreador = veiculo.NumeroEquipamentoRastreador,
                                CodigoClienteEmAlvo = codigosClientesEntregasEmAlvo[j],
                                Entrada = posicoesVeiculo[i].DataVeiculo.FloorMinute(),
                            });
                        }
                    }
                }

                // Fim das permanêncioas no alvo quando saiu do alvo
                int totalPosicoesVeiculoPermanenciasPendentes = posicoesVeiculoPermanencias.Count;
                for (int j = 0; j < totalPosicoesVeiculoPermanenciasPendentes; j++)
                {
                    // Está pendente e não está mais no cliente da permanência pendente
                    if (posicoesVeiculoPermanencias[j].Saida == DateTime.MinValue && !codigosClientesEntregasEmAlvo.Contains(posicoesVeiculoPermanencias[j].CodigoClienteEmAlvo))
                    {
                        posicoesVeiculoPermanencias[j].Saida = posicoesVeiculo[i].DataVeiculo.FloorMinute();
                        posicoesVeiculoPermanencias[j].TempoEstadia = posicoesVeiculoPermanencias[j].Saida - posicoesVeiculoPermanencias[j].Entrada;
                    }

                }
            }

            // Consulta os nomes de todos os clientes de uma única vez
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.ObjetosDeValor.Cliente> clientesNomes = repCliente.BuscarNomesClientes(codigosTodosClientesAlvos.Distinct().ToList());

            // Calcula o tempo de trânsito entre as permanências nos cliente
            int totalPosicoesVeiculoPermanencias = posicoesVeiculoPermanencias.Count;
            for (int i = 0; i < totalPosicoesVeiculoPermanencias; i++)
            {

                Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento posicaoVeiculoPermanencia = new Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento();
                posicaoVeiculoPermanencia.CodigoCargaEmbarcador = monitoramento.CargaCodigoEmbarcador;
                posicaoVeiculoPermanencia.Codigo = posicoesVeiculoPermanencias[i].Codigo;
                posicaoVeiculoPermanencia.Placa = posicoesVeiculoPermanencias[i].Placa;
                posicaoVeiculoPermanencia.Rastreador = posicoesVeiculoPermanencias[i].Rastreador;
                posicaoVeiculoPermanencia.TempoEstadia = posicoesVeiculoPermanencias[i].TempoEstadia;
                posicaoVeiculoPermanencia.Entrada = posicoesVeiculoPermanencias[i].Entrada;
                posicaoVeiculoPermanencia.Saida = posicoesVeiculoPermanencias[i].Saida;
                posicaoVeiculoPermanencia.Origem = BuscarNomeDoCliente(posicoesVeiculoPermanencias[i].CodigoClienteEmAlvo, clientesNomes, configuracao);

                // Há permanência posterior?
                Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento posicaoVeiculoPermanenciaPosterior = posicoesVeiculoPermanencias.ElementAtOrDefault(i + 1);
                if (posicaoVeiculoPermanenciaPosterior != null)
                {
                    posicaoVeiculoPermanencia.ChegadaDestino = posicaoVeiculoPermanenciaPosterior.Entrada;
                    posicaoVeiculoPermanencia.Destino = BuscarNomeDoCliente(posicaoVeiculoPermanenciaPosterior.CodigoClienteEmAlvo, clientesNomes, configuracao);
                    posicaoVeiculoPermanencia.TempoTransito = (posicaoVeiculoPermanencia.ChegadaDestino != DateTime.MinValue && posicaoVeiculoPermanencia.Saida != DateTime.MinValue && posicaoVeiculoPermanencia.ChegadaDestino > posicaoVeiculoPermanencia.Saida) ?
                        posicaoVeiculoPermanencia.ChegadaDestino - posicaoVeiculoPermanencia.Saida : new TimeSpan();
                }

                posicoesDeslocamentos.Add(posicaoVeiculoPermanencia);
            }

            return posicoesDeslocamentos;
        }

        private void Ordenar(List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoes, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
            {
                string direcao = (!string.IsNullOrWhiteSpace(parametrosConsulta.DirecaoOrdenar)) ? parametrosConsulta.DirecaoOrdenar.ToLower().Trim() : "asc";
                switch (parametrosConsulta.PropriedadeOrdenar)
                {
                    case "Placa":
                        if (direcao == "asc") posicoes.Sort((x, y) => String.Compare(x.Placa, y.Placa)); else posicoes.Sort((x, y) => String.Compare(y.Placa, x.Placa));
                        break;
                    case "Rastreador":
                        if (direcao == "asc") posicoes.Sort((x, y) => String.Compare(x.Rastreador, y.Rastreador)); else posicoes.Sort((x, y) => String.Compare(y.Rastreador, x.Rastreador));
                        break;
                    case "CodigoCargaEmbarcador":
                        if (direcao == "asc") posicoes.Sort((x, y) => String.Compare(x.CodigoCargaEmbarcador, y.CodigoCargaEmbarcador)); else posicoes.Sort((x, y) => String.Compare(y.CodigoCargaEmbarcador, x.CodigoCargaEmbarcador));
                        break;
                    case "TempoEstadiaFormatado":
                        if (direcao == "asc") posicoes.Sort((x, y) => TimeSpan.Compare(x.TempoEstadia, y.TempoEstadia)); else posicoes.Sort((x, y) => TimeSpan.Compare(y.TempoEstadia, x.TempoEstadia));
                        break;
                    case "Origem":
                        if (direcao == "asc") posicoes.Sort((x, y) => String.Compare(x.Origem, y.Origem)); else posicoes.Sort((x, y) => String.Compare(y.Origem, x.Origem));
                        break;
                    case "Destino":
                        if (direcao == "asc") posicoes.Sort((x, y) => String.Compare(x.Destino, y.Destino)); else posicoes.Sort((x, y) => String.Compare(y.Destino, x.Destino));
                        break;
                    case "EntradaFormatado":
                        if (direcao == "asc") posicoes.Sort((x, y) => DateTime.Compare(x.Entrada, y.Entrada)); else posicoes.Sort((x, y) => DateTime.Compare(y.Entrada, x.Entrada));
                        break;
                    case "SaidaFormatado":
                        if (direcao == "asc") posicoes.Sort((x, y) => DateTime.Compare(x.Saida, y.Saida)); else posicoes.Sort((x, y) => DateTime.Compare(y.Saida, x.Saida));
                        break;
                    case "ChegadaDestinoFormatado":
                        if (direcao == "asc") posicoes.Sort((x, y) => DateTime.Compare(x.ChegadaDestino, y.ChegadaDestino)); else posicoes.Sort((x, y) => DateTime.Compare(y.ChegadaDestino, x.ChegadaDestino));
                        break;
                    case "TempoTransitoFormatado":
                        if (direcao == "asc") posicoes.Sort((x, y) => TimeSpan.Compare(x.TempoTransito, y.TempoTransito)); else posicoes.Sort((x, y) => TimeSpan.Compare(y.TempoTransito, x.TempoTransito));
                        break;
                }
            }
        }

        private List<double> ExtrairCodigosClientes(string codigos, List<double> codigosClientesEntregas)
        {
            List<double> codigosClientes = new List<double>();
            string[] codigosClientesEmAlvo = (!string.IsNullOrEmpty(codigos)) ? codigos.Split(',') : null;
            int total = codigosClientesEmAlvo?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(codigosClientesEmAlvo[i]))
                {
                    double codigoCliente = double.Parse(codigosClientesEmAlvo[i]);
                    if (codigosClientesEntregas.Contains(codigoCliente))
                    {
                        codigosClientes.Add(codigoCliente);
                    }
                }
            }
            return codigosClientes;
        }

        private bool PossuiPermanenciaEmAberto(double codigoCliente, List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PosicaoFrotaRastreamento> posicoesVeiculoPermanencias)
        {
            int total = posicoesVeiculoPermanencias.Count;
            for (int i = 0; i < total; i++)
            {
                if (posicoesVeiculoPermanencias[i].CodigoClienteEmAlvo == codigoCliente && posicoesVeiculoPermanencias[i].Saida == DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }

        private string BuscarNomeDoCliente(double codigoCliente, List<Dominio.ObjetosDeValor.Cliente> clientesNomes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            int total = clientesNomes.Count;
            for (int i = 0; i < total; i++)
            {
                if (clientesNomes[i].Codigo == codigoCliente)
                {
                    if (configuracao.ApresentarCodigoIntegracaoComNomeFantasiaCliente)
                    {
                        return
                            (!string.IsNullOrWhiteSpace(clientesNomes[i].CodigoIntegracao) ? clientesNomes[i].CodigoIntegracao + "-" : "") +
                            (!string.IsNullOrWhiteSpace(clientesNomes[i].NomeFantasia) ? clientesNomes[i].NomeFantasia : clientesNomes[i].Nome) +
                            (!string.IsNullOrWhiteSpace(clientesNomes[i].Cidade) ? " (" + clientesNomes[i].Cidade : "") + (!string.IsNullOrWhiteSpace(clientesNomes[i].UF) ? "/" + clientesNomes[i].UF : "") + ")";
                    }
                    else
                    {
                        return clientesNomes[i].Nome;
                    }
                }
            }
            return string.Empty;
        }

        private List<Dominio.ObjetosDeValor.Veiculo> ExtrairVeiculos(string veiculosString)
        {
            List<Dominio.ObjetosDeValor.Veiculo> veiculos = new List<Dominio.ObjetosDeValor.Veiculo>();
            string[] veiculosVinculados = (!string.IsNullOrEmpty(veiculosString)) ? veiculosString.Split(';') : null;
            int totalVeiculos = veiculosVinculados?.Length ?? 0;
            for (int j = 0; j < totalVeiculos; j++)
            {
                string[] partes = (!string.IsNullOrEmpty(veiculosVinculados[j])) ? veiculosVinculados[j].Split(',') : null;
                int totalPartes = partes?.Length ?? 0;
                if (totalPartes == 3)
                {
                    veiculos.Add(new Dominio.ObjetosDeValor.Veiculo()
                    {
                        Codigo = Int32.Parse(partes[0]),
                        Placa = partes[1].Trim(),
                        NumeroEquipamentoRastreador = partes[2].Trim()
                    });
                }
            }
            return veiculos;
        }

        #endregion

    }

}
