using Dominio.ObjetosDeValor.Embarcador.Imposto.OutrasAliquotas;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.OutrasAliquotas
{
    public class OutrasAliquotasImposto : RepositorioBase<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto>
    {
        #region Construtores

        public OutrasAliquotasImposto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public OutrasAliquotasImposto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private string QueryConsultaOutrasAliquotas(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, int codigoOutrasAliquotas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto tipoImposto, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append(@"select OutrasAliquotasImposto.OAI_CODIGO As Codigo,
                            		ISNULL(OutrasAliquotasImposto.OAI_TIPO_IMPOSTO, 0) as TipoImposto,
                            		OutrasAliquotasImposto.OAI_ALIQUOTA as Aliquota,
                            		OutrasAliquotasImposto.OAI_REDUCAO as PercentualReducao,
                            		UF.UF_SIGLA as UF,
                            		OutrasAliquotasImposto.OAI_ALIQUOTA_UF as AliquotaUF,
                            		OutrasAliquotasImposto.OAI_REDUCAO_UF as PercentualReducaoUF,
                            		Localidade.LOC_DESCRICAO as Municipio, 
                            		OutrasAliquotasImposto.OAI_ALIQUOTA_MUNICIPIO as AliquotaMunicipio,
                            		OutrasAliquotasImposto.OAI_REDUCAO_MUNICIPIO as PercentualReducaoMunicipio,
                            		OutrasAliquotasImposto.OAI_DATA_VIGENCIA_INICIO as DataInicialVigencia,
                            		OutrasAliquotasImposto.OAI_DATA_VIGENCIA_FINAL as DataFinalVigencia");

            sql.AppendLine(@" From T_OUTRAS_ALIQUOTAS_IMPOSTO OutrasAliquotasImposto
                            	 left join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = OutrasAliquotasImposto.LOC_CODIGO
                            	 left join T_UF UF on UF.UF_SIGLA = OutrasAliquotasImposto.UF_SIGLA
                                 left join T_OUTRAS_ALIQUOTAS OutrasAliquotas on OutrasAliquotas.TOA_CODIGO = OutrasAliquotasImposto.TOA_CODIGO ");

            sql.AppendLine($"WHERE OutrasAliquotasImposto.OAI_TIPO_IMPOSTO = {tipoImposto.GetHashCode()}");

            if (codigoOutrasAliquotas > 0)
            {
                sql.AppendLine($" AND OutrasAliquotasImposto.TOA_CODIGO = {codigoOutrasAliquotas}");
            }


            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.AppendLine($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.AppendLine($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public Task<int> ContarConsultaOutrasAliquotasImposto(int codigoOutrasAliquotas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto tipoImposto)
        {
            string sql = QueryConsultaOutrasAliquotas(null, codigoOutrasAliquotas, tipoImposto, true);
            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Task<IList<DadosOutrasAliquotasImposto>> ConsultaOutrasAliquotasImposto(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, int codigoOutrasAliquotas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto tipoImposto)
        {
            string sql = QueryConsultaOutrasAliquotas(parametroConsulta, codigoOutrasAliquotas, tipoImposto, false);
            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(DadosOutrasAliquotasImposto)));

            return consulta.SetTimeout(600).ListAsync<DadosOutrasAliquotasImposto>(CancellationToken);
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto>> BuscarTodasOutrasAliquotasImpostoAsync()
        {
            string sql = $@"select outrasAliquotasImposto.OAI_CODIGO Codigo,
                                    outrasAliquotasImposto.TOA_CODIGO CodigoOutraAliquota,
                                    outrasAliquotasImposto.OAI_TIPO_IMPOSTO TipoImposto,
                                    outrasAliquotasImposto.OAI_ALIQUOTA Aliquota,
                                    outrasAliquotasImposto.OAI_ALIQUOTA_UF AliquotaUF,
                                    outrasAliquotasImposto.OAI_ALIQUOTA_MUNICIPIO AliquotaMunicipio,
                                    outrasAliquotasImposto.OAI_DATA_VIGENCIA_INICIO DataVigenciaInicial,
                                    outrasAliquotasImposto.OAI_DATA_VIGENCIA_FINAL DataVigenciaFinal,
                                    outrasAliquotasImposto.LOC_CODIGO CodigoLocalidade,
                                    outrasAliquotasImposto.UF_SIGLA SiglaUF,
                                    outrasAliquotasImposto.OAI_REDUCAO PercentualReducao,
                                    outrasAliquotasImposto.OAI_REDUCAO_UF PercentualReducaoUF,
                                    outrasAliquotasImposto.OAI_REDUCAO_MUNICIPIO PercentualReducaoMunicipio
                        from T_OUTRAS_ALIQUOTAS_IMPOSTO outrasAliquotasImposto
                        join T_OUTRAS_ALIQUOTAS outrasAliquotas on outrasAliquotas.TOA_CODIGO = outrasAliquotasImposto.TOA_CODIGO
                        where outrasAliquotas.TOA_ATIVO = 1 and outrasAliquotasImposto.OAI_DATA_VIGENCIA_FINAL >= '{DateTime.Now.Date.ToString("yyyy-MM-dd")}'";

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto)));

            return consulta.SetTimeout(600).ListAsync<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto>(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto> BuscarOutraAliquotaImpostoPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto> consultaOutrasAliquotasImposto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto>()
                .Where(o => o.Codigo == codigo);

            return consultaOutrasAliquotasImposto.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto>> BuscarOutraAliquotaImpostoPorCodigoDeOutraAliquotaAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto> consultaOutrasAliquotasImposto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotasImposto>()
                .Where(o => o.OutrasAliquotas.Codigo == codigo);

            return consultaOutrasAliquotasImposto.ToListAsync(CancellationToken);
        }

        #endregion
    }
}
