using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class Quantidades : ServicoBase
    {
        
        public Quantidades(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterQuantidadesCTeParaQuantidades(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade> quantidadesCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            if (quantidadesCTe != null)
            {
                Repositorio.UnidadeDeMedida repUnidadeMeidade = new Repositorio.UnidadeDeMedida(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade quantidadeCte in quantidadesCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    quantidade.Medida = "KG";
                    quantidade.Unidade = quantidadeCte.Unidade;
                    quantidade.Quantidade = quantidadeCte.Quantidade;
                    quantidades.Add(quantidade);
                }
            }
            return quantidades;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterQuantidadesCTeParaQuantidades(List<Dominio.Entidades.InformacaoCargaCTE> quantidadesCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            if (quantidadesCTe != null)
            {
                Repositorio.UnidadeDeMedida repUnidadeMeidade = new Repositorio.UnidadeDeMedida(unitOfWork);
                foreach (Dominio.Entidades.InformacaoCargaCTE quantidadeCte in quantidadesCTe)
                {
                    Dominio.Entidades.UnidadeDeMedida unidadeMedida = repUnidadeMeidade.BuscarPorCodigoUnidade(quantidadeCte.UnidadeMedida);
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    quantidade.Medida = quantidadeCte.UnidadeMedida;
                    quantidade.Unidade = unidadeMedida.UnidadeMedida;
                    quantidade.Quantidade = quantidadeCte.Quantidade;
                    quantidades.Add(quantidade);
                }
            }
            return quantidades;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ConverterDynamicParaQuantidades(dynamic dynQuantidades, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            if (dynQuantidades != null)
            {
                Repositorio.UnidadeDeMedida repUnidadeMeidade = new Repositorio.UnidadeDeMedida(unitOfWork);
                foreach (var dynQuantidade in dynQuantidades)
                {
                    Dominio.Entidades.UnidadeDeMedida unidadeMedida = repUnidadeMeidade.BuscarPorCodigo((int)dynQuantidade.CodigoUnidadeMedida);
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    quantidade.Medida = (string)dynQuantidade.TipoMedida;
                    quantidade.Unidade = unidadeMedida.UnidadeMedida;
                    decimal outQuantidade;
                    decimal.TryParse(dynQuantidade.Quantidade.ToString(), out outQuantidade);
                    quantidade.Quantidade = outQuantidade;

                    quantidades.Add(quantidade);
                }
            }
            return quantidades;
        }

        public void SalvarQuantidadesCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);

            if (cte.Codigo > 0)
                repInformacaoCarga.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade in quantidades)
            {
                if (cte.Peso <= 0m && quantidade.Unidade == Dominio.Enumeradores.UnidadeMedida.KG)
                    cte.Peso = quantidade.Quantidade;

                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = new Dominio.Entidades.InformacaoCargaCTE();

                informacaoCarga.CTE = cte;
                informacaoCarga.Quantidade = quantidade.Quantidade;
                informacaoCarga.Tipo = quantidade.Medida;
                informacaoCarga.UnidadeMedida = string.Format("{0:00}", (int)quantidade.Unidade);

                repInformacaoCarga.Inserir(informacaoCarga);
            }
        }


        public void SalvarQuantidadesPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaPreCTE repInformacaoPreCarga = new Repositorio.InformacaoCargaPreCTE(unitOfWork);

            if (preCte.Codigo > 0)
                repInformacaoPreCarga.DeletarPorPreCTE(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidade in quantidades)
            {
                Dominio.Entidades.InformacaoCargaPreCTE informacaoCarga = new Dominio.Entidades.InformacaoCargaPreCTE();

                informacaoCarga.PreCTE = preCte;
                informacaoCarga.Quantidade = quantidade.Quantidade;
                informacaoCarga.Tipo = quantidade.Medida;
                informacaoCarga.UnidadeMedida = string.Format("{0:00}", (int)quantidade.Unidade);

                repInformacaoPreCarga.Inserir(informacaoCarga);
            }
        }
    }
}
