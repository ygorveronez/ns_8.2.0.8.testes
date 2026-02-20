using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoGestaoPatio : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>
    {
        #region Construtores

        public FluxoGestaoPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public FluxoGestaoPatio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

            consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga == null || !o.Carga.OcultarNoPatio);

            if (!filtrosPesquisa.ListarCargasCanceladas)
            {
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) || o.Carga == null);
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) || o.PreCarga == null);
            }

            if (filtrosPesquisa.NumeroNfProdutor > 0)
            {
                var queryGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                    .Where(obj => obj.NumeroNfProdutor == filtrosPesquisa.NumeroNfProdutor);

                List<int> codigosCarga = queryGuarita.Select(x => x.Carga.Codigo).ToList();
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(obj => codigosCarga.Contains(obj.Carga.Codigo));
            }

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
            {
                IQueryable<int> codigosCargaQuery = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(cp => cp.NotasFiscais.Any(nf => nf.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNotaFiscal))
                    .Select(cp => cp.Carga.Codigo)
                    .Distinct();

                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(fgp => codigosCargaQuery.Contains(fgp.Carga.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                string numeroCarga = filtrosPesquisa.NumeroCarga;
                bool existeEmAgrupados = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                    .Where(c => c.CodigosAgrupados.Contains(numeroCarga)).Select(a => a.Codigo).Any();

                if (existeEmAgrupados)
                    consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga || o.Carga.CodigosAgrupados.Contains(numeroCarga));
                else
                    consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PreCarga))
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.PreCarga.NumeroPreCarga == filtrosPesquisa.PreCarga);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
            {
                // consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Veiculo.Placa == filtrosPesquisa.Placa || o.Veiculo.VeiculosVinculados.Any(v => v.Placa == filtrosPesquisa.Placa));
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o =>
                    o.Carga != null && (o.Carga.Veiculo.Placa == filtrosPesquisa.Placa || o.Carga.VeiculosVinculados.Any(v => v.Placa == filtrosPesquisa.Placa)) ||
                    o.Carga == null && (o.PreCarga.Veiculo.Placa == filtrosPesquisa.Placa || o.PreCarga.VeiculosVinculados.Any(v => v.Placa == filtrosPesquisa.Placa))
                );
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido))
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.Pedido) || o.PreCarga.Pedidos.Any(p => p.NumeroPedidoEmbarcador == filtrosPesquisa.Pedido));

            if (filtrosPesquisa.EtapaFluxoGestaoPatio?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => filtrosPesquisa.EtapaFluxoGestaoPatio.Contains(o.EtapaFluxoGestaoPatioAtual));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.SituacaoEtapaFluxoGestaoPatio == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.Tipo.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.Carga.DataCarregamentoCarga.Value >= filtrosPesquisa.DataInicial.Value) || (o.PreCarga.DataPrevisaoEntrega.Value >= filtrosPesquisa.DataInicial.Value));

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.Carga.DataCarregamentoCarga.Value <= filtrosPesquisa.DataFinal.Value.AddSeconds(59)) || (o.PreCarga.DataPrevisaoEntrega.Value <= filtrosPesquisa.DataFinal.Value.AddSeconds(59)));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => filtrosPesquisa.CodigosTransportador.Contains(o.Carga.Empresa.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo) || filtrosPesquisa.CodigosTipoOperacao.Contains(o.PreCarga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador) || (o.PreCarga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador));

            if (filtrosPesquisa.CpfCnpjDestinatario?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.DadosSumarizados.ClientesDestinatarios.Any(d => filtrosPesquisa.CpfCnpjDestinatario.Contains(d.CPF_CNPJ)) || o.PreCarga.Pedidos.Any(p => filtrosPesquisa.CpfCnpjDestinatario.Contains(p.Destinatario.CPF_CNPJ)));

            if (filtrosPesquisa.CpfCnpjRemetente?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.DadosSumarizados.ClientesRemetentes.Any(d => filtrosPesquisa.CpfCnpjRemetente.Contains(d.CPF_CNPJ)) || o.PreCarga.Pedidos.Any(p => filtrosPesquisa.CpfCnpjRemetente.Contains(p.Remetente.CPF_CNPJ)));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo) || filtrosPesquisa.CodigosTipoCarga.Contains(o.PreCarga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.SomenteFluxosAbertos)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando);

            if (filtrosPesquisa.CodigosAreaVeiculo?.Count > 0)
            {
                var consultaCargaAreaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo>()
                    .Where(o => filtrosPesquisa.CodigosAreaVeiculo.Contains(o.AreaVeiculo.Codigo));

                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => consultaCargaAreaVeiculo.Any(c => c.Carga.Codigo == o.Carga.Codigo));
            }

            if (filtrosPesquisa.CodigosLocalCarregamento?.Count > 0)
            {
                var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento>()
                    .Where(o => filtrosPesquisa.CodigosLocalCarregamento.Contains(o.LocalCarregamento.Codigo));

                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => consultaDocaCarregamento.Any(d => d.FluxoGestaoPatio.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista) || o.PreCarga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga || o.PreCarga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);


            if (filtrosPesquisa.CodigoTipoCarregamento > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.Carga.TipoCarregamento.Codigo == filtrosPesquisa.CodigoTipoCarregamento || o.PreCarga.Carga.TipoCarregamento.Codigo == filtrosPesquisa.CodigoTipoCarregamento);

            if (filtrosPesquisa.DataInicialChegadaVeiculo.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.DataChegadaVeiculo.Value >= filtrosPesquisa.DataInicialChegadaVeiculo.Value));

            if (filtrosPesquisa.DataFinalChegadaVeiculo.HasValue)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => (o.DataChegadaVeiculo.Value <= filtrosPesquisa.DataFinalChegadaVeiculo.Value.AddSeconds(59)));

            return consultaFluxoGestaoPatio;
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> ParametroConsulta(IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> consultaFluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Take(parametrosConsulta.LimiteRegistros);

            return consultaFluxoGestaoPatio;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga> BuscarCategoriasPorCargas(List<int> listaCargas)
        {
            if (listaCargas == null || !listaCargas.Any())
                return new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga>();

            const string sql = @"
                SELECT 
                    fgp.CAR_CODIGO AS CargaCodigo,
                    cat.CTG_DESCRICAO AS CategoriaAgendamento
                FROM 
                    T_FLUXO_GESTAO_PATIO fgp
                INNER JOIN 
                    T_CARGA c ON fgp.CAR_CODIGO = c.CAR_CODIGO
                INNER JOIN 
                    T_AGENDAMENTO_COLETA agc ON c.CAR_CODIGO = agc.CAR_CODIGO
                INNER JOIN 
                    T_CATEGORIA cat ON agc.CTG_CODIGO = cat.CTG_CODIGO
                WHERE 
                    c.CAR_CODIGO IN (:ListaCargas)";

            return this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameterList("ListaCargas", listaCargas)
                .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga>())
                .List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga>()
                .ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga BuscarCategoriasPorCodigoFluxoPatio(int codigoFluxoPatio)
        {
            const string sql = @"
                SELECT 
                    fgp.CAR_CODIGO AS CargaCodigo,
                    cat.CTG_DESCRICAO AS CategoriaAgendamento
                FROM 
                    T_FLUXO_GESTAO_PATIO fgp
                INNER JOIN 
                    T_CARGA c ON fgp.CAR_CODIGO = c.CAR_CODIGO
                INNER JOIN 
                    T_AGENDAMENTO_COLETA agc ON c.CAR_CODIGO = agc.CAR_CODIGO
                INNER JOIN 
                    T_CATEGORIA cat ON agc.CTG_CODIGO = cat.CTG_CODIGO
                WHERE 
                    fgp.FGP_CODIGO = :CodigoFluxoPatio";

            return this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("CodigoFluxoPatio", codigoFluxoPatio)
                .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga>())
                .UniqueResult<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga>();
        }

        private string ObterSelectConsultaRelatorioFluxoHorario(string campo, DateTime dataInicial, DateTime dataFinal, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, int filial, TipoFluxoGestaoPatio? tipo)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append($"select datepart(HOUR, Fluxo.{campo}) Hora, "); // SQL-INJECTION-SAFE
            sql.Append($"       datepart(DAY, Fluxo.{campo}) Dia, ");
            sql.Append("        count(*) Quantidade ");
            sql.Append("   from T_FLUXO_GESTAO_PATIO_ETAPAS Etapa ");
            sql.Append("   join T_FLUXO_GESTAO_PATIO Fluxo on Fluxo.FGP_CODIGO = Etapa.FGP_CODIGO ");
            sql.Append("   join T_CARGA Carga ON Carga.CAR_CODIGO = Fluxo.CAR_CODIGO ");
            sql.Append($" where Etapa.FGE_ETAPA_FLUXO_GESTAO = {(int)etapaFluxoGestaoPatio} ");
            sql.Append($"   and Fluxo.{campo} >= '{dataInicial.Date.ToString("yyyyMMdd HH:mm:ss")}' ");
            sql.Append($"   and Fluxo.{campo} <= '{dataFinal.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");
            sql.Append("    and Carga.CAR_CARGA_FECHADA = 1 ");

            if (filial > 0)
                sql.Append($"   and Fluxo.FIL_CODIGO = {filial} ");

            if (tipo.HasValue)
                sql.Append($"   and Fluxo.FGE_TIPO = {(int)tipo.Value} ");

            sql.Append($" group by datepart(HOUR, Fluxo.{campo}), ");
            sql.Append($"       datepart(DAY, Fluxo.{campo}), ");
            sql.Append($"       cast(Fluxo.{campo} AS DATE) ");

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos 

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarCodigosPentendesInicioViagemPorFimFluxoPatio(int maximoRegistros, DateTime dataComparacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

            query = query.Where(o => o.Carga.CargaFechada
            && !o.Carga.DataInicioViagem.HasValue
            && o.DataFinalizacaoFluxo.HasValue && o.DataFinalizacaoFluxo <= dataComparacao
            && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return query.Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarPorCarga(int codigoCarga)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorUnicaCarga(int codigoCarga)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorCargaETipo(int codigoCarga, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarPorCargaETipoAsync(int codigoCarga, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return await consultaFluxoGestaoPatio.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorCargaFilialETipo(int codigoCarga, int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Filial.Codigo == codigoFilial &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarPorCargaAgrupadaETipo(int codigoCargaAgrupada, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.CargaAgrupamento.Codigo == codigoCargaAgrupada &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.Carga.Codigo != codigoCargaAgrupada
                );

            return consultaFluxoGestaoPatio.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarPorCargas(List<int> cargas)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                     cargas.Contains(o.Carga.Codigo) &&
                     o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                     o.Tipo == TipoFluxoGestaoPatio.Origem
                );

            return consultaFluxoGestaoPatio.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorPreCargaETipo(int codigoPreCarga, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.PreCarga.Codigo == codigoPreCarga &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorPreCargaFilialETipo(int codigoPreCarga, int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.PreCarga.Codigo == codigoPreCarga &&
                    o.Filial.Codigo == codigoFilial &&
                    o.Tipo == tipo &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorEtapaVeiculoEFilial(EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, int codigoVeiculo, int codigoFilial)
        {
            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo() { Codigo = codigoVeiculo };

            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.EtapaFluxoGestaoPatioAtual == etapaFluxoGestaoPatio &&
                    o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    (o.Filial == null || o.Filial.Codigo == codigoFilial) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && (o.Carga.Veiculo.Codigo == codigoVeiculo || o.Carga.VeiculosVinculados.Contains(veiculo))) ||
                        (o.Carga == null && (o.PreCarga.Veiculo.Codigo == codigoVeiculo || o.PreCarga.VeiculosVinculados.Contains(veiculo)))
                    ) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada) ||
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                    )
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>> BuscarPorEtapaEPlacaVeiculo(EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, string placa, SituacaoEtapaFluxoGestaoPatio situacao, CancellationToken cancellationToken)
        {
            var consultaFluxoGestaoPatio = MontarBuscaPorEtapaEPlacaVeiculo(etapaFluxoGestaoPatio, placa).Where(o => o.SituacaoEtapaFluxoGestaoPatio == situacao);

            return await consultaFluxoGestaoPatio.ToListAsync(cancellationToken);
        }

        public IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> MontarBuscaPorEtapaEPlacaVeiculo(EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, string placa)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.EtapaFluxoGestaoPatioAtual == etapaFluxoGestaoPatio &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && (o.Carga.Veiculo.Placa == placa)) ||
                        (o.Carga == null && (o.PreCarga.Veiculo.Placa == placa))
                    ) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada) ||
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                    )
                ).OrderBy(x => x.Codigo);
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorVeiculoFilialMotorista(int codigoVeiculo, int codigoFilial, int codigoMotorista)
        {
            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo() { Codigo = codigoVeiculo };
            Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario() { Codigo = codigoMotorista };

            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    (o.Filial == null || o.Filial.Codigo == codigoFilial) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && (o.Carga.Veiculo.Codigo == codigoVeiculo || o.Carga.VeiculosVinculados.Contains(veiculo))) ||
                        (o.Carga == null && (o.PreCarga.Veiculo.Codigo == codigoVeiculo || o.PreCarga.VeiculosVinculados.Contains(veiculo)))
                    ) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada) ||
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                    ) &&
                    (
                        (o.Carga != null && o.Carga.CargaFechada && o.Carga.Motoristas.Contains(motorista)) ||
                        (o.Carga == null && o.PreCarga.Motoristas.Contains(motorista))
                    )
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarFluxosDestinoPorCarga(int codigoCarga)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Tipo == TipoFluxoGestaoPatio.Destino &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio
                .Fetch(o => o.Filial)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarFluxosPendentesDeIntegracao(int inicioRegistros, int maximoRegistros)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
               .Where(o => o.PendenteIntegracao == true);

            return consultaFluxoGestaoPatio
                .Fetch(obj => obj.Carga)
                .Skip(inicioRegistros)
                .Take(maximoRegistros).ToList();
        }

        public int ContarFluxosAgIntegracao()
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
               .Where(o => o.PendenteIntegracao == true);

            return consultaFluxoGestaoPatio.Count();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarIntegracaoTemperatura()
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
               .Where(o => o.DataFinalizacaoFluxo != null);

            return consultaFluxoGestaoPatio.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarAbertoPorCarga(int codigoCarga)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Carga.CargaFechada &&
                    o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    o.Tipo == TipoFluxoGestaoPatio.Origem
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorVeiculoECarga(int veiculo, bool apenasFluxoAtivo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Veiculo.Codigo == veiculo &&
                    o.Carga.CargaFechada &&
                    o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    o.Tipo == TipoFluxoGestaoPatio.Origem
                );

            if (apenasFluxoAtivo)
                consultaFluxoGestaoPatio = consultaFluxoGestaoPatio.Where(o => o.VeiculoAtivo == true);

            return consultaFluxoGestaoPatio
                .OrderBy("Carga.DataCarregamentoCarga descending")
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFluxoGestaoPatio = Consultar(filtrosPesquisa);

            consultaFluxoGestaoPatio = ParametroConsulta(consultaFluxoGestaoPatio, parametrosConsulta);

            return consultaFluxoGestaoPatio
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Veiculo).ThenFetch(o => o.ModeloVeicularCarga)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFluxoGestaoPatio = Consultar(filtrosPesquisa);

            consultaFluxoGestaoPatio = ParametroConsulta(consultaFluxoGestaoPatio, parametrosConsulta);

            return consultaFluxoGestaoPatio
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Veiculo).ThenFetch(o => o.ModeloVeicularCarga)
                .ToListAsync(CancellationToken);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa)
        {
            var consultaFluxoGestaoPatio = Consultar(filtrosPesquisa);

            return consultaFluxoGestaoPatio.Count();
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa)
        {
            var consultaFluxoGestaoPatio = Consultar(filtrosPesquisa);

            return consultaFluxoGestaoPatio.CountAsync(CancellationToken);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TemposGestaoPatio> ConsultarRelatorioTemposGestaoPatio(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaTemposGestaoPatio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TemposGestaoPatio)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.TemposGestaoPatio>();
        }

        public int ContarConsultaRelatorioTemposGestaoPatio(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRelatorioTemposGestaoPatio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaTemposGestaoPatio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario> ConsultarRelatorioFluxoHorario(string campo, DateTime dataInicial, DateTime dataFinal, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, int filial, TipoFluxoGestaoPatio? tipo)
        {
            string sql = ObterSelectConsultaRelatorioFluxoHorario(campo, dataInicial, dataFinal, etapaFluxoGestaoPatio, filial, tipo);
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaFluxoGestaoPatio.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario)));

            return consultaFluxoGestaoPatio.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario>();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorVeiculoDisponibilidade(int codigoPreCargaDesconsiderar, int codigoVeiculo)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    o.Carga.Veiculo.Codigo == codigoVeiculo &&
                    o.PreCarga.Codigo != codigoPreCargaDesconsiderar
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public async Task<List<int>> BuscarCodigoFluxoPorEtapaAsync(EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, CancellationToken cancellationToken)
        {
            List<SituacaoCarga> situacoesCargaValidas = new List<SituacaoCarga> { SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>();
            var result = from obj in query
                         where
                            obj.EtapaFluxoGestaoPatio == etapaFluxoGestaoPatio &&
                            obj.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                            situacoesCargaValidas.Contains(obj.FluxoGestaoPatio.Carga.SituacaoCarga) &&
                            obj.FluxoGestaoPatio.Carga.CargaFechada == true
                         select obj.FluxoGestaoPatio.Codigo;

            return await result.ToListAsync(cancellationToken);
        }

        public async Task<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> BuscarPorCodigoComDadosIntegracaoAsync(int codigo, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(obj => obj.Codigo == codigo)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Veiculo)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Motoristas)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Empresa);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public bool ExisteFluxoDeOrigemNaoFinalizadoPorCarga(int codigoCarga)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.DataFinalizacaoFluxo == null
                );

            return consultaFluxoGestaoPatio.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarPorCargaEtapaETipo(int codigoCarga, TipoFluxoGestaoPatio tipo, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Tipo == tipo &&
                    o.EtapaFluxoGestaoPatioAtual == etapaFluxoGestaoPatio &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio BuscarFluxoEmAbertoPorEquipamento(string numeroEquipamento)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Equipamento.Numero == numeroEquipamento &&
                    o.DataFinalizacaoFluxo == null
                );

            return consultaFluxoGestaoPatio.FirstOrDefault();
        }

        public bool ExisteEquipamentoEmUsoFluxoPatio(int codigoEquipamento)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(fluxoPatio => fluxoPatio.Equipamento.Codigo == codigoEquipamento && fluxoPatio.DataFinalizacaoFluxo == null
            );

            return consultaFluxoGestaoPatio.Any();
        }

        public Task<TipoIntegracao> BuscarTipoIntegracaoConfiguradoSequenciaGestaoPatioPorFluxoAsync(int codigoFluxo, TipoFluxoGestaoPatio tipo, CancellationToken cancellationToken)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(seq => seq.Tipo == tipo && this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                    .Any(fgp =>
                        fgp.Codigo == codigoFluxo &&
                        fgp.Filial.Codigo == seq.Filial.Codigo &&
                        fgp.Carga.TipoOperacao.Codigo == seq.TipoOperacao.Codigo
                    )
                ).Select(x => x.InformarDocaCarregamentoTipoIntegracao).FirstOrDefaultAsync(cancellationToken);

            return consultaFluxoGestaoPatio;
        }

        public DateTime? BuscarDataDocaInformadaPorCarga(int codigoCarga)
        {
            var consultaDocaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(fluxopatio => fluxopatio.Carga.Codigo == codigoCarga)
                .Select(x => x.DataDocaInformada).FirstOrDefault();
            return consultaDocaCarregamento;
        }

        #endregion
    }
}
