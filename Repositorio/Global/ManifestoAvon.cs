using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ManifestoAvon : RepositorioBase<Dominio.Entidades.ManifestoAvon>, Dominio.Interfaces.Repositorios.ManifestoAvon
    {
        public ManifestoAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ManifestoAvon BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ManifestoAvon BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ManifestoAvon> BuscarPorCodigo(int[] codigos, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.ManifestoAvon BuscarPorNumero(string numero, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto tipoIntegradora, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Numero.Equals(numero) && obj.Empresa.Codigo == codigoEmpresa && obj.TipoIntegradora == tipoIntegradora select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ManifestoAvon> BuscarPorStatus(Dominio.Enumeradores.StatusManifestoAvon status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Status == status select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoAvon> BuscarPorStatusValidarEnvioRetorno()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusManifestoAvon.Emitido || obj.Status == Dominio.Enumeradores.StatusManifestoAvon.FalhaNoRetorno select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ManifestoAvon> Consultar(int codigoEmpresa, string numero, DateTime dataInicial, DateTime dataFinal, int numeroCTe, int inicioRegistros, int maximoRegistros, Dominio.Enumeradores.StatusManifestoAvon? status = null, bool naoExistenteEmFaturas = false, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto? integradora = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (integradora.HasValue)
                result = result.Where(o => o.TipoIntegradora == integradora.Value);

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Contains(numero));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (status.HasValue)
                result = result.Where(o => o.Status == status.Value);

            if (naoExistenteEmFaturas)
                result = result.Where(o => o.Faturas.Where(fat => fat.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Cancelada).Count() <= 0);

            if (numeroCTe > 0)
            {
                var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

                result = result.Where(o => (from obj in queryCTe where obj.Manifesto.Codigo == o.Codigo && obj.CTe.Numero == numeroCTe select obj.Manifesto.Codigo).Contains(o.Codigo));
            }

            return result.OrderByDescending(o => o.Codigo)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string numero, DateTime dataInicial, DateTime dataFinal, int numeroCTe, Dominio.Enumeradores.StatusManifestoAvon? status = null, bool naoExistenteEmFaturas = false, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto? integradora = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (integradora.HasValue)
                result = result.Where(o => o.TipoIntegradora == integradora.Value);

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Contains(numero));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            if (status.HasValue)
                result = result.Where(o => o.Status == status.Value);

            if (naoExistenteEmFaturas)
                result = result.Where(o => o.Faturas.Where(fat => fat.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Cancelada).Count() <= 0);

            if (numeroCTe > 0)
            {

            }

            return result.Count();
        }

        public decimal BuscarValorTotalPedagioCTes(int codigoManifesto)
        {
            var queryDocumentosManifesto = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();
            var queryComponentesCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ComponentePrestacaoCTE>();

            var result = from componente in queryComponentesCTe
                         join
                             documento in queryDocumentosManifesto on
                             componente.CTE.Codigo equals documento.CTe.Codigo
                         where documento.Manifesto.Codigo == codigoManifesto &&
                               componente.NomeCTe.Contains("Pedágio")
                         select componente.Valor;

            return result.Sum(x => (decimal?)x) ?? 0m;
        }

        public decimal BuscarPesoTotalCTes(int codigoManifesto)
        {
            var queryDocumentosManifesto = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();
            var queryInformacaoCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.InformacaoCargaCTE>();

            var result = from peso in queryInformacaoCargaCTe
                         join
                             documento in queryDocumentosManifesto on
                             peso.CTE.Codigo equals documento.CTe.Codigo
                         where documento.Manifesto.Codigo == codigoManifesto
                         select peso.Quantidade;

            return result.Sum(x => (decimal?)x) ?? 0m;
        }

        public int BuscarNumeroFatura(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Manifestos.Contains(new Dominio.Entidades.ManifestoAvon() { Codigo = codigoManifesto }) select obj.Numero;

            return result.FirstOrDefault();
        }

        public KeyValuePair<string, string> BuscarDestinoManifesto(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoAvon>();

            var result = from obj in query where obj.Codigo == codigoManifesto select new KeyValuePair<string, string>(obj.Documentos.Select(o => o.CTe.LocalidadeTerminoPrestacao.Descricao).First(), obj.Documentos.Select(o => o.CTe.LocalidadeTerminoPrestacao.Estado.Sigla).First());

            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioManifestosEmitidosAvon> Relatorio(int codigoEmpresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int codigoMotorista, int codigoVeiculo, string numeroManifesto, Dominio.Enumeradores.StatusManifestoAvon? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Empresa.Codigo == codigoEmpresa select obj;

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTe.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTe.DataEmissao < dataEmissaoFinal.AddDays(1).Date);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Manifesto.Motorista.Codigo == codigoMotorista);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Manifesto.Veiculo.Codigo == codigoVeiculo);

            if (!string.IsNullOrWhiteSpace(numeroManifesto))
                result = result.Where(o => o.Manifesto.Numero.Equals(numeroManifesto));

            if (status.HasValue)
                result = result.Where(o => o.Manifesto.Status == status.Value);

            return result.GroupBy(documento => new
            {
                CodigoManifesto = documento.Manifesto.Codigo,
                NumeroManifesto = documento.Manifesto.Numero,
                Placa = documento.Manifesto.Veiculo.Placa,
                Motorista = documento.Manifesto.Motorista.Nome,
                Frota = documento.Manifesto.Veiculo.NumeroFrota,
                Proprietario = documento.Manifesto.Veiculo.Proprietario.Nome,
                CidadeDestino = documento.Manifesto.TabelaFrete.CidadeDestino.Descricao,
                UFDestino = documento.Manifesto.TabelaFrete.CidadeDestino.Estado.Sigla,
                DataEmissao = documento.Manifesto.DataEmissao
            }).Select(g => new Dominio.ObjetosDeValor.Relatorios.RelatorioManifestosEmitidosAvon()
            {
                CodigoManifesto = g.Key.CodigoManifesto,
                NumeroManifesto = g.Key.NumeroManifesto,
                Placa = g.Key.Placa,
                Motorista = g.Key.Motorista,
                ValorFrete = g.Sum(o => o.CTe.ValorFrete),
                ValorICMS = g.Sum(o => o.CTe.ValorICMS),
                ValorAReceber = g.Sum(o => o.CTe.ValorAReceber),
                QuantidadeCTes = g.Count(),
                NumeroFrota = g.Key.Frota,
                Proprietario = g.Key.Proprietario,
                CidadeDestino = g.Key.CidadeDestino,
                UFDestino = g.Key.UFDestino,
                ValorPedagio = 0,
                DataEmissao = g.Key.DataEmissao,
                NumeroInicialCTe = g.Min(o => o.CTe.Numero),
                NumeroFinalCTe = g.Max(o => o.CTe.Numero),
                NumeroFatura = 0,
                PesoCargaCTe = 0,
                ValorMercadoria = g.Sum(o => o.CTe.ValorTotalMercadoria)
            }).Timeout(240).ToList();
        }

        #region Relatório de Minutas

        public IList<Dominio.Relatorios.Embarcador.DataSource.Minutas.Minuta> ConsultarRelatorioMinutas(int codigoFatura, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicialMinuta, DateTime dataFinalMinuta, int codigoMotorista, string estadoOrigem, string estadoDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora, Dominio.Enumeradores.TipoAmbiente ambiente, string tipoPropriedadeVeiculo, bool? situacaoIntegracao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioMinutas(codigoFatura, false, propriedades, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, ambiente, tipoPropriedadeVeiculo, situacaoIntegracao, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Minutas.Minuta)));

            return query.SetTimeout(1000).List<Dominio.Relatorios.Embarcador.DataSource.Minutas.Minuta>();
        }

        public int ContarConsultaRelatorioMinutas(int codigoFatura, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicialMinuta, DateTime dataFinalMinuta, int codigoMotorista, string estadoOrigem, string estadoDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora, Dominio.Enumeradores.TipoAmbiente ambiente, string tipoPropriedadeVeiculo, bool? situacaoIntegracao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioMinutas(codigoFatura, true, propriedades, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, ambiente, tipoPropriedadeVeiculo, situacaoIntegracao, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarQuery(this.SessionNHiBernate);

            return query.SetTimeout(1000).UniqueResult<int>();
        }

        private SQLDinamico ObterSelectConsultaRelatorioMinutas(int codigoFatura, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicialMinuta, DateTime dataFinalMinuta, int codigoMotorista, string estadoOrigem, string estadoDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora, Dominio.Enumeradores.TipoAmbiente ambiente, string tipoPropriedadeVeiculo, bool? situacaoIntegracao, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty;
            var parametros = new List<ParametroSQL>();


            //SELECT NATURA

            if (!tipoIntegradora.HasValue || tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura)
            {
                string selectNatura = string.Empty,
                       groupByNatura = string.Empty,
                       joinsNatura = string.Empty,
                       whereNatura = string.Empty,
                       havingNatura = string.Empty;


                for (var i = propriedades.Count - 1; i >= 0; i--)
                    SetarSelectConsultaRelatorioMinutas(codigoFatura, propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref selectNatura, ref groupByNatura, ref joinsNatura, count, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura);

                SetarWhereConsultaRelatorioMinutas(codigoFatura, ref havingNatura, ref whereNatura, ref joinsNatura,ref parametros, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, ambiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura, tipoPropriedadeVeiculo, situacaoIntegracao);

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    SetarSelectConsultaRelatorioMinutas(codigoFatura, propAgrupa, 0, ref selectNatura, ref groupByNatura, ref joinsNatura, count, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura);

                select += (count ? "select distinct(count(0) over ())" : "select TipoIntegradora = 1 " + (selectNatura.Length > 0 ? ", " + selectNatura.Substring(0, selectNatura.Length - 2) : string.Empty)) +
                        " from T_CARGA_INTEGRACAO_NATURA CargaIntegracaoNatura left join T_INTEGRACAO_NATURA_DOCUMENTO_TRANSPORTE DocumentoTransporte on DocumentoTransporte.IDT_CODIGO = CargaIntegracaoNatura.IDT_CODIGO left join T_CARGA Carga on CargaIntegracaoNatura.CAR_CODIGO = Carga.CAR_CODIGO left join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO left join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO " + joinsNatura +
                        " where CTe.CON_STATUS = 'A' and Carga.CAR_SITUACAO in (15, 11, 10, 9, 8, 7) and CargaCTe.CCC_CODIGO is null " + whereNatura +
                        (groupByNatura.Length > 0 ? " group by Carga.CAR_CODIGO, " + groupByNatura.Substring(0, groupByNatura.Length - 2) : "") +
                        (havingNatura.Length > 0 ? " having " + havingNatura : string.Empty);
            }

            //FIM SELECT NATURA

            if (!tipoIntegradora.HasValue)
                if (!count)
                    select += " union all ";
                else
                    select += "), 0) + isnull((";

            //SELECT AVON

            if (!tipoIntegradora.HasValue || tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon)
            {
                string selectAvon = string.Empty,
                   groupByAvon = string.Empty,
                   joinsAvon = string.Empty,
                   whereAvon = string.Empty,
                   havingAvon = string.Empty;

                for (var i = propriedades.Count - 1; i >= 0; i--)
                    SetarSelectConsultaRelatorioMinutas(codigoFatura, propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref selectAvon, ref groupByAvon, ref joinsAvon, count, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon);

                SetarWhereConsultaRelatorioMinutas(codigoFatura, ref havingAvon, ref whereAvon, ref joinsAvon, ref parametros, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, ambiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon, tipoPropriedadeVeiculo, situacaoIntegracao);

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    SetarSelectConsultaRelatorioMinutas(codigoFatura, propAgrupa, 0, ref selectAvon, ref groupByAvon, ref joinsAvon, count, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon);

                select += (count ? "select distinct(count(0) over ())" : "select TipoIntegradora = 2 " + (selectAvon.Length > 0 ? ", " + selectAvon.Substring(0, selectAvon.Length - 2) : string.Empty)) +
                        " from T_CARGA_INTEGRACAO_AVON DocumentoTransporte inner join T_CARGA Carga on Carga.CAR_CODIGO = DocumentoTransporte.CAR_CODIGO left join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO left join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO " + joinsAvon +
                        " where CTe.CON_STATUS = 'A' and Carga.CAR_SITUACAO in (15, 11, 10, 9, 8, 7) and DocumentoTransporte.CIA_SITUACAO = 1 and CargaCTe.CCC_CODIGO is null " + whereAvon +
                        (groupByAvon.Length > 0 ? " group by Carga.CAR_CODIGO, " + groupByAvon.Substring(0, groupByAvon.Length - 2) : "") +
                        (havingAvon.Length > 0 ? " having " + havingAvon : string.Empty);
            }

            //FIM SELECT AVON

            string orderBy = string.Empty;

            if (!string.IsNullOrWhiteSpace(propAgrupa))
            {
                if (propAgrupa == "DescricaoStatus")
                    propAgrupa = "Status";

                orderBy = " order by " + propAgrupa + " " + dirAgrupa;
            }

            if (!string.IsNullOrWhiteSpace(propOrdena))
            {
                if (propOrdena == "DescricaoStatus")
                    propOrdena = "Status";

                if (propOrdena != propAgrupa && select.Contains(propOrdena))
                    orderBy += (orderBy.Length <= 0 ? " order by " : ", ") + propOrdena + " " + dirOrdena;
            }

            select += (count ? string.Empty : (orderBy.Length > 0 ? orderBy : " order by 1 asc ")) +
                      (count || (inicio <= 0 && limite <= 0) ? string.Empty : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");

            if (count)
                select = "select (isnull((" + select + "), 0))"; 

            return new SQLDinamico(select, parametros);
        }

        private void SetarSelectConsultaRelatorioMinutas(int codigoFatura, string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta tipoIntegradora)
        {
            switch (propriedade)
            {
                case "QuantidadeCTesIntegrados":
                    if (!select.Contains("QuantidadeCTesIntegrados"))
                    {
                        select += "(select count(CCI_CODIGO) from t_carga_cte_integracao inner join t_tipo_integracao on t_carga_cte_integracao.TPI_CODIGO = T_TIPO_INTEGRACAO.TPI_CODIGO inner join t_carga_cte on t_carga_cte.CCT_CODIGO = t_carga_cte_integracao.CCT_CODIGO where t_carga_cte.CAR_CODIGO = Carga.CAR_CODIGO and t_carga_cte_integracao.INT_SITUACAO_INTEGRACAO = 1 and T_TIPO_INTEGRACAO.TPI_TIPO in (6,7)) QuantidadeCTesIntegrados, ";
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador,"))
                        select += "substring((select ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR FROM T_CARGA_PEDIDO CargaPedido INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')), 3, 1000) NumeroPedidoEmbarcador,";
                    break;
                case "NumeroCTes":
                    if (!select.Contains("NumeroCTes"))
                    {
                        select += "convert(nvarchar(15), min(CTe.CON_NUM)) + '-' + convert(nvarchar(15), max(CTe.CON_NUM)) NumeroCTes, ";
                    }
                    break;
                case "Peso":
                    if (!select.Contains("Peso"))
                    {
                        select += "CargaDadosSumarizados.CDS_PESO_TOTAL Peso, ";
                        groupBy += "CargaDadosSumarizados.CDS_PESO_TOTAL, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "AliquotaICMS":
                    if (!select.Contains("AliquotaICMS"))
                    {
                        select += "min(CTe.CON_ALIQ_ICMS) AliquotaICMS, ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        if (!joins.Contains("GrupoPessoas"))
                            joins += @"left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo"))
                    {
                        select += "((select vei.VEI_PLACA from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Veiculo, ";
                        groupBy += "Carga.CAR_VEICULO, ";
                    }
                    break;
                case "PropriedadeVeiculo":
                    if (!select.Contains("PropriedadeVeiculo"))
                    {
                        select += "CASE Veiculo.VEI_TIPO WHEN 'T' THEN 'Terceiros' WHEN 'P' THEN 'Próprio' ELSE 'Não informado' END PropriedadeVeiculo, ";
                        groupBy += "Veiculo.VEI_TIPO, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += "left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ";
                    }
                    break;
                case "ProprietarioVeiculo":
                    if (!select.Contains("ProprietarioVeiculo"))
                    {
                        select += "ProprietarioVeiculo.CLI_NOME ProprietarioVeiculo, ";
                        groupBy += "ProprietarioVeiculo.CLI_NOME, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += "left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ";

                        if (!joins.Contains(" ProprietarioVeiculo "))
                            joins += "left outer join T_CLIENTE ProprietarioVeiculo on ProprietarioVeiculo.CLI_CGCCPF = Veiculo.VEI_PROPRIETARIO ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains("Transportador"))
                    {
                        select += "Transportador.EMP_RAZAO Transportador, ";
                        groupBy += "Transportador.EMP_RAZAO, ";

                        if (!joins.Contains(" Transportador "))
                            joins += "inner join T_EMPRESA Transportador on Carga.EMP_CODIGO = Transportador.EMP_CODIGO ";
                    }
                    break;
                case "LocalidadeOrigem":
                    if (!select.Contains("LocalidadeOrigem"))
                    {
                        select += "CargaDadosSumarizados.CDS_ORIGENS LocalidadeOrigem, ";
                        groupBy += "CargaDadosSumarizados.CDS_ORIGENS, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "EstadoOrigem":
                    if (!select.Contains("EstadoOrigem"))
                    {
                        select += "CargaDadosSumarizados.CDS_UF_ORIGENS EstadoOrigem, ";
                        groupBy += "CargaDadosSumarizados.CDS_UF_ORIGENS, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeDestino":
                    if (!select.Contains("LocalidadeDestino"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINOS LocalidadeDestino, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINOS, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "EstadoDestino":
                    if (!select.Contains("EstadoDestino"))
                    {
                        select += "CargaDadosSumarizados.CDS_UF_DESTINOS EstadoDestino, ";
                        groupBy += "CargaDadosSumarizados.CDS_UF_DESTINOS, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente"))
                    {
                        select += "CargaDadosSumarizados.CDS_REMETENTES Remetente, ";
                        groupBy += "CargaDadosSumarizados.CDS_REMETENTES, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains("Destinatario"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINATARIOS, ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += "left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains("Motorista"))
                    {
                        select += "SUBSTRING((SELECT ', ' + motorista1.FUN_NOME FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motorista, ";
                    }
                    break;
                case "NumeroFatura":
                    if (!select.Contains("NumeroFatura"))
                    {
                        if (tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon)
                        {
                            select += "isnull(SUBSTRING((SELECT ', ' + CONVERT(nvarchar(15), fatura1.FAT_NUMERO) FROM T_FATURA_CARGA faturaCarga1 INNER JOIN T_FATURA fatura1 ON faturaCarga1.FAT_CODIGO = fatura1.FAT_CODIGO WHERE fatura1.FAT_SITUACAO <> 3 AND faturaCarga1.FAC_STATUS <> 3 AND faturaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), '') + " +
                                "isnull(SUBSTRING((SELECT ', ' + CONVERT(nvarchar(15), fatura1.FAT_NUMERO) FROM T_FATURA_DOCUMENTO faturaDocumento INNER JOIN T_DOCUMENTO_FATURAMENTO documentoFaturamento ON faturaDocumento.DFA_CODIGO = documentoFaturamento.DFA_CODIGO INNER JOIN T_FATURA fatura1 ON faturaDocumento.FAT_CODIGO = fatura1.FAT_CODIGO WHERE fatura1.FAT_SITUACAO <> 3 AND documentoFaturamento.DFA_TIPO_DOCUMENTO = 2 and documentoFaturamento.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), '') NumeroFatura, ";
                        }
                        else
                        {
                            select += "isnull(SUBSTRING((SELECT ', ' + CONVERT(nvarchar(15), fatura1.FAT_NUMERO) FROM T_FATURA_CARGA faturaCarga1 INNER JOIN T_FATURA fatura1 ON faturaCarga1.FAT_CODIGO = fatura1.FAT_CODIGO WHERE fatura1.FAT_SITUACAO <> 3 AND faturaCarga1.FAC_STATUS <> 3 AND faturaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), '') + " +
                                "isnull(SUBSTRING((SELECT distinct ', ' + CONVERT(nvarchar(15), fatura1.FAT_NUMERO) FROM T_FATURA_DOCUMENTO faturaDocumento INNER JOIN T_DOCUMENTO_FATURAMENTO documentoFaturamento ON faturaDocumento.DFA_CODIGO = documentoFaturamento.DFA_CODIGO INNER JOIN T_CARGA_CTE cargacte1 ON cargacte1.CON_CODIGO = documentoFaturamento.CON_CODIGO INNER JOIN T_FATURA fatura1 ON faturaDocumento.FAT_CODIGO = fatura1.FAT_CODIGO WHERE fatura1.FAT_SITUACAO <> 3 AND documentoFaturamento.DFA_TIPO_DOCUMENTO = 1 and cargacte1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), '')  NumeroFatura, ";
                        }
                    }
                    break;
                case "SituacaoIntegracaoFatura":
                    if (!select.Contains("SituacaoIntegracaoFatura"))
                    {
                        select += "(SELECT (CASE WHEN COUNT(FAI_CODIGO) = 0 THEN 'Não Integrada' WHEN COUNT(FAI_CODIGO) = NULL THEN 'Não Integrada' WHEN COUNT(FAI_CODIGO) > 0 THEN 'Integrada' END) FROM T_FATURA_CARGA faturaCarga1 INNER JOIN T_FATURA fatura1 ON faturaCarga1.FAT_CODIGO = fatura1.FAT_CODIGO INNER JOIN T_FATURA_INTEGRACAO faturaIntegracao1 on faturaIntegracao1.FAT_CODIGO = fatura1.FAT_CODIGO WHERE faturaIntegracao1.FAI_SITUACAO = 1 AND fatura1.FAT_SITUACAO <> 3 AND faturaCarga1.FAC_STATUS <> 3 AND faturaCarga1.CAR_CODIGO = Carga.CAR_CODIGO) SituacaoIntegracaoFatura, ";
                    }
                    break;
                case "NumeroMinuta":
                    if (!select.Contains("NumeroMinuta"))
                    {
                        if (tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura)
                        {
                            select += "CONVERT(nvarchar(20), DocumentoTransporte.IDT_NUMERO) NumeroMinuta, ";
                            groupBy += "DocumentoTransporte.IDT_NUMERO, ";
                        }
                        else
                        {
                            select += "CONVERT(nvarchar(20), DocumentoTransporte.CIA_NUMERO_MINUTA) NumeroMinuta, ";
                            groupBy += "DocumentoTransporte.CIA_NUMERO_MINUTA, ";
                        }
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains("DataEmissao"))
                    {
                        if (tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura)
                        {
                            select += "DocumentoTransporte.IDT_DATA DataEmissao, ";
                            groupBy += "DocumentoTransporte.IDT_DATA, ";
                        }
                        else
                        {
                            select += "DocumentoTransporte.CIA_DATA_CONSULTA DataEmissao, ";
                            groupBy += "DocumentoTransporte.CIA_DATA_CONSULTA, ";
                        }
                    }
                    break;
                case "DescricaoStatus":
                    if (!select.Contains("Status"))
                    {
                        select += "Carga.CAR_SITUACAO Status, ";
                        groupBy += "Carga.CAR_SITUACAO, ";
                    }
                    break;
                case "ValorMinuta":
                    if (!select.Contains("ValorMinuta"))
                        if (tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Natura)
                        {
                            select += "DocumentoTransporte.IDT_VALOR_FRETE ValorMinuta, ";
                            groupBy += "DocumentoTransporte.IDT_VALOR_FRETE, ";
                        }
                        else
                        {
                            select += "CASE Carga.CAR_TIPO_FRETE_ESCOLHIDO WHEN 1 THEN Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE WHEN 2 THEN Carga.CAR_VALOR_FRETE_OPERADOR ELSE 0 END ValorMinuta, ";
                            groupBy += "Carga.CAR_TIPO_FRETE_ESCOLHIDO, Carga.CAR_VALOR_FRETE_TABELA_DE_FRETE, Carga.CAR_VALOR_FRETE_OPERADOR, ";
                        }
                    break;
                case "QuantidadeCTes":
                    if (!count && !select.Contains("QuantidadeCTes,"))
                        select += "COUNT(CTe.CON_CODIGO) QuantidadeCTes, ";
                    break;
                case "ValorFrete":
                    if (!count && !select.Contains("ValorFrete"))
                        select += "SUM(CTe.CON_VALOR_FRETE) ValorFrete, ";
                    break;
                case "ValorServico":
                    if (!count && !select.Contains("ValorServico"))
                        select += "SUM(CTe.CON_VALOR_PREST_SERVICO) ValorServico, ";
                    break;
                case "ValorReceber":
                    if (!count && !select.Contains("ValorReceber"))
                        select += "SUM(CTe.CON_VALOR_RECEBER) ValorReceber, ";
                    break;
                case "ValorICMS":
                    if (!count && !select.Contains("ValorICMS"))
                        select += "SUM(CTe.CON_VAL_ICMS) ValorICMS, ";
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereConsultaRelatorioMinutas(int codigoFatura, ref string having, ref string where, ref string joins, ref List<ParametroSQL> parametros, DateTime dataInicialMinuta, DateTime dataFinalMinuta, int codigoMotorista, string estadoOrigem, string estadoDestino, Dominio.Enumeradores.TipoAmbiente ambiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta tipoIntegradora, string tipoPropriedadeVeiculo, bool? situacaoIntegracao)
        {
            if (dataInicialMinuta != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO >= '" + dataInicialMinuta.ToString("yyyy-MM-dd") + "'";

            if (dataFinalMinuta != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO < '" + dataFinalMinuta.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigoMotorista > 0)
                where += " and DocumentoTransporte.FUN_CODIGO = " + codigoMotorista;

            if (codigoFatura > 0)
                where += " AND Carga.CAR_CODIGO IN (SELECT FC.CAR_CODIGO FROM T_FATURA_CARGA FC WHERE FC.FAT_CODIGO = " + codigoFatura.ToString() + ")"; 

            if (!string.IsNullOrWhiteSpace(estadoOrigem) && estadoOrigem != "0")
            {
                where += " and Origem.UF_SIGLA = :ORIGEM_UF_SIGLA";
                parametros.Add(new ParametroSQL("ORIGEM_UF_SIGLA", estadoOrigem));

                if (!joins.Contains(" Origem "))
                    joins += "left outer join T_LOCALIDADES Origem on Origem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ";
            }

            if (!string.IsNullOrWhiteSpace(estadoDestino) && estadoDestino != "0")
            {
                where += " and Destino.UF_SIGLA = :DESTINO_UF_SIGLA";
                parametros.Add(new ParametroSQL("DESTINO_UF_SIGLA", estadoDestino));

                if (!joins.Contains(" Destino "))
                    joins += "left outer join T_LOCALIDADES Destino on Destino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ";
            }

            if (!string.IsNullOrWhiteSpace(tipoPropriedadeVeiculo))
            {
                where += " and Veiculo.VEI_TIPO = :VEICULO_VEI_TIPO";
                parametros.Add(new ParametroSQL("VEICULO_VEI_TIPO", tipoPropriedadeVeiculo));

                if (!joins.Contains(" Veiculo "))
                    joins += "left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = DocumentoTransporte.VEI_CODIGO ";
            }

            if (situacaoIntegracao.HasValue)
            {
                if (situacaoIntegracao.Value)
                    having += " (select count(CCI_CODIGO) from t_carga_cte_integracao inner join t_tipo_integracao on t_carga_cte_integracao.TPI_CODIGO = T_TIPO_INTEGRACAO.TPI_CODIGO inner join t_carga_cte on t_carga_cte.CCT_CODIGO = t_carga_cte_integracao.CCT_CODIGO where t_carga_cte.CAR_CODIGO = Carga.CAR_CODIGO and t_carga_cte_integracao.INT_SITUACAO_INTEGRACAO = 1 and T_TIPO_INTEGRACAO.TPI_TIPO in (6,7)) = COUNT(CTe.CON_CODIGO)";
                else
                    having += " (select count(CCI_CODIGO) from t_carga_cte_integracao inner join t_tipo_integracao on t_carga_cte_integracao.TPI_CODIGO = T_TIPO_INTEGRACAO.TPI_CODIGO inner join t_carga_cte on t_carga_cte.CCT_CODIGO = t_carga_cte_integracao.CCT_CODIGO where t_carga_cte.CAR_CODIGO = Carga.CAR_CODIGO and t_carga_cte_integracao.INT_SITUACAO_INTEGRACAO = 1 and T_TIPO_INTEGRACAO.TPI_TIPO in (6,7)) <> COUNT(CTe.CON_CODIGO)";
            }

            where += " and CTe.CON_TIPO_AMBIENTE = " + (int)ambiente;
        }

        #endregion
    }
}
