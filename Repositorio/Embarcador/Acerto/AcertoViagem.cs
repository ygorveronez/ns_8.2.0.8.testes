using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.DataSource.AcertoViagem;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoViagem : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>
    {
        public AcertoViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemAcertoEmAndamento(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista && obj.Situacao == SituacaoAcertoViagem.EmAntamento select obj;
            return result.Any();
        }

        public List<int> BuscarInfracoesDoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigo && o.Infracao != null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual, UnitOfWork unitOfWork)
        {
            IQueryable<Dominio.Entidades.Embarcador.Acerto.AcertoViagem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            query = query.Where(o => o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual);

            return query.Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagem BuscarAcertoFechado(DateTime dataInicial, DateTime dataFinal, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal <= dataFinal.Date);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            result = result.Where(obj => obj.Situacao == SituacaoAcertoViagem.Fechado);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagem BuscarAcertoAberto(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista && obj.Situacao == SituacaoAcertoViagem.EmAntamento select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagem BuscarProximosDados(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado);

            result = result.OrderBy("Numero descending");

            return result.FirstOrDefault();
        }

        public bool ContemAcertoEmAberto(int codigoMotorista, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Codigo != codigoAcerto && obj.Motorista.Codigo == codigoMotorista && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento select obj;
            return result.Count() > 0;
        }

        public bool ContemAcertoFechadoMaior(int codigoMotorista, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Codigo > codigoAcerto && obj.Motorista.Codigo == codigoMotorista && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;
            return result.Any();
        }

        public int UltimoNumeracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            var result = from obj in query select obj;

            if (result.Count() > 0)
            {
                result = result.OrderBy("Numero descending");

                return result.FirstOrDefault().Numero;
                //return result.Select(obj => (int)obj.Numero).Sum();
            }
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoViagem> Consulta(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork, int numeroAcerto, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, DateTime dataAcerto, int codigoOperador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            var result = from obj in query select obj;

            if (numeroAcerto > 0)
                result = result.Where(obj => obj.Numero == numeroAcerto);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial && obj.DataFinal <= dataFinal);

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial);

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal <= dataFinal);

            if (dataAcerto > DateTime.MinValue)
                result = result.Where(obj => obj.DataAcerto == dataAcerto);

            if (codigoOperador > 0)
            {
                result = result.Where(obj => (from p in obj.Logs where p.Usuario.Codigo == codigoOperador && p.TipoAcao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Inicio select new { p.Codigo }).Count() > 0);
            }

            if (etapa > 0)
                result = result.Where(obj => obj.Etapa == etapa);

            if (situacao > 0)
                result = result.Where(obj => obj.Situacao == situacao);

            if (codigoCarga > 0)
            {
                result = result.Where(obj => (from p in obj.Cargas where p.Carga.Codigo == codigoCarga select new { p.Codigo }).Count() > 0);
            }
            if (codigoVeiculo > 0)
            {
                result = result.Where(obj => (from p in obj.Veiculos where p.Veiculo.Codigo == codigoVeiculo select new { p.Codigo }).Count() > 0);
            }

            return result.Fetch(o => o.Motorista).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public List<int> ConsultarSeExisteAcertoPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoAcertoViagem[] situacaoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            query = query.Where(o => o.DataAcerto < dataFechamento.AddDays(1).Date && situacaoAcertoViagem.Contains(o.Situacao));

            return query.Select(o => o.Numero).ToList();
        }

        public int ContaConsulta(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork, int numeroAcerto, int codigoMotorista, DateTime dataInicial, DateTime dataFinal, DateTime dataAcerto, int codigoOperador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();

            var result = from obj in query select obj;

            if (numeroAcerto > 0)
                result = result.Where(obj => obj.Numero == numeroAcerto);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial && obj.DataFinal <= dataFinal);

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial);

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal <= dataFinal);

            if (dataAcerto > DateTime.MinValue)
                result = result.Where(obj => obj.DataAcerto == dataAcerto);

            if (codigoOperador > 0)
            {
                result = result.Where(obj => (from p in obj.Logs where p.Usuario.Codigo == codigoOperador && p.TipoAcao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Inicio select new { p.Codigo }).Count() > 0);
            }

            if (etapa > 0)
                result = result.Where(obj => obj.Etapa == etapa);

            if (situacao > 0)
                result = result.Where(obj => obj.Situacao == situacao);

            if (codigoCarga > 0)
            {
                result = result.Where(obj => (from p in obj.Cargas where p.Carga.Codigo == codigoCarga select new { p.Codigo }).Count() > 0);
            }
            if (codigoVeiculo > 0)
            {
                result = result.Where(obj => (from p in obj.Veiculos where p.Veiculo.Codigo == codigoVeiculo select new { p.Codigo }).Count() > 0);
            }

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoViagem BuscarPorDocumento(string numeroDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagem>();
            var result = from obj in query where obj.Numero == Convert.ToInt32(numeroDocumento) select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Relatórios

        public IList<RelatorioAcertoViagem> BuscarRelatorioConsultaAcertoViagem(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagem(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(RelatorioAcertoViagem)));

            return query.SetTimeout(50000).List<RelatorioAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagem(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = "SELECT A.ACV_CODIGO Codigo,  " +
                    " A.ACV_NUMERO NumeroAcerto, " +
                    " A.ACV_DATA_ACERTO DataAcerto, " +
                    " A.ACV_DATA_INICIAL DataInicial, " +
                    " A.ACV_DATA_FINAL DataFinal, " +
                    " A.ACV_OBSERVACAO Observacao, " +
                    " A.ACV_SITUACAO Situacao, " +
                    " A.ACV_ETAPA Etapa, " +
                    " A.ECV_APROVACAO_ABASTECIMENTO AprovadoAbastecimento, " +
                    " A.ECV_APROVACAO_PEDAGIO AprovadoPedagio, " +
                    " V.VEI_PLACA Placa, " +
                    " V.VEI_CHASSI Chassi, " +
                    " V.VEI_ANO Ano, " +
                    " V.VEI_ANOMODELO AnoModelo, " +
                    " V.VEI_RENAVAM Renavam, " +
                    " V.VEI_NUMERO_FROTA NumeroFrota, " +
                    " M.VMO_DESCRICAO Modelo, " +
                    " MA.VMA_DESCRICAO Marca, " +
                    " (SELECT TOP(1) AR.AAR_KM_INICIAL FROM T_ACERTO_ABASTECIMENTO_RESUMO AR " +
                    "  WHERE AR.ACV_CODIGO = A.ACV_CODIGO AND AR.VEI_CODIGO = V.VEI_CODIGO AND AR.AAR_TIPO = 1 " +
                    "  ORDER BY AR.AAR_KM_INICIAL) KMInicial,  " +
                    "  (SELECT TOP(1) AR.AAR_KM_FINAL FROM T_ACERTO_ABASTECIMENTO_RESUMO AR " +
                    "  WHERE AR.ACV_CODIGO = A.ACV_CODIGO AND AR.VEI_CODIGO = V.VEI_CODIGO AND AR.AAR_TIPO = 1 " +
                    "  ORDER BY AR.AAR_KM_FINAL DESC) KMFinal, " +
                    " (SELECT TOP(1) AR.AAR_KM_TOTAL FROM T_ACERTO_ABASTECIMENTO_RESUMO AR " +
                    " WHERE AR.ACV_CODIGO = A.ACV_CODIGO AND AR.VEI_CODIGO = V.VEI_CODIGO AND AR.AAR_TIPO = 1 " +
                    " ORDER BY AR.AAR_KM_FINAL DESC) KMTotal, " +
                    " (SELECT TOP(1) AR.AAR_KM_TOTAL_AJUSTADO FROM T_ACERTO_ABASTECIMENTO_RESUMO AR " +
                    " WHERE AR.ACV_CODIGO = A.ACV_CODIGO AND AR.VEI_CODIGO = V.VEI_CODIGO AND AR.AAR_TIPO = 1 " +
                    " ORDER BY AR.AAR_KM_FINAL DESC) KMTotalAjustado, " +
                    " (SELECT COUNT(1) FROM T_ACERTO_OUTRA_DESPESA AD WHERE AD.ACV_CODIGO = A.ACV_CODIGO) Despesa, " +
                    " (SELECT COUNT(1) FROM T_ACERTO_PEDAGIO AD WHERE AD.ACV_CODIGO = A.ACV_CODIGO) Pedagio, " +
                    " (SELECT COUNT(1) FROM T_ACERTO_ABASTECIMENTO AD WHERE AD.ACV_CODIGO = A.ACV_CODIGO) Abastecimento, " +
                    " (SELECT COUNT(1) FROM T_ACERTO_CARGA AD WHERE AD.ACV_CODIGO = A.ACV_CODIGO) Carga, 0 FichaMotorista, 0 Documentos " +
                    " FROM T_ACERTO_DE_VIAGEM A " +
                    " LEFT OUTER JOIN T_ACERTO_VEICULO AV ON AV.ACV_CODIGO = A.ACV_CODIGO " +
                    " LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = AV.VEI_CODIGO " +
                    " LEFT OUTER JOIN T_VEICULO_MODELO M ON M.VMO_CODIGO = V.VMO_CODIGO " +
                    " LEFT OUTER JOIN T_VEICULO_MARCA MA ON MA.VMA_CODIGO = V.VMA_CODIGO " +
                    " WHERE A.ACV_CODIGO = " + acertoViagem.Codigo;
            return select;
        }

        public IList<CargasAcertoViagem> BuscarRelatorioConsultaAcertoViagemCargas(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagemCargas(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(CargasAcertoViagem)));

            return query.SetTimeout(50000).List<CargasAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagemCargas(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;
            //A.ACC_VALOR_BRUTO_CARGA ValorBrutoCarga, 
            select = @" SELECT A.ACV_CODIGO CodigoAcerto, 
                     A.ACC_CODIGO Codigo, 
                     ISNULL(A.ACC_PERCENTUAL_ACERTO, 0) PercentualCarga, 
                     A.ACC_PEDAGIO_ACERTO PedagioCarga, 
                     C.CAR_DATA_CRIACAO Data, 
                     V.VEI_PLACA Placa, 
                     C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, 
                     F.FIL_DESCRICAO Emitente, 
                     F.FIL_CNPJ CNPJEmitente, 
                     (SELECT SUM(T.CON_VALOR_RECEBER) 
                     FROM T_CARGA_CTE CT 
                     JOIN T_CTE T ON T.CON_CODIGO = CT.CON_CODIGO AND T.CON_STATUS = 'A' AND T.CON_TIPO_CTE <> 2
                     WHERE CT.CCC_CODIGO IS NULL AND CT.CAR_CODIGO = C.CAR_CODIGO) ValorFrete, 
                     (SELECT MAX(T.CON_DATAHORAEMISSAO) 
                     FROM T_CARGA_CTE CT 
                     JOIN T_CTE T ON T.CON_CODIGO = CT.CON_CODIGO AND T.CON_STATUS = 'A' AND T.CON_TIPO_CTE <> 2
                     WHERE CT.CCC_CODIGO IS NULL AND CT.CAR_CODIGO = C.CAR_CODIGO) DataEmissaoCTe, 
                     (SELECT ISNULL(SUM(T.CON_VAL_ICMS), 0) 
                     FROM T_CARGA_CTE CT 
                     JOIN T_CTE T ON T.CON_CODIGO = CT.CON_CODIGO 
                     WHERE CT.CCC_CODIGO IS NULL AND CT.CAR_CODIGO = C.CAR_CODIGO AND T.CON_STATUS = 'A' AND T.CON_TIPO_CTE <> 2 AND T.CON_CST <> '60' AND A.ACC_CARGA_FRACIONADA = 0) +
					 CASE
						WHEN A.ACC_CARGA_FRACIONADA = 1 THEN ISNULL(A.ACC_VALOR_ICMS_CARGA, 0)
                        --WHEN ISNULL(A.ACC_PERCENTUAL_ACERTO, 0) < 100 THEN ISNULL(A.ACC_VALOR_ICMS_CARGA, 0)
						ELSE 0
					 END ValorICMS, 
                     (SELECT SUM(I.ICA_QTD) 
                     FROM T_CARGA_CTE CT 
                     JOIN T_CTE_INF_CARGA I ON I.CON_CODIGO = CT.CON_CODIGO 
                     WHERE CT.CCC_CODIGO IS NULL AND CT.CAR_CODIGO = C.CAR_CODIGO AND I.ICA_UN = '01') Peso, 
                     C.CAR_CODIGO CodigoCarga, 
                     ISNULL((SELECT SUM(CC.CCF_VALOR_COMPONENTE) 
                     FROM T_CARGA_COMPONENTES_FRETE CC 
                     WHERE CC.CAR_CODIGO = C.CAR_CODIGO AND CC.CCF_TIPO_COMPONENTE_FRETE = 2), 0) ValorComponenteFrete, 
                     A.ACC_VALOR_BONIFICACAO_CLIENTE BonificacaoCliente, 
                     CASE
						WHEN A.ACC_CARGA_FRACIONADA = 1 THEN A.ACC_VALOR_BRUTO_CARGA 
                        WHEN ISNULL(A.ACC_PERCENTUAL_ACERTO, 0) < 100 THEN ((SELECT SUM(CC.CON_VALOR_RECEBER) FROM T_CTE CC JOIN T_CARGA_CTE EE ON EE.CON_CODIGO = CC.CON_CODIGO AND EE.CAR_CODIGO = A.CAR_CODIGO WHERE CC.CON_STATUS = 'A' AND CC.CON_TIPO_CTE <> 2 AND EE.CCC_CODIGO IS NULL) * (A.ACC_PERCENTUAL_ACERTO / 100))
						ELSE (SELECT SUM(CC.CON_VALOR_FRETE) FROM T_CTE CC JOIN T_CARGA_CTE EE ON EE.CON_CODIGO = CC.CON_CODIGO AND EE.CAR_CODIGO = A.CAR_CODIGO WHERE CC.CON_STATUS = 'A' AND CC.CON_TIPO_CTE <> 2 AND EE.CCC_CODIGO IS NULL)
					 END ValorBrutoCarga, 
                     ISNULL(A.ACC_VALOR_ICMS_CARGA, 0) ValorICMSCarga, 
                     ISNULL(A.ACC_PEDAGIO_ACERTO_CREDITO, 0) ValorPedagioCredito 
                     FROM T_ACERTO_CARGA A 
                     JOIN T_CARGA C ON C.CAR_CODIGO = A.CAR_CODIGO 
                     JOIN T_ACERTO_DE_VIAGEM AA ON AA.ACV_CODIGO = A.ACV_CODIGO 
                     LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO 
                     LEFT OUTER JOIN T_FILIAL F ON F.FIL_CODIGO = C.FIL_CODIGO 
                     WHERE A.ACV_CODIGO = " + acertoViagem.Codigo + " ORDER BY C.CAR_DATA_CRIACAO";
            return select;
        }

        public IList<AbastecimentosAcertoViagem> BuscarRelatorioConsultaAcertoViagemAbastecimentos(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagemAbastecimentos(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(AbastecimentosAcertoViagem)));

            return query.SetTimeout(50000).List<AbastecimentosAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagemAbastecimentos(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = " SELECT A.ACB_CODIGO Codigo, " +
                    " A.ACV_CODIGO CodigoAcerto, " +
                    " B.ABA_DATA Data, " +
                    " ISNULL((SELECT top(1) BB.ABA_KM " +
                    " FROM T_ABASTECIMENTO BB " +
                    " WHERE BB.VEI_CODIGO = B.VEI_CODIGO " +
                    " AND BB.ABA_KM < B.ABA_KM " +
                    " AND BB.ABA_TIPO = B.ABA_TIPO " +
                    " AND BB.ABA_LITROS > 0 " +
                    " AND BB.ABA_CODIGO IN (SELECT AAB.ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO AAB WHERE AAB.ACV_CODIGO = " + acertoViagem.Codigo + ") " + // SQL-INJECTION-SAFE
                    " ORDER BY BB.ABA_KM DESC), 0) KMAnterior, " +
                    " B.ABA_KM Kilometragem, " +
                    " B.ABA_LITROS Litros, " +
                    " B.ABA_VALOR_UN ValorUnitario, " +
                    " C.CLI_CGCCPF CNPJPosto, " +
                    " C.CLI_NOME NomePosto, " +
                    " L.LOC_DESCRICAO Cidade, " +
                    " L.UF_SIGLA Estado, " +
                    " V.VEI_CODIGO CodigoVeiculo, " +
                    " V.VEI_PLACA Placa, " +
                    " B.ABA_DOCUMENTO Documento, " +
                    " P.PRO_CODIGO CodigoProduto, " +
                    " P.PRO_DESCRICAO Produto, " +
                    " B.ABA_TIPO TipoAbastecimento, " +
                    " (B.ABA_LITROS * B.ABA_VALOR_UN) ValorTotal, " +
                    " AR.AAR_MEDIA_IDEAL ValorDigitado, " +
                    " 0.0 KmInicial, " +
                    " AR.AAR_KM_TOTAL KmTotal, " +
                    " AR.AAR_KM_TOTAL_AJUSTADO KmTotalAjustado, " +
                    " AR.AAR_PERCENTUAL_AJUSTE PercentualAjusteKM " +
                    " FROM T_ACERTO_ABASTECIMENTO A " +
                    " JOIN T_ABASTECIMENTO B ON B.ABA_CODIGO = A.ABA_CODIGO " +
                    " JOIN T_ACERTO_ABASTECIMENTO_RESUMO AR ON AR.ACV_CODIGO = A.ACV_CODIGO AND AR.VEI_CODIGO = B.VEI_CODIGO AND AR.AAR_TIPO = B.ABA_TIPO " +
                    " JOIN T_CLIENTE C ON C.CLI_CGCCPF = B.CLI_CGCCPF " +
                    " JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO " +
                    " JOIN T_VEICULO V ON V.VEI_CODIGO = B.VEI_CODIGO " +
                    " JOIN T_PRODUTO P ON P.PRO_CODIGO = B.PRO_CODIGO " +
                    " WHERE A.ACV_CODIGO = " + acertoViagem.Codigo +
                    " ORDER BY B.ABA_TIPO, V.VEI_CODIGO, B.ABA_KM";

            return select;
        }

        public IList<PedagiosAcertoViagem> BuscarRelatorioConsultaAcertoViagemPedagios(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagemPedagios(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(PedagiosAcertoViagem)));

            return query.SetTimeout(50000).List<PedagiosAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagemPedagios(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = " SELECT A.ACP_CODIGO Codigo, " +
                    " A.ACV_CODIGO CodigoAcerto, " +
                    " P.PED_DATA Data, " +
                    " P.PED_RODOVIA Rodovia, " +
                    " P.PED_PRACA Praca, " +
                    " P.PED_VALOR Valor, " +
                    " P.PED_IMPORTADO_SEM_PARAR Importado, " +
                    " V.VEI_CODIGO CodigoVeiculo, " +
                    " V.VEI_PLACA Placa, " +
                    " CASE WHEN A.ACV_LANCADO_MANUALMENTE = 1 THEN 1 ELSE 0 END LancadoManualmente, " +
                    " P.PED_SITUACAO Situacao, " +
                    " P.PED_TIPO Tipo " +
                    " FROM T_ACERTO_PEDAGIO A " +
                    " JOIN T_PEDAGIO P ON P.PED_CODIGO = A.PED_CODIGO " +
                    " JOIN T_VEICULO V ON V.VEI_CODIGO = P.VEI_CODIGO " +
                    " WHERE A.ACV_CODIGO = " + acertoViagem.Codigo;
            //" WHERE P.PED_TIPO = 2 and A.ACV_CODIGO = " + acertoViagem.Codigo;

            return select;
        }

        public IList<OcorrenciasAcertoViagem> BuscarRelatorioConsultaAcertoViagemOcorrencias(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagemOcorrencias(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(OcorrenciasAcertoViagem)));

            return query.SetTimeout(50000).List<OcorrenciasAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagemOcorrencias(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = @" SELECT A.ACV_CODIGO CodigoAcerto,
                    O.COC_CODIGO Codigo,
                    O.COC_NUMERO_CONTRATO NumeroOcorrencia,
                    O.COC_DATA_OCORRENCIA DataOcorrencia,
                    C.CAR_CODIGO_CARGA_EMBARCADOR CodigoCargaEmbarcador,
                    '' Veiculo,
                    '' TipoOcorrencia,
                    '' Motorista,
                    '' TipoOcorrencia,                    
                    O.COC_VALOR_OCORRENCIA Valor,
                    '' DescricaoSituacao,
                    ISNULL((SELECT SUM(CTe.CON_VAL_ICMS) FROM T_CARGA_OCORRENCIA_DOCUMENTO Docs JOIN T_CTE CTe on CTe.CON_CODIGO = Docs.CON_CODIGO WHERE Docs.COC_CODIGO = O.COC_CODIGO), 0) ValorICMS,
					ISNULL((SELECT SUM(CTe.CON_VALOR_FRETE) FROM T_CARGA_OCORRENCIA_DOCUMENTO Docs JOIN T_CTE CTe on CTe.CON_CODIGO = Docs.CON_CODIGO WHERE Docs.COC_CODIGO = O.COC_CODIGO), 0) FreteLiquido
                    FROM T_ACERTO_OCORRENCIA A
                    JOIN T_CARGA_OCORRENCIA O ON A.COC_CODIGO = O.COC_CODIGO
                    JOIN T_CARGA C ON C.CAR_CODIGO = O.CAR_CODIGO
                   WHERE A.ACV_CODIGO = " + acertoViagem.Codigo;

            return select;
        }

        public IList<DespesasAcertoViagem> BuscarRelatorioConsultaAcertoViagemDespesas(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaAcertoViagemDespesas(false, propriedades, acertoViagem, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(DespesasAcertoViagem)));

            return query.SetTimeout(50000).List<DespesasAcertoViagem>();
        }

        private string ObterSelectRelatorioConsultaAcertoViagemDespesas(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = " SELECT A.AOD_CODIGO Codigo, " +
                    " A.ACV_CODIGO CodigoAcerto, " +
                    " A.AOD_FORNECEDOR NomeFornecedor, " +
                    " A.AOD_NUMERO_DOCUMENTO NumeroDocumento, " +
                    " A.AOD_DATA Data, " +
                    " A.AOD_VALOR Valor, " +
                    " A.AOD_QUANTIDADE Quantidade, " +
                    " (A.AOD_VALOR * case when A.AOD_QUANTIDADE = 0 then 1 else A.AOD_QUANTIDADE end) ValorTotal, " +
                    " A.AOD_OBSERVACAO Observacao, " +
                    " C.CLI_CGCCPF CNPJFornecedor, " +
                    " C.CLI_NOME Fornecedor, " +
                    " L.LOC_DESCRICAO Cidade, " +
                    " L.UF_SIGLA Estado, " +
                    " P.PRO_CODIGO CodigoProduto, " +
                    " P.PRO_DESCRICAO Produto, " +
                    " V.VEI_CODIGO CodigoVeiculo, " +
                    " V.VEI_PLACA Placa " +
                    " FROM T_ACERTO_OUTRA_DESPESA A " +
                    " JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF " +
                    " JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO " +
                    " LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = A.PRO_CODIGO " +
                    " LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = A.VEI_CODIGO " +
                    " WHERE A.ACV_CODIGO = " + acertoViagem.Codigo;

            return select;
        }

        public IList<ResultadoAcertoViagem> RelatorioResultadoAcertoViagem(int codigoSegmentoVeiculo, int codigoMotorista, int codigoGrupoPessoa, int codigoModeloVeicular, int codigoVeiculoTracao, int codigoVeiculoReboque, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT AR.VEI_CODIGO_TRACAO CodigoTracao, 
                 V.VEI_PLACA PlacaTracao, 
                 M.VMO_CODIGO CodigoModeloTracao, 
                 M.VMO_DESCRICAO ModeloTracao, 
                 MA.VMA_DESCRICAO MarcaVeiculo,
                 ISNULL(CAST(VSE.VSE_CODIGO AS VARCHAR(20)), SUBSTRING((SELECT DISTINCT ', ' + CAST(S.VSE_CODIGO AS VARCHAR(20)) FROM T_VEICULO VV 
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000)) CodigoSegmento,                  
                 ISNULL(VSE.VSE_DESCRICAO, SUBSTRING((SELECT DISTINCT ', ' + S.VSE_DESCRICAO FROM T_VEICULO V 
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000)) Segmento,  
                 G.GRP_CODIGO CodigoGrupoPessoa, 
                 G.GRP_DESCRICAO GrupoPessoa, 
                 MV.MVC_CODIGO CodigoModeloVeicular, 
                 MV.MVC_DESCRICAO ModeloVeicular, 
                 V.VEI_ANO AnoTracao, 
                 A.VEI_NUMERO_FROTA FrotaTracao, 
                 A.ACV_NUMERO NumeroAcertoViagem, 
                 DATEDIFF(day, A.ACV_DATA_INICIAL, A.ACV_DATA_FINAL) QuantidadeDias, 
                 A.ACV_DATA_FINAL DataFinal, 
                 F.FUN_CODIGO CodigoMotorista, 
                 F.FUN_NOME NomeMotorista,
				 F.FUN_CODIGO_INTEGRACAO CodigoIntegracaoMotorista,
                 AR.AVR_FATURAMENTO_BRUTO + isnull((select sum(acb.acb_valor) from T_ACERTO_CARGA_BONIFICACAO ACB
                    JOIN T_ACERTO_CARGA AC ON AC.ACC_CODIGO = ACB.ACC_CODIGO
                    where AC.ACV_CODIGO = a.acv_codigo), 0) FaturamentoBruto, 
                 AR.AVR_SALDO_FINAL ResultadoLiquido, 
                 AR.AVR_ICMS ValorICMS, 
                 AR.AVR_COMBUSTIVEL_TRACAO CombustivelTracao, 
                 (AR.AVR_COMBUSTIVEL_TRACAO / CASE WHEN AR.AVR_KM_TOTAL > 0 THEN AR.AVR_KM_TOTAL ELSE 1 END) CombustivelTracaoKm, 
                 AR.AVR_COMBUSTIVEL_REBOQUES CombustivelEquipamentos, 
                 (AR.AVR_COMBUSTIVEL_REBOQUES / CASE WHEN AR.AVR_KM_TOTAL > 0 THEN AR.AVR_KM_TOTAL ELSE 1 END) CombustivelEquipamentosKm, 
                 AR.AVR_KM_TOTAL KMTotal, 
                 (AR.AVR_SALDO_FINAL / CASE WHEN AR.AVR_KM_TOTAL > 0 THEN AR.AVR_KM_TOTAL ELSE 1 END) ValorKMSemICMS, 
                 ((AR.AVR_SALDO_FINAL + AR.AVR_ICMS) / CASE WHEN AR.AVR_KM_TOTAL > 0 THEN AR.AVR_KM_TOTAL ELSE 1 END) ValorKMComICMS, 
                 AR.AVR_VALOR_LIQUIDO_MES TotalMesComICMS,                 
                 AR.AVR_PARAMETRO_MEDIA_TRACAO ParametroMedia, 
                 AR.AVR_MEDIA_TRACAO Media, 
                 A.ACV_OBSERVACAO Observacao, 
                 ((AR.AVR_COMBUSTIVEL_TRACAO + AR.AVR_COMBUSTIVEL_REBOQUES) / CASE WHEN AR.AVR_KM_TOTAL > 0 THEN AR.AVR_KM_TOTAL ELSE 1 END) DieselKM, 
                 ISNULL(AR.AVR_OCORRENCIAS, 0) Ocorrencias,
                 ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 0 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1) LitrosTracao,
				 ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 1 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1) LitrosReboque,
				 (AR.AVR_COMBUSTIVEL_TRACAO / 
				 CASE WHEN ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 0 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1) > 0
				 THEN ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 0 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1)
				 ELSE 1 END) MediaLitroCavalo,
				 (AR.AVR_COMBUSTIVEL_REBOQUES / 
				 CASE WHEN ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 1 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1)> 0
				 THEN ISNULL((SELECT SUM(ARR.AAR_LITROS) FROM T_ACERTO_ABASTECIMENTO_RESUMO ARR JOIN T_ACERTO_VEICULO AVV ON AVV.ACV_CODIGO = A.ACV_CODIGO AND AVV.VEI_CODIGO = ARR.VEI_CODIGO JOIN T_VEICULO VVV ON VVV.VEI_CODIGO = AVV.VEI_CODIGO AND VVV.VEI_TIPOVEICULO = 1 WHERE ARR.ACV_CODIGO = A.ACV_CODIGO AND ARR.AAR_TIPO = 1), 1)
				 ELSE 1 END) MediaLitroReboque,
                 AR.AVR_DESPESA_PEDAGIO PedagioPago,
				 AR.AVR_RECEITA_PEDAGIO PedagioRecebido
                 FROM T_ACERTO_DE_VIAGEM A  
                 JOIN T_ACERTO_VEICULO_RESULTADO AR ON A.ACV_CODIGO = AR.ACV_CODIGO 				 
                 LEFT OUTER JOIN T_ACERTO_VEICULO_SEGMENTO SA ON SA.VEI_CODIGO = AR.VEI_CODIGO_TRACAO AND SA.ACV_CODIGO = A.ACV_CODIGO 
                 LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = AR.VEI_CODIGO_TRACAO 
                 LEFT OUTER JOIN T_VEICULO_MARCA MA ON MA.VMA_CODIGO = V.VMA_CODIGO
				 LEFT OUTER JOIN T_ACERTO_VEICULO AC ON AC.ACV_CODIGO = A.ACV_CODIGO AND AC.VEI_CODIGO = V.VEI_CODIGO
				 LEFT OUTER JOIN T_VEICULO_SEGMENTO VS ON VS.VSE_CODIGO = AC.VSE_CODIGO
                 LEFT OUTER JOIN T_VEICULO_MODELO M ON M.VMO_CODIGO = V.VMO_CODIGO 
                 LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = SA.GRP_CODIGO 
                 LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MV ON MV.MVC_CODIGO = SA.MVC_CODIGO 
                 LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA 
                 LEFT OUTER JOIN T_VEICULO_SEGMENTO VSE ON VSE.VSE_CODIGO = A.VSE_CODIGO
                 WHERE 1 = 1 ";

            if (codigoSegmentoVeiculo > 0)
                query += @"AND (CASE WHEN A.VSE_CODIGO IS NOT NULL THEN A.VSE_CODIGO END = " + codigoSegmentoVeiculo.ToString() + @"
                                    OR
					            CASE WHEN A.VSE_CODIGO IS NULL THEN A.ACV_CODIGO END IN (SELECT AV.ACV_CODIGO FROM T_VEICULO VV 
						        JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
						        JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND AV.ACV_CODIGO = A.ACV_CODIGO AND S.VSE_CODIGO = " + codigoSegmentoVeiculo.ToString() + "))";

            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();
            if (codigoGrupoPessoa > 0)
                query += " AND G.GRP_CODIGO = " + codigoGrupoPessoa.ToString();
            if (codigoModeloVeicular > 0)
                query += " AND MV.MVC_CODIGO = " + codigoModeloVeicular.ToString();
            if (codigoVeiculoTracao > 0)
                query += " AND AR.VEI_CODIGO_TRACAO = " + codigoVeiculoTracao.ToString();
            if (codigoVeiculoReboque > 0)
                query += " AND AR.AVR_CODIGO IN (SELECT VV.AVR_CODIGO FROM T_ACERTO_VEICULO_RESULTADO_VEICULOS_VINCULADOS VV WHERE VV.VEI_CODIGO = " + codigoVeiculoTracao.ToString() + ") "; // SQL-INJECTION-SAFE

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ResultadoAcertoViagem)));

            return nhQuery.SetTimeout(50000).List<ResultadoAcertoViagem>();
        }

        public int ContarRelatorioResultadoAcertoViagem(int codigoSegmentoVeiculo, int codigoMotorista, int codigoGrupoPessoa, int codigoModeloVeicular, int codigoVeiculoTracao, int codigoVeiculoReboque, DateTime dataInicial, DateTime dataFinal, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                 FROM T_ACERTO_DE_VIAGEM A
                 JOIN T_ACERTO_VEICULO_RESULTADO AR ON A.ACV_CODIGO = AR.ACV_CODIGO
                 LEFT OUTER JOIN T_ACERTO_VEICULO_SEGMENTO SA ON SA.VEI_CODIGO = AR.VEI_CODIGO_TRACAO AND SA.ACV_CODIGO = A.ACV_CODIGO
                 LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = AR.VEI_CODIGO_TRACAO
                 LEFT OUTER JOIN T_ACERTO_VEICULO AC ON AC.ACV_CODIGO = A.ACV_CODIGO AND AC.VEI_CODIGO = V.VEI_CODIGO
                 LEFT OUTER JOIN T_VEICULO_SEGMENTO VS ON VS.VSE_CODIGO = AC.VSE_CODIGO
                 LEFT OUTER JOIN T_VEICULO_MODELO M ON M.VMO_CODIGO = V.VMO_CODIGO
                 LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = SA.GRP_CODIGO
                 LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA MV ON MV.MVC_CODIGO = SA.MVC_CODIGO
                 LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
                 LEFT OUTER JOIN T_VEICULO_SEGMENTO VSE ON VSE.VSE_CODIGO = A.VSE_CODIGO
                 WHERE 1 = 1 ";

            if (codigoSegmentoVeiculo > 0)
                query += @"AND (CASE WHEN A.VSE_CODIGO IS NOT NULL THEN A.VSE_CODIGO END = " + codigoSegmentoVeiculo.ToString() + @"
                                    OR
					            CASE WHEN A.VSE_CODIGO IS NULL THEN A.ACV_CODIGO END IN (SELECT AV.ACV_CODIGO FROM T_VEICULO VV 
						        JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
						        JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND AV.ACV_CODIGO = A.ACV_CODIGO AND S.VSE_CODIGO = " + codigoSegmentoVeiculo.ToString() + "))";

            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();
            if (codigoGrupoPessoa > 0)
                query += " AND G.GRP_CODIGO = " + codigoGrupoPessoa.ToString();
            if (codigoModeloVeicular > 0)
                query += " AND MV.MVC_CODIGO = " + codigoModeloVeicular.ToString();
            if (codigoVeiculoTracao > 0)
                query += " AND AR.VEI_CODIGO_TRACAO = " + codigoVeiculoTracao.ToString();
            if (codigoVeiculoReboque > 0)
                query += " AND AR.AVR_CODIGO IN (SELECT VV.AVR_CODIGO FROM T_ACERTO_VEICULO_RESULTADO_VEICULOS_VINCULADOS VV WHERE VV.VEI_CODIGO = " + codigoVeiculoTracao.ToString() + ") "; // SQL-INJECTION-SAFE

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<ResultadoAnualAcertoViagem> RelatorioResultadoAnualAcertoViagem(int codigoSegmentoVeiculo, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT SUM(AR.AVR_KM_TOTAL) KMTotal, 
                    SUM(AR.AVR_SALDO_FINAL) ResultadoLiquido, 
                    SUM(AR.AVR_ICMS) ValorICMS,
                    MONTH(A.ACV_DATA_FINAL) MesDataFinal,
                    YEAR(A.ACV_DATA_FINAL) AnoDataFinal,
                    S.VSE_DESCRICAO Segmento,
                    SUM(AR.AVR_NUMERO_VIAGEM) QtdDias,
                    S.VSE_CODIGO CodigoSegmento
                    FROM T_ACERTO_DE_VIAGEM A  
                    JOIN T_ACERTO_VEICULO_RESULTADO AR ON A.ACV_CODIGO = AR.ACV_CODIGO 	
                    JOIN T_VEICULO V ON V.VEI_CODIGO = AR.VEI_CODIGO_TRACAO
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    WHERE 1 = 1 ";

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            query += " GROUP BY MONTH(A.ACV_DATA_FINAL), YEAR(A.ACV_DATA_FINAL), S.VSE_DESCRICAO, S.VSE_CODIGO";

            query = @"SELECT (((T.ResultadoLiquido + T.ValorICMS) / T.QtdDias) * 30) ResultadoSegmento,
                    (T.ResultadoLiquido / T.KMTotal) SobraKM,
                    T.AnoDataFinal,
                    T.MesDataFinal,
                    T.Segmento, T.CodigoSegmento FROM 
                    (" + query + @") AS T
                    WHERE T.KMTotal > 0 ";

            if (codigoSegmentoVeiculo > 0)
                query += " AND T.Segmento = " + codigoSegmentoVeiculo.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ResultadoAnualAcertoViagem)));

            return nhQuery.SetTimeout(50000).List<ResultadoAnualAcertoViagem>();
        }

        public int ContarRelatorioResultadoAnualAcertoViagem(int codigoSegmentoVeiculo, DateTime dataInicial, DateTime dataFinal, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT SUM(AR.AVR_KM_TOTAL) KMTotal, 
                    SUM(AR.AVR_SALDO_FINAL) ResultadoLiquido, 
                    SUM(AR.AVR_ICMS) ValorICMS,
                    MONTH(A.ACV_DATA_FINAL) MesDataFinal,
                    YEAR(A.ACV_DATA_FINAL) AnoDataFinal,
                    S.VSE_DESCRICAO Segmento,
                    SUM(AR.AVR_NUMERO_VIAGEM) QtdDias,
                    S.VSE_CODIGO CodigoSegmento
                    FROM T_ACERTO_DE_VIAGEM A  
                    JOIN T_ACERTO_VEICULO_RESULTADO AR ON A.ACV_CODIGO = AR.ACV_CODIGO 	
                    JOIN T_VEICULO V ON V.VEI_CODIGO = AR.VEI_CODIGO_TRACAO
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    WHERE 1 = 1 ";

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            query += " GROUP BY MONTH(A.ACV_DATA_FINAL), YEAR(A.ACV_DATA_FINAL), S.VSE_DESCRICAO, S.VSE_CODIGO";

            query = @"SELECT  COUNT(0) as CONTADOR FROM 
                    (" + query + @") AS T
                    WHERE T.KMTotal > 0 ";

            if (codigoSegmentoVeiculo > 0)
                query += " AND T.Segmento = " + codigoSegmentoVeiculo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DiariaAcertoViagem> RelatorioDiariaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDiariaAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDiariaAcertoViagem(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DiariaAcertoViagem)));

            return query.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DiariaAcertoViagem>();
        }

        public int ContarConsultaRelatorioDiariaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDiariaAcertoViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioDiariaAcertoViagem(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(60000).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDiariaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDiariaAcertoViagem filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   groupBySub = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   whereSub = string.Empty,
                   orderBy = string.Empty;
            List<string> selectDinamico = new List<string>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDiariaAcertoViagem(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref whereSub, ref groupBy, ref joins, count, ref selectDinamico);

            SetarWhereRelatorioConsultaRelatorioDiariaAcertoViagem(ref where, ref whereSub, ref groupBySub, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDiariaAcertoViagem(propAgrupa, 0, ref select, ref whereSub, ref groupBy, ref joins, count, ref selectDinamico);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
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
            query += @" FROM T_ACERTO_DE_VIAGEM A 
                        JOIN T_FUNCIONARIO F  ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA                  
                        JOIN T_ACERTO_DIARIA D  ON D.ACV_CODIGO = A.ACV_CODIGO ";

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

        private void SetarSelectRelatorioConsultaRelatorioDiariaAcertoViagem(string propriedade, int codigoDinamico, ref string select, ref string whereSub, ref string groupBy, ref string joins, bool count, ref List<string> selectDinamico)
        {
            switch (propriedade)
            {
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select += "F.FUN_NOME Motorista, ";
                        selectDinamico.Add("Motorista");
                        groupBy += "F.FUN_NOME, ";
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "SUM(D.ACD_VALOR) Valor, ";
                        selectDinamico.Add("Valor");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select += "D.ACD_DESCRICAO Descricao, ";
                        selectDinamico.Add("Descricao");
                        groupBy += "D.ACD_DESCRICAO, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDiariaAcertoViagem(ref string where, ref string whereSub, ref string groupBySub, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDiariaAcertoViagem filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.Motorista > 0)
            {
                where += " AND A.FUN_CODIGO_MOTORISTA = " + filtrosPesquisa.Motorista.ToString();

                groupBy += "A.FUN_CODIGO_MOTORISTA, ";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                where += " AND CAST(A.ACV_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' AND CAST(A.ACV_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

                groupBy += "A.ACV_DATA_INICIAL, A.ACV_DATA_FINAL, ";
            }
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
            {
                where += " AND CAST(A.ACV_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' ";

                groupBy += "A.ACV_DATA_INICIAL, ";
            }
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                where += " AND CAST(A.ACV_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "' ";

                groupBy += "A.ACV_DATA_FINAL, ";
            }
        }


        //public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.DiariaAcertoViagem> RelatorioDiariaAcertoViagem(int codigoMotorista, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        //{
        //    string query = @"SELECT F.FUN_NOME Motorista, SUM(D.ACD_VALOR) Valor
        //        FROM T_ACERTO_DE_VIAGEM A
        //        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
        //        JOIN T_ACERTO_DIARIA D ON D.ACV_CODIGO = A.ACV_CODIGO
        //        WHERE ACV_SITUACAO = 2";

        //    if (codigoMotorista > 0)
        //        query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

        //    if (dataInicial != DateTime.MinValue)
        //        query += " AND A.ACV_DATA_INICIAL >= '" + dataInicial.ToString("MM/dd/yyyy 00:00:00") + "'";
        //    if (dataFinal != DateTime.MinValue)
        //        query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";

        //    query += " GROUP BY F.FUN_NOME";

        //    var agrup = false;
        //    if (!string.IsNullOrWhiteSpace(propGrupo))
        //    {
        //        agrup = true;
        //        query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
        //    }

        //    if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
        //    {
        //        if (agrup)
        //        {
        //            query += ", " + propOrdenacao + " " + dirOrdenacao;
        //        }
        //        else
        //        {
        //            query += " order by " + propOrdenacao + " " + dirOrdenacao;
        //        }
        //    }

        //    if (paginar)
        //        query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


        //    var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

        //    nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(DiariaAcertoViagem)));

        //    return nhQuery.SetTimeout(50000).List<DiariaAcertoViagem>();
        //}

        //public int ContarDiariaAcertoViagem(int codigoMotorista, DateTime dataInicial, DateTime dataFinal, bool todosCNPJdaRaizEmbarcador = false)
        //{
        //    string query = @"SELECT F.FUN_NOME Motorista, SUM(D.ACD_VALOR) Valor
        //        FROM T_ACERTO_DE_VIAGEM A
        //        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
        //        JOIN T_ACERTO_DIARIA D ON D.ACV_CODIGO = A.ACV_CODIGO
        //        WHERE ACV_SITUACAO = 2 ";

        //    if (codigoMotorista > 0)
        //        query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

        //    if (dataInicial != DateTime.MinValue)
        //        query += " AND A.ACV_DATA_INICIAL >= '" + dataInicial.ToString("MM/dd/yyyy 00:00:00") + "'";
        //    if (dataFinal != DateTime.MinValue)
        //        query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";

        //    query += " GROUP BY F.FUN_NOME";

        //    query = "SELECT COUNT(0) as CONTADOR FROM ( " + query + ") AS T";

        //    var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

        //    return nhQuery.UniqueResult<int>();
        //}

        public async Task<IList<DespesaAcertoViagem>> RelatorioDespesaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, CancellationToken cancellationToken, bool paginar = true)
        {
            string query = @"SELECT D.AOD_DATA Data,
                        C.CLI_NOME Fornecedor,
                        Pais.PAI_NOME PaisFornecedor,
                        A.ACV_NUMERO NumeroAcerto,
                        D.AOD_OBSERVACAO  + ' ' + ISNULL(P.PRO_DESCRICAO, '') Observacao,
                        D.AOD_QUANTIDADE Quantidade,
                        D.AOD_VALOR Valor,
                        V.VEI_PLACA Placa,
                        A.ACV_DATA_ACERTO DataAcerto,
                        A.ACV_DATA_INICIAL DataInicialAcerto,
                        A.ACV_DATA_FINAL DataFinalAcerto,
                        CASE
	                        WHEN A.ACV_SITUACAO = 1 THEN 'Em Andamento'
	                        WHEN A.ACV_SITUACAO = 2 THEN 'Fechado'
	                        ELSE 'Cancelado'
                        END Situacao,
                        F.FUN_NOME Motorista,
                        M.MVC_DESCRICAO ModeloVeiculo,
                        J.JUS_DESCRICAO Justificativa,
                        D.AOD_MOEDA_COTACAO_BANCO_CENTRAL MoedaCotacaoBancoCentral,
                        D.AOD_VALOR_MOEDA_COTACAO ValorMoedaCotacao,
                        D.AOD_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorOriginalMoedaEstrangeira,
                        '' Moeda
                    FROM T_ACERTO_OUTRA_DESPESA D
                    JOIN T_ACERTO_DE_VIAGEM A ON A.ACV_CODIGO = D.ACV_CODIGO
                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                    LEFT JOIN T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = C.LOC_CODIGO
                    LEFT JOIN T_PAIS Pais on Pais.PAI_CODIGO = Localidade.PAI_CODIGO
                    JOIN T_VEICULO V ON V.VEI_CODIGO = D.VEI_CODIGO
                    LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = D.PRO_CODIGO
                    LEFT OUTER JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = D.JUS_CODIGO  
                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA M ON M.MVC_CODIGO = V.MVC_CODIGO
                    WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoJustificativa > 0)
                query += " AND D.JUS_CODIGO = " + filtrosPesquisa.CodigoJustificativa.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                query += " AND M.MVC_CODIGO = " + filtrosPesquisa.CodigoModeloVeicular.ToString();
            if (filtrosPesquisa.CodigoVeiculoTracao > 0)
                query += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculoTracao.ToString();
            if (filtrosPesquisa.CodigoVeiculoReboque > 0)
                query += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculoReboque.ToString();
            if (filtrosPesquisa.CodigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + filtrosPesquisa.CodigoAcertoViagem.ToString();
            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND D.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();
            if (filtrosPesquisa.Situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)filtrosPesquisa.Situacao).ToString();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy") + "'";

            if (filtrosPesquisa.CodigoPais?.Count > 0)
                query += $" AND Pais.PAI_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigoPais) }) ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(DespesaAcertoViagem)));

            return await nhQuery.SetTimeout(50000).ListAsync<DespesaAcertoViagem>(cancellationToken);
        }

        public async Task<int> ContarDespesaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_OUTRA_DESPESA D
                    JOIN T_ACERTO_DE_VIAGEM A ON A.ACV_CODIGO = D.ACV_CODIGO
                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = D.CLI_CGCCPF
                    JOIN T_VEICULO V ON V.VEI_CODIGO = D.VEI_CODIGO
                    LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = D.PRO_CODIGO
                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA M ON M.MVC_CODIGO = V.MVC_CODIGO
                    WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoJustificativa > 0)
                query += " AND D.JUS_CODIGO = " + filtrosPesquisa.CodigoJustificativa.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                query += " AND M.MVC_CODIGO = " + filtrosPesquisa.CodigoModeloVeicular.ToString();
            if (filtrosPesquisa.CodigoVeiculoTracao > 0)
                query += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculoTracao.ToString();
            if (filtrosPesquisa.CodigoVeiculoReboque > 0)
                query += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculoReboque.ToString();
            if (filtrosPesquisa.CodigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + filtrosPesquisa.CodigoAcertoViagem.ToString();
            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND D.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();
            if (filtrosPesquisa.Situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)filtrosPesquisa.Situacao).ToString();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return await nhQuery.UniqueResultAsync<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.UltimoAcertoMotorista> RelatorioUltimoAcertoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, TipoMotorista tipoMotorista, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = @"SELECT F.FUN_CODIGO_INTEGRACAO CodigoIntegracao,
                F.FUN_NOME Motorista,
                F.FUN_CPF CPF,
                F.FUN_DATAADMISAO DataContratacao, 
                AA.ACV_DATA_ACERTO DataAcerto,
                AA.ACV_DATA_FINAL DataFinal,
                AA.ACV_DATA_INICIAL DataInicial,
                CASE
	                WHEN AA.ACV_SITUACAO = 1 THEN 'Em Andamento'
	                WHEN AA.ACV_SITUACAO = 2 THEN 'Fechado'
	                ELSE 'Sem Acerto'
                END Situacao, 
                AA.VEI_NUMERO_FROTA Frota, 
                AA.ACV_DATA_FECHAMENTO DataFechamento
                FROM T_FUNCIONARIO F
                LEFT OUTER JOIN T_ACERTO_DE_VIAGEM AA ON AA.ACV_CODIGO = 
                (SELECT TOP(1) MAX(A.ACV_CODIGO)
                FROM T_ACERTO_DE_VIAGEM A where a.ACV_SITUACAO in (1, 2) AND A.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO GROUP BY A.ACV_DATA_FINAL ORDER BY A.ACV_DATA_FINAL DESC)
                where F.FUN_TIPO = 'M'";

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND F.FUN_STATUS = 'A'";
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND F.FUN_STATUS = 'I'";

            if ((int)tipoMotorista > 0)
                query += " AND F.FUN_TIPO_MOTORISTA = " + ((int)tipoMotorista).ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.UltimoAcertoMotorista)));

            return nhQuery.SetTimeout(50000).List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.UltimoAcertoMotorista>();
        }

        public int ContarUltimoAcertoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, TipoMotorista tipoMotorista)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_FUNCIONARIO F                    
                    where F.FUN_TIPO = 'M'";

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND F.FUN_STATUS = 'A'";
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND F.FUN_STATUS = 'I'";

            if ((int)tipoMotorista > 0)
                query += " AND F.FUN_TIPO_MOTORISTA = " + ((int)tipoMotorista).ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem> RelatorioAcertoDeViagem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, bool visualizarUltimosAcertos, int codigoMotorista, int codigoSegmento, int codigoVeiculoTracao, int codigoVeiculoReboque, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialFechamento, DateTime dataFinalFechamento, int codigoAcertoViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT A.ACV_DATA_ACERTO Data,
                A.ACV_NUMERO NumeroAcerto,
                A.ACV_DATA_INICIAL DataInicial,
                A.ACV_DATA_FINAL DataFinal,
                A.ACV_DATA_FECHAMENTO DataFechamento,
                F.FUN_NOME Motorista,   
                F.FUN_CODIGO_INTEGRACAO CodigoIntegracao,
                F.FUN_CPF CPF,
                A.VEI_NUMERO_FROTA Frota,  
                ISNULL(S.VSE_DESCRICAO, SUBSTRING((SELECT DISTINCT ', ' + S.VSE_DESCRICAO FROM T_VEICULO V 
                JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000)) Segmento,
                CASE
	                WHEN A.ACV_SITUACAO = 1 THEN 'Em Andamento'
	                WHEN A.ACV_SITUACAO = 2 THEN 'Fechado'
	                ELSE 'Cancelado'
                END Situacao,
                O.FUN_NOME Operador, OI.FUN_NOME OperadorInicio
                FROM T_ACERTO_DE_VIAGEM A
                LEFT OUTER JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = A.VSE_CODIGO
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
                JOIN T_FUNCIONARIO O ON O.FUN_CODIGO = A.FUN_CODIGO_OPERADOR
                LEFT OUTER JOIN T_FUNCIONARIO OI ON OI.FUN_CODIGO = A.FUN_CODIGO_OPERADOR_INICIO_ACERTO
                WHERE 1 = 1 ";

            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

            if (codigoSegmento > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT AV.ACV_CODIGO FROM T_VEICULO V 
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND S.VSE_CODIGO = " + codigoSegmento.ToString() + @")";

            if (codigoVeiculoTracao > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculoTracao.ToString() + @")"; // SQL-INJECTION-SAFE

            if (codigoVeiculoReboque > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculoReboque.ToString() + @")"; // SQL-INJECTION-SAFE

            if (codigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + codigoAcertoViagem.ToString();

            if (situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)situacao).ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            if (dataInicialFechamento != DateTime.MinValue)
                query += " AND A.ACV_DATA_FECHAMENTO >= '" + dataInicialFechamento.ToString("MM/dd/yyyy") + "'";
            if (dataFinalFechamento != DateTime.MinValue)
                query += " AND A.ACV_DATA_FECHAMENTO <= '" + dataFinalFechamento.ToString("MM/dd/yyyy 23:59:59") + "'";

            if (tipoMotorista > 0)
                query += " AND F.FUN_TIPO_MOTORISTA = " + ((int)tipoMotorista).ToString();

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND F.FUN_STATUS = 'A'";
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND F.FUN_STATUS = 'I'";

            if (visualizarUltimosAcertos)
                query += @" AND A.ACV_CODIGO IN (SELECT TOP(1) MAX(AA.ACV_CODIGO) CODIGO
					FROM T_ACERTO_DE_VIAGEM AA 
					WHERE A.FUN_CODIGO_MOTORISTA = AA.FUN_CODIGO_MOTORISTA
					GROUP BY AA.FUN_CODIGO_MOTORISTA, AA.ACV_DATA_FINAL 
					ORDER BY AA.ACV_DATA_FINAL DESC) ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem)));

            return nhQuery.SetTimeout(50000).List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem>();
        }

        public int ContarAcertoDeViagem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista tipoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, bool visualizarUltimosAcertos, int codigoMotorista, int codigoSegmento, int codigoVeiculoTracao, int codigoVeiculoReboque, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialFechamento, DateTime dataFinalFechamento, int codigoAcertoViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_DE_VIAGEM A
                    LEFT OUTER JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = A.VSE_CODIGO
                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
                    JOIN T_FUNCIONARIO O ON O.FUN_CODIGO = A.FUN_CODIGO_OPERADOR
                    WHERE 1 = 1 ";

            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

            if (codigoSegmento > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT AV.ACV_CODIGO FROM T_VEICULO V 
                    JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = V.VSE_CODIGO
                    JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO AND S.VSE_CODIGO = " + codigoSegmento.ToString() + @")";

            if (codigoVeiculoTracao > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculoTracao.ToString() + @")"; // SQL-INJECTION-SAFE

            if (codigoVeiculoReboque > 0)
                query += @" AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculoReboque.ToString() + @")"; // SQL-INJECTION-SAFE

            if (codigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + codigoAcertoViagem.ToString();

            if (situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)situacao).ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            if (dataInicialFechamento != DateTime.MinValue)
                query += " AND A.ACV_DATA_FECHAMENTO >= '" + dataInicialFechamento.ToString("MM/dd/yyyy") + "'";
            if (dataFinalFechamento != DateTime.MinValue)
                query += " AND A.ACV_DATA_FECHAMENTO <= '" + dataFinalFechamento.ToString("MM/dd/yyyy 23:59:59") + "'";

            if (tipoMotorista > 0)
                query += " AND F.FUN_TIPO_MOTORISTA = " + ((int)tipoMotorista).ToString();

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND F.FUN_STATUS = 'A'";
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND F.FUN_STATUS = 'I'";

            if (visualizarUltimosAcertos)
                query += @" AND A.ACV_CODIGO IN (SELECT TOP(1) MAX(AA.ACV_CODIGO) CODIGO
					FROM T_ACERTO_DE_VIAGEM AA 
					WHERE A.FUN_CODIGO_MOTORISTA = AA.FUN_CODIGO_MOTORISTA
					GROUP BY AA.FUN_CODIGO_MOTORISTA, AA.ACV_DATA_FINAL 
					ORDER BY AA.ACV_DATA_FINAL DESC) ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargaCompartilhada> RelatorioCargaCompartilhada(int codigoCarga, DateTime dataInicial, DateTime dataFinal, int codigoMotorista, int numeroAcerto, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = string.Empty;
            string select = @"select C.CAR_DATA_CRIACAO DataCarga,
                A.ACV_DATA_FINAL DataAcerto,
                C.CAR_CODIGO_CARGA_EMBARCADOR Carga,
                ISNULL(S.CDS_REMETENTES, '') + ' ' + ISNULL(S.CDS_ORIGENS, '') Emitente, 
                ISNULL(S.CDS_DESTINATARIOS, '') + ' ' + ISNULL(S.CDS_DESTINOS, '') Destino,
                A.ACV_NUMERO NumeroAcerto,
                SUBSTRING(
	                (SELECT ', ' + 
		                Motorista.FUN_NOME 
	                FROM T_CARGA_MOTORISTA MotoristaCarga 
		                INNER JOIN T_FUNCIONARIO Motorista 
			                ON Motorista.FUN_CODIGO = MotoristaCarga.CAR_MOTORISTA 
	                WHERE MotoristaCarga.CAR_CODIGO = C.CAR_CODIGO 
	                FOR XML PATH('')), 
                3, 1000) Motoristas,
                AC.ACC_PERCENTUAL_ACERTO PercentualCompartilhado
                from T_ACERTO_CARGA AC ";

            string joins = @" JOIN T_ACERTO_DE_VIAGEM A ON A.ACV_CODIGO = AC.ACV_CODIGO 
                 JOIN T_CARGA C ON C.CAR_CODIGO = AC.CAR_CODIGO 
                 LEFT OUTER JOIN T_CARGA_DADOS_SUMARIZADOS S ON S.CDS_CODIGO = C.CDS_CODIGO ";

            string where = @" WHERE A.ACV_SITUACAO <> 3 AND AC.ACC_PERCENTUAL_ACERTO < 100 ";

            if (codigoCarga > 0)
                where += " AND C.CAR_CODIGO = " + codigoCarga.ToString();

            if (dataInicial != DateTime.MinValue)
                where += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                where += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            if (codigoMotorista > 0)
            {
                where += $" AND Motorista.FUN_CODIGO = {codigoMotorista} ";

                joins += @" LEFT OUTER JOIN T_CARGA_MOTORISTA MotoristaCarga ON MotoristaCarga.CAR_CODIGO = C.CAR_CODIGO 
                        LEFT OUTER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MotoristaCarga.CAR_MOTORISTA ";
            }

            if (numeroAcerto > 0)
                where += $" AND A.ACV_NUMERO = {numeroAcerto} ";

            query += select + joins + where;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargaCompartilhada)));

            return nhQuery.SetTimeout(50000).List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.CargaCompartilhada>();
        }

        public int ContarCargaCompartilhada(int codigoCarga, DateTime dataInicial, DateTime dataFinal, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                    from T_ACERTO_CARGA AC
                    JOIN T_ACERTO_DE_VIAGEM A ON A.ACV_CODIGO = AC.ACV_CODIGO
                    JOIN T_CARGA C ON C.CAR_CODIGO = AC.CAR_CODIGO
                    LEFT OUTER JOIN T_CARGA_DADOS_SUMARIZADOS S ON S.CDS_CODIGO = C.CDS_CODIGO
                    WHERE A.ACV_SITUACAO <> 3 AND AC.ACC_PERCENTUAL_ACERTO < 100 ";

            if (codigoCarga > 0)
                query += " AND C.CAR_CODIGO = " + codigoCarga.ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            if (dataFinal != DateTime.MinValue)
                query += " AND A.ACV_DATA_FINAL <= '" + dataFinal.ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<OcorrenciaComissaoAcertoViagem> RelatorioOcorrenciaComissaoAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa)
        {
            string query = @"SELECT I.INF_NUMERO Numero,
                    I.INF_NUMERO_ATUACAO NumeroAtuacao,
                    M.FUN_NOME Motorista,
                    T.TIN_DESCRICAO TipoInfracao,
                    V.VEI_PLACA Veiculo,
                    P.IFP_VALOR ValorInfracao,
                    T.TIN_PONTOS Pontos,
                    T.TIN_NIVEL Nivel,
                    ISNULL(T.TIN_PERCENTUAL_REDUCAO_COMISSAO_MOTORISTA, 0) ReducaoComissao,
                    M.FUN_CODIGO CodigoMotorista,
                    P.IFP_DATA_VENCIMENTO DataVencimento,
                    I.INF_DATA_EMISSAO DataEmissao
                    FROM T_INFRACAO I
                    JOIN T_FUNCIONARIO M ON M.FUN_CODIGO = I.FUN_CODIGO_MOTORISTA
					JOIN T_INFRACAO_PARCELA P ON P.INF_CODIGO = I.INF_CODIGO
                    LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = I.VEI_CODIGO
                    JOIN T_TIPO_INFRACAO T ON T.TIN_CODIGO = I.TIN_CODIGO
                    WHERE I.INF_SITUACAO <> 2";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND M.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
                query += " AND CAST(P.IFP_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicial.Date.ToString("MM/dd/yyyy") + "'";
            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
                query += " AND CAST(P.IFP_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinal.Date.ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(OcorrenciaComissaoAcertoViagem)));

            return nhQuery.SetTimeout(50000).List<OcorrenciaComissaoAcertoViagem>();
        }

        public IList<ComissaoAcertoViagem> RelatorioComissaoAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = @"SELECT A.ACV_NUMERO NumeroAcerto,
                        F.FUN_CODIGO CodigoMotorista,
						F.FUN_NOME Motorista,
						SUBSTRING((SELECT DISTINCT ', ' + V.VEI_PLACA
						FROM T_VEICULO V
						JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO
						WHERE V.VEI_TIPOVEICULO = '0' AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000) Cavalo,
						SUBSTRING((SELECT DISTINCT ', ' + V.VEI_PLACA
						FROM T_VEICULO V
						JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO
						WHERE V.VEI_TIPOVEICULO = '1' AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000) Reboques,
						SUBSTRING((SELECT DISTINCT ', ' + M.VMO_DESCRICAO
						FROM T_VEICULO V
						JOIN T_VEICULO_MODELO M ON M.VMO_CODIGO = V.VMO_CODIGO
						JOIN T_ACERTO_VEICULO AV ON AV.VEI_CODIGO = V.VEI_CODIGO
						WHERE V.VEI_TIPOVEICULO = '0' AND AV.ACV_CODIGO = A.ACV_CODIGO FOR XML PATH('')), 3, 1000) ModeloVeiculo,
						S.VSE_DESCRICAO Segmento,
						A.ACV_DATA_INICIAL DataInicialAcerto,
						A.ACV_DATA_FINAL DataFinalAcerto,
						R.AVR_MEDIA_TRACAO Media,
						ISNULL(R.AVR_COMBUSTIVEL_TRACAO, 0) +  ISNULL(R.AVR_COMBUSTIVEL_REBOQUES, 0) ConsumoCombustivel,
						R.AVR_FATURAMENTO_BRUTO ValorBruto,
						R.AVR_VALOR_BONIFICACAO Bonificacoes,
						R.AVR_VALOR_DESCONTO Descontos,
						R.AVR_PERCENTUAL_COMISSAO PercentualComissao,
						R.AVR_VALOR_COMISSAO ValorComissao			
                    FROM T_ACERTO_DE_VIAGEM A
					JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
					JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = A.VSE_CODIGO
					JOIN T_ACERTO_VEICULO_RESULTADO R ON R.ACV_CODIGO = A.ACV_CODIGO
                    WHERE A.ACV_SITUACAO = 2 ";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND S.VSE_CODIGO= " + filtrosPesquisa.CodigoSegmento.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if (filtrosPesquisa.CodigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + filtrosPesquisa.CodigoAcertoViagem.ToString();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.Date.ToString("MM/dd/yyyy") + "'";
            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.Date.ToString("MM/dd/yyyy") + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComissaoAcertoViagem)));

            return nhQuery.SetTimeout(50000).List<ComissaoAcertoViagem>();
        }

        public int ContarComissaoAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioComissaoAcertoViagem filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                   FROM T_ACERTO_DE_VIAGEM A
					JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO_MOTORISTA
					JOIN T_VEICULO_SEGMENTO S ON S.VSE_CODIGO = A.VSE_CODIGO
					JOIN T_ACERTO_VEICULO_RESULTADO R ON R.ACV_CODIGO = A.ACV_CODIGO
                    WHERE A.ACV_SITUACAO = 2 ";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND S.VSE_CODIGO= " + filtrosPesquisa.CodigoSegmento.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if (filtrosPesquisa.CodigoAcertoViagem > 0)
                query += " AND A.ACV_CODIGO = " + filtrosPesquisa.CodigoAcertoViagem.ToString();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_INICIAL AS DATE) >= '" + filtrosPesquisa.DataInicial.Date.ToString("MM/dd/yyyy") + "'";
            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_FINAL AS DATE) <= '" + filtrosPesquisa.DataFinal.Date.ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.TempoDeViagem> RelatorioTempoDeViagem(DateTime DataInicial, DateTime DataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, int codigoVeiculo, int codigoMotorista, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT F.FUN_NOME Motorista,
                            SUBSTRING((SELECT DISTINCT ', ' + v.VEI_PLACA
                            FROM T_VEICULO v	
                            join T_ACERTO_VEICULO av on av.VEI_CODIGO = v.VEI_CODIGO
                            WHERE av.ACV_CODIGO = a.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculo, 
                            SUBSTRING((SELECT DISTINCT ', ' + v.VEI_NUMERO_FROTA
                            FROM T_VEICULO v	
                            join T_ACERTO_VEICULO av on av.VEI_CODIGO = v.VEI_CODIGO
                            WHERE av.ACV_CODIGO = a.ACV_CODIGO FOR XML PATH('')), 3, 1000) Frota,
                            A.ACV_DATA_INICIAL DataSaida,
                            DATEDIFF(DAY, A.ACV_DATA_INICIAL, ISNULL(A.ACV_DATA_FINAL - 1, GETDATE())) TempoViagem
                            FROM T_FUNCIONARIO	F
                            JOIN T_ACERTO_DE_VIAGEM A ON A.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO
                            WHERE F.FUN_TIPO = 'M'";

            if (codigoVeiculo > 0)
                query += " AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculo.ToString() + ")"; // SQL-INJECTION-SAFE
            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();
            if (situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)situacao).ToString();
            if (DataInicial != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_INICIAL AS DATE) >= '" + DataInicial.Date.ToString("MM/dd/yyyy") + "'";
            if (DataFinal != DateTime.MinValue)
                query += " AND CAST(A.ACV_DATA_FINAL AS DATE) <= '" + DataFinal.Date.ToString("MM/dd/yyyy") + "'";


            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.TempoDeViagem)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.TempoDeViagem>();
        }

        public int ContarRelatorioTempoDeViagem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao, int codigoVeiculo, int codigoMotorista)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                               FROM T_FUNCIONARIO	F
                            JOIN T_ACERTO_DE_VIAGEM A ON A.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO
                            WHERE F.FUN_TIPO = 'M'";

            if (codigoVeiculo > 0)
                query += " AND A.ACV_CODIGO IN (SELECT V.ACV_CODIGO FROM T_ACERTO_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculo.ToString() + ")"; // SQL-INJECTION-SAFE
            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();
            if (situacao > 0)
                query += " AND A.ACV_SITUACAO = " + ((int)situacao).ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.JornadaMotorista> RelatorioJornadaMotorista(int codigoVeiculo, int codigoMotorista, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT F.FUN_NOME Motorista,
                            SUBSTRING((SELECT DISTINCT ', ' + v.VEI_PLACA
                            FROM T_VEICULO v	                                
                            WHERE v.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO FOR XML PATH('')), 3, 1000) Veiculo,
                            SUBSTRING((SELECT DISTINCT ', ' + v.VEI_NUMERO_FROTA
                            FROM T_VEICULO v	                                
                            WHERE v.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO FOR XML PATH('')), 3, 1000) Frota,
                            F.FUN_DIAS_TRABALHADO DiasEmViagem,
                            F.FUN_DIAS_FOLGA_RETIRADO DiasForaServico,
                            ISNULL(F.FUN_DIAS_TRABALHADO, 0) / 6 DiasPendentes,
                            ISNULL(F.FUN_SITUACAO_COLABORADOR, 6) SituacaoAtual,
                            ISNULL((SELECT TOP(1) A.ACV_NUMERO FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO AND A.ACV_SITUACAO = 2 ORDER BY A.ACV_NUMERO DESC), 0) UltimaViagem,
                            ISNULL((SELECT COUNT(1) FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = F.FUN_CODIGO AND A.ACV_SITUACAO = 1), 0) EmViagem
                            FROM T_FUNCIONARIO	F
                            WHERE F.FUN_TIPO = 'M' AND F.FUN_TIPO_MOTORISTA = 1 ";

            if (codigoVeiculo > 0)
                query += " AND F.FUN_CODIGO IN (SELECT ISNULL(V.FUN_CODIGO_MOTORISTA, 0) FROM T_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculo.ToString() + ")"; // SQL-INJECTION-SAFE
            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.JornadaMotorista)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.JornadaMotorista>();
        }

        public int ContarRelatorioJornadaMotorista(int codigoVeiculo, int codigoMotorista)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                                FROM T_FUNCIONARIO	F
                            WHERE F.FUN_TIPO = 'M' AND F.FUN_TIPO_MOTORISTA = 1 ";

            if (codigoVeiculo > 0)
                query += " AND F.FUN_CODIGO IN (SELECT ISNULL(V.FUN_CODIGO_MOTORISTA, 0) FROM T_VEICULO V WHERE V.VEI_CODIGO = " + codigoVeiculo.ToString() + ")"; // SQL-INJECTION-SAFE
            if (codigoMotorista > 0)
                query += " AND F.FUN_CODIGO = " + codigoMotorista.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Acertos.TotalMoedaEstrangeira DetalheMoedaEstrangeira(int codigoAcerto, int tipoDetalhe)
        {
            string query = "";

            if (tipoDetalhe == 1)
                query = @"SELECT 
                    SUM(A.ABA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(A.ABA_VALOR_UN * A.ABA_LITROS) TotalReais
                    FROM T_ACERTO_ABASTECIMENTO AA
                    JOIN T_ABASTECIMENTO A ON A.ABA_CODIGO = AA.ABA_CODIGO
                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                    WHERE A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABA_MOEDA_COTACAO_BANCO_CENTRAL > 0 ";
            else if (tipoDetalhe == 2)
                query = @"SELECT 
                    SUM(A.PED_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(A.PED_VALOR) TotalReais
                    FROM T_ACERTO_PEDAGIO AA
                    JOIN T_PEDAGIO A ON A.PED_CODIGO = AA.PED_CODIGO                    
                    WHERE A.PED_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PED_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 3)
                query = @"SELECT 
                    SUM(AA.AOD_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(CASE WHEN AA.AOD_QUANTIDADE > 0 THEN (AA.AOD_QUANTIDADE * AA.AOD_VALOR) ELSE AA.AOD_VALOR END) TotalReais
                    FROM T_ACERTO_OUTRA_DESPESA AA                    
                    WHERE AA.AOD_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND AOD_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 4)
                query = @"SELECT 
                    SUM(AA.ABO_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(AA.ABO_VALOR_BONIFICACAO) TotalReais
                    FROM T_ACERTO_BONIFICACAO AA                    
                    WHERE AA.ABO_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABO_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 5)
                query = @"SELECT 
                    SUM(A.PAM_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(A.PAM_VALOR) TotalReais
                    FROM T_ACERTO_ADIANTAMENTO AA
                    JOIN T_PAGAMENTO_MOTORISTA_TMS A ON A.PAM_CODIGO = AA.PAM_CODIGO                    
                    WHERE A.PAM_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PAM_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 6)
                query = @"SELECT 
                    SUM(AA.ADE_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) TotalMoeda,
                    SUM(AA.ADE_VALOR_DESCONTO) TotalReais
                    FROM T_ACERTO_DESCONTO AA                    
                    WHERE AA.ADE_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ADE_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA > 0";

            if (codigoAcerto > 0)
                query += " AND AA.ACV_CODIGO = " + codigoAcerto.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Acertos.TotalMoedaEstrangeira)));

            return nhQuery.SetTimeout(50000).List<Dominio.ObjetosDeValor.Embarcador.Acertos.TotalMoedaEstrangeira>().FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Acertos.DetalheMoedaEstrangeira> DetalheMoedaEstrangeira(int codigoAcerto, int tipoDetalhe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string query = "";

            if (tipoDetalhe == 1)
                query = @"SELECT AA.ABA_CODIGO Codigo,
                    A.ABA_DATA Data,
                    C.CLI_NOME Pessoa,
                    'KM: '+CAST(CAST(A.ABA_KM AS INT) AS VARCHAR(20))+' LITROS: '+REPLACE(CAST(A.ABA_LITROS AS VARCHAR(20)), '.', ',')+' DOCUMENTO: '+A.ABA_DOCUMENTO Descricao,
                    A.ABA_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    A.ABA_VALOR_MOEDA_COTACAO ValorMoeda,
                    A.ABA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    (A.ABA_VALOR_UN * A.ABA_LITROS) ValorReais
                    FROM T_ACERTO_ABASTECIMENTO AA
                    JOIN T_ABASTECIMENTO A ON A.ABA_CODIGO = AA.ABA_CODIGO
                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                    WHERE A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABA_MOEDA_COTACAO_BANCO_CENTRAL > 0 ";
            else if (tipoDetalhe == 2)
                query = @"SELECT AA.ACP_CODIGO Codigo,
                    A.PED_DATA Data,
                    A.PED_RODOVIA Pessoa,
                    A.PED_PRACA Descricao,
                    A.PED_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    A.PED_VALOR_MOEDA_COTACAO ValorMoeda,
                    A.PED_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    A.PED_VALOR ValorReais
                    FROM T_ACERTO_PEDAGIO AA
                    JOIN T_PEDAGIO A ON A.PED_CODIGO = AA.PED_CODIGO                    
                    WHERE A.PED_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PED_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 3)
                query = @"SELECT AA.AOD_CODIGO Codigo,
                    AA.AOD_DATA Data,
                    AA.AOD_FORNECEDOR Pessoa,
                    AA.AOD_OBSERVACAO Descricao,
                    AA.AOD_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    AA.AOD_VALOR_MOEDA_COTACAO ValorMoeda,
                    AA.AOD_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    CASE WHEN AA.AOD_QUANTIDADE > 0 THEN (AA.AOD_QUANTIDADE * AA.AOD_VALOR) ELSE AA.AOD_VALOR END ValorReais
                    FROM T_ACERTO_OUTRA_DESPESA AA                    
                    WHERE AA.AOD_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND AOD_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 4)
                query = @"SELECT AA.ABO_CODIGO Codigo,
                    AA.ABO_DATA Data,
                    'Bonificação' Pessoa,
                    AA.ABO_MOTIVO Descricao,
                    AA.ABO_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    AA.ABO_VALOR_MOEDA_COTACAO ValorMoeda,
                    AA.ABO_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    AA.ABO_VALOR_BONIFICACAO ValorReais
                    FROM T_ACERTO_BONIFICACAO AA                    
                    WHERE AA.ABO_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABO_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 5)
                query = @"SELECT AA.ADI_CODIGO Codigo,
                    A.PAM_DATA_PAGAMENTO Data,
                    'Adiantamento' Pessoa,
                    A.PAM_OBSERVACAO Descricao,
                    A.PAM_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    A.PAM_VALOR_MOEDA_COTACAO ValorMoeda,
                    A.PAM_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    A.PAM_VALOR ValorReais
                    FROM T_ACERTO_ADIANTAMENTO AA
                    JOIN T_PAGAMENTO_MOTORISTA_TMS A ON A.PAM_CODIGO = AA.PAM_CODIGO                    
                    WHERE A.PAM_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PAM_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 6)
                query = @"SELECT AA.ADE_CODIGO Codigo,
                    AA.ADE_DATA Data,
                    'Bonificação' Pessoa,
                    AA.ADE_MOTIVO Descricao,
                    AA.ADE_MOEDA_COTACAO_BANCO_CENTRAL Moeda,
                    AA.ADE_VALOR_MOEDA_COTACAO ValorMoeda,
                    AA.ADE_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorTotalMoeda,
                    AA.ADE_VALOR_DESCONTO ValorReais
                    FROM T_ACERTO_DESCONTO AA                    
                    WHERE AA.ADE_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ADE_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA > 0";

            if (codigoAcerto > 0)
                query += " AND AA.ACV_CODIGO = " + codigoAcerto.ToString();


            if (!string.IsNullOrWhiteSpace(propOrdenacao))
            {
                query += " order by " + propOrdenacao + " " + dirOrdenacao;

            }

            query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Acertos.DetalheMoedaEstrangeira)));

            return nhQuery.SetTimeout(50000).List<Dominio.ObjetosDeValor.Embarcador.Acertos.DetalheMoedaEstrangeira>();
        }

        public int ContarDetalheMoedaEstrangeira(int codigoAcerto, int tipoDetalhe)
        {
            string query = "";

            if (tipoDetalhe == 1)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_ABASTECIMENTO AA
                    JOIN T_ABASTECIMENTO A ON A.ABA_CODIGO = AA.ABA_CODIGO
                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                    WHERE A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABA_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 2)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_PEDAGIO AA
                    JOIN T_PEDAGIO A ON A.PED_CODIGO = AA.PED_CODIGO                    
                    WHERE A.PED_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PED_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 3)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_OUTRA_DESPESA AA                    
                    WHERE AA.AOD_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND AOD_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 4)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_BONIFICACAO AA                    
                    WHERE AA.ABO_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ABO_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 5)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_ADIANTAMENTO AA
                    JOIN T_PAGAMENTO_MOTORISTA_TMS A ON A.PAM_CODIGO = AA.PAM_CODIGO                    
                    WHERE A.PAM_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND PAM_MOEDA_COTACAO_BANCO_CENTRAL > 0";
            else if (tipoDetalhe == 6)
                query = @"SELECT COUNT(0) as CONTADOR 
                    FROM T_ACERTO_DESCONTO AA                    
                    WHERE AA.ADE_MOEDA_COTACAO_BANCO_CENTRAL IS NOT NULL AND ADE_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA > 0";

            if (codigoAcerto > 0)
                query += " AND AA.ACV_CODIGO = " + codigoAcerto.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion
    }
}