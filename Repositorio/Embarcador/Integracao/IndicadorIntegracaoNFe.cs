using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IndicadorIntegracaoNFe : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe>
    {
        #region Construtores

        public IndicadorIntegracaoNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoNFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaIndicadorIntegracaoNFe = consultaIndicadorIntegracaoNFe.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaIndicadorIntegracaoNFe = consultaIndicadorIntegracaoNFe.Where(o => o.DataIntegracao >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaIndicadorIntegracaoNFe = consultaIndicadorIntegracaoNFe.Where(o => o.DataIntegracao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Tipo.HasValue)
                consultaIndicadorIntegracaoNFe = consultaIndicadorIntegracaoNFe.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaIndicadorIntegracaoNFe = consultaIndicadorIntegracaoNFe.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaIndicadorIntegracaoNFe;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoNFe> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIndicadorIntegracaoNFe = Consultar(filtrosPesquisa);

            return ObterLista(consultaIndicadorIntegracaoNFe, parametrosConsulta);
        }

        public dynamic ConsultaGrafico(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoNFe = Consultar(filtrosPesquisa);
            int totalRegistros = consultaIndicadorIntegracaoNFe.Count();

            if (totalRegistros == 0)
                return null;

            int totalRegistrosRejeitado = consultaIndicadorIntegracaoNFe.Where(o => o.Situacao == SituacaoIndicadorIntegracaoNFe.Rejeitada).Count();
            int totalRegistrosSucesso = consultaIndicadorIntegracaoNFe.Where(o => o.Situacao == SituacaoIndicadorIntegracaoNFe.Sucesso).Count();

            List<dynamic> dados = new List<dynamic>();

            dados.Add(new { label = "Rejeitada", value = totalRegistrosRejeitado, color = "#FF0000" });
            dados.Add(new { label = "Sucesso", value = totalRegistrosSucesso, color = "#00b300" });

            return new
            {
                Informacoes = new
                {
                    TotalRegistros = totalRegistros.ToString("n0")
                },
                Dados = dados
            };
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoNFe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoNFe = Consultar(filtrosPesquisa);

            return consultaIndicadorIntegracaoNFe.Count();
        }

        #endregion
    }
}
