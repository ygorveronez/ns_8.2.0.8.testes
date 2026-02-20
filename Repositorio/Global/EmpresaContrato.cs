using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class EmpresaContrato : RepositorioBase<Dominio.Entidades.EmpresaContrato>, Dominio.Interfaces.Repositorios.EmpresaContrato
    {
        public EmpresaContrato(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public EmpresaContrato(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.EmpresaContrato BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.EmpresaContrato> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>()
                .Where(obj => obj.Codigo == codigo).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.EmpresaContrato BuscarPorEmpresa(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Empresa.Codigo == codigoTransportador select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaContrato BuscarPorEmpresaTMS(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Empresa.Codigo == codigoTransportador && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.EmpresaContrato BuscarPorTransportador(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Ativo select obj;

            var queryTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();
            var resultQueryTransportador = from obj in queryTransportador select obj;

            result = result.Where(o => resultQueryTransportador.Where(a => a.Contrato.Codigo == o.Codigo && a.Empresa.Codigo == codigoTransportador).Any() || !resultQueryTransportador.Where(a => a.Contrato.Codigo == o.Codigo).Any());

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EmpresaContrato> Consultar(int codigoTransportadorPai, string nomeEmpresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoTransportadorPai select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            result = result.Fetch(o => o.Empresa);

            return result.OrderBy(o => o.Empresa.NomeFantasia).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoTransportadorPai, string nomeEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Empresa.EmpresaPai.Codigo == codigoTransportadorPai select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            result = result.Fetch(o => o.Empresa);

            return result.Count();
        }

        public List<Dominio.Entidades.EmpresaContrato> ConsultarTMS(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = ConsultarTMS(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaTMS(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal filtrosPesquisa)
        {
            var result = ConsultarTMS(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.EmpresaContrato> Consultar(string nomeEmpresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            result = result.Fetch(o => o.Empresa);

            return result.OrderBy(o => o.Empresa.NomeFantasia).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string nomeEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nomeEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeEmpresa) || o.Empresa.RazaoSocial.Contains(nomeEmpresa));

            result = result.Fetch(o => o.Empresa);

            return result.Count();
        }

        public bool PossuiOutroContratoSemTransportador(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();
            var result = from obj in query where obj.Codigo != codigoContrato && obj.Ativo select obj;

            var queryTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();
            var resultQueryTransportador = from obj in queryTransportador select obj;

            result = result.Where(o => !resultQueryTransportador.Where(a => a.Contrato.Codigo == o.Codigo).Any());

            return result.Any();
        }

        public List<Dominio.Entidades.Empresa> ConsultarEmpresaContratosPendentesParaNotificarPorEmail()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            var result = from obj in query select obj;

            var queryTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();
            var resultQueryTransportador = from obj in queryTransportador where obj.Contrato.NotificarPorEmail && obj.Contrato.Ativo select obj;

            result = result.Where(o => resultQueryTransportador.Where(t => t.Empresa.Codigo == o.Codigo && (!o.AceitouTermosUso || (t.Contrato.RecorrenciaEmDias > 0 && !o.DataAceiteTermosUso.HasValue) || (t.Contrato.RecorrenciaEmDias > 0 && o.DataAceiteTermosUso.HasValue && o.DataAceiteTermosUso.Value.Date < DateTime.Now.Date.AddDays(-t.Contrato.RecorrenciaEmDias)))).Any());

            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.EmpresaContrato> ConsultarTMS(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaContratoNotaFiscal filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContrato>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Contrato))
                result = result.Where(obj => obj.Contrato.Contains(filtrosPesquisa.Contrato));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(filtrosPesquisa.CodigoEmpresa) || obj.Empresa == null);

            return result;
        }

        #endregion

        #region Relatório de Aceite de Contrato

        public IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato> ConsultarRelatorioAceiteContrato(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioAceiteContrato(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato>> ConsultarRelatorioAceiteContratoAsync(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioAceiteContrato(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Transportadores.AceiteContrato>();
        }

        public int ContarConsultaRelatorioAceiteContrato(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioAceiteContrato(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioAceiteContrato(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioAceiteContrato(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioAceiteContrato(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioAceiteContrato(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_EMPRESA AceiteContrato ";

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

        private void SetarSelectRelatorioConsultaRelatorioAceiteContrato(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "AceiteContrato.VED_CODIGO Codigo, ";
                        groupBy += "AceiteContrato.VED_CODIGO, ";
                    }
                    break;

                case "CPFCNPJFormatado":
                    if (!select.Contains(" CPFCNPJ, "))
                    {
                        select += "AceiteContrato.EMP_CNPJ CPFCNPJ, AceiteContrato.EMP_TIPO Tipo, ";
                        groupBy += "AceiteContrato.EMP_CNPJ, AceiteContrato.EMP_TIPO, ";
                    }
                    break;

                case "Razao":
                    if (!select.Contains(" Razao, "))
                    {
                        select += "AceiteContrato.EMP_RAZAO Razao, ";
                        groupBy += "AceiteContrato.EMP_RAZAO, ";
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        if (!joins.Contains(" Localidade "))
                            joins += " LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = AceiteContrato.LOC_CODIGO";

                        select += "Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA Cidade, ";
                        groupBy += "Localidade.LOC_DESCRICAO, Localidade.UF_SIGLA, ";
                    }
                    break;

                case "DescricaoAceite":
                    if (!select.Contains(" Aceite, "))
                    {
                        select += "AceiteContrato.EMP_ACEITOU_TERMOS_USO Aceite, ";
                        groupBy += "AceiteContrato.EMP_ACEITOU_TERMOS_USO, ";
                    }
                    break;

                case "LogAceite":
                    if (!select.Contains(" LogAceite, "))
                    {
                        select += "AceiteContrato.EMP_ACEITOU_TERMOS_USO_LOG LogAceite, ";
                        groupBy += "AceiteContrato.EMP_ACEITOU_TERMOS_USO_LOG, ";
                    }
                    break;

                case "NomeDoContrato":
                    if (!select.Contains(" NomeDoContrato, "))
                    {
                        if (!joins.Contains(" Contrato "))
                            joins += " LEFT JOIN T_EMPRESA_CONTRATO Contrato ON Contrato.ECO_CODIGO = AceiteContrato.EMP_CODIGO_EMPRESA_CONTRATO";

                        select += "Contrato.ECO_DESCRICAO NomeDoContrato, ";
                        groupBy += "Contrato.ECO_DESCRICAO, ";
                    }
                    break;

                case "DataAceiteFormatada":
                    if (!select.Contains(" DataAceite, "))
                    {
                        select += "AceiteContrato.EMP_DATA_ACEITE_TERMOS_USO DataAceite, ";
                        groupBy += "AceiteContrato.EMP_DATA_ACEITE_TERMOS_USO, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioAceiteContrato(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioAceiteContrato filtrosPesquisa)
        {
            if (filtrosPesquisa.CodigoTransportador > 0)
                where += " AND AceiteContrato.EMP_CODIGO = " + filtrosPesquisa.CodigoTransportador;

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteContrato.Aceito)
                where += " AND AceiteContrato.EMP_ACEITOU_TERMOS_USO = 1";
            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteContrato.Pendente)
                where += " AND (AceiteContrato.EMP_ACEITOU_TERMOS_USO = 0 or AceiteContrato.EMP_ACEITOU_TERMOS_USO is null)";

            if (filtrosPesquisa.CodigoContratoNotaFiscal > 0)
                where += $" AND AceiteContrato.EMP_CODIGO in (SELECT EMP_CODIGO FROM T_EMPRESA_CONTRATO_TRANSPORTADOR WHERE ECO_CODIGO = {filtrosPesquisa.CodigoContratoNotaFiscal})";
        }

        #endregion
    }
}
