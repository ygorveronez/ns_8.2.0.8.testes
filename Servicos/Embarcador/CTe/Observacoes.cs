using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class Observacoes : ServicoBase
    {
        
        public Observacoes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> ConverterObservacaoContribuintesCTeParaObservacoes(List<Dominio.Entidades.ObservacaoContribuinteCTE> observacoesContribuinte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();

            if (observacoesContribuinte != null)
            {
                foreach (Dominio.Entidades.ObservacaoContribuinteCTE obsFisco in observacoesContribuinte)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obs = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obs.Campo = obsFisco.Identificador;
                    obs.Texto = obsFisco.Descricao;
                    observacoes.Add(obs);
                }
            }
            return observacoes;

        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> ConverterObservacaoFiscoCTeParaObservacoes(List<Dominio.Entidades.ObservacaoFiscoCTE> observacoesFisco)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();

            if (observacoesFisco != null)
            {
                foreach (Dominio.Entidades.ObservacaoFiscoCTE obsFisco in observacoesFisco)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obs = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obs.Campo = obsFisco.Identificador;
                    obs.Texto = obsFisco.Descricao;
                    observacoes.Add(obs);
                }
            }
            return observacoes;

        }

        public void SetarObservacaoContribuinte(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            //Servicos.Log.TratarErro("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");

            if (cte == null)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesFisco = new List<Dominio.ObjetosDeValor.CTe.Observacao>();
            List<Dominio.ObjetosDeValor.CTe.Observacao> observacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>();

            List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> observacoes = repObservacaoContribuinte.BuscarAtivos();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCargaCTe.BuscarPorCTe(cte.Codigo)?.CargaOrigem;

            if (carga != null)
                observacoes.AddRange(repObservacaoContribuinte.BuscarPorCarga(carga.Codigo));

            if (observacoes.Count > 0)
            {
                observacoesFisco = observacoes.Where(o => o.Tipo == TipoObservacaoCTe.Fisco).Select(o => o.ObterObservacaoCTe()).ToList();
                observacoesContribuinte = observacoes.Where(o => o.Tipo == TipoObservacaoCTe.Contribuinte).Select(o => o.ObterObservacaoCTe()).ToList();
            }
            else
                return;

            if (observacoesFisco.Count <= 0 && observacoesContribuinte.Count <= 0)
                return;

            //Servicos.Log.TratarErro("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");

            if (cte.ObservacoesContribuinte == null)
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();

            if (cte.ObservacoesFisco == null)
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();

            cte.ObservacoesContribuinte.AddRange(observacoesContribuinte.Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao()
            {
                Campo = Utilidades.String.Left(o.Identificador, 20),
                Texto = Utilidades.String.Left(o.Descricao, 160)
            }).ToList());

            cte.ObservacoesFisco.AddRange(observacoesFisco.Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao()
            {
                Campo = Utilidades.String.Left(o.Identificador, 20),
                Texto = Utilidades.String.Left(o.Descricao, 160)
            }).ToList());

            //Servicos.Log.TratarErro("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "SetarObservacaoContribuinte");
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> ConverterDynamicParaObservacoes(dynamic dynObservacoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();

            if (dynObservacoes != null)
            {
                foreach (var dynObs in dynObservacoes)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obs = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obs.Campo = (string)dynObs.Identificador;
                    obs.Texto = (string)dynObs.Descricao;
                    observacoes.Add(obs);
                }
            }


            return observacoes;

        }

        public void SalvarObservacoesFisco(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoesFisco, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoFiscoCTE repObsFisco = new Repositorio.ObservacaoFiscoCTE(unitOfWork);

            if (cte.Codigo > 0)
                repObsFisco.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco in observacoesFisco)
            {
                Dominio.Entidades.ObservacaoFiscoCTE observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                observacao.CTE = cte;
                observacao.Descricao = obsFisco.Texto;
                observacao.Identificador = obsFisco.Campo;
                repObsFisco.Inserir(observacao);
            }
        }

        public void SalvarObservacoesFiscoPreCte(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoesFisco, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoFiscoPreCTE repObsFiscoPreCTe = new Repositorio.ObservacaoFiscoPreCTE(unitOfWork);

            if (preCte.Codigo > 0)
                repObsFiscoPreCTe.DeletarPorPreCTe(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco in observacoesFisco)
            {
                Dominio.Entidades.ObservacaoFiscoPreCTE observacao = new Dominio.Entidades.ObservacaoFiscoPreCTE();
                observacao.preCTE = preCte;
                observacao.Descricao = obsFisco.Texto;
                observacao.Identificador = obsFisco.Campo;
                repObsFiscoPreCTe.Inserir(observacao);
            }
        }

        public void SalvarObservacoesContribuinte(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoesContribuinte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);

            if (cte.Codigo > 0)
                repObsContribuinte.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco in observacoesContribuinte)
            {
                Dominio.Entidades.ObservacaoContribuinteCTE observacao = new Dominio.Entidades.ObservacaoContribuinteCTE();
                observacao.CTE = cte;
                observacao.Descricao = obsFisco.Texto;
                observacao.Identificador = obsFisco.Campo;
                repObsContribuinte.Inserir(observacao);
            }
        }

        public void SalvarObservacoesContribuintePreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao> observacoesContribuinte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuintePreCTE repObsContibuentePreCTe = new Repositorio.ObservacaoContribuintePreCTE(unitOfWork);

            if (preCTe.Codigo > 0)
                repObsContibuentePreCTe.DeletarPorPreCTe(preCTe.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco in observacoesContribuinte)
            {
                Dominio.Entidades.ObservacaoContribuintePreCTE observacao = new Dominio.Entidades.ObservacaoContribuintePreCTE();
                observacao.PreCTE = preCTe;
                observacao.Descricao = obsFisco.Texto;
                observacao.Identificador = obsFisco.Campo;
                repObsContibuentePreCTe.Inserir(observacao);
            }
        }

    }
}
