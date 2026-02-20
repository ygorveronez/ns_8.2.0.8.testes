using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class GuiaNacionalRecolhimentoTributoEtadual : RepositorioBase<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>
    {
        #region Constructores
        public GuiaNacionalRecolhimentoTributoEtadual(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos 
        public Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual BuscarPorCte(int codigoCte)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            query = from obj in query where obj.Cte.Codigo == codigoCte select obj;
            return query.FirstOrDefault();
        }

        public List<(int, int)> PesquisarGuiasNaoValidasPelosCte(List<int> codigoCte)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            consulta = from obj in consulta where codigoCte.Contains(obj.Cte.Codigo) && (!(obj.GuiaValidadaManualmente.HasValue && obj.GuiaValidadaManualmente.Value) || !(obj.ComprovanteValidadoManualmente.HasValue && obj.ComprovanteValidadoManualmente.Value) || !(obj.ValidouTodasInformacoesManualmente.HasValue && obj.ValidouTodasInformacoesManualmente.Value)) select obj;
            return consulta.Select(x => ValueTuple.Create(x.Codigo, x.Cte.Codigo)).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> BuscarPorCarga(int carga, int numeroCte = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            query = from obj in query where obj.Carga.Codigo == carga select obj;

            if (numeroCte > 0)
                query = query.Where(c => c.Cte.Numero == numeroCte);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> BuscarPendentes()
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            query = from obj in query where obj.Situacao == SituacaoGuia.AguardandoEnvio || obj.Situacao == SituacaoGuia.NaoEmitido select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> Consultar(Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento filtroPesquisaGuiaRecolhimento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            query = from obj in query select obj;

            if (filtroPesquisaGuiaRecolhimento.Status != null && filtroPesquisaGuiaRecolhimento.Status.Count > 0)
                query = query.Where(c => filtroPesquisaGuiaRecolhimento.Status.Contains(c.Situacao));

            if (filtroPesquisaGuiaRecolhimento.Codigotransportador > 0)
                query = query.Where(c => c.Cte.Empresa.Codigo == filtroPesquisaGuiaRecolhimento.Codigotransportador);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaGuiaRecolhimento.CPFMotorista))
                query = query.Where(c => c.Cte.Motoristas.Any(a => a.CPFMotorista == filtroPesquisaGuiaRecolhimento.CPFMotorista));

            if (filtroPesquisaGuiaRecolhimento.CodigoVeiculo > 0)
                query = query.Where(c => c.Cte.Veiculos.Any(a => a.Veiculo.Codigo == filtroPesquisaGuiaRecolhimento.CodigoVeiculo));

            if (filtroPesquisaGuiaRecolhimento.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(c => c.DataEmissao.Value.Date >= filtroPesquisaGuiaRecolhimento.DataEmissaoInicial.Date);

            if (filtroPesquisaGuiaRecolhimento.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(c => c.DataEmissao.Value.Date <= filtroPesquisaGuiaRecolhimento.DataEmissaoFinal.Date);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaGuiaRecolhimento.NumeroCarga))
            {
                if (filtroPesquisaGuiaRecolhimento.FiltrarCargasPorParteDoNumero)
                    query = query.Where(c => c.Carga.CodigoCargaEmbarcador.Contains(filtroPesquisaGuiaRecolhimento.NumeroCarga));
                else
                    query = query.Where(c => c.Carga.CodigoCargaEmbarcador.Equals(filtroPesquisaGuiaRecolhimento.NumeroCarga));
            }

            if (filtroPesquisaGuiaRecolhimento.NumeroCte > 0)
                query = query.Where(c => c.Cte.Numero == filtroPesquisaGuiaRecolhimento.NumeroCte);

            if (filtroPesquisaGuiaRecolhimento.SerieCte > 0)
                query = query.Where(c => c.Cte.Serie.Numero == filtroPesquisaGuiaRecolhimento.SerieCte);

            if (filtroPesquisaGuiaRecolhimento.ChaveCte != string.Empty && filtroPesquisaGuiaRecolhimento.ChaveCte.Length == 44)
                query = query.Where(c => c.Cte.Chave == filtroPesquisaGuiaRecolhimento.ChaveCte);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento filtroPesquisaGuiaRecolhimento)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>();
            query = from obj in query select obj;

            if (filtroPesquisaGuiaRecolhimento.Status != null && filtroPesquisaGuiaRecolhimento.Status.Count > 0)
                query = query.Where(c => filtroPesquisaGuiaRecolhimento.Status.Contains(c.Situacao));

            if (filtroPesquisaGuiaRecolhimento.Codigotransportador > 0)
                query = query.Where(c => c.Cte.Empresa.Codigo == filtroPesquisaGuiaRecolhimento.Codigotransportador);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaGuiaRecolhimento.CPFMotorista))
                query = query.Where(c => c.Cte.Motoristas.Any(a => a.CPFMotorista == filtroPesquisaGuiaRecolhimento.CPFMotorista));

            if (filtroPesquisaGuiaRecolhimento.CodigoVeiculo > 0)
                query = query.Where(c => c.Cte.Veiculos.Any(a => a.Veiculo.Codigo == filtroPesquisaGuiaRecolhimento.CodigoVeiculo));

            if (filtroPesquisaGuiaRecolhimento.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(c => c.DataEmissao.Value.Date >= filtroPesquisaGuiaRecolhimento.DataEmissaoInicial.Date);

            if (filtroPesquisaGuiaRecolhimento.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(c => c.DataEmissao.Value.Date <= filtroPesquisaGuiaRecolhimento.DataEmissaoFinal.Date);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaGuiaRecolhimento.NumeroCarga))
                query = query.Where(c => c.Carga.CodigoCargaEmbarcador.Equals(filtroPesquisaGuiaRecolhimento.NumeroCarga));

            if (filtroPesquisaGuiaRecolhimento.NumeroCte > 0)
                query = query.Where(c => c.Cte.Numero == filtroPesquisaGuiaRecolhimento.NumeroCte);

            if (filtroPesquisaGuiaRecolhimento.SerieCte > 0)
                query = query.Where(c => c.Cte.Serie.Numero == filtroPesquisaGuiaRecolhimento.SerieCte);

            if (filtroPesquisaGuiaRecolhimento.ChaveCte != string.Empty && filtroPesquisaGuiaRecolhimento.ChaveCte.Length == 44)
                query = query.Where(c => c.Cte.Chave == filtroPesquisaGuiaRecolhimento.ChaveCte);

            return query.Count();
        }
        #endregion
    }
}
