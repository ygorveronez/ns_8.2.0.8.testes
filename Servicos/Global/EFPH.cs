using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades.EFPH;

namespace Servicos
{
    public class EFPH
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EFPH(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal)
        {
            this.ArquivoEFPH = new ArquivoEFPH();
            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            this.CTes = repCTe.BuscarTodosPorStatus(empresa.Codigo, dataInicial, dataFinal, new string[] { "A", "C" }, empresa.TipoAmbiente);
        }

        #endregion

        #region Propriedades
        
        private Dominio.Entidades.Empresa Empresa;

        private DateTime DataInicial, DataFinal;

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes;

        private Dominio.Entidades.EFPH.ArquivoEFPH ArquivoEFPH;

        #endregion

        #region Métodos

        public System.IO.MemoryStream Gerar()
        {
            this.ArquivoEFPH.Blocos.Add(this.ObterBloco10());

            this.ArquivoEFPH.Blocos.Add(this.ObterBloco20());

            this.ArquivoEFPH.Blocos.Add(this.ObterBloco90()); //Sempre por ultimo pois é o totalizador dos registros gerados

            return this.ArquivoEFPH.ObterArquivo();
        }

        private Bloco ObterBloco10()
        {
            Bloco bloco10 = new Bloco("10");

            bloco10.Registros.Add(new _10()
            {
                Empresa = this.Empresa,
                Data = this.DataInicial
            });

            return bloco10;
        }

        private Bloco ObterBloco20()
        {
            Bloco bloco20 = new Bloco("20");

            Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(_unitOfWork);
            Repositorio.InformacaoCargaCTE repInfoCarga = new Repositorio.InformacaoCargaCTE(_unitOfWork);

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
            {
                List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(this.Empresa.Codigo, cte.Codigo);

                decimal peso = repInfoCarga.ObterPesoTotal(cte.Codigo);

                _20 registro20 = new _20() { CTe = cte };

                registro20.Registros.Add(new _21() { CTe = cte });
                registro20.Registros.Add(new _22() { CTe = cte });
                registro20.Registros.Add(new _23() { CTe = cte });
                registro20.Registros.Add(new _47() { CTe = cte, Veiculo = veiculos.Count() > 0 ? veiculos[0] : null });
                registro20.Registros.Add(new _49() { CTe = cte, PesoTotal = peso });

                bloco20.Registros.Add(registro20);
            }

            return bloco20;
        }

        private Bloco ObterBloco90()
        {
            Bloco bloco90 = new Bloco("90");

            _90 registro = new _90();

            foreach (Bloco bloco in this.ArquivoEFPH.Blocos)
            {
                registro.TotalRegistros += bloco.ObterTotalDeRegistros();
            }

            registro.TotalRegistros += 1;

            bloco90.Registros.Add(registro);

            return bloco90;
        }

        #endregion
    }
}
