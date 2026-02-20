using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Configuracoes
{
    public class Feriado
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Feriado(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<DateTime> ObterDatasComFeriado(DateTime dataInicial, DateTime dataFinal)
        {
            return ObterDatasComFeriado(dataInicial, dataFinal, codigoLocalidade: 0, siglaEstado: "");
        }

        public List<DateTime> ObterDatasComFeriado(DateTime dataInicial, DateTime dataFinal, int codigoLocalidade, string siglaEstado)
        {
            Repositorio.Embarcador.Configuracoes.Feriado repositorioFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.Feriado> feriados = repositorioFeriado.BuscarAtivos(dataInicial, dataFinal, codigoLocalidade, siglaEstado);
            List<DateTime> datasComFeriado = new List<DateTime>();
            List<int> anos = new List<int>();

            for (int ano = dataInicial.Year; ano <= dataFinal.Year; ano++)
                anos.Add(ano);

            foreach (Dominio.Entidades.Embarcador.Configuracoes.Feriado feriado in feriados)
            {
                if (feriado.Ano.HasValue)
                    datasComFeriado.Add(new DateTime(feriado.Ano.Value, feriado.Mes, feriado.Dia));
                else
                {
                    foreach (int ano in anos)
                        datasComFeriado.Add(new DateTime(ano, feriado.Mes, feriado.Dia));
                }
            }

            return datasComFeriado
                .Where(o => o >= dataInicial && o <= dataFinal)
                .OrderBy(o => o)
                .ToList();
        }

        public bool VerificarSePossuiFeriado(DateTime data)
        {
            return VerificarSePossuiFeriado(data, localidade: null);
        }

        public bool VerificarSePossuiFeriado(DateTime data, Dominio.Entidades.Localidade localidade)
        {
            Repositorio.Embarcador.Configuracoes.Feriado repositorioFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(_unitOfWork);

            return repositorioFeriado.VerificarSeExisteFeriado(data, localidade?.Codigo ?? 0, localidade?.Estado?.Sigla ?? string.Empty);
        }


        #endregion Métodos Públicos
    }
}
