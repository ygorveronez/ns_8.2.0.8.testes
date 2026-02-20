using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Pedidos
{
    public class Container : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.Container>
    {
        public Container(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.Container BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool ValidarDuplicidadeContainer(string numero, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

            var result = from obj in query where obj.Numero == numero && obj.Codigo != codigo select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Container BuscarPorNumero(string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

            var result = from obj in query where obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.Container> BuscarPorListaDeNumero(List<int> Containers)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>().Where(x => Containers.Contains(x.Codigo)).ToList();
        }


        public Dominio.Entidades.Embarcador.Pedidos.Container BuscarTodosPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.Container BuscarTodosPorNumero(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();
            var result = from obj in query where obj.Numero == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Container BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();
            var result = from obj in query where obj.CodigoIntegracao == codigo && obj.Status == true select obj;
            return result.FirstOrDefault();
        }

        //public List<Dominio.Entidades.Embarcador.Pedidos.Container> Consultar(string descricao, string codigoIntegracao, string numero, int tipocontainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        //{
        //    var result = Consultar(descricao, codigoIntegracao, numero, tipocontainer, localContainer, status, statusColetaContainer);
        //    return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        //}

        //public int ContarConsulta(string descricao, string codigoIntegracao, string numero, int tipoContainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer)
        //{
        //    var result = Consultar(descricao, codigoIntegracao, numero, tipoContainer, localContainer, status, statusColetaContainer);

        //    return result.Count();
        //}

        //private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Container> Consultar(string descricao, string codigoIntegracao, string numero, int tipoContainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer)
        //{
        //    var consultaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

        //    if (!string.IsNullOrWhiteSpace(descricao))
        //        consultaContainer = consultaContainer.Where(o => o.Descricao.Contains(descricao));

        //    if (!string.IsNullOrWhiteSpace(codigoIntegracao))
        //        consultaContainer = consultaContainer.Where(o => o.CodigoIntegracao.Contains(codigoIntegracao));

        //    if (!string.IsNullOrWhiteSpace(numero))
        //        consultaContainer = consultaContainer.Where(o => o.Numero.Contains(numero));

        //    if (tipoContainer > 0)
        //        consultaContainer = consultaContainer.Where(o => o.ContainerTipo.Codigo == tipoContainer);

        //    if ((localContainer > 0) || statusColetaContainer.HasValue)
        //    {
        //        var consultaColetaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

        //        if (localContainer > 0)
        //            consultaColetaContainer = consultaColetaContainer.Where(o => o.LocalAtual.CPF_CNPJ == localContainer);

        //        if (statusColetaContainer.HasValue)
        //            consultaColetaContainer = consultaColetaContainer.Where(o => o.Status == statusColetaContainer.Value);

        //        consultaContainer = consultaContainer.Where(container => consultaColetaContainer.Any(coletaContainer => coletaContainer.Container.Codigo == container.Codigo));
        //    }

        //    if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
        //        consultaContainer = consultaContainer.Where(o => o.Status);
        //    else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
        //        consultaContainer = consultaContainer.Where(o => !o.Status);

        //    return consultaContainer;
        //}


        public IList<Dominio.ObjetosDeValor.Embarcador.Container.RetornoConsultaContainer> Consultar(string descricao, string codigoIntegracao, string numero, int tipocontainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var sqlDinamico = ObterQueryConsultarContainers(descricao, codigoIntegracao, numero, tipocontainer, localContainer, status, statusColetaContainer, false);

            
            if ((localContainer > 0) || statusColetaContainer.HasValue)
                sqlDinamico.StringQuery = $"{sqlDinamico.StringQuery} ORDER BY ColetaContainer.CCR_DATA_COLETA Desc ";
            else
                sqlDinamico.StringQuery = $"{sqlDinamico.StringQuery} ORDER BY {propOrdenacao} {dirOrdenacao} ";

            if (maximoRegistros > 0)
                sqlDinamico.StringQuery += $" OFFSET {inicioRegistros} ROWS FETCH NEXT {maximoRegistros} ROWS ONLY;";
            else
                sqlDinamico.StringQuery += ";";

            var hibernateQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            hibernateQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Container.RetornoConsultaContainer)));

            return hibernateQuery.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Container.RetornoConsultaContainer>();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao, string numero, int tipoContainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer)
        {
            var sqlQuery = ObterQueryConsultarContainers(descricao, codigoIntegracao, numero, tipoContainer, localContainer, status, statusColetaContainer, true);
            var hibernateQuery = sqlQuery.CriarSQLQuery(this.SessionNHiBernate);

            return hibernateQuery.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Container> BuscarContainerDataAtualizacao(DateTime dataAtualizacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

            var result = from obj in query select obj;

            if (dataAtualizacao > DateTime.MinValue)
                result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Timeout(99999).ToList();
        }

        public int ContarContainerBuscarPorDataAtualizacao(DateTime dataAtualizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Container>();

            var result = from obj in query select obj;

            if (dataAtualizacao > DateTime.MinValue)
                result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.Timeout(99999).Count();
        }


        #region Metodos Privados

        private SQLDinamico ObterQueryConsultarContainers(string descricao, string codigoIntegracao, string numero, int tipocontainer, double localContainer, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer? statusColetaContainer, bool contarConsulta)
        {
            StringBuilder sqlQuery = new StringBuilder("select ");
            var parametros = new List<ParametroSQL>();

            if (contarConsulta)
                sqlQuery.Append("count(1) ");
            else
                sqlQuery.Append(@"
                    
                       Container.CTR_CODIGO [Codigo],
                       Container.CTR_DESCRICAO [Descricao],
                       Container.CTR_NUMERO [Numero],
		               Container.CTR_CODIGO_INTEGRACAO [CodigoIntegracao],
		               Container.CTR_STATUS [Status],
		               Container.CTR_TARA [Tara],
		               Container.CTR_PROPRIEDADE [TipoPropriedade],
		               ClienteArmador.CLI_CGCCPF [Armador],
		               ClienteArmador.CLI_NOME [ClienteArmador],
		               TipoContainer.CTI_DESCRICAO [DescricaoTipoContainer]
                       ");

            sqlQuery.Append(@"
                        from
                        T_CONTAINER Container
	                    LEFT join
	                    T_CONTAINER_TIPO TipoContainer on TipoContainer.CTI_CODIGO = Container.CTI_CODIGO
	                    LEFT join 
	                    T_CLIENTE ClienteArmador on Container.CLI_CODIGO_ARMADOR = ClienteArmador.CLI_CGCCPF

            ");

            sqlQuery.Append($" where 1=1 ");

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                sqlQuery.Append($"and Container.CTR_DESCRICAO like :CONTAINER_CTR_DESCRICAO ");
                parametros.Add(new ParametroSQL("CONTAINER_CTR_DESCRICAO", $"%{descricao}%"));
            }

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
            {
                sqlQuery.Append($"and Container.CTR_CODIGO_INTEGRACAO like :CONTAINER_CTR_CODIGO_INTEGRACAO ");
                parametros.Add(new ParametroSQL("CONTAINER_CTR_CODIGO_INTEGRACAO", $"%{descricao}%"));

            }

            if (!string.IsNullOrWhiteSpace(numero))
            {
                sqlQuery.Append($"and Container.CTR_NUMERO like :CONTAINER_CTR_NUMERO ");
                parametros.Add(new ParametroSQL("CONTAINER_CTR_NUMERO", $"%{descricao}%"));
            }

            if (tipocontainer > 0)
                sqlQuery.Append($"and TipoContainer.CTI_CODIGO = {tipocontainer} ");

            if ((localContainer > 0) || statusColetaContainer.HasValue)
            {
                if (localContainer > 0)
                    sqlQuery.Append(@$"and Container.CTR_CODIGO in (select CTR_CODIGO from T_COLETA_CONTAINER ColetaContainer 
                                       inner join   T_CLIENTE ClienteAtual on ColetaContainer.CLI_CODIGO_ATUAL = ClienteAtual.CLI_CGCCPF
                                       where ClienteAtual.CLI_CGCCPF = {localContainer} ) " ); // SQL-INJECTION-SAFE

                if (statusColetaContainer.HasValue)
                    sqlQuery.Append($"and Container.CTR_CODIGO in (select CTR_CODIGO from T_COLETA_CONTAINER where CCR_STATUS = {(int)statusColetaContainer.Value} ) "); // SQL-INJECTION-SAFE
            }

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                sqlQuery.Append($"and Container.CTR_STATUS = 1 ");
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                sqlQuery.Append($"and Container.CTR_STATUS = 0 ");


            return new SQLDinamico(sqlQuery
                .ToString(),parametros);
        }


        #endregion 
    }
}
