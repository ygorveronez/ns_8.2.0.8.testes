using Dominio.Interfaces.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.WebService.Carga
{
    public class ModeloVeicularCarga : ServicoBase
    {        
        public ModeloVeicularCarga(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> RetornarModelosVeiculares(List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesDeCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> modelosVeiculares = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>();
            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga in modelosVeicularesDeCarga)
            {
                modelosVeiculares.Add(ConverterObjetoModeloVeicular(modeloVeicularCarga));
            }
            return modelosVeiculares;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular ConverterObjetoModeloVeicular(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            if (modeloVeicularCarga == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular modeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular();
            modeloVeicular.CodigoIntegracao = modeloVeicularCarga.CodigoIntegracao;
            modeloVeicular.Descricao = modeloVeicularCarga.Descricao;
            modeloVeicular.TipoModeloVeicular = modeloVeicularCarga.Tipo;
            modeloVeicular.QuantidadeExtraExcedenteTolerado = modeloVeicularCarga.ToleranciaPesoExtra;

            modeloVeicular.DivisaoCapacidade = ConverterObjetoListaDivisaoCapacidade(modeloVeicularCarga);

            return modeloVeicular;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular> ConverterObjetoListaDivisaoCapacidade(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular> divisoesCapacidade = new List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade in modeloVeicularCarga.DivisoesCapacidade.ToList())
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular divisaoCapacidadeModelo = new Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular();
                divisaoCapacidadeModelo.Capacidade = modeloVeicularCargaDivisaoCapacidade.Quantidade;
                divisaoCapacidadeModelo.Codigo = modeloVeicularCargaDivisaoCapacidade.Codigo;
                divisaoCapacidadeModelo.Descricao = modeloVeicularCargaDivisaoCapacidade.Descricao;
                divisaoCapacidadeModelo.Coluna = modeloVeicularCargaDivisaoCapacidade.Coluna;
                divisaoCapacidadeModelo.Piso = modeloVeicularCargaDivisaoCapacidade.Piso;
                divisaoCapacidadeModelo.UnidadeDeMedida = new Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida()
                {
                    Codigo = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Codigo ?? 0,
                    UnidadeMedida = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN,
                    Descricao = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Descricao ?? ""
                };

                divisoesCapacidade.Add(divisaoCapacidadeModelo);
            }

            return divisoesCapacidade;
        }

        #endregion
    }
}
