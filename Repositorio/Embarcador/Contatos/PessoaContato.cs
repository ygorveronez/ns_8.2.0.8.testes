using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Contatos
{
    public class PessoaContato : RepositorioBase<Dominio.Entidades.Embarcador.Contatos.PessoaContato>
    {
        public PessoaContato(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> BuscarPorGrupoPessoas(int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.PessoaContato>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> BuscarPorPessoa(double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.PessoaContato>();

            query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> Consultar(bool obrigatorioPessoaOuGrupo, double cpfCnpjPessoa, int codigoGrupoPessoas, string nome, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = MontarConsulta(obrigatorioPessoaOuGrupo, cpfCnpjPessoa, codigoGrupoPessoas, nome);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(bool obrigatorioPessoaOuGrupo, double cpfCnpjPessoa, int codigoGrupoPessoas, string nome)
        {
            var query = MontarConsulta(obrigatorioPessoaOuGrupo, cpfCnpjPessoa, codigoGrupoPessoas, nome);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Contatos.PessoaContato> MontarConsulta(bool obrigatorioPessoaOuGrupo, double cpfCnpjPessoa, int codigoGrupoPessoas, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.PessoaContato>();

            if (cpfCnpjPessoa > 0d && codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pessoa.CPF_CNPJ == cpfCnpjPessoa || o.Pessoa.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else if (cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);
            else if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pessoa.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else if (obrigatorioPessoaOuGrupo)
                query = query.Where(o => o.Codigo == 0); //não retorna registros se não tiver grupo ou pessoa

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(o => o.Contato.Contains(nome));

            return query;
        }


        #region Relatório de Tipo de Contatos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Contatos.TipoContatoCliente.TipoContatoCliente> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioContatosRealizados(filtrosPesquisa, propriedades, parametrosConsulta, false));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Contatos.TipoContatoCliente.TipoContatoCliente)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Contatos.TipoContatoCliente.TipoContatoCliente>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioContatosRealizados(filtrosPesquisa, propriedades, parametrosConsulta, true));

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioContatosRealizados(Dominio.ObjetosDeValor.Embarcador.Contatos.FiltroPesquisaRelatorioTipoContatoCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta , bool count)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectConsultaRelatorioContatosRealizados(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereConsultaRelatorioContatosRealizados(ref where, ref groupBy, ref joins, filtrosPesquisa.CodigoEmpresa, filtrosPesquisa.CodigoGrupoPessoas, filtrosPesquisa.CpfCnpjPessoa, filtrosPesquisa.TiposContato);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                {
                    SetarSelectConsultaRelatorioContatosRealizados(parametrosConsulta.PropriedadeAgrupar, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(parametrosConsulta.PropriedadeAgrupar))
                        orderBy = parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                {
                    if (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta.PropriedadeAgrupar && select.Contains(parametrosConsulta.PropriedadeOrdenar))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            return (count ? "select distinct(count(0) over ())" : "select ContatoCliente.PCO_CODIGO Codigo, " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_PESSOA_CONTATO ContatoCliente " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by ContatoCliente.PCO_CODIGO, " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (parametrosConsulta.InicioRegistros  <= 0 && parametrosConsulta.LimiteRegistros <= 0) ? "" : " offset " + parametrosConsulta.InicioRegistros.ToString() + " rows fetch next " + parametrosConsulta.LimiteRegistros.ToString() + " rows only;");
        }

        private void SetarSelectConsultaRelatorioContatosRealizados(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {

                case "NomeEntidade":
                    if (!select.Contains(" NomeEntidade,"))
                    {
                        select += "ISNULL(GrupoPessoas.GRP_DESCRICAO, Pessoa.CLI_NOME) NomeEntidade, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, Pessoa.CLI_NOME, ";

                        if (!joins.Contains(" GrupoPessoas "))
                            joins += " left join T_GRUPO_PESSOAS GrupoPessoas on ContatoCliente.GRP_CODIGO = GrupoPessoas.GRP_CODIGO ";

                        if (!joins.Contains(" Pessoa "))
                            joins += " left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = ContatoCliente.CLI_CGCCPF ";
                    }
                    break;
                case "Entidade":
                    if (!select.Contains(" Entidade,"))
                    {
                        select += "CASE WHEN ContatoCliente.GRP_CODIGO IS NULL THEN 'Pessoa' ELSE 'Grupo de Pessoa' END Entidade, ";
                        groupBy += "ContatoCliente.GRP_CODIGO, ContatoCliente.CLI_CGCCPF, ";
                    }
                    break;
                case "NomeContato":
                    if (!select.Contains(" NomeContato,"))
                    {
                        select += "ContatoCliente.PCO_CONTATO NomeContato, ";
                        groupBy += "ContatoCliente.PCO_CONTATO, ";
                    }
                    break;
                case "EmailContato":
                    if (!select.Contains(" EmailContato,"))
                    {
                        select += "ContatoCliente.PCO_EMAIL EmailContato, ";
                        groupBy += "ContatoCliente.PCO_EMAIL, ";
                    }
                    break;
                case "TelefoneContato":
                    if (!select.Contains(" TelefoneContato,"))
                    {
                        select += "ContatoCliente.PCO_TELEFONE TelefoneContato, ";
                        groupBy += "ContatoCliente.PCO_TELEFONE, ";
                    }
                    break;
                case "TipoContato":
                    if (!select.Contains(" TipoContato,"))
                        select += "substring((select ', ' + tipo.TCO_DESCRICAO  from T_PESSOA_CONTATO_TIPO_CONTATO tipoContatoCliente inner join T_TIPO_CONTATO tipo on tipoContatoCliente.TCO_CODIGO = tipo.TCO_CODIGO where tipoContatoCliente.PCO_CODIGO = ContatoCliente.PCO_CODIGO for xml path('')), 3, 1000) TipoContato, ";
                    break;
            }
        }

        private void SetarWhereConsultaRelatorioContatosRealizados(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos)
        {
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

            if (tipos.Count > 0)
                where += " and exists (select TipoContatoCliente.PCO_CODIGO from T_PESSOA_CONTATO_TIPO_CONTATO TipoContatoCliente where TipoContatoCliente.PCO_CODIGO = ContatoCliente.PCO_CODIGO and TipoContatoCliente.TCO_CODIGO in (" + string.Join(", ", tipos) + "))"; // SQL-INJECTION-SAFE
        }

        #endregion
    }
}
