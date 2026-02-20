using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class TipoTrecho : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoTrecho>
    {
        #region Construtores

        public TipoTrecho(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoTrecho> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho filtroPesquisa)
        {
            var consultaTipoTrecho = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoTrecho>();

            if (!string.IsNullOrEmpty(filtroPesquisa.Descricao))
                consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.Descricao.Contains(filtroPesquisa.Descricao));

            if (filtroPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
            {
                if (filtroPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo)
                    consultaTipoTrecho = consultaTipoTrecho.Where(tt => tt.Ativo == true);
                else
                    consultaTipoTrecho = consultaTipoTrecho.Where(tt => tt.Ativo == false);
            }

            return consultaTipoTrecho;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.TipoTrecho> Consultar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho filtrosPesquisa)
        {
            var consultaTipoTrecho = Consultar(filtrosPesquisa);

            return ObterLista(consultaTipoTrecho, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho filtrosPesquisa)
        {
            var consultaTipoTrecho = Consultar(filtrosPesquisa);

            return consultaTipoTrecho.Count();
        }

        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.Cargas.TipoTrecho tipoTrecho)
        {
            var consultaTipoTrecho = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoTrecho>();

            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.Codigo != tipoTrecho.Codigo);

            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.Descricao.Equals(tipoTrecho.Descricao));
            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.TiposOperacao.All(o => tipoTrecho.TiposOperacao.Contains(o)));
            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.CategoriasOrigem.All(o => tipoTrecho.CategoriasOrigem.Contains(o)));
            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.CategoriasDestino.All(o => tipoTrecho.CategoriasDestino.Contains(o)));
            consultaTipoTrecho = consultaTipoTrecho.Where(obj => obj.ModelosVeiculares.All(o => tipoTrecho.ModelosVeiculares.Contains(o)));

            return consultaTipoTrecho.Any();
        }

        public List<Dominio.ObjetosDeValor.MemoryCache.TipoTrecho> ObterDadosTiposTrechoAtivos()
        {
            var consultaTipoTrecho = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoTrecho>()
                .Where(tipoTrecho => tipoTrecho.Ativo);

            List<Dominio.ObjetosDeValor.MemoryCache.TipoTrecho> tiposTrecho = consultaTipoTrecho
                .Select(tipoTrecho => new Dominio.ObjetosDeValor.MemoryCache.TipoTrecho()
                {
                    Codigo = tipoTrecho.Codigo,
                    Descricao = tipoTrecho.Descricao,
                    Ativo = tipoTrecho.Ativo
                })
                .ToList();

            if (tiposTrecho.Count > 0)
            {
                int limiteRegistros = 1000;
                List<int> codigosTiposTrecho = tiposTrecho.Select(tipoTrecho => tipoTrecho.Codigo).ToList();
                List<(int CodigoTipoTrecho, int CodigoTipoOperacao)> tiposOperacao = new List<(int CodigoTipoTrecho, int CodigoTipoOperacao)>();
                List<(int CodigoTipoTrecho, int CodigoCategoria)> categoriasOrigem = new List<(int CodigoTipoTrecho, int CodigoCategoria)>();
                List<(int CodigoTipoTrecho, int CodigoCategoria)> categoriasDestino = new List<(int CodigoTipoTrecho, int CodigoCategoria)>();
                List<(int CodigoTipoTrecho, int CodigoCategoria)> categoriasExpedidor = new List<(int CodigoTipoTrecho, int CodigoCategoria)>();
                List<(int CodigoTipoTrecho, int CodigoCategoria)> categoriasRecebedor = new List<(int CodigoTipoTrecho, int CodigoCategoria)>();
                List<(int CodigoTipoTrecho, int CodigoModeloVeicularCarga)> modelosVeicularesCarga = new List<(int CodigoTipoTrecho, int CodigoModeloVeicularCarga)>();
                List<(int CodigoTipoTrecho, double CpfCnpjCliente)> clientesOrigem = new List<(int CodigoTipoTrecho, double CpfCnpjCliente)>();
                List<(int CodigoTipoTrecho, double CpfCnpjCliente)> clientesDestino = new List<(int CodigoTipoTrecho, double CpfCnpjCliente)>();

                for (int registroInicial = 0; registroInicial < codigosTiposTrecho.Count; registroInicial += limiteRegistros)
                {
                    List<int> codigosTiposTrechoPaginado = codigosTiposTrecho.Skip(registroInicial).Take(limiteRegistros).ToList();

                    tiposOperacao.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, TOP_CODIGO CodigoTipoOperacao from T_TIPO_TRECHO_SETOR where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoTipoOperacao)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoTipoOperacao)>()
                    );

                    categoriasOrigem.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CTP_CODIGO CodigoCategoria from T_TIPO_TRECHO_CATEGORIA_ORIGEM where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoCategoria)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoCategoria)>()
                    );

                    categoriasDestino.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CTP_CODIGO CodigoCategoria from T_TIPO_TRECHO_CATEGORIA_DESTINO where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoCategoria)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoCategoria)>()
                    );

                    categoriasExpedidor.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CTP_CODIGO CodigoCategoria from T_TIPO_TRECHO_CATEGORIA_EXPEDIDOR where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoCategoria)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoCategoria)>()
                    );

                    categoriasRecebedor.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CTP_CODIGO CodigoCategoria from T_TIPO_TRECHO_CATEGORIA_RECEBEDOR where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoCategoria)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoCategoria)>()
                    );

                    modelosVeicularesCarga.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, MVC_CODIGO CodigoModeloVeicularCarga from T_TIPO_TRECHO_MODELO_VEICULAR where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, int CodigoModeloVeicularCarga)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, int CodigoModeloVeicularCarga)>()
                    );

                    clientesOrigem.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CLI_CGCCPF CpfCnpjCliente from T_TIPO_TRECHO_CLIENTES_ORIGEM where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, double CpfCnpjCliente)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, double CpfCnpjCliente)>()
                    );

                    clientesDestino.AddRange(
                        this.SessionNHiBernate
                            .CreateSQLQuery("select TTR_CODIGO CodigoTipoTrecho, CLI_CGCCPF CpfCnpjCliente from T_TIPO_TRECHO_CLIENTES_DESTINO where TTR_CODIGO in (:Codigos)")
                            .SetParameterList("Codigos", codigosTiposTrechoPaginado)
                            .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTipoTrecho, double CpfCnpjCliente)).GetConstructors().FirstOrDefault()))
                            .List<(int CodigoTipoTrecho, double CpfCnpjCliente)>()
                    );
                }

                foreach (Dominio.ObjetosDeValor.MemoryCache.TipoTrecho tipoTrecho in tiposTrecho)
                {
                    tipoTrecho.TiposOperacao = tiposOperacao.Where(tipoOperacao => tipoOperacao.CodigoTipoTrecho == tipoTrecho.Codigo).Select(tipoOperacao => tipoOperacao.CodigoTipoOperacao).ToList();
                    tipoTrecho.CategoriasOrigem = categoriasOrigem.Where(categoria => categoria.CodigoTipoTrecho == tipoTrecho.Codigo).Select(categoria => categoria.CodigoCategoria).ToList();
                    tipoTrecho.CategoriasDestino = categoriasDestino.Where(categoria => categoria.CodigoTipoTrecho == tipoTrecho.Codigo).Select(categoria => categoria.CodigoCategoria).ToList();
                    tipoTrecho.CategoriasExpedidor = categoriasExpedidor.Where(categoria => categoria.CodigoTipoTrecho == tipoTrecho.Codigo).Select(categoria => categoria.CodigoCategoria).ToList();
                    tipoTrecho.CategoriasRecebedor = categoriasRecebedor.Where(categoria => categoria.CodigoTipoTrecho == tipoTrecho.Codigo).Select(categoria => categoria.CodigoCategoria).ToList();
                    tipoTrecho.ModelosVeiculares = modelosVeicularesCarga.Where(modelo => modelo.CodigoTipoTrecho == tipoTrecho.Codigo).Select(modelo => modelo.CodigoModeloVeicularCarga).ToList();
                    tipoTrecho.ClientesOrigem = clientesOrigem.Where(cliente => cliente.CodigoTipoTrecho == tipoTrecho.Codigo).Select(cliente => cliente.CpfCnpjCliente).ToList();
                    tipoTrecho.ClientesDestino = clientesDestino.Where(cliente => cliente.CodigoTipoTrecho == tipoTrecho.Codigo).Select(cliente => cliente.CpfCnpjCliente).ToList();
                }
            }

            return tiposTrecho;
        }

        #endregion
    }
}
