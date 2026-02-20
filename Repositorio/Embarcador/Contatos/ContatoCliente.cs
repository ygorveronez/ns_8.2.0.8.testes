using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Repositorio.Embarcador.Contatos
{
    public class ContatoCliente : RepositorioBase<Dominio.Entidades.Embarcador.Contatos.ContatoCliente>
    {
        public ContatoCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Contatos.ContatoCliente> Consultar(int codigoObjeto, TipoDocumentoContatoCliente tipoDocumento, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = MontarConsulta(codigoObjeto, tipoDocumento);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoObjeto, TipoDocumentoContatoCliente tipoDocumento)
        {
            var query = MontarConsulta(codigoObjeto, tipoDocumento);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Contatos.ContatoCliente> MontarConsulta(int codigoObjeto, TipoDocumentoContatoCliente tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.ContatoCliente>();

            switch (tipoDocumento)
            {
                case TipoDocumentoContatoCliente.Titulo:
                    query = query.Where(o => o.Documentos.Any(d => d.Tipo == tipoDocumento && d.Titulo.Codigo == codigoObjeto));
                    break;
                case TipoDocumentoContatoCliente.Fatura:
                    query = query.Where(o => o.Documentos.Any(d => d.Tipo == tipoDocumento && d.Fatura.Codigo == codigoObjeto));
                    break;
                case TipoDocumentoContatoCliente.Bordero:
                    query = query.Where(o => o.Documentos.Any(d => d.Tipo == tipoDocumento && d.Bordero.Codigo == codigoObjeto));
                    break;
                default:
                    query = query.Where(o => o.Codigo == 0); //não retorna nada pois o tipo de documento não existe
                    break;
            }

            return query;
        }

        public Dominio.Entidades.Embarcador.Contatos.ContatoCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.ContatoCliente>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public int BuscarUltimoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.ContatoCliente>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        #region Relatório de Contatos Realizados

        public IList<Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente.ContatoCliente> ConsultarRelatorio(int codigoEmpresa, List<PropriedadeAgrupamento> agrupamentos, DateTime dataInicial, DateTime dataFinal, int codigoTitulo, int codigoFatura, int codigoBordero, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos, List<int> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioContatosRealizados(codigoEmpresa, false, agrupamentos, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente.ContatoCliente)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente.ContatoCliente>();
        }

        public int ContarConsultaRelatorio(int codigoEmpresa, List<PropriedadeAgrupamento> agrupamentos, DateTime dataInicial, DateTime dataFinal, int codigoTitulo, int codigoFatura, int codigoBordero, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos, List<int> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioContatosRealizados(codigoEmpresa, true, agrupamentos, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioContatosRealizados(int codigoEmpresa, bool count, List<PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int codigoTitulo, int codigoFatura, int codigoBordero, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos, List<int> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectConsultaRelatorioContatosRealizados(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereConsultaRelatorioContatosRealizados(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectConsultaRelatorioContatosRealizados(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return (count ? "select distinct(count(0) over ())" : "select ContatoCliente.CCL_CODIGO Codigo, " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CONTATO_CLIENTE ContatoCliente " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by ContatoCliente.CCL_CODIGO, " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarSelectConsultaRelatorioContatosRealizados(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Descricao":
                    if (!select.Contains(" Descricao,"))
                    {
                        select += "ContatoCliente.CCL_DESCRICAO Descricao, ";
                        groupBy += "ContatoCliente.CCL_DESCRICAO, ";
                    }
                    break;
                case "DataContato":
                    if (!select.Contains(" DataContato,"))
                    {
                        select += "ContatoCliente.CCL_DATA DataContato, ";
                        groupBy += "ContatoCliente.CCL_DATA, ";
                    }
                    break;
                case "DataPrevistaRetorno":
                    if (!select.Contains(" DataPrevistaRetorno"))
                    {
                        select += "FORMAT(ContatoCliente.CCL_DATA_PREVISTA_RETORNO, 'dd/MM/yyyy HH:mm') DataPrevistaRetorno, ";
                        groupBy += "ContatoCliente.CCL_DATA_PREVISTA_RETORNO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero,"))
                    {
                        select += "ContatoCliente.CCL_NUMERO Numero, ";
                        groupBy += "ContatoCliente.CCL_NUMERO, ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoPessoas "))
                            joins += " left join T_GRUPO_PESSOAS GrupoPessoas on ContatoCliente.GRP_CODIGO = GrupoPessoas.GRP_CODIGO ";
                    }
                    break;
                case "Pessoa":
                    if (!select.Contains(" Pessoa,"))
                    {
                        select += "Pessoa.CLI_NOME Pessoa, ";
                        groupBy += "Pessoa.CLI_NOME, ";

                        if (!joins.Contains(" Pessoa "))
                            joins += " left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = ContatoCliente.CLI_CGCCPF ";
                    }
                    break;
                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains(" CPFCNPJPessoa,"))
                    {
                        select += "Pessoa.CLI_CGCCPF CPFCNPJPessoa, Pessoa.CLI_FISJUR TipoPessoa, ";
                        groupBy += "Pessoa.CLI_CGCCPF, Pessoa.CLI_FISJUR, ";

                        if (!joins.Contains(" Pessoa "))
                            joins += " left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = ContatoCliente.CLI_CGCCPF ";
                    }
                    break;
                case "Usuario":
                    if (!select.Contains(" Usuario,"))
                    {
                        select += "Usuario.FUN_NOME Usuario, ";
                        groupBy += "Usuario.FUN_NOME, ";

                        if (!joins.Contains(" Usuario "))
                            joins += " left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = ContatoCliente.FUN_CODIGO ";
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select += "Situacao.SCO_DESCRICAO Situacao, ";
                        groupBy += "Situacao.SCO_DESCRICAO, ";

                        if (!joins.Contains(" Situacao "))
                            joins += " left join T_SITUACAO_CONTATO Situacao on Situacao.SCO_CODIGO = ContatoCliente.SCO_CODIGO ";
                    }
                    break;
                case "Contato":
                    if (!select.Contains(" Contato,"))
                    {
                        select += "ISNULL(Contato.PCO_CONTATO, ContatoCliente.CCL_CONTATO_SEM_CADASTRO) Contato, ";
                        groupBy += "Contato.PCO_CONTATO, ContatoCliente.CCL_CONTATO_SEM_CADASTRO, ";

                        if (!joins.Contains(" Contato "))
                            joins += " left join T_PESSOA_CONTATO Contato on Contato.PCO_CODIGO = ContatoCliente.PCO_CODIGO ";
                    }
                    break;
                case "Documento":
                    if (!select.Contains(" Documento,"))
                        select += "substring((select ', ' + (case documento.CCD_TIPO when 1 then 'Título ' + convert(nvarchar(20), titulo.TIT_CODIGO) when 2 then 'Fatura ' + convert(nvarchar(20), fatura.FAT_NUMERO) when 3 then 'Borderô ' + convert(nvarchar(20), bordero.BOR_NUMERO) else '' end) from T_CONTATO_CLIENTE_DOCUMENTO documento left join T_FATURA fatura on documento.FAT_CODIGO = fatura.FAT_CODIGO left join T_TITULO titulo on documento.TIT_CODIGO = titulo.TIT_CODIGO left join T_BORDERO bordero on bordero.BOR_CODIGO = documento.BOR_CODIGO where ContatoCliente.CCL_CODIGO = documento.CCL_CODIGO for xml path('')), 3, 1000) Documento, ";
                    break;
                case "Tipo":
                    if (!select.Contains(" Tipo,"))
                        select += "substring((select ', ' + tipo.TCO_DESCRICAO  from T_CONTATO_CLIENTE_TIPO_CONTATO tipoContatoCliente inner join T_TIPO_CONTATO tipo on tipoContatoCliente.TCO_CODIGO = tipo.TCO_CODIGO where tipoContatoCliente.CCL_CODIGO = ContatoCliente.CCL_CODIGO for xml path('')), 3, 1000) Tipo, ";
                    break;
            }
        }

        private void SetarWhereConsultaRelatorioContatosRealizados(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoTitulo, int codigoFatura, int codigoBordero, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos, List<int> situacoes)
        {
            if (dataInicial != DateTime.MinValue)
                where += " and ContatoCliente.CCL_DATA >= '" + dataInicial.ToString("yyyy-MM-dd") + "'";
            
            if (dataFinal != DateTime.MinValue)
                where += " and ContatoCliente.CCL_DATA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigoBordero > 0)
                where += " and exists (select documento.CCD_CODIGO from T_CONTATO_CLIENTE_DOCUMENTO documento where documento.CCL_CODIGO = ContatoCliente.CCL_CODIGO and documento.CCD_TIPO = 3 and documento.BOR_CODIGO = " + codigoBordero.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFatura > 0)
                where += "and exists (select documento.CCD_CODIGO from T_CONTATO_CLIENTE_DOCUMENTO documento where documento.CCL_CODIGO = ContatoCliente.CCL_CODIGO and documento.CCD_TIPO = 2 and documento.FAT_CODIGO = " + codigoFatura.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoTitulo > 0)
                where += "and exists (select documento.CCD_CODIGO from T_CONTATO_CLIENTE_DOCUMENTO documento where documento.CCL_CODIGO = ContatoCliente.CCL_CODIGO and documento.CCD_TIPO = 1 and documento.TIT_CODIGO = " + codigoTitulo.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoGrupoPessoas > 0)
                where += " and ContatoCliente.GRP_CODIGO = " + codigoGrupoPessoas.ToString();

            if (cpfCnpjPessoa > 0)
                where += " and ContatoCliente.CLI_CGCCPF = " + cpfCnpjPessoa.ToString("F0");

            if (codigoEmpresa > 0)
            {
                where += " and Usuario.EMP_CODIGO = " + codigoEmpresa.ToString();
                if (!joins.Contains(" Usuario "))
                    joins += " left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = ContatoCliente.FUN_CODIGO ";
            }

            if (situacoes?.Count > 0)
                where += " and ContatoCliente.SCO_CODIGO in (" + string.Join(", ", situacoes) + ")";

            if (tipos?.Count > 0)
                where += " and exists (select TipoContatoCliente.CCL_CODIGO from T_CONTATO_CLIENTE_TIPO_CONTATO TipoContatoCliente where TipoContatoCliente.CCL_CODIGO = ContatoCliente.CCL_CODIGO and TipoContatoCliente.TCO_CODIGO in (" + string.Join(", ", tipos) + "))"; // SQL-INJECTION-SAFE
        }

        #endregion
    }
}
